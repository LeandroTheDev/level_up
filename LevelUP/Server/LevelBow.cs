using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.GameContent;

namespace LevelUP.Server;

class LevelBow
{
#pragma warning disable CA1822
    public void Init()
    {
        // Instanciate death event
        Instance.api.Event.OnEntityDeath += OnEntityDeath;

        Debug.Log("Level Bow initialized");
    }

    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulateBowConfiguration(coreAPI);
    }

    public void OnEntityDeath(Entity entity, DamageSource damageSource)
    {
        // Error treatment
        if (damageSource == null) return;
        // Checking ranged weapon damage
        if (damageSource.SourceEntity is not EntityProjectile || damageSource.GetCauseEntity() is not EntityPlayer) return;

        // Get entities
        EntityProjectile itemDamage = damageSource.SourceEntity as EntityProjectile;
        // Check if projectile is not from any arrow
        if (!itemDamage.GetName().Contains("arrow")) return;
        EntityPlayer playerEntity = damageSource.GetCauseEntity() as EntityPlayer;

        // Get player instance
        IPlayer player = playerEntity.Player;

        // Get the exp received
        ulong exp = (ulong)Configuration.entityExpBow.GetValueOrDefault(entity.Code.ToString(), 0);

        // Get the actual player total exp
        ulong playerExp = Experience.GetExperience(player, "Bow");
        if (Configuration.BowIsMaxLevel(playerExp)) return;

        if (Configuration.enableLevelUpExperienceServerLog)
            Debug.Log($"{player.PlayerName} killed: {entity.Code}, bow exp earned: {exp}, actual: {playerExp}");

        // Incrementing
        Experience.IncreaseExperience(player, "Bow", exp);
    }
}