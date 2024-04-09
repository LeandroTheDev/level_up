using System.Collections.Generic;
using System.Text.Json;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Util;

namespace LevelUP.Server;

class LevelHunter
{
    private Instance instance;

    readonly Dictionary<string, int> entityExp = [];

    public void Init(Instance _instance)
    {
        instance = _instance;
        // Instanciate death event
        instance.api.Event.OnEntityDeath += OnEntityDeath;

        // Populate configuration
        Configuration.PopulateHunterConfiguration();

        Debug.Log("Level Hunter initialized");
    }

    private Dictionary<string, int> GetSavedLevels()
    {
        byte[] dataBytes = instance.api.WorldManager.SaveGame.GetData("LevelUPData_Hunter");
        string data = dataBytes == null ? "{}" : SerializerUtil.Deserialize<string>(dataBytes);
        return JsonSerializer.Deserialize<Dictionary<string, int>>(data);
    }

    private void SaveLevels(Dictionary<string, int> hunterLevels)
    {
        instance.api.WorldManager.SaveGame.StoreData("LevelUPData_Hunter", JsonSerializer.Serialize(hunterLevels));
    }

    public void OnEntityDeath(Entity entity, DamageSource damageSource)
    {
        // Error treatment
        if (damageSource == null || damageSource.SourceEntity == null) return;
        // Entity kill is not from a player
        if (damageSource.SourceEntity is not EntityPlayer) return;

        // Get player entity
        EntityPlayer playerEntity = damageSource.SourceEntity as EntityPlayer;

        // Get player instance
        IPlayer player = instance.api.World.PlayerByUid(playerEntity.PlayerUID);

        // Get all players levels
        Dictionary<string, int> hunterLevels = GetSavedLevels();

        // Get the exp received
        int exp = entityExp.GetValueOrDefault(entity.GetName(), 0);

        // Get the actual player total exp
        int playerExp = hunterLevels.GetValueOrDefault(playerEntity.GetName(), 0);

        Debug.Log($"{playerEntity.GetName()} killed: {entity.GetName()}, hunter exp earned: {exp}, actual: {playerExp}");

        // Incrementing
        hunterLevels[playerEntity.GetName()] = playerExp + exp;

        // Saving
        SaveLevels(hunterLevels);
        // Updating
        Shared.Instance.UpdateLevelAndNotify(instance.api, player, "Hunter", playerExp + exp);
    }
}