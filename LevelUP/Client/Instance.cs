using System.Collections.Generic;
using System.Threading.Tasks;
using Vintagestory.API.Client;

namespace LevelUP.Client;

class Instance
{
    public ICoreClientAPI api;
    public IClientNetworkChannel channel;

    // Store the view for character menu
    private readonly CharacterView characterView = new();

    public static void PreInit() {
    }
    public static void Dispose() {        
    }

    public void Init(ICoreClientAPI clientAPI)
    {
        api = clientAPI;
        characterView.Init(this);
        channel = api.Network.RegisterChannel("LevelUP").RegisterMessageType(typeof(string));

        if (channel.Connected) channel.SendPacket("UpdateLevels");
        // If not connect is because the client not connected to the channel yet,
        // so we wait 1 sec to try again, if cannot connect simple ignore the update.
        else Task.Delay(1000).ContinueWith((_) => { if (channel.Connected) channel.SendPacket("UpdateLevels"); });

        Debug.Log("Client side fully initialized");
    }
}