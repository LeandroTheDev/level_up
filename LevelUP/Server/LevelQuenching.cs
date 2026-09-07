#pragma warning disable CA1822
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using HarmonyLib;
using LevelUP.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace LevelUP.Server;

class LevelQuenching
{
    public readonly Harmony patch = new("levelup_quenching");
    public void Patch()
    {
        if (!Harmony.HasAnyPatches("levelup_quenching"))
        {
            patch.PatchCategory("levelup_quenching");
        }
    }
    public void Unpatch()
    {
        if (Harmony.HasAnyPatches("levelup_quenching"))
        {
            patch.UnpatchCategory("levelup_quenching");
        }
    }

    public void Init()
    {
        Configuration.RegisterNewLevel("Quenching");
        Configuration.RegisterNewLevelTypeEXP("Quenching", Configuration.QuenchingGetLevelByEXP);
        Configuration.RegisterNewEXPLevelType("Quenching", Configuration.QuenchingGetExpByLevel);

        Debug.Log("Level Quenching initialized");
    }

    public void InitClient()
    {
        StatusViewEvents.OnStatusRequested -= StatusViewRequested;
        StatusViewEvents.OnStatusRequested += StatusViewRequested;

        Debug.Log("Level Quenching initialized");
    }

    public void Dispose()
    {
        StatusViewEvents.OnStatusRequested -= StatusViewRequested;
    }

    // The only 3 metals that carry the Quenchable behavior in the base game
    // (see worldproperties/block/metal.json and the vanilla handbook page craftinginfo-quenching)
    public static readonly Dictionary<string, string> MetalSubLevelPatterns = new()
    {
        { "iron", "Iron" },
        { "meteoriciron", "MeteoricIron" },
        { "steel", "Steel" },
    };

    private void StatusViewRequested(IPlayer player, ref StringBuilder stringBuilder, string levelType)
    {
        if (levelType != "Quenching") return;

        int mainLevel = player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Quenching");

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_quenching_shatterchance",
                Utils.GetPorcentageFromFloatsStart1(Configuration.QuenchingGetShatterChanceAddedMultiplyByLevel(mainLevel))
            )
        );

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_quenching_powergain",
                Utils.GetPorcentageFromFloatsStart1(Configuration.QuenchingGetPowerGainMultiplyByLevel(mainLevel))
            )
        );

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_quenching_temperefficiency",
                Utils.GetPorcentageFromFloatsStart1(Configuration.QuenchingGetTemperEfficiencyMultiplyByLevel(mainLevel))
            )
        );

        stringBuilder.AppendLine("");

        stringBuilder.AppendLine(Lang.Get("levelup:status_proficiency"));

        foreach (var pair in MetalSubLevelPatterns)
        {
            stringBuilder.AppendLine($"{Lang.Get($"levelup:{pair.Value.ToLower()}")}: {player.Entity.WatchedAttributes.GetInt($"LevelUP_Level_Quenching_Sub_{pair.Value}")}");
        }
    }

    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        Configuration.PopulateQuenchingConfiguration(coreAPI);
        if (Configuration.enableLevelQuenching)
        {
            Configuration.RegisterNewMaxLevelByLevelTypeEXP("Quenching", Configuration.quenchingMaxLevel);
        }
    }

    // Saved by CoolToTemperature prefix so applyTemperedStats can fall back to NearestPlayer
    // when the item is in a block entity (barrel/ground storage) not owned by any player inventory.
    [ThreadStatic]
    private static double _coolPosX, _coolPosY, _coolPosZ;

    private static readonly FieldInfo MetalGroupCodeField = AccessTools.Field(typeof(CollectibleBehaviorQuenchable), "metalGroupCode");
    private static readonly Dictionary<string, string> MetalCodeCache = [];

    /// <summary>
    /// Resolves the metal code (e.g. "iron", "steel", "meteoriciron") a quenchable itemstack is made of,
    /// reading the same variant group the vanilla CollectibleBehaviorQuenchable behavior was configured with.
    /// </summary>
    private static string GetMetalCode(ItemStack itemstack)
    {
        string collectibleCode = itemstack.Collectible.Code.ToString();
        if (MetalCodeCache.TryGetValue(collectibleCode, out string cached)) return cached;

        string metalCode = null;
        CollectibleBehaviorQuenchable behavior = itemstack.Collectible.GetBehavior<CollectibleBehaviorQuenchable>();
        if (behavior != null && MetalGroupCodeField != null)
        {
            if (MetalGroupCodeField.GetValue(behavior) is string groupCode)
            {
                metalCode = itemstack.Collectible.Variant[groupCode];
            }
        }

        MetalCodeCache[collectibleCode] = metalCode;
        return metalCode;
    }

    /// <summary>
    /// The vanilla quench logic bakes powervalue/durationbonus into "hardened" CollectibleBehaviorBuffable
    /// buffs (attackpower/miningspeed/maxdurability) the moment it applies them, before this mod's postfix
    /// gets to rescale the delta. Without this, only the tooltip numbers would reflect the level bonus while
    /// the actual stat effect would stay at the vanilla, unscaled value.
    /// </summary>
    private static void RescaleHardenedBuffs(ItemStack itemstack, float? newPowerValue, float? newDurationBonus)
    {
        CollectibleBehaviorBuffable buffable = itemstack.Collectible.GetBehavior<CollectibleBehaviorBuffable>();
        if (buffable == null) return;

        List<AppliedCollectibleBuff> buffs = buffable.GetItemBuffs(itemstack);
        bool changed = false;
        foreach (AppliedCollectibleBuff buff in buffs)
        {
            if (buff.Code != "hardened") continue;
            if (newPowerValue.HasValue && (buff.StatCode == "attackpower" || buff.StatCode == "miningspeed"))
            {
                buff.Multiplier = 1f + newPowerValue.Value;
                changed = true;
            }
            else if (newDurationBonus.HasValue && buff.StatCode == "maxdurability")
            {
                buff.Multiplier = 1f + newDurationBonus.Value;
                changed = true;
            }
        }
        if (changed) buffable.StoreItemBuffs(itemstack, buffs);
    }

    // Finds the player who owns this ItemStack.
    // Tempering often settles via passive cooling (AfterGetTemperature) with no position context,
    // so we search player inventories first, then fall back to NearestPlayer via a dropped EntityItem.
    private static IPlayer FindPlayerWithItem(IWorldAccessor world, ItemStack itemstack)
    {
        // Check all player inventories (hotbar, backpack, etc.)
        foreach (IPlayer player in world.AllOnlinePlayers)
        {
            if (player.InventoryManager == null) continue;
            foreach (IInventory inv in player.InventoryManager.Inventories.Values)
            {
                for (int i = 0; i < inv.Count; i++)
                    if (inv[i]?.Itemstack == itemstack) return player;
            }
        }

        // Fallback: item is dropped — find it as a world EntityItem
        if (world is IServerWorldAccessor serverWorld)
        {
            foreach (Entity entity in serverWorld.LoadedEntities.Values)
            {
                if (entity is not EntityItem ei) continue;
                if (ei.Itemstack != itemstack) continue;
                return world.NearestPlayer(ei.Pos.X, ei.Pos.Y, ei.Pos.Z);
            }
        }

        // Last resort: item is in a block entity (barrel/ground storage) —
        // use the position saved by the CoolToTemperature prefix of this same call chain.
        if (_coolPosX != 0 || _coolPosY != 0 || _coolPosZ != 0)
            return world.NearestPlayer(_coolPosX, _coolPosY, _coolPosZ);

        return null;
    }

    [HarmonyPatchCategory("levelup_quenching")]
    private class LevelQuenchingPatch
    {
        // Snapshot taken before CoolToTemperature runs (used for quench XP + scaling).
        internal readonly struct CoolState(int quenchIteration, float shatterChance, float powerValue, float durationBonus)
        {
            public readonly int QuenchIteration = quenchIteration;
            public readonly float ShatterChance = shatterChance;
            public readonly float PowerValue = powerValue;
            public readonly float DurationBonus = durationBonus;
        }

        // Snapshot taken before applyTemperedStats runs (used for temper XP + scaling).
        internal readonly struct TemperState(float shatterChance, float powerValue)
        {
            public readonly float ShatterChance = shatterChance;
            public readonly float PowerValue = powerValue;
        }

        // ── CoolToTemperature — handles QUENCH XP + quench scaling ──────────────

        [HarmonyPrefix]
        [HarmonyPatch(typeof(CollectibleBehaviorQuenchable), "CoolToTemperature")]
        internal static void CoolToTemperatureStart(ItemSlot slot, Vec3d pos, out CoolState? __state)
        {
            __state = null;

            // Save position so applyTemperedStats can fall back to NearestPlayer for block entities
            if (pos != null) { _coolPosX = pos.X; _coolPosY = pos.Y; _coolPosZ = pos.Z; }

            if (!Configuration.enableLevelQuenching) return;
            if (slot?.Itemstack == null) return;

            ITreeAttribute attr = slot.Itemstack.Attributes;
            __state = new CoolState(
                attr.GetInt("quenchIteration", 0),
                attr.GetFloat("shatterchance", 0f),
                attr.GetFloat("powervalue", 0f),
                attr.GetFloat("durationbonus", 0f)
            );
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(CollectibleBehaviorQuenchable), "CoolToTemperature")]
        internal static void CoolToTemperatureFinish(IWorldAccessor world, ItemSlot slot, Vec3d pos, CoolState? __state)
        {
            // Clear the saved position so it cannot leak into applyTemperedStats calls
            // triggered by a different code path (e.g. AfterGetTemperature on another item).
            _coolPosX = 0; _coolPosY = 0; _coolPosZ = 0;

            if (!Configuration.enableLevelQuenching) return;
            if (__state == null) return;
            if (world.Side != EnumAppSide.Server) return;
            if (slot?.Itemstack == null) return;

            CoolState before = __state.Value;
            ITreeAttribute attr = slot.Itemstack.Attributes;

            int quenchIteration = attr.GetInt("quenchIteration", 0);
            if (quenchIteration == before.QuenchIteration) return;

            IPlayer player = world.NearestPlayer(pos.X, pos.Y, pos.Z);
            if (player == null) { Debug.LogDebug("[Quenching] Quench settled but no player found near pos"); return; }

            string metalCode = GetMetalCode(slot.Itemstack);
            string subLevelType = metalCode != null && MetalSubLevelPatterns.TryGetValue(metalCode, out string sub) ? sub : null;

            int mainLevel = Configuration.QuenchingGetLevelByEXP(Experience.GetExperience(player, "Quenching"));
            int subLevel = subLevelType != null ? Configuration.QuenchingGetLevelByEXP(Experience.GetSubExperience(player, "Quenching", subLevelType)) : 0;

            Debug.LogDebug($"[Quenching] Quench XP granted to {player.PlayerName} item={slot.Itemstack.Collectible.Code} metal={metalCode ?? "unknown"}");
            Experience.IncreaseExperience(player, "Quenching", "Quench");
            if (subLevelType != null)
                Experience.IncreaseSubExperience(player, "Quenching", subLevelType,
                    (ulong)Math.Round(Configuration.quenchingBaseExpPerQuench * Configuration.quenchingSubLevelEXPMultiply));

            float shatterMultiply = Configuration.QuenchingGetShatterChanceAddedMultiplyByLevel(mainLevel);
            float powerMultiply = Configuration.QuenchingGetPowerGainMultiplyByLevel(mainLevel);
            if (subLevelType != null)
            {
                shatterMultiply *= Configuration.QuenchingGetShatterChanceAddedMultiplyByLevel(subLevel);
                powerMultiply *= Configuration.QuenchingGetPowerGainMultiplyByLevel(subLevel);
            }

            float shatterDelta = attr.GetFloat("shatterchance", 0f) - before.ShatterChance;
            if (shatterDelta != 0f)
                attr.SetFloat("shatterchance", Math.Max(0f, before.ShatterChance + shatterDelta * shatterMultiply));

            float? newPowerValue = null;
            float powerDelta = attr.GetFloat("powervalue", 0f) - before.PowerValue;
            if (powerDelta != 0f)
            {
                newPowerValue = before.PowerValue + powerDelta * powerMultiply;
                attr.SetFloat("powervalue", newPowerValue.Value);
            }

            float? newDurationBonus = null;
            float durationDelta = attr.GetFloat("durationbonus", 0f) - before.DurationBonus;
            if (durationDelta != 0f)
            {
                newDurationBonus = before.DurationBonus + durationDelta * powerMultiply;
                attr.SetFloat("durationbonus", newDurationBonus.Value);
            }

            if (newPowerValue.HasValue || newDurationBonus.HasValue)
                RescaleHardenedBuffs(slot.Itemstack, newPowerValue, newDurationBonus);
        }

        // ── applyTemperedStats — handles TEMPER XP + temper scaling ─────────────
        // Tempering settles via passive cooling (AfterGetTemperature), not through
        // CoolToTemperature, so this patch is the only reliable interception point.

        [HarmonyPrefix]
        [HarmonyPatch(typeof(CollectibleBehaviorQuenchable), "applyTemperedStats")]
        internal static void ApplyTemperedStatsStart(ItemStack itemstack, out TemperState? __state)
        {
            __state = null;
            if (!Configuration.enableLevelQuenching) return;
            if (itemstack?.Attributes == null) return;
            __state = new TemperState(
                itemstack.Attributes.GetFloat("shatterchance", 0f),
                itemstack.Attributes.GetFloat("powervalue", 0f)
            );
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(CollectibleBehaviorQuenchable), "applyTemperedStats")]
        internal static void ApplyTemperedStatsFinish(IWorldAccessor world, ItemStack itemstack, TemperState? __state)
        {
            if (!Configuration.enableLevelQuenching) return;
            if (__state == null) return;
            if (world.Side != EnumAppSide.Server) return;
            if (itemstack == null) return;

            IPlayer player = FindPlayerWithItem(world, itemstack);
            if (player == null) { Debug.LogDebug("[Quenching] Temper settled but no player found for item"); return; }

            string metalCode = GetMetalCode(itemstack);
            string subLevelType = metalCode != null && MetalSubLevelPatterns.TryGetValue(metalCode, out string sub) ? sub : null;

            int mainLevel = Configuration.QuenchingGetLevelByEXP(Experience.GetExperience(player, "Quenching"));
            int subLevel = subLevelType != null ? Configuration.QuenchingGetLevelByEXP(Experience.GetSubExperience(player, "Quenching", subLevelType)) : 0;

            Debug.LogDebug($"[Quenching] Temper XP granted to {player.PlayerName} item={itemstack.Collectible.Code} metal={metalCode ?? "unknown"}");
            Experience.IncreaseExperience(player, "Quenching", "Temper");
            if (subLevelType != null)
                Experience.IncreaseSubExperience(player, "Quenching", subLevelType,
                    (ulong)Math.Round(Configuration.quenchingBaseExpPerTemper * Configuration.quenchingSubLevelEXPMultiply));

            float temperMultiply = Configuration.QuenchingGetTemperEfficiencyMultiplyByLevel(mainLevel);
            if (subLevelType != null)
                temperMultiply *= Configuration.QuenchingGetTemperEfficiencyMultiplyByLevel(subLevel);

            ITreeAttribute attr = itemstack.Attributes;
            float shatterDelta = attr.GetFloat("shatterchance", 0f) - __state.Value.ShatterChance;
            if (shatterDelta != 0f)
                attr.SetFloat("shatterchance", Math.Max(0f, __state.Value.ShatterChance + shatterDelta * temperMultiply));
        }
    }
}
