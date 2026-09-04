using System;
using LevelUP.Server;
using OpenConfiguration;
using Vintagestory.API.Common;

namespace LevelUP;

public class VitalityLevelStatsConfiguration
{
    public int vitalityEXPPerReceiveHit = 10;
    public float vitalityEXPMultiplyByDamage = 0.3f;
    public int vitalityEXPIncreaseByAmountDamage = 2;
    public int vitalityEXPPerLevelBase = 500;
    public double vitalityEXPMultiplyPerLevel = 1.2;
    public float vitalityBaseHP = 15.0f;
    public float vitalityHPIncreasePerLevel = 0.5f;
    public float vitalityBaseHPRegen = 1.0f;
    public float vitalityHPRegenIncreasePerLevel = 0.1f;
    public int vitalityDamageLimit = 100;
    public int vitalityMaxLevel = 999;
}

public static partial class Configuration
{
    private static int vitalityEXPPerReceiveHit = 10;
    private static float vitalityEXPMultiplyByDamage = 0.3f;
    private static int vitalityEXPIncreaseByAmountDamage = 2;
    private static int vitalityEXPPerLevelBase = 500;
    private static double vitalityEXPMultiplyPerLevel = 1.2;
    private static float vitalityBaseHP = 15.0f;
    private static float vitalityHPIncreasePerLevel = 0.5f;
    private static float vitalityBaseHPRegen = 1.0f;
    private static float vitalityHPRegenIncreasePerLevel = 0.1f;
    private static int vitalityDamageLimit = 100;
    public static int vitalityMaxLevel = 999;

    public static int DamageLimitVitality => vitalityDamageLimit;
    public static float BaseHPVitality => vitalityBaseHP;
    public static float BaseHPRegenVitality => vitalityBaseHPRegen;

    public static void PopulateVitalityConfiguration(ICoreAPI api)
    {
        VitalityLevelStatsConfiguration vitalityLevelStats = ConfigManager.Load<VitalityLevelStatsConfiguration>(
            api, "ModConfig/LevelUP/config/levelstats", "vitality", Logger(api));

        vitalityEXPPerLevelBase = vitalityLevelStats.vitalityEXPPerLevelBase;
        vitalityEXPMultiplyPerLevel = vitalityLevelStats.vitalityEXPMultiplyPerLevel;
        vitalityEXPPerReceiveHit = vitalityLevelStats.vitalityEXPPerReceiveHit;
        Experience.LoadExperience("Vitality", "Hit", (ulong)vitalityEXPPerReceiveHit);
        vitalityEXPMultiplyByDamage = vitalityLevelStats.vitalityEXPMultiplyByDamage;
        vitalityHPIncreasePerLevel = vitalityLevelStats.vitalityHPIncreasePerLevel;
        vitalityBaseHP = vitalityLevelStats.vitalityBaseHP;
        vitalityEXPIncreaseByAmountDamage = vitalityLevelStats.vitalityEXPIncreaseByAmountDamage;
        vitalityBaseHPRegen = vitalityLevelStats.vitalityBaseHPRegen;
        vitalityHPRegenIncreasePerLevel = vitalityLevelStats.vitalityHPRegenIncreasePerLevel;
        vitalityDamageLimit = vitalityLevelStats.vitalityDamageLimit;
        vitalityMaxLevel = vitalityLevelStats.vitalityMaxLevel;

        Debug.Log("Vitality configuration set");
    }

    public static int VitalityGetLevelByEXP(ulong exp)
    {
        double baseExp = vitalityEXPPerLevelBase;
        double multiplier = vitalityEXPMultiplyPerLevel;

        if (multiplier <= 1.0)
        {
            return (int)(exp / baseExp);
        }

        double expDouble = exp;

        double level = Math.Log((expDouble * (multiplier - 1) / baseExp) + 1) / Math.Log(multiplier);

        return Math.Max(0, (int)Math.Floor(level));
    }

    public static ulong VitalityGetExpByLevel(int level)
    {
        double baseExp = vitalityEXPPerLevelBase;
        double multiplier = vitalityEXPMultiplyPerLevel;

        if (multiplier == 1.0)
        {
            return (ulong)(baseExp * level);
        }

        double exp = baseExp * (Math.Pow(multiplier, level) - 1) / (multiplier - 1);
        return (ulong)Math.Floor(exp);
    }


    public static float VitalityGetMaxHealthByLevel(int level)
    {
        return vitalityBaseHP + vitalityHPIncreasePerLevel * level;
    }

    public static float VitalityGetHealthRegenMultiplyByLevel(int level)
    {
        return vitalityBaseHPRegen + vitalityHPRegenIncreasePerLevel * level;
    }

    public static int VitalityEXPEarnedByDAMAGE(float damage)
    {
        int calcDamage = (int)Math.Round(damage);
        int multiplesCount = calcDamage / vitalityEXPIncreaseByAmountDamage;
        float multiplier = 1 + vitalityEXPMultiplyByDamage;

        float baseMultiply = vitalityEXPPerReceiveHit * (float)Math.Pow(multiplier, multiplesCount);

        return (int)Math.Round(baseMultiply);
    }
}
