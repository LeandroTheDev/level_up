using System.Collections.Generic;
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

        Task.Delay(1000).ContinueWith((_) => { if (communicationChannel.Connected) communicationChannel.SendPacket("UpdateLevels"); });

        Debug.Log("Client side fully initialized");
    }

    private void OnServerMessage(ServerMessage bruteMessage)
    {
        string[] messages = bruteMessage.message.Split('&');
        switch (messages[0])
        {
            case "playerlevelup": LevelUPMessage(int.Parse(messages[1]), messages[2]); return;
        }
    }

    private void LevelUPMessage(int level, string levelType)
    => api.ShowChatMessage($"{Lang.Get("levelup:player_levelup_1")}{level}{Lang.Get("levelup:player_levelup_2")}{Lang.Get($"levelup:{levelType.ToLower()}")}");
}

[ProtoContract]
public class ServerMessage
{
    [ProtoMember(1)]
    public string message;
}