using System;
using System.Collections.Generic;
using LevelUP.Server;
using OpenConfiguration;
using Vintagestory.API.Common;

namespace LevelUP;

public class CookingLevelStatsConfiguration
{
    public int cookingBaseExpPerCooking = 30;
    public int cookingEXPPerLevelBase = 100;
    public double cookingEXPMultiplyPerLevel = 1.3;
    public float cookingBaseFreshHoursMultiply = 0.5f;
    public float cookingFreshHoursMultiplyPerLevel = 0.04f;
    public float cookingBaseChanceToIncreaseServings = 1.0f;
    public int cookingReduceChanceToIncreaseServings = 5;
    public float cookingIncrementChanceToIncreaseServings = 2.0f;
    public float cookingChanceToIncreaseServingsReducerTotal = 0.2f;
    public int cookingBaseRollsChanceToIncreaseServings = 1;
    public int cookingEarnRollsChanceToIncreaseServingsEveryLevel = 5;
    public int cookingEarnRollsChanceToIncreaseServingsQuantity = 1;
    public double cookingSubLevelEXPMultiply = 3.0;
    public int cookingMaxLevel = 999;
}

public class CookingSinglesConfiguration : Dictionary<string, double>
{
    public CookingSinglesConfiguration() : base(new Dictionary<string, double>
    {
        ["game:redmeat-cooked"] = 0.5,
        ["game:poultry-cooked"] = 0.4,
        ["game:fish-cooked"] = 0.4,
        ["game:bushmeat-cooked"] = 0.3,
        ["game:vegetable-cookedcattailroot"] = 0.1,
    })
    { }
}

public class CookingPotsConfiguration : Dictionary<string, double>
{
    public CookingPotsConfiguration() : base(new Dictionary<string, double>
    {
        ["game:claypot-cooked"] = 3.0,
        ["game:claypot-blue-cooked"] = 3.0,
        ["game:claypot-fire-cooked"] = 3.0,
        ["game:claypot-black-cooked"] = 3.0,
        ["game:claypot-brown-cooked"] = 3.0,
        ["game:claypot-cream-cooked"] = 3.0,
        ["game:claypot-earthyorange-cooked"] = 3.0,
        ["game:claypot-gray-cooked"] = 3.0,
        ["game:claypot-orange-cooked"] = 3.0,
        ["game:claypot-red-cooked"] = 3.0,
        ["game:claypot-tan-cooked"] = 3.0,
    })
    { }
}

public class CookingOvenConfiguration : Dictionary<string, double>
{
    public CookingOvenConfiguration() : base(new Dictionary<string, double>
    {
        ["game:bread-spelt-perfect"] = 0.5,
        ["game:bread-rye-perfect"] = 0.5,
        ["game:bread-flax-perfect"] = 0.5,
        ["game:bread-rice-perfect"] = 0.6,
        ["game:bread-cassava-perfect"] = 0.4,
        ["game:bread-amaranth-perfect"] = 0.5,
        ["game:bread-sunflower-perfect"] = 0.5,
        ["game:pie-perfect"] = 3.0,
    })
    { }
}

public static partial class Configuration
{
    public static Dictionary<string, double> expMultiplySingleCooking = [];
    public static Dictionary<string, double> expMultiplyPotsCooking = [];
    public static Dictionary<string, double> expMultiplyOvenCooking = [];
    private static int cookingBaseExpPerCooking = 30;
    private static int cookingEXPPerLevelBase = 100;
    private static double cookingEXPMultiplyPerLevel = 1.3;
    private static float cookingBaseFreshHoursMultiply = 0.5f;
    private static float cookingFreshHoursMultiplyPerLevel = 0.04f;
    private static float cookingBaseChanceToIncreaseServings = 1.0f;
    private static int cookingReduceChanceToIncreaseServings = 5;
    private static float cookingIncrementChanceToIncreaseServings = 2.0f;
    private static float cookingChanceToIncreaseServingsReducerTotal = 0.2f;
    private static int cookingBaseRollsChanceToIncreaseServings = 1;
    private static int cookingEarnRollsChanceToIncreaseServingsEveryLevel = 5;
    private static int cookingEarnRollsChanceToIncreaseServingsQuantity = 1;
    public static double cookingSubLevelEXPMultiply = 3.0;
    public static int cookingMaxLevel = 999;

    public static int ExpPerCookingcooking => cookingBaseExpPerCooking;

    public static void PopulateCookingConfiguration(ICoreAPI api)
    {
        CookingLevelStatsConfiguration cookingLevelStats = ConfigManager.Load<CookingLevelStatsConfiguration>(
            api, "ModConfig/LevelUP/levelstats", "cooking", Logger(api));

        cookingBaseExpPerCooking = cookingLevelStats.cookingBaseExpPerCooking;
        Experience.LoadExperience("Cooking", "Cooking", (ulong)cookingBaseExpPerCooking);
        cookingEXPPerLevelBase = cookingLevelStats.cookingEXPPerLevelBase;
        cookingEXPMultiplyPerLevel = cookingLevelStats.cookingEXPMultiplyPerLevel;
        cookingBaseFreshHoursMultiply = cookingLevelStats.cookingBaseFreshHoursMultiply;
        cookingFreshHoursMultiplyPerLevel = cookingLevelStats.cookingFreshHoursMultiplyPerLevel;
        cookingBaseChanceToIncreaseServings = cookingLevelStats.cookingBaseChanceToIncreaseServings;
        cookingIncrementChanceToIncreaseServings = cookingLevelStats.cookingIncrementChanceToIncreaseServings;
        cookingReduceChanceToIncreaseServings = cookingLevelStats.cookingReduceChanceToIncreaseServings;
        cookingChanceToIncreaseServingsReducerTotal = cookingLevelStats.cookingChanceToIncreaseServingsReducerTotal;
        cookingBaseRollsChanceToIncreaseServings = cookingLevelStats.cookingBaseRollsChanceToIncreaseServings;
        cookingEarnRollsChanceToIncreaseServingsEveryLevel = cookingLevelStats.cookingEarnRollsChanceToIncreaseServingsEveryLevel;
        cookingEarnRollsChanceToIncreaseServingsQuantity = cookingLevelStats.cookingEarnRollsChanceToIncreaseServingsQuantity;
        cookingMaxLevel = cookingLevelStats.cookingMaxLevel;
        cookingSubLevelEXPMultiply = cookingLevelStats.cookingSubLevelEXPMultiply;

        expMultiplySingleCooking = ConfigManager.Load<CookingSinglesConfiguration>(
            api, "ModConfig/LevelUP/levelstats", "cookingsingles", Logger(api));

        expMultiplyPotsCooking = ConfigManager.Load<CookingPotsConfiguration>(
            api, "ModConfig/LevelUP/levelstats", "cookingpots", Logger(api));

        expMultiplyOvenCooking = ConfigManager.Load<CookingOvenConfiguration>(
            api, "ModConfig/LevelUP/levelstats", "cookingoven", Logger(api));

        Debug.Log("Cooking configuration set");
    }

    public static int CookingGetLevelByEXP(ulong exp)
    {
        double baseExp = cookingEXPPerLevelBase;
        double multiplier = cookingEXPMultiplyPerLevel;

        if (multiplier <= 1.0)
        {
            return (int)(exp / baseExp);
        }

        double expDouble = exp;

        double level = Math.Log((expDouble * (multiplier - 1) / baseExp) + 1) / Math.Log(multiplier);

        return Math.Max(0, (int)Math.Floor(level));
    }

    public static ulong CookingGetExpByLevel(int level)
    {
        double baseExp = cookingEXPPerLevelBase;
        double multiplier = cookingEXPMultiplyPerLevel;

        if (multiplier == 1.0)
        {
            return (ulong)(baseExp * level);
        }

        double exp = baseExp * (Math.Pow(multiplier, level) - 1) / (multiplier - 1);
        return (ulong)Math.Floor(exp);
    }


    public static float CookingGetFreshHoursMultiplyByLevel(int level)
    {
        return cookingBaseFreshHoursMultiply * (1 + cookingFreshHoursMultiplyPerLevel * level);
    }

    public static int CookingGetServingsByLevelAndServings(int level, int quantityServings)
    {
        double finalChance = CookingGetRollChanceByLevel(level);

        int rolls = CookingGetRollsByLevel(level);

        if (enableExtendedLog)
            Debug.LogDebug($"Cooking serving rolls: {rolls}");

        for (int i = 0; i < rolls; i++)
        {
            int servingChance = Random.Next(0, 100);

            if (enableExtendedLog)
                Debug.LogDebug($"Cooking serving roll: {finalChance} : {servingChance}");

            if (finalChance >= servingChance)
                quantityServings += 1;
        }

        return quantityServings;
    }

    public static int CookingGetRollsByLevel(int level)
    {
        int rolls = cookingBaseRollsChanceToIncreaseServings;
        rolls += level / cookingEarnRollsChanceToIncreaseServingsEveryLevel * cookingEarnRollsChanceToIncreaseServingsQuantity;
        return rolls;
    }

    public static double CookingGetRollChanceByLevel(int level)
    {
        int reduceEvery = cookingReduceChanceToIncreaseServings;
        float baseChance = cookingBaseChanceToIncreaseServings;
        float baseIncrement = cookingIncrementChanceToIncreaseServings;
        float reductionPerStep = cookingChanceToIncreaseServingsReducerTotal;

        double r = Math.Pow(1 - reductionPerStep, 1.0 / reduceEvery);

        double finalChance = baseIncrement * (1 - Math.Pow(r, level)) / (1 - r);
        finalChance += baseChance;
        return finalChance;
    }
}
