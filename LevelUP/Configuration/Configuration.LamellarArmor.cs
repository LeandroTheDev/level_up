using System;
using System.Collections.Generic;
using LevelUP.Server;
using OpenConfiguration;
using Vintagestory.API.Common;

namespace LevelUP;

public class LamellarArmorLevelStatsConfiguration
{
    public int lamellarArmorEXPPerReceiveHit = 10;
    public float lamellarArmorEXPMultiplyByDamage = 0.3f;
    public int lamellarArmorEXPIncreaseByAmountDamage = 2;
    public int lamellarArmorEXPPerLevelBase = 500;
    public double lamellarArmorEXPMultiplyPerLevel = 1.2;

    public float lamellarArmorRelativeProtectionMultiply = 1.0f;
    public float lamellarArmorRelativeProtectionMultiplyPerLevel = 0.065f;
    public int lamellarArmorRelativeProtectionMultiplyReductionEveryLevel = 1;
    public float lamellarArmorRelativeProtectionMultiplyReductionPerReduce = 0.15f;

    public float lamellarArmorFlatDamageReductionMultiply = 1.0f;
    public float lamellarArmorFlatDamageReductionMultiplyPerLevel = 0.065f;
    public int lamellarArmorFlatDamageReductionMultiplyReductionEveryLevel = 1;
    public float lamellarArmorFlatDamageReductionMultiplyReductionPerReduce = 0.05f;

    public float lamellarArmorHealingEffectivnessMultiply = 1.0f;
    public float lamellarArmorHealingEffectivnessMultiplyPerLevel = 0.05f;
    public int lamellarArmorHealingEffectivnessMultiplyReductionEveryLevel = 1;
    public float lamellarArmorHealingEffectivnessMultiplyReductionPerReduce = 0.05f;

    public float lamellarArmorHungerRateMultiply = 1.0f;
    public float lamellarArmorHungerRateMultiplyPerLevel = 0.05f;
    public int lamellarArmorHungerRateMultiplyReductionEveryLevel = 1;
    public float lamellarArmorHungerRateMultiplyReductionPerReduce = 0.05f;

    public float lamellarArmorRangedWeaponsAccuracyMultiply = 1.0f;
    public float lamellarArmorRangedWeaponsAccuracyMultiplyPerLevel = 0.01f;
    public int lamellarArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel = 1;
    public float lamellarArmorRangedWeaponsAccuracyMultiplyReductionPerReduce = 0.05f;

    public float lamellarArmorRangedWeaponsSpeedMultiply = 1.0f;
    public float lamellarArmorRangedWeaponsSpeedMultiplyPerLevel = 0.01f;
    public int lamellarArmorRangedWeaponsSpeedMultiplyReductionEveryLevel = 1;
    public float lamellarArmorRangedWeaponsSpeedMultiplyReductionPerReduce = 0.05f;

    public float lamellarArmorWalkSpeedMultiply = 1.0f;
    public float lamellarArmorWalkSpeedMultiplyPerLevel = 0.01f;
    public int lamellarArmorWalkSpeedMultiplyReductionEveryLevel = 1;
    public float lamellarArmorWalkSpeedMultiplyReductionPerReduce = 0.05f;

    public int lamellarArmorMaxLevel = 999;
    public double lamellarArmorSubLevelEXPMultiply = 3.0;
}

public class LamellarArmorItemsConfiguration : Dictionary<string, double>
{
    public LamellarArmorItemsConfiguration() : base(new Dictionary<string, double>
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
    })
    { }
}

public static partial class Configuration
{
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

    public static void PopulateLamellarArmorConfiguration(ICoreAPI api)
    {
        LamellarArmorLevelStatsConfiguration lamellarArmorLevelStats = ConfigManager.Load<LamellarArmorLevelStatsConfiguration>(
            api, "ModConfig/LevelUP/config/levelstats", "lamellararmor", Logger(api));

        lamellarArmorEXPPerReceiveHit = lamellarArmorLevelStats.lamellarArmorEXPPerReceiveHit;
        Experience.LoadExperience("LamellarArmor", "Hit", (ulong)lamellarArmorEXPPerReceiveHit);
        lamellarArmorEXPMultiplyByDamage = lamellarArmorLevelStats.lamellarArmorEXPMultiplyByDamage;
        lamellarArmorEXPIncreaseByAmountDamage = lamellarArmorLevelStats.lamellarArmorEXPIncreaseByAmountDamage;
        lamellarArmorEXPPerLevelBase = lamellarArmorLevelStats.lamellarArmorEXPPerLevelBase;
        lamellarArmorEXPMultiplyPerLevel = lamellarArmorLevelStats.lamellarArmorEXPMultiplyPerLevel;

        lamellarArmorRelativeProtectionMultiply = lamellarArmorLevelStats.lamellarArmorRelativeProtectionMultiply;
        lamellarArmorRelativeProtectionMultiplyPerLevel = lamellarArmorLevelStats.lamellarArmorRelativeProtectionMultiplyPerLevel;
        lamellarArmorRelativeProtectionMultiplyReductionEveryLevel = lamellarArmorLevelStats.lamellarArmorRelativeProtectionMultiplyReductionEveryLevel;
        lamellarArmorRelativeProtectionMultiplyReductionPerReduce = lamellarArmorLevelStats.lamellarArmorRelativeProtectionMultiplyReductionPerReduce;

        lamellarArmorFlatDamageReductionMultiply = lamellarArmorLevelStats.lamellarArmorFlatDamageReductionMultiply;
        lamellarArmorFlatDamageReductionMultiplyPerLevel = lamellarArmorLevelStats.lamellarArmorFlatDamageReductionMultiplyPerLevel;
        lamellarArmorFlatDamageReductionMultiplyReductionEveryLevel = lamellarArmorLevelStats.lamellarArmorFlatDamageReductionMultiplyReductionEveryLevel;
        lamellarArmorFlatDamageReductionMultiplyReductionPerReduce = lamellarArmorLevelStats.lamellarArmorFlatDamageReductionMultiplyReductionPerReduce;

        lamellarArmorHealingEffectivnessMultiply = lamellarArmorLevelStats.lamellarArmorHealingEffectivnessMultiply;
        lamellarArmorHealingEffectivnessMultiplyPerLevel = lamellarArmorLevelStats.lamellarArmorHealingEffectivnessMultiplyPerLevel;
        lamellarArmorHealingEffectivnessMultiplyReductionEveryLevel = lamellarArmorLevelStats.lamellarArmorHealingEffectivnessMultiplyReductionEveryLevel;
        lamellarArmorHealingEffectivnessMultiplyReductionPerReduce = lamellarArmorLevelStats.lamellarArmorHealingEffectivnessMultiplyReductionPerReduce;

        lamellarArmorHungerRateMultiply = lamellarArmorLevelStats.lamellarArmorHungerRateMultiply;
        lamellarArmorHungerRateMultiplyPerLevel = lamellarArmorLevelStats.lamellarArmorHungerRateMultiplyPerLevel;
        lamellarArmorHungerRateMultiplyReductionEveryLevel = lamellarArmorLevelStats.lamellarArmorHungerRateMultiplyReductionEveryLevel;
        lamellarArmorHungerRateMultiplyReductionPerReduce = lamellarArmorLevelStats.lamellarArmorHungerRateMultiplyReductionPerReduce;

        lamellarArmorRangedWeaponsAccuracyMultiply = lamellarArmorLevelStats.lamellarArmorRangedWeaponsAccuracyMultiply;
        lamellarArmorRangedWeaponsAccuracyMultiplyPerLevel = lamellarArmorLevelStats.lamellarArmorRangedWeaponsAccuracyMultiplyPerLevel;
        lamellarArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel = lamellarArmorLevelStats.lamellarArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel;
        lamellarArmorRangedWeaponsAccuracyMultiplyReductionPerReduce = lamellarArmorLevelStats.lamellarArmorRangedWeaponsAccuracyMultiplyReductionPerReduce;

        lamellarArmorRangedWeaponsSpeedMultiply = lamellarArmorLevelStats.lamellarArmorRangedWeaponsSpeedMultiply;
        lamellarArmorRangedWeaponsSpeedMultiplyPerLevel = lamellarArmorLevelStats.lamellarArmorRangedWeaponsSpeedMultiplyPerLevel;
        lamellarArmorRangedWeaponsSpeedMultiplyReductionEveryLevel = lamellarArmorLevelStats.lamellarArmorRangedWeaponsSpeedMultiplyReductionEveryLevel;
        lamellarArmorRangedWeaponsSpeedMultiplyReductionPerReduce = lamellarArmorLevelStats.lamellarArmorRangedWeaponsSpeedMultiplyReductionPerReduce;

        lamellarArmorWalkSpeedMultiply = lamellarArmorLevelStats.lamellarArmorWalkSpeedMultiply;
        lamellarArmorWalkSpeedMultiplyPerLevel = lamellarArmorLevelStats.lamellarArmorWalkSpeedMultiplyPerLevel;
        lamellarArmorWalkSpeedMultiplyReductionEveryLevel = lamellarArmorLevelStats.lamellarArmorWalkSpeedMultiplyReductionEveryLevel;
        lamellarArmorWalkSpeedMultiplyReductionPerReduce = lamellarArmorLevelStats.lamellarArmorWalkSpeedMultiplyReductionPerReduce;

        lamellarArmorMaxLevel = lamellarArmorLevelStats.lamellarArmorMaxLevel;
        lamellarArmorSubLevelEXPMultiply = lamellarArmorLevelStats.lamellarArmorSubLevelEXPMultiply;

        expMultiplyHitLamellarArmor = ConfigManager.Load<LamellarArmorItemsConfiguration>(
            api, "ModConfig/LevelUP/config/levelstats", "lamellararmoritems", Logger(api));

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
}
