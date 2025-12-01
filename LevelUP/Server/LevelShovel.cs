#pragma warning disable CA1822
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

class LevelShovel
{
    public readonly Harmony patch = new("levelup_shovel");
    public void Patch()
    {
        if (!Harmony.HasAnyPatches("levelup_shovel"))
        {
            patch.PatchCategory("levelup_shovel");
        }
    }
    public void Unpatch()
    {
        if (Harmony.HasAnyPatches("levelup_shovel"))
        {
            patch.UnpatchCategory("levelup_shovel");
        }
    }

    public void Init()
    {
        // Instanciate death event
        Instance.api.Event.OnEntityDeath += OnEntityDeath;
        // Instanciate break block event
        Instance.api.Event.BreakBlock += OnBreakBlock;
        OverwriteDamageInteractionEvents.OnPlayerMeleeDoDamageStart += HandleDamage;
        Configuration.RegisterNewLevel("Shovel");
        Configuration.RegisterNewLevelTypeEXP("Shovel", Configuration.ShovelGetLevelByEXP);
        Configuration.RegisterNewEXPLevelType("Shovel", Configuration.ShovelGetExpByLevel);

        Debug.Log("Level Shovel initialized");
    }

    public void InitClient()
    {
        StatusViewEvents.OnStatusRequested += StatusViewRequested;
        OverwriteBlockBreakEvents.OnMiningSpeedRefreshed += RefreshMiningSpeed;
        Client.Instance.RefreshWatchedAttributes += RefreshWatchedAttributes;

        Debug.Log("Level Shovel initialized");
    }

    public void Dispose()
    {
        OverwriteDamageInteractionEvents.OnPlayerMeleeDoDamageStart -= HandleDamage;
        StatusViewEvents.OnStatusRequested -= StatusViewRequested;
        OverwriteBlockBreakEvents.OnMiningSpeedRefreshed -= RefreshMiningSpeed;
        Client.Instance.RefreshWatchedAttributes -= RefreshWatchedAttributes;
    }

    static private float currentShovelMiningSpeed = 1.0f;
    private void RefreshWatchedAttributes()
    {
        var api = Shared.Instance.api as ICoreClientAPI;
        currentShovelMiningSpeed = api.World.Player.Entity.WatchedAttributes.GetFloat("LevelUP_Shovel_MiningSpeed");
    }
    private void RefreshMiningSpeed(CollectibleObject collectible, IItemStack itemstack, BlockSelection blockSel, Block block, IPlayer player, ref float multiply)
    {
        if (block.BlockMaterial == EnumBlockMaterial.Soil ||
            block.BlockMaterial == EnumBlockMaterial.Gravel ||
            block.BlockMaterial == EnumBlockMaterial.Sand ||
            block.BlockMaterial == EnumBlockMaterial.Snow)
            multiply *= currentShovelMiningSpeed;
    }

    private void StatusViewRequested(IPlayer player, ref StringBuilder stringBuilder, string levelType)
    {
        if (levelType != "Shovel") return;

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_damage",
                Utils.GetPorcentageFromFloatsStart1(Configuration.ShovelGetDamageMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Shovel")))
            )
        );

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_miningspeed",
                Utils.GetPorcentageFromFloatsStart1(Configuration.ShovelGetMiningMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Shovel")))
            )
        );
    }

    private void HandleDamage(IPlayer player, DamageSource damageSource, ref float damage)
    {
        if (player.InventoryManager.ActiveTool == EnumTool.Shovel)
        {
            damage *= Configuration.ShovelGetDamageMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Shovel"));
            Experience.IncreaseExperience(player, "Shovel", "Hit");
        }
    }

    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulateShovelConfiguration(coreAPI);
        Configuration.RegisterNewMaxLevelByLevelTypeEXP("Shovel", Configuration.shovelMaxLevel);
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

        // Check if player is using a Shovel
        if (player.InventoryManager.ActiveTool != EnumTool.Pickaxe) return;

        ulong exp = (ulong)Configuration.entityExpShovel.GetValueOrDefault(entity.Code.ToString(), 0);
        Experience.IncreaseExperience(player, "Shovel", exp);
    }

    public void OnBreakBlock(IServerPlayer player, BlockSelection breakedBlock, ref float dropQuantityMultiplier, ref EnumHandling handling)
    {
        // If not a shovel ignore
        if (player.InventoryManager.ActiveTool != EnumTool.Shovel) return;
        // If not a valid block for shovel
        switch (breakedBlock.Block.BlockMaterial)
        {
            case EnumBlockMaterial.Gravel: break;
            case EnumBlockMaterial.Soil: break;
            case EnumBlockMaterial.Snow: break;
            case EnumBlockMaterial.Sand: break;
            default: return;
        }

        // Get the exp received
        ulong exp = (ulong)Configuration.ExpPerBreakingShovel;

        // Get the actual player total exp
        ulong playerExp = Experience.GetExperience(player, "Shovel");

        Debug.LogDebug($"{player.PlayerName} breaked: {breakedBlock.Block.Code}, shovel exp earned: {exp}, actual: {playerExp}");

        // Incrementing
        Experience.IncreaseExperience(player, "Shovel", exp);
    }

    [HarmonyPatchCategory("levelup_shovel")]
    private class LevelShovelPatch
    { }
}