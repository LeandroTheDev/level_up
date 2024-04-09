using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
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
    public LevelCutlery levelCutlery = new();
    public LevelAxe levelAxe = new();
    public LevelPickaxe levelPickaxe = new();
    public LevelShovel levelShovel = new();
    public LevelSpear levelSpear = new();
    public LevelFarming levelFarming = new();
    private readonly Dictionary<string, bool> increaseExpDelay = [];

    public void Init(ICoreServerAPI serverAPI)
    {
        api = serverAPI;
        levelHunter.Init(this);
        levelBow.Init(this);
        levelCutlery.Init(this);
        levelAxe.Init(this);
        levelPickaxe.Init(this);
        levelShovel.Init(this);
        levelSpear.Init(this);
        levelFarming.Init(this);
        Debug.Log("Server Levels instanciated");
        channel = api.Network.RegisterChannel("LevelUP").RegisterMessageType(typeof(string));
        channel.SetMessageHandler<string>(OnClientMessage);
        Debug.Log("Server Network registered");
    }

    public void OnClientMessage(IServerPlayer player, string message)
    {
        switch (message)
        {
            case "UpdateLevels": UpdatePlayerLevels(player); return;
            #region hit
            case "Increase_Cutlery_Hit": IncreaseExp(player, "Cutlery", "Hit"); return;
            case "Increase_Bow_Hit": IncreaseExp(player, "Bow", "Hit"); return;
            case "Increase_Axe_Hit": IncreaseExp(player, "Axe", "Hit"); return;
            case "Increase_Pickaxe_Hit": IncreaseExp(player, "Pickaxe", "Hit"); return;
            case "Increase_Shovel_Hit": IncreaseExp(player, "Shovel", "Hit"); return;
            case "Increase_Spear_Hit": IncreaseExp(player, "Spear", "Hit"); return;
            case "Increase_Spear_Hit_Throw": IncreaseExp(player, "Spear", "Hit_Throw"); return;
            #endregion
            #region breaking
            case "Block_Breaked_Axe": IncreaseExp(player, "Axe", "Breaking"); return;
            case "Block_Breaked_Pickaxe": IncreaseExp(player, "Pickaxe", "Breaking"); return;
            case "Block_Breaked_Shovel": IncreaseExp(player, "Shovel", "Breaking"); return;
            #endregion
            #region harvesting
            case "Tree_Breaked_Axe": IncreaseExp(player, "Axe", "Chop_Tree"); return;
            case "Cutlery_Harvest_Entity": IncreaseExp(player, "Cutlery", "Harvest"); return;
                #endregion
        }
    }

    private void IncreaseExp(IServerPlayer player, string levelType, string reason, bool delayIgnorable = false)
    {
        // Check for delay
        if (increaseExpDelay.GetValueOrDefault(player.PlayerName, false) && !delayIgnorable) return;
        increaseExpDelay[player.PlayerName] = true;
        Task.Delay(100).ContinueWith((_) => increaseExpDelay.Remove(player.PlayerName));

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
            int exp = levels.GetValueOrDefault(player.PlayerName, 0) + Configuration.expPerHitBow;
            // Increment
            levels[player.PlayerName] = exp;
            // Save it
            SaveLevels(levels);
            // Update it
            Shared.Instance.UpdateLevelAndNotify(api, player, levelType, exp);
            Debug.Log($"{player.PlayerName} earned {Configuration.expPerHitCutlery} exp with {levelType} by {reason}, actual: {exp}");
        }
        #endregion
        #region cutlery
        // Hit
        if (levelType == "Cutlery" && reason == "Hit")
        {
            // Get levels
            var levels = GetSavedLevels();
            int exp = levels.GetValueOrDefault(player.PlayerName, 0) + Configuration.expPerHitCutlery;
            // Increment
            levels[player.PlayerName] = exp;
            // Save it
            SaveLevels(levels);
            // Update it
            Shared.Instance.UpdateLevelAndNotify(api, player, levelType, exp);
            Debug.Log($"{player.PlayerName} earned {Configuration.expPerHitCutlery} exp with {levelType} by {reason}, actual: {exp}");
        }
        // Harvest
        else if (levelType == "Cutlery" && reason == "Harvest")
        {
            // Get levels
            var levels = GetSavedLevels();
            int exp = levels.GetValueOrDefault(player.PlayerName, 0) + Configuration.expPerHarvestCutlery;
            // Increment
            levels[player.PlayerName] = exp;
            // Save it
            SaveLevels(levels);
            // Update it
            Shared.Instance.UpdateLevelAndNotify(api, player, levelType, exp);
            Debug.Log($"{player.PlayerName} earned {Configuration.expPerHarvestCutlery} exp with {levelType} by {reason}, actual: {exp}");
        }
        #endregion
        #region axe
        // Hit
        if (levelType == "Axe" && reason == "Hit")
        {
            // Get levels
            var levels = GetSavedLevels();
            int exp = levels.GetValueOrDefault(player.PlayerName, 0) + Configuration.expPerHitAxe;
            // Increment
            levels[player.PlayerName] = exp;
            // Save it
            SaveLevels(levels);
            // Update it
            Shared.Instance.UpdateLevelAndNotify(api, player, levelType, exp);
            Debug.Log($"{player.PlayerName} earned {Configuration.expPerHitAxe} exp with {levelType} by {reason}, actual: {exp}");
        }
        // Block breaking
        else if (levelType == "Axe" && reason == "Breaking")
        {
            // Get levels
            var levels = GetSavedLevels();
            int exp = levels.GetValueOrDefault(player.PlayerName, 0) + Configuration.expPerBreakingAxe;
            // Increment
            levels[player.PlayerName] = exp;
            // Save it
            SaveLevels(levels);
            // Update it
            Shared.Instance.UpdateLevelAndNotify(api, player, levelType, exp);
            Debug.Log($"{player.PlayerName} earned {Configuration.expPerBreakingAxe} exp with {levelType} by {reason}, actual: {exp}");
        }
        // Chop Tree
        else if (levelType == "Axe" && reason == "Chop_Tree")
        {
            // Get levels
            var levels = GetSavedLevels();
            int exp = levels.GetValueOrDefault(player.PlayerName, 0) + Configuration.expPerTreeBreakingAxe;
            // Increment
            levels[player.PlayerName] = exp;
            // Save it
            SaveLevels(levels);
            // Update it
            Shared.Instance.UpdateLevelAndNotify(api, player, levelType, exp);
            Debug.Log($"{player.PlayerName} earned {Configuration.expPerTreeBreakingAxe} exp with {levelType} by {reason}, actual: {exp}");
        }
        #endregion
        #region pickaxe
        // Hit
        if (levelType == "Pickaxe" && reason == "Hit")
        {
            // Get levels
            var levels = GetSavedLevels();
            int exp = levels.GetValueOrDefault(player.PlayerName, 0) + Configuration.expPerHitPickaxe;
            // Increment
            levels[player.PlayerName] = exp;
            // Save it
            SaveLevels(levels);
            // Update it
            Shared.Instance.UpdateLevelAndNotify(api, player, levelType, exp);
            Debug.Log($"{player.PlayerName} earned {Configuration.expPerHitPickaxe} exp with {levelType} by {reason}, actual: {exp}");
        }
        // Break blocks
        else if (levelType == "Pickaxe" && reason == "Breaking")
        {
            // Get levels
            var levels = GetSavedLevels();
            int exp = levels.GetValueOrDefault(player.PlayerName, 0) + Configuration.expPerBreakingPickaxe;
            // Increment
            levels[player.PlayerName] = exp;
            // Save it
            SaveLevels(levels);
            // Update it
            Shared.Instance.UpdateLevelAndNotify(api, player, levelType, exp);
            Debug.Log($"{player.PlayerName} earned {Configuration.expPerBreakingPickaxe} exp with {levelType} by {reason}, actual: {exp}");
        }
        #endregion
        #region shovel
        // Hit
        if (levelType == "Shovel" && reason == "Hit")
        {
            // Get levels
            var levels = GetSavedLevels();
            int exp = levels.GetValueOrDefault(player.PlayerName, 0) + Configuration.expPerHitShovel;
            // Increment
            levels[player.PlayerName] = exp;
            // Save it
            SaveLevels(levels);
            // Update it
            Shared.Instance.UpdateLevelAndNotify(api, player, levelType, exp);
            Debug.Log($"{player.PlayerName} earned {Configuration.expPerHitShovel} exp with {levelType} by {reason}, actual: {exp}");
        }
        // Break block
        else if (levelType == "Shovel" && reason == "Breaking")
        {
            // Get levels
            var levels = GetSavedLevels();
            int exp = levels.GetValueOrDefault(player.PlayerName, 0) + Configuration.expPerBreakingShovel;
            // Increment
            levels[player.PlayerName] = exp;
            // Save it
            SaveLevels(levels);
            // Update it
            Shared.Instance.UpdateLevelAndNotify(api, player, levelType, exp);
            Debug.Log($"{player.PlayerName} earned {Configuration.expPerBreakingShovel} exp with {levelType} by {reason}, actual: {exp}");
        }
        #endregion
        #region spear
        // Hit
        if (levelType == "Spear" && reason == "Hit")
        {
            // Get levels
            var levels = GetSavedLevels();
            int exp = levels.GetValueOrDefault(player.PlayerName, 0) + Configuration.expPerHitSpear;
            // Increment
            levels[player.PlayerName] = exp;
            // Save it
            SaveLevels(levels);
            // Update it
            Shared.Instance.UpdateLevelAndNotify(api, player, levelType, exp);
            Debug.Log($"{player.PlayerName} earned {Configuration.expPerHitSpear} exp with {levelType} by {reason}, actual: {exp}");
        }
        // Throw
        else if (levelType == "Spear" && reason == "Hit_Throw")
        {
            // Get levels
            var levels = GetSavedLevels();
            int exp = levels.GetValueOrDefault(player.PlayerName, 0) + Configuration.expPerThrowSpear;
            // Increment
            levels[player.PlayerName] = exp;
            // Save it
            SaveLevels(levels);
            // Update it
            Shared.Instance.UpdateLevelAndNotify(api, player, levelType, exp);
            Debug.Log($"{player.PlayerName} earned {Configuration.expPerThrowSpear} exp with {levelType} by {reason}, actual: {exp}");
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
        Shared.Instance.UpdateLevelAndNotify(api, player, "Cutlery", GetSavedLevels("Cutlery").GetValueOrDefault(player.PlayerName, 0), true);

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
    }
}