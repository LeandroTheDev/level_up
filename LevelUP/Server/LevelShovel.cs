using System;
using System.Collections.Generic;
using System.Text.Json;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Server;
using Vintagestory.API.Util;

namespace LevelUP.Server;

class LevelShovel
{
    private Instance instance;

    readonly Dictionary<string, int> entityExp = [];

    public void Init(Instance _instance)
    {
        instance = _instance;
        // Instanciate death event
        instance.api.Event.OnEntityDeath += OnEntityDeath;
        // Instanciate break block event
        instance.api.Event.BreakBlock += OnBreakBlock;

        // Populate configuration
        Configuration.PopulateShovelConfiguration();

        Debug.Log("Level Shovel initialized");
    }

    private Dictionary<string, int> GetSavedLevels()
    {
        byte[] dataBytes = instance.api.WorldManager.SaveGame.GetData("LevelUPData_Shovel");
        string data = dataBytes == null ? "{}" : SerializerUtil.Deserialize<string>(dataBytes);
        return JsonSerializer.Deserialize<Dictionary<string, int>>(data);
    }

    private void SaveLevels(Dictionary<string, int> shovelLevels)
    {
        instance.api.WorldManager.SaveGame.StoreData("LevelUPData_Shovel", JsonSerializer.Serialize(shovelLevels));
    }

    public void OnEntityDeath(Entity entity, DamageSource damageSource)
    {
        // Entity kill is not from a player
        if (damageSource.SourceEntity is not EntityPlayer) return;

        // Get player entity
        EntityPlayer playerEntity = damageSource.SourceEntity as EntityPlayer;

        // Get player instance
        IPlayer player = playerEntity.Api.World.PlayerByUid(playerEntity.PlayerUID);

        // Check if player is using a Shovel
        if(player.InventoryManager.ActiveTool != EnumTool.Pickaxe) return;

        // Get all players levels
        Dictionary<string, int> shovelLevels = GetSavedLevels();

        // Get the exp received
        int exp = entityExp.GetValueOrDefault(playerEntity.GetName(), 0);

        // Get the actual player total exp
        int playerExp = shovelLevels.GetValueOrDefault(playerEntity.GetName(), 0);

        Debug.Log($"{playerEntity.GetName()} killed: {entity.GetName()}, shovel exp earned: {exp}, actual: {playerExp}");

        // Incrementing
        shovelLevels[playerEntity.GetName()] = playerExp + exp;

        // Saving
        SaveLevels(shovelLevels);
        // Updating
        Shared.Instance.UpdateLevelAndNotify(player, "Shovel", playerExp + exp);
    }

    public void OnBreakBlock(IServerPlayer player, BlockSelection breakedBlock, ref float dropQuantityMultiplier, ref EnumHandling handling) {
        EntityPlayer playerEntity = player.Entity;
        // If not a shovel ignore
        if(player.InventoryManager.ActiveTool != EnumTool.Shovel) return;

        // Get all players levels
        Dictionary<string, int> shovelLevels = GetSavedLevels();

        // Get the exp received
        int exp = Configuration.expPerBlockShovel;

        // Get the actual player total exp
        int playerExp = shovelLevels.GetValueOrDefault(playerEntity.GetName(), 0);

        Debug.Log($"{playerEntity.GetName()} breaked: {breakedBlock.Block.BlockId}, shovel exp earned: {exp}, actual: {playerExp}");
        // Incrementing
        shovelLevels[playerEntity.GetName()] = playerExp + exp;

        // Saving
        SaveLevels(shovelLevels);
        // Updating
        Shared.Instance.UpdateLevelAndNotify(player, "Shovel", playerExp + exp);
    }
}