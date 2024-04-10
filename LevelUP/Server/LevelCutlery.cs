using System.Collections.Generic;
using System.Text.Json;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Util;

namespace LevelUP.Server;

class LevelCutlery
{
    private Instance instance;

    readonly Dictionary<string, int> entityExp = [];

    public void Init(Instance _instance)
    {
        instance = _instance;
        // Instanciate death event
        instance.api.Event.OnEntityDeath += OnEntityDeath;

        // Populate configuration
        Configuration.PopulateCutleryConfiguration();

        Debug.Log("Level Cutlery initialized");
    }

    private Dictionary<string, int> GetSavedLevels()
    {
        byte[] dataBytes = instance.api.WorldManager.SaveGame.GetData("LevelUPData_Cutlery");
        string data = dataBytes == null ? "{}" : SerializerUtil.Deserialize<string>(dataBytes);
        return JsonSerializer.Deserialize<Dictionary<string, int>>(data);
    }

    private void SaveLevels(Dictionary<string, int> cutleryLevels)
    {
        instance.api.WorldManager.SaveGame.StoreData("LevelUPData_Cutlery", JsonSerializer.Serialize(cutleryLevels));
    }

    public void OnEntityDeath(Entity entity, DamageSource damageSource)
    {
        // Check if entity is alive
        if(!entity.Alive) return;
        // Error treatment
        if (damageSource == null || damageSource.SourceEntity == null) return;
        // The cause of the death is from a projectile
        if(damageSource.GetCauseEntity() is EntityPlayer) return;
        // Entity kill is not from a player
        if (damageSource.SourceEntity is not EntityPlayer) return;

        // Get player entity
        EntityPlayer playerEntity = damageSource.SourceEntity as EntityPlayer;

        // Get player instance
        IPlayer player = instance.api.World.PlayerByUid(playerEntity.PlayerUID);

        // Check if player is using a bow
        if(player.InventoryManager.ActiveTool != EnumTool.Knife) return;

        // Get all players levels
        Dictionary<string, int> cutleryLevels = GetSavedLevels();

        // Get the exp received
        int exp = entityExp.GetValueOrDefault(entity.GetName(), 0);

        // Get the actual player total exp
        int playerExp = cutleryLevels.GetValueOrDefault(playerEntity.GetName(), 0);

        Debug.Log($"{playerEntity.GetName()} killed: {entity.GetName()}, cutlery exp earned: {exp}, actual: {playerExp}");

        // Incrementing
        cutleryLevels[playerEntity.GetName()] = playerExp + exp;

        // Saving
        SaveLevels(cutleryLevels);
        // Updating
        Shared.Instance.UpdateLevelAndNotify(instance.api, player, "Cutlery", playerExp + exp);
    }
}