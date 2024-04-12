using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Timers;
using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using Vintagestory.API.Util;

namespace LevelUP.Server;

class Instance
{
    public ICoreServerAPI api;
    private IServerNetworkChannel channel;

    // Levels
    public LevelHunter levelHunter = new();
    public LevelBow levelBow = new();
    public LevelKnife levelKnife = new();
    public LevelAxe levelAxe = new();
    public LevelPickaxe levelPickaxe = new();
    public LevelShovel levelShovel = new();
    public LevelSpear levelSpear = new();
    public LevelFarming levelFarming = new();
    public LevelVitality levelVitality = new();

    public void Init(ICoreServerAPI serverAPI)
    {
        api = serverAPI;
        levelHunter.Init(this);
        levelBow.Init(this);
        levelKnife.Init(this);
        levelAxe.Init(this);
        levelPickaxe.Init(this);
        levelShovel.Init(this);
        levelSpear.Init(this);
        levelFarming.Init(this);
        levelVitality.Init(this);
        Debug.Log("Server Levels instanciated");
        channel = api.Network.RegisterChannel("LevelUP").RegisterMessageType(typeof(string));
        channel.SetMessageHandler<string>(OnClientMessage);
        Debug.Log("Server Network registered");
    }

    public void PopulateConfigurations(ICoreAPI coreAPI)
    {
        levelHunter.PopulateConfiguration(coreAPI);
        levelBow.PopulateConfiguration(coreAPI);
        levelKnife.PopulateConfiguration(coreAPI);
        levelAxe.PopulateConfiguration(coreAPI);
        levelPickaxe.PopulateConfiguration(coreAPI);
        levelShovel.PopulateConfiguration(coreAPI);
        levelSpear.PopulateConfiguration(coreAPI);
        levelFarming.PopulateConfiguration(coreAPI);
        levelVitality.PopulateConfiguration(coreAPI);
    }

    public void OnClientMessage(IServerPlayer player, string bruteMessage)
    {
        // Translate aguments if exist
        Dictionary<string, object> arguments = [];
        string message;
        if (bruteMessage.Contains('&'))
        {
            string[] messages = bruteMessage.Split('&');

            message = messages[0];

            // Remove message context from the arguments
            string[] argumentsArray = new string[messages.Length - 1];
            Array.Copy(messages, 1, argumentsArray, 0, messages.Length - 1);

            // Swipe arguments
            foreach (string value in argumentsArray)
            {
                string[] keyValue = value.Split('=');
                // Add argument to the list
                if (keyValue.Length == 2) arguments[keyValue[0]] = keyValue[1].ToString();
                else Debug.Log($"ERROR OnClientMessage: invalid argument quantity: {keyValue.Length}");
            }
        }
        else message = bruteMessage;

        switch (message)
        {
            case "UpdateLevels": UpdatePlayerLevels(player); return;
            #region hit
            case "Increase_Knife_Hit": IncreaseExp(player, "Knife", "Hit"); return;
            case "Increase_Bow_Hit": IncreaseExp(player, "Bow", "Hit"); return;
            case "Increase_Axe_Hit": IncreaseExp(player, "Axe", "Hit"); return;
            case "Increase_Pickaxe_Hit": IncreaseExp(player, "Pickaxe", "Hit"); return;
            case "Increase_Shovel_Hit": IncreaseExp(player, "Shovel", "Hit"); return;
            case "Increase_Spear_Hit": IncreaseExp(player, "Spear", "Hit"); return;
            case "Increase_Spear_Hit_Throw": IncreaseExp(player, "Spear", "Hit_Throw"); return;
            case "Increase_Vitality_Hit": IncreaseExp(player, "Vitality", "Hit", arguments["forceexp"].ToString().ToInt()); return;
            #endregion
            #region breaking
            case "Block_Breaked_Axe": IncreaseExp(player, "Axe", "Breaking"); return;
            case "Block_Breaked_Pickaxe": IncreaseExp(player, "Pickaxe", "Breaking"); return;
            case "Block_Breaked_Shovel": IncreaseExp(player, "Shovel", "Breaking"); return;
            #endregion
            #region harvesting
            case "Tree_Breaked_Axe": IncreaseExp(player, "Axe", "Chop_Tree"); return;
            case "Knife_Harvest_Entity": IncreaseExp(player, "Knife", "Harvest"); return;
            case "Soil_Till": IncreaseExp(player, "Farming", "Till"); return;
            #endregion
            #region crafting
            case "Cooking_Finished": IncreaseExp(player, "Cooking", "Cooking_Finished"); return;
                #endregion
        }

    }

    private void IncreaseExp(IServerPlayer player, string levelType, string reason, int forceexp = 0)
    {
        Dictionary<string, int> GetSavedLevels()
        {
            byte[] dataBytes = api.WorldManager.SaveGame.GetData($"LevelUPData_{levelType}");
            string data = dataBytes == null ? "{}" : SerializerUtil.Deserialize<string>(dataBytes);
            return JsonSerializer.Deserialize<Dictionary<string, int>>(data);
        }

        void SaveLevels(Dictionary<string, int> levels)
        {
            api.WorldManager.SaveGame.StoreData($"LevelUPData_{levelType}", JsonSerializer.Serialize(levels));
        }

        #region bow
        // Hit
        if (levelType == "Bow" && reason == "Hit")
        {
            // Get levels
            var levels = GetSavedLevels();
            int exp = levels.GetValueOrDefault(player.PlayerName, 0) + Configuration.ExpPerHitBow;
            // Increment
            levels[player.PlayerName] = exp;
            // Save it
            SaveLevels(levels);
            // Update it
            Shared.Instance.UpdateLevelAndNotify(api, player, levelType, exp);
            Debug.Log($"{player.PlayerName} earned {Configuration.ExpPerHitKnife} exp with {levelType} by {reason}, actual: {exp}");
        }
        #endregion
        #region knife
        // Hit
        if (levelType == "Knife" && reason == "Hit")
        {
            // Get levels
            var levels = GetSavedLevels();
            int exp = levels.GetValueOrDefault(player.PlayerName, 0) + Configuration.ExpPerHitKnife;
            // Increment
            levels[player.PlayerName] = exp;
            // Save it
            SaveLevels(levels);
            // Update it
            Shared.Instance.UpdateLevelAndNotify(api, player, levelType, exp);
            Debug.Log($"{player.PlayerName} earned {Configuration.ExpPerHitKnife} exp with {levelType} by {reason}, actual: {exp}");
        }
        // Harvest
        else if (levelType == "Knife" && reason == "Harvest")
        {
            // Get levels
            var levels = GetSavedLevels();
            int exp = levels.GetValueOrDefault(player.PlayerName, 0) + Configuration.ExpPerHarvestKnife;
            // Increment
            levels[player.PlayerName] = exp;
            // Save it
            SaveLevels(levels);
            // Update it
            Shared.Instance.UpdateLevelAndNotify(api, player, levelType, exp);
            Debug.Log($"{player.PlayerName} earned {Configuration.ExpPerHarvestKnife} exp with {levelType} by {reason}, actual: {exp}");
        }
        #endregion
        #region axe
        // Hit
        if (levelType == "Axe" && reason == "Hit")
        {
            // Get levels
            var levels = GetSavedLevels();
            int exp = levels.GetValueOrDefault(player.PlayerName, 0) + Configuration.ExpPerHitAxe;
            // Increment
            levels[player.PlayerName] = exp;
            // Save it
            SaveLevels(levels);
            // Update it
            Shared.Instance.UpdateLevelAndNotify(api, player, levelType, exp);
            Debug.Log($"{player.PlayerName} earned {Configuration.ExpPerHitAxe} exp with {levelType} by {reason}, actual: {exp}");
        }
        // Block breaking
        else if (levelType == "Axe" && reason == "Breaking")
        {
            // Get levels
            var levels = GetSavedLevels();
            int exp = levels.GetValueOrDefault(player.PlayerName, 0) + Configuration.ExpPerBreakingAxe;
            // Increment
            levels[player.PlayerName] = exp;
            // Save it
            SaveLevels(levels);
            // Update it
            Shared.Instance.UpdateLevelAndNotify(api, player, levelType, exp);
            Debug.Log($"{player.PlayerName} earned {Configuration.ExpPerBreakingAxe} exp with {levelType} by {reason}, actual: {exp}");
        }
        // Chop Tree
        else if (levelType == "Axe" && reason == "Chop_Tree")
        {
            // Get levels
            var levels = GetSavedLevels();
            int exp = levels.GetValueOrDefault(player.PlayerName, 0) + Configuration.ExpPerTreeBreakingAxe;
            // Increment
            levels[player.PlayerName] = exp;
            // Save it
            SaveLevels(levels);
            // Update it
            Shared.Instance.UpdateLevelAndNotify(api, player, levelType, exp);
            Debug.Log($"{player.PlayerName} earned {Configuration.ExpPerTreeBreakingAxe} exp with {levelType} by {reason}, actual: {exp}");
        }
        #endregion
        #region pickaxe
        // Hit
        if (levelType == "Pickaxe" && reason == "Hit")
        {
            // Get levels
            var levels = GetSavedLevels();
            int exp = levels.GetValueOrDefault(player.PlayerName, 0) + Configuration.ExpPerHitPickaxe;
            // Increment
            levels[player.PlayerName] = exp;
            // Save it
            SaveLevels(levels);
            // Update it
            Shared.Instance.UpdateLevelAndNotify(api, player, levelType, exp);
            Debug.Log($"{player.PlayerName} earned {Configuration.ExpPerHitPickaxe} exp with {levelType} by {reason}, actual: {exp}");
        }
        // Break blocks
        else if (levelType == "Pickaxe" && reason == "Breaking")
        {
            // Get levels
            var levels = GetSavedLevels();
            int exp = levels.GetValueOrDefault(player.PlayerName, 0) + Configuration.ExpPerBreakingPickaxe;
            // Increment
            levels[player.PlayerName] = exp;
            // Save it
            SaveLevels(levels);
            // Update it
            Shared.Instance.UpdateLevelAndNotify(api, player, levelType, exp);
            Debug.Log($"{player.PlayerName} earned {Configuration.ExpPerBreakingPickaxe} exp with {levelType} by {reason}, actual: {exp}");
        }
        #endregion
        #region shovel
        // Hit
        if (levelType == "Shovel" && reason == "Hit")
        {
            // Get levels
            var levels = GetSavedLevels();
            int exp = levels.GetValueOrDefault(player.PlayerName, 0) + Configuration.ExpPerHitShovel;
            // Increment
            levels[player.PlayerName] = exp;
            // Save it
            SaveLevels(levels);
            // Update it
            Shared.Instance.UpdateLevelAndNotify(api, player, levelType, exp);
            Debug.Log($"{player.PlayerName} earned {Configuration.ExpPerHitShovel} exp with {levelType} by {reason}, actual: {exp}");
        }
        // Break block
        else if (levelType == "Shovel" && reason == "Breaking")
        {
            // Get levels
            var levels = GetSavedLevels();
            int exp = levels.GetValueOrDefault(player.PlayerName, 0) + Configuration.ExpPerBreakingShovel;
            // Increment
            levels[player.PlayerName] = exp;
            // Save it
            SaveLevels(levels);
            // Update it
            Shared.Instance.UpdateLevelAndNotify(api, player, levelType, exp);
            Debug.Log($"{player.PlayerName} earned {Configuration.ExpPerBreakingShovel} exp with {levelType} by {reason}, actual: {exp}");
        }
        #endregion
        #region spear
        // Hit
        if (levelType == "Spear" && reason == "Hit")
        {
            // Get levels
            var levels = GetSavedLevels();
            int exp = levels.GetValueOrDefault(player.PlayerName, 0) + Configuration.ExpPerHitSpear;
            // Increment
            levels[player.PlayerName] = exp;
            // Save it
            SaveLevels(levels);
            // Update it
            Shared.Instance.UpdateLevelAndNotify(api, player, levelType, exp);
            Debug.Log($"{player.PlayerName} earned {Configuration.ExpPerHitSpear} exp with {levelType} by {reason}, actual: {exp}");
        }
        // Throw
        else if (levelType == "Spear" && reason == "Hit_Throw")
        {
            // Get levels
            var levels = GetSavedLevels();
            int exp = levels.GetValueOrDefault(player.PlayerName, 0) + Configuration.ExpPerThrowSpear;
            // Increment
            levels[player.PlayerName] = exp;
            // Save it
            SaveLevels(levels);
            // Update it
            Shared.Instance.UpdateLevelAndNotify(api, player, levelType, exp);
            Debug.Log($"{player.PlayerName} earned {Configuration.ExpPerThrowSpear} exp with {levelType} by {reason}, actual: {exp}");
        }
        #endregion
        #region farming
        // Till Soil
        if (levelType == "Farming" && reason == "Till")
        {
            // Get levels
            var levels = GetSavedLevels();
            int exp = levels.GetValueOrDefault(player.PlayerName, 0) + Configuration.ExpPerTillFarming;
            // Increment
            levels[player.PlayerName] = exp;
            // Save it
            SaveLevels(levels);
            // Update it
            Shared.Instance.UpdateLevelAndNotify(api, player, levelType, exp);
            Debug.Log($"{player.PlayerName} earned {Configuration.ExpPerTillFarming} exp with {levelType} by {reason}, actual: {exp}");
        }
        #endregion
        #region cooking
        // Cooking
        if (levelType == "Cooking" && reason == "Cooking_Finished")
        {
            // Get levels
            var levels = GetSavedLevels();
            int exp = levels.GetValueOrDefault(player.PlayerName, 0) + Configuration.expPerCookedCooking;
            // Increment
            levels[player.PlayerName] = exp;
            // Save it
            SaveLevels(levels);
            // Update it
            Shared.Instance.UpdateLevelAndNotify(api, player, levelType, exp);
            Debug.Log($"{player.PlayerName} earned {Configuration.expPerCookedCooking} exp with {levelType} by {reason}, actual: {exp}");
        }
        #endregion
        #region vitality
        // Get hit
        if (levelType == "Vitality" && reason == "Hit" && forceexp > 0)
        {
            // Get levels
            var levels = GetSavedLevels();
            int exp = levels.GetValueOrDefault(player.PlayerName, 0) + forceexp;
            // Increment
            levels[player.PlayerName] = exp;
            // Save it
            SaveLevels(levels);
            // Update it
            Shared.Instance.UpdateLevelAndNotify(api, player, levelType, exp);
            Debug.Log($"{player.PlayerName} earned {forceexp} exp with {levelType} by {reason}, actual: {exp}");
        }
        #endregion
    }

    private void UpdatePlayerLevels(IServerPlayer player)
    {
        // Get all players hunter level
        Dictionary<string, int> GetSavedLevels(string levelType)
        {
            byte[] dataBytes = api.WorldManager.SaveGame.GetData($"LevelUPData_{levelType}");
            string data = dataBytes == null ? "{}" : SerializerUtil.Deserialize<string>(dataBytes);
            return JsonSerializer.Deserialize<Dictionary<string, int>>(data);
        }

        // Hunter Level
        Shared.Instance.UpdateLevelAndNotify(api, player, "Hunter", GetSavedLevels("Hunter").GetValueOrDefault(player.PlayerName, 0), true);

        // Bow Level
        Shared.Instance.UpdateLevelAndNotify(api, player, "Bow", GetSavedLevels("Bow").GetValueOrDefault(player.PlayerName, 0), true);

        // Axe Level
        Shared.Instance.UpdateLevelAndNotify(api, player, "Knife", GetSavedLevels("Knife").GetValueOrDefault(player.PlayerName, 0), true);

        // Axe Level
        Shared.Instance.UpdateLevelAndNotify(api, player, "Axe", GetSavedLevels("Axe").GetValueOrDefault(player.PlayerName, 0), true);

        // Pickaxe Level
        Shared.Instance.UpdateLevelAndNotify(api, player, "Pickaxe", GetSavedLevels("Pickaxe").GetValueOrDefault(player.PlayerName, 0), true);

        // Shovel Level
        Shared.Instance.UpdateLevelAndNotify(api, player, "Shovel", GetSavedLevels("Shovel").GetValueOrDefault(player.PlayerName, 0), true);

        // Spear Level
        Shared.Instance.UpdateLevelAndNotify(api, player, "Spear", GetSavedLevels("Spear").GetValueOrDefault(player.PlayerName, 0), true);

        // Farming Level
        Shared.Instance.UpdateLevelAndNotify(api, player, "Farming", GetSavedLevels("Farming").GetValueOrDefault(player.PlayerName, 0), true);

        // Cooking Level
        Shared.Instance.UpdateLevelAndNotify(api, player, "Cooking", GetSavedLevels("Cooking").GetValueOrDefault(player.PlayerName, 0), true);

        // Vitality Level
        Shared.Instance.UpdateLevelAndNotify(api, player, "Vitality", GetSavedLevels("Vitality").GetValueOrDefault(player.PlayerName, 0), true);
    }
}