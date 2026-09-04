using System;
using System.Collections.Generic;
using LevelUP.Server;
using OpenConfiguration;
using Vintagestory.API.Common;

namespace LevelUP;

public class BrigandineArmorLevelStatsConfiguration
{
    public int brigandineArmorEXPPerReceiveHit = 10;
    public float brigandineArmorEXPMultiplyByDamage = 0.3f;
    public int brigandineArmorEXPIncreaseByAmountDamage = 2;
    public int brigandineArmorEXPPerLevelBase = 500;
    public double brigandineArmorEXPMultiplyPerLevel = 1.2;

    public float brigandineArmorRelativeProtectionMultiply = 1.0f;
    public float brigandineArmorRelativeProtectionMultiplyPerLevel = 0.04f;
    public int brigandineArmorRelativeProtectionMultiplyReductionEveryLevel = 1;
    public float brigandineArmorRelativeProtectionMultiplyReductionPerReduce = 0.18f;

    public float brigandineArmorFlatDamageReductionMultiply = 1.0f;
    public float brigandineArmorFlatDamageReductionMultiplyPerLevel = 0.04f;
    public int brigandineArmorFlatDamageReductionMultiplyReductionEveryLevel = 1;
    public float brigandineArmorFlatDamageReductionMultiplyReductionPerReduce = 0.05f;

    public float brigandineArmorHealingEffectivnessMultiply = 1.0f;
    public float brigandineArmorHealingEffectivnessMultiplyPerLevel = 0.04f;
    public int brigandineArmorHealingEffectivnessMultiplyReductionEveryLevel = 1;
    public float brigandineArmorHealingEffectivnessMultiplyReductionPerReduce = 0.05f;

    public float brigandineArmorHungerRateMultiply = 1.0f;
    public float brigandineArmorHungerRateMultiplyPerLevel = 0.035f;
    public int brigandineArmorHungerRateMultiplyReductionEveryLevel = 1;
    public float brigandineArmorHungerRateMultiplyReductionPerReduce = 0.05f;

    public float brigandineArmorRangedWeaponsAccuracyMultiply = 1.0f;
    public float brigandineArmorRangedWeaponsAccuracyMultiplyPerLevel = 0.03f;
    public int brigandineArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel = 1;
    public float brigandineArmorRangedWeaponsAccuracyMultiplyReductionPerReduce = 0.05f;

    public float brigandineArmorRangedWeaponsSpeedMultiply = 1.0f;
    public float brigandineArmorRangedWeaponsSpeedMultiplyPerLevel = 0.03f;
    public int brigandineArmorRangedWeaponsSpeedMultiplyReductionEveryLevel = 1;
    public float brigandineArmorRangedWeaponsSpeedMultiplyReductionPerReduce = 0.05f;

    public float brigandineArmorWalkSpeedMultiply = 1.0f;
    public float brigandineArmorWalkSpeedMultiplyPerLevel = 0.025f;
    public int brigandineArmorWalkSpeedMultiplyReductionEveryLevel = 1;
    public float brigandineArmorWalkSpeedMultiplyReductionPerReduce = 0.05f;

    public int brigandineArmorMaxLevel = 999;
    public double brigandineArmorSubLevelEXPMultiply = 3.0;
}

public class BrigandineArmorItemsConfiguration : Dictionary<string, double>
{
    public BrigandineArmorItemsConfiguration() : base(new Dictionary<string, double>
    {
        ["game:armor-head-brigandine-copper"] = 0.3,
        ["game:armor-body-brigandine-copper"] = 0.5,
        ["game:armor-legs-brigandine-copper"] = 0.2,
        ["game:armor-head-brigandine-tinbronze"] = 0.3,
        ["game:armor-body-brigandine-tinbronze"] = 0.5,
        ["game:armor-legs-brigandine-tinbronze"] = 0.2,
        ["game:armor-head-brigandine-bismuthbronze"] = 0.3,
        ["game:armor-body-brigandine-bismuthbronze"] = 0.5,
        ["game:armor-legs-brigandine-bismuthbronze"] = 0.2,
        ["game:armor-head-brigandine-blackbronze"] = 0.3,
        ["game:armor-body-brigandine-blackbronze"] = 0.5,
        ["game:armor-legs-brigandine-blackbronze"] = 0.2,
        ["game:armor-head-brigandine-iron"] = 0.3,
        ["game:armor-body-brigandine-iron"] = 0.5,
        ["game:armor-legs-brigandine-iron"] = 0.2,
        ["game:armor-head-brigandine-meteoriciron"] = 0.3,
        ["game:armor-body-brigandine-meteoriciron"] = 0.5,
        ["game:armor-legs-brigandine-meteoriciron"] = 0.2,
        ["game:armor-head-brigandine-steel"] = 0.3,
        ["game:armor-body-brigandine-steel"] = 0.5,
        ["game:armor-legs-brigandine-steel"] = 0.2,
    })
    { }
}

public static partial class Configuration
{
    public static Dictionary<string, double> expMultiplyHitBrigandineArmor = [];
    private static int brigandineArmorEXPPerReceiveHit = 10;
    private static float brigandineArmorEXPMultiplyByDamage = 0.3f;
    private static int brigandineArmorEXPIncreaseByAmountDamage = 2;
    private static int brigandineArmorEXPPerLevelBase = 500;
    private static double brigandineArmorEXPMultiplyPerLevel = 1.2;

    private static float brigandineArmorRelativeProtectionMultiply = 1.0f;
    private static float brigandineArmorRelativeProtectionMultiplyPerLevel = 0.04f;
    private static int brigandineArmorRelativeProtectionMultiplyReductionEveryLevel = 1;
    private static float brigandineArmorRelativeProtectionMultiplyReductionPerReduce = 0.18f;

    private static float brigandineArmorFlatDamageReductionMultiply = 1.0f;
    private static float brigandineArmorFlatDamageReductionMultiplyPerLevel = 0.04f;
    private static int brigandineArmorFlatDamageReductionMultiplyReductionEveryLevel = 1;
    private static float brigandineArmorFlatDamageReductionMultiplyReductionPerReduce = 0.05f;

    private static float brigandineArmorHealingEffectivnessMultiply = 1.0f;
    private static float brigandineArmorHealingEffectivnessMultiplyPerLevel = 0.04f;
    private static int brigandineArmorHealingEffectivnessMultiplyReductionEveryLevel = 1;
    private static float brigandineArmorHealingEffectivnessMultiplyReductionPerReduce = 0.05f;

    private static float brigandineArmorHungerRateMultiply = 1.0f;
    private static float brigandineArmorHungerRateMultiplyPerLevel = 0.035f;
    private static int brigandineArmorHungerRateMultiplyReductionEveryLevel = 1;
    private static float brigandineArmorHungerRateMultiplyReductionPerReduce = 0.05f;

    private static float brigandineArmorRangedWeaponsAccuracyMultiply = 1.0f;
    private static float brigandineArmorRangedWeaponsAccuracyMultiplyPerLevel = 0.03f;
    private static int brigandineArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel = 1;
    private static float brigandineArmorRangedWeaponsAccuracyMultiplyReductionPerReduce = 0.05f;

    private static float brigandineArmorRangedWeaponsSpeedMultiply = 1.0f;
    private static float brigandineArmorRangedWeaponsSpeedMultiplyPerLevel = 0.03f;
    private static int brigandineArmorRangedWeaponsSpeedMultiplyReductionEveryLevel = 1;
    private static float brigandineArmorRangedWeaponsSpeedMultiplyReductionPerReduce = 0.05f;

    private static float brigandineArmorWalkSpeedMultiply = 1.0f;
    private static float brigandineArmorWalkSpeedMultiplyPerLevel = 0.025f;
    private static int brigandineArmorWalkSpeedMultiplyReductionEveryLevel = 1;
    private static float brigandineArmorWalkSpeedMultiplyReductionPerReduce = 0.05f;

    public static int brigandineArmorMaxLevel = 999;
    public static double brigandineArmorSubLevelEXPMultiply = 3.0;

    public static void PopulateBrigandineArmorConfiguration(ICoreAPI api)
    {
        BrigandineArmorLevelStatsConfiguration brigandineArmorLevelStats = ConfigManager.Load<BrigandineArmorLevelStatsConfiguration>(
            api, "ModConfig/LevelUP/config/levelstats", "brigandinearmor", Logger(api));

        brigandineArmorEXPPerReceiveHit = brigandineArmorLevelStats.brigandineArmorEXPPerReceiveHit;
        Experience.LoadExperience("BrigandineArmor", "Hit", (ulong)brigandineArmorEXPPerReceiveHit);
        brigandineArmorEXPMultiplyByDamage = brigandineArmorLevelStats.brigandineArmorEXPMultiplyByDamage;
        brigandineArmorEXPIncreaseByAmountDamage = brigandineArmorLevelStats.brigandineArmorEXPIncreaseByAmountDamage;
        brigandineArmorEXPPerLevelBase = brigandineArmorLevelStats.brigandineArmorEXPPerLevelBase;
        brigandineArmorEXPMultiplyPerLevel = brigandineArmorLevelStats.brigandineArmorEXPMultiplyPerLevel;

        brigandineArmorRelativeProtectionMultiply = brigandineArmorLevelStats.brigandineArmorRelativeProtectionMultiply;
        brigandineArmorRelativeProtectionMultiplyPerLevel = brigandineArmorLevelStats.brigandineArmorRelativeProtectionMultiplyPerLevel;
        brigandineArmorRelativeProtectionMultiplyReductionEveryLevel = brigandineArmorLevelStats.brigandineArmorRelativeProtectionMultiplyReductionEveryLevel;
        brigandineArmorRelativeProtectionMultiplyReductionPerReduce = brigandineArmorLevelStats.brigandineArmorRelativeProtectionMultiplyReductionPerReduce;

        brigandineArmorFlatDamageReductionMultiply = brigandineArmorLevelStats.brigandineArmorFlatDamageReductionMultiply;
        brigandineArmorFlatDamageReductionMultiplyPerLevel = brigandineArmorLevelStats.brigandineArmorFlatDamageReductionMultiplyPerLevel;
        brigandineArmorFlatDamageReductionMultiplyReductionEveryLevel = brigandineArmorLevelStats.brigandineArmorFlatDamageReductionMultiplyReductionEveryLevel;
        brigandineArmorFlatDamageReductionMultiplyReductionPerReduce = brigandineArmorLevelStats.brigandineArmorFlatDamageReductionMultiplyReductionPerReduce;

        brigandineArmorHealingEffectivnessMultiply = brigandineArmorLevelStats.brigandineArmorHealingEffectivnessMultiply;
        brigandineArmorHealingEffectivnessMultiplyPerLevel = brigandineArmorLevelStats.brigandineArmorHealingEffectivnessMultiplyPerLevel;
        brigandineArmorHealingEffectivnessMultiplyReductionEveryLevel = brigandineArmorLevelStats.brigandineArmorHealingEffectivnessMultiplyReductionEveryLevel;
        brigandineArmorHealingEffectivnessMultiplyReductionPerReduce = brigandineArmorLevelStats.brigandineArmorHealingEffectivnessMultiplyReductionPerReduce;

        brigandineArmorHungerRateMultiply = brigandineArmorLevelStats.brigandineArmorHungerRateMultiply;
        brigandineArmorHungerRateMultiplyPerLevel = brigandineArmorLevelStats.brigandineArmorHungerRateMultiplyPerLevel;
        brigandineArmorHungerRateMultiplyReductionEveryLevel = brigandineArmorLevelStats.brigandineArmorHungerRateMultiplyReductionEveryLevel;
        brigandineArmorHungerRateMultiplyReductionPerReduce = brigandineArmorLevelStats.brigandineArmorHungerRateMultiplyReductionPerReduce;

        brigandineArmorRangedWeaponsAccuracyMultiply = brigandineArmorLevelStats.brigandineArmorRangedWeaponsAccuracyMultiply;
        brigandineArmorRangedWeaponsAccuracyMultiplyPerLevel = brigandineArmorLevelStats.brigandineArmorRangedWeaponsAccuracyMultiplyPerLevel;
        brigandineArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel = brigandineArmorLevelStats.brigandineArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel;
        brigandineArmorRangedWeaponsAccuracyMultiplyReductionPerReduce = brigandineArmorLevelStats.brigandineArmorRangedWeaponsAccuracyMultiplyReductionPerReduce;

        brigandineArmorRangedWeaponsSpeedMultiply = brigandineArmorLevelStats.brigandineArmorRangedWeaponsSpeedMultiply;
        brigandineArmorRangedWeaponsSpeedMultiplyPerLevel = brigandineArmorLevelStats.brigandineArmorRangedWeaponsSpeedMultiplyPerLevel;
        brigandineArmorRangedWeaponsSpeedMultiplyReductionEveryLevel = brigandineArmorLevelStats.brigandineArmorRangedWeaponsSpeedMultiplyReductionEveryLevel;
        brigandineArmorRangedWeaponsSpeedMultiplyReductionPerReduce = brigandineArmorLevelStats.brigandineArmorRangedWeaponsSpeedMultiplyReductionPerReduce;

        brigandineArmorWalkSpeedMultiply = brigandineArmorLevelStats.brigandineArmorWalkSpeedMultiply;
        brigandineArmorWalkSpeedMultiplyPerLevel = brigandineArmorLevelStats.brigandineArmorWalkSpeedMultiplyPerLevel;
        brigandineArmorWalkSpeedMultiplyReductionEveryLevel = brigandineArmorLevelStats.brigandineArmorWalkSpeedMultiplyReductionEveryLevel;
        brigandineArmorWalkSpeedMultiplyReductionPerReduce = brigandineArmorLevelStats.brigandineArmorWalkSpeedMultiplyReductionPerReduce;

        brigandineArmorMaxLevel = brigandineArmorLevelStats.brigandineArmorMaxLevel;
        brigandineArmorSubLevelEXPMultiply = brigandineArmorLevelStats.brigandineArmorSubLevelEXPMultiply;

        expMultiplyHitBrigandineArmor = ConfigManager.Load<BrigandineArmorItemsConfiguration>(
            api, "ModConfig/LevelUP/config/levelstats", "brigandinearmoritems", Logger(api));

        Debug.Log("Brigandine Armor configuration set");
    }

    public static int BrigandineArmorGetLevelByEXP(ulong exp)
    {
        double baseExp = brigandineArmorEXPPerLevelBase;
        double multiplier = brigandineArmorEXPMultiplyPerLevel;

        if (multiplier <= 1.0)
        {
            return (int)(exp / baseExp);
        }

        double expDouble = exp;

        double level = Math.Log((expDouble * (multiplier - 1) / baseExp) + 1) / Math.Log(multiplier);

        return Math.Max(0, (int)Math.Floor(level));
    }

    public static ulong BrigandineArmorGetExpByLevel(int level)
    {
        double baseExp = brigandineArmorEXPPerLevelBase;
        double multiplier = brigandineArmorEXPMultiplyPerLevel;

        if (multiplier == 1.0)
        {
            return (ulong)(baseExp * level);
        }

        double exp = baseExp * (Math.Pow(multiplier, level) - 1) / (multiplier - 1);
        return (ulong)Math.Floor(exp);
    }

    public static int BrigandineArmorBaseEXPEarnedByDAMAGE(float damage)
    {
        int calcDamage = (int)Math.Round(damage);
        int multiplesCount = calcDamage / brigandineArmorEXPIncreaseByAmountDamage;
        float multiplier = 1 + brigandineArmorEXPMultiplyByDamage;

        float baseMultiply = brigandineArmorEXPPerReceiveHit * (float)Math.Pow(multiplier, multiplesCount);

        return (int)Math.Round(baseMultiply);
    }

    public static float BrigandineArmorRelativeProtectionMultiplyByLevel(int level)
    {
        int reduceEvery = brigandineArmorRelativeProtectionMultiplyReductionEveryLevel;
        float baseMultiply = brigandineArmorRelativeProtectionMultiply;
        float baseIncrement = brigandineArmorRelativeProtectionMultiplyPerLevel;
        float reductionPerStep = brigandineArmorRelativeProtectionMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double multiply = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        multiply += baseMultiply;

        return (float)multiply;
    }

    public static float BrigandineArmorFlatDamageReductionMultiplyByLevel(int level)
    {
        int reduceEvery = brigandineArmorFlatDamageReductionMultiplyReductionEveryLevel;
        float baseMultiply = brigandineArmorFlatDamageReductionMultiply;
        float baseIncrement = brigandineArmorFlatDamageReductionMultiplyPerLevel;
        float reductionPerStep = brigandineArmorFlatDamageReductionMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double multiply = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        multiply += baseMultiply;

        return (float)multiply;
    }

    public static float BrigandineArmorHealingEffectivnessMultiplyByLevel(int level)
    {
        int reduceEvery = brigandineArmorHealingEffectivnessMultiplyReductionEveryLevel;
        float baseMultiply = brigandineArmorHealingEffectivnessMultiply;
        float baseIncrement = brigandineArmorHealingEffectivnessMultiplyPerLevel;
        float reductionPerStep = brigandineArmorHealingEffectivnessMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double multiply = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        multiply += baseMultiply;

        return (float)multiply;
    }

    public static float BrigandineArmorHungerRateMultiplyByLevel(int level)
    {
        int reduceEvery = brigandineArmorHungerRateMultiplyReductionEveryLevel;
        float baseMultiply = brigandineArmorHungerRateMultiply;
        float baseIncrement = brigandineArmorHungerRateMultiplyPerLevel;
        float reductionPerStep = brigandineArmorHungerRateMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double multiply = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        multiply += baseMultiply;

        return (float)multiply;
    }

    public static float BrigandineArmorRangedWeaponsAccuracyMultiplyByLevel(int level)
    {
        int reduceEvery = brigandineArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel;
        float baseMultiply = brigandineArmorRangedWeaponsAccuracyMultiply;
        float baseIncrement = brigandineArmorRangedWeaponsAccuracyMultiplyPerLevel;
        float reductionPerStep = brigandineArmorRangedWeaponsAccuracyMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double multiply = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        multiply += baseMultiply;

        return (float)multiply;
    }

    public static float BrigandineArmorRangedWeaponsSpeedMultiplyByLevel(int level)
    {
        int reduceEvery = brigandineArmorRangedWeaponsSpeedMultiplyReductionEveryLevel;
        float baseMultiply = brigandineArmorRangedWeaponsSpeedMultiply;
        float baseIncrement = brigandineArmorRangedWeaponsSpeedMultiplyPerLevel;
        float reductionPerStep = brigandineArmorRangedWeaponsSpeedMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double multiply = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        multiply += baseMultiply;

        return (float)multiply;
    }

    public static float BrigandineArmorWalkSpeedMultiplyByLevel(int level)
    {
        int reduceEvery = brigandineArmorWalkSpeedMultiplyReductionEveryLevel;
        float baseMultiply = brigandineArmorWalkSpeedMultiply;
        float baseIncrement = brigandineArmorWalkSpeedMultiplyPerLevel;
        float reductionPerStep = brigandineArmorWalkSpeedMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double multiply = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        multiply += baseMultiply;

        return (float)multiply;
    }
}
