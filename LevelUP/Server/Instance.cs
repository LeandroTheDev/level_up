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
            int currentLevel = Configuration.HunterGetLevelByEXP(exp);
            ulong minExpToStayAtThisLevel = Configuration.HunterGetExpForLevel(currentLevel);

            ulong maxExpToLose = exp - minExpToStayAtThisLevel;

            ulong expToReduce = Math.Min((ulong)Math.Round(exp * losePercentage), maxExpToLose);
            Experience.ReduceExperience(player, "Hunter", expToReduce);
        }
        { // Bow
            ulong exp = Experience.GetExperience(player, "Bow");
            int currentLevel = Configuration.BowGetLevelByEXP(exp);
            ulong minExpToStayAtThisLevel = Configuration.BowGetExpForLevel(currentLevel);

            ulong maxExpToLose = exp - minExpToStayAtThisLevel;

            ulong expToReduce = Math.Min((ulong)Math.Round(exp * losePercentage), maxExpToLose);
            Experience.ReduceExperience(player, "Bow", expToReduce);
        }
        { // Knife
            ulong exp = Experience.GetExperience(player, "Knife");
            int currentLevel = Configuration.KnifeGetLevelByEXP(exp);
            ulong minExpToStayAtThisLevel = Configuration.KnifeGetExpForLevel(currentLevel);

            ulong maxExpToLose = exp - minExpToStayAtThisLevel;

            ulong expToReduce = Math.Min((ulong)Math.Round(exp * losePercentage), maxExpToLose);
            Experience.ReduceExperience(player, "Knife", expToReduce);
        }
        { // Axe
            ulong exp = Experience.GetExperience(player, "Axe");
            int currentLevel = Configuration.AxeGetLevelByEXP(exp);
            ulong minExpToStayAtThisLevel = Configuration.AxeGetExpForLevel(currentLevel);

            ulong maxExpToLose = exp - minExpToStayAtThisLevel;

            ulong expToReduce = Math.Min((ulong)Math.Round(exp * losePercentage), maxExpToLose);
            Experience.ReduceExperience(player, "Axe", expToReduce);
        }
        { // Pickaxe
            ulong exp = Experience.GetExperience(player, "Pickaxe");
            int currentLevel = Configuration.PickaxeGetLevelByEXP(exp);
            ulong minExpToStayAtThisLevel = Configuration.PickaxeGetExpForLevel(currentLevel);

            ulong maxExpToLose = exp - minExpToStayAtThisLevel;

            ulong expToReduce = Math.Min((ulong)Math.Round(exp * losePercentage), maxExpToLose);
            Experience.ReduceExperience(player, "Pickaxe", expToReduce);
        }
        { // Shovel
            ulong exp = Experience.GetExperience(player, "Shovel");
            int currentLevel = Configuration.ShovelGetLevelByEXP(exp);
            ulong minExpToStayAtThisLevel = Configuration.ShovelGetExpForLevel(currentLevel);

            ulong maxExpToLose = exp - minExpToStayAtThisLevel;

            ulong expToReduce = Math.Min((ulong)Math.Round(exp * losePercentage), maxExpToLose);
            Experience.ReduceExperience(player, "Shovel", expToReduce);
        }
        { // Spear
            ulong exp = Experience.GetExperience(player, "Spear");
            int currentLevel = Configuration.SpearGetLevelByEXP(exp);
            ulong minExpToStayAtThisLevel = Configuration.SpearGetExpForLevel(currentLevel);

            ulong maxExpToLose = exp - minExpToStayAtThisLevel;

            ulong expToReduce = Math.Min((ulong)Math.Round(exp * losePercentage), maxExpToLose);
            Experience.ReduceExperience(player, "Spear", expToReduce);
        }
        { // Hammer
            ulong exp = Experience.GetExperience(player, "Hammer");
            int currentLevel = Configuration.HammerGetLevelByEXP(exp);
            ulong minExpToStayAtThisLevel = Configuration.HammerGetExpForLevel(currentLevel);

            ulong maxExpToLose = exp - minExpToStayAtThisLevel;

            ulong expToReduce = Math.Min((ulong)Math.Round(exp * losePercentage), maxExpToLose);
            Experience.ReduceExperience(player, "Hammer", expToReduce);
        }
        { // Sword
            ulong exp = Experience.GetExperience(player, "Sword");
            int currentLevel = Configuration.SwordGetLevelByEXP(exp);
            ulong minExpToStayAtThisLevel = Configuration.SwordGetExpForLevel(currentLevel);

            ulong maxExpToLose = exp - minExpToStayAtThisLevel;

            ulong expToReduce = Math.Min((ulong)Math.Round(exp * losePercentage), maxExpToLose);
            Experience.ReduceExperience(player, "Sword", expToReduce);
        }
        { // Shield
            ulong exp = Experience.GetExperience(player, "Shield");
            int currentLevel = Configuration.ShieldGetLevelByEXP(exp);
            ulong minExpToStayAtThisLevel = Configuration.ShieldGetExpForLevel(currentLevel);

            ulong maxExpToLose = exp - minExpToStayAtThisLevel;

            ulong expToReduce = Math.Min((ulong)Math.Round(exp * losePercentage), maxExpToLose);
            Experience.ReduceExperience(player, "Shield", expToReduce);
        }
        { // Hand
            ulong exp = Experience.GetExperience(player, "Hand");
            int currentLevel = Configuration.HandGetLevelByEXP(exp);
            ulong minExpToStayAtThisLevel = Configuration.HandGetExpForLevel(currentLevel);

            ulong maxExpToLose = exp - minExpToStayAtThisLevel;

            ulong expToReduce = Math.Min((ulong)Math.Round(exp * losePercentage), maxExpToLose);
            Experience.ReduceExperience(player, "Hand", expToReduce);
        }
        { // Farming
            ulong exp = Experience.GetExperience(player, "Farming");
            int currentLevel = Configuration.FarmingGetLevelByEXP(exp);
            ulong minExpToStayAtThisLevel = Configuration.FarmingGetExpForLevel(currentLevel);

            ulong maxExpToLose = exp - minExpToStayAtThisLevel;

            ulong expToReduce = Math.Min((ulong)Math.Round(exp * losePercentage), maxExpToLose);
            Experience.ReduceExperience(player, "Farming", expToReduce);
        }
        { // Cooking
            ulong exp = Experience.GetExperience(player, "Cooking");
            int currentLevel = Configuration.CookingGetLevelByEXP(exp);
            ulong minExpToStayAtThisLevel = Configuration.CookingGetExpForLevel(currentLevel);

            ulong maxExpToLose = exp - minExpToStayAtThisLevel;

            ulong expToReduce = Math.Min((ulong)Math.Round(exp * losePercentage), maxExpToLose);
            Experience.ReduceExperience(player, "Cooking", expToReduce);
        }
        { // Panning
            ulong exp = Experience.GetExperience(player, "Panning");
            int currentLevel = Configuration.PanningGetLevelByEXP(exp);
            ulong minExpToStayAtThisLevel = Configuration.PanningGetExpForLevel(currentLevel);

            ulong maxExpToLose = exp - minExpToStayAtThisLevel;

            ulong expToReduce = Math.Min((ulong)Math.Round(exp * losePercentage), maxExpToLose);
            Experience.ReduceExperience(player, "Panning", expToReduce);
        }
        { // Vitality
            ulong exp = Experience.GetExperience(player, "Vitality");
            int currentLevel = Configuration.VitalityGetLevelByEXP(exp);
            ulong minExpToStayAtThisLevel = Configuration.VitalityGetExpForLevel(currentLevel);

            ulong maxExpToLose = exp - minExpToStayAtThisLevel;

            ulong expToReduce = Math.Min((ulong)Math.Round(exp * losePercentage), maxExpToLose);
            Experience.ReduceExperience(player, "Vitality", expToReduce);
        }
        { // LeatherArmor
            ulong exp = Experience.GetExperience(player, "LeatherArmor");
            int currentLevel = Configuration.LeatherArmorGetLevelByEXP(exp);
            ulong minExpToStayAtThisLevel = Configuration.LeatherArmorGetExpForLevel(currentLevel);

            ulong maxExpToLose = exp - minExpToStayAtThisLevel;

            ulong expToReduce = Math.Min((ulong)Math.Round(exp * losePercentage), maxExpToLose);
            Experience.ReduceExperience(player, "LeatherArmor", expToReduce);
        }
        { // ChainArmor
            ulong exp = Experience.GetExperience(player, "ChainArmor");
            int currentLevel = Configuration.ChainArmorGetLevelByEXP(exp);
            ulong minExpToStayAtThisLevel = Configuration.ChainArmorGetExpForLevel(currentLevel);

            ulong maxExpToLose = exp - minExpToStayAtThisLevel;

            ulong expToReduce = Math.Min((ulong)Math.Round(exp * losePercentage), maxExpToLose);
            Experience.ReduceExperience(player, "ChainArmor", expToReduce);
        }
        { // BrigandineArmor
            ulong exp = Experience.GetExperience(player, "BrigandineArmor");
            int currentLevel = Configuration.BrigandineArmorGetLevelByEXP(exp);
            ulong minExpToStayAtThisLevel = Configuration.BrigandineArmorGetExpForLevel(currentLevel);

            ulong maxExpToLose = exp - minExpToStayAtThisLevel;

            ulong expToReduce = Math.Min((ulong)Math.Round(exp * losePercentage), maxExpToLose);
            Experience.ReduceExperience(player, "BrigandineArmor", expToReduce);
        }
        { // PlateArmor
            ulong exp = Experience.GetExperience(player, "PlateArmor");
            int currentLevel = Configuration.PlateArmorGetLevelByEXP(exp);
            ulong minExpToStayAtThisLevel = Configuration.PlateArmorGetExpForLevel(currentLevel);

            ulong maxExpToLose = exp - minExpToStayAtThisLevel;

            ulong expToReduce = Math.Min((ulong)Math.Round(exp * losePercentage), maxExpToLose);
            Experience.ReduceExperience(player, "PlateArmor", expToReduce);
        }
        { // ScaleArmor
            ulong exp = Experience.GetExperience(player, "ScaleArmor");
            int currentLevel = Configuration.ScaleArmorGetLevelByEXP(exp);
            ulong minExpToStayAtThisLevel = Configuration.ScaleArmorGetExpForLevel(currentLevel);

            ulong maxExpToLose = exp - minExpToStayAtThisLevel;

            ulong expToReduce = Math.Min((ulong)Math.Round(exp * losePercentage), maxExpToLose);
            Experience.ReduceExperience(player, "ScaleArmor", expToReduce);
        }
        { // Smithing
            ulong exp = Experience.GetExperience(player, "Smithing");
            int currentLevel = Configuration.SmithingGetLevelByEXP(exp);
            ulong minExpToStayAtThisLevel = Configuration.SmithingGetExpForLevel(currentLevel);

            ulong maxExpToLose = exp - minExpToStayAtThisLevel;

            ulong expToReduce = Math.Min((ulong)Math.Round(exp * losePercentage), maxExpToLose);
            Experience.ReduceExperience(player, "Smithing", expToReduce);
        }

        if (overwriteLose == -1)
        {
            Debug.LogDebug($"{player.PlayerUID} died and lost {(int)((1.0 - losePercentage) * 100)}% progress to the next level");
            if (Configuration.hardcoreMessageWhenDying)
                communicationChannel.SendPacket(new ServerMessage() { message = $"playerhardcoredied&{(int)((1.0 - losePercentage) * 100)}" }, player);
        }
    }

}