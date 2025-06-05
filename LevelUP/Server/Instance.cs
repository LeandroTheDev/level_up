using System;
using System.Collections.Generic;
using System.Text.Json;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace LevelUP.Server;

class Instance
{
    public ICoreServerAPI api;
    static public IServerNetworkChannel communicationChannel;
    private readonly Commands commands = new();

    // Levels
    public LevelHunter levelHunter = new();
    public LevelBow levelBow = new();
    public LevelKnife levelKnife = new();
    public LevelAxe levelAxe = new();
    public LevelPickaxe levelPickaxe = new();
    public LevelShovel levelShovel = new();
    public LevelSpear levelSpear = new();
    public LevelHammer levelHammer = new();
    public LevelSword levelSword = new();
    public LevelShield levelShield = new();
    public LevelHand levelHand = new();
    public LevelFarming levelFarming = new();
    public LevelVitality levelVitality = new();
    public LevelCooking levelCooking = new();
    public LevelPanning levelPanning = new();
    public LevelLeatherArmor levelLeatherArmor = new();
    public LevelChainArmor levelChainArmor = new();
    public LevelBrigandineArmor levelBrigandineArmor = new();
    public LevelPlateArmor levelPlateArmor = new();
    public LevelScaleArmor levelScaleArmor = new();
    public LevelSmithing levelSmithing = new();

    public void Init(ICoreServerAPI serverAPI)
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
        if (Configuration.enableLevelHunter) levelHunter.Init(this);
        if (Configuration.enableLevelBow) levelBow.Init(this);
        if (Configuration.enableLevelKnife) levelKnife.Init(this);
        if (Configuration.enableLevelAxe) levelAxe.Init(this);
        if (Configuration.enableLevelPickaxe) levelPickaxe.Init(this);
        if (Configuration.enableLevelShovel) levelShovel.Init(this);
        if (Configuration.enableLevelSpear) levelSpear.Init(this);
        if (Configuration.enableLevelHammer) levelHammer.Init(this);
        if (Configuration.enableLevelSword) levelSword.Init(this);
        if (Configuration.enableLevelShield) levelShield.Init(this);
        if (Configuration.enableLevelHand) levelHand.Init(this);
        if (Configuration.enableLevelFarming) levelFarming.Init(this);
        if (Configuration.enableLevelVitality) levelVitality.Init(this);
        if (Configuration.enableLevelCooking) levelCooking.Init(this);
        if (Configuration.enableLevelPanning) levelPanning.Init(this);
        if (Configuration.enableLevelLeatherArmor) levelLeatherArmor.Init(this);
        if (Configuration.enableLevelChainArmor) levelChainArmor.Init(this);
        if (Configuration.enableLevelBrigandineArmor) levelBrigandineArmor.Init(this);
        if (Configuration.enableLevelPlateArmor) levelPlateArmor.Init(this);
        if (Configuration.enableLevelScaleArmor) levelScaleArmor.Init(this);
        if (Configuration.enableLevelSmithing) levelSmithing.Init(this);
        Debug.Log("Server Levels instanciated");

        // Register commands
        commands.Init(this);
        communicationChannel = api.Network.RegisterChannel("LevelUPServer").RegisterMessageType(typeof(ServerMessage));
        communicationChannel.SetMessageHandler<ServerMessage>(OnChannelMessage);
        Debug.Log("Server Communication Network registered");

        // Enable hardcore death event
        if (Configuration.enableHardcore)
        {
            api.Event.PlayerDeath += (player, damageSource) => ResetPlayerLevels(player, api);
            Debug.Log("Hardcore death event instanciated");
        }
    }
    public void PopulateConfigurations(ICoreAPI coreAPI)
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

    public void OnChannelMessage(IServerPlayer player, ServerMessage bruteMessage)
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
    {
        Dictionary<string, bool> enabledLevels = [];
        enabledLevels.Add("Hunter", Configuration.enableLevelHunter);
        enabledLevels.Add("Bow", Configuration.enableLevelBow);
        enabledLevels.Add("Knife", Configuration.enableLevelKnife);
        enabledLevels.Add("Spear", Configuration.enableLevelSpear);
        enabledLevels.Add("Hammer", Configuration.enableLevelHammer);
        enabledLevels.Add("Axe", Configuration.enableLevelAxe);
        enabledLevels.Add("Pickaxe", Configuration.enableLevelPickaxe);
        enabledLevels.Add("Shovel", Configuration.enableLevelShovel);
        enabledLevels.Add("Sword", Configuration.enableLevelSword);
        enabledLevels.Add("Shield", Configuration.enableLevelShield);
        enabledLevels.Add("Hand", Configuration.enableLevelHand);
        enabledLevels.Add("Farming", Configuration.enableLevelFarming);
        enabledLevels.Add("Cooking", Configuration.enableLevelCooking);
        enabledLevels.Add("Panning", Configuration.enableLevelPanning);
        enabledLevels.Add("Vitality", Configuration.enableLevelVitality);
        enabledLevels.Add("LeatherArmor", Configuration.enableLevelLeatherArmor);
        enabledLevels.Add("ChainArmor", Configuration.enableLevelChainArmor);
        enabledLevels.Add("BrigandineArmor", Configuration.enableLevelBrigandineArmor);
        enabledLevels.Add("PlateArmor", Configuration.enableLevelPlateArmor);
        enabledLevels.Add("ScaleArmor", Configuration.enableLevelScaleArmor);
        enabledLevels.Add("Smithing", Configuration.enableLevelSmithing);

        // Sending the configurations to the player
        communicationChannel.SendPacket(new ServerMessage() { message = $"enabledlevels&{JsonSerializer.Serialize(enabledLevels)}" }, player);
    }

    public static readonly Dictionary<string, long> playersHardcoreDelay = [];
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

        { // Hunter
            ulong exp = Experience.GetExperience(player, "Hunter");
            ulong expToReduce = (ulong)Math.Round(exp * losePercentage);
            while (true)
            {
                if (expToReduce <= 10) break;

                if (Experience.ReduceExperience(player, "Hunter", expToReduce)) break;
                else expToReduce /= 2;
            }
        }
        { // Bow
            ulong exp = Experience.GetExperience(player, "Bow");
            ulong expToReduce = (ulong)Math.Round(exp * losePercentage);
            while (true)
            {
                if (expToReduce <= 10) break;

                if (Experience.ReduceExperience(player, "Bow", expToReduce)) break;
                else expToReduce /= 2;
            }
        }
        { // Knife
            ulong exp = Experience.GetExperience(player, "Knife");
            ulong expToReduce = (ulong)Math.Round(exp * losePercentage);
            while (true)
            {
                if (expToReduce <= 10) break;

                if (Experience.ReduceExperience(player, "Knife", expToReduce)) break;
                else expToReduce /= 2;
            }
        }
        { // Axe
            ulong exp = Experience.GetExperience(player, "Axe");
            ulong expToReduce = (ulong)Math.Round(exp * losePercentage);
            while (true)
            {
                if (expToReduce <= 10) break;

                if (Experience.ReduceExperience(player, "Axe", expToReduce)) break;
                else expToReduce /= 2;
            }
        }
        { // Pickaxe
            ulong exp = Experience.GetExperience(player, "Pickaxe");
            ulong expToReduce = (ulong)Math.Round(exp * losePercentage);
            while (true)
            {
                if (expToReduce <= 10) break;

                if (Experience.ReduceExperience(player, "Pickaxe", expToReduce)) break;
                else expToReduce /= 2;
            }
        }
        { // Shovel
            ulong exp = Experience.GetExperience(player, "Shovel");
            ulong expToReduce = (ulong)Math.Round(exp * losePercentage);
            while (true)
            {
                if (expToReduce <= 10) break;

                if (Experience.ReduceExperience(player, "Shovel", expToReduce)) break;
                else expToReduce /= 2;
            }
        }
        { // Spear
            ulong exp = Experience.GetExperience(player, "Spear");
            ulong expToReduce = (ulong)Math.Round(exp * losePercentage);
            while (true)
            {
                if (expToReduce <= 10) break;

                if (Experience.ReduceExperience(player, "Spear", expToReduce)) break;
                else expToReduce /= 2;
            }
        }
        { // Hammer
            ulong exp = Experience.GetExperience(player, "Hammer");
            ulong expToReduce = (ulong)Math.Round(exp * losePercentage);
            while (true)
            {
                if (expToReduce <= 10) break;

                if (Experience.ReduceExperience(player, "Hammer", expToReduce)) break;
                else expToReduce /= 2;
            }
        }
        { // Sword
            ulong exp = Experience.GetExperience(player, "Sword");
            ulong expToReduce = (ulong)Math.Round(exp * losePercentage);
            while (true)
            {
                if (expToReduce <= 10) break;

                if (Experience.ReduceExperience(player, "Sword", expToReduce)) break;
                else expToReduce /= 2;
            }
        }
        { // Shield
            ulong exp = Experience.GetExperience(player, "Shield");
            ulong expToReduce = (ulong)Math.Round(exp * losePercentage);
            while (true)
            {
                if (expToReduce <= 10) break;

                if (Experience.ReduceExperience(player, "Shield", expToReduce)) break;
                else expToReduce /= 2;
            }
        }
        { // Hand
            ulong exp = Experience.GetExperience(player, "Hand");
            ulong expToReduce = (ulong)Math.Round(exp * losePercentage);
            while (true)
            {
                if (expToReduce <= 10) break;

                if (Experience.ReduceExperience(player, "Hand", expToReduce)) break;
                else expToReduce /= 2;
            }
        }
        { // Farming
            ulong exp = Experience.GetExperience(player, "Farming");
            ulong expToReduce = (ulong)Math.Round(exp * losePercentage);
            while (true)
            {
                if (expToReduce <= 10) break;

                if (Experience.ReduceExperience(player, "Farming", expToReduce)) break;
                else expToReduce /= 2;
            }
        }
        { // Cooking
            ulong exp = Experience.GetExperience(player, "Cooking");
            ulong expToReduce = (ulong)Math.Round(exp * losePercentage);
            while (true)
            {
                if (expToReduce <= 10) break;

                if (Experience.ReduceExperience(player, "Cooking", expToReduce)) break;
                else expToReduce /= 2;
            }
        }
        { // Panning
            ulong exp = Experience.GetExperience(player, "Panning");
            ulong expToReduce = (ulong)Math.Round(exp * losePercentage);
            while (true)
            {
                if (expToReduce <= 10) break;

                if (Experience.ReduceExperience(player, "Panning", expToReduce)) break;
                else expToReduce /= 2;
            }
        }
        { // Vitality
            ulong exp = Experience.GetExperience(player, "Vitality");
            ulong expToReduce = (ulong)Math.Round(exp * losePercentage);
            while (true)
            {
                if (expToReduce <= 10) break;

                if (Experience.ReduceExperience(player, "Vitality", expToReduce)) break;
                else expToReduce /= 2;
            }
        }
        { // LeatherArmor
            ulong exp = Experience.GetExperience(player, "LeatherArmor");
            ulong expToReduce = (ulong)Math.Round(exp * losePercentage);
            while (true)
            {
                if (expToReduce <= 10) break;

                if (Experience.ReduceExperience(player, "LeatherArmor", expToReduce)) break;
                else expToReduce /= 2;
            }
        }
        { // ChainArmor
            ulong exp = Experience.GetExperience(player, "ChainArmor");
            ulong expToReduce = (ulong)Math.Round(exp * losePercentage);
            while (true)
            {
                if (expToReduce <= 10) break;

                if (Experience.ReduceExperience(player, "ChainArmor", expToReduce)) break;
                else expToReduce /= 2;
            }
        }
        { // BrigandineArmor
            ulong exp = Experience.GetExperience(player, "BrigandineArmor");
            ulong expToReduce = (ulong)Math.Round(exp * losePercentage);
            while (true)
            {
                if (expToReduce <= 10) break;

                if (Experience.ReduceExperience(player, "BrigandineArmor", expToReduce)) break;
                else expToReduce /= 2;
            }
        }
        { // PlateArmor
            ulong exp = Experience.GetExperience(player, "PlateArmor");
            ulong expToReduce = (ulong)Math.Round(exp * losePercentage);
            while (true)
            {
                if (expToReduce <= 10) break;

                if (Experience.ReduceExperience(player, "PlateArmor", expToReduce)) break;
                else expToReduce /= 2;
            }
        }
        { // ScaleArmor
            ulong exp = Experience.GetExperience(player, "ScaleArmor");
            ulong expToReduce = (ulong)Math.Round(exp * losePercentage);
            while (true)
            {
                if (expToReduce <= 10) break;

                if (Experience.ReduceExperience(player, "ScaleArmor", expToReduce)) break;
                else expToReduce /= 2;
            }
        }
        { // Smithing
            ulong exp = Experience.GetExperience(player, "Smithing");
            ulong expToReduce = (ulong)Math.Round(exp * losePercentage);
            while (true)
            {
                if (expToReduce <= 10) break;

                if (Experience.ReduceExperience(player, "Smithing", expToReduce)) break;
                else expToReduce /= 2;
            }
        }

        if (overwriteLose == -1)
        {
            Debug.LogDebug($"{player.PlayerUID} died and lost {(int)((1.0 - losePercentage) * 100)}% of all experience");
            if (Configuration.hardcoreMessageWhenDying)
                communicationChannel.SendPacket(new ServerMessage() { message = $"playerhardcoredied&{(int)((1.0 - losePercentage) * 100)}" }, player);
        }
    }

}