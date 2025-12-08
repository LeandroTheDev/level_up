#pragma warning disable CA1822
using System;
using System.Collections.Generic;
using System.Text.Json;
using HarmonyLib;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.GameContent;

namespace LevelUP.Client;

class Instance
{
    public ICoreClientAPI api;
    public IClientNetworkChannel CommunicationChannel;
    public Dictionary<string, bool> enabledLevels = [];

    long temporaryTickListener;

    private readonly LevelsView levelsView = new();

    public void Init(ICoreClientAPI clientAPI)
    {
        api = clientAPI;
        levelsView.Init(this);
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

    public static event Action RefreshWatchedAttributes;

    private void OnServerMessage(ServerMessage bruteMessage)
    {
        string[] messages = bruteMessage.message.Split('&');
        switch (messages[0])
        {
            case "playerlevelup": LevelUPMessage(int.Parse(messages[1]), messages[2]); RefreshWatchedAttributes?.Invoke(); return;
            case "playersublevelup": SubLevelUPMessage(int.Parse(messages[1]), messages[2], messages[3]); RefreshWatchedAttributes?.Invoke(); return;
            case "enabledlevels": enabledLevels = JsonSerializer.Deserialize<Dictionary<string, bool>>(messages[1]); return;
            case "playerhardcoredied": api.ShowChatMessage(Lang.Get("levelup:hardcore_message", 0)); return;
            case "syncconfig": SyncConfigurations(messages[1]); return;
            case "maxsaturationupdated": UpdateMaxSaturation(messages[1]); return;
        }
    }

    private void UpdateMaxSaturation(string maxsaturation)
    {
        if (float.TryParse(maxsaturation, out float value))
        {
            IPlayer player = api.World.Player;
            EntityBehaviorHunger playerStats = player.Entity.GetBehavior<EntityBehaviorHunger>();

            playerStats.MaxSaturation = value;
            playerStats.UpdateNutrientHealthBoost();
        }
    }

    private void SyncConfigurations(string json)
    {
        Configuration.ConsumeGeneratedClassJsonParameters(json);
        Shared.Instance.PatchAll();
        Debug.LogDebug("Client functions patched!");
    }

    private void LevelUPMessage(int level, string levelType)
        => api.ShowChatMessage(Lang.Get("levelup:player_levelup", level, Lang.Get($"levelup:{levelType.ToLower()}")));

    private void SubLevelUPMessage(int level, string levelType, string subLevelType)
        => api.ShowChatMessage(Lang.Get("levelup:player_sublevelup", level, Lang.Get($"levelup:{subLevelType.ToLower()}"), Lang.Get($"levelup:{levelType.ToLower()}")));
}

