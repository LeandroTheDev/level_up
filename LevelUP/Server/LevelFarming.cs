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

    public void Init(Instance _instance)
    {
        instance = _instance;
        // Instanciate break block event
        instance.api.Event.BreakBlock += OnBreakBlock;

        Debug.Log("Level Farming initialized");
    }

    #pragma warning disable CA1822
    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulateFarmingConfiguration(coreAPI);
    }
    #pragma warning restore CA1822


    private Dictionary<string, ulong> GetSavedLevels()
    {
        byte[] dataBytes = instance.api.WorldManager.SaveGame.GetData("LevelUPData_Farming");
        string data = dataBytes == null ? "{}" : SerializerUtil.Deserialize<string>(dataBytes);
        return JsonSerializer.Deserialize<Dictionary<string, ulong>>(data);
    }

    private void SaveLevels(Dictionary<string, ulong> farmingLevels)
    {
        instance.api.WorldManager.SaveGame.StoreData("LevelUPData_Farming", JsonSerializer.Serialize(farmingLevels));
    }

    public void OnBreakBlock(IServerPlayer player, BlockSelection breakedBlock, ref float dropQuantityMultiplier, ref EnumHandling handling)
    {
        EntityPlayer playerEntity = player.Entity;
        // If not a plant ignore
        if (breakedBlock.Block.BlockMaterial != EnumBlockMaterial.Plant && breakedBlock.Block.CropProps == null) return;
        // if (breakedBlock.Block.CropProps.TotalGrowthDays <= 2) return;

        Debug.Log($"{breakedBlock.Block.Code}");

        // Get all players levels
        Dictionary<string, ulong> farmingLevels = GetSavedLevels();

        // Get the exp received
        int exp = Configuration.expPerHarvestFarming.Get(breakedBlock.Block.Code.ToString(), 0);

        // Get the actual player total exp
        ulong playerExp = farmingLevels.GetValueOrDefault<string, ulong>(playerEntity.GetName(), 0);

        // Incrementing
        farmingLevels[playerEntity.GetName()] = playerExp + (ulong)exp;

        // Saving
        SaveLevels(farmingLevels);
        // Updating
        Shared.Instance.UpdateLevelAndNotify(instance.api, player, "Farming", farmingLevels[playerEntity.GetName()]);
    }
}