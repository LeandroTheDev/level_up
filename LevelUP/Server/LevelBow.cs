#pragma warning disable CA1822
using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using LevelUP.Client;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.GameContent;

namespace LevelUP.Server;

class LevelBow
{
    public readonly Harmony patch = new("levelup_bow");
    public void Patch()
    {
        if (!Harmony.HasAnyPatches("levelup_bow"))
        {
            patch.PatchCategory("levelup_bow");
        }
    }
    public void Unpatch()
    {
        if (Harmony.HasAnyPatches("levelup_bow"))
        {
            patch.UnpatchCategory("levelup_bow");
        }
    }

    public void Init()
    {
        Instance.api.Event.OnEntityDeath += OnEntityDeath;
        Instance.api.Event.PlayerJoin += ApplyBowStats;
        ExperienceEvents.OnLevelUp += OnBowLevelUp;
        OverwriteDamageInteractionEvents.OnPlayerRangedDoDamageStart += HandleRangedDamage;
        Configuration.RegisterNewLevel("Bow");
        Configuration.RegisterNewLevelTypeEXP("Bow", Configuration.BowGetLevelByEXP);
        Configuration.RegisterNewEXPLevelType("Bow", Configuration.BowGetExpByLevel);

        Debug.Log("Level Bow initialized");
    }

    public static void ApplyBowStats(IPlayer player)
    {
        int level = player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Bow");
        player.Entity.Stats.Set("rangedWeaponsAcc", "levelup_bow", Configuration.BowGetRangedAccuracyBonusByLevel(level));
        player.Entity.Stats.Set("rangedWeaponsSpeed", "levelup_bow", Configuration.BowGetRangedSpeedBonusByLevel(level));
    }

    private void OnBowLevelUp(IPlayer player, string type, ulong exp, int level)
    {
        if (type != "Bow") return;
        ApplyBowStats(player);
    }

    public void InitClient()
    {
        StatusViewEvents.OnStatusRequested -= StatusViewRequested;
        StatusViewEvents.OnStatusRequested += StatusViewRequested;
        OverwriteDamageInteractionEvents.OnPlayerToolViewStats -= RefreshDamage;
        OverwriteDamageInteractionEvents.OnPlayerToolViewStats += RefreshDamage;

        Debug.Log("Level Bow initialized");
    }

    public void Dispose()
    {
        StatusViewEvents.OnStatusRequested -= StatusViewRequested;
        OverwriteDamageInteractionEvents.OnPlayerToolViewStats -= RefreshDamage;

        OverwriteDamageInteractionEvents.OnPlayerRangedDoDamageStart -= HandleRangedDamage;
    }

    private void RefreshDamage(IPlayer player, ItemStack item, ref float damage)
    {
        if (item.Item.Tool == EnumTool.Bow)
        {
            damage *= Configuration.BowGetDamageMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Bow"));
        }
    }

    private void StatusViewRequested(IPlayer player, ref StringBuilder stringBuilder, string levelType)
    {
        if (levelType != "Bow") return;

        int level = player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Bow");

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_rangedaccuracy",
                (int)(Configuration.BowGetRangedAccuracyBonusByLevel(level) * 100f)
            )
        );

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_rangedspeed",
                (int)(Configuration.BowGetRangedSpeedBonusByLevel(level) * 100f)
            )
        );

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_movepenaltyreduction",
                (int)(Configuration.BowGetMovePenaltyReductionByLevel(level) * 100f)
            )
        );

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_arrowchance",
                Configuration.BowGetRawChanceToNotLoseArrowByLevel(level)
            )
        );

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_damage",
                Utils.GetPorcentageFromFloatsStart1(Configuration.BowGetDamageMultiplyByLevel(level))
            )
        );
    }

    private void HandleRangedDamage(IPlayer player, DamageSource damageSource, ref float damage)
    {
        if (damageSource.SourceEntity.GetName().Contains("arrow"))
        {
            damage *= Configuration.BowGetDamageMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Bow"));
            Experience.IncreaseExperience(player, "Bow", "Hit");
        }
    }

    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulateBowConfiguration(coreAPI);
        Configuration.RegisterNewMaxLevelByLevelTypeEXP("Bow", Configuration.bowMaxLevel);
    }

    public void OnEntityDeath(Entity entity, DamageSource damageSource)
    {
        // Error treatment
        if (damageSource == null) return;
        // Checking ranged weapon damage
        if (damageSource.SourceEntity is not EntityProjectile || damageSource.GetCauseEntity() is not EntityPlayer) return;

        // Get entities
        EntityProjectile itemDamage = damageSource.SourceEntity as EntityProjectile;
        // Check if projectile is not from any arrow
        if (!itemDamage.GetName().Contains("arrow")) return;
        EntityPlayer playerEntity = damageSource.GetCauseEntity() as EntityPlayer;

        // Get player instance
        IPlayer player = playerEntity.Player;

        ulong exp = (ulong)Configuration.entityExpBow.GetValueOrDefault(entity.Code.ToString(), 0);
        if (exp <= 0) return;
        Experience.IncreaseExperience(player, "Bow", exp);
    }

    [HarmonyPatchCategory("levelup_bow")]
    private class LevelBowPatch
    {
        // Overwrite Projectile impact
        [HarmonyPrefix]
        [HarmonyPatch(typeof(EntityProjectileBase), "ImpactOnEntity")]
        internal static void ImpactOnEntity(EntityProjectileBase __instance)
        {
            if (!Configuration.enableLevelBow) return;
            if (__instance.World.Side != EnumAppSide.Server) return;

            // Check if is a arrow
            if (__instance.Code.ToString().Contains("arrow"))
            {
                // Check if arrow is shotted by a player
                if (__instance.FiredBy is EntityPlayer)
                {
                    EntityPlayer playerEntity = __instance.FiredBy as EntityPlayer;

                    float increment = Configuration.BowGetChanceToNotLoseArrowByLevel(playerEntity.WatchedAttributes.GetInt("LevelUP_Level_Bow"));

                    // Integration
                    increment = LevelBowEvents.GetExternalBowDropChance(playerEntity.Player, increment);

                    // Increment the drop chance based on level, keeping the arrow's base chance
                    __instance.DropOnImpactChance = Math.Min(1.0f, __instance.DropOnImpactChance + increment);
                }
            }
        }

        // Replace vanilla break chance line with level-adjusted value
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ItemArrow), "GetHeldItemInfo")]
        internal static void GetArrowHeldItemInfo(ItemSlot inSlot, StringBuilder dsc, IWorldAccessor world, bool withDebugInfo)
        {
            if (!Configuration.enableLevelBow) return;
            if (world.Side != EnumAppSide.Client) return;
            if (world.Api is not ICoreClientAPI capi) return;

            IPlayer player = capi.World.Player;
            if (player == null) return;

            int bowLevel = player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Bow");
            if (bowLevel <= 0) return;

            float increment = Configuration.BowGetChanceToNotLoseArrowByLevel(bowLevel);
            if (increment <= 0) return;

            if (inSlot.Itemstack?.Collectible?.Attributes == null) return;
            float baseBreakChance = inSlot.Itemstack.Collectible.Attributes["breakChanceOnImpact"].AsFloat(0.5f);
            float effectiveBreakChance = Math.Max(0f, baseBreakChance - increment);

            string vanillaLine = Lang.Get("breakchanceonimpact", (int)(baseBreakChance * 100f));
            string effectiveLine = Lang.Get("breakchanceonimpact", (int)(effectiveBreakChance * 100f));

            string content = dsc.ToString();
            if (content.Contains(vanillaLine))
            {
                dsc.Clear();
                dsc.Append(content.Replace(vanillaLine, effectiveLine));
            }
        }
    }
}

public class LevelBowEvents
{
    public delegate void PlayerFloatModifierHandler(IPlayer player, ref float number);

    public static event PlayerFloatModifierHandler OnBowDropChanceRefresh;

    internal static float GetExternalBowDropChance(IPlayer player, float chance)
    {
        OnBowDropChanceRefresh?.Invoke(player, ref chance);
        return chance;
    }
}