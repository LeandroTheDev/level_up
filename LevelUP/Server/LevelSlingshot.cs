using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.GameContent;

namespace LevelUP.Server;

class LevelSlingshot
{
#pragma warning disable CA1822
    public void Init()
    {
        // Instanciate death event
        Instance.api.Event.OnEntityDeath += OnEntityDeath;
        Configuration.RegisterNewLevel("Slingshot");
        Configuration.RegisterNewLevelTypeEXP("Slingshot", Configuration.SlingshotGetLevelByEXP);
        Configuration.RegisterNewEXPLevelType("Slingshot", Configuration.SlingshotGetExpByLevel);

        Debug.Log("Level Slingshot initialized");
    }

    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulateSlingshotConfiguration(coreAPI);
        Configuration.RegisterNewMaxLevelByLevelTypeEXP("Slingshot", Configuration.slingshotMaxLevel);
    }

    public void OnEntityDeath(Entity entity, DamageSource damageSource)
    {
        // Error treatment
        if (damageSource == null) return;
        // Checking ranged weapon damage
        if (damageSource.SourceEntity is not EntityProjectile || damageSource.GetCauseEntity() is not EntityPlayer) return;

        // Get entities
        EntityProjectile itemDamage = damageSource.SourceEntity as EntityProjectile;
        // !!!!!!!!!!!!!
        // !!!!!!!!!!!!!
        // Need to debug i don't know if is rock or stone whatevers
        // !!!!!!!!!!!!!
        // !!!!!!!!!!!!!
        // Check if projectile is not from any rock
        if (!itemDamage.GetName().Contains("rock")) return;
        EntityPlayer playerEntity = damageSource.GetCauseEntity() as EntityPlayer;

        // Get player instance
        IPlayer player = playerEntity.Player;

        // Get the exp received
        ulong exp = (ulong)Configuration.entityExpSlingshot.GetValueOrDefault(entity.Code.ToString(), 0);

        // Get the actual player total exp
        ulong playerExp = Experience.GetExperience(player, "Slingshot");

        Debug.LogDebug($"{player.PlayerName} killed: {entity.Code}, slingshot exp earned: {exp}, actual: {playerExp}");

        // Incrementing
        Experience.IncreaseExperience(player, "Slingshot", exp);
    }
}