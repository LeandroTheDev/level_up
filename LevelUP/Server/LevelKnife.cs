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
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace LevelUP.Server;

class LevelKnife
{
    public readonly Harmony patch = new("levelup_knife");
    public void Patch()
    {
        if (!Harmony.HasAnyPatches("levelup_knife"))
        {
            patch.PatchCategory("levelup_knife");
        }
    }
    public void Unpatch()
    {
        if (Harmony.HasAnyPatches("levelup_knife"))
        {
            patch.UnpatchCategory("levelup_knife");
        }
    }

    public void Init()
    {
        Instance.api.Event.OnEntityDeath += OnEntityDeath;
        Instance.api.Event.BreakBlock += OnBreakBlock;
        OverwriteDamageInteractionEvents.OnPlayerMeleeDoDamageStart += HandleDamage;
        Configuration.RegisterNewLevel("Knife");
        Configuration.RegisterNewLevelTypeEXP("Knife", Configuration.KnifeGetLevelByEXP);
        Configuration.RegisterNewEXPLevelType("Knife", Configuration.KnifeGetExpByLevel);

        Debug.Log("Level Knife initialized");
    }

    public void InitClient()
    {
        StatusViewEvents.OnStatusRequested += StatusViewRequested;
        OverwriteBlockBreakEvents.OnMiningSpeedRefreshed += RefreshMiningSpeed;
        OverwriteDamageInteractionEvents.OnPlayerToolViewStats += RefreshDamage;
        Client.Instance.RefreshWatchedAttributes += RefreshWatchedAttributes;

        Debug.Log("Level Knife initialized");
    }

    public void Dispose()
    {
        OverwriteDamageInteractionEvents.OnPlayerMeleeDoDamageStart -= HandleDamage;
        StatusViewEvents.OnStatusRequested -= StatusViewRequested;
        OverwriteBlockBreakEvents.OnMiningSpeedRefreshed -= RefreshMiningSpeed;
        OverwriteDamageInteractionEvents.OnPlayerToolViewStats -= RefreshDamage;
        Client.Instance.RefreshWatchedAttributes -= RefreshWatchedAttributes;
    }

    private void RefreshDamage(IPlayer player, ItemStack item, ref float damage)
    {
        if (item.Item.Tool == EnumTool.Knife)
        {
            damage *= Configuration.KnifeGetDamageMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Knife"));
        }
    }

    static private float currentKnifeMiningSpeed = 1.0f;
    private void RefreshWatchedAttributes()
    {
        var api = Shared.Instance.api as ICoreClientAPI;
        currentKnifeMiningSpeed = api.World.Player.Entity.WatchedAttributes.GetFloat("LevelUP_Knife_MiningSpeed");
    }
    private void RefreshMiningSpeed(CollectibleObject collectible, IItemStack itemstack, BlockSelection blockSel, Block block, IPlayer player, ref float multiply)
    {
        if (block.BlockMaterial == EnumBlockMaterial.Plant ||
            block.BlockMaterial == EnumBlockMaterial.Leaves)
            multiply *= currentKnifeMiningSpeed;
    }

    private void StatusViewRequested(IPlayer player, ref StringBuilder stringBuilder, string levelType)
    {
        if (levelType != "Knife") return;

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_damage",
                Utils.GetPorcentageFromFloatsStart1(Configuration.KnifeGetDamageMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Knife")))
            )
        );
        stringBuilder.AppendLine(
            Lang.Get("levelup:status_harvest",
                Utils.GetPorcentageFromFloatsStart0(Configuration.KnifeGetHarvestMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Knife")))
            )
        );

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_miningspeed",
                Utils.GetPorcentageFromFloatsStart1(Configuration.KnifeGetMiningMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Knife")))
            )
        );
    }

    private void HandleDamage(IPlayer player, DamageSource damageSource, ref float damage)
    {
        if (player.InventoryManager.ActiveTool == EnumTool.Knife)
        {
            damage *= Configuration.KnifeGetDamageMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Knife"));
            Experience.IncreaseExperience(player, "Knife", "Hit");
        }
    }

    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulateKnifeConfiguration(coreAPI);
        Configuration.RegisterNewMaxLevelByLevelTypeEXP("Knife", Configuration.knifeMaxLevel);
    }

    public void OnEntityDeath(Entity entity, DamageSource damageSource)
    {
        // Error treatment
        if (damageSource == null) return;
        // The cause of the death is from a projectile
        if (damageSource.GetCauseEntity() is not EntityPlayer && damageSource.SourceEntity is EntityProjectile) return;
        // Entity kill is not from a player
        if (damageSource.SourceEntity is not EntityPlayer) return;

        // Get player entity
        EntityPlayer playerEntity = damageSource.SourceEntity as EntityPlayer;

        // Get player instance
        IPlayer player = playerEntity.Player;

        // Check if player is using a bow
        if (player.InventoryManager.ActiveTool != EnumTool.Knife) return;

        ulong exp = (ulong)Configuration.entityExpKnife.GetValueOrDefault(entity.Code.ToString());
        Experience.IncreaseExperience(player, "Knife", exp);
    }

    public void OnBreakBlock(IServerPlayer player, BlockSelection breakedBlock, ref float dropQuantityMultiplier, ref EnumHandling handling)
    {
        // If not a knife ignore
        if (player.InventoryManager.ActiveTool != EnumTool.Knife) return;
        // If not a valid block for knife
        switch (breakedBlock.Block.BlockMaterial)
        {
            case EnumBlockMaterial.Plant: break;
            case EnumBlockMaterial.Leaves: break;
            default: return;
        }

        ulong exp = (ulong)Configuration.ExpPerBreakingKnife;
        Experience.IncreaseExperience(player, "Knife", exp);
    }

    [HarmonyPatchCategory("levelup_knife")]
    private class LevelKnifePatch
    {
        // Overwrite Knife Harvesting
        [HarmonyPrefix]
        [HarmonyPatch(typeof(EntityBehaviorHarvestable), "GenerateDrops")]
        internal static void GenerateDropsStart(EntityBehaviorHarvestable __instance, IPlayer byPlayer)
        {
            if (!Configuration.enableLevelKnife) return;
            if (__instance.entity.World.Side != EnumAppSide.Server) return;

            // Get the final droprate
            float dropRate = Configuration.KnifeGetHarvestMultiplyByLevel(byPlayer.Entity.WatchedAttributes.GetInt("LevelUP_Level_Knife"));

            // Integration
            dropRate = LevelKnifeEvents.GetExternalKnifeHarvest(byPlayer, __instance.entity, dropRate);

            // Don't worry, it will be reseted automatically by the game
            byPlayer.Entity.Stats.Set("animalLootDropRate", "animalLootDropRate", dropRate);

            // Earn xp by harvesting the entity
            Experience.IncreaseExperience(byPlayer, "Knife", "Harvest");

            Debug.LogDebug($"{byPlayer.PlayerName} harvested any entity with knife, multiply drop: {dropRate}/{byPlayer.Entity.Stats.GetBlended("animalLootDropRate")}");
        }
    }
}
public class LevelKnifeEvents
{
    public delegate void PlayerEntityFloatModifierHandler(IPlayer player, Entity harvestedEntity, ref float number);

    public static event PlayerEntityFloatModifierHandler OnKnifeHarvested;

    internal static float GetExternalKnifeHarvest(IPlayer player, Entity harvestedEntity, float multiply)
    {
        OnKnifeHarvested?.Invoke(player, harvestedEntity, ref multiply);
        return multiply;
    }
}