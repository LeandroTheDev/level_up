using System;
using System.Collections.Generic;
using LevelUP.Server;
using OpenConfiguration;
using Vintagestory.API.Common;

namespace LevelUP;

public class PickaxeLevelStatsConfiguration
{
    public int pickaxeEXPPerHit = 10;
    public int pickaxeEXPPerBreaking = 10;
    public int pickaxeEXPPerLevelBase = 500;
    public double pickaxeEXPMultiplyPerLevel = 1.5;
    public float pickaxeBaseDamage = 1.0f;
    public float pickaxeIncrementDamagePerLevel = 0.03f;
    public float pickaxeBaseMiningSpeed = 1.0f;
    public float pickaxeIncrementMiningSpeedMultiplyPerLevel = 0.03f;
    public float pickaxeBaseOreMultiply = 0.0f;
    public float pickaxeIncrementOreMultiplyPerLevel = 0.1f;
    public int pickaxeMaxLevel = 999;
}

public static partial class Configuration
{
    public static Dictionary<string, int> entityExpPickaxe = [];
    public static Dictionary<string, int> oresExpPickaxe = [];
    private static int pickaxeEXPPerHit = 10;
    private static int pickaxeEXPPerBreaking = 10;
    private static int pickaxeEXPPerLevelBase = 500;
    private static double pickaxeEXPMultiplyPerLevel = 1.5;
    private static float pickaxeBaseDamage = 1.0f;
    private static float pickaxeIncrementDamagePerLevel = 0.03f;
    private static float pickaxeBaseMiningSpeed = 1.0f;
    private static float pickaxeIncrementMiningSpeedMultiplyPerLevel = 0.03f;
    private static float pickaxeBaseOreMultiply = 0.0f;
    private static float pickaxeIncrementOreMultiplyPerLevel = 0.1f;
    public static int pickaxeMaxLevel = 999;

    public static int ExpPerHitPickaxe => pickaxeEXPPerHit;
    public static int ExpPerBreakingPickaxe => pickaxeEXPPerBreaking;

    public static void PopulatePickaxeConfiguration(ICoreAPI api)
    {
        PickaxeLevelStatsConfiguration pickaxeLevelStats = ConfigManager.Load<PickaxeLevelStatsConfiguration>(
            api, "ModConfig/LevelUP/levelstats", "pickaxe", Logger(api));

        pickaxeEXPPerLevelBase = pickaxeLevelStats.pickaxeEXPPerLevelBase;
        pickaxeEXPMultiplyPerLevel = pickaxeLevelStats.pickaxeEXPMultiplyPerLevel;
        pickaxeBaseDamage = pickaxeLevelStats.pickaxeBaseDamage;
        pickaxeIncrementDamagePerLevel = pickaxeLevelStats.pickaxeIncrementDamagePerLevel;
        pickaxeEXPPerHit = pickaxeLevelStats.pickaxeEXPPerHit;
        Experience.LoadExperience("Pickaxe", "Hit", (ulong)pickaxeEXPPerHit);
        pickaxeEXPPerBreaking = pickaxeLevelStats.pickaxeEXPPerBreaking;
        Experience.LoadExperience("Pickaxe", "Break", (ulong)pickaxeEXPPerBreaking);
        pickaxeBaseMiningSpeed = pickaxeLevelStats.pickaxeBaseMiningSpeed;
        pickaxeIncrementMiningSpeedMultiplyPerLevel = pickaxeLevelStats.pickaxeIncrementMiningSpeedMultiplyPerLevel;
        pickaxeBaseOreMultiply = pickaxeLevelStats.pickaxeBaseOreMultiply;
        pickaxeIncrementOreMultiplyPerLevel = pickaxeLevelStats.pickaxeIncrementOreMultiplyPerLevel;
        pickaxeMaxLevel = pickaxeLevelStats.pickaxeMaxLevel;

        entityExpPickaxe = ConfigManager.Load<Dictionary<string, int>>(
            api, "ModConfig/LevelUP/entityexp", "pickaxe", Logger(api), "levelup:config/entityexp/pickaxe.json");

        oresExpPickaxe = ConfigManager.Load<Dictionary<string, int>>(
            api, "ModConfig/LevelUP/levelstats", "pickaxesores", Logger(api), "levelup:config/ores/pickaxe.json");

        Debug.Log("Pickaxe configuration set");
    }

    public static int PickaxeGetLevelByEXP(ulong exp)
    {
        double baseExp = pickaxeEXPPerLevelBase;
        double multiplier = pickaxeEXPMultiplyPerLevel;

        if (multiplier <= 1.0)
        {
            return (int)(exp / baseExp);
        }

        double expDouble = exp;

        double level = Math.Log((expDouble * (multiplier - 1) / baseExp) + 1) / Math.Log(multiplier);

        return Math.Max(0, (int)Math.Floor(level));
    }

    public static ulong PickaxeGetExpByLevel(int level)
    {
        double baseExp = pickaxeEXPPerLevelBase;
        double multiplier = pickaxeEXPMultiplyPerLevel;

        if (multiplier == 1.0)
        {
            return (ulong)(baseExp * level);
        }

        double exp = baseExp * (Math.Pow(multiplier, level) - 1) / (multiplier - 1);
        return (ulong)Math.Floor(exp);
    }


    public static float PickaxeGetOreMultiplyByLevel(int level)
    {
        return pickaxeBaseOreMultiply * (1 + pickaxeIncrementOreMultiplyPerLevel * Math.Max(0, level - 1));
    }

    public static float PickaxeGetDamageMultiplyByLevel(int level)
    {
        return pickaxeBaseDamage + pickaxeIncrementDamagePerLevel * level;
    }

    public static float PickaxeGetMiningMultiplyByLevel(int level)
    {
        return pickaxeBaseMiningSpeed * (1 + pickaxeIncrementMiningSpeedMultiplyPerLevel * level);
    }
}
