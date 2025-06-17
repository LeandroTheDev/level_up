using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.GameContent;

namespace LevelUP.Server;

class LevelSpear
{
    public void Init()
    {        
        // Instanciate death event
        Instance.api.Event.OnEntityDeath += OnEntityDeath;
        Configuration.RegisterNewLevelTypeEXP("Spear", Configuration.SpearGetLevelByEXP);

        Debug.Log("Level Spear initialized");
    }

#pragma warning disable CA1822
    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulateSpearConfiguration(coreAPI);
    }
#pragma warning restore CA1822

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

        // Get the actual player total exp
        ulong playerExp = Experience.GetExperience(player, "Spear");
        if (Configuration.SpearIsMaxLevel(playerExp)) return;

        Debug.LogDebug($"{player.PlayerName} killed: {entity.Code}, spear exp earned: {exp}, actual: {playerExp}");

        // Incrementing
        Experience.IncreaseExperience(player, "Spear", exp);
    }
}