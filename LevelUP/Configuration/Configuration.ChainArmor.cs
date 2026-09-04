using System;
using System.Collections.Generic;
using LevelUP.Server;
using OpenConfiguration;
using Vintagestory.API.Common;

namespace LevelUP;

public class ChainArmorLevelStatsConfiguration
{
    public int chainArmorEXPPerReceiveHit = 10;
    public float chainArmorEXPMultiplyByDamage = 0.3f;
    public int chainArmorEXPIncreaseByAmountDamage = 2;
    public int chainArmorEXPPerLevelBase = 500;
    public double chainArmorEXPMultiplyPerLevel = 1.2;

    public float chainArmorRelativeProtectionMultiply = 1.0f;
    public float chainArmorRelativeProtectionMultiplyPerLevel = 0.025f;
    public int chainArmorRelativeProtectionMultiplyReductionEveryLevel = 1;
    public float chainArmorRelativeProtectionMultiplyReductionPerReduce = 0.20f;

    public float chainArmorFlatDamageReductionMultiply = 1.0f;
    public float chainArmorFlatDamageReductionMultiplyPerLevel = 0.025f;
    public int chainArmorFlatDamageReductionMultiplyReductionEveryLevel = 1;
    public float chainArmorFlatDamageReductionMultiplyReductionPerReduce = 0.05f;

    public float chainArmorHealingEffectivnessMultiply = 1.0f;
    public float chainArmorHealingEffectivnessMultiplyPerLevel = 0.04f;
    public int chainArmorHealingEffectivnessMultiplyReductionEveryLevel = 1;
    public float chainArmorHealingEffectivnessMultiplyReductionPerReduce = 0.05f;

    public float chainArmorHungerRateMultiply = 1.0f;
    public float chainArmorHungerRateMultiplyPerLevel = 0.03f;
    public int chainArmorHungerRateMultiplyReductionEveryLevel = 1;
    public float chainArmorHungerRateMultiplyReductionPerReduce = 0.05f;

    public float chainArmorRangedWeaponsAccuracyMultiply = 1.0f;
    public float chainArmorRangedWeaponsAccuracyMultiplyPerLevel = 0.05f;
    public int chainArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel = 1;
    public float chainArmorRangedWeaponsAccuracyMultiplyReductionPerReduce = 0.05f;

    public float chainArmorRangedWeaponsSpeedMultiply = 1.0f;
    public float chainArmorRangedWeaponsSpeedMultiplyPerLevel = 0.05f;
    public int chainArmorRangedWeaponsSpeedMultiplyReductionEveryLevel = 1;
    public float chainArmorRangedWeaponsSpeedMultiplyReductionPerReduce = 0.05f;

    public float chainArmorWalkSpeedMultiply = 1.0f;
    public float chainArmorWalkSpeedMultiplyPerLevel = 0.04f;
    public int chainArmorWalkSpeedMultiplyReductionEveryLevel = 1;
    public float chainArmorWalkSpeedMultiplyReductionPerReduce = 0.05f;

    public int chainArmorMaxLevel = 999;
    public double chainArmorSubLevelEXPMultiply = 3.0;
}

public class ChainArmorItemsConfiguration : Dictionary<string, double>
{
    public ChainArmorItemsConfiguration() : base(new Dictionary<string, double>
    {
        ["game:armor-head-chain-copper"] = 0.3,
        ["game:armor-body-chain-copper"] = 0.5,
        ["game:armor-legs-chain-copper"] = 0.2,
        ["game:armor-head-chain-tinbronze"] = 0.3,
        ["game:armor-body-chain-tinbronze"] = 0.5,
        ["game:armor-legs-chain-tinbronze"] = 0.2,
        ["game:armor-head-chain-bismuthbronze"] = 0.3,
        ["game:armor-body-chain-bismuthbronze"] = 0.5,
        ["game:armor-legs-chain-bismuthbronze"] = 0.2,
        ["game:armor-head-chain-blackbronze"] = 0.3,
        ["game:armor-body-chain-blackbronze"] = 0.5,
        ["game:armor-legs-chain-blackbronze"] = 0.2,
        ["game:armor-head-chain-iron"] = 0.3,
        ["game:armor-body-chain-iron"] = 0.5,
        ["game:armor-legs-chain-iron"] = 0.2,
        ["game:armor-head-chain-meteoriciron"] = 0.3,
        ["game:armor-body-chain-meteoriciron"] = 0.5,
        ["game:armor-legs-chain-meteoriciron"] = 0.2,
        ["game:armor-head-chain-steel"] = 0.3,
        ["game:armor-body-chain-steel"] = 0.5,
        ["game:armor-legs-chain-steel"] = 0.2,
        ["game:armor-head-chain-gold"] = 0.3,
        ["game:armor-body-chain-gold"] = 0.5,
        ["game:armor-legs-chain-gold"] = 0.2,
        ["game:armor-head-chain-silver"] = 0.3,
        ["game:armor-body-chain-silver"] = 0.5,
        ["game:armor-legs-chain-silver"] = 0.2,
    })
    { }
}

public static partial class Configuration
{
    public static Dictionary<string, double> expMultiplyHitChainArmor = [];
    private static int chainArmorEXPPerReceiveHit = 10;
    private static float chainArmorEXPMultiplyByDamage = 0.3f;
    private static int chainArmorEXPIncreaseByAmountDamage = 2;
    private static int chainArmorEXPPerLevelBase = 500;
    private static double chainArmorEXPMultiplyPerLevel = 1.2;

    private static float chainArmorRelativeProtectionMultiply = 1.0f;
    private static float chainArmorRelativeProtectionMultiplyPerLevel = 0.025f;
    private static int chainArmorRelativeProtectionMultiplyReductionEveryLevel = 1;
    private static float chainArmorRelativeProtectionMultiplyReductionPerReduce = 0.20f;

    private static float chainArmorFlatDamageReductionMultiply = 1.0f;
    private static float chainArmorFlatDamageReductionMultiplyPerLevel = 0.025f;
    private static int chainArmorFlatDamageReductionMultiplyReductionEveryLevel = 1;
    private static float chainArmorFlatDamageReductionMultiplyReductionPerReduce = 0.05f;

    private static float chainArmorHealingEffectivnessMultiply = 1.0f;
    private static float chainArmorHealingEffectivnessMultiplyPerLevel = 0.04f;
    private static int chainArmorHealingEffectivnessMultiplyReductionEveryLevel = 1;
    private static float chainArmorHealingEffectivnessMultiplyReductionPerReduce = 0.05f;

    private static float chainArmorHungerRateMultiply = 1.0f;
    private static float chainArmorHungerRateMultiplyPerLevel = 0.03f;
    private static int chainArmorHungerRateMultiplyReductionEveryLevel = 1;
    private static float chainArmorHungerRateMultiplyReductionPerReduce = 0.05f;

    private static float chainArmorRangedWeaponsAccuracyMultiply = 1.0f;
    private static float chainArmorRangedWeaponsAccuracyMultiplyPerLevel = 0.05f;
    private static int chainArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel = 1;
    private static float chainArmorRangedWeaponsAccuracyMultiplyReductionPerReduce = 0.05f;

    private static float chainArmorRangedWeaponsSpeedMultiply = 1.0f;
    private static float chainArmorRangedWeaponsSpeedMultiplyPerLevel = 0.05f;
    private static int chainArmorRangedWeaponsSpeedMultiplyReductionEveryLevel = 1;
    private static float chainArmorRangedWeaponsSpeedMultiplyReductionPerReduce = 0.05f;

    private static float chainArmorWalkSpeedMultiply = 1.0f;
    private static float chainArmorWalkSpeedMultiplyPerLevel = 0.04f;
    private static int chainArmorWalkSpeedMultiplyReductionEveryLevel = 1;
    private static float chainArmorWalkSpeedMultiplyReductionPerReduce = 0.05f;

    public static int chainArmorMaxLevel = 999;
    public static double chainArmorSubLevelEXPMultiply = 3.0;

    public static void PopulateChainArmorConfiguration(ICoreAPI api)
    {
        ChainArmorLevelStatsConfiguration chainArmorLevelStats = ConfigManager.Load<ChainArmorLevelStatsConfiguration>(
            api, "ModConfig/LevelUP/config/levelstats", "chainarmor", Logger(api));

        chainArmorEXPPerReceiveHit = chainArmorLevelStats.chainArmorEXPPerReceiveHit;
        Experience.LoadExperience("ChainArmor", "Hit", (ulong)chainArmorEXPPerReceiveHit);
        chainArmorEXPMultiplyByDamage = chainArmorLevelStats.chainArmorEXPMultiplyByDamage;
        chainArmorEXPIncreaseByAmountDamage = chainArmorLevelStats.chainArmorEXPIncreaseByAmountDamage;
        chainArmorEXPPerLevelBase = chainArmorLevelStats.chainArmorEXPPerLevelBase;
        chainArmorEXPMultiplyPerLevel = chainArmorLevelStats.chainArmorEXPMultiplyPerLevel;

        chainArmorRelativeProtectionMultiply = chainArmorLevelStats.chainArmorRelativeProtectionMultiply;
        chainArmorRelativeProtectionMultiplyPerLevel = chainArmorLevelStats.chainArmorRelativeProtectionMultiplyPerLevel;
        chainArmorRelativeProtectionMultiplyReductionEveryLevel = chainArmorLevelStats.chainArmorRelativeProtectionMultiplyReductionEveryLevel;
        chainArmorRelativeProtectionMultiplyReductionPerReduce = chainArmorLevelStats.chainArmorRelativeProtectionMultiplyReductionPerReduce;

        chainArmorFlatDamageReductionMultiply = chainArmorLevelStats.chainArmorFlatDamageReductionMultiply;
        chainArmorFlatDamageReductionMultiplyPerLevel = chainArmorLevelStats.chainArmorFlatDamageReductionMultiplyPerLevel;
        chainArmorFlatDamageReductionMultiplyReductionEveryLevel = chainArmorLevelStats.chainArmorFlatDamageReductionMultiplyReductionEveryLevel;
        chainArmorFlatDamageReductionMultiplyReductionPerReduce = chainArmorLevelStats.chainArmorFlatDamageReductionMultiplyReductionPerReduce;

        chainArmorHealingEffectivnessMultiply = chainArmorLevelStats.chainArmorHealingEffectivnessMultiply;
        chainArmorHealingEffectivnessMultiplyPerLevel = chainArmorLevelStats.chainArmorHealingEffectivnessMultiplyPerLevel;
        chainArmorHealingEffectivnessMultiplyReductionEveryLevel = chainArmorLevelStats.chainArmorHealingEffectivnessMultiplyReductionEveryLevel;
        chainArmorHealingEffectivnessMultiplyReductionPerReduce = chainArmorLevelStats.chainArmorHealingEffectivnessMultiplyReductionPerReduce;

        chainArmorHungerRateMultiply = chainArmorLevelStats.chainArmorHungerRateMultiply;
        chainArmorHungerRateMultiplyPerLevel = chainArmorLevelStats.chainArmorHungerRateMultiplyPerLevel;
        chainArmorHungerRateMultiplyReductionEveryLevel = chainArmorLevelStats.chainArmorHungerRateMultiplyReductionEveryLevel;
        chainArmorHungerRateMultiplyReductionPerReduce = chainArmorLevelStats.chainArmorHungerRateMultiplyReductionPerReduce;

        chainArmorRangedWeaponsAccuracyMultiply = chainArmorLevelStats.chainArmorRangedWeaponsAccuracyMultiply;
        chainArmorRangedWeaponsAccuracyMultiplyPerLevel = chainArmorLevelStats.chainArmorRangedWeaponsAccuracyMultiplyPerLevel;
        chainArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel = chainArmorLevelStats.chainArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel;
        chainArmorRangedWeaponsAccuracyMultiplyReductionPerReduce = chainArmorLevelStats.chainArmorRangedWeaponsAccuracyMultiplyReductionPerReduce;

        chainArmorRangedWeaponsSpeedMultiply = chainArmorLevelStats.chainArmorRangedWeaponsSpeedMultiply;
        chainArmorRangedWeaponsSpeedMultiplyPerLevel = chainArmorLevelStats.chainArmorRangedWeaponsSpeedMultiplyPerLevel;
        chainArmorRangedWeaponsSpeedMultiplyReductionEveryLevel = chainArmorLevelStats.chainArmorRangedWeaponsSpeedMultiplyReductionEveryLevel;
        chainArmorRangedWeaponsSpeedMultiplyReductionPerReduce = chainArmorLevelStats.chainArmorRangedWeaponsSpeedMultiplyReductionPerReduce;

        chainArmorWalkSpeedMultiply = chainArmorLevelStats.chainArmorWalkSpeedMultiply;
        chainArmorWalkSpeedMultiplyPerLevel = chainArmorLevelStats.chainArmorWalkSpeedMultiplyPerLevel;
        chainArmorWalkSpeedMultiplyReductionEveryLevel = chainArmorLevelStats.chainArmorWalkSpeedMultiplyReductionEveryLevel;
        chainArmorWalkSpeedMultiplyReductionPerReduce = chainArmorLevelStats.chainArmorWalkSpeedMultiplyReductionPerReduce;

        chainArmorMaxLevel = chainArmorLevelStats.chainArmorMaxLevel;
        chainArmorSubLevelEXPMultiply = chainArmorLevelStats.chainArmorSubLevelEXPMultiply;

        expMultiplyHitChainArmor = ConfigManager.Load<ChainArmorItemsConfiguration>(
            api, "ModConfig/LevelUP/config/levelstats", "chainarmoritems", Logger(api));

        Debug.Log("Chain Armor configuration set");
    }

    public static int ChainArmorGetLevelByEXP(ulong exp)
    {
        double baseExp = chainArmorEXPPerLevelBase;
        double multiplier = chainArmorEXPMultiplyPerLevel;

        if (multiplier <= 1.0)
        {
            return (int)(exp / baseExp);
        }

        double expDouble = exp;

        double level = Math.Log((expDouble * (multiplier - 1) / baseExp) + 1) / Math.Log(multiplier);

        return Math.Max(0, (int)Math.Floor(level));
    }

    public static ulong ChainArmorGetExpByLevel(int level)
    {
        double baseExp = chainArmorEXPPerLevelBase;
        double multiplier = chainArmorEXPMultiplyPerLevel;

        if (multiplier == 1.0)
        {
            return (ulong)(baseExp * level);
        }

        double exp = baseExp * (Math.Pow(multiplier, level) - 1) / (multiplier - 1);
        return (ulong)Math.Floor(exp);
    }

    public static int ChainArmorBaseEXPEarnedByDAMAGE(float damage)
    {
        int calcDamage = (int)Math.Round(damage);
        int multiplesCount = calcDamage / chainArmorEXPIncreaseByAmountDamage;
        float multiplier = 1 + chainArmorEXPMultiplyByDamage;

        float baseMultiply = chainArmorEXPPerReceiveHit * (float)Math.Pow(multiplier, multiplesCount);

        return (int)Math.Round(baseMultiply);
    }

    public static float ChainArmorRelativeProtectionMultiplyByLevel(int level)
    {
        int reduceEvery = chainArmorRelativeProtectionMultiplyReductionEveryLevel;
        float baseMultiply = chainArmorRelativeProtectionMultiply;
        float baseIncrement = chainArmorRelativeProtectionMultiplyPerLevel;
        float reductionPerStep = chainArmorRelativeProtectionMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double multiply = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        multiply += baseMultiply;

        return (float)multiply;
    }

    public static float ChainArmorFlatDamageReductionMultiplyByLevel(int level)
    {
        int reduceEvery = chainArmorFlatDamageReductionMultiplyReductionEveryLevel;
        float baseMultiply = chainArmorFlatDamageReductionMultiply;
        float baseIncrement = chainArmorFlatDamageReductionMultiplyPerLevel;
        float reductionPerStep = chainArmorFlatDamageReductionMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double multiply = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        multiply += baseMultiply;

        return (float)multiply;
    }

    public static float ChainArmorHealingEffectivnessMultiplyByLevel(int level)
    {
        int reduceEvery = chainArmorHealingEffectivnessMultiplyReductionEveryLevel;
        float baseMultiply = chainArmorHealingEffectivnessMultiply;
        float baseIncrement = chainArmorHealingEffectivnessMultiplyPerLevel;
        float reductionPerStep = chainArmorHealingEffectivnessMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double multiply = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        multiply += baseMultiply;

        return (float)multiply;
    }

    public static float ChainArmorHungerRateMultiplyByLevel(int level)
    {
        int reduceEvery = chainArmorHungerRateMultiplyReductionEveryLevel;
        float baseMultiply = chainArmorHungerRateMultiply;
        float baseIncrement = chainArmorHungerRateMultiplyPerLevel;
        float reductionPerStep = chainArmorHungerRateMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double multiply = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        multiply += baseMultiply;

        return (float)multiply;
    }

    public static float ChainArmorRangedWeaponsAccuracyMultiplyByLevel(int level)
    {
        int reduceEvery = chainArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel;
        float baseMultiply = chainArmorRangedWeaponsAccuracyMultiply;
        float baseIncrement = chainArmorRangedWeaponsAccuracyMultiplyPerLevel;
        float reductionPerStep = chainArmorRangedWeaponsAccuracyMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double multiply = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        multiply += baseMultiply;

        return (float)multiply;
    }

    public static float ChainArmorRangedWeaponsSpeedMultiplyByLevel(int level)
    {
        int reduceEvery = chainArmorRangedWeaponsSpeedMultiplyReductionEveryLevel;
        float baseMultiply = chainArmorRangedWeaponsSpeedMultiply;
        float baseIncrement = chainArmorRangedWeaponsSpeedMultiplyPerLevel;
        float reductionPerStep = chainArmorRangedWeaponsSpeedMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double multiply = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        multiply += baseMultiply;

        return (float)multiply;
    }

    public static float ChainArmorWalkSpeedMultiplyByLevel(int level)
    {
        int reduceEvery = chainArmorWalkSpeedMultiplyReductionEveryLevel;
        float baseMultiply = chainArmorWalkSpeedMultiply;
        float baseIncrement = chainArmorWalkSpeedMultiplyPerLevel;
        float reductionPerStep = chainArmorWalkSpeedMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double multiply = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        multiply += baseMultiply;

        return (float)multiply;
    }
}
