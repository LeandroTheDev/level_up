using System;
using System.Collections.Generic;
using System.Text.Json;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using Vintagestory.API.Util;

namespace LevelUP.Server;

class Instance
{
    public ICoreServerAPI api;
    public IServerNetworkChannel compatibilityChannel;
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

    public void Init(ICoreServerAPI serverAPI)
    {
        api = serverAPI;
        // Update player levels when player enters in the world
        api.Event.PlayerNowPlaying += (IServerPlayer player) => UpdatePlayerLevels(player, api);

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
        Debug.Log("Server Levels instanciated");

        // Register commands
        commands.Init(this);

        // Check for channel communication
        if (!Configuration.disableServerChannel)
        {
            compatibilityChannel = api.Network.RegisterChannel("LevelUP").RegisterMessageType(typeof(string));
            compatibilityChannel.SetMessageHandler<string>(OnExperienceEarned);
            Debug.Log("Server Compatibility Network registered, this is unsafe");
        }
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

        // Class Configs
        Configuration.PopulateClassConfigurations(coreAPI);
    }

    public void OnExperienceEarned(IServerPlayer player, string bruteMessage)
    {
        // Translate aguments if exist
        Dictionary<string, object> arguments = [];
        string message;
        if (bruteMessage.Contains('&'))
        {
            string[] messages = bruteMessage.Split('&');

            message = messages[0];

            // Remove message context from the arguments
            string[] argumentsArray = new string[messages.Length - 1];
            Array.Copy(messages, 1, argumentsArray, 0, messages.Length - 1);

            // Swipe arguments
            foreach (string value in argumentsArray)
            {
                string[] keyValue = value.Split('=');
                // Add argument to the list
                if (keyValue.Length == 2) arguments[keyValue[0]] = keyValue[1].ToString();
                else Debug.Log($"ERROR OnClientMessage: invalid argument quantity: {keyValue.Length}");
            }
        }
        else message = bruteMessage;

        // Lan treatment
        if (!Configuration.disableServerChannel && arguments.TryGetValue("lanplayername", out object lanName))
        {
            IPlayer[] playersOnline = player.Entity.World.AllOnlinePlayers;
            // Swipe all lan players to find the actual to earn experience
            foreach (IPlayer lanPlayer in playersOnline)
                if (lanPlayer.PlayerName == lanName as string) player = lanPlayer as IServerPlayer;

            // If lanName cannot find any player, cancel the function
            if (lanName as string != player.PlayerName)
            {
                Debug.Log($"Channel Error: someone send a packet, but the lan name does not exist in online players: {lanName}, the function has been canceled");
                return;
            }
            ;
        }

        switch (message)
        {
            #region hit
            case "Increase_Knife_Hit": IncreaseExp(player, "Knife", "Hit"); return;
            case "Increase_Bow_Hit": IncreaseExp(player, "Bow", "Hit"); return;
            case "Increase_Axe_Hit": IncreaseExp(player, "Axe", "Hit"); return;
            case "Increase_Pickaxe_Hit": IncreaseExp(player, "Pickaxe", "Hit"); return;
            case "Increase_Shovel_Hit": IncreaseExp(player, "Shovel", "Hit"); return;
            case "Increase_Spear_Hit": IncreaseExp(player, "Spear", "Hit"); return;
            case "Increase_Spear_Hit_Throw": IncreaseExp(player, "Spear", "Hit_Throw"); return;
            case "Increase_Hammer_Hit": IncreaseExp(player, "Hammer", "Hit"); return;
            case "Increase_Sword_Hit": IncreaseExp(player, "Sword", "Hit"); return;
            case "Increase_Shield_Hit": IncreaseExp(player, "Shield", "Hit"); return;
            case "Increase_Hand_Hit": IncreaseExp(player, "Hand", "Hit"); return;
            case "Increase_Vitality_Hit": IncreaseExp(player, "Vitality", "Hit", arguments["forceexp"].ToString().ToInt()); return;
            case "Increase_LeatherArmor_Hit": IncreaseExp(player, "LeatherArmor", "Hit", arguments["forceexp"].ToString().ToInt()); return;
            case "Increase_ChainArmor_Hit": IncreaseExp(player, "ChainArmor", "Hit", arguments["forceexp"].ToString().ToInt()); return;
            case "Increase_BrigandineArmor_Hit": IncreaseExp(player, "BrigandineArmor", "Hit", arguments["forceexp"].ToString().ToInt()); return;
            case "Increase_PlateArmor_Hit": IncreaseExp(player, "PlateArmor", "Hit", arguments["forceexp"].ToString().ToInt()); return;
            case "Increase_ScaleArmor_Hit": IncreaseExp(player, "ScaleArmor", "Hit", arguments["forceexp"].ToString().ToInt()); return;
            #endregion
            #region breaking
            case "Block_Breaked_Axe": IncreaseExp(player, "Axe", "Breaking"); return;
            case "Block_Breaked_Pickaxe": IncreaseExp(player, "Pickaxe", "Breaking"); return;
            case "Block_Breaked_Shovel": IncreaseExp(player, "Shovel", "Breaking"); return;
            #endregion
            #region harvesting
            case "Tree_Breaked_Axe": IncreaseExp(player, "Axe", "Chop_Tree"); return;
            case "Knife_Harvest_Entity": IncreaseExp(player, "Knife", "Harvest"); return;
            case "Soil_Till": IncreaseExp(player, "Farming", "Till"); return;
            case "Farming_Harvest": IncreaseExp(player, "Farming", "Harvest", arguments["forceexp"].ToString().ToInt()); return;
            #endregion
            #region crafting
            case "Cooking_Finished": IncreaseExp(player, "Cooking", "Cooking_Finished", arguments["forceexp"].ToString().ToInt()); return;
            case "Panning_Finished": IncreaseExp(player, "Panning", "Panning_Finished"); return;
                #endregion
        }

    }

    public void OnChannelMessage(IServerPlayer player, ServerMessage bruteMessage)
    {
        switch (bruteMessage.message)
        {
            case "UpdateLevels": UpdatePlayerLevels(player, api); return;
            case "GetEnabledLevels": GetEnabledLevels(player); return;
        }
    }

    private void IncreaseExp(IServerPlayer player, string levelType, string reason, int forceexp = 0)
    {
        float experienceMultiplierCompatibility = player.Entity.Attributes.GetFloat("LevelUP_Server_Instance_ExperienceMultiplier_IncreaseExp");
        /// Automatically gets the level from levelType parameter
        Dictionary<string, ulong> GetSavedLevels()
        {
            byte[] dataBytes = api.WorldManager.SaveGame.GetData($"LevelUPData_{levelType}");
            string data = dataBytes == null ? "{}" : SerializerUtil.Deserialize<string>(dataBytes);
            return JsonSerializer.Deserialize<Dictionary<string, ulong>>(data);
        }

        void SaveLevels(Dictionary<string, ulong> levels)
        {
            api.WorldManager.SaveGame.StoreData($"LevelUPData_{levelType}", JsonSerializer.Serialize(levels));
        }

        #region bow
        // Hit
        if (levelType == "Bow" && reason == "Hit")
        {
            // Get levels
            var levels = GetSavedLevels();
            ulong earnedExp = (ulong)(Configuration.ExpPerHitBow + (Configuration.ExpPerHitBow * experienceMultiplierCompatibility));
            // Increasing by player class
            earnedExp = (ulong)Math.Round(earnedExp * Configuration.GetEXPMultiplyByClassAndLevelType(player.Entity.WatchedAttributes.GetString("characterClass"), "Bow"));
            // Minium exp earned is 1
            if (earnedExp <= 0) earnedExp = (ulong)Configuration.minimumEXPEarned;
            ulong exp = levels.GetValueOrDefault<string, ulong>(player.PlayerUID, 0) + earnedExp;
            if (Configuration.BowIsMaxLevel(exp)) return;

            // Increment
            levels[player.PlayerUID] = exp;
            // Save it
            SaveLevels(levels);
            // Update it
            Shared.Instance.UpdateLevelAndNotify(api, player, levelType, exp);
            if (Configuration.enableLevelUpExperienceServerLog)
                Debug.Log($"{player.PlayerName} earned {earnedExp} exp with {levelType} by {reason}, actual: {exp}");
        }
        #endregion
        #region knife
        // Hit
        if (levelType == "Knife" && reason == "Hit")
        {
            // Get levels
            var levels = GetSavedLevels();
            ulong earnedExp = (ulong)(Configuration.ExpPerHitKnife + (Configuration.ExpPerHitKnife * experienceMultiplierCompatibility));
            // Increasing by player class
            earnedExp = (ulong)Math.Round(earnedExp * Configuration.GetEXPMultiplyByClassAndLevelType(player.Entity.WatchedAttributes.GetString("characterClass"), "Knife"));
            // Minium exp earned is 1
            if (earnedExp <= 0) earnedExp = (ulong)Configuration.minimumEXPEarned;
            ulong exp = levels.GetValueOrDefault<string, ulong>(player.PlayerUID, 0) + earnedExp;
            if (Configuration.KnifeIsMaxLevel(exp)) return;

            // Increment
            levels[player.PlayerUID] = exp;
            // Save it
            SaveLevels(levels);
            // Update it
            Shared.Instance.UpdateLevelAndNotify(api, player, levelType, exp);
            if (Configuration.enableLevelUpExperienceServerLog)
                Debug.Log($"{player.PlayerName} earned {earnedExp} exp with {levelType} by {reason}, actual: {exp}");
        }
        // Harvest
        else if (levelType == "Knife" && reason == "Harvest")
        {
            // Get levels
            var levels = GetSavedLevels();
            ulong earnedExp = (ulong)(Configuration.ExpPerHarvestKnife + (Configuration.ExpPerHarvestKnife * experienceMultiplierCompatibility));
            // Increasing by player class
            earnedExp = (ulong)Math.Round(earnedExp * Configuration.GetEXPMultiplyByClassAndLevelType(player.Entity.WatchedAttributes.GetString("characterClass"), "Bow"));
            // Minium exp earned is 1
            if (earnedExp <= 0) earnedExp = (ulong)Configuration.minimumEXPEarned;
            ulong exp = levels.GetValueOrDefault<string, ulong>(player.PlayerUID, 0) + earnedExp;
            if (Configuration.KnifeIsMaxLevel(exp)) return;

            // Increment
            levels[player.PlayerUID] = exp;
            // Save it
            SaveLevels(levels);
            // Update it
            Shared.Instance.UpdateLevelAndNotify(api, player, levelType, exp);
            if (Configuration.enableLevelUpExperienceServerLog)
                Debug.Log($"{player.PlayerName} earned {earnedExp} exp with {levelType} by {reason}, actual: {exp}");
        }
        #endregion
        #region axe
        // Hit
        if (levelType == "Axe" && reason == "Hit")
        {
            // Get levels
            var levels = GetSavedLevels();
            ulong earnedExp = (ulong)(Configuration.ExpPerHitAxe + (Configuration.ExpPerHitAxe * experienceMultiplierCompatibility));
            // Increasing by player class
            earnedExp = (ulong)Math.Round(earnedExp * Configuration.GetEXPMultiplyByClassAndLevelType(player.Entity.WatchedAttributes.GetString("characterClass"), "Axe"));
            // Minium exp earned is 1
            if (earnedExp <= 0) earnedExp = (ulong)Configuration.minimumEXPEarned;
            ulong exp = levels.GetValueOrDefault<string, ulong>(player.PlayerUID, 0) + earnedExp;
            if (Configuration.AxeIsMaxLevel(exp)) return;

            // Increment
            levels[player.PlayerUID] = exp;
            // Save it
            SaveLevels(levels);
            // Update it
            Shared.Instance.UpdateLevelAndNotify(api, player, levelType, exp);
            if (Configuration.enableLevelUpExperienceServerLog)
                Debug.Log($"{player.PlayerName} earned {earnedExp} exp with {levelType} by {reason}, actual: {exp}");
        }
        // Block breaking
        else if (levelType == "Axe" && reason == "Breaking")
        {
            // Get levels
            var levels = GetSavedLevels();
            ulong earnedExp = (ulong)(Configuration.ExpPerBreakingAxe + (Configuration.ExpPerBreakingAxe * experienceMultiplierCompatibility));
            // Increasing by player class
            earnedExp = (ulong)Math.Round(earnedExp * Configuration.GetEXPMultiplyByClassAndLevelType(player.Entity.WatchedAttributes.GetString("characterClass"), "Axe"));
            // Minium exp earned is 1
            if (earnedExp <= 0) earnedExp = (ulong)Configuration.minimumEXPEarned;
            ulong exp = levels.GetValueOrDefault<string, ulong>(player.PlayerUID, 0) + earnedExp;
            if (Configuration.AxeIsMaxLevel(exp)) return;

            // Increment
            levels[player.PlayerUID] = exp;
            // Save it
            SaveLevels(levels);
            // Update it
            Shared.Instance.UpdateLevelAndNotify(api, player, levelType, exp);
            if (Configuration.enableLevelUpExperienceServerLog)
                Debug.Log($"{player.PlayerName} earned {earnedExp} exp with {levelType} by {reason}, actual: {exp}");
        }
        // Chop Tree
        else if (levelType == "Axe" && reason == "Chop_Tree")
        {
            // Get levels
            var levels = GetSavedLevels();
            ulong earnedExp = (ulong)(Configuration.ExpPerTreeBreakingAxe + (Configuration.ExpPerTreeBreakingAxe * experienceMultiplierCompatibility));
            // Increasing by player class
            earnedExp = (ulong)Math.Round(earnedExp * Configuration.GetEXPMultiplyByClassAndLevelType(player.Entity.WatchedAttributes.GetString("characterClass"), "Axe"));
            // Minium exp earned is 1
            if (earnedExp <= 0) earnedExp = (ulong)Configuration.minimumEXPEarned;
            ulong exp = levels.GetValueOrDefault<string, ulong>(player.PlayerUID, 0) + earnedExp;
            if (Configuration.AxeIsMaxLevel(exp)) return;

            // Increment
            levels[player.PlayerUID] = exp;
            // Save it
            SaveLevels(levels);
            // Update it
            Shared.Instance.UpdateLevelAndNotify(api, player, levelType, exp);
            if (Configuration.enableLevelUpExperienceServerLog)
                Debug.Log($"{player.PlayerName} earned {earnedExp} exp with {levelType} by {reason}, actual: {exp}");
        }
        #endregion
        #region pickaxe
        // Hit
        if (levelType == "Pickaxe" && reason == "Hit")
        {
            // Get levels
            var levels = GetSavedLevels();
            ulong earnedExp = (ulong)(Configuration.ExpPerHitPickaxe + (Configuration.ExpPerHitPickaxe * experienceMultiplierCompatibility));
            // Increasing by player class
            earnedExp = (ulong)Math.Round(earnedExp * Configuration.GetEXPMultiplyByClassAndLevelType(player.Entity.WatchedAttributes.GetString("characterClass"), "Pickaxe"));
            // Minium exp earned is 1
            if (earnedExp <= 0) earnedExp = (ulong)Configuration.minimumEXPEarned;
            ulong exp = levels.GetValueOrDefault<string, ulong>(player.PlayerUID, 0) + earnedExp;
            if (Configuration.PickaxeIsMaxLevel(exp)) return;

            // Increment
            levels[player.PlayerUID] = exp;
            // Save it
            SaveLevels(levels);
            // Update it
            Shared.Instance.UpdateLevelAndNotify(api, player, levelType, exp);
            if (Configuration.enableLevelUpExperienceServerLog)
                Debug.Log($"{player.PlayerName} earned {earnedExp} exp with {levelType} by {reason}, actual: {exp}");
        }
        // Break blocks
        else if (levelType == "Pickaxe" && reason == "Breaking")
        {
            // Get levels
            var levels = GetSavedLevels();
            ulong earnedExp = (ulong)(Configuration.ExpPerBreakingPickaxe + (Configuration.ExpPerBreakingPickaxe * experienceMultiplierCompatibility));
            // Increasing by player class
            earnedExp = (ulong)Math.Round(earnedExp * Configuration.GetEXPMultiplyByClassAndLevelType(player.Entity.WatchedAttributes.GetString("characterClass"), "Pickaxe"));
            // Minium exp earned is 1
            if (earnedExp <= 0) earnedExp = (ulong)Configuration.minimumEXPEarned;
            ulong exp = levels.GetValueOrDefault<string, ulong>(player.PlayerUID, 0) + earnedExp;
            if (Configuration.PickaxeIsMaxLevel(exp)) return;

            // Increment
            levels[player.PlayerUID] = exp;
            // Save it
            SaveLevels(levels);
            // Update it
            Shared.Instance.UpdateLevelAndNotify(api, player, levelType, exp);
            if (Configuration.enableLevelUpExperienceServerLog)
                Debug.Log($"{player.PlayerName} earned {earnedExp} exp with {levelType} by {reason}, actual: {exp}");
        }
        #endregion
        #region shovel
        // Hit
        if (levelType == "Shovel" && reason == "Hit")
        {
            // Get levels
            var levels = GetSavedLevels();
            ulong earnedExp = (ulong)(Configuration.ExpPerHitShovel + (Configuration.ExpPerHitShovel * experienceMultiplierCompatibility));
            // Increasing by player class
            earnedExp = (ulong)Math.Round(earnedExp * Configuration.GetEXPMultiplyByClassAndLevelType(player.Entity.WatchedAttributes.GetString("characterClass"), "Shovel"));
            // Minium exp earned is 1
            if (earnedExp <= 0) earnedExp = (ulong)Configuration.minimumEXPEarned;
            ulong exp = levels.GetValueOrDefault<string, ulong>(player.PlayerUID, 0) + earnedExp;
            if (Configuration.ShovelIsMaxLevel(exp)) return;

            // Increment
            levels[player.PlayerUID] = exp;
            // Save it
            SaveLevels(levels);
            // Update it
            Shared.Instance.UpdateLevelAndNotify(api, player, levelType, exp);
            if (Configuration.enableLevelUpExperienceServerLog)
                Debug.Log($"{player.PlayerName} earned {earnedExp} exp with {levelType} by {reason}, actual: {exp}");
        }
        // Break block
        else if (levelType == "Shovel" && reason == "Breaking")
        {
            // Get levels
            var levels = GetSavedLevels();
            ulong earnedExp = (ulong)(Configuration.ExpPerBreakingShovel + (Configuration.ExpPerBreakingShovel * experienceMultiplierCompatibility));
            // Increasing by player class
            earnedExp = (ulong)Math.Round(earnedExp * Configuration.GetEXPMultiplyByClassAndLevelType(player.Entity.WatchedAttributes.GetString("characterClass"), "Shovel"));
            // Minium exp earned is 1
            if (earnedExp <= 0) earnedExp = (ulong)Configuration.minimumEXPEarned;
            ulong exp = levels.GetValueOrDefault<string, ulong>(player.PlayerUID, 0) + earnedExp;
            if (Configuration.ShovelIsMaxLevel(exp)) return;

            // Increment
            levels[player.PlayerUID] = exp;
            // Save it
            SaveLevels(levels);
            // Update it
            Shared.Instance.UpdateLevelAndNotify(api, player, levelType, exp);
            if (Configuration.enableLevelUpExperienceServerLog)
                Debug.Log($"{player.PlayerName} earned {earnedExp} exp with {levelType} by {reason}, actual: {exp}");
        }
        #endregion
        #region spear
        // Hit
        if (levelType == "Spear" && reason == "Hit")
        {
            // Get levels
            var levels = GetSavedLevels();
            ulong earnedExp = (ulong)(Configuration.ExpPerHitSpear + (Configuration.ExpPerHitSpear * experienceMultiplierCompatibility));
            // Increasing by player class
            earnedExp = (ulong)Math.Round(earnedExp * Configuration.GetEXPMultiplyByClassAndLevelType(player.Entity.WatchedAttributes.GetString("characterClass"), "Spear"));
            // Minium exp earned is 1
            if (earnedExp <= 0) earnedExp = (ulong)Configuration.minimumEXPEarned;
            ulong exp = levels.GetValueOrDefault<string, ulong>(player.PlayerUID, 0) + earnedExp;
            if (Configuration.SpearIsMaxLevel(exp)) return;

            // Increment
            levels[player.PlayerUID] = exp;
            // Save it
            SaveLevels(levels);
            // Update it
            Shared.Instance.UpdateLevelAndNotify(api, player, levelType, exp);
            if (Configuration.enableLevelUpExperienceServerLog)
                Debug.Log($"{player.PlayerName} earned {earnedExp} exp with {levelType} by {reason}, actual: {exp}");
        }
        // Throw
        else if (levelType == "Spear" && reason == "Hit_Throw")
        {
            // Get levels
            var levels = GetSavedLevels();
            ulong earnedExp = (ulong)(Configuration.ExpPerThrowSpear + (Configuration.ExpPerThrowSpear * experienceMultiplierCompatibility));
            // Increasing by player class
            earnedExp = (ulong)Math.Round(earnedExp * Configuration.GetEXPMultiplyByClassAndLevelType(player.Entity.WatchedAttributes.GetString("characterClass"), "Spear"));
            // Minium exp earned is 1
            if (earnedExp <= 0) earnedExp = (ulong)Configuration.minimumEXPEarned;
            ulong exp = levels.GetValueOrDefault<string, ulong>(player.PlayerUID, 0) + earnedExp;
            if (Configuration.SpearIsMaxLevel(exp)) return;

            // Increment
            levels[player.PlayerUID] = exp;
            // Save it
            SaveLevels(levels);
            // Update it
            Shared.Instance.UpdateLevelAndNotify(api, player, levelType, exp);
            if (Configuration.enableLevelUpExperienceServerLog)
                Debug.Log($"{player.PlayerName} earned {earnedExp} exp with {levelType} by {reason}, actual: {exp}");
        }
        #endregion
        #region hammer
        // Hit
        if (levelType == "Hammer" && reason == "Hit")
        {
            // Get levels
            var levels = GetSavedLevels();
            ulong earnedExp = (ulong)(Configuration.ExpPerHitHammer + (Configuration.ExpPerHitHammer * experienceMultiplierCompatibility));
            // Increasing by player class
            earnedExp = (ulong)Math.Round(earnedExp * Configuration.GetEXPMultiplyByClassAndLevelType(player.Entity.WatchedAttributes.GetString("characterClass"), "Hammer"));
            // Minium exp earned is 1
            if (earnedExp <= 0) earnedExp = (ulong)Configuration.minimumEXPEarned;
            ulong exp = levels.GetValueOrDefault<string, ulong>(player.PlayerUID, 0) + earnedExp;
            if (Configuration.HammerIsMaxLevel(exp)) return;

            // Increment
            levels[player.PlayerUID] = exp;
            // Save it
            SaveLevels(levels);
            // Update it
            Shared.Instance.UpdateLevelAndNotify(api, player, levelType, exp);
            if (Configuration.enableLevelUpExperienceServerLog)
                Debug.Log($"{player.PlayerName} earned {earnedExp} exp with {levelType} by {reason}, actual: {exp}");
        }
        #endregion
        #region sword
        // Hit
        if (levelType == "Sword" && reason == "Hit")
        {
            // Get levels
            var levels = GetSavedLevels();
            ulong earnedExp = (ulong)(Configuration.ExpPerHitSword + (Configuration.ExpPerHitSword * experienceMultiplierCompatibility));
            // Increasing by player class
            earnedExp = (ulong)Math.Round(earnedExp * Configuration.GetEXPMultiplyByClassAndLevelType(player.Entity.WatchedAttributes.GetString("characterClass"), "Sword"));
            // Minium exp earned is 1
            if (earnedExp <= 0) earnedExp = (ulong)Configuration.minimumEXPEarned;
            ulong exp = levels.GetValueOrDefault<string, ulong>(player.PlayerUID, 0) + earnedExp;
            if (Configuration.SwordIsMaxLevel(exp)) return;

            // Increment
            levels[player.PlayerUID] = exp;
            // Save it
            SaveLevels(levels);
            // Update it
            Shared.Instance.UpdateLevelAndNotify(api, player, levelType, exp);
            if (Configuration.enableLevelUpExperienceServerLog)
                Debug.Log($"{player.PlayerName} earned {earnedExp} exp with {levelType} by {reason}, actual: {exp}");
        }
        #endregion
        #region shield
        // Hit
        if (levelType == "Shield" && reason == "Hit")
        {
            // Get levels
            var levels = GetSavedLevels();
            ulong earnedExp = (ulong)(Configuration.ExpPerHitShield + (Configuration.ExpPerHitShield * experienceMultiplierCompatibility));
            // Increasing by player class
            earnedExp = (ulong)Math.Round(earnedExp * Configuration.GetEXPMultiplyByClassAndLevelType(player.Entity.WatchedAttributes.GetString("characterClass"), "Shield"));
            // Minium exp earned is 1
            if (earnedExp <= 0) earnedExp = (ulong)Configuration.minimumEXPEarned;
            ulong exp = levels.GetValueOrDefault<string, ulong>(player.PlayerUID, 0) + earnedExp;
            if (Configuration.ShieldIsMaxLevel(exp)) return;

            // Increment
            levels[player.PlayerUID] = exp;
            // Save it
            SaveLevels(levels);
            // Update it
            Shared.Instance.UpdateLevelAndNotify(api, player, levelType, exp);
            if (Configuration.enableLevelUpExperienceServerLog)
                Debug.Log($"{player.PlayerName} earned {earnedExp} exp with {levelType} by {reason}, actual: {exp}");
        }
        #endregion
        #region hand
        // Hit
        if (levelType == "Hand" && reason == "Hit")
        {
            // Get levels
            var levels = GetSavedLevels();
            ulong earnedExp = (ulong)(Configuration.ExpPerHitHand + (Configuration.ExpPerHitHand * experienceMultiplierCompatibility));
            // Increasing by player class
            earnedExp = (ulong)Math.Round(earnedExp * Configuration.GetEXPMultiplyByClassAndLevelType(player.Entity.WatchedAttributes.GetString("characterClass"), "Hand"));
            // Minium exp earned is 1
            if (earnedExp <= 0) earnedExp = (ulong)Configuration.minimumEXPEarned;
            ulong exp = levels.GetValueOrDefault<string, ulong>(player.PlayerUID, 0) + earnedExp;
            if (Configuration.HandIsMaxLevel(exp)) return;

            // Increment
            levels[player.PlayerUID] = exp;
            // Save it
            SaveLevels(levels);
            // Update it
            Shared.Instance.UpdateLevelAndNotify(api, player, levelType, exp);
            if (Configuration.enableLevelUpExperienceServerLog)
                Debug.Log($"{player.PlayerName} earned {earnedExp} exp with {levelType} by {reason}, actual: {exp}");
        }
        #endregion

        #region farming
        // Till Soil
        if (levelType == "Farming" && reason == "Till")
        {
            // Get levels
            var levels = GetSavedLevels();
            ulong earnedExp = (ulong)(Configuration.ExpPerTillFarming + (Configuration.ExpPerTillFarming * experienceMultiplierCompatibility));
            // Increasing by player class
            earnedExp = (ulong)Math.Round(earnedExp * Configuration.GetEXPMultiplyByClassAndLevelType(player.Entity.WatchedAttributes.GetString("characterClass"), "Farming"));
            // Minium exp earned is 1
            if (earnedExp <= 0) earnedExp = (ulong)Configuration.minimumEXPEarned;
            ulong exp = levels.GetValueOrDefault<string, ulong>(player.PlayerUID, 0) + earnedExp;
            if (Configuration.FarmingIsMaxLevel(exp)) return;

            // Increment
            levels[player.PlayerUID] = exp;
            // Save it
            SaveLevels(levels);
            // Update it
            Shared.Instance.UpdateLevelAndNotify(api, player, levelType, exp);
            if (Configuration.enableLevelUpExperienceServerLog)
                Debug.Log($"{player.PlayerName} earned {earnedExp} exp with {levelType} by {reason}, actual: {exp}");
        }
        // Harvest Crop
        if (levelType == "Farming" && reason == "Harvest" && forceexp > 0)
        {
            // Get levels
            var levels = GetSavedLevels();
            ulong earnedExp = (ulong)(forceexp + (forceexp * experienceMultiplierCompatibility));
            // Increasing by player class
            earnedExp = (ulong)Math.Round(earnedExp * Configuration.GetEXPMultiplyByClassAndLevelType(player.Entity.WatchedAttributes.GetString("characterClass"), "Farming"));
            // Minium exp earned is 1
            if (earnedExp <= 0) earnedExp = (ulong)Configuration.minimumEXPEarned;
            ulong exp = levels.GetValueOrDefault<string, ulong>(player.PlayerUID, 0) + earnedExp;
            if (Configuration.FarmingIsMaxLevel(exp)) return;

            // Increment
            levels[player.PlayerUID] = exp;
            // Save it
            SaveLevels(levels);
            // Update it
            Shared.Instance.UpdateLevelAndNotify(api, player, levelType, exp);
            if (Configuration.enableLevelUpExperienceServerLog)
                Debug.Log($"{player.PlayerName} earned {earnedExp} exp with {levelType} by {reason}, actual: {exp}");
        }
        #endregion
        #region cooking
        // Cooking
        if (levelType == "Cooking" && reason == "Cooking_Finished" && forceexp > 0)
        {
            // Get levels
            var levels = GetSavedLevels();
            ulong earnedExp = (ulong)(forceexp + (forceexp * experienceMultiplierCompatibility));
            // Increasing by player class
            earnedExp = (ulong)Math.Round(earnedExp * Configuration.GetEXPMultiplyByClassAndLevelType(player.Entity.WatchedAttributes.GetString("characterClass"), "Cooking"));
            // Minium exp earned is 1
            if (earnedExp <= 0) earnedExp = (ulong)Configuration.minimumEXPEarned;
            ulong exp = levels.GetValueOrDefault<string, ulong>(player.PlayerUID, 0) + earnedExp;
            if (Configuration.CookingIsMaxLevel(exp)) return;

            // Increment
            levels[player.PlayerUID] = exp;
            // Save it
            SaveLevels(levels);
            // Update it
            Shared.Instance.UpdateLevelAndNotify(api, player, levelType, exp);
            if (Configuration.enableLevelUpExperienceServerLog)
                Debug.Log($"{player.PlayerName} earned {earnedExp} exp with {levelType} by {reason}, actual: {exp}");
        }
        #endregion
        #region panning
        // Panning
        if (levelType == "Panning" && reason == "Panning_Finished")
        {
            // Get levels
            var levels = GetSavedLevels();
            ulong earnedExp = (ulong)(Configuration.ExpPerPanning + (Configuration.ExpPerPanning * experienceMultiplierCompatibility));
            // Increasing by player class
            earnedExp = (ulong)Math.Round(earnedExp * Configuration.GetEXPMultiplyByClassAndLevelType(player.Entity.WatchedAttributes.GetString("characterClass"), "Panning"));
            // Minium exp earned is 1
            if (earnedExp <= 0) earnedExp = (ulong)Configuration.minimumEXPEarned;
            ulong exp = levels.GetValueOrDefault<string, ulong>(player.PlayerUID, 0) + earnedExp;
            if (Configuration.PanningIsMaxLevel(exp)) return;

            // Increment
            levels[player.PlayerUID] = exp;
            // Save it
            SaveLevels(levels);
            // Update it
            Shared.Instance.UpdateLevelAndNotify(api, player, levelType, exp);
            if (Configuration.enableLevelUpExperienceServerLog)
                Debug.Log($"{player.PlayerName} earned {earnedExp} exp with {levelType} by {reason}, actual: {exp}");
        }
        #endregion
        #region vitality
        // Get hit
        if (levelType == "Vitality" && reason == "Hit" && forceexp > 0)
        {
            // Get levels
            var levels = GetSavedLevels();
            ulong earnedExp = (ulong)(forceexp + (forceexp * experienceMultiplierCompatibility));
            // Increasing by player class
            earnedExp = (ulong)Math.Round(earnedExp * Configuration.GetEXPMultiplyByClassAndLevelType(player.Entity.WatchedAttributes.GetString("characterClass"), "Vitality"));
            // Minium exp earned is 1
            if (earnedExp <= 0) earnedExp = (ulong)Configuration.minimumEXPEarned;
            ulong exp = levels.GetValueOrDefault<string, ulong>(player.PlayerUID, 0) + earnedExp;
            if (Configuration.VitalityIsMaxLevel(exp)) return;

            // Increment
            levels[player.PlayerUID] = exp;
            // Save it
            SaveLevels(levels);
            // Update it
            Shared.Instance.UpdateLevelAndNotify(api, player, levelType, exp);
            if (Configuration.enableLevelUpExperienceServerLog)
                Debug.Log($"{player.PlayerName} earned {earnedExp} exp with {levelType} by {reason}, actual: {exp}");
        }
        #endregion
        #region leatherarmor
        // Get hit
        if (levelType == "LeatherArmor" && reason == "Hit" && forceexp > 0)
        {
            // Get levels
            var levels = GetSavedLevels();
            ulong earnedExp = (ulong)(forceexp + (forceexp * experienceMultiplierCompatibility));
            // Increasing by player class
            earnedExp = (ulong)Math.Round(earnedExp * Configuration.GetEXPMultiplyByClassAndLevelType(player.Entity.WatchedAttributes.GetString("characterClass"), "LeatherArmor"));
            // Minium exp earned is 1
            if (earnedExp <= 0) earnedExp = (ulong)Configuration.minimumEXPEarned;
            ulong exp = levels.GetValueOrDefault<string, ulong>(player.PlayerUID, 0) + earnedExp;
            if (Configuration.LeatherArmorIsMaxLevel(exp)) return;

            // Increment
            levels[player.PlayerUID] = exp;
            // Save it
            SaveLevels(levels);
            // Update it
            Shared.Instance.UpdateLevelAndNotify(api, player, levelType, exp);
            if (Configuration.enableLevelUpExperienceServerLog)
                Debug.Log($"{player.PlayerName} earned {earnedExp} exp with {levelType} by {reason}, actual: {exp}");
        }
        #endregion
        #region chainarmor
        // Get hit
        if (levelType == "ChainArmor" && reason == "Hit" && forceexp > 0)
        {
            // Get levels
            var levels = GetSavedLevels();
            ulong earnedExp = (ulong)(forceexp + (forceexp * experienceMultiplierCompatibility));
            // Increasing by player class
            earnedExp = (ulong)Math.Round(earnedExp * Configuration.GetEXPMultiplyByClassAndLevelType(player.Entity.WatchedAttributes.GetString("characterClass"), "ChainArmor"));
            // Minium exp earned is 1
            if (earnedExp <= 0) earnedExp = (ulong)Configuration.minimumEXPEarned;
            ulong exp = levels.GetValueOrDefault<string, ulong>(player.PlayerUID, 0) + earnedExp;
            if (Configuration.ChainArmorIsMaxLevel(exp)) return;

            // Increment
            levels[player.PlayerUID] = exp;
            // Save it
            SaveLevels(levels);
            // Update it
            Shared.Instance.UpdateLevelAndNotify(api, player, levelType, exp);
            if (Configuration.enableLevelUpExperienceServerLog)
                Debug.Log($"{player.PlayerName} earned {earnedExp} exp with {levelType} by {reason}, actual: {exp}");
        }
        #endregion
        #region brigandinearmor
        // Get hit
        if (levelType == "BrigandineArmor" && reason == "Hit" && forceexp > 0)
        {
            // Get levels
            var levels = GetSavedLevels();
            ulong earnedExp = (ulong)(forceexp + (forceexp * experienceMultiplierCompatibility));
            // Increasing by player class
            earnedExp = (ulong)Math.Round(earnedExp * Configuration.GetEXPMultiplyByClassAndLevelType(player.Entity.WatchedAttributes.GetString("characterClass"), "ChainArmor"));
            // Minium exp earned is 1
            if (earnedExp <= 0) earnedExp = (ulong)Configuration.minimumEXPEarned;
            ulong exp = levels.GetValueOrDefault<string, ulong>(player.PlayerUID, 0) + earnedExp;
            if (Configuration.BrigandineArmorIsMaxLevel(exp)) return;

            // Increment
            levels[player.PlayerUID] = exp;
            // Save it
            SaveLevels(levels);
            // Update it
            Shared.Instance.UpdateLevelAndNotify(api, player, levelType, exp);
            if (Configuration.enableLevelUpExperienceServerLog)
                Debug.Log($"{player.PlayerName} earned {earnedExp} exp with {levelType} by {reason}, actual: {exp}");
        }
        #endregion
        #region platearmor
        // Get hit
        if (levelType == "PlateArmor" && reason == "Hit" && forceexp > 0)
        {
            // Get levels
            var levels = GetSavedLevels();
            ulong earnedExp = (ulong)(forceexp + (forceexp * experienceMultiplierCompatibility));
            // Increasing by player class
            earnedExp = (ulong)Math.Round(earnedExp * Configuration.GetEXPMultiplyByClassAndLevelType(player.Entity.WatchedAttributes.GetString("characterClass"), "ChainArmor"));
            // Minium exp earned is 1
            if (earnedExp <= 0) earnedExp = (ulong)Configuration.minimumEXPEarned;
            ulong exp = levels.GetValueOrDefault<string, ulong>(player.PlayerUID, 0) + earnedExp;
            if (Configuration.PlateArmorIsMaxLevel(exp)) return;

            // Increment
            levels[player.PlayerUID] = exp;
            // Save it
            SaveLevels(levels);
            // Update it
            Shared.Instance.UpdateLevelAndNotify(api, player, levelType, exp);
            if (Configuration.enableLevelUpExperienceServerLog)
                Debug.Log($"{player.PlayerName} earned {earnedExp} exp with {levelType} by {reason}, actual: {exp}");
        }
        #endregion
        #region scalearmor
        // Get hit
        if (levelType == "ScaleArmor" && reason == "Hit" && forceexp > 0)
        {
            // Get levels
            var levels = GetSavedLevels();
            ulong earnedExp = (ulong)(forceexp + (forceexp * experienceMultiplierCompatibility));
            // Increasing by player class
            earnedExp = (ulong)Math.Round(earnedExp * Configuration.GetEXPMultiplyByClassAndLevelType(player.Entity.WatchedAttributes.GetString("characterClass"), "ChainArmor"));
            // Minium exp earned is 1
            if (earnedExp <= 0) earnedExp = (ulong)Configuration.minimumEXPEarned;
            ulong exp = levels.GetValueOrDefault<string, ulong>(player.PlayerUID, 0) + earnedExp;
            if (Configuration.ScaleArmorIsMaxLevel(exp)) return;

            // Increment
            levels[player.PlayerUID] = exp;
            // Save it
            SaveLevels(levels);
            // Update it
            Shared.Instance.UpdateLevelAndNotify(api, player, levelType, exp);
            if (Configuration.enableLevelUpExperienceServerLog)
                Debug.Log($"{player.PlayerName} earned {earnedExp} exp with {levelType} by {reason}, actual: {exp}");
        }
        #endregion
    }

    public static void UpdatePlayerLevels(IServerPlayer player, ICoreServerAPI api)
    {
        // Get all players saved level
        Dictionary<string, ulong> GetSavedLevels(string levelType)
        {
            byte[] dataBytes = api.WorldManager.SaveGame.GetData($"LevelUPData_{levelType}");
            string data = dataBytes == null ? "{}" : SerializerUtil.Deserialize<string>(dataBytes);
            return JsonSerializer.Deserialize<Dictionary<string, ulong>>(data);
        }

        // Hunter Level
        Shared.Instance.UpdateLevelAndNotify(api, player, "Hunter", GetSavedLevels("Hunter").GetValueOrDefault<string, ulong>(player.PlayerUID, 0), true);

        // Bow Level
        Shared.Instance.UpdateLevelAndNotify(api, player, "Bow", GetSavedLevels("Bow").GetValueOrDefault<string, ulong>(player.PlayerUID, 0), true);

        // Axe Level
        Shared.Instance.UpdateLevelAndNotify(api, player, "Knife", GetSavedLevels("Knife").GetValueOrDefault<string, ulong>(player.PlayerUID, 0), true);

        // Axe Level
        Shared.Instance.UpdateLevelAndNotify(api, player, "Axe", GetSavedLevels("Axe").GetValueOrDefault<string, ulong>(player.PlayerUID, 0), true);

        // Pickaxe Level
        Shared.Instance.UpdateLevelAndNotify(api, player, "Pickaxe", GetSavedLevels("Pickaxe").GetValueOrDefault<string, ulong>(player.PlayerUID, 0), true);

        // Shovel Level
        Shared.Instance.UpdateLevelAndNotify(api, player, "Shovel", GetSavedLevels("Shovel").GetValueOrDefault<string, ulong>(player.PlayerUID, 0), true);

        // Spear Level
        Shared.Instance.UpdateLevelAndNotify(api, player, "Spear", GetSavedLevels("Spear").GetValueOrDefault<string, ulong>(player.PlayerUID, 0), true);

        // Hammer Level
        Shared.Instance.UpdateLevelAndNotify(api, player, "Hammer", GetSavedLevels("Hammer").GetValueOrDefault<string, ulong>(player.PlayerUID, 0), true);

        // Sword Level
        Shared.Instance.UpdateLevelAndNotify(api, player, "Sword", GetSavedLevels("Sword").GetValueOrDefault<string, ulong>(player.PlayerUID, 0), true);

        // Shield Level
        Shared.Instance.UpdateLevelAndNotify(api, player, "Shield", GetSavedLevels("Shield").GetValueOrDefault<string, ulong>(player.PlayerUID, 0), true);

        // Hand Level
        Shared.Instance.UpdateLevelAndNotify(api, player, "Hand", GetSavedLevels("Hand").GetValueOrDefault<string, ulong>(player.PlayerUID, 0), true);

        // Farming Level
        Shared.Instance.UpdateLevelAndNotify(api, player, "Farming", GetSavedLevels("Farming").GetValueOrDefault<string, ulong>(player.PlayerUID, 0), true);

        // Cooking Level
        Shared.Instance.UpdateLevelAndNotify(api, player, "Cooking", GetSavedLevels("Cooking").GetValueOrDefault<string, ulong>(player.PlayerUID, 0), true);

        // Panning Level
        Shared.Instance.UpdateLevelAndNotify(api, player, "Panning", GetSavedLevels("Panning").GetValueOrDefault<string, ulong>(player.PlayerUID, 0), true);

        // Vitality Level
        Shared.Instance.UpdateLevelAndNotify(api, player, "Vitality", GetSavedLevels("Vitality").GetValueOrDefault<string, ulong>(player.PlayerUID, 0), true);

        // Leather Armor Level
        Shared.Instance.UpdateLevelAndNotify(api, player, "LeatherArmor", GetSavedLevels("LeatherArmor").GetValueOrDefault<string, ulong>(player.PlayerUID, 0), true);

        // Chain Armor Level
        Shared.Instance.UpdateLevelAndNotify(api, player, "ChainArmor", GetSavedLevels("ChainArmor").GetValueOrDefault<string, ulong>(player.PlayerUID, 0), true);

        // Brigandine Armor Level
        Shared.Instance.UpdateLevelAndNotify(api, player, "BrigandineArmor", GetSavedLevels("BrigandineArmor").GetValueOrDefault<string, ulong>(player.PlayerUID, 0), true);

        // Plate Armor Level
        Shared.Instance.UpdateLevelAndNotify(api, player, "PlateArmor", GetSavedLevels("PlateArmor").GetValueOrDefault<string, ulong>(player.PlayerUID, 0), true);

        // Scale Armor Level
        Shared.Instance.UpdateLevelAndNotify(api, player, "ScaleArmor", GetSavedLevels("ScaleArmor").GetValueOrDefault<string, ulong>(player.PlayerUID, 0), true);
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

        // Sending the configurations to the player
        communicationChannel.SendPacket(new ServerMessage() { message = $"enabledlevels&{JsonSerializer.Serialize(enabledLevels)}" }, player);
    }

    public static readonly Dictionary<string, long> playersHardcoreDelay = [];
    static public void ResetPlayerLevels(IServerPlayer player, ICoreServerAPI api, double overwriteLose = -1)
    {
        // Check if delay config is enabled
        if (Configuration.hardcorePenaltyDelayInWorldSeconds > 0 && overwriteLose == -1)
        {
            // Check if player exist in the delay
            if (playersHardcoreDelay.TryGetValue(player.PlayerUID, out long delay))
            {
                // Check if the player is on delay
                if (api.World.Calendar.ElapsedSeconds - delay < Configuration.hardcorePenaltyDelayInWorldSeconds)
                {
                    if (Configuration.enableExtendedLog)
                        Debug.Log($"{player.PlayerName} died but hes on the hardcore delay, not losing any exp, seconds to remove from delay: {Configuration.hardcorePenaltyDelayInWorldSeconds - (api.World.Calendar.ElapsedSeconds - delay)}");
                    return;
                }
                // Exist but is not on delay
                else playersHardcoreDelay.Remove(player.PlayerUID);
            }
            // Player does not exist in hardcore delay so we add and continue to reduction
            else playersHardcoreDelay.Add(player.PlayerUID, api.World.Calendar.ElapsedSeconds);
        }
        // Get all players hunter level
        Dictionary<string, ulong> GetSavedLevels(string levelType)
        {
            byte[] dataBytes = api.WorldManager.SaveGame.GetData($"LevelUPData_{levelType}");
            string data = dataBytes == null ? "{}" : SerializerUtil.Deserialize<string>(dataBytes);
            return JsonSerializer.Deserialize<Dictionary<string, ulong>>(data);
        }

        // Get the lose porcentage
        double losePercentage;
        if (overwriteLose == -1) losePercentage = Configuration.hardcoreLosePercentage;
        else losePercentage = overwriteLose;

        {
            Dictionary<string, ulong> level = GetSavedLevels("Hunter");
            if (level.TryGetValue(player.PlayerUID, out ulong value) && value > 0)
            {
                double newValue = value * (ulong)losePercentage;
                level[player.PlayerUID] = (ulong)Math.Round(newValue);
            }
            api.WorldManager.SaveGame.StoreData("LevelUPData_Hunter", JsonSerializer.Serialize(level));
        }
        {
            Dictionary<string, ulong> level = GetSavedLevels("Bow");
            if (level.TryGetValue(player.PlayerUID, out ulong value) && value > 0)
            {
                double newValue = value * (ulong)losePercentage;
                level[player.PlayerUID] = (ulong)Math.Round(newValue);
            }
            api.WorldManager.SaveGame.StoreData("LevelUPData_Bow", JsonSerializer.Serialize(level));
        }
        {
            Dictionary<string, ulong> level = GetSavedLevels("Knife");
            if (level.TryGetValue(player.PlayerUID, out ulong value) && value > 0)
            {
                double newValue = value * (ulong)losePercentage;
                level[player.PlayerUID] = (ulong)Math.Round(newValue);
            }
            api.WorldManager.SaveGame.StoreData("LevelUPData_Knife", JsonSerializer.Serialize(level));
        }
        {
            Dictionary<string, ulong> level = GetSavedLevels("Axe");
            if (level.TryGetValue(player.PlayerUID, out ulong value) && value > 0)
            {
                double newValue = value * (ulong)losePercentage;
                level[player.PlayerUID] = (ulong)Math.Round(newValue);
            }
            api.WorldManager.SaveGame.StoreData("LevelUPData_Axe", JsonSerializer.Serialize(level));
        }
        {
            Dictionary<string, ulong> level = GetSavedLevels("Pickaxe");
            if (level.TryGetValue(player.PlayerUID, out ulong value) && value > 0)
            {
                double newValue = value * (ulong)losePercentage;
                level[player.PlayerUID] = (ulong)Math.Round(newValue);
            }
            api.WorldManager.SaveGame.StoreData("LevelUPData_Pickaxe", JsonSerializer.Serialize(level));
        }
        {
            Dictionary<string, ulong> level = GetSavedLevels("Shovel");
            if (level.TryGetValue(player.PlayerUID, out ulong value) && value > 0)
            {
                double newValue = value * (ulong)losePercentage;
                level[player.PlayerUID] = (ulong)Math.Round(newValue);
            }
            api.WorldManager.SaveGame.StoreData("LevelUPData_Shovel", JsonSerializer.Serialize(level));
        }
        {
            Dictionary<string, ulong> level = GetSavedLevels("Spear");
            if (level.TryGetValue(player.PlayerUID, out ulong value) && value > 0)
            {
                double newValue = value * (ulong)losePercentage;
                level[player.PlayerUID] = (ulong)Math.Round(newValue);
            }
            api.WorldManager.SaveGame.StoreData("LevelUPData_Spear", JsonSerializer.Serialize(level));
        }
        {
            Debug.Log("ALRIGHTT CALLED");
            Dictionary<string, ulong> level = GetSavedLevels("Hammer");
            Debug.Log($"{level.TryGetValue(player.PlayerUID, out ulong flinstons)} : {flinstons}");
            if (level.TryGetValue(player.PlayerUID, out ulong value) && value > 0)
            {
                double newValue = value * (ulong)losePercentage;
                level[player.PlayerUID] = (ulong)Math.Round(newValue);
            }
            api.WorldManager.SaveGame.StoreData("LevelUPData_Hammer", JsonSerializer.Serialize(level));
        }
        {
            Dictionary<string, ulong> level = GetSavedLevels("Sword");
            if (level.TryGetValue(player.PlayerUID, out ulong value) && value > 0)
            {
                double newValue = value * (ulong)losePercentage;
                level[player.PlayerUID] = (ulong)Math.Round(newValue);
            }
            api.WorldManager.SaveGame.StoreData("LevelUPData_Sword", JsonSerializer.Serialize(level));
        }
        {
            Dictionary<string, ulong> level = GetSavedLevels("Shield");
            if (level.TryGetValue(player.PlayerUID, out ulong value) && value > 0)
            {
                double newValue = value * (ulong)losePercentage;
                level[player.PlayerUID] = (ulong)Math.Round(newValue);
            }
            api.WorldManager.SaveGame.StoreData("LevelUPData_Shield", JsonSerializer.Serialize(level));
        }
        {
            Dictionary<string, ulong> level = GetSavedLevels("Hand");
            if (level.TryGetValue(player.PlayerUID, out ulong value) && value > 0)
            {
                double newValue = value * (ulong)losePercentage;
                level[player.PlayerUID] = (ulong)Math.Round(newValue);
            }
            api.WorldManager.SaveGame.StoreData("LevelUPData_Hand", JsonSerializer.Serialize(level));
        }
        {
            Dictionary<string, ulong> level = GetSavedLevels("Farming");
            if (level.TryGetValue(player.PlayerUID, out ulong value) && value > 0)
            {
                double newValue = value * (ulong)losePercentage;
                level[player.PlayerUID] = (ulong)Math.Round(newValue);
            }
            api.WorldManager.SaveGame.StoreData("LevelUPData_Farming", JsonSerializer.Serialize(level));
        }
        {
            Dictionary<string, ulong> level = GetSavedLevels("Cooking");
            if (level.TryGetValue(player.PlayerUID, out ulong value) && value > 0)
            {
                double newValue = value * (ulong)losePercentage;
                level[player.PlayerUID] = (ulong)Math.Round(newValue);
            }
            api.WorldManager.SaveGame.StoreData("LevelUPData_Cooking", JsonSerializer.Serialize(level));
        }
        {
            Dictionary<string, ulong> level = GetSavedLevels("Panning");
            if (level.TryGetValue(player.PlayerUID, out ulong value) && value > 0)
            {
                double newValue = value * (ulong)losePercentage;
                level[player.PlayerUID] = (ulong)Math.Round(newValue);
            }
            api.WorldManager.SaveGame.StoreData("LevelUPData_Panning", JsonSerializer.Serialize(level));
        }
        {
            Dictionary<string, ulong> level = GetSavedLevels("Vitality");
            if (level.TryGetValue(player.PlayerUID, out ulong value) && value > 0)
            {
                double newValue = value * (ulong)losePercentage;
                level[player.PlayerUID] = (ulong)Math.Round(newValue);
            }
            api.WorldManager.SaveGame.StoreData("LevelUPData_Vitality", JsonSerializer.Serialize(level));
        }
        {
            Dictionary<string, ulong> level = GetSavedLevels("LeatherArmor");
            if (level.TryGetValue(player.PlayerUID, out ulong value) && value > 0)
            {
                double newValue = value * (ulong)losePercentage;
                level[player.PlayerUID] = (ulong)Math.Round(newValue);
            }
            api.WorldManager.SaveGame.StoreData("LevelUPData_LeatherArmor", JsonSerializer.Serialize(level));
        }
        {
            Dictionary<string, ulong> level = GetSavedLevels("ChainArmor");
            if (level.TryGetValue(player.PlayerUID, out ulong value) && value > 0)
            {
                double newValue = value * (ulong)losePercentage;
                level[player.PlayerUID] = (ulong)Math.Round(newValue);
            }
            api.WorldManager.SaveGame.StoreData("LevelUPData_ChainArmor", JsonSerializer.Serialize(level));
        }
        {
            Dictionary<string, ulong> level = GetSavedLevels("BrigandineArmor");
            if (level.TryGetValue(player.PlayerUID, out ulong value) && value > 0)
            {
                double newValue = value * (ulong)losePercentage;
                level[player.PlayerUID] = (ulong)Math.Round(newValue);
            }
            api.WorldManager.SaveGame.StoreData("LevelUPData_BrigandineArmor", JsonSerializer.Serialize(level));
        }
        {
            Dictionary<string, ulong> level = GetSavedLevels("PlateArmor");
            if (level.TryGetValue(player.PlayerUID, out ulong value) && value > 0)
            {
                double newValue = value * (ulong)losePercentage;
                level[player.PlayerUID] = (ulong)Math.Round(newValue);
            }
            api.WorldManager.SaveGame.StoreData("LevelUPData_PlateArmor", JsonSerializer.Serialize(level));
        }
        {
            Dictionary<string, ulong> level = GetSavedLevels("ScaleArmor");
            if (level.TryGetValue(player.PlayerUID, out ulong value) && value > 0)
            {
                double newValue = value * (ulong)losePercentage;
                level[player.PlayerUID] = (ulong)Math.Round(newValue);
            }
            api.WorldManager.SaveGame.StoreData("LevelUPData_ScaleArmor", JsonSerializer.Serialize(level));
        }
        if (overwriteLose == -1)
        {
            if (Configuration.enableExtendedLog)
                Debug.Log($"{player.PlayerUID} died and lost {(int)((1.0 - losePercentage) * 100)}% of all experience");
            if (Configuration.hardcoreMessageWhenDying)
                communicationChannel.SendPacket(new ServerMessage() { message = $"playerhardcoredied&{(int)((1.0 - losePercentage) * 100)}" }, player);
        }
    }

}