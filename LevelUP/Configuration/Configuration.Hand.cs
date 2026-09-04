using System;
using System.Collections.Generic;
using LevelUP.Server;
using OpenConfiguration;
using Vintagestory.API.Common;

namespace LevelUP;

public class HandLevelStatsConfiguration
{
    public int handEXPPerHit = 10;
    public int handEXPPerLevelBase = 300;
    public double handEXPMultiplyPerLevel = 1.5;
    public float handBaseDamage = 1.0f;
    public float handIncrementDamagePerLevel = 0.2f;
    public int handMaxLevel = 999;
}

public static partial class Configuration
{
    public static Dictionary<string, int> entityExpHand = [];
    private static int handEXPPerHit = 10;
    private static int handEXPPerLevelBase = 300;
    private static double handEXPMultiplyPerLevel = 1.5;
    private static float handBaseDamage = 1.0f;
    private static float handIncrementDamagePerLevel = 0.2f;
    public static int handMaxLevel = 999;

    public static int ExpPerHitHand => handEXPPerHit;

    public static void PopulateHandConfiguration(ICoreAPI api)
    {
        HandLevelStatsConfiguration handLevelStats = ConfigManager.Load<HandLevelStatsConfiguration>(
            api, "ModConfig/LevelUP/levelstats", "hand", Logger(api));
        handEXPPerLevelBase = handLevelStats.handEXPPerLevelBase;
        handEXPMultiplyPerLevel = handLevelStats.handEXPMultiplyPerLevel;
        handBaseDamage = handLevelStats.handBaseDamage;
        handIncrementDamagePerLevel = handLevelStats.handIncrementDamagePerLevel;
        handEXPPerHit = handLevelStats.handEXPPerHit;
        Experience.LoadExperience("Hand", "Hit", (ulong)handEXPPerHit);
        handMaxLevel = handLevelStats.handMaxLevel;

        entityExpHand = ConfigManager.Load<Dictionary<string, int>>(
            api, "ModConfig/LevelUP/entityexp", "hand", Logger(api), "levelup:config/entityexp/hand.json");

        Debug.Log("Hand configuration set");
    }

    public static int HandGetLevelByEXP(ulong exp)
    {
        double baseExp = handEXPPerLevelBase;
        double multiplier = handEXPMultiplyPerLevel;

        if (multiplier <= 1.0)
        {
            return (int)(exp / baseExp);
        }

        double expDouble = exp;

        double level = Math.Log((expDouble * (multiplier - 1) / baseExp) + 1) / Math.Log(multiplier);

        return Math.Max(0, (int)Math.Floor(level));
    }

    public static ulong HandGetExpByLevel(int level)
    {
        double baseExp = handEXPPerLevelBase;
        double multiplier = handEXPMultiplyPerLevel;

        if (multiplier == 1.0)
        {
            return (ulong)(baseExp * level);
        }

        double exp = baseExp * (Math.Pow(multiplier, level) - 1) / (multiplier - 1);
        return (ulong)Math.Floor(exp);
    }


    public static float HandGetDamageMultiplyByLevel(int level)
    {
        return handBaseDamage + handIncrementDamagePerLevel * level;
    }
}
