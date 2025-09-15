using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace LevelUP.Server;

class LevelPickaxe
{
    public void Init()
    {
        // Instanciate death event
        Instance.api.Event.OnEntityDeath += OnEntityDeath;
        // Instanciate break block event
        Instance.api.Event.BreakBlock += OnBreakBlock;
        Configuration.RegisterNewLevelTypeEXP("Pickaxe", Configuration.PickaxeGetLevelByEXP);
        Configuration.RegisterNewEXPLevelType("Pickaxe", Configuration.PickaxeGetExpByLevel);

        Debug.Log("Level Pickaxe initialized");
    }

#pragma warning disable CA1822
    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulatePickaxeConfiguration(coreAPI);
        Configuration.RegisterNewMaxLevelByLevelTypeEXP("Pickaxe", Configuration.pickaxeMaxLevel);
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

        // Check if player is using a Pickaxe
        if (player.InventoryManager.ActiveTool != EnumTool.Pickaxe) return;

        // Get the exp received
        ulong exp = (ulong)Configuration.entityExpPickaxe.GetValueOrDefault(entity.Code.ToString());

        // Get the actual player total exp
        ulong playerExp = Experience.GetExperience(player, "Pickaxe");

        Debug.LogDebug($"{player.PlayerName} killed: {entity.Code}, pickaxe exp earned: {exp}, actual: {playerExp}");

        // Incrementing
        Experience.IncreaseExperience(player, "Pickaxe", exp);
    }

    public void OnBreakBlock(IServerPlayer player, BlockSelection breakedBlock, ref float dropQuantityMultiplier, ref EnumHandling handling)
    {
        // If not a shovel ignore
        if (player.InventoryManager.ActiveTool != EnumTool.Pickaxe) return;
        if (breakedBlock.Block.BlockMaterial != EnumBlockMaterial.Stone && breakedBlock.Block.BlockMaterial != EnumBlockMaterial.Ore) return;

        // Get the exp received
        ulong exp = (ulong)Configuration.oresExpPickaxe.GetValueOrDefault(breakedBlock.Block.Code.ToString(), Configuration.ExpPerBreakingPickaxe);

        // Get the actual player total exp
        ulong playerExp = Experience.GetExperience(player, "Pickaxe");

        Debug.LogDebug($"{player.PlayerName} breaked: {breakedBlock.Block.Code}, pickaxe exp earned: {exp}, actual: {playerExp}");
        
        // Incrementing
        Experience.IncreaseExperience(player, "Pickaxe", exp);
    }
}