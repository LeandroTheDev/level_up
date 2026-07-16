#pragma warning disable CA1822
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using LevelUP.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.GameContent;

namespace LevelUP.Server;

class LevelSpear
{
    public readonly Harmony patch = new("levelup_spear");
    public void Patch()
    {
        if (!Harmony.HasAnyPatches("levelup_spear"))
        {
            patch.PatchCategory("levelup_spear");
        }
    }
    public void Unpatch()
    {
        if (Harmony.HasAnyPatches("levelup_spear"))
        {
            patch.UnpatchCategory("levelup_spear");
        }
    }

    public void Init()
    {
        Instance.api.Event.OnEntityDeath += OnEntityDeath;
        Instance.api.Event.PlayerJoin += ApplySpearStats;
        ExperienceEvents.OnLevelUp += OnSpearLevelUp;
        OverwriteDamageInteractionEvents.OnPlayerMeleeDoDamageStart += HandleDamage;
        OverwriteDamageInteractionEvents.OnPlayerRangedDoDamageStart += HandleRangedDamage;
        OverwriteDamageInteractionEvents.OnPlayerToolViewStats += RefreshDamage;
        Configuration.RegisterNewLevel("Spear");
        Configuration.RegisterNewLevelTypeEXP("Spear", Configuration.SpearGetLevelByEXP);
        Configuration.RegisterNewEXPLevelType("Spear", Configuration.SpearGetExpByLevel);

        Debug.Log("Level Spear initialized");
    }

    public static void ApplySpearStats(IPlayer player)
    {
        int level = player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Spear");
        player.Entity.Stats.Set("rangedWeaponsAcc", "levelup_spear", Configuration.SpearGetRangedAccuracyBonusByLevel(level));
        player.Entity.Stats.Set("rangedWeaponsSpeed", "levelup_spear", Configuration.SpearGetRangedSpeedBonusByLevel(level));
    }

    private void OnSpearLevelUp(IPlayer player, string type, ulong exp, int level)
    {
        if (type != "Spear") return;
        ApplySpearStats(player);
    }

    public void InitClient()
    {
        StatusViewEvents.OnStatusRequested += StatusViewRequested;

        Debug.Log("Level Spear initialized");
    }

    public void Dispose()
    {
        OverwriteDamageInteractionEvents.OnPlayerMeleeDoDamageStart -= HandleDamage;
        OverwriteDamageInteractionEvents.OnPlayerRangedDoDamageStart -= HandleRangedDamage;
        OverwriteDamageInteractionEvents.OnPlayerToolViewStats -= RefreshDamage;
        StatusViewEvents.OnStatusRequested -= StatusViewRequested;
    }

    private void RefreshDamage(IPlayer player, ItemStack item, ref float damage)
    {
        if (item.Item.Tool == EnumTool.Spear)
        {
            damage *= Configuration.SpearGetDamageMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Spear"));
        }
    }

    private void StatusViewRequested(IPlayer player, ref StringBuilder stringBuilder, string levelType)
    {
        if (levelType != "Spear") return;

        int level = player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Spear");

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_damage",
                Utils.GetPorcentageFromFloatsStart1(Configuration.SpearGetDamageMultiplyByLevel(level))
            )
        );

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_rangedaccuracy",
                (int)(Configuration.SpearGetRangedAccuracyBonusByLevel(level) * 100f)
            )
        );

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_rangedspeed",
                (int)(Configuration.SpearGetRangedSpeedBonusByLevel(level) * 100f)
            )
        );

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_movepenaltyreduction",
                (int)(Configuration.SpearGetMovePenaltyReductionByLevel(level) * 100f)
            )
        );
    }

    private void HandleRangedDamage(IPlayer player, DamageSource damageSource, ref float damage)
    {
        if (damageSource.SourceEntity.GetName().Contains("spear"))
        {
            damage *= Configuration.SpearGetDamageMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Spear"));
            Experience.IncreaseExperience(player, "Spear", "Hit");
        }
    }

    private void HandleDamage(IPlayer player, DamageSource damageSource, ref float damage)
    {
        if (player.InventoryManager.ActiveTool == EnumTool.Spear)
        {
            damage *= Configuration.SpearGetDamageMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Spear"));
            Experience.IncreaseExperience(player, "Spear", "Hit");
        }
    }

    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulateSpearConfiguration(coreAPI);
        Configuration.RegisterNewMaxLevelByLevelTypeEXP("Spear", Configuration.spearMaxLevel);
    }

    public void OnEntityDeath(Entity entity, DamageSource damageSource)
    {
        // Error treatment
        if (damageSource == null) return;

        EntityProjectile itemDamage;
        EntityPlayer playerEntity;
        IPlayer player;
        // If Spear is throwing
        if (damageSource.SourceEntity is EntityProjectile && damageSource.GetCauseEntity() is EntityPlayer)
        {
            // Get damage entity
            itemDamage = damageSource.SourceEntity as EntityProjectile;
            // Check if damage is from a spear
            if (!itemDamage.GetName().Contains("spear")) return;
            // Get player entity
            playerEntity = damageSource.GetCauseEntity() as EntityPlayer;
            // Get player instance
            player = playerEntity.Player;
        }
        // If Spear is a normal attack
        else
        {
            // Check if the source damage is not from a player
            if (damageSource.SourceEntity is not EntityPlayer) return;
            // Get player entity
            playerEntity = damageSource.SourceEntity as EntityPlayer;
            // Get player instance
            player = playerEntity.Player;
            // Check if player is using a spear
            if (player.InventoryManager.ActiveTool != EnumTool.Spear) return;
        }

        ulong exp = (ulong)Configuration.entityExpSpear.GetValueOrDefault(entity.Code.ToString(), 0);
        Experience.IncreaseExperience(player, "Spear", exp);
    }

    [HarmonyPatchCategory("levelup_spear")]
    private class LevelSpearPatch { }
}

public class LevelSpearEvents
{
    public delegate void PlayerFloatModifierHandler(IPlayer player, ref float number);
}