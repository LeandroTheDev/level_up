using System;
using LevelUP.Server;
using OpenConfiguration;
using Vintagestory.API.Common;

namespace LevelUP;

public class QuenchingLevelStatsConfiguration
{
    public int quenchingEXPPerLevelBase = 300;
    public double quenchingEXPMultiplyPerLevel = 1.1;
    public ulong quenchingBaseExpPerQuench = 50;
    public ulong quenchingBaseExpPerTemper = 40;
    public float quenchingBaseShatterChanceAddedMultiply = 1.0f;
    public float quenchingReduceShatterChanceAddedMultiplyPerLevel = 0.05f;
    public float quenchingMinShatterChanceAddedMultiply = 0.15f;
    public float quenchingBasePowerGainMultiply = 1.0f;
    public float quenchingIncrementPowerGainMultiplyPerLevel = 0.01f;
    public float quenchingBaseTemperEfficiencyMultiply = 1.0f;
    public float quenchingIncrementTemperEfficiencyMultiplyPerLevel = 0.012f;
    public int quenchingMaxLevel = 999;
    public double quenchingSubLevelEXPMultiply = 3.0;
}

public static partial class Configuration
{
    private static int quenchingEXPPerLevelBase = 300;
    private static double quenchingEXPMultiplyPerLevel = 1.1;
    public static ulong quenchingBaseExpPerQuench = 50;
    public static ulong quenchingBaseExpPerTemper = 40;
    private static float quenchingBaseShatterChanceAddedMultiply = 1.0f;
    private static float quenchingReduceShatterChanceAddedMultiplyPerLevel = 0.05f;
    private static float quenchingMinShatterChanceAddedMultiply = 0.15f;
    private static float quenchingBasePowerGainMultiply = 1.0f;
    private static float quenchingIncrementPowerGainMultiplyPerLevel = 0.01f;
    private static float quenchingBaseTemperEfficiencyMultiply = 1.0f;
    private static float quenchingIncrementTemperEfficiencyMultiplyPerLevel = 0.012f;
    public static int quenchingMaxLevel = 999;
    public static double quenchingSubLevelEXPMultiply = 3.0;

    public static void PopulateQuenchingConfiguration(ICoreAPI api)
    {
        QuenchingLevelStatsConfiguration quenchingLevelStats = ConfigManager.Load<QuenchingLevelStatsConfiguration>(
            api, "ModConfig/LevelUP/config/levelstats", "quenching", Logger(api));

        quenchingEXPPerLevelBase = quenchingLevelStats.quenchingEXPPerLevelBase;
        quenchingEXPMultiplyPerLevel = quenchingLevelStats.quenchingEXPMultiplyPerLevel;
        quenchingBaseExpPerQuench = quenchingLevelStats.quenchingBaseExpPerQuench;
        quenchingBaseExpPerTemper = quenchingLevelStats.quenchingBaseExpPerTemper;
        quenchingBaseShatterChanceAddedMultiply = quenchingLevelStats.quenchingBaseShatterChanceAddedMultiply;
        quenchingReduceShatterChanceAddedMultiplyPerLevel = quenchingLevelStats.quenchingReduceShatterChanceAddedMultiplyPerLevel;
        quenchingMinShatterChanceAddedMultiply = quenchingLevelStats.quenchingMinShatterChanceAddedMultiply;
        quenchingBasePowerGainMultiply = quenchingLevelStats.quenchingBasePowerGainMultiply;
        quenchingIncrementPowerGainMultiplyPerLevel = quenchingLevelStats.quenchingIncrementPowerGainMultiplyPerLevel;
        quenchingBaseTemperEfficiencyMultiply = quenchingLevelStats.quenchingBaseTemperEfficiencyMultiply;
        quenchingIncrementTemperEfficiencyMultiplyPerLevel = quenchingLevelStats.quenchingIncrementTemperEfficiencyMultiplyPerLevel;
        quenchingMaxLevel = quenchingLevelStats.quenchingMaxLevel;
        quenchingSubLevelEXPMultiply = quenchingLevelStats.quenchingSubLevelEXPMultiply;

        Experience.LoadExperience("Quenching", "Quench", quenchingBaseExpPerQuench);
        Experience.LoadExperience("Quenching", "Temper", quenchingBaseExpPerTemper);

        Debug.Log("Quenching configuration set");
    }

    public static int QuenchingGetLevelByEXP(ulong exp)
    {
        double baseExp = quenchingEXPPerLevelBase;
        double multiplier = quenchingEXPMultiplyPerLevel;

        if (multiplier <= 1.0)
        {
            return (int)(exp / baseExp);
        }

        double expDouble = exp;

        double level = Math.Log((expDouble * (multiplier - 1) / baseExp) + 1) / Math.Log(multiplier);

        return Math.Max(0, (int)Math.Floor(level));
    }

    public static ulong QuenchingGetExpByLevel(int level)
    {
        double baseExp = quenchingEXPPerLevelBase;
        double multiplier = quenchingEXPMultiplyPerLevel;

        if (multiplier == 1.0)
        {
            return (ulong)(baseExp * level);
        }

        double exp = baseExp * (Math.Pow(multiplier, level) - 1) / (multiplier - 1);
        return (ulong)Math.Floor(exp);
    }

    /// <summary>
    /// Multiplier applied on top of the shatterchance delta the vanilla quench just added.
    /// Decreases with level (safer quenching), floored at quenchingMinShatterChanceAddedMultiply.
    /// </summary>
    public static float QuenchingGetShatterChanceAddedMultiplyByLevel(int level)
    {
        return Math.Max(quenchingMinShatterChanceAddedMultiply, quenchingBaseShatterChanceAddedMultiply * (1 - quenchingReduceShatterChanceAddedMultiplyPerLevel * level));
    }

    /// <summary>
    /// Multiplier applied on top of the power/durability bonus delta the vanilla quench just added.
    /// </summary>
    public static float QuenchingGetPowerGainMultiplyByLevel(int level)
    {
        return quenchingBasePowerGainMultiply * (1 + quenchingIncrementPowerGainMultiplyPerLevel * level);
    }

    /// <summary>
    /// Multiplier applied on top of the (negative) shatterchance delta the vanilla temper just subtracted.
    /// </summary>
    public static float QuenchingGetTemperEfficiencyMultiplyByLevel(int level)
    {
        return quenchingBaseTemperEfficiencyMultiply * (1 + quenchingIncrementTemperEfficiencyMultiplyPerLevel * level);
    }
}
