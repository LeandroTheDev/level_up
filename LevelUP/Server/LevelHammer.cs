using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.GameContent;

namespace LevelUP.Server;

class LevelHammer
{
    public void Init()
    {
        Instance.api.Event.OnEntityDeath += OnEntityDeath;
        Configuration.RegisterNewLevelTypeEXP("Hammer", Configuration.HammerGetLevelByEXP);
        Configuration.RegisterNewEXPLevelType("Hammer", Configuration.HammerGetExpByLevel);

        Debug.Log("Level Hammer initialized");
    }

#pragma warning disable CA1822
    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulateHammerConfiguration(coreAPI);
        Configuration.RegisterNewMaxLevelByLevelTypeEXP("Hammer", Configuration.hammerMaxLevel);
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

        // Check if player is using a Hammer
        if (player.InventoryManager.ActiveTool != EnumTool.Hammer) return;

        ulong exp = (ulong)Configuration.entityExpHammer.GetValueOrDefault(entity.Code.ToString());

        // Get the actual player total exp
        ulong playerExp = Experience.GetExperience(player, "Hammer");

        Debug.LogDebug($"{player.PlayerName} killed: {entity.Code}, hammer exp earned: {exp}, actual: {playerExp}");

        // Incrementing
        Experience.IncreaseExperience(player, "Hammer", exp);
    }
}