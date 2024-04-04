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
    // private IServerNetworkChannel channel;

    // Levels
    public LevelHunter levelHunter = new();
    public LevelBow levelBow = new();
    public LevelCutlery levelCutlery = new();
    private readonly Dictionary<string, bool> increaseExpDelay = [];
    
    public void Init(ICoreServerAPI serverAPI)
    {
        api = serverAPI;
        levelHunter.Init(this);
        levelBow.Init(this);
        levelCutlery.Init(this);
        // channel = 
        api.Network.RegisterChannel("LevelUP").RegisterMessageType(typeof(string)).SetMessageHandler<string>(OnClientMessage);
        Debug.Log("Server Network registered");
    }

    private void OnClientMessage(IServerPlayer player, string message)
    {
        Debug.Log($"Message received: {message} from {player.PlayerName}");

        switch (message)
        {
            case "UpdateLevels": UpdatePlayerLevels(player); return;
            case "Increase_Cutlery_Hit": IncreaseExp(player, "Cutlery", "Hit"); return;
            case "Increase_Bow_Hit": IncreaseExp(player, "Bow", "Hit"); return;
        }
    }

    private void IncreaseExp(IServerPlayer player, string levelType, string reason){
        // Check for delay
        if(increaseExpDelay.GetValueOrDefault(player.PlayerName, false)) return;
        increaseExpDelay[player.PlayerName] = true;
        Task.Delay(200).ContinueWith((_) => increaseExpDelay.Remove(player.PlayerName));

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
        if(levelType == "Bow" && reason == "Hit") {
            // Get levels
            var levels = GetSavedLevels();
            int exp = levels.GetValueOrDefault(player.PlayerName, 0) + Configuration.expPerHitBow;
            // Increment
            levels[player.PlayerName] = exp;
            // Save it
            SaveLevels(levels);
            // Update it
            Shared.Instance.UpdateLevelAndNotify(player, levelType, exp);
            Debug.Log($"{player.PlayerName} earned {Configuration.expPerHitCutlery} exp with {levelType} by {reason}");
        }
        #endregion
        #region cutlery
        // Hit
        if(levelType == "Cutlery" && reason == "Hit") {
            // Get levels
            var levels = GetSavedLevels();
            int exp = levels.GetValueOrDefault(player.PlayerName, 0) + Configuration.expPerHitCutlery;
            // Increment
            levels[player.PlayerName] = exp;
            // Save it
            SaveLevels(levels);
            // Update it
            Shared.Instance.UpdateLevelAndNotify(player, levelType, exp);
            Debug.Log($"{player.PlayerName} earned {Configuration.expPerHitCutlery} exp with {levelType} by {reason}");
        }
        #endregion
        #region axe
        // Hit
        if(levelType == "Axe" && reason == "Hit") {
            // Get levels
            var levels = GetSavedLevels();
            int exp = levels.GetValueOrDefault(player.PlayerName, 0) + Configuration.expPerHitAxe;
            // Increment
            levels[player.PlayerName] = exp;
            // Save it
            SaveLevels(levels);
            // Update it
            Shared.Instance.UpdateLevelAndNotify(player, levelType, exp);
            Debug.Log($"{player.PlayerName} earned {Configuration.expPerHitAxe} exp with {levelType} by {reason}");
        }
        #endregion
        #region pickaxe
        // Hit
        if(levelType == "Pickaxe" && reason == "Hit") {
            // Get levels
            var levels = GetSavedLevels();
            int exp = levels.GetValueOrDefault(player.PlayerName, 0) + Configuration.expPerHitPickaxe;
            // Increment
            levels[player.PlayerName] = exp;
            // Save it
            SaveLevels(levels);
            // Update it
            Shared.Instance.UpdateLevelAndNotify(player, levelType, exp);
            Debug.Log($"{player.PlayerName} earned {Configuration.expPerHitPickaxe} exp with {levelType} by {reason}");
        }
        #endregion
        #region shovel
        // Hit
        if(levelType == "Shovel" && reason == "Hit") {
            // Get levels
            var levels = GetSavedLevels();
            int exp = levels.GetValueOrDefault(player.PlayerName, 0) + Configuration.expPerHitShovel;
            // Increment
            levels[player.PlayerName] = exp;
            // Save it
            SaveLevels(levels);
            // Update it
            Shared.Instance.UpdateLevelAndNotify(player, levelType, exp);
            Debug.Log($"{player.PlayerName} earned {Configuration.expPerHitShovel} exp with {levelType} by {reason}");
        }
        #endregion
        #region spear
        // Hit
        if(levelType == "Spear" && reason == "Hit") {
            // Get levels
            var levels = GetSavedLevels();
            int exp = levels.GetValueOrDefault(player.PlayerName, 0) + Configuration.expPerHitSpear;
            // Increment
            levels[player.PlayerName] = exp;
            // Save it
            SaveLevels(levels);
            // Update it
            Shared.Instance.UpdateLevelAndNotify(player, levelType, exp);
            Debug.Log($"{player.PlayerName} earned {Configuration.expPerHitSpear} exp with {levelType} by {reason}");
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
        Shared.Instance.UpdateLevelAndNotify(player, "Hunter", GetSavedLevels("Hunter").GetValueOrDefault(player.PlayerName, 0), true);

        // Bow Level
        Shared.Instance.UpdateLevelAndNotify(player, "Bow", GetSavedLevels("Bow").GetValueOrDefault(player.PlayerName, 0), true);

        // Axe Level
        Shared.Instance.UpdateLevelAndNotify(player, "Cutlery", GetSavedLevels("Cutlery").GetValueOrDefault(player.PlayerName, 0), true);

        // Axe Level
        Shared.Instance.UpdateLevelAndNotify(player, "Axe", GetSavedLevels("Axe").GetValueOrDefault(player.PlayerName, 0), true);

        // Pickaxe Level
        Shared.Instance.UpdateLevelAndNotify(player, "Pickaxe", GetSavedLevels("Pickaxe").GetValueOrDefault(player.PlayerName, 0), true);

        // Shovel Level
        Shared.Instance.UpdateLevelAndNotify(player, "Shovel", GetSavedLevels("Shovel").GetValueOrDefault(player.PlayerName, 0), true);

        // Spear Level
        Shared.Instance.UpdateLevelAndNotify(player, "Spear", GetSavedLevels("Spear").GetValueOrDefault(player.PlayerName, 0), true);
    }
}