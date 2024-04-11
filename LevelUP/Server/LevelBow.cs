using System.Collections.Generic;
using System.Text.Json;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Util;
using Vintagestory.GameContent;

namespace LevelUP.Server;

class LevelBow
{
    private Instance instance;

    readonly Dictionary<string, int> entityExp = [];

    public void Init(Instance _instance)
    {
        instance = _instance;
        // Instanciate death event
        instance.api.Event.OnEntityDeath += OnEntityDeath;

        Debug.Log("Level Bow initialized");
    }

    #pragma warning disable CA1822
    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulateBowConfiguration(coreAPI);
    }
    #pragma warning restore CA1822

    private Dictionary<string, int> GetSavedLevels()
    {
        byte[] dataBytes = instance.api.WorldManager.SaveGame.GetData("LevelUPData_Bow");
        string data = dataBytes == null ? "{}" : SerializerUtil.Deserialize<string>(dataBytes);
        return JsonSerializer.Deserialize<Dictionary<string, int>>(data);
    }

    private void SaveLevels(Dictionary<string, int> bowLevels)
    {
        instance.api.WorldManager.SaveGame.StoreData("LevelUPData_Bow", JsonSerializer.Serialize(bowLevels));
    }

    public void OnEntityDeath(Entity entity, DamageSource damageSource)
    {
        // Error treatment
        if (damageSource == null || damageSource.SourceEntity == null) return;
        // Checking ranged weapon damage
        if (damageSource.SourceEntity is EntityProjectile && damageSource.GetCauseEntity() is EntityPlayer)
        {
            // Get entities
            EntityProjectile itemDamage = damageSource.SourceEntity as EntityProjectile;
            // Check if projectile is not from any arrow
            if (!itemDamage.GetName().Contains("arrow")) return;
            EntityPlayer playerEntity = damageSource.GetCauseEntity() as EntityPlayer;

            // Get player instance
            IPlayer player = instance.api.World.PlayerByUid(playerEntity.PlayerUID);

            // Get all players levels
            Dictionary<string, int> bowLevels = GetSavedLevels();

            // Get the exp received
            int exp = Configuration.entityExpBow.GetValueOrDefault(entity.GetName(), 0);

            // Get the actual player total exp
            int playerExp = bowLevels.GetValueOrDefault(playerEntity.GetName(), 0);

            Debug.Log($"{playerEntity.GetName()} killed: {entity.GetName()}, bow exp earned: {exp}, actual: {playerExp}");

            // Incrementing
            bowLevels[playerEntity.GetName()] = playerExp + exp;

            // Saving
            SaveLevels(bowLevels);
            // Updating
            Shared.Instance.UpdateLevelAndNotify(instance.api, player, "Bow", playerExp + exp);
        }
    }
}