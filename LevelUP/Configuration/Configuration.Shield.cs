using System;
using LevelUP.Server;
using OpenConfiguration;
using Vintagestory.API.Common;

namespace LevelUP;

public class ShieldLevelStatsConfiguration
{
    public int shieldEXPPerHit = 10;
    public int shieldEXPPerLevelBase = 600;
    public double shieldEXPMultiplyPerLevel = 1.5;
    public float shieldBasePassiveProjectile = 1.0f;
    public float shieldPassiveProjectilePerLevel = 0.08f;
    public float shieldBaseActiveProjectile = 1.0f;
    public float shieldActiveProjectilePerLevel = 0.12f;
    public float shieldBasePassive = 1.0f;
    public float shieldPassivePerLevel = 0.07f;
    public float shieldBaseActive = 1.0f;
    public float shieldActivePerLevel = 0.1f;
    public float shieldBaseProjectileDamageAbsorption = 1.0f;
    public float shieldProjectileDamageAbsorptionPerLevel = 0.11f;
    public float shieldBaseDamageAbsorption = 1.0f;
    public float shieldDamageAbsorptionPerLevel = 0.06f;
    public int shieldMaxLevel = 999;
}

public static partial class Configuration
{
    private static int shieldEXPPerHit = 10;
    private static int shieldEXPPerLevelBase = 600;
    private static double shieldEXPMultiplyPerLevel = 1.5;
    private static float shieldBasePassiveProjectile = 1.0f;
    private static float shieldPassiveProjectilePerLevel = 0.08f;
    private static float shieldBaseActiveProjectile = 1.0f;
    private static float shieldActiveProjectilePerLevel = 0.12f;
    private static float shieldBasePassive = 1.0f;
    private static float shieldPassivePerLevel = 0.07f;
    private static float shieldBaseActive = 1.0f;
    private static float shieldActivePerLevel = 0.1f;
    private static float shieldBaseProjectileDamageAbsorption = 1.0f;
    private static float shieldProjectileDamageAbsorptionPerLevel = 0.11f;
    private static float shieldBaseDamageAbsorption = 1.0f;
    private static float shieldDamageAbsorptionPerLevel = 0.06f;
    public static int shieldMaxLevel = 999;

    public static int ExpPerHitShield => shieldEXPPerHit;

    public static void PopulateShieldConfiguration(ICoreAPI api)
    {
        ShieldLevelStatsConfiguration shieldLevelStats = ConfigManager.Load<ShieldLevelStatsConfiguration>(
            api, "ModConfig/LevelUP/config/levelstats", "shield", Logger(api));
        shieldEXPPerLevelBase = shieldLevelStats.shieldEXPPerLevelBase;
        shieldEXPMultiplyPerLevel = shieldLevelStats.shieldEXPMultiplyPerLevel;
        shieldBasePassiveProjectile = shieldLevelStats.shieldBasePassiveProjectile;
        shieldPassiveProjectilePerLevel = shieldLevelStats.shieldPassiveProjectilePerLevel;
        shieldBaseActiveProjectile = shieldLevelStats.shieldBaseActiveProjectile;
        shieldActiveProjectilePerLevel = shieldLevelStats.shieldActiveProjectilePerLevel;
        shieldBasePassive = shieldLevelStats.shieldBasePassive;
        shieldPassivePerLevel = shieldLevelStats.shieldPassivePerLevel;
        shieldBaseActive = shieldLevelStats.shieldBaseActive;
        shieldActivePerLevel = shieldLevelStats.shieldActivePerLevel;
        shieldBaseProjectileDamageAbsorption = shieldLevelStats.shieldBaseProjectileDamageAbsorption;
        shieldProjectileDamageAbsorptionPerLevel = shieldLevelStats.shieldProjectileDamageAbsorptionPerLevel;
        shieldBaseDamageAbsorption = shieldLevelStats.shieldBaseDamageAbsorption;
        shieldDamageAbsorptionPerLevel = shieldLevelStats.shieldDamageAbsorptionPerLevel;
        shieldEXPPerHit = shieldLevelStats.shieldEXPPerHit;
        Experience.LoadExperience("Shield", "Hit", (ulong)shieldEXPPerHit);
        shieldMaxLevel = shieldLevelStats.shieldMaxLevel;

        Debug.Log("Shield configuration set");
    }

    public static int ShieldGetLevelByEXP(ulong exp)
    {
        double baseExp = shieldEXPPerLevelBase;
        double multiplier = shieldEXPMultiplyPerLevel;

        if (multiplier <= 1.0)
        {
            return (int)(exp / baseExp);
        }

        double expDouble = exp;

        double level = Math.Log((expDouble * (multiplier - 1) / baseExp) + 1) / Math.Log(multiplier);

        return Math.Max(0, (int)Math.Floor(level));
    }

    public static ulong ShieldGetExpByLevel(int level)
    {
        double baseExp = shieldEXPPerLevelBase;
        double multiplier = shieldEXPMultiplyPerLevel;

        if (multiplier == 1.0)
        {
            return (ulong)(baseExp * level);
        }

        double exp = baseExp * (Math.Pow(multiplier, level) - 1) / (multiplier - 1);
        return (ulong)Math.Floor(exp);
    }

    public static float ShieldGetPassiveProjectileByLevel(int level)
    {
        return shieldBasePassiveProjectile + shieldPassiveProjectilePerLevel * level;
    }

    public static float ShieldGetActiveProjectileByLevel(int level)
    {
        return shieldBaseActiveProjectile + shieldActiveProjectilePerLevel * level;
    }

    public static float ShieldGetPassiveByLevel(int level)
    {
        return shieldBasePassive + shieldPassivePerLevel * level;
    }

    public static float ShieldGetActiveByLevel(int level)
    {
        return shieldBaseActive + shieldActivePerLevel * level;
    }

    public static float ShieldGetProjectileDamageAbsorptionByLevel(int level)
    {
        return shieldBaseProjectileDamageAbsorption + shieldProjectileDamageAbsorptionPerLevel * level;
    }

    public static float ShieldGetDamageAbsorptionByLevel(int level)
    {
        return shieldBaseDamageAbsorption + shieldDamageAbsorptionPerLevel * level;
    }
}
