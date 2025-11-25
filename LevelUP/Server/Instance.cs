using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace LevelUP.Server;

public class Instance
{
    static internal ICoreServerAPI api;
    static public IServerNetworkChannel CommunicationChannel { get; private set; }
    private readonly Commands commands = new();


    internal void Init(ICoreServerAPI serverAPI)
    {
        api = serverAPI;

        // Player Experience Load
        api.Event.PlayerJoin += Experience.LoadPlayer;
        api.Event.PlayerDisconnect += Experience.UnloadPlayer;

        // World Save
        api.Event.GameWorldSave += () => Experience.SaveExperience(api);

        // Update Watched Attributes
        api.Event.PlayerNowPlaying += player => UpdatePlayerLevels(player, api);
        // Sync server configurations with client
        api.Event.PlayerNowPlaying += SyncPlayerConfigs;

        Shared.Instance.PatchAll(api);
        Debug.Log("Server Levels instanciated");

        // Register commands
        commands.Init();
        CommunicationChannel = api.Network.RegisterChannel("LevelUPServer").RegisterMessageType(typeof(ServerMessage));
        CommunicationChannel.SetMessageHandler<ServerMessage>(OnChannelMessage);
        Debug.Log("Server Communication Network registered");

        // Enable hardcore death event
        if (Configuration.enableHardcore)
        {
            api.Event.PlayerDeath += (player, damageSource) => ResetPlayerLevels(player, api);
            Debug.Log("Hardcore death event instanciated");
        }
    }

    internal static void PopulateConfigurations(ICoreAPI coreAPI)
    {
        // Base mod Configs
        Configuration.UpdateBaseConfigurations(coreAPI);

        // Levels Configs
        Shared.Instance.levelHunter.PopulateConfiguration(coreAPI);
        Shared.Instance.levelBow.PopulateConfiguration(coreAPI);
        Shared.Instance.levelSlingshot.PopulateConfiguration(coreAPI);
        Shared.Instance.levelKnife.PopulateConfiguration(coreAPI);
        Shared.Instance.levelAxe.PopulateConfiguration(coreAPI);
        Shared.Instance.levelPickaxe.PopulateConfiguration(coreAPI);
        Shared.Instance.levelShovel.PopulateConfiguration(coreAPI);
        Shared.Instance.levelSpear.PopulateConfiguration(coreAPI);
        Shared.Instance.levelHammer.PopulateConfiguration(coreAPI);
        Shared.Instance.levelSword.PopulateConfiguration(coreAPI);
        Shared.Instance.levelShield.PopulateConfiguration(coreAPI);
        Shared.Instance.levelHand.PopulateConfiguration(coreAPI);
        Shared.Instance.levelFarming.PopulateConfiguration(coreAPI);
        Shared.Instance.levelVitality.PopulateConfiguration(coreAPI);
        Shared.Instance.levelMetabolism.PopulateConfiguration(coreAPI);
        Shared.Instance.levelCooking.PopulateConfiguration(coreAPI);
        Shared.Instance.levelPanning.PopulateConfiguration(coreAPI);
        Shared.Instance.levelLeatherArmor.PopulateConfiguration(coreAPI);
        Shared.Instance.levelChainArmor.PopulateConfiguration(coreAPI);
        Shared.Instance.levelBrigandineArmor.PopulateConfiguration(coreAPI);
        Shared.Instance.levelPlateArmor.PopulateConfiguration(coreAPI);
        Shared.Instance.levelScaleArmor.PopulateConfiguration(coreAPI);
        Shared.Instance.levelSmithing.PopulateConfiguration(coreAPI);

        // Class Configs
        Configuration.PopulateClassConfigurations(coreAPI);
    }

    internal void OnChannelMessage(IServerPlayer player, ServerMessage bruteMessage)
    {
        switch (bruteMessage.message)
        {
            case "UpdateLevels": UpdatePlayerLevels(player, api); return;
            case "GetEnabledLevels": GetEnabledLevels(player); return;
        }
    }

    /// <summary>
    /// Refresh all levels
    /// </summary>
    /// <param name="player"></param>
    /// <param name="api"></param>
    public static void UpdatePlayerLevels(IServerPlayer player, ICoreServerAPI api)
    {
        foreach (KeyValuePair<string, bool> keyValuePair in Configuration.EnabledLevels)
        {
            if (keyValuePair.Value)
            {
                string levelType = keyValuePair.Key;

                UpdateLevelAndNotify(api, player, levelType, Experience.GetExperience(player, levelType), true);
            }
        }
    }

    /// <summary>
    /// Refresh a single level
    /// </summary>
    /// <param name="_"></param>
    /// <param name="player"></param>
    /// <param name="levelType"></param>
    /// <param name="exp"></param>
    /// <param name="disableLevelUpNotify"></param>
    public static void UpdateLevelAndNotify(ICoreServerAPI _, IPlayer player, string levelType, ulong exp, bool disableLevelUpNotify = false)
    {
        // Previous exp level, before getting the new experience
        int previousLevel = Configuration.GetLevelByLevelTypeEXP(levelType, (ulong)player.Entity.WatchedAttributes.GetLong($"LevelUP_{levelType}", 0));
        // Actual player level
        int nextLevel = Configuration.GetLevelByLevelTypeEXP(levelType, exp);

        // Check if player leveled up
        if (previousLevel < nextLevel)
        {
            // Check if we want to notify
            if (!disableLevelUpNotify)
            {
                // Notify player
                if (Configuration.enableLevelUpChatMessages)
                    CommunicationChannel.SendPacket(new ServerMessage() { message = $"playerlevelup&{nextLevel}&{levelType}" }, player as IServerPlayer);
            }
            Debug.Log($"{player.PlayerName} reached level {nextLevel} in {levelType}");

            ExperienceEvents.PlayerLeveledUp(player, levelType, exp, nextLevel);
        }

        // This is a heavy formula calculations, we run on task to reduce and prevent lag spikes
        Task.Run(() =>
        {
            // Experience
            player.Entity.WatchedAttributes.SetLong($"LevelUP_{levelType}", (long)exp);
            // Level
            player.Entity.WatchedAttributes.SetInt($"LevelUP_Level_{levelType}", nextLevel);

            // Mining speed
            float miningspeed = Configuration.GetMiningSpeedByLevelTypeLevel(levelType, nextLevel);
            // Check if this levelType has mining speed
            if (miningspeed != -1)
                // Set the mining speed for clients
                player.Entity.WatchedAttributes.SetFloat($"LevelUP_{levelType}_MiningSpeed", miningspeed);

            // Refresh metabolism
            if (levelType == "Metabolism")
            {
                LevelMetabolism.RefreshMaxSaturation(player);
                LevelMetabolism.RefreshSaturationReceiveMultiply(player);
            }
            // Refresh vitality
            else if (levelType == "Vitality")
            {
                LevelVitality.RefreshMaxHealth(player);
            }
        });
    }

    /// <summary>
    /// Refresh a single sub level
    /// </summary>
    /// <param name="_"></param>
    /// <param name="player"></param>
    /// <param name="levelType"></param>
    /// <param name="subLevelType"></param>
    /// <param name="exp"></param>
    /// <param name="disableLevelUpNotify"></param>
    public static void UpdateSubLevelAndNotify(ICoreServerAPI _, IPlayer player, string levelType, string subLevelType, ulong exp, bool disableLevelUpNotify = false)
    {
        // Previous exp level, before getting the new experience
        int previousLevel = Configuration.GetLevelByLevelTypeEXP(levelType, (ulong)player.Entity.WatchedAttributes.GetLong($"LevelUP_{levelType}_Sub_{subLevelType}", 0));
        // Actual player level
        int nextLevel = Configuration.GetLevelByLevelTypeEXP(levelType, exp);

        // Check if player leveled up
        if (previousLevel < nextLevel)
        {
            // Check if we want to notify
            if (!disableLevelUpNotify)
            {
                // Notify player
                if (Configuration.enableLevelUpChatMessages)
                    CommunicationChannel.SendPacket(new ServerMessage() { message = $"playersublevelup&{nextLevel}&{levelType}&{subLevelType}" }, player as IServerPlayer);
            }
            Debug.Log($"{player.PlayerName} reached sub level {nextLevel} in {levelType}/{subLevelType}");

            ExperienceEvents.PlayerLeveledUp(player, levelType + "/" + subLevelType, exp, nextLevel);
        }

        // Experience
        player.Entity.WatchedAttributes.SetLong($"LevelUP_{levelType}_Sub_{subLevelType}", (long)exp);
        // Level
        player.Entity.WatchedAttributes.SetInt($"LevelUP_Level_{levelType}_Sub_{subLevelType}", nextLevel);
    }

    private static void SyncPlayerConfigs(IServerPlayer player)
        => CommunicationChannel.SendPacket(new ServerMessage() { message = $"syncconfig&{Configuration.GenerateClassJsonParameters()}" }, player);

    private static void GetEnabledLevels(IServerPlayer player)
        => CommunicationChannel.SendPacket(new ServerMessage() { message = $"enabledlevels&{JsonSerializer.Serialize(Configuration.EnabledLevels)}" }, player);

    internal static readonly Dictionary<string, long> playersHardcoreDelay = [];
    static public void ResetPlayerLevels(IServerPlayer player, ICoreServerAPI api, double overwriteLose = -1)
    {
        // Hardcore delay cleanup
        List<string> keysToRemove = [];
        foreach (KeyValuePair<string, long> keyValuePair in playersHardcoreDelay)
        {
            if (api.World.Calendar.ElapsedSeconds - keyValuePair.Value < Configuration.hardcorePenaltyDelayInWorldSeconds)
            {
                keysToRemove.Add(keyValuePair.Key);
            }
        }
        foreach (string key in keysToRemove)
        {
            playersHardcoreDelay.Remove(key);
        }


        // Check if delay config is enabled
        if (Configuration.hardcorePenaltyDelayInWorldSeconds > 0 && overwriteLose == -1)
        {
            // Check if player exist in the delay
            if (playersHardcoreDelay.TryGetValue(player.PlayerUID, out long delay))
            {
                // Check if the player is on delay
                if (api.World.Calendar.ElapsedSeconds - delay < Configuration.hardcorePenaltyDelayInWorldSeconds)
                {
                    Debug.LogDebug($"{player.PlayerName} died but hes on the hardcore delay, not losing any exp, seconds to remove from delay: {Configuration.hardcorePenaltyDelayInWorldSeconds - (api.World.Calendar.ElapsedSeconds - delay)}");
                    return;
                }
                // Exist but is not on delay
                else playersHardcoreDelay.Remove(player.PlayerUID);
            }
            // Player does not exist in hardcore delay so we add and continue to reduction
            else playersHardcoreDelay.Add(player.PlayerUID, api.World.Calendar.ElapsedSeconds);
        }

        // Get the lose porcentage
        double losePercentage;
        if (overwriteLose == -1) losePercentage = Configuration.hardcoreLosePercentage;
        else losePercentage = overwriteLose;

        foreach (KeyValuePair<string, bool> keyValuePair in Configuration.EnabledLevels)
        {
            if (keyValuePair.Value)
            {
                string levelType = keyValuePair.Key;

                if (Configuration.hardcoreIgnoreLevelMinimum)
                {
                    ulong exp = Experience.GetExperience(player, levelType);

                    ulong expToReduce = (ulong)Math.Round(exp * losePercentage);
                    Experience.ReduceExperience(player, levelType, expToReduce, true);
                }
                else
                {
                    ulong exp = Experience.GetExperience(player, levelType);
                    int currentLevel = Configuration.GetLevelByLevelTypeEXP(levelType, exp);
                    ulong minExpToStayAtThisLevel = Configuration.GetEXPByLevelTypeLevel(levelType, currentLevel);

                    ulong maxExpToLose = exp - minExpToStayAtThisLevel;

                    ulong expToReduce = Math.Min((ulong)Math.Round(exp * losePercentage), maxExpToLose);
                    Experience.ReduceExperience(player, levelType, expToReduce);
                }
            }
        }

        if (overwriteLose == -1)
        {
            Debug.LogDebug($"{player.PlayerUID} died and lost {(int)((1.0 - losePercentage) * 100)}% progress to the next level");
            if (Configuration.hardcoreMessageWhenDying)
                CommunicationChannel.SendPacket(new ServerMessage() { message = $"playerhardcoredied&{(int)((1.0 - losePercentage) * 100)}" }, player);
        }
    }
}