using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;

namespace LevelUP.Server;

class LevelHunter
{
    public void Init()
    {
        // Instanciate death event
        Instance.api.Event.OnEntityDeath += OnEntityDeath;
        Configuration.RegisterNewLevelTypeEXP("Hunter", Configuration.HunterGetLevelByEXP);

        Debug.Log("Level Hunter initialized");
    }

    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulateHunterConfiguration(coreAPI);
    }

    public void OnEntityDeath(Entity entity, DamageSource damageSource)
    {
        // Error treatment
        if (damageSource == null) return;
        // Get player entity
        EntityPlayer playerEntity;
        if (damageSource.SourceEntity is EntityPlayer) playerEntity = damageSource.SourceEntity as EntityPlayer;
        else if (damageSource.GetCauseEntity() is EntityPlayer) playerEntity = damageSource.GetCauseEntity() as EntityPlayer;
        else return;

        // Get player instance
        IPlayer player = playerEntity.Player;

        ulong exp = (ulong)Configuration.entityExpHunter.GetValueOrDefault(entity.Code.ToString());

        // Get the actual player total exp
        ulong playerExp = Experience.GetExperience(player, "Hunter");
        if (Configuration.HunterIsMaxLevel(playerExp)) return;

        Debug.LogDebug($"{player.PlayerName} killed: {entity.Code}, hunter exp earned: {exp}, actual: {playerExp}");

        // Incrementing
        Experience.IncreaseExperience(player, "Hunter", exp);
    }
}