using System;
using System.Collections.Generic;
using System.IO;
using LevelUP.Server;
using Newtonsoft.Json;
using ProtoBuf;
using Vintagestory.API.Common;

namespace LevelUP;

#pragma warning disable CA2211
public static class Configuration
{
    private static readonly Random Random = new();

    private static Dictionary<string, object> LoadConfigurationByDirectoryAndName(ICoreAPI api, string directory, string name, string defaultDirectory)
    {
        string directoryPath = Path.Combine(api.DataBasePath, directory);
        string configPath = Path.Combine(api.DataBasePath, directory, $"{name}.json");
        Dictionary<string, object> loadedConfig;
        try
        {
            // Load server configurations
            string jsonConfig = File.ReadAllText(configPath);
            loadedConfig = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonConfig);
        }
        catch (DirectoryNotFoundException)
        {
            Debug.LogWarn($"WARNING: Server configurations directory does not exist creating {name}.json and directory...");
            try
            {
                Directory.CreateDirectory(directoryPath);
            }
            catch (Exception ex)
            {
                Debug.LogError($"ERROR: Cannot create directory: {ex.Message}");
            }
            Debug.Log("Loading default configurations...");
            // Load default configurations
            loadedConfig = api.Assets.Get(new AssetLocation(defaultDirectory)).ToObject<Dictionary<string, object>>();

            Debug.Log($"Configurations loaded, saving configs in: {configPath}");
            try
            {
                // Saving default configurations
                string defaultJson = JsonConvert.SerializeObject(loadedConfig, Formatting.Indented);
                File.WriteAllText(configPath, defaultJson);
                return LoadConfigurationByDirectoryAndName(api, directory, name, defaultDirectory);
            }
            catch (Exception ex)
            {
                Debug.LogError($"ERROR: Cannot save default files to {configPath}, reason: {ex.Message}");
            }
        }
        catch (FileNotFoundException)
        {
            Debug.LogWarn($"WARNING: Server configurations {name}.json cannot be found, recreating file from default");
            Debug.Log("Loading default configurations...");
            // Load default configurations
            loadedConfig = api.Assets.Get(new AssetLocation(defaultDirectory)).ToObject<Dictionary<string, object>>();

            Debug.Log($"Configurations loaded, saving configs in: {configPath}");
            try
            {
                // Saving default configurations
                string defaultJson = JsonConvert.SerializeObject(loadedConfig, Formatting.Indented);
                File.WriteAllText(configPath, defaultJson);
                return LoadConfigurationByDirectoryAndName(api, directory, name, defaultDirectory);
            }
            catch (Exception ex)
            {
                Debug.Log($"ERROR: Cannot save default files to {configPath}, reason: {ex.Message}");
            }

        }
        catch (Exception ex)
        {
            Debug.LogError($"ERROR: Cannot read the server configurations: {ex.Message}");
            Debug.Log("Loading default values from mod assets...");
            // Load default configurations
            loadedConfig = api.Assets.Get(new AssetLocation(defaultDirectory)).ToObject<Dictionary<string, object>>();
        }
        return loadedConfig;
    }

    #region baseconfigs
    public static bool enableHardcore = false;
    public static double hardcoreLosePercentage = 0.8;
    public static int hardcorePenaltyDelayInWorldSeconds = 1000;
    public static bool hardcoreMessageWhenDying = true;
    public static bool enableLevelHunter = true;
    public static bool enableLevelBow = true;
    public static bool enableLevelKnife = true;
    public static bool enableLevelSpear = true;
    public static bool enableLevelHammer = true;
    public static bool enableLevelAxe = true;
    public static bool enableLevelPickaxe = true;
    public static bool enableLevelShovel = true;
    public static bool enableLevelSword = true;
    public static bool enableLevelShield = true;
    public static bool enableLevelHand = true;
    public static bool enableLevelFarming = true;
    public static bool enableLevelCooking = true;
    public static bool enableLevelPanning = true;
    public static bool enableLevelVitality = true;
    public static bool enableLevelLeatherArmor = true;
    public static bool enableLevelChainArmor = true;
    public static bool enableLevelBrigandineArmor = true;
    public static bool enableLevelPlateArmor = true;
    public static bool enableLevelScaleArmor = true;
    public static bool enableLevelSmithing = true;
    public static int minimumEXPEarned = 1;
    public static bool enableLevelUPUIDSecurity = false;
    public static bool enableLevelUpChatMessages = false;
    public static bool enableLevelUpExperienceServerLog = false;
    public static bool enableExtendedLog = false;

    public static void UpdateBaseConfigurations(ICoreAPI api)
    {
        Dictionary<string, object> baseConfigs = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config",
            "base",
            "levelup:config/base.json");
        { //enableHardcore
            if (baseConfigs.TryGetValue("enableHardcore", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: enableHardcore is null");
                else if (value is not bool) Debug.Log($"CONFIGURATION ERROR: enableHardcore is not boolean is {value.GetType()}");
                else enableHardcore = (bool)value;
            else Debug.LogError("CONFIGURATION ERROR: enableHardcore not set");
        }
        { //hardcoreLosePercentage
            if (baseConfigs.TryGetValue("hardcoreLosePercentage", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: hardcoreLosePercentage is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: hardcoreLosePercentage is not double is {value.GetType()}");
                else hardcoreLosePercentage = (double)value;
            else Debug.LogError("CONFIGURATION ERROR: hardcoreLosePercentage not set");
        }
        { //hardcorePenaltyDelayInWorldSeconds
            if (baseConfigs.TryGetValue("hardcorePenaltyDelayInWorldSeconds", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: hardcorePenaltyDelayInWorldSeconds is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: hardcorePenaltyDelayInWorldSeconds is not int is {value.GetType()}");
                else hardcorePenaltyDelayInWorldSeconds = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: hardcorePenaltyDelayInWorldSeconds not set");
        }
        { //hardcoreMessageWhenDying
            if (baseConfigs.TryGetValue("hardcoreMessageWhenDying", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: hardcoreMessageWhenDying is null");
                else if (value is not bool) Debug.Log($"CONFIGURATION ERROR: hardcoreMessageWhenDying is not boolean is {value.GetType()}");
                else hardcoreMessageWhenDying = (bool)value;
            else Debug.LogError("CONFIGURATION ERROR: hardcoreMessageWhenDying not set");
        }
        { //enableLevelHunter
            if (baseConfigs.TryGetValue("enableLevelHunter", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: enableLevelHunter is null");
                else if (value is not bool) Debug.Log($"CONFIGURATION ERROR: enableLevelHunter is not boolean is {value.GetType()}");
                else enableLevelHunter = (bool)value;
            else Debug.LogError("CONFIGURATION ERROR: enableLevelHunter not set");
        }
        { //enableLevelBow
            if (baseConfigs.TryGetValue("enableLevelBow", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: enableLevelBow is null");
                else if (value is not bool) Debug.Log($"CONFIGURATION ERROR: enableLevelBow is not boolean is {value.GetType()}");
                else enableLevelBow = (bool)value;
            else Debug.LogError("CONFIGURATION ERROR: enableLevelBow not set");
        }
        { //enableLevelKnife
            if (baseConfigs.TryGetValue("enableLevelKnife", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: enableLevelKnife is null");
                else if (value is not bool) Debug.Log($"CONFIGURATION ERROR: enableLevelKnife is not boolean is {value.GetType()}");
                else enableLevelKnife = (bool)value;
            else Debug.LogError("CONFIGURATION ERROR: enableLevelKnife not set");
        }
        { //enableLevelSpear
            if (baseConfigs.TryGetValue("enableLevelSpear", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: enableLevelSpear is null");
                else if (value is not bool) Debug.Log($"CONFIGURATION ERROR: enableLevelSpear is not boolean is {value.GetType()}");
                else enableLevelSpear = (bool)value;
            else Debug.LogError("CONFIGURATION ERROR: enableLevelSpear not set");
        }
        { //enableLevelHammer
            if (baseConfigs.TryGetValue("enableLevelHammer", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: enableLevelHammer is null");
                else if (value is not bool) Debug.Log($"CONFIGURATION ERROR: enableLevelHammer is not boolean is {value.GetType()}");
                else enableLevelHammer = (bool)value;
            else Debug.LogError("CONFIGURATION ERROR: enableLevelHammer not set");
        }
        { //enableLevelAxe
            if (baseConfigs.TryGetValue("enableLevelAxe", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: enableLevelAxe is null");
                else if (value is not bool) Debug.Log($"CONFIGURATION ERROR: enableLevelAxe is not boolean is {value.GetType()}");
                else enableLevelAxe = (bool)value;
            else Debug.LogError("CONFIGURATION ERROR: enableLevelAxe not set");
        }
        { //enableLevelPickaxe
            if (baseConfigs.TryGetValue("enableLevelPickaxe", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: enableLevelPickaxe is null");
                else if (value is not bool) Debug.Log($"CONFIGURATION ERROR: enableLevelPickaxe is not boolean is {value.GetType()}");
                else enableLevelPickaxe = (bool)value;
            else Debug.LogError("CONFIGURATION ERROR: enableLevelPickaxe not set");
        }
        { //enableLevelShovel
            if (baseConfigs.TryGetValue("enableLevelShovel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: enableLevelShovel is null");
                else if (value is not bool) Debug.Log($"CONFIGURATION ERROR: enableLevelShovel is not boolean is {value.GetType()}");
                else enableLevelShovel = (bool)value;
            else Debug.LogError("CONFIGURATION ERROR: enableLevelShovel not set");
        }
        { //enableLevelHammer
            if (baseConfigs.TryGetValue("enableLevelHammer", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: enableLevelHammer is null");
                else if (value is not bool) Debug.Log($"CONFIGURATION ERROR: enableLevelHammer is not boolean is {value.GetType()}");
                else enableLevelHammer = (bool)value;
            else Debug.LogError("CONFIGURATION ERROR: enableLevelHammer not set");
        }
        { //enableLevelSword
            if (baseConfigs.TryGetValue("enableLevelSword", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: enableLevelSword is null");
                else if (value is not bool) Debug.Log($"CONFIGURATION ERROR: enableLevelSword is not boolean is {value.GetType()}");
                else enableLevelSword = (bool)value;
            else Debug.LogError("CONFIGURATION ERROR: enableLevelSword not set");
        }
        { //enableLevelShield
            if (baseConfigs.TryGetValue("enableLevelShield", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: enableLevelShield is null");
                else if (value is not bool) Debug.Log($"CONFIGURATION ERROR: enableLevelShield is not boolean is {value.GetType()}");
                else enableLevelShield = (bool)value;
            else Debug.LogError("CONFIGURATION ERROR: enableLevelShield not set");
        }
        { //enableLevelHand
            if (baseConfigs.TryGetValue("enableLevelHand", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: enableLevelHand is null");
                else if (value is not bool) Debug.Log($"CONFIGURATION ERROR: enableLevelHand is not boolean is {value.GetType()}");
                else enableLevelHand = (bool)value;
            else Debug.LogError("CONFIGURATION ERROR: enableLevelHand not set");
        }
        { //enableLevelFarming
            if (baseConfigs.TryGetValue("enableLevelFarming", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: enableLevelFarming is null");
                else if (value is not bool) Debug.Log($"CONFIGURATION ERROR: enableLevelFarming is not boolean is {value.GetType()}");
                else enableLevelFarming = (bool)value;
            else Debug.LogError("CONFIGURATION ERROR: enableLevelFarming not set");
        }
        { //enableLevelCooking
            if (baseConfigs.TryGetValue("enableLevelCooking", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: enableLevelCooking is null");
                else if (value is not bool) Debug.Log($"CONFIGURATION ERROR: enableLevelCooking is not boolean is {value.GetType()}");
                else enableLevelCooking = (bool)value;
            else Debug.LogError("CONFIGURATION ERROR: enableLevelCooking not set");
        }
        { //enableLevelPanning
            if (baseConfigs.TryGetValue("enableLevelPanning", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: enableLevelPanning is null");
                else if (value is not bool) Debug.Log($"CONFIGURATION ERROR: enableLevelPanning is not boolean is {value.GetType()}");
                else enableLevelPanning = (bool)value;
            else Debug.LogError("CONFIGURATION ERROR: enableLevelPanning not set");
        }
        { //enableLevelVitality
            if (baseConfigs.TryGetValue("enableLevelVitality", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: enableLevelVitality is null");
                else if (value is not bool) Debug.Log($"CONFIGURATION ERROR: enableLevelVitality is not boolean is {value.GetType()}");
                else enableLevelVitality = (bool)value;
            else Debug.LogError("CONFIGURATION ERROR: enableLevelVitality not set");
        }
        { //enableLevelLeatherArmor
            if (baseConfigs.TryGetValue("enableLevelLeatherArmor", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: enableLevelLeatherArmor is null");
                else if (value is not bool) Debug.Log($"CONFIGURATION ERROR: enableLevelLeatherArmor is not boolean is {value.GetType()}");
                else enableLevelLeatherArmor = (bool)value;
            else Debug.LogError("CONFIGURATION ERROR: enableLevelLeatherArmor not set");
        }
        { //enableLevelChainArmor
            if (baseConfigs.TryGetValue("enableLevelChainArmor", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: enableLevelChainArmor is null");
                else if (value is not bool) Debug.Log($"CONFIGURATION ERROR: enableLevelChainArmor is not boolean is {value.GetType()}");
                else enableLevelChainArmor = (bool)value;
            else Debug.LogError("CONFIGURATION ERROR: enableLevelChainArmor not set");
        }
        { //enableLevelBrigandineArmor
            if (baseConfigs.TryGetValue("enableLevelBrigandineArmor", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: enableLevelBrigandineArmor is null");
                else if (value is not bool) Debug.Log($"CONFIGURATION ERROR: enableLevelBrigandineArmor is not boolean is {value.GetType()}");
                else enableLevelBrigandineArmor = (bool)value;
            else Debug.LogError("CONFIGURATION ERROR: enableLevelBrigandineArmor not set");
        }
        { //enableLevelPlateArmor
            if (baseConfigs.TryGetValue("enableLevelPlateArmor", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: enableLevelPlateArmor is null");
                else if (value is not bool) Debug.Log($"CONFIGURATION ERROR: enableLevelPlateArmor is not boolean is {value.GetType()}");
                else enableLevelPlateArmor = (bool)value;
            else Debug.LogError("CONFIGURATION ERROR: enableLevelPlateArmor not set");
        }
        { //enableLevelScaleArmor
            if (baseConfigs.TryGetValue("enableLevelScaleArmor", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: enableLevelScaleArmor is null");
                else if (value is not bool) Debug.Log($"CONFIGURATION ERROR: enableLevelScaleArmor is not boolean is {value.GetType()}");
                else enableLevelScaleArmor = (bool)value;
            else Debug.LogError("CONFIGURATION ERROR: enableLevelScaleArmor not set");
        }
        { //enableLevelSmithing
            if (baseConfigs.TryGetValue("enableLevelSmithing", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: enableLevelSmithing is null");
                else if (value is not bool) Debug.Log($"CONFIGURATION ERROR: enableLevelSmithing is not boolean is {value.GetType()}");
                else enableLevelSmithing = (bool)value;
            else Debug.LogError("CONFIGURATION ERROR: enableLevelSmithing not set");
        }
        { //minimumEXPEarned
            if (baseConfigs.TryGetValue("minimumEXPEarned", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: minimumEXPEarned is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: minimumEXPEarned is not int is {value.GetType()}");
                else minimumEXPEarned = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: minimumEXPEarned not set");
        }
        { //enableLevelUPUIDSecurity
            if (baseConfigs.TryGetValue("enableLevelUPUIDSecurity", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: enableLevelUPUIDSecurity is null");
                else if (value is not bool) Debug.Log($"CONFIGURATION ERROR: enableLevelUPUIDSecurity is not boolean is {value.GetType()}");
                else enableLevelUPUIDSecurity = (bool)value;
            else Debug.LogError("CONFIGURATION ERROR: enableLevelUPUIDSecurity not set");
        }
        { //enableLevelUpChatMessages
            if (baseConfigs.TryGetValue("enableLevelUpChatMessages", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: enableLevelUpChatMessages is null");
                else if (value is not bool) Debug.Log($"CONFIGURATION ERROR: enableLevelUpChatMessages is not boolean is {value.GetType()}");
                else enableLevelUpChatMessages = (bool)value;
            else Debug.LogError("CONFIGURATION ERROR: enableLevelUpChatMessages not set");
        }
        { //enableLevelUpExperienceServerLog
            if (baseConfigs.TryGetValue("enableLevelUpExperienceServerLog", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: enableLevelUpExperienceServerLog is null");
                else if (value is not bool) Debug.Log($"CONFIGURATION ERROR: enableLevelUpExperienceServerLog is not boolean is {value.GetType()}");
                else enableLevelUpExperienceServerLog = (bool)value;
            else Debug.LogError("CONFIGURATION ERROR: enableLevelUpExperienceServerLog not set");
        }
        { //enableExtendedLog
            if (baseConfigs.TryGetValue("enableExtendedLog", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: enableExtendedLog is null");
                else if (value is not bool) Debug.Log($"CONFIGURATION ERROR: enableExtendedLog is not boolean is {value.GetType()}");
                else enableExtendedLog = (bool)value;
            else Debug.LogError("CONFIGURATION ERROR: enableExtendedLog not set");
        }
    }

    #endregion

    private static readonly Dictionary<string, System.Func<ulong, int>> levelsByLevelTypeEXP = [];
    private static readonly Dictionary<string, System.Func<int, ulong>> expByLevelTypeLevel = [];
    /// <summary>
    /// Register a new level type for the function GetLevelByLevelTypeEXP
    /// </summary>
    /// <param name="levelType"></param>
    /// <param name="function"></param>
    public static void RegisterNewLevelTypeEXP(string levelType, System.Func<ulong, int> function)
    {
        if (levelsByLevelTypeEXP.ContainsKey(levelType))
        {
            Debug.LogError($"The leveltype {levelType} already exist in levelsByLevelTypeEXP");
            return;
        }

        levelsByLevelTypeEXP.Add(levelType, function);
    }

    /// <summary>
    /// Register a new level type for the function GetEXPByLevelTypeLevel
    /// </summary>
    /// <param name="levelType"></param>
    /// <param name="function"></param>
    public static void RegisterNewEXPLevelType(string levelType, System.Func<int, ulong> function)
    {
        if (expByLevelTypeLevel.ContainsKey(levelType))
        {
            Debug.LogError($"The leveltype {levelType} already exist in expByLevelTypeLevel");
            return;
        }

        expByLevelTypeLevel.Add(levelType, function);
    }


    public static int GetLevelByLevelTypeEXP(string levelType, ulong exp)
    {
        if (levelsByLevelTypeEXP.TryGetValue(levelType, out System.Func<ulong, int> function))
            return function(exp);

        Debug.LogWarn($"WARNING: {levelType} doesn't belong to the function GetLevelByLevelTypeEXP did you forget to add it? check the wiki");
        return 1;
    }

    public static float GetMiningSpeedByLevelTypeLevel(string levelType, int level)
    {
        switch (levelType)
        {
            case "Axe": return AxeGetMiningMultiplyByLevel(level);
            case "Pickaxe": return PickaxeGetMiningMultiplyByLevel(level);
            case "Shovel": return ShovelGetMiningMultiplyByLevel(level);
            case "Knife": return KnifeGetMiningMultiplyByLevel(level);
            default: break;
        }
        return -1.0f;
    }

    public static ulong GetEXPByLevelTypeLevel(int level, string levelType)
    {
        if (expByLevelTypeLevel.TryGetValue(levelType, out System.Func<int, ulong> function))
            return function(level);

        Debug.LogWarn($"WARNING: {levelType} doesn't belong to the function GetEXPByLevelTypeLevel did you forget to add it? check the wiki");
        return 0;
    }

    private static readonly Dictionary<string, int> maxLevels = [];
    /// <summary>
    /// Register a new level type for the function CheckMaxLevelByLevelTypeEXP
    /// </summary>
    /// <param name="levelType"></param>
    /// <param name="maxLevel"></param>
    public static void RegisterNewMaxLevelByLevelTypeEXP(string levelType, int maxLevel)
    {
        if (maxLevels.ContainsKey(levelType))
        {
            Debug.LogError($"The leveltype {levelType} already exist in maxLevels");
            return;
        }

        maxLevels.Add(levelType, maxLevel);
    }
    public static bool CheckMaxLevelByLevelTypeEXP(string levelType, ulong exp)
    {
        if (maxLevels.TryGetValue(levelType, out int maxLevel))
            return maxLevel <= GetLevelByLevelTypeEXP(levelType, exp);

        Debug.LogWarn($"WARNING: {levelType} doesn't belong to the function CheckMaxLevelByLevelTypeEXP did you forget to add it? check the wiki");
        return false;
    }

    #region hunter
    public static readonly Dictionary<string, int> entityExpHunter = [];
    private static int hunterEXPPerLevelBase = 10;
    private static double hunterEXPMultiplyPerLevel = 1.5;
    private static float hunterBaseDamage = 1.0f;
    private static float hunterIncrementDamagePerLevel = 0.1f;
    public static int hunterMaxLevel = 999;

    public static void PopulateHunterConfiguration(ICoreAPI api)
    {
        Dictionary<string, object> hunterLevelStats = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/levelstats",
            "hunter",
            "levelup:config/levelstats/hunter.json");
        { //hunterEXPPerLevelBase
            if (hunterLevelStats.TryGetValue("hunterEXPPerLevelBase", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: hunterEXPPerLevelBase is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: hunterEXPPerLevelBase is not int is {value.GetType()}");
                else hunterEXPPerLevelBase = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: hunterEXPPerLevelBase not set");
        }
        { //hunterEXPMultiplyPerLevel
            if (hunterLevelStats.TryGetValue("hunterEXPMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: hunterEXPMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: hunterEXPMultiplyPerLevel is not double is {value.GetType()}");
                else hunterEXPMultiplyPerLevel = (double)value;
            else Debug.LogError("CONFIGURATION ERROR: hunterEXPMultiplyPerLevel not set");
        }
        { //hunterBaseDamage
            if (hunterLevelStats.TryGetValue("hunterBaseDamage", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: hunterBaseDamage is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: hunterBaseDamage is not double is {value.GetType()}");
                else hunterBaseDamage = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: hunterBaseDamage not set");
        }
        { //hunterIncrementDamagePerLevel
            if (hunterLevelStats.TryGetValue("hunterIncrementDamagePerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: hunterIncrementDamagePerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: hunterIncrementDamagePerLevel is not double is {value.GetType()}");
                else hunterIncrementDamagePerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: hunterIncrementDamagePerLevel not set");
        }
        { //hunterMaxLevel
            if (hunterLevelStats.TryGetValue("hunterMaxLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: hunterMaxLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: hunterMaxLevel is not int is {value.GetType()}");
                else hunterMaxLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: hunterMaxLevel not set");
        }

        // Get entity exp
        entityExpHunter.Clear();
        Dictionary<string, object> tmpentityExpHunter = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/entityexp",
            "bow",
            "levelup:config/entityexp/hunter.json");
        foreach (KeyValuePair<string, object> pair in tmpentityExpHunter)
        {
            if (pair.Value is long value) entityExpHunter.Add(pair.Key, (int)value);
            else Debug.Log($"CONFIGURATION ERROR: entityExpHunter {pair.Key} is not int");
        }

        Debug.Log("Hunter configuration set");
    }

    public static int HunterGetLevelByEXP(ulong exp)
    {
        double baseExp = hunterEXPPerLevelBase;
        double multiplier = hunterEXPMultiplyPerLevel;

        if (multiplier == 1.0)
        {
            return (int)(exp / baseExp);
        }

        double expDouble = exp;

        double level = Math.Log((expDouble * (multiplier - 1) / baseExp) + 1) / Math.Log(multiplier);

        return Math.Max(0, (int)Math.Floor(level));
    }

    public static ulong HunterGetExpByLevel(int level)
    {
        double baseExp = hunterEXPPerLevelBase;
        double multiplier = hunterEXPMultiplyPerLevel;

        if (multiplier == 1.0)
        {
            return (ulong)(baseExp * level);
        }

        double exp = baseExp * (Math.Pow(multiplier, level) - 1) / (multiplier - 1);
        return (ulong)Math.Floor(exp);
    }

    public static float HunterGetDamageMultiplyByLevel(int level)
    {
        return hunterBaseDamage + hunterIncrementDamagePerLevel * level;
    }
    #endregion

    #region bow
    public static readonly Dictionary<string, int> entityExpBow = [];
    private static int bowEXPPerHit = 1;
    private static int bowEXPPerLevelBase = 10;
    private static double bowEXPMultiplyPerLevel = 1.3;
    private static float bowBaseDamage = 1.0f;
    private static float bowIncrementDamagePerLevel = 0.1f;
    private static float bowBaseChanceToNotLoseArrow = 50.0f;
    private static float bowChanceToNotLoseArrowBaseIncreasePerLevel = 2.0f;
    private static int bowChanceToNotLoseArrowReduceIncreaseEveryLevel = 5;
    private static float bowChanceToNotLoseArrowReduceQuantityEveryLevel = 0.5f;
    private static float bowBaseAimAccuracy = 1.0f;
    private static float bowIncreaseAimAccuracyPerLevel = 0.5f;
    public static int bowMaxLevel = 999;

    public static int ExpPerHitBow => bowEXPPerHit;

    public static void PopulateBowConfiguration(ICoreAPI api)
    {
        Dictionary<string, object> bowLevelStats = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/levelstats",
            "bow",
            "levelup:config/levelstats/bow.json");

        { //bowEXPPerLevelBase
            if (bowLevelStats.TryGetValue("bowEXPPerLevelBase", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: bowEXPPerLevelBase is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: bowEXPPerLevelBase is not int is {value.GetType()}");
                else bowEXPPerLevelBase = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: bowEXPPerLevelBase not set");
        }
        { //bowEXPMultiplyPerLevel
            if (bowLevelStats.TryGetValue("bowEXPMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: bowEXPMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: bowEXPMultiplyPerLevel is not double is {value.GetType()}");
                else bowEXPMultiplyPerLevel = (double)value;
            else Debug.LogError("CONFIGURATION ERROR: bowEXPMultiplyPerLevel not set");
        }
        { //bowBaseDamage
            if (bowLevelStats.TryGetValue("bowBaseDamage", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: bowBaseDamage is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: bowBaseDamage is not double is {value.GetType()}");
                else bowBaseDamage = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: bowBaseDamage not set");
        }
        { //bowIncrementDamagePerLevel
            if (bowLevelStats.TryGetValue("bowIncrementDamagePerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: bowIncrementDamagePerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: bowIncrementDamagePerLevel is not double is {value.GetType()}");
                else bowIncrementDamagePerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: bowIncrementDamagePerLevel not set");
        }
        { //bowEXPPerHit
            if (bowLevelStats.TryGetValue("bowEXPPerHit", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: bowEXPPerHit is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: bowEXPPerHit is not int is {value.GetType()}");
                else bowEXPPerHit = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: bowEXPPerHit not set");
            Experience.LoadExperience("Bow", "Hit", (ulong)bowEXPPerHit);
        }
        { //bowBaseChanceToNotLoseArrow
            if (bowLevelStats.TryGetValue("bowBaseChanceToNotLoseArrow", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: bowBaseChanceToNotLoseArrow is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: bowBaseChanceToNotLoseArrow is not double is {value.GetType()}");
                else bowBaseChanceToNotLoseArrow = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: bowBaseChanceToNotLoseArrow not set");
        }
        { //bowChanceToNotLoseArrowBaseIncreasePerLevel
            if (bowLevelStats.TryGetValue("bowChanceToNotLoseArrowBaseIncreasePerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: bowChanceToNotLoseArrowBaseIncreasePerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: bowChanceToNotLoseArrowBaseIncreasePerLevel is not double is {value.GetType()}");
                else bowChanceToNotLoseArrowBaseIncreasePerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: bowChanceToNotLoseArrowBaseIncreasePerLevel not set");
        }
        { //bowChanceToNotLoseArrowReduceIncreaseEveryLevel
            if (bowLevelStats.TryGetValue("bowChanceToNotLoseArrowReduceIncreaseEveryLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: bowChanceToNotLoseArrowReduceIncreaseEveryLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: bowChanceToNotLoseArrowReduceIncreaseEveryLevel is not int is {value.GetType()}");
                else bowChanceToNotLoseArrowReduceIncreaseEveryLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: bowChanceToNotLoseArrowReduceIncreaseEveryLevel not set");
        }
        { //bowChanceToNotLoseArrowReduceQuantityEveryLevel
            if (bowLevelStats.TryGetValue("bowChanceToNotLoseArrowReduceQuantityEveryLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: bowChanceToNotLoseArrowReduceQuantityEveryLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: bowChanceToNotLoseArrowReduceQuantityEveryLevel is not double is {value.GetType()}");
                else bowChanceToNotLoseArrowReduceQuantityEveryLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: bowChanceToNotLoseArrowReduceQuantityEveryLevel not set");
        }
        { //bowBaseAimAccuracy
            if (bowLevelStats.TryGetValue("bowBaseAimAccuracy", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: bowBaseAimAccuracy is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: bowBaseAimAccuracy is not double is {value.GetType()}");
                else bowBaseAimAccuracy = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: bowBaseAimAccuracy not set");
        }
        { //bowIncreaseAimAccuracyPerLevel
            if (bowLevelStats.TryGetValue("bowIncreaseAimAccuracyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: bowIncreaseAimAccuracyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: bowIncreaseAimAccuracyPerLevel is not double is {value.GetType()}");
                else bowIncreaseAimAccuracyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: bowIncreaseAimAccuracyPerLevel not set");
        }
        { //bowMaxLevel
            if (bowLevelStats.TryGetValue("bowMaxLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: bowMaxLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: bowMaxLevel is not int is {value.GetType()}");
                else bowMaxLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: bowMaxLevel not set");
        }

        // Get entity exp
        entityExpBow.Clear();
        Dictionary<string, object> tmpentityExpBow = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/entityexp",
            "bow",
            "levelup:config/entityexp/bow.json");
        foreach (KeyValuePair<string, object> pair in tmpentityExpBow)
        {
            if (pair.Value is long value) entityExpBow.Add(pair.Key, (int)value);
            else Debug.Log($"CONFIGURATION ERROR: entityExpBow {pair.Key} is not int");
        }

        Debug.Log("Bow configuration set");
    }

    public static int BowGetLevelByEXP(ulong exp)
    {
        double baseExp = bowEXPPerLevelBase;
        double multiplier = bowEXPMultiplyPerLevel;

        if (multiplier <= 1.0)
        {
            return (int)(exp / baseExp);
        }

        double expDouble = exp;

        double level = Math.Log((expDouble * (multiplier - 1) / baseExp) + 1) / Math.Log(multiplier);

        return Math.Max(0, (int)Math.Floor(level));
    }

    public static ulong BowGetExpByLevel(int level)
    {
        double baseExp = bowEXPPerLevelBase;
        double multiplier = bowEXPMultiplyPerLevel;

        if (multiplier == 1.0)
        {
            return (ulong)(baseExp * level);
        }

        double exp = baseExp * (Math.Pow(multiplier, level) - 1) / (multiplier - 1);
        return (ulong)Math.Floor(exp);
    }

    public static float BowGetDamageMultiplyByLevel(int level)
    {
        return bowBaseDamage + bowIncrementDamagePerLevel * level;
    }

    public static float BowGetChanceToNotLoseArrowByLevel(int level)
    {
        int totalLevels = level - 1;
        int reduceEvery = bowChanceToNotLoseArrowReduceIncreaseEveryLevel;
        float baseIncrement = bowChanceToNotLoseArrowBaseIncreasePerLevel;
        float reductionPerStep = bowChanceToNotLoseArrowReduceQuantityEveryLevel;

        int numberOfFullBlocks = totalLevels / reduceEvery;
        int remainingLevels = totalLevels % reduceEvery;

        float sumFullBlocks = numberOfFullBlocks * reduceEvery *
            (2 * baseIncrement - (numberOfFullBlocks - 1) * reductionPerStep) / 2;

        float currentIncrement = baseIncrement - numberOfFullBlocks * reductionPerStep;
        float sumRemaining = remainingLevels * currentIncrement;

        float finalChance = bowBaseChanceToNotLoseArrow + sumFullBlocks + sumRemaining;

        if (finalChance >= Random.Next(0, 100))
            return 1.0f;
        else
            return 0.0f;
    }

    public static float BowGetAimAccuracyByLevel(int level)
    {
        return bowBaseAimAccuracy + bowIncreaseAimAccuracyPerLevel * level;
    }

    #endregion

    #region knife
    public static readonly Dictionary<string, int> entityExpKnife = [];
    private static int knifeEXPPerHit = 1;
    private static int knifeEXPPerHarvest = 5;
    private static int knifeEXPPerBreaking = 1;
    private static int knifeEXPPerLevelBase = 10;
    private static double knifeEXPMultiplyPerLevel = 1.3;
    private static float knifeBaseDamage = 1.0f;
    private static float knifeIncrementDamagePerLevel = 0.1f;
    private static float knifeBaseHarvestMultiply = 0.5f;
    private static float knifeIncrementHarvestMultiplyPerLevel = 0.2f;
    private static float knifeBaseMiningSpeed = 1.0f;
    private static float knifeIncrementMiningSpeedMultiplyPerLevel = 0.1f;
    public static int knifeMaxLevel = 999;

    public static int ExpPerHitKnife => knifeEXPPerHit;
    public static int ExpPerHarvestKnife => knifeEXPPerHarvest;
    public static int ExpPerBreakingKnife => knifeEXPPerBreaking;

    public static void PopulateKnifeConfiguration(ICoreAPI api)
    {
        Dictionary<string, object> knifeLevelStats = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/levelstats",
            "knife",
            "levelup:config/levelstats/knife.json");

        { //knifeEXPPerLevelBase
            if (knifeLevelStats.TryGetValue("knifeEXPPerLevelBase", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: knifeEXPPerLevelBase is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: knifeEXPPerLevelBase is not int is {value.GetType()}");
                else knifeEXPPerLevelBase = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: knifeEXPPerLevelBase not set");
        }
        { //knifeEXPMultiplyPerLevel
            if (knifeLevelStats.TryGetValue("knifeEXPMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: knifeEXPMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: knifeEXPMultiplyPerLevel is not double is {value.GetType()}");
                else knifeEXPMultiplyPerLevel = (double)value;
            else Debug.LogError("CONFIGURATION ERROR: knifeEXPMultiplyPerLevel not set");
        }
        { //knifeBaseDamage
            if (knifeLevelStats.TryGetValue("knifeBaseDamage", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: knifeBaseDamage is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: knifeBaseDamage is not double is {value.GetType()}");
                else knifeBaseDamage = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: knifeBaseDamage not set");
        }
        { //knifeIncrementDamagePerLevel
            if (knifeLevelStats.TryGetValue("knifeIncrementDamagePerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: knifeIncrementDamagePerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: knifeIncrementDamagePerLevel is not double is {value.GetType()}");
                else knifeIncrementDamagePerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: knifeIncrementDamagePerLevel not set");
        }
        { //knifeEXPPerHit
            if (knifeLevelStats.TryGetValue("knifeEXPPerHit", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: knifeEXPPerHit is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: knifeEXPPerHit is not int is {value.GetType()}");
                else knifeEXPPerHit = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: knifeEXPPerHit not set");
            Experience.LoadExperience("Knife", "Hit", (ulong)knifeEXPPerHit);
        }
        { //knifeEXPPerHarvest
            if (knifeLevelStats.TryGetValue("knifeEXPPerHarvest", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: knifeEXPPerHarvest is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: knifeEXPPerHarvest is not int is {value.GetType()}");
                else knifeEXPPerHarvest = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: knifeEXPPerHarvest not set");
            Experience.LoadExperience("Knife", "Harvest", (ulong)knifeEXPPerHarvest);
        }
        { //knifeEXPPerBreaking
            if (knifeLevelStats.TryGetValue("knifeEXPPerBreaking", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: knifeEXPPerBreaking is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: knifeEXPPerBreaking is not int is {value.GetType()}");
                else knifeEXPPerBreaking = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: knifeEXPPerBreaking not set");
            Experience.LoadExperience("Knife", "Break", (ulong)knifeEXPPerBreaking);
        }
        { //knifeBaseHarvestMultiply
            if (knifeLevelStats.TryGetValue("knifeBaseHarvestMultiply", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: knifeBaseHarvestMultiply is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: knifeBaseHarvestMultiply is not double is {value.GetType()}");
                else knifeBaseHarvestMultiply = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: knifeBaseHarvestMultiply not set");
        }
        { //knifeIncrementHarvestMultiplyPerLevel
            if (knifeLevelStats.TryGetValue("knifeIncrementHarvestMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: knifeIncrementHarvestMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: knifeIncrementHarvestMultiplyPerLevel is not double is {value.GetType()}");
                else knifeIncrementHarvestMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: knifeIncrementHarvestMultiplyPerLevel not set");
        }
        { //knifeBaseMiningSpeed
            if (knifeLevelStats.TryGetValue("knifeBaseMiningSpeed", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: knifeBaseMiningSpeed is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: knifeBaseMiningSpeed is not double is {value.GetType()}");
                else knifeBaseMiningSpeed = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: knifeBaseMiningSpeed not set");
        }
        { //knifeIncrementMiningSpeedMultiplyPerLevel
            if (knifeLevelStats.TryGetValue("knifeIncrementMiningSpeedMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: knifeIncrementMiningSpeedMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: knifeIncrementMiningSpeedMultiplyPerLevel is not double is {value.GetType()}");
                else knifeIncrementMiningSpeedMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: knifeIncrementMiningSpeedMultiplyPerLevel not set");
        }
        { //knifeMaxLevel
            if (knifeLevelStats.TryGetValue("knifeMaxLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: knifeMaxLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: knifeMaxLevel is not int is {value.GetType()}");
                else knifeMaxLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: knifeMaxLevel not set");
        }

        // Get entity exp
        entityExpKnife.Clear();
        Dictionary<string, object> tmpentityExpKnife = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/entityexp",
            "knife",
            "levelup:config/entityexp/knife.json");
        foreach (KeyValuePair<string, object> pair in tmpentityExpKnife)
        {
            if (pair.Value is long value) entityExpKnife.Add(pair.Key, (int)value);
            else Debug.Log($"CONFIGURATION ERROR: entityExpKnife {pair.Key} is not int");
        }

        Debug.Log("Knife configuration set");
    }

    public static int KnifeGetLevelByEXP(ulong exp)
    {
        double baseExp = knifeEXPPerLevelBase;
        double multiplier = knifeEXPMultiplyPerLevel;

        if (multiplier <= 1.0)
        {
            return (int)(exp / baseExp);
        }

        double expDouble = exp;

        double level = Math.Log((expDouble * (multiplier - 1) / baseExp) + 1) / Math.Log(multiplier);

        return Math.Max(0, (int)Math.Floor(level));
    }

    public static ulong KnifeGetExpByLevel(int level)
    {
        double baseExp = knifeEXPPerLevelBase;
        double multiplier = knifeEXPMultiplyPerLevel;

        if (multiplier == 1.0)
        {
            return (ulong)(baseExp * level);
        }

        double exp = baseExp * (Math.Pow(multiplier, level) - 1) / (multiplier - 1);
        return (ulong)Math.Floor(exp);
    }

    public static float KnifeGetDamageMultiplyByLevel(int level)
    {
        return knifeBaseDamage + knifeIncrementDamagePerLevel * level;
    }

    public static float KnifeGetHarvestMultiplyByLevel(int level)
    {
        return knifeBaseHarvestMultiply * (1 + knifeIncrementHarvestMultiplyPerLevel * level);
    }

    public static float KnifeGetMiningMultiplyByLevel(int level)
    {
        float baseSpeed = knifeBaseMiningSpeed;
        float incrementSpeed = knifeIncrementMiningSpeedMultiplyPerLevel;

        float multiply = incrementSpeed * level;
        baseSpeed += baseSpeed * multiply;

        return baseSpeed;
    }
    #endregion

    #region axe
    public static readonly Dictionary<string, int> entityExpAxe = [];
    private static int axeEXPPerHit = 1;
    private static int axeEXPPerBreaking = 1;
    private static int axeEXPPerTreeBreaking = 10;

    private static int axeEXPPerLevelBase = 10;
    private static double axeEXPMultiplyPerLevel = 1.8;
    private static float axeBaseDamage = 1.0f;
    private static float axeIncrementDamagePerLevel = 0.1f;
    private static float axeBaseMiningSpeed = 1.0f;
    private static float axeIncrementMiningSpeedMultiplyPerLevel = 0.1f;
    public static int axeMaxLevel = 999;


    public static int ExpPerHitAxe => axeEXPPerHit;
    public static int ExpPerBreakingAxe => axeEXPPerBreaking;
    public static int ExpPerTreeBreakingAxe => axeEXPPerTreeBreaking;

    public static void PopulateAxeConfiguration(ICoreAPI api)
    {
        Dictionary<string, object> axeLevelStats = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/levelstats",
            "axe",
            "levelup:config/levelstats/axe.json");
        { //axeEXPPerLevelBase
            if (axeLevelStats.TryGetValue("axeEXPPerLevelBase", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: axeEXPPerLevelBase is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: axeEXPPerLevelBase is not int is {value.GetType()}");
                else axeEXPPerLevelBase = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: axeEXPPerLevelBase not set");
        }
        { //axeEXPMultiplyPerLevel
            if (axeLevelStats.TryGetValue("axeEXPMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: axeEXPMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: axeEXPMultiplyPerLevel is not double is {value.GetType()}");
                else axeEXPMultiplyPerLevel = (double)value;
            else Debug.LogError("CONFIGURATION ERROR: axeEXPMultiplyPerLevel not set");
        }
        { //axeBaseDamage
            if (axeLevelStats.TryGetValue("axeBaseDamage", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: axeBaseDamage is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: axeBaseDamage is not double is {value.GetType()}");
                else axeBaseDamage = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: axeBaseDamage not set");
        }
        { //axeIncrementDamagePerLevel
            if (axeLevelStats.TryGetValue("axeIncrementDamagePerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: axeIncrementDamagePerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: axeIncrementDamagePerLevel is not double is {value.GetType()}");
                else axeIncrementDamagePerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: axeIncrementDamagePerLevel not set");
        }
        { //axeEXPPerHit
            if (axeLevelStats.TryGetValue("axeEXPPerHit", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: axeEXPPerHit is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: axeEXPPerHit is not int is {value.GetType()}");
                else axeEXPPerHit = (int)(long)value;

            else Debug.LogError("CONFIGURATION ERROR: axeEXPPerHit not set");
            Experience.LoadExperience("Axe", "Hit", (ulong)axeEXPPerHit);
        }
        { //axeEXPPerBreaking
            if (axeLevelStats.TryGetValue("axeEXPPerBreaking", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: axeEXPPerBreaking is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: axeEXPPerBreaking is not int is {value.GetType()}");
                else axeEXPPerBreaking = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: axeEXPPerBreaking not set");
            Experience.LoadExperience("Axe", "Break", (ulong)axeEXPPerBreaking);
        }
        { //axeEXPPerTreeBreaking
            if (axeLevelStats.TryGetValue("axeEXPPerTreeBreaking", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: axeEXPPerTreeBreaking is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: axeEXPPerTreeBreaking is not int is {value.GetType()}");
                else axeEXPPerTreeBreaking = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: axeEXPPerTreeBreaking not set");
            Experience.LoadExperience("Axe", "TreeBreak", (ulong)axeEXPPerTreeBreaking);
        }
        { //axeBaseMiningSpeed
            if (axeLevelStats.TryGetValue("axeBaseMiningSpeed", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: axeBaseMiningSpeed is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: axeBaseMiningSpeed is not double is {value.GetType()}");
                else axeBaseMiningSpeed = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: axeBaseMiningSpeed not set");
        }
        { //axeIncrementMiningSpeedMultiplyPerLevel
            if (axeLevelStats.TryGetValue("axeIncrementMiningSpeedMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: axeIncrementMiningSpeedMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: axeIncrementMiningSpeedMultiplyPerLevel is not double is {value.GetType()}");
                else axeIncrementMiningSpeedMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: axeIncrementMiningSpeedMultiplyPerLevel not set");
        }
        { //axeMaxLevel
            if (axeLevelStats.TryGetValue("axeMaxLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: axeMaxLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: axeMaxLevel is not int is {value.GetType()}");
                else axeMaxLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: axeMaxLevel not set");
        }

        // Get entity exp
        entityExpAxe.Clear();
        Dictionary<string, object> tmpentityExpAxe = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/entityexp",
            "axe",
            "levelup:config/entityexp/axe.json");
        foreach (KeyValuePair<string, object> pair in tmpentityExpAxe)
        {
            if (pair.Value is long value) entityExpAxe.Add(pair.Key, (int)value);
            else Debug.Log($"CONFIGURATION ERROR: entityExpAxe {pair.Key} is not int");
        }

        Debug.Log("Axe configuration set");
    }

    public static int AxeGetLevelByEXP(ulong exp)
    {
        double baseExp = axeEXPPerLevelBase;
        double multiplier = axeEXPMultiplyPerLevel;

        if (multiplier <= 1.0)
        {
            return (int)(exp / baseExp);
        }

        double expDouble = exp;

        double level = Math.Log((expDouble * (multiplier - 1) / baseExp) + 1) / Math.Log(multiplier);

        return Math.Max(0, (int)Math.Floor(level));
    }

    public static ulong AxeGetExpByLevel(int level)
    {
        double baseExp = axeEXPPerLevelBase;
        double multiplier = axeEXPMultiplyPerLevel;

        if (multiplier == 1.0)
        {
            return (ulong)(baseExp * level);
        }

        double exp = baseExp * (Math.Pow(multiplier, level) - 1) / (multiplier - 1);
        return (ulong)Math.Floor(exp);
    }


    public static float AxeGetDamageMultiplyByLevel(int level)
    {
        return axeBaseDamage + axeIncrementDamagePerLevel * level;
    }

    public static float AxeGetMiningMultiplyByLevel(int level)
    {
        return axeBaseMiningSpeed * (1 + axeIncrementMiningSpeedMultiplyPerLevel * level);
    }
    #endregion

    #region pickaxe
    public static readonly Dictionary<string, int> entityExpPickaxe = [];
    public static readonly Dictionary<string, int> oresExpPickaxe = [];
    private static int pickaxeEXPPerHit = 1;
    private static int pickaxeEXPPerBreaking = 1;
    private static int pickaxeEXPPerLevelBase = 10;
    private static double pickaxeEXPMultiplyPerLevel = 2.0;
    private static float pickaxeBaseDamage = 1.0f;
    private static float pickaxeIncrementDamagePerLevel = 0.1f;
    private static float pickaxeBaseMiningSpeed = 1.0f;
    private static float pickaxeIncrementMiningSpeedMultiplyPerLevel = 0.1f;
    private static float pickaxeBaseOreMultiply = 0.5f;
    private static float pickaxeIncrementOreMultiplyPerLevel = 0.2f;
    public static int pickaxeMaxLevel = 999;


    public static int ExpPerHitPickaxe => pickaxeEXPPerHit;
    public static int ExpPerBreakingPickaxe => pickaxeEXPPerBreaking;

    public static void PopulatePickaxeConfiguration(ICoreAPI api)
    {
        Dictionary<string, object> pickaxeLevelStats = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/levelstats",
            "pickaxe",
            "levelup:config/levelstats/pickaxe.json");
        { //pickaxeEXPPerLevelBase
            if (pickaxeLevelStats.TryGetValue("pickaxeEXPPerLevelBase", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: pickaxeEXPPerLevelBase is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: pickaxeEXPPerLevelBase is not int is {value.GetType()}");
                else pickaxeEXPPerLevelBase = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: pickaxeEXPPerLevelBase not set");
        }
        { //pickaxeEXPMultiplyPerLevel
            if (pickaxeLevelStats.TryGetValue("pickaxeEXPMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: pickaxeEXPMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: pickaxeEXPMultiplyPerLevel is not double is {value.GetType()}");
                else pickaxeEXPMultiplyPerLevel = (double)value;
            else Debug.LogError("CONFIGURATION ERROR: pickaxeEXPMultiplyPerLevel not set");
        }
        { //pickaxeBaseDamage
            if (pickaxeLevelStats.TryGetValue("pickaxeBaseDamage", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: pickaxeBaseDamage is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: pickaxeBaseDamage is not double is {value.GetType()}");
                else pickaxeBaseDamage = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: pickaxeBaseDamage not set");
        }
        { //pickaxeIncrementDamagePerLevel
            if (pickaxeLevelStats.TryGetValue("pickaxeIncrementDamagePerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: pickaxeIncrementDamagePerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: pickaxeIncrementDamagePerLevel is not double is {value.GetType()}");
                else pickaxeIncrementDamagePerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: pickaxeIncrementDamagePerLevel not set");
        }
        { //pickaxeEXPPerHit
            if (pickaxeLevelStats.TryGetValue("pickaxeEXPPerHit", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: pickaxeEXPPerHit is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: pickaxeEXPPerHit is not int is {value.GetType()}");
                else pickaxeEXPPerHit = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: pickaxeEXPPerHit not set");
            Experience.LoadExperience("Pickaxe", "Hit", (ulong)pickaxeEXPPerHit);
        }
        { //pickaxeEXPPerBreaking
            if (pickaxeLevelStats.TryGetValue("pickaxeEXPPerBreaking", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: pickaxeEXPPerBreaking is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: pickaxeEXPPerBreaking is not int is {value.GetType()}");
                else pickaxeEXPPerBreaking = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: pickaxeEXPPerBreaking not set");
            Experience.LoadExperience("Pickaxe", "Break", (ulong)pickaxeEXPPerBreaking);
        }
        { //pickaxeBaseMiningSpeed
            if (pickaxeLevelStats.TryGetValue("pickaxeBaseMiningSpeed", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: pickaxeBaseMiningSpeed is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: pickaxeBaseMiningSpeed is not double is {value.GetType()}");
                else pickaxeBaseMiningSpeed = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: pickaxeBaseMiningSpeed not set");
        }
        { //pickaxeIncrementMiningSpeedMultiplyPerLevel
            if (pickaxeLevelStats.TryGetValue("pickaxeIncrementMiningSpeedMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: pickaxeIncrementMiningSpeedMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: pickaxeIncrementMiningSpeedMultiplyPerLevel is not double is {value.GetType()}");
                else pickaxeIncrementMiningSpeedMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: pickaxeIncrementMiningSpeedMultiplyPerLevel not set");
        }
        { //pickaxeBaseOreMultiply
            if (pickaxeLevelStats.TryGetValue("pickaxeBaseOreMultiply", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: pickaxeBaseOreMultiply is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: pickaxeBaseOreMultiply is not double is {value.GetType()}");
                else pickaxeBaseOreMultiply = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: pickaxeBaseOreMultiply not set");
        }
        { //pickaxeIncrementOreMultiplyPerLevel
            if (pickaxeLevelStats.TryGetValue("pickaxeIncrementOreMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: pickaxeIncrementOreMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: pickaxeIncrementOreMultiplyPerLevel is not double is {value.GetType()}");
                else pickaxeIncrementOreMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: pickaxeIncrementOreMultiplyPerLevel not set");
        }
        { //pickaxeMaxLevel
            if (pickaxeLevelStats.TryGetValue("pickaxeMaxLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: pickaxeMaxLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: pickaxeMaxLevel is not int is {value.GetType()}");
                else pickaxeMaxLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: pickaxeMaxLevel not set");
        }

        // Get entity exp
        entityExpPickaxe.Clear();
        Dictionary<string, object> tmpentityExpPickaxe = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/entityexp",
            "pickaxe",
            "levelup:config/entityexp/pickaxe.json");
        foreach (KeyValuePair<string, object> pair in tmpentityExpPickaxe)
        {
            if (pair.Value is long value) entityExpPickaxe.Add(pair.Key, (int)value);
            else Debug.Log($"CONFIGURATION ERROR: entityExpPickaxe {pair.Key} is not int");
        }

        // Get ores exp
        oresExpPickaxe.Clear();
        Dictionary<string, object> tmporesExpPickaxe = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/levelstats",
            "pickaxesores",
            "levelup:config/levelstats/pickaxesores.json");
        foreach (KeyValuePair<string, object> pair in tmporesExpPickaxe)
        {
            if (pair.Value is long value) oresExpPickaxe.Add(pair.Key, (int)value);
            else Debug.Log($"CONFIGURATION ERROR: oresExpPickaxe {pair.Key} is not int");
        }


        Debug.Log("Pickaxe configuration set");
    }

    public static int PickaxeGetLevelByEXP(ulong exp)
    {
        double baseExp = pickaxeEXPPerLevelBase;
        double multiplier = pickaxeEXPMultiplyPerLevel;

        if (multiplier <= 1.0)
        {
            return (int)(exp / baseExp);
        }

        double expDouble = exp;

        double level = Math.Log((expDouble * (multiplier - 1) / baseExp) + 1) / Math.Log(multiplier);

        return Math.Max(0, (int)Math.Floor(level));
    }

    public static ulong PickaxeGetExpByLevel(int level)
    {
        double baseExp = pickaxeEXPPerLevelBase;
        double multiplier = pickaxeEXPMultiplyPerLevel;

        if (multiplier == 1.0)
        {
            return (ulong)(baseExp * level);
        }

        double exp = baseExp * (Math.Pow(multiplier, level) - 1) / (multiplier - 1);
        return (ulong)Math.Floor(exp);
    }


    public static float PickaxeGetOreMultiplyByLevel(int level)
    {
        return pickaxeBaseOreMultiply * (1 + pickaxeIncrementOreMultiplyPerLevel * Math.Max(0, level - 1));
    }

    public static float PickaxeGetDamageMultiplyByLevel(int level)
    {
        return pickaxeBaseDamage + pickaxeIncrementDamagePerLevel * level;
    }

    public static float PickaxeGetMiningMultiplyByLevel(int level)
    {
        return pickaxeBaseMiningSpeed * (1 + pickaxeIncrementMiningSpeedMultiplyPerLevel * level);
    }
    #endregion

    #region shovel
    public static readonly Dictionary<string, int> entityExpShovel = [];
    private static int shovelEXPPerHit = 1;
    private static int shovelEXPPerBreaking = 1;
    private static int shovelEXPPerLevelBase = 10;
    private static double shovelEXPMultiplyPerLevel = 2.0;
    private static float shovelBaseDamage = 1.0f;
    private static float shovelIncrementDamagePerLevel = 0.1f;
    private static float shovelBaseMiningSpeed = 1.0f;
    private static float shovelIncrementMiningSpeedMultiplyPerLevel = 0.1f;
    public static int shovelMaxLevel = 999;


    public static int ExpPerHitShovel => shovelEXPPerHit;
    public static int ExpPerBreakingShovel => shovelEXPPerBreaking;

    public static void PopulateShovelConfiguration(ICoreAPI api)
    {
        Dictionary<string, object> shovelLevelStats = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/levelstats",
            "shovel",
            "levelup:config/levelstats/shovel.json");
        { //shovelEXPPerLevelBase
            if (shovelLevelStats.TryGetValue("shovelEXPPerLevelBase", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: shovelEXPPerLevelBase is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: shovelEXPPerLevelBase is not int is {value.GetType()}");
                else shovelEXPPerLevelBase = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: shovelEXPPerLevelBase not set");
        }
        { //shovelEXPMultiplyPerLevel
            if (shovelLevelStats.TryGetValue("shovelEXPMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: shovelEXPMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: shovelEXPMultiplyPerLevel is not double is {value.GetType()}");
                else shovelEXPMultiplyPerLevel = (double)value;
            else Debug.LogError("CONFIGURATION ERROR: shovelEXPMultiplyPerLevel not set");
        }
        { //shovelBaseDamage
            if (shovelLevelStats.TryGetValue("shovelBaseDamage", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: shovelBaseDamage is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: shovelBaseDamage is not double is {value.GetType()}");
                else shovelBaseDamage = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: shovelBaseDamage not set");
        }
        { //shovelIncrementDamagePerLevel
            if (shovelLevelStats.TryGetValue("shovelIncrementDamagePerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: shovelIncrementDamagePerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: shovelIncrementDamagePerLevel is not double is {value.GetType()}");
                else shovelIncrementDamagePerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: shovelIncrementDamagePerLevel not set");
        }
        { //shovelEXPPerHit
            if (shovelLevelStats.TryGetValue("shovelEXPPerHit", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: shovelEXPPerHit is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: shovelEXPPerHit is not int is {value.GetType()}");
                else shovelEXPPerHit = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: shovelEXPPerHit not set");
            Experience.LoadExperience("Shovel", "Hit", (ulong)shovelEXPPerHit);
        }
        { //shovelEXPPerBreaking
            if (shovelLevelStats.TryGetValue("shovelEXPPerBreaking", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: shovelEXPPerBreaking is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: shovelEXPPerBreaking is not int is {value.GetType()}");
                else shovelEXPPerBreaking = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: shovelEXPPerBreaking not set");
            Experience.LoadExperience("Shovel", "Break", (ulong)shovelEXPPerBreaking);
        }
        { //shovelBaseMiningSpeed
            if (shovelLevelStats.TryGetValue("shovelBaseMiningSpeed", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: shovelBaseMiningSpeed is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: shovelBaseMiningSpeed is not double is {value.GetType()}");
                else shovelBaseMiningSpeed = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: shovelBaseMiningSpeed not set");
        }
        { //shovelIncrementMiningSpeedMultiplyPerLevel
            if (shovelLevelStats.TryGetValue("shovelIncrementMiningSpeedMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: shovelIncrementMiningSpeedMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: shovelIncrementMiningSpeedMultiplyPerLevel is not double is {value.GetType()}");
                else shovelIncrementMiningSpeedMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: shovelIncrementMiningSpeedMultiplyPerLevel not set");
        }
        { //shovelMaxLevel
            if (shovelLevelStats.TryGetValue("shovelMaxLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: shovelMaxLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: shovelMaxLevel is not int is {value.GetType()}");
                else shovelMaxLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: shovelMaxLevel not set");
        }
        // Get entity exp
        entityExpShovel.Clear();
        Dictionary<string, object> tmpentityExpShovel = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/entityexp",
            "shovel",
            "levelup:config/entityexp/shovel.json");
        foreach (KeyValuePair<string, object> pair in tmpentityExpShovel)
        {
            if (pair.Value is long value) entityExpShovel.Add(pair.Key, (int)value);
            else Debug.Log($"CONFIGURATION ERROR: entityExpShovel {pair.Key} is not int");
        }

        Debug.Log("Shovel configuration set");
    }

    public static int ShovelGetLevelByEXP(ulong exp)
    {
        double baseExp = shovelEXPPerLevelBase;
        double multiplier = shovelEXPMultiplyPerLevel;

        if (multiplier <= 1.0)
        {
            return (int)(exp / baseExp);
        }

        double expDouble = exp;

        double level = Math.Log((expDouble * (multiplier - 1) / baseExp) + 1) / Math.Log(multiplier);

        return Math.Max(0, (int)Math.Floor(level));
    }

    public static ulong ShovelGetExpByLevel(int level)
    {
        double baseExp = shovelEXPPerLevelBase;
        double multiplier = shovelEXPMultiplyPerLevel;

        if (multiplier == 1.0)
        {
            return (ulong)(baseExp * level);
        }

        double exp = baseExp * (Math.Pow(multiplier, level) - 1) / (multiplier - 1);
        return (ulong)Math.Floor(exp);
    }


    public static float ShovelGetDamageMultiplyByLevel(int level)
    {
        return shovelBaseDamage + shovelIncrementDamagePerLevel * level;
    }

    public static float ShovelGetMiningMultiplyByLevel(int level)
    {
        return shovelBaseMiningSpeed * (1 + shovelIncrementMiningSpeedMultiplyPerLevel * level);
    }
    #endregion

    #region spear
    public static readonly Dictionary<string, int> entityExpSpear = [];
    private static int spearEXPPerHit = 1;
    private static int spearEXPPerThrow = 2;
    private static int spearEXPPerLevelBase = 10;
    private static double spearEXPMultiplyPerLevel = 1.5;
    private static float spearBaseDamage = 1.0f;
    private static float spearIncrementDamagePerLevel = 0.1f;
    private static float spearBaseAimAccuracy = 1.0f;
    private static float spearIncreaseAimAccuracyPerLevel = 0.5f;
    public static int spearMaxLevel = 999;


    public static int ExpPerHitSpear => spearEXPPerHit;
    public static int ExpPerThrowSpear => spearEXPPerThrow;

    public static void PopulateSpearConfiguration(ICoreAPI api)
    {
        Dictionary<string, object> spearLevelStats = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/levelstats",
            "spear",
            "levelup:config/levelstats/spear.json");
        { //spearEXPPerLevelBase
            if (spearLevelStats.TryGetValue("spearEXPPerLevelBase", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: spearEXPPerLevelBase is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: spearEXPPerLevelBase is not int is {value.GetType()}");
                else spearEXPPerLevelBase = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: spearEXPPerLevelBase not set");
        }
        { //spearEXPMultiplyPerLevel
            if (spearLevelStats.TryGetValue("spearEXPMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: spearEXPMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: spearEXPMultiplyPerLevel is not double is {value.GetType()}");
                else spearEXPMultiplyPerLevel = (double)value;
            else Debug.LogError("CONFIGURATION ERROR: spearEXPMultiplyPerLevel not set");
        }
        { //spearBaseDamage
            if (spearLevelStats.TryGetValue("spearBaseDamage", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: spearBaseDamage is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: spearBaseDamage is not double is {value.GetType()}");
                else spearBaseDamage = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: spearBaseDamage not set");
        }
        { //spearIncrementDamagePerLevel
            if (spearLevelStats.TryGetValue("spearIncrementDamagePerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: spearIncrementDamagePerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: spearIncrementDamagePerLevel is not double is {value.GetType()}");
                else spearIncrementDamagePerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: spearIncrementDamagePerLevel not set");
        }
        { //spearEXPPerHit
            if (spearLevelStats.TryGetValue("spearEXPPerHit", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: spearEXPPerHit is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: spearEXPPerHit is not int is {value.GetType()}");
                else spearEXPPerHit = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: spearEXPPerHit not set");
            Experience.LoadExperience("Spear", "Hit", (ulong)spearEXPPerHit);
        }
        { //spearEXPPerThrow
            if (spearLevelStats.TryGetValue("spearEXPPerThrow", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: spearEXPPerThrow is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: spearEXPPerThrow is not int is {value.GetType()}");
                else spearEXPPerThrow = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: spearEXPPerThrow not set");
            Experience.LoadExperience("Spear", "Throw", (ulong)spearEXPPerThrow);
        }
        { //spearBaseAimAccuracy
            if (spearLevelStats.TryGetValue("spearBaseAimAccuracy", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: spearBaseAimAccuracy is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: spearBaseAimAccuracy is not double is {value.GetType()}");
                else spearBaseAimAccuracy = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: spearBaseAimAccuracy not set");
        }
        { //spearIncreaseAimAccuracyPerLevel
            if (spearLevelStats.TryGetValue("spearIncreaseAimAccuracyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: spearIncreaseAimAccuracyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: spearIncreaseAimAccuracyPerLevel is not double is {value.GetType()}");
                else spearIncreaseAimAccuracyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: spearIncreaseAimAccuracyPerLevel not set");
        }
        { //spearMaxLevel
            if (spearLevelStats.TryGetValue("spearMaxLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: spearMaxLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: spearMaxLevel is not int is {value.GetType()}");
                else spearMaxLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: spearMaxLevel not set");
        }


        // Get entity exp
        entityExpSpear.Clear();
        Dictionary<string, object> tmpentityExpSpear = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/entityexp",
            "spear",
            "levelup:config/entityexp/spear.json");
        foreach (KeyValuePair<string, object> pair in tmpentityExpSpear)
        {
            if (pair.Value is long value) entityExpSpear.Add(pair.Key, (int)value);
            else Debug.Log($"CONFIGURATION ERROR: entityExpSpear {pair.Key} is not int");
        }
        Debug.Log("Spear configuration set");
    }

    public static int SpearGetLevelByEXP(ulong exp)
    {
        double baseExp = spearEXPPerLevelBase;
        double multiplier = spearEXPMultiplyPerLevel;

        if (multiplier <= 1.0)
        {
            return (int)(exp / baseExp);
        }

        double expDouble = exp;

        double level = Math.Log((expDouble * (multiplier - 1) / baseExp) + 1) / Math.Log(multiplier);

        return Math.Max(0, (int)Math.Floor(level));
    }

    public static ulong SpearGetExpByLevel(int level)
    {
        double baseExp = spearEXPPerLevelBase;
        double multiplier = spearEXPMultiplyPerLevel;

        if (multiplier == 1.0)
        {
            return (ulong)(baseExp * level);
        }

        double exp = baseExp * (Math.Pow(multiplier, level) - 1) / (multiplier - 1);
        return (ulong)Math.Floor(exp);
    }


    public static float SpearGetDamageMultiplyByLevel(int level)
    {
        return spearBaseDamage + spearIncrementDamagePerLevel * level;
    }

    public static float SpearGetAimAccuracyByLevel(int level)
    {
        return spearBaseAimAccuracy + spearIncreaseAimAccuracyPerLevel * level;
    }
    #endregion

    #region hammer
    public static readonly Dictionary<string, int> entityExpHammer = [];
    public static readonly Dictionary<string, string> smithChanceHammer = [];
    private static int hammerEXPPerHit = 1;
    private static int hammerEXPPerLevelBase = 10;
    private static double hammerEXPMultiplyPerLevel = 1.5;
    private static float hammerBaseDamage = 1.0f;
    private static float hammerIncrementDamagePerLevel = 0.1f;
    private static float hammerBaseSmithRetrieveChance = 0.0f;
    private static float hammerSmithRetrieveChancePerLevel = 2.0f;
    private static int hammerSmithRetrieveEveryLevelReduceChance = 10;
    private static float hammerSmithRetrieveReduceChanceForEveryLevel = 0.5f;
    private static float hammerBaseChanceToDouble = 0.0f;
    private static float hammerIncreaseChanceToDoublePerLevel = 2.0f;
    private static int hammerIncreaseChanceToDoublePerLevelReducerPerLevel = 5;
    private static float hammerIncreaseChanceToDoublePerLevelReducer = 0.2f;
    private static float hammerBaseChanceToTriple = 0.0f;
    private static float hammerIncreaseChanceToTriplePerLevel = 1.0f;
    private static int hammerIncreaseChanceToTriplePerLevelReducerPerLevel = 5;
    private static float hammerIncreaseChanceToTriplePerLevelReducer = 0.1f;
    private static float hammerBaseChanceToQuadruple = 0.0f;
    private static float hammerIncreaseChanceToQuadruplePerLevel = 0.5f;
    private static int hammerIncreaseChanceToQuadruplePerLevelReducerPerLevel = 5;
    private static float hammerIncreaseChanceToQuadruplePerLevelReducer = 0.05f;
    public static int hammerMaxLevel = 999;

    public static int ExpPerHitHammer => hammerEXPPerHit;

    public static void PopulateHammerConfiguration(ICoreAPI api)
    {
        Dictionary<string, object> hammerLevelStats = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/levelstats",
            "hammer",
            "levelup:config/levelstats/hammer.json");
        { //hammerEXPPerLevelBase
            if (hammerLevelStats.TryGetValue("hammerEXPPerLevelBase", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: hammerEXPPerLevelBase is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: hammerEXPPerLevelBase is not int is {value.GetType()}");
                else hammerEXPPerLevelBase = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: hammerEXPPerLevelBase not set");
        }
        { //hammerEXPMultiplyPerLevel
            if (hammerLevelStats.TryGetValue("hammerEXPMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: hammerEXPMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: hammerEXPMultiplyPerLevel is not double is {value.GetType()}");
                else hammerEXPMultiplyPerLevel = (double)value;
            else Debug.LogError("CONFIGURATION ERROR: hammerEXPMultiplyPerLevel not set");
        }
        { //hammerBaseDamage
            if (hammerLevelStats.TryGetValue("hammerBaseDamage", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: hammerBaseDamage is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: hammerBaseDamage is not double is {value.GetType()}");
                else hammerBaseDamage = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: hammerBaseDamage not set");
        }
        { //hammerIncrementDamagePerLevel
            if (hammerLevelStats.TryGetValue("hammerIncrementDamagePerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: hammerIncrementDamagePerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: hammerIncrementDamagePerLevel is not double is {value.GetType()}");
                else hammerIncrementDamagePerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: hammerIncrementDamagePerLevel not set");
        }
        { //hammerEXPPerHit
            if (hammerLevelStats.TryGetValue("hammerEXPPerHit", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: hammerEXPPerHit is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: hammerEXPPerHit is not int is {value.GetType()}");
                else hammerEXPPerHit = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: hammerEXPPerHit not set");
            Experience.LoadExperience("Hammer", "Hit", (ulong)hammerEXPPerHit);
        }
        { //hammerBaseSmithRetrieveChance
            if (hammerLevelStats.TryGetValue("hammerBaseSmithRetrieveChance", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: hammerBaseSmithRetrieveChance is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: hammerBaseSmithRetrieveChance is not double is {value.GetType()}");
                else hammerBaseSmithRetrieveChance = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: hammerBaseSmithRetrieveChance not set");
        }
        { //hammerSmithRetrieveChancePerLevel
            if (hammerLevelStats.TryGetValue("hammerSmithRetrieveChancePerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: hammerSmithRetrieveChancePerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: hammerSmithRetrieveChancePerLevel is not double is {value.GetType()}");
                else hammerSmithRetrieveChancePerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: hammerSmithRetrieveChancePerLevel not set");
        }
        { //hammerSmithRetrieveEveryLevelReduceChance
            if (hammerLevelStats.TryGetValue("hammerSmithRetrieveEveryLevelReduceChance", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: hammerSmithRetrieveEveryLevelReduceChance is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: hammerSmithRetrieveEveryLevelReduceChance is not int is {value.GetType()}");
                else hammerSmithRetrieveEveryLevelReduceChance = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: hammerSmithRetrieveEveryLevelReduceChance not set");
        }
        { //hammerSmithRetrieveReduceChanceForEveryLevel
            if (hammerLevelStats.TryGetValue("hammerSmithRetrieveReduceChanceForEveryLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: hammerSmithRetrieveReduceChanceForEveryLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: hammerSmithRetrieveReduceChanceForEveryLevel is not double is {value.GetType()}");
                else hammerSmithRetrieveReduceChanceForEveryLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: hammerSmithRetrieveReduceChanceForEveryLevel not set");
        }
        { //hammerBaseChanceToDouble
            if (hammerLevelStats.TryGetValue("hammerBaseChanceToDouble", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: hammerBaseChanceToDouble is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: hammerBaseChanceToDouble is not double is {value.GetType()}");
                else hammerBaseChanceToDouble = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: hammerBaseChanceToDouble not set");
        }
        { //hammerIncreaseChanceToDoublePerLevel
            if (hammerLevelStats.TryGetValue("hammerIncreaseChanceToDoublePerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: hammerIncreaseChanceToDoublePerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: hammerIncreaseChanceToDoublePerLevel is not double is {value.GetType()}");
                else hammerIncreaseChanceToDoublePerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: hammerIncreaseChanceToDoublePerLevel not set");
        }
        { //hammerIncreaseChanceToDoublePerLevelReducerPerLevel
            if (hammerLevelStats.TryGetValue("hammerIncreaseChanceToDoublePerLevelReducerPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: hammerIncreaseChanceToDoublePerLevelReducerPerLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: hammerIncreaseChanceToDoublePerLevelReducerPerLevel is not int is {value.GetType()}");
                else hammerIncreaseChanceToDoublePerLevelReducerPerLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: hammerIncreaseChanceToDoublePerLevelReducerPerLevel not set");
        }
        { //hammerIncreaseChanceToDoublePerLevelReducer
            if (hammerLevelStats.TryGetValue("hammerIncreaseChanceToDoublePerLevelReducer", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: hammerIncreaseChanceToDoublePerLevelReducer is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: hammerIncreaseChanceToDoublePerLevelReducer is not double is {value.GetType()}");
                else hammerIncreaseChanceToDoublePerLevelReducer = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: hammerIncreaseChanceToDoublePerLevelReducer not set");
        }
        { //hammerBaseChanceToTriple
            if (hammerLevelStats.TryGetValue("hammerBaseChanceToTriple", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: hammerBaseChanceToTriple is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: hammerBaseChanceToTriple is not double is {value.GetType()}");
                else hammerBaseChanceToTriple = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: hammerBaseChanceToTriple not set");
        }
        { //hammerIncreaseChanceToTriplePerLevel
            if (hammerLevelStats.TryGetValue("hammerIncreaseChanceToTriplePerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: hammerIncreaseChanceToTriplePerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: hammerIncreaseChanceToTriplePerLevel is not double is {value.GetType()}");
                else hammerIncreaseChanceToTriplePerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: hammerIncreaseChanceToTriplePerLevel not set");
        }
        { //hammerIncreaseChanceToTriplePerLevelReducerPerLevel
            if (hammerLevelStats.TryGetValue("hammerIncreaseChanceToTriplePerLevelReducerPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: hammerIncreaseChanceToTriplePerLevelReducerPerLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: hammerIncreaseChanceToTriplePerLevelReducerPerLevel is not int is {value.GetType()}");
                else hammerIncreaseChanceToTriplePerLevelReducerPerLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: hammerIncreaseChanceToTriplePerLevelReducerPerLevel not set");
        }
        { //hammerIncreaseChanceToTriplePerLevelReducer
            if (hammerLevelStats.TryGetValue("hammerIncreaseChanceToTriplePerLevelReducer", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: hammerIncreaseChanceToTriplePerLevelReducer is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: hammerIncreaseChanceToTriplePerLevelReducer is not double is {value.GetType()}");
                else hammerIncreaseChanceToTriplePerLevelReducer = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: hammerIncreaseChanceToTriplePerLevelReducer not set");
        }
        { //hammerBaseChanceToQuadruple
            if (hammerLevelStats.TryGetValue("hammerBaseChanceToQuadruple", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: hammerBaseChanceToQuadruple is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: hammerBaseChanceToQuadruple is not double is {value.GetType()}");
                else hammerBaseChanceToQuadruple = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: hammerBaseChanceToQuadruple not set");
        }
        { //hammerIncreaseChanceToQuadruplePerLevel
            if (hammerLevelStats.TryGetValue("hammerIncreaseChanceToQuadruplePerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: hammerIncreaseChanceToQuadruplePerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: hammerIncreaseChanceToQuadruplePerLevel is not double is {value.GetType()}");
                else hammerIncreaseChanceToQuadruplePerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: hammerIncreaseChanceToQuadruplePerLevel not set");
        }
        { //hammerIncreaseChanceToQuadruplePerLevelReducerPerLevel
            if (hammerLevelStats.TryGetValue("hammerIncreaseChanceToQuadruplePerLevelReducerPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: hammerIncreaseChanceToQuadruplePerLevelReducerPerLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: hammerIncreaseChanceToQuadruplePerLevelReducerPerLevel is not int is {value.GetType()}");
                else hammerIncreaseChanceToQuadruplePerLevelReducerPerLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: hammerIncreaseChanceToQuadruplePerLevelReducerPerLevel not set");
        }
        { //hammerIncreaseChanceToQuadruplePerLevelReducer
            if (hammerLevelStats.TryGetValue("hammerIncreaseChanceToQuadruplePerLevelReducer", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: hammerIncreaseChanceToQuadruplePerLevelReducer is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: hammerIncreaseChanceToQuadruplePerLevelReducer is not double is {value.GetType()}");
                else hammerIncreaseChanceToQuadruplePerLevelReducer = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: hammerIncreaseChanceToQuadruplePerLevelReducer not set");
        }
        { //hammerMaxLevel
            if (hammerLevelStats.TryGetValue("hammerMaxLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: hammerMaxLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: hammerMaxLevel is not int is {value.GetType()}");
                else hammerMaxLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: hammerMaxLevel not set");
        }

        // Get entity exp
        entityExpHammer.Clear();
        Dictionary<string, object> tmpentityExpHammer = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/entityexp",
            "hammer",
            "levelup:config/entityexp/hammer.json");
        foreach (KeyValuePair<string, object> pair in tmpentityExpHammer)
        {
            if (pair.Value is long value) entityExpHammer.Add(pair.Key, (int)value);
            else Debug.Log($"CONFIGURATION ERROR: entityExpHammer {pair.Key} is not int");
        }

        // Get smith chance
        smithChanceHammer.Clear();
        Dictionary<string, object> tmpsmithChanceHammer = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/levelstats",
            "hammersmiths",
            "levelup:config/levelstats/hammersmiths.json");
        foreach (KeyValuePair<string, object> pair in tmpsmithChanceHammer)
        {
            if (pair.Value is string value) smithChanceHammer.Add(pair.Key, value);
            else Debug.Log($"CONFIGURATION ERROR: smithChanceHammer {pair.Key} is not string");
        }
        Debug.Log("Hammer configuration set");
    }

    public static int HammerGetLevelByEXP(ulong exp)
    {
        double baseExp = hammerEXPPerLevelBase;
        double multiplier = hammerEXPMultiplyPerLevel;

        if (multiplier <= 1.0)
        {
            return (int)(exp / baseExp);
        }

        double expDouble = exp;

        double level = Math.Log((expDouble * (multiplier - 1) / baseExp) + 1) / Math.Log(multiplier);

        return Math.Max(0, (int)Math.Floor(level));
    }

    public static ulong HammerGetExpByLevel(int level)
    {
        double baseExp = hammerEXPPerLevelBase;
        double multiplier = hammerEXPMultiplyPerLevel;

        if (multiplier == 1.0)
        {
            return (ulong)(baseExp * level);
        }

        double exp = baseExp * (Math.Pow(multiplier, level) - 1) / (multiplier - 1);
        return (ulong)Math.Floor(exp);
    }


    public static float HammerGetDamageMultiplyByLevel(int level)
    {
        return hammerBaseDamage + hammerIncrementDamagePerLevel * level;
    }

    public static bool HammerShouldRetrieveSmithByLevel(int level)
    {
        int totalLevels = level - 1;
        int reduceEvery = hammerSmithRetrieveEveryLevelReduceChance;
        float baseChance = hammerBaseSmithRetrieveChance;
        float baseIncrement = hammerSmithRetrieveChancePerLevel;
        float reductionPerStep = hammerSmithRetrieveReduceChanceForEveryLevel;

        int fullBlocks = totalLevels / reduceEvery;
        int remainingLevels = totalLevels % reduceEvery;

        float sumFullBlocks = fullBlocks * reduceEvery *
            (2 * baseIncrement - (fullBlocks - 1) * reductionPerStep) / 2;

        float currentIncrement = baseIncrement - fullBlocks * reductionPerStep;
        float sumRemaining = remainingLevels * currentIncrement;

        float finalChance = baseChance + sumFullBlocks + sumRemaining;

        int chance = Random.Next(0, 100);

        if (enableExtendedLog)
            Debug.Log($"Hammer should retrieve smith mechanic check: {finalChance} : {chance}");

        return finalChance >= chance;
    }

    public static int HammerGetResultMultiplyByLevel(int level)
    {
        static float CalculateChance(
            int level,
            float baseChance,
            float incrementChancePerLevel,
            int reducerEveryLevels,
            float reducerAmountPerStep)
        {
            int totalLevels = level - 1;
            int fullBlocks = totalLevels / reducerEveryLevels;
            int remainingLevels = totalLevels % reducerEveryLevels;

            float sumFullBlocks = fullBlocks * reducerEveryLevels *
                (2 * incrementChancePerLevel - (fullBlocks - 1) * reducerAmountPerStep) / 2;

            float currentIncrement = incrementChancePerLevel - fullBlocks * reducerAmountPerStep;
            float sumRemaining = remainingLevels * currentIncrement;

            return baseChance + sumFullBlocks + sumRemaining;
        }

        // Quadruple
        float quadChance = CalculateChance(
            level,
            hammerBaseChanceToQuadruple,
            hammerIncreaseChanceToQuadruplePerLevel,
            hammerIncreaseChanceToQuadruplePerLevelReducerPerLevel,
            hammerIncreaseChanceToQuadruplePerLevelReducer);

        if (enableExtendedLog) Debug.Log($"Quadruple chance: {quadChance}");
        if (quadChance >= Random.Next(0, 100)) return 4;

        // Triple
        float tripleChance = CalculateChance(
            level,
            hammerBaseChanceToTriple,
            hammerIncreaseChanceToTriplePerLevel,
            hammerIncreaseChanceToTriplePerLevelReducerPerLevel,
            hammerIncreaseChanceToTriplePerLevelReducer);

        if (enableExtendedLog) Debug.Log($"Triple chance: {tripleChance}");
        if (tripleChance >= Random.Next(0, 100)) return 3;

        // Double
        float doubleChance = CalculateChance(
            level,
            hammerBaseChanceToDouble,
            hammerIncreaseChanceToDoublePerLevel,
            hammerIncreaseChanceToDoublePerLevelReducerPerLevel,
            hammerIncreaseChanceToDoublePerLevelReducer);

        if (enableExtendedLog) Debug.Log($"Double chance: {doubleChance}");
        if (doubleChance >= Random.Next(0, 100)) return 2;

        return 1;
    }

    #endregion

    #region sword
    public static readonly Dictionary<string, int> entityExpSword = [];
    private static int swordEXPPerHit = 1;
    private static int swordEXPPerLevelBase = 10;
    private static double swordEXPMultiplyPerLevel = 2.0;
    private static float swordBaseDamage = 1.0f;
    private static float swordIncrementDamagePerLevel = 0.1f;
    public static int swordMaxLevel = 999;


    public static int ExpPerHitSword => swordEXPPerHit;

    public static void PopulateSwordConfiguration(ICoreAPI api)
    {
        Dictionary<string, object> swordLevelStats = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/levelstats",
            "sword",
            "levelup:config/levelstats/sword.json");
        { //swordEXPPerLevelBase
            if (swordLevelStats.TryGetValue("swordEXPPerLevelBase", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: swordEXPPerLevelBase is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: swordEXPPerLevelBase is not int is {value.GetType()}");
                else swordEXPPerLevelBase = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: swordEXPPerLevelBase not set");
        }
        { //swordEXPMultiplyPerLevel
            if (swordLevelStats.TryGetValue("swordEXPMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: swordEXPMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: swordEXPMultiplyPerLevel is not double is {value.GetType()}");
                else swordEXPMultiplyPerLevel = (double)value;
            else Debug.LogError("CONFIGURATION ERROR: swordEXPMultiplyPerLevel not set");
        }
        { //swordBaseDamage
            if (swordLevelStats.TryGetValue("swordBaseDamage", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: swordBaseDamage is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: swordBaseDamage is not double is {value.GetType()}");
                else swordBaseDamage = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: swordBaseDamage not set");
        }
        { //swordIncrementDamagePerLevel
            if (swordLevelStats.TryGetValue("swordIncrementDamagePerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: swordIncrementDamagePerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: swordIncrementDamagePerLevel is not double is {value.GetType()}");
                else swordIncrementDamagePerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: swordIncrementDamagePerLevel not set");
        }
        { //swordEXPPerHit
            if (swordLevelStats.TryGetValue("swordEXPPerHit", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: swordEXPPerHit is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: swordEXPPerHit is not int is {value.GetType()}");
                else swordEXPPerHit = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: swordEXPPerHit not set");
            Experience.LoadExperience("Sword", "Hit", (ulong)swordEXPPerHit);
        }
        { //swordMaxLevel
            if (swordLevelStats.TryGetValue("swordMaxLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: swordMaxLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: swordMaxLevel is not int is {value.GetType()}");
                else swordMaxLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: swordMaxLevel not set");
        }

        // Get entity exp
        entityExpSword.Clear();
        Dictionary<string, object> tmpentityExpSword = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/entityexp",
            "sword",
            "levelup:config/entityexp/sword.json");
        foreach (KeyValuePair<string, object> pair in tmpentityExpSword)
        {
            if (pair.Value is long value) entityExpSword.Add(pair.Key, (int)value);
            else Debug.Log($"CONFIGURATION ERROR: entityExpSword {pair.Key} is not int");
        }

        Debug.Log("Hand configuration set");
    }

    public static int SwordGetLevelByEXP(ulong exp)
    {
        double baseExp = swordEXPPerLevelBase;
        double multiplier = swordEXPMultiplyPerLevel;

        if (multiplier <= 1.0)
        {
            return (int)(exp / baseExp);
        }

        double expDouble = exp;

        double level = Math.Log((expDouble * (multiplier - 1) / baseExp) + 1) / Math.Log(multiplier);

        return Math.Max(0, (int)Math.Floor(level));
    }

    public static ulong SwordGetExpByLevel(int level)
    {
        double baseExp = swordEXPPerLevelBase;
        double multiplier = swordEXPMultiplyPerLevel;

        if (multiplier == 1.0)
        {
            return (ulong)(baseExp * level);
        }

        double exp = baseExp * (Math.Pow(multiplier, level) - 1) / (multiplier - 1);
        return (ulong)Math.Floor(exp);
    }


    public static float SwordGetDamageMultiplyByLevel(int level)
    {
        return swordBaseDamage + swordIncrementDamagePerLevel * level;
    }
    #endregion

    #region shield
    private static int shieldEXPPerHit = 1;
    private static int shieldEXPPerLevelBase = 10;
    private static double shieldEXPMultiplyPerLevel = 2.0;
    private static float shieldBaseReduction = 1.0f;
    private static float shieldIncreamentReductionPerLevel = 0.1f;
    public static int shieldMaxLevel = 999;


    public static int ExpPerHitShield => shieldEXPPerHit;

    public static void PopulateShieldConfiguration(ICoreAPI api)
    {
        Dictionary<string, object> shieldLevelStats = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/levelstats",
            "shield",
            "levelup:config/levelstats/shield.json");
        { //shieldEXPPerLevelBase
            if (shieldLevelStats.TryGetValue("shieldEXPPerLevelBase", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: shieldEXPPerLevelBase is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: shieldEXPPerLevelBase is not int is {value.GetType()}");
                else shieldEXPPerLevelBase = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: shieldEXPPerLevelBase not set");
        }
        { //shieldEXPMultiplyPerLevel
            if (shieldLevelStats.TryGetValue("shieldEXPMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: shieldEXPMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: shieldEXPMultiplyPerLevel is not double is {value.GetType()}");
                else shieldEXPMultiplyPerLevel = (double)value;
            else Debug.LogError("CONFIGURATION ERROR: shieldEXPMultiplyPerLevel not set");
        }
        { //shieldBaseReduction
            if (shieldLevelStats.TryGetValue("shieldBaseReduction", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: shieldBaseReduction is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: shieldBaseReduction is not double is {value.GetType()}");
                else shieldBaseReduction = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: shieldBaseReduction not set");
        }
        { //shieldIncreamentReductionPerLevel
            if (shieldLevelStats.TryGetValue("shieldIncreamentReductionPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: shieldIncreamentReductionPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: shieldIncreamentReductionPerLevel is not double is {value.GetType()}");
                else shieldIncreamentReductionPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: shieldIncreamentReductionPerLevel not set");
        }
        { //shieldEXPPerHit
            if (shieldLevelStats.TryGetValue("shieldEXPPerHit", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: shieldEXPPerHit is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: shieldEXPPerHit is not int is {value.GetType()}");
                else shieldEXPPerHit = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: shieldEXPPerHit not set");
            Experience.LoadExperience("Shield", "Hit", (ulong)shieldEXPPerHit);
        }
        { //shieldMaxLevel
            if (shieldLevelStats.TryGetValue("shieldMaxLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: shieldMaxLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: shieldMaxLevel is not int is {value.GetType()}");
                else shieldMaxLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: shieldMaxLevel not set");
        }

        Debug.Log("Shield configuration set");
    }

    public static int ShieldGetLevelByEXP(ulong exp)
    {
        double baseExp = shieldEXPPerLevelBase;
        double multiplier = shieldEXPMultiplyPerLevel;

        if (multiplier <= 1.0)
        {
            return (int)(exp / baseExp);
        }

        double expDouble = exp;

        double level = Math.Log((expDouble * (multiplier - 1) / baseExp) + 1) / Math.Log(multiplier);

        return Math.Max(0, (int)Math.Floor(level));
    }

    public static ulong ShieldGetExpByLevel(int level)
    {
        double baseExp = shieldEXPPerLevelBase;
        double multiplier = shieldEXPMultiplyPerLevel;

        if (multiplier == 1.0)
        {
            return (ulong)(baseExp * level);
        }

        double exp = baseExp * (Math.Pow(multiplier, level) - 1) / (multiplier - 1);
        return (ulong)Math.Floor(exp);
    }


    public static float ShieldGetReductionMultiplyByLevel(int level)
    {
        return shieldBaseReduction + shieldIncreamentReductionPerLevel * Math.Max(0, level - 1);
    }
    #endregion

    #region hand
    public static readonly Dictionary<string, int> entityExpHand = [];
    private static int handEXPPerHit = 1;
    private static int handEXPPerLevelBase = 10;
    private static double handEXPMultiplyPerLevel = 2.0;
    private static float handBaseDamage = 1.0f;
    private static float handIncrementDamagePerLevel = 0.1f;
    public static int handMaxLevel = 999;

    public static int ExpPerHitHand => handEXPPerHit;

    public static void PopulateHandConfiguration(ICoreAPI api)
    {
        Dictionary<string, object> handLevelStats = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/levelstats",
            "hand",
            "levelup:config/levelstats/hand.json");
        { //handEXPPerLevelBase
            if (handLevelStats.TryGetValue("handEXPPerLevelBase", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: handEXPPerLevelBase is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: handEXPPerLevelBase is not int is {value.GetType()}");
                else handEXPPerLevelBase = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: handEXPPerLevelBase not set");
        }
        { //handEXPMultiplyPerLevel
            if (handLevelStats.TryGetValue("handEXPMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: handEXPMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: handEXPMultiplyPerLevel is not double is {value.GetType()}");
                else handEXPMultiplyPerLevel = (double)value;
            else Debug.LogError("CONFIGURATION ERROR: handEXPMultiplyPerLevel not set");
        }
        { //handBaseDamage
            if (handLevelStats.TryGetValue("handBaseDamage", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: handBaseDamage is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: handBaseDamage is not double is {value.GetType()}");
                else handBaseDamage = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: handBaseDamage not set");
        }
        { //handIncrementDamagePerLevel
            if (handLevelStats.TryGetValue("handIncrementDamagePerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: handIncrementDamagePerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: handIncrementDamagePerLevel is not double is {value.GetType()}");
                else handIncrementDamagePerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: handIncrementDamagePerLevel not set");
        }
        { //handEXPPerHit
            if (handLevelStats.TryGetValue("handEXPPerHit", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: handEXPPerHit is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: handEXPPerHit is not int is {value.GetType()}");
                else handEXPPerHit = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: handEXPPerHit not set");
            Experience.LoadExperience("Hand", "Hit", (ulong)handEXPPerHit);
        }
        { //handMaxLevel
            if (handLevelStats.TryGetValue("handMaxLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: handMaxLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: handMaxLevel is not int is {value.GetType()}");
                else handMaxLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: handMaxLevel not set");
        }

        // Get entity exp
        entityExpHand.Clear();
        Dictionary<string, object> tmpentityExpHand = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/entityexp",
            "hand",
            "levelup:config/entityexp/hand.json");
        foreach (KeyValuePair<string, object> pair in tmpentityExpHand)
        {
            if (pair.Value is long value) entityExpHand.Add(pair.Key, (int)value);
            else Debug.Log($"CONFIGURATION ERROR: entityExpHand {pair.Key} is not int");
        }

        Debug.Log("Hand configuration set");
    }

    public static int HandGetLevelByEXP(ulong exp)
    {
        double baseExp = handEXPPerLevelBase;
        double multiplier = handEXPMultiplyPerLevel;

        if (multiplier <= 1.0)
        {
            return (int)(exp / baseExp);
        }

        double expDouble = exp;

        double level = Math.Log((expDouble * (multiplier - 1) / baseExp) + 1) / Math.Log(multiplier);

        return Math.Max(0, (int)Math.Floor(level));
    }

    public static ulong HandGetExpByLevel(int level)
    {
        double baseExp = handEXPPerLevelBase;
        double multiplier = handEXPMultiplyPerLevel;

        if (multiplier == 1.0)
        {
            return (ulong)(baseExp * level);
        }

        double exp = baseExp * (Math.Pow(multiplier, level) - 1) / (multiplier - 1);
        return (ulong)Math.Floor(exp);
    }


    public static float HandGetDamageMultiplyByLevel(int level)
    {
        return handBaseDamage + handIncrementDamagePerLevel * level;
    }
    #endregion

    #region farming
    public static readonly Dictionary<string, int> expPerHarvestFarming = [];
    private static int farmingEXPPerTill = 1;
    private static int farmingEXPPerLevelBase = 10;
    private static double farmingEXPMultiplyPerLevel = 2.5;
    private static float farmingBaseHarvestMultiply = 0.5f;
    private static float farmingIncrementHarvestMultiplyPerLevel = 0.2f;
    private static float farmingBaseForageMultiply = 1.0f;
    private static float farmingIncrementForageMultiplyPerLevel = 0.2f;
    public static int farmingMaxLevel = 999;

    public static int ExpPerTillFarming => farmingEXPPerTill;

    public static void PopulateFarmingConfiguration(ICoreAPI api)
    {
        Dictionary<string, object> farmingLevelStats = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/levelstats",
            "farming",
            "levelup:config/levelstats/farming.json");
        { //farmingEXPPerLevelBase
            if (farmingLevelStats.TryGetValue("farmingEXPPerLevelBase", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: farmingEXPPerLevelBase is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: farmingEXPPerLevelBase is not int is {value.GetType()}");
                else farmingEXPPerLevelBase = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: farmingEXPPerLevelBase not set");
        }
        { //farmingEXPMultiplyPerLevel
            if (farmingLevelStats.TryGetValue("farmingEXPMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: farmingEXPMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: farmingEXPMultiplyPerLevel is not double is {value.GetType()}");
                else farmingEXPMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: farmingEXPMultiplyPerLevel not set");
        }
        { //farmingEXPPerTill
            if (farmingLevelStats.TryGetValue("farmingEXPPerTill", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: farmingEXPPerTill is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: farmingEXPPerTill is not int is {value.GetType()}");
                else farmingEXPPerTill = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: farmingEXPPerTill not set");
            Experience.LoadExperience("Farming", "Till", (ulong)farmingEXPPerTill);
        }
        { //farmingBaseHarvestMultiply
            if (farmingLevelStats.TryGetValue("farmingBaseHarvestMultiply", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: farmingBaseHarvestMultiply is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: farmingBaseHarvestMultiply is not double is {value.GetType()}");
                else farmingBaseHarvestMultiply = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: farmingBaseHarvestMultiply not set");
        }
        { //farmingIncrementHarvestMultiplyPerLevel
            if (farmingLevelStats.TryGetValue("farmingIncrementHarvestMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: farmingIncrementHarvestMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: farmingIncrementHarvestMultiplyPerLevel is not double is {value.GetType()}");
                else farmingIncrementHarvestMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: farmingIncrementHarvestMultiplyPerLevel not set");
        }
        { //farmingBaseForageMultiply
            if (farmingLevelStats.TryGetValue("farmingBaseForageMultiply", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: farmingBaseForageMultiply is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: farmingBaseForageMultiply is not double is {value.GetType()}");
                else farmingBaseForageMultiply = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: farmingBaseForageMultiply not set");
        }
        { //farmingIncrementForageMultiplyPerLevel
            if (farmingLevelStats.TryGetValue("farmingIncrementForageMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: farmingIncrementForageMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: farmingIncrementForageMultiplyPerLevel is not double is {value.GetType()}");
                else farmingIncrementForageMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: farmingIncrementForageMultiplyPerLevel not set");
        }
        { //farmingMaxLevel
            if (farmingLevelStats.TryGetValue("farmingMaxLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: farmingMaxLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: farmingMaxLevel is not int is {value.GetType()}");
                else farmingMaxLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: farmingMaxLevel not set");
        }


        // Get crop exp
        expPerHarvestFarming.Clear();
        Dictionary<string, object> tmpexpPerHarvestFarming = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/levelstats",
            "farmingcrops",
            "levelup:config/levelstats/farmingcrops.json");
        foreach (KeyValuePair<string, object> pair in tmpexpPerHarvestFarming)
        {
            if (pair.Value is long value) expPerHarvestFarming.Add(pair.Key, (int)value);
            else Debug.Log($"CONFIGURATION ERROR: expPerHarvestFarming {pair.Key} is not int");
        }

        Debug.Log("Farming configuration set");
    }

    public static int FarmingGetLevelByEXP(ulong exp)
    {
        double baseExp = farmingEXPPerLevelBase;
        double multiplier = farmingEXPMultiplyPerLevel;

        if (multiplier <= 1.0)
        {
            return (int)(exp / baseExp);
        }

        double expDouble = exp;

        double level = Math.Log((expDouble * (multiplier - 1) / baseExp) + 1) / Math.Log(multiplier);

        return Math.Max(0, (int)Math.Floor(level));
    }

    public static ulong FarmingGetExpByLevel(int level)
    {
        double baseExp = farmingEXPPerLevelBase;
        double multiplier = farmingEXPMultiplyPerLevel;

        if (multiplier == 1.0)
        {
            return (ulong)(baseExp * level);
        }

        double exp = baseExp * (Math.Pow(multiplier, level) - 1) / (multiplier - 1);
        return (ulong)Math.Floor(exp);
    }


    public static float FarmingGetHarvestMultiplyByLevel(int level)
    {
        return farmingBaseHarvestMultiply * (1 + farmingIncrementHarvestMultiplyPerLevel * level);
    }

    public static float FarmingGetForageMultiplyByLevel(int level)
    {
        return farmingBaseForageMultiply * (1 + farmingIncrementForageMultiplyPerLevel * level);
    }
    #endregion

    #region cooking
    public static readonly Dictionary<string, double> expMultiplySingleCooking = [];
    public static readonly Dictionary<string, double> expMultiplyPotsCooking = [];
    private static int cookingBaseExpPerCooking = 3;
    private static int cookingEXPPerLevelBase = 10;
    private static double cookingEXPMultiplyPerLevel = 1.3;
    private static float cookingBaseFreshHoursMultiply = 1.0f;
    private static float cookingFreshHoursMultiplyPerLevel = 0.1f;
    private static float cookingBaseChanceToIncreaseServings = 1.0f;
    private static float cookingIncrementChanceToIncreaseServings = 2.0f;
    private static float cookingEveryChanceToIncreaseServingsReduceChance = 20.0f;
    private static float cookingChanceToIncreaseServingsReducerTotal = 0.5f;
    private static int cookingBaseRollsChanceToIncreaseServings = 1;
    private static int cookingEarnRollsChanceToIncreaseServingsEveryLevel = 5;
    private static int cookingEarnRollsChanceToIncreaseServingsQuantity = 1;
    public static int cookingMaxLevel = 999;

    public static int ExpPerCookingcooking => cookingBaseExpPerCooking;

    public static void PopulateCookingConfiguration(ICoreAPI api)
    {
        Dictionary<string, object> cookingLevelStats = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/levelstats",
            "cooking",
            "levelup:config/levelstats/cooking.json");
        { //cookingBaseExpPerCooking
            if (cookingLevelStats.TryGetValue("cookingBaseExpPerCooking", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: cookingBaseExpPerCooking is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: cookingBaseExpPerCooking is not int is {value.GetType()}");
                else cookingBaseExpPerCooking = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: cookingBaseExpPerCooking not set");
            Experience.LoadExperience("Cooking", "Cooking", (ulong)cookingBaseExpPerCooking);
        }
        { //cookingEXPPerLevelBase
            if (cookingLevelStats.TryGetValue("cookingEXPPerLevelBase", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: cookingEXPPerLevelBase is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: cookingEXPPerLevelBase is not int is {value.GetType()}");
                else cookingEXPPerLevelBase = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: cookingEXPPerLevelBase not set");
        }
        { //cookingEXPMultiplyPerLevel
            if (cookingLevelStats.TryGetValue("cookingEXPMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: cookingEXPMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: cookingEXPMultiplyPerLevel is not double is {value.GetType()}");
                else cookingEXPMultiplyPerLevel = (double)value;
            else Debug.LogError("CONFIGURATION ERROR: cookingEXPMultiplyPerLevel not set");
        }
        { //cookingBaseFreshHoursMultiply
            if (cookingLevelStats.TryGetValue("cookingBaseFreshHoursMultiply", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: cookingBaseFreshHoursMultiply is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: cookingBaseFreshHoursMultiply is not double is {value.GetType()}");
                else cookingBaseFreshHoursMultiply = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: cookingBaseFreshHoursMultiply not set");
        }
        { //cookingFreshHoursMultiplyPerLevel
            if (cookingLevelStats.TryGetValue("cookingFreshHoursMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: cookingFreshHoursMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: cookingFreshHoursMultiplyPerLevel is not double is {value.GetType()}");
                else cookingFreshHoursMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: cookingFreshHoursMultiplyPerLevel not set");
        }
        { //cookingBaseChanceToIncreaseServings
            if (cookingLevelStats.TryGetValue("cookingBaseChanceToIncreaseServings", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: cookingBaseChanceToIncreaseServings is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: cookingBaseChanceToIncreaseServings is not double is {value.GetType()}");
                else cookingBaseChanceToIncreaseServings = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: cookingBaseChanceToIncreaseServings not set");
        }
        { //cookingIncrementChanceToIncreaseServings
            if (cookingLevelStats.TryGetValue("cookingIncrementChanceToIncreaseServings", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: cookingIncrementChanceToIncreaseServings is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: cookingIncrementChanceToIncreaseServings is not double is {value.GetType()}");
                else cookingIncrementChanceToIncreaseServings = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: cookingIncrementChanceToIncreaseServings not set");
        }
        { //cookingEveryChanceToIncreaseServingsReduceChance
            if (cookingLevelStats.TryGetValue("cookingEveryChanceToIncreaseServingsReduceChance", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: cookingEveryChanceToIncreaseServingsReduceChance is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: cookingEveryChanceToIncreaseServingsReduceChance is not double is {value.GetType()}");
                else cookingEveryChanceToIncreaseServingsReduceChance = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: cookingEveryChanceToIncreaseServingsReduceChance not set");
        }
        { //cookingChanceToIncreaseServingsReducerTotal
            if (cookingLevelStats.TryGetValue("cookingChanceToIncreaseServingsReducerTotal", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: cookingChanceToIncreaseServingsReducerTotal is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: cookingChanceToIncreaseServingsReducerTotal is not double is {value.GetType()}");
                else cookingChanceToIncreaseServingsReducerTotal = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: cookingChanceToIncreaseServingsReducerTotal not set");
        }
        { //cookingBaseRollsChanceToIncreaseServings
            if (cookingLevelStats.TryGetValue("cookingBaseRollsChanceToIncreaseServings", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: cookingBaseRollsChanceToIncreaseServings is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: cookingBaseRollsChanceToIncreaseServings is not int is {value.GetType()}");
                else cookingBaseRollsChanceToIncreaseServings = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: cookingBaseRollsChanceToIncreaseServings not set");
        }
        { //cookingEarnRollsChanceToIncreaseServingsEveryLevel
            if (cookingLevelStats.TryGetValue("cookingEarnRollsChanceToIncreaseServingsEveryLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: cookingEarnRollsChanceToIncreaseServingsEveryLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: cookingEarnRollsChanceToIncreaseServingsEveryLevel is not int is {value.GetType()}");
                else cookingEarnRollsChanceToIncreaseServingsEveryLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: cookingEarnRollsChanceToIncreaseServingsEveryLevel not set");
        }
        { //cookingEarnRollsChanceToIncreaseServingsQuantity
            if (cookingLevelStats.TryGetValue("cookingEarnRollsChanceToIncreaseServingsQuantity", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: cookingEarnRollsChanceToIncreaseServingsQuantity is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: cookingEarnRollsChanceToIncreaseServingsQuantity is not int is {value.GetType()}");
                else cookingEarnRollsChanceToIncreaseServingsQuantity = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: cookingEarnRollsChanceToIncreaseServingsQuantity not set");
        }
        { //cookingMaxLevel
            if (cookingLevelStats.TryGetValue("cookingMaxLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: cookingMaxLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: cookingMaxLevel is not int is {value.GetType()}");
                else cookingMaxLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: cookingMaxLevel not set");
        }

        // Get single food exp multiply
        expMultiplySingleCooking.Clear();
        Dictionary<string, object> tmpexpMultiplySingleCooking = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/levelstats",
            "cookingsingles",
            "levelup:config/levelstats/cookingsingles.json");
        foreach (KeyValuePair<string, object> pair in tmpexpMultiplySingleCooking)
        {
            if (pair.Value is double value) expMultiplySingleCooking.Add(pair.Key, value);
            else Debug.Log($"CONFIGURATION ERROR: expMultiplySingleCooking {pair.Key} is not double");
        }
        // Get pots food exp multiply
        expMultiplyPotsCooking.Clear();
        Dictionary<string, object> tmpexpMultiplyPotsCooking = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/levelstats",
            "cookingpots",
            "levelup:config/levelstats/cookingpots.json");
        foreach (KeyValuePair<string, object> pair in tmpexpMultiplyPotsCooking)
        {
            if (pair.Value is double value) expMultiplyPotsCooking.Add(pair.Key, value);
            else Debug.Log($"CONFIGURATION ERROR: expMultiplyPotsCooking {pair.Key} is not double");
        }

        Debug.Log("Cooking configuration set");
    }

    public static int CookingGetLevelByEXP(ulong exp)
    {
        double baseExp = cookingEXPPerLevelBase;
        double multiplier = cookingEXPMultiplyPerLevel;

        if (multiplier <= 1.0)
        {
            return (int)(exp / baseExp);
        }

        double expDouble = exp;

        double level = Math.Log((expDouble * (multiplier - 1) / baseExp) + 1) / Math.Log(multiplier);

        return Math.Max(0, (int)Math.Floor(level));
    }

    public static ulong CookingGetExpByLevel(int level)
    {
        double baseExp = cookingEXPPerLevelBase;
        double multiplier = cookingEXPMultiplyPerLevel;

        if (multiplier == 1.0)
        {
            return (ulong)(baseExp * level);
        }

        double exp = baseExp * (Math.Pow(multiplier, level) - 1) / (multiplier - 1);
        return (ulong)Math.Floor(exp);
    }


    public static float CookingGetFreshHoursMultiplyByLevel(int level)
    {
        return cookingBaseFreshHoursMultiply * (1 + cookingFreshHoursMultiplyPerLevel * level);
    }

    public static int CookingGetServingsByLevelAndServings(int level, int quantityServings)
    {
        float chanceToIncrease = cookingBaseChanceToIncreaseServings;
        float incrementChance = cookingIncrementChanceToIncreaseServings;
        int rolls = cookingBaseRollsChanceToIncreaseServings;

        int levelsToProcess = level - 1;

        float levelsToProcessInt = levelsToProcess;
        float reduceSteps = levelsToProcessInt / cookingEveryChanceToIncreaseServingsReduceChance;
        float remainingLevels = levelsToProcessInt % cookingEveryChanceToIncreaseServingsReduceChance;

        float sumReducedIncrements = reduceSteps * cookingEveryChanceToIncreaseServingsReduceChance *
            (2 * incrementChance - (reduceSteps - 1) * cookingChanceToIncreaseServingsReducerTotal) / 2;

        float currentIncrement = incrementChance - reduceSteps * cookingChanceToIncreaseServingsReducerTotal;
        float sumRemaining = remainingLevels * currentIncrement;

        chanceToIncrease += sumReducedIncrements + sumRemaining;

        rolls += levelsToProcess / cookingEarnRollsChanceToIncreaseServingsEveryLevel * cookingEarnRollsChanceToIncreaseServingsQuantity;

        for (int i = 0; i < rolls; i++)
        {
            if (chanceToIncrease >= Random.Next(0, 100))
                quantityServings += 1;
        }

        return quantityServings;
    }
    #endregion

    #region panning
    private static int panningBaseExpPerPanning = 3;
    private static int panningEXPPerLevelBase = 10;
    private static double panningEXPMultiplyPerLevel = 1.3;
    private static float panningBaseLootMultiply = 0.0f;
    private static float panningLootMultiplyPerLevel = 0.1f;
    private static float panningBaseChanceToDoubleLoot = 0.0f;
    private static float panningChanceToDoubleLootPerLevel = 1.0f;
    private static float panningBaseChanceToTripleLoot = 0.0f;
    private static float panningChanceToTripleLootPerLevel = 0.5f;
    private static float panningBaseChanceToQuadrupleLoot = 0.0f;
    private static float panningChanceToQuadrupleLootPerLevel = 0.3f;
    public static int panningMaxLevel = 999;

    public static int ExpPerPanning => panningBaseExpPerPanning;

    public static void PopulatePanningConfiguration(ICoreAPI api)
    {
        Dictionary<string, object> panningLevelStats = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/levelstats",
            "panning",
            "levelup:config/levelstats/panning.json");
        { //panningBaseExpPerPanning
            if (panningLevelStats.TryGetValue("panningBaseExpPerPanning", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: panningBaseExpPerPanning is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: panningBaseExpPerPanning is not int is {value.GetType()}");
                else panningBaseExpPerPanning = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: panningBaseExpPerPanning not set");
            Experience.LoadExperience("Panning", "Panning", (ulong)panningBaseExpPerPanning);
        }
        { //panningEXPPerLevelBase
            if (panningLevelStats.TryGetValue("panningEXPPerLevelBase", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: panningEXPPerLevelBase is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: panningEXPPerLevelBase is not int is {value.GetType()}");
                else panningEXPPerLevelBase = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: panningEXPPerLevelBase not set");
        }
        { //panningEXPMultiplyPerLevel
            if (panningLevelStats.TryGetValue("panningEXPMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: panningEXPMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: panningEXPMultiplyPerLevel is not double is {value.GetType()}");
                else panningEXPMultiplyPerLevel = (double)value;
            else Debug.LogError("CONFIGURATION ERROR: panningEXPMultiplyPerLevel not set");
        }
        { //panningBaseLootMultiply
            if (panningLevelStats.TryGetValue("panningBaseLootMultiply", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: panningBaseLootMultiply is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: panningBaseLootMultiply is not double is {value.GetType()}");
                else panningBaseLootMultiply = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: panningBaseLootMultiply not set");
        }
        { //panningLootMultiplyPerLevel
            if (panningLevelStats.TryGetValue("panningLootMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: panningLootMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: panningLootMultiplyPerLevel is not double is {value.GetType()}");
                else panningLootMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: panningLootMultiplyPerLevel not set");
        }
        { //panningBaseChanceToDoubleLoot
            if (panningLevelStats.TryGetValue("panningBaseChanceToDoubleLoot", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: panningBaseChanceToDoubleLoot is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: panningBaseChanceToDoubleLoot is not double is {value.GetType()}");
                else panningBaseChanceToDoubleLoot = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: panningBaseChanceToDoubleLoot not set");
        }
        { //panningChanceToDoubleLootPerLevel
            if (panningLevelStats.TryGetValue("panningChanceToDoubleLootPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: panningChanceToDoubleLootPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: panningChanceToDoubleLootPerLevel is not double is {value.GetType()}");
                else panningChanceToDoubleLootPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: panningChanceToDoubleLootPerLevel not set");
        }
        { //panningBaseChanceToTripleLoot
            if (panningLevelStats.TryGetValue("panningBaseChanceToTripleLoot", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: panningBaseChanceToTripleLoot is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: panningBaseChanceToTripleLoot is not double is {value.GetType()}");
                else panningBaseChanceToTripleLoot = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: panningBaseChanceToTripleLoot not set");
        }
        { //panningChanceToTripleLootPerLevel
            if (panningLevelStats.TryGetValue("panningChanceToTripleLootPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: panningChanceToTripleLootPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: panningChanceToTripleLootPerLevel is not double is {value.GetType()}");
                else panningChanceToTripleLootPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: panningChanceToTripleLootPerLevel not set");
        }
        { //panningBaseChanceToQuadrupleLoot
            if (panningLevelStats.TryGetValue("panningBaseChanceToQuadrupleLoot", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: panningBaseChanceToQuadrupleLoot is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: panningBaseChanceToQuadrupleLoot is not double is {value.GetType()}");
                else panningBaseChanceToQuadrupleLoot = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: panningBaseChanceToQuadrupleLoot not set");
        }
        { //panningChanceToQuadrupleLootPerLevel
            if (panningLevelStats.TryGetValue("panningChanceToQuadrupleLootPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: panningChanceToQuadrupleLootPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: panningChanceToQuadrupleLootPerLevel is not double is {value.GetType()}");
                else panningChanceToQuadrupleLootPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: panningChanceToQuadrupleLootPerLevel not set");
        }
        { //panningMaxLevel
            if (panningLevelStats.TryGetValue("panningMaxLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: panningMaxLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: panningMaxLevel is not int is {value.GetType()}");
                else panningMaxLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: panningMaxLevel not set");
        }
        Debug.Log("Panning configuration set");
    }

    public static int PanningGetLevelByEXP(ulong exp)
    {
        double baseExp = panningEXPPerLevelBase;
        double multiplier = panningEXPMultiplyPerLevel;

        if (multiplier <= 1.0)
        {
            return (int)(exp / baseExp);
        }

        double expDouble = exp;

        double level = Math.Log((expDouble * (multiplier - 1) / baseExp) + 1) / Math.Log(multiplier);

        return Math.Max(0, (int)Math.Floor(level));
    }

    public static ulong PanningGetExpByLevel(int level)
    {
        double baseExp = panningEXPPerLevelBase;
        double multiplier = panningEXPMultiplyPerLevel;

        if (multiplier == 1.0)
        {
            return (ulong)(baseExp * level);
        }

        double exp = baseExp * (Math.Pow(multiplier, level) - 1) / (multiplier - 1);
        return (ulong)Math.Floor(exp);
    }


    public static float PanningGetLootMultiplyByLevel(int level)
    {
        return panningBaseLootMultiply * (1 + panningLootMultiplyPerLevel * level);
    }

    public static int PanningGetLootQuantityMultiplyByLevel(int level)
    {
        double chanceToDouble = panningBaseChanceToDoubleLoot + panningChanceToDoubleLootPerLevel * level;
        double chanceToTriple = panningBaseChanceToTripleLoot + panningChanceToTripleLootPerLevel * level;
        double chanceToQuadruple = panningBaseChanceToQuadrupleLoot + panningChanceToQuadrupleLootPerLevel * level;

        double roll = Random.NextDouble();

        if (roll <= chanceToQuadruple) return 4;
        if (roll <= chanceToTriple) return 3;
        if (roll <= chanceToDouble) return 2;
        return 1;
    }
    #endregion

    #region smithing
    public static readonly Dictionary<string, int> expPerCraftSmithing = [];
    private static int smithingEXPPerLevelBase = 10;
    private static double smithingEXPMultiplyPerLevel = 2.5;
    private static float smithingBaseDurabilityMultiply = 1.0f;
    private static float smithingIncrementDurabilityMultiplyPerLevel = 0.05f;
    private static float smithingBaseAttackPowerMultiply = 1.0f;
    private static float smithingIncrementAttackPowerMultiplyPerLevel = 0.05f;
    private static float smithingBaseMiningSpeedMultiply = 1.0f;
    private static float smithingIncrementMiningSpeedMultiplyPerLevel = 0.05f;
    private static float smithingBaseArmorProtectionMultiply = 1.0f;
    private static float smithingIncrementArmorProtectionMultiplyPerLevel = 0.05f;
    public static int smithingMaxLevel = 999;

    public static void PopulateSmithingConfiguration(ICoreAPI api)
    {
        Dictionary<string, object> smithingLevelStats = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/levelstats",
            "smithing",
            "levelup:config/levelstats/smithing.json");
        { //smithingEXPPerLevelBase
            if (smithingLevelStats.TryGetValue("smithingEXPPerLevelBase", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: smithingEXPPerLevelBase is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: smithingEXPPerLevelBase is not int is {value.GetType()}");
                else smithingEXPPerLevelBase = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: smithingEXPPerLevelBase not set");
        }
        { //smithingEXPMultiplyPerLevel
            if (smithingLevelStats.TryGetValue("smithingEXPMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: smithingEXPMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: smithingEXPMultiplyPerLevel is not double is {value.GetType()}");
                else smithingEXPMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: smithingEXPMultiplyPerLevel not set");
        }
        { //smithingBaseDurabilityMultiply
            if (smithingLevelStats.TryGetValue("smithingBaseDurabilityMultiply", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: smithingBaseDurabilityMultiply is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: smithingBaseDurabilityMultiply is not double is {value.GetType()}");
                else smithingBaseDurabilityMultiply = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: smithingBaseDurabilityMultiply not set");
        }
        { //smithingIncrementDurabilityMultiplyPerLevel
            if (smithingLevelStats.TryGetValue("smithingIncrementDurabilityMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: smithingIncrementDurabilityMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: smithingIncrementDurabilityMultiplyPerLevel is not double is {value.GetType()}");
                else smithingIncrementDurabilityMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: smithingIncrementDurabilityMultiplyPerLevel not set");
        }
        { //smithingBaseAttackPowerMultiply
            if (smithingLevelStats.TryGetValue("smithingBaseAttackPowerMultiply", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: smithingBaseAttackPowerMultiply is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: smithingBaseAttackPowerMultiply is not double is {value.GetType()}");
                else smithingBaseAttackPowerMultiply = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: smithingBaseAttackPowerMultiply not set");
        }
        { //smithingIncrementAttackPowerMultiplyPerLevel
            if (smithingLevelStats.TryGetValue("smithingIncrementAttackPowerMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: smithingIncrementAttackPowerMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: smithingIncrementAttackPowerMultiplyPerLevel is not double is {value.GetType()}");
                else smithingIncrementAttackPowerMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: smithingIncrementAttackPowerMultiplyPerLevel not set");
        }
        { //smithingBaseMiningSpeedMultiply
            if (smithingLevelStats.TryGetValue("smithingBaseMiningSpeedMultiply", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: smithingBaseMiningSpeedMultiply is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: smithingBaseMiningSpeedMultiply is not double is {value.GetType()}");
                else smithingBaseMiningSpeedMultiply = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: smithingBaseMiningSpeedMultiply not set");
        }
        { //smithingIncrementMiningSpeedMultiplyPerLevel
            if (smithingLevelStats.TryGetValue("smithingIncrementMiningSpeedMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: smithingIncrementMiningSpeedMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: smithingIncrementMiningSpeedMultiplyPerLevel is not double is {value.GetType()}");
                else smithingIncrementMiningSpeedMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: smithingIncrementMiningSpeedMultiplyPerLevel not set");
        }
        { //smithingBaseArmorProtectionMultiply
            if (smithingLevelStats.TryGetValue("smithingBaseArmorProtectionMultiply", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: smithingBaseArmorProtectionMultiply is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: smithingBaseArmorProtectionMultiply is not double is {value.GetType()}");
                else smithingBaseArmorProtectionMultiply = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: smithingBaseArmorProtectionMultiply not set");
        }
        { //smithingIncrementArmorProtectionMultiplyPerLevel
            if (smithingLevelStats.TryGetValue("smithingIncrementArmorProtectionMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: smithingIncrementArmorProtectionMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: smithingIncrementArmorProtectionMultiplyPerLevel is not double is {value.GetType()}");
                else smithingIncrementArmorProtectionMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: smithingIncrementArmorProtectionMultiplyPerLevel not set");
        }
        { //smithingMaxLevel
            if (smithingLevelStats.TryGetValue("smithingMaxLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: smithingMaxLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: smithingMaxLevel is not int is {value.GetType()}");
                else smithingMaxLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: smithingMaxLevel not set");
        }


        // Get crop exp
        expPerCraftSmithing.Clear();
        Dictionary<string, object> tmpexpPerCraftSmithing = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/levelstats",
            "smithingcrafts",
            "levelup:config/levelstats/smithingcrafts.json");
        foreach (KeyValuePair<string, object> pair in tmpexpPerCraftSmithing)
        {
            if (pair.Value is long value) expPerCraftSmithing.Add(pair.Key, (int)value);
            else Debug.Log($"CONFIGURATION ERROR: expPerCraftSmithing {pair.Key} is not int");
        }

        Debug.Log("Smithing configuration set");
    }

    public static int SmithingGetLevelByEXP(ulong exp)
    {
        double baseExp = smithingEXPPerLevelBase;
        double multiplier = smithingEXPMultiplyPerLevel;

        if (multiplier <= 1.0)
        {
            return (int)(exp / baseExp);
        }

        double expDouble = exp;

        double level = Math.Log((expDouble * (multiplier - 1) / baseExp) + 1) / Math.Log(multiplier);

        return Math.Max(0, (int)Math.Floor(level));
    }

    public static ulong SmithingGetExpByLevel(int level)
    {
        double baseExp = smithingEXPPerLevelBase;
        double multiplier = smithingEXPMultiplyPerLevel;

        if (multiplier == 1.0)
        {
            return (ulong)(baseExp * level);
        }

        double exp = baseExp * (Math.Pow(multiplier, level) - 1) / (multiplier - 1);
        return (ulong)Math.Floor(exp);
    }


    public static float SmithingGetDurabilityMultiplyByLevel(int level)
    {
        return smithingBaseDurabilityMultiply * (1 + smithingIncrementDurabilityMultiplyPerLevel * level);
    }

    public static float SmithingGetAttackPowerMultiplyByLevel(int level)
    {
        return smithingBaseAttackPowerMultiply * (1 + smithingIncrementAttackPowerMultiplyPerLevel * level);
    }

    public static float SmithingGetMiningSpeedMultiplyByLevel(int level)
    {
        return smithingBaseMiningSpeedMultiply * (1 + smithingIncrementMiningSpeedMultiplyPerLevel * level);
    }

    public static float SmithingGetArmorProtectionMultiplyByLevel(int level)
    {
        return smithingBaseArmorProtectionMultiply * (1 + smithingIncrementArmorProtectionMultiplyPerLevel * level);
    }
    #endregion

    #region vitality
    private static int vitalityEXPPerReceiveHit = 1;
    private static float vitalityEXPMultiplyByDamage = 0.5f;
    private static int vitalityEXPIncreaseByAmountDamage = 1;
    private static int vitalityEXPPerLevelBase = 10;
    private static double vitalityEXPMultiplyPerLevel = 2.0;
    private static float vitalityHPIncreasePerLevel = 10.0f;
    private static float vitalityBaseHP = 10.0f;
    private static float vitalityBaseHPRegen = 1.0f;
    private static float vitalityHPRegenIncreasePerLevel = 0.1f;
    private static int vitalityDamageLimit = 100;
    public static int vitalityMaxLevel = 999;

    public static int DamageLimitVitality => vitalityDamageLimit;
    public static float BaseHPVitality => vitalityBaseHP;
    public static float BaseHPRegenVitality => vitalityBaseHPRegen;

    public static void PopulateVitalityConfiguration(ICoreAPI api)
    {
        Dictionary<string, object> vitalityLevelStats = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/levelstats",
            "vitality",
            "levelup:config/levelstats/vitality.json");
        { //vitalityEXPPerLevelBase
            if (vitalityLevelStats.TryGetValue("vitalityEXPPerLevelBase", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: vitalityEXPPerLevelBase is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: vitalityEXPPerLevelBase is not int is {value.GetType()}");
                else vitalityEXPPerLevelBase = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: vitalityEXPPerLevelBase not set");
        }
        { //vitalityEXPMultiplyPerLevel
            if (vitalityLevelStats.TryGetValue("vitalityEXPMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: vitalityEXPMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: vitalityEXPMultiplyPerLevel is not double is {value.GetType()}");
                else vitalityEXPMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: vitalityEXPMultiplyPerLevel not set");
        }
        { //vitalityEXPPerReceiveHit
            if (vitalityLevelStats.TryGetValue("vitalityEXPPerReceiveHit", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: vitalityEXPPerReceiveHit is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: vitalityEXPPerReceiveHit is not int is {value.GetType()}");
                else vitalityEXPPerReceiveHit = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: vitalityEXPPerReceiveHit not set");
            Experience.LoadExperience("Vitality", "Hit", (ulong)vitalityEXPPerReceiveHit);
        }
        { //vitalityEXPMultiplyByDamage
            if (vitalityLevelStats.TryGetValue("vitalityEXPMultiplyByDamage", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: vitalityEXPMultiplyByDamage is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: vitalityEXPMultiplyByDamage is not double is {value.GetType()}");
                else vitalityEXPMultiplyByDamage = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: vitalityEXPMultiplyByDamage not set");
        }
        { //vitalityHPIncreasePerLevel
            if (vitalityLevelStats.TryGetValue("vitalityHPIncreasePerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: vitalityHPIncreasePerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: vitalityHPIncreasePerLevel is not double is {value.GetType()}");
                else vitalityHPIncreasePerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: vitalityHPIncreasePerLevel not set");
        }
        { //vitalityBaseHP
            if (vitalityLevelStats.TryGetValue("vitalityBaseHP", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: vitalityBaseHP is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: vitalityBaseHP is not double is {value.GetType()}");
                else vitalityBaseHP = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: vitalityBaseHP not set");
        }
        { //vitalityEXPIncreaseByAmountDamage
            if (vitalityLevelStats.TryGetValue("vitalityEXPIncreaseByAmountDamage", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: vitalityEXPIncreaseByAmountDamage is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: vitalityEXPIncreaseByAmountDamage is not int is {value.GetType()}");
                else vitalityEXPIncreaseByAmountDamage = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: vitalityEXPIncreaseByAmountDamage not set");
        }
        { //vitalityBaseHPRegen
            if (vitalityLevelStats.TryGetValue("vitalityBaseHPRegen", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: vitalityBaseHPRegen is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: vitalityBaseHPRegen is not double is {value.GetType()}");
                else vitalityBaseHPRegen = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: vitalityBaseHPRegen not set");
        }
        { //vitalityHPRegenIncreasePerLevel
            if (vitalityLevelStats.TryGetValue("vitalityHPRegenIncreasePerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: vitalityHPRegenIncreasePerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: vitalityHPRegenIncreasePerLevel is not double is {value.GetType()}");
                else vitalityHPRegenIncreasePerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: vitalityHPRegenIncreasePerLevel not set");
        }
        { //vitalityDamageLimit
            if (vitalityLevelStats.TryGetValue("vitalityDamageLimit", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: vitalityDamageLimit is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: vitalityDamageLimit is not int is {value.GetType()}");
                else vitalityDamageLimit = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: vitalityDamageLimit not set");
        }
        { //vitalityMaxLevel
            if (vitalityLevelStats.TryGetValue("vitalityMaxLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: vitalityMaxLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: vitalityMaxLevel is not int is {value.GetType()}");
                else vitalityMaxLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: vitalityMaxLevel not set");
        }

        Debug.Log("Vitality configuration set");
    }

    public static int VitalityGetLevelByEXP(ulong exp)
    {
        double baseExp = vitalityEXPPerLevelBase;
        double multiplier = vitalityEXPMultiplyPerLevel;

        if (multiplier <= 1.0)
        {
            return (int)(exp / baseExp);
        }

        double expDouble = exp;

        double level = Math.Log((expDouble * (multiplier - 1) / baseExp) + 1) / Math.Log(multiplier);

        return Math.Max(0, (int)Math.Floor(level));
    }

    public static ulong VitalityGetExpByLevel(int level)
    {
        double baseExp = vitalityEXPPerLevelBase;
        double multiplier = vitalityEXPMultiplyPerLevel;

        if (multiplier == 1.0)
        {
            return (ulong)(baseExp * level);
        }

        double exp = baseExp * (Math.Pow(multiplier, level) - 1) / (multiplier - 1);
        return (ulong)Math.Floor(exp);
    }


    public static float VitalityGetMaxHealthByLevel(int level)
    {
        return vitalityBaseHP + vitalityHPIncreasePerLevel * level;
    }

    public static float VitalityGetHealthRegenMultiplyByLevel(int level)
    {
        return vitalityBaseHPRegen + vitalityHPRegenIncreasePerLevel * level;
    }

    public static int VitalityEXPEarnedByDAMAGE(float damage)
    {
        int calcDamage = (int)Math.Round(damage);
        int multiplesCount = calcDamage / vitalityEXPIncreaseByAmountDamage;
        float multiplier = 1 + vitalityEXPMultiplyByDamage;

        float baseMultiply = vitalityEXPPerReceiveHit * (float)Math.Pow(multiplier, multiplesCount);

        return (int)Math.Round(baseMultiply);
    }
    #endregion

    #region leatherarmor
    public static readonly Dictionary<string, double> expMultiplyHitLeatherArmor = [];
    private static int leatherArmorEXPPerReceiveHit = 1;
    private static float leatherArmorEXPMultiplyByDamage = 0.5f;
    private static int leatherArmorEXPIncreaseByAmountDamage = 1;
    private static int leatherArmorEXPPerLevelBase = 50;
    private static double leatherArmorEXPMultiplyPerLevel = 2.0;
    private static float leatherArmorBaseDamageReduction = 0.0f;
    private static float leatherArmorDamageReductionPerLevel = 0.05f;
    private static int leatherArmorDamageLimit = 1000;
    public static int leatherArmorMaxLevel = 999;

    public static int DamageLimitLeatherArmor => leatherArmorDamageLimit;

    public static void PopulateLeatherArmorConfiguration(ICoreAPI api)
    {
        Dictionary<string, object> leatherArmorLevelStats = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/levelstats",
            "leatherarmor",
            "levelup:config/levelstats/leatherarmor.json");
        { //leatherArmorEXPPerReceiveHit
            if (leatherArmorLevelStats.TryGetValue("leatherArmorEXPPerReceiveHit", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: leatherArmorEXPPerReceiveHit is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: leatherArmorEXPPerReceiveHit is not int is {value.GetType()}");
                else leatherArmorEXPPerReceiveHit = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: leatherArmorEXPPerReceiveHit not set");
            Experience.LoadExperience("LeatherArmor", "Hit", (ulong)leatherArmorEXPPerReceiveHit);
        }
        { //leatherArmorEXPMultiplyByDamage
            if (leatherArmorLevelStats.TryGetValue("leatherArmorEXPMultiplyByDamage", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: leatherArmorEXPMultiplyByDamage is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: leatherArmorEXPMultiplyByDamage is not double is {value.GetType()}");
                else leatherArmorEXPMultiplyByDamage = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: leatherArmorEXPMultiplyByDamage not set");
        }
        { //leatherArmorEXPIncreaseByAmountDamage
            if (leatherArmorLevelStats.TryGetValue("leatherArmorEXPIncreaseByAmountDamage", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: leatherArmorEXPIncreaseByAmountDamage is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: leatherArmorEXPIncreaseByAmountDamage is not int is {value.GetType()}");
                else leatherArmorEXPIncreaseByAmountDamage = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: leatherArmorEXPIncreaseByAmountDamage not set");
        }
        { //leatherArmorEXPPerLevelBase
            if (leatherArmorLevelStats.TryGetValue("leatherArmorEXPPerLevelBase", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: leatherArmorEXPPerLevelBase is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: leatherArmorEXPPerLevelBase is not int is {value.GetType()}");
                else leatherArmorEXPPerLevelBase = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: leatherArmorEXPPerLevelBase not set");
        }
        { //leatherArmorEXPMultiplyPerLevel
            if (leatherArmorLevelStats.TryGetValue("leatherArmorEXPMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: leatherArmorEXPMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: leatherArmorEXPMultiplyPerLevel is not double is {value.GetType()}");
                else leatherArmorEXPMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: leatherArmorEXPMultiplyPerLevel not set");
        }
        { //leatherArmorBaseDamageReduction
            if (leatherArmorLevelStats.TryGetValue("leatherArmorBaseDamageReduction", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: leatherArmorBaseDamageReduction is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: leatherArmorBaseDamageReduction is not double is {value.GetType()}");
                else leatherArmorBaseDamageReduction = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: leatherArmorBaseDamageReduction not set");
        }
        { //leatherArmorDamageReductionPerLevel
            if (leatherArmorLevelStats.TryGetValue("leatherArmorDamageReductionPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: leatherArmorDamageReductionPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: leatherArmorDamageReductionPerLevel is not double is {value.GetType()}");
                else leatherArmorDamageReductionPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: leatherArmorDamageReductionPerLevel not set");
        }
        { //leatherArmorDamageLimit
            if (leatherArmorLevelStats.TryGetValue("leatherArmorDamageLimit", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: leatherArmorDamageLimit is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: leatherArmorDamageLimit is not int is {value.GetType()}");
                else leatherArmorDamageLimit = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: leatherArmorDamageLimit not set");
        }
        { //leatherArmorMaxLevel
            if (leatherArmorLevelStats.TryGetValue("leatherArmorMaxLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: leatherArmorMaxLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: leatherArmorMaxLevel is not int is {value.GetType()}");
                else leatherArmorMaxLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: leatherArmorMaxLevel not set");
        }

        // Get leather armor multiply exp
        expMultiplyHitLeatherArmor.Clear();
        Dictionary<string, object> tmpexpMultiplyHitLeatherArmor = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/levelstats",
            "leatherarmoritems",
            "levelup:config/levelstats/leatherarmoritems.json");
        foreach (KeyValuePair<string, object> pair in tmpexpMultiplyHitLeatherArmor)
        {
            if (pair.Value is double value) expMultiplyHitLeatherArmor.Add(pair.Key, (double)value);
            else Debug.Log($"CONFIGURATION ERROR: expMultiplyHitLeatherArmor {pair.Key} is not double");
        }
        Debug.Log("Leather Armor configuration set");
    }

    public static int LeatherArmorGetLevelByEXP(ulong exp)
    {
        double baseExp = leatherArmorEXPPerLevelBase;
        double multiplier = leatherArmorEXPMultiplyPerLevel;

        if (multiplier <= 1.0)
        {
            return (int)(exp / baseExp);
        }

        double expDouble = exp;

        double level = Math.Log((expDouble * (multiplier - 1) / baseExp) + 1) / Math.Log(multiplier);

        return Math.Max(0, (int)Math.Floor(level));
    }

    public static ulong LeatherArmorGetExpByLevel(int level)
    {
        double baseExp = leatherArmorEXPPerLevelBase;
        double multiplier = leatherArmorEXPMultiplyPerLevel;

        if (multiplier == 1.0)
        {
            return (ulong)(baseExp * level);
        }

        double exp = baseExp * (Math.Pow(multiplier, level) - 1) / (multiplier - 1);
        return (ulong)Math.Floor(exp);
    }


    public static int LeatherArmorBaseEXPEarnedByDAMAGE(float damage)
    {
        int calcDamage = (int)Math.Round(damage);
        int multiplesCount = calcDamage / leatherArmorEXPIncreaseByAmountDamage;
        float multiplier = 1 + leatherArmorEXPMultiplyByDamage;

        float baseMultiply = leatherArmorEXPPerReceiveHit * (float)Math.Pow(multiplier, multiplesCount);

        return (int)Math.Round(baseMultiply);
    }

    public static float LeatherArmorDamageReductionByLevel(int level)
    {
        return leatherArmorBaseDamageReduction + leatherArmorDamageReductionPerLevel * level;
    }
    #endregion

    #region chainarmor
    public static readonly Dictionary<string, double> expMultiplyHitChainArmor = [];
    private static int chainArmorEXPPerReceiveHit = 1;
    private static float chainArmorEXPMultiplyByDamage = 0.5f;
    private static int chainArmorEXPIncreaseByAmountDamage = 1;
    private static int chainArmorEXPPerLevelBase = 50;
    private static double chainArmorEXPMultiplyPerLevel = 2.0;
    private static float chainArmorBaseDamageReduction = 0.0f;
    private static float chainArmorDamageReductionPerLevel = 0.05f;
    private static int chainArmorDamageLimit = 1000;
    public static int chainArmorMaxLevel = 999;

    public static int DamageLimitChainArmor => chainArmorDamageLimit;

    public static void PopulateChainArmorConfiguration(ICoreAPI api)
    {
        Dictionary<string, object> chainArmorLevelStats = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/levelstats",
            "chainarmor",
            "levelup:config/levelstats/chainarmor.json");
        { //chainArmorEXPPerReceiveHit
            if (chainArmorLevelStats.TryGetValue("chainArmorEXPPerReceiveHit", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: chainArmorEXPPerReceiveHit is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: chainArmorEXPPerReceiveHit is not int is {value.GetType()}");
                else chainArmorEXPPerReceiveHit = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: chainArmorEXPPerReceiveHit not set");
            Experience.LoadExperience("ChainArmor", "Hit", (ulong)chainArmorEXPPerReceiveHit);
        }
        { //chainArmorEXPMultiplyByDamage
            if (chainArmorLevelStats.TryGetValue("chainArmorEXPMultiplyByDamage", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: chainArmorEXPMultiplyByDamage is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: chainArmorEXPMultiplyByDamage is not double is {value.GetType()}");
                else chainArmorEXPMultiplyByDamage = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: chainArmorEXPMultiplyByDamage not set");
        }
        { //chainArmorEXPIncreaseByAmountDamage
            if (chainArmorLevelStats.TryGetValue("chainArmorEXPIncreaseByAmountDamage", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: chainArmorEXPIncreaseByAmountDamage is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: chainArmorEXPIncreaseByAmountDamage is not int is {value.GetType()}");
                else chainArmorEXPIncreaseByAmountDamage = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: chainArmorEXPIncreaseByAmountDamage not set");
        }
        { //chainArmorEXPPerLevelBase
            if (chainArmorLevelStats.TryGetValue("chainArmorEXPPerLevelBase", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: chainArmorEXPPerLevelBase is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: chainArmorEXPPerLevelBase is not int is {value.GetType()}");
                else chainArmorEXPPerLevelBase = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: chainArmorEXPPerLevelBase not set");
        }
        { //chainArmorEXPMultiplyPerLevel
            if (chainArmorLevelStats.TryGetValue("chainArmorEXPMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: chainArmorEXPMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: chainArmorEXPMultiplyPerLevel is not double is {value.GetType()}");
                else chainArmorEXPMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: chainArmorEXPMultiplyPerLevel not set");
        }
        { //chainArmorBaseDamageReduction
            if (chainArmorLevelStats.TryGetValue("chainArmorBaseDamageReduction", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: chainArmorBaseDamageReduction is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: chainArmorBaseDamageReduction is not double is {value.GetType()}");
                else chainArmorBaseDamageReduction = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: chainArmorBaseDamageReduction not set");
        }
        { //chainArmorDamageReductionPerLevel
            if (chainArmorLevelStats.TryGetValue("chainArmorDamageReductionPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: chainArmorDamageReductionPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: chainArmorDamageReductionPerLevel is not double is {value.GetType()}");
                else chainArmorDamageReductionPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: chainArmorDamageReductionPerLevel not set");
        }
        { //chainArmorDamageLimit
            if (chainArmorLevelStats.TryGetValue("chainArmorDamageLimit", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: chainArmorDamageLimit is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: chainArmorDamageLimit is not int is {value.GetType()}");
                else chainArmorDamageLimit = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: chainArmorDamageLimit not set");
        }
        { //chainArmorMaxLevel
            if (chainArmorLevelStats.TryGetValue("chainArmorMaxLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: chainArmorMaxLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: chainArmorMaxLevel is not int is {value.GetType()}");
                else chainArmorMaxLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: chainArmorMaxLevel not set");
        }

        // Get leather armor multiply exp
        expMultiplyHitChainArmor.Clear();
        Dictionary<string, object> tmpexpMultiplyHitChainArmor = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/levelstats",
            "chainarmoritems",
            "levelup:config/levelstats/chainarmoritems.json");
        foreach (KeyValuePair<string, object> pair in tmpexpMultiplyHitChainArmor)
        {
            if (pair.Value is double value) expMultiplyHitChainArmor.Add(pair.Key, (double)value);
            else Debug.Log($"CONFIGURATION ERROR: expMultiplyHitChainArmor {pair.Key} is not double");
        }
        Debug.Log("Chain Armor configuration set");
    }

    public static int ChainArmorGetLevelByEXP(ulong exp)
    {
        double baseExp = chainArmorEXPPerLevelBase;
        double multiplier = chainArmorEXPMultiplyPerLevel;

        if (multiplier <= 1.0)
        {
            return (int)(exp / baseExp);
        }

        double expDouble = exp;

        double level = Math.Log((expDouble * (multiplier - 1) / baseExp) + 1) / Math.Log(multiplier);

        return Math.Max(0, (int)Math.Floor(level));
    }

    public static ulong ChainArmorGetExpByLevel(int level)
    {
        double baseExp = chainArmorEXPPerLevelBase;
        double multiplier = chainArmorEXPMultiplyPerLevel;

        if (multiplier == 1.0)
        {
            return (ulong)(baseExp * level);
        }

        double exp = baseExp * (Math.Pow(multiplier, level) - 1) / (multiplier - 1);
        return (ulong)Math.Floor(exp);
    }


    public static int ChainArmorBaseEXPEarnedByDAMAGE(float damage)
    {
        int calcDamage = (int)Math.Round(damage);
        int multiplesCount = calcDamage / chainArmorEXPIncreaseByAmountDamage;
        float multiplier = 1 + chainArmorEXPMultiplyByDamage;

        float baseMultiply = chainArmorEXPPerReceiveHit * (float)Math.Pow(multiplier, multiplesCount);

        return (int)Math.Round(baseMultiply);
    }

    public static float ChainArmorDamageReductionByLevel(int level)
    {
        return chainArmorBaseDamageReduction + chainArmorDamageReductionPerLevel * level;
    }
    #endregion

    #region brigandinearmor
    public static readonly Dictionary<string, double> expMultiplyHitBrigandineArmor = [];
    private static int brigandineArmorEXPPerReceiveHit = 1;
    private static float brigandineArmorEXPMultiplyByDamage = 0.5f;
    private static int brigandineArmorEXPIncreaseByAmountDamage = 1;
    private static int brigandineArmorEXPPerLevelBase = 50;
    private static double brigandineArmorEXPMultiplyPerLevel = 2.0;
    private static float brigandineArmorBaseDamageReduction = 0.0f;
    private static float brigandineArmorDamageReductionPerLevel = 0.05f;
    private static int brigandineArmorDamageLimit = 1000;
    public static int brigandineArmorMaxLevel = 999;

    public static int DamageLimitBrigandineArmor => brigandineArmorDamageLimit;

    public static void PopulateBrigandineArmorConfiguration(ICoreAPI api)
    {
        Dictionary<string, object> brigandineArmorLevelStats = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/levelstats",
            "brigandinearmor",
            "levelup:config/levelstats/brigandinearmor.json");
        { //brigandineArmorEXPPerReceiveHit
            if (brigandineArmorLevelStats.TryGetValue("brigandineArmorEXPPerReceiveHit", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: brigandineArmorEXPPerReceiveHit is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: brigandineArmorEXPPerReceiveHit is not int is {value.GetType()}");
                else brigandineArmorEXPPerReceiveHit = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: brigandineArmorEXPPerReceiveHit not set");
            Experience.LoadExperience("BrigandineArmor", "Hit", (ulong)brigandineArmorEXPPerReceiveHit);
        }
        { //brigandineArmorEXPMultiplyByDamage
            if (brigandineArmorLevelStats.TryGetValue("brigandineArmorEXPMultiplyByDamage", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: brigandineArmorEXPMultiplyByDamage is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: brigandineArmorEXPMultiplyByDamage is not double is {value.GetType()}");
                else brigandineArmorEXPMultiplyByDamage = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: brigandineArmorEXPMultiplyByDamage not set");
        }
        { //brigandineArmorEXPIncreaseByAmountDamage
            if (brigandineArmorLevelStats.TryGetValue("brigandineArmorEXPIncreaseByAmountDamage", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: brigandineArmorEXPIncreaseByAmountDamage is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: brigandineArmorEXPIncreaseByAmountDamage is not int is {value.GetType()}");
                else brigandineArmorEXPIncreaseByAmountDamage = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: brigandineArmorEXPIncreaseByAmountDamage not set");
        }
        { //brigandineArmorEXPPerLevelBase
            if (brigandineArmorLevelStats.TryGetValue("brigandineArmorEXPPerLevelBase", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: brigandineArmorEXPPerLevelBase is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: brigandineArmorEXPPerLevelBase is not int is {value.GetType()}");
                else brigandineArmorEXPPerLevelBase = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: brigandineArmorEXPPerLevelBase not set");
        }
        { //brigandineArmorEXPMultiplyPerLevel
            if (brigandineArmorLevelStats.TryGetValue("brigandineArmorEXPMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: brigandineArmorEXPMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: brigandineArmorEXPMultiplyPerLevel is not double is {value.GetType()}");
                else brigandineArmorEXPMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: brigandineArmorEXPMultiplyPerLevel not set");
        }
        { //brigandineArmorBaseDamageReduction
            if (brigandineArmorLevelStats.TryGetValue("brigandineArmorBaseDamageReduction", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: brigandineArmorBaseDamageReduction is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: brigandineArmorBaseDamageReduction is not double is {value.GetType()}");
                else brigandineArmorBaseDamageReduction = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: brigandineArmorBaseDamageReduction not set");
        }
        { //brigandineArmorDamageReductionPerLevel
            if (brigandineArmorLevelStats.TryGetValue("brigandineArmorDamageReductionPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: brigandineArmorDamageReductionPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: brigandineArmorDamageReductionPerLevel is not double is {value.GetType()}");
                else brigandineArmorDamageReductionPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: brigandineArmorDamageReductionPerLevel not set");
        }
        { //brigandineArmorDamageLimit
            if (brigandineArmorLevelStats.TryGetValue("brigandineArmorDamageLimit", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: brigandineArmorDamageLimit is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: brigandineArmorDamageLimit is not int is {value.GetType()}");
                else brigandineArmorDamageLimit = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: brigandineArmorDamageLimit not set");
        }
        { //brigandineArmorMaxLevel
            if (brigandineArmorLevelStats.TryGetValue("brigandineArmorMaxLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: brigandineArmorMaxLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: brigandineArmorMaxLevel is not int is {value.GetType()}");
                else brigandineArmorMaxLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: brigandineArmorMaxLevel not set");
        }

        // Get leather armor multiply exp
        expMultiplyHitBrigandineArmor.Clear();
        Dictionary<string, object> tmpexpMultiplyHitBrigandineArmor = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/levelstats",
            "brigandinearmoritems",
            "levelup:config/levelstats/brigandinearmoritems.json");
        foreach (KeyValuePair<string, object> pair in tmpexpMultiplyHitBrigandineArmor)
        {
            if (pair.Value is double value) expMultiplyHitBrigandineArmor.Add(pair.Key, (double)value);
            else Debug.Log($"CONFIGURATION ERROR: expMultiplyHitBrigandineArmor {pair.Key} is not double");
        }
        Debug.Log("Brigandine Armor configuration set");
    }

    public static int BrigandineArmorGetLevelByEXP(ulong exp)
    {
        double baseExp = brigandineArmorEXPPerLevelBase;
        double multiplier = brigandineArmorEXPMultiplyPerLevel;

        if (multiplier <= 1.0)
        {
            return (int)(exp / baseExp);
        }

        double expDouble = exp;
        double level = Math.Log((expDouble * (multiplier - 1) / baseExp) + 1) / Math.Log(multiplier);

        return Math.Max(0, (int)Math.Floor(level));
    }

    public static ulong BrigandineArmorGetExpByLevel(int level)
    {
        double baseExp = brigandineArmorEXPPerLevelBase;
        double multiplier = brigandineArmorEXPMultiplyPerLevel;

        if (multiplier == 1.0)
        {
            return (ulong)(baseExp * level);
        }

        double exp = baseExp * (Math.Pow(multiplier, level) - 1) / (multiplier - 1);
        return (ulong)Math.Floor(exp);
    }


    public static int BrigandineArmorBaseEXPEarnedByDAMAGE(float damage)
    {
        int calcDamage = (int)Math.Round(damage);
        int multiplesCount = calcDamage / brigandineArmorEXPIncreaseByAmountDamage;
        float multiplier = 1 + brigandineArmorEXPMultiplyByDamage;

        float baseMultiply = brigandineArmorEXPPerReceiveHit * (float)Math.Pow(multiplier, multiplesCount);

        return (int)Math.Round(baseMultiply);
    }

    public static float BrigandineArmorDamageReductionByLevel(int level)
    {
        return brigandineArmorBaseDamageReduction + brigandineArmorDamageReductionPerLevel * level;
    }
    #endregion

    #region platearmor
    public static readonly Dictionary<string, double> expMultiplyHitPlateArmor = [];
    private static int plateArmorEXPPerReceiveHit = 1;
    private static float plateArmorEXPMultiplyByDamage = 0.5f;
    private static int plateArmorEXPIncreaseByAmountDamage = 1;
    private static int plateArmorEXPPerLevelBase = 10;
    private static double plateArmorEXPMultiplyPerLevel = 2.0;
    private static float plateArmorBaseDamageReduction = 0.0f;
    private static float plateArmorDamageReductionPerLevel = 0.05f;
    private static int plateArmorDamageLimit = 1000;
    public static int plateArmorMaxLevel = 999;

    public static int DamageLimitPlateArmor => plateArmorDamageLimit;

    public static void PopulatePlateArmorConfiguration(ICoreAPI api)
    {
        Dictionary<string, object> plateArmorLevelStats = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/levelstats",
            "platearmor",
            "levelup:config/levelstats/platearmor.json");
        { //plateArmorEXPPerReceiveHit
            if (plateArmorLevelStats.TryGetValue("plateArmorEXPPerReceiveHit", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: plateArmorEXPPerReceiveHit is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: plateArmorEXPPerReceiveHit is not int is {value.GetType()}");
                else plateArmorEXPPerReceiveHit = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: plateArmorEXPPerReceiveHit not set");
            Experience.LoadExperience("PlateArmor", "Hit", (ulong)plateArmorEXPPerReceiveHit);
        }
        { //plateArmorEXPMultiplyByDamage
            if (plateArmorLevelStats.TryGetValue("plateArmorEXPMultiplyByDamage", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: plateArmorEXPMultiplyByDamage is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: plateArmorEXPMultiplyByDamage is not double is {value.GetType()}");
                else plateArmorEXPMultiplyByDamage = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: plateArmorEXPMultiplyByDamage not set");
        }
        { //plateArmorEXPIncreaseByAmountDamage
            if (plateArmorLevelStats.TryGetValue("plateArmorEXPIncreaseByAmountDamage", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: plateArmorEXPIncreaseByAmountDamage is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: plateArmorEXPIncreaseByAmountDamage is not int is {value.GetType()}");
                else plateArmorEXPIncreaseByAmountDamage = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: plateArmorEXPIncreaseByAmountDamage not set");
        }
        { //plateArmorEXPPerLevelBase
            if (plateArmorLevelStats.TryGetValue("plateArmorEXPPerLevelBase", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: plateArmorEXPPerLevelBase is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: plateArmorEXPPerLevelBase is not int is {value.GetType()}");
                else plateArmorEXPPerLevelBase = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: plateArmorEXPPerLevelBase not set");
        }
        { //plateArmorEXPMultiplyPerLevel
            if (plateArmorLevelStats.TryGetValue("plateArmorEXPMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: plateArmorEXPMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: plateArmorEXPMultiplyPerLevel is not double is {value.GetType()}");
                else plateArmorEXPMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: plateArmorEXPMultiplyPerLevel not set");
        }
        { //plateArmorBaseDamageReduction
            if (plateArmorLevelStats.TryGetValue("plateArmorBaseDamageReduction", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: plateArmorBaseDamageReduction is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: plateArmorBaseDamageReduction is not double is {value.GetType()}");
                else plateArmorBaseDamageReduction = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: plateArmorBaseDamageReduction not set");
        }
        { //plateArmorDamageReductionPerLevel
            if (plateArmorLevelStats.TryGetValue("plateArmorDamageReductionPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: plateArmorDamageReductionPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: plateArmorDamageReductionPerLevel is not double is {value.GetType()}");
                else plateArmorDamageReductionPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: plateArmorDamageReductionPerLevel not set");
        }
        { //plateArmorDamageLimit
            if (plateArmorLevelStats.TryGetValue("plateArmorDamageLimit", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: plateArmorDamageLimit is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: plateArmorDamageLimit is not int is {value.GetType()}");
                else plateArmorDamageLimit = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: plateArmorDamageLimit not set");
        }
        { //plateArmorMaxLevel
            if (plateArmorLevelStats.TryGetValue("plateArmorMaxLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: plateArmorMaxLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: plateArmorMaxLevel is not int is {value.GetType()}");
                else plateArmorMaxLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: plateArmorMaxLevel not set");
        }

        // Get leather armor multiply exp
        expMultiplyHitPlateArmor.Clear();
        Dictionary<string, object> tmpexpMultiplyHitPlateArmor = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/levelstats",
            "platearmoritems",
            "levelup:config/levelstats/platearmoritems.json");
        foreach (KeyValuePair<string, object> pair in tmpexpMultiplyHitPlateArmor)
        {
            if (pair.Value is double value) expMultiplyHitPlateArmor.Add(pair.Key, (double)value);
            else Debug.Log($"CONFIGURATION ERROR: expMultiplyHitPlateArmor {pair.Key} is not double");
        }
        Debug.Log("Plate Armor configuration set");
    }

    public static int PlateArmorGetLevelByEXP(ulong exp)
    {
        double baseExp = plateArmorEXPPerLevelBase;
        double multiplier = plateArmorEXPMultiplyPerLevel;

        if (multiplier <= 1.0)
        {
            return (int)(exp / baseExp);
        }

        double expDouble = exp;
        double level = Math.Log((expDouble * (multiplier - 1) / baseExp) + 1) / Math.Log(multiplier);

        return Math.Max(0, (int)Math.Floor(level));
    }

    public static ulong PlateArmorGetExpByLevel(int level)
    {
        double baseExp = plateArmorEXPPerLevelBase;
        double multiplier = plateArmorEXPMultiplyPerLevel;

        if (multiplier == 1.0)
        {
            return (ulong)(baseExp * level);
        }

        double exp = baseExp * (Math.Pow(multiplier, level) - 1) / (multiplier - 1);
        return (ulong)Math.Floor(exp);
    }


    public static int PlateArmorBaseEXPEarnedByDAMAGE(float damage)
    {
        int calcDamage = (int)Math.Round(damage);
        int multiplesCount = calcDamage / plateArmorEXPIncreaseByAmountDamage;
        float multiplier = 1 + plateArmorEXPMultiplyByDamage;

        float baseMultiply = plateArmorEXPPerReceiveHit * (float)Math.Pow(multiplier, multiplesCount);

        return (int)Math.Round(baseMultiply);
    }

    public static float PlateArmorDamageReductionByLevel(int level)
    {
        return plateArmorBaseDamageReduction + plateArmorDamageReductionPerLevel * Math.Max(0, level - 1);
    }
    #endregion

    #region scalearmor
    public static readonly Dictionary<string, double> expMultiplyHitScaleArmor = [];
    private static int scaleArmorEXPPerReceiveHit = 1;
    private static float scaleArmorEXPMultiplyByDamage = 0.5f;
    private static int scaleArmorEXPIncreaseByAmountDamage = 1;
    private static int scaleArmorEXPPerLevelBase = 50;
    private static double scaleArmorEXPMultiplyPerLevel = 2.0;
    private static float scaleArmorBaseDamageReduction = 0.0f;
    private static float scaleArmorDamageReductionPerLevel = 0.05f;
    private static int scaleArmorDamageLimit = 1000;
    public static int scaleArmorMaxLevel = 999;

    public static int DamageLimitScaleArmor => scaleArmorDamageLimit;

    public static void PopulateScaleArmorConfiguration(ICoreAPI api)
    {
        Dictionary<string, object> scaleArmorLevelStats = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/levelstats",
            "scalearmor",
            "levelup:config/levelstats/scalearmor.json");
        { //scaleArmorEXPPerReceiveHit
            if (scaleArmorLevelStats.TryGetValue("scaleArmorEXPPerReceiveHit", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: scaleArmorEXPPerReceiveHit is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: scaleArmorEXPPerReceiveHit is not int is {value.GetType()}");
                else scaleArmorEXPPerReceiveHit = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: scaleArmorEXPPerReceiveHit not set");
            Experience.LoadExperience("ScaleArmor", "Hit", (ulong)scaleArmorEXPPerReceiveHit);
        }
        { //scaleArmorEXPMultiplyByDamage
            if (scaleArmorLevelStats.TryGetValue("scaleArmorEXPMultiplyByDamage", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: scaleArmorEXPMultiplyByDamage is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: scaleArmorEXPMultiplyByDamage is not double is {value.GetType()}");
                else scaleArmorEXPMultiplyByDamage = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: scaleArmorEXPMultiplyByDamage not set");
        }
        { //scaleArmorEXPIncreaseByAmountDamage
            if (scaleArmorLevelStats.TryGetValue("scaleArmorEXPIncreaseByAmountDamage", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: scaleArmorEXPIncreaseByAmountDamage is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: scaleArmorEXPIncreaseByAmountDamage is not int is {value.GetType()}");
                else scaleArmorEXPIncreaseByAmountDamage = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: scaleArmorEXPIncreaseByAmountDamage not set");
        }
        { //scaleArmorEXPPerLevelBase
            if (scaleArmorLevelStats.TryGetValue("scaleArmorEXPPerLevelBase", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: scaleArmorEXPPerLevelBase is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: scaleArmorEXPPerLevelBase is not int is {value.GetType()}");
                else scaleArmorEXPPerLevelBase = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: scaleArmorEXPPerLevelBase not set");
        }
        { //scaleArmorEXPMultiplyPerLevel
            if (scaleArmorLevelStats.TryGetValue("scaleArmorEXPMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: scaleArmorEXPMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: scaleArmorEXPMultiplyPerLevel is not double is {value.GetType()}");
                else scaleArmorEXPMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: scaleArmorEXPMultiplyPerLevel not set");
        }
        { //scaleArmorBaseDamageReduction
            if (scaleArmorLevelStats.TryGetValue("scaleArmorBaseDamageReduction", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: scaleArmorBaseDamageReduction is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: scaleArmorBaseDamageReduction is not double is {value.GetType()}");
                else scaleArmorBaseDamageReduction = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: scaleArmorBaseDamageReduction not set");
        }
        { //scaleArmorDamageReductionPerLevel
            if (scaleArmorLevelStats.TryGetValue("scaleArmorDamageReductionPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: scaleArmorDamageReductionPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: scaleArmorDamageReductionPerLevel is not double is {value.GetType()}");
                else scaleArmorDamageReductionPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: scaleArmorDamageReductionPerLevel not set");
        }
        { //scaleArmorDamageLimit
            if (scaleArmorLevelStats.TryGetValue("scaleArmorDamageLimit", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: scaleArmorDamageLimit is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: scaleArmorDamageLimit is not int is {value.GetType()}");
                else scaleArmorDamageLimit = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: scaleArmorDamageLimit not set");
        }
        { //scaleArmorMaxLevel
            if (scaleArmorLevelStats.TryGetValue("scaleArmorMaxLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: scaleArmorMaxLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: scaleArmorMaxLevel is not int is {value.GetType()}");
                else scaleArmorMaxLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: scaleArmorMaxLevel not set");
        }

        // Get leather armor multiply exp
        expMultiplyHitScaleArmor.Clear();
        Dictionary<string, object> tmpexpMultiplyHitScaleArmor = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/levelstats",
            "scalearmoritems",
            "levelup:config/levelstats/scalearmoritems.json");
        foreach (KeyValuePair<string, object> pair in tmpexpMultiplyHitScaleArmor)
        {
            if (pair.Value is double value) expMultiplyHitScaleArmor.Add(pair.Key, (double)value);
            else Debug.Log($"CONFIGURATION ERROR: expMultiplyHitScaleArmor {pair.Key} is not double");
        }
        Debug.Log("Scale Armor configuration set");
    }

    public static int ScaleArmorGetLevelByEXP(ulong exp)
    {
        double baseExp = scaleArmorEXPPerLevelBase;
        double multiplier = scaleArmorEXPMultiplyPerLevel;

        if (multiplier <= 1.0)
        {
            return (int)(exp / baseExp);
        }

        double expDouble = exp;
        double level = Math.Log((expDouble * (multiplier - 1) / baseExp) + 1) / Math.Log(multiplier);

        return Math.Max(0, (int)Math.Floor(level));
    }

    public static ulong ScaleArmorGetExpByLevel(int level)
    {
        double baseExp = scaleArmorEXPPerLevelBase;
        double multiplier = scaleArmorEXPMultiplyPerLevel;

        if (multiplier == 1.0)
        {
            return (ulong)(baseExp * level);
        }

        double exp = baseExp * (Math.Pow(multiplier, level) - 1) / (multiplier - 1);
        return (ulong)Math.Floor(exp);
    }


    public static int ScaleArmorBaseEXPEarnedByDAMAGE(float damage)
    {
        int calcDamage = (int)Math.Round(damage);
        int multiplesCount = calcDamage / scaleArmorEXPIncreaseByAmountDamage;
        float multiplier = 1 + scaleArmorEXPMultiplyByDamage;

        float baseMultiply = scaleArmorEXPPerReceiveHit * (float)Math.Pow(multiplier, multiplesCount);

        return (int)Math.Round(baseMultiply);
    }

    public static float ScaleArmorDamageReductionByLevel(int level)
    {
        return scaleArmorBaseDamageReduction + scaleArmorDamageReductionPerLevel * Math.Max(0, level - 1);
    }
    #endregion

    #region classexp
    public static Dictionary<string, Dictionary<string, object>> ClassExperience { get; private set; } = [];
    public static void RegisterNewClassLevel(string currentClass, string levelType, float multiply)
    {
        if (ClassExperience.TryGetValue(currentClass, out Dictionary<string, object> availableLevels))
            if (!availableLevels.TryGetValue(levelType, out _))
                availableLevels.Add(levelType, multiply);
            else
                Debug.LogWarn($"You are trying to set up {levelType} but that level already exist in {currentClass}, it will be ignored...");
        else
            ClassExperience.Add(currentClass, new Dictionary<string, object>
            {
                { levelType, multiply }
            });

        Debug.LogDebug($"Class added: {currentClass} levelType: {levelType} multiply: {multiply}");
    }
    public static float GetEXPMultiplyByClassAndLevelType(string playerClass, string levelType)
    {
        // Class converssion
        playerClass += "class";
        if (ClassExperience.TryGetValue(playerClass, out Dictionary<string, object> classConfigs))
        {
            try
            {
                return (float)Convert.ToSingle(classConfigs[$"class{levelType}LevelMultiply"]);
            }
            catch (Exception ex)
            {
                Debug.LogError($"ERROR: Unable to find the value from {levelType} in {playerClass} configurations, you probably miss something in the json configuration, ex message: {ex.Message}");
                return 1.0f;
            }
        }
        Debug.LogError($"ERROR: The class {playerClass} does not exist in the configurations, probably a custom class without configs");
        return 1.0f;
    }

    public static void PopulateClassConfigurations(ICoreAPI api)
    {
        ClassExperience.Clear();
        string directoryPath = Path.Combine(api.DataBasePath, "ModConfig/LevelUP/config/classexp");
        // Classes directory exists
        if (Directory.Exists(directoryPath))
        {
            Debug.Log("Loading server classes...");
            string[] configs = Directory.GetFiles(directoryPath);
            // Swipe all files
            foreach (string confignameWithExtension in configs)
            {
                // Get only the file name
                string configname = Path.GetFileNameWithoutExtension(confignameWithExtension);
                try
                {
                    // Null check
                    if (!ClassExperience.TryGetValue(configname, out _)) ClassExperience.Add(configname, []);
                    else
                    {
                        Debug.LogWarn($"WARNING: {configname} already exist in memory, duplicated class? how?");
                        continue;
                    }
                    ;

                    // Get the configuration for the respective file
                    Dictionary<string, object> configClass = LoadConfigurationByDirectoryAndName(api, "ModConfig/LevelUP/config/classexp", configname, null);
                    foreach (KeyValuePair<string, object> configuration in configClass)
                    {
                        // Configuration addition
                        RegisterNewClassLevel(configname, configuration.Key, Convert.ToSingle(configuration.Value));
                    }
                    Debug.Log($"{configname} configuration set");
                }
                catch (Exception ex)
                {
                    Debug.Log($"ERROR: Cannot load the class {configname}, probably the file is invalid? reason: {ex.Message}");
                }
            }

            Debug.Log($"Server classes loaded, total loaded classes: {ClassExperience.Count}");
        }
        // Classes directory doesn't exist
        else
        {
            Debug.LogWarn("WARNING: Server configuration classes directory doesn't exist, creating default classes");

            Dictionary<string, object> hunterclass = LoadConfigurationByDirectoryAndName(api, "ModConfig/LevelUP/config/classexp", "hunterclass", "levelup:config/classexp/hunterclass.json");
            foreach (KeyValuePair<string, object> pair in hunterclass)
            {
                RegisterNewClassLevel("hunterclass", pair.Key, Convert.ToSingle(pair.Value));
            }

            Dictionary<string, object> commonerclass = LoadConfigurationByDirectoryAndName(api, "ModConfig/LevelUP/config/classexp", "commonerclass", "levelup:config/classexp/commonerclass.json");
            foreach (KeyValuePair<string, object> pair in commonerclass)
            {
                RegisterNewClassLevel("commonerclass", pair.Key, Convert.ToSingle(pair.Value));
            }

            Dictionary<string, object> blackguardclass = LoadConfigurationByDirectoryAndName(api, "ModConfig/LevelUP/config/classexp", "blackguardclass", "levelup:config/classexp/blackguardclass.json");
            foreach (KeyValuePair<string, object> pair in blackguardclass)
            {
                RegisterNewClassLevel("blackguardclass", pair.Key, Convert.ToSingle(pair.Value));
            }

            Dictionary<string, object> clockmakerclass = LoadConfigurationByDirectoryAndName(api, "ModConfig/LevelUP/config/classexp", "clockmakerclass", "levelup:config/classexp/clockmakerclass.json");
            foreach (KeyValuePair<string, object> pair in clockmakerclass)
            {
                RegisterNewClassLevel("clockmakerclass", pair.Key, Convert.ToSingle(pair.Value));
            }

            Dictionary<string, object> malefactorclass = LoadConfigurationByDirectoryAndName(api, "ModConfig/LevelUP/config/classexp", "malefactorclass", "levelup:config/classexp/malefactorclass.json");
            foreach (KeyValuePair<string, object> pair in malefactorclass)
            {
                RegisterNewClassLevel("malefactorclass", pair.Key, Convert.ToSingle(pair.Value));
            }

            Dictionary<string, object> tailorclass = LoadConfigurationByDirectoryAndName(api, "ModConfig/LevelUP/config/classexp", "tailorclass", "levelup:config/classexp/tailorclass.json");
            foreach (KeyValuePair<string, object> pair in tailorclass)
            {
                RegisterNewClassLevel("tailorclass", pair.Key, Convert.ToSingle(pair.Value));
            }
        }
    }
    #endregion
}

[ProtoContract]
public class ServerMessage
{
    [ProtoMember(1)]
    public string message;
}