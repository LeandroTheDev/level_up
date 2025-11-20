using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace LevelUP.Server;

class LevelAxe
{
    public void Init()
    {
        // Instanciate death event
        Instance.api.Event.OnEntityDeath += OnEntityDeath;
        // Instanciate break block event
        Instance.api.Event.BreakBlock += OnBreakBlock;
        Configuration.RegisterNewLevel("Axe");
        Configuration.RegisterNewLevelTypeEXP("Axe", Configuration.AxeGetLevelByEXP);
        Configuration.RegisterNewEXPLevelType("Axe", Configuration.AxeGetExpByLevel);
        
        Debug.Log("Level Axe initialized");
    }

#pragma warning disable CA1822
    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulateAxeConfiguration(coreAPI);
        Configuration.RegisterNewMaxLevelByLevelTypeEXP("Axe", Configuration.axeMaxLevel);
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

        // Check if player is using a Axe
        if (player.InventoryManager.ActiveTool != EnumTool.Axe) return;

        ulong exp = (ulong)Configuration.entityExpAxe.GetValueOrDefault(entity.Code.ToString(), 0);

        // Get the actual player total exp
        ulong playerExp = Experience.GetExperience(player, "Axe");
        if (Configuration.CheckMaxLevelByLevelTypeEXP("Axe", playerExp)) return;

        Debug.LogDebug($"{player.PlayerName} killed: {entity.Code}, axe exp earned: {exp}, actual: {playerExp}");

        // Incrementing
        Experience.IncreaseExperience(player, "Axe", exp);
    }

    public void OnBreakBlock(IServerPlayer player, BlockSelection breakedBlock, ref float dropQuantityMultiplier, ref EnumHandling handling)
    {
        // If not a axe ignore
        if (player.InventoryManager.ActiveTool != EnumTool.Axe) return;
        if (breakedBlock.Block.BlockMaterial != EnumBlockMaterial.Wood) return;

        ulong exp = (ulong)Configuration.ExpPerBreakingAxe;

        // Get the actual player total exp
        ulong playerExp = Experience.GetExperience(player, "Axe");

        Debug.LogDebug($"{player.PlayerName} breaked: {breakedBlock.Block.Code}, axe exp earned: {exp}, actual: {playerExp}");

        // Incrementing
        Experience.IncreaseExperience(player, "Axe", exp);
    }
}