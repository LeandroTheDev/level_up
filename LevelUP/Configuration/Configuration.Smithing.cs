using System;
using System.Collections.Generic;
using OpenConfiguration;
using Vintagestory.API.Common;

namespace LevelUP;

public class SmithingLevelStatsConfiguration
{
    public int smithingEXPPerLevelBase = 500;
    public double smithingEXPMultiplyPerLevel = 1.1;
    public float smithingBaseDurabilityMultiply = 1.0f;
    public float smithingIncrementDurabilityMultiplyPerLevel = 0.05f;
    public float smithingBaseAttackPowerMultiply = 1.0f;
    public float smithingIncrementAttackPowerMultiplyPerLevel = 0.04f;
    public float smithingBaseMiningSpeedMultiply = 1.0f;
    public float smithingIncrementMiningSpeedMultiplyPerLevel = 0.025f;
    public float smithingBaseArmorProtectionMultiply = 1.0f;
    public float smithingIncrementArmorProtectionMultiplyPerLevel = 0.015f;
    public float smithingBaseArmorStatusMultiply = 1.0f;
    public float smithingIncrementArmorStatusMultiplyPerLevel = 0.02f;
    public int smithingMaxLevel = 999;
    public double smithingSubLevelEXPMultiply = 3.0;
}

public static partial class Configuration
{
    public static Dictionary<string, int> expPerCraftSmithing = [];
    private static int smithingEXPPerLevelBase = 500;
    private static double smithingEXPMultiplyPerLevel = 1.1;
    private static float smithingBaseDurabilityMultiply = 1.0f;
    private static float smithingIncrementDurabilityMultiplyPerLevel = 0.05f;
    private static float smithingBaseAttackPowerMultiply = 1.0f;
    private static float smithingIncrementAttackPowerMultiplyPerLevel = 0.04f;
    private static float smithingBaseMiningSpeedMultiply = 1.0f;
    private static float smithingIncrementMiningSpeedMultiplyPerLevel = 0.025f;
    private static float smithingBaseArmorProtectionMultiply = 1.0f;
    private static float smithingIncrementArmorProtectionMultiplyPerLevel = 0.015f;
    private static float smithingBaseArmorStatusMultiply = 1.0f;
    private static float smithingIncrementArmorStatusMultiplyPerLevel = 0.02f;
    public static int smithingMaxLevel = 999;
    public static double smithingSubLevelEXPMultiply = 3.0;

    public static void PopulateSmithingConfiguration(ICoreAPI api)
    {
        SmithingLevelStatsConfiguration smithingLevelStats = ConfigManager.Load<SmithingLevelStatsConfiguration>(
            api, "ModConfig/LevelUP/config/levelstats", "smithing", Logger(api));

        smithingEXPPerLevelBase = smithingLevelStats.smithingEXPPerLevelBase;
        smithingEXPMultiplyPerLevel = smithingLevelStats.smithingEXPMultiplyPerLevel;
        smithingBaseDurabilityMultiply = smithingLevelStats.smithingBaseDurabilityMultiply;
        smithingIncrementDurabilityMultiplyPerLevel = smithingLevelStats.smithingIncrementDurabilityMultiplyPerLevel;
        smithingBaseAttackPowerMultiply = smithingLevelStats.smithingBaseAttackPowerMultiply;
        smithingIncrementAttackPowerMultiplyPerLevel = smithingLevelStats.smithingIncrementAttackPowerMultiplyPerLevel;
        smithingBaseMiningSpeedMultiply = smithingLevelStats.smithingBaseMiningSpeedMultiply;
        smithingIncrementMiningSpeedMultiplyPerLevel = smithingLevelStats.smithingIncrementMiningSpeedMultiplyPerLevel;
        smithingBaseArmorProtectionMultiply = smithingLevelStats.smithingBaseArmorProtectionMultiply;
        smithingIncrementArmorProtectionMultiplyPerLevel = smithingLevelStats.smithingIncrementArmorProtectionMultiplyPerLevel;
        smithingBaseArmorStatusMultiply = smithingLevelStats.smithingBaseArmorStatusMultiply;
        smithingIncrementArmorStatusMultiplyPerLevel = smithingLevelStats.smithingIncrementArmorStatusMultiplyPerLevel;
        smithingMaxLevel = smithingLevelStats.smithingMaxLevel;
        smithingSubLevelEXPMultiply = smithingLevelStats.smithingSubLevelEXPMultiply;

        expPerCraftSmithing = ConfigManager.Load<Dictionary<string, int>>(
            api, "ModConfig/LevelUP/config/levelstats", "smithingcrafts", Logger(api), "levelup:config/crafts/smithing.json");

        Debug.Log("Smithing configuration set");
    }

    public static int SmithingGetLevelByEXP(ulong exp)
    {
        double baseExp = smithingEXPPerLevelBase;
        double multiplier = smithingEXPMultiplyPerLevel;

        if (multiplier <= 1.0)
        {
            return (int)(exp / baseExp);
        }

        double expDouble = exp;

        double level = Math.Log((expDouble * (multiplier - 1) / baseExp) + 1) / Math.Log(multiplier);

        return Math.Max(0, (int)Math.Floor(level));
    }

    public static ulong SmithingGetExpByLevel(int level)
    {
        double baseExp = smithingEXPPerLevelBase;
        double multiplier = smithingEXPMultiplyPerLevel;

        if (multiplier == 1.0)
        {
            return (ulong)(baseExp * level);
        }

        double exp = baseExp * (Math.Pow(multiplier, level) - 1) / (multiplier - 1);
        return (ulong)Math.Floor(exp);
    }


    public static float SmithingGetDurabilityMultiplyByLevel(int level)
    {
        return smithingBaseDurabilityMultiply * (1 + smithingIncrementDurabilityMultiplyPerLevel * level);
    }

    public static float SmithingGetAttackPowerMultiplyByLevel(int level)
    {
        return smithingBaseAttackPowerMultiply * (1 + smithingIncrementAttackPowerMultiplyPerLevel * level);
    }

    public static float SmithingGetMiningSpeedMultiplyByLevel(int level)
    {
        return smithingBaseMiningSpeedMultiply * (1 + smithingIncrementMiningSpeedMultiplyPerLevel * level);
    }

    public static float SmithingGetArmorProtectionMultiplyByLevel(int level)
    {
        return smithingBaseArmorProtectionMultiply * (1 + smithingIncrementArmorProtectionMultiplyPerLevel * level);
    }

    public static float SmithingGetArmorStatusMultiplyByLevel(int level)
    {
        return smithingBaseArmorStatusMultiply * (1 + smithingIncrementArmorStatusMultiplyPerLevel * level);
    }
}
