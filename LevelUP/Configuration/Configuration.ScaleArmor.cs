using System;
using System.Collections.Generic;
using LevelUP.Server;
using OpenConfiguration;
using Vintagestory.API.Common;

namespace LevelUP;

public class ScaleArmorLevelStatsConfiguration
{
    public int scaleArmorEXPPerReceiveHit = 10;
    public float scaleArmorEXPMultiplyByDamage = 0.3f;
    public int scaleArmorEXPIncreaseByAmountDamage = 2;
    public int scaleArmorEXPPerLevelBase = 500;
    public double scaleArmorEXPMultiplyPerLevel = 1.2;

    public float scaleArmorRelativeProtectionMultiply = 1.0f;
    public float scaleArmorRelativeProtectionMultiplyPerLevel = 0.05f;
    public int scaleArmorRelativeProtectionMultiplyReductionEveryLevel = 1;
    public float scaleArmorRelativeProtectionMultiplyReductionPerReduce = 0.16f;

    public float scaleArmorFlatDamageReductionMultiply = 1.0f;
    public float scaleArmorFlatDamageReductionMultiplyPerLevel = 0.05f;
    public int scaleArmorFlatDamageReductionMultiplyReductionEveryLevel = 1;
    public float scaleArmorFlatDamageReductionMultiplyReductionPerReduce = 0.05f;

    public float scaleArmorHealingEffectivnessMultiply = 1.0f;
    public float scaleArmorHealingEffectivnessMultiplyPerLevel = 0.045f;
    public int scaleArmorHealingEffectivnessMultiplyReductionEveryLevel = 1;
    public float scaleArmorHealingEffectivnessMultiplyReductionPerReduce = 0.05f;

    public float scaleArmorHungerRateMultiply = 1.0f;
    public float scaleArmorHungerRateMultiplyPerLevel = 0.045f;
    public int scaleArmorHungerRateMultiplyReductionEveryLevel = 1;
    public float scaleArmorHungerRateMultiplyReductionPerReduce = 0.05f;

    public float scaleArmorRangedWeaponsAccuracyMultiply = 1.0f;
    public float scaleArmorRangedWeaponsAccuracyMultiplyPerLevel = 0.02f;
    public int scaleArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel = 1;
    public float scaleArmorRangedWeaponsAccuracyMultiplyReductionPerReduce = 0.05f;

    public float scaleArmorRangedWeaponsSpeedMultiply = 1.0f;
    public float scaleArmorRangedWeaponsSpeedMultiplyPerLevel = 0.02f;
    public int scaleArmorRangedWeaponsSpeedMultiplyReductionEveryLevel = 1;
    public float scaleArmorRangedWeaponsSpeedMultiplyReductionPerReduce = 0.05f;

    public float scaleArmorWalkSpeedMultiply = 1.0f;
    public float scaleArmorWalkSpeedMultiplyPerLevel = 0.015f;
    public int scaleArmorWalkSpeedMultiplyReductionEveryLevel = 1;
    public float scaleArmorWalkSpeedMultiplyReductionPerReduce = 0.05f;

    public int scaleArmorMaxLevel = 999;
    public double scaleArmorSubLevelEXPMultiply = 3.0;
}

public class ScaleArmorItemsConfiguration : Dictionary<string, double>
{
    public ScaleArmorItemsConfiguration() : base(new Dictionary<string, double>
    {
        ["game:armor-head-scale-copper"] = 0.3,
        ["game:armor-body-scale-copper"] = 0.5,
        ["game:armor-legs-scale-copper"] = 0.2,
        ["game:armor-head-scale-tinbronze"] = 0.3,
        ["game:armor-body-scale-tinbronze"] = 0.5,
        ["game:armor-legs-scale-tinbronze"] = 0.2,
        ["game:armor-head-scale-bismuthbronze"] = 0.3,
        ["game:armor-body-scale-bismuthbronze"] = 0.5,
        ["game:armor-legs-scale-bismuthbronze"] = 0.2,
        ["game:armor-head-scale-blackbronze"] = 0.3,
        ["game:armor-body-scale-blackbronze"] = 0.5,
        ["game:armor-legs-scale-blackbronze"] = 0.2,
        ["game:armor-head-scale-iron"] = 0.3,
        ["game:armor-body-scale-iron"] = 0.5,
        ["game:armor-legs-scale-iron"] = 0.2,
        ["game:armor-head-scale-meteoriciron"] = 0.3,
        ["game:armor-body-scale-meteoriciron"] = 0.5,
        ["game:armor-legs-scale-meteoriciron"] = 0.2,
        ["game:armor-head-scale-steel"] = 0.3,
        ["game:armor-body-scale-steel"] = 0.5,
        ["game:armor-legs-scale-steel"] = 0.2,
    })
    { }
}

public static partial class Configuration
{
    public static Dictionary<string, double> expMultiplyHitScaleArmor = [];
    private static int scaleArmorEXPPerReceiveHit = 10;
    private static float scaleArmorEXPMultiplyByDamage = 0.3f;
    private static int scaleArmorEXPIncreaseByAmountDamage = 2;
    private static int scaleArmorEXPPerLevelBase = 500;
    private static double scaleArmorEXPMultiplyPerLevel = 1.2;

    private static float scaleArmorRelativeProtectionMultiply = 1.0f;
    private static float scaleArmorRelativeProtectionMultiplyPerLevel = 0.05f;
    private static int scaleArmorRelativeProtectionMultiplyReductionEveryLevel = 1;
    private static float scaleArmorRelativeProtectionMultiplyReductionPerReduce = 0.16f;

    private static float scaleArmorFlatDamageReductionMultiply = 1.0f;
    private static float scaleArmorFlatDamageReductionMultiplyPerLevel = 0.05f;
    private static int scaleArmorFlatDamageReductionMultiplyReductionEveryLevel = 1;
    private static float scaleArmorFlatDamageReductionMultiplyReductionPerReduce = 0.05f;

    private static float scaleArmorHealingEffectivnessMultiply = 1.0f;
    private static float scaleArmorHealingEffectivnessMultiplyPerLevel = 0.045f;
    private static int scaleArmorHealingEffectivnessMultiplyReductionEveryLevel = 1;
    private static float scaleArmorHealingEffectivnessMultiplyReductionPerReduce = 0.05f;

    private static float scaleArmorHungerRateMultiply = 1.0f;
    private static float scaleArmorHungerRateMultiplyPerLevel = 0.045f;
    private static int scaleArmorHungerRateMultiplyReductionEveryLevel = 1;
    private static float scaleArmorHungerRateMultiplyReductionPerReduce = 0.05f;

    private static float scaleArmorRangedWeaponsAccuracyMultiply = 1.0f;
    private static float scaleArmorRangedWeaponsAccuracyMultiplyPerLevel = 0.02f;
    private static int scaleArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel = 1;
    private static float scaleArmorRangedWeaponsAccuracyMultiplyReductionPerReduce = 0.05f;

    private static float scaleArmorRangedWeaponsSpeedMultiply = 1.0f;
    private static float scaleArmorRangedWeaponsSpeedMultiplyPerLevel = 0.02f;
    private static int scaleArmorRangedWeaponsSpeedMultiplyReductionEveryLevel = 1;
    private static float scaleArmorRangedWeaponsSpeedMultiplyReductionPerReduce = 0.05f;

    private static float scaleArmorWalkSpeedMultiply = 1.0f;
    private static float scaleArmorWalkSpeedMultiplyPerLevel = 0.015f;
    private static int scaleArmorWalkSpeedMultiplyReductionEveryLevel = 1;
    private static float scaleArmorWalkSpeedMultiplyReductionPerReduce = 0.05f;

    public static int scaleArmorMaxLevel = 999;
    public static double scaleArmorSubLevelEXPMultiply = 3.0;

    public static void PopulateScaleArmorConfiguration(ICoreAPI api)
    {
        ScaleArmorLevelStatsConfiguration scaleArmorLevelStats = ConfigManager.Load<ScaleArmorLevelStatsConfiguration>(
            api, "ModConfig/LevelUP/config/levelstats", "scalearmor", Logger(api));

        scaleArmorEXPPerReceiveHit = scaleArmorLevelStats.scaleArmorEXPPerReceiveHit;
        Experience.LoadExperience("ScaleArmor", "Hit", (ulong)scaleArmorEXPPerReceiveHit);
        scaleArmorEXPMultiplyByDamage = scaleArmorLevelStats.scaleArmorEXPMultiplyByDamage;
        scaleArmorEXPIncreaseByAmountDamage = scaleArmorLevelStats.scaleArmorEXPIncreaseByAmountDamage;
        scaleArmorEXPPerLevelBase = scaleArmorLevelStats.scaleArmorEXPPerLevelBase;
        scaleArmorEXPMultiplyPerLevel = scaleArmorLevelStats.scaleArmorEXPMultiplyPerLevel;

        scaleArmorRelativeProtectionMultiply = scaleArmorLevelStats.scaleArmorRelativeProtectionMultiply;
        scaleArmorRelativeProtectionMultiplyPerLevel = scaleArmorLevelStats.scaleArmorRelativeProtectionMultiplyPerLevel;
        scaleArmorRelativeProtectionMultiplyReductionEveryLevel = scaleArmorLevelStats.scaleArmorRelativeProtectionMultiplyReductionEveryLevel;
        scaleArmorRelativeProtectionMultiplyReductionPerReduce = scaleArmorLevelStats.scaleArmorRelativeProtectionMultiplyReductionPerReduce;

        scaleArmorFlatDamageReductionMultiply = scaleArmorLevelStats.scaleArmorFlatDamageReductionMultiply;
        scaleArmorFlatDamageReductionMultiplyPerLevel = scaleArmorLevelStats.scaleArmorFlatDamageReductionMultiplyPerLevel;
        scaleArmorFlatDamageReductionMultiplyReductionEveryLevel = scaleArmorLevelStats.scaleArmorFlatDamageReductionMultiplyReductionEveryLevel;
        scaleArmorFlatDamageReductionMultiplyReductionPerReduce = scaleArmorLevelStats.scaleArmorFlatDamageReductionMultiplyReductionPerReduce;

        scaleArmorHealingEffectivnessMultiply = scaleArmorLevelStats.scaleArmorHealingEffectivnessMultiply;
        scaleArmorHealingEffectivnessMultiplyPerLevel = scaleArmorLevelStats.scaleArmorHealingEffectivnessMultiplyPerLevel;
        scaleArmorHealingEffectivnessMultiplyReductionEveryLevel = scaleArmorLevelStats.scaleArmorHealingEffectivnessMultiplyReductionEveryLevel;
        scaleArmorHealingEffectivnessMultiplyReductionPerReduce = scaleArmorLevelStats.scaleArmorHealingEffectivnessMultiplyReductionPerReduce;

        scaleArmorHungerRateMultiply = scaleArmorLevelStats.scaleArmorHungerRateMultiply;
        scaleArmorHungerRateMultiplyPerLevel = scaleArmorLevelStats.scaleArmorHungerRateMultiplyPerLevel;
        scaleArmorHungerRateMultiplyReductionEveryLevel = scaleArmorLevelStats.scaleArmorHungerRateMultiplyReductionEveryLevel;
        scaleArmorHungerRateMultiplyReductionPerReduce = scaleArmorLevelStats.scaleArmorHungerRateMultiplyReductionPerReduce;

        scaleArmorRangedWeaponsAccuracyMultiply = scaleArmorLevelStats.scaleArmorRangedWeaponsAccuracyMultiply;
        scaleArmorRangedWeaponsAccuracyMultiplyPerLevel = scaleArmorLevelStats.scaleArmorRangedWeaponsAccuracyMultiplyPerLevel;
        scaleArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel = scaleArmorLevelStats.scaleArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel;
        scaleArmorRangedWeaponsAccuracyMultiplyReductionPerReduce = scaleArmorLevelStats.scaleArmorRangedWeaponsAccuracyMultiplyReductionPerReduce;

        scaleArmorRangedWeaponsSpeedMultiply = scaleArmorLevelStats.scaleArmorRangedWeaponsSpeedMultiply;
        scaleArmorRangedWeaponsSpeedMultiplyPerLevel = scaleArmorLevelStats.scaleArmorRangedWeaponsSpeedMultiplyPerLevel;
        scaleArmorRangedWeaponsSpeedMultiplyReductionEveryLevel = scaleArmorLevelStats.scaleArmorRangedWeaponsSpeedMultiplyReductionEveryLevel;
        scaleArmorRangedWeaponsSpeedMultiplyReductionPerReduce = scaleArmorLevelStats.scaleArmorRangedWeaponsSpeedMultiplyReductionPerReduce;

        scaleArmorWalkSpeedMultiply = scaleArmorLevelStats.scaleArmorWalkSpeedMultiply;
        scaleArmorWalkSpeedMultiplyPerLevel = scaleArmorLevelStats.scaleArmorWalkSpeedMultiplyPerLevel;
        scaleArmorWalkSpeedMultiplyReductionEveryLevel = scaleArmorLevelStats.scaleArmorWalkSpeedMultiplyReductionEveryLevel;
        scaleArmorWalkSpeedMultiplyReductionPerReduce = scaleArmorLevelStats.scaleArmorWalkSpeedMultiplyReductionPerReduce;

        scaleArmorMaxLevel = scaleArmorLevelStats.scaleArmorMaxLevel;
        scaleArmorSubLevelEXPMultiply = scaleArmorLevelStats.scaleArmorSubLevelEXPMultiply;

        expMultiplyHitScaleArmor = ConfigManager.Load<ScaleArmorItemsConfiguration>(
            api, "ModConfig/LevelUP/config/levelstats", "scalearmoritems", Logger(api));

        Debug.Log("Scale Armor configuration set");
    }

    public static int ScaleArmorGetLevelByEXP(ulong exp)
    {
        double baseExp = scaleArmorEXPPerLevelBase;
        double multiplier = scaleArmorEXPMultiplyPerLevel;

        if (multiplier <= 1.0)
        {
            return (int)(exp / baseExp);
        }

        double expDouble = exp;

        double level = Math.Log((expDouble * (multiplier - 1) / baseExp) + 1) / Math.Log(multiplier);

        return Math.Max(0, (int)Math.Floor(level));
    }

    public static ulong ScaleArmorGetExpByLevel(int level)
    {
        double baseExp = scaleArmorEXPPerLevelBase;
        double multiplier = scaleArmorEXPMultiplyPerLevel;

        if (multiplier == 1.0)
        {
            return (ulong)(baseExp * level);
        }

        double exp = baseExp * (Math.Pow(multiplier, level) - 1) / (multiplier - 1);
        return (ulong)Math.Floor(exp);
    }

    public static int ScaleArmorBaseEXPEarnedByDAMAGE(float damage)
    {
        int calcDamage = (int)Math.Round(damage);
        int multiplesCount = calcDamage / scaleArmorEXPIncreaseByAmountDamage;
        float multiplier = 1 + scaleArmorEXPMultiplyByDamage;

        float baseMultiply = scaleArmorEXPPerReceiveHit * (float)Math.Pow(multiplier, multiplesCount);

        return (int)Math.Round(baseMultiply);
    }

    public static float ScaleArmorRelativeProtectionMultiplyByLevel(int level)
    {
        int reduceEvery = scaleArmorRelativeProtectionMultiplyReductionEveryLevel;
        float baseMultiply = scaleArmorRelativeProtectionMultiply;
        float baseIncrement = scaleArmorRelativeProtectionMultiplyPerLevel;
        float reductionPerStep = scaleArmorRelativeProtectionMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double multiply = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        multiply += baseMultiply;

        return (float)multiply;
    }

    public static float ScaleArmorFlatDamageReductionMultiplyByLevel(int level)
    {
        int reduceEvery = scaleArmorFlatDamageReductionMultiplyReductionEveryLevel;
        float baseMultiply = scaleArmorFlatDamageReductionMultiply;
        float baseIncrement = scaleArmorFlatDamageReductionMultiplyPerLevel;
        float reductionPerStep = scaleArmorFlatDamageReductionMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double multiply = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        multiply += baseMultiply;

        return (float)multiply;
    }

    public static float ScaleArmorHealingEffectivnessMultiplyByLevel(int level)
    {
        int reduceEvery = scaleArmorHealingEffectivnessMultiplyReductionEveryLevel;
        float baseMultiply = scaleArmorHealingEffectivnessMultiply;
        float baseIncrement = scaleArmorHealingEffectivnessMultiplyPerLevel;
        float reductionPerStep = scaleArmorHealingEffectivnessMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double multiply = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        multiply += baseMultiply;

        return (float)multiply;
    }

    public static float ScaleArmorHungerRateMultiplyByLevel(int level)
    {
        int reduceEvery = scaleArmorHungerRateMultiplyReductionEveryLevel;
        float baseMultiply = scaleArmorHungerRateMultiply;
        float baseIncrement = scaleArmorHungerRateMultiplyPerLevel;
        float reductionPerStep = scaleArmorHungerRateMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double multiply = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        multiply += baseMultiply;

        return (float)multiply;
    }

    public static float ScaleArmorRangedWeaponsAccuracyMultiplyByLevel(int level)
    {
        int reduceEvery = scaleArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel;
        float baseMultiply = scaleArmorRangedWeaponsAccuracyMultiply;
        float baseIncrement = scaleArmorRangedWeaponsAccuracyMultiplyPerLevel;
        float reductionPerStep = scaleArmorRangedWeaponsAccuracyMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double multiply = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        multiply += baseMultiply;

        return (float)multiply;
    }

    public static float ScaleArmorRangedWeaponsSpeedMultiplyByLevel(int level)
    {
        int reduceEvery = scaleArmorRangedWeaponsSpeedMultiplyReductionEveryLevel;
        float baseMultiply = scaleArmorRangedWeaponsSpeedMultiply;
        float baseIncrement = scaleArmorRangedWeaponsSpeedMultiplyPerLevel;
        float reductionPerStep = scaleArmorRangedWeaponsSpeedMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double multiply = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        multiply += baseMultiply;

        return (float)multiply;
    }

    public static float ScaleArmorWalkSpeedMultiplyByLevel(int level)
    {
        int reduceEvery = scaleArmorWalkSpeedMultiplyReductionEveryLevel;
        float baseMultiply = scaleArmorWalkSpeedMultiply;
        float baseIncrement = scaleArmorWalkSpeedMultiplyPerLevel;
        float reductionPerStep = scaleArmorWalkSpeedMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double multiply = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        multiply += baseMultiply;

        return (float)multiply;
    }
}
