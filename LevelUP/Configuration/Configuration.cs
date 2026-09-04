using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenConfiguration;
using Vintagestory.API.Common;

namespace LevelUP;

#pragma warning disable CA2211
#pragma warning disable IDE0044
public static partial class Configuration
{
    /// <summary>Shared RNG used by several level types (chance rolls for double/triple/quadruple outputs, serving bonuses, etc).</summary>
    private static readonly Random Random = new();

    /// <summary>Per-mod logger for OpenConfiguration's own diagnostics (missing/invalid keys, IO errors).</summary>
    private static ModLogger Logger(ICoreAPI api) => new(api.Logger, "LevelUP", enableExtendedLog);

    #region baseconfigs
    /// <summary>Sync key this mod registers with <see cref="OpenConfiguration.ConfigManager.RegisterSync(Vintagestory.API.Server.ICoreServerAPI, string, Func{string})"/>.</summary>
    internal const string ConfigSyncKey = "levelup:baseconfig";

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
}
