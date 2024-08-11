using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using ProtoBuf;
using Vintagestory.API.Client;
using Vintagestory.API.Config;

namespace LevelUP.Client;

class Instance
{
    public ICoreClientAPI api;
    public IClientNetworkChannel compatibilityChannel;
    public IClientNetworkChannel communicationChannel;
    public Dictionary<string, bool> enabledLevels = [];

    long temporaryTickListener;

    // Store the view for character menu
    private readonly CharacterView characterView = new();

    public static void PreInit()
    {
    }
    public static void Dispose()
    {
    }

    public void Init(ICoreClientAPI clientAPI)
    {
        api = clientAPI;
        characterView.Init(this);
        compatibilityChannel = api.Network.RegisterChannel("LevelUP").RegisterMessageType(typeof(string));
        communicationChannel = api.Network.RegisterChannel("LevelUPServer").RegisterMessageType(typeof(ServerMessage));
        communicationChannel.SetMessageHandler<ServerMessage>(OnServerMessage);
        temporaryTickListener = api.Event.RegisterGameTickListener(OnTick, 1000, 1000);
        Debug.Log("Client side fully initialized");
    }

    private void OnTick(float obj)
    {
        if (communicationChannel.Connected)
        {
            communicationChannel.SendPacket(new ServerMessage() { message = "UpdateLevels" });
            communicationChannel.SendPacket(new ServerMessage() { message = "GetEnabledLevels" });
            api.Event.UnregisterGameTickListener(temporaryTickListener);
            Debug.Log("Channel connected and instances refreshed");
        }
    }

    private void OnServerMessage(ServerMessage bruteMessage)
    {
        string[] messages = bruteMessage.message.Split('&');
        switch (messages[0])
        {
            case "playerlevelup": LevelUPMessage(int.Parse(messages[1]), messages[2]); return;
            case "enabledlevels": enabledLevels = JsonSerializer.Deserialize<Dictionary<string, bool>>(messages[1]); return;
        }
    }

    private void LevelUPMessage(int level, string levelType)
    => api.ShowChatMessage($"{Lang.Get("levelup:player_levelup_1")}{level}{Lang.Get("levelup:player_levelup_2")}{Lang.Get($"levelup:{levelType.ToLower()}")}");
}

