using System;
using System.Collections.Generic;
using LevelUP.Server;
using OpenConfiguration;
using Vintagestory.API.Common;

namespace LevelUP;

public class BowLevelStatsConfiguration
{
    public int bowEXPPerHit = 10;
    public int bowEXPPerLevelBase = 500;
    public double bowEXPMultiplyPerLevel = 1.1;
    public float bowBaseDamage = 1.0f;
    public float bowIncrementDamagePerLevel = 0.05f;
    public float bowChanceToNotLoseArrowBaseIncreasePerLevel = 1.0f;
    public int bowChanceToNotLoseArrowReduceIncreaseEveryLevel = 5;
    public float bowChanceToNotLoseArrowReduceQuantityEveryLevel = 0.2f;
    public float bowBaseRangedAccuracy = 0.0f;
    public float bowIncrementRangedAccuracyPerLevel = 0.015f;
    public float bowBaseRangedSpeed = 0.0f;
    public float bowIncrementRangedSpeedPerLevel = 0.01f;
    public float bowBaseMovePenaltyReduction = 0.0f;
    public float bowIncrementMovePenaltyReductionPerLevel = 0.005f;
    public int bowMaxLevel = 999;
}

public static partial class Configuration
{
    public static Dictionary<string, int> entityExpBow = [];
    private static int bowEXPPerHit = 10;
    private static int bowEXPPerLevelBase = 500;
    private static double bowEXPMultiplyPerLevel = 1.1;
    private static float bowBaseDamage = 1.0f;
    private static float bowIncrementDamagePerLevel = 0.05f;
    private static float bowChanceToNotLoseArrowBaseIncreasePerLevel = 1.0f;
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

    public static void PopulateBowConfiguration(ICoreAPI api)
    {
        BowLevelStatsConfiguration bowLevelStats = ConfigManager.Load<BowLevelStatsConfiguration>(
            api, "ModConfig/LevelUP/levelstats", "bow", Logger(api));

        bowEXPPerLevelBase = bowLevelStats.bowEXPPerLevelBase;
        bowEXPMultiplyPerLevel = bowLevelStats.bowEXPMultiplyPerLevel;
        bowBaseDamage = bowLevelStats.bowBaseDamage;
        bowIncrementDamagePerLevel = bowLevelStats.bowIncrementDamagePerLevel;
        bowEXPPerHit = bowLevelStats.bowEXPPerHit;
        Experience.LoadExperience("Bow", "Hit", (ulong)bowEXPPerHit);
        bowChanceToNotLoseArrowBaseIncreasePerLevel = bowLevelStats.bowChanceToNotLoseArrowBaseIncreasePerLevel;
        bowChanceToNotLoseArrowReduceIncreaseEveryLevel = bowLevelStats.bowChanceToNotLoseArrowReduceIncreaseEveryLevel;
        bowChanceToNotLoseArrowReduceQuantityEveryLevel = bowLevelStats.bowChanceToNotLoseArrowReduceQuantityEveryLevel;
        bowBaseRangedAccuracy = bowLevelStats.bowBaseRangedAccuracy;
        bowIncrementRangedAccuracyPerLevel = bowLevelStats.bowIncrementRangedAccuracyPerLevel;
        bowBaseRangedSpeed = bowLevelStats.bowBaseRangedSpeed;
        bowIncrementRangedSpeedPerLevel = bowLevelStats.bowIncrementRangedSpeedPerLevel;
        bowBaseMovePenaltyReduction = bowLevelStats.bowBaseMovePenaltyReduction;
        bowIncrementMovePenaltyReductionPerLevel = bowLevelStats.bowIncrementMovePenaltyReductionPerLevel;
        bowMaxLevel = bowLevelStats.bowMaxLevel;

        entityExpBow = ConfigManager.Load<Dictionary<string, int>>(
            api, "ModConfig/LevelUP/entityexp", "bow", Logger(api), "levelup:config/entityexp/bow.json");

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
}
