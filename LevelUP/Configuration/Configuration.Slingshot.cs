using System;
using System.Collections.Generic;
using LevelUP.Server;
using OpenConfiguration;
using Vintagestory.API.Common;

namespace LevelUP;

public class SlingshotLevelStatsConfiguration
{
    public int slingshotEXPPerHit = 10;
    public int slingshotEXPPerLevelBase = 500;
    public double slingshotEXPMultiplyPerLevel = 1.1;
    public float slingshotBaseDamage = 1.0f;
    public float slingshotIncrementDamagePerLevel = 0.08f;
    public float slingshotBaseChanceToNotLoseRock = 50.0f;
    public float slingshotChanceToNotLoseRockBaseIncreasePerLevel = 0.5f;
    public int slingshotChanceToNotLoseRockReduceIncreaseEveryLevel = 5;
    public float slingshotChanceToNotLoseRockReduceQuantityEveryLevel = 0.2f;
    public float slingshotBaseAimAccuracy = 0.8f;
    public float slingshotIncreaseAimAccuracyPerLevel = 0.02f;
    public int slingshotMaxLevel = 999;
}

public static partial class Configuration
{
    public static Dictionary<string, int> entityExpSlingshot = [];
    private static int slingshotEXPPerHit = 10;
    private static int slingshotEXPPerLevelBase = 500;
    private static double slingshotEXPMultiplyPerLevel = 1.1;
    private static float slingshotBaseDamage = 1.0f;
    private static float slingshotIncrementDamagePerLevel = 0.08f;
    private static float slingshotBaseChanceToNotLoseRock = 50.0f;
    private static float slingshotChanceToNotLoseRockBaseIncreasePerLevel = 0.5f;
    private static int slingshotChanceToNotLoseRockReduceIncreaseEveryLevel = 5;
    private static float slingshotChanceToNotLoseRockReduceQuantityEveryLevel = 0.2f;
    private static float slingshotBaseAimAccuracy = 0.8f;
    private static float slingshotIncreaseAimAccuracyPerLevel = 0.02f;
    public static int slingshotMaxLevel = 999;

    public static int ExpPerHitSlingshot => slingshotEXPPerHit;
    public static float BaseAimAccuracySlingshot => slingshotBaseAimAccuracy;

    public static void PopulateSlingshotConfiguration(ICoreAPI api)
    {
        SlingshotLevelStatsConfiguration slingshotLevelStats = ConfigManager.Load<SlingshotLevelStatsConfiguration>(
            api, "ModConfig/LevelUP/levelstats", "slingshot", Logger(api));

        slingshotEXPPerLevelBase = slingshotLevelStats.slingshotEXPPerLevelBase;
        slingshotEXPMultiplyPerLevel = slingshotLevelStats.slingshotEXPMultiplyPerLevel;
        slingshotBaseDamage = slingshotLevelStats.slingshotBaseDamage;
        slingshotIncrementDamagePerLevel = slingshotLevelStats.slingshotIncrementDamagePerLevel;
        slingshotEXPPerHit = slingshotLevelStats.slingshotEXPPerHit;
        Experience.LoadExperience("Slingshot", "Hit", (ulong)slingshotEXPPerHit);
        slingshotBaseChanceToNotLoseRock = slingshotLevelStats.slingshotBaseChanceToNotLoseRock;
        slingshotChanceToNotLoseRockBaseIncreasePerLevel = slingshotLevelStats.slingshotChanceToNotLoseRockBaseIncreasePerLevel;
        slingshotChanceToNotLoseRockReduceIncreaseEveryLevel = slingshotLevelStats.slingshotChanceToNotLoseRockReduceIncreaseEveryLevel;
        slingshotChanceToNotLoseRockReduceQuantityEveryLevel = slingshotLevelStats.slingshotChanceToNotLoseRockReduceQuantityEveryLevel;
        slingshotBaseAimAccuracy = slingshotLevelStats.slingshotBaseAimAccuracy;
        slingshotIncreaseAimAccuracyPerLevel = slingshotLevelStats.slingshotIncreaseAimAccuracyPerLevel;
        slingshotMaxLevel = slingshotLevelStats.slingshotMaxLevel;

        entityExpSlingshot = ConfigManager.Load<Dictionary<string, int>>(
            api, "ModConfig/LevelUP/entityexp", "slingshot", Logger(api), "levelup:config/entityexp/slingshot.json");

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
}
