using System;
using System.Collections.Generic;
using System.Text.Json;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace LevelUP.Server;

public class Instance
{
    static internal ICoreServerAPI api;
    static public IServerNetworkChannel CommunicationChannel { get; private set; }
    private readonly Commands commands = new();

    // Levels
    internal LevelHunter levelHunter = new();
    internal LevelBow levelBow = new();
    internal LevelKnife levelKnife = new();
    internal LevelAxe levelAxe = new();
    internal LevelPickaxe levelPickaxe = new();
    internal LevelShovel levelShovel = new();
    internal LevelSpear levelSpear = new();
    internal LevelHammer levelHammer = new();
    internal LevelSword levelSword = new();
    internal LevelShield levelShield = new();
    internal LevelHand levelHand = new();
    internal LevelFarming levelFarming = new();
    internal LevelVitality levelVitality = new();
    internal LevelCooking levelCooking = new();
    internal LevelPanning levelPanning = new();
    internal LevelLeatherArmor levelLeatherArmor = new();
    internal LevelChainArmor levelChainArmor = new();
    internal LevelBrigandineArmor levelBrigandineArmor = new();
    internal LevelPlateArmor levelPlateArmor = new();
    internal LevelScaleArmor levelScaleArmor = new();
    internal LevelSmithing levelSmithing = new();

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

        // Enable levels
        if (Configuration.enableLevelHunter) levelHunter.Init();
        Configuration.RegisterNewLevel("Hunter", Configuration.enableLevelHunter);
        if (Configuration.enableLevelBow) levelBow.Init();
        Configuration.RegisterNewLevel("Bow", Configuration.enableLevelBow);
        if (Configuration.enableLevelKnife) levelKnife.Init();
        Configuration.RegisterNewLevel("Knife", Configuration.enableLevelKnife);
        if (Configuration.enableLevelAxe) levelAxe.Init();
        Configuration.RegisterNewLevel("Axe", Configuration.enableLevelAxe);
        if (Configuration.enableLevelPickaxe) levelPickaxe.Init();
        Configuration.RegisterNewLevel("Pickaxe", Configuration.enableLevelPickaxe);
        if (Configuration.enableLevelShovel) levelShovel.Init();
        Configuration.RegisterNewLevel("Shovel", Configuration.enableLevelShovel);
        if (Configuration.enableLevelSpear) levelSpear.Init();
        Configuration.RegisterNewLevel("Spear", Configuration.enableLevelSpear);
        if (Configuration.enableLevelHammer) levelHammer.Init();
        Configuration.RegisterNewLevel("Hammer", Configuration.enableLevelHammer);
        if (Configuration.enableLevelSword) levelSword.Init();
        Configuration.RegisterNewLevel("Sword", Configuration.enableLevelSword);
        if (Configuration.enableLevelShield) levelShield.Init();
        Configuration.RegisterNewLevel("Shield", Configuration.enableLevelShield);
        if (Configuration.enableLevelHand) levelHand.Init();
        Configuration.RegisterNewLevel("Hand", Configuration.enableLevelHand);
        if (Configuration.enableLevelFarming) levelFarming.Init();
        Configuration.RegisterNewLevel("Farming", Configuration.enableLevelFarming);
        if (Configuration.enableLevelVitality) levelVitality.Init();
        Configuration.RegisterNewLevel("Vitality", Configuration.enableLevelVitality);
        if (Configuration.enableLevelCooking) levelCooking.Init();
        Configuration.RegisterNewLevel("Cooking", Configuration.enableLevelCooking);
        if (Configuration.enableLevelPanning) levelPanning.Init();
        Configuration.RegisterNewLevel("Panning", Configuration.enableLevelPanning);
        if (Configuration.enableLevelLeatherArmor) levelLeatherArmor.Init();
        Configuration.RegisterNewLevel("LeatherArmor", Configuration.enableLevelLeatherArmor);
        if (Configuration.enableLevelChainArmor) levelChainArmor.Init();
        Configuration.RegisterNewLevel("ChainArmor", Configuration.enableLevelChainArmor);
        if (Configuration.enableLevelBrigandineArmor) levelBrigandineArmor.Init();
        Configuration.RegisterNewLevel("BrigandineArmor", Configuration.enableLevelBrigandineArmor);
        if (Configuration.enableLevelPlateArmor) levelPlateArmor.Init();
        Configuration.RegisterNewLevel("PlateArmor", Configuration.enableLevelPlateArmor);
        if (Configuration.enableLevelScaleArmor) levelScaleArmor.Init();
        Configuration.RegisterNewLevel("ScaleArmor", Configuration.enableLevelScaleArmor);
        if (Configuration.enableLevelSmithing) levelSmithing.Init();
        Configuration.RegisterNewLevel("Smithing", Configuration.enableLevelSmithing);
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
    internal void PopulateConfigurations(ICoreAPI coreAPI)
    {
        // Base mod Configs
        Configuration.UpdateBaseConfigurations(coreAPI);

        // Levels Configs
        levelHunter.PopulateConfiguration(coreAPI);
        levelBow.PopulateConfiguration(coreAPI);
        levelKnife.PopulateConfiguration(coreAPI);
        levelAxe.PopulateConfiguration(coreAPI);
        levelPickaxe.PopulateConfiguration(coreAPI);
        levelShovel.PopulateConfiguration(coreAPI);
        levelSpear.PopulateConfiguration(coreAPI);
        levelHammer.PopulateConfiguration(coreAPI);
        levelSword.PopulateConfiguration(coreAPI);
        levelShield.PopulateConfiguration(coreAPI);
        levelHand.PopulateConfiguration(coreAPI);
        levelFarming.PopulateConfiguration(coreAPI);
        levelVitality.PopulateConfiguration(coreAPI);
        levelCooking.PopulateConfiguration(coreAPI);
        levelPanning.PopulateConfiguration(coreAPI);
        levelLeatherArmor.PopulateConfiguration(coreAPI);
        levelChainArmor.PopulateConfiguration(coreAPI);
        levelBrigandineArmor.PopulateConfiguration(coreAPI);
        levelPlateArmor.PopulateConfiguration(coreAPI);
        levelScaleArmor.PopulateConfiguration(coreAPI);
        levelSmithing.PopulateConfiguration(coreAPI);

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

    public static void UpdatePlayerLevels(IServerPlayer player, ICoreServerAPI api)
    {
        // Hunter Level
        Shared.Instance.UpdateLevelAndNotify(api, player, "Hunter", Experience.GetExperience(player, "Hunter"), true);

        // Bow Level
        Shared.Instance.UpdateLevelAndNotify(api, player, "Bow", Experience.GetExperience(player, "Bow"), true);

        // Axe Level
        Shared.Instance.UpdateLevelAndNotify(api, player, "Knife", Experience.GetExperience(player, "Knife"), true);

        // Axe Level
        Shared.Instance.UpdateLevelAndNotify(api, player, "Axe", Experience.GetExperience(player, "Axe"), true);

        // Pickaxe Level
        Shared.Instance.UpdateLevelAndNotify(api, player, "Pickaxe", Experience.GetExperience(player, "Pickaxe"), true);

        // Shovel Level
        Shared.Instance.UpdateLevelAndNotify(api, player, "Shovel", Experience.GetExperience(player, "Shovel"), true);

        // Spear Level
        Shared.Instance.UpdateLevelAndNotify(api, player, "Spear", Experience.GetExperience(player, "Spear"), true);

        // Hammer Level
        Shared.Instance.UpdateLevelAndNotify(api, player, "Hammer", Experience.GetExperience(player, "Hammer"), true);

        // Sword Level
        Shared.Instance.UpdateLevelAndNotify(api, player, "Sword", Experience.GetExperience(player, "Sword"), true);

        // Shield Level
        Shared.Instance.UpdateLevelAndNotify(api, player, "Shield", Experience.GetExperience(player, "Shield"), true);

        // Hand Level
        Shared.Instance.UpdateLevelAndNotify(api, player, "Hand", Experience.GetExperience(player, "Hand"), true);

        // Farming Level
        Shared.Instance.UpdateLevelAndNotify(api, player, "Farming", Experience.GetExperience(player, "Farming"), true);

        // Cooking Level
        Shared.Instance.UpdateLevelAndNotify(api, player, "Cooking", Experience.GetExperience(player, "Cooking"), true);

        // Panning Level
        Shared.Instance.UpdateLevelAndNotify(api, player, "Panning", Experience.GetExperience(player, "Panning"), true);

        // Vitality Level
        Shared.Instance.UpdateLevelAndNotify(api, player, "Vitality", Experience.GetExperience(player, "Vitality"), true);

        // Leather Armor Level
        Shared.Instance.UpdateLevelAndNotify(api, player, "LeatherArmor", Experience.GetExperience(player, "LeatherArmor"), true);

        // Chain Armor Level
        Shared.Instance.UpdateLevelAndNotify(api, player, "ChainArmor", Experience.GetExperience(player, "ChainArmor"), true);

        // Brigandine Armor Level
        Shared.Instance.UpdateLevelAndNotify(api, player, "BrigandineArmor", Experience.GetExperience(player, "BrigandineArmor"), true);

        // Plate Armor Level
        Shared.Instance.UpdateLevelAndNotify(api, player, "PlateArmor", Experience.GetExperience(player, "PlateArmor"), true);

        // Scale Armor Level
        Shared.Instance.UpdateLevelAndNotify(api, player, "ScaleArmor", Experience.GetExperience(player, "ScaleArmor"), true);

        // Smithing Level
        Shared.Instance.UpdateLevelAndNotify(api, player, "Smithing", Experience.GetExperience(player, "Smithing"), true);
    }

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