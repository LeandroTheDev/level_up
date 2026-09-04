using System;
using LevelUP.Server;
using OpenConfiguration;
using Vintagestory.API.Common;

namespace LevelUP;

public class MetabolismLevelStatsConfiguration
{
    public int metabolismEXPPerReceiveHit = 100;
    public float metabolismEXPPerSaturationLost = 0.11f;
    public int metabolismEXPPerLevelBase = 500;
    public double metabolismEXPMultiplyPerLevel = 1.33;
    public float metabolismSaturationIncreasePerLevel = 50.0f;
    public float metabolismBaseSaturation = 1500.0f;
    public float metabolismBaseSaturationReceiveMultiply = 1.0f;
    public float metabolismSaturationReceiveMultiplyPerLevel = 0.05f;
    public int metabolismSaturationReceiveMultiplyReductionEveryLevel = 1;
    public float metabolismSaturationReceiveMultiplyReductionPerReduce = 0.05f;
    public int metabolismMaxLevel = 999;
}

public static partial class Configuration
{
    private static int metabolismEXPPerReceiveHit = 100;
    private static float metabolismEXPPerSaturationLost = 0.11f;
    private static int metabolismEXPPerLevelBase = 500;
    private static double metabolismEXPMultiplyPerLevel = 1.33;
    private static float metabolismSaturationIncreasePerLevel = 50.0f;
    private static float metabolismBaseSaturation = 1500.0f;
    private static float metabolismBaseSaturationReceiveMultiply = 1.0f;
    private static float metabolismSaturationReceiveMultiplyPerLevel = 0.05f;
    private static int metabolismSaturationReceiveMultiplyReductionEveryLevel = 1;
    private static float metabolismSaturationReceiveMultiplyReductionPerReduce = 0.05f;
    public static int metabolismMaxLevel = 999;

    public static int EXPPerHitMetabolism => metabolismEXPPerReceiveHit;
    public static float EXPPerSaturationLostMetabolism => metabolismEXPPerSaturationLost;

    public static float BaseSaturationMetabolism => metabolismBaseSaturation;

    public static void PopulateMetabolismConfiguration(ICoreAPI api)
    {
        MetabolismLevelStatsConfiguration metabolismLevelStats = ConfigManager.Load<MetabolismLevelStatsConfiguration>(
            api, "ModConfig/LevelUP/levelstats", "metabolism", Logger(api));

        metabolismEXPPerReceiveHit = metabolismLevelStats.metabolismEXPPerReceiveHit;
        Experience.LoadExperience("Metabolism", "Hit", (ulong)metabolismEXPPerReceiveHit);
        metabolismEXPPerSaturationLost = metabolismLevelStats.metabolismEXPPerSaturationLost;
        metabolismEXPPerLevelBase = metabolismLevelStats.metabolismEXPPerLevelBase;
        metabolismEXPMultiplyPerLevel = metabolismLevelStats.metabolismEXPMultiplyPerLevel;
        metabolismSaturationIncreasePerLevel = metabolismLevelStats.metabolismSaturationIncreasePerLevel;
        metabolismBaseSaturation = metabolismLevelStats.metabolismBaseSaturation;
        metabolismBaseSaturationReceiveMultiply = metabolismLevelStats.metabolismBaseSaturationReceiveMultiply;
        metabolismSaturationReceiveMultiplyPerLevel = metabolismLevelStats.metabolismSaturationReceiveMultiplyPerLevel;
        metabolismSaturationReceiveMultiplyReductionEveryLevel = metabolismLevelStats.metabolismSaturationReceiveMultiplyReductionEveryLevel;
        metabolismSaturationReceiveMultiplyReductionPerReduce = metabolismLevelStats.metabolismSaturationReceiveMultiplyReductionPerReduce;
        metabolismMaxLevel = metabolismLevelStats.metabolismMaxLevel;

        Debug.Log("Metabolism configuration set");
    }

    public static int MetabolismGetLevelByEXP(ulong exp)
    {
        double baseExp = metabolismEXPPerLevelBase;
        double multiplier = metabolismEXPMultiplyPerLevel;

        if (multiplier <= 1.0)
        {
            return (int)(exp / baseExp);
        }

        double expDouble = exp;

        double level = Math.Log((expDouble * (multiplier - 1) / baseExp) + 1) / Math.Log(multiplier);

        return Math.Max(0, (int)Math.Floor(level));
    }

    public static ulong MetabolismGetExpByLevel(int level)
    {
        double baseExp = metabolismEXPPerLevelBase;
        double multiplier = metabolismEXPMultiplyPerLevel;

        if (multiplier == 1.0)
        {
            return (ulong)(baseExp * level);
        }

        double exp = baseExp * (Math.Pow(multiplier, level) - 1) / (multiplier - 1);
        return (ulong)Math.Floor(exp);
    }


    public static float MetabolismGetMaxSaturationByLevel(int level)
    {
        return metabolismBaseSaturation + metabolismSaturationIncreasePerLevel * level;
    }

    public static float MetabolismGetSaturationReceiveMultiplyByLevel(int level)
    {
        int reduceEvery = metabolismSaturationReceiveMultiplyReductionEveryLevel;
        float baseSaturation = metabolismBaseSaturationReceiveMultiply;
        float baseIncrement = metabolismSaturationReceiveMultiplyPerLevel;
        float reductionPerStep = metabolismSaturationReceiveMultiplyReductionPerReduce;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double reducer = baseIncrement * (Math.Pow(r, level) - 1) / (r - 1);
        reducer = baseSaturation - reducer;

        Debug.LogDebug($"[MetabolismGetSaturationReceiveMultiplyByLevel] reducer returned: {reducer}");

        return (float)reducer;
    }
}
