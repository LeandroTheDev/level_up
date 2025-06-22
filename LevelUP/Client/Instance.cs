using System.Collections.Generic;
using System.Text.Json;
using Vintagestory.API.Client;
using Vintagestory.API.Config;

namespace LevelUP.Client;

class Instance
{
    public ICoreClientAPI api;
    public IClientNetworkChannel CommunicationChannel;
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
        CommunicationChannel = api.Network.RegisterChannel("LevelUPServer").RegisterMessageType(typeof(ServerMessage));
        CommunicationChannel.SetMessageHandler<ServerMessage>(OnServerMessage);
        temporaryTickListener = api.Event.RegisterGameTickListener(OnTick, 1000, 1000);
        Debug.Log("Client side fully initialized");
    }

    private void OnTick(float obj)
    {
        if (CommunicationChannel.Connected)
        {
            CommunicationChannel.SendPacket(new ServerMessage() { message = "UpdateLevels" });
            CommunicationChannel.SendPacket(new ServerMessage() { message = "GetEnabledLevels" });
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
            case "playersublevelup": SubLevelUPMessage(int.Parse(messages[1]), messages[2], messages[3]); return;
            case "enabledlevels": enabledLevels = JsonSerializer.Deserialize<Dictionary<string, bool>>(messages[1]); return;
        }
    }

    private void LevelUPMessage(int level, string levelType)
        => api.ShowChatMessage(Lang.Get("levelup:player_levelup", level, Lang.Get($"levelup:{levelType.ToLower()}")));

    private void SubLevelUPMessage(int level, string levelType, string subLevelType)
        => api.ShowChatMessage(Lang.Get("levelup:player_sublevelup", level, Lang.Get($"levelup:{subLevelType.ToLower()}"), Lang.Get($"levelup:{levelType.ToLower()}")));
}

