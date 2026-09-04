using System;
using System.Collections.Generic;
using LevelUP.Server;
using OpenConfiguration;
using Vintagestory.API.Common;

namespace LevelUP;

public class PlateArmorLevelStatsConfiguration
{
    public int plateArmorEXPPerReceiveHit = 10;
    public float plateArmorEXPMultiplyByDamage = 0.3f;
    public int plateArmorEXPIncreaseByAmountDamage = 2;
    public int plateArmorEXPPerLevelBase = 500;
    public double plateArmorEXPMultiplyPerLevel = 1.2;

    public float plateArmorRelativeProtectionMultiply = 1.0f;
    public float plateArmorRelativeProtectionMultiplyPerLevel = 0.065f;
    public int plateArmorRelativeProtectionMultiplyReductionEveryLevel = 1;
    public float plateArmorRelativeProtectionMultiplyReductionPerReduce = 0.15f;

    public float plateArmorFlatDamageReductionMultiply = 1.0f;
    public float plateArmorFlatDamageReductionMultiplyPerLevel = 0.065f;
    public int plateArmorFlatDamageReductionMultiplyReductionEveryLevel = 1;
    public float plateArmorFlatDamageReductionMultiplyReductionPerReduce = 0.05f;

    public float plateArmorHealingEffectivnessMultiply = 1.0f;
    public float plateArmorHealingEffectivnessMultiplyPerLevel = 0.045f;
    public int plateArmorHealingEffectivnessMultiplyReductionEveryLevel = 1;
    public float plateArmorHealingEffectivnessMultiplyReductionPerReduce = 0.05f;

    public float plateArmorHungerRateMultiply = 1.0f;
    public float plateArmorHungerRateMultiplyPerLevel = 0.05f;
    public int plateArmorHungerRateMultiplyReductionEveryLevel = 1;
    public float plateArmorHungerRateMultiplyReductionPerReduce = 0.05f;

    public float plateArmorRangedWeaponsAccuracyMultiply = 1.0f;
    public float plateArmorRangedWeaponsAccuracyMultiplyPerLevel = 0.01f;
    public int plateArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel = 1;
    public float plateArmorRangedWeaponsAccuracyMultiplyReductionPerReduce = 0.05f;

    public float plateArmorRangedWeaponsSpeedMultiply = 1.0f;
    public float plateArmorRangedWeaponsSpeedMultiplyPerLevel = 0.01f;
    public int plateArmorRangedWeaponsSpeedMultiplyReductionEveryLevel = 1;
    public float plateArmorRangedWeaponsSpeedMultiplyReductionPerReduce = 0.05f;

    public float plateArmorWalkSpeedMultiply = 1.0f;
    public float plateArmorWalkSpeedMultiplyPerLevel = 0.01f;
    public int plateArmorWalkSpeedMultiplyReductionEveryLevel = 1;
    public float plateArmorWalkSpeedMultiplyReductionPerReduce = 0.05f;

    public int plateArmorMaxLevel = 999;
    public double plateArmorSubLevelEXPMultiply = 3.0;
}

public class PlateArmorItemsConfiguration : Dictionary<string, double>
{
    public PlateArmorItemsConfiguration() : base(new Dictionary<string, double>
    {
        ["game:armor-head-plate-copper"] = 0.3,
        ["game:armor-body-plate-copper"] = 0.5,
        ["game:armor-legs-plate-copper"] = 0.2,
        ["game:armor-head-plate-tinbronze"] = 0.3,
        ["game:armor-body-plate-tinbronze"] = 0.5,
        ["game:armor-legs-plate-tinbronze"] = 0.2,
        ["game:armor-head-plate-bismuthbronze"] = 0.3,
        ["game:armor-body-plate-bismuthbronze"] = 0.5,
        ["game:armor-legs-plate-bismuthbronze"] = 0.2,
        ["game:armor-head-plate-blackbronze"] = 0.3,
        ["game:armor-body-plate-blackbronze"] = 0.5,
        ["game:armor-legs-plate-blackbronze"] = 0.2,
        ["game:armor-head-plate-iron"] = 0.3,
        ["game:armor-body-plate-iron"] = 0.5,
        ["game:armor-legs-plate-iron"] = 0.2,
        ["game:armor-head-plate-meteoriciron"] = 0.3,
        ["game:armor-body-plate-meteoriciron"] = 0.5,
        ["game:armor-legs-plate-meteoriciron"] = 0.2,
        ["game:armor-head-plate-steel"] = 0.3,
        ["game:armor-body-plate-steel"] = 0.5,
        ["game:armor-legs-plate-steel"] = 0.2,
        ["game:armor-head-plate-gold"] = 0.3,
        ["game:armor-body-plate-gold"] = 0.5,
        ["game:armor-legs-plate-gold"] = 0.2,
        ["game:armor-head-plate-silver"] = 0.3,
        ["game:armor-body-plate-silver"] = 0.5,
        ["game:armor-legs-plate-silver"] = 0.2,
        ["game:armor-head-antique-forlorn-pristine"] = 0.3,
        ["game:armor-body-antique-forlorn-pristine"] = 0.5,
        ["game:armor-legs-antique-forlorn-pristine"] = 0.2,
        ["game:armor-head-antique-forlorn-damaged"] = 0.3,
        ["game:armor-body-antique-forlorn-damaged"] = 0.5,
        ["game:armor-legs-antique-forlorn-damaged"] = 0.2,
        ["game:armor-head-antique-forlorn-broken"] = 0.3,
        ["game:armor-body-antique-forlorn-broken"] = 0.5,
        ["game:armor-legs-antique-forlorn-broken"] = 0.2,
    })
    { }
}

public static partial class Configuration
{
    public static Dictionary<string, double> expMultiplyHitPlateArmor = [];
    private static int plateArmorEXPPerReceiveHit = 10;
    private static float plateArmorEXPMultiplyByDamage = 0.3f;
    private static int plateArmorEXPIncreaseByAmountDamage = 2;
    private static int plateArmorEXPPerLevelBase = 500;
    private static double plateArmorEXPMultiplyPerLevel = 1.2;

    private static float plateArmorRelativeProtectionMultiply = 1.0f;
    private static float plateArmorRelativeProtectionMultiplyPerLevel = 0.065f;
    private static int plateArmorRelativeProtectionMultiplyReductionEveryLevel = 1;
    private static float plateArmorRelativeProtectionMultiplyReductionPerReduce = 0.15f;

    private static float plateArmorFlatDamageReductionMultiply = 1.0f;
    private static float plateArmorFlatDamageReductionMultiplyPerLevel = 0.065f;
    private static int plateArmorFlatDamageReductionMultiplyReductionEveryLevel = 1;
    private static float plateArmorFlatDamageReductionMultiplyReductionPerReduce = 0.05f;

    private static float plateArmorHealingEffectivnessMultiply = 1.0f;
    private static float plateArmorHealingEffectivnessMultiplyPerLevel = 0.045f;
    private static int plateArmorHealingEffectivnessMultiplyReductionEveryLevel = 1;
    private static float plateArmorHealingEffectivnessMultiplyReductionPerReduce = 0.05f;

    private static float plateArmorHungerRateMultiply = 1.0f;
    private static float plateArmorHungerRateMultiplyPerLevel = 0.05f;
    private static int plateArmorHungerRateMultiplyReductionEveryLevel = 1;
    private static float plateArmorHungerRateMultiplyReductionPerReduce = 0.05f;

    private static float plateArmorRangedWeaponsAccuracyMultiply = 1.0f;
    private static float plateArmorRangedWeaponsAccuracyMultiplyPerLevel = 0.01f;
    private static int plateArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel = 1;
    private static float plateArmorRangedWeaponsAccuracyMultiplyReductionPerReduce = 0.05f;

    private static float plateArmorRangedWeaponsSpeedMultiply = 1.0f;
    private static float plateArmorRangedWeaponsSpeedMultiplyPerLevel = 0.01f;
    private static int plateArmorRangedWeaponsSpeedMultiplyReductionEveryLevel = 1;
    private static float plateArmorRangedWeaponsSpeedMultiplyReductionPerReduce = 0.05f;

    private static float plateArmorWalkSpeedMultiply = 1.0f;
    private static float plateArmorWalkSpeedMultiplyPerLevel = 0.01f;
    private static int plateArmorWalkSpeedMultiplyReductionEveryLevel = 1;
    private static float plateArmorWalkSpeedMultiplyReductionPerReduce = 0.05f;

    public static int plateArmorMaxLevel = 999;
    public static double plateArmorSubLevelEXPMultiply = 3.0;

    public static void PopulatePlateArmorConfiguration(ICoreAPI api)
    {
        PlateArmorLevelStatsConfiguration plateArmorLevelStats = ConfigManager.Load<PlateArmorLevelStatsConfiguration>(
            api, "ModConfig/LevelUP/config/levelstats", "platearmor", Logger(api));

        plateArmorEXPPerReceiveHit = plateArmorLevelStats.plateArmorEXPPerReceiveHit;
        Experience.LoadExperience("PlateArmor", "Hit", (ulong)plateArmorEXPPerReceiveHit);
        plateArmorEXPMultiplyByDamage = plateArmorLevelStats.plateArmorEXPMultiplyByDamage;
        plateArmorEXPIncreaseByAmountDamage = plateArmorLevelStats.plateArmorEXPIncreaseByAmountDamage;
        plateArmorEXPPerLevelBase = plateArmorLevelStats.plateArmorEXPPerLevelBase;
        plateArmorEXPMultiplyPerLevel = plateArmorLevelStats.plateArmorEXPMultiplyPerLevel;

        plateArmorRelativeProtectionMultiply = plateArmorLevelStats.plateArmorRelativeProtectionMultiply;
        plateArmorRelativeProtectionMultiplyPerLevel = plateArmorLevelStats.plateArmorRelativeProtectionMultiplyPerLevel;
        plateArmorRelativeProtectionMultiplyReductionEveryLevel = plateArmorLevelStats.plateArmorRelativeProtectionMultiplyReductionEveryLevel;
        plateArmorRelativeProtectionMultiplyReductionPerReduce = plateArmorLevelStats.plateArmorRelativeProtectionMultiplyReductionPerReduce;

        plateArmorFlatDamageReductionMultiply = plateArmorLevelStats.plateArmorFlatDamageReductionMultiply;
        plateArmorFlatDamageReductionMultiplyPerLevel = plateArmorLevelStats.plateArmorFlatDamageReductionMultiplyPerLevel;
        plateArmorFlatDamageReductionMultiplyReductionEveryLevel = plateArmorLevelStats.plateArmorFlatDamageReductionMultiplyReductionEveryLevel;
        plateArmorFlatDamageReductionMultiplyReductionPerReduce = plateArmorLevelStats.plateArmorFlatDamageReductionMultiplyReductionPerReduce;

        plateArmorHealingEffectivnessMultiply = plateArmorLevelStats.plateArmorHealingEffectivnessMultiply;
        plateArmorHealingEffectivnessMultiplyPerLevel = plateArmorLevelStats.plateArmorHealingEffectivnessMultiplyPerLevel;
        plateArmorHealingEffectivnessMultiplyReductionEveryLevel = plateArmorLevelStats.plateArmorHealingEffectivnessMultiplyReductionEveryLevel;
        plateArmorHealingEffectivnessMultiplyReductionPerReduce = plateArmorLevelStats.plateArmorHealingEffectivnessMultiplyReductionPerReduce;

        plateArmorHungerRateMultiply = plateArmorLevelStats.plateArmorHungerRateMultiply;
        plateArmorHungerRateMultiplyPerLevel = plateArmorLevelStats.plateArmorHungerRateMultiplyPerLevel;
        plateArmorHungerRateMultiplyReductionEveryLevel = plateArmorLevelStats.plateArmorHungerRateMultiplyReductionEveryLevel;
        plateArmorHungerRateMultiplyReductionPerReduce = plateArmorLevelStats.plateArmorHungerRateMultiplyReductionPerReduce;

        plateArmorRangedWeaponsAccuracyMultiply = plateArmorLevelStats.plateArmorRangedWeaponsAccuracyMultiply;
        plateArmorRangedWeaponsAccuracyMultiplyPerLevel = plateArmorLevelStats.plateArmorRangedWeaponsAccuracyMultiplyPerLevel;
        plateArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel = plateArmorLevelStats.plateArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel;
        plateArmorRangedWeaponsAccuracyMultiplyReductionPerReduce = plateArmorLevelStats.plateArmorRangedWeaponsAccuracyMultiplyReductionPerReduce;

        plateArmorRangedWeaponsSpeedMultiply = plateArmorLevelStats.plateArmorRangedWeaponsSpeedMultiply;
        plateArmorRangedWeaponsSpeedMultiplyPerLevel = plateArmorLevelStats.plateArmorRangedWeaponsSpeedMultiplyPerLevel;
        plateArmorRangedWeaponsSpeedMultiplyReductionEveryLevel = plateArmorLevelStats.plateArmorRangedWeaponsSpeedMultiplyReductionEveryLevel;
        plateArmorRangedWeaponsSpeedMultiplyReductionPerReduce = plateArmorLevelStats.plateArmorRangedWeaponsSpeedMultiplyReductionPerReduce;

        plateArmorWalkSpeedMultiply = plateArmorLevelStats.plateArmorWalkSpeedMultiply;
        plateArmorWalkSpeedMultiplyPerLevel = plateArmorLevelStats.plateArmorWalkSpeedMultiplyPerLevel;
        plateArmorWalkSpeedMultiplyReductionEveryLevel = plateArmorLevelStats.plateArmorWalkSpeedMultiplyReductionEveryLevel;
        plateArmorWalkSpeedMultiplyReductionPerReduce = plateArmorLevelStats.plateArmorWalkSpeedMultiplyReductionPerReduce;

        plateArmorMaxLevel = plateArmorLevelStats.plateArmorMaxLevel;
        plateArmorSubLevelEXPMultiply = plateArmorLevelStats.plateArmorSubLevelEXPMultiply;

        expMultiplyHitPlateArmor = ConfigManager.Load<PlateArmorItemsConfiguration>(
            api, "ModConfig/LevelUP/config/levelstats", "platearmoritems", Logger(api));

        Debug.Log("Plate Armor configuration set");
    }

    public static int PlateArmorGetLevelByEXP(ulong exp)
    {
        double baseExp = plateArmorEXPPerLevelBase;
        double multiplier = plateArmorEXPMultiplyPerLevel;

        if (multiplier <= 1.0)
        {
            return (int)(exp / baseExp);
        }

        double expDouble = exp;

        double level = Math.Log((expDouble * (multiplier - 1) / baseExp) + 1) / Math.Log(multiplier);

        return Math.Max(0, (int)Math.Floor(level));
    }

    public static ulong PlateArmorGetExpByLevel(int level)
    {
        double baseExp = plateArmorEXPPerLevelBase;
        double multiplier = plateArmorEXPMultiplyPerLevel;

        if (multiplier == 1.0)
        {
            return (ulong)(baseExp * level);
        }

        double exp = baseExp * (Math.Pow(multiplier, level) - 1) / (multiplier - 1);
        return (ulong)Math.Floor(exp);
    }

    public static int PlateArmorBaseEXPEarnedByDAMAGE(float damage)
    {
        int calcDamage = (int)Math.Round(damage);
        int multiplesCount = calcDamage / plateArmorEXPIncreaseByAmountDamage;
        float multiplier = 1 + plateArmorEXPMultiplyByDamage;

        float baseMultiply = plateArmorEXPPerReceiveHit * (float)Math.Pow(multiplier, multiplesCount);

        return (int)Math.Round(baseMultiply);
    }

    public static float PlateArmorRelativeProtectionMultiplyByLevel(int level)
    {
        int reduceEvery = plateArmorRelativeProtectionMultiplyReductionEveryLevel;
        float baseMultiply = plateArmorRelativeProtectionMultiply;
        float baseIncrement = plateArmorRelativeProtectionMultiplyPerLevel;
        float reductionPerStep = plateArmorRelativeProtectionMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double multiply = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        multiply += baseMultiply;

        return (float)multiply;
    }

    public static float PlateArmorFlatDamageReductionMultiplyByLevel(int level)
    {
        int reduceEvery = plateArmorFlatDamageReductionMultiplyReductionEveryLevel;
        float baseMultiply = plateArmorFlatDamageReductionMultiply;
        float baseIncrement = plateArmorFlatDamageReductionMultiplyPerLevel;
        float reductionPerStep = plateArmorFlatDamageReductionMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double multiply = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        multiply += baseMultiply;

        return (float)multiply;
    }

    public static float PlateArmorHealingEffectivnessMultiplyByLevel(int level)
    {
        int reduceEvery = plateArmorHealingEffectivnessMultiplyReductionEveryLevel;
        float baseMultiply = plateArmorHealingEffectivnessMultiply;
        float baseIncrement = plateArmorHealingEffectivnessMultiplyPerLevel;
        float reductionPerStep = plateArmorHealingEffectivnessMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double multiply = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        multiply += baseMultiply;

        return (float)multiply;
    }

    public static float PlateArmorHungerRateMultiplyByLevel(int level)
    {
        int reduceEvery = plateArmorHungerRateMultiplyReductionEveryLevel;
        float baseMultiply = plateArmorHungerRateMultiply;
        float baseIncrement = plateArmorHungerRateMultiplyPerLevel;
        float reductionPerStep = plateArmorHungerRateMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double multiply = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        multiply += baseMultiply;

        return (float)multiply;
    }

    public static float PlateArmorRangedWeaponsAccuracyMultiplyByLevel(int level)
    {
        int reduceEvery = plateArmorRangedWeaponsAccuracyMultiplyReductionEveryLevel;
        float baseMultiply = plateArmorRangedWeaponsAccuracyMultiply;
        float baseIncrement = plateArmorRangedWeaponsAccuracyMultiplyPerLevel;
        float reductionPerStep = plateArmorRangedWeaponsAccuracyMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double multiply = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        multiply += baseMultiply;

        return (float)multiply;
    }

    public static float PlateArmorRangedWeaponsSpeedMultiplyByLevel(int level)
    {
        int reduceEvery = plateArmorRangedWeaponsSpeedMultiplyReductionEveryLevel;
        float baseMultiply = plateArmorRangedWeaponsSpeedMultiply;
        float baseIncrement = plateArmorRangedWeaponsSpeedMultiplyPerLevel;
        float reductionPerStep = plateArmorRangedWeaponsSpeedMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double multiply = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        multiply += baseMultiply;

        return (float)multiply;
    }

    public static float PlateArmorWalkSpeedMultiplyByLevel(int level)
    {
        int reduceEvery = plateArmorWalkSpeedMultiplyReductionEveryLevel;
        float baseMultiply = plateArmorWalkSpeedMultiply;
        float baseIncrement = plateArmorWalkSpeedMultiplyPerLevel;
        float reductionPerStep = plateArmorWalkSpeedMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double multiply = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        multiply += baseMultiply;

        return (float)multiply;
    }
}
