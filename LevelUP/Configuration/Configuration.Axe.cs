using System;
using System.Collections.Generic;
using LevelUP.Server;
using OpenConfiguration;
using Vintagestory.API.Common;

namespace LevelUP;

public class AxeLevelStatsConfiguration
{
    public int axeEXPPerHit = 10;
    public int axeEXPPerBreaking = 5;
    public int axeEXPPerTreeBreaking = 200;
    public int axeEXPPerLevelBase = 1000;
    public double axeEXPMultiplyPerLevel = 1.2;
    public float axeBaseDamage = 1.0f;
    public float axeIncrementDamagePerLevel = 0.05f;
    public float axeBaseMiningSpeed = 1.0f;
    public float axeIncrementMiningSpeedMultiplyPerLevel = 0.05f;
    public int axeMaxLevel = 999;
}

public static partial class Configuration
{
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

    public static void PopulateAxeConfiguration(ICoreAPI api)
    {
        AxeLevelStatsConfiguration axeLevelStats = ConfigManager.Load<AxeLevelStatsConfiguration>(
            api, "ModConfig/LevelUP/config/levelstats", "axe", Logger(api));

        axeEXPPerLevelBase = axeLevelStats.axeEXPPerLevelBase;
        axeEXPMultiplyPerLevel = axeLevelStats.axeEXPMultiplyPerLevel;
        axeBaseDamage = axeLevelStats.axeBaseDamage;
        axeIncrementDamagePerLevel = axeLevelStats.axeIncrementDamagePerLevel;
        axeEXPPerHit = axeLevelStats.axeEXPPerHit;
        Experience.LoadExperience("Axe", "Hit", (ulong)axeEXPPerHit);
        axeEXPPerBreaking = axeLevelStats.axeEXPPerBreaking;
        Experience.LoadExperience("Axe", "Break", (ulong)axeEXPPerBreaking);
        axeEXPPerTreeBreaking = axeLevelStats.axeEXPPerTreeBreaking;
        Experience.LoadExperience("Axe", "TreeBreak", (ulong)axeEXPPerTreeBreaking);
        axeBaseMiningSpeed = axeLevelStats.axeBaseMiningSpeed;
        axeIncrementMiningSpeedMultiplyPerLevel = axeLevelStats.axeIncrementMiningSpeedMultiplyPerLevel;
        axeMaxLevel = axeLevelStats.axeMaxLevel;

        entityExpAxe = ConfigManager.Load<Dictionary<string, int>>(
            api, "ModConfig/LevelUP/config/entityexp", "axe", Logger(api), "levelup:config/entityexp/axe.json");

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
}
