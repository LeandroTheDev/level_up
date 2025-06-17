using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace LevelUP.Server;

class LevelKnife
{
    public void Init()
    {
        // Instanciate death event
        Instance.api.Event.OnEntityDeath += OnEntityDeath;
        // Instanciate break block event
        Instance.api.Event.BreakBlock += OnBreakBlock;
        Configuration.RegisterNewLevelTypeEXP("Knife", Configuration.KnifeGetLevelByEXP);

        Debug.Log("Level Knife initialized");
    }

#pragma warning disable CA1822
    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulateKnifeConfiguration(coreAPI);
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

        // Check if player is using a bow
        if (player.InventoryManager.ActiveTool != EnumTool.Knife) return;

        // Get the exp received
        ulong exp = (ulong)Configuration.entityExpKnife.GetValueOrDefault(entity.Code.ToString());

        // Get the actual player total exp
        ulong playerExp = Experience.GetExperience(player, "Knife");
        if (Configuration.KnifeIsMaxLevel(playerExp)) return;

        Debug.LogDebug($"{player.PlayerName} killed: {entity.Code}, knife exp earned: {exp}, actual: {playerExp}");

        // Incrementing
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

        // Get the exp received
        ulong exp = (ulong)Configuration.ExpPerBreakingKnife;

        // Get the actual player total exp
        ulong playerExp = Experience.GetExperience(player, "Knife");
        if (Configuration.KnifeIsMaxLevel(playerExp)) return;

        Debug.Log($"{player.PlayerName} breaked: {breakedBlock.Block.Code}, knife exp earned: {exp}, actual: {playerExp}");

        // Incrementing
        Experience.IncreaseExperience(player, "Knife", exp);
    }
}