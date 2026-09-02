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

    private static Dictionary<string, object> LoadConfigurationByDirectoryAndName(ICoreAPI api, string directory, string name, Dictionary<string, object> defaultConfig)
    {
        string directoryPath = Path.Combine(api.DataBasePath, directory);
        string configPath = Path.Combine(api.DataBasePath, directory, $"{name}.json");
        Dictionary<string, object> loadedConfig;
        try
        {
            // Load server configurations
            string jsonConfig = File.ReadAllText(configPath);
            loadedConfig = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonConfig) ?? defaultConfig;

            // Backfill keys missing from the user's file (e.g. added by a mod update) with their default value
            bool missingKeyAdded = false;
            foreach (var entry in defaultConfig)
            {
                if (loadedConfig.ContainsKey(entry.Key)) continue;

                Debug.LogWarn($"WARNING: Configuration key '{entry.Key}' missing from {name}.json, adding it with its default value");
                loadedConfig[entry.Key] = entry.Value;
                missingKeyAdded = true;
            }

            if (missingKeyAdded)
            {
                try
                {
                    string mergedJson = JsonConvert.SerializeObject(loadedConfig, Formatting.Indented);
                    File.WriteAllText(configPath, mergedJson);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"ERROR: Cannot save updated configs to {configPath}, reason: {ex.Message}");
                }
            }
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
            loadedConfig = defaultConfig;

            Debug.Log($"Configurations loaded, saving configs in: {configPath}");
            try
            {
                // Saving default configurations
                string defaultJson = JsonConvert.SerializeObject(loadedConfig, Formatting.Indented);
                File.WriteAllText(configPath, defaultJson);
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
            loadedConfig = defaultConfig;

            Debug.Log($"Configurations loaded, saving configs in: {configPath}");
            try
            {
                // Saving default configurations
                string defaultJson = JsonConvert.SerializeObject(loadedConfig, Formatting.Indented);
                File.WriteAllText(configPath, defaultJson);
            }
            catch (Exception ex)
            {
                Debug.Log($"ERROR: Cannot save default files to {configPath}, reason: {ex.Message}");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"ERROR: Cannot read the server configurations: {ex.Message}");
            Debug.Log("Loading default values...");
            loadedConfig = defaultConfig;
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

        var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
            .Where(f => f.FieldType.IsPrimitive || f.FieldType == typeof(string) || f.FieldType == typeof(Dictionary<string, double>));
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
        var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
            .Where(f => f.FieldType.IsPrimitive || f.FieldType == typeof(string) || f.FieldType == typeof(Dictionary<string, double>));

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
    public static bool enableLevelQuenching = true;
    public static int minimumEXPEarned = 1;
    public static bool enableLevelUPUIDSecurity = false;
    public static bool enableLevelUpChatMessages = true;
    public static bool enableLevelUpExperienceServerLog = false;
    public static bool enableExtendedLog = false;

    private static Dictionary<string, bool> enabledLevels = [];
    public static IReadOnlyDictionary<string, bool> EnabledLevels => enabledLevels;

    private static Dictionary<string, object> BuildBaseDefaultConfig() => new()
    {
        ["enableHardcore"] = enableHardcore,
        ["hardcoreLosePercentage"] = hardcoreLosePercentage,
        ["hardcorePenaltyDelayInWorldSeconds"] = (long)hardcorePenaltyDelayInWorldSeconds,
        ["hardcoreIgnoreLevelMinimum"] = hardcoreIgnoreLevelMinimum,
        ["hardcoreMessageWhenDying"] = hardcoreMessageWhenDying,
        ["enableLevelHunter"] = enableLevelHunter,
        ["enableLevelBow"] = enableLevelBow,
        ["enableLevelSlingshot"] = enableLevelSlingshot,
        ["enableLevelKnife"] = enableLevelKnife,
        ["enableLevelSpear"] = enableLevelSpear,
        ["enableLevelHammer"] = enableLevelHammer,
        ["enableLevelAxe"] = enableLevelAxe,
        ["enableLevelPickaxe"] = enableLevelPickaxe,
        ["enableLevelShovel"] = enableLevelShovel,
        ["enableLevelSword"] = enableLevelSword,
        ["enableLevelShield"] = enableLevelShield,
        ["enableLevelHand"] = enableLevelHand,
        ["enableLevelFarming"] = enableLevelFarming,
        ["enableLevelCooking"] = enableLevelCooking,
        ["enableLevelPanning"] = enableLevelPanning,
        ["enableLevelVitality"] = enableLevelVitality,
        ["enableLevelMetabolism"] = enableLevelMetabolism,
        ["enableLevelLeatherArmor"] = enableLevelLeatherArmor,
        ["enableLevelChainArmor"] = enableLevelChainArmor,
        ["enableLevelBrigandineArmor"] = enableLevelBrigandineArmor,
        ["enableLevelLamellarArmor"] = enableLevelLamellarArmor,
        ["enableLevelPlateArmor"] = enableLevelPlateArmor,
        ["enableLevelScaleArmor"] = enableLevelScaleArmor,
        ["enableLevelSmithing"] = enableLevelSmithing,
        ["enableLevelQuenching"] = enableLevelQuenching,
        ["minimumEXPEarned"] = (long)minimumEXPEarned,
        ["enableLevelUPUIDSecurity"] = enableLevelUPUIDSecurity,
        ["enableLevelUpChatMessages"] = enableLevelUpChatMessages,
        ["enableLevelUpExperienceServerLog"] = enableLevelUpExperienceServerLog,
        ["enableExtendedLog"] = enableExtendedLog,
    };

    internal static void UpdateBaseConfigurations(ICoreAPI api)
    {
        Dictionary<string, object> baseConfigs = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config",
            "base",
            BuildBaseDefaultConfig());
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
        { //hardcoreIgnoreLevelMinimum
            if (baseConfigs.TryGetValue("hardcoreIgnoreLevelMinimum", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: hardcoreIgnoreLevelMinimum is null");
                else if (value is not bool) Debug.Log($"CONFIGURATION ERROR: hardcoreIgnoreLevelMinimum is not boolean is {value.GetType()}");
                else hardcoreIgnoreLevelMinimum = (bool)value;
            else Debug.LogError("CONFIGURATION ERROR: hardcoreIgnoreLevelMinimum not set");
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
        { //enableLevelQuenching
            if (baseConfigs.TryGetValue("enableLevelQuenching", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: enableLevelQuenching is null");
                else if (value is not bool) Debug.Log($"CONFIGURATION ERROR: enableLevelQuenching is not boolean is {value.GetType()}");
                else enableLevelQuenching = (bool)value;
            else Debug.LogError("CONFIGURATION ERROR: enableLevelQuenching not set");
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

    private static Dictionary<string, object> BuildHunterDefaultConfig() => new()
    {
        ["hunterEXPPerLevelBase"] = (long)hunterEXPPerLevelBase,
        ["hunterEXPMultiplyPerLevel"] = hunterEXPMultiplyPerLevel,
        ["hunterBaseDamage"] = (double)hunterBaseDamage,
        ["hunterIncrementDamagePerLevel"] = (double)hunterIncrementDamagePerLevel,
        ["hunterMaxLevel"] = (long)hunterMaxLevel,
    };

    private static Dictionary<string, object> BuildHunterEntityExpDefaultConfig() => new()
    {
        ["game:sheep-bighorn-male"] = (long)50,
        ["game:sheep-bighorn-female"] = (long)50,
        ["game:sheep-bighorn-lamb"] = (long)20,
        ["game:chicken-rooster"] = (long)10,
        ["game:chicken-hen"] = (long)10,
        ["game:chicken-baby"] = (long)10,
        ["game:wolf-male"] = (long)40,
        ["game:wolf-female"] = (long)40,
        ["game:wolf-pup"] = (long)10,
        ["game:hyena-male"] = (long)40,
        ["game:hyena-female"] = (long)40,
        ["game:hyena-pup"] = (long)10,
        ["game:fox-male-red"] = (long)20,
        ["game:fox-female-red"] = (long)20,
        ["game:fox-pup"] = (long)10,
        ["game:fox-pup-red"] = (long)20,
        ["game:fox-pup-arctic"] = (long)20,
        ["game:fox-male-arctic"] = (long)10,
        ["game:fox-female-arctic"] = (long)10,
        ["game:raccoon-male"] = (long)20,
        ["game:raccoon-female"] = (long)20,
        ["game:raccoon-pup"] = (long)10,
        ["game:hare-male-arctic"] = (long)30,
        ["game:hare-male-ashgrey"] = (long)30,
        ["game:hare-male-darkbrown"] = (long)30,
        ["game:hare-male-desert"] = (long)30,
        ["game:hare-male-gold"] = (long)40,
        ["game:hare-male-lightbrown"] = (long)40,
        ["game:hare-male-lightgrey"] = (long)40,
        ["game:hare-male-silver"] = (long)40,
        ["game:hare-male-smokegrey"] = (long)50,
        ["game:hare-female-arctic"] = (long)60,
        ["game:hare-female-ashgrey"] = (long)60,
        ["game:hare-female-gold"] = (long)70,
        ["game:hare-female-lightbrown"] = (long)40,
        ["game:hare-female-lightgrey"] = (long)40,
        ["game:hare-female-silver"] = (long)40,
        ["game:hare-female-smokegrey"] = (long)30,
        ["game:hare-baby"] = (long)20,
        ["game:drifter-normal"] = (long)40,
        ["game:drifter-deep"] = (long)50,
        ["game:drifter-tainted"] = (long)60,
        ["game:drifter-corrupt"] = (long)70,
        ["game:drifter-nightmare"] = (long)80,
        ["game:drifter-double-headed"] = (long)90,
        ["game:locust-bronze"] = (long)60,
        ["game:locust-corrupt"] = (long)60,
        ["game:bell-normal"] = (long)100,
        ["game:bear-female-black"] = (long)50,
        ["game:bear-female-brown"] = (long)50,
        ["game:bear-female-sun"] = (long)50,
        ["game:bear-female-panda"] = (long)50,
        ["game:bear-female-polar"] = (long)50,
        ["game:bear-male-black"] = (long)50,
        ["game:bear-male-brown"] = (long)50,
        ["game:bear-male-sun"] = (long)50,
        ["game:bear-male-panda"] = (long)50,
        ["game:bear-male-polar"] = (long)50,
        ["game:locust-bronze-hacked"] = (long)60,
        ["game:locust-corrupt-hacked"] = (long)60,
        ["game:gazelle-male"] = (long)50,
        ["game:gazelle-female"] = (long)50,
        ["game:gazelle-calf"] = (long)30,
        ["game:deer-moose-male-adult"] = (long)30,
        ["game:deer-moose-female-adult"] = (long)50,
        ["game:deer-moose-male-baby"] = (long)50,
        ["game:deer-moose-female-baby"] = (long)30,
        ["game:deer-whitetail-male-adult"] = (long)30,
        ["game:deer-whitetail-female-adult"] = (long)30,
        ["game:deer-whitetail-male-baby"] = (long)30,
        ["game:deer-whitetail-female-baby"] = (long)10,
        ["game:deer-redbrocket-male-adult"] = (long)10,
        ["game:deer-chital-female-baby"] = (long)60,
        ["game:deer-guemal-male-adult"] = (long)60,
        ["game:deer-guemal-female-adult"] = (long)20,
        ["game:deer-guemal-male-baby"] = (long)20,
        ["game:deer-guemal-female-baby"] = (long)60,
        ["game:deer-pampas-male-adult"] = (long)60,
        ["game:deer-pampas-female-adult"] = (long)70,
        ["game:deer-pampas-male-baby"] = (long)70,
        ["game:deer-pampas-female-baby"] = (long)40,
        ["game:deer-pudu-male-adult"] = (long)40,
        ["game:deer-pudu-female-adult"] = (long)10,
        ["game:deer-pudu-male-baby"] = (long)10,
        ["game:deer-pudu-female-baby"] = (long)60,
        ["game:deer-elk-male-adult"] = (long)60,
        ["game:deer-elk-female-adult"] = (long)20,
        ["game:deer-elk-male-baby"] = (long)20,
        ["game:deer-elk-female-baby"] = (long)50,
        ["game:deer-taruca-male-adult"] = (long)50,
        ["game:deer-taruca-female-adult"] = (long)20,
        ["game:deer-taruca-male-baby"] = (long)20,
        ["game:deer-taruca-female-baby"] = (long)60,
        ["game:deer-chital-male-adult"] = (long)60,
        ["game:deer-chital-female-adult"] = (long)20,
        ["game:deer-chital-male-baby"] = (long)20,
        ["game:deer-fallow-female-baby"] = (long)60,
        ["game:deer-fallow-male-adult"] = (long)60,
        ["game:deer-fallow-male-baby"] = (long)20,
        ["game:deer-fallow-female-adult"] = (long)20,
        ["game:goat-angora-male-adult"] = (long)70,
        ["game:goat-angora-female-adult"] = (long)70,
        ["game:goat-angora-male-baby"] = (long)30,
        ["game:goat-angora-female-baby"] = (long)30,
        ["game:goat-ibexalp-male-adult"] = (long)70,
        ["game:goat-ibexalp-female-adult"] = (long)70,
        ["game:goat-ibexalp-male-baby"] = (long)30,
        ["game:goat-ibexalp-female-baby"] = (long)30,
        ["game:goat-ibexnub-male-adult"] = (long)50,
        ["game:goat-ibexnub-female-adult"] = (long)50,
        ["game:goat-ibexnub-male-baby"] = (long)20,
        ["game:goat-ibexnub-female-baby"] = (long)20,
        ["game:goat-markhor-male-adult"] = (long)60,
        ["game:goat-markhor-female-adult"] = (long)60,
        ["game:goat-markhor-male-baby"] = (long)20,
        ["game:goat-markhor-female-baby"] = (long)20,
        ["game:goat-mountain-male-adult"] = (long)40,
        ["game:goat-mountain-female-adult"] = (long)40,
        ["game:goat-mountain-male-baby"] = (long)20,
        ["game:goat-mountain-female-baby"] = (long)20,
        ["game:goat-muskox-male-adult"] = (long)40,
        ["game:goat-muskox-female-adult"] = (long)40,
        ["game:goat-muskox-male-baby"] = (long)20,
        ["game:goat-muskox-female-baby"] = (long)20,
        ["game:goat-nubian-male-adult"] = (long)40,
        ["game:goat-nubian-female-adult"] = (long)40,
        ["game:goat-nubian-male-baby"] = (long)20,
        ["game:goat-sirohi-male-adult"] = (long)40,
        ["game:goat-sirohi-female-adult"] = (long)40,
        ["game:goat-sirohi-male-baby"] = (long)20,
        ["game:goat-sirohi-female-baby"] = (long)20,
        ["game:goat-takingold-male-adult"] = (long)40,
        ["game:goat-takingold-female-adult"] = (long)40,
        ["game:goat-takingold-male-baby"] = (long)20,
        ["game:goat-takingold-female-baby"] = (long)20,
        ["game:goat-turdag-male-adult"] = (long)40,
        ["game:goat-turdag-female-adult"] = (long)40,
        ["game:goat-turdag-male-baby"] = (long)20,
        ["game:goat-turdag-female-baby"] = (long)20,
        ["game:goat-valais-male-adult"] = (long)40,
        ["game:goat-valais-female-adult"] = (long)40,
        ["game:goat-valais-male-baby"] = (long)20,
        ["game:goat-valais-female-baby"] = (long)20,
        ["game:pig-eurasian-adult-male"] = (long)30,
        ["game:pig-eurasian-adult-female"] = (long)30,
        ["game:pig-eurasian-elder-male"] = (long)40,
        ["game:pig-eurasian-elder-female"] = (long)40,
        ["game:pig-redriver-adult-male"] = (long)35,
        ["game:pig-redriver-adult-female"] = (long)35,
        ["game:pig-warthog-adult-male"] = (long)40,
        ["game:pig-warthog-adult-female"] = (long)40,
        ["game:pig-eurasian-baby-male"] = (long)10,
        ["game:pig-eurasian-baby-female"] = (long)10,
        ["game:pig-redriver-baby-male"] = (long)10,
        ["game:pig-redriver-baby-female"] = (long)10,
        ["game:pig-warthog-baby-male"] = (long)10,
        ["game:pig-warthog-baby-female"] = (long)10,
        ["game:sheep-mouflon-male"] = (long)50,
        ["game:sheep-mouflon-female"] = (long)50,
        ["game:sheep-mouflon-lamb"] = (long)20,
        ["game:shiver-surface"] = (long)50,
        ["game:shiver-deep"] = (long)60,
        ["game:shiver-tainted"] = (long)70,
        ["game:shiver-corrupt"] = (long)80,
        ["game:shiver-nightmare"] = (long)90,
        ["game:shiver-stilt"] = (long)60,
        ["game:shiver-bellhead"] = (long)80,
        ["game:shiver-deepsplit"] = (long)90,
        ["game:bowtorn-surface"] = (long)50,
        ["game:bowtorn-deep"] = (long)60,
        ["game:bowtorn-tainted"] = (long)70,
        ["game:bowtorn-corrupt"] = (long)80,
        ["game:bowtorn-nightmare"] = (long)90,
        ["game:bowtorn-gearfoot"] = (long)80,
        ["game:erel-pristine"] = (long)200,
        ["game:erel-corrupted"] = (long)250,
        ["game:eidolon-immobilized"] = (long)300,
        ["game:bellmini-normal"] = (long)100,
        ["game:locust-corrupt-sawblade"] = (long)60,
        ["game:chicken-henpoult"] = (long)10,
        ["game:chicken-roosterpoult"] = (long)10,
        ["game:deer-marsh-male-adult"] = (long)30,
        ["game:deer-marsh-female-adult"] = (long)30,
        ["game:deer-marsh-male-baby"] = (long)10,
        ["game:deer-marsh-female-baby"] = (long)10,
        ["game:deer-caribou-male-adult"] = (long)30,
        ["game:deer-caribou-female-adult"] = (long)30,
        ["game:deer-caribou-male-baby"] = (long)10,
        ["game:deer-caribou-female-baby"] = (long)10,
        ["game:deer-water-male-adult"] = (long)30,
        ["game:deer-water-female-adult"] = (long)30,
        ["game:deer-water-male-baby"] = (long)10,
        ["game:deer-water-female-baby"] = (long)10,
        ["game:deer-redbrocket-female-adult"] = (long)10,
        ["game:deer-redbrocket-male-baby"] = (long)10,
        ["game:deer-redbrocket-female-baby"] = (long)10,
        ["game:fish-freshwater-alewife-shad-adult"] = (long)5,
        ["game:fish-freshwater-chub-river-adult"] = (long)5,
        ["game:fish-freshwater-crappie-black-adult"] = (long)5,
        ["game:fish-freshwater-crappie-white-adult"] = (long)5,
        ["game:fish-freshwater-perch-european-adult"] = (long)5,
        ["game:fish-freshwater-perch-yellow-adult"] = (long)5,
        ["game:fish-freshwater-piranha-black-adult"] = (long)5,
        ["game:fish-freshwater-piranha-red-adult"] = (long)5,
        ["game:fish-freshwater-trout-brown-adult"] = (long)5,
        ["game:fish-freshwater-trout-rainbow-adult"] = (long)5,
        ["game:fish-freshwater-bass-largemouth-adult"] = (long)10,
        ["game:fish-freshwater-bass-smallmouth-adult"] = (long)10,
        ["game:fish-freshwater-carp-common-adult"] = (long)10,
        ["game:fish-freshwater-carp-grass-adult"] = (long)10,
        ["game:fish-freshwater-catfish-blue-adult"] = (long)10,
        ["game:fish-freshwater-catfish-channel-adult"] = (long)10,
        ["game:fish-freshwater-pickerel-chain-adult"] = (long)10,
        ["game:fish-freshwater-salmon-coho-adult"] = (long)10,
        ["game:fish-freshwater-tilapia-nile-adult"] = (long)10,
        ["game:fish-freshwater-tilapia-red-adult"] = (long)10,
        ["game:fish-freshwater-walleye-common-adult"] = (long)10,
        ["game:fish-freshwater-pike-northern-adult"] = (long)15,
        ["game:fish-freshwater-arapaima-arapaima-adult"] = (long)20,
        ["game:fish-freshwater-arapaima-gigas-adult"] = (long)20,
        ["game:fish-freshwater-sheatfish-black-adult"] = (long)20,
        ["game:fish-freshwater-sheatfish-white-adult"] = (long)20,
        ["game:fish-saltwater-bream-sea-adult"] = (long)5,
        ["game:fish-saltwater-gurnard-cape-adult"] = (long)5,
        ["game:fish-saltwater-haddock-common-adult"] = (long)5,
        ["game:fish-saltwater-hake-silver-adult"] = (long)5,
        ["game:fish-saltwater-herring-atlantic-adult"] = (long)5,
        ["game:fish-saltwater-mackerel-atlantic-adult"] = (long)5,
        ["game:fish-saltwater-pollock-alaska-adult"] = (long)5,
        ["game:fish-saltwater-perch-pacific-adult"] = (long)5,
        ["game:fish-saltwater-barracuda-great-adult"] = (long)10,
        ["game:fish-saltwater-grouper-black-adult"] = (long)10,
        ["game:fish-saltwater-salmon-pink-adult"] = (long)10,
        ["game:fish-saltwater-snapper-red-adult"] = (long)10,
        ["game:fish-saltwater-tuna-skipjack-adult"] = (long)10,
        ["game:fish-saltwater-wolf-bering-adult"] = (long)10,
        ["game:fish-saltwater-amberjack-yellowtail-adult"] = (long)15,
        ["game:fish-saltwater-mahi-mahi-common-adult"] = (long)15,
        ["game:fish-saltwater-wreckfish-atlantic-adult"] = (long)15,
        ["game:fish-saltwater-coelacanth-common-adult"] = (long)20,
        ["game:fish-saltwater-sturgeon-atlantic-adult"] = (long)20,
        ["game:fish-reef-angel-bicolor-adult"] = (long)5,
        ["game:fish-reef-butterfly-copperband-adult"] = (long)5,
        ["game:fish-reef-butterfly-blackwedged-adult"] = (long)5,
        ["game:fish-reef-clown-black-adult"] = (long)5,
        ["game:fish-reef-clown-common-adult"] = (long)5,
        ["game:fish-reef-clown-yellowstripe-adult"] = (long)5,
        ["game:fish-reef-puffer-longspine-adult"] = (long)5,
        ["game:fish-reef-tang-banded-adult"] = (long)5,
        ["game:fish-reef-tang-powderblue-adult"] = (long)5,
        ["game:fish-reef-trigger-titan-adult"] = (long)5,
        ["game:fish-reef-wrasse-creole-adult"] = (long)5,
    };

    public static void PopulateHunterConfiguration(ICoreAPI api)
    {
        Dictionary<string, object> hunterLevelStats = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/levelstats",
            "hunter",
            BuildHunterDefaultConfig());
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
            "hunter",
            BuildHunterEntityExpDefaultConfig());
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
    private static float bowChanceToNotLoseArrowBaseIncreasePerLevel = 2.0f;
    private static int bowChanceToNotLoseArrowReduceIncreaseEveryLevel = 5;
    private static float bowChanceToNotLoseArrowReduceQuantityEveryLevel = 0.2f;
    private static float bowBaseRangedAccuracy = 0.0f;
    private static float bowIncrementRangedAccuracyPerLevel = 0.015f;
    private static float bowBaseRangedSpeed = 0.0f;
    private static float bowIncrementRangedSpeedPerLevel = 0.01f;
    private static float bowBaseMovePenaltyReduction = 0.0f;
    private static float bowIncrementMovePenaltyReductionPerLevel = 0.005f;
    public static int bowMaxLevel = 999;

    public static int ExpPerHitBow => bowEXPPerHit;

    private static Dictionary<string, object> BuildBowDefaultConfig() => new()
    {
        ["bowEXPPerHit"] = (long)bowEXPPerHit,
        ["bowEXPPerLevelBase"] = (long)bowEXPPerLevelBase,
        ["bowEXPMultiplyPerLevel"] = bowEXPMultiplyPerLevel,
        ["bowBaseDamage"] = (double)bowBaseDamage,
        ["bowIncrementDamagePerLevel"] = (double)bowIncrementDamagePerLevel,
        ["bowChanceToNotLoseArrowBaseIncreasePerLevel"] = (double)bowChanceToNotLoseArrowBaseIncreasePerLevel,
        ["bowChanceToNotLoseArrowReduceIncreaseEveryLevel"] = (long)bowChanceToNotLoseArrowReduceIncreaseEveryLevel,
        ["bowChanceToNotLoseArrowReduceQuantityEveryLevel"] = (double)bowChanceToNotLoseArrowReduceQuantityEveryLevel,
        ["bowBaseRangedAccuracy"] = (double)bowBaseRangedAccuracy,
        ["bowIncrementRangedAccuracyPerLevel"] = (double)bowIncrementRangedAccuracyPerLevel,
        ["bowBaseRangedSpeed"] = (double)bowBaseRangedSpeed,
        ["bowIncrementRangedSpeedPerLevel"] = (double)bowIncrementRangedSpeedPerLevel,
        ["bowBaseMovePenaltyReduction"] = (double)bowBaseMovePenaltyReduction,
        ["bowIncrementMovePenaltyReductionPerLevel"] = (double)bowIncrementMovePenaltyReductionPerLevel,
        ["bowMaxLevel"] = (long)bowMaxLevel,
    };

    private static Dictionary<string, object> BuildBowEntityExpDefaultConfig() => new()
    {
        ["game:sheep-bighorn-male"] = (long)50,
        ["game:sheep-bighorn-female"] = (long)50,
        ["game:sheep-bighorn-lamb"] = (long)20,
        ["game:chicken-rooster"] = (long)10,
        ["game:chicken-hen"] = (long)10,
        ["game:chicken-baby"] = (long)10,
        ["game:wolf-male"] = (long)40,
        ["game:wolf-female"] = (long)40,
        ["game:wolf-pup"] = (long)10,
        ["game:hyena-male"] = (long)40,
        ["game:hyena-female"] = (long)40,
        ["game:hyena-pup"] = (long)10,
        ["game:fox-male-red"] = (long)20,
        ["game:fox-female-red"] = (long)20,
        ["game:fox-pup"] = (long)10,
        ["game:fox-pup-red"] = (long)20,
        ["game:fox-pup-arctic"] = (long)20,
        ["game:fox-male-arctic"] = (long)10,
        ["game:fox-female-arctic"] = (long)10,
        ["game:raccoon-male"] = (long)20,
        ["game:raccoon-female"] = (long)20,
        ["game:raccoon-pup"] = (long)10,
        ["game:hare-male-arctic"] = (long)30,
        ["game:hare-male-ashgrey"] = (long)30,
        ["game:hare-male-darkbrown"] = (long)30,
        ["game:hare-male-desert"] = (long)30,
        ["game:hare-male-gold"] = (long)40,
        ["game:hare-male-lightbrown"] = (long)40,
        ["game:hare-male-lightgrey"] = (long)40,
        ["game:hare-male-silver"] = (long)40,
        ["game:hare-male-smokegrey"] = (long)50,
        ["game:hare-female-arctic"] = (long)60,
        ["game:hare-female-ashgrey"] = (long)60,
        ["game:hare-female-gold"] = (long)70,
        ["game:hare-female-lightbrown"] = (long)40,
        ["game:hare-female-lightgrey"] = (long)40,
        ["game:hare-female-silver"] = (long)40,
        ["game:hare-female-smokegrey"] = (long)30,
        ["game:hare-baby"] = (long)20,
        ["game:drifter-normal"] = (long)40,
        ["game:drifter-deep"] = (long)50,
        ["game:drifter-tainted"] = (long)60,
        ["game:drifter-corrupt"] = (long)70,
        ["game:drifter-nightmare"] = (long)80,
        ["game:drifter-double-headed"] = (long)90,
        ["game:locust-bronze"] = (long)60,
        ["game:locust-corrupt"] = (long)60,
        ["game:bell-normal"] = (long)100,
        ["game:bear-female-black"] = (long)50,
        ["game:bear-female-brown"] = (long)50,
        ["game:bear-female-sun"] = (long)50,
        ["game:bear-female-panda"] = (long)50,
        ["game:bear-female-polar"] = (long)50,
        ["game:bear-male-black"] = (long)50,
        ["game:bear-male-brown"] = (long)50,
        ["game:bear-male-sun"] = (long)50,
        ["game:bear-male-panda"] = (long)50,
        ["game:bear-male-polar"] = (long)50,
        ["game:locust-bronze-hacked"] = (long)60,
        ["game:locust-corrupt-hacked"] = (long)60,
        ["game:gazelle-male"] = (long)50,
        ["game:gazelle-female"] = (long)50,
        ["game:gazelle-calf"] = (long)30,
        ["game:deer-moose-male-adult"] = (long)30,
        ["game:deer-moose-female-adult"] = (long)50,
        ["game:deer-moose-male-baby"] = (long)50,
        ["game:deer-moose-female-baby"] = (long)30,
        ["game:deer-whitetail-male-adult"] = (long)30,
        ["game:deer-whitetail-female-adult"] = (long)30,
        ["game:deer-whitetail-male-baby"] = (long)30,
        ["game:deer-whitetail-female-baby"] = (long)10,
        ["game:deer-redbrocket-male-adult"] = (long)10,
        ["game:deer-chital-female-baby"] = (long)60,
        ["game:deer-guemal-male-adult"] = (long)60,
        ["game:deer-guemal-female-adult"] = (long)20,
        ["game:deer-guemal-male-baby"] = (long)20,
        ["game:deer-guemal-female-baby"] = (long)60,
        ["game:deer-pampas-male-adult"] = (long)60,
        ["game:deer-pampas-female-adult"] = (long)70,
        ["game:deer-pampas-male-baby"] = (long)70,
        ["game:deer-pampas-female-baby"] = (long)40,
        ["game:deer-pudu-male-adult"] = (long)40,
        ["game:deer-pudu-female-adult"] = (long)10,
        ["game:deer-pudu-male-baby"] = (long)10,
        ["game:deer-pudu-female-baby"] = (long)60,
        ["game:deer-elk-male-adult"] = (long)60,
        ["game:deer-elk-female-adult"] = (long)20,
        ["game:deer-elk-male-baby"] = (long)20,
        ["game:deer-elk-female-baby"] = (long)50,
        ["game:deer-taruca-male-adult"] = (long)50,
        ["game:deer-taruca-female-adult"] = (long)20,
        ["game:deer-taruca-male-baby"] = (long)20,
        ["game:deer-taruca-female-baby"] = (long)60,
        ["game:deer-chital-male-adult"] = (long)60,
        ["game:deer-chital-female-adult"] = (long)20,
        ["game:deer-chital-male-baby"] = (long)20,
        ["game:deer-fallow-female-baby"] = (long)60,
        ["game:deer-fallow-male-adult"] = (long)60,
        ["game:deer-fallow-male-baby"] = (long)20,
        ["game:deer-fallow-female-adult"] = (long)20,
        ["game:goat-angora-male-adult"] = (long)70,
        ["game:goat-angora-female-adult"] = (long)70,
        ["game:goat-angora-male-baby"] = (long)30,
        ["game:goat-angora-female-baby"] = (long)30,
        ["game:goat-ibexalp-male-adult"] = (long)70,
        ["game:goat-ibexalp-female-adult"] = (long)70,
        ["game:goat-ibexalp-male-baby"] = (long)30,
        ["game:goat-ibexalp-female-baby"] = (long)30,
        ["game:goat-ibexnub-male-adult"] = (long)50,
        ["game:goat-ibexnub-female-adult"] = (long)50,
        ["game:goat-ibexnub-male-baby"] = (long)20,
        ["game:goat-ibexnub-female-baby"] = (long)20,
        ["game:goat-markhor-male-adult"] = (long)60,
        ["game:goat-markhor-female-adult"] = (long)60,
        ["game:goat-markhor-male-baby"] = (long)20,
        ["game:goat-markhor-female-baby"] = (long)20,
        ["game:goat-mountain-male-adult"] = (long)40,
        ["game:goat-mountain-female-adult"] = (long)40,
        ["game:goat-mountain-male-baby"] = (long)20,
        ["game:goat-mountain-female-baby"] = (long)20,
        ["game:goat-muskox-male-adult"] = (long)40,
        ["game:goat-muskox-female-adult"] = (long)40,
        ["game:goat-muskox-male-baby"] = (long)20,
        ["game:goat-muskox-female-baby"] = (long)20,
        ["game:goat-nubian-male-adult"] = (long)40,
        ["game:goat-nubian-female-adult"] = (long)40,
        ["game:goat-nubian-male-baby"] = (long)20,
        ["game:goat-sirohi-male-adult"] = (long)40,
        ["game:goat-sirohi-female-adult"] = (long)40,
        ["game:goat-sirohi-male-baby"] = (long)20,
        ["game:goat-sirohi-female-baby"] = (long)20,
        ["game:goat-takingold-male-adult"] = (long)40,
        ["game:goat-takingold-female-adult"] = (long)40,
        ["game:goat-takingold-male-baby"] = (long)20,
        ["game:goat-takingold-female-baby"] = (long)20,
        ["game:goat-turdag-male-adult"] = (long)40,
        ["game:goat-turdag-female-adult"] = (long)40,
        ["game:goat-turdag-male-baby"] = (long)20,
        ["game:goat-turdag-female-baby"] = (long)20,
        ["game:goat-valais-male-adult"] = (long)40,
        ["game:goat-valais-female-adult"] = (long)40,
        ["game:goat-valais-male-baby"] = (long)20,
        ["game:goat-valais-female-baby"] = (long)20,
        ["game:pig-eurasian-adult-male"] = (long)30,
        ["game:pig-eurasian-adult-female"] = (long)30,
        ["game:pig-eurasian-elder-male"] = (long)40,
        ["game:pig-eurasian-elder-female"] = (long)40,
        ["game:pig-redriver-adult-male"] = (long)35,
        ["game:pig-redriver-adult-female"] = (long)35,
        ["game:pig-warthog-adult-male"] = (long)40,
        ["game:pig-warthog-adult-female"] = (long)40,
        ["game:pig-eurasian-baby-male"] = (long)10,
        ["game:pig-eurasian-baby-female"] = (long)10,
        ["game:pig-redriver-baby-male"] = (long)10,
        ["game:pig-redriver-baby-female"] = (long)10,
        ["game:pig-warthog-baby-male"] = (long)10,
        ["game:pig-warthog-baby-female"] = (long)10,
        ["game:sheep-mouflon-male"] = (long)50,
        ["game:sheep-mouflon-female"] = (long)50,
        ["game:sheep-mouflon-lamb"] = (long)20,
        ["game:shiver-surface"] = (long)50,
        ["game:shiver-deep"] = (long)60,
        ["game:shiver-tainted"] = (long)70,
        ["game:shiver-corrupt"] = (long)80,
        ["game:shiver-nightmare"] = (long)90,
        ["game:shiver-stilt"] = (long)60,
        ["game:shiver-bellhead"] = (long)80,
        ["game:shiver-deepsplit"] = (long)90,
        ["game:bowtorn-surface"] = (long)50,
        ["game:bowtorn-deep"] = (long)60,
        ["game:bowtorn-tainted"] = (long)70,
        ["game:bowtorn-corrupt"] = (long)80,
        ["game:bowtorn-nightmare"] = (long)90,
        ["game:bowtorn-gearfoot"] = (long)80,
        ["game:erel-pristine"] = (long)200,
        ["game:erel-corrupted"] = (long)250,
        ["game:eidolon-immobilized"] = (long)300,
        ["game:bellmini-normal"] = (long)100,
        ["game:locust-corrupt-sawblade"] = (long)60,
        ["game:chicken-henpoult"] = (long)10,
        ["game:chicken-roosterpoult"] = (long)10,
        ["game:deer-marsh-male-adult"] = (long)30,
        ["game:deer-marsh-female-adult"] = (long)30,
        ["game:deer-marsh-male-baby"] = (long)10,
        ["game:deer-marsh-female-baby"] = (long)10,
        ["game:deer-caribou-male-adult"] = (long)30,
        ["game:deer-caribou-female-adult"] = (long)30,
        ["game:deer-caribou-male-baby"] = (long)10,
        ["game:deer-caribou-female-baby"] = (long)10,
        ["game:deer-water-male-adult"] = (long)30,
        ["game:deer-water-female-adult"] = (long)30,
        ["game:deer-water-male-baby"] = (long)10,
        ["game:deer-water-female-baby"] = (long)10,
        ["game:deer-redbrocket-female-adult"] = (long)10,
        ["game:deer-redbrocket-male-baby"] = (long)10,
        ["game:deer-redbrocket-female-baby"] = (long)10,
        ["game:fish-freshwater-alewife-shad-adult"] = (long)5,
        ["game:fish-freshwater-chub-river-adult"] = (long)5,
        ["game:fish-freshwater-crappie-black-adult"] = (long)5,
        ["game:fish-freshwater-crappie-white-adult"] = (long)5,
        ["game:fish-freshwater-perch-european-adult"] = (long)5,
        ["game:fish-freshwater-perch-yellow-adult"] = (long)5,
        ["game:fish-freshwater-piranha-black-adult"] = (long)5,
        ["game:fish-freshwater-piranha-red-adult"] = (long)5,
        ["game:fish-freshwater-trout-brown-adult"] = (long)5,
        ["game:fish-freshwater-trout-rainbow-adult"] = (long)5,
        ["game:fish-freshwater-bass-largemouth-adult"] = (long)10,
        ["game:fish-freshwater-bass-smallmouth-adult"] = (long)10,
        ["game:fish-freshwater-carp-common-adult"] = (long)10,
        ["game:fish-freshwater-carp-grass-adult"] = (long)10,
        ["game:fish-freshwater-catfish-blue-adult"] = (long)10,
        ["game:fish-freshwater-catfish-channel-adult"] = (long)10,
        ["game:fish-freshwater-pickerel-chain-adult"] = (long)10,
        ["game:fish-freshwater-salmon-coho-adult"] = (long)10,
        ["game:fish-freshwater-tilapia-nile-adult"] = (long)10,
        ["game:fish-freshwater-tilapia-red-adult"] = (long)10,
        ["game:fish-freshwater-walleye-common-adult"] = (long)10,
        ["game:fish-freshwater-pike-northern-adult"] = (long)15,
        ["game:fish-freshwater-arapaima-arapaima-adult"] = (long)20,
        ["game:fish-freshwater-arapaima-gigas-adult"] = (long)20,
        ["game:fish-freshwater-sheatfish-black-adult"] = (long)20,
        ["game:fish-freshwater-sheatfish-white-adult"] = (long)20,
        ["game:fish-saltwater-bream-sea-adult"] = (long)5,
        ["game:fish-saltwater-gurnard-cape-adult"] = (long)5,
        ["game:fish-saltwater-haddock-common-adult"] = (long)5,
        ["game:fish-saltwater-hake-silver-adult"] = (long)5,
        ["game:fish-saltwater-herring-atlantic-adult"] = (long)5,
        ["game:fish-saltwater-mackerel-atlantic-adult"] = (long)5,
        ["game:fish-saltwater-pollock-alaska-adult"] = (long)5,
        ["game:fish-saltwater-perch-pacific-adult"] = (long)5,
        ["game:fish-saltwater-barracuda-great-adult"] = (long)10,
        ["game:fish-saltwater-grouper-black-adult"] = (long)10,
        ["game:fish-saltwater-salmon-pink-adult"] = (long)10,
        ["game:fish-saltwater-snapper-red-adult"] = (long)10,
        ["game:fish-saltwater-tuna-skipjack-adult"] = (long)10,
        ["game:fish-saltwater-wolf-bering-adult"] = (long)10,
        ["game:fish-saltwater-amberjack-yellowtail-adult"] = (long)15,
        ["game:fish-saltwater-mahi-mahi-common-adult"] = (long)15,
        ["game:fish-saltwater-wreckfish-atlantic-adult"] = (long)15,
        ["game:fish-saltwater-coelacanth-common-adult"] = (long)20,
        ["game:fish-saltwater-sturgeon-atlantic-adult"] = (long)20,
        ["game:fish-reef-angel-bicolor-adult"] = (long)5,
        ["game:fish-reef-butterfly-copperband-adult"] = (long)5,
        ["game:fish-reef-butterfly-blackwedged-adult"] = (long)5,
        ["game:fish-reef-clown-black-adult"] = (long)5,
        ["game:fish-reef-clown-common-adult"] = (long)5,
        ["game:fish-reef-clown-yellowstripe-adult"] = (long)5,
        ["game:fish-reef-puffer-longspine-adult"] = (long)5,
        ["game:fish-reef-tang-banded-adult"] = (long)5,
        ["game:fish-reef-tang-powderblue-adult"] = (long)5,
        ["game:fish-reef-trigger-titan-adult"] = (long)5,
        ["game:fish-reef-wrasse-creole-adult"] = (long)5,
    };

    public static void PopulateBowConfiguration(ICoreAPI api)
    {
        Dictionary<string, object> bowLevelStats = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/levelstats",
            "bow",
            BuildBowDefaultConfig());

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
        { //bowBaseRangedAccuracy
            if (bowLevelStats.TryGetValue("bowBaseRangedAccuracy", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: bowBaseRangedAccuracy is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: bowBaseRangedAccuracy is not double is {value.GetType()}");
                else bowBaseRangedAccuracy = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: bowBaseRangedAccuracy not set");
        }
        { //bowIncrementRangedAccuracyPerLevel
            if (bowLevelStats.TryGetValue("bowIncrementRangedAccuracyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: bowIncrementRangedAccuracyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: bowIncrementRangedAccuracyPerLevel is not double is {value.GetType()}");
                else bowIncrementRangedAccuracyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: bowIncrementRangedAccuracyPerLevel not set");
        }
        { //bowBaseRangedSpeed
            if (bowLevelStats.TryGetValue("bowBaseRangedSpeed", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: bowBaseRangedSpeed is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: bowBaseRangedSpeed is not double is {value.GetType()}");
                else bowBaseRangedSpeed = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: bowBaseRangedSpeed not set");
        }
        { //bowIncrementRangedSpeedPerLevel
            if (bowLevelStats.TryGetValue("bowIncrementRangedSpeedPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: bowIncrementRangedSpeedPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: bowIncrementRangedSpeedPerLevel is not double is {value.GetType()}");
                else bowIncrementRangedSpeedPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: bowIncrementRangedSpeedPerLevel not set");
        }
        { //bowBaseMovePenaltyReduction
            if (bowLevelStats.TryGetValue("bowBaseMovePenaltyReduction", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: bowBaseMovePenaltyReduction is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: bowBaseMovePenaltyReduction is not double is {value.GetType()}");
                else bowBaseMovePenaltyReduction = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: bowBaseMovePenaltyReduction not set");
        }
        { //bowIncrementMovePenaltyReductionPerLevel
            if (bowLevelStats.TryGetValue("bowIncrementMovePenaltyReductionPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: bowIncrementMovePenaltyReductionPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: bowIncrementMovePenaltyReductionPerLevel is not double is {value.GetType()}");
                else bowIncrementMovePenaltyReductionPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: bowIncrementMovePenaltyReductionPerLevel not set");
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
            BuildBowEntityExpDefaultConfig());
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
        float baseIncrement = bowChanceToNotLoseArrowBaseIncreasePerLevel;
        float reductionPerStep = bowChanceToNotLoseArrowReduceQuantityEveryLevel;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double increment = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);

        if (enableExtendedLog)
            Debug.LogDebug($"Bow arrow drop increment: {increment}%");

        return (float)(increment / 100.0);
    }

    public static double BowGetRawChanceToNotLoseArrowByLevel(int level)
    {
        int reduceEvery = bowChanceToNotLoseArrowReduceIncreaseEveryLevel;
        float baseIncrement = bowChanceToNotLoseArrowBaseIncreasePerLevel;
        float reductionPerStep = bowChanceToNotLoseArrowReduceQuantityEveryLevel;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double increment = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);

        return increment;
    }

    public static float BowGetRangedAccuracyBonusByLevel(int level)
    {
        return bowBaseRangedAccuracy + bowIncrementRangedAccuracyPerLevel * level;
    }

    public static float BowGetRangedSpeedBonusByLevel(int level)
    {
        return bowBaseRangedSpeed + bowIncrementRangedSpeedPerLevel * level;
    }

    public static float BowGetMovePenaltyReductionByLevel(int level)
    {
        return bowBaseMovePenaltyReduction + bowIncrementMovePenaltyReductionPerLevel * level;
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

    private static Dictionary<string, object> BuildSlingshotDefaultConfig() => new()
    {
        ["slingshotEXPPerHit"] = (long)slingshotEXPPerHit,
        ["slingshotEXPPerLevelBase"] = (long)slingshotEXPPerLevelBase,
        ["slingshotEXPMultiplyPerLevel"] = slingshotEXPMultiplyPerLevel,
        ["slingshotBaseDamage"] = (double)slingshotBaseDamage,
        ["slingshotIncrementDamagePerLevel"] = (double)slingshotIncrementDamagePerLevel,
        ["slingshotBaseChanceToNotLoseRock"] = (double)slingshotBaseChanceToNotLoseRock,
        ["slingshotChanceToNotLoseRockBaseIncreasePerLevel"] = (double)slingshotChanceToNotLoseRockBaseIncreasePerLevel,
        ["slingshotChanceToNotLoseRockReduceIncreaseEveryLevel"] = (long)slingshotChanceToNotLoseRockReduceIncreaseEveryLevel,
        ["slingshotChanceToNotLoseRockReduceQuantityEveryLevel"] = (double)slingshotChanceToNotLoseRockReduceQuantityEveryLevel,
        ["slingshotBaseAimAccuracy"] = (double)slingshotBaseAimAccuracy,
        ["slingshotIncreaseAimAccuracyPerLevel"] = (double)slingshotIncreaseAimAccuracyPerLevel,
        ["slingshotMaxLevel"] = (long)slingshotMaxLevel,
    };

    private static Dictionary<string, object> BuildSlingshotEntityExpDefaultConfig() => new()
    {
        ["game:sheep-bighorn-male"] = (long)50,
        ["game:sheep-bighorn-female"] = (long)50,
        ["game:sheep-bighorn-lamb"] = (long)20,
        ["game:chicken-rooster"] = (long)10,
        ["game:chicken-hen"] = (long)10,
        ["game:chicken-baby"] = (long)10,
        ["game:wolf-male"] = (long)40,
        ["game:wolf-female"] = (long)40,
        ["game:wolf-pup"] = (long)10,
        ["game:hyena-male"] = (long)40,
        ["game:hyena-female"] = (long)40,
        ["game:hyena-pup"] = (long)10,
        ["game:fox-male-red"] = (long)20,
        ["game:fox-female-red"] = (long)20,
        ["game:fox-pup"] = (long)10,
        ["game:fox-pup-red"] = (long)20,
        ["game:fox-pup-arctic"] = (long)20,
        ["game:fox-male-arctic"] = (long)10,
        ["game:fox-female-arctic"] = (long)10,
        ["game:raccoon-male"] = (long)20,
        ["game:raccoon-female"] = (long)20,
        ["game:raccoon-pup"] = (long)10,
        ["game:hare-male-arctic"] = (long)30,
        ["game:hare-male-ashgrey"] = (long)30,
        ["game:hare-male-darkbrown"] = (long)30,
        ["game:hare-male-desert"] = (long)30,
        ["game:hare-male-gold"] = (long)40,
        ["game:hare-male-lightbrown"] = (long)40,
        ["game:hare-male-lightgrey"] = (long)40,
        ["game:hare-male-silver"] = (long)40,
        ["game:hare-male-smokegrey"] = (long)50,
        ["game:hare-female-arctic"] = (long)60,
        ["game:hare-female-ashgrey"] = (long)60,
        ["game:hare-female-gold"] = (long)70,
        ["game:hare-female-lightbrown"] = (long)40,
        ["game:hare-female-lightgrey"] = (long)40,
        ["game:hare-female-silver"] = (long)40,
        ["game:hare-female-smokegrey"] = (long)30,
        ["game:hare-baby"] = (long)20,
        ["game:drifter-normal"] = (long)40,
        ["game:drifter-deep"] = (long)50,
        ["game:drifter-tainted"] = (long)60,
        ["game:drifter-corrupt"] = (long)70,
        ["game:drifter-nightmare"] = (long)80,
        ["game:drifter-double-headed"] = (long)90,
        ["game:locust-bronze"] = (long)60,
        ["game:locust-corrupt"] = (long)60,
        ["game:bell-normal"] = (long)100,
        ["game:bear-female-black"] = (long)50,
        ["game:bear-female-brown"] = (long)50,
        ["game:bear-female-sun"] = (long)50,
        ["game:bear-female-panda"] = (long)50,
        ["game:bear-female-polar"] = (long)50,
        ["game:bear-male-black"] = (long)50,
        ["game:bear-male-brown"] = (long)50,
        ["game:bear-male-sun"] = (long)50,
        ["game:bear-male-panda"] = (long)50,
        ["game:bear-male-polar"] = (long)50,
        ["game:locust-bronze-hacked"] = (long)60,
        ["game:locust-corrupt-hacked"] = (long)60,
        ["game:gazelle-male"] = (long)50,
        ["game:gazelle-female"] = (long)50,
        ["game:gazelle-calf"] = (long)30,
        ["game:deer-moose-male-adult"] = (long)30,
        ["game:deer-moose-female-adult"] = (long)50,
        ["game:deer-moose-male-baby"] = (long)50,
        ["game:deer-moose-female-baby"] = (long)30,
        ["game:deer-whitetail-male-adult"] = (long)30,
        ["game:deer-whitetail-female-adult"] = (long)30,
        ["game:deer-whitetail-male-baby"] = (long)30,
        ["game:deer-whitetail-female-baby"] = (long)10,
        ["game:deer-redbrocket-male-adult"] = (long)10,
        ["game:deer-chital-female-baby"] = (long)60,
        ["game:deer-guemal-male-adult"] = (long)60,
        ["game:deer-guemal-female-adult"] = (long)20,
        ["game:deer-guemal-male-baby"] = (long)20,
        ["game:deer-guemal-female-baby"] = (long)60,
        ["game:deer-pampas-male-adult"] = (long)60,
        ["game:deer-pampas-female-adult"] = (long)70,
        ["game:deer-pampas-male-baby"] = (long)70,
        ["game:deer-pampas-female-baby"] = (long)40,
        ["game:deer-pudu-male-adult"] = (long)40,
        ["game:deer-pudu-female-adult"] = (long)10,
        ["game:deer-pudu-male-baby"] = (long)10,
        ["game:deer-pudu-female-baby"] = (long)60,
        ["game:deer-elk-male-adult"] = (long)60,
        ["game:deer-elk-female-adult"] = (long)20,
        ["game:deer-elk-male-baby"] = (long)20,
        ["game:deer-elk-female-baby"] = (long)50,
        ["game:deer-taruca-male-adult"] = (long)50,
        ["game:deer-taruca-female-adult"] = (long)20,
        ["game:deer-taruca-male-baby"] = (long)20,
        ["game:deer-taruca-female-baby"] = (long)60,
        ["game:deer-chital-male-adult"] = (long)60,
        ["game:deer-chital-female-adult"] = (long)20,
        ["game:deer-chital-male-baby"] = (long)20,
        ["game:deer-fallow-female-baby"] = (long)60,
        ["game:deer-fallow-male-adult"] = (long)60,
        ["game:deer-fallow-male-baby"] = (long)20,
        ["game:deer-fallow-female-adult"] = (long)20,
        ["game:goat-angora-male-adult"] = (long)70,
        ["game:goat-angora-female-adult"] = (long)70,
        ["game:goat-angora-male-baby"] = (long)30,
        ["game:goat-angora-female-baby"] = (long)30,
        ["game:goat-ibexalp-male-adult"] = (long)70,
        ["game:goat-ibexalp-female-adult"] = (long)70,
        ["game:goat-ibexalp-male-baby"] = (long)30,
        ["game:goat-ibexalp-female-baby"] = (long)30,
        ["game:goat-ibexnub-male-adult"] = (long)50,
        ["game:goat-ibexnub-female-adult"] = (long)50,
        ["game:goat-ibexnub-male-baby"] = (long)20,
        ["game:goat-ibexnub-female-baby"] = (long)20,
        ["game:goat-markhor-male-adult"] = (long)60,
        ["game:goat-markhor-female-adult"] = (long)60,
        ["game:goat-markhor-male-baby"] = (long)20,
        ["game:goat-markhor-female-baby"] = (long)20,
        ["game:goat-mountain-male-adult"] = (long)40,
        ["game:goat-mountain-female-adult"] = (long)40,
        ["game:goat-mountain-male-baby"] = (long)20,
        ["game:goat-mountain-female-baby"] = (long)20,
        ["game:goat-muskox-male-adult"] = (long)40,
        ["game:goat-muskox-female-adult"] = (long)40,
        ["game:goat-muskox-male-baby"] = (long)20,
        ["game:goat-muskox-female-baby"] = (long)20,
        ["game:goat-nubian-male-adult"] = (long)40,
        ["game:goat-nubian-female-adult"] = (long)40,
        ["game:goat-nubian-male-baby"] = (long)20,
        ["game:goat-sirohi-male-adult"] = (long)40,
        ["game:goat-sirohi-female-adult"] = (long)40,
        ["game:goat-sirohi-male-baby"] = (long)20,
        ["game:goat-sirohi-female-baby"] = (long)20,
        ["game:goat-takingold-male-adult"] = (long)40,
        ["game:goat-takingold-female-adult"] = (long)40,
        ["game:goat-takingold-male-baby"] = (long)20,
        ["game:goat-takingold-female-baby"] = (long)20,
        ["game:goat-turdag-male-adult"] = (long)40,
        ["game:goat-turdag-female-adult"] = (long)40,
        ["game:goat-turdag-male-baby"] = (long)20,
        ["game:goat-turdag-female-baby"] = (long)20,
        ["game:goat-valais-male-adult"] = (long)40,
        ["game:goat-valais-female-adult"] = (long)40,
        ["game:goat-valais-male-baby"] = (long)20,
        ["game:goat-valais-female-baby"] = (long)20,
        ["game:pig-eurasian-adult-male"] = (long)30,
        ["game:pig-eurasian-adult-female"] = (long)30,
        ["game:pig-eurasian-elder-male"] = (long)40,
        ["game:pig-eurasian-elder-female"] = (long)40,
        ["game:pig-redriver-adult-male"] = (long)35,
        ["game:pig-redriver-adult-female"] = (long)35,
        ["game:pig-warthog-adult-male"] = (long)40,
        ["game:pig-warthog-adult-female"] = (long)40,
        ["game:pig-eurasian-baby-male"] = (long)10,
        ["game:pig-eurasian-baby-female"] = (long)10,
        ["game:pig-redriver-baby-male"] = (long)10,
        ["game:pig-redriver-baby-female"] = (long)10,
        ["game:pig-warthog-baby-male"] = (long)10,
        ["game:pig-warthog-baby-female"] = (long)10,
        ["game:sheep-mouflon-male"] = (long)50,
        ["game:sheep-mouflon-female"] = (long)50,
        ["game:sheep-mouflon-lamb"] = (long)20,
        ["game:shiver-surface"] = (long)50,
        ["game:shiver-deep"] = (long)60,
        ["game:shiver-tainted"] = (long)70,
        ["game:shiver-corrupt"] = (long)80,
        ["game:shiver-nightmare"] = (long)90,
        ["game:shiver-stilt"] = (long)60,
        ["game:shiver-bellhead"] = (long)80,
        ["game:shiver-deepsplit"] = (long)90,
        ["game:bowtorn-surface"] = (long)50,
        ["game:bowtorn-deep"] = (long)60,
        ["game:bowtorn-tainted"] = (long)70,
        ["game:bowtorn-corrupt"] = (long)80,
        ["game:bowtorn-nightmare"] = (long)90,
        ["game:bowtorn-gearfoot"] = (long)80,
        ["game:erel-pristine"] = (long)200,
        ["game:erel-corrupted"] = (long)250,
        ["game:eidolon-immobilized"] = (long)300,
        ["game:bellmini-normal"] = (long)100,
        ["game:locust-corrupt-sawblade"] = (long)60,
        ["game:chicken-henpoult"] = (long)10,
        ["game:chicken-roosterpoult"] = (long)10,
        ["game:deer-marsh-male-adult"] = (long)30,
        ["game:deer-marsh-female-adult"] = (long)30,
        ["game:deer-marsh-male-baby"] = (long)10,
        ["game:deer-marsh-female-baby"] = (long)10,
        ["game:deer-caribou-male-adult"] = (long)30,
        ["game:deer-caribou-female-adult"] = (long)30,
        ["game:deer-caribou-male-baby"] = (long)10,
        ["game:deer-caribou-female-baby"] = (long)10,
        ["game:deer-water-male-adult"] = (long)30,
        ["game:deer-water-female-adult"] = (long)30,
        ["game:deer-water-male-baby"] = (long)10,
        ["game:deer-water-female-baby"] = (long)10,
        ["game:deer-redbrocket-female-adult"] = (long)10,
        ["game:deer-redbrocket-male-baby"] = (long)10,
        ["game:deer-redbrocket-female-baby"] = (long)10,
        ["game:fish-freshwater-alewife-shad-adult"] = (long)5,
        ["game:fish-freshwater-chub-river-adult"] = (long)5,
        ["game:fish-freshwater-crappie-black-adult"] = (long)5,
        ["game:fish-freshwater-crappie-white-adult"] = (long)5,
        ["game:fish-freshwater-perch-european-adult"] = (long)5,
        ["game:fish-freshwater-perch-yellow-adult"] = (long)5,
        ["game:fish-freshwater-piranha-black-adult"] = (long)5,
        ["game:fish-freshwater-piranha-red-adult"] = (long)5,
        ["game:fish-freshwater-trout-brown-adult"] = (long)5,
        ["game:fish-freshwater-trout-rainbow-adult"] = (long)5,
        ["game:fish-freshwater-bass-largemouth-adult"] = (long)10,
        ["game:fish-freshwater-bass-smallmouth-adult"] = (long)10,
        ["game:fish-freshwater-carp-common-adult"] = (long)10,
        ["game:fish-freshwater-carp-grass-adult"] = (long)10,
        ["game:fish-freshwater-catfish-blue-adult"] = (long)10,
        ["game:fish-freshwater-catfish-channel-adult"] = (long)10,
        ["game:fish-freshwater-pickerel-chain-adult"] = (long)10,
        ["game:fish-freshwater-salmon-coho-adult"] = (long)10,
        ["game:fish-freshwater-tilapia-nile-adult"] = (long)10,
        ["game:fish-freshwater-tilapia-red-adult"] = (long)10,
        ["game:fish-freshwater-walleye-common-adult"] = (long)10,
        ["game:fish-freshwater-pike-northern-adult"] = (long)15,
        ["game:fish-freshwater-arapaima-arapaima-adult"] = (long)20,
        ["game:fish-freshwater-arapaima-gigas-adult"] = (long)20,
        ["game:fish-freshwater-sheatfish-black-adult"] = (long)20,
        ["game:fish-freshwater-sheatfish-white-adult"] = (long)20,
        ["game:fish-saltwater-bream-sea-adult"] = (long)5,
        ["game:fish-saltwater-gurnard-cape-adult"] = (long)5,
        ["game:fish-saltwater-haddock-common-adult"] = (long)5,
        ["game:fish-saltwater-hake-silver-adult"] = (long)5,
        ["game:fish-saltwater-herring-atlantic-adult"] = (long)5,
        ["game:fish-saltwater-mackerel-atlantic-adult"] = (long)5,
        ["game:fish-saltwater-pollock-alaska-adult"] = (long)5,
        ["game:fish-saltwater-perch-pacific-adult"] = (long)5,
        ["game:fish-saltwater-barracuda-great-adult"] = (long)10,
        ["game:fish-saltwater-grouper-black-adult"] = (long)10,
        ["game:fish-saltwater-salmon-pink-adult"] = (long)10,
        ["game:fish-saltwater-snapper-red-adult"] = (long)10,
        ["game:fish-saltwater-tuna-skipjack-adult"] = (long)10,
        ["game:fish-saltwater-wolf-bering-adult"] = (long)10,
        ["game:fish-saltwater-amberjack-yellowtail-adult"] = (long)15,
        ["game:fish-saltwater-mahi-mahi-common-adult"] = (long)15,
        ["game:fish-saltwater-wreckfish-atlantic-adult"] = (long)15,
        ["game:fish-saltwater-coelacanth-common-adult"] = (long)20,
        ["game:fish-saltwater-sturgeon-atlantic-adult"] = (long)20,
        ["game:fish-reef-angel-bicolor-adult"] = (long)5,
        ["game:fish-reef-butterfly-copperband-adult"] = (long)5,
        ["game:fish-reef-butterfly-blackwedged-adult"] = (long)5,
        ["game:fish-reef-clown-black-adult"] = (long)5,
        ["game:fish-reef-clown-common-adult"] = (long)5,
        ["game:fish-reef-clown-yellowstripe-adult"] = (long)5,
        ["game:fish-reef-puffer-longspine-adult"] = (long)5,
        ["game:fish-reef-tang-banded-adult"] = (long)5,
        ["game:fish-reef-tang-powderblue-adult"] = (long)5,
        ["game:fish-reef-trigger-titan-adult"] = (long)5,
        ["game:fish-reef-wrasse-creole-adult"] = (long)5,
    };

    public static void PopulateSlingshotConfiguration(ICoreAPI api)
    {
        Dictionary<string, object> slingshotLevelStats = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/levelstats",
            "slingshot",
            BuildSlingshotDefaultConfig());

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
            BuildSlingshotEntityExpDefaultConfig());
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

    // This is a dispersionFactor value fed directly into EntityProjectileBase.SpawnProjectile - lower means
    // less random spread (more accurate). So leveling up must SUBTRACT from the base, not add to it, and the
    // result is floored so it can never reach zero/negative (which would collapse to zero spread or flip sign).
    public static float SlingshotGetAimAccuracyByLevel(int level)
    {
        return Math.Max(0.05f, slingshotBaseAimAccuracy - slingshotIncreaseAimAccuracyPerLevel * level);
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
    private static float knifeIncrementDamagePerLevel = 0.03f;
    private static float knifeBaseHarvestMultiply = 0.5f;
    private static float knifeIncrementHarvestMultiplyPerLevel = 0.1f;
    private static float knifeBaseMiningSpeed = 1.0f;
    private static float knifeIncrementMiningSpeedMultiplyPerLevel = 0.05f;
    public static int knifeMaxLevel = 999;

    public static int ExpPerHitKnife => knifeEXPPerHit;
    public static int ExpPerHarvestKnife => knifeEXPPerHarvest;
    public static int ExpPerBreakingKnife => knifeEXPPerBreaking;
    public static float BaseHarvestMultiplyKnife = knifeBaseHarvestMultiply;
    public static float BaseMinigSpeedKnife = knifeBaseMiningSpeed;

    private static Dictionary<string, object> BuildKnifeDefaultConfig() => new()
    {
        ["knifeEXPPerHit"] = (long)knifeEXPPerHit,
        ["knifeEXPPerHarvest"] = (long)knifeEXPPerHarvest,
        ["knifeEXPPerBreaking"] = (long)knifeEXPPerBreaking,
        ["knifeEXPPerLevelBase"] = (long)knifeEXPPerLevelBase,
        ["knifeEXPMultiplyPerLevel"] = knifeEXPMultiplyPerLevel,
        ["knifeBaseDamage"] = (double)knifeBaseDamage,
        ["knifeIncrementDamagePerLevel"] = (double)knifeIncrementDamagePerLevel,
        ["knifeBaseHarvestMultiply"] = (double)knifeBaseHarvestMultiply,
        ["knifeIncrementHarvestMultiplyPerLevel"] = (double)knifeIncrementHarvestMultiplyPerLevel,
        ["knifeBaseMiningSpeed"] = (double)knifeBaseMiningSpeed,
        ["knifeIncrementMiningSpeedMultiplyPerLevel"] = (double)knifeIncrementMiningSpeedMultiplyPerLevel,
        ["knifeMaxLevel"] = (long)knifeMaxLevel,
    };

    private static Dictionary<string, object> BuildKnifeEntityExpDefaultConfig() => new()
    {
        ["game:sheep-bighorn-male"] = (long)50,
        ["game:sheep-bighorn-female"] = (long)50,
        ["game:sheep-bighorn-lamb"] = (long)20,
        ["game:chicken-rooster"] = (long)10,
        ["game:chicken-hen"] = (long)10,
        ["game:chicken-baby"] = (long)10,
        ["game:wolf-male"] = (long)40,
        ["game:wolf-female"] = (long)40,
        ["game:wolf-pup"] = (long)10,
        ["game:hyena-male"] = (long)40,
        ["game:hyena-female"] = (long)40,
        ["game:hyena-pup"] = (long)10,
        ["game:fox-male-red"] = (long)20,
        ["game:fox-female-red"] = (long)20,
        ["game:fox-pup"] = (long)10,
        ["game:fox-pup-red"] = (long)20,
        ["game:fox-pup-arctic"] = (long)20,
        ["game:fox-male-arctic"] = (long)10,
        ["game:fox-female-arctic"] = (long)10,
        ["game:raccoon-male"] = (long)20,
        ["game:raccoon-female"] = (long)20,
        ["game:raccoon-pup"] = (long)10,
        ["game:hare-male-arctic"] = (long)30,
        ["game:hare-male-ashgrey"] = (long)30,
        ["game:hare-male-darkbrown"] = (long)30,
        ["game:hare-male-desert"] = (long)30,
        ["game:hare-male-gold"] = (long)40,
        ["game:hare-male-lightbrown"] = (long)40,
        ["game:hare-male-lightgrey"] = (long)40,
        ["game:hare-male-silver"] = (long)40,
        ["game:hare-male-smokegrey"] = (long)50,
        ["game:hare-female-arctic"] = (long)60,
        ["game:hare-female-ashgrey"] = (long)60,
        ["game:hare-female-gold"] = (long)70,
        ["game:hare-female-lightbrown"] = (long)40,
        ["game:hare-female-lightgrey"] = (long)40,
        ["game:hare-female-silver"] = (long)40,
        ["game:hare-female-smokegrey"] = (long)30,
        ["game:hare-baby"] = (long)20,
        ["game:drifter-normal"] = (long)40,
        ["game:drifter-deep"] = (long)50,
        ["game:drifter-tainted"] = (long)60,
        ["game:drifter-corrupt"] = (long)70,
        ["game:drifter-nightmare"] = (long)80,
        ["game:drifter-double-headed"] = (long)90,
        ["game:locust-bronze"] = (long)60,
        ["game:locust-corrupt"] = (long)60,
        ["game:bell-normal"] = (long)100,
        ["game:bear-female-black"] = (long)50,
        ["game:bear-female-brown"] = (long)50,
        ["game:bear-female-sun"] = (long)50,
        ["game:bear-female-panda"] = (long)50,
        ["game:bear-female-polar"] = (long)50,
        ["game:bear-male-black"] = (long)50,
        ["game:bear-male-brown"] = (long)50,
        ["game:bear-male-sun"] = (long)50,
        ["game:bear-male-panda"] = (long)50,
        ["game:bear-male-polar"] = (long)50,
        ["game:locust-bronze-hacked"] = (long)60,
        ["game:locust-corrupt-hacked"] = (long)60,
        ["game:gazelle-male"] = (long)50,
        ["game:gazelle-female"] = (long)50,
        ["game:gazelle-calf"] = (long)30,
        ["game:deer-moose-male-adult"] = (long)30,
        ["game:deer-moose-female-adult"] = (long)50,
        ["game:deer-moose-male-baby"] = (long)50,
        ["game:deer-moose-female-baby"] = (long)30,
        ["game:deer-whitetail-male-adult"] = (long)30,
        ["game:deer-whitetail-female-adult"] = (long)30,
        ["game:deer-whitetail-male-baby"] = (long)30,
        ["game:deer-whitetail-female-baby"] = (long)10,
        ["game:deer-redbrocket-male-adult"] = (long)10,
        ["game:deer-chital-female-baby"] = (long)60,
        ["game:deer-guemal-male-adult"] = (long)60,
        ["game:deer-guemal-female-adult"] = (long)20,
        ["game:deer-guemal-male-baby"] = (long)20,
        ["game:deer-guemal-female-baby"] = (long)60,
        ["game:deer-pampas-male-adult"] = (long)60,
        ["game:deer-pampas-female-adult"] = (long)70,
        ["game:deer-pampas-male-baby"] = (long)70,
        ["game:deer-pampas-female-baby"] = (long)40,
        ["game:deer-pudu-male-adult"] = (long)40,
        ["game:deer-pudu-female-adult"] = (long)10,
        ["game:deer-pudu-male-baby"] = (long)10,
        ["game:deer-pudu-female-baby"] = (long)60,
        ["game:deer-elk-male-adult"] = (long)60,
        ["game:deer-elk-female-adult"] = (long)20,
        ["game:deer-elk-male-baby"] = (long)20,
        ["game:deer-elk-female-baby"] = (long)50,
        ["game:deer-taruca-male-adult"] = (long)50,
        ["game:deer-taruca-female-adult"] = (long)20,
        ["game:deer-taruca-male-baby"] = (long)20,
        ["game:deer-taruca-female-baby"] = (long)60,
        ["game:deer-chital-male-adult"] = (long)60,
        ["game:deer-chital-female-adult"] = (long)20,
        ["game:deer-chital-male-baby"] = (long)20,
        ["game:deer-fallow-female-baby"] = (long)60,
        ["game:deer-fallow-male-adult"] = (long)60,
        ["game:deer-fallow-male-baby"] = (long)20,
        ["game:deer-fallow-female-adult"] = (long)20,
        ["game:goat-angora-male-adult"] = (long)70,
        ["game:goat-angora-female-adult"] = (long)70,
        ["game:goat-angora-male-baby"] = (long)30,
        ["game:goat-angora-female-baby"] = (long)30,
        ["game:goat-ibexalp-male-adult"] = (long)70,
        ["game:goat-ibexalp-female-adult"] = (long)70,
        ["game:goat-ibexalp-male-baby"] = (long)30,
        ["game:goat-ibexalp-female-baby"] = (long)30,
        ["game:goat-ibexnub-male-adult"] = (long)50,
        ["game:goat-ibexnub-female-adult"] = (long)50,
        ["game:goat-ibexnub-male-baby"] = (long)20,
        ["game:goat-ibexnub-female-baby"] = (long)20,
        ["game:goat-markhor-male-adult"] = (long)60,
        ["game:goat-markhor-female-adult"] = (long)60,
        ["game:goat-markhor-male-baby"] = (long)20,
        ["game:goat-markhor-female-baby"] = (long)20,
        ["game:goat-mountain-male-adult"] = (long)40,
        ["game:goat-mountain-female-adult"] = (long)40,
        ["game:goat-mountain-male-baby"] = (long)20,
        ["game:goat-mountain-female-baby"] = (long)20,
        ["game:goat-muskox-male-adult"] = (long)40,
        ["game:goat-muskox-female-adult"] = (long)40,
        ["game:goat-muskox-male-baby"] = (long)20,
        ["game:goat-muskox-female-baby"] = (long)20,
        ["game:goat-nubian-male-adult"] = (long)40,
        ["game:goat-nubian-female-adult"] = (long)40,
        ["game:goat-nubian-male-baby"] = (long)20,
        ["game:goat-sirohi-male-adult"] = (long)40,
        ["game:goat-sirohi-female-adult"] = (long)40,
        ["game:goat-sirohi-male-baby"] = (long)20,
        ["game:goat-sirohi-female-baby"] = (long)20,
        ["game:goat-takingold-male-adult"] = (long)40,
        ["game:goat-takingold-female-adult"] = (long)40,
        ["game:goat-takingold-male-baby"] = (long)20,
        ["game:goat-takingold-female-baby"] = (long)20,
        ["game:goat-turdag-male-adult"] = (long)40,
        ["game:goat-turdag-female-adult"] = (long)40,
        ["game:goat-turdag-male-baby"] = (long)20,
        ["game:goat-turdag-female-baby"] = (long)20,
        ["game:goat-valais-male-adult"] = (long)40,
        ["game:goat-valais-female-adult"] = (long)40,
        ["game:goat-valais-male-baby"] = (long)20,
        ["game:goat-valais-female-baby"] = (long)20,
        ["game:pig-eurasian-adult-male"] = (long)30,
        ["game:pig-eurasian-adult-female"] = (long)30,
        ["game:pig-eurasian-elder-male"] = (long)40,
        ["game:pig-eurasian-elder-female"] = (long)40,
        ["game:pig-redriver-adult-male"] = (long)35,
        ["game:pig-redriver-adult-female"] = (long)35,
        ["game:pig-warthog-adult-male"] = (long)40,
        ["game:pig-warthog-adult-female"] = (long)40,
        ["game:pig-eurasian-baby-male"] = (long)10,
        ["game:pig-eurasian-baby-female"] = (long)10,
        ["game:pig-redriver-baby-male"] = (long)10,
        ["game:pig-redriver-baby-female"] = (long)10,
        ["game:pig-warthog-baby-male"] = (long)10,
        ["game:pig-warthog-baby-female"] = (long)10,
        ["game:sheep-mouflon-male"] = (long)50,
        ["game:sheep-mouflon-female"] = (long)50,
        ["game:sheep-mouflon-lamb"] = (long)20,
        ["game:shiver-surface"] = (long)50,
        ["game:shiver-deep"] = (long)60,
        ["game:shiver-tainted"] = (long)70,
        ["game:shiver-corrupt"] = (long)80,
        ["game:shiver-nightmare"] = (long)90,
        ["game:shiver-stilt"] = (long)60,
        ["game:shiver-bellhead"] = (long)80,
        ["game:shiver-deepsplit"] = (long)90,
        ["game:bowtorn-surface"] = (long)50,
        ["game:bowtorn-deep"] = (long)60,
        ["game:bowtorn-tainted"] = (long)70,
        ["game:bowtorn-corrupt"] = (long)80,
        ["game:bowtorn-nightmare"] = (long)90,
        ["game:bowtorn-gearfoot"] = (long)80,
        ["game:erel-pristine"] = (long)200,
        ["game:erel-corrupted"] = (long)250,
        ["game:eidolon-immobilized"] = (long)300,
        ["game:bellmini-normal"] = (long)100,
        ["game:locust-corrupt-sawblade"] = (long)60,
        ["game:chicken-henpoult"] = (long)10,
        ["game:chicken-roosterpoult"] = (long)10,
        ["game:deer-marsh-male-adult"] = (long)30,
        ["game:deer-marsh-female-adult"] = (long)30,
        ["game:deer-marsh-male-baby"] = (long)10,
        ["game:deer-marsh-female-baby"] = (long)10,
        ["game:deer-caribou-male-adult"] = (long)30,
        ["game:deer-caribou-female-adult"] = (long)30,
        ["game:deer-caribou-male-baby"] = (long)10,
        ["game:deer-caribou-female-baby"] = (long)10,
        ["game:deer-water-male-adult"] = (long)30,
        ["game:deer-water-female-adult"] = (long)30,
        ["game:deer-water-male-baby"] = (long)10,
        ["game:deer-water-female-baby"] = (long)10,
        ["game:deer-redbrocket-female-adult"] = (long)10,
        ["game:deer-redbrocket-male-baby"] = (long)10,
        ["game:deer-redbrocket-female-baby"] = (long)10,
        ["game:fish-freshwater-alewife-shad-adult"] = (long)5,
        ["game:fish-freshwater-chub-river-adult"] = (long)5,
        ["game:fish-freshwater-crappie-black-adult"] = (long)5,
        ["game:fish-freshwater-crappie-white-adult"] = (long)5,
        ["game:fish-freshwater-perch-european-adult"] = (long)5,
        ["game:fish-freshwater-perch-yellow-adult"] = (long)5,
        ["game:fish-freshwater-piranha-black-adult"] = (long)5,
        ["game:fish-freshwater-piranha-red-adult"] = (long)5,
        ["game:fish-freshwater-trout-brown-adult"] = (long)5,
        ["game:fish-freshwater-trout-rainbow-adult"] = (long)5,
        ["game:fish-freshwater-bass-largemouth-adult"] = (long)10,
        ["game:fish-freshwater-bass-smallmouth-adult"] = (long)10,
        ["game:fish-freshwater-carp-common-adult"] = (long)10,
        ["game:fish-freshwater-carp-grass-adult"] = (long)10,
        ["game:fish-freshwater-catfish-blue-adult"] = (long)10,
        ["game:fish-freshwater-catfish-channel-adult"] = (long)10,
        ["game:fish-freshwater-pickerel-chain-adult"] = (long)10,
        ["game:fish-freshwater-salmon-coho-adult"] = (long)10,
        ["game:fish-freshwater-tilapia-nile-adult"] = (long)10,
        ["game:fish-freshwater-tilapia-red-adult"] = (long)10,
        ["game:fish-freshwater-walleye-common-adult"] = (long)10,
        ["game:fish-freshwater-pike-northern-adult"] = (long)15,
        ["game:fish-freshwater-arapaima-arapaima-adult"] = (long)20,
        ["game:fish-freshwater-arapaima-gigas-adult"] = (long)20,
        ["game:fish-freshwater-sheatfish-black-adult"] = (long)20,
        ["game:fish-freshwater-sheatfish-white-adult"] = (long)20,
        ["game:fish-saltwater-bream-sea-adult"] = (long)5,
        ["game:fish-saltwater-gurnard-cape-adult"] = (long)5,
        ["game:fish-saltwater-haddock-common-adult"] = (long)5,
        ["game:fish-saltwater-hake-silver-adult"] = (long)5,
        ["game:fish-saltwater-herring-atlantic-adult"] = (long)5,
        ["game:fish-saltwater-mackerel-atlantic-adult"] = (long)5,
        ["game:fish-saltwater-pollock-alaska-adult"] = (long)5,
        ["game:fish-saltwater-perch-pacific-adult"] = (long)5,
        ["game:fish-saltwater-barracuda-great-adult"] = (long)10,
        ["game:fish-saltwater-grouper-black-adult"] = (long)10,
        ["game:fish-saltwater-salmon-pink-adult"] = (long)10,
        ["game:fish-saltwater-snapper-red-adult"] = (long)10,
        ["game:fish-saltwater-tuna-skipjack-adult"] = (long)10,
        ["game:fish-saltwater-wolf-bering-adult"] = (long)10,
        ["game:fish-saltwater-amberjack-yellowtail-adult"] = (long)15,
        ["game:fish-saltwater-mahi-mahi-common-adult"] = (long)15,
        ["game:fish-saltwater-wreckfish-atlantic-adult"] = (long)15,
        ["game:fish-saltwater-coelacanth-common-adult"] = (long)20,
        ["game:fish-saltwater-sturgeon-atlantic-adult"] = (long)20,
        ["game:fish-reef-angel-bicolor-adult"] = (long)5,
        ["game:fish-reef-butterfly-copperband-adult"] = (long)5,
        ["game:fish-reef-butterfly-blackwedged-adult"] = (long)5,
        ["game:fish-reef-clown-black-adult"] = (long)5,
        ["game:fish-reef-clown-common-adult"] = (long)5,
        ["game:fish-reef-clown-yellowstripe-adult"] = (long)5,
        ["game:fish-reef-puffer-longspine-adult"] = (long)5,
        ["game:fish-reef-tang-banded-adult"] = (long)5,
        ["game:fish-reef-tang-powderblue-adult"] = (long)5,
        ["game:fish-reef-trigger-titan-adult"] = (long)5,
        ["game:fish-reef-wrasse-creole-adult"] = (long)5,
    };

    public static void PopulateKnifeConfiguration(ICoreAPI api)
    {
        Dictionary<string, object> knifeLevelStats = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/levelstats",
            "knife",
            BuildKnifeDefaultConfig());

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
            BuildKnifeEntityExpDefaultConfig());
        foreach (KeyValuePair<string, object> pair in tmpentityExpKnife)
        {
            if (pair.Value is long value) entityExpKnife.Add(pair.Key, (int)value);
            else Debug.Log($"CONFIGURATION ERROR: entityExpKnife {pair.Key} is not int");
        }

        BaseHarvestMultiplyKnife = knifeBaseHarvestMultiply;
        BaseMinigSpeedKnife = knifeBaseMiningSpeed;

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
    private static float axeIncrementDamagePerLevel = 0.05f;
    private static float axeBaseMiningSpeed = 1.0f;
    private static float axeIncrementMiningSpeedMultiplyPerLevel = 0.05f;
    public static int axeMaxLevel = 999;


    public static int ExpPerHitAxe => axeEXPPerHit;
    public static int ExpPerBreakingAxe => axeEXPPerBreaking;
    public static int ExpPerTreeBreakingAxe => axeEXPPerTreeBreaking;

    private static Dictionary<string, object> BuildAxeDefaultConfig() => new()
    {
        ["axeEXPPerHit"] = (long)axeEXPPerHit,
        ["axeEXPPerBreaking"] = (long)axeEXPPerBreaking,
        ["axeEXPPerTreeBreaking"] = (long)axeEXPPerTreeBreaking,
        ["axeEXPPerLevelBase"] = (long)axeEXPPerLevelBase,
        ["axeEXPMultiplyPerLevel"] = axeEXPMultiplyPerLevel,
        ["axeBaseDamage"] = (double)axeBaseDamage,
        ["axeIncrementDamagePerLevel"] = (double)axeIncrementDamagePerLevel,
        ["axeBaseMiningSpeed"] = (double)axeBaseMiningSpeed,
        ["axeIncrementMiningSpeedMultiplyPerLevel"] = (double)axeIncrementMiningSpeedMultiplyPerLevel,
        ["axeMaxLevel"] = (long)axeMaxLevel,
    };

    private static Dictionary<string, object> BuildAxeEntityExpDefaultConfig() => new()
    {
        ["game:sheep-bighorn-male"] = (long)50,
        ["game:sheep-bighorn-female"] = (long)50,
        ["game:sheep-bighorn-lamb"] = (long)20,
        ["game:chicken-rooster"] = (long)10,
        ["game:chicken-hen"] = (long)10,
        ["game:chicken-baby"] = (long)10,
        ["game:wolf-male"] = (long)40,
        ["game:wolf-female"] = (long)40,
        ["game:wolf-pup"] = (long)10,
        ["game:hyena-male"] = (long)40,
        ["game:hyena-female"] = (long)40,
        ["game:hyena-pup"] = (long)10,
        ["game:fox-male-red"] = (long)20,
        ["game:fox-female-red"] = (long)20,
        ["game:fox-pup"] = (long)10,
        ["game:fox-pup-red"] = (long)20,
        ["game:fox-pup-arctic"] = (long)20,
        ["game:fox-male-arctic"] = (long)10,
        ["game:fox-female-arctic"] = (long)10,
        ["game:raccoon-male"] = (long)20,
        ["game:raccoon-female"] = (long)20,
        ["game:raccoon-pup"] = (long)10,
        ["game:hare-male-arctic"] = (long)30,
        ["game:hare-male-ashgrey"] = (long)30,
        ["game:hare-male-darkbrown"] = (long)30,
        ["game:hare-male-desert"] = (long)30,
        ["game:hare-male-gold"] = (long)40,
        ["game:hare-male-lightbrown"] = (long)40,
        ["game:hare-male-lightgrey"] = (long)40,
        ["game:hare-male-silver"] = (long)40,
        ["game:hare-male-smokegrey"] = (long)50,
        ["game:hare-female-arctic"] = (long)60,
        ["game:hare-female-ashgrey"] = (long)60,
        ["game:hare-female-gold"] = (long)70,
        ["game:hare-female-lightbrown"] = (long)40,
        ["game:hare-female-lightgrey"] = (long)40,
        ["game:hare-female-silver"] = (long)40,
        ["game:hare-female-smokegrey"] = (long)30,
        ["game:hare-baby"] = (long)20,
        ["game:drifter-normal"] = (long)40,
        ["game:drifter-deep"] = (long)50,
        ["game:drifter-tainted"] = (long)60,
        ["game:drifter-corrupt"] = (long)70,
        ["game:drifter-nightmare"] = (long)80,
        ["game:drifter-double-headed"] = (long)90,
        ["game:locust-bronze"] = (long)60,
        ["game:locust-corrupt"] = (long)60,
        ["game:bell-normal"] = (long)100,
        ["game:bear-female-black"] = (long)50,
        ["game:bear-female-brown"] = (long)50,
        ["game:bear-female-sun"] = (long)50,
        ["game:bear-female-panda"] = (long)50,
        ["game:bear-female-polar"] = (long)50,
        ["game:bear-male-black"] = (long)50,
        ["game:bear-male-brown"] = (long)50,
        ["game:bear-male-sun"] = (long)50,
        ["game:bear-male-panda"] = (long)50,
        ["game:bear-male-polar"] = (long)50,
        ["game:locust-bronze-hacked"] = (long)60,
        ["game:locust-corrupt-hacked"] = (long)60,
        ["game:gazelle-male"] = (long)50,
        ["game:gazelle-female"] = (long)50,
        ["game:gazelle-calf"] = (long)30,
        ["game:deer-moose-male-adult"] = (long)30,
        ["game:deer-moose-female-adult"] = (long)50,
        ["game:deer-moose-male-baby"] = (long)50,
        ["game:deer-moose-female-baby"] = (long)30,
        ["game:deer-whitetail-male-adult"] = (long)30,
        ["game:deer-whitetail-female-adult"] = (long)30,
        ["game:deer-whitetail-male-baby"] = (long)30,
        ["game:deer-whitetail-female-baby"] = (long)10,
        ["game:deer-redbrocket-male-adult"] = (long)10,
        ["game:deer-chital-female-baby"] = (long)60,
        ["game:deer-guemal-male-adult"] = (long)60,
        ["game:deer-guemal-female-adult"] = (long)20,
        ["game:deer-guemal-male-baby"] = (long)20,
        ["game:deer-guemal-female-baby"] = (long)60,
        ["game:deer-pampas-male-adult"] = (long)60,
        ["game:deer-pampas-female-adult"] = (long)70,
        ["game:deer-pampas-male-baby"] = (long)70,
        ["game:deer-pampas-female-baby"] = (long)40,
        ["game:deer-pudu-male-adult"] = (long)40,
        ["game:deer-pudu-female-adult"] = (long)10,
        ["game:deer-pudu-male-baby"] = (long)10,
        ["game:deer-pudu-female-baby"] = (long)60,
        ["game:deer-elk-male-adult"] = (long)60,
        ["game:deer-elk-female-adult"] = (long)20,
        ["game:deer-elk-male-baby"] = (long)20,
        ["game:deer-elk-female-baby"] = (long)50,
        ["game:deer-taruca-male-adult"] = (long)50,
        ["game:deer-taruca-female-adult"] = (long)20,
        ["game:deer-taruca-male-baby"] = (long)20,
        ["game:deer-taruca-female-baby"] = (long)60,
        ["game:deer-chital-male-adult"] = (long)60,
        ["game:deer-chital-female-adult"] = (long)20,
        ["game:deer-chital-male-baby"] = (long)20,
        ["game:deer-fallow-female-baby"] = (long)60,
        ["game:deer-fallow-male-adult"] = (long)60,
        ["game:deer-fallow-male-baby"] = (long)20,
        ["game:deer-fallow-female-adult"] = (long)20,
        ["game:goat-angora-male-adult"] = (long)70,
        ["game:goat-angora-female-adult"] = (long)70,
        ["game:goat-angora-male-baby"] = (long)30,
        ["game:goat-angora-female-baby"] = (long)30,
        ["game:goat-ibexalp-male-adult"] = (long)70,
        ["game:goat-ibexalp-female-adult"] = (long)70,
        ["game:goat-ibexalp-male-baby"] = (long)30,
        ["game:goat-ibexalp-female-baby"] = (long)30,
        ["game:goat-ibexnub-male-adult"] = (long)50,
        ["game:goat-ibexnub-female-adult"] = (long)50,
        ["game:goat-ibexnub-male-baby"] = (long)20,
        ["game:goat-ibexnub-female-baby"] = (long)20,
        ["game:goat-markhor-male-adult"] = (long)60,
        ["game:goat-markhor-female-adult"] = (long)60,
        ["game:goat-markhor-male-baby"] = (long)20,
        ["game:goat-markhor-female-baby"] = (long)20,
        ["game:goat-mountain-male-adult"] = (long)40,
        ["game:goat-mountain-female-adult"] = (long)40,
        ["game:goat-mountain-male-baby"] = (long)20,
        ["game:goat-mountain-female-baby"] = (long)20,
        ["game:goat-muskox-male-adult"] = (long)40,
        ["game:goat-muskox-female-adult"] = (long)40,
        ["game:goat-muskox-male-baby"] = (long)20,
        ["game:goat-muskox-female-baby"] = (long)20,
        ["game:goat-nubian-male-adult"] = (long)40,
        ["game:goat-nubian-female-adult"] = (long)40,
        ["game:goat-nubian-male-baby"] = (long)20,
        ["game:goat-sirohi-male-adult"] = (long)40,
        ["game:goat-sirohi-female-adult"] = (long)40,
        ["game:goat-sirohi-male-baby"] = (long)20,
        ["game:goat-sirohi-female-baby"] = (long)20,
        ["game:goat-takingold-male-adult"] = (long)40,
        ["game:goat-takingold-female-adult"] = (long)40,
        ["game:goat-takingold-male-baby"] = (long)20,
        ["game:goat-takingold-female-baby"] = (long)20,
        ["game:goat-turdag-male-adult"] = (long)40,
        ["game:goat-turdag-female-adult"] = (long)40,
        ["game:goat-turdag-male-baby"] = (long)20,
        ["game:goat-turdag-female-baby"] = (long)20,
        ["game:goat-valais-male-adult"] = (long)40,
        ["game:goat-valais-female-adult"] = (long)40,
        ["game:goat-valais-male-baby"] = (long)20,
        ["game:goat-valais-female-baby"] = (long)20,
        ["game:pig-eurasian-adult-male"] = (long)30,
        ["game:pig-eurasian-adult-female"] = (long)30,
        ["game:pig-eurasian-elder-male"] = (long)40,
        ["game:pig-eurasian-elder-female"] = (long)40,
        ["game:pig-redriver-adult-male"] = (long)35,
        ["game:pig-redriver-adult-female"] = (long)35,
        ["game:pig-warthog-adult-male"] = (long)40,
        ["game:pig-warthog-adult-female"] = (long)40,
        ["game:pig-eurasian-baby-male"] = (long)10,
        ["game:pig-eurasian-baby-female"] = (long)10,
        ["game:pig-redriver-baby-male"] = (long)10,
        ["game:pig-redriver-baby-female"] = (long)10,
        ["game:pig-warthog-baby-male"] = (long)10,
        ["game:pig-warthog-baby-female"] = (long)10,
        ["game:sheep-mouflon-male"] = (long)50,
        ["game:sheep-mouflon-female"] = (long)50,
        ["game:sheep-mouflon-lamb"] = (long)20,
        ["game:shiver-surface"] = (long)50,
        ["game:shiver-deep"] = (long)60,
        ["game:shiver-tainted"] = (long)70,
        ["game:shiver-corrupt"] = (long)80,
        ["game:shiver-nightmare"] = (long)90,
        ["game:shiver-stilt"] = (long)60,
        ["game:shiver-bellhead"] = (long)80,
        ["game:shiver-deepsplit"] = (long)90,
        ["game:bowtorn-surface"] = (long)50,
        ["game:bowtorn-deep"] = (long)60,
        ["game:bowtorn-tainted"] = (long)70,
        ["game:bowtorn-corrupt"] = (long)80,
        ["game:bowtorn-nightmare"] = (long)90,
        ["game:bowtorn-gearfoot"] = (long)80,
        ["game:erel-pristine"] = (long)200,
        ["game:erel-corrupted"] = (long)250,
        ["game:eidolon-immobilized"] = (long)300,
        ["game:bellmini-normal"] = (long)100,
        ["game:locust-corrupt-sawblade"] = (long)60,
        ["game:chicken-henpoult"] = (long)10,
        ["game:chicken-roosterpoult"] = (long)10,
        ["game:deer-marsh-male-adult"] = (long)30,
        ["game:deer-marsh-female-adult"] = (long)30,
        ["game:deer-marsh-male-baby"] = (long)10,
        ["game:deer-marsh-female-baby"] = (long)10,
        ["game:deer-caribou-male-adult"] = (long)30,
        ["game:deer-caribou-female-adult"] = (long)30,
        ["game:deer-caribou-male-baby"] = (long)10,
        ["game:deer-caribou-female-baby"] = (long)10,
        ["game:deer-water-male-adult"] = (long)30,
        ["game:deer-water-female-adult"] = (long)30,
        ["game:deer-water-male-baby"] = (long)10,
        ["game:deer-water-female-baby"] = (long)10,
        ["game:deer-redbrocket-female-adult"] = (long)10,
        ["game:deer-redbrocket-male-baby"] = (long)10,
        ["game:deer-redbrocket-female-baby"] = (long)10,
        ["game:fish-freshwater-alewife-shad-adult"] = (long)5,
        ["game:fish-freshwater-chub-river-adult"] = (long)5,
        ["game:fish-freshwater-crappie-black-adult"] = (long)5,
        ["game:fish-freshwater-crappie-white-adult"] = (long)5,
        ["game:fish-freshwater-perch-european-adult"] = (long)5,
        ["game:fish-freshwater-perch-yellow-adult"] = (long)5,
        ["game:fish-freshwater-piranha-black-adult"] = (long)5,
        ["game:fish-freshwater-piranha-red-adult"] = (long)5,
        ["game:fish-freshwater-trout-brown-adult"] = (long)5,
        ["game:fish-freshwater-trout-rainbow-adult"] = (long)5,
        ["game:fish-freshwater-bass-largemouth-adult"] = (long)10,
        ["game:fish-freshwater-bass-smallmouth-adult"] = (long)10,
        ["game:fish-freshwater-carp-common-adult"] = (long)10,
        ["game:fish-freshwater-carp-grass-adult"] = (long)10,
        ["game:fish-freshwater-catfish-blue-adult"] = (long)10,
        ["game:fish-freshwater-catfish-channel-adult"] = (long)10,
        ["game:fish-freshwater-pickerel-chain-adult"] = (long)10,
        ["game:fish-freshwater-salmon-coho-adult"] = (long)10,
        ["game:fish-freshwater-tilapia-nile-adult"] = (long)10,
        ["game:fish-freshwater-tilapia-red-adult"] = (long)10,
        ["game:fish-freshwater-walleye-common-adult"] = (long)10,
        ["game:fish-freshwater-pike-northern-adult"] = (long)15,
        ["game:fish-freshwater-arapaima-arapaima-adult"] = (long)20,
        ["game:fish-freshwater-arapaima-gigas-adult"] = (long)20,
        ["game:fish-freshwater-sheatfish-black-adult"] = (long)20,
        ["game:fish-freshwater-sheatfish-white-adult"] = (long)20,
        ["game:fish-saltwater-bream-sea-adult"] = (long)5,
        ["game:fish-saltwater-gurnard-cape-adult"] = (long)5,
        ["game:fish-saltwater-haddock-common-adult"] = (long)5,
        ["game:fish-saltwater-hake-silver-adult"] = (long)5,
        ["game:fish-saltwater-herring-atlantic-adult"] = (long)5,
        ["game:fish-saltwater-mackerel-atlantic-adult"] = (long)5,
        ["game:fish-saltwater-pollock-alaska-adult"] = (long)5,
        ["game:fish-saltwater-perch-pacific-adult"] = (long)5,
        ["game:fish-saltwater-barracuda-great-adult"] = (long)10,
        ["game:fish-saltwater-grouper-black-adult"] = (long)10,
        ["game:fish-saltwater-salmon-pink-adult"] = (long)10,
        ["game:fish-saltwater-snapper-red-adult"] = (long)10,
        ["game:fish-saltwater-tuna-skipjack-adult"] = (long)10,
        ["game:fish-saltwater-wolf-bering-adult"] = (long)10,
        ["game:fish-saltwater-amberjack-yellowtail-adult"] = (long)15,
        ["game:fish-saltwater-mahi-mahi-common-adult"] = (long)15,
        ["game:fish-saltwater-wreckfish-atlantic-adult"] = (long)15,
        ["game:fish-saltwater-coelacanth-common-adult"] = (long)20,
        ["game:fish-saltwater-sturgeon-atlantic-adult"] = (long)20,
        ["game:fish-reef-angel-bicolor-adult"] = (long)5,
        ["game:fish-reef-butterfly-copperband-adult"] = (long)5,
        ["game:fish-reef-butterfly-blackwedged-adult"] = (long)5,
        ["game:fish-reef-clown-black-adult"] = (long)5,
        ["game:fish-reef-clown-common-adult"] = (long)5,
        ["game:fish-reef-clown-yellowstripe-adult"] = (long)5,
        ["game:fish-reef-puffer-longspine-adult"] = (long)5,
        ["game:fish-reef-tang-banded-adult"] = (long)5,
        ["game:fish-reef-tang-powderblue-adult"] = (long)5,
        ["game:fish-reef-trigger-titan-adult"] = (long)5,
        ["game:fish-reef-wrasse-creole-adult"] = (long)5,
    };

    public static void PopulateAxeConfiguration(ICoreAPI api)
    {
        Dictionary<string, object> axeLevelStats = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/levelstats",
            "axe",
            BuildAxeDefaultConfig());
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
            BuildAxeEntityExpDefaultConfig());
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
    private static float pickaxeIncrementDamagePerLevel = 0.03f;
    private static float pickaxeBaseMiningSpeed = 1.0f;
    private static float pickaxeIncrementMiningSpeedMultiplyPerLevel = 0.03f;
    private static float pickaxeBaseOreMultiply = 0.0f;
    private static float pickaxeIncrementOreMultiplyPerLevel = 0.1f;
    public static int pickaxeMaxLevel = 999;


    public static int ExpPerHitPickaxe => pickaxeEXPPerHit;
    public static int ExpPerBreakingPickaxe => pickaxeEXPPerBreaking;

    private static Dictionary<string, object> BuildPickaxeEntityExpDefaultConfig() => new()
    {
        ["game:sheep-bighorn-male"] = (long)50,
        ["game:sheep-bighorn-female"] = (long)50,
        ["game:sheep-bighorn-lamb"] = (long)20,
        ["game:chicken-rooster"] = (long)10,
        ["game:chicken-hen"] = (long)10,
        ["game:chicken-baby"] = (long)10,
        ["game:wolf-male"] = (long)40,
        ["game:wolf-female"] = (long)40,
        ["game:wolf-pup"] = (long)10,
        ["game:hyena-male"] = (long)40,
        ["game:hyena-female"] = (long)40,
        ["game:hyena-pup"] = (long)10,
        ["game:fox-male-red"] = (long)20,
        ["game:fox-female-red"] = (long)20,
        ["game:fox-pup"] = (long)10,
        ["game:fox-pup-red"] = (long)20,
        ["game:fox-pup-arctic"] = (long)20,
        ["game:fox-male-arctic"] = (long)10,
        ["game:fox-female-arctic"] = (long)10,
        ["game:raccoon-male"] = (long)20,
        ["game:raccoon-female"] = (long)20,
        ["game:raccoon-pup"] = (long)10,
        ["game:hare-male-arctic"] = (long)30,
        ["game:hare-male-ashgrey"] = (long)30,
        ["game:hare-male-darkbrown"] = (long)30,
        ["game:hare-male-desert"] = (long)30,
        ["game:hare-male-gold"] = (long)40,
        ["game:hare-male-lightbrown"] = (long)40,
        ["game:hare-male-lightgrey"] = (long)40,
        ["game:hare-male-silver"] = (long)40,
        ["game:hare-male-smokegrey"] = (long)50,
        ["game:hare-female-arctic"] = (long)60,
        ["game:hare-female-ashgrey"] = (long)60,
        ["game:hare-female-gold"] = (long)70,
        ["game:hare-female-lightbrown"] = (long)40,
        ["game:hare-female-lightgrey"] = (long)40,
        ["game:hare-female-silver"] = (long)40,
        ["game:hare-female-smokegrey"] = (long)30,
        ["game:hare-baby"] = (long)20,
        ["game:drifter-normal"] = (long)40,
        ["game:drifter-deep"] = (long)50,
        ["game:drifter-tainted"] = (long)60,
        ["game:drifter-corrupt"] = (long)70,
        ["game:drifter-nightmare"] = (long)80,
        ["game:drifter-double-headed"] = (long)90,
        ["game:locust-bronze"] = (long)60,
        ["game:locust-corrupt"] = (long)60,
        ["game:bell-normal"] = (long)100,
        ["game:bear-female-black"] = (long)50,
        ["game:bear-female-brown"] = (long)50,
        ["game:bear-female-sun"] = (long)50,
        ["game:bear-female-panda"] = (long)50,
        ["game:bear-female-polar"] = (long)50,
        ["game:bear-male-black"] = (long)50,
        ["game:bear-male-brown"] = (long)50,
        ["game:bear-male-sun"] = (long)50,
        ["game:bear-male-panda"] = (long)50,
        ["game:bear-male-polar"] = (long)50,
        ["game:locust-bronze-hacked"] = (long)60,
        ["game:locust-corrupt-hacked"] = (long)60,
        ["game:gazelle-male"] = (long)50,
        ["game:gazelle-female"] = (long)50,
        ["game:gazelle-calf"] = (long)30,
        ["game:deer-moose-male-adult"] = (long)30,
        ["game:deer-moose-female-adult"] = (long)50,
        ["game:deer-moose-male-baby"] = (long)50,
        ["game:deer-moose-female-baby"] = (long)30,
        ["game:deer-whitetail-male-adult"] = (long)30,
        ["game:deer-whitetail-female-adult"] = (long)30,
        ["game:deer-whitetail-male-baby"] = (long)30,
        ["game:deer-whitetail-female-baby"] = (long)10,
        ["game:deer-redbrocket-male-adult"] = (long)10,
        ["game:deer-chital-female-baby"] = (long)60,
        ["game:deer-guemal-male-adult"] = (long)60,
        ["game:deer-guemal-female-adult"] = (long)20,
        ["game:deer-guemal-male-baby"] = (long)20,
        ["game:deer-guemal-female-baby"] = (long)60,
        ["game:deer-pampas-male-adult"] = (long)60,
        ["game:deer-pampas-female-adult"] = (long)70,
        ["game:deer-pampas-male-baby"] = (long)70,
        ["game:deer-pampas-female-baby"] = (long)40,
        ["game:deer-pudu-male-adult"] = (long)40,
        ["game:deer-pudu-female-adult"] = (long)10,
        ["game:deer-pudu-male-baby"] = (long)10,
        ["game:deer-pudu-female-baby"] = (long)60,
        ["game:deer-elk-male-adult"] = (long)60,
        ["game:deer-elk-female-adult"] = (long)20,
        ["game:deer-elk-male-baby"] = (long)20,
        ["game:deer-elk-female-baby"] = (long)50,
        ["game:deer-taruca-male-adult"] = (long)50,
        ["game:deer-taruca-female-adult"] = (long)20,
        ["game:deer-taruca-male-baby"] = (long)20,
        ["game:deer-taruca-female-baby"] = (long)60,
        ["game:deer-chital-male-adult"] = (long)60,
        ["game:deer-chital-female-adult"] = (long)20,
        ["game:deer-chital-male-baby"] = (long)20,
        ["game:deer-fallow-female-baby"] = (long)60,
        ["game:deer-fallow-male-adult"] = (long)60,
        ["game:deer-fallow-male-baby"] = (long)20,
        ["game:deer-fallow-female-adult"] = (long)20,
        ["game:goat-angora-male-adult"] = (long)70,
        ["game:goat-angora-female-adult"] = (long)70,
        ["game:goat-angora-male-baby"] = (long)30,
        ["game:goat-angora-female-baby"] = (long)30,
        ["game:goat-ibexalp-male-adult"] = (long)70,
        ["game:goat-ibexalp-female-adult"] = (long)70,
        ["game:goat-ibexalp-male-baby"] = (long)30,
        ["game:goat-ibexalp-female-baby"] = (long)30,
        ["game:goat-ibexnub-male-adult"] = (long)50,
        ["game:goat-ibexnub-female-adult"] = (long)50,
        ["game:goat-ibexnub-male-baby"] = (long)20,
        ["game:goat-ibexnub-female-baby"] = (long)20,
        ["game:goat-markhor-male-adult"] = (long)60,
        ["game:goat-markhor-female-adult"] = (long)60,
        ["game:goat-markhor-male-baby"] = (long)20,
        ["game:goat-markhor-female-baby"] = (long)20,
        ["game:goat-mountain-male-adult"] = (long)40,
        ["game:goat-mountain-female-adult"] = (long)40,
        ["game:goat-mountain-male-baby"] = (long)20,
        ["game:goat-mountain-female-baby"] = (long)20,
        ["game:goat-muskox-male-adult"] = (long)40,
        ["game:goat-muskox-female-adult"] = (long)40,
        ["game:goat-muskox-male-baby"] = (long)20,
        ["game:goat-muskox-female-baby"] = (long)20,
        ["game:goat-nubian-male-adult"] = (long)40,
        ["game:goat-nubian-female-adult"] = (long)40,
        ["game:goat-nubian-male-baby"] = (long)20,
        ["game:goat-sirohi-male-adult"] = (long)40,
        ["game:goat-sirohi-female-adult"] = (long)40,
        ["game:goat-sirohi-male-baby"] = (long)20,
        ["game:goat-sirohi-female-baby"] = (long)20,
        ["game:goat-takingold-male-adult"] = (long)40,
        ["game:goat-takingold-female-adult"] = (long)40,
        ["game:goat-takingold-male-baby"] = (long)20,
        ["game:goat-takingold-female-baby"] = (long)20,
        ["game:goat-turdag-male-adult"] = (long)40,
        ["game:goat-turdag-female-adult"] = (long)40,
        ["game:goat-turdag-male-baby"] = (long)20,
        ["game:goat-turdag-female-baby"] = (long)20,
        ["game:goat-valais-male-adult"] = (long)40,
        ["game:goat-valais-female-adult"] = (long)40,
        ["game:goat-valais-male-baby"] = (long)20,
        ["game:goat-valais-female-baby"] = (long)20,
        ["game:pig-eurasian-adult-male"] = (long)30,
        ["game:pig-eurasian-adult-female"] = (long)30,
        ["game:pig-eurasian-elder-male"] = (long)40,
        ["game:pig-eurasian-elder-female"] = (long)40,
        ["game:pig-redriver-adult-male"] = (long)35,
        ["game:pig-redriver-adult-female"] = (long)35,
        ["game:pig-warthog-adult-male"] = (long)40,
        ["game:pig-warthog-adult-female"] = (long)40,
        ["game:pig-eurasian-baby-male"] = (long)10,
        ["game:pig-eurasian-baby-female"] = (long)10,
        ["game:pig-redriver-baby-male"] = (long)10,
        ["game:pig-redriver-baby-female"] = (long)10,
        ["game:pig-warthog-baby-male"] = (long)10,
        ["game:pig-warthog-baby-female"] = (long)10,
        ["game:sheep-mouflon-male"] = (long)50,
        ["game:sheep-mouflon-female"] = (long)50,
        ["game:sheep-mouflon-lamb"] = (long)20,
        ["game:shiver-surface"] = (long)50,
        ["game:shiver-deep"] = (long)60,
        ["game:shiver-tainted"] = (long)70,
        ["game:shiver-corrupt"] = (long)80,
        ["game:shiver-nightmare"] = (long)90,
        ["game:shiver-stilt"] = (long)60,
        ["game:shiver-bellhead"] = (long)80,
        ["game:shiver-deepsplit"] = (long)90,
        ["game:bowtorn-surface"] = (long)50,
        ["game:bowtorn-deep"] = (long)60,
        ["game:bowtorn-tainted"] = (long)70,
        ["game:bowtorn-corrupt"] = (long)80,
        ["game:bowtorn-nightmare"] = (long)90,
        ["game:bowtorn-gearfoot"] = (long)80,
        ["game:erel-pristine"] = (long)200,
        ["game:erel-corrupted"] = (long)250,
        ["game:eidolon-immobilized"] = (long)300,
        ["game:bellmini-normal"] = (long)100,
        ["game:locust-corrupt-sawblade"] = (long)60,
        ["game:chicken-henpoult"] = (long)10,
        ["game:chicken-roosterpoult"] = (long)10,
        ["game:deer-marsh-male-adult"] = (long)30,
        ["game:deer-marsh-female-adult"] = (long)30,
        ["game:deer-marsh-male-baby"] = (long)10,
        ["game:deer-marsh-female-baby"] = (long)10,
        ["game:deer-caribou-male-adult"] = (long)30,
        ["game:deer-caribou-female-adult"] = (long)30,
        ["game:deer-caribou-male-baby"] = (long)10,
        ["game:deer-caribou-female-baby"] = (long)10,
        ["game:deer-water-male-adult"] = (long)30,
        ["game:deer-water-female-adult"] = (long)30,
        ["game:deer-water-male-baby"] = (long)10,
        ["game:deer-water-female-baby"] = (long)10,
        ["game:deer-redbrocket-female-adult"] = (long)10,
        ["game:deer-redbrocket-male-baby"] = (long)10,
        ["game:deer-redbrocket-female-baby"] = (long)10,
        ["game:fish-freshwater-alewife-shad-adult"] = (long)5,
        ["game:fish-freshwater-chub-river-adult"] = (long)5,
        ["game:fish-freshwater-crappie-black-adult"] = (long)5,
        ["game:fish-freshwater-crappie-white-adult"] = (long)5,
        ["game:fish-freshwater-perch-european-adult"] = (long)5,
        ["game:fish-freshwater-perch-yellow-adult"] = (long)5,
        ["game:fish-freshwater-piranha-black-adult"] = (long)5,
        ["game:fish-freshwater-piranha-red-adult"] = (long)5,
        ["game:fish-freshwater-trout-brown-adult"] = (long)5,
        ["game:fish-freshwater-trout-rainbow-adult"] = (long)5,
        ["game:fish-freshwater-bass-largemouth-adult"] = (long)10,
        ["game:fish-freshwater-bass-smallmouth-adult"] = (long)10,
        ["game:fish-freshwater-carp-common-adult"] = (long)10,
        ["game:fish-freshwater-carp-grass-adult"] = (long)10,
        ["game:fish-freshwater-catfish-blue-adult"] = (long)10,
        ["game:fish-freshwater-catfish-channel-adult"] = (long)10,
        ["game:fish-freshwater-pickerel-chain-adult"] = (long)10,
        ["game:fish-freshwater-salmon-coho-adult"] = (long)10,
        ["game:fish-freshwater-tilapia-nile-adult"] = (long)10,
        ["game:fish-freshwater-tilapia-red-adult"] = (long)10,
        ["game:fish-freshwater-walleye-common-adult"] = (long)10,
        ["game:fish-freshwater-pike-northern-adult"] = (long)15,
        ["game:fish-freshwater-arapaima-arapaima-adult"] = (long)20,
        ["game:fish-freshwater-arapaima-gigas-adult"] = (long)20,
        ["game:fish-freshwater-sheatfish-black-adult"] = (long)20,
        ["game:fish-freshwater-sheatfish-white-adult"] = (long)20,
        ["game:fish-saltwater-bream-sea-adult"] = (long)5,
        ["game:fish-saltwater-gurnard-cape-adult"] = (long)5,
        ["game:fish-saltwater-haddock-common-adult"] = (long)5,
        ["game:fish-saltwater-hake-silver-adult"] = (long)5,
        ["game:fish-saltwater-herring-atlantic-adult"] = (long)5,
        ["game:fish-saltwater-mackerel-atlantic-adult"] = (long)5,
        ["game:fish-saltwater-pollock-alaska-adult"] = (long)5,
        ["game:fish-saltwater-perch-pacific-adult"] = (long)5,
        ["game:fish-saltwater-barracuda-great-adult"] = (long)10,
        ["game:fish-saltwater-grouper-black-adult"] = (long)10,
        ["game:fish-saltwater-salmon-pink-adult"] = (long)10,
        ["game:fish-saltwater-snapper-red-adult"] = (long)10,
        ["game:fish-saltwater-tuna-skipjack-adult"] = (long)10,
        ["game:fish-saltwater-wolf-bering-adult"] = (long)10,
        ["game:fish-saltwater-amberjack-yellowtail-adult"] = (long)15,
        ["game:fish-saltwater-mahi-mahi-common-adult"] = (long)15,
        ["game:fish-saltwater-wreckfish-atlantic-adult"] = (long)15,
        ["game:fish-saltwater-coelacanth-common-adult"] = (long)20,
        ["game:fish-saltwater-sturgeon-atlantic-adult"] = (long)20,
        ["game:fish-reef-angel-bicolor-adult"] = (long)5,
        ["game:fish-reef-butterfly-copperband-adult"] = (long)5,
        ["game:fish-reef-butterfly-blackwedged-adult"] = (long)5,
        ["game:fish-reef-clown-black-adult"] = (long)5,
        ["game:fish-reef-clown-common-adult"] = (long)5,
        ["game:fish-reef-clown-yellowstripe-adult"] = (long)5,
        ["game:fish-reef-puffer-longspine-adult"] = (long)5,
        ["game:fish-reef-tang-banded-adult"] = (long)5,
        ["game:fish-reef-tang-powderblue-adult"] = (long)5,
        ["game:fish-reef-trigger-titan-adult"] = (long)5,
        ["game:fish-reef-wrasse-creole-adult"] = (long)5,
    };

    private static Dictionary<string, object> BuildPickaxeOresDefaultConfig() => new()
    {
        ["game:ore-poor-nativecopper-andesite"] = (long)20,
        ["game:ore-medium-nativecopper-andesite"] = (long)30,
        ["game:ore-rich-nativecopper-andesite"] = (long)40,
        ["game:ore-poor-quartz_nativegold-andesite"] = (long)50,
        ["game:ore-medium-quartz_nativegold-andesite"] = (long)60,
        ["game:ore-rich-quartz_nativegold-andesite"] = (long)70,
        ["game:ore-bountiful-quartz_nativegold-andesite"] = (long)80,
        ["game:ore-poor-cassiterite-andesite"] = (long)20,
        ["game:ore-medium-cassiterite-andesite"] = (long)30,
        ["game:ore-poor-chromite-andesite"] = (long)20,
        ["game:ore-medium-chromite-andesite"] = (long)30,
        ["game:ore-poor-ilmenite-andesite"] = (long)20,
        ["game:ore-medium-ilmenite-andesite"] = (long)30,
        ["game:ore-poor-sphalerite-andesite"] = (long)20,
        ["game:ore-medium-sphalerite-andesite"] = (long)30,
        ["game:ore-poor-quartz_nativesilver-andesite"] = (long)20,
        ["game:ore-medium-quartz_nativesilver-andesite"] = (long)30,
        ["game:ore-rich-quartz_nativesilver-andesite"] = (long)40,
        ["game:ore-bountiful-quartz_nativesilver-andesite"] = (long)50,
        ["game:ore-poor-bismuthinite-andesite"] = (long)20,
        ["game:ore-medium-bismuthinite-andesite"] = (long)30,
        ["game:ore-rich-bismuthinite-andesite"] = (long)40,
        ["game:ore-poor-magnetite-andesite"] = (long)20,
        ["game:ore-medium-magnetite-andesite"] = (long)30,
        ["game:ore-rich-magnetite-andesite"] = (long)40,
        ["game:ore-poor-pentlandite-andesite"] = (long)20,
        ["game:ore-medium-pentlandite-andesite"] = (long)30,
        ["game:ore-poor-uranium-andesite"] = (long)40,
        ["game:ore-medium-uranium-andesite"] = (long)60,
        ["game:ore-rich-uranium-andesite"] = (long)70,
        ["game:ore-poor-nativecopper-chalk"] = (long)20,
        ["game:ore-medium-nativecopper-chalk"] = (long)30,
        ["game:ore-poor-quartz_nativegold-chalk"] = (long)50,
        ["game:ore-medium-quartz_nativegold-chalk"] = (long)60,
        ["game:ore-rich-quartz_nativegold-chalk"] = (long)70,
        ["game:ore-bountiful-quartz_nativegold-chalk"] = (long)80,
        ["game:ore-poor-galena-chalk"] = (long)20,
        ["game:ore-medium-galena-chalk"] = (long)30,
        ["game:ore-poor-sphalerite-chalk"] = (long)20,
        ["game:ore-medium-sphalerite-chalk"] = (long)30,
        ["game:ore-rich-sphalerite-chalk"] = (long)40,
        ["game:ore-poor-quartz_nativesilver-chalk"] = (long)20,
        ["game:ore-medium-quartz_nativesilver-chalk"] = (long)30,
        ["game:ore-rich-quartz_nativesilver-chalk"] = (long)40,
        ["game:ore-bountiful-quartz_nativesilver-chalk"] = (long)50,
        ["game:ore-poor-galena_nativesilver-chalk"] = (long)20,
        ["game:ore-medium-galena_nativesilver-chalk"] = (long)30,
        ["game:ore-poor-magnetite-chalk"] = (long)20,
        ["game:ore-medium-magnetite-chalk"] = (long)30,
        ["game:ore-poor-uranium-chalk"] = (long)40,
        ["game:ore-medium-uranium-chalk"] = (long)60,
        ["game:ore-rich-uranium-chalk"] = (long)70,
        ["game:ore-poor-rhodochrosite-chalk"] = (long)20,
        ["game:ore-poor-nativecopper-chert"] = (long)20,
        ["game:ore-medium-nativecopper-chert"] = (long)30,
        ["game:ore-poor-limonite-chert"] = (long)20,
        ["game:ore-medium-limonite-chert"] = (long)30,
        ["game:ore-rich-limonite-chert"] = (long)40,
        ["game:ore-bountiful-limonite-chert"] = (long)50,
        ["game:ore-poor-quartz_nativegold-chert"] = (long)50,
        ["game:ore-medium-quartz_nativegold-chert"] = (long)60,
        ["game:ore-rich-quartz_nativegold-chert"] = (long)70,
        ["game:ore-bountiful-quartz_nativegold-chert"] = (long)80,
        ["game:ore-poor-galena-chert"] = (long)20,
        ["game:ore-medium-galena-chert"] = (long)30,
        ["game:ore-rich-galena-chert"] = (long)40,
        ["game:ore-bountiful-galena-chert"] = (long)50,
        ["game:ore-poor-sphalerite-chert"] = (long)20,
        ["game:ore-medium-sphalerite-chert"] = (long)30,
        ["game:ore-rich-sphalerite-chert"] = (long)40,
        ["game:ore-poor-quartz_nativesilver-chert"] = (long)20,
        ["game:ore-medium-quartz_nativesilver-chert"] = (long)30,
        ["game:ore-rich-quartz_nativesilver-chert"] = (long)40,
        ["game:ore-bountiful-quartz_nativesilver-chert"] = (long)50,
        ["game:ore-poor-galena_nativesilver-chert"] = (long)20,
        ["game:ore-medium-galena_nativesilver-chert"] = (long)30,
        ["game:ore-poor-uranium-chert"] = (long)40,
        ["game:ore-medium-uranium-chert"] = (long)60,
        ["game:ore-rich-uranium-chert"] = (long)70,
        ["game:ore-poor-rhodochrosite-chert"] = (long)20,
        ["game:ore-poor-nativecopper-conglomerate"] = (long)20,
        ["game:ore-medium-nativecopper-conglomerate"] = (long)30,
        ["game:ore-poor-quartz_nativegold-conglomerate"] = (long)50,
        ["game:ore-medium-quartz_nativegold-conglomerate"] = (long)60,
        ["game:ore-rich-quartz_nativegold-conglomerate"] = (long)70,
        ["game:ore-bountiful-quartz_nativegold-conglomerate"] = (long)80,
        ["game:ore-poor-galena-conglomerate"] = (long)20,
        ["game:ore-medium-galena-conglomerate"] = (long)30,
        ["game:ore-poor-sphalerite-conglomerate"] = (long)20,
        ["game:ore-medium-sphalerite-conglomerate"] = (long)30,
        ["game:ore-rich-sphalerite-conglomerate"] = (long)40,
        ["game:ore-poor-quartz_nativesilver-conglomerate"] = (long)20,
        ["game:ore-medium-quartz_nativesilver-conglomerate"] = (long)30,
        ["game:ore-rich-quartz_nativesilver-conglomerate"] = (long)40,
        ["game:ore-bountiful-quartz_nativesilver-conglomerate"] = (long)50,
        ["game:ore-poor-galena_nativesilver-conglomerate"] = (long)20,
        ["game:ore-medium-galena_nativesilver-conglomerate"] = (long)30,
        ["game:ore-poor-magnetite-conglomerate"] = (long)20,
        ["game:ore-medium-magnetite-conglomerate"] = (long)30,
        ["game:ore-poor-uranium-conglomerate"] = (long)40,
        ["game:ore-medium-uranium-conglomerate"] = (long)60,
        ["game:ore-poor-rhodochrosite-conglomerate"] = (long)20,
        ["game:ore-rich-rhodochrosite-conglomerate"] = (long)40,
        ["game:ore-poor-quartz_nativegold-limestone"] = (long)50,
        ["game:ore-medium-quartz_nativegold-limestone"] = (long)60,
        ["game:ore-rich-quartz_nativegold-limestone"] = (long)70,
        ["game:ore-bountiful-quartz_nativegold-limestone"] = (long)80,
        ["game:ore-poor-galena-limestone"] = (long)20,
        ["game:ore-medium-galena-limestone"] = (long)30,
        ["game:ore-poor-sphalerite-limestone"] = (long)20,
        ["game:ore-medium-sphalerite-limestone"] = (long)30,
        ["game:ore-rich-sphalerite-limestone"] = (long)40,
        ["game:ore-poor-quartz_nativesilver-limestone"] = (long)20,
        ["game:ore-medium-quartz_nativesilver-limestone"] = (long)30,
        ["game:ore-rich-quartz_nativesilver-limestone"] = (long)40,
        ["game:ore-bountiful-quartz_nativesilver-limestone"] = (long)50,
        ["game:ore-poor-galena_nativesilver-limestone"] = (long)20,
        ["game:ore-medium-galena_nativesilver-limestone"] = (long)30,
        ["game:ore-poor-hematite-limestone"] = (long)20,
        ["game:ore-medium-hematite-limestone"] = (long)30,
        ["game:ore-rich-hematite-limestone"] = (long)40,
        ["game:ore-poor-malachite-limestone"] = (long)20,
        ["game:ore-medium-malachite-limestone"] = (long)30,
        ["game:ore-rich-malachite-limestone"] = (long)40,
        ["game:ore-bountiful-malachite-limestone"] = (long)50,
        ["game:ore-poor-uranium-limestone"] = (long)40,
        ["game:ore-medium-uranium-limestone"] = (long)60,
        ["game:ore-rich-uranium-limestone"] = (long)70,
        ["game:ore-poor-rhodochrosite-limestone"] = (long)20,
        ["game:ore-poor-nativecopper-claystone"] = (long)20,
        ["game:ore-medium-nativecopper-claystone"] = (long)30,
        ["game:ore-poor-quartz_nativegold-claystone"] = (long)50,
        ["game:ore-medium-quartz_nativegold-claystone"] = (long)60,
        ["game:ore-rich-quartz_nativegold-claystone"] = (long)70,
        ["game:ore-bountiful-quartz_nativegold-claystone"] = (long)80,
        ["game:ore-poor-bismuthinite-granite"] = (long)20,
        ["game:ore-medium-bismuthinite-granite"] = (long)30,
        ["game:ore-rich-bismuthinite-granite"] = (long)40,
        ["game:ore-poor-bismuthinite-basalt"] = (long)20,
        ["game:ore-medium-bismuthinite-basalt"] = (long)30,
        ["game:ore-rich-bismuthinite-basalt"] = (long)40,
        ["game:ore-bountiful-bismuthinite-basalt"] = (long)50,
        ["game:ore-poor-bismuthinite-peridotite"] = (long)20,
        ["game:ore-medium-bismuthinite-peridotite"] = (long)30,
        ["game:ore-rich-bismuthinite-peridotite"] = (long)40,
        ["game:ore-poor-bismuthinite-phyllite"] = (long)20,
        ["game:ore-medium-bismuthinite-phyllite"] = (long)30,
        ["game:ore-poor-bismuthinite-slate"] = (long)20,
        ["game:ore-medium-bismuthinite-slate"] = (long)30,
        ["game:ore-poor-cassiterite-granite"] = (long)20,
        ["game:ore-medium-cassiterite-granite"] = (long)30,
        ["game:ore-rich-cassiterite-granite"] = (long)40,
        ["game:ore-bountiful-cassiterite-granite"] = (long)50,
        ["game:ore-poor-cassiterite-basalt"] = (long)20,
        ["game:ore-medium-cassiterite-basalt"] = (long)30,
        ["game:ore-poor-cassiterite-peridotite"] = (long)20,
        ["game:ore-medium-cassiterite-peridotite"] = (long)30,
        ["game:ore-poor-cassiterite-phyllite"] = (long)20,
        ["game:ore-medium-cassiterite-phyllite"] = (long)30,
        ["game:ore-poor-cassiterite-slate"] = (long)20,
        ["game:ore-medium-cassiterite-slate"] = (long)30,
        ["game:ore-poor-sphalerite-granite"] = (long)20,
        ["game:ore-medium-sphalerite-granite"] = (long)30,
        ["game:ore-poor-sphalerite-basalt"] = (long)20,
        ["game:ore-medium-sphalerite-basalt"] = (long)30,
        ["game:ore-poor-sphalerite-peridotite"] = (long)20,
        ["game:ore-medium-sphalerite-peridotite"] = (long)30,
        ["game:ore-poor-sphalerite-claystone"] = (long)20,
        ["game:ore-medium-sphalerite-claystone"] = (long)30,
        ["game:ore-rich-sphalerite-claystone"] = (long)40,
        ["game:ore-poor-sphalerite-sandstone"] = (long)20,
        ["game:ore-medium-sphalerite-sandstone"] = (long)30,
        ["game:ore-rich-sphalerite-sandstone"] = (long)40,
        ["game:ore-poor-sphalerite-shale"] = (long)20,
        ["game:ore-medium-sphalerite-shale"] = (long)30,
        ["game:ore-rich-sphalerite-shale"] = (long)40,
        ["game:ore-poor-sphalerite-phyllite"] = (long)20,
        ["game:ore-medium-sphalerite-phyllite"] = (long)30,
        ["game:ore-rich-sphalerite-phyllite"] = (long)40,
        ["game:ore-bountiful-sphalerite-phyllite"] = (long)50,
        ["game:ore-poor-sphalerite-slate"] = (long)20,
        ["game:ore-medium-sphalerite-slate"] = (long)30,
        ["game:ore-rich-sphalerite-slate"] = (long)40,
        ["game:ore-bountiful-sphalerite-slate"] = (long)50,
        ["game:ore-poor-nativecopper-granite"] = (long)20,
        ["game:ore-medium-nativecopper-granite"] = (long)30,
        ["game:ore-rich-nativecopper-granite"] = (long)40,
        ["game:ore-poor-nativecopper-basalt"] = (long)20,
        ["game:ore-medium-nativecopper-basalt"] = (long)30,
        ["game:ore-rich-nativecopper-basalt"] = (long)40,
        ["game:ore-bountiful-nativecopper-basalt"] = (long)50,
        ["game:ore-poor-nativecopper-slate"] = (long)20,
        ["game:ore-medium-nativecopper-slate"] = (long)30,
        ["game:ore-rich-nativecopper-slate"] = (long)40,
        ["game:ore-poor-nativecopper-peridotite"] = (long)20,
        ["game:ore-medium-nativecopper-peridotite"] = (long)30,
        ["game:ore-rich-nativecopper-peridotite"] = (long)40,
        ["game:ore-poor-nativecopper-sandstone"] = (long)20,
        ["game:ore-medium-nativecopper-sandstone"] = (long)30,
        ["game:ore-poor-nativecopper-shale"] = (long)20,
        ["game:ore-medium-nativecopper-shale"] = (long)30,
        ["game:ore-poor-nativecopper-phyllite"] = (long)20,
        ["game:ore-medium-nativecopper-phyllite"] = (long)30,
        ["game:ore-rich-nativecopper-phyllite"] = (long)40,
        ["game:ore-poor-malachite-whitemarble"] = (long)20,
        ["game:ore-medium-malachite-whitemarble"] = (long)30,
        ["game:ore-rich-malachite-whitemarble"] = (long)40,
        ["game:ore-poor-malachite-redmarble"] = (long)20,
        ["game:ore-medium-malachite-redmarble"] = (long)30,
        ["game:ore-rich-malachite-redmarble"] = (long)40,
        ["game:ore-poor-malachite-greenmarble"] = (long)20,
        ["game:ore-medium-malachite-greenmarble"] = (long)30,
        ["game:ore-rich-malachite-greenmarble"] = (long)40,
        ["game:ore-poor-galena-claystone"] = (long)20,
        ["game:ore-medium-galena-claystone"] = (long)30,
        ["game:ore-rich-galena-claystone"] = (long)40,
        ["game:ore-poor-galena-sandstone"] = (long)20,
        ["game:ore-medium-galena-sandstone"] = (long)30,
        ["game:ore-rich-galena-sandstone"] = (long)40,
        ["game:ore-poor-galena-shale"] = (long)20,
        ["game:ore-medium-galena-shale"] = (long)30,
        ["game:ore-rich-galena-shale"] = (long)40,
        ["game:ore-bountiful-galena-shale"] = (long)50,
        ["game:ore-poor-galena_nativesilver-claystone"] = (long)20,
        ["game:ore-medium-galena_nativesilver-claystone"] = (long)30,
        ["game:ore-poor-galena_nativesilver-sandstone"] = (long)20,
        ["game:ore-medium-galena_nativesilver-sandstone"] = (long)30,
        ["game:ore-poor-galena_nativesilver-shale"] = (long)20,
        ["game:ore-medium-galena_nativesilver-shale"] = (long)30,
        ["game:ore-poor-quartz_nativesilver-granite"] = (long)20,
        ["game:ore-medium-quartz_nativesilver-granite"] = (long)30,
        ["game:ore-rich-quartz_nativesilver-granite"] = (long)40,
        ["game:ore-bountiful-quartz_nativesilver-granite"] = (long)50,
        ["game:ore-poor-quartz_nativesilver-basalt"] = (long)20,
        ["game:ore-medium-quartz_nativesilver-basalt"] = (long)30,
        ["game:ore-rich-quartz_nativesilver-basalt"] = (long)40,
        ["game:ore-bountiful-quartz_nativesilver-basalt"] = (long)50,
        ["game:ore-poor-quartz_nativesilver-peridotite"] = (long)20,
        ["game:ore-medium-quartz_nativesilver-peridotite"] = (long)30,
        ["game:ore-rich-quartz_nativesilver-peridotite"] = (long)40,
        ["game:ore-bountiful-quartz_nativesilver-peridotite"] = (long)50,
        ["game:ore-poor-quartz_nativesilver-claystone"] = (long)20,
        ["game:ore-medium-quartz_nativesilver-claystone"] = (long)30,
        ["game:ore-rich-quartz_nativesilver-claystone"] = (long)40,
        ["game:ore-bountiful-quartz_nativesilver-claystone"] = (long)50,
        ["game:ore-poor-quartz_nativesilver-sandstone"] = (long)20,
        ["game:ore-medium-quartz_nativesilver-sandstone"] = (long)30,
        ["game:ore-rich-quartz_nativesilver-sandstone"] = (long)40,
        ["game:ore-bountiful-quartz_nativesilver-sandstone"] = (long)50,
        ["game:ore-poor-quartz_nativesilver-shale"] = (long)20,
        ["game:ore-medium-quartz_nativesilver-shale"] = (long)30,
        ["game:ore-rich-quartz_nativesilver-shale"] = (long)40,
        ["game:ore-bountiful-quartz_nativesilver-shale"] = (long)50,
        ["game:ore-poor-quartz_nativesilver-phyllite"] = (long)20,
        ["game:ore-medium-quartz_nativesilver-phyllite"] = (long)30,
        ["game:ore-rich-quartz_nativesilver-phyllite"] = (long)40,
        ["game:ore-bountiful-quartz_nativesilver-phyllite"] = (long)50,
        ["game:ore-poor-quartz_nativesilver-slate"] = (long)20,
        ["game:ore-medium-quartz_nativesilver-slate"] = (long)30,
        ["game:ore-rich-quartz_nativesilver-slate"] = (long)40,
        ["game:ore-bountiful-quartz_nativesilver-slate"] = (long)50,
        ["game:ore-poor-quartz_nativegold-granite"] = (long)50,
        ["game:ore-medium-quartz_nativegold-granite"] = (long)60,
        ["game:ore-rich-quartz_nativegold-granite"] = (long)70,
        ["game:ore-bountiful-quartz_nativegold-granite"] = (long)80,
        ["game:ore-poor-quartz_nativegold-basalt"] = (long)50,
        ["game:ore-medium-quartz_nativegold-basalt"] = (long)60,
        ["game:ore-rich-quartz_nativegold-basalt"] = (long)70,
        ["game:ore-bountiful-quartz_nativegold-basalt"] = (long)80,
        ["game:ore-poor-quartz_nativegold-peridotite"] = (long)50,
        ["game:ore-medium-quartz_nativegold-peridotite"] = (long)60,
        ["game:ore-rich-quartz_nativegold-peridotite"] = (long)70,
        ["game:ore-bountiful-quartz_nativegold-peridotite"] = (long)80,
        ["game:ore-poor-quartz_nativegold-sandstone"] = (long)50,
        ["game:ore-medium-quartz_nativegold-sandstone"] = (long)60,
        ["game:ore-rich-quartz_nativegold-sandstone"] = (long)70,
        ["game:ore-bountiful-quartz_nativegold-sandstone"] = (long)80,
        ["game:ore-poor-quartz_nativegold-shale"] = (long)50,
        ["game:ore-medium-quartz_nativegold-shale"] = (long)60,
        ["game:ore-rich-quartz_nativegold-shale"] = (long)70,
        ["game:ore-bountiful-quartz_nativegold-shale"] = (long)80,
        ["game:ore-poor-quartz_nativegold-phyllite"] = (long)50,
        ["game:ore-medium-quartz_nativegold-phyllite"] = (long)60,
        ["game:ore-rich-quartz_nativegold-phyllite"] = (long)70,
        ["game:ore-bountiful-quartz_nativegold-phyllite"] = (long)80,
        ["game:ore-poor-quartz_nativegold-slate"] = (long)50,
        ["game:ore-medium-quartz_nativegold-slate"] = (long)60,
        ["game:ore-rich-quartz_nativegold-slate"] = (long)70,
        ["game:ore-bountiful-quartz_nativegold-slate"] = (long)80,
        ["game:ore-poor-limonite-basalt"] = (long)20,
        ["game:ore-medium-limonite-basalt"] = (long)30,
        ["game:ore-poor-limonite-shale"] = (long)20,
        ["game:ore-medium-limonite-shale"] = (long)30,
        ["game:ore-rich-limonite-shale"] = (long)40,
        ["game:ore-bountiful-limonite-shale"] = (long)50,
        ["game:ore-poor-hematite-granite"] = (long)20,
        ["game:ore-medium-hematite-granite"] = (long)30,
        ["game:ore-rich-hematite-granite"] = (long)40,
        ["game:ore-bountiful-hematite-granite"] = (long)50,
        ["game:ore-poor-hematite-peridotite"] = (long)20,
        ["game:ore-medium-hematite-peridotite"] = (long)30,
        ["game:ore-rich-hematite-peridotite"] = (long)40,
        ["game:ore-bountiful-hematite-peridotite"] = (long)50,
        ["game:ore-poor-hematite-sandstone"] = (long)20,
        ["game:ore-medium-hematite-sandstone"] = (long)30,
        ["game:ore-rich-hematite-sandstone"] = (long)40,
        ["game:ore-poor-hematite-phyllite"] = (long)20,
        ["game:ore-medium-hematite-phyllite"] = (long)30,
        ["game:ore-poor-magnetite-claystone"] = (long)20,
        ["game:ore-medium-magnetite-claystone"] = (long)30,
        ["game:ore-poor-magnetite-slate"] = (long)20,
        ["game:ore-medium-magnetite-slate"] = (long)30,
        ["game:ore-rich-magnetite-slate"] = (long)40,
        ["game:ore-bountiful-magnetite-slate"] = (long)50,
        ["game:ore-poor-chromite-granite"] = (long)20,
        ["game:ore-medium-chromite-granite"] = (long)30,
        ["game:ore-poor-chromite-basalt"] = (long)20,
        ["game:ore-medium-chromite-basalt"] = (long)30,
        ["game:ore-rich-chromite-basalt"] = (long)40,
        ["game:ore-poor-chromite-peridotite"] = (long)20,
        ["game:ore-medium-chromite-peridotite"] = (long)30,
        ["game:ore-rich-chromite-peridotite"] = (long)40,
        ["game:ore-poor-chromite-kimberlite"] = (long)20,
        ["game:ore-medium-chromite-kimberlite"] = (long)30,
        ["game:ore-rich-chromite-kimberlite"] = (long)40,
        ["game:ore-bountiful-chromite-kimberlite"] = (long)50,
        ["game:ore-poor-rhodochrosite-claystone"] = (long)20,
        ["game:ore-medium-rhodochrosite-claystone"] = (long)30,
        ["game:ore-rich-rhodochrosite-claystone"] = (long)40,
        ["game:ore-poor-rhodochrosite-sandstone"] = (long)20,
        ["game:ore-medium-rhodochrosite-sandstone"] = (long)30,
        ["game:ore-rich-rhodochrosite-sandstone"] = (long)40,
        ["game:ore-poor-rhodochrosite-shale"] = (long)20,
        ["game:ore-medium-rhodochrosite-shale"] = (long)30,
        ["game:ore-rich-rhodochrosite-shale"] = (long)40,
        ["game:ore-medium-rhodochrosite-chalk"] = (long)30,
        ["game:ore-medium-rhodochrosite-limestone"] = (long)30,
        ["game:ore-medium-rhodochrosite-chert"] = (long)30,
        ["game:ore-medium-rhodochrosite-conglomerate"] = (long)30,
        ["game:ore-poor-rhodochrosite-phyllite"] = (long)20,
        ["game:ore-medium-rhodochrosite-phyllite"] = (long)30,
        ["game:ore-rich-rhodochrosite-phyllite"] = (long)40,
        ["game:ore-bountiful-rhodochrosite-phyllite"] = (long)50,
        ["game:ore-poor-rhodochrosite-slate"] = (long)20,
        ["game:ore-medium-rhodochrosite-slate"] = (long)30,
        ["game:ore-rich-rhodochrosite-slate"] = (long)40,
        ["game:ore-bountiful-rhodochrosite-slate"] = (long)50,
        ["game:ore-poor-ilmenite-granite"] = (long)20,
        ["game:ore-medium-ilmenite-granite"] = (long)30,
        ["game:ore-poor-ilmenite-basalt"] = (long)20,
        ["game:ore-medium-ilmenite-basalt"] = (long)30,
        ["game:ore-rich-ilmenite-basalt"] = (long)40,
        ["game:ore-poor-ilmenite-peridotite"] = (long)20,
        ["game:ore-medium-ilmenite-peridotite"] = (long)30,
        ["game:ore-rich-ilmenite-peridotite"] = (long)40,
        ["game:ore-poor-ilmenite-kimberlite"] = (long)20,
        ["game:ore-medium-ilmenite-kimberlite"] = (long)30,
        ["game:ore-rich-ilmenite-kimberlite"] = (long)40,
        ["game:ore-bountiful-ilmenite-kimberlite"] = (long)50,
        ["game:ore-poor-ilmenite-phyllite"] = (long)20,
        ["game:ore-medium-ilmenite-phyllite"] = (long)30,
        ["game:ore-rich-ilmenite-phyllite"] = (long)40,
        ["game:ore-poor-ilmenite-slate"] = (long)20,
        ["game:ore-medium-ilmenite-slate"] = (long)30,
        ["game:ore-rich-ilmenite-slate"] = (long)40,
        ["game:ore-poor-stibnite-andesite"] = (long)20,
        ["game:ore-medium-stibnite-andesite"] = (long)30,
        ["game:ore-poor-stibnite-granite"] = (long)20,
        ["game:ore-medium-stibnite-granite"] = (long)30,
        ["game:ore-rich-stibnite-granite"] = (long)40,
        ["game:ore-poor-stibnite-basalt"] = (long)20,
        ["game:ore-medium-stibnite-basalt"] = (long)30,
        ["game:ore-rich-stibnite-basalt"] = (long)40,
        ["game:ore-bountiful-stibnite-basalt"] = (long)50,
        ["game:ore-poor-uranium-granite"] = (long)40,
        ["game:ore-medium-uranium-granite"] = (long)60,
        ["game:ore-rich-uranium-granite"] = (long)70,
        ["game:ore-poor-uranium-basalt"] = (long)40,
        ["game:ore-medium-uranium-basalt"] = (long)60,
        ["game:ore-poor-uranium-peridotite"] = (long)40,
        ["game:ore-medium-uranium-peridotite"] = (long)60,
        ["game:ore-poor-uranium-kimberlite"] = (long)40,
        ["game:ore-medium-uranium-kimberlite"] = (long)60,
        ["game:ore-rich-uranium-kimberlite"] = (long)70,
        ["game:ore-bountiful-uranium-kimberlite"] = (long)80,
        ["game:ore-poor-uranium-claystone"] = (long)40,
        ["game:ore-medium-uranium-claystone"] = (long)60,
        ["game:ore-poor-uranium-sandstone"] = (long)40,
        ["game:ore-medium-uranium-sandstone"] = (long)60,
        ["game:ore-poor-uranium-shale"] = (long)40,
        ["game:ore-medium-uranium-shale"] = (long)60,
        ["game:ore-poor-uranium-phyllite"] = (long)40,
        ["game:ore-medium-uranium-phyllite"] = (long)60,
        ["game:ore-rich-uranium-phyllite"] = (long)70,
        ["game:ore-bountiful-uranium-phyllite"] = (long)80,
        ["game:ore-poor-uranium-slate"] = (long)40,
        ["game:ore-medium-uranium-slate"] = (long)60,
        ["game:ore-rich-uranium-slate"] = (long)70,
        ["game:ore-bountiful-uranium-slate"] = (long)80,
        ["game:ore-poor-pentlandite-granite"] = (long)20,
        ["game:ore-medium-pentlandite-granite"] = (long)30,
        ["game:ore-poor-pentlandite-basalt"] = (long)20,
        ["game:ore-medium-pentlandite-basalt"] = (long)30,
        ["game:ore-poor-pentlandite-peridotite"] = (long)20,
        ["game:ore-medium-pentlandite-peridotite"] = (long)30,
        ["game:ore-rich-pentlandite-peridotite"] = (long)40,
        ["game:ore-bountiful-pentlandite-peridotite"] = (long)50,
        ["game:ore-flint-andesite"] = (long)5,
        ["game:ore-flint-basalt"] = (long)5,
        ["game:ore-flint-bauxite"] = (long)5,
        ["game:ore-flint-chalk"] = (long)5,
        ["game:ore-flint-chert"] = (long)5,
        ["game:ore-flint-claystone"] = (long)5,
        ["game:ore-flint-conglomerate"] = (long)5,
        ["game:ore-flint-granite"] = (long)5,
        ["game:ore-flint-kimberlite"] = (long)5,
        ["game:ore-flint-limestone"] = (long)5,
        ["game:ore-flint-peridotite"] = (long)5,
        ["game:ore-flint-phyllite"] = (long)5,
        ["game:ore-flint-sandstone"] = (long)5,
        ["game:ore-flint-shale"] = (long)5,
        ["game:ore-flint-slate"] = (long)5,
        ["game:ore-flint-travertine"] = (long)5,
        ["game:ore-quartz-andesite"] = (long)10,
        ["game:ore-quartz-granite"] = (long)10,
        ["game:ore-quartz-basalt"] = (long)10,
        ["game:ore-quartz-peridotite"] = (long)10,
        ["game:ore-quartz-claystone"] = (long)10,
        ["game:ore-quartz-sandstone"] = (long)10,
        ["game:ore-quartz-shale"] = (long)10,
        ["game:ore-quartz-chalk"] = (long)10,
        ["game:ore-quartz-limestone"] = (long)10,
        ["game:ore-quartz-chert"] = (long)10,
        ["game:ore-quartz-conglomerate"] = (long)10,
        ["game:ore-quartz-phyllite"] = (long)10,
        ["game:ore-quartz-slate"] = (long)10,
        ["game:ore-quartz_wolframite-granite"] = (long)30,
        ["game:ore-alum-claystone"] = (long)10,
        ["game:ore-alum-sandstone"] = (long)10,
        ["game:ore-alum-shale"] = (long)10,
        ["game:ore-alum-chalk"] = (long)10,
        ["game:ore-alum-limestone"] = (long)10,
        ["game:ore-alum-chert"] = (long)10,
        ["game:ore-alum-conglomerate"] = (long)10,
        ["game:ore-stibnite-limestone"] = (long)20,
        ["game:ore-lignite-claystone"] = (long)10,
        ["game:ore-lignite-sandstone"] = (long)10,
        ["game:ore-lignite-shale"] = (long)10,
        ["game:ore-lignite-chalk"] = (long)10,
        ["game:ore-lignite-limestone"] = (long)10,
        ["game:ore-lignite-chert"] = (long)10,
        ["game:ore-lignite-conglomerate"] = (long)10,
        ["game:ore-bituminouscoal-claystone"] = (long)15,
        ["game:ore-bituminouscoal-sandstone"] = (long)15,
        ["game:ore-bituminouscoal-shale"] = (long)15,
        ["game:ore-bituminouscoal-chalk"] = (long)15,
        ["game:ore-bituminouscoal-limestone"] = (long)15,
        ["game:ore-bituminouscoal-chert"] = (long)15,
        ["game:ore-bituminouscoal-conglomerate"] = (long)15,
        ["game:ore-anthracite-claystone"] = (long)20,
        ["game:ore-anthracite-sandstone"] = (long)20,
        ["game:ore-anthracite-shale"] = (long)20,
        ["game:ore-anthracite-chalk"] = (long)20,
        ["game:ore-anthracite-limestone"] = (long)20,
        ["game:ore-anthracite-chert"] = (long)20,
        ["game:ore-anthracite-conglomerate"] = (long)20,
        ["game:ore-sulfur-claystone"] = (long)10,
        ["game:ore-sulfur-sandstone"] = (long)10,
        ["game:ore-sulfur-shale"] = (long)10,
        ["game:ore-sulfur-chalk"] = (long)10,
        ["game:ore-sulfur-limestone"] = (long)10,
        ["game:ore-sulfur-chert"] = (long)10,
        ["game:ore-sulfur-conglomerate"] = (long)10,
        ["game:ore-sylvite-halite"] = (long)15,
        ["game:ore-borax-claystone"] = (long)10,
        ["game:ore-borax-sandstone"] = (long)10,
        ["game:ore-borax-shale"] = (long)10,
        ["game:ore-borax-chalk"] = (long)10,
        ["game:ore-borax-limestone"] = (long)10,
        ["game:ore-borax-chert"] = (long)10,
        ["game:ore-borax-conglomerate"] = (long)10,
        ["game:ore-kernite-claystone"] = (long)15,
        ["game:ore-kernite-sandstone"] = (long)15,
        ["game:ore-kernite-shale"] = (long)15,
        ["game:ore-kernite-chalk"] = (long)15,
        ["game:ore-kernite-limestone"] = (long)15,
        ["game:ore-kernite-chert"] = (long)15,
        ["game:ore-kernite-conglomerate"] = (long)15,
        ["game:ore-graphite-phyllite"] = (long)20,
        ["game:ore-graphite-slate"] = (long)20,
        ["game:ore-graphite-whitemarble"] = (long)20,
        ["game:ore-graphite-redmarble"] = (long)20,
        ["game:ore-graphite-greenmarble"] = (long)20,
        ["game:ore-cinnabar-andesite"] = (long)20,
        ["game:ore-cinnabar-granite"] = (long)20,
        ["game:ore-cinnabar-basalt"] = (long)20,
        ["game:ore-cinnabar-peridotite"] = (long)20,
        ["game:ore-cinnabar-phyllite"] = (long)20,
        ["game:ore-cinnabar-slate"] = (long)20,
        ["game:ore-corundum-peridotite"] = (long)25,
        ["game:ore-corundum-phyllite"] = (long)25,
        ["game:ore-corundum-slate"] = (long)25,
        ["game:ore-corundum-whitemarble"] = (long)25,
        ["game:ore-corundum-redmarble"] = (long)25,
        ["game:ore-corundum-greenmarble"] = (long)25,
        ["game:ore-lapislazuli-limestone"] = (long)25,
        ["game:ore-lapislazuli-bauxite"] = (long)25,
        ["game:ore-lapislazuli-whitemarble"] = (long)25,
        ["game:ore-lapislazuli-redmarble"] = (long)25,
        ["game:ore-lapislazuli-greenmarble"] = (long)25,
        ["game:ore-olivine-peridotite"] = (long)20,
        ["game:ore-fluorite-claystone"] = (long)15,
        ["game:ore-fluorite-sandstone"] = (long)15,
        ["game:ore-fluorite-shale"] = (long)15,
        ["game:ore-fluorite-chalk"] = (long)15,
        ["game:ore-fluorite-limestone"] = (long)15,
        ["game:ore-fluorite-chert"] = (long)15,
        ["game:ore-fluorite-conglomerate"] = (long)15,
        ["game:ore-fluorite-phyllite"] = (long)15,
        ["game:ore-fluorite-slate"] = (long)15,
        ["game:ore-phosphorite-claystone"] = (long)10,
        ["game:ore-phosphorite-sandstone"] = (long)10,
        ["game:ore-phosphorite-shale"] = (long)10,
        ["game:ore-phosphorite-chalk"] = (long)10,
        ["game:ore-phosphorite-limestone"] = (long)10,
        ["game:ore-phosphorite-chert"] = (long)10,
        ["game:ore-phosphorite-conglomerate"] = (long)10,
        ["game:ore-low-emerald-basalt"] = (long)50,
        ["game:ore-medium-emerald-basalt"] = (long)70,
        ["game:ore-low-emerald-peridotite"] = (long)50,
        ["game:ore-medium-emerald-peridotite"] = (long)70,
        ["game:ore-low-emerald-shale"] = (long)50,
        ["game:ore-medium-emerald-shale"] = (long)70,
        ["game:ore-high-emerald-shale"] = (long)90,
        ["game:ore-low-emerald-limestone"] = (long)50,
        ["game:ore-medium-emerald-limestone"] = (long)70,
        ["game:ore-high-emerald-limestone"] = (long)90,
        ["game:ore-low-emerald-phyllite"] = (long)50,
        ["game:ore-medium-emerald-phyllite"] = (long)70,
        ["game:ore-low-emerald-slate"] = (long)50,
        ["game:ore-medium-emerald-slate"] = (long)70,
        ["game:ore-low-diamond-kimberlite"] = (long)80,
        ["game:ore-medium-diamond-kimberlite"] = (long)100,
        ["game:ore-high-diamond-kimberlite"] = (long)120,
        ["game:ore-low-diamond-suevite"] = (long)80,
        ["game:ore-low-olivine_peridot-peridotite"] = (long)30,
        ["game:ore-medium-olivine_peridot-peridotite"] = (long)50,
        ["game:ore-high-olivine_peridot-peridotite"] = (long)70,
    };

    private static Dictionary<string, object> BuildPickaxeDefaultConfig() => new()
    {
        ["pickaxeEXPPerHit"] = (long)pickaxeEXPPerHit,
        ["pickaxeEXPPerBreaking"] = (long)pickaxeEXPPerBreaking,
        ["pickaxeEXPPerLevelBase"] = (long)pickaxeEXPPerLevelBase,
        ["pickaxeEXPMultiplyPerLevel"] = pickaxeEXPMultiplyPerLevel,
        ["pickaxeBaseDamage"] = (double)pickaxeBaseDamage,
        ["pickaxeIncrementDamagePerLevel"] = (double)pickaxeIncrementDamagePerLevel,
        ["pickaxeBaseMiningSpeed"] = (double)pickaxeBaseMiningSpeed,
        ["pickaxeIncrementMiningSpeedMultiplyPerLevel"] = (double)pickaxeIncrementMiningSpeedMultiplyPerLevel,
        ["pickaxeBaseOreMultiply"] = (double)pickaxeBaseOreMultiply,
        ["pickaxeIncrementOreMultiplyPerLevel"] = (double)pickaxeIncrementOreMultiplyPerLevel,
        ["pickaxeMaxLevel"] = (long)pickaxeMaxLevel,
    };

    public static void PopulatePickaxeConfiguration(ICoreAPI api)
    {
        Dictionary<string, object> pickaxeLevelStats = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/levelstats",
            "pickaxe",
            BuildPickaxeDefaultConfig());
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
            BuildPickaxeEntityExpDefaultConfig());
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
            BuildPickaxeOresDefaultConfig());
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
    private static float shovelIncrementDamagePerLevel = 0.03f;
    private static float shovelBaseMiningSpeed = 1.0f;
    private static float shovelIncrementMiningSpeedMultiplyPerLevel = 0.02f;
    public static int shovelMaxLevel = 999;


    public static int ExpPerHitShovel => shovelEXPPerHit;
    public static int ExpPerBreakingShovel => shovelEXPPerBreaking;

    private static Dictionary<string, object> BuildShovelDefaultConfig() => new()
    {
        ["shovelEXPPerHit"] = (long)shovelEXPPerHit,
        ["shovelEXPPerBreaking"] = (long)shovelEXPPerBreaking,
        ["shovelEXPPerLevelBase"] = (long)shovelEXPPerLevelBase,
        ["shovelEXPMultiplyPerLevel"] = shovelEXPMultiplyPerLevel,
        ["shovelBaseDamage"] = (double)shovelBaseDamage,
        ["shovelIncrementDamagePerLevel"] = (double)shovelIncrementDamagePerLevel,
        ["shovelBaseMiningSpeed"] = (double)shovelBaseMiningSpeed,
        ["shovelIncrementMiningSpeedMultiplyPerLevel"] = (double)shovelIncrementMiningSpeedMultiplyPerLevel,
        ["shovelMaxLevel"] = (long)shovelMaxLevel,
    };

    private static Dictionary<string, object> BuildShovelEntityExpDefaultConfig() => new()
    {
        ["game:sheep-bighorn-male"] = (long)50,
        ["game:sheep-bighorn-female"] = (long)50,
        ["game:sheep-bighorn-lamb"] = (long)20,
        ["game:chicken-rooster"] = (long)10,
        ["game:chicken-hen"] = (long)10,
        ["game:chicken-baby"] = (long)10,
        ["game:wolf-male"] = (long)40,
        ["game:wolf-female"] = (long)40,
        ["game:wolf-pup"] = (long)10,
        ["game:hyena-male"] = (long)40,
        ["game:hyena-female"] = (long)40,
        ["game:hyena-pup"] = (long)10,
        ["game:fox-male-red"] = (long)20,
        ["game:fox-female-red"] = (long)20,
        ["game:fox-pup"] = (long)10,
        ["game:fox-pup-red"] = (long)20,
        ["game:fox-pup-arctic"] = (long)20,
        ["game:fox-male-arctic"] = (long)10,
        ["game:fox-female-arctic"] = (long)10,
        ["game:raccoon-male"] = (long)20,
        ["game:raccoon-female"] = (long)20,
        ["game:raccoon-pup"] = (long)10,
        ["game:hare-male-arctic"] = (long)30,
        ["game:hare-male-ashgrey"] = (long)30,
        ["game:hare-male-darkbrown"] = (long)30,
        ["game:hare-male-desert"] = (long)30,
        ["game:hare-male-gold"] = (long)40,
        ["game:hare-male-lightbrown"] = (long)40,
        ["game:hare-male-lightgrey"] = (long)40,
        ["game:hare-male-silver"] = (long)40,
        ["game:hare-male-smokegrey"] = (long)50,
        ["game:hare-female-arctic"] = (long)60,
        ["game:hare-female-ashgrey"] = (long)60,
        ["game:hare-female-gold"] = (long)70,
        ["game:hare-female-lightbrown"] = (long)40,
        ["game:hare-female-lightgrey"] = (long)40,
        ["game:hare-female-silver"] = (long)40,
        ["game:hare-female-smokegrey"] = (long)30,
        ["game:hare-baby"] = (long)20,
        ["game:drifter-normal"] = (long)40,
        ["game:drifter-deep"] = (long)50,
        ["game:drifter-tainted"] = (long)60,
        ["game:drifter-corrupt"] = (long)70,
        ["game:drifter-nightmare"] = (long)80,
        ["game:drifter-double-headed"] = (long)90,
        ["game:locust-bronze"] = (long)60,
        ["game:locust-corrupt"] = (long)60,
        ["game:bell-normal"] = (long)100,
        ["game:bear-female-black"] = (long)50,
        ["game:bear-female-brown"] = (long)50,
        ["game:bear-female-sun"] = (long)50,
        ["game:bear-female-panda"] = (long)50,
        ["game:bear-female-polar"] = (long)50,
        ["game:bear-male-black"] = (long)50,
        ["game:bear-male-brown"] = (long)50,
        ["game:bear-male-sun"] = (long)50,
        ["game:bear-male-panda"] = (long)50,
        ["game:bear-male-polar"] = (long)50,
        ["game:locust-bronze-hacked"] = (long)60,
        ["game:locust-corrupt-hacked"] = (long)60,
        ["game:gazelle-male"] = (long)50,
        ["game:gazelle-female"] = (long)50,
        ["game:gazelle-calf"] = (long)30,
        ["game:deer-moose-male-adult"] = (long)30,
        ["game:deer-moose-female-adult"] = (long)50,
        ["game:deer-moose-male-baby"] = (long)50,
        ["game:deer-moose-female-baby"] = (long)30,
        ["game:deer-whitetail-male-adult"] = (long)30,
        ["game:deer-whitetail-female-adult"] = (long)30,
        ["game:deer-whitetail-male-baby"] = (long)30,
        ["game:deer-whitetail-female-baby"] = (long)10,
        ["game:deer-redbrocket-male-adult"] = (long)10,
        ["game:deer-chital-female-baby"] = (long)60,
        ["game:deer-guemal-male-adult"] = (long)60,
        ["game:deer-guemal-female-adult"] = (long)20,
        ["game:deer-guemal-male-baby"] = (long)20,
        ["game:deer-guemal-female-baby"] = (long)60,
        ["game:deer-pampas-male-adult"] = (long)60,
        ["game:deer-pampas-female-adult"] = (long)70,
        ["game:deer-pampas-male-baby"] = (long)70,
        ["game:deer-pampas-female-baby"] = (long)40,
        ["game:deer-pudu-male-adult"] = (long)40,
        ["game:deer-pudu-female-adult"] = (long)10,
        ["game:deer-pudu-male-baby"] = (long)10,
        ["game:deer-pudu-female-baby"] = (long)60,
        ["game:deer-elk-male-adult"] = (long)60,
        ["game:deer-elk-female-adult"] = (long)20,
        ["game:deer-elk-male-baby"] = (long)20,
        ["game:deer-elk-female-baby"] = (long)50,
        ["game:deer-taruca-male-adult"] = (long)50,
        ["game:deer-taruca-female-adult"] = (long)20,
        ["game:deer-taruca-male-baby"] = (long)20,
        ["game:deer-taruca-female-baby"] = (long)60,
        ["game:deer-chital-male-adult"] = (long)60,
        ["game:deer-chital-female-adult"] = (long)20,
        ["game:deer-chital-male-baby"] = (long)20,
        ["game:deer-fallow-female-baby"] = (long)60,
        ["game:deer-fallow-male-adult"] = (long)60,
        ["game:deer-fallow-male-baby"] = (long)20,
        ["game:deer-fallow-female-adult"] = (long)20,
        ["game:goat-angora-male-adult"] = (long)70,
        ["game:goat-angora-female-adult"] = (long)70,
        ["game:goat-angora-male-baby"] = (long)30,
        ["game:goat-angora-female-baby"] = (long)30,
        ["game:goat-ibexalp-male-adult"] = (long)70,
        ["game:goat-ibexalp-female-adult"] = (long)70,
        ["game:goat-ibexalp-male-baby"] = (long)30,
        ["game:goat-ibexalp-female-baby"] = (long)30,
        ["game:goat-ibexnub-male-adult"] = (long)50,
        ["game:goat-ibexnub-female-adult"] = (long)50,
        ["game:goat-ibexnub-male-baby"] = (long)20,
        ["game:goat-ibexnub-female-baby"] = (long)20,
        ["game:goat-markhor-male-adult"] = (long)60,
        ["game:goat-markhor-female-adult"] = (long)60,
        ["game:goat-markhor-male-baby"] = (long)20,
        ["game:goat-markhor-female-baby"] = (long)20,
        ["game:goat-mountain-male-adult"] = (long)40,
        ["game:goat-mountain-female-adult"] = (long)40,
        ["game:goat-mountain-male-baby"] = (long)20,
        ["game:goat-mountain-female-baby"] = (long)20,
        ["game:goat-muskox-male-adult"] = (long)40,
        ["game:goat-muskox-female-adult"] = (long)40,
        ["game:goat-muskox-male-baby"] = (long)20,
        ["game:goat-muskox-female-baby"] = (long)20,
        ["game:goat-nubian-male-adult"] = (long)40,
        ["game:goat-nubian-female-adult"] = (long)40,
        ["game:goat-nubian-male-baby"] = (long)20,
        ["game:goat-sirohi-male-adult"] = (long)40,
        ["game:goat-sirohi-female-adult"] = (long)40,
        ["game:goat-sirohi-male-baby"] = (long)20,
        ["game:goat-sirohi-female-baby"] = (long)20,
        ["game:goat-takingold-male-adult"] = (long)40,
        ["game:goat-takingold-female-adult"] = (long)40,
        ["game:goat-takingold-male-baby"] = (long)20,
        ["game:goat-takingold-female-baby"] = (long)20,
        ["game:goat-turdag-male-adult"] = (long)40,
        ["game:goat-turdag-female-adult"] = (long)40,
        ["game:goat-turdag-male-baby"] = (long)20,
        ["game:goat-turdag-female-baby"] = (long)20,
        ["game:goat-valais-male-adult"] = (long)40,
        ["game:goat-valais-female-adult"] = (long)40,
        ["game:goat-valais-male-baby"] = (long)20,
        ["game:goat-valais-female-baby"] = (long)20,
        ["game:pig-eurasian-adult-male"] = (long)30,
        ["game:pig-eurasian-adult-female"] = (long)30,
        ["game:pig-eurasian-elder-male"] = (long)40,
        ["game:pig-eurasian-elder-female"] = (long)40,
        ["game:pig-redriver-adult-male"] = (long)35,
        ["game:pig-redriver-adult-female"] = (long)35,
        ["game:pig-warthog-adult-male"] = (long)40,
        ["game:pig-warthog-adult-female"] = (long)40,
        ["game:pig-eurasian-baby-male"] = (long)10,
        ["game:pig-eurasian-baby-female"] = (long)10,
        ["game:pig-redriver-baby-male"] = (long)10,
        ["game:pig-redriver-baby-female"] = (long)10,
        ["game:pig-warthog-baby-male"] = (long)10,
        ["game:pig-warthog-baby-female"] = (long)10,
        ["game:sheep-mouflon-male"] = (long)50,
        ["game:sheep-mouflon-female"] = (long)50,
        ["game:sheep-mouflon-lamb"] = (long)20,
        ["game:shiver-surface"] = (long)50,
        ["game:shiver-deep"] = (long)60,
        ["game:shiver-tainted"] = (long)70,
        ["game:shiver-corrupt"] = (long)80,
        ["game:shiver-nightmare"] = (long)90,
        ["game:shiver-stilt"] = (long)60,
        ["game:shiver-bellhead"] = (long)80,
        ["game:shiver-deepsplit"] = (long)90,
        ["game:bowtorn-surface"] = (long)50,
        ["game:bowtorn-deep"] = (long)60,
        ["game:bowtorn-tainted"] = (long)70,
        ["game:bowtorn-corrupt"] = (long)80,
        ["game:bowtorn-nightmare"] = (long)90,
        ["game:bowtorn-gearfoot"] = (long)80,
        ["game:erel-pristine"] = (long)200,
        ["game:erel-corrupted"] = (long)250,
        ["game:eidolon-immobilized"] = (long)300,
        ["game:bellmini-normal"] = (long)100,
        ["game:locust-corrupt-sawblade"] = (long)60,
        ["game:chicken-henpoult"] = (long)10,
        ["game:chicken-roosterpoult"] = (long)10,
        ["game:deer-marsh-male-adult"] = (long)30,
        ["game:deer-marsh-female-adult"] = (long)30,
        ["game:deer-marsh-male-baby"] = (long)10,
        ["game:deer-marsh-female-baby"] = (long)10,
        ["game:deer-caribou-male-adult"] = (long)30,
        ["game:deer-caribou-female-adult"] = (long)30,
        ["game:deer-caribou-male-baby"] = (long)10,
        ["game:deer-caribou-female-baby"] = (long)10,
        ["game:deer-water-male-adult"] = (long)30,
        ["game:deer-water-female-adult"] = (long)30,
        ["game:deer-water-male-baby"] = (long)10,
        ["game:deer-water-female-baby"] = (long)10,
        ["game:deer-redbrocket-female-adult"] = (long)10,
        ["game:deer-redbrocket-male-baby"] = (long)10,
        ["game:deer-redbrocket-female-baby"] = (long)10,
        ["game:fish-freshwater-alewife-shad-adult"] = (long)5,
        ["game:fish-freshwater-chub-river-adult"] = (long)5,
        ["game:fish-freshwater-crappie-black-adult"] = (long)5,
        ["game:fish-freshwater-crappie-white-adult"] = (long)5,
        ["game:fish-freshwater-perch-european-adult"] = (long)5,
        ["game:fish-freshwater-perch-yellow-adult"] = (long)5,
        ["game:fish-freshwater-piranha-black-adult"] = (long)5,
        ["game:fish-freshwater-piranha-red-adult"] = (long)5,
        ["game:fish-freshwater-trout-brown-adult"] = (long)5,
        ["game:fish-freshwater-trout-rainbow-adult"] = (long)5,
        ["game:fish-freshwater-bass-largemouth-adult"] = (long)10,
        ["game:fish-freshwater-bass-smallmouth-adult"] = (long)10,
        ["game:fish-freshwater-carp-common-adult"] = (long)10,
        ["game:fish-freshwater-carp-grass-adult"] = (long)10,
        ["game:fish-freshwater-catfish-blue-adult"] = (long)10,
        ["game:fish-freshwater-catfish-channel-adult"] = (long)10,
        ["game:fish-freshwater-pickerel-chain-adult"] = (long)10,
        ["game:fish-freshwater-salmon-coho-adult"] = (long)10,
        ["game:fish-freshwater-tilapia-nile-adult"] = (long)10,
        ["game:fish-freshwater-tilapia-red-adult"] = (long)10,
        ["game:fish-freshwater-walleye-common-adult"] = (long)10,
        ["game:fish-freshwater-pike-northern-adult"] = (long)15,
        ["game:fish-freshwater-arapaima-arapaima-adult"] = (long)20,
        ["game:fish-freshwater-arapaima-gigas-adult"] = (long)20,
        ["game:fish-freshwater-sheatfish-black-adult"] = (long)20,
        ["game:fish-freshwater-sheatfish-white-adult"] = (long)20,
        ["game:fish-saltwater-bream-sea-adult"] = (long)5,
        ["game:fish-saltwater-gurnard-cape-adult"] = (long)5,
        ["game:fish-saltwater-haddock-common-adult"] = (long)5,
        ["game:fish-saltwater-hake-silver-adult"] = (long)5,
        ["game:fish-saltwater-herring-atlantic-adult"] = (long)5,
        ["game:fish-saltwater-mackerel-atlantic-adult"] = (long)5,
        ["game:fish-saltwater-pollock-alaska-adult"] = (long)5,
        ["game:fish-saltwater-perch-pacific-adult"] = (long)5,
        ["game:fish-saltwater-barracuda-great-adult"] = (long)10,
        ["game:fish-saltwater-grouper-black-adult"] = (long)10,
        ["game:fish-saltwater-salmon-pink-adult"] = (long)10,
        ["game:fish-saltwater-snapper-red-adult"] = (long)10,
        ["game:fish-saltwater-tuna-skipjack-adult"] = (long)10,
        ["game:fish-saltwater-wolf-bering-adult"] = (long)10,
        ["game:fish-saltwater-amberjack-yellowtail-adult"] = (long)15,
        ["game:fish-saltwater-mahi-mahi-common-adult"] = (long)15,
        ["game:fish-saltwater-wreckfish-atlantic-adult"] = (long)15,
        ["game:fish-saltwater-coelacanth-common-adult"] = (long)20,
        ["game:fish-saltwater-sturgeon-atlantic-adult"] = (long)20,
        ["game:fish-reef-angel-bicolor-adult"] = (long)5,
        ["game:fish-reef-butterfly-copperband-adult"] = (long)5,
        ["game:fish-reef-butterfly-blackwedged-adult"] = (long)5,
        ["game:fish-reef-clown-black-adult"] = (long)5,
        ["game:fish-reef-clown-common-adult"] = (long)5,
        ["game:fish-reef-clown-yellowstripe-adult"] = (long)5,
        ["game:fish-reef-puffer-longspine-adult"] = (long)5,
        ["game:fish-reef-tang-banded-adult"] = (long)5,
        ["game:fish-reef-tang-powderblue-adult"] = (long)5,
        ["game:fish-reef-trigger-titan-adult"] = (long)5,
        ["game:fish-reef-wrasse-creole-adult"] = (long)5,
    };

    public static void PopulateShovelConfiguration(ICoreAPI api)
    {
        Dictionary<string, object> shovelLevelStats = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/levelstats",
            "shovel",
            BuildShovelDefaultConfig());
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
            BuildShovelEntityExpDefaultConfig());
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
    private static float spearBaseRangedAccuracy = 0.0f;
    private static float spearIncrementRangedAccuracyPerLevel = 0.015f;
    private static float spearBaseRangedSpeed = 0.0f;
    private static float spearIncrementRangedSpeedPerLevel = 0.01f;
    private static float spearBaseMovePenaltyReduction = 0.0f;
    private static float spearIncrementMovePenaltyReductionPerLevel = 0.005f;
    public static int spearMaxLevel = 999;


    public static int ExpPerHitSpear => spearEXPPerHit;
    public static int ExpPerThrowSpear => spearEXPPerThrow;

    private static Dictionary<string, object> BuildSpearDefaultConfig() => new()
    {
        ["spearEXPPerHit"] = (long)spearEXPPerHit,
        ["spearEXPPerThrow"] = (long)spearEXPPerThrow,
        ["spearEXPPerLevelBase"] = (long)spearEXPPerLevelBase,
        ["spearEXPMultiplyPerLevel"] = spearEXPMultiplyPerLevel,
        ["spearBaseDamage"] = (double)spearBaseDamage,
        ["spearIncrementDamagePerLevel"] = (double)spearIncrementDamagePerLevel,
        ["spearBaseRangedAccuracy"] = (double)spearBaseRangedAccuracy,
        ["spearIncrementRangedAccuracyPerLevel"] = (double)spearIncrementRangedAccuracyPerLevel,
        ["spearBaseRangedSpeed"] = (double)spearBaseRangedSpeed,
        ["spearIncrementRangedSpeedPerLevel"] = (double)spearIncrementRangedSpeedPerLevel,
        ["spearBaseMovePenaltyReduction"] = (double)spearBaseMovePenaltyReduction,
        ["spearIncrementMovePenaltyReductionPerLevel"] = (double)spearIncrementMovePenaltyReductionPerLevel,
        ["spearMaxLevel"] = (long)spearMaxLevel,
    };

    private static Dictionary<string, object> BuildSpearEntityExpDefaultConfig() => new()
    {
        ["game:sheep-bighorn-male"] = (long)50,
        ["game:sheep-bighorn-female"] = (long)50,
        ["game:sheep-bighorn-lamb"] = (long)20,
        ["game:chicken-rooster"] = (long)10,
        ["game:chicken-hen"] = (long)10,
        ["game:chicken-baby"] = (long)10,
        ["game:wolf-male"] = (long)40,
        ["game:wolf-female"] = (long)40,
        ["game:wolf-pup"] = (long)10,
        ["game:hyena-male"] = (long)40,
        ["game:hyena-female"] = (long)40,
        ["game:hyena-pup"] = (long)10,
        ["game:fox-male-red"] = (long)20,
        ["game:fox-female-red"] = (long)20,
        ["game:fox-pup"] = (long)10,
        ["game:fox-pup-red"] = (long)20,
        ["game:fox-pup-arctic"] = (long)20,
        ["game:fox-male-arctic"] = (long)10,
        ["game:fox-female-arctic"] = (long)10,
        ["game:raccoon-male"] = (long)20,
        ["game:raccoon-female"] = (long)20,
        ["game:raccoon-pup"] = (long)10,
        ["game:hare-male-arctic"] = (long)30,
        ["game:hare-male-ashgrey"] = (long)30,
        ["game:hare-male-darkbrown"] = (long)30,
        ["game:hare-male-desert"] = (long)30,
        ["game:hare-male-gold"] = (long)40,
        ["game:hare-male-lightbrown"] = (long)40,
        ["game:hare-male-lightgrey"] = (long)40,
        ["game:hare-male-silver"] = (long)40,
        ["game:hare-male-smokegrey"] = (long)50,
        ["game:hare-female-arctic"] = (long)60,
        ["game:hare-female-ashgrey"] = (long)60,
        ["game:hare-female-gold"] = (long)70,
        ["game:hare-female-lightbrown"] = (long)40,
        ["game:hare-female-lightgrey"] = (long)40,
        ["game:hare-female-silver"] = (long)40,
        ["game:hare-female-smokegrey"] = (long)30,
        ["game:hare-baby"] = (long)20,
        ["game:drifter-normal"] = (long)40,
        ["game:drifter-deep"] = (long)50,
        ["game:drifter-tainted"] = (long)60,
        ["game:drifter-corrupt"] = (long)70,
        ["game:drifter-nightmare"] = (long)80,
        ["game:drifter-double-headed"] = (long)90,
        ["game:locust-bronze"] = (long)60,
        ["game:locust-corrupt"] = (long)60,
        ["game:bell-normal"] = (long)100,
        ["game:bear-female-black"] = (long)50,
        ["game:bear-female-brown"] = (long)50,
        ["game:bear-female-sun"] = (long)50,
        ["game:bear-female-panda"] = (long)50,
        ["game:bear-female-polar"] = (long)50,
        ["game:bear-male-black"] = (long)50,
        ["game:bear-male-brown"] = (long)50,
        ["game:bear-male-sun"] = (long)50,
        ["game:bear-male-panda"] = (long)50,
        ["game:bear-male-polar"] = (long)50,
        ["game:locust-bronze-hacked"] = (long)60,
        ["game:locust-corrupt-hacked"] = (long)60,
        ["game:gazelle-male"] = (long)50,
        ["game:gazelle-female"] = (long)50,
        ["game:gazelle-calf"] = (long)30,
        ["game:deer-moose-male-adult"] = (long)30,
        ["game:deer-moose-female-adult"] = (long)50,
        ["game:deer-moose-male-baby"] = (long)50,
        ["game:deer-moose-female-baby"] = (long)30,
        ["game:deer-whitetail-male-adult"] = (long)30,
        ["game:deer-whitetail-female-adult"] = (long)30,
        ["game:deer-whitetail-male-baby"] = (long)30,
        ["game:deer-whitetail-female-baby"] = (long)10,
        ["game:deer-redbrocket-male-adult"] = (long)10,
        ["game:deer-chital-female-baby"] = (long)60,
        ["game:deer-guemal-male-adult"] = (long)60,
        ["game:deer-guemal-female-adult"] = (long)20,
        ["game:deer-guemal-male-baby"] = (long)20,
        ["game:deer-guemal-female-baby"] = (long)60,
        ["game:deer-pampas-male-adult"] = (long)60,
        ["game:deer-pampas-female-adult"] = (long)70,
        ["game:deer-pampas-male-baby"] = (long)70,
        ["game:deer-pampas-female-baby"] = (long)40,
        ["game:deer-pudu-male-adult"] = (long)40,
        ["game:deer-pudu-female-adult"] = (long)10,
        ["game:deer-pudu-male-baby"] = (long)10,
        ["game:deer-pudu-female-baby"] = (long)60,
        ["game:deer-elk-male-adult"] = (long)60,
        ["game:deer-elk-female-adult"] = (long)20,
        ["game:deer-elk-male-baby"] = (long)20,
        ["game:deer-elk-female-baby"] = (long)50,
        ["game:deer-taruca-male-adult"] = (long)50,
        ["game:deer-taruca-female-adult"] = (long)20,
        ["game:deer-taruca-male-baby"] = (long)20,
        ["game:deer-taruca-female-baby"] = (long)60,
        ["game:deer-chital-male-adult"] = (long)60,
        ["game:deer-chital-female-adult"] = (long)20,
        ["game:deer-chital-male-baby"] = (long)20,
        ["game:deer-fallow-female-baby"] = (long)60,
        ["game:deer-fallow-male-adult"] = (long)60,
        ["game:deer-fallow-male-baby"] = (long)20,
        ["game:deer-fallow-female-adult"] = (long)20,
        ["game:goat-angora-male-adult"] = (long)70,
        ["game:goat-angora-female-adult"] = (long)70,
        ["game:goat-angora-male-baby"] = (long)30,
        ["game:goat-angora-female-baby"] = (long)30,
        ["game:goat-ibexalp-male-adult"] = (long)70,
        ["game:goat-ibexalp-female-adult"] = (long)70,
        ["game:goat-ibexalp-male-baby"] = (long)30,
        ["game:goat-ibexalp-female-baby"] = (long)30,
        ["game:goat-ibexnub-male-adult"] = (long)50,
        ["game:goat-ibexnub-female-adult"] = (long)50,
        ["game:goat-ibexnub-male-baby"] = (long)20,
        ["game:goat-ibexnub-female-baby"] = (long)20,
        ["game:goat-markhor-male-adult"] = (long)60,
        ["game:goat-markhor-female-adult"] = (long)60,
        ["game:goat-markhor-male-baby"] = (long)20,
        ["game:goat-markhor-female-baby"] = (long)20,
        ["game:goat-mountain-male-adult"] = (long)40,
        ["game:goat-mountain-female-adult"] = (long)40,
        ["game:goat-mountain-male-baby"] = (long)20,
        ["game:goat-mountain-female-baby"] = (long)20,
        ["game:goat-muskox-male-adult"] = (long)40,
        ["game:goat-muskox-female-adult"] = (long)40,
        ["game:goat-muskox-male-baby"] = (long)20,
        ["game:goat-muskox-female-baby"] = (long)20,
        ["game:goat-nubian-male-adult"] = (long)40,
        ["game:goat-nubian-female-adult"] = (long)40,
        ["game:goat-nubian-male-baby"] = (long)20,
        ["game:goat-sirohi-male-adult"] = (long)40,
        ["game:goat-sirohi-female-adult"] = (long)40,
        ["game:goat-sirohi-male-baby"] = (long)20,
        ["game:goat-sirohi-female-baby"] = (long)20,
        ["game:goat-takingold-male-adult"] = (long)40,
        ["game:goat-takingold-female-adult"] = (long)40,
        ["game:goat-takingold-male-baby"] = (long)20,
        ["game:goat-takingold-female-baby"] = (long)20,
        ["game:goat-turdag-male-adult"] = (long)40,
        ["game:goat-turdag-female-adult"] = (long)40,
        ["game:goat-turdag-male-baby"] = (long)20,
        ["game:goat-turdag-female-baby"] = (long)20,
        ["game:goat-valais-male-adult"] = (long)40,
        ["game:goat-valais-female-adult"] = (long)40,
        ["game:goat-valais-male-baby"] = (long)20,
        ["game:goat-valais-female-baby"] = (long)20,
        ["game:pig-eurasian-adult-male"] = (long)30,
        ["game:pig-eurasian-adult-female"] = (long)30,
        ["game:pig-eurasian-elder-male"] = (long)40,
        ["game:pig-eurasian-elder-female"] = (long)40,
        ["game:pig-redriver-adult-male"] = (long)35,
        ["game:pig-redriver-adult-female"] = (long)35,
        ["game:pig-warthog-adult-male"] = (long)40,
        ["game:pig-warthog-adult-female"] = (long)40,
        ["game:pig-eurasian-baby-male"] = (long)10,
        ["game:pig-eurasian-baby-female"] = (long)10,
        ["game:pig-redriver-baby-male"] = (long)10,
        ["game:pig-redriver-baby-female"] = (long)10,
        ["game:pig-warthog-baby-male"] = (long)10,
        ["game:pig-warthog-baby-female"] = (long)10,
        ["game:sheep-mouflon-male"] = (long)50,
        ["game:sheep-mouflon-female"] = (long)50,
        ["game:sheep-mouflon-lamb"] = (long)20,
        ["game:shiver-surface"] = (long)50,
        ["game:shiver-deep"] = (long)60,
        ["game:shiver-tainted"] = (long)70,
        ["game:shiver-corrupt"] = (long)80,
        ["game:shiver-nightmare"] = (long)90,
        ["game:shiver-stilt"] = (long)60,
        ["game:shiver-bellhead"] = (long)80,
        ["game:shiver-deepsplit"] = (long)90,
        ["game:bowtorn-surface"] = (long)50,
        ["game:bowtorn-deep"] = (long)60,
        ["game:bowtorn-tainted"] = (long)70,
        ["game:bowtorn-corrupt"] = (long)80,
        ["game:bowtorn-nightmare"] = (long)90,
        ["game:bowtorn-gearfoot"] = (long)80,
        ["game:erel-pristine"] = (long)200,
        ["game:erel-corrupted"] = (long)250,
        ["game:eidolon-immobilized"] = (long)300,
        ["game:bellmini-normal"] = (long)100,
        ["game:locust-corrupt-sawblade"] = (long)60,
        ["game:chicken-henpoult"] = (long)10,
        ["game:chicken-roosterpoult"] = (long)10,
        ["game:deer-marsh-male-adult"] = (long)30,
        ["game:deer-marsh-female-adult"] = (long)30,
        ["game:deer-marsh-male-baby"] = (long)10,
        ["game:deer-marsh-female-baby"] = (long)10,
        ["game:deer-caribou-male-adult"] = (long)30,
        ["game:deer-caribou-female-adult"] = (long)30,
        ["game:deer-caribou-male-baby"] = (long)10,
        ["game:deer-caribou-female-baby"] = (long)10,
        ["game:deer-water-male-adult"] = (long)30,
        ["game:deer-water-female-adult"] = (long)30,
        ["game:deer-water-male-baby"] = (long)10,
        ["game:deer-water-female-baby"] = (long)10,
        ["game:deer-redbrocket-female-adult"] = (long)10,
        ["game:deer-redbrocket-male-baby"] = (long)10,
        ["game:deer-redbrocket-female-baby"] = (long)10,
        ["game:fish-freshwater-alewife-shad-adult"] = (long)5,
        ["game:fish-freshwater-chub-river-adult"] = (long)5,
        ["game:fish-freshwater-crappie-black-adult"] = (long)5,
        ["game:fish-freshwater-crappie-white-adult"] = (long)5,
        ["game:fish-freshwater-perch-european-adult"] = (long)5,
        ["game:fish-freshwater-perch-yellow-adult"] = (long)5,
        ["game:fish-freshwater-piranha-black-adult"] = (long)5,
        ["game:fish-freshwater-piranha-red-adult"] = (long)5,
        ["game:fish-freshwater-trout-brown-adult"] = (long)5,
        ["game:fish-freshwater-trout-rainbow-adult"] = (long)5,
        ["game:fish-freshwater-bass-largemouth-adult"] = (long)10,
        ["game:fish-freshwater-bass-smallmouth-adult"] = (long)10,
        ["game:fish-freshwater-carp-common-adult"] = (long)10,
        ["game:fish-freshwater-carp-grass-adult"] = (long)10,
        ["game:fish-freshwater-catfish-blue-adult"] = (long)10,
        ["game:fish-freshwater-catfish-channel-adult"] = (long)10,
        ["game:fish-freshwater-pickerel-chain-adult"] = (long)10,
        ["game:fish-freshwater-salmon-coho-adult"] = (long)10,
        ["game:fish-freshwater-tilapia-nile-adult"] = (long)10,
        ["game:fish-freshwater-tilapia-red-adult"] = (long)10,
        ["game:fish-freshwater-walleye-common-adult"] = (long)10,
        ["game:fish-freshwater-pike-northern-adult"] = (long)15,
        ["game:fish-freshwater-arapaima-arapaima-adult"] = (long)20,
        ["game:fish-freshwater-arapaima-gigas-adult"] = (long)20,
        ["game:fish-freshwater-sheatfish-black-adult"] = (long)20,
        ["game:fish-freshwater-sheatfish-white-adult"] = (long)20,
        ["game:fish-saltwater-bream-sea-adult"] = (long)5,
        ["game:fish-saltwater-gurnard-cape-adult"] = (long)5,
        ["game:fish-saltwater-haddock-common-adult"] = (long)5,
        ["game:fish-saltwater-hake-silver-adult"] = (long)5,
        ["game:fish-saltwater-herring-atlantic-adult"] = (long)5,
        ["game:fish-saltwater-mackerel-atlantic-adult"] = (long)5,
        ["game:fish-saltwater-pollock-alaska-adult"] = (long)5,
        ["game:fish-saltwater-perch-pacific-adult"] = (long)5,
        ["game:fish-saltwater-barracuda-great-adult"] = (long)10,
        ["game:fish-saltwater-grouper-black-adult"] = (long)10,
        ["game:fish-saltwater-salmon-pink-adult"] = (long)10,
        ["game:fish-saltwater-snapper-red-adult"] = (long)10,
        ["game:fish-saltwater-tuna-skipjack-adult"] = (long)10,
        ["game:fish-saltwater-wolf-bering-adult"] = (long)10,
        ["game:fish-saltwater-amberjack-yellowtail-adult"] = (long)15,
        ["game:fish-saltwater-mahi-mahi-common-adult"] = (long)15,
        ["game:fish-saltwater-wreckfish-atlantic-adult"] = (long)15,
        ["game:fish-saltwater-coelacanth-common-adult"] = (long)20,
        ["game:fish-saltwater-sturgeon-atlantic-adult"] = (long)20,
        ["game:fish-reef-angel-bicolor-adult"] = (long)5,
        ["game:fish-reef-butterfly-copperband-adult"] = (long)5,
        ["game:fish-reef-butterfly-blackwedged-adult"] = (long)5,
        ["game:fish-reef-clown-black-adult"] = (long)5,
        ["game:fish-reef-clown-common-adult"] = (long)5,
        ["game:fish-reef-clown-yellowstripe-adult"] = (long)5,
        ["game:fish-reef-puffer-longspine-adult"] = (long)5,
        ["game:fish-reef-tang-banded-adult"] = (long)5,
        ["game:fish-reef-tang-powderblue-adult"] = (long)5,
        ["game:fish-reef-trigger-titan-adult"] = (long)5,
        ["game:fish-reef-wrasse-creole-adult"] = (long)5,
    };

    public static void PopulateSpearConfiguration(ICoreAPI api)
    {
        Dictionary<string, object> spearLevelStats = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/levelstats",
            "spear",
            BuildSpearDefaultConfig());
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
        { //spearBaseRangedAccuracy
            if (spearLevelStats.TryGetValue("spearBaseRangedAccuracy", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: spearBaseRangedAccuracy is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: spearBaseRangedAccuracy is not double is {value.GetType()}");
                else spearBaseRangedAccuracy = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: spearBaseRangedAccuracy not set");
        }
        { //spearIncrementRangedAccuracyPerLevel
            if (spearLevelStats.TryGetValue("spearIncrementRangedAccuracyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: spearIncrementRangedAccuracyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: spearIncrementRangedAccuracyPerLevel is not double is {value.GetType()}");
                else spearIncrementRangedAccuracyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: spearIncrementRangedAccuracyPerLevel not set");
        }
        { //spearBaseRangedSpeed
            if (spearLevelStats.TryGetValue("spearBaseRangedSpeed", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: spearBaseRangedSpeed is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: spearBaseRangedSpeed is not double is {value.GetType()}");
                else spearBaseRangedSpeed = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: spearBaseRangedSpeed not set");
        }
        { //spearIncrementRangedSpeedPerLevel
            if (spearLevelStats.TryGetValue("spearIncrementRangedSpeedPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: spearIncrementRangedSpeedPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: spearIncrementRangedSpeedPerLevel is not double is {value.GetType()}");
                else spearIncrementRangedSpeedPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: spearIncrementRangedSpeedPerLevel not set");
        }
        { //spearBaseMovePenaltyReduction
            if (spearLevelStats.TryGetValue("spearBaseMovePenaltyReduction", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: spearBaseMovePenaltyReduction is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: spearBaseMovePenaltyReduction is not double is {value.GetType()}");
                else spearBaseMovePenaltyReduction = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: spearBaseMovePenaltyReduction not set");
        }
        { //spearIncrementMovePenaltyReductionPerLevel
            if (spearLevelStats.TryGetValue("spearIncrementMovePenaltyReductionPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: spearIncrementMovePenaltyReductionPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: spearIncrementMovePenaltyReductionPerLevel is not double is {value.GetType()}");
                else spearIncrementMovePenaltyReductionPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: spearIncrementMovePenaltyReductionPerLevel not set");
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
            BuildSpearEntityExpDefaultConfig());
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

    public static float SpearGetRangedAccuracyBonusByLevel(int level)
    {
        return spearBaseRangedAccuracy + spearIncrementRangedAccuracyPerLevel * level;
    }

    public static float SpearGetRangedSpeedBonusByLevel(int level)
    {
        return spearBaseRangedSpeed + spearIncrementRangedSpeedPerLevel * level;
    }

    public static float SpearGetMovePenaltyReductionByLevel(int level)
    {
        return spearBaseMovePenaltyReduction + spearIncrementMovePenaltyReductionPerLevel * level;
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

    private static Dictionary<string, object> BuildHammerDefaultConfig() => new()
    {
        ["hammerEXPPerHit"] = (long)hammerEXPPerHit,
        ["hammerEXPPerLevelBase"] = (long)hammerEXPPerLevelBase,
        ["hammerEXPMultiplyPerLevel"] = hammerEXPMultiplyPerLevel,
        ["hammerBaseDamage"] = (double)hammerBaseDamage,
        ["hammerIncrementDamagePerLevel"] = (double)hammerIncrementDamagePerLevel,
        ["hammerBaseSmithRetrieveChance"] = (double)hammerBaseSmithRetrieveChance,
        ["hammerSmithRetrieveChancePerLevel"] = (double)hammerSmithRetrieveChancePerLevel,
        ["hammerSmithRetrieveEveryLevelReduceChance"] = (long)hammerSmithRetrieveEveryLevelReduceChance,
        ["hammerSmithRetrieveReduceChanceForEveryLevel"] = (double)hammerSmithRetrieveReduceChanceForEveryLevel,
        ["hammerBaseChanceToDouble"] = (double)hammerBaseChanceToDouble,
        ["hammerIncreaseChanceToDoublePerLevel"] = (double)hammerIncreaseChanceToDoublePerLevel,
        ["hammerIncreaseChanceToDoublePerLevelReducerPerLevel"] = (long)hammerIncreaseChanceToDoublePerLevelReducerPerLevel,
        ["hammerIncreaseChanceToDoublePerLevelReducer"] = (double)hammerIncreaseChanceToDoublePerLevelReducer,
        ["hammerBaseChanceToTriple"] = (double)hammerBaseChanceToTriple,
        ["hammerIncreaseChanceToTriplePerLevel"] = (double)hammerIncreaseChanceToTriplePerLevel,
        ["hammerIncreaseChanceToTriplePerLevelReducerPerLevel"] = (long)hammerIncreaseChanceToTriplePerLevelReducerPerLevel,
        ["hammerIncreaseChanceToTriplePerLevelReducer"] = (double)hammerIncreaseChanceToTriplePerLevelReducer,
        ["hammerBaseChanceToQuadruple"] = (double)hammerBaseChanceToQuadruple,
        ["hammerIncreaseChanceToQuadruplePerLevel"] = (double)hammerIncreaseChanceToQuadruplePerLevel,
        ["hammerIncreaseChanceToQuadruplePerLevelReducerPerLevel"] = (long)hammerIncreaseChanceToQuadruplePerLevelReducerPerLevel,
        ["hammerIncreaseChanceToQuadruplePerLevelReducer"] = (double)hammerIncreaseChanceToQuadruplePerLevelReducer,
        ["hammerMaxLevel"] = (long)hammerMaxLevel,
    };

    private static Dictionary<string, object> BuildHammerEntityExpDefaultConfig() => new()
    {
        ["game:sheep-bighorn-male"] = (long)50,
        ["game:sheep-bighorn-female"] = (long)50,
        ["game:sheep-bighorn-lamb"] = (long)20,
        ["game:chicken-rooster"] = (long)10,
        ["game:chicken-hen"] = (long)10,
        ["game:chicken-baby"] = (long)10,
        ["game:wolf-male"] = (long)40,
        ["game:wolf-female"] = (long)40,
        ["game:wolf-pup"] = (long)10,
        ["game:hyena-male"] = (long)40,
        ["game:hyena-female"] = (long)40,
        ["game:hyena-pup"] = (long)10,
        ["game:fox-male-red"] = (long)20,
        ["game:fox-female-red"] = (long)20,
        ["game:fox-pup"] = (long)10,
        ["game:fox-pup-red"] = (long)20,
        ["game:fox-pup-arctic"] = (long)20,
        ["game:fox-male-arctic"] = (long)10,
        ["game:fox-female-arctic"] = (long)10,
        ["game:raccoon-male"] = (long)20,
        ["game:raccoon-female"] = (long)20,
        ["game:raccoon-pup"] = (long)10,
        ["game:hare-male-arctic"] = (long)30,
        ["game:hare-male-ashgrey"] = (long)30,
        ["game:hare-male-darkbrown"] = (long)30,
        ["game:hare-male-desert"] = (long)30,
        ["game:hare-male-gold"] = (long)40,
        ["game:hare-male-lightbrown"] = (long)40,
        ["game:hare-male-lightgrey"] = (long)40,
        ["game:hare-male-silver"] = (long)40,
        ["game:hare-male-smokegrey"] = (long)50,
        ["game:hare-female-arctic"] = (long)60,
        ["game:hare-female-ashgrey"] = (long)60,
        ["game:hare-female-gold"] = (long)70,
        ["game:hare-female-lightbrown"] = (long)40,
        ["game:hare-female-lightgrey"] = (long)40,
        ["game:hare-female-silver"] = (long)40,
        ["game:hare-female-smokegrey"] = (long)30,
        ["game:hare-baby"] = (long)20,
        ["game:drifter-normal"] = (long)40,
        ["game:drifter-deep"] = (long)50,
        ["game:drifter-tainted"] = (long)60,
        ["game:drifter-corrupt"] = (long)70,
        ["game:drifter-nightmare"] = (long)80,
        ["game:drifter-double-headed"] = (long)90,
        ["game:locust-bronze"] = (long)60,
        ["game:locust-corrupt"] = (long)60,
        ["game:bell-normal"] = (long)100,
        ["game:bear-female-black"] = (long)50,
        ["game:bear-female-brown"] = (long)50,
        ["game:bear-female-sun"] = (long)50,
        ["game:bear-female-panda"] = (long)50,
        ["game:bear-female-polar"] = (long)50,
        ["game:bear-male-black"] = (long)50,
        ["game:bear-male-brown"] = (long)50,
        ["game:bear-male-sun"] = (long)50,
        ["game:bear-male-panda"] = (long)50,
        ["game:bear-male-polar"] = (long)50,
        ["game:locust-bronze-hacked"] = (long)60,
        ["game:locust-corrupt-hacked"] = (long)60,
        ["game:gazelle-male"] = (long)50,
        ["game:gazelle-female"] = (long)50,
        ["game:gazelle-calf"] = (long)30,
        ["game:deer-moose-male-adult"] = (long)30,
        ["game:deer-moose-female-adult"] = (long)50,
        ["game:deer-moose-male-baby"] = (long)50,
        ["game:deer-moose-female-baby"] = (long)30,
        ["game:deer-whitetail-male-adult"] = (long)30,
        ["game:deer-whitetail-female-adult"] = (long)30,
        ["game:deer-whitetail-male-baby"] = (long)30,
        ["game:deer-whitetail-female-baby"] = (long)10,
        ["game:deer-redbrocket-male-adult"] = (long)10,
        ["game:deer-chital-female-baby"] = (long)60,
        ["game:deer-guemal-male-adult"] = (long)60,
        ["game:deer-guemal-female-adult"] = (long)20,
        ["game:deer-guemal-male-baby"] = (long)20,
        ["game:deer-guemal-female-baby"] = (long)60,
        ["game:deer-pampas-male-adult"] = (long)60,
        ["game:deer-pampas-female-adult"] = (long)70,
        ["game:deer-pampas-male-baby"] = (long)70,
        ["game:deer-pampas-female-baby"] = (long)40,
        ["game:deer-pudu-male-adult"] = (long)40,
        ["game:deer-pudu-female-adult"] = (long)10,
        ["game:deer-pudu-male-baby"] = (long)10,
        ["game:deer-pudu-female-baby"] = (long)60,
        ["game:deer-elk-male-adult"] = (long)60,
        ["game:deer-elk-female-adult"] = (long)20,
        ["game:deer-elk-male-baby"] = (long)20,
        ["game:deer-elk-female-baby"] = (long)50,
        ["game:deer-taruca-male-adult"] = (long)50,
        ["game:deer-taruca-female-adult"] = (long)20,
        ["game:deer-taruca-male-baby"] = (long)20,
        ["game:deer-taruca-female-baby"] = (long)60,
        ["game:deer-chital-male-adult"] = (long)60,
        ["game:deer-chital-female-adult"] = (long)20,
        ["game:deer-chital-male-baby"] = (long)20,
        ["game:deer-fallow-female-baby"] = (long)60,
        ["game:deer-fallow-male-adult"] = (long)60,
        ["game:deer-fallow-male-baby"] = (long)20,
        ["game:deer-fallow-female-adult"] = (long)20,
        ["game:goat-angora-male-adult"] = (long)70,
        ["game:goat-angora-female-adult"] = (long)70,
        ["game:goat-angora-male-baby"] = (long)30,
        ["game:goat-angora-female-baby"] = (long)30,
        ["game:goat-ibexalp-male-adult"] = (long)70,
        ["game:goat-ibexalp-female-adult"] = (long)70,
        ["game:goat-ibexalp-male-baby"] = (long)30,
        ["game:goat-ibexalp-female-baby"] = (long)30,
        ["game:goat-ibexnub-male-adult"] = (long)50,
        ["game:goat-ibexnub-female-adult"] = (long)50,
        ["game:goat-ibexnub-male-baby"] = (long)20,
        ["game:goat-ibexnub-female-baby"] = (long)20,
        ["game:goat-markhor-male-adult"] = (long)60,
        ["game:goat-markhor-female-adult"] = (long)60,
        ["game:goat-markhor-male-baby"] = (long)20,
        ["game:goat-markhor-female-baby"] = (long)20,
        ["game:goat-mountain-male-adult"] = (long)40,
        ["game:goat-mountain-female-adult"] = (long)40,
        ["game:goat-mountain-male-baby"] = (long)20,
        ["game:goat-mountain-female-baby"] = (long)20,
        ["game:goat-muskox-male-adult"] = (long)40,
        ["game:goat-muskox-female-adult"] = (long)40,
        ["game:goat-muskox-male-baby"] = (long)20,
        ["game:goat-muskox-female-baby"] = (long)20,
        ["game:goat-nubian-male-adult"] = (long)40,
        ["game:goat-nubian-female-adult"] = (long)40,
        ["game:goat-nubian-male-baby"] = (long)20,
        ["game:goat-sirohi-male-adult"] = (long)40,
        ["game:goat-sirohi-female-adult"] = (long)40,
        ["game:goat-sirohi-male-baby"] = (long)20,
        ["game:goat-sirohi-female-baby"] = (long)20,
        ["game:goat-takingold-male-adult"] = (long)40,
        ["game:goat-takingold-female-adult"] = (long)40,
        ["game:goat-takingold-male-baby"] = (long)20,
        ["game:goat-takingold-female-baby"] = (long)20,
        ["game:goat-turdag-male-adult"] = (long)40,
        ["game:goat-turdag-female-adult"] = (long)40,
        ["game:goat-turdag-male-baby"] = (long)20,
        ["game:goat-turdag-female-baby"] = (long)20,
        ["game:goat-valais-male-adult"] = (long)40,
        ["game:goat-valais-female-adult"] = (long)40,
        ["game:goat-valais-male-baby"] = (long)20,
        ["game:goat-valais-female-baby"] = (long)20,
        ["game:pig-eurasian-adult-male"] = (long)30,
        ["game:pig-eurasian-adult-female"] = (long)30,
        ["game:pig-eurasian-elder-male"] = (long)40,
        ["game:pig-eurasian-elder-female"] = (long)40,
        ["game:pig-redriver-adult-male"] = (long)35,
        ["game:pig-redriver-adult-female"] = (long)35,
        ["game:pig-warthog-adult-male"] = (long)40,
        ["game:pig-warthog-adult-female"] = (long)40,
        ["game:pig-eurasian-baby-male"] = (long)10,
        ["game:pig-eurasian-baby-female"] = (long)10,
        ["game:pig-redriver-baby-male"] = (long)10,
        ["game:pig-redriver-baby-female"] = (long)10,
        ["game:pig-warthog-baby-male"] = (long)10,
        ["game:pig-warthog-baby-female"] = (long)10,
        ["game:sheep-mouflon-male"] = (long)50,
        ["game:sheep-mouflon-female"] = (long)50,
        ["game:sheep-mouflon-lamb"] = (long)20,
        ["game:shiver-surface"] = (long)50,
        ["game:shiver-deep"] = (long)60,
        ["game:shiver-tainted"] = (long)70,
        ["game:shiver-corrupt"] = (long)80,
        ["game:shiver-nightmare"] = (long)90,
        ["game:shiver-stilt"] = (long)60,
        ["game:shiver-bellhead"] = (long)80,
        ["game:shiver-deepsplit"] = (long)90,
        ["game:bowtorn-surface"] = (long)50,
        ["game:bowtorn-deep"] = (long)60,
        ["game:bowtorn-tainted"] = (long)70,
        ["game:bowtorn-corrupt"] = (long)80,
        ["game:bowtorn-nightmare"] = (long)90,
        ["game:bowtorn-gearfoot"] = (long)80,
        ["game:erel-pristine"] = (long)200,
        ["game:erel-corrupted"] = (long)250,
        ["game:eidolon-immobilized"] = (long)300,
        ["game:bellmini-normal"] = (long)100,
        ["game:locust-corrupt-sawblade"] = (long)60,
        ["game:chicken-henpoult"] = (long)10,
        ["game:chicken-roosterpoult"] = (long)10,
        ["game:deer-marsh-male-adult"] = (long)30,
        ["game:deer-marsh-female-adult"] = (long)30,
        ["game:deer-marsh-male-baby"] = (long)10,
        ["game:deer-marsh-female-baby"] = (long)10,
        ["game:deer-caribou-male-adult"] = (long)30,
        ["game:deer-caribou-female-adult"] = (long)30,
        ["game:deer-caribou-male-baby"] = (long)10,
        ["game:deer-caribou-female-baby"] = (long)10,
        ["game:deer-water-male-adult"] = (long)30,
        ["game:deer-water-female-adult"] = (long)30,
        ["game:deer-water-male-baby"] = (long)10,
        ["game:deer-water-female-baby"] = (long)10,
        ["game:deer-redbrocket-female-adult"] = (long)10,
        ["game:deer-redbrocket-male-baby"] = (long)10,
        ["game:deer-redbrocket-female-baby"] = (long)10,
        ["game:fish-freshwater-alewife-shad-adult"] = (long)5,
        ["game:fish-freshwater-chub-river-adult"] = (long)5,
        ["game:fish-freshwater-crappie-black-adult"] = (long)5,
        ["game:fish-freshwater-crappie-white-adult"] = (long)5,
        ["game:fish-freshwater-perch-european-adult"] = (long)5,
        ["game:fish-freshwater-perch-yellow-adult"] = (long)5,
        ["game:fish-freshwater-piranha-black-adult"] = (long)5,
        ["game:fish-freshwater-piranha-red-adult"] = (long)5,
        ["game:fish-freshwater-trout-brown-adult"] = (long)5,
        ["game:fish-freshwater-trout-rainbow-adult"] = (long)5,
        ["game:fish-freshwater-bass-largemouth-adult"] = (long)10,
        ["game:fish-freshwater-bass-smallmouth-adult"] = (long)10,
        ["game:fish-freshwater-carp-common-adult"] = (long)10,
        ["game:fish-freshwater-carp-grass-adult"] = (long)10,
        ["game:fish-freshwater-catfish-blue-adult"] = (long)10,
        ["game:fish-freshwater-catfish-channel-adult"] = (long)10,
        ["game:fish-freshwater-pickerel-chain-adult"] = (long)10,
        ["game:fish-freshwater-salmon-coho-adult"] = (long)10,
        ["game:fish-freshwater-tilapia-nile-adult"] = (long)10,
        ["game:fish-freshwater-tilapia-red-adult"] = (long)10,
        ["game:fish-freshwater-walleye-common-adult"] = (long)10,
        ["game:fish-freshwater-pike-northern-adult"] = (long)15,
        ["game:fish-freshwater-arapaima-arapaima-adult"] = (long)20,
        ["game:fish-freshwater-arapaima-gigas-adult"] = (long)20,
        ["game:fish-freshwater-sheatfish-black-adult"] = (long)20,
        ["game:fish-freshwater-sheatfish-white-adult"] = (long)20,
        ["game:fish-saltwater-bream-sea-adult"] = (long)5,
        ["game:fish-saltwater-gurnard-cape-adult"] = (long)5,
        ["game:fish-saltwater-haddock-common-adult"] = (long)5,
        ["game:fish-saltwater-hake-silver-adult"] = (long)5,
        ["game:fish-saltwater-herring-atlantic-adult"] = (long)5,
        ["game:fish-saltwater-mackerel-atlantic-adult"] = (long)5,
        ["game:fish-saltwater-pollock-alaska-adult"] = (long)5,
        ["game:fish-saltwater-perch-pacific-adult"] = (long)5,
        ["game:fish-saltwater-barracuda-great-adult"] = (long)10,
        ["game:fish-saltwater-grouper-black-adult"] = (long)10,
        ["game:fish-saltwater-salmon-pink-adult"] = (long)10,
        ["game:fish-saltwater-snapper-red-adult"] = (long)10,
        ["game:fish-saltwater-tuna-skipjack-adult"] = (long)10,
        ["game:fish-saltwater-wolf-bering-adult"] = (long)10,
        ["game:fish-saltwater-amberjack-yellowtail-adult"] = (long)15,
        ["game:fish-saltwater-mahi-mahi-common-adult"] = (long)15,
        ["game:fish-saltwater-wreckfish-atlantic-adult"] = (long)15,
        ["game:fish-saltwater-coelacanth-common-adult"] = (long)20,
        ["game:fish-saltwater-sturgeon-atlantic-adult"] = (long)20,
        ["game:fish-reef-angel-bicolor-adult"] = (long)5,
        ["game:fish-reef-butterfly-copperband-adult"] = (long)5,
        ["game:fish-reef-butterfly-blackwedged-adult"] = (long)5,
        ["game:fish-reef-clown-black-adult"] = (long)5,
        ["game:fish-reef-clown-common-adult"] = (long)5,
        ["game:fish-reef-clown-yellowstripe-adult"] = (long)5,
        ["game:fish-reef-puffer-longspine-adult"] = (long)5,
        ["game:fish-reef-tang-banded-adult"] = (long)5,
        ["game:fish-reef-tang-powderblue-adult"] = (long)5,
        ["game:fish-reef-trigger-titan-adult"] = (long)5,
        ["game:fish-reef-wrasse-creole-adult"] = (long)5,
    };

    private static Dictionary<string, object> BuildHammerSmithChanceDefaultConfig() => new()
    {
        ["copper"] = "game:nugget-nativecopper",
        ["limonite"] = "game:nugget-limonite",
        ["gold"] = "game:nugget-nativegold",
        ["galena"] = "game:nugget-galena",
        ["cassiterite"] = "game:nugget-cassiterite",
        ["chromite"] = "game:nugget-chromite",
        ["ilmenite"] = "game:nugget-ilmenite",
        ["sphalerite"] = "game:nugget-sphalerite",
        ["silver"] = "game:nugget-nativesilver",
        ["bismuthinite"] = "game:nugget-bismuthinite",
        ["magnetite"] = "game:nugget-magnetite",
        ["hematite"] = "game:nugget-hematite",
        ["malachite"] = "game:nugget-malachite",
        ["pentlandite"] = "game:nugget-pentlandite",
        ["uranium"] = "game:nugget-uranium",
        ["wolframite"] = "game:nugget-wolframite",
        ["rhodochrosite"] = "game:nugget-rhodochrosite",
    };

    public static void PopulateHammerConfiguration(ICoreAPI api)
    {
        Dictionary<string, object> hammerLevelStats = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/levelstats",
            "hammer",
            BuildHammerDefaultConfig());
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
            BuildHammerEntityExpDefaultConfig());
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
            BuildHammerSmithChanceDefaultConfig());
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

    private static Dictionary<string, object> BuildSwordDefaultConfig() => new()
    {
        ["swordEXPPerHit"] = (long)swordEXPPerHit,
        ["swordEXPPerLevelBase"] = (long)swordEXPPerLevelBase,
        ["swordEXPMultiplyPerLevel"] = swordEXPMultiplyPerLevel,
        ["swordBaseDamage"] = (double)swordBaseDamage,
        ["swordIncrementDamagePerLevel"] = (double)swordIncrementDamagePerLevel,
        ["swordMaxLevel"] = (long)swordMaxLevel,
    };

    private static Dictionary<string, object> BuildSwordEntityExpDefaultConfig() => new()
    {
        ["game:sheep-bighorn-male"] = (long)50,
        ["game:sheep-bighorn-female"] = (long)50,
        ["game:sheep-bighorn-lamb"] = (long)20,
        ["game:chicken-rooster"] = (long)10,
        ["game:chicken-hen"] = (long)10,
        ["game:chicken-baby"] = (long)10,
        ["game:wolf-male"] = (long)40,
        ["game:wolf-female"] = (long)40,
        ["game:wolf-pup"] = (long)10,
        ["game:hyena-male"] = (long)40,
        ["game:hyena-female"] = (long)40,
        ["game:hyena-pup"] = (long)10,
        ["game:fox-male-red"] = (long)20,
        ["game:fox-female-red"] = (long)20,
        ["game:fox-pup"] = (long)10,
        ["game:fox-pup-red"] = (long)20,
        ["game:fox-pup-arctic"] = (long)20,
        ["game:fox-male-arctic"] = (long)10,
        ["game:fox-female-arctic"] = (long)10,
        ["game:raccoon-male"] = (long)20,
        ["game:raccoon-female"] = (long)20,
        ["game:raccoon-pup"] = (long)10,
        ["game:hare-male-arctic"] = (long)30,
        ["game:hare-male-ashgrey"] = (long)30,
        ["game:hare-male-darkbrown"] = (long)30,
        ["game:hare-male-desert"] = (long)30,
        ["game:hare-male-gold"] = (long)40,
        ["game:hare-male-lightbrown"] = (long)40,
        ["game:hare-male-lightgrey"] = (long)40,
        ["game:hare-male-silver"] = (long)40,
        ["game:hare-male-smokegrey"] = (long)50,
        ["game:hare-female-arctic"] = (long)60,
        ["game:hare-female-ashgrey"] = (long)60,
        ["game:hare-female-gold"] = (long)70,
        ["game:hare-female-lightbrown"] = (long)40,
        ["game:hare-female-lightgrey"] = (long)40,
        ["game:hare-female-silver"] = (long)40,
        ["game:hare-female-smokegrey"] = (long)30,
        ["game:hare-baby"] = (long)20,
        ["game:drifter-normal"] = (long)40,
        ["game:drifter-deep"] = (long)50,
        ["game:drifter-tainted"] = (long)60,
        ["game:drifter-corrupt"] = (long)70,
        ["game:drifter-nightmare"] = (long)80,
        ["game:drifter-double-headed"] = (long)90,
        ["game:locust-bronze"] = (long)60,
        ["game:locust-corrupt"] = (long)60,
        ["game:bell-normal"] = (long)100,
        ["game:bear-female-black"] = (long)50,
        ["game:bear-female-brown"] = (long)50,
        ["game:bear-female-sun"] = (long)50,
        ["game:bear-female-panda"] = (long)50,
        ["game:bear-female-polar"] = (long)50,
        ["game:bear-male-black"] = (long)50,
        ["game:bear-male-brown"] = (long)50,
        ["game:bear-male-sun"] = (long)50,
        ["game:bear-male-panda"] = (long)50,
        ["game:bear-male-polar"] = (long)50,
        ["game:locust-bronze-hacked"] = (long)60,
        ["game:locust-corrupt-hacked"] = (long)60,
        ["game:gazelle-male"] = (long)50,
        ["game:gazelle-female"] = (long)50,
        ["game:gazelle-calf"] = (long)30,
        ["game:deer-moose-male-adult"] = (long)30,
        ["game:deer-moose-female-adult"] = (long)50,
        ["game:deer-moose-male-baby"] = (long)50,
        ["game:deer-moose-female-baby"] = (long)30,
        ["game:deer-whitetail-male-adult"] = (long)30,
        ["game:deer-whitetail-female-adult"] = (long)30,
        ["game:deer-whitetail-male-baby"] = (long)30,
        ["game:deer-whitetail-female-baby"] = (long)10,
        ["game:deer-redbrocket-male-adult"] = (long)10,
        ["game:deer-chital-female-baby"] = (long)60,
        ["game:deer-guemal-male-adult"] = (long)60,
        ["game:deer-guemal-female-adult"] = (long)20,
        ["game:deer-guemal-male-baby"] = (long)20,
        ["game:deer-guemal-female-baby"] = (long)60,
        ["game:deer-pampas-male-adult"] = (long)60,
        ["game:deer-pampas-female-adult"] = (long)70,
        ["game:deer-pampas-male-baby"] = (long)70,
        ["game:deer-pampas-female-baby"] = (long)40,
        ["game:deer-pudu-male-adult"] = (long)40,
        ["game:deer-pudu-female-adult"] = (long)10,
        ["game:deer-pudu-male-baby"] = (long)10,
        ["game:deer-pudu-female-baby"] = (long)60,
        ["game:deer-elk-male-adult"] = (long)60,
        ["game:deer-elk-female-adult"] = (long)20,
        ["game:deer-elk-male-baby"] = (long)20,
        ["game:deer-elk-female-baby"] = (long)50,
        ["game:deer-taruca-male-adult"] = (long)50,
        ["game:deer-taruca-female-adult"] = (long)20,
        ["game:deer-taruca-male-baby"] = (long)20,
        ["game:deer-taruca-female-baby"] = (long)60,
        ["game:deer-chital-male-adult"] = (long)60,
        ["game:deer-chital-female-adult"] = (long)20,
        ["game:deer-chital-male-baby"] = (long)20,
        ["game:deer-fallow-female-baby"] = (long)60,
        ["game:deer-fallow-male-adult"] = (long)60,
        ["game:deer-fallow-male-baby"] = (long)20,
        ["game:deer-fallow-female-adult"] = (long)20,
        ["game:goat-angora-male-adult"] = (long)70,
        ["game:goat-angora-female-adult"] = (long)70,
        ["game:goat-angora-male-baby"] = (long)30,
        ["game:goat-angora-female-baby"] = (long)30,
        ["game:goat-ibexalp-male-adult"] = (long)70,
        ["game:goat-ibexalp-female-adult"] = (long)70,
        ["game:goat-ibexalp-male-baby"] = (long)30,
        ["game:goat-ibexalp-female-baby"] = (long)30,
        ["game:goat-ibexnub-male-adult"] = (long)50,
        ["game:goat-ibexnub-female-adult"] = (long)50,
        ["game:goat-ibexnub-male-baby"] = (long)20,
        ["game:goat-ibexnub-female-baby"] = (long)20,
        ["game:goat-markhor-male-adult"] = (long)60,
        ["game:goat-markhor-female-adult"] = (long)60,
        ["game:goat-markhor-male-baby"] = (long)20,
        ["game:goat-markhor-female-baby"] = (long)20,
        ["game:goat-mountain-male-adult"] = (long)40,
        ["game:goat-mountain-female-adult"] = (long)40,
        ["game:goat-mountain-male-baby"] = (long)20,
        ["game:goat-mountain-female-baby"] = (long)20,
        ["game:goat-muskox-male-adult"] = (long)40,
        ["game:goat-muskox-female-adult"] = (long)40,
        ["game:goat-muskox-male-baby"] = (long)20,
        ["game:goat-muskox-female-baby"] = (long)20,
        ["game:goat-nubian-male-adult"] = (long)40,
        ["game:goat-nubian-female-adult"] = (long)40,
        ["game:goat-nubian-male-baby"] = (long)20,
        ["game:goat-sirohi-male-adult"] = (long)40,
        ["game:goat-sirohi-female-adult"] = (long)40,
        ["game:goat-sirohi-male-baby"] = (long)20,
        ["game:goat-sirohi-female-baby"] = (long)20,
        ["game:goat-takingold-male-adult"] = (long)40,
        ["game:goat-takingold-female-adult"] = (long)40,
        ["game:goat-takingold-male-baby"] = (long)20,
        ["game:goat-takingold-female-baby"] = (long)20,
        ["game:goat-turdag-male-adult"] = (long)40,
        ["game:goat-turdag-female-adult"] = (long)40,
        ["game:goat-turdag-male-baby"] = (long)20,
        ["game:goat-turdag-female-baby"] = (long)20,
        ["game:goat-valais-male-adult"] = (long)40,
        ["game:goat-valais-female-adult"] = (long)40,
        ["game:goat-valais-male-baby"] = (long)20,
        ["game:goat-valais-female-baby"] = (long)20,
        ["game:pig-eurasian-adult-male"] = (long)30,
        ["game:pig-eurasian-adult-female"] = (long)30,
        ["game:pig-eurasian-elder-male"] = (long)40,
        ["game:pig-eurasian-elder-female"] = (long)40,
        ["game:pig-redriver-adult-male"] = (long)35,
        ["game:pig-redriver-adult-female"] = (long)35,
        ["game:pig-warthog-adult-male"] = (long)40,
        ["game:pig-warthog-adult-female"] = (long)40,
        ["game:pig-eurasian-baby-male"] = (long)10,
        ["game:pig-eurasian-baby-female"] = (long)10,
        ["game:pig-redriver-baby-male"] = (long)10,
        ["game:pig-redriver-baby-female"] = (long)10,
        ["game:pig-warthog-baby-male"] = (long)10,
        ["game:pig-warthog-baby-female"] = (long)10,
        ["game:sheep-mouflon-male"] = (long)50,
        ["game:sheep-mouflon-female"] = (long)50,
        ["game:sheep-mouflon-lamb"] = (long)20,
        ["game:shiver-surface"] = (long)50,
        ["game:shiver-deep"] = (long)60,
        ["game:shiver-tainted"] = (long)70,
        ["game:shiver-corrupt"] = (long)80,
        ["game:shiver-nightmare"] = (long)90,
        ["game:shiver-stilt"] = (long)60,
        ["game:shiver-bellhead"] = (long)80,
        ["game:shiver-deepsplit"] = (long)90,
        ["game:bowtorn-surface"] = (long)50,
        ["game:bowtorn-deep"] = (long)60,
        ["game:bowtorn-tainted"] = (long)70,
        ["game:bowtorn-corrupt"] = (long)80,
        ["game:bowtorn-nightmare"] = (long)90,
        ["game:bowtorn-gearfoot"] = (long)80,
        ["game:erel-pristine"] = (long)200,
        ["game:erel-corrupted"] = (long)250,
        ["game:eidolon-immobilized"] = (long)300,
        ["game:bellmini-normal"] = (long)100,
        ["game:locust-corrupt-sawblade"] = (long)60,
        ["game:chicken-henpoult"] = (long)10,
        ["game:chicken-roosterpoult"] = (long)10,
        ["game:deer-marsh-male-adult"] = (long)30,
        ["game:deer-marsh-female-adult"] = (long)30,
        ["game:deer-marsh-male-baby"] = (long)10,
        ["game:deer-marsh-female-baby"] = (long)10,
        ["game:deer-caribou-male-adult"] = (long)30,
        ["game:deer-caribou-female-adult"] = (long)30,
        ["game:deer-caribou-male-baby"] = (long)10,
        ["game:deer-caribou-female-baby"] = (long)10,
        ["game:deer-water-male-adult"] = (long)30,
        ["game:deer-water-female-adult"] = (long)30,
        ["game:deer-water-male-baby"] = (long)10,
        ["game:deer-water-female-baby"] = (long)10,
        ["game:deer-redbrocket-female-adult"] = (long)10,
        ["game:deer-redbrocket-male-baby"] = (long)10,
        ["game:deer-redbrocket-female-baby"] = (long)10,
        ["game:fish-freshwater-alewife-shad-adult"] = (long)5,
        ["game:fish-freshwater-chub-river-adult"] = (long)5,
        ["game:fish-freshwater-crappie-black-adult"] = (long)5,
        ["game:fish-freshwater-crappie-white-adult"] = (long)5,
        ["game:fish-freshwater-perch-european-adult"] = (long)5,
        ["game:fish-freshwater-perch-yellow-adult"] = (long)5,
        ["game:fish-freshwater-piranha-black-adult"] = (long)5,
        ["game:fish-freshwater-piranha-red-adult"] = (long)5,
        ["game:fish-freshwater-trout-brown-adult"] = (long)5,
        ["game:fish-freshwater-trout-rainbow-adult"] = (long)5,
        ["game:fish-freshwater-bass-largemouth-adult"] = (long)10,
        ["game:fish-freshwater-bass-smallmouth-adult"] = (long)10,
        ["game:fish-freshwater-carp-common-adult"] = (long)10,
        ["game:fish-freshwater-carp-grass-adult"] = (long)10,
        ["game:fish-freshwater-catfish-blue-adult"] = (long)10,
        ["game:fish-freshwater-catfish-channel-adult"] = (long)10,
        ["game:fish-freshwater-pickerel-chain-adult"] = (long)10,
        ["game:fish-freshwater-salmon-coho-adult"] = (long)10,
        ["game:fish-freshwater-tilapia-nile-adult"] = (long)10,
        ["game:fish-freshwater-tilapia-red-adult"] = (long)10,
        ["game:fish-freshwater-walleye-common-adult"] = (long)10,
        ["game:fish-freshwater-pike-northern-adult"] = (long)15,
        ["game:fish-freshwater-arapaima-arapaima-adult"] = (long)20,
        ["game:fish-freshwater-arapaima-gigas-adult"] = (long)20,
        ["game:fish-freshwater-sheatfish-black-adult"] = (long)20,
        ["game:fish-freshwater-sheatfish-white-adult"] = (long)20,
        ["game:fish-saltwater-bream-sea-adult"] = (long)5,
        ["game:fish-saltwater-gurnard-cape-adult"] = (long)5,
        ["game:fish-saltwater-haddock-common-adult"] = (long)5,
        ["game:fish-saltwater-hake-silver-adult"] = (long)5,
        ["game:fish-saltwater-herring-atlantic-adult"] = (long)5,
        ["game:fish-saltwater-mackerel-atlantic-adult"] = (long)5,
        ["game:fish-saltwater-pollock-alaska-adult"] = (long)5,
        ["game:fish-saltwater-perch-pacific-adult"] = (long)5,
        ["game:fish-saltwater-barracuda-great-adult"] = (long)10,
        ["game:fish-saltwater-grouper-black-adult"] = (long)10,
        ["game:fish-saltwater-salmon-pink-adult"] = (long)10,
        ["game:fish-saltwater-snapper-red-adult"] = (long)10,
        ["game:fish-saltwater-tuna-skipjack-adult"] = (long)10,
        ["game:fish-saltwater-wolf-bering-adult"] = (long)10,
        ["game:fish-saltwater-amberjack-yellowtail-adult"] = (long)15,
        ["game:fish-saltwater-mahi-mahi-common-adult"] = (long)15,
        ["game:fish-saltwater-wreckfish-atlantic-adult"] = (long)15,
        ["game:fish-saltwater-coelacanth-common-adult"] = (long)20,
        ["game:fish-saltwater-sturgeon-atlantic-adult"] = (long)20,
        ["game:fish-reef-angel-bicolor-adult"] = (long)5,
        ["game:fish-reef-butterfly-copperband-adult"] = (long)5,
        ["game:fish-reef-butterfly-blackwedged-adult"] = (long)5,
        ["game:fish-reef-clown-black-adult"] = (long)5,
        ["game:fish-reef-clown-common-adult"] = (long)5,
        ["game:fish-reef-clown-yellowstripe-adult"] = (long)5,
        ["game:fish-reef-puffer-longspine-adult"] = (long)5,
        ["game:fish-reef-tang-banded-adult"] = (long)5,
        ["game:fish-reef-tang-powderblue-adult"] = (long)5,
        ["game:fish-reef-trigger-titan-adult"] = (long)5,
        ["game:fish-reef-wrasse-creole-adult"] = (long)5,
    };

    public static void PopulateSwordConfiguration(ICoreAPI api)
    {
        Dictionary<string, object> swordLevelStats = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/levelstats",
            "sword",
            BuildSwordDefaultConfig());
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
            BuildSwordEntityExpDefaultConfig());
        foreach (KeyValuePair<string, object> pair in tmpentityExpSword)
        {
            if (pair.Value is long value) entityExpSword.Add(pair.Key, (int)value);
            else Debug.Log($"CONFIGURATION ERROR: entityExpSword {pair.Key} is not int");
        }

        Debug.Log("Sword configuration set");
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
    private static float shieldBasePassiveProjectile = 1.0f;
    private static float shieldPassiveProjectilePerLevel = 0.1f;
    private static float shieldBaseActiveProjectile = 1.0f;
    private static float shieldActiveProjectilePerLevel = 0.1f;
    private static float shieldBasePassive = 1.0f;
    private static float shieldPassivePerLevel = 0.1f;
    private static float shieldBaseActive = 1.0f;
    private static float shieldActivePerLevel = 0.1f;
    private static float shieldBaseProjectileDamageAbsorption = 1.0f;
    private static float shieldProjectileDamageAbsorptionPerLevel = 0.1f;
    private static float shieldBaseDamageAbsorption = 1.0f;
    private static float shieldDamageAbsorptionPerLevel = 0.1f;
    public static int shieldMaxLevel = 999;


    public static int ExpPerHitShield => shieldEXPPerHit;

    private static Dictionary<string, object> BuildShieldDefaultConfig() => new()
    {
        ["shieldEXPPerHit"] = (long)shieldEXPPerHit,
        ["shieldEXPPerLevelBase"] = (long)shieldEXPPerLevelBase,
        ["shieldEXPMultiplyPerLevel"] = shieldEXPMultiplyPerLevel,
        ["shieldBasePassiveProjectile"] = (double)shieldBasePassiveProjectile,
        ["shieldPassiveProjectilePerLevel"] = (double)shieldPassiveProjectilePerLevel,
        ["shieldBaseActiveProjectile"] = (double)shieldBaseActiveProjectile,
        ["shieldActiveProjectilePerLevel"] = (double)shieldActiveProjectilePerLevel,
        ["shieldBasePassive"] = (double)shieldBasePassive,
        ["shieldPassivePerLevel"] = (double)shieldPassivePerLevel,
        ["shieldBaseActive"] = (double)shieldBaseActive,
        ["shieldActivePerLevel"] = (double)shieldActivePerLevel,
        ["shieldBaseProjectileDamageAbsorption"] = (double)shieldBaseProjectileDamageAbsorption,
        ["shieldProjectileDamageAbsorptionPerLevel"] = (double)shieldProjectileDamageAbsorptionPerLevel,
        ["shieldBaseDamageAbsorption"] = (double)shieldBaseDamageAbsorption,
        ["shieldDamageAbsorptionPerLevel"] = (double)shieldDamageAbsorptionPerLevel,
        ["shieldMaxLevel"] = (long)shieldMaxLevel,
    };

    public static void PopulateShieldConfiguration(ICoreAPI api)
    {
        Dictionary<string, object> shieldLevelStats = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/levelstats",
            "shield",
            BuildShieldDefaultConfig());
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
        { //shieldBasePassiveProjectile
            if (shieldLevelStats.TryGetValue("shieldBasePassiveProjectile", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: shieldBasePassiveProjectile is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: shieldBasePassiveProjectile is not double is {value.GetType()}");
                else shieldBasePassiveProjectile = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: shieldBasePassiveProjectile not set");
        }
        { //shieldPassiveProjectilePerLevel
            if (shieldLevelStats.TryGetValue("shieldPassiveProjectilePerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: shieldPassiveProjectilePerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: shieldPassiveProjectilePerLevel is not double is {value.GetType()}");
                else shieldPassiveProjectilePerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: shieldPassiveProjectilePerLevel not set");
        }
        { //shieldBaseActiveProjectile
            if (shieldLevelStats.TryGetValue("shieldBaseActiveProjectile", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: shieldBaseActiveProjectile is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: shieldBaseActiveProjectile is not double is {value.GetType()}");
                else shieldBaseActiveProjectile = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: shieldBaseActiveProjectile not set");
        }
        { //shieldActiveProjectilePerLevel
            if (shieldLevelStats.TryGetValue("shieldActiveProjectilePerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: shieldActiveProjectilePerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: shieldActiveProjectilePerLevel is not double is {value.GetType()}");
                else shieldActiveProjectilePerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: shieldActiveProjectilePerLevel not set");
        }
        { //shieldBasePassive
            if (shieldLevelStats.TryGetValue("shieldBasePassive", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: shieldBasePassive is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: shieldBasePassive is not double is {value.GetType()}");
                else shieldBasePassive = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: shieldBasePassive not set");
        }
        { //shieldPassivePerLevel
            if (shieldLevelStats.TryGetValue("shieldPassivePerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: shieldPassivePerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: shieldPassivePerLevel is not double is {value.GetType()}");
                else shieldPassivePerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: shieldPassivePerLevel not set");
        }
        { //shieldBaseActive
            if (shieldLevelStats.TryGetValue("shieldBaseActive", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: shieldBaseActive is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: shieldBaseActive is not double is {value.GetType()}");
                else shieldBaseActive = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: shieldBaseActive not set");
        }
        { //shieldActivePerLevel
            if (shieldLevelStats.TryGetValue("shieldActivePerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: shieldActivePerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: shieldActivePerLevel is not double is {value.GetType()}");
                else shieldActivePerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: shieldActivePerLevel not set");
        }
        { //shieldBaseProjectileDamageAbsorption
            if (shieldLevelStats.TryGetValue("shieldBaseProjectileDamageAbsorption", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: shieldBaseProjectileDamageAbsorption is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: shieldBaseProjectileDamageAbsorption is not double is {value.GetType()}");
                else shieldBaseProjectileDamageAbsorption = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: shieldBaseProjectileDamageAbsorption not set");
        }
        { //shieldProjectileDamageAbsorptionPerLevel
            if (shieldLevelStats.TryGetValue("shieldProjectileDamageAbsorptionPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: shieldProjectileDamageAbsorptionPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: shieldProjectileDamageAbsorptionPerLevel is not double is {value.GetType()}");
                else shieldProjectileDamageAbsorptionPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: shieldProjectileDamageAbsorptionPerLevel not set");
        }
        { //shieldBaseDamageAbsorption
            if (shieldLevelStats.TryGetValue("shieldBaseDamageAbsorption", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: shieldBaseDamageAbsorption is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: shieldBaseDamageAbsorption is not double is {value.GetType()}");
                else shieldBaseDamageAbsorption = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: shieldBaseDamageAbsorption not set");
        }
        { //shieldDamageAbsorptionPerLevel
            if (shieldLevelStats.TryGetValue("shieldDamageAbsorptionPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: shieldDamageAbsorptionPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: shieldDamageAbsorptionPerLevel is not double is {value.GetType()}");
                else shieldDamageAbsorptionPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: shieldDamageAbsorptionPerLevel not set");
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

    public static float ShieldGetPassiveProjectileByLevel(int level)
    {
        return shieldBasePassiveProjectile + shieldPassiveProjectilePerLevel * level;
    }

    public static float ShieldGetActiveProjectileByLevel(int level)
    {
        return shieldBaseActiveProjectile + shieldActiveProjectilePerLevel * level;
    }

    public static float ShieldGetPassiveByLevel(int level)
    {
        return shieldBasePassive + shieldPassivePerLevel * level;
    }

    public static float ShieldGetActiveByLevel(int level)
    {
        return shieldBaseActive + shieldActivePerLevel * level;
    }

    public static float ShieldGetProjectileDamageAbsorptionByLevel(int level)
    {
        return shieldBaseProjectileDamageAbsorption + shieldProjectileDamageAbsorptionPerLevel * level;
    }

    public static float ShieldGetDamageAbsorptionByLevel(int level)
    {
        return shieldBaseDamageAbsorption + shieldDamageAbsorptionPerLevel * level;
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

    private static Dictionary<string, object> BuildHandDefaultConfig() => new()
    {
        ["handEXPPerHit"] = (long)handEXPPerHit,
        ["handEXPPerLevelBase"] = (long)handEXPPerLevelBase,
        ["handEXPMultiplyPerLevel"] = handEXPMultiplyPerLevel,
        ["handBaseDamage"] = (double)handBaseDamage,
        ["handIncrementDamagePerLevel"] = (double)handIncrementDamagePerLevel,
        ["handMaxLevel"] = (long)handMaxLevel,
    };

    private static Dictionary<string, object> BuildHandEntityExpDefaultConfig() => new()
    {
        ["game:sheep-bighorn-male"] = (long)50,
        ["game:sheep-bighorn-female"] = (long)50,
        ["game:sheep-bighorn-lamb"] = (long)20,
        ["game:chicken-rooster"] = (long)10,
        ["game:chicken-hen"] = (long)10,
        ["game:chicken-baby"] = (long)10,
        ["game:wolf-male"] = (long)40,
        ["game:wolf-female"] = (long)40,
        ["game:wolf-pup"] = (long)10,
        ["game:hyena-male"] = (long)40,
        ["game:hyena-female"] = (long)40,
        ["game:hyena-pup"] = (long)10,
        ["game:fox-male-red"] = (long)20,
        ["game:fox-female-red"] = (long)20,
        ["game:fox-pup"] = (long)10,
        ["game:fox-pup-red"] = (long)20,
        ["game:fox-pup-arctic"] = (long)20,
        ["game:fox-male-arctic"] = (long)10,
        ["game:fox-female-arctic"] = (long)10,
        ["game:raccoon-male"] = (long)20,
        ["game:raccoon-female"] = (long)20,
        ["game:raccoon-pup"] = (long)10,
        ["game:hare-male-arctic"] = (long)30,
        ["game:hare-male-ashgrey"] = (long)30,
        ["game:hare-male-darkbrown"] = (long)30,
        ["game:hare-male-desert"] = (long)30,
        ["game:hare-male-gold"] = (long)40,
        ["game:hare-male-lightbrown"] = (long)40,
        ["game:hare-male-lightgrey"] = (long)40,
        ["game:hare-male-silver"] = (long)40,
        ["game:hare-male-smokegrey"] = (long)50,
        ["game:hare-female-arctic"] = (long)60,
        ["game:hare-female-ashgrey"] = (long)60,
        ["game:hare-female-gold"] = (long)70,
        ["game:hare-female-lightbrown"] = (long)40,
        ["game:hare-female-lightgrey"] = (long)40,
        ["game:hare-female-silver"] = (long)40,
        ["game:hare-female-smokegrey"] = (long)30,
        ["game:hare-baby"] = (long)20,
        ["game:drifter-normal"] = (long)40,
        ["game:drifter-deep"] = (long)50,
        ["game:drifter-tainted"] = (long)60,
        ["game:drifter-corrupt"] = (long)70,
        ["game:drifter-nightmare"] = (long)80,
        ["game:drifter-double-headed"] = (long)90,
        ["game:locust-bronze"] = (long)60,
        ["game:locust-corrupt"] = (long)60,
        ["game:bell-normal"] = (long)100,
        ["game:bear-female-black"] = (long)50,
        ["game:bear-female-brown"] = (long)50,
        ["game:bear-female-sun"] = (long)50,
        ["game:bear-female-panda"] = (long)50,
        ["game:bear-female-polar"] = (long)50,
        ["game:bear-male-black"] = (long)50,
        ["game:bear-male-brown"] = (long)50,
        ["game:bear-male-sun"] = (long)50,
        ["game:bear-male-panda"] = (long)50,
        ["game:bear-male-polar"] = (long)50,
        ["game:locust-bronze-hacked"] = (long)60,
        ["game:locust-corrupt-hacked"] = (long)60,
        ["game:gazelle-male"] = (long)50,
        ["game:gazelle-female"] = (long)50,
        ["game:gazelle-calf"] = (long)30,
        ["game:deer-moose-male-adult"] = (long)30,
        ["game:deer-moose-female-adult"] = (long)50,
        ["game:deer-moose-male-baby"] = (long)50,
        ["game:deer-moose-female-baby"] = (long)30,
        ["game:deer-whitetail-male-adult"] = (long)30,
        ["game:deer-whitetail-female-adult"] = (long)30,
        ["game:deer-whitetail-male-baby"] = (long)30,
        ["game:deer-whitetail-female-baby"] = (long)10,
        ["game:deer-redbrocket-male-adult"] = (long)10,
        ["game:deer-chital-female-baby"] = (long)60,
        ["game:deer-guemal-male-adult"] = (long)60,
        ["game:deer-guemal-female-adult"] = (long)20,
        ["game:deer-guemal-male-baby"] = (long)20,
        ["game:deer-guemal-female-baby"] = (long)60,
        ["game:deer-pampas-male-adult"] = (long)60,
        ["game:deer-pampas-female-adult"] = (long)70,
        ["game:deer-pampas-male-baby"] = (long)70,
        ["game:deer-pampas-female-baby"] = (long)40,
        ["game:deer-pudu-male-adult"] = (long)40,
        ["game:deer-pudu-female-adult"] = (long)10,
        ["game:deer-pudu-male-baby"] = (long)10,
        ["game:deer-pudu-female-baby"] = (long)60,
        ["game:deer-elk-male-adult"] = (long)60,
        ["game:deer-elk-female-adult"] = (long)20,
        ["game:deer-elk-male-baby"] = (long)20,
        ["game:deer-elk-female-baby"] = (long)50,
        ["game:deer-taruca-male-adult"] = (long)50,
        ["game:deer-taruca-female-adult"] = (long)20,
        ["game:deer-taruca-male-baby"] = (long)20,
        ["game:deer-taruca-female-baby"] = (long)60,
        ["game:deer-chital-male-adult"] = (long)60,
        ["game:deer-chital-female-adult"] = (long)20,
        ["game:deer-chital-male-baby"] = (long)20,
        ["game:deer-fallow-female-baby"] = (long)60,
        ["game:deer-fallow-male-adult"] = (long)60,
        ["game:deer-fallow-male-baby"] = (long)20,
        ["game:deer-fallow-female-adult"] = (long)20,
        ["game:goat-angora-male-adult"] = (long)70,
        ["game:goat-angora-female-adult"] = (long)70,
        ["game:goat-angora-male-baby"] = (long)30,
        ["game:goat-angora-female-baby"] = (long)30,
        ["game:goat-ibexalp-male-adult"] = (long)70,
        ["game:goat-ibexalp-female-adult"] = (long)70,
        ["game:goat-ibexalp-male-baby"] = (long)30,
        ["game:goat-ibexalp-female-baby"] = (long)30,
        ["game:goat-ibexnub-male-adult"] = (long)50,
        ["game:goat-ibexnub-female-adult"] = (long)50,
        ["game:goat-ibexnub-male-baby"] = (long)20,
        ["game:goat-ibexnub-female-baby"] = (long)20,
        ["game:goat-markhor-male-adult"] = (long)60,
        ["game:goat-markhor-female-adult"] = (long)60,
        ["game:goat-markhor-male-baby"] = (long)20,
        ["game:goat-markhor-female-baby"] = (long)20,
        ["game:goat-mountain-male-adult"] = (long)40,
        ["game:goat-mountain-female-adult"] = (long)40,
        ["game:goat-mountain-male-baby"] = (long)20,
        ["game:goat-mountain-female-baby"] = (long)20,
        ["game:goat-muskox-male-adult"] = (long)40,
        ["game:goat-muskox-female-adult"] = (long)40,
        ["game:goat-muskox-male-baby"] = (long)20,
        ["game:goat-muskox-female-baby"] = (long)20,
        ["game:goat-nubian-male-adult"] = (long)40,
        ["game:goat-nubian-female-adult"] = (long)40,
        ["game:goat-nubian-male-baby"] = (long)20,
        ["game:goat-sirohi-male-adult"] = (long)40,
        ["game:goat-sirohi-female-adult"] = (long)40,
        ["game:goat-sirohi-male-baby"] = (long)20,
        ["game:goat-sirohi-female-baby"] = (long)20,
        ["game:goat-takingold-male-adult"] = (long)40,
        ["game:goat-takingold-female-adult"] = (long)40,
        ["game:goat-takingold-male-baby"] = (long)20,
        ["game:goat-takingold-female-baby"] = (long)20,
        ["game:goat-turdag-male-adult"] = (long)40,
        ["game:goat-turdag-female-adult"] = (long)40,
        ["game:goat-turdag-male-baby"] = (long)20,
        ["game:goat-turdag-female-baby"] = (long)20,
        ["game:goat-valais-male-adult"] = (long)40,
        ["game:goat-valais-female-adult"] = (long)40,
        ["game:goat-valais-male-baby"] = (long)20,
        ["game:goat-valais-female-baby"] = (long)20,
        ["game:pig-eurasian-adult-male"] = (long)30,
        ["game:pig-eurasian-adult-female"] = (long)30,
        ["game:pig-eurasian-elder-male"] = (long)40,
        ["game:pig-eurasian-elder-female"] = (long)40,
        ["game:pig-redriver-adult-male"] = (long)35,
        ["game:pig-redriver-adult-female"] = (long)35,
        ["game:pig-warthog-adult-male"] = (long)40,
        ["game:pig-warthog-adult-female"] = (long)40,
        ["game:pig-eurasian-baby-male"] = (long)10,
        ["game:pig-eurasian-baby-female"] = (long)10,
        ["game:pig-redriver-baby-male"] = (long)10,
        ["game:pig-redriver-baby-female"] = (long)10,
        ["game:pig-warthog-baby-male"] = (long)10,
        ["game:pig-warthog-baby-female"] = (long)10,
        ["game:sheep-mouflon-male"] = (long)50,
        ["game:sheep-mouflon-female"] = (long)50,
        ["game:sheep-mouflon-lamb"] = (long)20,
        ["game:shiver-surface"] = (long)50,
        ["game:shiver-deep"] = (long)60,
        ["game:shiver-tainted"] = (long)70,
        ["game:shiver-corrupt"] = (long)80,
        ["game:shiver-nightmare"] = (long)90,
        ["game:shiver-stilt"] = (long)60,
        ["game:shiver-bellhead"] = (long)80,
        ["game:shiver-deepsplit"] = (long)90,
        ["game:bowtorn-surface"] = (long)50,
        ["game:bowtorn-deep"] = (long)60,
        ["game:bowtorn-tainted"] = (long)70,
        ["game:bowtorn-corrupt"] = (long)80,
        ["game:bowtorn-nightmare"] = (long)90,
        ["game:bowtorn-gearfoot"] = (long)80,
        ["game:erel-pristine"] = (long)200,
        ["game:erel-corrupted"] = (long)250,
        ["game:eidolon-immobilized"] = (long)300,
        ["game:bellmini-normal"] = (long)100,
        ["game:locust-corrupt-sawblade"] = (long)60,
        ["game:chicken-henpoult"] = (long)10,
        ["game:chicken-roosterpoult"] = (long)10,
        ["game:deer-marsh-male-adult"] = (long)30,
        ["game:deer-marsh-female-adult"] = (long)30,
        ["game:deer-marsh-male-baby"] = (long)10,
        ["game:deer-marsh-female-baby"] = (long)10,
        ["game:deer-caribou-male-adult"] = (long)30,
        ["game:deer-caribou-female-adult"] = (long)30,
        ["game:deer-caribou-male-baby"] = (long)10,
        ["game:deer-caribou-female-baby"] = (long)10,
        ["game:deer-water-male-adult"] = (long)30,
        ["game:deer-water-female-adult"] = (long)30,
        ["game:deer-water-male-baby"] = (long)10,
        ["game:deer-water-female-baby"] = (long)10,
        ["game:deer-redbrocket-female-adult"] = (long)10,
        ["game:deer-redbrocket-male-baby"] = (long)10,
        ["game:deer-redbrocket-female-baby"] = (long)10,
        ["game:fish-freshwater-alewife-shad-adult"] = (long)5,
        ["game:fish-freshwater-chub-river-adult"] = (long)5,
        ["game:fish-freshwater-crappie-black-adult"] = (long)5,
        ["game:fish-freshwater-crappie-white-adult"] = (long)5,
        ["game:fish-freshwater-perch-european-adult"] = (long)5,
        ["game:fish-freshwater-perch-yellow-adult"] = (long)5,
        ["game:fish-freshwater-piranha-black-adult"] = (long)5,
        ["game:fish-freshwater-piranha-red-adult"] = (long)5,
        ["game:fish-freshwater-trout-brown-adult"] = (long)5,
        ["game:fish-freshwater-trout-rainbow-adult"] = (long)5,
        ["game:fish-freshwater-bass-largemouth-adult"] = (long)10,
        ["game:fish-freshwater-bass-smallmouth-adult"] = (long)10,
        ["game:fish-freshwater-carp-common-adult"] = (long)10,
        ["game:fish-freshwater-carp-grass-adult"] = (long)10,
        ["game:fish-freshwater-catfish-blue-adult"] = (long)10,
        ["game:fish-freshwater-catfish-channel-adult"] = (long)10,
        ["game:fish-freshwater-pickerel-chain-adult"] = (long)10,
        ["game:fish-freshwater-salmon-coho-adult"] = (long)10,
        ["game:fish-freshwater-tilapia-nile-adult"] = (long)10,
        ["game:fish-freshwater-tilapia-red-adult"] = (long)10,
        ["game:fish-freshwater-walleye-common-adult"] = (long)10,
        ["game:fish-freshwater-pike-northern-adult"] = (long)15,
        ["game:fish-freshwater-arapaima-arapaima-adult"] = (long)20,
        ["game:fish-freshwater-arapaima-gigas-adult"] = (long)20,
        ["game:fish-freshwater-sheatfish-black-adult"] = (long)20,
        ["game:fish-freshwater-sheatfish-white-adult"] = (long)20,
        ["game:fish-saltwater-bream-sea-adult"] = (long)5,
        ["game:fish-saltwater-gurnard-cape-adult"] = (long)5,
        ["game:fish-saltwater-haddock-common-adult"] = (long)5,
        ["game:fish-saltwater-hake-silver-adult"] = (long)5,
        ["game:fish-saltwater-herring-atlantic-adult"] = (long)5,
        ["game:fish-saltwater-mackerel-atlantic-adult"] = (long)5,
        ["game:fish-saltwater-pollock-alaska-adult"] = (long)5,
        ["game:fish-saltwater-perch-pacific-adult"] = (long)5,
        ["game:fish-saltwater-barracuda-great-adult"] = (long)10,
        ["game:fish-saltwater-grouper-black-adult"] = (long)10,
        ["game:fish-saltwater-salmon-pink-adult"] = (long)10,
        ["game:fish-saltwater-snapper-red-adult"] = (long)10,
        ["game:fish-saltwater-tuna-skipjack-adult"] = (long)10,
        ["game:fish-saltwater-wolf-bering-adult"] = (long)10,
        ["game:fish-saltwater-amberjack-yellowtail-adult"] = (long)15,
        ["game:fish-saltwater-mahi-mahi-common-adult"] = (long)15,
        ["game:fish-saltwater-wreckfish-atlantic-adult"] = (long)15,
        ["game:fish-saltwater-coelacanth-common-adult"] = (long)20,
        ["game:fish-saltwater-sturgeon-atlantic-adult"] = (long)20,
        ["game:fish-reef-angel-bicolor-adult"] = (long)5,
        ["game:fish-reef-butterfly-copperband-adult"] = (long)5,
        ["game:fish-reef-butterfly-blackwedged-adult"] = (long)5,
        ["game:fish-reef-clown-black-adult"] = (long)5,
        ["game:fish-reef-clown-common-adult"] = (long)5,
        ["game:fish-reef-clown-yellowstripe-adult"] = (long)5,
        ["game:fish-reef-puffer-longspine-adult"] = (long)5,
        ["game:fish-reef-tang-banded-adult"] = (long)5,
        ["game:fish-reef-tang-powderblue-adult"] = (long)5,
        ["game:fish-reef-trigger-titan-adult"] = (long)5,
        ["game:fish-reef-wrasse-creole-adult"] = (long)5,
    };

    public static void PopulateHandConfiguration(ICoreAPI api)
    {
        Dictionary<string, object> handLevelStats = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/levelstats",
            "hand",
            BuildHandDefaultConfig());
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
            BuildHandEntityExpDefaultConfig());
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
    private static float farmingIncrementHarvestMultiplyPerLevel = 0.1f;
    private static float farmingBaseForageMultiply = 1.0f;
    private static float farmingIncrementForageMultiplyPerLevel = 0.08f;
    public static int farmingMaxLevel = 999;

    public static int ExpPerTillFarming => farmingEXPPerTill;
    public static float BaseHarvestMultiplyFarming => farmingBaseHarvestMultiply;

    private static Dictionary<string, object> BuildFarmingDefaultConfig() => new()
    {
        ["farmingEXPPerTill"] = (long)farmingEXPPerTill,
        ["farmingEXPPerLevelBase"] = (long)farmingEXPPerLevelBase,
        ["farmingEXPMultiplyPerLevel"] = farmingEXPMultiplyPerLevel,
        ["farmingBaseHarvestMultiply"] = (double)farmingBaseHarvestMultiply,
        ["farmingIncrementHarvestMultiplyPerLevel"] = (double)farmingIncrementHarvestMultiplyPerLevel,
        ["farmingBaseForageMultiply"] = (double)farmingBaseForageMultiply,
        ["farmingIncrementForageMultiplyPerLevel"] = (double)farmingIncrementForageMultiplyPerLevel,
        ["farmingMaxLevel"] = (long)farmingMaxLevel,
    };

    private static Dictionary<string, object> BuildFarmingCropsDefaultConfig()
    {
        Dictionary<string, object> config = new()
        {
            // Crops
            ["game:crop-turnip-5"] = (long)40,
            ["game:crop-turnip-4"] = (long)10,
            ["game:crop-carrot-6"] = (long)20,
            ["game:crop-carrot-7"] = (long)50,
            ["game:crop-flax-9"] = (long)80,
            ["game:crop-flax-8"] = (long)40,
            ["game:crop-onion-7"] = (long)50,
            ["game:crop-onion-6"] = (long)30,
            ["game:crop-spelt-9"] = (long)80,
            ["game:crop-spelt-8"] = (long)40,
            ["game:crop-parsnip-8"] = (long)70,
            ["game:crop-parsnip-7"] = (long)30,
            ["game:crop-rye-9"] = (long)80,
            ["game:crop-rye-8"] = (long)40,
            ["game:crop-rice-10"] = (long)100,
            ["game:crop-rice-9"] = (long)50,
            ["game:crop-soybean-11"] = (long)120,
            ["game:crop-soybean-10"] = (long)50,
            ["game:crop-amaranth-9"] = (long)70,
            ["game:crop-amaranth-8"] = (long)30,
            ["game:crop-cassava-9"] = (long)80,
            ["game:crop-cassava-8"] = (long)30,
            ["game:crop-peanut-9"] = (long)70,
            ["game:crop-peanut-8"] = (long)20,
            ["game:crop-pineapple-16"] = (long)200,
            ["game:crop-pineapple-15"] = (long)90,
            ["game:crop-sunflower-12"] = (long)100,
            ["game:crop-sunflower-11"] = (long)60,
            ["game:crop-pumpkin-8"] = (long)120,
            ["game:crop-pumpkin-7"] = (long)50,
            ["game:crop-cabbage-12"] = (long)140,
            ["game:crop-cabbage-11"] = (long)60,
            // Mushrooms
            ["game:mushroom-fieldmushroom-normal"] = (long)10,
            ["game:mushroom-almondmushroom-normal"] = (long)10,
            ["game:mushroom-flyagaric-normal"] = (long)10,
            ["game:mushroom-bitterbolete-normal"] = (long)10,
            ["game:mushroom-blacktrumpet-normal"] = (long)10,
            ["game:mushroom-chanterelle-normal"] = (long)10,
            ["game:mushroom-commonmorel-normal"] = (long)10,
            ["game:mushroom-deathcap-normal"] = (long)10,
            ["game:mushroom-devilstooth-normal"] = (long)10,
            ["game:mushroom-devilbolete-normal"] = (long)10,
            ["game:mushroom-earthball-normal"] = (long)10,
            ["game:mushroom-elfinsaddle-normal"] = (long)10,
            ["game:mushroom-golddropmilkcap-normal-north"] = (long)10,
            ["game:mushroom-greencrackedrussula-normal"] = (long)10,
            ["game:mushroom-indigomilkcap-normal"] = (long)10,
            ["game:mushroom-jackolantern-normal"] = (long)10,
            ["game:mushroom-kingbolete-normal"] = (long)10,
            ["game:mushroom-lobster-normal"] = (long)10,
            ["game:mushroom-orangeoakbolete-normal"] = (long)10,
            ["game:mushroom-paddystraw-normal"] = (long)10,
            ["game:mushroom-puffball-normal"] = (long)10,
            ["game:mushroom-redwinecap-normal"] = (long)10,
            ["game:mushroom-saffronmilkcap-normal"] = (long)10,
            ["game:mushroom-violetwebcap-normal"] = (long)10,
            ["game:mushroom-witchhat-normal"] = (long)10,
            ["game:mushroom-beardedtooth-normal"] = (long)10,
            ["game:mushroom-chickenofthewoods-normal-north"] = (long)10,
            ["game:mushroom-dryadsaddle-normal"] = (long)10,
            ["game:mushroom-pinkoyster-normal-north"] = (long)10,
            ["game:mushroom-tinderhoof-normal-north"] = (long)10,
            ["game:mushroom-whiteoyster-normal-north"] = (long)10,
            ["game:mushroom-reishi-normal-north"] = (long)10,
            ["game:mushroom-funeralbell-normal-north"] = (long)10,
            ["game:mushroom-deerear-normal-north"] = (long)10,
            ["game:mushroom-livermushroom-normal-north"] = (long)10,
            ["game:mushroom-pinkbonnet-normal-north"] = (long)10,
            ["game:mushroom-shiitake-normal-north"] = (long)10,
        };

        // Bush Berries (current fruiting bush system: fruitingbush-{wild|grown}-{type}-{free|snow})
        string[] fruitingBushTypes =
        [
            "beautyberry", "blueberry", "cloudberry", "cranberry", "blackberry",
            "blackcurrant", "raspberry", "redcurrant", "whitecurrant", "strawberry"
        ];
        foreach (string state in new[] { "wild", "grown" })
            foreach (string type in fruitingBushTypes)
                foreach (string cover in new[] { "free", "snow" })
                    config[$"game:fruitingbush-{state}-{type}-{cover}"] = (long)10;

        return config;
    }

    public static void PopulateFarmingConfiguration(ICoreAPI api)
    {
        Dictionary<string, object> farmingLevelStats = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/levelstats",
            "farming",
            BuildFarmingDefaultConfig());
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
                else farmingEXPMultiplyPerLevel = (double)value;
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
            BuildFarmingCropsDefaultConfig());
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

    private static Dictionary<string, object> BuildCookingDefaultConfig() => new()
    {
        ["cookingBaseExpPerCooking"] = (long)cookingBaseExpPerCooking,
        ["cookingEXPPerLevelBase"] = (long)cookingEXPPerLevelBase,
        ["cookingEXPMultiplyPerLevel"] = cookingEXPMultiplyPerLevel,
        ["cookingBaseFreshHoursMultiply"] = (double)cookingBaseFreshHoursMultiply,
        ["cookingFreshHoursMultiplyPerLevel"] = (double)cookingFreshHoursMultiplyPerLevel,
        ["cookingBaseChanceToIncreaseServings"] = (double)cookingBaseChanceToIncreaseServings,
        ["cookingReduceChanceToIncreaseServings"] = (long)cookingReduceChanceToIncreaseServings,
        ["cookingIncrementChanceToIncreaseServings"] = (double)cookingIncrementChanceToIncreaseServings,
        ["cookingChanceToIncreaseServingsReducerTotal"] = (double)cookingChanceToIncreaseServingsReducerTotal,
        ["cookingBaseRollsChanceToIncreaseServings"] = (long)cookingBaseRollsChanceToIncreaseServings,
        ["cookingEarnRollsChanceToIncreaseServingsEveryLevel"] = (long)cookingEarnRollsChanceToIncreaseServingsEveryLevel,
        ["cookingEarnRollsChanceToIncreaseServingsQuantity"] = (long)cookingEarnRollsChanceToIncreaseServingsQuantity,
        ["cookingMaxLevel"] = (long)cookingMaxLevel,
    };

    private static Dictionary<string, object> BuildCookingSinglesDefaultConfig() => new()
    {
        ["game:redmeat-cooked"] = 0.5,
        ["game:poultry-cooked"] = 0.4,
        ["game:fish-cooked"] = 0.4,
        ["game:bushmeat-cooked"] = 0.3,
        ["game:vegetable-cookedcattailroot"] = 0.1,
    };

    private static Dictionary<string, object> BuildCookingPotsDefaultConfig() => new()
    {
        ["game:claypot-cooked"] = 3.0,
        ["game:claypot-blue-cooked"] = 3.0,
        ["game:claypot-fire-cooked"] = 3.0,
        ["game:claypot-black-cooked"] = 3.0,
        ["game:claypot-brown-cooked"] = 3.0,
        ["game:claypot-cream-cooked"] = 3.0,
        ["game:claypot-earthyorange-cooked"] = 3.0,
        ["game:claypot-gray-cooked"] = 3.0,
        ["game:claypot-orange-cooked"] = 3.0,
        ["game:claypot-red-cooked"] = 3.0,
        ["game:claypot-tan-cooked"] = 3.0,
    };

    public static void PopulateCookingConfiguration(ICoreAPI api)
    {
        Dictionary<string, object> cookingLevelStats = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/levelstats",
            "cooking",
            BuildCookingDefaultConfig());
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
            BuildCookingSinglesDefaultConfig());
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
            BuildCookingPotsDefaultConfig());
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

    private static Dictionary<string, object> BuildPanningDefaultConfig() => new()
    {
        ["panningBaseExpPerPanning"] = (long)panningBaseExpPerPanning,
        ["panningEXPPerLevelBase"] = (long)panningEXPPerLevelBase,
        ["panningEXPMultiplyPerLevel"] = panningEXPMultiplyPerLevel,
        ["panningBaseLootMultiply"] = (double)panningBaseLootMultiply,
        ["panningLootMultiplyPerLevel"] = (double)panningLootMultiplyPerLevel,
        ["panningBaseChanceToDoubleLoot"] = (double)panningBaseChanceToDoubleLoot,
        ["panningChanceToDoubleLootPerLevel"] = (double)panningChanceToDoubleLootPerLevel,
        ["panningBaseChanceToTripleLoot"] = (double)panningBaseChanceToTripleLoot,
        ["panningChanceToTripleLootPerLevel"] = (double)panningChanceToTripleLootPerLevel,
        ["panningBaseChanceToQuadrupleLoot"] = (double)panningBaseChanceToQuadrupleLoot,
        ["panningChanceToQuadrupleLootPerLevel"] = (double)panningChanceToQuadrupleLootPerLevel,
        ["panningMaxLevel"] = (long)panningMaxLevel,
    };

    public static void PopulatePanningConfiguration(ICoreAPI api)
    {
        Dictionary<string, object> panningLevelStats = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/levelstats",
            "panning",
            BuildPanningDefaultConfig());
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
    private static float smithingIncrementAttackPowerMultiplyPerLevel = 0.04f;
    private static float smithingBaseMiningSpeedMultiply = 1.0f;
    private static float smithingIncrementMiningSpeedMultiplyPerLevel = 0.025f;
    private static float smithingBaseArmorProtectionMultiply = 1.0f;
    private static float smithingIncrementArmorProtectionMultiplyPerLevel = 0.015f;
    private static float smithingBaseArmorStatusMultiply = 1.0f;
    private static float smithingIncrementArmorStatusMultiplyPerLevel = 0.02f;
    public static int smithingMaxLevel = 999;
    public static double smithingSubLevelEXPMultiply = 3.0;

    private static Dictionary<string, object> BuildSmithingDefaultConfig() => new()
    {
        ["smithingEXPPerLevelBase"] = (long)smithingEXPPerLevelBase,
        ["smithingEXPMultiplyPerLevel"] = smithingEXPMultiplyPerLevel,
        ["smithingBaseDurabilityMultiply"] = (double)smithingBaseDurabilityMultiply,
        ["smithingIncrementDurabilityMultiplyPerLevel"] = (double)smithingIncrementDurabilityMultiplyPerLevel,
        ["smithingBaseAttackPowerMultiply"] = (double)smithingBaseAttackPowerMultiply,
        ["smithingIncrementAttackPowerMultiplyPerLevel"] = (double)smithingIncrementAttackPowerMultiplyPerLevel,
        ["smithingBaseMiningSpeedMultiply"] = (double)smithingBaseMiningSpeedMultiply,
        ["smithingIncrementMiningSpeedMultiplyPerLevel"] = (double)smithingIncrementMiningSpeedMultiplyPerLevel,
        ["smithingBaseArmorProtectionMultiply"] = (double)smithingBaseArmorProtectionMultiply,
        ["smithingIncrementArmorProtectionMultiplyPerLevel"] = (double)smithingIncrementArmorProtectionMultiplyPerLevel,
        ["smithingBaseArmorStatusMultiply"] = (double)smithingBaseArmorStatusMultiply,
        ["smithingIncrementArmorStatusMultiplyPerLevel"] = (double)smithingIncrementArmorStatusMultiplyPerLevel,
        ["smithingMaxLevel"] = (long)smithingMaxLevel,
        ["smithingSubLevelEXPMultiply"] = smithingSubLevelEXPMultiply,
    };

    private static Dictionary<string, object> BuildSmithingCraftsDefaultConfig() => new()
    {
        // Tools
        ["game:pickaxe-copper"] = (long)200,
        ["game:pickaxe-tinbronze"] = (long)300,
        ["game:pickaxe-bismuthbronze"] = (long)400,
        ["game:pickaxe-blackbronze"] = (long)500,
        ["game:pickaxe-gold"] = (long)600,
        ["game:pickaxe-silver"] = (long)700,
        ["game:pickaxe-iron"] = (long)500,
        ["game:pickaxe-meteoriciron"] = (long)800,
        ["game:pickaxe-steel"] = (long)1000,
        ["game:shovel-chert"] = (long)20,
        ["game:shovel-granite"] = (long)20,
        ["game:shovel-andesite"] = (long)20,
        ["game:shovel-basalt"] = (long)20,
        ["game:shovel-obsidian"] = (long)50,
        ["game:shovel-peridotite"] = (long)20,
        ["game:shovel-flint"] = (long)20,
        ["game:shovel-copper"] = (long)200,
        ["game:shovel-tinbronze"] = (long)300,
        ["game:shovel-bismuthbronze"] = (long)400,
        ["game:shovel-blackbronze"] = (long)500,
        ["game:shovel-gold"] = (long)600,
        ["game:shovel-silver"] = (long)700,
        ["game:shovel-iron"] = (long)500,
        ["game:shovel-meteoriciron"] = (long)800,
        ["game:shovel-steel"] = (long)1000,
        ["game:axe-chert"] = (long)20,
        ["game:axe-granite"] = (long)20,
        ["game:axe-andesite"] = (long)20,
        ["game:axe-basalt"] = (long)20,
        ["game:axe-obsidian"] = (long)50,
        ["game:axe-peridotite"] = (long)20,
        ["game:axe-flint"] = (long)20,
        ["game:axe-bone-chert"] = (long)40,
        ["game:axe-bone-granite"] = (long)40,
        ["game:axe-bone-andesite"] = (long)40,
        ["game:axe-bone-basalt"] = (long)40,
        ["game:axe-bone-obsidian"] = (long)100,
        ["game:axe-bone-peridotite"] = (long)40,
        ["game:axe-bone-flint"] = (long)40,
        ["game:axe-felling-copper"] = (long)200,
        ["game:axe-felling-tinbronze"] = (long)300,
        ["game:axe-felling-bismuthbronze"] = (long)400,
        ["game:axe-felling-blackbronze"] = (long)500,
        ["game:axe-felling-gold"] = (long)600,
        ["game:axe-felling-silver"] = (long)700,
        ["game:axe-felling-iron"] = (long)500,
        ["game:axe-felling-meteoriciron"] = (long)800,
        ["game:axe-felling-steel"] = (long)1000,
        ["game:blade-falx-copper"] = (long)200,
        ["game:blade-falx-tinbronze"] = (long)300,
        ["game:blade-falx-bismuthbronze"] = (long)400,
        ["game:blade-falx-blackbronze"] = (long)500,
        ["game:blade-falx-gold"] = (long)600,
        ["game:blade-falx-silver"] = (long)700,
        ["game:blade-falx-iron"] = (long)500,
        ["game:blade-blackguard-iron"] = (long)600,
        ["game:blade-falx-meteoriciron"] = (long)800,
        ["game:blade-falx-steel"] = (long)1000,
        ["game:knife-generic-chert"] = (long)20,
        ["game:knife-generic-granite"] = (long)20,
        ["game:knife-generic-andesite"] = (long)20,
        ["game:knife-generic-basalt"] = (long)20,
        ["game:knife-generic-obsidian"] = (long)50,
        ["game:knife-generic-peridotite"] = (long)20,
        ["game:knife-generic-flint"] = (long)20,
        ["game:knife-generic-bonechert"] = (long)40,
        ["game:knife-generic-bonegranite"] = (long)40,
        ["game:knife-generic-boneandesite"] = (long)40,
        ["game:knife-generic-bonebasalt"] = (long)40,
        ["game:knife-generic-boneobsidian"] = (long)100,
        ["game:knife-generic-boneperidotite"] = (long)40,
        ["game:knife-generic-boneflint"] = (long)40,
        ["game:knife-generic-copper"] = (long)200,
        ["game:knife-generic-tinbronze"] = (long)300,
        ["game:knife-generic-bismuthbronze"] = (long)400,
        ["game:knife-generic-blackbronze"] = (long)500,
        ["game:knife-generic-gold"] = (long)600,
        ["game:knife-generic-silver"] = (long)700,
        ["game:knife-generic-iron"] = (long)500,
        ["game:knife-generic-meteoriciron"] = (long)800,
        ["game:knife-generic-steel"] = (long)1000,
        ["game:hoe-chert"] = (long)20,
        ["game:hoe-granite"] = (long)20,
        ["game:hoe-andesite"] = (long)20,
        ["game:hoe-basalt"] = (long)20,
        ["game:hoe-obsidian"] = (long)50,
        ["game:hoe-peridotite"] = (long)20,
        ["game:hoe-flint"] = (long)20,
        ["game:hoe-copper"] = (long)200,
        ["game:hoe-tinbronze"] = (long)300,
        ["game:hoe-bismuthbronze"] = (long)400,
        ["game:hoe-blackbronze"] = (long)500,
        ["game:hoe-gold"] = (long)600,
        ["game:hoe-silver"] = (long)700,
        ["game:hoe-iron"] = (long)500,
        ["game:hoe-meteoriciron"] = (long)800,
        ["game:hoe-steel"] = (long)1000,
        ["game:hammer-copper"] = (long)200,
        ["game:hammer-tinbronze"] = (long)300,
        ["game:hammer-bismuthbronze"] = (long)400,
        ["game:hammer-blackbronze"] = (long)500,
        ["game:hammer-gold"] = (long)600,
        ["game:hammer-silver"] = (long)700,
        ["game:hammer-iron"] = (long)500,
        ["game:hammer-meteoriciron"] = (long)800,
        ["game:hammer-steel"] = (long)1000,
        ["game:spear-generic-chert"] = (long)20,
        ["game:spear-generic-granite"] = (long)20,
        ["game:spear-generic-andesite"] = (long)20,
        ["game:spear-generic-basalt"] = (long)20,
        ["game:spear-generic-obsidian"] = (long)50,
        ["game:spear-generic-peridotite"] = (long)20,
        ["game:spear-generic-copper"] = (long)100,
        ["game:spear-generic-tinbronze"] = (long)150,
        ["game:spear-generic-bismuthbronze"] = (long)200,
        ["game:spear-generic-blackbronze"] = (long)250,
        ["game:spear-generic-ornategold"] = (long)300,
        ["game:spear-generic-ornatesilver"] = (long)300,
        ["game:bow-simple"] = (long)100,
        ["game:bow-crude"] = (long)200,
        ["game:bow-long"] = (long)300,
        ["game:bow-recurve"] = (long)400,
        // Tools Utils
        ["game:shears-copper"] = (long)100,
        ["game:shears-tinbronze"] = (long)200,
        ["game:shears-bismuthbronze"] = (long)300,
        ["game:shears-blackbronze"] = (long)400,
        ["game:shears-gold"] = (long)500,
        ["game:shears-silver"] = (long)600,
        ["game:shears-iron"] = (long)400,
        ["game:shears-meteoriciron"] = (long)700,
        ["game:shears-steel"] = (long)900,
        ["game:chisel-copper"] = (long)100,
        ["game:chisel-tinbronze"] = (long)200,
        ["game:chisel-bismuthbronze"] = (long)300,
        ["game:chisel-blackbronze"] = (long)400,
        ["game:chisel-gold"] = (long)500,
        ["game:chisel-silver"] = (long)600,
        ["game:chisel-iron"] = (long)400,
        ["game:chisel-meteoriciron"] = (long)700,
        ["game:chisel-steel"] = (long)900,
        ["game:wrench-copper"] = (long)100,
        ["game:wrench-tinbronze"] = (long)200,
        ["game:wrench-bismuthbronze"] = (long)300,
        ["game:wrench-blackbronze"] = (long)400,
        ["game:wrench-gold"] = (long)500,
        ["game:wrench-silver"] = (long)600,
        ["game:wrench-iron"] = (long)400,
        ["game:wrench-meteoriciron"] = (long)700,
        ["game:wrench-steel"] = (long)900,
        ["game:saw-copper"] = (long)100,
        ["game:saw-tinbronze"] = (long)200,
        ["game:saw-bismuthbronze"] = (long)300,
        ["game:saw-blackbronze"] = (long)400,
        ["game:saw-gold"] = (long)500,
        ["game:saw-silver"] = (long)600,
        ["game:saw-iron"] = (long)400,
        ["game:saw-meteoriciron"] = (long)700,
        ["game:saw-steel"] = (long)900,
        ["game:scythe-copper"] = (long)100,
        ["game:scythe-tinbronze"] = (long)200,
        ["game:scythe-bismuthbronze"] = (long)300,
        ["game:scythe-blackbronze"] = (long)400,
        ["game:scythe-gold"] = (long)500,
        ["game:scythe-silver"] = (long)600,
        ["game:scythe-iron"] = (long)400,
        ["game:scythe-meteoriciron"] = (long)700,
        ["game:scythe-steel"] = (long)900,
        ["game:cleaver-copper"] = (long)100,
        ["game:cleaver-tinbronze"] = (long)200,
        ["game:cleaver-bismuthbronze"] = (long)300,
        ["game:cleaver-blackbronze"] = (long)400,
        ["game:cleaver-gold"] = (long)500,
        ["game:cleaver-silver"] = (long)600,
        ["game:cleaver-iron"] = (long)400,
        ["game:cleaver-meteoriciron"] = (long)700,
        ["game:cleaver-steel"] = (long)900,
        ["game:arrow-crude"] = (long)10,
        ["game:arrow-flint"] = (long)10,
        ["game:arrow-copper"] = (long)20,
        ["game:arrow-tinbronze"] = (long)30,
        ["game:arrow-bismuthbronze"] = (long)40,
        ["game:arrow-blackbronze"] = (long)50,
        ["game:arrow-gold"] = (long)60,
        ["game:arrow-silver"] = (long)70,
        ["game:arrow-iron"] = (long)80,
        ["game:arrow-meteoriciron"] = (long)90,
        ["game:arrow-steel"] = (long)100,
        // Armors Below (Armors needs to have the level type before the code, so the mod can understand which type is the armor from)
        // Brigandine Armor    
        ["BrigandineArmor?game:armor-head-brigandine-copper"] = (long)100,
        ["BrigandineArmor?game:armor-body-brigandine-copper"] = (long)100,
        ["BrigandineArmor?game:armor-legs-brigandine-copper"] = (long)100,
        ["BrigandineArmor?game:armor-head-brigandine-tinbronze"] = (long)200,
        ["BrigandineArmor?game:armor-body-brigandine-tinbronze"] = (long)200,
        ["BrigandineArmor?game:armor-legs-brigandine-tinbronze"] = (long)200,
        ["BrigandineArmor?game:armor-head-brigandine-bismuthbronze"] = (long)300,
        ["BrigandineArmor?game:armor-body-brigandine-bismuthbronze"] = (long)300,
        ["BrigandineArmor?game:armor-legs-brigandine-bismuthbronze"] = (long)300,
        ["BrigandineArmor?game:armor-head-brigandine-blackbronze"] = (long)400,
        ["BrigandineArmor?game:armor-body-brigandine-blackbronze"] = (long)400,
        ["BrigandineArmor?game:armor-legs-brigandine-blackbronze"] = (long)400,
        ["BrigandineArmor?game:armor-head-brigandine-iron"] = (long)500,
        ["BrigandineArmor?game:armor-body-brigandine-iron"] = (long)500,
        ["BrigandineArmor?game:armor-legs-brigandine-iron"] = (long)500,
        ["BrigandineArmor?game:armor-head-brigandine-meteoriciron"] = (long)600,
        ["BrigandineArmor?game:armor-body-brigandine-meteoriciron"] = (long)600,
        ["BrigandineArmor?game:armor-legs-brigandine-meteoriciron"] = (long)600,
        ["BrigandineArmor?game:armor-head-brigandine-steel"] = (long)1000,
        ["BrigandineArmor?game:armor-body-brigandine-steel"] = (long)1000,
        ["BrigandineArmor?game:armor-legs-brigandine-steel"] = (long)1000,
        // Chain Armor
        ["ChainArmor?game:armor-head-chain-copper"] = (long)100,
        ["ChainArmor?game:armor-body-chain-copper"] = (long)100,
        ["ChainArmor?game:armor-legs-chain-copper"] = (long)100,
        ["ChainArmor?game:armor-head-chain-tinbronze"] = (long)200,
        ["ChainArmor?game:armor-body-chain-tinbronze"] = (long)200,
        ["ChainArmor?game:armor-legs-chain-tinbronze"] = (long)200,
        ["ChainArmor?game:armor-head-chain-bismuthbronze"] = (long)300,
        ["ChainArmor?game:armor-body-chain-bismuthbronze"] = (long)300,
        ["ChainArmor?game:armor-legs-chain-bismuthbronze"] = (long)300,
        ["ChainArmor?game:armor-head-chain-blackbronze"] = (long)400,
        ["ChainArmor?game:armor-body-chain-blackbronze"] = (long)400,
        ["ChainArmor?game:armor-legs-chain-blackbronze"] = (long)400,
        ["ChainArmor?game:armor-head-chain-iron"] = (long)500,
        ["ChainArmor?game:armor-body-chain-iron"] = (long)500,
        ["ChainArmor?game:armor-legs-chain-iron"] = (long)500,
        ["ChainArmor?game:armor-head-chain-meteoriciron"] = (long)600,
        ["ChainArmor?game:armor-body-chain-meteoriciron"] = (long)600,
        ["ChainArmor?game:armor-legs-chain-meteoriciron"] = (long)600,
        ["ChainArmor?game:armor-head-chain-steel"] = (long)1000,
        ["ChainArmor?game:armor-body-chain-steel"] = (long)1000,
        ["ChainArmor?game:armor-legs-chain-steel"] = (long)1000,
        ["ChainArmor?game:armor-head-chain-gold"] = (long)700,
        ["ChainArmor?game:armor-body-chain-gold"] = (long)700,
        ["ChainArmor?game:armor-legs-chain-gold"] = (long)700,
        ["ChainArmor?game:armor-head-chain-silver"] = (long)700,
        ["ChainArmor?game:armor-body-chain-silver"] = (long)700,
        ["ChainArmor?game:armor-legs-chain-silver"] = (long)700,
        // Leather Armor    
        ["LeatherArmor?game:armor-head-sewn-leather"] = (long)150,
        ["LeatherArmor?game:armor-body-sewn-leather"] = (long)200,
        ["LeatherArmor?game:armor-legs-sewn-leather"] = (long)150,
        ["LeatherArmor?game:clothes-shoulder-stained-leather-poncho"] = (long)100,
        ["LeatherArmor?game:clothes-hand-heavy-leather-gloves"] = (long)100,
        ["LeatherArmor?game:clothes-upperbodyover-malefactor-tunic"] = (long)100,
        ["LeatherArmor?game:armor-body-jerkin-leather"] = (long)50,
        ["LeatherArmor?game:armor-legs-jerkin-leather"] = (long)50,
        ["LeatherArmor?game:clothes-foot-high-leather-boots"] = (long)75,
        ["LeatherArmor?game:clothes-upperbody-raw-hide-mantle"] = (long)75,
        ["LeatherArmor?game:clothes-lowerbody-raw-hide-trousers"] = (long)75,
        ["LeatherArmor?game:clothes-foot-knee-high-fur-boots"] = (long)75,
        ["LeatherArmor?game:clothes-hand-fur-gloves"] = (long)75,
        ["LeatherArmor?game:clothes-upperbodyover-fur-coat"] = (long)50,
        ["LeatherArmor?game:clothes-upperbodyover-warm-robe"] = (long)50,
        ["LeatherArmor?game:clothes-upperbodyover-reindeer-herder-fur-coat"] = (long)50,
        ["LeatherArmor?game:clothes-foot-fur-lined-reindeer-herder-shoes"] = (long)50,
        // Plate Armor    
        ["PlateArmor?game:armor-head-plate-copper"] = (long)100,
        ["PlateArmor?game:armor-body-plate-copper"] = (long)100,
        ["PlateArmor?game:armor-legs-plate-copper"] = (long)100,
        ["PlateArmor?game:armor-head-plate-tinbronze"] = (long)200,
        ["PlateArmor?game:armor-body-plate-tinbronze"] = (long)200,
        ["PlateArmor?game:armor-legs-plate-tinbronze"] = (long)200,
        ["PlateArmor?game:armor-head-plate-bismuthbronze"] = (long)300,
        ["PlateArmor?game:armor-body-plate-bismuthbronze"] = (long)300,
        ["PlateArmor?game:armor-legs-plate-bismuthbronze"] = (long)300,
        ["PlateArmor?game:armor-head-plate-blackbronze"] = (long)400,
        ["PlateArmor?game:armor-body-plate-blackbronze"] = (long)400,
        ["PlateArmor?game:armor-legs-plate-blackbronze"] = (long)400,
        ["PlateArmor?game:armor-head-plate-iron"] = (long)500,
        ["PlateArmor?game:armor-body-plate-iron"] = (long)500,
        ["PlateArmor?game:armor-legs-plate-iron"] = (long)500,
        ["PlateArmor?game:armor-head-plate-meteoriciron"] = (long)600,
        ["PlateArmor?game:armor-body-plate-meteoriciron"] = (long)600,
        ["PlateArmor?game:armor-legs-plate-meteoriciron"] = (long)600,
        ["PlateArmor?game:armor-head-plate-steel"] = (long)1000,
        ["PlateArmor?game:armor-body-plate-steel"] = (long)1000,
        ["PlateArmor?game:armor-legs-plate-steel"] = (long)1000,
        ["PlateArmor?game:armor-head-plate-gold"] = (long)700,
        ["PlateArmor?game:armor-body-plate-gold"] = (long)700,
        ["PlateArmor?game:armor-legs-plate-gold"] = (long)700,
        ["PlateArmor?game:armor-head-plate-silver"] = (long)700,
        ["PlateArmor?game:armor-body-plate-silver"] = (long)700,
        ["PlateArmor?game:armor-legs-plate-silver"] = (long)700,
        // Scale Armor
        ["ScaleArmor?game:armor-head-scale-copper"] = (long)100,
        ["ScaleArmor?game:armor-body-scale-copper"] = (long)100,
        ["ScaleArmor?game:armor-legs-scale-copper"] = (long)100,
        ["ScaleArmor?game:armor-head-scale-tinbronze"] = (long)200,
        ["ScaleArmor?game:armor-body-scale-tinbronze"] = (long)200,
        ["ScaleArmor?game:armor-legs-scale-tinbronze"] = (long)200,
        ["ScaleArmor?game:armor-head-scale-bismuthbronze"] = (long)300,
        ["ScaleArmor?game:armor-body-scale-bismuthbronze"] = (long)300,
        ["ScaleArmor?game:armor-legs-scale-bismuthbronze"] = (long)300,
        ["ScaleArmor?game:armor-head-scale-blackbronze"] = (long)400,
        ["ScaleArmor?game:armor-body-scale-blackbronze"] = (long)400,
        ["ScaleArmor?game:armor-legs-scale-blackbronze"] = (long)400,
        ["ScaleArmor?game:armor-head-scale-iron"] = (long)500,
        ["ScaleArmor?game:armor-body-scale-iron"] = (long)500,
        ["ScaleArmor?game:armor-legs-scale-iron"] = (long)500,
        ["ScaleArmor?game:armor-head-scale-meteoriciron"] = (long)600,
        ["ScaleArmor?game:armor-body-scale-meteoriciron"] = (long)600,
        ["ScaleArmor?game:armor-legs-scale-meteoriciron"] = (long)600,
        ["ScaleArmor?game:armor-head-scale-steel"] = (long)1000,
        ["ScaleArmor?game:armor-body-scale-steel"] = (long)1000,
        ["ScaleArmor?game:armor-legs-scale-steel"] = (long)1000,
        // Shield generic
        ["Shield?game:shield*"] = (long)100,
    };

    public static void PopulateSmithingConfiguration(ICoreAPI api)
    {
        Dictionary<string, object> smithingLevelStats = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/levelstats",
            "smithing",
            BuildSmithingDefaultConfig());
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
        { //smithingSubLevelEXPMultiply
            if (smithingLevelStats.TryGetValue("smithingSubLevelEXPMultiply", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: smithingSubLevelEXPMultiply is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: smithingSubLevelEXPMultiply is not double is {value.GetType()}");
                else smithingSubLevelEXPMultiply = (double)value;
            else Debug.LogError("CONFIGURATION ERROR: smithingSubLevelEXPMultiply not set");
        }


        // Get crop exp
        expPerCraftSmithing.Clear();
        Dictionary<string, object> tmpexpPerCraftSmithing = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/levelstats",
            "smithingcrafts",
            BuildSmithingCraftsDefaultConfig());
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

    #region quenching
    private static int quenchingEXPPerLevelBase = 300;
    private static double quenchingEXPMultiplyPerLevel = 1.1;
    public static ulong quenchingBaseExpPerQuench = 50;
    public static ulong quenchingBaseExpPerTemper = 40;
    private static float quenchingBaseShatterChanceAddedMultiply = 1.0f;
    private static float quenchingReduceShatterChanceAddedMultiplyPerLevel = 0.01f;
    private static float quenchingMinShatterChanceAddedMultiply = 0.1f;
    private static float quenchingBasePowerGainMultiply = 1.0f;
    private static float quenchingIncrementPowerGainMultiplyPerLevel = 0.01f;
    private static float quenchingBaseTemperEfficiencyMultiply = 1.0f;
    private static float quenchingIncrementTemperEfficiencyMultiplyPerLevel = 0.01f;
    public static int quenchingMaxLevel = 999;
    public static double quenchingSubLevelEXPMultiply = 3.0;

    private static Dictionary<string, object> BuildQuenchingDefaultConfig() => new()
    {
        ["quenchingEXPPerLevelBase"] = (long)quenchingEXPPerLevelBase,
        ["quenchingEXPMultiplyPerLevel"] = quenchingEXPMultiplyPerLevel,
        ["quenchingBaseExpPerQuench"] = (long)quenchingBaseExpPerQuench,
        ["quenchingBaseExpPerTemper"] = (long)quenchingBaseExpPerTemper,
        ["quenchingBaseShatterChanceAddedMultiply"] = (double)quenchingBaseShatterChanceAddedMultiply,
        ["quenchingReduceShatterChanceAddedMultiplyPerLevel"] = (double)quenchingReduceShatterChanceAddedMultiplyPerLevel,
        ["quenchingMinShatterChanceAddedMultiply"] = (double)quenchingMinShatterChanceAddedMultiply,
        ["quenchingBasePowerGainMultiply"] = (double)quenchingBasePowerGainMultiply,
        ["quenchingIncrementPowerGainMultiplyPerLevel"] = (double)quenchingIncrementPowerGainMultiplyPerLevel,
        ["quenchingBaseTemperEfficiencyMultiply"] = (double)quenchingBaseTemperEfficiencyMultiply,
        ["quenchingIncrementTemperEfficiencyMultiplyPerLevel"] = (double)quenchingIncrementTemperEfficiencyMultiplyPerLevel,
        ["quenchingMaxLevel"] = (long)quenchingMaxLevel,
        ["quenchingSubLevelEXPMultiply"] = quenchingSubLevelEXPMultiply,
    };

    public static void PopulateQuenchingConfiguration(ICoreAPI api)
    {
        Dictionary<string, object> quenchingLevelStats = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/levelstats",
            "quenching",
            BuildQuenchingDefaultConfig());
        { //quenchingEXPPerLevelBase
            if (quenchingLevelStats.TryGetValue("quenchingEXPPerLevelBase", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: quenchingEXPPerLevelBase is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: quenchingEXPPerLevelBase is not int is {value.GetType()}");
                else quenchingEXPPerLevelBase = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: quenchingEXPPerLevelBase not set");
        }
        { //quenchingEXPMultiplyPerLevel
            if (quenchingLevelStats.TryGetValue("quenchingEXPMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: quenchingEXPMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: quenchingEXPMultiplyPerLevel is not double is {value.GetType()}");
                else quenchingEXPMultiplyPerLevel = (double)value;
            else Debug.LogError("CONFIGURATION ERROR: quenchingEXPMultiplyPerLevel not set");
        }
        { //quenchingBaseExpPerQuench
            if (quenchingLevelStats.TryGetValue("quenchingBaseExpPerQuench", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: quenchingBaseExpPerQuench is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: quenchingBaseExpPerQuench is not int is {value.GetType()}");
                else quenchingBaseExpPerQuench = (ulong)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: quenchingBaseExpPerQuench not set");
        }
        { //quenchingBaseExpPerTemper
            if (quenchingLevelStats.TryGetValue("quenchingBaseExpPerTemper", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: quenchingBaseExpPerTemper is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: quenchingBaseExpPerTemper is not int is {value.GetType()}");
                else quenchingBaseExpPerTemper = (ulong)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: quenchingBaseExpPerTemper not set");
        }
        { //quenchingBaseShatterChanceAddedMultiply
            if (quenchingLevelStats.TryGetValue("quenchingBaseShatterChanceAddedMultiply", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: quenchingBaseShatterChanceAddedMultiply is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: quenchingBaseShatterChanceAddedMultiply is not double is {value.GetType()}");
                else quenchingBaseShatterChanceAddedMultiply = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: quenchingBaseShatterChanceAddedMultiply not set");
        }
        { //quenchingReduceShatterChanceAddedMultiplyPerLevel
            if (quenchingLevelStats.TryGetValue("quenchingReduceShatterChanceAddedMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: quenchingReduceShatterChanceAddedMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: quenchingReduceShatterChanceAddedMultiplyPerLevel is not double is {value.GetType()}");
                else quenchingReduceShatterChanceAddedMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: quenchingReduceShatterChanceAddedMultiplyPerLevel not set");
        }
        { //quenchingMinShatterChanceAddedMultiply
            if (quenchingLevelStats.TryGetValue("quenchingMinShatterChanceAddedMultiply", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: quenchingMinShatterChanceAddedMultiply is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: quenchingMinShatterChanceAddedMultiply is not double is {value.GetType()}");
                else quenchingMinShatterChanceAddedMultiply = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: quenchingMinShatterChanceAddedMultiply not set");
        }
        { //quenchingBasePowerGainMultiply
            if (quenchingLevelStats.TryGetValue("quenchingBasePowerGainMultiply", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: quenchingBasePowerGainMultiply is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: quenchingBasePowerGainMultiply is not double is {value.GetType()}");
                else quenchingBasePowerGainMultiply = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: quenchingBasePowerGainMultiply not set");
        }
        { //quenchingIncrementPowerGainMultiplyPerLevel
            if (quenchingLevelStats.TryGetValue("quenchingIncrementPowerGainMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: quenchingIncrementPowerGainMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: quenchingIncrementPowerGainMultiplyPerLevel is not double is {value.GetType()}");
                else quenchingIncrementPowerGainMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: quenchingIncrementPowerGainMultiplyPerLevel not set");
        }
        { //quenchingBaseTemperEfficiencyMultiply
            if (quenchingLevelStats.TryGetValue("quenchingBaseTemperEfficiencyMultiply", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: quenchingBaseTemperEfficiencyMultiply is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: quenchingBaseTemperEfficiencyMultiply is not double is {value.GetType()}");
                else quenchingBaseTemperEfficiencyMultiply = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: quenchingBaseTemperEfficiencyMultiply not set");
        }
        { //quenchingIncrementTemperEfficiencyMultiplyPerLevel
            if (quenchingLevelStats.TryGetValue("quenchingIncrementTemperEfficiencyMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: quenchingIncrementTemperEfficiencyMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: quenchingIncrementTemperEfficiencyMultiplyPerLevel is not double is {value.GetType()}");
                else quenchingIncrementTemperEfficiencyMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: quenchingIncrementTemperEfficiencyMultiplyPerLevel not set");
        }
        { //quenchingMaxLevel
            if (quenchingLevelStats.TryGetValue("quenchingMaxLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: quenchingMaxLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: quenchingMaxLevel is not int is {value.GetType()}");
                else quenchingMaxLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: quenchingMaxLevel not set");
        }
        { //quenchingSubLevelEXPMultiply
            if (quenchingLevelStats.TryGetValue("quenchingSubLevelEXPMultiply", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: quenchingSubLevelEXPMultiply is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: quenchingSubLevelEXPMultiply is not double is {value.GetType()}");
                else quenchingSubLevelEXPMultiply = (double)value;
            else Debug.LogError("CONFIGURATION ERROR: quenchingSubLevelEXPMultiply not set");
        }

        Experience.LoadExperience("Quenching", "Quench", quenchingBaseExpPerQuench);
        Experience.LoadExperience("Quenching", "Temper", quenchingBaseExpPerTemper);

        Debug.Log("Quenching configuration set");
    }

    public static int QuenchingGetLevelByEXP(ulong exp)
    {
        double baseExp = quenchingEXPPerLevelBase;
        double multiplier = quenchingEXPMultiplyPerLevel;

        if (multiplier <= 1.0)
        {
            return (int)(exp / baseExp);
        }

        double expDouble = exp;

        double level = Math.Log((expDouble * (multiplier - 1) / baseExp) + 1) / Math.Log(multiplier);

        return Math.Max(0, (int)Math.Floor(level));
    }

    public static ulong QuenchingGetExpByLevel(int level)
    {
        double baseExp = quenchingEXPPerLevelBase;
        double multiplier = quenchingEXPMultiplyPerLevel;

        if (multiplier == 1.0)
        {
            return (ulong)(baseExp * level);
        }

        double exp = baseExp * (Math.Pow(multiplier, level) - 1) / (multiplier - 1);
        return (ulong)Math.Floor(exp);
    }

    /// <summary>
    /// Multiplier applied on top of the shatterchance delta the vanilla quench just added.
    /// Decreases with level (safer quenching), floored at quenchingMinShatterChanceAddedMultiply.
    /// </summary>
    public static float QuenchingGetShatterChanceAddedMultiplyByLevel(int level)
    {
        return Math.Max(quenchingMinShatterChanceAddedMultiply, quenchingBaseShatterChanceAddedMultiply * (1 - quenchingReduceShatterChanceAddedMultiplyPerLevel * level));
    }

    /// <summary>
    /// Multiplier applied on top of the power/durability bonus delta the vanilla quench just added.
    /// </summary>
    public static float QuenchingGetPowerGainMultiplyByLevel(int level)
    {
        return quenchingBasePowerGainMultiply * (1 + quenchingIncrementPowerGainMultiplyPerLevel * level);
    }

    /// <summary>
    /// Multiplier applied on top of the (negative) shatterchance delta the vanilla temper just subtracted.
    /// </summary>
    public static float QuenchingGetTemperEfficiencyMultiplyByLevel(int level)
    {
        return quenchingBaseTemperEfficiencyMultiply * (1 + quenchingIncrementTemperEfficiencyMultiplyPerLevel * level);
    }
    #endregion

    #region vitality
    private static int vitalityEXPPerReceiveHit = 10;
    private static float vitalityEXPMultiplyByDamage = 0.3f;
    private static int vitalityEXPIncreaseByAmountDamage = 2;
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

    private static Dictionary<string, object> BuildVitalityDefaultConfig() => new()
    {
        ["vitalityEXPPerLevelBase"] = (long)vitalityEXPPerLevelBase,
        ["vitalityEXPMultiplyPerLevel"] = vitalityEXPMultiplyPerLevel,
        ["vitalityEXPPerReceiveHit"] = (long)vitalityEXPPerReceiveHit,
        ["vitalityEXPMultiplyByDamage"] = (double)vitalityEXPMultiplyByDamage,
        ["vitalityHPIncreasePerLevel"] = (double)vitalityHPIncreasePerLevel,
        ["vitalityBaseHP"] = (double)vitalityBaseHP,
        ["vitalityEXPIncreaseByAmountDamage"] = (long)vitalityEXPIncreaseByAmountDamage,
        ["vitalityBaseHPRegen"] = (double)vitalityBaseHPRegen,
        ["vitalityHPRegenIncreasePerLevel"] = (double)vitalityHPRegenIncreasePerLevel,
        ["vitalityDamageLimit"] = (long)vitalityDamageLimit,
        ["vitalityMaxLevel"] = (long)vitalityMaxLevel,
    };

    public static void PopulateVitalityConfiguration(ICoreAPI api)
    {
        Dictionary<string, object> vitalityLevelStats = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/levelstats",
            "vitality",
            BuildVitalityDefaultConfig());
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
    private static int metabolismEXPPerReceiveHit = 100;
    private static float metabolismEXPPerSaturationLost = 0.11f;
    private static int metabolismEXPPerLevelBase = 500;
    private static double metabolismEXPMultiplyPerLevel = 1.33;
    private static float metabolismSaturationIncreasePerLevel = 50.0f;
    private static float metabolismBaseSaturation = 1500.0f;
    private static float metabolismBaseSaturationReceiveMultiply = 1.0f;
    private static float metabolismSaturationReceiveMultiplyPerLevel = 0.05f;
    private static int metabolismSaturationReceiveMultiplyReductionEveryLevel = 1;
    private static float metabolismSaturationReceiveMultiplyReductionPerReduce = 0.05f;
    public static int metabolismMaxLevel = 999;

    public static int EXPPerHitMetabolism => metabolismEXPPerReceiveHit;
    public static float EXPPerSaturationLostMetabolism => metabolismEXPPerSaturationLost;

    public static float BaseSaturationMetabolism => metabolismBaseSaturation;

    private static Dictionary<string, object> BuildMetabolismDefaultConfig() => new()
    {
        ["metabolismEXPPerReceiveHit"] = (long)metabolismEXPPerReceiveHit,
        ["metabolismEXPPerSaturationLost"] = (double)metabolismEXPPerSaturationLost,
        ["metabolismEXPPerLevelBase"] = (long)metabolismEXPPerLevelBase,
        ["metabolismEXPMultiplyPerLevel"] = metabolismEXPMultiplyPerLevel,
        ["metabolismSaturationIncreasePerLevel"] = (double)metabolismSaturationIncreasePerLevel,
        ["metabolismBaseSaturation"] = (double)metabolismBaseSaturation,
        ["metabolismBaseSaturationReceiveMultiply"] = (double)metabolismBaseSaturationReceiveMultiply,
        ["metabolismSaturationReceiveMultiplyPerLevel"] = (double)metabolismSaturationReceiveMultiplyPerLevel,
        ["metabolismSaturationReceiveMultiplyReductionEveryLevel"] = (long)metabolismSaturationReceiveMultiplyReductionEveryLevel,
        ["metabolismSaturationReceiveMultiplyReductionPerReduce"] = (double)metabolismSaturationReceiveMultiplyReductionPerReduce,
        ["metabolismMaxLevel"] = (long)metabolismMaxLevel,
    };

    public static void PopulateMetabolismConfiguration(ICoreAPI api)
    {
        Dictionary<string, object> metabolismLevelStats = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/levelstats",
            "metabolism",
            BuildMetabolismDefaultConfig());
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
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: metabolismEXPPerSaturationLost is not double is {value.GetType()}");
                else metabolismEXPPerSaturationLost = (float)(double)value;
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
        { //metabolismSaturationReceiveMultiplyReductionEveryLevel
            if (metabolismLevelStats.TryGetValue("metabolismSaturationReceiveMultiplyReductionEveryLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: metabolismSaturationReceiveMultiplyReductionEveryLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: metabolismSaturationReceiveMultiplyReductionEveryLevel is not int is {value.GetType()}");
                else metabolismSaturationReceiveMultiplyReductionEveryLevel = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: metabolismSaturationReceiveMultiplyReductionEveryLevel not set");
        }
        { //metabolismSaturationReceiveMultiplyReductionPerReduce
            if (metabolismLevelStats.TryGetValue("metabolismSaturationReceiveMultiplyReductionPerReduce", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: metabolismSaturationReceiveMultiplyReductionPerReduce is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: metabolismSaturationReceiveMultiplyReductionPerReduce is not double is {value.GetType()}");
                else metabolismSaturationReceiveMultiplyReductionPerReduce = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: metabolismSaturationReceiveMultiplyReductionPerReduce not set");
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
    private static int leatherArmorEXPIncreaseByAmountDamage = 2;
    private static int leatherArmorEXPPerLevelBase = 500;
    private static double leatherArmorEXPMultiplyPerLevel = 1.2;

    private static float leatherArmorRelativeProtectionMultiply = 1.0f;
    private static float leatherArmorRelativeProtectionMultiplyPerLevel = 0.015f;
    private static int leatherArmorRelativeProtectionMultiplyReductionEveryLevel = 1;
    private static float leatherArmorRelativeProtectionMultiplyReductionPerReduce = 0.25f;

    private static float leatherArmorFlatDamageReductionMultiply = 1.0f;
    private static float leatherArmorFlatDamageReductionMultiplyPerLevel = 0.015f;
    private static int leatherArmorFlatDamageReductionMultiplyReductionEveryLevel = 1;
    private static float leatherArmorFlatDamageReductionMultiplyReductionPerReduce = 0.05f;

    private static float leatherArmorHealingEffectivnessMultiply = 1.0f;
    private static float leatherArmorHealingEffectivnessMultiplyPerLevel = 0.035f;
    private static int leatherArmorHealingEffectivnessMultiplyReductionEveryLevel = 1;
    private static float leatherArmorHealingEffectivnessMultiplyReductionPerReduce = 0.05f;

    private static float leatherArmorHungerRateMultiply = 1.0f;
    private static float leatherArmorHungerRateMultiplyPerLevel = 0.02f;
    private static int leatherArmorHungerRateMultiplyReductionEveryLevel = 1;
    private static float leatherArmorHungerRateMultiplyReductionPerReduce = 0.05f;

    private static float leatherArmorRangedWeaponsAccuracyMultiply = 1.0f;
    private static float leatherArmorRangedWeaponsAccuracyMultiplyPerLevel = 0.075f;
    private static int leatherArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel = 1;
    private static float leatherArmorRangedWeaponsAccuracyMultiplyReductionPerReduce = 0.05f;

    private static float leatherArmorRangedWeaponsSpeedMultiply = 1.0f;
    private static float leatherArmorRangedWeaponsSpeedMultiplyPerLevel = 0.075f;
    private static int leatherArmorRangedWeaponsSpeedMultiplyReductionEveryLevel = 1;
    private static float leatherArmorRangedWeaponsSpeedMultiplyReductionPerReduce = 0.05f;

    private static float leatherArmorWalkSpeedMultiply = 1.0f;
    private static float leatherArmorWalkSpeedMultiplyPerLevel = 0.06f;
    private static int leatherArmorWalkSpeedMultiplyReductionEveryLevel = 1;
    private static float leatherArmorWalkSpeedMultiplyReductionPerReduce = 0.05f;

    public static int leatherArmorMaxLevel = 999;
    public static double leatherArmorSubLevelEXPMultiply = 3.0;

    private static Dictionary<string, object> BuildLeatherArmorDefaultConfig() => new()
    {
        ["leatherArmorEXPPerReceiveHit"] = (long)leatherArmorEXPPerReceiveHit,
        ["leatherArmorEXPMultiplyByDamage"] = (double)leatherArmorEXPMultiplyByDamage,
        ["leatherArmorEXPIncreaseByAmountDamage"] = (long)leatherArmorEXPIncreaseByAmountDamage,
        ["leatherArmorEXPPerLevelBase"] = (long)leatherArmorEXPPerLevelBase,
        ["leatherArmorEXPMultiplyPerLevel"] = leatherArmorEXPMultiplyPerLevel,
        ["leatherArmorRelativeProtectionMultiply"] = (double)leatherArmorRelativeProtectionMultiply,
        ["leatherArmorRelativeProtectionMultiplyPerLevel"] = (double)leatherArmorRelativeProtectionMultiplyPerLevel,
        ["leatherArmorRelativeProtectionMultiplyReductionEveryLevel"] = (long)leatherArmorRelativeProtectionMultiplyReductionEveryLevel,
        ["leatherArmorRelativeProtectionMultiplyReductionPerReduce"] = (double)leatherArmorRelativeProtectionMultiplyReductionPerReduce,
        ["leatherArmorFlatDamageReductionMultiply"] = (double)leatherArmorFlatDamageReductionMultiply,
        ["leatherArmorFlatDamageReductionMultiplyPerLevel"] = (double)leatherArmorFlatDamageReductionMultiplyPerLevel,
        ["leatherArmorFlatDamageReductionMultiplyReductionEveryLevel"] = (long)leatherArmorFlatDamageReductionMultiplyReductionEveryLevel,
        ["leatherArmorFlatDamageReductionMultiplyReductionPerReduce"] = (double)leatherArmorFlatDamageReductionMultiplyReductionPerReduce,
        ["leatherArmorHealingEffectivnessMultiply"] = (double)leatherArmorHealingEffectivnessMultiply,
        ["leatherArmorHealingEffectivnessMultiplyPerLevel"] = (double)leatherArmorHealingEffectivnessMultiplyPerLevel,
        ["leatherArmorHealingEffectivnessMultiplyReductionEveryLevel"] = (long)leatherArmorHealingEffectivnessMultiplyReductionEveryLevel,
        ["leatherArmorHealingEffectivnessMultiplyReductionPerReduce"] = (double)leatherArmorHealingEffectivnessMultiplyReductionPerReduce,
        ["leatherArmorHungerRateMultiply"] = (double)leatherArmorHungerRateMultiply,
        ["leatherArmorHungerRateMultiplyPerLevel"] = (double)leatherArmorHungerRateMultiplyPerLevel,
        ["leatherArmorHungerRateMultiplyReductionEveryLevel"] = (long)leatherArmorHungerRateMultiplyReductionEveryLevel,
        ["leatherArmorHungerRateMultiplyReductionPerReduce"] = (double)leatherArmorHungerRateMultiplyReductionPerReduce,
        ["leatherArmorRangedWeaponsAccuracyMultiply"] = (double)leatherArmorRangedWeaponsAccuracyMultiply,
        ["leatherArmorRangedWeaponsAccuracyMultiplyPerLevel"] = (double)leatherArmorRangedWeaponsAccuracyMultiplyPerLevel,
        ["leatherArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel"] = (long)leatherArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel,
        ["leatherArmorRangedWeaponsAccuracyMultiplyReductionPerReduce"] = (double)leatherArmorRangedWeaponsAccuracyMultiplyReductionPerReduce,
        ["leatherArmorRangedWeaponsSpeedMultiply"] = (double)leatherArmorRangedWeaponsSpeedMultiply,
        ["leatherArmorRangedWeaponsSpeedMultiplyPerLevel"] = (double)leatherArmorRangedWeaponsSpeedMultiplyPerLevel,
        ["leatherArmorRangedWeaponsSpeedMultiplyReductionEveryLevel"] = (long)leatherArmorRangedWeaponsSpeedMultiplyReductionEveryLevel,
        ["leatherArmorRangedWeaponsSpeedMultiplyReductionPerReduce"] = (double)leatherArmorRangedWeaponsSpeedMultiplyReductionPerReduce,
        ["leatherArmorWalkSpeedMultiply"] = (double)leatherArmorWalkSpeedMultiply,
        ["leatherArmorWalkSpeedMultiplyPerLevel"] = (double)leatherArmorWalkSpeedMultiplyPerLevel,
        ["leatherArmorWalkSpeedMultiplyReductionEveryLevel"] = (long)leatherArmorWalkSpeedMultiplyReductionEveryLevel,
        ["leatherArmorWalkSpeedMultiplyReductionPerReduce"] = (double)leatherArmorWalkSpeedMultiplyReductionPerReduce,
        ["leatherArmorMaxLevel"] = (long)leatherArmorMaxLevel,
        ["leatherArmorSubLevelEXPMultiply"] = leatherArmorSubLevelEXPMultiply,
    };

    private static Dictionary<string, object> BuildLeatherArmorItemsDefaultConfig() => new()
    {
        ["game:armor-head-sewn-leather"] = 0.2,
        ["game:armor-body-sewn-leather"] = 0.5,
        ["game:armor-legs-sewn-leather"] = 0.2,
        ["game:armor-body-jerkin-leather"] = 0.4,
        ["game:armor-legs-jerkin-leather"] = 0.2,
        ["game:armor-head-hide-bear-black"] = 0.3,
        ["game:armor-body-hide-bear-black"] = 0.5,
        ["game:armor-legs-hide-bear-black"] = 0.2,
        ["game:armor-head-hide-bear-brown"] = 0.3,
        ["game:armor-body-hide-bear-brown"] = 0.5,
        ["game:armor-legs-hide-bear-brown"] = 0.2,
        ["game:armor-head-hide-bear-polar"] = 0.3,
        ["game:armor-body-hide-bear-polar"] = 0.5,
        ["game:armor-legs-hide-bear-polar"] = 0.2,
        ["game:armor-head-hide-bear-sun"] = 0.3,
        ["game:armor-body-hide-bear-sun"] = 0.5,
        ["game:armor-legs-hide-bear-sun"] = 0.2,
    };

    public static void PopulateLeatherArmorConfiguration(ICoreAPI api)
    {
        Dictionary<string, object> leatherArmorLevelStats = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/levelstats",
            "leatherarmor",
            BuildLeatherArmorDefaultConfig());
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
        { //leatherArmorSubLevelEXPMultiply
            if (leatherArmorLevelStats.TryGetValue("leatherArmorSubLevelEXPMultiply", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: leatherArmorSubLevelEXPMultiply is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: leatherArmorSubLevelEXPMultiply is not double is {value.GetType()}");
                else leatherArmorSubLevelEXPMultiply = (double)value;
            else Debug.LogError("CONFIGURATION ERROR: leatherArmorSubLevelEXPMultiply not set");
        }

        // Get leather armor multiply exp
        expMultiplyHitLeatherArmor.Clear();
        Dictionary<string, object> tmpexpMultiplyHitLeatherArmor = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/levelstats",
            "leatherarmoritems",
            BuildLeatherArmorItemsDefaultConfig());
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
    private static int chainArmorEXPIncreaseByAmountDamage = 2;
    private static int chainArmorEXPPerLevelBase = 500;
    private static double chainArmorEXPMultiplyPerLevel = 1.2;

    private static float chainArmorRelativeProtectionMultiply = 1.0f;
    private static float chainArmorRelativeProtectionMultiplyPerLevel = 0.025f;
    private static int chainArmorRelativeProtectionMultiplyReductionEveryLevel = 1;
    private static float chainArmorRelativeProtectionMultiplyReductionPerReduce = 0.20f;

    private static float chainArmorFlatDamageReductionMultiply = 1.0f;
    private static float chainArmorFlatDamageReductionMultiplyPerLevel = 0.025f;
    private static int chainArmorFlatDamageReductionMultiplyReductionEveryLevel = 1;
    private static float chainArmorFlatDamageReductionMultiplyReductionPerReduce = 0.05f;

    private static float chainArmorHealingEffectivnessMultiply = 1.0f;
    private static float chainArmorHealingEffectivnessMultiplyPerLevel = 0.04f;
    private static int chainArmorHealingEffectivnessMultiplyReductionEveryLevel = 1;
    private static float chainArmorHealingEffectivnessMultiplyReductionPerReduce = 0.05f;

    private static float chainArmorHungerRateMultiply = 1.0f;
    private static float chainArmorHungerRateMultiplyPerLevel = 0.03f;
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
    private static float chainArmorWalkSpeedMultiplyPerLevel = 0.04f;
    private static int chainArmorWalkSpeedMultiplyReductionEveryLevel = 1;
    private static float chainArmorWalkSpeedMultiplyReductionPerReduce = 0.05f;

    public static int chainArmorMaxLevel = 999;
    public static double chainArmorSubLevelEXPMultiply = 3.0;

    private static Dictionary<string, object> BuildChainArmorDefaultConfig() => new()
    {
        ["chainArmorEXPPerReceiveHit"] = (long)chainArmorEXPPerReceiveHit,
        ["chainArmorEXPMultiplyByDamage"] = (double)chainArmorEXPMultiplyByDamage,
        ["chainArmorEXPIncreaseByAmountDamage"] = (long)chainArmorEXPIncreaseByAmountDamage,
        ["chainArmorEXPPerLevelBase"] = (long)chainArmorEXPPerLevelBase,
        ["chainArmorEXPMultiplyPerLevel"] = chainArmorEXPMultiplyPerLevel,
        ["chainArmorRelativeProtectionMultiply"] = (double)chainArmorRelativeProtectionMultiply,
        ["chainArmorRelativeProtectionMultiplyPerLevel"] = (double)chainArmorRelativeProtectionMultiplyPerLevel,
        ["chainArmorRelativeProtectionMultiplyReductionEveryLevel"] = (long)chainArmorRelativeProtectionMultiplyReductionEveryLevel,
        ["chainArmorRelativeProtectionMultiplyReductionPerReduce"] = (double)chainArmorRelativeProtectionMultiplyReductionPerReduce,
        ["chainArmorFlatDamageReductionMultiply"] = (double)chainArmorFlatDamageReductionMultiply,
        ["chainArmorFlatDamageReductionMultiplyPerLevel"] = (double)chainArmorFlatDamageReductionMultiplyPerLevel,
        ["chainArmorFlatDamageReductionMultiplyReductionEveryLevel"] = (long)chainArmorFlatDamageReductionMultiplyReductionEveryLevel,
        ["chainArmorFlatDamageReductionMultiplyReductionPerReduce"] = (double)chainArmorFlatDamageReductionMultiplyReductionPerReduce,
        ["chainArmorHealingEffectivnessMultiply"] = (double)chainArmorHealingEffectivnessMultiply,
        ["chainArmorHealingEffectivnessMultiplyPerLevel"] = (double)chainArmorHealingEffectivnessMultiplyPerLevel,
        ["chainArmorHealingEffectivnessMultiplyReductionEveryLevel"] = (long)chainArmorHealingEffectivnessMultiplyReductionEveryLevel,
        ["chainArmorHealingEffectivnessMultiplyReductionPerReduce"] = (double)chainArmorHealingEffectivnessMultiplyReductionPerReduce,
        ["chainArmorHungerRateMultiply"] = (double)chainArmorHungerRateMultiply,
        ["chainArmorHungerRateMultiplyPerLevel"] = (double)chainArmorHungerRateMultiplyPerLevel,
        ["chainArmorHungerRateMultiplyReductionEveryLevel"] = (long)chainArmorHungerRateMultiplyReductionEveryLevel,
        ["chainArmorHungerRateMultiplyReductionPerReduce"] = (double)chainArmorHungerRateMultiplyReductionPerReduce,
        ["chainArmorRangedWeaponsAccuracyMultiply"] = (double)chainArmorRangedWeaponsAccuracyMultiply,
        ["chainArmorRangedWeaponsAccuracyMultiplyPerLevel"] = (double)chainArmorRangedWeaponsAccuracyMultiplyPerLevel,
        ["chainArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel"] = (long)chainArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel,
        ["chainArmorRangedWeaponsAccuracyMultiplyReductionPerReduce"] = (double)chainArmorRangedWeaponsAccuracyMultiplyReductionPerReduce,
        ["chainArmorRangedWeaponsSpeedMultiply"] = (double)chainArmorRangedWeaponsSpeedMultiply,
        ["chainArmorRangedWeaponsSpeedMultiplyPerLevel"] = (double)chainArmorRangedWeaponsSpeedMultiplyPerLevel,
        ["chainArmorRangedWeaponsSpeedMultiplyReductionEveryLevel"] = (long)chainArmorRangedWeaponsSpeedMultiplyReductionEveryLevel,
        ["chainArmorRangedWeaponsSpeedMultiplyReductionPerReduce"] = (double)chainArmorRangedWeaponsSpeedMultiplyReductionPerReduce,
        ["chainArmorWalkSpeedMultiply"] = (double)chainArmorWalkSpeedMultiply,
        ["chainArmorWalkSpeedMultiplyPerLevel"] = (double)chainArmorWalkSpeedMultiplyPerLevel,
        ["chainArmorWalkSpeedMultiplyReductionEveryLevel"] = (long)chainArmorWalkSpeedMultiplyReductionEveryLevel,
        ["chainArmorWalkSpeedMultiplyReductionPerReduce"] = (double)chainArmorWalkSpeedMultiplyReductionPerReduce,
        ["chainArmorMaxLevel"] = (long)chainArmorMaxLevel,
        ["chainArmorSubLevelEXPMultiply"] = chainArmorSubLevelEXPMultiply,
    };

    private static Dictionary<string, object> BuildChainArmorItemsDefaultConfig() => new()
    {
        ["game:armor-head-chain-copper"] = 0.3,
        ["game:armor-body-chain-copper"] = 0.5,
        ["game:armor-legs-chain-copper"] = 0.2,
        ["game:armor-head-chain-tinbronze"] = 0.3,
        ["game:armor-body-chain-tinbronze"] = 0.5,
        ["game:armor-legs-chain-tinbronze"] = 0.2,
        ["game:armor-head-chain-bismuthbronze"] = 0.3,
        ["game:armor-body-chain-bismuthbronze"] = 0.5,
        ["game:armor-legs-chain-bismuthbronze"] = 0.2,
        ["game:armor-head-chain-blackbronze"] = 0.3,
        ["game:armor-body-chain-blackbronze"] = 0.5,
        ["game:armor-legs-chain-blackbronze"] = 0.2,
        ["game:armor-head-chain-iron"] = 0.3,
        ["game:armor-body-chain-iron"] = 0.5,
        ["game:armor-legs-chain-iron"] = 0.2,
        ["game:armor-head-chain-meteoriciron"] = 0.3,
        ["game:armor-body-chain-meteoriciron"] = 0.5,
        ["game:armor-legs-chain-meteoriciron"] = 0.2,
        ["game:armor-head-chain-steel"] = 0.3,
        ["game:armor-body-chain-steel"] = 0.5,
        ["game:armor-legs-chain-steel"] = 0.2,
        ["game:armor-head-chain-gold"] = 0.3,
        ["game:armor-body-chain-gold"] = 0.5,
        ["game:armor-legs-chain-gold"] = 0.2,
        ["game:armor-head-chain-silver"] = 0.3,
        ["game:armor-body-chain-silver"] = 0.5,
        ["game:armor-legs-chain-silver"] = 0.2,
    };

    public static void PopulateChainArmorConfiguration(ICoreAPI api)
    {
        Dictionary<string, object> chainArmorLevelStats = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/levelstats",
            "chainarmor",
            BuildChainArmorDefaultConfig());
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
        { //chainArmorSubLevelEXPMultiply
            if (chainArmorLevelStats.TryGetValue("chainArmorSubLevelEXPMultiply", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: chainArmorSubLevelEXPMultiply is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: chainArmorSubLevelEXPMultiply is not double is {value.GetType()}");
                else chainArmorSubLevelEXPMultiply = (double)value;
            else Debug.LogError("CONFIGURATION ERROR: chainArmorSubLevelEXPMultiply not set");
        }

        // Get chain armor multiply exp
        expMultiplyHitChainArmor.Clear();
        Dictionary<string, object> tmpexpMultiplyHitChainArmor = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/levelstats",
            "chainarmoritems",
            BuildChainArmorItemsDefaultConfig());
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
    private static int brigandineArmorEXPIncreaseByAmountDamage = 2;
    private static int brigandineArmorEXPPerLevelBase = 500;
    private static double brigandineArmorEXPMultiplyPerLevel = 1.2;

    private static float brigandineArmorRelativeProtectionMultiply = 1.0f;
    private static float brigandineArmorRelativeProtectionMultiplyPerLevel = 0.04f;
    private static int brigandineArmorRelativeProtectionMultiplyReductionEveryLevel = 1;
    private static float brigandineArmorRelativeProtectionMultiplyReductionPerReduce = 0.18f;

    private static float brigandineArmorFlatDamageReductionMultiply = 1.0f;
    private static float brigandineArmorFlatDamageReductionMultiplyPerLevel = 0.04f;
    private static int brigandineArmorFlatDamageReductionMultiplyReductionEveryLevel = 1;
    private static float brigandineArmorFlatDamageReductionMultiplyReductionPerReduce = 0.05f;

    private static float brigandineArmorHealingEffectivnessMultiply = 1.0f;
    private static float brigandineArmorHealingEffectivnessMultiplyPerLevel = 0.04f;
    private static int brigandineArmorHealingEffectivnessMultiplyReductionEveryLevel = 1;
    private static float brigandineArmorHealingEffectivnessMultiplyReductionPerReduce = 0.05f;

    private static float brigandineArmorHungerRateMultiply = 1.0f;
    private static float brigandineArmorHungerRateMultiplyPerLevel = 0.035f;
    private static int brigandineArmorHungerRateMultiplyReductionEveryLevel = 1;
    private static float brigandineArmorHungerRateMultiplyReductionPerReduce = 0.05f;

    private static float brigandineArmorRangedWeaponsAccuracyMultiply = 1.0f;
    private static float brigandineArmorRangedWeaponsAccuracyMultiplyPerLevel = 0.03f;
    private static int brigandineArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel = 1;
    private static float brigandineArmorRangedWeaponsAccuracyMultiplyReductionPerReduce = 0.05f;

    private static float brigandineArmorRangedWeaponsSpeedMultiply = 1.0f;
    private static float brigandineArmorRangedWeaponsSpeedMultiplyPerLevel = 0.03f;
    private static int brigandineArmorRangedWeaponsSpeedMultiplyReductionEveryLevel = 1;
    private static float brigandineArmorRangedWeaponsSpeedMultiplyReductionPerReduce = 0.05f;

    private static float brigandineArmorWalkSpeedMultiply = 1.0f;
    private static float brigandineArmorWalkSpeedMultiplyPerLevel = 0.025f;
    private static int brigandineArmorWalkSpeedMultiplyReductionEveryLevel = 1;
    private static float brigandineArmorWalkSpeedMultiplyReductionPerReduce = 0.05f;

    public static int brigandineArmorMaxLevel = 999;
    public static double brigandineArmorSubLevelEXPMultiply = 3.0;

    private static Dictionary<string, object> BuildBrigandineArmorDefaultConfig() => new()
    {
        ["brigandineArmorEXPPerReceiveHit"] = (long)brigandineArmorEXPPerReceiveHit,
        ["brigandineArmorEXPMultiplyByDamage"] = (double)brigandineArmorEXPMultiplyByDamage,
        ["brigandineArmorEXPIncreaseByAmountDamage"] = (long)brigandineArmorEXPIncreaseByAmountDamage,
        ["brigandineArmorEXPPerLevelBase"] = (long)brigandineArmorEXPPerLevelBase,
        ["brigandineArmorEXPMultiplyPerLevel"] = brigandineArmorEXPMultiplyPerLevel,
        ["brigandineArmorRelativeProtectionMultiply"] = (double)brigandineArmorRelativeProtectionMultiply,
        ["brigandineArmorRelativeProtectionMultiplyPerLevel"] = (double)brigandineArmorRelativeProtectionMultiplyPerLevel,
        ["brigandineArmorRelativeProtectionMultiplyReductionEveryLevel"] = (long)brigandineArmorRelativeProtectionMultiplyReductionEveryLevel,
        ["brigandineArmorRelativeProtectionMultiplyReductionPerReduce"] = (double)brigandineArmorRelativeProtectionMultiplyReductionPerReduce,
        ["brigandineArmorFlatDamageReductionMultiply"] = (double)brigandineArmorFlatDamageReductionMultiply,
        ["brigandineArmorFlatDamageReductionMultiplyPerLevel"] = (double)brigandineArmorFlatDamageReductionMultiplyPerLevel,
        ["brigandineArmorFlatDamageReductionMultiplyReductionEveryLevel"] = (long)brigandineArmorFlatDamageReductionMultiplyReductionEveryLevel,
        ["brigandineArmorFlatDamageReductionMultiplyReductionPerReduce"] = (double)brigandineArmorFlatDamageReductionMultiplyReductionPerReduce,
        ["brigandineArmorHealingEffectivnessMultiply"] = (double)brigandineArmorHealingEffectivnessMultiply,
        ["brigandineArmorHealingEffectivnessMultiplyPerLevel"] = (double)brigandineArmorHealingEffectivnessMultiplyPerLevel,
        ["brigandineArmorHealingEffectivnessMultiplyReductionEveryLevel"] = (long)brigandineArmorHealingEffectivnessMultiplyReductionEveryLevel,
        ["brigandineArmorHealingEffectivnessMultiplyReductionPerReduce"] = (double)brigandineArmorHealingEffectivnessMultiplyReductionPerReduce,
        ["brigandineArmorHungerRateMultiply"] = (double)brigandineArmorHungerRateMultiply,
        ["brigandineArmorHungerRateMultiplyPerLevel"] = (double)brigandineArmorHungerRateMultiplyPerLevel,
        ["brigandineArmorHungerRateMultiplyReductionEveryLevel"] = (long)brigandineArmorHungerRateMultiplyReductionEveryLevel,
        ["brigandineArmorHungerRateMultiplyReductionPerReduce"] = (double)brigandineArmorHungerRateMultiplyReductionPerReduce,
        ["brigandineArmorRangedWeaponsAccuracyMultiply"] = (double)brigandineArmorRangedWeaponsAccuracyMultiply,
        ["brigandineArmorRangedWeaponsAccuracyMultiplyPerLevel"] = (double)brigandineArmorRangedWeaponsAccuracyMultiplyPerLevel,
        ["brigandineArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel"] = (long)brigandineArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel,
        ["brigandineArmorRangedWeaponsAccuracyMultiplyReductionPerReduce"] = (double)brigandineArmorRangedWeaponsAccuracyMultiplyReductionPerReduce,
        ["brigandineArmorRangedWeaponsSpeedMultiply"] = (double)brigandineArmorRangedWeaponsSpeedMultiply,
        ["brigandineArmorRangedWeaponsSpeedMultiplyPerLevel"] = (double)brigandineArmorRangedWeaponsSpeedMultiplyPerLevel,
        ["brigandineArmorRangedWeaponsSpeedMultiplyReductionEveryLevel"] = (long)brigandineArmorRangedWeaponsSpeedMultiplyReductionEveryLevel,
        ["brigandineArmorRangedWeaponsSpeedMultiplyReductionPerReduce"] = (double)brigandineArmorRangedWeaponsSpeedMultiplyReductionPerReduce,
        ["brigandineArmorWalkSpeedMultiply"] = (double)brigandineArmorWalkSpeedMultiply,
        ["brigandineArmorWalkSpeedMultiplyPerLevel"] = (double)brigandineArmorWalkSpeedMultiplyPerLevel,
        ["brigandineArmorWalkSpeedMultiplyReductionEveryLevel"] = (long)brigandineArmorWalkSpeedMultiplyReductionEveryLevel,
        ["brigandineArmorWalkSpeedMultiplyReductionPerReduce"] = (double)brigandineArmorWalkSpeedMultiplyReductionPerReduce,
        ["brigandineArmorMaxLevel"] = (long)brigandineArmorMaxLevel,
        ["brigandineArmorSubLevelEXPMultiply"] = brigandineArmorSubLevelEXPMultiply,
    };

    private static Dictionary<string, object> BuildBrigandineArmorItemsDefaultConfig() => new()
    {
        ["game:armor-head-brigandine-copper"] = 0.3,
        ["game:armor-body-brigandine-copper"] = 0.5,
        ["game:armor-legs-brigandine-copper"] = 0.2,
        ["game:armor-head-brigandine-tinbronze"] = 0.3,
        ["game:armor-body-brigandine-tinbronze"] = 0.5,
        ["game:armor-legs-brigandine-tinbronze"] = 0.2,
        ["game:armor-head-brigandine-bismuthbronze"] = 0.3,
        ["game:armor-body-brigandine-bismuthbronze"] = 0.5,
        ["game:armor-legs-brigandine-bismuthbronze"] = 0.2,
        ["game:armor-head-brigandine-blackbronze"] = 0.3,
        ["game:armor-body-brigandine-blackbronze"] = 0.5,
        ["game:armor-legs-brigandine-blackbronze"] = 0.2,
        ["game:armor-head-brigandine-iron"] = 0.3,
        ["game:armor-body-brigandine-iron"] = 0.5,
        ["game:armor-legs-brigandine-iron"] = 0.2,
        ["game:armor-head-brigandine-meteoriciron"] = 0.3,
        ["game:armor-body-brigandine-meteoriciron"] = 0.5,
        ["game:armor-legs-brigandine-meteoriciron"] = 0.2,
        ["game:armor-head-brigandine-steel"] = 0.3,
        ["game:armor-body-brigandine-steel"] = 0.5,
        ["game:armor-legs-brigandine-steel"] = 0.2,
    };

    public static void PopulateBrigandineArmorConfiguration(ICoreAPI api)
    {
        Dictionary<string, object> brigandineArmorLevelStats = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/levelstats",
            "brigandinearmor",
            BuildBrigandineArmorDefaultConfig());
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
        { //brigandineArmorSubLevelEXPMultiply
            if (brigandineArmorLevelStats.TryGetValue("brigandineArmorSubLevelEXPMultiply", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: brigandineArmorSubLevelEXPMultiply is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: brigandineArmorSubLevelEXPMultiply is not double is {value.GetType()}");
                else brigandineArmorSubLevelEXPMultiply = (double)value;
            else Debug.LogError("CONFIGURATION ERROR: brigandineArmorSubLevelEXPMultiply not set");
        }

        // Get brigandine armor multiply exp
        expMultiplyHitBrigandineArmor.Clear();
        Dictionary<string, object> tmpexpMultiplyHitBrigandineArmor = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/levelstats",
            "brigandinearmoritems",
            BuildBrigandineArmorItemsDefaultConfig());
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
    private static int lamellarArmorEXPIncreaseByAmountDamage = 2;
    private static int lamellarArmorEXPPerLevelBase = 500;
    private static double lamellarArmorEXPMultiplyPerLevel = 1.2;

    private static float lamellarArmorRelativeProtectionMultiply = 1.0f;
    private static float lamellarArmorRelativeProtectionMultiplyPerLevel = 0.065f;
    private static int lamellarArmorRelativeProtectionMultiplyReductionEveryLevel = 1;
    private static float lamellarArmorRelativeProtectionMultiplyReductionPerReduce = 0.15f;

    private static float lamellarArmorFlatDamageReductionMultiply = 1.0f;
    private static float lamellarArmorFlatDamageReductionMultiplyPerLevel = 0.065f;
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
    private static float lamellarArmorRangedWeaponsAccuracyMultiplyPerLevel = 0.01f;
    private static int lamellarArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel = 1;
    private static float lamellarArmorRangedWeaponsAccuracyMultiplyReductionPerReduce = 0.05f;

    private static float lamellarArmorRangedWeaponsSpeedMultiply = 1.0f;
    private static float lamellarArmorRangedWeaponsSpeedMultiplyPerLevel = 0.01f;
    private static int lamellarArmorRangedWeaponsSpeedMultiplyReductionEveryLevel = 1;
    private static float lamellarArmorRangedWeaponsSpeedMultiplyReductionPerReduce = 0.05f;

    private static float lamellarArmorWalkSpeedMultiply = 1.0f;
    private static float lamellarArmorWalkSpeedMultiplyPerLevel = 0.01f;
    private static int lamellarArmorWalkSpeedMultiplyReductionEveryLevel = 1;
    private static float lamellarArmorWalkSpeedMultiplyReductionPerReduce = 0.05f;

    public static int lamellarArmorMaxLevel = 999;
    public static double lamellarArmorSubLevelEXPMultiply = 3.0;

    private static Dictionary<string, object> BuildLamellarArmorDefaultConfig() => new()
    {
        ["lamellarArmorEXPPerReceiveHit"] = (long)lamellarArmorEXPPerReceiveHit,
        ["lamellarArmorEXPMultiplyByDamage"] = (double)lamellarArmorEXPMultiplyByDamage,
        ["lamellarArmorEXPIncreaseByAmountDamage"] = (long)lamellarArmorEXPIncreaseByAmountDamage,
        ["lamellarArmorEXPPerLevelBase"] = (long)lamellarArmorEXPPerLevelBase,
        ["lamellarArmorEXPMultiplyPerLevel"] = lamellarArmorEXPMultiplyPerLevel,
        ["lamellarArmorRelativeProtectionMultiply"] = (double)lamellarArmorRelativeProtectionMultiply,
        ["lamellarArmorRelativeProtectionMultiplyPerLevel"] = (double)lamellarArmorRelativeProtectionMultiplyPerLevel,
        ["lamellarArmorRelativeProtectionMultiplyReductionEveryLevel"] = (long)lamellarArmorRelativeProtectionMultiplyReductionEveryLevel,
        ["lamellarArmorRelativeProtectionMultiplyReductionPerReduce"] = (double)lamellarArmorRelativeProtectionMultiplyReductionPerReduce,
        ["lamellarArmorFlatDamageReductionMultiply"] = (double)lamellarArmorFlatDamageReductionMultiply,
        ["lamellarArmorFlatDamageReductionMultiplyPerLevel"] = (double)lamellarArmorFlatDamageReductionMultiplyPerLevel,
        ["lamellarArmorFlatDamageReductionMultiplyReductionEveryLevel"] = (long)lamellarArmorFlatDamageReductionMultiplyReductionEveryLevel,
        ["lamellarArmorFlatDamageReductionMultiplyReductionPerReduce"] = (double)lamellarArmorFlatDamageReductionMultiplyReductionPerReduce,
        ["lamellarArmorHealingEffectivnessMultiply"] = (double)lamellarArmorHealingEffectivnessMultiply,
        ["lamellarArmorHealingEffectivnessMultiplyPerLevel"] = (double)lamellarArmorHealingEffectivnessMultiplyPerLevel,
        ["lamellarArmorHealingEffectivnessMultiplyReductionEveryLevel"] = (long)lamellarArmorHealingEffectivnessMultiplyReductionEveryLevel,
        ["lamellarArmorHealingEffectivnessMultiplyReductionPerReduce"] = (double)lamellarArmorHealingEffectivnessMultiplyReductionPerReduce,
        ["lamellarArmorHungerRateMultiply"] = (double)lamellarArmorHungerRateMultiply,
        ["lamellarArmorHungerRateMultiplyPerLevel"] = (double)lamellarArmorHungerRateMultiplyPerLevel,
        ["lamellarArmorHungerRateMultiplyReductionEveryLevel"] = (long)lamellarArmorHungerRateMultiplyReductionEveryLevel,
        ["lamellarArmorHungerRateMultiplyReductionPerReduce"] = (double)lamellarArmorHungerRateMultiplyReductionPerReduce,
        ["lamellarArmorRangedWeaponsAccuracyMultiply"] = (double)lamellarArmorRangedWeaponsAccuracyMultiply,
        ["lamellarArmorRangedWeaponsAccuracyMultiplyPerLevel"] = (double)lamellarArmorRangedWeaponsAccuracyMultiplyPerLevel,
        ["lamellarArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel"] = (long)lamellarArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel,
        ["lamellarArmorRangedWeaponsAccuracyMultiplyReductionPerReduce"] = (double)lamellarArmorRangedWeaponsAccuracyMultiplyReductionPerReduce,
        ["lamellarArmorRangedWeaponsSpeedMultiply"] = (double)lamellarArmorRangedWeaponsSpeedMultiply,
        ["lamellarArmorRangedWeaponsSpeedMultiplyPerLevel"] = (double)lamellarArmorRangedWeaponsSpeedMultiplyPerLevel,
        ["lamellarArmorRangedWeaponsSpeedMultiplyReductionEveryLevel"] = (long)lamellarArmorRangedWeaponsSpeedMultiplyReductionEveryLevel,
        ["lamellarArmorRangedWeaponsSpeedMultiplyReductionPerReduce"] = (double)lamellarArmorRangedWeaponsSpeedMultiplyReductionPerReduce,
        ["lamellarArmorWalkSpeedMultiply"] = (double)lamellarArmorWalkSpeedMultiply,
        ["lamellarArmorWalkSpeedMultiplyPerLevel"] = (double)lamellarArmorWalkSpeedMultiplyPerLevel,
        ["lamellarArmorWalkSpeedMultiplyReductionEveryLevel"] = (long)lamellarArmorWalkSpeedMultiplyReductionEveryLevel,
        ["lamellarArmorWalkSpeedMultiplyReductionPerReduce"] = (double)lamellarArmorWalkSpeedMultiplyReductionPerReduce,
        ["lamellarArmorMaxLevel"] = (long)lamellarArmorMaxLevel,
        ["lamellarArmorSubLevelEXPMultiply"] = lamellarArmorSubLevelEXPMultiply,
    };

    private static Dictionary<string, object> BuildLamellarArmorItemsDefaultConfig() => new()
    {
        ["game:armor-head-lamellar-wood"] = 0.3,
        ["game:armor-body-lamellar-wood"] = 0.5,
        ["game:armor-legs-lamellar-wood"] = 0.2,
        ["game:armor-head-lamellar-copper"] = 0.3,
        ["game:armor-body-lamellar-copper"] = 0.5,
        ["game:armor-legs-lamellar-copper"] = 0.2,
        ["game:armor-head-lamellar-tinbronze"] = 0.3,
        ["game:armor-body-lamellar-tinbronze"] = 0.5,
        ["game:armor-legs-lamellar-tinbronze"] = 0.2,
        ["game:armor-head-lamellar-blackbronze"] = 0.3,
        ["game:armor-body-lamellar-blackbronze"] = 0.5,
        ["game:armor-legs-lamellar-blackbronze"] = 0.2,
        ["game:armor-head-lamellar-bismuthbronze"] = 0.3,
        ["game:armor-body-lamellar-bismuthbronze"] = 0.5,
        ["game:armor-legs-lamellar-bismuthbronze"] = 0.2,
    };

    public static void PopulateLamellarArmorConfiguration(ICoreAPI api)
    {
        Dictionary<string, object> lamellarArmorLevelStats = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/levelstats",
            "lamellararmor",
            BuildLamellarArmorDefaultConfig());
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
        { //lamellarArmorSubLevelEXPMultiply
            if (lamellarArmorLevelStats.TryGetValue("lamellarArmorSubLevelEXPMultiply", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: lamellarArmorSubLevelEXPMultiply is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: lamellarArmorSubLevelEXPMultiply is not double is {value.GetType()}");
                else lamellarArmorSubLevelEXPMultiply = (double)value;
            else Debug.LogError("CONFIGURATION ERROR: lamellarArmorSubLevelEXPMultiply not set");
        }

        // Get lamellar armor multiply exp
        expMultiplyHitLamellarArmor.Clear();
        Dictionary<string, object> tmpexpMultiplyHitLamellarArmor = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/levelstats",
            "lamellararmoritems",
            BuildLamellarArmorItemsDefaultConfig());
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
    private static int plateArmorEXPIncreaseByAmountDamage = 2;
    private static int plateArmorEXPPerLevelBase = 500;
    private static double plateArmorEXPMultiplyPerLevel = 1.2;

    private static float plateArmorRelativeProtectionMultiply = 1.0f;
    private static float plateArmorRelativeProtectionMultiplyPerLevel = 0.065f;
    private static int plateArmorRelativeProtectionMultiplyReductionEveryLevel = 1;
    private static float plateArmorRelativeProtectionMultiplyReductionPerReduce = 0.15f;

    private static float plateArmorFlatDamageReductionMultiply = 1.0f;
    private static float plateArmorFlatDamageReductionMultiplyPerLevel = 0.065f;
    private static int plateArmorFlatDamageReductionMultiplyReductionEveryLevel = 1;
    private static float plateArmorFlatDamageReductionMultiplyReductionPerReduce = 0.05f;

    private static float plateArmorHealingEffectivnessMultiply = 1.0f;
    private static float plateArmorHealingEffectivnessMultiplyPerLevel = 0.045f;
    private static int plateArmorHealingEffectivnessMultiplyReductionEveryLevel = 1;
    private static float plateArmorHealingEffectivnessMultiplyReductionPerReduce = 0.05f;

    private static float plateArmorHungerRateMultiply = 1.0f;
    private static float plateArmorHungerRateMultiplyPerLevel = 0.05f;
    private static int plateArmorHungerRateMultiplyReductionEveryLevel = 1;
    private static float plateArmorHungerRateMultiplyReductionPerReduce = 0.05f;

    private static float plateArmorRangedWeaponsAccuracyMultiply = 1.0f;
    private static float plateArmorRangedWeaponsAccuracyMultiplyPerLevel = 0.01f;
    private static int plateArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel = 1;
    private static float plateArmorRangedWeaponsAccuracyMultiplyReductionPerReduce = 0.05f;

    private static float plateArmorRangedWeaponsSpeedMultiply = 1.0f;
    private static float plateArmorRangedWeaponsSpeedMultiplyPerLevel = 0.01f;
    private static int plateArmorRangedWeaponsSpeedMultiplyReductionEveryLevel = 1;
    private static float plateArmorRangedWeaponsSpeedMultiplyReductionPerReduce = 0.05f;

    private static float plateArmorWalkSpeedMultiply = 1.0f;
    private static float plateArmorWalkSpeedMultiplyPerLevel = 0.01f;
    private static int plateArmorWalkSpeedMultiplyReductionEveryLevel = 1;
    private static float plateArmorWalkSpeedMultiplyReductionPerReduce = 0.05f;

    public static int plateArmorMaxLevel = 999;
    public static double plateArmorSubLevelEXPMultiply = 3.0;

    private static Dictionary<string, object> BuildPlateArmorDefaultConfig() => new()
    {
        ["plateArmorEXPPerReceiveHit"] = (long)plateArmorEXPPerReceiveHit,
        ["plateArmorEXPMultiplyByDamage"] = (double)plateArmorEXPMultiplyByDamage,
        ["plateArmorEXPIncreaseByAmountDamage"] = (long)plateArmorEXPIncreaseByAmountDamage,
        ["plateArmorEXPPerLevelBase"] = (long)plateArmorEXPPerLevelBase,
        ["plateArmorEXPMultiplyPerLevel"] = plateArmorEXPMultiplyPerLevel,
        ["plateArmorRelativeProtectionMultiply"] = (double)plateArmorRelativeProtectionMultiply,
        ["plateArmorRelativeProtectionMultiplyPerLevel"] = (double)plateArmorRelativeProtectionMultiplyPerLevel,
        ["plateArmorRelativeProtectionMultiplyReductionEveryLevel"] = (long)plateArmorRelativeProtectionMultiplyReductionEveryLevel,
        ["plateArmorRelativeProtectionMultiplyReductionPerReduce"] = (double)plateArmorRelativeProtectionMultiplyReductionPerReduce,
        ["plateArmorFlatDamageReductionMultiply"] = (double)plateArmorFlatDamageReductionMultiply,
        ["plateArmorFlatDamageReductionMultiplyPerLevel"] = (double)plateArmorFlatDamageReductionMultiplyPerLevel,
        ["plateArmorFlatDamageReductionMultiplyReductionEveryLevel"] = (long)plateArmorFlatDamageReductionMultiplyReductionEveryLevel,
        ["plateArmorFlatDamageReductionMultiplyReductionPerReduce"] = (double)plateArmorFlatDamageReductionMultiplyReductionPerReduce,
        ["plateArmorHealingEffectivnessMultiply"] = (double)plateArmorHealingEffectivnessMultiply,
        ["plateArmorHealingEffectivnessMultiplyPerLevel"] = (double)plateArmorHealingEffectivnessMultiplyPerLevel,
        ["plateArmorHealingEffectivnessMultiplyReductionEveryLevel"] = (long)plateArmorHealingEffectivnessMultiplyReductionEveryLevel,
        ["plateArmorHealingEffectivnessMultiplyReductionPerReduce"] = (double)plateArmorHealingEffectivnessMultiplyReductionPerReduce,
        ["plateArmorHungerRateMultiply"] = (double)plateArmorHungerRateMultiply,
        ["plateArmorHungerRateMultiplyPerLevel"] = (double)plateArmorHungerRateMultiplyPerLevel,
        ["plateArmorHungerRateMultiplyReductionEveryLevel"] = (long)plateArmorHungerRateMultiplyReductionEveryLevel,
        ["plateArmorHungerRateMultiplyReductionPerReduce"] = (double)plateArmorHungerRateMultiplyReductionPerReduce,
        ["plateArmorRangedWeaponsAccuracyMultiply"] = (double)plateArmorRangedWeaponsAccuracyMultiply,
        ["plateArmorRangedWeaponsAccuracyMultiplyPerLevel"] = (double)plateArmorRangedWeaponsAccuracyMultiplyPerLevel,
        ["plateArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel"] = (long)plateArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel,
        ["plateArmorRangedWeaponsAccuracyMultiplyReductionPerReduce"] = (double)plateArmorRangedWeaponsAccuracyMultiplyReductionPerReduce,
        ["plateArmorRangedWeaponsSpeedMultiply"] = (double)plateArmorRangedWeaponsSpeedMultiply,
        ["plateArmorRangedWeaponsSpeedMultiplyPerLevel"] = (double)plateArmorRangedWeaponsSpeedMultiplyPerLevel,
        ["plateArmorRangedWeaponsSpeedMultiplyReductionEveryLevel"] = (long)plateArmorRangedWeaponsSpeedMultiplyReductionEveryLevel,
        ["plateArmorRangedWeaponsSpeedMultiplyReductionPerReduce"] = (double)plateArmorRangedWeaponsSpeedMultiplyReductionPerReduce,
        ["plateArmorWalkSpeedMultiply"] = (double)plateArmorWalkSpeedMultiply,
        ["plateArmorWalkSpeedMultiplyPerLevel"] = (double)plateArmorWalkSpeedMultiplyPerLevel,
        ["plateArmorWalkSpeedMultiplyReductionEveryLevel"] = (long)plateArmorWalkSpeedMultiplyReductionEveryLevel,
        ["plateArmorWalkSpeedMultiplyReductionPerReduce"] = (double)plateArmorWalkSpeedMultiplyReductionPerReduce,
        ["plateArmorMaxLevel"] = (long)plateArmorMaxLevel,
        ["plateArmorSubLevelEXPMultiply"] = plateArmorSubLevelEXPMultiply,
    };

    private static Dictionary<string, object> BuildPlateArmorItemsDefaultConfig() => new()
    {
        ["game:armor-head-plate-copper"] = 0.3,
        ["game:armor-body-plate-copper"] = 0.5,
        ["game:armor-legs-plate-copper"] = 0.2,
        ["game:armor-head-plate-tinbronze"] = 0.3,
        ["game:armor-body-plate-tinbronze"] = 0.5,
        ["game:armor-legs-plate-tinbronze"] = 0.2,
        ["game:armor-head-plate-bismuthbronze"] = 0.3,
        ["game:armor-body-plate-bismuthbronze"] = 0.5,
        ["game:armor-legs-plate-bismuthbronze"] = 0.2,
        ["game:armor-head-plate-blackbronze"] = 0.3,
        ["game:armor-body-plate-blackbronze"] = 0.5,
        ["game:armor-legs-plate-blackbronze"] = 0.2,
        ["game:armor-head-plate-iron"] = 0.3,
        ["game:armor-body-plate-iron"] = 0.5,
        ["game:armor-legs-plate-iron"] = 0.2,
        ["game:armor-head-plate-meteoriciron"] = 0.3,
        ["game:armor-body-plate-meteoriciron"] = 0.5,
        ["game:armor-legs-plate-meteoriciron"] = 0.2,
        ["game:armor-head-plate-steel"] = 0.3,
        ["game:armor-body-plate-steel"] = 0.5,
        ["game:armor-legs-plate-steel"] = 0.2,
        ["game:armor-head-plate-gold"] = 0.3,
        ["game:armor-body-plate-gold"] = 0.5,
        ["game:armor-legs-plate-gold"] = 0.2,
        ["game:armor-head-plate-silver"] = 0.3,
        ["game:armor-body-plate-silver"] = 0.5,
        ["game:armor-legs-plate-silver"] = 0.2,
        ["game:armor-head-antique-forlorn-pristine"] = 0.3,
        ["game:armor-body-antique-forlorn-pristine"] = 0.5,
        ["game:armor-legs-antique-forlorn-pristine"] = 0.2,
        ["game:armor-head-antique-forlorn-damaged"] = 0.3,
        ["game:armor-body-antique-forlorn-damaged"] = 0.5,
        ["game:armor-legs-antique-forlorn-damaged"] = 0.2,
        ["game:armor-head-antique-forlorn-broken"] = 0.3,
        ["game:armor-body-antique-forlorn-broken"] = 0.5,
        ["game:armor-legs-antique-forlorn-broken"] = 0.2,
    };

    public static void PopulatePlateArmorConfiguration(ICoreAPI api)
    {
        Dictionary<string, object> plateArmorLevelStats = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/levelstats",
            "platearmor",
            BuildPlateArmorDefaultConfig());
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
        { //plateArmorSubLevelEXPMultiply
            if (plateArmorLevelStats.TryGetValue("plateArmorSubLevelEXPMultiply", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: plateArmorSubLevelEXPMultiply is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: plateArmorSubLevelEXPMultiply is not double is {value.GetType()}");
                else plateArmorSubLevelEXPMultiply = (double)value;
            else Debug.LogError("CONFIGURATION ERROR: plateArmorSubLevelEXPMultiply not set");
        }

        // Get plate armor multiply exp
        expMultiplyHitPlateArmor.Clear();
        Dictionary<string, object> tmpexpMultiplyHitPlateArmor = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/levelstats",
            "platearmoritems",
            BuildPlateArmorItemsDefaultConfig());
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
    private static int scaleArmorEXPIncreaseByAmountDamage = 2;
    private static int scaleArmorEXPPerLevelBase = 500;
    private static double scaleArmorEXPMultiplyPerLevel = 1.2;

    private static float scaleArmorRelativeProtectionMultiply = 1.0f;
    private static float scaleArmorRelativeProtectionMultiplyPerLevel = 0.05f;
    private static int scaleArmorRelativeProtectionMultiplyReductionEveryLevel = 1;
    private static float scaleArmorRelativeProtectionMultiplyReductionPerReduce = 0.16f;

    private static float scaleArmorFlatDamageReductionMultiply = 1.0f;
    private static float scaleArmorFlatDamageReductionMultiplyPerLevel = 0.05f;
    private static int scaleArmorFlatDamageReductionMultiplyReductionEveryLevel = 1;
    private static float scaleArmorFlatDamageReductionMultiplyReductionPerReduce = 0.05f;

    private static float scaleArmorHealingEffectivnessMultiply = 1.0f;
    private static float scaleArmorHealingEffectivnessMultiplyPerLevel = 0.045f;
    private static int scaleArmorHealingEffectivnessMultiplyReductionEveryLevel = 1;
    private static float scaleArmorHealingEffectivnessMultiplyReductionPerReduce = 0.05f;

    private static float scaleArmorHungerRateMultiply = 1.0f;
    private static float scaleArmorHungerRateMultiplyPerLevel = 0.045f;
    private static int scaleArmorHungerRateMultiplyReductionEveryLevel = 1;
    private static float scaleArmorHungerRateMultiplyReductionPerReduce = 0.05f;

    private static float scaleArmorRangedWeaponsAccuracyMultiply = 1.0f;
    private static float scaleArmorRangedWeaponsAccuracyMultiplyPerLevel = 0.02f;
    private static int scaleArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel = 1;
    private static float scaleArmorRangedWeaponsAccuracyMultiplyReductionPerReduce = 0.05f;

    private static float scaleArmorRangedWeaponsSpeedMultiply = 1.0f;
    private static float scaleArmorRangedWeaponsSpeedMultiplyPerLevel = 0.02f;
    private static int scaleArmorRangedWeaponsSpeedMultiplyReductionEveryLevel = 1;
    private static float scaleArmorRangedWeaponsSpeedMultiplyReductionPerReduce = 0.05f;

    private static float scaleArmorWalkSpeedMultiply = 1.0f;
    private static float scaleArmorWalkSpeedMultiplyPerLevel = 0.015f;
    private static int scaleArmorWalkSpeedMultiplyReductionEveryLevel = 1;
    private static float scaleArmorWalkSpeedMultiplyReductionPerReduce = 0.05f;

    public static int scaleArmorMaxLevel = 999;
    public static double scaleArmorSubLevelEXPMultiply = 3.0;

    private static Dictionary<string, object> BuildScaleArmorDefaultConfig() => new()
    {
        ["scaleArmorEXPPerReceiveHit"] = (long)scaleArmorEXPPerReceiveHit,
        ["scaleArmorEXPMultiplyByDamage"] = (double)scaleArmorEXPMultiplyByDamage,
        ["scaleArmorEXPIncreaseByAmountDamage"] = (long)scaleArmorEXPIncreaseByAmountDamage,
        ["scaleArmorEXPPerLevelBase"] = (long)scaleArmorEXPPerLevelBase,
        ["scaleArmorEXPMultiplyPerLevel"] = scaleArmorEXPMultiplyPerLevel,
        ["scaleArmorRelativeProtectionMultiply"] = (double)scaleArmorRelativeProtectionMultiply,
        ["scaleArmorRelativeProtectionMultiplyPerLevel"] = (double)scaleArmorRelativeProtectionMultiplyPerLevel,
        ["scaleArmorRelativeProtectionMultiplyReductionEveryLevel"] = (long)scaleArmorRelativeProtectionMultiplyReductionEveryLevel,
        ["scaleArmorRelativeProtectionMultiplyReductionPerReduce"] = (double)scaleArmorRelativeProtectionMultiplyReductionPerReduce,
        ["scaleArmorFlatDamageReductionMultiply"] = (double)scaleArmorFlatDamageReductionMultiply,
        ["scaleArmorFlatDamageReductionMultiplyPerLevel"] = (double)scaleArmorFlatDamageReductionMultiplyPerLevel,
        ["scaleArmorFlatDamageReductionMultiplyReductionEveryLevel"] = (long)scaleArmorFlatDamageReductionMultiplyReductionEveryLevel,
        ["scaleArmorFlatDamageReductionMultiplyReductionPerReduce"] = (double)scaleArmorFlatDamageReductionMultiplyReductionPerReduce,
        ["scaleArmorHealingEffectivnessMultiply"] = (double)scaleArmorHealingEffectivnessMultiply,
        ["scaleArmorHealingEffectivnessMultiplyPerLevel"] = (double)scaleArmorHealingEffectivnessMultiplyPerLevel,
        ["scaleArmorHealingEffectivnessMultiplyReductionEveryLevel"] = (long)scaleArmorHealingEffectivnessMultiplyReductionEveryLevel,
        ["scaleArmorHealingEffectivnessMultiplyReductionPerReduce"] = (double)scaleArmorHealingEffectivnessMultiplyReductionPerReduce,
        ["scaleArmorHungerRateMultiply"] = (double)scaleArmorHungerRateMultiply,
        ["scaleArmorHungerRateMultiplyPerLevel"] = (double)scaleArmorHungerRateMultiplyPerLevel,
        ["scaleArmorHungerRateMultiplyReductionEveryLevel"] = (long)scaleArmorHungerRateMultiplyReductionEveryLevel,
        ["scaleArmorHungerRateMultiplyReductionPerReduce"] = (double)scaleArmorHungerRateMultiplyReductionPerReduce,
        ["scaleArmorRangedWeaponsAccuracyMultiply"] = (double)scaleArmorRangedWeaponsAccuracyMultiply,
        ["scaleArmorRangedWeaponsAccuracyMultiplyPerLevel"] = (double)scaleArmorRangedWeaponsAccuracyMultiplyPerLevel,
        ["scaleArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel"] = (long)scaleArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel,
        ["scaleArmorRangedWeaponsAccuracyMultiplyReductionPerReduce"] = (double)scaleArmorRangedWeaponsAccuracyMultiplyReductionPerReduce,
        ["scaleArmorRangedWeaponsSpeedMultiply"] = (double)scaleArmorRangedWeaponsSpeedMultiply,
        ["scaleArmorRangedWeaponsSpeedMultiplyPerLevel"] = (double)scaleArmorRangedWeaponsSpeedMultiplyPerLevel,
        ["scaleArmorRangedWeaponsSpeedMultiplyReductionEveryLevel"] = (long)scaleArmorRangedWeaponsSpeedMultiplyReductionEveryLevel,
        ["scaleArmorRangedWeaponsSpeedMultiplyReductionPerReduce"] = (double)scaleArmorRangedWeaponsSpeedMultiplyReductionPerReduce,
        ["scaleArmorWalkSpeedMultiply"] = (double)scaleArmorWalkSpeedMultiply,
        ["scaleArmorWalkSpeedMultiplyPerLevel"] = (double)scaleArmorWalkSpeedMultiplyPerLevel,
        ["scaleArmorWalkSpeedMultiplyReductionEveryLevel"] = (long)scaleArmorWalkSpeedMultiplyReductionEveryLevel,
        ["scaleArmorWalkSpeedMultiplyReductionPerReduce"] = (double)scaleArmorWalkSpeedMultiplyReductionPerReduce,
        ["scaleArmorMaxLevel"] = (long)scaleArmorMaxLevel,
        ["scaleArmorSubLevelEXPMultiply"] = scaleArmorSubLevelEXPMultiply,
    };

    private static Dictionary<string, object> BuildScaleArmorItemsDefaultConfig() => new()
    {
        ["game:armor-head-scale-copper"] = 0.3,
        ["game:armor-body-scale-copper"] = 0.5,
        ["game:armor-legs-scale-copper"] = 0.2,
        ["game:armor-head-scale-tinbronze"] = 0.3,
        ["game:armor-body-scale-tinbronze"] = 0.5,
        ["game:armor-legs-scale-tinbronze"] = 0.2,
        ["game:armor-head-scale-bismuthbronze"] = 0.3,
        ["game:armor-body-scale-bismuthbronze"] = 0.5,
        ["game:armor-legs-scale-bismuthbronze"] = 0.2,
        ["game:armor-head-scale-blackbronze"] = 0.3,
        ["game:armor-body-scale-blackbronze"] = 0.5,
        ["game:armor-legs-scale-blackbronze"] = 0.2,
        ["game:armor-head-scale-iron"] = 0.3,
        ["game:armor-body-scale-iron"] = 0.5,
        ["game:armor-legs-scale-iron"] = 0.2,
        ["game:armor-head-scale-meteoriciron"] = 0.3,
        ["game:armor-body-scale-meteoriciron"] = 0.5,
        ["game:armor-legs-scale-meteoriciron"] = 0.2,
        ["game:armor-head-scale-steel"] = 0.3,
        ["game:armor-body-scale-steel"] = 0.5,
        ["game:armor-legs-scale-steel"] = 0.2,
    };

    public static void PopulateScaleArmorConfiguration(ICoreAPI api)
    {
        Dictionary<string, object> scaleArmorLevelStats = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/levelstats",
            "scalearmor",
            BuildScaleArmorDefaultConfig());
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
        { //scaleArmorSubLevelEXPMultiply
            if (scaleArmorLevelStats.TryGetValue("scaleArmorSubLevelEXPMultiply", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: scaleArmorSubLevelEXPMultiply is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: scaleArmorSubLevelEXPMultiply is not double is {value.GetType()}");
                else scaleArmorSubLevelEXPMultiply = (double)value;
            else Debug.LogError("CONFIGURATION ERROR: scaleArmorSubLevelEXPMultiply not set");
        }

        // Get scale armor multiply exp
        expMultiplyHitScaleArmor.Clear();
        Dictionary<string, object> tmpexpMultiplyHitScaleArmor = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/LevelUP/config/levelstats",
            "scalearmoritems",
            BuildScaleArmorItemsDefaultConfig());
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

    private static Dictionary<string, object> BuildHunterClassDefaultConfig() => new()
    {
        ["classHunterLevelMultiply"] = 2.0,
        ["classBowLevelMultiply"] = 2.5,
        ["classSlingshotLevelMultiply"] = 0.9,
        ["classKnifeLevelMultiply"] = 1.5,
        ["classAxeLevelMultiply"] = 0.7,
        ["classPickaxeLevelMultiply"] = 0.5,
        ["classShovelLevelMultiply"] = 0.5,
        ["classSpearLevelMultiply"] = 1.8,
        ["classHammerLevelMultiply"] = 0.7,
        ["classSwordLevelMultiply"] = 1.0,
        ["classShieldLevelMultiply"] = 0.9,
        ["classHandLevelMultiply"] = 1.0,
        ["classFarmingLevelMultiply"] = 0.7,
        ["classCookingLevelMultiply"] = 0.6,
        ["classPanningLevelMultiply"] = 0.5,
        ["classVitalityLevelMultiply"] = 0.5,
        ["classMetabolismLevelMultiply"] = 2.0,
        ["classLeatherArmorLevelMultiply"] = 2.0,
        ["classChainArmorLevelMultiply"] = 0.4,
        ["classBrigandineArmorLevelMultiply"] = 1.0,
        ["classPlateArmorLevelMultiply"] = 1.0,
        ["classScaleArmorLevelMultiply"] = 1.0,
        ["classLamellarArmorLevelMultiply"] = 1.0,
        ["classSmithingLevelMultiply"] = 0.5,
    };

    private static Dictionary<string, object> BuildCommonerClassDefaultConfig() => new()
    {
        ["classHunterLevelMultiply"] = 1.0,
        ["classBowLevelMultiply"] = 1.0,
        ["classSlingshotLevelMultiply"] = 1.0,
        ["classKnifeLevelMultiply"] = 1.0,
        ["classAxeLevelMultiply"] = 1.0,
        ["classPickaxeLevelMultiply"] = 1.0,
        ["classShovelLevelMultiply"] = 1.0,
        ["classSpearLevelMultiply"] = 1.0,
        ["classHammerLevelMultiply"] = 1.0,
        ["classSwordLevelMultiply"] = 1.0,
        ["classShieldLevelMultiply"] = 1.0,
        ["classHandLevelMultiply"] = 1.0,
        ["classFarmingLevelMultiply"] = 1.0,
        ["classCookingLevelMultiply"] = 1.0,
        ["classPanningLevelMultiply"] = 1.0,
        ["classVitalityLevelMultiply"] = 1.0,
        ["classMetabolismLevelMultiply"] = 1.0,
        ["classLeatherArmorLevelMultiply"] = 1.0,
        ["classChainArmorLevelMultiply"] = 1.0,
        ["classBrigandineArmorLevelMultiply"] = 1.0,
        ["classPlateArmorLevelMultiply"] = 1.0,
        ["classScaleArmorLevelMultiply"] = 1.0,
        ["classLamellarArmorLevelMultiply"] = 1.0,
        ["classSmithingLevelMultiply"] = 1.0,
    };

    private static Dictionary<string, object> BuildBlackguardClassDefaultConfig() => new()
    {
        ["classHunterLevelMultiply"] = 0.5,
        ["classBowLevelMultiply"] = 0.3,
        ["classSlingshotLevelMultiply"] = 0.5,
        ["classKnifeLevelMultiply"] = 0.4,
        ["classAxeLevelMultiply"] = 1.3,
        ["classPickaxeLevelMultiply"] = 2.0,
        ["classShovelLevelMultiply"] = 2.0,
        ["classSpearLevelMultiply"] = 0.6,
        ["classHammerLevelMultiply"] = 1.0,
        ["classSwordLevelMultiply"] = 2.5,
        ["classShieldLevelMultiply"] = 2.0,
        ["classHandLevelMultiply"] = 2.0,
        ["classFarmingLevelMultiply"] = 0.7,
        ["classCookingLevelMultiply"] = 0.5,
        ["classPanningLevelMultiply"] = 0.5,
        ["classVitalityLevelMultiply"] = 1.5,
        ["classMetabolismLevelMultiply"] = 0.5,
        ["classLeatherArmorLevelMultiply"] = 1.0,
        ["classChainArmorLevelMultiply"] = 1.0,
        ["classBrigandineArmorLevelMultiply"] = 1.0,
        ["classPlateArmorLevelMultiply"] = 1.0,
        ["classScaleArmorLevelMultiply"] = 1.0,
        ["classLamellarArmorLevelMultiply"] = 1.0,
        ["classSmithingLevelMultiply"] = 1.5,
    };

    private static Dictionary<string, object> BuildClockmakerClassDefaultConfig() => new()
    {
        ["classHunterLevelMultiply"] = 0.8,
        ["classBowLevelMultiply"] = 0.7,
        ["classSlingshotLevelMultiply"] = 1.0,
        ["classKnifeLevelMultiply"] = 1.0,
        ["classAxeLevelMultiply"] = 1.5,
        ["classPickaxeLevelMultiply"] = 2.0,
        ["classShovelLevelMultiply"] = 2.0,
        ["classSpearLevelMultiply"] = 0.7,
        ["classHammerLevelMultiply"] = 1.5,
        ["classSwordLevelMultiply"] = 0.4,
        ["classShieldLevelMultiply"] = 0.6,
        ["classHandLevelMultiply"] = 0.5,
        ["classFarmingLevelMultiply"] = 1.2,
        ["classCookingLevelMultiply"] = 1.0,
        ["classPanningLevelMultiply"] = 1.4,
        ["classVitalityLevelMultiply"] = 0.7,
        ["classMetabolismLevelMultiply"] = 1.3,
        ["classLeatherArmorLevelMultiply"] = 1.0,
        ["classChainArmorLevelMultiply"] = 1.0,
        ["classBrigandineArmorLevelMultiply"] = 1.0,
        ["classPlateArmorLevelMultiply"] = 1.0,
        ["classScaleArmorLevelMultiply"] = 1.0,
        ["classLamellarArmorLevelMultiply"] = 1.0,
        ["classSmithingLevelMultiply"] = 2.0,
    };

    private static Dictionary<string, object> BuildMalefactorClassDefaultConfig() => new()
    {
        ["classHunterLevelMultiply"] = 1.2,
        ["classBowLevelMultiply"] = 0.6,
        ["classSlingshotLevelMultiply"] = 2.5,
        ["classKnifeLevelMultiply"] = 1.5,
        ["classAxeLevelMultiply"] = 1.0,
        ["classPickaxeLevelMultiply"] = 1.0,
        ["classShovelLevelMultiply"] = 1.0,
        ["classSpearLevelMultiply"] = 1.0,
        ["classHammerLevelMultiply"] = 1.0,
        ["classSwordLevelMultiply"] = 1.0,
        ["classShieldLevelMultiply"] = 1.0,
        ["classHandLevelMultiply"] = 1.5,
        ["classFarmingLevelMultiply"] = 1.5,
        ["classCookingLevelMultiply"] = 2.0,
        ["classPanningLevelMultiply"] = 1.8,
        ["classVitalityLevelMultiply"] = 0.5,
        ["classMetabolismLevelMultiply"] = 1.0,
        ["classLeatherArmorLevelMultiply"] = 1.5,
        ["classChainArmorLevelMultiply"] = 1.5,
        ["classBrigandineArmorLevelMultiply"] = 1.0,
        ["classPlateArmorLevelMultiply"] = 1.0,
        ["classScaleArmorLevelMultiply"] = 1.0,
        ["classLamellarArmorLevelMultiply"] = 1.0,
        ["classSmithingLevelMultiply"] = 0.7,
    };

    private static Dictionary<string, object> BuildTailorClassDefaultConfig() => new()
    {
        ["classHunterLevelMultiply"] = 0.7,
        ["classBowLevelMultiply"] = 1.8,
        ["classSlingshotLevelMultiply"] = 1.0,
        ["classKnifeLevelMultiply"] = 1.8,
        ["classAxeLevelMultiply"] = 1.7,
        ["classPickaxeLevelMultiply"] = 1.4,
        ["classShovelLevelMultiply"] = 1.6,
        ["classSpearLevelMultiply"] = 1.0,
        ["classHammerLevelMultiply"] = 1.0,
        ["classSwordLevelMultiply"] = 0.6,
        ["classShieldLevelMultiply"] = 0.6,
        ["classHandLevelMultiply"] = 0.4,
        ["classFarmingLevelMultiply"] = 2.0,
        ["classCookingLevelMultiply"] = 2.0,
        ["classPanningLevelMultiply"] = 2.5,
        ["classVitalityLevelMultiply"] = 0.3,
        ["classMetabolismLevelMultiply"] = 1.5,
        ["classLeatherArmorLevelMultiply"] = 2.0,
        ["classChainArmorLevelMultiply"] = 2.0,
        ["classBrigandineArmorLevelMultiply"] = 1.0,
        ["classPlateArmorLevelMultiply"] = 1.0,
        ["classScaleArmorLevelMultiply"] = 1.0,
        ["classLamellarArmorLevelMultiply"] = 1.0,
        ["classSmithingLevelMultiply"] = 2.0,
    };

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
                    Dictionary<string, object> configClass = LoadConfigurationByDirectoryAndName(api, "ModConfig/LevelUP/config/classexp", configname, new Dictionary<string, object>());
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

            Dictionary<string, object> hunterclass = LoadConfigurationByDirectoryAndName(api, "ModConfig/LevelUP/config/classexp", "hunterclass", BuildHunterClassDefaultConfig());
            foreach (KeyValuePair<string, object> pair in hunterclass)
            {
                RegisterNewClassLevel("hunterclass", pair.Key, Convert.ToSingle(pair.Value));
            }

            Dictionary<string, object> commonerclass = LoadConfigurationByDirectoryAndName(api, "ModConfig/LevelUP/config/classexp", "commonerclass", BuildCommonerClassDefaultConfig());
            foreach (KeyValuePair<string, object> pair in commonerclass)
            {
                RegisterNewClassLevel("commonerclass", pair.Key, Convert.ToSingle(pair.Value));
            }

            Dictionary<string, object> blackguardclass = LoadConfigurationByDirectoryAndName(api, "ModConfig/LevelUP/config/classexp", "blackguardclass", BuildBlackguardClassDefaultConfig());
            foreach (KeyValuePair<string, object> pair in blackguardclass)
            {
                RegisterNewClassLevel("blackguardclass", pair.Key, Convert.ToSingle(pair.Value));
            }

            Dictionary<string, object> clockmakerclass = LoadConfigurationByDirectoryAndName(api, "ModConfig/LevelUP/config/classexp", "clockmakerclass", BuildClockmakerClassDefaultConfig());
            foreach (KeyValuePair<string, object> pair in clockmakerclass)
            {
                RegisterNewClassLevel("clockmakerclass", pair.Key, Convert.ToSingle(pair.Value));
            }

            Dictionary<string, object> malefactorclass = LoadConfigurationByDirectoryAndName(api, "ModConfig/LevelUP/config/classexp", "malefactorclass", BuildMalefactorClassDefaultConfig());
            foreach (KeyValuePair<string, object> pair in malefactorclass)
            {
                RegisterNewClassLevel("malefactorclass", pair.Key, Convert.ToSingle(pair.Value));
            }

            Dictionary<string, object> tailorclass = LoadConfigurationByDirectoryAndName(api, "ModConfig/LevelUP/config/classexp", "tailorclass", BuildTailorClassDefaultConfig());
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