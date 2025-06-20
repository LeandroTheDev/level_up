using System;
using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace LevelUP.Server;

class LevelShovel
{
    public void Init()
    {
        // Instanciate death event
        Instance.api.Event.OnEntityDeath += OnEntityDeath;
        // Instanciate break block event
        Instance.api.Event.BreakBlock += OnBreakBlock;
        Configuration.RegisterNewLevelTypeEXP("Shovel", Configuration.ShovelGetLevelByEXP);

        Debug.Log("Level Shovel initialized");
    }

#pragma warning disable CA1822
    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulateShovelConfiguration(coreAPI);
        Configuration.RegisterNewMaxLevelByLevelTypeEXP("Shovel", Configuration.shovelMaxLevel);
    }
#pragma warning restore CA1822

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

        // Get the exp received
        ulong exp = (ulong)Configuration.entityExpShovel.GetValueOrDefault(entity.Code.ToString(), 0);

        // Get the actual player total exp
        ulong playerExp = Experience.GetExperience(player, "Shovel");

        Debug.LogDebug($"{player.PlayerName} killed: {entity.Code}, shovel exp earned: {exp}, actual: {playerExp}");

        // Incrementing
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
}