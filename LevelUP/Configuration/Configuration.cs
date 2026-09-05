using System;
using System.Collections.Generic;
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
    internal static ModLogger Logger(ICoreAPI api) => new(api.Logger, "LevelUP", enableExtendedLog);

    /// <summary>
    /// Sync key this mod registers with <see cref="ConfigManager.RegisterStaticFieldSync(Vintagestory.API.Server.ICoreServerAPI, string, Type)"/>/
    /// <see cref="ConfigManager.RegisterStaticFieldSync(Vintagestory.API.Client.ICoreClientAPI, string, Type, Action, ModLogger)"/>,
    /// which sync every static primitive/string/Dictionary&lt;string, double&gt; field on this class from server to client.
    /// </summary>
    internal static string ConfigSyncKey => "levelup:config";

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
