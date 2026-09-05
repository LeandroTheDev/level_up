#pragma warning disable CA1822
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using HarmonyLib;
using LevelUP.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
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

        int mainLevel = Configuration.QuenchingGetLevelByEXP(Experience.GetExperience(player, "Quenching"));

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

    [HarmonyPatchCategory("levelup_quenching")]
    private class LevelQuenchingPatch
    {
        // Snapshot of the itemstack's quench/temper attributes taken before the vanilla
        // cooling logic runs, so the postfix can diff what vanilla just changed.
        internal readonly struct CoolState(int quenchIteration, int temperIteration, float shatterChance, float powerValue, float durationBonus)
        {
            public readonly int QuenchIteration = quenchIteration;
            public readonly int TemperIteration = temperIteration;
            public readonly float ShatterChance = shatterChance;
            public readonly float PowerValue = powerValue;
            public readonly float DurationBonus = durationBonus;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(CollectibleBehaviorQuenchable), "CoolToTemperature")]
        internal static void CoolToTemperatureStart(ItemSlot slot, out CoolState? __state)
        {
            __state = null;

            if (!Configuration.enableLevelQuenching) return;
            if (slot?.Itemstack == null) return;

            ITreeAttribute attr = slot.Itemstack.Attributes;
            __state = new CoolState(
                attr.GetInt("quenchIteration", 0),
                attr.GetInt("temperIteration", 0),
                attr.GetFloat("shatterchance", 0f),
                attr.GetFloat("powervalue", 0f),
                attr.GetFloat("durationbonus", 0f)
            );
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(CollectibleBehaviorQuenchable), "CoolToTemperature")]
        internal static void CoolToTemperatureFinish(IWorldAccessor world, ItemSlot slot, Vec3d pos, CoolState? __state)
        {
            if (!Configuration.enableLevelQuenching) return;
            if (__state == null) return;
            if (world.Side != EnumAppSide.Server) return;
            // Item shattered during this call, nothing left to grant XP/bonuses for
            if (slot?.Itemstack == null) return;

            IPlayer player = world.NearestPlayer(pos.X, pos.Y, pos.Z);
            if (player == null) return;

            CoolState before = __state.Value;
            ITreeAttribute attr = slot.Itemstack.Attributes;

            int quenchIteration = attr.GetInt("quenchIteration", 0);
            int temperIteration = attr.GetInt("temperIteration", 0);

            // Nothing settled this call (still heating/cooling within the same state)
            if (quenchIteration == before.QuenchIteration && temperIteration == before.TemperIteration) return;

            string metalCode = GetMetalCode(slot.Itemstack);
            string subLevelType = metalCode != null && MetalSubLevelPatterns.TryGetValue(metalCode, out string sub) ? sub : null;

            int mainLevel = Configuration.QuenchingGetLevelByEXP(Experience.GetExperience(player, "Quenching"));
            int subLevel = subLevelType != null ? Configuration.QuenchingGetLevelByEXP(Experience.GetSubExperience(player, "Quenching", subLevelType)) : 0;

            if (quenchIteration > before.QuenchIteration)
            {
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

                // Only rescale the delta vanilla just applied this call, keeping earlier quenches
                // (possibly done at a different level) untouched.
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
            else if (temperIteration > before.TemperIteration)
            {
                Experience.IncreaseExperience(player, "Quenching", "Temper");
                if (subLevelType != null)
                    Experience.IncreaseSubExperience(player, "Quenching", subLevelType,
                        (ulong)Math.Round(Configuration.quenchingBaseExpPerTemper * Configuration.quenchingSubLevelEXPMultiply));

                float temperMultiply = Configuration.QuenchingGetTemperEfficiencyMultiplyByLevel(mainLevel);
                if (subLevelType != null)
                    temperMultiply *= Configuration.QuenchingGetTemperEfficiencyMultiplyByLevel(subLevel);

                // Tempering only ever reduces shatterchance (negative delta); scale the reduction up.
                float shatterDelta = attr.GetFloat("shatterchance", 0f) - before.ShatterChance;
                if (shatterDelta != 0f)
                    attr.SetFloat("shatterchance", Math.Max(0f, before.ShatterChance + shatterDelta * temperMultiply));
            }
        }
    }
}
