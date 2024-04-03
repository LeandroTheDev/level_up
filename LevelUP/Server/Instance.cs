using System;
using System.Collections.Generic;
using System.Text.Json;
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
            player.Entity.WatchedAttributes.SetInt($"LevelUP_{levelType}", exp);
            Debug.Log($"{player.PlayerName} earned {Configuration.expPerHitCutery} exp with {levelType} by {reason}");
        }
        #endregion
        #region cutlery
        // Hit
        if(levelType == "Cutlery" && reason == "Hit") {
            // Get levels
            var levels = GetSavedLevels();
            int exp = levels.GetValueOrDefault(player.PlayerName, 0) + Configuration.expPerHitCutery;
            // Increment
            levels[player.PlayerName] = exp;
            // Save it
            SaveLevels(levels);
            // Update it
            player.Entity.WatchedAttributes.SetInt($"LevelUP_{levelType}", exp);
            Debug.Log($"{player.PlayerName} earned {Configuration.expPerHitCutery} exp with {levelType} by {reason}");
        }
        #endregion
    }

    private void UpdatePlayerLevels(IServerPlayer player)
    {
        // Get all players hunter level
        Dictionary<string, int> GetSavedHunterLevels()
        {
            byte[] dataBytes = api.WorldManager.SaveGame.GetData("LevelUPData_Hunter");
            string data = dataBytes == null ? "{}" : SerializerUtil.Deserialize<string>(dataBytes);
            return JsonSerializer.Deserialize<Dictionary<string, int>>(data);
        }
        // Hunter Level
        player.Entity.WatchedAttributes.SetInt("LevelUP_Hunter", GetSavedHunterLevels().GetValueOrDefault(player.PlayerName, 0));

        // Get all players bow level
        Dictionary<string, int> GetSavedBowLevels()
        {
            byte[] dataBytes = api.WorldManager.SaveGame.GetData("LevelUPData_Bow");
            string data = dataBytes == null ? "{}" : SerializerUtil.Deserialize<string>(dataBytes);
            return JsonSerializer.Deserialize<Dictionary<string, int>>(data);
        }
        // Bow Level
        player.Entity.WatchedAttributes.SetInt("LevelUP_Bow", GetSavedBowLevels().GetValueOrDefault(player.PlayerName, 0));

        // Get all players cutlery level
        Dictionary<string, int> GetSavedCutleryLevels()
        {
            byte[] dataBytes = api.WorldManager.SaveGame.GetData("LevelUPData_Cutlery");
            string data = dataBytes == null ? "{}" : SerializerUtil.Deserialize<string>(dataBytes);
            return JsonSerializer.Deserialize<Dictionary<string, int>>(data);
        }
        // Axe Level
        player.Entity.WatchedAttributes.SetInt("LevelUP_Axe", GetSavedAxeLevels().GetValueOrDefault(player.PlayerName, 0));

        // Get all players axe level
        Dictionary<string, int> GetSavedAxeLevels()
        {
            byte[] dataBytes = api.WorldManager.SaveGame.GetData("LevelUPData_Axe");
            string data = dataBytes == null ? "{}" : SerializerUtil.Deserialize<string>(dataBytes);
            return JsonSerializer.Deserialize<Dictionary<string, int>>(data);
        }
        // Axe Level
        player.Entity.WatchedAttributes.SetInt("LevelUP_Axe", GetSavedCutleryLevels().GetValueOrDefault(player.PlayerName, 0));

        // Get all players pickaxe level
        Dictionary<string, int> GetSavedPickaxeLevels()
        {
            byte[] dataBytes = api.WorldManager.SaveGame.GetData("LevelUPData_Pickaxe");
            string data = dataBytes == null ? "{}" : SerializerUtil.Deserialize<string>(dataBytes);
            return JsonSerializer.Deserialize<Dictionary<string, int>>(data);
        }
        // Pickaxe Level
        player.Entity.WatchedAttributes.SetInt("LevelUP_Pickaxe", GetSavedPickaxeLevels().GetValueOrDefault(player.PlayerName, 0));

        // Get all players shovel level
        Dictionary<string, int> GetSavedShovelLevels()
        {
            byte[] dataBytes = api.WorldManager.SaveGame.GetData("LevelUPData_Shovel");
            string data = dataBytes == null ? "{}" : SerializerUtil.Deserialize<string>(dataBytes);
            return JsonSerializer.Deserialize<Dictionary<string, int>>(data);
        }
        // Shovel Level
        player.Entity.WatchedAttributes.SetInt("LevelUP_Shovel", GetSavedShovelLevels().GetValueOrDefault(player.PlayerName, 0));
    }
}