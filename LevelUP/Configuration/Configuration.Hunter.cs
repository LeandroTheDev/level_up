using System;
using System.Collections.Generic;
using OpenConfiguration;
using Vintagestory.API.Common;

namespace LevelUP;

public class HunterLevelStatsConfiguration
{
    public int hunterEXPPerLevelBase = 800;
    public double hunterEXPMultiplyPerLevel = 1.2;
    public float hunterBaseDamage = 1.0f;
    public float hunterIncrementDamagePerLevel = 0.03f;
    public int hunterMaxLevel = 999;
}

public static partial class Configuration
{
    public static Dictionary<string, int> entityExpHunter = [];
    private static int hunterEXPPerLevelBase = 800;
    private static double hunterEXPMultiplyPerLevel = 1.2;
    private static float hunterBaseDamage = 1.0f;
    private static float hunterIncrementDamagePerLevel = 0.03f;
    public static int hunterMaxLevel = 999;

    public static void PopulateHunterConfiguration(ICoreAPI api)
    {
        HunterLevelStatsConfiguration hunterLevelStats = ConfigManager.Load<HunterLevelStatsConfiguration>(
            api, "ModConfig/LevelUP/config/levelstats", "hunter", Logger(api));
        hunterEXPPerLevelBase = hunterLevelStats.hunterEXPPerLevelBase;
        hunterEXPMultiplyPerLevel = hunterLevelStats.hunterEXPMultiplyPerLevel;
        hunterBaseDamage = hunterLevelStats.hunterBaseDamage;
        hunterIncrementDamagePerLevel = hunterLevelStats.hunterIncrementDamagePerLevel;
        hunterMaxLevel = hunterLevelStats.hunterMaxLevel;

        entityExpHunter = ConfigManager.Load<Dictionary<string, int>>(
            api, "ModConfig/LevelUP/config/entityexp", "hunter", Logger(api), "levelup:config/entityexp/hunter.json");

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
}
