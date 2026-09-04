using System;
using System.Collections.Generic;
using LevelUP.Server;
using OpenConfiguration;
using Vintagestory.API.Common;

namespace LevelUP;

public class SpearLevelStatsConfiguration
{
    public int spearEXPPerHit = 10;
    public int spearEXPPerThrow = 20;
    public int spearEXPPerLevelBase = 500;
    public double spearEXPMultiplyPerLevel = 1.3;
    public float spearBaseDamage = 1.0f;
    public float spearIncrementDamagePerLevel = 0.04f;
    public float spearBaseRangedAccuracy = 0.0f;
    public float spearIncrementRangedAccuracyPerLevel = 0.015f;
    public float spearBaseRangedSpeed = 0.0f;
    public float spearIncrementRangedSpeedPerLevel = 0.01f;
    public float spearBaseMovePenaltyReduction = 0.0f;
    public float spearIncrementMovePenaltyReductionPerLevel = 0.005f;
    public int spearMaxLevel = 999;
}

public static partial class Configuration
{
    public static Dictionary<string, int> entityExpSpear = [];
    private static int spearEXPPerHit = 10;
    private static int spearEXPPerThrow = 20;
    private static int spearEXPPerLevelBase = 500;
    private static double spearEXPMultiplyPerLevel = 1.3;
    private static float spearBaseDamage = 1.0f;
    private static float spearIncrementDamagePerLevel = 0.04f;
    private static float spearBaseRangedAccuracy = 0.0f;
    private static float spearIncrementRangedAccuracyPerLevel = 0.015f;
    private static float spearBaseRangedSpeed = 0.0f;
    private static float spearIncrementRangedSpeedPerLevel = 0.01f;
    private static float spearBaseMovePenaltyReduction = 0.0f;
    private static float spearIncrementMovePenaltyReductionPerLevel = 0.005f;
    public static int spearMaxLevel = 999;

    public static int ExpPerHitSpear => spearEXPPerHit;
    public static int ExpPerThrowSpear => spearEXPPerThrow;

    public static void PopulateSpearConfiguration(ICoreAPI api)
    {
        SpearLevelStatsConfiguration spearLevelStats = ConfigManager.Load<SpearLevelStatsConfiguration>(
            api, "ModConfig/LevelUP/config/levelstats", "spear", Logger(api));

        spearEXPPerLevelBase = spearLevelStats.spearEXPPerLevelBase;
        spearEXPMultiplyPerLevel = spearLevelStats.spearEXPMultiplyPerLevel;
        spearBaseDamage = spearLevelStats.spearBaseDamage;
        spearIncrementDamagePerLevel = spearLevelStats.spearIncrementDamagePerLevel;

        spearEXPPerHit = spearLevelStats.spearEXPPerHit;
        Experience.LoadExperience("Spear", "Hit", (ulong)spearEXPPerHit);

        spearEXPPerThrow = spearLevelStats.spearEXPPerThrow;
        Experience.LoadExperience("Spear", "Throw", (ulong)spearEXPPerThrow);

        spearBaseRangedAccuracy = spearLevelStats.spearBaseRangedAccuracy;
        spearIncrementRangedAccuracyPerLevel = spearLevelStats.spearIncrementRangedAccuracyPerLevel;
        spearBaseRangedSpeed = spearLevelStats.spearBaseRangedSpeed;
        spearIncrementRangedSpeedPerLevel = spearLevelStats.spearIncrementRangedSpeedPerLevel;
        spearBaseMovePenaltyReduction = spearLevelStats.spearBaseMovePenaltyReduction;
        spearIncrementMovePenaltyReductionPerLevel = spearLevelStats.spearIncrementMovePenaltyReductionPerLevel;
        spearMaxLevel = spearLevelStats.spearMaxLevel;

        entityExpSpear = ConfigManager.Load<Dictionary<string, int>>(
            api, "ModConfig/LevelUP/config/entityexp", "spear", Logger(api), "levelup:config/entityexp/spear.json");

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
}
