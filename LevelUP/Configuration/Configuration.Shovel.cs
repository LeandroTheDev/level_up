using System;
using System.Collections.Generic;
using LevelUP.Server;
using OpenConfiguration;
using Vintagestory.API.Common;

namespace LevelUP;

public class ShovelLevelStatsConfiguration
{
    public int shovelEXPPerHit = 10;
    public int shovelEXPPerBreaking = 10;
    public int shovelEXPPerLevelBase = 500;
    public double shovelEXPMultiplyPerLevel = 1.5;
    public float shovelBaseDamage = 1.0f;
    public float shovelIncrementDamagePerLevel = 0.03f;
    public float shovelBaseMiningSpeed = 1.0f;
    public float shovelIncrementMiningSpeedMultiplyPerLevel = 0.02f;
    public int shovelMaxLevel = 999;
}

public static partial class Configuration
{
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

    public static void PopulateShovelConfiguration(ICoreAPI api)
    {
        ShovelLevelStatsConfiguration shovelLevelStats = ConfigManager.Load<ShovelLevelStatsConfiguration>(
            api, "ModConfig/LevelUP/config/levelstats", "shovel", Logger(api));

        shovelEXPPerLevelBase = shovelLevelStats.shovelEXPPerLevelBase;
        shovelEXPMultiplyPerLevel = shovelLevelStats.shovelEXPMultiplyPerLevel;
        shovelBaseDamage = shovelLevelStats.shovelBaseDamage;
        shovelIncrementDamagePerLevel = shovelLevelStats.shovelIncrementDamagePerLevel;
        shovelEXPPerHit = shovelLevelStats.shovelEXPPerHit;
        Experience.LoadExperience("Shovel", "Hit", (ulong)shovelEXPPerHit);
        shovelEXPPerBreaking = shovelLevelStats.shovelEXPPerBreaking;
        Experience.LoadExperience("Shovel", "Break", (ulong)shovelEXPPerBreaking);
        shovelBaseMiningSpeed = shovelLevelStats.shovelBaseMiningSpeed;
        shovelIncrementMiningSpeedMultiplyPerLevel = shovelLevelStats.shovelIncrementMiningSpeedMultiplyPerLevel;
        shovelMaxLevel = shovelLevelStats.shovelMaxLevel;

        entityExpShovel = ConfigManager.Load<Dictionary<string, int>>(
            api, "ModConfig/LevelUP/config/entityexp", "shovel", Logger(api), "levelup:config/entityexp/shovel.json");

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
}
