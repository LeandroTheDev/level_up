using System;
using System.Collections.Generic;
using LevelUP.Server;
using OpenConfiguration;
using Vintagestory.API.Common;

namespace LevelUP;

public class FarmingLevelStatsConfiguration
{
    public int farmingEXPPerTill = 10;
    public int farmingEXPPerLevelBase = 500;
    public double farmingEXPMultiplyPerLevel = 1.2;
    public float farmingBaseHarvestMultiply = 1.0f;
    public float farmingIncrementHarvestMultiplyPerLevel = 0.09f;
    public float farmingBaseForageMultiply = 1.0f;
    public float farmingIncrementForageMultiplyPerLevel = 0.08f;
    public int farmingMaxLevel = 999;
}

public static partial class Configuration
{
    public static Dictionary<string, int> expPerHarvestFarming = [];
    private static int farmingEXPPerTill = 10;
    private static int farmingEXPPerLevelBase = 500;
    private static double farmingEXPMultiplyPerLevel = 1.2;
    private static float farmingBaseHarvestMultiply = 1.0f;
    private static float farmingIncrementHarvestMultiplyPerLevel = 0.09f;
    private static float farmingBaseForageMultiply = 1.0f;
    private static float farmingIncrementForageMultiplyPerLevel = 0.08f;
    public static int farmingMaxLevel = 999;

    public static int ExpPerTillFarming => farmingEXPPerTill;
    public static float BaseHarvestMultiplyFarming => farmingBaseHarvestMultiply;

    public static void PopulateFarmingConfiguration(ICoreAPI api)
    {
        FarmingLevelStatsConfiguration farmingLevelStats = ConfigManager.Load<FarmingLevelStatsConfiguration>(
            api, "ModConfig/LevelUP/levelstats", "farming", Logger(api));
        farmingEXPPerLevelBase = farmingLevelStats.farmingEXPPerLevelBase;
        farmingEXPMultiplyPerLevel = farmingLevelStats.farmingEXPMultiplyPerLevel;
        farmingEXPPerTill = farmingLevelStats.farmingEXPPerTill;
        Experience.LoadExperience("Farming", "Till", (ulong)farmingEXPPerTill);
        farmingBaseHarvestMultiply = farmingLevelStats.farmingBaseHarvestMultiply;
        farmingIncrementHarvestMultiplyPerLevel = farmingLevelStats.farmingIncrementHarvestMultiplyPerLevel;
        farmingBaseForageMultiply = farmingLevelStats.farmingBaseForageMultiply;
        farmingIncrementForageMultiplyPerLevel = farmingLevelStats.farmingIncrementForageMultiplyPerLevel;
        farmingMaxLevel = farmingLevelStats.farmingMaxLevel;

        expPerHarvestFarming = ConfigManager.Load<Dictionary<string, int>>(
            api, "ModConfig/LevelUP/levelstats", "farmingcrops", Logger(api), "levelup:config/crops/farming.json");

        Debug.Log("Farming configuration set");
    }

    public static int FarmingGetLevelByEXP(ulong exp)
    {
        double baseExp = farmingEXPPerLevelBase;
        double multiplier = farmingEXPMultiplyPerLevel;

        if (multiplier <= 1.0)
        {
            return (int)(exp / baseExp);
        }

        double expDouble = exp;

        double level = Math.Log((expDouble * (multiplier - 1) / baseExp) + 1) / Math.Log(multiplier);

        return Math.Max(0, (int)Math.Floor(level));
    }

    public static ulong FarmingGetExpByLevel(int level)
    {
        double baseExp = farmingEXPPerLevelBase;
        double multiplier = farmingEXPMultiplyPerLevel;

        if (multiplier == 1.0)
        {
            return (ulong)(baseExp * level);
        }

        double exp = baseExp * (Math.Pow(multiplier, level) - 1) / (multiplier - 1);
        return (ulong)Math.Floor(exp);
    }


    public static float FarmingGetHarvestMultiplyByLevel(int level)
    {
        return farmingBaseHarvestMultiply * (1 + farmingIncrementHarvestMultiplyPerLevel * level);
    }

    public static float FarmingGetForageMultiplyByLevel(int level)
    {
        return farmingBaseForageMultiply * (1 + farmingIncrementForageMultiplyPerLevel * level);
    }
}
