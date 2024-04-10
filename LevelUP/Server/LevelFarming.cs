using System.Collections.Generic;
using System.Text.Json;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Server;
using Vintagestory.API.Util;

namespace LevelUP.Server;

class LevelFarming
{
    private Instance instance;

    readonly Dictionary<string, int> entityExp = [];

    public void Init(Instance _instance)
    {
        instance = _instance;
        // Instanciate break block event
        instance.api.Event.BreakBlock += OnBreakBlock;

        // Populate configuration
        Configuration.PopulateFarmingConfiguration();

        Debug.Log("Level Farming initialized");
    }

    private Dictionary<string, int> GetSavedLevels()
    {
        byte[] dataBytes = instance.api.WorldManager.SaveGame.GetData("LevelUPData_Farming");
        string data = dataBytes == null ? "{}" : SerializerUtil.Deserialize<string>(dataBytes);
        return JsonSerializer.Deserialize<Dictionary<string, int>>(data);
    }

    private void SaveLevels(Dictionary<string, int> farmingLevels)
    {
        instance.api.WorldManager.SaveGame.StoreData("LevelUPData_Farming", JsonSerializer.Serialize(farmingLevels));
    }

    public void OnBreakBlock(IServerPlayer player, BlockSelection breakedBlock, ref float dropQuantityMultiplier, ref EnumHandling handling)
    {
        EntityPlayer playerEntity = player.Entity;
        // If not a plant ignore
        if (breakedBlock.Block.BlockMaterial != EnumBlockMaterial.Plant && breakedBlock.Block.CropProps == null) return;
        if (breakedBlock.Block.CropProps.TotalGrowthDays <= 2) return;

        // Get all players levels
        Dictionary<string, int> farmingLevels = GetSavedLevels();

        // Get the exp received
        int exp = Configuration.expPerHarvestFarming.Get(breakedBlock.Block.Id, 0);

        // Get the actual player total exp
        int playerExp = farmingLevels.GetValueOrDefault(playerEntity.GetName(), 0);

        // Incrementing
        farmingLevels[playerEntity.GetName()] = playerExp + exp;

        // Saving
        SaveLevels(farmingLevels);
        // Updating
        Shared.Instance.UpdateLevelAndNotify(instance.api, player, "Farming", playerExp + exp);
    }
}