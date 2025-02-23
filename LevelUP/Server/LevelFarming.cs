using System;
using System.Collections.Generic;
using System.Text.Json;
using Vintagestory.API.Common;
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
        // Get the exp received
        float experienceMultiplierCompatibility = player.Entity.Attributes.GetFloat("LevelUP_Server_Instance_ExperienceMultiplier_IncreaseExp");
        int exp = (int)(Configuration.expPerHarvestFarming.GetValueOrDefault(breakedBlock.Block.Code.ToString(), 0) + (Configuration.expPerHarvestFarming.GetValueOrDefault(breakedBlock.Block.Code.ToString(), 0) * experienceMultiplierCompatibility));
        // No crop exp finded
        if (exp <= 0) return;
        // Increasing by player class
        exp = (int)Math.Round(exp * Configuration.GetEXPMultiplyByClassAndLevelType(player.Entity.WatchedAttributes.GetString("characterClass"), "Farming"));
        // Minium exp earned is 1
        if (exp <= 0) exp = Configuration.minimumEXPEarned;

        // Get all players levels
        Dictionary<string, ulong> farmingLevels = GetSavedLevels();

        // Get the actual player total exp
        ulong playerExp = farmingLevels.GetValueOrDefault<string, ulong>(player.PlayerUID, 0);
        if (Configuration.FarmingIsMaxLevel(playerExp)) return;

        if (Configuration.enableLevelUpExperienceServerLog)
            Debug.Log($"{player.PlayerName} breaked: {breakedBlock.Block.Code}, farming exp earned: {exp}, actual: {playerExp}");

        // Incrementing
        farmingLevels[player.PlayerUID] = playerExp + (ulong)exp;

        // Saving
        SaveLevels(farmingLevels);
        // Updating
        Shared.Instance.UpdateLevelAndNotify(instance.api, player, "Farming", farmingLevels[player.PlayerUID]);
    }
}