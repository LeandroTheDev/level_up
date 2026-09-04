using System;
using System.Collections.Generic;
using LevelUP.Server;
using OpenConfiguration;
using Vintagestory.API.Common;

namespace LevelUP;

public class KnifeLevelStatsConfiguration
{
    public int knifeEXPPerHit = 10;
    public int knifeEXPPerHarvest = 50;
    public int knifeEXPPerBreaking = 10;
    public int knifeEXPPerLevelBase = 500;
    public double knifeEXPMultiplyPerLevel = 1.3;
    public float knifeBaseDamage = 1.0f;
    public float knifeIncrementDamagePerLevel = 0.03f;
    public float knifeBaseHarvestMultiply = 0.5f;
    public float knifeIncrementHarvestMultiplyPerLevel = 0.04f;
    public float knifeBaseMiningSpeed = 1.0f;
    public float knifeIncrementMiningSpeedMultiplyPerLevel = 0.05f;
    public int knifeMaxLevel = 999;
}

public static partial class Configuration
{
    public static Dictionary<string, int> entityExpKnife = [];
    private static int knifeEXPPerHit = 10;
    private static int knifeEXPPerHarvest = 50;
    private static int knifeEXPPerBreaking = 10;
    private static int knifeEXPPerLevelBase = 500;
    private static double knifeEXPMultiplyPerLevel = 1.3;
    private static float knifeBaseDamage = 1.0f;
    private static float knifeIncrementDamagePerLevel = 0.03f;
    private static float knifeBaseHarvestMultiply = 0.5f;
    private static float knifeIncrementHarvestMultiplyPerLevel = 0.04f;
    private static float knifeBaseMiningSpeed = 1.0f;
    private static float knifeIncrementMiningSpeedMultiplyPerLevel = 0.05f;
    public static int knifeMaxLevel = 999;

    public static int ExpPerHitKnife => knifeEXPPerHit;
    public static int ExpPerHarvestKnife => knifeEXPPerHarvest;
    public static int ExpPerBreakingKnife => knifeEXPPerBreaking;
    public static float BaseHarvestMultiplyKnife = knifeBaseHarvestMultiply;
    public static float BaseMinigSpeedKnife = knifeBaseMiningSpeed;

    public static void PopulateKnifeConfiguration(ICoreAPI api)
    {
        KnifeLevelStatsConfiguration knifeLevelStats = ConfigManager.Load<KnifeLevelStatsConfiguration>(
            api, "ModConfig/LevelUP/config/levelstats", "knife", Logger(api));

        knifeEXPPerLevelBase = knifeLevelStats.knifeEXPPerLevelBase;
        knifeEXPMultiplyPerLevel = knifeLevelStats.knifeEXPMultiplyPerLevel;
        knifeBaseDamage = knifeLevelStats.knifeBaseDamage;
        knifeIncrementDamagePerLevel = knifeLevelStats.knifeIncrementDamagePerLevel;
        knifeEXPPerHit = knifeLevelStats.knifeEXPPerHit;
        Experience.LoadExperience("Knife", "Hit", (ulong)knifeEXPPerHit);
        knifeEXPPerHarvest = knifeLevelStats.knifeEXPPerHarvest;
        Experience.LoadExperience("Knife", "Harvest", (ulong)knifeEXPPerHarvest);
        knifeEXPPerBreaking = knifeLevelStats.knifeEXPPerBreaking;
        Experience.LoadExperience("Knife", "Break", (ulong)knifeEXPPerBreaking);
        knifeBaseHarvestMultiply = knifeLevelStats.knifeBaseHarvestMultiply;
        knifeIncrementHarvestMultiplyPerLevel = knifeLevelStats.knifeIncrementHarvestMultiplyPerLevel;
        knifeBaseMiningSpeed = knifeLevelStats.knifeBaseMiningSpeed;
        knifeIncrementMiningSpeedMultiplyPerLevel = knifeLevelStats.knifeIncrementMiningSpeedMultiplyPerLevel;
        knifeMaxLevel = knifeLevelStats.knifeMaxLevel;

        entityExpKnife = ConfigManager.Load<Dictionary<string, int>>(
            api, "ModConfig/LevelUP/config/entityexp", "knife", Logger(api), "levelup:config/entityexp/knife.json");

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
}
