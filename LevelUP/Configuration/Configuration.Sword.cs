using System;
using System.Collections.Generic;
using LevelUP.Server;
using OpenConfiguration;
using Vintagestory.API.Common;

namespace LevelUP;

public class SwordLevelStatsConfiguration
{
    public int swordEXPPerHit = 10;
    public int swordEXPPerLevelBase = 500;
    public double swordEXPMultiplyPerLevel = 1.3;
    public float swordBaseDamage = 1.0f;
    public float swordIncrementDamagePerLevel = 0.07f;
    public int swordMaxLevel = 999;
}

public static partial class Configuration
{
    public static Dictionary<string, int> entityExpSword = [];
    private static int swordEXPPerHit = 10;
    private static int swordEXPPerLevelBase = 500;
    private static double swordEXPMultiplyPerLevel = 1.3;
    private static float swordBaseDamage = 1.0f;
    private static float swordIncrementDamagePerLevel = 0.07f;
    public static int swordMaxLevel = 999;

    public static int ExpPerHitSword => swordEXPPerHit;

    public static void PopulateSwordConfiguration(ICoreAPI api)
    {
        SwordLevelStatsConfiguration swordLevelStats = ConfigManager.Load<SwordLevelStatsConfiguration>(
            api, "ModConfig/LevelUP/levelstats", "sword", Logger(api));
        swordEXPPerLevelBase = swordLevelStats.swordEXPPerLevelBase;
        swordEXPMultiplyPerLevel = swordLevelStats.swordEXPMultiplyPerLevel;
        swordBaseDamage = swordLevelStats.swordBaseDamage;
        swordIncrementDamagePerLevel = swordLevelStats.swordIncrementDamagePerLevel;
        swordEXPPerHit = swordLevelStats.swordEXPPerHit;
        Experience.LoadExperience("Sword", "Hit", (ulong)swordEXPPerHit);
        swordMaxLevel = swordLevelStats.swordMaxLevel;

        entityExpSword = ConfigManager.Load<Dictionary<string, int>>(
            api, "ModConfig/LevelUP/entityexp", "sword", Logger(api), "levelup:config/entityexp/sword.json");

        Debug.Log("Sword configuration set");
    }

    public static int SwordGetLevelByEXP(ulong exp)
    {
        double baseExp = swordEXPPerLevelBase;
        double multiplier = swordEXPMultiplyPerLevel;

        if (multiplier <= 1.0)
        {
            return (int)(exp / baseExp);
        }

        double expDouble = exp;

        double level = Math.Log((expDouble * (multiplier - 1) / baseExp) + 1) / Math.Log(multiplier);

        return Math.Max(0, (int)Math.Floor(level));
    }

    public static ulong SwordGetExpByLevel(int level)
    {
        double baseExp = swordEXPPerLevelBase;
        double multiplier = swordEXPMultiplyPerLevel;

        if (multiplier == 1.0)
        {
            return (ulong)(baseExp * level);
        }

        double exp = baseExp * (Math.Pow(multiplier, level) - 1) / (multiplier - 1);
        return (ulong)Math.Floor(exp);
    }


    public static float SwordGetDamageMultiplyByLevel(int level)
    {
        return swordBaseDamage + swordIncrementDamagePerLevel * level;
    }
}
