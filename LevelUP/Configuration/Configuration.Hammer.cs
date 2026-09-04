using System;
using System.Collections.Generic;
using LevelUP.Server;
using OpenConfiguration;
using Vintagestory.API.Common;

namespace LevelUP;

public class HammerLevelStatsConfiguration
{
    public int hammerEXPPerHit = 10;
    public int hammerEXPPerLevelBase = 500;
    public double hammerEXPMultiplyPerLevel = 1.2;
    public float hammerBaseDamage = 1.0f;
    public float hammerIncrementDamagePerLevel = 0.08f;
    public float hammerBaseSmithRetrieveChance = 0.0f;
    public float hammerSmithRetrieveChancePerLevel = 2.0f;
    public int hammerSmithRetrieveEveryLevelReduceChance = 10;
    public float hammerSmithRetrieveReduceChanceForEveryLevel = 0.3f;
    public float hammerBaseChanceToDouble = 0.0f;
    public float hammerIncreaseChanceToDoublePerLevel = 2.0f;
    public int hammerIncreaseChanceToDoublePerLevelReducerPerLevel = 5;
    public float hammerIncreaseChanceToDoublePerLevelReducer = 0.2f;
    public float hammerBaseChanceToTriple = 0.0f;
    public float hammerIncreaseChanceToTriplePerLevel = 1.0f;
    public int hammerIncreaseChanceToTriplePerLevelReducerPerLevel = 5;
    public float hammerIncreaseChanceToTriplePerLevelReducer = 0.1f;
    public float hammerBaseChanceToQuadruple = 0.0f;
    public float hammerIncreaseChanceToQuadruplePerLevel = 0.5f;
    public int hammerIncreaseChanceToQuadruplePerLevelReducerPerLevel = 5;
    public float hammerIncreaseChanceToQuadruplePerLevelReducer = 0.05f;
    public int hammerMaxLevel = 999;
}

public class HammerSmithChanceConfiguration : Dictionary<string, string>
{
    public HammerSmithChanceConfiguration() : base(new Dictionary<string, string>
    {
        ["copper"] = "game:nugget-nativecopper",
        ["limonite"] = "game:nugget-limonite",
        ["gold"] = "game:nugget-nativegold",
        ["galena"] = "game:nugget-galena",
        ["cassiterite"] = "game:nugget-cassiterite",
        ["chromite"] = "game:nugget-chromite",
        ["ilmenite"] = "game:nugget-ilmenite",
        ["sphalerite"] = "game:nugget-sphalerite",
        ["silver"] = "game:nugget-nativesilver",
        ["bismuthinite"] = "game:nugget-bismuthinite",
        ["magnetite"] = "game:nugget-magnetite",
        ["hematite"] = "game:nugget-hematite",
        ["malachite"] = "game:nugget-malachite",
        ["pentlandite"] = "game:nugget-pentlandite",
        ["uranium"] = "game:nugget-uranium",
        ["wolframite"] = "game:nugget-wolframite",
        ["rhodochrosite"] = "game:nugget-rhodochrosite",
    })
    { }
}

public static partial class Configuration
{
    public static Dictionary<string, int> entityExpHammer = [];
    public static Dictionary<string, string> smithChanceHammer = [];
    private static int hammerEXPPerHit = 10;
    private static int hammerEXPPerLevelBase = 500;
    private static double hammerEXPMultiplyPerLevel = 1.2;
    private static float hammerBaseDamage = 1.0f;
    private static float hammerIncrementDamagePerLevel = 0.08f;
    private static float hammerBaseSmithRetrieveChance = 0.0f;
    private static float hammerSmithRetrieveChancePerLevel = 2.0f;
    private static int hammerSmithRetrieveEveryLevelReduceChance = 10;
    private static float hammerSmithRetrieveReduceChanceForEveryLevel = 0.3f;
    private static float hammerBaseChanceToDouble = 0.0f;
    private static float hammerIncreaseChanceToDoublePerLevel = 2.0f;
    private static int hammerIncreaseChanceToDoublePerLevelReducerPerLevel = 5;
    private static float hammerIncreaseChanceToDoublePerLevelReducer = 0.2f;
    private static float hammerBaseChanceToTriple = 0.0f;
    private static float hammerIncreaseChanceToTriplePerLevel = 1.0f;
    private static int hammerIncreaseChanceToTriplePerLevelReducerPerLevel = 5;
    private static float hammerIncreaseChanceToTriplePerLevelReducer = 0.1f;
    private static float hammerBaseChanceToQuadruple = 0.0f;
    private static float hammerIncreaseChanceToQuadruplePerLevel = 0.5f;
    private static int hammerIncreaseChanceToQuadruplePerLevelReducerPerLevel = 5;
    private static float hammerIncreaseChanceToQuadruplePerLevelReducer = 0.05f;
    public static int hammerMaxLevel = 999;

    public static int ExpPerHitHammer => hammerEXPPerHit;

    public static void PopulateHammerConfiguration(ICoreAPI api)
    {
        HammerLevelStatsConfiguration hammerLevelStats = ConfigManager.Load<HammerLevelStatsConfiguration>(
            api, "ModConfig/LevelUP/levelstats", "hammer", Logger(api));

        hammerEXPPerLevelBase = hammerLevelStats.hammerEXPPerLevelBase;
        hammerEXPMultiplyPerLevel = hammerLevelStats.hammerEXPMultiplyPerLevel;
        hammerBaseDamage = hammerLevelStats.hammerBaseDamage;
        hammerIncrementDamagePerLevel = hammerLevelStats.hammerIncrementDamagePerLevel;

        hammerEXPPerHit = hammerLevelStats.hammerEXPPerHit;
        Experience.LoadExperience("Hammer", "Hit", (ulong)hammerEXPPerHit);

        hammerBaseSmithRetrieveChance = hammerLevelStats.hammerBaseSmithRetrieveChance;
        hammerSmithRetrieveChancePerLevel = hammerLevelStats.hammerSmithRetrieveChancePerLevel;
        hammerSmithRetrieveEveryLevelReduceChance = hammerLevelStats.hammerSmithRetrieveEveryLevelReduceChance;
        hammerSmithRetrieveReduceChanceForEveryLevel = hammerLevelStats.hammerSmithRetrieveReduceChanceForEveryLevel;

        hammerBaseChanceToDouble = hammerLevelStats.hammerBaseChanceToDouble;
        hammerIncreaseChanceToDoublePerLevel = hammerLevelStats.hammerIncreaseChanceToDoublePerLevel;
        hammerIncreaseChanceToDoublePerLevelReducerPerLevel = hammerLevelStats.hammerIncreaseChanceToDoublePerLevelReducerPerLevel;
        hammerIncreaseChanceToDoublePerLevelReducer = hammerLevelStats.hammerIncreaseChanceToDoublePerLevelReducer;

        hammerBaseChanceToTriple = hammerLevelStats.hammerBaseChanceToTriple;
        hammerIncreaseChanceToTriplePerLevel = hammerLevelStats.hammerIncreaseChanceToTriplePerLevel;
        hammerIncreaseChanceToTriplePerLevelReducerPerLevel = hammerLevelStats.hammerIncreaseChanceToTriplePerLevelReducerPerLevel;
        hammerIncreaseChanceToTriplePerLevelReducer = hammerLevelStats.hammerIncreaseChanceToTriplePerLevelReducer;

        hammerBaseChanceToQuadruple = hammerLevelStats.hammerBaseChanceToQuadruple;
        hammerIncreaseChanceToQuadruplePerLevel = hammerLevelStats.hammerIncreaseChanceToQuadruplePerLevel;
        hammerIncreaseChanceToQuadruplePerLevelReducerPerLevel = hammerLevelStats.hammerIncreaseChanceToQuadruplePerLevelReducerPerLevel;
        hammerIncreaseChanceToQuadruplePerLevelReducer = hammerLevelStats.hammerIncreaseChanceToQuadruplePerLevelReducer;

        hammerMaxLevel = hammerLevelStats.hammerMaxLevel;

        entityExpHammer = ConfigManager.Load<Dictionary<string, int>>(
            api, "ModConfig/LevelUP/entityexp", "hammer", Logger(api), "levelup:config/entityexp/hammer.json");

        smithChanceHammer = ConfigManager.Load<HammerSmithChanceConfiguration>(
            api, "ModConfig/LevelUP/levelstats", "hammersmiths", Logger(api));

        Debug.Log("Hammer configuration set");
    }

    public static int HammerGetLevelByEXP(ulong exp)
    {
        double baseExp = hammerEXPPerLevelBase;
        double multiplier = hammerEXPMultiplyPerLevel;

        if (multiplier <= 1.0)
        {
            return (int)(exp / baseExp);
        }

        double expDouble = exp;

        double level = Math.Log((expDouble * (multiplier - 1) / baseExp) + 1) / Math.Log(multiplier);

        return Math.Max(0, (int)Math.Floor(level));
    }

    public static ulong HammerGetExpByLevel(int level)
    {
        double baseExp = hammerEXPPerLevelBase;
        double multiplier = hammerEXPMultiplyPerLevel;

        if (multiplier == 1.0)
        {
            return (ulong)(baseExp * level);
        }

        double exp = baseExp * (Math.Pow(multiplier, level) - 1) / (multiplier - 1);
        return (ulong)Math.Floor(exp);
    }


    public static float HammerGetDamageMultiplyByLevel(int level)
    {
        return hammerBaseDamage + hammerIncrementDamagePerLevel * level;
    }

    public static bool HammerShouldRetrieveSmithByLevel(int level)
    {
        int reduceEvery = hammerSmithRetrieveEveryLevelReduceChance;
        float baseChance = hammerBaseSmithRetrieveChance;
        float baseIncrement = hammerSmithRetrieveChancePerLevel;
        float reductionPerStep = hammerSmithRetrieveReduceChanceForEveryLevel;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double finalChance = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        finalChance += baseChance;

        int chance = Random.Next(0, 100);

        if (enableExtendedLog)
            Debug.LogDebug($"Hammer should retrieve smith mechanic check: {finalChance} : {chance}");

        return finalChance >= chance;
    }

    private static float HammerCalculateChance(
            int level,
            float baseChance,
            float baseIncrement,
            int reduceEvery,
            float reductionPerStep)
    {
        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double finalChance = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        finalChance += baseChance;

        int chance = Random.Next(0, 100);

        if (enableExtendedLog)
            Debug.LogDebug($"Hammer result multiply smith mechanic check: {finalChance} : {chance}");

        return (float)finalChance;
    }

    public static float HammerGetChanceToDouble(int level)
    {
        return HammerCalculateChance(
            level,
            hammerBaseChanceToDouble,
            hammerIncreaseChanceToDoublePerLevel,
            hammerIncreaseChanceToDoublePerLevelReducerPerLevel,
            hammerIncreaseChanceToDoublePerLevelReducer);
    }

    public static float HammerGetChanceToTriple(int level)
    {
        return HammerCalculateChance(
            level,
            hammerBaseChanceToTriple,
            hammerIncreaseChanceToTriplePerLevel,
            hammerIncreaseChanceToTriplePerLevelReducerPerLevel,
            hammerIncreaseChanceToTriplePerLevelReducer);
    }

    public static float HammerGetChanceToQuadruple(int level)
    {
        return HammerCalculateChance(
            level,
            hammerBaseChanceToQuadruple,
            hammerIncreaseChanceToQuadruplePerLevel,
            hammerIncreaseChanceToQuadruplePerLevelReducerPerLevel,
            hammerIncreaseChanceToQuadruplePerLevelReducer);
    }

    public static int HammerGetResultMultiplyByLevel(int level)
    {
        // Quadruple
        float quadChance = HammerGetChanceToQuadruple(level);

        if (enableExtendedLog) Debug.Log($"Quadruple chance: {quadChance}");
        if (quadChance >= Random.Next(0, 100)) return 4;

        // Triple
        float tripleChance = HammerGetChanceToTriple(level);

        if (enableExtendedLog) Debug.Log($"Triple chance: {tripleChance}");
        if (tripleChance >= Random.Next(0, 100)) return 3;

        // Double
        float doubleChance = HammerGetChanceToDouble(level);

        if (enableExtendedLog) Debug.Log($"Double chance: {doubleChance}");
        if (doubleChance >= Random.Next(0, 100)) return 2;

        return 1;
    }
}
