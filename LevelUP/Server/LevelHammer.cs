using System;
using System.Collections.Generic;
using System.Text.Json;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Server;
using Vintagestory.API.Util;
using Vintagestory.GameContent;

namespace LevelUP.Server;

class LevelHammer
{
    public Instance instance;

    public void Init(Instance _instance)
    {
        instance = _instance;
        instance.api.Event.OnEntityDeath += OnEntityDeath;
        Debug.Log("Level Hammer initialized");
    }

#pragma warning disable CA1822
    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulateHammerConfiguration(coreAPI);
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
        if (Configuration.HammerIsMaxLevel(playerExp)) return;

        if (Configuration.enableLevelUpExperienceServerLog)
            Debug.Log($"{player.PlayerName} killed: {entity.Code}, hammer exp earned: {exp}, actual: {playerExp}");

        // Incrementing
        Experience.IncreaseExperience(player, "Hammer", exp);
    }
}