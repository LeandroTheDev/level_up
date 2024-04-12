using System.Collections.Generic;
using System.Text.Json;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Util;

namespace LevelUP.Server;

class LevelKnife
{
    private Instance instance;

    public void Init(Instance _instance)
    {
        instance = _instance;
        // Instanciate death event
        instance.api.Event.OnEntityDeath += OnEntityDeath;

        Debug.Log("Level Knife initialized");
    }

#pragma warning disable CA1822
    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulateKnifeConfiguration(coreAPI);
    }
#pragma warning restore CA1822

    private Dictionary<string, int> GetSavedLevels()
    {
        byte[] dataBytes = instance.api.WorldManager.SaveGame.GetData("LevelUPData_Knife");
        string data = dataBytes == null ? "{}" : SerializerUtil.Deserialize<string>(dataBytes);
        return JsonSerializer.Deserialize<Dictionary<string, int>>(data);
    }

    private void SaveLevels(Dictionary<string, int> knifeLevels)
    {
        instance.api.WorldManager.SaveGame.StoreData("LevelUPData_Knife", JsonSerializer.Serialize(knifeLevels));
    }

    public void OnEntityDeath(Entity entity, DamageSource damageSource)
    {
        // Error treatment
        if (damageSource == null || damageSource.SourceEntity == null) return;
        // The cause of the death is from a projectile
        if (damageSource.GetCauseEntity() is EntityPlayer) return;
        // Entity kill is not from a player
        if (damageSource.SourceEntity is not EntityPlayer) return;

        // Get player entity
        EntityPlayer playerEntity = damageSource.SourceEntity as EntityPlayer;

        // Get player instance
        IPlayer player = instance.api.World.PlayerByUid(playerEntity.PlayerUID);

        // Check if player is using a bow
        if (player.InventoryManager.ActiveTool != EnumTool.Knife) return;

        // Get all players levels
        Dictionary<string, int> knifeLevels = GetSavedLevels();

        // Get the exp received
        int exp = Configuration.entityExpKnife.GetValueOrDefault(entity.GetName(), 0);

        // Get the actual player total exp
        int playerExp = knifeLevels.GetValueOrDefault(playerEntity.GetName(), 0);

        Debug.Log($"{playerEntity.GetName()} killed: {entity.GetName()}, knife exp earned: {exp}, actual: {playerExp}");

        // Incrementing
        knifeLevels[playerEntity.GetName()] = playerExp + exp;

        // Saving
        SaveLevels(knifeLevels);
        // Updating
        Shared.Instance.UpdateLevelAndNotify(instance.api, player, "Knife", playerExp + exp);
    }
}