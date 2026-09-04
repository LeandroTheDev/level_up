using System;
using System.Collections.Generic;
using LevelUP.Server;
using OpenConfiguration;
using Vintagestory.API.Common;

namespace LevelUP;

public class LeatherArmorLevelStatsConfiguration
{
    public int leatherArmorEXPPerReceiveHit = 10;
    public float leatherArmorEXPMultiplyByDamage = 0.3f;
    public int leatherArmorEXPIncreaseByAmountDamage = 2;
    public int leatherArmorEXPPerLevelBase = 500;
    public double leatherArmorEXPMultiplyPerLevel = 1.2;

    public float leatherArmorRelativeProtectionMultiply = 1.0f;
    public float leatherArmorRelativeProtectionMultiplyPerLevel = 0.015f;
    public int leatherArmorRelativeProtectionMultiplyReductionEveryLevel = 1;
    public float leatherArmorRelativeProtectionMultiplyReductionPerReduce = 0.25f;

    public float leatherArmorFlatDamageReductionMultiply = 1.0f;
    public float leatherArmorFlatDamageReductionMultiplyPerLevel = 0.015f;
    public int leatherArmorFlatDamageReductionMultiplyReductionEveryLevel = 1;
    public float leatherArmorFlatDamageReductionMultiplyReductionPerReduce = 0.05f;

    public float leatherArmorHealingEffectivnessMultiply = 1.0f;
    public float leatherArmorHealingEffectivnessMultiplyPerLevel = 0.035f;
    public int leatherArmorHealingEffectivnessMultiplyReductionEveryLevel = 1;
    public float leatherArmorHealingEffectivnessMultiplyReductionPerReduce = 0.05f;

    public float leatherArmorHungerRateMultiply = 1.0f;
    public float leatherArmorHungerRateMultiplyPerLevel = 0.02f;
    public int leatherArmorHungerRateMultiplyReductionEveryLevel = 1;
    public float leatherArmorHungerRateMultiplyReductionPerReduce = 0.05f;

    public float leatherArmorRangedWeaponsAccuracyMultiply = 1.0f;
    public float leatherArmorRangedWeaponsAccuracyMultiplyPerLevel = 0.075f;
    public int leatherArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel = 1;
    public float leatherArmorRangedWeaponsAccuracyMultiplyReductionPerReduce = 0.05f;

    public float leatherArmorRangedWeaponsSpeedMultiply = 1.0f;
    public float leatherArmorRangedWeaponsSpeedMultiplyPerLevel = 0.075f;
    public int leatherArmorRangedWeaponsSpeedMultiplyReductionEveryLevel = 1;
    public float leatherArmorRangedWeaponsSpeedMultiplyReductionPerReduce = 0.05f;

    public float leatherArmorWalkSpeedMultiply = 1.0f;
    public float leatherArmorWalkSpeedMultiplyPerLevel = 0.06f;
    public int leatherArmorWalkSpeedMultiplyReductionEveryLevel = 1;
    public float leatherArmorWalkSpeedMultiplyReductionPerReduce = 0.05f;

    public int leatherArmorMaxLevel = 999;
    public double leatherArmorSubLevelEXPMultiply = 3.0;
}

public class LeatherArmorItemsConfiguration : Dictionary<string, double>
{
    public LeatherArmorItemsConfiguration() : base(new Dictionary<string, double>
    {
        ["game:armor-head-sewn-leather"] = 0.2,
        ["game:armor-body-sewn-leather"] = 0.5,
        ["game:armor-legs-sewn-leather"] = 0.2,
        ["game:armor-body-jerkin-leather"] = 0.4,
        ["game:armor-legs-jerkin-leather"] = 0.2,
        ["game:armor-head-hide-bear-black"] = 0.3,
        ["game:armor-body-hide-bear-black"] = 0.5,
        ["game:armor-legs-hide-bear-black"] = 0.2,
        ["game:armor-head-hide-bear-brown"] = 0.3,
        ["game:armor-body-hide-bear-brown"] = 0.5,
        ["game:armor-legs-hide-bear-brown"] = 0.2,
        ["game:armor-head-hide-bear-polar"] = 0.3,
        ["game:armor-body-hide-bear-polar"] = 0.5,
        ["game:armor-legs-hide-bear-polar"] = 0.2,
        ["game:armor-head-hide-bear-sun"] = 0.3,
        ["game:armor-body-hide-bear-sun"] = 0.5,
        ["game:armor-legs-hide-bear-sun"] = 0.2,
    })
    { }
}

public static partial class Configuration
{
    public static Dictionary<string, double> expMultiplyHitLeatherArmor = [];
    private static int leatherArmorEXPPerReceiveHit = 10;
    private static float leatherArmorEXPMultiplyByDamage = 0.3f;
    private static int leatherArmorEXPIncreaseByAmountDamage = 2;
    private static int leatherArmorEXPPerLevelBase = 500;
    private static double leatherArmorEXPMultiplyPerLevel = 1.2;

    private static float leatherArmorRelativeProtectionMultiply = 1.0f;
    private static float leatherArmorRelativeProtectionMultiplyPerLevel = 0.015f;
    private static int leatherArmorRelativeProtectionMultiplyReductionEveryLevel = 1;
    private static float leatherArmorRelativeProtectionMultiplyReductionPerReduce = 0.25f;

    private static float leatherArmorFlatDamageReductionMultiply = 1.0f;
    private static float leatherArmorFlatDamageReductionMultiplyPerLevel = 0.015f;
    private static int leatherArmorFlatDamageReductionMultiplyReductionEveryLevel = 1;
    private static float leatherArmorFlatDamageReductionMultiplyReductionPerReduce = 0.05f;

    private static float leatherArmorHealingEffectivnessMultiply = 1.0f;
    private static float leatherArmorHealingEffectivnessMultiplyPerLevel = 0.035f;
    private static int leatherArmorHealingEffectivnessMultiplyReductionEveryLevel = 1;
    private static float leatherArmorHealingEffectivnessMultiplyReductionPerReduce = 0.05f;

    private static float leatherArmorHungerRateMultiply = 1.0f;
    private static float leatherArmorHungerRateMultiplyPerLevel = 0.02f;
    private static int leatherArmorHungerRateMultiplyReductionEveryLevel = 1;
    private static float leatherArmorHungerRateMultiplyReductionPerReduce = 0.05f;

    private static float leatherArmorRangedWeaponsAccuracyMultiply = 1.0f;
    private static float leatherArmorRangedWeaponsAccuracyMultiplyPerLevel = 0.075f;
    private static int leatherArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel = 1;
    private static float leatherArmorRangedWeaponsAccuracyMultiplyReductionPerReduce = 0.05f;

    private static float leatherArmorRangedWeaponsSpeedMultiply = 1.0f;
    private static float leatherArmorRangedWeaponsSpeedMultiplyPerLevel = 0.075f;
    private static int leatherArmorRangedWeaponsSpeedMultiplyReductionEveryLevel = 1;
    private static float leatherArmorRangedWeaponsSpeedMultiplyReductionPerReduce = 0.05f;

    private static float leatherArmorWalkSpeedMultiply = 1.0f;
    private static float leatherArmorWalkSpeedMultiplyPerLevel = 0.06f;
    private static int leatherArmorWalkSpeedMultiplyReductionEveryLevel = 1;
    private static float leatherArmorWalkSpeedMultiplyReductionPerReduce = 0.05f;

    public static int leatherArmorMaxLevel = 999;
    public static double leatherArmorSubLevelEXPMultiply = 3.0;

    public static void PopulateLeatherArmorConfiguration(ICoreAPI api)
    {
        LeatherArmorLevelStatsConfiguration leatherArmorLevelStats = ConfigManager.Load<LeatherArmorLevelStatsConfiguration>(
            api, "ModConfig/LevelUP/config/levelstats", "leatherarmor", Logger(api));

        leatherArmorEXPPerReceiveHit = leatherArmorLevelStats.leatherArmorEXPPerReceiveHit;
        Experience.LoadExperience("LeatherArmor", "Hit", (ulong)leatherArmorEXPPerReceiveHit);
        leatherArmorEXPMultiplyByDamage = leatherArmorLevelStats.leatherArmorEXPMultiplyByDamage;
        leatherArmorEXPIncreaseByAmountDamage = leatherArmorLevelStats.leatherArmorEXPIncreaseByAmountDamage;
        leatherArmorEXPPerLevelBase = leatherArmorLevelStats.leatherArmorEXPPerLevelBase;
        leatherArmorEXPMultiplyPerLevel = leatherArmorLevelStats.leatherArmorEXPMultiplyPerLevel;

        leatherArmorRelativeProtectionMultiply = leatherArmorLevelStats.leatherArmorRelativeProtectionMultiply;
        leatherArmorRelativeProtectionMultiplyPerLevel = leatherArmorLevelStats.leatherArmorRelativeProtectionMultiplyPerLevel;
        leatherArmorRelativeProtectionMultiplyReductionEveryLevel = leatherArmorLevelStats.leatherArmorRelativeProtectionMultiplyReductionEveryLevel;
        leatherArmorRelativeProtectionMultiplyReductionPerReduce = leatherArmorLevelStats.leatherArmorRelativeProtectionMultiplyReductionPerReduce;

        leatherArmorFlatDamageReductionMultiply = leatherArmorLevelStats.leatherArmorFlatDamageReductionMultiply;
        leatherArmorFlatDamageReductionMultiplyPerLevel = leatherArmorLevelStats.leatherArmorFlatDamageReductionMultiplyPerLevel;
        leatherArmorFlatDamageReductionMultiplyReductionEveryLevel = leatherArmorLevelStats.leatherArmorFlatDamageReductionMultiplyReductionEveryLevel;
        leatherArmorFlatDamageReductionMultiplyReductionPerReduce = leatherArmorLevelStats.leatherArmorFlatDamageReductionMultiplyReductionPerReduce;

        leatherArmorHealingEffectivnessMultiply = leatherArmorLevelStats.leatherArmorHealingEffectivnessMultiply;
        leatherArmorHealingEffectivnessMultiplyPerLevel = leatherArmorLevelStats.leatherArmorHealingEffectivnessMultiplyPerLevel;
        leatherArmorHealingEffectivnessMultiplyReductionEveryLevel = leatherArmorLevelStats.leatherArmorHealingEffectivnessMultiplyReductionEveryLevel;
        leatherArmorHealingEffectivnessMultiplyReductionPerReduce = leatherArmorLevelStats.leatherArmorHealingEffectivnessMultiplyReductionPerReduce;

        leatherArmorHungerRateMultiply = leatherArmorLevelStats.leatherArmorHungerRateMultiply;
        leatherArmorHungerRateMultiplyPerLevel = leatherArmorLevelStats.leatherArmorHungerRateMultiplyPerLevel;
        leatherArmorHungerRateMultiplyReductionEveryLevel = leatherArmorLevelStats.leatherArmorHungerRateMultiplyReductionEveryLevel;
        leatherArmorHungerRateMultiplyReductionPerReduce = leatherArmorLevelStats.leatherArmorHungerRateMultiplyReductionPerReduce;

        leatherArmorRangedWeaponsAccuracyMultiply = leatherArmorLevelStats.leatherArmorRangedWeaponsAccuracyMultiply;
        leatherArmorRangedWeaponsAccuracyMultiplyPerLevel = leatherArmorLevelStats.leatherArmorRangedWeaponsAccuracyMultiplyPerLevel;
        leatherArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel = leatherArmorLevelStats.leatherArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel;
        leatherArmorRangedWeaponsAccuracyMultiplyReductionPerReduce = leatherArmorLevelStats.leatherArmorRangedWeaponsAccuracyMultiplyReductionPerReduce;

        leatherArmorRangedWeaponsSpeedMultiply = leatherArmorLevelStats.leatherArmorRangedWeaponsSpeedMultiply;
        leatherArmorRangedWeaponsSpeedMultiplyPerLevel = leatherArmorLevelStats.leatherArmorRangedWeaponsSpeedMultiplyPerLevel;
        leatherArmorRangedWeaponsSpeedMultiplyReductionEveryLevel = leatherArmorLevelStats.leatherArmorRangedWeaponsSpeedMultiplyReductionEveryLevel;
        leatherArmorRangedWeaponsSpeedMultiplyReductionPerReduce = leatherArmorLevelStats.leatherArmorRangedWeaponsSpeedMultiplyReductionPerReduce;

        leatherArmorWalkSpeedMultiply = leatherArmorLevelStats.leatherArmorWalkSpeedMultiply;
        leatherArmorWalkSpeedMultiplyPerLevel = leatherArmorLevelStats.leatherArmorWalkSpeedMultiplyPerLevel;
        leatherArmorWalkSpeedMultiplyReductionEveryLevel = leatherArmorLevelStats.leatherArmorWalkSpeedMultiplyReductionEveryLevel;
        leatherArmorWalkSpeedMultiplyReductionPerReduce = leatherArmorLevelStats.leatherArmorWalkSpeedMultiplyReductionPerReduce;

        leatherArmorMaxLevel = leatherArmorLevelStats.leatherArmorMaxLevel;
        leatherArmorSubLevelEXPMultiply = leatherArmorLevelStats.leatherArmorSubLevelEXPMultiply;

        expMultiplyHitLeatherArmor = ConfigManager.Load<LeatherArmorItemsConfiguration>(
            api, "ModConfig/LevelUP/config/levelstats", "leatherarmoritems", Logger(api));

        Debug.Log("Leather Armor configuration set");
    }

    public static int LeatherArmorGetLevelByEXP(ulong exp)
    {
        double baseExp = leatherArmorEXPPerLevelBase;
        double multiplier = leatherArmorEXPMultiplyPerLevel;

        if (multiplier <= 1.0)
        {
            return (int)(exp / baseExp);
        }

        double expDouble = exp;

        double level = Math.Log((expDouble * (multiplier - 1) / baseExp) + 1) / Math.Log(multiplier);

        return Math.Max(0, (int)Math.Floor(level));
    }

    public static ulong LeatherArmorGetExpByLevel(int level)
    {
        double baseExp = leatherArmorEXPPerLevelBase;
        double multiplier = leatherArmorEXPMultiplyPerLevel;

        if (multiplier == 1.0)
        {
            return (ulong)(baseExp * level);
        }

        double exp = baseExp * (Math.Pow(multiplier, level) - 1) / (multiplier - 1);
        return (ulong)Math.Floor(exp);
    }

    public static int LeatherArmorBaseEXPEarnedByDAMAGE(float damage)
    {
        int calcDamage = (int)Math.Round(damage);
        int multiplesCount = calcDamage / leatherArmorEXPIncreaseByAmountDamage;
        float multiplier = 1 + leatherArmorEXPMultiplyByDamage;

        float baseMultiply = leatherArmorEXPPerReceiveHit * (float)Math.Pow(multiplier, multiplesCount);

        return (int)Math.Round(baseMultiply);
    }

    public static float LeatherArmorRelativeProtectionMultiplyByLevel(int level)
    {
        int reduceEvery = leatherArmorRelativeProtectionMultiplyReductionEveryLevel;
        float baseMultiply = leatherArmorRelativeProtectionMultiply;
        float baseIncrement = leatherArmorRelativeProtectionMultiplyPerLevel;
        float reductionPerStep = leatherArmorRelativeProtectionMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double multiply = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        multiply += baseMultiply;

        return (float)multiply;
    }

    public static float LeatherArmorFlatDamageReductionMultiplyByLevel(int level)
    {
        int reduceEvery = leatherArmorFlatDamageReductionMultiplyReductionEveryLevel;
        float baseMultiply = leatherArmorFlatDamageReductionMultiply;
        float baseIncrement = leatherArmorFlatDamageReductionMultiplyPerLevel;
        float reductionPerStep = leatherArmorFlatDamageReductionMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double multiply = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        multiply += baseMultiply;

        return (float)multiply;
    }

    public static float LeatherArmorHealingEffectivnessMultiplyByLevel(int level)
    {
        int reduceEvery = leatherArmorHealingEffectivnessMultiplyReductionEveryLevel;
        float baseMultiply = leatherArmorHealingEffectivnessMultiply;
        float baseIncrement = leatherArmorHealingEffectivnessMultiplyPerLevel;
        float reductionPerStep = leatherArmorHealingEffectivnessMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double multiply = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        multiply += baseMultiply;

        return (float)multiply;
    }

    public static float LeatherArmorHungerRateMultiplyByLevel(int level)
    {
        int reduceEvery = leatherArmorHungerRateMultiplyReductionEveryLevel;
        float baseMultiply = leatherArmorHungerRateMultiply;
        float baseIncrement = leatherArmorHungerRateMultiplyPerLevel;
        float reductionPerStep = leatherArmorHungerRateMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double multiply = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        multiply += baseMultiply;

        return (float)multiply;
    }

    public static float LeatherArmorRangedWeaponsAccuracyMultiplyByLevel(int level)
    {
        int reduceEvery = leatherArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel;
        float baseMultiply = leatherArmorRangedWeaponsAccuracyMultiply;
        float baseIncrement = leatherArmorRangedWeaponsAccuracyMultiplyPerLevel;
        float reductionPerStep = leatherArmorRangedWeaponsAccuracyMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double multiply = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        multiply += baseMultiply;

        return (float)multiply;
    }

    public static float LeatherArmorRangedWeaponsSpeedMultiplyByLevel(int level)
    {
        int reduceEvery = leatherArmorRangedWeaponsSpeedMultiplyReductionEveryLevel;
        float baseMultiply = leatherArmorRangedWeaponsSpeedMultiply;
        float baseIncrement = leatherArmorRangedWeaponsSpeedMultiplyPerLevel;
        float reductionPerStep = leatherArmorRangedWeaponsSpeedMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double multiply = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        multiply += baseMultiply;

        return (float)multiply;
    }

    public static float LeatherArmorWalkSpeedMultiplyByLevel(int level)
    {
        int reduceEvery = leatherArmorWalkSpeedMultiplyReductionEveryLevel;
        float baseMultiply = leatherArmorWalkSpeedMultiply;
        float baseIncrement = leatherArmorWalkSpeedMultiplyPerLevel;
        float reductionPerStep = leatherArmorWalkSpeedMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double multiply = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        multiply += baseMultiply;

        return (float)multiply;
    }
}
