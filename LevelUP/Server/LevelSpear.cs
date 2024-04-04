using System.Collections.Generic;
using System.Text.Json;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Util;
using Vintagestory.GameContent;

namespace LevelUP.Server;

class LevelSpear
{
    private Instance instance;

    readonly Dictionary<string, int> entityExp = [];

    public void Init(Instance _instance)
    {
        instance = _instance;
        // Instanciate death event
        instance.api.Event.OnEntityDeath += OnEntityDeath;

        // Populate configuration
        Configuration.PopulateSpearConfiguration();

        Debug.Log("Level Spear initialized");
    }

    private Dictionary<string, int> GetSavedLevels()
    {
        byte[] dataBytes = instance.api.WorldManager.SaveGame.GetData("LevelUPData_Spear");
        string data = dataBytes == null ? "{}" : SerializerUtil.Deserialize<string>(dataBytes);
        return JsonSerializer.Deserialize<Dictionary<string, int>>(data);
    }

    private void SaveLevels(Dictionary<string, int> spearLevels)
    {
        instance.api.WorldManager.SaveGame.StoreData("LevelUPData_Spear", JsonSerializer.Serialize(spearLevels));
    }

    public void OnEntityDeath(Entity entity, DamageSource damageSource)
    {
        Debug.Log($"SOURCE ENTITY: {damageSource.SourceEntity?.GetType()}");
        Debug.Log($"SOURCE BLOCK: {damageSource.SourceBlock?.GetType()}");
        Debug.Log($"SOURCE: {damageSource.Source}");
        Debug.Log($"CAUSE: {damageSource.GetCauseEntity()?.GetType()}");

        EntityProjectile itemDamage;
        EntityPlayer playerEntity;
        IPlayer player;
        // If Spear is throwing
        if (damageSource.SourceEntity is EntityProjectile && damageSource.GetCauseEntity() is EntityPlayer)
        {
            // Get damage entity
            itemDamage = damageSource.SourceEntity as EntityProjectile;
            // Check if damage is from a spear
            if (!itemDamage.GetName().Contains("Spear")) return;
            // Get player entity
            playerEntity = damageSource.GetCauseEntity() as EntityPlayer;
            // Get player instance
            player = playerEntity.Api.World.PlayerByUid(playerEntity.PlayerUID);
        }
        // If Spear is a normal attack
        else
        {
            // Check if the source damage is not from a player
            if (damageSource.SourceEntity is not EntityPlayer) return;
            // Get player entity
            playerEntity = damageSource.SourceEntity as EntityPlayer;
            // Get player instance
            player = playerEntity.Api.World.PlayerByUid(playerEntity.PlayerUID);
            // Check if player is using a spear
            if (player.InventoryManager.ActiveTool != EnumTool.Spear) return;
        }

        // Get all players levels
        Dictionary<string, int> spearLevels = GetSavedLevels();

        // Get the exp received
        int exp = entityExp.GetValueOrDefault(entity.GetName(), 0);

        // Get the actual player total exp
        int playerExp = spearLevels.GetValueOrDefault(playerEntity.GetName(), 0);

        Debug.Log($"{playerEntity.GetName()} killed: {entity.GetName()}, spear exp earned: {exp}, actual: {playerExp}");

        // Incrementing
        spearLevels[playerEntity.GetName()] = playerExp + exp;

        // Saving
        SaveLevels(spearLevels);
        // Updating
        Shared.Instance.UpdateLevelAndNotify(player, "Spear", playerExp + exp);
    }
}