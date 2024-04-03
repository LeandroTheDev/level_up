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

    public void PreInit()
    {
        levelHunter.OverrideNativeFunctions();
    }
    public void Dispose()
    {
        levelHunter.OverrideDispose();
    }
    
    public void Init(ICoreServerAPI serverAPI)
    {
        api = serverAPI;
        levelHunter.Init(this);
        Debug.Log("Level system fully initialized");
        // channel = 
        api.Network.RegisterChannel("LevelUP").RegisterMessageType(typeof(string)).SetMessageHandler<string>(OnClientMessage);
        Debug.Log("Server Network registered");

        Debug.Log("Server side fully initialized");
    }

    private void OnClientMessage(IServerPlayer player, string message)
    {
        Debug.Log($"Message received: {message} from {player.PlayerName}");

        // Send message for the players or player
        //void SendStringMessage(string message, IServerPlayer[] players) {
        //    channel.SendPacket<string>(message, players);
        //};

        switch (message)
        {
            case "UpdateLevels": UpdatePlayerLevels(player); return;
        }
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
    }
}