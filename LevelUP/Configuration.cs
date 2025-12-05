using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using LevelUP.Server;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProtoBuf;
using Vintagestory.API.Common;

namespace LevelUP;

#pragma warning disable CA2211
#pragma warning disable IDE0044
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
    /// <summary>
    /// Generates a class json to send it to the client and sync configurations
    /// </summary>
    /// <returns></returns>
    internal static string GenerateClassJsonParameters()
    {
        var type = typeof(Configuration);

        var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);
        var dict = fields.ToDictionary(
            f => f.Name,
            f => f.GetValue(null)
        );

        return JsonConvert.SerializeObject(dict);
    }

    /// <summary>
    /// Consumes the give json from server GenerateClassJsonParameters
    /// use this function only in client!
    /// </summary>
    /// <param name="json"></param>
    internal static void ConsumeGeneratedClassJsonParameters(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            Debug.LogWarn($"Empty json on ConsumeGeneratedClassJsonParameters");
            return;
        }

        var type = typeof(Configuration);
        var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);

        var data = JsonConvert.DeserializeObject<Dictionary<string, JToken>>(json);
        if (data == null)
        {
            Debug.LogError($"Cannot deserialize class parameters");
            return;
        }

        foreach (var field in fields)
        {
            if (data.TryGetValue(field.Name, out var token))
            {
                try
                {
                    var value = token.ToObject(field.FieldType);
                    field.SetValue(null, value);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Failed to convert '{field.Name}': {ex.Message}");
                }
            }
        }

        Debug.LogDebug("Configurations json consumed, now i am in sync with the server!");
    }

    public static bool enableHardcore = false;
    public static double hardcoreLosePercentage = 0.8;
    public static int hardcorePenaltyDelayInWorldSeconds = 1000;
    public static bool hardcoreIgnoreLevelMinimum = false;
    public static bool hardcoreMessageWhenDying = true;
    public static bool enableLevelHunter = true;
    public static bool enableLevelBow = true;
    public static bool enableLevelSlingshot = true;
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
    public static bool enableLevelMetabolism = true;
    public static bool enableLevelLeatherArmor = true;
    public static bool enableLevelChainArmor = true;
    public static bool enableLevelBrigandineArmor = true;
    public static bool enableLevelLamellarArmor = true;
    public static bool enableLevelPlateArmor = true;
    public static bool enableLevelScaleArmor = true;
    public static bool enableLevelSmithing = true;
    public static int minimumEXPEarned = 1;
    public static bool enableLevelUPUIDSecurity = false;
    public static bool enableLevelUpChatMessages = false;
    public static bool enableLevelUpExperienceServerLog = false;
    public static bool enableExtendedLog = false;

    private static Dictionary<string, bool> enabledLevels = [];
    public static IReadOnlyDictionary<string, bool> EnabledLevels => enabledLevels;

    internal static void UpdateBaseConfigurations(ICoreAPI api)
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
        { //enableLevelSlingshot
            if (baseConfigs.TryGetValue("enableLevelSlingshot", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: enableLevelSlingshot is null");
                else if (value is not bool) Debug.Log($"CONFIGURATION ERROR: enableLevelSlingshot is not boolean is {value.GetType()}");
                else enableLevelSlingshot = (bool)value;
            else Debug.LogError("CONFIGURATION ERROR: enableLevelSlingshot not set");
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
        { //enableLevelMetabolism
            if (baseConfigs.TryGetValue("enableLevelMetabolism", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: enableLevelMetabolism is null");
                else if (value is not bool) Debug.Log($"CONFIGURATION ERROR: enableLevelMetabolism is not boolean is {value.GetType()}");
                else enableLevelMetabolism = (bool)value;
            else Debug.LogError("CONFIGURATION ERROR: enableLevelMetabolism not set");
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
        { //enableLevelLamellarArmor
            if (baseConfigs.TryGetValue("enableLevelLamellarArmor", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: enableLevelLamellarArmor is null");
                else if (value is not bool) Debug.Log($"CONFIGURATION ERROR: enableLevelLamellarArmor is not boolean is {value.GetType()}");
                else enableLevelLamellarArmor = (bool)value;
            else Debug.LogError("CONFIGURATION ERROR: enableLevelLamellarArmor not set");
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

    /// <summary>
    /// Register a new level in EnabledLevels variable class
    /// </summary>
    public static void RegisterNewLevel(string levelType, bool enabled = true)
    {
        if (levelsByLevelTypeEXP.ContainsKey(levelType))
        {
            Debug.LogError($"The leveltype {levelType} already exist in enabledLevels");
            return;
        }

        enabledLevels.Add(levelType, enabled);
    }
    #endregion

    private static Dictionary<string, System.Func<ulong, int>> levelsByLevelTypeEXP = [];
    private static Dictionary<string, System.Func<int, ulong>> expByLevelTypeLevel = [];

    /// <summary>
    /// Resets configurations variables
    /// </summary>
    internal static void ClearVariables()
    {
        levelsByLevelTypeEXP.Clear();
        expByLevelTypeLevel.Clear();
        maxLevels.Clear();
        enabledLevels.Clear();
        Debug.Log("Variables cleared");
    }

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

    public static ulong GetEXPByLevelTypeLevel(string levelType, int level)
    {
        if (expByLevelTypeLevel.TryGetValue(levelType, out System.Func<int, ulong> function))
            return function(level);

        Debug.LogWarn($"WARNING: {levelType} doesn't belong to the function GetEXPByLevelTypeLevel did you forget to add it? check the wiki");
        return 0;
    }

    private static Dictionary<string, int> maxLevels = [];
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
    public static Dictionary<string, int> entityExpHunter = [];
    private static int hunterEXPPerLevelBase = 800;
    private static double hunterEXPMultiplyPerLevel = 1.2;
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
    public static Dictionary<string, int> entityExpBow = [];
    private static int bowEXPPerHit = 10;
    private static int bowEXPPerLevelBase = 500;
    private static double bowEXPMultiplyPerLevel = 1.1;
    private static float bowBaseDamage = 1.0f;
    private static float bowIncrementDamagePerLevel = 0.1f;
    private static float bowBaseChanceToNotLoseArrow = 50.0f;
    private static float bowChanceToNotLoseArrowBaseIncreasePerLevel = 2.0f;
    private static int bowChanceToNotLoseArrowReduceIncreaseEveryLevel = 5;
    private static float bowChanceToNotLoseArrowReduceQuantityEveryLevel = 0.2f;
    private static float bowBaseAimAccuracy = 0.8f;
    private static float bowIncreaseAimAccuracyPerLevel = 0.02f;
    public static int bowMaxLevel = 999;

    public static int ExpPerHitBow => bowEXPPerHit;
    public static float BaseAimAccuracyBow => bowBaseAimAccuracy;

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
        int reduceEvery = bowChanceToNotLoseArrowReduceIncreaseEveryLevel;
        float baseChance = bowBaseChanceToNotLoseArrow;
        float baseIncrement = bowChanceToNotLoseArrowBaseIncreasePerLevel;
        float reductionPerStep = bowChanceToNotLoseArrowReduceQuantityEveryLevel;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double finalChance = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        finalChance += baseChance;

        int chance = Random.Next(0, 100);

        if (enableExtendedLog)
            Debug.LogDebug($"Bow should not lose arrow: {finalChance} : {chance}");

        if (finalChance >= chance)
            return 1.0f;
        else
            return 0.0f;
    }

    public static double BowGetRawChanceToNotLoseArrowByLevel(int level)
    {
        int reduceEvery = bowChanceToNotLoseArrowReduceIncreaseEveryLevel;
        float baseChance = bowBaseChanceToNotLoseArrow;
        float baseIncrement = bowChanceToNotLoseArrowBaseIncreasePerLevel;
        float reductionPerStep = bowChanceToNotLoseArrowReduceQuantityEveryLevel;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double finalChance = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        finalChance += baseChance;

        return finalChance;
    }

    public static float BowGetAimAccuracyByLevel(int level)
    {
        return bowBaseAimAccuracy + bowIncreaseAimAccuracyPerLevel * level;
    }

    #endregion

    #region slingshot
    public static Dictionary<string, int> entityExpSlingshot = [];
    private static int slingshotEXPPerHit = 10;
    private static int slingshotEXPPerLevelBase = 500;
    private static double slingshotEXPMultiplyPerLevel = 1.1;
    private static float slingshotBaseDamage = 1.0f;
    private static float slingshotIncrementDamagePerLevel = 0.1f;
    private static float slingshotBaseChanceToNotLoseRock = 50.0f;
    private static float slingshotChanceToNotLoseRockBaseIncreasePerLevel = 2.0f;
    private static int slingshotChanceToNotLoseRockReduceIncreaseEveryLevel = 5;
    private static float slingshotChanceToNotLoseRockReduceQuantityEveryLevel = 0.2f;
    private static float slingshotBaseAimAccuracy = 0.8f;
    private static float slingshotIncreaseAimAccuracyPerLevel = 0.02f;
    public static int slingshotMaxLevel = 999;

    public static int ExpPerHitSlingshot => slingshotEXPPerHit;
    public static float BaseAimAccuracySlingshot => slingshotBaseAimAccuracy;

    public static void PopulateSlingshotConfiguration(ICoreAPI api)
    {
        Dictionary<string, object> slingshotLevelStats = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/levelstats",
            "slingshot",
            "levelup:config/levelstats/slingshot.json");

        { //slingshotEXPPerLevelBase
            if (slingshotLevelStats.TryGetValue("slingshotEXPPerLevelBase", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: slingshotEXPPerLevelBase is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: slingshotEXPPerLevelBase is not int is {value.GetType()}");
                else slingshotEXPPerLevelBase = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: slingshotEXPPerLevelBase not set");
        }
        { //slingshotEXPMultiplyPerLevel
            if (slingshotLevelStats.TryGetValue("slingshotEXPMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: slingshotEXPMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: slingshotEXPMultiplyPerLevel is not double is {value.GetType()}");
                else slingshotEXPMultiplyPerLevel = (double)value;
            else Debug.LogError("CONFIGURATION ERROR: slingshotEXPMultiplyPerLevel not set");
        }
        { //slingshotBaseDamage
            if (slingshotLevelStats.TryGetValue("slingshotBaseDamage", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: slingshotBaseDamage is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: slingshotBaseDamage is not double is {value.GetType()}");
                else slingshotBaseDamage = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: slingshotBaseDamage not set");
        }
        { //slingshotIncrementDamagePerLevel
            if (slingshotLevelStats.TryGetValue("slingshotIncrementDamagePerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: slingshotIncrementDamagePerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: slingshotIncrementDamagePerLevel is not double is {value.GetType()}");
                else slingshotIncrementDamagePerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: slingshotIncrementDamagePerLevel not set");
        }
        { //slingshotEXPPerHit
            if (slingshotLevelStats.TryGetValue("slingshotEXPPerHit", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: slingshotEXPPerHit is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: slingshotEXPPerHit is not int is {value.GetType()}");
                else slingshotEXPPerHit = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: slingshotEXPPerHit not set");
            Experience.LoadExperience("Slingshot", "Hit", (ulong)slingshotEXPPerHit);
        }
        { //slingshotBaseChanceToNotLoseRock
            if (slingshotLevelStats.TryGetValue("slingshotBaseChanceToNotLoseRock", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: slingshotBaseChanceToNotLoseRock is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: slingshotBaseChanceToNotLoseRock is not double is {value.GetType()}");
                else slingshotBaseChanceToNotLoseRock = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: slingshotBaseChanceToNotLoseRock not set");
        }
        { //slingshotChanceToNotLoseRockBaseIncreasePerLevel
            if (slingshotLevelStats.TryGetValue("slingshotChanceToNotLoseRockBaseIncreasePerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: slingshotChanceToNotLoseRockBaseIncreasePerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: slingshotChanceToNotLoseRockBaseIncreasePerLevel is not double is {value.GetType()}");
                else slingshotChanceToNotLoseRockBaseIncreasePerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: slingshotChanceToNotLoseRockBaseIncreasePerLevel not set");
        }
        { //slingshotChanceToNotLoseRockReduceIncreaseEveryLevel
            if (slingshotLevelStats.TryGetValue("slingshotChanceToNotLoseRockReduceIncreaseEveryLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: slingshotChanceToNotLoseRockReduceIncreaseEveryLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: slingshotChanceToNotLoseRockReduceIncreaseEveryLevel is not int is {value.GetType()}");
                else slingshotChanceToNotLoseRockReduceIncreaseEveryLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: slingshotChanceToNotLoseRockReduceIncreaseEveryLevel not set");
        }
        { //slingshotChanceToNotLoseRockReduceQuantityEveryLevel
            if (slingshotLevelStats.TryGetValue("slingshotChanceToNotLoseRockReduceQuantityEveryLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: slingshotChanceToNotLoseRockReduceQuantityEveryLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: slingshotChanceToNotLoseRockReduceQuantityEveryLevel is not double is {value.GetType()}");
                else slingshotChanceToNotLoseRockReduceQuantityEveryLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: slingshotChanceToNotLoseRockReduceQuantityEveryLevel not set");
        }
        { //slingshotBaseAimAccuracy
            if (slingshotLevelStats.TryGetValue("slingshotBaseAimAccuracy", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: slingshotBaseAimAccuracy is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: slingshotBaseAimAccuracy is not double is {value.GetType()}");
                else slingshotBaseAimAccuracy = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: slingshotBaseAimAccuracy not set");
        }
        { //slingshotIncreaseAimAccuracyPerLevel
            if (slingshotLevelStats.TryGetValue("slingshotIncreaseAimAccuracyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: slingshotIncreaseAimAccuracyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: slingshotIncreaseAimAccuracyPerLevel is not double is {value.GetType()}");
                else slingshotIncreaseAimAccuracyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: slingshotIncreaseAimAccuracyPerLevel not set");
        }
        { //slingshotMaxLevel
            if (slingshotLevelStats.TryGetValue("slingshotMaxLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: slingshotMaxLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: slingshotMaxLevel is not int is {value.GetType()}");
                else slingshotMaxLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: slingshotMaxLevel not set");
        }

        // Get entity exp
        entityExpSlingshot.Clear();
        Dictionary<string, object> tmpentityExpSlingshot = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/entityexp",
            "slingshot",
            "levelup:config/entityexp/slingshot.json");
        foreach (KeyValuePair<string, object> pair in tmpentityExpSlingshot)
        {
            if (pair.Value is long value) entityExpSlingshot.Add(pair.Key, (int)value);
            else Debug.Log($"CONFIGURATION ERROR: entityExpSlingshot {pair.Key} is not int");
        }

        Debug.Log("Slingshot configuration set");
    }

    public static int SlingshotGetLevelByEXP(ulong exp)
    {
        double baseExp = slingshotEXPPerLevelBase;
        double multiplier = slingshotEXPMultiplyPerLevel;

        if (multiplier <= 1.0)
        {
            return (int)(exp / baseExp);
        }

        double expDouble = exp;

        double level = Math.Log((expDouble * (multiplier - 1) / baseExp) + 1) / Math.Log(multiplier);

        return Math.Max(0, (int)Math.Floor(level));
    }

    public static ulong SlingshotGetExpByLevel(int level)
    {
        double baseExp = slingshotEXPPerLevelBase;
        double multiplier = slingshotEXPMultiplyPerLevel;

        if (multiplier == 1.0)
        {
            return (ulong)(baseExp * level);
        }

        double exp = baseExp * (Math.Pow(multiplier, level) - 1) / (multiplier - 1);
        return (ulong)Math.Floor(exp);
    }

    public static float SlingshotGetDamageMultiplyByLevel(int level)
    {
        return slingshotBaseDamage + slingshotIncrementDamagePerLevel * level;
    }

    public static bool SlingshotGetChanceToNotLoseRockByLevel(int level)
    {
        int reduceEvery = slingshotChanceToNotLoseRockReduceIncreaseEveryLevel;
        float baseChance = slingshotBaseChanceToNotLoseRock;
        float baseIncrement = slingshotChanceToNotLoseRockBaseIncreasePerLevel;
        float reductionPerStep = slingshotChanceToNotLoseRockReduceQuantityEveryLevel;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double finalChance = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        finalChance += baseChance;

        int chance = Random.Next(0, 100);

        if (enableExtendedLog)
            Debug.LogDebug($"Slingshot should not lose rock: {finalChance} : {chance}");

        if (finalChance >= chance)
            return true;
        else
            return false;
    }

    public static double SlingshotGetRawChanceToNotLoseRockByLevel(int level)
    {
        int reduceEvery = slingshotChanceToNotLoseRockReduceIncreaseEveryLevel;
        float baseChance = slingshotBaseChanceToNotLoseRock;
        float baseIncrement = slingshotChanceToNotLoseRockBaseIncreasePerLevel;
        float reductionPerStep = slingshotChanceToNotLoseRockReduceQuantityEveryLevel;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double finalChance = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        finalChance += baseChance;

        return finalChance;
    }

    public static float SlingshotGetAimAccuracyByLevel(int level)
    {
        return slingshotBaseAimAccuracy + slingshotIncreaseAimAccuracyPerLevel * level;
    }

    #endregion

    #region knife
    public static Dictionary<string, int> entityExpKnife = [];
    private static int knifeEXPPerHit = 10;
    private static int knifeEXPPerHarvest = 50;
    private static int knifeEXPPerBreaking = 10;
    private static int knifeEXPPerLevelBase = 500;
    private static double knifeEXPMultiplyPerLevel = 1.3;
    private static float knifeBaseDamage = 1.0f;
    private static float knifeIncrementDamagePerLevel = 0.1f;
    private static float knifeBaseHarvestMultiply = 0.5f;
    private static float knifeIncrementHarvestMultiplyPerLevel = 0.1f;
    private static float knifeBaseMiningSpeed = 1.0f;
    private static float knifeIncrementMiningSpeedMultiplyPerLevel = 0.1f;
    public static int knifeMaxLevel = 999;

    public static int ExpPerHitKnife => knifeEXPPerHit;
    public static int ExpPerHarvestKnife => knifeEXPPerHarvest;
    public static int ExpPerBreakingKnife => knifeEXPPerBreaking;
    public static float BaseHarvestMultiplyKnife = knifeBaseHarvestMultiply;
    public static float BaseMinigSpeedKnife = knifeBaseMiningSpeed;

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
        return knifeBaseHarvestMultiply * (knifeIncrementHarvestMultiplyPerLevel * level);
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
    public static Dictionary<string, int> entityExpAxe = [];
    private static int axeEXPPerHit = 10;
    private static int axeEXPPerBreaking = 5;
    private static int axeEXPPerTreeBreaking = 200;

    private static int axeEXPPerLevelBase = 1000;
    private static double axeEXPMultiplyPerLevel = 1.2;
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
    public static Dictionary<string, int> entityExpPickaxe = [];
    public static Dictionary<string, int> oresExpPickaxe = [];
    private static int pickaxeEXPPerHit = 10;
    private static int pickaxeEXPPerBreaking = 10;
    private static int pickaxeEXPPerLevelBase = 500;
    private static double pickaxeEXPMultiplyPerLevel = 1.5;
    private static float pickaxeBaseDamage = 1.0f;
    private static float pickaxeIncrementDamagePerLevel = 0.1f;
    private static float pickaxeBaseMiningSpeed = 1.0f;
    private static float pickaxeIncrementMiningSpeedMultiplyPerLevel = 0.03f;
    private static float pickaxeBaseOreMultiply = 0.0f;
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
    public static Dictionary<string, int> entityExpShovel = [];
    private static int shovelEXPPerHit = 10;
    private static int shovelEXPPerBreaking = 10;
    private static int shovelEXPPerLevelBase = 500;
    private static double shovelEXPMultiplyPerLevel = 1.5;
    private static float shovelBaseDamage = 1.0f;
    private static float shovelIncrementDamagePerLevel = 0.1f;
    private static float shovelBaseMiningSpeed = 1.0f;
    private static float shovelIncrementMiningSpeedMultiplyPerLevel = 0.02f;
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
    public static Dictionary<string, int> entityExpSpear = [];
    private static int spearEXPPerHit = 10;
    private static int spearEXPPerThrow = 20;
    private static int spearEXPPerLevelBase = 500;
    private static double spearEXPMultiplyPerLevel = 1.3;
    private static float spearBaseDamage = 1.0f;
    private static float spearIncrementDamagePerLevel = 0.1f;
    private static float spearBaseAimAccuracy = 0.8f;
    private static float spearIncreaseAimAccuracyPerLevel = 0.03f;
    public static int spearMaxLevel = 999;


    public static int ExpPerHitSpear => spearEXPPerHit;
    public static int ExpPerThrowSpear => spearEXPPerThrow;
    public static float BaseAimAccuracySpear => spearBaseAimAccuracy;

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
    public static Dictionary<string, int> entityExpHammer = [];
    public static Dictionary<string, string> smithChanceHammer = [];
    private static int hammerEXPPerHit = 10;
    private static int hammerEXPPerLevelBase = 500;
    private static double hammerEXPMultiplyPerLevel = 1.2;
    private static float hammerBaseDamage = 1.0f;
    private static float hammerIncrementDamagePerLevel = 0.1f;
    private static float hammerBaseSmithRetrieveChance = 0.0f;
    private static float hammerSmithRetrieveChancePerLevel = 2.0f;
    private static int hammerSmithRetrieveEveryLevelReduceChance = 10;
    private static float hammerSmithRetrieveReduceChanceForEveryLevel = 0.3f;
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
        int reduceEvery = hammerSmithRetrieveEveryLevelReduceChance;
        float baseChance = hammerBaseSmithRetrieveChance;
        float baseIncrement = hammerSmithRetrieveChancePerLevel;
        float reductionPerStep = hammerSmithRetrieveReduceChanceForEveryLevel;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double finalChance = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        finalChance += baseChance;

        int chance = Random.Next(0, 100);

        if (enableExtendedLog)
            Debug.LogDebug($"Hammer should retrieve smith mechanic check: {finalChance} : {chance}");

        return finalChance >= chance;
    }

    private static float HammerCalculateChance(
            int level,
            float baseChance,
            float baseIncrement,
            int reduceEvery,
            float reductionPerStep)
    {
        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double finalChance = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        finalChance += baseChance;

        int chance = Random.Next(0, 100);

        if (enableExtendedLog)
            Debug.LogDebug($"Hammer result multiply smith mechanic check: {finalChance} : {chance}");

        return (float)finalChance;
    }

    public static float HammerGetChanceToDouble(int level)
    {
        return HammerCalculateChance(
            level,
            hammerBaseChanceToDouble,
            hammerIncreaseChanceToDoublePerLevel,
            hammerIncreaseChanceToDoublePerLevelReducerPerLevel,
            hammerIncreaseChanceToDoublePerLevelReducer);
    }

    public static float HammerGetChanceToTriple(int level)
    {
        return HammerCalculateChance(
            level,
            hammerBaseChanceToTriple,
            hammerIncreaseChanceToTriplePerLevel,
            hammerIncreaseChanceToTriplePerLevelReducerPerLevel,
            hammerIncreaseChanceToTriplePerLevelReducer);
    }

    public static float HammerGetChanceToQuadruple(int level)
    {
        return HammerCalculateChance(
            level,
            hammerBaseChanceToQuadruple,
            hammerIncreaseChanceToQuadruplePerLevel,
            hammerIncreaseChanceToQuadruplePerLevelReducerPerLevel,
            hammerIncreaseChanceToQuadruplePerLevelReducer);
    }

    public static int HammerGetResultMultiplyByLevel(int level)
    {
        // Quadruple
        float quadChance = HammerGetChanceToQuadruple(level);

        if (enableExtendedLog) Debug.Log($"Quadruple chance: {quadChance}");
        if (quadChance >= Random.Next(0, 100)) return 4;

        // Triple
        float tripleChance = HammerGetChanceToTriple(level);

        if (enableExtendedLog) Debug.Log($"Triple chance: {tripleChance}");
        if (tripleChance >= Random.Next(0, 100)) return 3;

        // Double
        float doubleChance = HammerGetChanceToDouble(level);

        if (enableExtendedLog) Debug.Log($"Double chance: {doubleChance}");
        if (doubleChance >= Random.Next(0, 100)) return 2;

        return 1;
    }

    #endregion

    #region sword
    public static Dictionary<string, int> entityExpSword = [];
    private static int swordEXPPerHit = 10;
    private static int swordEXPPerLevelBase = 500;
    private static double swordEXPMultiplyPerLevel = 1.3;
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
    private static int shieldEXPPerHit = 10;
    private static int shieldEXPPerLevelBase = 600;
    private static double shieldEXPMultiplyPerLevel = 1.5;
    private static float shieldBaseStatsIncrease = 1.0f;
    private static float shieldStatsIncreasePerLevel = 0.1f;
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
        { //shieldBaseStatsIncrease
            if (shieldLevelStats.TryGetValue("shieldBaseStatsIncrease", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: shieldBaseStatsIncrease is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: shieldBaseStatsIncrease is not double is {value.GetType()}");
                else shieldBaseStatsIncrease = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: shieldBaseStatsIncrease not set");
        }
        { //shieldStatsIncreasePerLevel
            if (shieldLevelStats.TryGetValue("shieldStatsIncreasePerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: shieldStatsIncreasePerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: shieldStatsIncreasePerLevel is not double is {value.GetType()}");
                else shieldStatsIncreasePerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: shieldStatsIncreasePerLevel not set");
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

    public static float ShieldGetStatsIncreaseByLevel(int level)
    {
        return shieldBaseStatsIncrease + shieldStatsIncreasePerLevel * level;
    }
    #endregion

    #region hand
    public static Dictionary<string, int> entityExpHand = [];
    private static int handEXPPerHit = 10;
    private static int handEXPPerLevelBase = 300;
    private static double handEXPMultiplyPerLevel = 1.5;
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
    public static Dictionary<string, int> expPerHarvestFarming = [];
    private static int farmingEXPPerTill = 10;
    private static int farmingEXPPerLevelBase = 100;
    private static double farmingEXPMultiplyPerLevel = 2.5;
    private static float farmingBaseHarvestMultiply = 0.5f;
    private static float farmingIncrementHarvestMultiplyPerLevel = 0.2f;
    private static float farmingBaseForageMultiply = 1.0f;
    private static float farmingIncrementForageMultiplyPerLevel = 0.2f;
    public static int farmingMaxLevel = 999;

    public static int ExpPerTillFarming => farmingEXPPerTill;
    public static float BaseHarvestMultiplyFarming => farmingBaseHarvestMultiply;

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
    public static Dictionary<string, double> expMultiplySingleCooking = [];
    public static Dictionary<string, double> expMultiplyPotsCooking = [];
    private static int cookingBaseExpPerCooking = 30;
    private static int cookingEXPPerLevelBase = 100;
    private static double cookingEXPMultiplyPerLevel = 1.3;
    private static float cookingBaseFreshHoursMultiply = 1.0f;
    private static float cookingFreshHoursMultiplyPerLevel = 0.04f;
    private static float cookingBaseChanceToIncreaseServings = 1.0f;
    private static int cookingReduceChanceToIncreaseServings = 5;
    private static float cookingIncrementChanceToIncreaseServings = 2.0f;
    private static float cookingChanceToIncreaseServingsReducerTotal = 0.2f;
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
        { //cookingReduceChanceToIncreaseServings
            if (cookingLevelStats.TryGetValue("cookingReduceChanceToIncreaseServings", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: cookingReduceChanceToIncreaseServings is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: cookingReduceChanceToIncreaseServings is not int is {value.GetType()}");
                else cookingReduceChanceToIncreaseServings = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: cookingReduceChanceToIncreaseServings not set");
            Experience.LoadExperience("Cooking", "Cooking", (ulong)cookingReduceChanceToIncreaseServings);
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
        double finalChance = CookingGetRollChanceByLevel(level);

        int rolls = CookingGetRollsByLevel(level);

        if (enableExtendedLog)
            Debug.LogDebug($"Cooking serving rolls: {rolls}");

        for (int i = 0; i < rolls; i++)
        {
            int servingChance = Random.Next(0, 100);

            if (enableExtendedLog)
                Debug.LogDebug($"Cooking serving roll: {finalChance} : {servingChance}");

            if (finalChance >= servingChance)
                quantityServings += 1;
        }

        return quantityServings;
    }

    public static int CookingGetRollsByLevel(int level)
    {
        int rolls = cookingBaseRollsChanceToIncreaseServings;
        rolls += level / cookingEarnRollsChanceToIncreaseServingsEveryLevel * cookingEarnRollsChanceToIncreaseServingsQuantity;
        return rolls;
    }

    public static double CookingGetRollChanceByLevel(int level)
    {
        int reduceEvery = cookingReduceChanceToIncreaseServings;
        float baseChance = cookingBaseChanceToIncreaseServings;
        float baseIncrement = cookingIncrementChanceToIncreaseServings;
        float reductionPerStep = cookingChanceToIncreaseServingsReducerTotal;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double finalChance = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        finalChance += baseChance;
        return finalChance;
    }
    #endregion

    #region panning
    private static int panningBaseExpPerPanning = 30;
    private static int panningEXPPerLevelBase = 300;
    private static double panningEXPMultiplyPerLevel = 1.3;
    private static float panningBaseLootMultiply = 0.0f;
    private static float panningLootMultiplyPerLevel = 0.1f;
    private static float panningBaseChanceToDoubleLoot = 0.0f;
    private static float panningChanceToDoubleLootPerLevel = 0.05f;
    private static float panningBaseChanceToTripleLoot = 0.0f;
    private static float panningChanceToTripleLootPerLevel = 0.03f;
    private static float panningBaseChanceToQuadrupleLoot = 0.0f;
    private static float panningChanceToQuadrupleLootPerLevel = 0.01f;
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

    public static double PanningGetChanceToDouble(int level)
    {
        return panningBaseChanceToDoubleLoot + panningChanceToDoubleLootPerLevel * level;
    }

    public static double PanningGetChanceToTriple(int level)
    {
        return panningBaseChanceToTripleLoot + panningChanceToTripleLootPerLevel * level;
    }

    public static double PanningGetChanceToQuadruple(int level)
    {
        return panningBaseChanceToQuadrupleLoot + panningChanceToQuadrupleLootPerLevel * level;
    }

    public static int PanningGetLootQuantityMultiplyByLevel(int level)
    {
        double chanceToDouble = PanningGetChanceToDouble(level);
        double chanceToTriple = PanningGetChanceToTriple(level);
        double chanceToQuadruple = PanningGetChanceToQuadruple(level);

        double roll = Random.NextDouble();

        if (roll <= chanceToQuadruple) return 4;
        if (roll <= chanceToTriple) return 3;
        if (roll <= chanceToDouble) return 2;
        return 1;
    }
    #endregion

    #region smithing
    public static Dictionary<string, int> expPerCraftSmithing = [];
    private static int smithingEXPPerLevelBase = 500;
    private static double smithingEXPMultiplyPerLevel = 1.1;
    private static float smithingBaseDurabilityMultiply = 1.0f;
    private static float smithingIncrementDurabilityMultiplyPerLevel = 0.05f;
    private static float smithingBaseAttackPowerMultiply = 1.0f;
    private static float smithingIncrementAttackPowerMultiplyPerLevel = 0.05f;
    private static float smithingBaseMiningSpeedMultiply = 1.0f;
    private static float smithingIncrementMiningSpeedMultiplyPerLevel = 0.01f;
    private static float smithingBaseArmorProtectionMultiply = 1.0f;
    private static float smithingIncrementArmorProtectionMultiplyPerLevel = 0.05f;
    private static float smithingBaseArmorStatusMultiply = 1.0f;
    private static float smithingIncrementArmorStatusMultiplyPerLevel = 0.05f;
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
        { //smithingBaseArmorStatusMultiply
            if (smithingLevelStats.TryGetValue("smithingBaseArmorStatusMultiply", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: smithingBaseArmorStatusMultiply is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: smithingBaseArmorStatusMultiply is not double is {value.GetType()}");
                else smithingBaseArmorStatusMultiply = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: smithingBaseArmorStatusMultiply not set");
        }
        { //smithingIncrementArmorStatusMultiplyPerLevel
            if (smithingLevelStats.TryGetValue("smithingIncrementArmorStatusMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: smithingIncrementArmorStatusMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: smithingIncrementArmorStatusMultiplyPerLevel is not double is {value.GetType()}");
                else smithingIncrementArmorStatusMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: smithingIncrementArmorStatusMultiplyPerLevel not set");
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

    public static float SmithingGetArmorStatusMultiplyByLevel(int level)
    {
        return smithingBaseArmorStatusMultiply * (1 + smithingIncrementArmorStatusMultiplyPerLevel * level);
    }
    #endregion

    #region vitality
    private static int vitalityEXPPerReceiveHit = 10;
    private static float vitalityEXPMultiplyByDamage = 0.3f;
    private static int vitalityEXPIncreaseByAmountDamage = 20;
    private static int vitalityEXPPerLevelBase = 500;
    private static double vitalityEXPMultiplyPerLevel = 1.2;
    private static float vitalityBaseHP = 15.0f;
    private static float vitalityHPIncreasePerLevel = 0.5f;
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

    #region metabolism
    private static int metabolismEXPPerReceiveHit = 30;
    private static int metabolismEXPPerSaturationLost = 5;
    private static int metabolismEXPPerLevelBase = 200;
    private static double metabolismEXPMultiplyPerLevel = 2.0;
    private static float metabolismSaturationIncreasePerLevel = 100.0f;
    private static float metabolismBaseSaturation = 1500.0f;
    private static float metabolismBaseSaturationReceiveMultiply = 1.0f;
    private static float metabolismSaturationReceiveMultiplyPerLevel = 0.05f;
    private static int metabolismSaturationReceiveMultiplyReductionEveryLevel = 1;
    private static float metabolismSaturationReceiveMultiplyReductionPerReduce = 0.05f;
    public static int metabolismMaxLevel = 999;

    public static int EXPPerHitMetabolism => metabolismEXPPerReceiveHit;
    public static int EXPPerSaturationLostMetabolism => metabolismEXPPerSaturationLost;

    public static float BaseSaturationMetabolism => metabolismBaseSaturation;

    public static void PopulateMetabolismConfiguration(ICoreAPI api)
    {
        Dictionary<string, object> metabolismLevelStats = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/levelstats",
            "metabolism",
            "levelup:config/levelstats/metabolism.json");
        { //metabolismEXPPerReceiveHit
            if (metabolismLevelStats.TryGetValue("metabolismEXPPerReceiveHit", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: metabolismEXPPerReceiveHit is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: metabolismEXPPerReceiveHit is not int is {value.GetType()}");
                else metabolismEXPPerReceiveHit = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: metabolismEXPPerReceiveHit not set");
            Experience.LoadExperience("Metabolism", "Hit", (ulong)metabolismEXPPerReceiveHit);
        }
        { //metabolismEXPPerSaturationLost
            if (metabolismLevelStats.TryGetValue("metabolismEXPPerSaturationLost", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: metabolismEXPPerSaturationLost is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: metabolismEXPPerSaturationLost is not int is {value.GetType()}");
                else metabolismEXPPerSaturationLost = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: metabolismEXPPerSaturationLost not set");
        }
        { //metabolismEXPPerLevelBase
            if (metabolismLevelStats.TryGetValue("metabolismEXPPerLevelBase", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: metabolismEXPPerLevelBase is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: metabolismEXPPerLevelBase is not int is {value.GetType()}");
                else metabolismEXPPerLevelBase = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: metabolismEXPPerLevelBase not set");
        }
        { //metabolismEXPMultiplyPerLevel
            if (metabolismLevelStats.TryGetValue("metabolismEXPMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: metabolismEXPMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: metabolismEXPMultiplyPerLevel is not double is {value.GetType()}");
                else metabolismEXPMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: metabolismEXPMultiplyPerLevel not set");
        }
        { //metabolismSaturationIncreasePerLevel
            if (metabolismLevelStats.TryGetValue("metabolismSaturationIncreasePerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: metabolismSaturationIncreasePerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: metabolismSaturationIncreasePerLevel is not double is {value.GetType()}");
                else metabolismSaturationIncreasePerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: metabolismSaturationIncreasePerLevel not set");
        }
        { //metabolismBaseSaturation
            if (metabolismLevelStats.TryGetValue("metabolismBaseSaturation", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: metabolismBaseSaturation is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: metabolismBaseSaturation is not double is {value.GetType()}");
                else metabolismBaseSaturation = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: metabolismBaseSaturation not set");
        }
        { //metabolismBaseSaturationReceiveMultiply
            if (metabolismLevelStats.TryGetValue("metabolismBaseSaturationReceiveMultiply", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: metabolismBaseSaturationReceiveMultiply is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: metabolismBaseSaturationReceiveMultiply is not double is {value.GetType()}");
                else metabolismBaseSaturationReceiveMultiply = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: metabolismBaseSaturationReceiveMultiply not set");
        }
        { //metabolismSaturationReceiveMultiplyPerLevel
            if (metabolismLevelStats.TryGetValue("metabolismSaturationReceiveMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: metabolismSaturationReceiveMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: metabolismSaturationReceiveMultiplyPerLevel is not double is {value.GetType()}");
                else metabolismSaturationReceiveMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: metabolismSaturationReceiveMultiplyPerLevel not set");
        }
        { //metabolismMaxLevel
            if (metabolismLevelStats.TryGetValue("metabolismMaxLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: metabolismMaxLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: metabolismMaxLevel is not int is {value.GetType()}");
                else metabolismMaxLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: metabolismMaxLevel not set");
        }

        Debug.Log("Metabolism configuration set");
    }

    public static int MetabolismGetLevelByEXP(ulong exp)
    {
        double baseExp = metabolismEXPPerLevelBase;
        double multiplier = metabolismEXPMultiplyPerLevel;

        if (multiplier <= 1.0)
        {
            return (int)(exp / baseExp);
        }

        double expDouble = exp;

        double level = Math.Log((expDouble * (multiplier - 1) / baseExp) + 1) / Math.Log(multiplier);

        return Math.Max(0, (int)Math.Floor(level));
    }

    public static ulong MetabolismGetExpByLevel(int level)
    {
        double baseExp = metabolismEXPPerLevelBase;
        double multiplier = metabolismEXPMultiplyPerLevel;

        if (multiplier == 1.0)
        {
            return (ulong)(baseExp * level);
        }

        double exp = baseExp * (Math.Pow(multiplier, level) - 1) / (multiplier - 1);
        return (ulong)Math.Floor(exp);
    }


    public static float MetabolismGetMaxSaturationByLevel(int level)
    {
        return metabolismBaseSaturation + metabolismSaturationIncreasePerLevel * level;
    }

    public static float MetabolismGetSaturationReceiveMultiplyByLevel(int level)
    {
        int reduceEvery = metabolismSaturationReceiveMultiplyReductionEveryLevel;
        float baseSaturation = metabolismBaseSaturationReceiveMultiply;
        float baseIncrement = metabolismSaturationReceiveMultiplyPerLevel;
        float reductionPerStep = metabolismSaturationReceiveMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double reducer = baseIncrement * (Math.Pow(r, level) - 1) / (r - 1);
        reducer = baseSaturation - reducer;

        Debug.LogDebug($"[MetabolismGetSaturationReceiveMultiplyByLevel] reducer returned: {reducer}");

        return (float)reducer;
    }
    #endregion

    #region leatherarmor
    public static Dictionary<string, double> expMultiplyHitLeatherArmor = [];
    private static int leatherArmorEXPPerReceiveHit = 10;
    private static float leatherArmorEXPMultiplyByDamage = 0.3f;
    private static int leatherArmorEXPIncreaseByAmountDamage = 20;
    private static int leatherArmorEXPPerLevelBase = 500;
    private static double leatherArmorEXPMultiplyPerLevel = 1.2;

    private static float leatherArmorRelativeProtectionMultiply = 1.0f;
    private static float leatherArmorRelativeProtectionMultiplyPerLevel = 0.05f;
    private static int leatherArmorRelativeProtectionMultiplyReductionEveryLevel = 1;
    private static float leatherArmorRelativeProtectionMultiplyReductionPerReduce = 0.05f;

    private static float leatherArmorFlatDamageReductionMultiply = 1.0f;
    private static float leatherArmorFlatDamageReductionMultiplyPerLevel = 0.05f;
    private static int leatherArmorFlatDamageReductionMultiplyReductionEveryLevel = 1;
    private static float leatherArmorFlatDamageReductionMultiplyReductionPerReduce = 0.05f;

    private static float leatherArmorHealingEffectivnessMultiply = 1.0f;
    private static float leatherArmorHealingEffectivnessMultiplyPerLevel = 0.05f;
    private static int leatherArmorHealingEffectivnessMultiplyReductionEveryLevel = 1;
    private static float leatherArmorHealingEffectivnessMultiplyReductionPerReduce = 0.05f;

    private static float leatherArmorHungerRateMultiply = 1.0f;
    private static float leatherArmorHungerRateMultiplyPerLevel = 0.05f;
    private static int leatherArmorHungerRateMultiplyReductionEveryLevel = 1;
    private static float leatherArmorHungerRateMultiplyReductionPerReduce = 0.05f;

    private static float leatherArmorRangedWeaponsAccuracyMultiply = 1.0f;
    private static float leatherArmorRangedWeaponsAccuracyMultiplyPerLevel = 0.05f;
    private static int leatherArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel = 1;
    private static float leatherArmorRangedWeaponsAccuracyMultiplyReductionPerReduce = 0.05f;

    private static float leatherArmorRangedWeaponsSpeedMultiply = 1.0f;
    private static float leatherArmorRangedWeaponsSpeedMultiplyPerLevel = 0.05f;
    private static int leatherArmorRangedWeaponsSpeedMultiplyReductionEveryLevel = 1;
    private static float leatherArmorRangedWeaponsSpeedMultiplyReductionPerReduce = 0.05f;

    private static float leatherArmorWalkSpeedMultiply = 1.0f;
    private static float leatherArmorWalkSpeedMultiplyPerLevel = 0.05f;
    private static int leatherArmorWalkSpeedMultiplyReductionEveryLevel = 1;
    private static float leatherArmorWalkSpeedMultiplyReductionPerReduce = 0.05f;

    public static int leatherArmorMaxLevel = 999;

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
        { //leatherArmorEXPMultiplyPerLevel
            if (leatherArmorLevelStats.TryGetValue("leatherArmorEXPMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: leatherArmorEXPMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: leatherArmorEXPMultiplyPerLevel is not double is {value.GetType()}");
                else leatherArmorEXPMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: leatherArmorEXPMultiplyPerLevel not set");
        }

        { //leatherArmorRelativeProtectionMultiply
            if (leatherArmorLevelStats.TryGetValue("leatherArmorRelativeProtectionMultiply", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: leatherArmorRelativeProtectionMultiply is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: leatherArmorRelativeProtectionMultiply is not double is {value.GetType()}");
                else leatherArmorRelativeProtectionMultiply = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: leatherArmorRelativeProtectionMultiply not set");
        }
        { //leatherArmorRelativeProtectionMultiplyPerLevel
            if (leatherArmorLevelStats.TryGetValue("leatherArmorRelativeProtectionMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: leatherArmorRelativeProtectionMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: leatherArmorRelativeProtectionMultiplyPerLevel is not double is {value.GetType()}");
                else leatherArmorRelativeProtectionMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: leatherArmorRelativeProtectionMultiplyPerLevel not set");
        }
        { //leatherArmorRelativeProtectionMultiplyReductionEveryLevel
            if (leatherArmorLevelStats.TryGetValue("leatherArmorRelativeProtectionMultiplyReductionEveryLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: leatherArmorRelativeProtectionMultiplyReductionEveryLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: leatherArmorRelativeProtectionMultiplyReductionEveryLevel is not int is {value.GetType()}");
                else leatherArmorRelativeProtectionMultiplyReductionEveryLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: leatherArmorRelativeProtectionMultiplyReductionEveryLevel not set");
        }
        { //leatherArmorRelativeProtectionMultiplyReductionPerReduce
            if (leatherArmorLevelStats.TryGetValue("leatherArmorRelativeProtectionMultiplyReductionPerReduce", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: leatherArmorRelativeProtectionMultiplyReductionPerReduce is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: leatherArmorRelativeProtectionMultiplyReductionPerReduce is not double is {value.GetType()}");
                else leatherArmorRelativeProtectionMultiplyReductionPerReduce = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: leatherArmorRelativeProtectionMultiplyReductionPerReduce not set");
        }

        { //leatherArmorFlatDamageReductionMultiply
            if (leatherArmorLevelStats.TryGetValue("leatherArmorFlatDamageReductionMultiply", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: leatherArmorFlatDamageReductionMultiply is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: leatherArmorFlatDamageReductionMultiply is not double is {value.GetType()}");
                else leatherArmorFlatDamageReductionMultiply = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: leatherArmorFlatDamageReductionMultiply not set");
        }
        { //leatherArmorFlatDamageReductionMultiplyPerLevel
            if (leatherArmorLevelStats.TryGetValue("leatherArmorFlatDamageReductionMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: leatherArmorFlatDamageReductionMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: leatherArmorFlatDamageReductionMultiplyPerLevel is not double is {value.GetType()}");
                else leatherArmorFlatDamageReductionMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: leatherArmorFlatDamageReductionMultiplyPerLevel not set");
        }
        { //leatherArmorFlatDamageReductionMultiplyReductionEveryLevel
            if (leatherArmorLevelStats.TryGetValue("leatherArmorFlatDamageReductionMultiplyReductionEveryLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: leatherArmorFlatDamageReductionMultiplyReductionEveryLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: leatherArmorFlatDamageReductionMultiplyReductionEveryLevel is not int is {value.GetType()}");
                else leatherArmorFlatDamageReductionMultiplyReductionEveryLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: leatherArmorFlatDamageReductionMultiplyReductionEveryLevel not set");
        }
        { //leatherArmorFlatDamageReductionMultiplyReductionPerReduce
            if (leatherArmorLevelStats.TryGetValue("leatherArmorFlatDamageReductionMultiplyReductionPerReduce", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: leatherArmorFlatDamageReductionMultiplyReductionPerReduce is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: leatherArmorFlatDamageReductionMultiplyReductionPerReduce is not double is {value.GetType()}");
                else leatherArmorFlatDamageReductionMultiplyReductionPerReduce = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: leatherArmorFlatDamageReductionMultiplyReductionPerReduce not set");
        }

        { //leatherArmorHealingEffectivnessMultiply
            if (leatherArmorLevelStats.TryGetValue("leatherArmorHealingEffectivnessMultiply", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: leatherArmorHealingEffectivnessMultiply is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: leatherArmorHealingEffectivnessMultiply is not double is {value.GetType()}");
                else leatherArmorHealingEffectivnessMultiply = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: leatherArmorHealingEffectivnessMultiply not set");
        }
        { //leatherArmorHealingEffectivnessMultiplyPerLevel
            if (leatherArmorLevelStats.TryGetValue("leatherArmorHealingEffectivnessMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: leatherArmorHealingEffectivnessMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: leatherArmorHealingEffectivnessMultiplyPerLevel is not double is {value.GetType()}");
                else leatherArmorHealingEffectivnessMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: leatherArmorHealingEffectivnessMultiplyPerLevel not set");
        }
        { //leatherArmorHealingEffectivnessMultiplyReductionEveryLevel
            if (leatherArmorLevelStats.TryGetValue("leatherArmorHealingEffectivnessMultiplyReductionEveryLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: leatherArmorHealingEffectivnessMultiplyReductionEveryLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: leatherArmorHealingEffectivnessMultiplyReductionEveryLevel is not int is {value.GetType()}");
                else leatherArmorHealingEffectivnessMultiplyReductionEveryLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: leatherArmorHealingEffectivnessMultiplyReductionEveryLevel not set");
        }
        { //leatherArmorHealingEffectivnessMultiplyReductionPerReduce
            if (leatherArmorLevelStats.TryGetValue("leatherArmorHealingEffectivnessMultiplyReductionPerReduce", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: leatherArmorHealingEffectivnessMultiplyReductionPerReduce is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: leatherArmorHealingEffectivnessMultiplyReductionPerReduce is not double is {value.GetType()}");
                else leatherArmorHealingEffectivnessMultiplyReductionPerReduce = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: leatherArmorHealingEffectivnessMultiplyReductionPerReduce not set");
        }

        { //leatherArmorHungerRateMultiply
            if (leatherArmorLevelStats.TryGetValue("leatherArmorHungerRateMultiply", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: leatherArmorHungerRateMultiply is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: leatherArmorHungerRateMultiply is not double is {value.GetType()}");
                else leatherArmorHungerRateMultiply = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: leatherArmorHungerRateMultiply not set");
        }
        { //leatherArmorHungerRateMultiplyPerLevel
            if (leatherArmorLevelStats.TryGetValue("leatherArmorHungerRateMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: leatherArmorHungerRateMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: leatherArmorHungerRateMultiplyPerLevel is not double is {value.GetType()}");
                else leatherArmorHungerRateMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: leatherArmorHungerRateMultiplyPerLevel not set");
        }
        { //leatherArmorHungerRateMultiplyReductionEveryLevel
            if (leatherArmorLevelStats.TryGetValue("leatherArmorHungerRateMultiplyReductionEveryLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: leatherArmorHungerRateMultiplyReductionEveryLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: leatherArmorHungerRateMultiplyReductionEveryLevel is not int is {value.GetType()}");
                else leatherArmorHungerRateMultiplyReductionEveryLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: leatherArmorHungerRateMultiplyReductionEveryLevel not set");
        }
        { //leatherArmorHungerRateMultiplyReductionPerReduce
            if (leatherArmorLevelStats.TryGetValue("leatherArmorHungerRateMultiplyReductionPerReduce", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: leatherArmorHungerRateMultiplyReductionPerReduce is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: leatherArmorHungerRateMultiplyReductionPerReduce is not double is {value.GetType()}");
                else leatherArmorHungerRateMultiplyReductionPerReduce = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: leatherArmorHungerRateMultiplyReductionPerReduce not set");
        }

        { //leatherArmorRangedWeaponsAccuracyMultiply
            if (leatherArmorLevelStats.TryGetValue("leatherArmorRangedWeaponsAccuracyMultiply", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: leatherArmorRangedWeaponsAccuracyMultiply is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: leatherArmorRangedWeaponsAccuracyMultiply is not double is {value.GetType()}");
                else leatherArmorRangedWeaponsAccuracyMultiply = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: leatherArmorRangedWeaponsAccuracyMultiply not set");
        }
        { //leatherArmorRangedWeaponsAccuracyMultiplyPerLevel
            if (leatherArmorLevelStats.TryGetValue("leatherArmorRangedWeaponsAccuracyMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: leatherArmorRangedWeaponsAccuracyMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: leatherArmorRangedWeaponsAccuracyMultiplyPerLevel is not double is {value.GetType()}");
                else leatherArmorRangedWeaponsAccuracyMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: leatherArmorRangedWeaponsAccuracyMultiplyPerLevel not set");
        }
        { //leatherArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel
            if (leatherArmorLevelStats.TryGetValue("leatherArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: leatherArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: leatherArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel is not int is {value.GetType()}");
                else leatherArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: leatherArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel not set");
        }
        { //leatherArmorRangedWeaponsAccuracyMultiplyReductionPerReduce
            if (leatherArmorLevelStats.TryGetValue("leatherArmorRangedWeaponsAccuracyMultiplyReductionPerReduce", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: leatherArmorRangedWeaponsAccuracyMultiplyReductionPerReduce is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: leatherArmorRangedWeaponsAccuracyMultiplyReductionPerReduce is not double is {value.GetType()}");
                else leatherArmorRangedWeaponsAccuracyMultiplyReductionPerReduce = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: leatherArmorRangedWeaponsAccuracyMultiplyReductionPerReduce not set");
        }

        { //leatherArmorRangedWeaponsSpeedMultiply
            if (leatherArmorLevelStats.TryGetValue("leatherArmorRangedWeaponsSpeedMultiply", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: leatherArmorRangedWeaponsSpeedMultiply is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: leatherArmorRangedWeaponsSpeedMultiply is not double is {value.GetType()}");
                else leatherArmorRangedWeaponsSpeedMultiply = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: leatherArmorRangedWeaponsSpeedMultiply not set");
        }
        { //leatherArmorRangedWeaponsSpeedMultiplyPerLevel
            if (leatherArmorLevelStats.TryGetValue("leatherArmorRangedWeaponsSpeedMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: leatherArmorRangedWeaponsSpeedMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: leatherArmorRangedWeaponsSpeedMultiplyPerLevel is not double is {value.GetType()}");
                else leatherArmorRangedWeaponsSpeedMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: leatherArmorRangedWeaponsSpeedMultiplyPerLevel not set");
        }
        { //leatherArmorRangedWeaponsSpeedMultiplyReductionEveryLevel
            if (leatherArmorLevelStats.TryGetValue("leatherArmorRangedWeaponsSpeedMultiplyReductionEveryLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: leatherArmorRangedWeaponsSpeedMultiplyReductionEveryLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: leatherArmorRangedWeaponsSpeedMultiplyReductionEveryLevel is not int is {value.GetType()}");
                else leatherArmorRangedWeaponsSpeedMultiplyReductionEveryLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: leatherArmorRangedWeaponsSpeedMultiplyReductionEveryLevel not set");
        }
        { //leatherArmorRangedWeaponsSpeedMultiplyReductionPerReduce
            if (leatherArmorLevelStats.TryGetValue("leatherArmorRangedWeaponsSpeedMultiplyReductionPerReduce", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: leatherArmorRangedWeaponsSpeedMultiplyReductionPerReduce is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: leatherArmorRangedWeaponsSpeedMultiplyReductionPerReduce is not double is {value.GetType()}");
                else leatherArmorRangedWeaponsSpeedMultiplyReductionPerReduce = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: leatherArmorRangedWeaponsSpeedMultiplyReductionPerReduce not set");
        }

        { //leatherArmorWalkSpeedMultiply
            if (leatherArmorLevelStats.TryGetValue("leatherArmorWalkSpeedMultiply", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: leatherArmorWalkSpeedMultiply is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: leatherArmorWalkSpeedMultiply is not double is {value.GetType()}");
                else leatherArmorWalkSpeedMultiply = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: leatherArmorWalkSpeedMultiply not set");
        }
        { //leatherArmorWalkSpeedMultiplyPerLevel
            if (leatherArmorLevelStats.TryGetValue("leatherArmorWalkSpeedMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: leatherArmorWalkSpeedMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: leatherArmorWalkSpeedMultiplyPerLevel is not double is {value.GetType()}");
                else leatherArmorWalkSpeedMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: leatherArmorWalkSpeedMultiplyPerLevel not set");
        }
        { //leatherArmorWalkSpeedMultiplyReductionEveryLevel
            if (leatherArmorLevelStats.TryGetValue("leatherArmorWalkSpeedMultiplyReductionEveryLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: leatherArmorWalkSpeedMultiplyReductionEveryLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: leatherArmorWalkSpeedMultiplyReductionEveryLevel is not int is {value.GetType()}");
                else leatherArmorWalkSpeedMultiplyReductionEveryLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: leatherArmorWalkSpeedMultiplyReductionEveryLevel not set");
        }
        { //leatherArmorWalkSpeedMultiplyReductionPerReduce
            if (leatherArmorLevelStats.TryGetValue("leatherArmorWalkSpeedMultiplyReductionPerReduce", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: leatherArmorWalkSpeedMultiplyReductionPerReduce is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: leatherArmorWalkSpeedMultiplyReductionPerReduce is not double is {value.GetType()}");
                else leatherArmorWalkSpeedMultiplyReductionPerReduce = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: leatherArmorWalkSpeedMultiplyReductionPerReduce not set");
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

    public static float LeatherArmorRelativeProtectionMultiplyByLevel(int level)
    {
        int reduceEvery = leatherArmorRelativeProtectionMultiplyReductionEveryLevel;
        float baseMultiply = leatherArmorRelativeProtectionMultiply;
        float baseIncrement = leatherArmorRelativeProtectionMultiplyPerLevel;
        float reductionPerStep = leatherArmorRelativeProtectionMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double multiply = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        multiply += baseMultiply;

        return (float)multiply;
    }

    public static float LeatherArmorFlatDamageReductionMultiplyByLevel(int level)
    {
        int reduceEvery = leatherArmorFlatDamageReductionMultiplyReductionEveryLevel;
        float baseMultiply = leatherArmorFlatDamageReductionMultiply;
        float baseIncrement = leatherArmorFlatDamageReductionMultiplyPerLevel;
        float reductionPerStep = leatherArmorFlatDamageReductionMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double multiply = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        multiply += baseMultiply;

        return (float)multiply;
    }

    public static float LeatherArmorHealingEffectivnessMultiplyByLevel(int level)
    {
        int reduceEvery = leatherArmorHealingEffectivnessMultiplyReductionEveryLevel;
        float baseMultiply = leatherArmorHealingEffectivnessMultiply;
        float baseIncrement = leatherArmorHealingEffectivnessMultiplyPerLevel;
        float reductionPerStep = leatherArmorHealingEffectivnessMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double multiply = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        multiply += baseMultiply;

        return (float)multiply;
    }

    public static float LeatherArmorHungerRateMultiplyByLevel(int level)
    {
        int reduceEvery = leatherArmorHungerRateMultiplyReductionEveryLevel;
        float baseMultiply = leatherArmorHungerRateMultiply;
        float baseIncrement = leatherArmorHungerRateMultiplyPerLevel;
        float reductionPerStep = leatherArmorHungerRateMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double multiply = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        multiply += baseMultiply;

        return (float)multiply;
    }

    public static float LeatherArmorRangedWeaponsAccuracyMultiplyByLevel(int level)
    {
        int reduceEvery = leatherArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel;
        float baseMultiply = leatherArmorRangedWeaponsAccuracyMultiply;
        float baseIncrement = leatherArmorRangedWeaponsAccuracyMultiplyPerLevel;
        float reductionPerStep = leatherArmorRangedWeaponsAccuracyMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double multiply = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        multiply += baseMultiply;

        return (float)multiply;
    }

    public static float LeatherArmorRangedWeaponsSpeedMultiplyByLevel(int level)
    {
        int reduceEvery = leatherArmorRangedWeaponsSpeedMultiplyReductionEveryLevel;
        float baseMultiply = leatherArmorRangedWeaponsSpeedMultiply;
        float baseIncrement = leatherArmorRangedWeaponsSpeedMultiplyPerLevel;
        float reductionPerStep = leatherArmorRangedWeaponsSpeedMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double multiply = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        multiply += baseMultiply;

        return (float)multiply;
    }

    public static float LeatherArmorWalkSpeedMultiplyByLevel(int level)
    {
        int reduceEvery = leatherArmorWalkSpeedMultiplyReductionEveryLevel;
        float baseMultiply = leatherArmorWalkSpeedMultiply;
        float baseIncrement = leatherArmorWalkSpeedMultiplyPerLevel;
        float reductionPerStep = leatherArmorWalkSpeedMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double multiply = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        multiply += baseMultiply;

        return (float)multiply;
    }
    #endregion

    #region chainarmor
    public static Dictionary<string, double> expMultiplyHitChainArmor = [];
    private static int chainArmorEXPPerReceiveHit = 10;
    private static float chainArmorEXPMultiplyByDamage = 0.3f;
    private static int chainArmorEXPIncreaseByAmountDamage = 20;
    private static int chainArmorEXPPerLevelBase = 500;
    private static double chainArmorEXPMultiplyPerLevel = 1.2;

    private static float chainArmorRelativeProtectionMultiply = 1.0f;
    private static float chainArmorRelativeProtectionMultiplyPerLevel = 0.05f;
    private static int chainArmorRelativeProtectionMultiplyReductionEveryLevel = 1;
    private static float chainArmorRelativeProtectionMultiplyReductionPerReduce = 0.05f;

    private static float chainArmorFlatDamageReductionMultiply = 1.0f;
    private static float chainArmorFlatDamageReductionMultiplyPerLevel = 0.05f;
    private static int chainArmorFlatDamageReductionMultiplyReductionEveryLevel = 1;
    private static float chainArmorFlatDamageReductionMultiplyReductionPerReduce = 0.05f;

    private static float chainArmorHealingEffectivnessMultiply = 1.0f;
    private static float chainArmorHealingEffectivnessMultiplyPerLevel = 0.05f;
    private static int chainArmorHealingEffectivnessMultiplyReductionEveryLevel = 1;
    private static float chainArmorHealingEffectivnessMultiplyReductionPerReduce = 0.05f;

    private static float chainArmorHungerRateMultiply = 1.0f;
    private static float chainArmorHungerRateMultiplyPerLevel = 0.05f;
    private static int chainArmorHungerRateMultiplyReductionEveryLevel = 1;
    private static float chainArmorHungerRateMultiplyReductionPerReduce = 0.05f;

    private static float chainArmorRangedWeaponsAccuracyMultiply = 1.0f;
    private static float chainArmorRangedWeaponsAccuracyMultiplyPerLevel = 0.05f;
    private static int chainArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel = 1;
    private static float chainArmorRangedWeaponsAccuracyMultiplyReductionPerReduce = 0.05f;

    private static float chainArmorRangedWeaponsSpeedMultiply = 1.0f;
    private static float chainArmorRangedWeaponsSpeedMultiplyPerLevel = 0.05f;
    private static int chainArmorRangedWeaponsSpeedMultiplyReductionEveryLevel = 1;
    private static float chainArmorRangedWeaponsSpeedMultiplyReductionPerReduce = 0.05f;

    private static float chainArmorWalkSpeedMultiply = 1.0f;
    private static float chainArmorWalkSpeedMultiplyPerLevel = 0.05f;
    private static int chainArmorWalkSpeedMultiplyReductionEveryLevel = 1;
    private static float chainArmorWalkSpeedMultiplyReductionPerReduce = 0.05f;

    public static int chainArmorMaxLevel = 999;

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
        { //chainArmorEXPMultiplyPerLevel
            if (chainArmorLevelStats.TryGetValue("chainArmorEXPMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: chainArmorEXPMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: chainArmorEXPMultiplyPerLevel is not double is {value.GetType()}");
                else chainArmorEXPMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: chainArmorEXPMultiplyPerLevel not set");
        }

        { //chainArmorRelativeProtectionMultiply
            if (chainArmorLevelStats.TryGetValue("chainArmorRelativeProtectionMultiply", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: chainArmorRelativeProtectionMultiply is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: chainArmorRelativeProtectionMultiply is not double is {value.GetType()}");
                else chainArmorRelativeProtectionMultiply = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: chainArmorRelativeProtectionMultiply not set");
        }
        { //chainArmorRelativeProtectionMultiplyPerLevel
            if (chainArmorLevelStats.TryGetValue("chainArmorRelativeProtectionMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: chainArmorRelativeProtectionMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: chainArmorRelativeProtectionMultiplyPerLevel is not double is {value.GetType()}");
                else chainArmorRelativeProtectionMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: chainArmorRelativeProtectionMultiplyPerLevel not set");
        }
        { //chainArmorRelativeProtectionMultiplyReductionEveryLevel
            if (chainArmorLevelStats.TryGetValue("chainArmorRelativeProtectionMultiplyReductionEveryLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: chainArmorRelativeProtectionMultiplyReductionEveryLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: chainArmorRelativeProtectionMultiplyReductionEveryLevel is not int is {value.GetType()}");
                else chainArmorRelativeProtectionMultiplyReductionEveryLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: chainArmorRelativeProtectionMultiplyReductionEveryLevel not set");
        }
        { //chainArmorRelativeProtectionMultiplyReductionPerReduce
            if (chainArmorLevelStats.TryGetValue("chainArmorRelativeProtectionMultiplyReductionPerReduce", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: chainArmorRelativeProtectionMultiplyReductionPerReduce is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: chainArmorRelativeProtectionMultiplyReductionPerReduce is not double is {value.GetType()}");
                else chainArmorRelativeProtectionMultiplyReductionPerReduce = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: chainArmorRelativeProtectionMultiplyReductionPerReduce not set");
        }

        { //chainArmorFlatDamageReductionMultiply
            if (chainArmorLevelStats.TryGetValue("chainArmorFlatDamageReductionMultiply", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: chainArmorFlatDamageReductionMultiply is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: chainArmorFlatDamageReductionMultiply is not double is {value.GetType()}");
                else chainArmorFlatDamageReductionMultiply = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: chainArmorFlatDamageReductionMultiply not set");
        }
        { //chainArmorFlatDamageReductionMultiplyPerLevel
            if (chainArmorLevelStats.TryGetValue("chainArmorFlatDamageReductionMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: chainArmorFlatDamageReductionMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: chainArmorFlatDamageReductionMultiplyPerLevel is not double is {value.GetType()}");
                else chainArmorFlatDamageReductionMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: chainArmorFlatDamageReductionMultiplyPerLevel not set");
        }
        { //chainArmorFlatDamageReductionMultiplyReductionEveryLevel
            if (chainArmorLevelStats.TryGetValue("chainArmorFlatDamageReductionMultiplyReductionEveryLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: chainArmorFlatDamageReductionMultiplyReductionEveryLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: chainArmorFlatDamageReductionMultiplyReductionEveryLevel is not int is {value.GetType()}");
                else chainArmorFlatDamageReductionMultiplyReductionEveryLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: chainArmorFlatDamageReductionMultiplyReductionEveryLevel not set");
        }
        { //chainArmorFlatDamageReductionMultiplyReductionPerReduce
            if (chainArmorLevelStats.TryGetValue("chainArmorFlatDamageReductionMultiplyReductionPerReduce", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: chainArmorFlatDamageReductionMultiplyReductionPerReduce is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: chainArmorFlatDamageReductionMultiplyReductionPerReduce is not double is {value.GetType()}");
                else chainArmorFlatDamageReductionMultiplyReductionPerReduce = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: chainArmorFlatDamageReductionMultiplyReductionPerReduce not set");
        }

        { //chainArmorHealingEffectivnessMultiply
            if (chainArmorLevelStats.TryGetValue("chainArmorHealingEffectivnessMultiply", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: chainArmorHealingEffectivnessMultiply is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: chainArmorHealingEffectivnessMultiply is not double is {value.GetType()}");
                else chainArmorHealingEffectivnessMultiply = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: chainArmorHealingEffectivnessMultiply not set");
        }
        { //chainArmorHealingEffectivnessMultiplyPerLevel
            if (chainArmorLevelStats.TryGetValue("chainArmorHealingEffectivnessMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: chainArmorHealingEffectivnessMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: chainArmorHealingEffectivnessMultiplyPerLevel is not double is {value.GetType()}");
                else chainArmorHealingEffectivnessMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: chainArmorHealingEffectivnessMultiplyPerLevel not set");
        }
        { //chainArmorHealingEffectivnessMultiplyReductionEveryLevel
            if (chainArmorLevelStats.TryGetValue("chainArmorHealingEffectivnessMultiplyReductionEveryLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: chainArmorHealingEffectivnessMultiplyReductionEveryLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: chainArmorHealingEffectivnessMultiplyReductionEveryLevel is not int is {value.GetType()}");
                else chainArmorHealingEffectivnessMultiplyReductionEveryLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: chainArmorHealingEffectivnessMultiplyReductionEveryLevel not set");
        }
        { //chainArmorHealingEffectivnessMultiplyReductionPerReduce
            if (chainArmorLevelStats.TryGetValue("chainArmorHealingEffectivnessMultiplyReductionPerReduce", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: chainArmorHealingEffectivnessMultiplyReductionPerReduce is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: chainArmorHealingEffectivnessMultiplyReductionPerReduce is not double is {value.GetType()}");
                else chainArmorHealingEffectivnessMultiplyReductionPerReduce = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: chainArmorHealingEffectivnessMultiplyReductionPerReduce not set");
        }

        { //chainArmorHungerRateMultiply
            if (chainArmorLevelStats.TryGetValue("chainArmorHungerRateMultiply", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: chainArmorHungerRateMultiply is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: chainArmorHungerRateMultiply is not double is {value.GetType()}");
                else chainArmorHungerRateMultiply = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: chainArmorHungerRateMultiply not set");
        }
        { //chainArmorHungerRateMultiplyPerLevel
            if (chainArmorLevelStats.TryGetValue("chainArmorHungerRateMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: chainArmorHungerRateMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: chainArmorHungerRateMultiplyPerLevel is not double is {value.GetType()}");
                else chainArmorHungerRateMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: chainArmorHungerRateMultiplyPerLevel not set");
        }
        { //chainArmorHungerRateMultiplyReductionEveryLevel
            if (chainArmorLevelStats.TryGetValue("chainArmorHungerRateMultiplyReductionEveryLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: chainArmorHungerRateMultiplyReductionEveryLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: chainArmorHungerRateMultiplyReductionEveryLevel is not int is {value.GetType()}");
                else chainArmorHungerRateMultiplyReductionEveryLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: chainArmorHungerRateMultiplyReductionEveryLevel not set");
        }
        { //chainArmorHungerRateMultiplyReductionPerReduce
            if (chainArmorLevelStats.TryGetValue("chainArmorHungerRateMultiplyReductionPerReduce", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: chainArmorHungerRateMultiplyReductionPerReduce is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: chainArmorHungerRateMultiplyReductionPerReduce is not double is {value.GetType()}");
                else chainArmorHungerRateMultiplyReductionPerReduce = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: chainArmorHungerRateMultiplyReductionPerReduce not set");
        }

        { //chainArmorRangedWeaponsAccuracyMultiply
            if (chainArmorLevelStats.TryGetValue("chainArmorRangedWeaponsAccuracyMultiply", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: chainArmorRangedWeaponsAccuracyMultiply is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: chainArmorRangedWeaponsAccuracyMultiply is not double is {value.GetType()}");
                else chainArmorRangedWeaponsAccuracyMultiply = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: chainArmorRangedWeaponsAccuracyMultiply not set");
        }
        { //chainArmorRangedWeaponsAccuracyMultiplyPerLevel
            if (chainArmorLevelStats.TryGetValue("chainArmorRangedWeaponsAccuracyMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: chainArmorRangedWeaponsAccuracyMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: chainArmorRangedWeaponsAccuracyMultiplyPerLevel is not double is {value.GetType()}");
                else chainArmorRangedWeaponsAccuracyMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: chainArmorRangedWeaponsAccuracyMultiplyPerLevel not set");
        }
        { //chainArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel
            if (chainArmorLevelStats.TryGetValue("chainArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: chainArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: chainArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel is not int is {value.GetType()}");
                else chainArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: chainArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel not set");
        }
        { //chainArmorRangedWeaponsAccuracyMultiplyReductionPerReduce
            if (chainArmorLevelStats.TryGetValue("chainArmorRangedWeaponsAccuracyMultiplyReductionPerReduce", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: chainArmorRangedWeaponsAccuracyMultiplyReductionPerReduce is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: chainArmorRangedWeaponsAccuracyMultiplyReductionPerReduce is not double is {value.GetType()}");
                else chainArmorRangedWeaponsAccuracyMultiplyReductionPerReduce = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: chainArmorRangedWeaponsAccuracyMultiplyReductionPerReduce not set");
        }

        { //chainArmorRangedWeaponsSpeedMultiply
            if (chainArmorLevelStats.TryGetValue("chainArmorRangedWeaponsSpeedMultiply", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: chainArmorRangedWeaponsSpeedMultiply is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: chainArmorRangedWeaponsSpeedMultiply is not double is {value.GetType()}");
                else chainArmorRangedWeaponsSpeedMultiply = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: chainArmorRangedWeaponsSpeedMultiply not set");
        }
        { //chainArmorRangedWeaponsSpeedMultiplyPerLevel
            if (chainArmorLevelStats.TryGetValue("chainArmorRangedWeaponsSpeedMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: chainArmorRangedWeaponsSpeedMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: chainArmorRangedWeaponsSpeedMultiplyPerLevel is not double is {value.GetType()}");
                else chainArmorRangedWeaponsSpeedMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: chainArmorRangedWeaponsSpeedMultiplyPerLevel not set");
        }
        { //chainArmorRangedWeaponsSpeedMultiplyReductionEveryLevel
            if (chainArmorLevelStats.TryGetValue("chainArmorRangedWeaponsSpeedMultiplyReductionEveryLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: chainArmorRangedWeaponsSpeedMultiplyReductionEveryLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: chainArmorRangedWeaponsSpeedMultiplyReductionEveryLevel is not int is {value.GetType()}");
                else chainArmorRangedWeaponsSpeedMultiplyReductionEveryLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: chainArmorRangedWeaponsSpeedMultiplyReductionEveryLevel not set");
        }
        { //chainArmorRangedWeaponsSpeedMultiplyReductionPerReduce
            if (chainArmorLevelStats.TryGetValue("chainArmorRangedWeaponsSpeedMultiplyReductionPerReduce", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: chainArmorRangedWeaponsSpeedMultiplyReductionPerReduce is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: chainArmorRangedWeaponsSpeedMultiplyReductionPerReduce is not double is {value.GetType()}");
                else chainArmorRangedWeaponsSpeedMultiplyReductionPerReduce = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: chainArmorRangedWeaponsSpeedMultiplyReductionPerReduce not set");
        }

        { //chainArmorWalkSpeedMultiply
            if (chainArmorLevelStats.TryGetValue("chainArmorWalkSpeedMultiply", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: chainArmorWalkSpeedMultiply is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: chainArmorWalkSpeedMultiply is not double is {value.GetType()}");
                else chainArmorWalkSpeedMultiply = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: chainArmorWalkSpeedMultiply not set");
        }
        { //chainArmorWalkSpeedMultiplyPerLevel
            if (chainArmorLevelStats.TryGetValue("chainArmorWalkSpeedMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: chainArmorWalkSpeedMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: chainArmorWalkSpeedMultiplyPerLevel is not double is {value.GetType()}");
                else chainArmorWalkSpeedMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: chainArmorWalkSpeedMultiplyPerLevel not set");
        }
        { //chainArmorWalkSpeedMultiplyReductionEveryLevel
            if (chainArmorLevelStats.TryGetValue("chainArmorWalkSpeedMultiplyReductionEveryLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: chainArmorWalkSpeedMultiplyReductionEveryLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: chainArmorWalkSpeedMultiplyReductionEveryLevel is not int is {value.GetType()}");
                else chainArmorWalkSpeedMultiplyReductionEveryLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: chainArmorWalkSpeedMultiplyReductionEveryLevel not set");
        }
        { //chainArmorWalkSpeedMultiplyReductionPerReduce
            if (chainArmorLevelStats.TryGetValue("chainArmorWalkSpeedMultiplyReductionPerReduce", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: chainArmorWalkSpeedMultiplyReductionPerReduce is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: chainArmorWalkSpeedMultiplyReductionPerReduce is not double is {value.GetType()}");
                else chainArmorWalkSpeedMultiplyReductionPerReduce = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: chainArmorWalkSpeedMultiplyReductionPerReduce not set");
        }

        { //chainArmorMaxLevel
            if (chainArmorLevelStats.TryGetValue("chainArmorMaxLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: chainArmorMaxLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: chainArmorMaxLevel is not int is {value.GetType()}");
                else chainArmorMaxLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: chainArmorMaxLevel not set");
        }

        // Get chain armor multiply exp
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

    public static float ChainArmorRelativeProtectionMultiplyByLevel(int level)
    {
        int reduceEvery = chainArmorRelativeProtectionMultiplyReductionEveryLevel;
        float baseMultiply = chainArmorRelativeProtectionMultiply;
        float baseIncrement = chainArmorRelativeProtectionMultiplyPerLevel;
        float reductionPerStep = chainArmorRelativeProtectionMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double multiply = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        multiply += baseMultiply;

        return (float)multiply;
    }

    public static float ChainArmorFlatDamageReductionMultiplyByLevel(int level)
    {
        int reduceEvery = chainArmorFlatDamageReductionMultiplyReductionEveryLevel;
        float baseMultiply = chainArmorFlatDamageReductionMultiply;
        float baseIncrement = chainArmorFlatDamageReductionMultiplyPerLevel;
        float reductionPerStep = chainArmorFlatDamageReductionMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double multiply = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        multiply += baseMultiply;

        return (float)multiply;
    }

    public static float ChainArmorHealingEffectivnessMultiplyByLevel(int level)
    {
        int reduceEvery = chainArmorHealingEffectivnessMultiplyReductionEveryLevel;
        float baseMultiply = chainArmorHealingEffectivnessMultiply;
        float baseIncrement = chainArmorHealingEffectivnessMultiplyPerLevel;
        float reductionPerStep = chainArmorHealingEffectivnessMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double multiply = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        multiply += baseMultiply;

        return (float)multiply;
    }

    public static float ChainArmorHungerRateMultiplyByLevel(int level)
    {
        int reduceEvery = chainArmorHungerRateMultiplyReductionEveryLevel;
        float baseMultiply = chainArmorHungerRateMultiply;
        float baseIncrement = chainArmorHungerRateMultiplyPerLevel;
        float reductionPerStep = chainArmorHungerRateMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double multiply = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        multiply += baseMultiply;

        return (float)multiply;
    }

    public static float ChainArmorRangedWeaponsAccuracyMultiplyByLevel(int level)
    {
        int reduceEvery = chainArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel;
        float baseMultiply = chainArmorRangedWeaponsAccuracyMultiply;
        float baseIncrement = chainArmorRangedWeaponsAccuracyMultiplyPerLevel;
        float reductionPerStep = chainArmorRangedWeaponsAccuracyMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double multiply = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        multiply += baseMultiply;

        return (float)multiply;
    }

    public static float ChainArmorRangedWeaponsSpeedMultiplyByLevel(int level)
    {
        int reduceEvery = chainArmorRangedWeaponsSpeedMultiplyReductionEveryLevel;
        float baseMultiply = chainArmorRangedWeaponsSpeedMultiply;
        float baseIncrement = chainArmorRangedWeaponsSpeedMultiplyPerLevel;
        float reductionPerStep = chainArmorRangedWeaponsSpeedMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double multiply = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        multiply += baseMultiply;

        return (float)multiply;
    }

    public static float ChainArmorWalkSpeedMultiplyByLevel(int level)
    {
        int reduceEvery = chainArmorWalkSpeedMultiplyReductionEveryLevel;
        float baseMultiply = chainArmorWalkSpeedMultiply;
        float baseIncrement = chainArmorWalkSpeedMultiplyPerLevel;
        float reductionPerStep = chainArmorWalkSpeedMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double multiply = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        multiply += baseMultiply;

        return (float)multiply;
    }
    #endregion

    #region brigandinearmor
    public static Dictionary<string, double> expMultiplyHitBrigandineArmor = [];
    private static int brigandineArmorEXPPerReceiveHit = 10;
    private static float brigandineArmorEXPMultiplyByDamage = 0.3f;
    private static int brigandineArmorEXPIncreaseByAmountDamage = 20;
    private static int brigandineArmorEXPPerLevelBase = 500;
    private static double brigandineArmorEXPMultiplyPerLevel = 1.2;

    private static float brigandineArmorRelativeProtectionMultiply = 1.0f;
    private static float brigandineArmorRelativeProtectionMultiplyPerLevel = 0.05f;
    private static int brigandineArmorRelativeProtectionMultiplyReductionEveryLevel = 1;
    private static float brigandineArmorRelativeProtectionMultiplyReductionPerReduce = 0.05f;

    private static float brigandineArmorFlatDamageReductionMultiply = 1.0f;
    private static float brigandineArmorFlatDamageReductionMultiplyPerLevel = 0.05f;
    private static int brigandineArmorFlatDamageReductionMultiplyReductionEveryLevel = 1;
    private static float brigandineArmorFlatDamageReductionMultiplyReductionPerReduce = 0.05f;

    private static float brigandineArmorHealingEffectivnessMultiply = 1.0f;
    private static float brigandineArmorHealingEffectivnessMultiplyPerLevel = 0.05f;
    private static int brigandineArmorHealingEffectivnessMultiplyReductionEveryLevel = 1;
    private static float brigandineArmorHealingEffectivnessMultiplyReductionPerReduce = 0.05f;

    private static float brigandineArmorHungerRateMultiply = 1.0f;
    private static float brigandineArmorHungerRateMultiplyPerLevel = 0.05f;
    private static int brigandineArmorHungerRateMultiplyReductionEveryLevel = 1;
    private static float brigandineArmorHungerRateMultiplyReductionPerReduce = 0.05f;

    private static float brigandineArmorRangedWeaponsAccuracyMultiply = 1.0f;
    private static float brigandineArmorRangedWeaponsAccuracyMultiplyPerLevel = 0.05f;
    private static int brigandineArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel = 1;
    private static float brigandineArmorRangedWeaponsAccuracyMultiplyReductionPerReduce = 0.05f;

    private static float brigandineArmorRangedWeaponsSpeedMultiply = 1.0f;
    private static float brigandineArmorRangedWeaponsSpeedMultiplyPerLevel = 0.05f;
    private static int brigandineArmorRangedWeaponsSpeedMultiplyReductionEveryLevel = 1;
    private static float brigandineArmorRangedWeaponsSpeedMultiplyReductionPerReduce = 0.05f;

    private static float brigandineArmorWalkSpeedMultiply = 1.0f;
    private static float brigandineArmorWalkSpeedMultiplyPerLevel = 0.05f;
    private static int brigandineArmorWalkSpeedMultiplyReductionEveryLevel = 1;
    private static float brigandineArmorWalkSpeedMultiplyReductionPerReduce = 0.05f;

    public static int brigandineArmorMaxLevel = 999;

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
        { //brigandineArmorEXPMultiplyPerLevel
            if (brigandineArmorLevelStats.TryGetValue("brigandineArmorEXPMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: brigandineArmorEXPMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: brigandineArmorEXPMultiplyPerLevel is not double is {value.GetType()}");
                else brigandineArmorEXPMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: brigandineArmorEXPMultiplyPerLevel not set");
        }

        { //brigandineArmorRelativeProtectionMultiply
            if (brigandineArmorLevelStats.TryGetValue("brigandineArmorRelativeProtectionMultiply", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: brigandineArmorRelativeProtectionMultiply is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: brigandineArmorRelativeProtectionMultiply is not double is {value.GetType()}");
                else brigandineArmorRelativeProtectionMultiply = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: brigandineArmorRelativeProtectionMultiply not set");
        }
        { //brigandineArmorRelativeProtectionMultiplyPerLevel
            if (brigandineArmorLevelStats.TryGetValue("brigandineArmorRelativeProtectionMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: brigandineArmorRelativeProtectionMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: brigandineArmorRelativeProtectionMultiplyPerLevel is not double is {value.GetType()}");
                else brigandineArmorRelativeProtectionMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: brigandineArmorRelativeProtectionMultiplyPerLevel not set");
        }
        { //brigandineArmorRelativeProtectionMultiplyReductionEveryLevel
            if (brigandineArmorLevelStats.TryGetValue("brigandineArmorRelativeProtectionMultiplyReductionEveryLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: brigandineArmorRelativeProtectionMultiplyReductionEveryLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: brigandineArmorRelativeProtectionMultiplyReductionEveryLevel is not int is {value.GetType()}");
                else brigandineArmorRelativeProtectionMultiplyReductionEveryLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: brigandineArmorRelativeProtectionMultiplyReductionEveryLevel not set");
        }
        { //brigandineArmorRelativeProtectionMultiplyReductionPerReduce
            if (brigandineArmorLevelStats.TryGetValue("brigandineArmorRelativeProtectionMultiplyReductionPerReduce", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: brigandineArmorRelativeProtectionMultiplyReductionPerReduce is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: brigandineArmorRelativeProtectionMultiplyReductionPerReduce is not double is {value.GetType()}");
                else brigandineArmorRelativeProtectionMultiplyReductionPerReduce = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: brigandineArmorRelativeProtectionMultiplyReductionPerReduce not set");
        }

        { //brigandineArmorFlatDamageReductionMultiply
            if (brigandineArmorLevelStats.TryGetValue("brigandineArmorFlatDamageReductionMultiply", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: brigandineArmorFlatDamageReductionMultiply is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: brigandineArmorFlatDamageReductionMultiply is not double is {value.GetType()}");
                else brigandineArmorFlatDamageReductionMultiply = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: brigandineArmorFlatDamageReductionMultiply not set");
        }
        { //brigandineArmorFlatDamageReductionMultiplyPerLevel
            if (brigandineArmorLevelStats.TryGetValue("brigandineArmorFlatDamageReductionMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: brigandineArmorFlatDamageReductionMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: brigandineArmorFlatDamageReductionMultiplyPerLevel is not double is {value.GetType()}");
                else brigandineArmorFlatDamageReductionMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: brigandineArmorFlatDamageReductionMultiplyPerLevel not set");
        }
        { //brigandineArmorFlatDamageReductionMultiplyReductionEveryLevel
            if (brigandineArmorLevelStats.TryGetValue("brigandineArmorFlatDamageReductionMultiplyReductionEveryLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: brigandineArmorFlatDamageReductionMultiplyReductionEveryLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: brigandineArmorFlatDamageReductionMultiplyReductionEveryLevel is not int is {value.GetType()}");
                else brigandineArmorFlatDamageReductionMultiplyReductionEveryLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: brigandineArmorFlatDamageReductionMultiplyReductionEveryLevel not set");
        }
        { //brigandineArmorFlatDamageReductionMultiplyReductionPerReduce
            if (brigandineArmorLevelStats.TryGetValue("brigandineArmorFlatDamageReductionMultiplyReductionPerReduce", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: brigandineArmorFlatDamageReductionMultiplyReductionPerReduce is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: brigandineArmorFlatDamageReductionMultiplyReductionPerReduce is not double is {value.GetType()}");
                else brigandineArmorFlatDamageReductionMultiplyReductionPerReduce = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: brigandineArmorFlatDamageReductionMultiplyReductionPerReduce not set");
        }

        { //brigandineArmorHealingEffectivnessMultiply
            if (brigandineArmorLevelStats.TryGetValue("brigandineArmorHealingEffectivnessMultiply", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: brigandineArmorHealingEffectivnessMultiply is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: brigandineArmorHealingEffectivnessMultiply is not double is {value.GetType()}");
                else brigandineArmorHealingEffectivnessMultiply = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: brigandineArmorHealingEffectivnessMultiply not set");
        }
        { //brigandineArmorHealingEffectivnessMultiplyPerLevel
            if (brigandineArmorLevelStats.TryGetValue("brigandineArmorHealingEffectivnessMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: brigandineArmorHealingEffectivnessMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: brigandineArmorHealingEffectivnessMultiplyPerLevel is not double is {value.GetType()}");
                else brigandineArmorHealingEffectivnessMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: brigandineArmorHealingEffectivnessMultiplyPerLevel not set");
        }
        { //brigandineArmorHealingEffectivnessMultiplyReductionEveryLevel
            if (brigandineArmorLevelStats.TryGetValue("brigandineArmorHealingEffectivnessMultiplyReductionEveryLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: brigandineArmorHealingEffectivnessMultiplyReductionEveryLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: brigandineArmorHealingEffectivnessMultiplyReductionEveryLevel is not int is {value.GetType()}");
                else brigandineArmorHealingEffectivnessMultiplyReductionEveryLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: brigandineArmorHealingEffectivnessMultiplyReductionEveryLevel not set");
        }
        { //brigandineArmorHealingEffectivnessMultiplyReductionPerReduce
            if (brigandineArmorLevelStats.TryGetValue("brigandineArmorHealingEffectivnessMultiplyReductionPerReduce", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: brigandineArmorHealingEffectivnessMultiplyReductionPerReduce is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: brigandineArmorHealingEffectivnessMultiplyReductionPerReduce is not double is {value.GetType()}");
                else brigandineArmorHealingEffectivnessMultiplyReductionPerReduce = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: brigandineArmorHealingEffectivnessMultiplyReductionPerReduce not set");
        }

        { //brigandineArmorHungerRateMultiply
            if (brigandineArmorLevelStats.TryGetValue("brigandineArmorHungerRateMultiply", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: brigandineArmorHungerRateMultiply is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: brigandineArmorHungerRateMultiply is not double is {value.GetType()}");
                else brigandineArmorHungerRateMultiply = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: brigandineArmorHungerRateMultiply not set");
        }
        { //brigandineArmorHungerRateMultiplyPerLevel
            if (brigandineArmorLevelStats.TryGetValue("brigandineArmorHungerRateMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: brigandineArmorHungerRateMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: brigandineArmorHungerRateMultiplyPerLevel is not double is {value.GetType()}");
                else brigandineArmorHungerRateMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: brigandineArmorHungerRateMultiplyPerLevel not set");
        }
        { //brigandineArmorHungerRateMultiplyReductionEveryLevel
            if (brigandineArmorLevelStats.TryGetValue("brigandineArmorHungerRateMultiplyReductionEveryLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: brigandineArmorHungerRateMultiplyReductionEveryLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: brigandineArmorHungerRateMultiplyReductionEveryLevel is not int is {value.GetType()}");
                else brigandineArmorHungerRateMultiplyReductionEveryLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: brigandineArmorHungerRateMultiplyReductionEveryLevel not set");
        }
        { //brigandineArmorHungerRateMultiplyReductionPerReduce
            if (brigandineArmorLevelStats.TryGetValue("brigandineArmorHungerRateMultiplyReductionPerReduce", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: brigandineArmorHungerRateMultiplyReductionPerReduce is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: brigandineArmorHungerRateMultiplyReductionPerReduce is not double is {value.GetType()}");
                else brigandineArmorHungerRateMultiplyReductionPerReduce = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: brigandineArmorHungerRateMultiplyReductionPerReduce not set");
        }

        { //brigandineArmorRangedWeaponsAccuracyMultiply
            if (brigandineArmorLevelStats.TryGetValue("brigandineArmorRangedWeaponsAccuracyMultiply", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: brigandineArmorRangedWeaponsAccuracyMultiply is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: brigandineArmorRangedWeaponsAccuracyMultiply is not double is {value.GetType()}");
                else brigandineArmorRangedWeaponsAccuracyMultiply = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: brigandineArmorRangedWeaponsAccuracyMultiply not set");
        }
        { //brigandineArmorRangedWeaponsAccuracyMultiplyPerLevel
            if (brigandineArmorLevelStats.TryGetValue("brigandineArmorRangedWeaponsAccuracyMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: brigandineArmorRangedWeaponsAccuracyMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: brigandineArmorRangedWeaponsAccuracyMultiplyPerLevel is not double is {value.GetType()}");
                else brigandineArmorRangedWeaponsAccuracyMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: brigandineArmorRangedWeaponsAccuracyMultiplyPerLevel not set");
        }
        { //brigandineArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel
            if (brigandineArmorLevelStats.TryGetValue("brigandineArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: brigandineArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: brigandineArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel is not int is {value.GetType()}");
                else brigandineArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: brigandineArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel not set");
        }
        { //brigandineArmorRangedWeaponsAccuracyMultiplyReductionPerReduce
            if (brigandineArmorLevelStats.TryGetValue("brigandineArmorRangedWeaponsAccuracyMultiplyReductionPerReduce", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: brigandineArmorRangedWeaponsAccuracyMultiplyReductionPerReduce is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: brigandineArmorRangedWeaponsAccuracyMultiplyReductionPerReduce is not double is {value.GetType()}");
                else brigandineArmorRangedWeaponsAccuracyMultiplyReductionPerReduce = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: brigandineArmorRangedWeaponsAccuracyMultiplyReductionPerReduce not set");
        }

        { //brigandineArmorRangedWeaponsSpeedMultiply
            if (brigandineArmorLevelStats.TryGetValue("brigandineArmorRangedWeaponsSpeedMultiply", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: brigandineArmorRangedWeaponsSpeedMultiply is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: brigandineArmorRangedWeaponsSpeedMultiply is not double is {value.GetType()}");
                else brigandineArmorRangedWeaponsSpeedMultiply = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: brigandineArmorRangedWeaponsSpeedMultiply not set");
        }
        { //brigandineArmorRangedWeaponsSpeedMultiplyPerLevel
            if (brigandineArmorLevelStats.TryGetValue("brigandineArmorRangedWeaponsSpeedMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: brigandineArmorRangedWeaponsSpeedMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: brigandineArmorRangedWeaponsSpeedMultiplyPerLevel is not double is {value.GetType()}");
                else brigandineArmorRangedWeaponsSpeedMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: brigandineArmorRangedWeaponsSpeedMultiplyPerLevel not set");
        }
        { //brigandineArmorRangedWeaponsSpeedMultiplyReductionEveryLevel
            if (brigandineArmorLevelStats.TryGetValue("brigandineArmorRangedWeaponsSpeedMultiplyReductionEveryLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: brigandineArmorRangedWeaponsSpeedMultiplyReductionEveryLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: brigandineArmorRangedWeaponsSpeedMultiplyReductionEveryLevel is not int is {value.GetType()}");
                else brigandineArmorRangedWeaponsSpeedMultiplyReductionEveryLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: brigandineArmorRangedWeaponsSpeedMultiplyReductionEveryLevel not set");
        }
        { //brigandineArmorRangedWeaponsSpeedMultiplyReductionPerReduce
            if (brigandineArmorLevelStats.TryGetValue("brigandineArmorRangedWeaponsSpeedMultiplyReductionPerReduce", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: brigandineArmorRangedWeaponsSpeedMultiplyReductionPerReduce is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: brigandineArmorRangedWeaponsSpeedMultiplyReductionPerReduce is not double is {value.GetType()}");
                else brigandineArmorRangedWeaponsSpeedMultiplyReductionPerReduce = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: brigandineArmorRangedWeaponsSpeedMultiplyReductionPerReduce not set");
        }

        { //brigandineArmorWalkSpeedMultiply
            if (brigandineArmorLevelStats.TryGetValue("brigandineArmorWalkSpeedMultiply", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: brigandineArmorWalkSpeedMultiply is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: brigandineArmorWalkSpeedMultiply is not double is {value.GetType()}");
                else brigandineArmorWalkSpeedMultiply = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: brigandineArmorWalkSpeedMultiply not set");
        }
        { //brigandineArmorWalkSpeedMultiplyPerLevel
            if (brigandineArmorLevelStats.TryGetValue("brigandineArmorWalkSpeedMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: brigandineArmorWalkSpeedMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: brigandineArmorWalkSpeedMultiplyPerLevel is not double is {value.GetType()}");
                else brigandineArmorWalkSpeedMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: brigandineArmorWalkSpeedMultiplyPerLevel not set");
        }
        { //brigandineArmorWalkSpeedMultiplyReductionEveryLevel
            if (brigandineArmorLevelStats.TryGetValue("brigandineArmorWalkSpeedMultiplyReductionEveryLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: brigandineArmorWalkSpeedMultiplyReductionEveryLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: brigandineArmorWalkSpeedMultiplyReductionEveryLevel is not int is {value.GetType()}");
                else brigandineArmorWalkSpeedMultiplyReductionEveryLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: brigandineArmorWalkSpeedMultiplyReductionEveryLevel not set");
        }
        { //brigandineArmorWalkSpeedMultiplyReductionPerReduce
            if (brigandineArmorLevelStats.TryGetValue("brigandineArmorWalkSpeedMultiplyReductionPerReduce", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: brigandineArmorWalkSpeedMultiplyReductionPerReduce is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: brigandineArmorWalkSpeedMultiplyReductionPerReduce is not double is {value.GetType()}");
                else brigandineArmorWalkSpeedMultiplyReductionPerReduce = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: brigandineArmorWalkSpeedMultiplyReductionPerReduce not set");
        }

        { //brigandineArmorMaxLevel
            if (brigandineArmorLevelStats.TryGetValue("brigandineArmorMaxLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: brigandineArmorMaxLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: brigandineArmorMaxLevel is not int is {value.GetType()}");
                else brigandineArmorMaxLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: brigandineArmorMaxLevel not set");
        }

        // Get brigandine armor multiply exp
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

    public static float BrigandineArmorRelativeProtectionMultiplyByLevel(int level)
    {
        int reduceEvery = brigandineArmorRelativeProtectionMultiplyReductionEveryLevel;
        float baseMultiply = brigandineArmorRelativeProtectionMultiply;
        float baseIncrement = brigandineArmorRelativeProtectionMultiplyPerLevel;
        float reductionPerStep = brigandineArmorRelativeProtectionMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double multiply = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        multiply += baseMultiply;

        return (float)multiply;
    }

    public static float BrigandineArmorFlatDamageReductionMultiplyByLevel(int level)
    {
        int reduceEvery = brigandineArmorFlatDamageReductionMultiplyReductionEveryLevel;
        float baseMultiply = brigandineArmorFlatDamageReductionMultiply;
        float baseIncrement = brigandineArmorFlatDamageReductionMultiplyPerLevel;
        float reductionPerStep = brigandineArmorFlatDamageReductionMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double multiply = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        multiply += baseMultiply;

        return (float)multiply;
    }

    public static float BrigandineArmorHealingEffectivnessMultiplyByLevel(int level)
    {
        int reduceEvery = brigandineArmorHealingEffectivnessMultiplyReductionEveryLevel;
        float baseMultiply = brigandineArmorHealingEffectivnessMultiply;
        float baseIncrement = brigandineArmorHealingEffectivnessMultiplyPerLevel;
        float reductionPerStep = brigandineArmorHealingEffectivnessMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double multiply = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        multiply += baseMultiply;

        return (float)multiply;
    }

    public static float BrigandineArmorHungerRateMultiplyByLevel(int level)
    {
        int reduceEvery = brigandineArmorHungerRateMultiplyReductionEveryLevel;
        float baseMultiply = brigandineArmorHungerRateMultiply;
        float baseIncrement = brigandineArmorHungerRateMultiplyPerLevel;
        float reductionPerStep = brigandineArmorHungerRateMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double multiply = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        multiply += baseMultiply;

        return (float)multiply;
    }

    public static float BrigandineArmorRangedWeaponsAccuracyMultiplyByLevel(int level)
    {
        int reduceEvery = brigandineArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel;
        float baseMultiply = brigandineArmorRangedWeaponsAccuracyMultiply;
        float baseIncrement = brigandineArmorRangedWeaponsAccuracyMultiplyPerLevel;
        float reductionPerStep = brigandineArmorRangedWeaponsAccuracyMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double multiply = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        multiply += baseMultiply;

        return (float)multiply;
    }

    public static float BrigandineArmorRangedWeaponsSpeedMultiplyByLevel(int level)
    {
        int reduceEvery = brigandineArmorRangedWeaponsSpeedMultiplyReductionEveryLevel;
        float baseMultiply = brigandineArmorRangedWeaponsSpeedMultiply;
        float baseIncrement = brigandineArmorRangedWeaponsSpeedMultiplyPerLevel;
        float reductionPerStep = brigandineArmorRangedWeaponsSpeedMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double multiply = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        multiply += baseMultiply;

        return (float)multiply;
    }

    public static float BrigandineArmorWalkSpeedMultiplyByLevel(int level)
    {
        int reduceEvery = brigandineArmorWalkSpeedMultiplyReductionEveryLevel;
        float baseMultiply = brigandineArmorWalkSpeedMultiply;
        float baseIncrement = brigandineArmorWalkSpeedMultiplyPerLevel;
        float reductionPerStep = brigandineArmorWalkSpeedMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double multiply = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        multiply += baseMultiply;

        return (float)multiply;
    }
    #endregion

    #region lamellararmor
    public static Dictionary<string, double> expMultiplyHitLamellarArmor = [];
    private static int lamellarArmorEXPPerReceiveHit = 10;
    private static float lamellarArmorEXPMultiplyByDamage = 0.3f;
    private static int lamellarArmorEXPIncreaseByAmountDamage = 20;
    private static int lamellarArmorEXPPerLevelBase = 500;
    private static double lamellarArmorEXPMultiplyPerLevel = 1.2;

    private static float lamellarArmorRelativeProtectionMultiply = 1.0f;
    private static float lamellarArmorRelativeProtectionMultiplyPerLevel = 0.05f;
    private static int lamellarArmorRelativeProtectionMultiplyReductionEveryLevel = 1;
    private static float lamellarArmorRelativeProtectionMultiplyReductionPerReduce = 0.05f;

    private static float lamellarArmorFlatDamageReductionMultiply = 1.0f;
    private static float lamellarArmorFlatDamageReductionMultiplyPerLevel = 0.05f;
    private static int lamellarArmorFlatDamageReductionMultiplyReductionEveryLevel = 1;
    private static float lamellarArmorFlatDamageReductionMultiplyReductionPerReduce = 0.05f;

    private static float lamellarArmorHealingEffectivnessMultiply = 1.0f;
    private static float lamellarArmorHealingEffectivnessMultiplyPerLevel = 0.05f;
    private static int lamellarArmorHealingEffectivnessMultiplyReductionEveryLevel = 1;
    private static float lamellarArmorHealingEffectivnessMultiplyReductionPerReduce = 0.05f;

    private static float lamellarArmorHungerRateMultiply = 1.0f;
    private static float lamellarArmorHungerRateMultiplyPerLevel = 0.05f;
    private static int lamellarArmorHungerRateMultiplyReductionEveryLevel = 1;
    private static float lamellarArmorHungerRateMultiplyReductionPerReduce = 0.05f;

    private static float lamellarArmorRangedWeaponsAccuracyMultiply = 1.0f;
    private static float lamellarArmorRangedWeaponsAccuracyMultiplyPerLevel = 0.05f;
    private static int lamellarArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel = 1;
    private static float lamellarArmorRangedWeaponsAccuracyMultiplyReductionPerReduce = 0.05f;

    private static float lamellarArmorRangedWeaponsSpeedMultiply = 1.0f;
    private static float lamellarArmorRangedWeaponsSpeedMultiplyPerLevel = 0.05f;
    private static int lamellarArmorRangedWeaponsSpeedMultiplyReductionEveryLevel = 1;
    private static float lamellarArmorRangedWeaponsSpeedMultiplyReductionPerReduce = 0.05f;

    private static float lamellarArmorWalkSpeedMultiply = 1.0f;
    private static float lamellarArmorWalkSpeedMultiplyPerLevel = 0.05f;
    private static int lamellarArmorWalkSpeedMultiplyReductionEveryLevel = 1;
    private static float lamellarArmorWalkSpeedMultiplyReductionPerReduce = 0.05f;

    public static int lamellarArmorMaxLevel = 999;

    public static void PopulateLamellarArmorConfiguration(ICoreAPI api)
    {
        Dictionary<string, object> lamellarArmorLevelStats = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/levelstats",
            "lamellararmor",
            "levelup:config/levelstats/lamellararmor.json");
        { //lamellarArmorEXPPerReceiveHit
            if (lamellarArmorLevelStats.TryGetValue("lamellarArmorEXPPerReceiveHit", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: lamellarArmorEXPPerReceiveHit is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: lamellarArmorEXPPerReceiveHit is not int is {value.GetType()}");
                else lamellarArmorEXPPerReceiveHit = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: lamellarArmorEXPPerReceiveHit not set");
            Experience.LoadExperience("LamellarArmor", "Hit", (ulong)lamellarArmorEXPPerReceiveHit);
        }
        { //lamellarArmorEXPMultiplyByDamage
            if (lamellarArmorLevelStats.TryGetValue("lamellarArmorEXPMultiplyByDamage", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: lamellarArmorEXPMultiplyByDamage is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: lamellarArmorEXPMultiplyByDamage is not double is {value.GetType()}");
                else lamellarArmorEXPMultiplyByDamage = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: lamellarArmorEXPMultiplyByDamage not set");
        }
        { //lamellarArmorEXPIncreaseByAmountDamage
            if (lamellarArmorLevelStats.TryGetValue("lamellarArmorEXPIncreaseByAmountDamage", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: lamellarArmorEXPIncreaseByAmountDamage is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: lamellarArmorEXPIncreaseByAmountDamage is not int is {value.GetType()}");
                else lamellarArmorEXPIncreaseByAmountDamage = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: lamellarArmorEXPIncreaseByAmountDamage not set");
        }
        { //lamellarArmorEXPPerLevelBase
            if (lamellarArmorLevelStats.TryGetValue("lamellarArmorEXPPerLevelBase", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: lamellarArmorEXPPerLevelBase is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: lamellarArmorEXPPerLevelBase is not int is {value.GetType()}");
                else lamellarArmorEXPPerLevelBase = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: lamellarArmorEXPPerLevelBase not set");
        }
        { //lamellarArmorEXPMultiplyPerLevel
            if (lamellarArmorLevelStats.TryGetValue("lamellarArmorEXPMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: lamellarArmorEXPMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: lamellarArmorEXPMultiplyPerLevel is not double is {value.GetType()}");
                else lamellarArmorEXPMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: lamellarArmorEXPMultiplyPerLevel not set");
        }
        { //lamellarArmorEXPMultiplyPerLevel
            if (lamellarArmorLevelStats.TryGetValue("lamellarArmorEXPMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: lamellarArmorEXPMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: lamellarArmorEXPMultiplyPerLevel is not double is {value.GetType()}");
                else lamellarArmorEXPMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: lamellarArmorEXPMultiplyPerLevel not set");
        }

        { //lamellarArmorRelativeProtectionMultiply
            if (lamellarArmorLevelStats.TryGetValue("lamellarArmorRelativeProtectionMultiply", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: lamellarArmorRelativeProtectionMultiply is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: lamellarArmorRelativeProtectionMultiply is not double is {value.GetType()}");
                else lamellarArmorRelativeProtectionMultiply = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: lamellarArmorRelativeProtectionMultiply not set");
        }
        { //lamellarArmorRelativeProtectionMultiplyPerLevel
            if (lamellarArmorLevelStats.TryGetValue("lamellarArmorRelativeProtectionMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: lamellarArmorRelativeProtectionMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: lamellarArmorRelativeProtectionMultiplyPerLevel is not double is {value.GetType()}");
                else lamellarArmorRelativeProtectionMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: lamellarArmorRelativeProtectionMultiplyPerLevel not set");
        }
        { //lamellarArmorRelativeProtectionMultiplyReductionEveryLevel
            if (lamellarArmorLevelStats.TryGetValue("lamellarArmorRelativeProtectionMultiplyReductionEveryLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: lamellarArmorRelativeProtectionMultiplyReductionEveryLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: lamellarArmorRelativeProtectionMultiplyReductionEveryLevel is not int is {value.GetType()}");
                else lamellarArmorRelativeProtectionMultiplyReductionEveryLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: lamellarArmorRelativeProtectionMultiplyReductionEveryLevel not set");
        }
        { //lamellarArmorRelativeProtectionMultiplyReductionPerReduce
            if (lamellarArmorLevelStats.TryGetValue("lamellarArmorRelativeProtectionMultiplyReductionPerReduce", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: lamellarArmorRelativeProtectionMultiplyReductionPerReduce is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: lamellarArmorRelativeProtectionMultiplyReductionPerReduce is not double is {value.GetType()}");
                else lamellarArmorRelativeProtectionMultiplyReductionPerReduce = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: lamellarArmorRelativeProtectionMultiplyReductionPerReduce not set");
        }

        { //lamellarArmorFlatDamageReductionMultiply
            if (lamellarArmorLevelStats.TryGetValue("lamellarArmorFlatDamageReductionMultiply", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: lamellarArmorFlatDamageReductionMultiply is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: lamellarArmorFlatDamageReductionMultiply is not double is {value.GetType()}");
                else lamellarArmorFlatDamageReductionMultiply = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: lamellarArmorFlatDamageReductionMultiply not set");
        }
        { //lamellarArmorFlatDamageReductionMultiplyPerLevel
            if (lamellarArmorLevelStats.TryGetValue("lamellarArmorFlatDamageReductionMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: lamellarArmorFlatDamageReductionMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: lamellarArmorFlatDamageReductionMultiplyPerLevel is not double is {value.GetType()}");
                else lamellarArmorFlatDamageReductionMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: lamellarArmorFlatDamageReductionMultiplyPerLevel not set");
        }
        { //lamellarArmorFlatDamageReductionMultiplyReductionEveryLevel
            if (lamellarArmorLevelStats.TryGetValue("lamellarArmorFlatDamageReductionMultiplyReductionEveryLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: lamellarArmorFlatDamageReductionMultiplyReductionEveryLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: lamellarArmorFlatDamageReductionMultiplyReductionEveryLevel is not int is {value.GetType()}");
                else lamellarArmorFlatDamageReductionMultiplyReductionEveryLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: lamellarArmorFlatDamageReductionMultiplyReductionEveryLevel not set");
        }
        { //lamellarArmorFlatDamageReductionMultiplyReductionPerReduce
            if (lamellarArmorLevelStats.TryGetValue("lamellarArmorFlatDamageReductionMultiplyReductionPerReduce", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: lamellarArmorFlatDamageReductionMultiplyReductionPerReduce is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: lamellarArmorFlatDamageReductionMultiplyReductionPerReduce is not double is {value.GetType()}");
                else lamellarArmorFlatDamageReductionMultiplyReductionPerReduce = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: lamellarArmorFlatDamageReductionMultiplyReductionPerReduce not set");
        }

        { //lamellarArmorHealingEffectivnessMultiply
            if (lamellarArmorLevelStats.TryGetValue("lamellarArmorHealingEffectivnessMultiply", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: lamellarArmorHealingEffectivnessMultiply is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: lamellarArmorHealingEffectivnessMultiply is not double is {value.GetType()}");
                else lamellarArmorHealingEffectivnessMultiply = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: lamellarArmorHealingEffectivnessMultiply not set");
        }
        { //lamellarArmorHealingEffectivnessMultiplyPerLevel
            if (lamellarArmorLevelStats.TryGetValue("lamellarArmorHealingEffectivnessMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: lamellarArmorHealingEffectivnessMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: lamellarArmorHealingEffectivnessMultiplyPerLevel is not double is {value.GetType()}");
                else lamellarArmorHealingEffectivnessMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: lamellarArmorHealingEffectivnessMultiplyPerLevel not set");
        }
        { //lamellarArmorHealingEffectivnessMultiplyReductionEveryLevel
            if (lamellarArmorLevelStats.TryGetValue("lamellarArmorHealingEffectivnessMultiplyReductionEveryLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: lamellarArmorHealingEffectivnessMultiplyReductionEveryLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: lamellarArmorHealingEffectivnessMultiplyReductionEveryLevel is not int is {value.GetType()}");
                else lamellarArmorHealingEffectivnessMultiplyReductionEveryLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: lamellarArmorHealingEffectivnessMultiplyReductionEveryLevel not set");
        }
        { //lamellarArmorHealingEffectivnessMultiplyReductionPerReduce
            if (lamellarArmorLevelStats.TryGetValue("lamellarArmorHealingEffectivnessMultiplyReductionPerReduce", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: lamellarArmorHealingEffectivnessMultiplyReductionPerReduce is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: lamellarArmorHealingEffectivnessMultiplyReductionPerReduce is not double is {value.GetType()}");
                else lamellarArmorHealingEffectivnessMultiplyReductionPerReduce = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: lamellarArmorHealingEffectivnessMultiplyReductionPerReduce not set");
        }

        { //lamellarArmorHungerRateMultiply
            if (lamellarArmorLevelStats.TryGetValue("lamellarArmorHungerRateMultiply", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: lamellarArmorHungerRateMultiply is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: lamellarArmorHungerRateMultiply is not double is {value.GetType()}");
                else lamellarArmorHungerRateMultiply = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: lamellarArmorHungerRateMultiply not set");
        }
        { //lamellarArmorHungerRateMultiplyPerLevel
            if (lamellarArmorLevelStats.TryGetValue("lamellarArmorHungerRateMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: lamellarArmorHungerRateMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: lamellarArmorHungerRateMultiplyPerLevel is not double is {value.GetType()}");
                else lamellarArmorHungerRateMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: lamellarArmorHungerRateMultiplyPerLevel not set");
        }
        { //lamellarArmorHungerRateMultiplyReductionEveryLevel
            if (lamellarArmorLevelStats.TryGetValue("lamellarArmorHungerRateMultiplyReductionEveryLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: lamellarArmorHungerRateMultiplyReductionEveryLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: lamellarArmorHungerRateMultiplyReductionEveryLevel is not int is {value.GetType()}");
                else lamellarArmorHungerRateMultiplyReductionEveryLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: lamellarArmorHungerRateMultiplyReductionEveryLevel not set");
        }
        { //lamellarArmorHungerRateMultiplyReductionPerReduce
            if (lamellarArmorLevelStats.TryGetValue("lamellarArmorHungerRateMultiplyReductionPerReduce", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: lamellarArmorHungerRateMultiplyReductionPerReduce is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: lamellarArmorHungerRateMultiplyReductionPerReduce is not double is {value.GetType()}");
                else lamellarArmorHungerRateMultiplyReductionPerReduce = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: lamellarArmorHungerRateMultiplyReductionPerReduce not set");
        }

        { //lamellarArmorRangedWeaponsAccuracyMultiply
            if (lamellarArmorLevelStats.TryGetValue("lamellarArmorRangedWeaponsAccuracyMultiply", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: lamellarArmorRangedWeaponsAccuracyMultiply is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: lamellarArmorRangedWeaponsAccuracyMultiply is not double is {value.GetType()}");
                else lamellarArmorRangedWeaponsAccuracyMultiply = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: lamellarArmorRangedWeaponsAccuracyMultiply not set");
        }
        { //lamellarArmorRangedWeaponsAccuracyMultiplyPerLevel
            if (lamellarArmorLevelStats.TryGetValue("lamellarArmorRangedWeaponsAccuracyMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: lamellarArmorRangedWeaponsAccuracyMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: lamellarArmorRangedWeaponsAccuracyMultiplyPerLevel is not double is {value.GetType()}");
                else lamellarArmorRangedWeaponsAccuracyMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: lamellarArmorRangedWeaponsAccuracyMultiplyPerLevel not set");
        }
        { //lamellarArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel
            if (lamellarArmorLevelStats.TryGetValue("lamellarArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: lamellarArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: lamellarArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel is not int is {value.GetType()}");
                else lamellarArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: lamellarArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel not set");
        }
        { //lamellarArmorRangedWeaponsAccuracyMultiplyReductionPerReduce
            if (lamellarArmorLevelStats.TryGetValue("lamellarArmorRangedWeaponsAccuracyMultiplyReductionPerReduce", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: lamellarArmorRangedWeaponsAccuracyMultiplyReductionPerReduce is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: lamellarArmorRangedWeaponsAccuracyMultiplyReductionPerReduce is not double is {value.GetType()}");
                else lamellarArmorRangedWeaponsAccuracyMultiplyReductionPerReduce = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: lamellarArmorRangedWeaponsAccuracyMultiplyReductionPerReduce not set");
        }

        { //lamellarArmorRangedWeaponsSpeedMultiply
            if (lamellarArmorLevelStats.TryGetValue("lamellarArmorRangedWeaponsSpeedMultiply", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: lamellarArmorRangedWeaponsSpeedMultiply is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: lamellarArmorRangedWeaponsSpeedMultiply is not double is {value.GetType()}");
                else lamellarArmorRangedWeaponsSpeedMultiply = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: lamellarArmorRangedWeaponsSpeedMultiply not set");
        }
        { //lamellarArmorRangedWeaponsSpeedMultiplyPerLevel
            if (lamellarArmorLevelStats.TryGetValue("lamellarArmorRangedWeaponsSpeedMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: lamellarArmorRangedWeaponsSpeedMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: lamellarArmorRangedWeaponsSpeedMultiplyPerLevel is not double is {value.GetType()}");
                else lamellarArmorRangedWeaponsSpeedMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: lamellarArmorRangedWeaponsSpeedMultiplyPerLevel not set");
        }
        { //lamellarArmorRangedWeaponsSpeedMultiplyReductionEveryLevel
            if (lamellarArmorLevelStats.TryGetValue("lamellarArmorRangedWeaponsSpeedMultiplyReductionEveryLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: lamellarArmorRangedWeaponsSpeedMultiplyReductionEveryLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: lamellarArmorRangedWeaponsSpeedMultiplyReductionEveryLevel is not int is {value.GetType()}");
                else lamellarArmorRangedWeaponsSpeedMultiplyReductionEveryLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: lamellarArmorRangedWeaponsSpeedMultiplyReductionEveryLevel not set");
        }
        { //lamellarArmorRangedWeaponsSpeedMultiplyReductionPerReduce
            if (lamellarArmorLevelStats.TryGetValue("lamellarArmorRangedWeaponsSpeedMultiplyReductionPerReduce", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: lamellarArmorRangedWeaponsSpeedMultiplyReductionPerReduce is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: lamellarArmorRangedWeaponsSpeedMultiplyReductionPerReduce is not double is {value.GetType()}");
                else lamellarArmorRangedWeaponsSpeedMultiplyReductionPerReduce = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: lamellarArmorRangedWeaponsSpeedMultiplyReductionPerReduce not set");
        }

        { //lamellarArmorWalkSpeedMultiply
            if (lamellarArmorLevelStats.TryGetValue("lamellarArmorWalkSpeedMultiply", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: lamellarArmorWalkSpeedMultiply is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: lamellarArmorWalkSpeedMultiply is not double is {value.GetType()}");
                else lamellarArmorWalkSpeedMultiply = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: lamellarArmorWalkSpeedMultiply not set");
        }
        { //lamellarArmorWalkSpeedMultiplyPerLevel
            if (lamellarArmorLevelStats.TryGetValue("lamellarArmorWalkSpeedMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: lamellarArmorWalkSpeedMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: lamellarArmorWalkSpeedMultiplyPerLevel is not double is {value.GetType()}");
                else lamellarArmorWalkSpeedMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: lamellarArmorWalkSpeedMultiplyPerLevel not set");
        }
        { //lamellarArmorWalkSpeedMultiplyReductionEveryLevel
            if (lamellarArmorLevelStats.TryGetValue("lamellarArmorWalkSpeedMultiplyReductionEveryLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: lamellarArmorWalkSpeedMultiplyReductionEveryLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: lamellarArmorWalkSpeedMultiplyReductionEveryLevel is not int is {value.GetType()}");
                else lamellarArmorWalkSpeedMultiplyReductionEveryLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: lamellarArmorWalkSpeedMultiplyReductionEveryLevel not set");
        }
        { //lamellarArmorWalkSpeedMultiplyReductionPerReduce
            if (lamellarArmorLevelStats.TryGetValue("lamellarArmorWalkSpeedMultiplyReductionPerReduce", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: lamellarArmorWalkSpeedMultiplyReductionPerReduce is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: lamellarArmorWalkSpeedMultiplyReductionPerReduce is not double is {value.GetType()}");
                else lamellarArmorWalkSpeedMultiplyReductionPerReduce = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: lamellarArmorWalkSpeedMultiplyReductionPerReduce not set");
        }

        { //lamellarArmorMaxLevel
            if (lamellarArmorLevelStats.TryGetValue("lamellarArmorMaxLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: lamellarArmorMaxLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: lamellarArmorMaxLevel is not int is {value.GetType()}");
                else lamellarArmorMaxLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: lamellarArmorMaxLevel not set");
        }

        // Get lamellar armor multiply exp
        expMultiplyHitLamellarArmor.Clear();
        Dictionary<string, object> tmpexpMultiplyHitLamellarArmor = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/levelstats",
            "lamellararmoritems",
            "levelup:config/levelstats/lamellararmoritems.json");
        foreach (KeyValuePair<string, object> pair in tmpexpMultiplyHitLamellarArmor)
        {
            if (pair.Value is double value) expMultiplyHitLamellarArmor.Add(pair.Key, (double)value);
            else Debug.Log($"CONFIGURATION ERROR: expMultiplyHitLamellarArmor {pair.Key} is not double");
        }
        Debug.Log("Lamellar Armor configuration set");
    }

    public static int LamellarArmorGetLevelByEXP(ulong exp)
    {
        double baseExp = lamellarArmorEXPPerLevelBase;
        double multiplier = lamellarArmorEXPMultiplyPerLevel;

        if (multiplier <= 1.0)
        {
            return (int)(exp / baseExp);
        }

        double expDouble = exp;

        double level = Math.Log((expDouble * (multiplier - 1) / baseExp) + 1) / Math.Log(multiplier);

        return Math.Max(0, (int)Math.Floor(level));
    }

    public static ulong LamellarArmorGetExpByLevel(int level)
    {
        double baseExp = lamellarArmorEXPPerLevelBase;
        double multiplier = lamellarArmorEXPMultiplyPerLevel;

        if (multiplier == 1.0)
        {
            return (ulong)(baseExp * level);
        }

        double exp = baseExp * (Math.Pow(multiplier, level) - 1) / (multiplier - 1);
        return (ulong)Math.Floor(exp);
    }

    public static int LamellarArmorBaseEXPEarnedByDAMAGE(float damage)
    {
        int calcDamage = (int)Math.Round(damage);
        int multiplesCount = calcDamage / lamellarArmorEXPIncreaseByAmountDamage;
        float multiplier = 1 + lamellarArmorEXPMultiplyByDamage;

        float baseMultiply = lamellarArmorEXPPerReceiveHit * (float)Math.Pow(multiplier, multiplesCount);

        return (int)Math.Round(baseMultiply);
    }

    public static float LamellarArmorRelativeProtectionMultiplyByLevel(int level)
    {
        int reduceEvery = lamellarArmorRelativeProtectionMultiplyReductionEveryLevel;
        float baseMultiply = lamellarArmorRelativeProtectionMultiply;
        float baseIncrement = lamellarArmorRelativeProtectionMultiplyPerLevel;
        float reductionPerStep = lamellarArmorRelativeProtectionMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double multiply = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        multiply += baseMultiply;

        return (float)multiply;
    }

    public static float LamellarArmorFlatDamageReductionMultiplyByLevel(int level)
    {
        int reduceEvery = lamellarArmorFlatDamageReductionMultiplyReductionEveryLevel;
        float baseMultiply = lamellarArmorFlatDamageReductionMultiply;
        float baseIncrement = lamellarArmorFlatDamageReductionMultiplyPerLevel;
        float reductionPerStep = lamellarArmorFlatDamageReductionMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double multiply = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        multiply += baseMultiply;

        return (float)multiply;
    }

    public static float LamellarArmorHealingEffectivnessMultiplyByLevel(int level)
    {
        int reduceEvery = lamellarArmorHealingEffectivnessMultiplyReductionEveryLevel;
        float baseMultiply = lamellarArmorHealingEffectivnessMultiply;
        float baseIncrement = lamellarArmorHealingEffectivnessMultiplyPerLevel;
        float reductionPerStep = lamellarArmorHealingEffectivnessMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double multiply = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        multiply += baseMultiply;

        return (float)multiply;
    }

    public static float LamellarArmorHungerRateMultiplyByLevel(int level)
    {
        int reduceEvery = lamellarArmorHungerRateMultiplyReductionEveryLevel;
        float baseMultiply = lamellarArmorHungerRateMultiply;
        float baseIncrement = lamellarArmorHungerRateMultiplyPerLevel;
        float reductionPerStep = lamellarArmorHungerRateMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double multiply = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        multiply += baseMultiply;

        return (float)multiply;
    }

    public static float LamellarArmorRangedWeaponsAccuracyMultiplyByLevel(int level)
    {
        int reduceEvery = lamellarArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel;
        float baseMultiply = lamellarArmorRangedWeaponsAccuracyMultiply;
        float baseIncrement = lamellarArmorRangedWeaponsAccuracyMultiplyPerLevel;
        float reductionPerStep = lamellarArmorRangedWeaponsAccuracyMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double multiply = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        multiply += baseMultiply;

        return (float)multiply;
    }

    public static float LamellarArmorRangedWeaponsSpeedMultiplyByLevel(int level)
    {
        int reduceEvery = lamellarArmorRangedWeaponsSpeedMultiplyReductionEveryLevel;
        float baseMultiply = lamellarArmorRangedWeaponsSpeedMultiply;
        float baseIncrement = lamellarArmorRangedWeaponsSpeedMultiplyPerLevel;
        float reductionPerStep = lamellarArmorRangedWeaponsSpeedMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double multiply = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        multiply += baseMultiply;

        return (float)multiply;
    }

    public static float LamellarArmorWalkSpeedMultiplyByLevel(int level)
    {
        int reduceEvery = lamellarArmorWalkSpeedMultiplyReductionEveryLevel;
        float baseMultiply = lamellarArmorWalkSpeedMultiply;
        float baseIncrement = lamellarArmorWalkSpeedMultiplyPerLevel;
        float reductionPerStep = lamellarArmorWalkSpeedMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double multiply = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        multiply += baseMultiply;

        return (float)multiply;
    }
    #endregion

    #region platearmor
    public static Dictionary<string, double> expMultiplyHitPlateArmor = [];
    private static int plateArmorEXPPerReceiveHit = 10;
    private static float plateArmorEXPMultiplyByDamage = 0.3f;
    private static int plateArmorEXPIncreaseByAmountDamage = 20;
    private static int plateArmorEXPPerLevelBase = 500;
    private static double plateArmorEXPMultiplyPerLevel = 1.2;

    private static float plateArmorRelativeProtectionMultiply = 1.0f;
    private static float plateArmorRelativeProtectionMultiplyPerLevel = 0.05f;
    private static int plateArmorRelativeProtectionMultiplyReductionEveryLevel = 1;
    private static float plateArmorRelativeProtectionMultiplyReductionPerReduce = 0.05f;

    private static float plateArmorFlatDamageReductionMultiply = 1.0f;
    private static float plateArmorFlatDamageReductionMultiplyPerLevel = 0.05f;
    private static int plateArmorFlatDamageReductionMultiplyReductionEveryLevel = 1;
    private static float plateArmorFlatDamageReductionMultiplyReductionPerReduce = 0.05f;

    private static float plateArmorHealingEffectivnessMultiply = 1.0f;
    private static float plateArmorHealingEffectivnessMultiplyPerLevel = 0.05f;
    private static int plateArmorHealingEffectivnessMultiplyReductionEveryLevel = 1;
    private static float plateArmorHealingEffectivnessMultiplyReductionPerReduce = 0.05f;

    private static float plateArmorHungerRateMultiply = 1.0f;
    private static float plateArmorHungerRateMultiplyPerLevel = 0.05f;
    private static int plateArmorHungerRateMultiplyReductionEveryLevel = 1;
    private static float plateArmorHungerRateMultiplyReductionPerReduce = 0.05f;

    private static float plateArmorRangedWeaponsAccuracyMultiply = 1.0f;
    private static float plateArmorRangedWeaponsAccuracyMultiplyPerLevel = 0.05f;
    private static int plateArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel = 1;
    private static float plateArmorRangedWeaponsAccuracyMultiplyReductionPerReduce = 0.05f;

    private static float plateArmorRangedWeaponsSpeedMultiply = 1.0f;
    private static float plateArmorRangedWeaponsSpeedMultiplyPerLevel = 0.05f;
    private static int plateArmorRangedWeaponsSpeedMultiplyReductionEveryLevel = 1;
    private static float plateArmorRangedWeaponsSpeedMultiplyReductionPerReduce = 0.05f;

    private static float plateArmorWalkSpeedMultiply = 1.0f;
    private static float plateArmorWalkSpeedMultiplyPerLevel = 0.05f;
    private static int plateArmorWalkSpeedMultiplyReductionEveryLevel = 1;
    private static float plateArmorWalkSpeedMultiplyReductionPerReduce = 0.05f;

    public static int plateArmorMaxLevel = 999;

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
        { //plateArmorEXPMultiplyPerLevel
            if (plateArmorLevelStats.TryGetValue("plateArmorEXPMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: plateArmorEXPMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: plateArmorEXPMultiplyPerLevel is not double is {value.GetType()}");
                else plateArmorEXPMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: plateArmorEXPMultiplyPerLevel not set");
        }

        { //plateArmorRelativeProtectionMultiply
            if (plateArmorLevelStats.TryGetValue("plateArmorRelativeProtectionMultiply", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: plateArmorRelativeProtectionMultiply is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: plateArmorRelativeProtectionMultiply is not double is {value.GetType()}");
                else plateArmorRelativeProtectionMultiply = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: plateArmorRelativeProtectionMultiply not set");
        }
        { //plateArmorRelativeProtectionMultiplyPerLevel
            if (plateArmorLevelStats.TryGetValue("plateArmorRelativeProtectionMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: plateArmorRelativeProtectionMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: plateArmorRelativeProtectionMultiplyPerLevel is not double is {value.GetType()}");
                else plateArmorRelativeProtectionMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: plateArmorRelativeProtectionMultiplyPerLevel not set");
        }
        { //plateArmorRelativeProtectionMultiplyReductionEveryLevel
            if (plateArmorLevelStats.TryGetValue("plateArmorRelativeProtectionMultiplyReductionEveryLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: plateArmorRelativeProtectionMultiplyReductionEveryLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: plateArmorRelativeProtectionMultiplyReductionEveryLevel is not int is {value.GetType()}");
                else plateArmorRelativeProtectionMultiplyReductionEveryLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: plateArmorRelativeProtectionMultiplyReductionEveryLevel not set");
        }
        { //plateArmorRelativeProtectionMultiplyReductionPerReduce
            if (plateArmorLevelStats.TryGetValue("plateArmorRelativeProtectionMultiplyReductionPerReduce", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: plateArmorRelativeProtectionMultiplyReductionPerReduce is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: plateArmorRelativeProtectionMultiplyReductionPerReduce is not double is {value.GetType()}");
                else plateArmorRelativeProtectionMultiplyReductionPerReduce = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: plateArmorRelativeProtectionMultiplyReductionPerReduce not set");
        }

        { //plateArmorFlatDamageReductionMultiply
            if (plateArmorLevelStats.TryGetValue("plateArmorFlatDamageReductionMultiply", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: plateArmorFlatDamageReductionMultiply is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: plateArmorFlatDamageReductionMultiply is not double is {value.GetType()}");
                else plateArmorFlatDamageReductionMultiply = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: plateArmorFlatDamageReductionMultiply not set");
        }
        { //plateArmorFlatDamageReductionMultiplyPerLevel
            if (plateArmorLevelStats.TryGetValue("plateArmorFlatDamageReductionMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: plateArmorFlatDamageReductionMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: plateArmorFlatDamageReductionMultiplyPerLevel is not double is {value.GetType()}");
                else plateArmorFlatDamageReductionMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: plateArmorFlatDamageReductionMultiplyPerLevel not set");
        }
        { //plateArmorFlatDamageReductionMultiplyReductionEveryLevel
            if (plateArmorLevelStats.TryGetValue("plateArmorFlatDamageReductionMultiplyReductionEveryLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: plateArmorFlatDamageReductionMultiplyReductionEveryLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: plateArmorFlatDamageReductionMultiplyReductionEveryLevel is not int is {value.GetType()}");
                else plateArmorFlatDamageReductionMultiplyReductionEveryLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: plateArmorFlatDamageReductionMultiplyReductionEveryLevel not set");
        }
        { //plateArmorFlatDamageReductionMultiplyReductionPerReduce
            if (plateArmorLevelStats.TryGetValue("plateArmorFlatDamageReductionMultiplyReductionPerReduce", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: plateArmorFlatDamageReductionMultiplyReductionPerReduce is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: plateArmorFlatDamageReductionMultiplyReductionPerReduce is not double is {value.GetType()}");
                else plateArmorFlatDamageReductionMultiplyReductionPerReduce = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: plateArmorFlatDamageReductionMultiplyReductionPerReduce not set");
        }

        { //plateArmorHealingEffectivnessMultiply
            if (plateArmorLevelStats.TryGetValue("plateArmorHealingEffectivnessMultiply", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: plateArmorHealingEffectivnessMultiply is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: plateArmorHealingEffectivnessMultiply is not double is {value.GetType()}");
                else plateArmorHealingEffectivnessMultiply = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: plateArmorHealingEffectivnessMultiply not set");
        }
        { //plateArmorHealingEffectivnessMultiplyPerLevel
            if (plateArmorLevelStats.TryGetValue("plateArmorHealingEffectivnessMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: plateArmorHealingEffectivnessMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: plateArmorHealingEffectivnessMultiplyPerLevel is not double is {value.GetType()}");
                else plateArmorHealingEffectivnessMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: plateArmorHealingEffectivnessMultiplyPerLevel not set");
        }
        { //plateArmorHealingEffectivnessMultiplyReductionEveryLevel
            if (plateArmorLevelStats.TryGetValue("plateArmorHealingEffectivnessMultiplyReductionEveryLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: plateArmorHealingEffectivnessMultiplyReductionEveryLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: plateArmorHealingEffectivnessMultiplyReductionEveryLevel is not int is {value.GetType()}");
                else plateArmorHealingEffectivnessMultiplyReductionEveryLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: plateArmorHealingEffectivnessMultiplyReductionEveryLevel not set");
        }
        { //plateArmorHealingEffectivnessMultiplyReductionPerReduce
            if (plateArmorLevelStats.TryGetValue("plateArmorHealingEffectivnessMultiplyReductionPerReduce", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: plateArmorHealingEffectivnessMultiplyReductionPerReduce is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: plateArmorHealingEffectivnessMultiplyReductionPerReduce is not double is {value.GetType()}");
                else plateArmorHealingEffectivnessMultiplyReductionPerReduce = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: plateArmorHealingEffectivnessMultiplyReductionPerReduce not set");
        }

        { //plateArmorHungerRateMultiply
            if (plateArmorLevelStats.TryGetValue("plateArmorHungerRateMultiply", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: plateArmorHungerRateMultiply is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: plateArmorHungerRateMultiply is not double is {value.GetType()}");
                else plateArmorHungerRateMultiply = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: plateArmorHungerRateMultiply not set");
        }
        { //plateArmorHungerRateMultiplyPerLevel
            if (plateArmorLevelStats.TryGetValue("plateArmorHungerRateMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: plateArmorHungerRateMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: plateArmorHungerRateMultiplyPerLevel is not double is {value.GetType()}");
                else plateArmorHungerRateMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: plateArmorHungerRateMultiplyPerLevel not set");
        }
        { //plateArmorHungerRateMultiplyReductionEveryLevel
            if (plateArmorLevelStats.TryGetValue("plateArmorHungerRateMultiplyReductionEveryLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: plateArmorHungerRateMultiplyReductionEveryLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: plateArmorHungerRateMultiplyReductionEveryLevel is not int is {value.GetType()}");
                else plateArmorHungerRateMultiplyReductionEveryLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: plateArmorHungerRateMultiplyReductionEveryLevel not set");
        }
        { //plateArmorHungerRateMultiplyReductionPerReduce
            if (plateArmorLevelStats.TryGetValue("plateArmorHungerRateMultiplyReductionPerReduce", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: plateArmorHungerRateMultiplyReductionPerReduce is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: plateArmorHungerRateMultiplyReductionPerReduce is not double is {value.GetType()}");
                else plateArmorHungerRateMultiplyReductionPerReduce = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: plateArmorHungerRateMultiplyReductionPerReduce not set");
        }

        { //plateArmorRangedWeaponsAccuracyMultiply
            if (plateArmorLevelStats.TryGetValue("plateArmorRangedWeaponsAccuracyMultiply", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: plateArmorRangedWeaponsAccuracyMultiply is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: plateArmorRangedWeaponsAccuracyMultiply is not double is {value.GetType()}");
                else plateArmorRangedWeaponsAccuracyMultiply = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: plateArmorRangedWeaponsAccuracyMultiply not set");
        }
        { //plateArmorRangedWeaponsAccuracyMultiplyPerLevel
            if (plateArmorLevelStats.TryGetValue("plateArmorRangedWeaponsAccuracyMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: plateArmorRangedWeaponsAccuracyMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: plateArmorRangedWeaponsAccuracyMultiplyPerLevel is not double is {value.GetType()}");
                else plateArmorRangedWeaponsAccuracyMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: plateArmorRangedWeaponsAccuracyMultiplyPerLevel not set");
        }
        { //plateArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel
            if (plateArmorLevelStats.TryGetValue("plateArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: plateArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: plateArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel is not int is {value.GetType()}");
                else plateArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: plateArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel not set");
        }
        { //plateArmorRangedWeaponsAccuracyMultiplyReductionPerReduce
            if (plateArmorLevelStats.TryGetValue("plateArmorRangedWeaponsAccuracyMultiplyReductionPerReduce", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: plateArmorRangedWeaponsAccuracyMultiplyReductionPerReduce is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: plateArmorRangedWeaponsAccuracyMultiplyReductionPerReduce is not double is {value.GetType()}");
                else plateArmorRangedWeaponsAccuracyMultiplyReductionPerReduce = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: plateArmorRangedWeaponsAccuracyMultiplyReductionPerReduce not set");
        }

        { //plateArmorRangedWeaponsSpeedMultiply
            if (plateArmorLevelStats.TryGetValue("plateArmorRangedWeaponsSpeedMultiply", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: plateArmorRangedWeaponsSpeedMultiply is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: plateArmorRangedWeaponsSpeedMultiply is not double is {value.GetType()}");
                else plateArmorRangedWeaponsSpeedMultiply = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: plateArmorRangedWeaponsSpeedMultiply not set");
        }
        { //plateArmorRangedWeaponsSpeedMultiplyPerLevel
            if (plateArmorLevelStats.TryGetValue("plateArmorRangedWeaponsSpeedMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: plateArmorRangedWeaponsSpeedMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: plateArmorRangedWeaponsSpeedMultiplyPerLevel is not double is {value.GetType()}");
                else plateArmorRangedWeaponsSpeedMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: plateArmorRangedWeaponsSpeedMultiplyPerLevel not set");
        }
        { //plateArmorRangedWeaponsSpeedMultiplyReductionEveryLevel
            if (plateArmorLevelStats.TryGetValue("plateArmorRangedWeaponsSpeedMultiplyReductionEveryLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: plateArmorRangedWeaponsSpeedMultiplyReductionEveryLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: plateArmorRangedWeaponsSpeedMultiplyReductionEveryLevel is not int is {value.GetType()}");
                else plateArmorRangedWeaponsSpeedMultiplyReductionEveryLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: plateArmorRangedWeaponsSpeedMultiplyReductionEveryLevel not set");
        }
        { //plateArmorRangedWeaponsSpeedMultiplyReductionPerReduce
            if (plateArmorLevelStats.TryGetValue("plateArmorRangedWeaponsSpeedMultiplyReductionPerReduce", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: plateArmorRangedWeaponsSpeedMultiplyReductionPerReduce is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: plateArmorRangedWeaponsSpeedMultiplyReductionPerReduce is not double is {value.GetType()}");
                else plateArmorRangedWeaponsSpeedMultiplyReductionPerReduce = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: plateArmorRangedWeaponsSpeedMultiplyReductionPerReduce not set");
        }

        { //plateArmorWalkSpeedMultiply
            if (plateArmorLevelStats.TryGetValue("plateArmorWalkSpeedMultiply", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: plateArmorWalkSpeedMultiply is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: plateArmorWalkSpeedMultiply is not double is {value.GetType()}");
                else plateArmorWalkSpeedMultiply = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: plateArmorWalkSpeedMultiply not set");
        }
        { //plateArmorWalkSpeedMultiplyPerLevel
            if (plateArmorLevelStats.TryGetValue("plateArmorWalkSpeedMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: plateArmorWalkSpeedMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: plateArmorWalkSpeedMultiplyPerLevel is not double is {value.GetType()}");
                else plateArmorWalkSpeedMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: plateArmorWalkSpeedMultiplyPerLevel not set");
        }
        { //plateArmorWalkSpeedMultiplyReductionEveryLevel
            if (plateArmorLevelStats.TryGetValue("plateArmorWalkSpeedMultiplyReductionEveryLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: plateArmorWalkSpeedMultiplyReductionEveryLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: plateArmorWalkSpeedMultiplyReductionEveryLevel is not int is {value.GetType()}");
                else plateArmorWalkSpeedMultiplyReductionEveryLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: plateArmorWalkSpeedMultiplyReductionEveryLevel not set");
        }
        { //plateArmorWalkSpeedMultiplyReductionPerReduce
            if (plateArmorLevelStats.TryGetValue("plateArmorWalkSpeedMultiplyReductionPerReduce", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: plateArmorWalkSpeedMultiplyReductionPerReduce is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: plateArmorWalkSpeedMultiplyReductionPerReduce is not double is {value.GetType()}");
                else plateArmorWalkSpeedMultiplyReductionPerReduce = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: plateArmorWalkSpeedMultiplyReductionPerReduce not set");
        }

        { //plateArmorMaxLevel
            if (plateArmorLevelStats.TryGetValue("plateArmorMaxLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: plateArmorMaxLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: plateArmorMaxLevel is not int is {value.GetType()}");
                else plateArmorMaxLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: plateArmorMaxLevel not set");
        }

        // Get plate armor multiply exp
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

    public static float PlateArmorRelativeProtectionMultiplyByLevel(int level)
    {
        int reduceEvery = plateArmorRelativeProtectionMultiplyReductionEveryLevel;
        float baseMultiply = plateArmorRelativeProtectionMultiply;
        float baseIncrement = plateArmorRelativeProtectionMultiplyPerLevel;
        float reductionPerStep = plateArmorRelativeProtectionMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double multiply = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        multiply += baseMultiply;

        return (float)multiply;
    }

    public static float PlateArmorFlatDamageReductionMultiplyByLevel(int level)
    {
        int reduceEvery = plateArmorFlatDamageReductionMultiplyReductionEveryLevel;
        float baseMultiply = plateArmorFlatDamageReductionMultiply;
        float baseIncrement = plateArmorFlatDamageReductionMultiplyPerLevel;
        float reductionPerStep = plateArmorFlatDamageReductionMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double multiply = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        multiply += baseMultiply;

        return (float)multiply;
    }

    public static float PlateArmorHealingEffectivnessMultiplyByLevel(int level)
    {
        int reduceEvery = plateArmorHealingEffectivnessMultiplyReductionEveryLevel;
        float baseMultiply = plateArmorHealingEffectivnessMultiply;
        float baseIncrement = plateArmorHealingEffectivnessMultiplyPerLevel;
        float reductionPerStep = plateArmorHealingEffectivnessMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double multiply = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        multiply += baseMultiply;

        return (float)multiply;
    }

    public static float PlateArmorHungerRateMultiplyByLevel(int level)
    {
        int reduceEvery = plateArmorHungerRateMultiplyReductionEveryLevel;
        float baseMultiply = plateArmorHungerRateMultiply;
        float baseIncrement = plateArmorHungerRateMultiplyPerLevel;
        float reductionPerStep = plateArmorHungerRateMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double multiply = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        multiply += baseMultiply;

        return (float)multiply;
    }

    public static float PlateArmorRangedWeaponsAccuracyMultiplyByLevel(int level)
    {
        int reduceEvery = plateArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel;
        float baseMultiply = plateArmorRangedWeaponsAccuracyMultiply;
        float baseIncrement = plateArmorRangedWeaponsAccuracyMultiplyPerLevel;
        float reductionPerStep = plateArmorRangedWeaponsAccuracyMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double multiply = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        multiply += baseMultiply;

        return (float)multiply;
    }

    public static float PlateArmorRangedWeaponsSpeedMultiplyByLevel(int level)
    {
        int reduceEvery = plateArmorRangedWeaponsSpeedMultiplyReductionEveryLevel;
        float baseMultiply = plateArmorRangedWeaponsSpeedMultiply;
        float baseIncrement = plateArmorRangedWeaponsSpeedMultiplyPerLevel;
        float reductionPerStep = plateArmorRangedWeaponsSpeedMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double multiply = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        multiply += baseMultiply;

        return (float)multiply;
    }

    public static float PlateArmorWalkSpeedMultiplyByLevel(int level)
    {
        int reduceEvery = plateArmorWalkSpeedMultiplyReductionEveryLevel;
        float baseMultiply = plateArmorWalkSpeedMultiply;
        float baseIncrement = plateArmorWalkSpeedMultiplyPerLevel;
        float reductionPerStep = plateArmorWalkSpeedMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double multiply = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        multiply += baseMultiply;

        return (float)multiply;
    }
    #endregion

    #region scalearmor
    public static Dictionary<string, double> expMultiplyHitScaleArmor = [];
    private static int scaleArmorEXPPerReceiveHit = 10;
    private static float scaleArmorEXPMultiplyByDamage = 0.3f;
    private static int scaleArmorEXPIncreaseByAmountDamage = 20;
    private static int scaleArmorEXPPerLevelBase = 500;
    private static double scaleArmorEXPMultiplyPerLevel = 1.2;

    private static float scaleArmorRelativeProtectionMultiply = 1.0f;
    private static float scaleArmorRelativeProtectionMultiplyPerLevel = 0.05f;
    private static int scaleArmorRelativeProtectionMultiplyReductionEveryLevel = 1;
    private static float scaleArmorRelativeProtectionMultiplyReductionPerReduce = 0.05f;

    private static float scaleArmorFlatDamageReductionMultiply = 1.0f;
    private static float scaleArmorFlatDamageReductionMultiplyPerLevel = 0.05f;
    private static int scaleArmorFlatDamageReductionMultiplyReductionEveryLevel = 1;
    private static float scaleArmorFlatDamageReductionMultiplyReductionPerReduce = 0.05f;

    private static float scaleArmorHealingEffectivnessMultiply = 1.0f;
    private static float scaleArmorHealingEffectivnessMultiplyPerLevel = 0.05f;
    private static int scaleArmorHealingEffectivnessMultiplyReductionEveryLevel = 1;
    private static float scaleArmorHealingEffectivnessMultiplyReductionPerReduce = 0.05f;

    private static float scaleArmorHungerRateMultiply = 1.0f;
    private static float scaleArmorHungerRateMultiplyPerLevel = 0.05f;
    private static int scaleArmorHungerRateMultiplyReductionEveryLevel = 1;
    private static float scaleArmorHungerRateMultiplyReductionPerReduce = 0.05f;

    private static float scaleArmorRangedWeaponsAccuracyMultiply = 1.0f;
    private static float scaleArmorRangedWeaponsAccuracyMultiplyPerLevel = 0.05f;
    private static int scaleArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel = 1;
    private static float scaleArmorRangedWeaponsAccuracyMultiplyReductionPerReduce = 0.05f;

    private static float scaleArmorRangedWeaponsSpeedMultiply = 1.0f;
    private static float scaleArmorRangedWeaponsSpeedMultiplyPerLevel = 0.05f;
    private static int scaleArmorRangedWeaponsSpeedMultiplyReductionEveryLevel = 1;
    private static float scaleArmorRangedWeaponsSpeedMultiplyReductionPerReduce = 0.05f;

    private static float scaleArmorWalkSpeedMultiply = 1.0f;
    private static float scaleArmorWalkSpeedMultiplyPerLevel = 0.05f;
    private static int scaleArmorWalkSpeedMultiplyReductionEveryLevel = 1;
    private static float scaleArmorWalkSpeedMultiplyReductionPerReduce = 0.05f;

    public static int scaleArmorMaxLevel = 999;

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
        { //scaleArmorEXPMultiplyPerLevel
            if (scaleArmorLevelStats.TryGetValue("scaleArmorEXPMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: scaleArmorEXPMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: scaleArmorEXPMultiplyPerLevel is not double is {value.GetType()}");
                else scaleArmorEXPMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: scaleArmorEXPMultiplyPerLevel not set");
        }

        { //scaleArmorRelativeProtectionMultiply
            if (scaleArmorLevelStats.TryGetValue("scaleArmorRelativeProtectionMultiply", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: scaleArmorRelativeProtectionMultiply is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: scaleArmorRelativeProtectionMultiply is not double is {value.GetType()}");
                else scaleArmorRelativeProtectionMultiply = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: scaleArmorRelativeProtectionMultiply not set");
        }
        { //scaleArmorRelativeProtectionMultiplyPerLevel
            if (scaleArmorLevelStats.TryGetValue("scaleArmorRelativeProtectionMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: scaleArmorRelativeProtectionMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: scaleArmorRelativeProtectionMultiplyPerLevel is not double is {value.GetType()}");
                else scaleArmorRelativeProtectionMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: scaleArmorRelativeProtectionMultiplyPerLevel not set");
        }
        { //scaleArmorRelativeProtectionMultiplyReductionEveryLevel
            if (scaleArmorLevelStats.TryGetValue("scaleArmorRelativeProtectionMultiplyReductionEveryLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: scaleArmorRelativeProtectionMultiplyReductionEveryLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: scaleArmorRelativeProtectionMultiplyReductionEveryLevel is not int is {value.GetType()}");
                else scaleArmorRelativeProtectionMultiplyReductionEveryLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: scaleArmorRelativeProtectionMultiplyReductionEveryLevel not set");
        }
        { //scaleArmorRelativeProtectionMultiplyReductionPerReduce
            if (scaleArmorLevelStats.TryGetValue("scaleArmorRelativeProtectionMultiplyReductionPerReduce", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: scaleArmorRelativeProtectionMultiplyReductionPerReduce is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: scaleArmorRelativeProtectionMultiplyReductionPerReduce is not double is {value.GetType()}");
                else scaleArmorRelativeProtectionMultiplyReductionPerReduce = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: scaleArmorRelativeProtectionMultiplyReductionPerReduce not set");
        }

        { //scaleArmorFlatDamageReductionMultiply
            if (scaleArmorLevelStats.TryGetValue("scaleArmorFlatDamageReductionMultiply", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: scaleArmorFlatDamageReductionMultiply is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: scaleArmorFlatDamageReductionMultiply is not double is {value.GetType()}");
                else scaleArmorFlatDamageReductionMultiply = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: scaleArmorFlatDamageReductionMultiply not set");
        }
        { //scaleArmorFlatDamageReductionMultiplyPerLevel
            if (scaleArmorLevelStats.TryGetValue("scaleArmorFlatDamageReductionMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: scaleArmorFlatDamageReductionMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: scaleArmorFlatDamageReductionMultiplyPerLevel is not double is {value.GetType()}");
                else scaleArmorFlatDamageReductionMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: scaleArmorFlatDamageReductionMultiplyPerLevel not set");
        }
        { //scaleArmorFlatDamageReductionMultiplyReductionEveryLevel
            if (scaleArmorLevelStats.TryGetValue("scaleArmorFlatDamageReductionMultiplyReductionEveryLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: scaleArmorFlatDamageReductionMultiplyReductionEveryLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: scaleArmorFlatDamageReductionMultiplyReductionEveryLevel is not int is {value.GetType()}");
                else scaleArmorFlatDamageReductionMultiplyReductionEveryLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: scaleArmorFlatDamageReductionMultiplyReductionEveryLevel not set");
        }
        { //scaleArmorFlatDamageReductionMultiplyReductionPerReduce
            if (scaleArmorLevelStats.TryGetValue("scaleArmorFlatDamageReductionMultiplyReductionPerReduce", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: scaleArmorFlatDamageReductionMultiplyReductionPerReduce is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: scaleArmorFlatDamageReductionMultiplyReductionPerReduce is not double is {value.GetType()}");
                else scaleArmorFlatDamageReductionMultiplyReductionPerReduce = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: scaleArmorFlatDamageReductionMultiplyReductionPerReduce not set");
        }

        { //scaleArmorHealingEffectivnessMultiply
            if (scaleArmorLevelStats.TryGetValue("scaleArmorHealingEffectivnessMultiply", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: scaleArmorHealingEffectivnessMultiply is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: scaleArmorHealingEffectivnessMultiply is not double is {value.GetType()}");
                else scaleArmorHealingEffectivnessMultiply = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: scaleArmorHealingEffectivnessMultiply not set");
        }
        { //scaleArmorHealingEffectivnessMultiplyPerLevel
            if (scaleArmorLevelStats.TryGetValue("scaleArmorHealingEffectivnessMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: scaleArmorHealingEffectivnessMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: scaleArmorHealingEffectivnessMultiplyPerLevel is not double is {value.GetType()}");
                else scaleArmorHealingEffectivnessMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: scaleArmorHealingEffectivnessMultiplyPerLevel not set");
        }
        { //scaleArmorHealingEffectivnessMultiplyReductionEveryLevel
            if (scaleArmorLevelStats.TryGetValue("scaleArmorHealingEffectivnessMultiplyReductionEveryLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: scaleArmorHealingEffectivnessMultiplyReductionEveryLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: scaleArmorHealingEffectivnessMultiplyReductionEveryLevel is not int is {value.GetType()}");
                else scaleArmorHealingEffectivnessMultiplyReductionEveryLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: scaleArmorHealingEffectivnessMultiplyReductionEveryLevel not set");
        }
        { //scaleArmorHealingEffectivnessMultiplyReductionPerReduce
            if (scaleArmorLevelStats.TryGetValue("scaleArmorHealingEffectivnessMultiplyReductionPerReduce", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: scaleArmorHealingEffectivnessMultiplyReductionPerReduce is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: scaleArmorHealingEffectivnessMultiplyReductionPerReduce is not double is {value.GetType()}");
                else scaleArmorHealingEffectivnessMultiplyReductionPerReduce = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: scaleArmorHealingEffectivnessMultiplyReductionPerReduce not set");
        }

        { //scaleArmorHungerRateMultiply
            if (scaleArmorLevelStats.TryGetValue("scaleArmorHungerRateMultiply", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: scaleArmorHungerRateMultiply is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: scaleArmorHungerRateMultiply is not double is {value.GetType()}");
                else scaleArmorHungerRateMultiply = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: scaleArmorHungerRateMultiply not set");
        }
        { //scaleArmorHungerRateMultiplyPerLevel
            if (scaleArmorLevelStats.TryGetValue("scaleArmorHungerRateMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: scaleArmorHungerRateMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: scaleArmorHungerRateMultiplyPerLevel is not double is {value.GetType()}");
                else scaleArmorHungerRateMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: scaleArmorHungerRateMultiplyPerLevel not set");
        }
        { //scaleArmorHungerRateMultiplyReductionEveryLevel
            if (scaleArmorLevelStats.TryGetValue("scaleArmorHungerRateMultiplyReductionEveryLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: scaleArmorHungerRateMultiplyReductionEveryLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: scaleArmorHungerRateMultiplyReductionEveryLevel is not int is {value.GetType()}");
                else scaleArmorHungerRateMultiplyReductionEveryLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: scaleArmorHungerRateMultiplyReductionEveryLevel not set");
        }
        { //scaleArmorHungerRateMultiplyReductionPerReduce
            if (scaleArmorLevelStats.TryGetValue("scaleArmorHungerRateMultiplyReductionPerReduce", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: scaleArmorHungerRateMultiplyReductionPerReduce is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: scaleArmorHungerRateMultiplyReductionPerReduce is not double is {value.GetType()}");
                else scaleArmorHungerRateMultiplyReductionPerReduce = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: scaleArmorHungerRateMultiplyReductionPerReduce not set");
        }

        { //scaleArmorRangedWeaponsAccuracyMultiply
            if (scaleArmorLevelStats.TryGetValue("scaleArmorRangedWeaponsAccuracyMultiply", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: scaleArmorRangedWeaponsAccuracyMultiply is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: scaleArmorRangedWeaponsAccuracyMultiply is not double is {value.GetType()}");
                else scaleArmorRangedWeaponsAccuracyMultiply = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: scaleArmorRangedWeaponsAccuracyMultiply not set");
        }
        { //scaleArmorRangedWeaponsAccuracyMultiplyPerLevel
            if (scaleArmorLevelStats.TryGetValue("scaleArmorRangedWeaponsAccuracyMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: scaleArmorRangedWeaponsAccuracyMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: scaleArmorRangedWeaponsAccuracyMultiplyPerLevel is not double is {value.GetType()}");
                else scaleArmorRangedWeaponsAccuracyMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: scaleArmorRangedWeaponsAccuracyMultiplyPerLevel not set");
        }
        { //scaleArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel
            if (scaleArmorLevelStats.TryGetValue("scaleArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: scaleArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: scaleArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel is not int is {value.GetType()}");
                else scaleArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: scaleArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel not set");
        }
        { //scaleArmorRangedWeaponsAccuracyMultiplyReductionPerReduce
            if (scaleArmorLevelStats.TryGetValue("scaleArmorRangedWeaponsAccuracyMultiplyReductionPerReduce", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: scaleArmorRangedWeaponsAccuracyMultiplyReductionPerReduce is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: scaleArmorRangedWeaponsAccuracyMultiplyReductionPerReduce is not double is {value.GetType()}");
                else scaleArmorRangedWeaponsAccuracyMultiplyReductionPerReduce = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: scaleArmorRangedWeaponsAccuracyMultiplyReductionPerReduce not set");
        }

        { //scaleArmorRangedWeaponsSpeedMultiply
            if (scaleArmorLevelStats.TryGetValue("scaleArmorRangedWeaponsSpeedMultiply", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: scaleArmorRangedWeaponsSpeedMultiply is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: scaleArmorRangedWeaponsSpeedMultiply is not double is {value.GetType()}");
                else scaleArmorRangedWeaponsSpeedMultiply = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: scaleArmorRangedWeaponsSpeedMultiply not set");
        }
        { //scaleArmorRangedWeaponsSpeedMultiplyPerLevel
            if (scaleArmorLevelStats.TryGetValue("scaleArmorRangedWeaponsSpeedMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: scaleArmorRangedWeaponsSpeedMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: scaleArmorRangedWeaponsSpeedMultiplyPerLevel is not double is {value.GetType()}");
                else scaleArmorRangedWeaponsSpeedMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: scaleArmorRangedWeaponsSpeedMultiplyPerLevel not set");
        }
        { //scaleArmorRangedWeaponsSpeedMultiplyReductionEveryLevel
            if (scaleArmorLevelStats.TryGetValue("scaleArmorRangedWeaponsSpeedMultiplyReductionEveryLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: scaleArmorRangedWeaponsSpeedMultiplyReductionEveryLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: scaleArmorRangedWeaponsSpeedMultiplyReductionEveryLevel is not int is {value.GetType()}");
                else scaleArmorRangedWeaponsSpeedMultiplyReductionEveryLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: scaleArmorRangedWeaponsSpeedMultiplyReductionEveryLevel not set");
        }
        { //scaleArmorRangedWeaponsSpeedMultiplyReductionPerReduce
            if (scaleArmorLevelStats.TryGetValue("scaleArmorRangedWeaponsSpeedMultiplyReductionPerReduce", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: scaleArmorRangedWeaponsSpeedMultiplyReductionPerReduce is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: scaleArmorRangedWeaponsSpeedMultiplyReductionPerReduce is not double is {value.GetType()}");
                else scaleArmorRangedWeaponsSpeedMultiplyReductionPerReduce = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: scaleArmorRangedWeaponsSpeedMultiplyReductionPerReduce not set");
        }

        { //scaleArmorWalkSpeedMultiply
            if (scaleArmorLevelStats.TryGetValue("scaleArmorWalkSpeedMultiply", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: scaleArmorWalkSpeedMultiply is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: scaleArmorWalkSpeedMultiply is not double is {value.GetType()}");
                else scaleArmorWalkSpeedMultiply = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: scaleArmorWalkSpeedMultiply not set");
        }
        { //scaleArmorWalkSpeedMultiplyPerLevel
            if (scaleArmorLevelStats.TryGetValue("scaleArmorWalkSpeedMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: scaleArmorWalkSpeedMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: scaleArmorWalkSpeedMultiplyPerLevel is not double is {value.GetType()}");
                else scaleArmorWalkSpeedMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: scaleArmorWalkSpeedMultiplyPerLevel not set");
        }
        { //scaleArmorWalkSpeedMultiplyReductionEveryLevel
            if (scaleArmorLevelStats.TryGetValue("scaleArmorWalkSpeedMultiplyReductionEveryLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: scaleArmorWalkSpeedMultiplyReductionEveryLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: scaleArmorWalkSpeedMultiplyReductionEveryLevel is not int is {value.GetType()}");
                else scaleArmorWalkSpeedMultiplyReductionEveryLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: scaleArmorWalkSpeedMultiplyReductionEveryLevel not set");
        }
        { //scaleArmorWalkSpeedMultiplyReductionPerReduce
            if (scaleArmorLevelStats.TryGetValue("scaleArmorWalkSpeedMultiplyReductionPerReduce", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: scaleArmorWalkSpeedMultiplyReductionPerReduce is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: scaleArmorWalkSpeedMultiplyReductionPerReduce is not double is {value.GetType()}");
                else scaleArmorWalkSpeedMultiplyReductionPerReduce = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: scaleArmorWalkSpeedMultiplyReductionPerReduce not set");
        }

        { //scaleArmorMaxLevel
            if (scaleArmorLevelStats.TryGetValue("scaleArmorMaxLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: scaleArmorMaxLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: scaleArmorMaxLevel is not int is {value.GetType()}");
                else scaleArmorMaxLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: scaleArmorMaxLevel not set");
        }

        // Get scale armor multiply exp
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

    public static float ScaleArmorRelativeProtectionMultiplyByLevel(int level)
    {
        int reduceEvery = scaleArmorRelativeProtectionMultiplyReductionEveryLevel;
        float baseMultiply = scaleArmorRelativeProtectionMultiply;
        float baseIncrement = scaleArmorRelativeProtectionMultiplyPerLevel;
        float reductionPerStep = scaleArmorRelativeProtectionMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double multiply = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        multiply += baseMultiply;

        return (float)multiply;
    }

    public static float ScaleArmorFlatDamageReductionMultiplyByLevel(int level)
    {
        int reduceEvery = scaleArmorFlatDamageReductionMultiplyReductionEveryLevel;
        float baseMultiply = scaleArmorFlatDamageReductionMultiply;
        float baseIncrement = scaleArmorFlatDamageReductionMultiplyPerLevel;
        float reductionPerStep = scaleArmorFlatDamageReductionMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double multiply = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        multiply += baseMultiply;

        return (float)multiply;
    }

    public static float ScaleArmorHealingEffectivnessMultiplyByLevel(int level)
    {
        int reduceEvery = scaleArmorHealingEffectivnessMultiplyReductionEveryLevel;
        float baseMultiply = scaleArmorHealingEffectivnessMultiply;
        float baseIncrement = scaleArmorHealingEffectivnessMultiplyPerLevel;
        float reductionPerStep = scaleArmorHealingEffectivnessMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double multiply = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        multiply += baseMultiply;

        return (float)multiply;
    }

    public static float ScaleArmorHungerRateMultiplyByLevel(int level)
    {
        int reduceEvery = scaleArmorHungerRateMultiplyReductionEveryLevel;
        float baseMultiply = scaleArmorHungerRateMultiply;
        float baseIncrement = scaleArmorHungerRateMultiplyPerLevel;
        float reductionPerStep = scaleArmorHungerRateMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double multiply = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        multiply += baseMultiply;

        return (float)multiply;
    }

    public static float ScaleArmorRangedWeaponsAccuracyMultiplyByLevel(int level)
    {
        int reduceEvery = scaleArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel;
        float baseMultiply = scaleArmorRangedWeaponsAccuracyMultiply;
        float baseIncrement = scaleArmorRangedWeaponsAccuracyMultiplyPerLevel;
        float reductionPerStep = scaleArmorRangedWeaponsAccuracyMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double multiply = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        multiply += baseMultiply;

        return (float)multiply;
    }

    public static float ScaleArmorRangedWeaponsSpeedMultiplyByLevel(int level)
    {
        int reduceEvery = scaleArmorRangedWeaponsSpeedMultiplyReductionEveryLevel;
        float baseMultiply = scaleArmorRangedWeaponsSpeedMultiply;
        float baseIncrement = scaleArmorRangedWeaponsSpeedMultiplyPerLevel;
        float reductionPerStep = scaleArmorRangedWeaponsSpeedMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double multiply = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        multiply += baseMultiply;

        return (float)multiply;
    }

    public static float ScaleArmorWalkSpeedMultiplyByLevel(int level)
    {
        int reduceEvery = scaleArmorWalkSpeedMultiplyReductionEveryLevel;
        float baseMultiply = scaleArmorWalkSpeedMultiply;
        float baseIncrement = scaleArmorWalkSpeedMultiplyPerLevel;
        float reductionPerStep = scaleArmorWalkSpeedMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double multiply = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        multiply += baseMultiply;

        return (float)multiply;
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