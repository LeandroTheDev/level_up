using System;
using LevelUP.Server;
using OpenConfiguration;
using Vintagestory.API.Common;

namespace LevelUP;

public class PanningLevelStatsConfiguration
{
    public int panningBaseExpPerPanning = 30;
    public int panningEXPPerLevelBase = 300;
    public double panningEXPMultiplyPerLevel = 1.3;
    public float panningBaseLootMultiply = 0.0f;
    public float panningLootMultiplyPerLevel = 0.15f;
    public float panningBaseChanceToDoubleLoot = 0.0f;
    public float panningChanceToDoubleLootPerLevel = 0.05f;
    public float panningBaseChanceToTripleLoot = 0.0f;
    public float panningChanceToTripleLootPerLevel = 0.03f;
    public float panningBaseChanceToQuadrupleLoot = 0.0f;
    public float panningChanceToQuadrupleLootPerLevel = 0.01f;
    public int panningMaxLevel = 999;
}

public static partial class Configuration
{
    private static int panningBaseExpPerPanning = 30;
    private static int panningEXPPerLevelBase = 300;
    private static double panningEXPMultiplyPerLevel = 1.3;
    private static float panningBaseLootMultiply = 0.0f;
    private static float panningLootMultiplyPerLevel = 0.15f;
    private static float panningBaseChanceToDoubleLoot = 0.0f;
    private static float panningChanceToDoubleLootPerLevel = 0.05f;
    private static float panningBaseChanceToTripleLoot = 0.0f;
    private static float panningChanceToTripleLootPerLevel = 0.03f;
    private static float panningBaseChanceToQuadrupleLoot = 0.0f;
    private static float panningChanceToQuadrupleLootPerLevel = 0.01f;
    public static int panningMaxLevel = 999;

    public static int ExpPerPanning => panningBaseExpPerPanning;

    public static void PopulatePanningConfiguration(ICoreAPI api)
    {
        PanningLevelStatsConfiguration panningLevelStats = ConfigManager.Load<PanningLevelStatsConfiguration>(
            api, "ModConfig/LevelUP/config/levelstats", "panning", Logger(api));
        panningBaseExpPerPanning = panningLevelStats.panningBaseExpPerPanning;
        Experience.LoadExperience("Panning", "Panning", (ulong)panningBaseExpPerPanning);
        panningEXPPerLevelBase = panningLevelStats.panningEXPPerLevelBase;
        panningEXPMultiplyPerLevel = panningLevelStats.panningEXPMultiplyPerLevel;
        panningBaseLootMultiply = panningLevelStats.panningBaseLootMultiply;
        panningLootMultiplyPerLevel = panningLevelStats.panningLootMultiplyPerLevel;
        panningBaseChanceToDoubleLoot = panningLevelStats.panningBaseChanceToDoubleLoot;
        panningChanceToDoubleLootPerLevel = panningLevelStats.panningChanceToDoubleLootPerLevel;
        panningBaseChanceToTripleLoot = panningLevelStats.panningBaseChanceToTripleLoot;
        panningChanceToTripleLootPerLevel = panningLevelStats.panningChanceToTripleLootPerLevel;
        panningBaseChanceToQuadrupleLoot = panningLevelStats.panningBaseChanceToQuadrupleLoot;
        panningChanceToQuadrupleLootPerLevel = panningLevelStats.panningChanceToQuadrupleLootPerLevel;
        panningMaxLevel = panningLevelStats.panningMaxLevel;

        Debug.Log("Panning configuration set");
    }

    public static int PanningGetLevelByEXP(ulong exp)
    {
        double baseExp = panningEXPPerLevelBase;
        double multiplier = panningEXPMultiplyPerLevel;

        if (multiplier <= 1.0)
        {
            return (int)(exp / baseExp);
        }

        double expDouble = exp;

        double level = Math.Log((expDouble * (multiplier - 1) / baseExp) + 1) / Math.Log(multiplier);

        return Math.Max(0, (int)Math.Floor(level));
    }

    public static ulong PanningGetExpByLevel(int level)
    {
        double baseExp = panningEXPPerLevelBase;
        double multiplier = panningEXPMultiplyPerLevel;

        if (multiplier == 1.0)
        {
            return (ulong)(baseExp * level);
        }

        double exp = baseExp * (Math.Pow(multiplier, level) - 1) / (multiplier - 1);
        return (ulong)Math.Floor(exp);
    }


    public static float PanningGetLootMultiplyByLevel(int level)
    {
        return panningBaseLootMultiply * (1 + panningLootMultiplyPerLevel * level);
    }

    public static double PanningGetChanceToDouble(int level)
    {
        return panningBaseChanceToDoubleLoot + panningChanceToDoubleLootPerLevel * level;
    }

    public static double PanningGetChanceToTriple(int level)
    {
        return panningBaseChanceToTripleLoot + panningChanceToTripleLootPerLevel * level;
    }

    public static double PanningGetChanceToQuadruple(int level)
    {
        return panningBaseChanceToQuadrupleLoot + panningChanceToQuadrupleLootPerLevel * level;
    }

    public static int PanningGetLootQuantityMultiplyByLevel(int level)
    {
        double chanceToDouble = PanningGetChanceToDouble(level);
        double chanceToTriple = PanningGetChanceToTriple(level);
        double chanceToQuadruple = PanningGetChanceToQuadruple(level);

        double roll = Random.NextDouble();

        if (roll <= chanceToQuadruple) return 4;
        if (roll <= chanceToTriple) return 3;
        if (roll <= chanceToDouble) return 2;
        return 1;
    }
}
