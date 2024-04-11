using System;
using System.Collections.Generic;
using Vintagestory.API.Common;

namespace LevelUP;

public static class Configuration
{
    public static int GetLevelByLevelTypeEXP(string levelType, int exp)
    {
        switch (levelType)
        {
            case "Hunter": return HunterGetLevelByEXP(exp);
            case "Bow": return BowGetLevelByEXP(exp);
            case "Cutlery": return CutleryGetLevelByEXP(exp);
            case "Axe": return AxeGetLevelByEXP(exp);
            case "Pickaxe": return PickaxeGetLevelByEXP(exp);
            case "Shovel": return ShovelGetLevelByEXP(exp);
            case "Spear": return SpearGetLevelByEXP(exp);
            case "Farming": return FarmingGetLevelByEXP(exp);
            case "Cooking": return CookingGetLevelByEXP(exp);
            default: break;
        }
        return 1;
    }

    public static float GetMiningSpeedByLevelTypeEXP(string levelType, int exp)
    {
        switch (levelType)
        {
            case "Axe": return AxeGetMiningMultiplyByEXP(exp);
            case "Pickaxe": return PickaxeGetMiningMultiplyByEXP(exp);
            case "Shovel": return ShovelGetMiningMultiplyByEXP(exp);
            default: break;
        }
        return -1.0f;
    }

    #region hunter
    public static readonly Dictionary<string, int> entityExpHunter = [];
    private static int hunterEXPPerLevelBase = 10;
    private static double hunterEXPMultiplyPerLevel = 1.5;
    private static float hunterBaseDamage = 1.0f;
    private static float hunterIncrementDamagePerLevel = 0.1f;

    public static void PopulateHunterConfiguration(ICoreAPI api)
    {
        Dictionary<string, object> hunterLevelStats = api.Assets.Get(new AssetLocation("levelup:config/levelstats/hunter.json")).ToObject<Dictionary<string, object>>();
        { //hunterEXPPerLevelBase
            if (hunterLevelStats.TryGetValue("hunterEXPPerLevelBase", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: hunterEXPPerLevelBase is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: hunterEXPPerLevelBase is not int is {value.GetType()}");
                else hunterEXPPerLevelBase = (int)(long)value;
            else Debug.Log("CONFIGURATION ERROR: hunterEXPPerLevelBase not set");
        }
        { //hunterEXPMultiplyPerLevel
            if (hunterLevelStats.TryGetValue("hunterEXPMultiplyPerLevel", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: hunterEXPMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: hunterEXPMultiplyPerLevel is not double is {value.GetType()}");
                else hunterEXPMultiplyPerLevel = (double)value;
            else Debug.Log("CONFIGURATION ERROR: hunterEXPMultiplyPerLevel not set");
        }
        { //hunterBaseDamage
            if (hunterLevelStats.TryGetValue("hunterBaseDamage", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: hunterBaseDamage is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: hunterBaseDamage is not double is {value.GetType()}");
                else hunterBaseDamage = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: hunterBaseDamage not set");
        }
        { //hunterIncrementDamagePerLevel
            if (hunterLevelStats.TryGetValue("hunterIncrementDamagePerLevel", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: hunterIncrementDamagePerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: hunterIncrementDamagePerLevel is not double is {value.GetType()}");
                else hunterIncrementDamagePerLevel = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: hunterIncrementDamagePerLevel not set");
        }

        // Get entity exp
        Dictionary<string, object> tmpentityExpHunter = api.Assets.Get(new AssetLocation("levelup:config/entityexp/hunter.json")).ToObject<Dictionary<string, object>>();
        foreach (KeyValuePair<string, object> pair in tmpentityExpHunter)
        {
            if (pair.Value is long value) entityExpHunter.Add(pair.Key, (int)value);
            else Debug.Log($"CONFIGURATION ERROR: entityExpHunter {pair.Key} is not int");
        }

        Debug.Log("Hunter configuration set");
    }

    public static int HunterGetLevelByEXP(int exp)
    {

        int level = 0;
        // Exp base for level
        double expPerLevelBase = hunterEXPPerLevelBase;
        double calcExp = double.Parse(exp.ToString());
        while (calcExp > 0)
        {
            level += 1;
            calcExp -= expPerLevelBase;
            // 10 percentage increasing per level
            expPerLevelBase *= hunterEXPMultiplyPerLevel;
        }
        return level;
    }

    public static float HunterGetDamageMultiplyByEXP(int exp)
    {
        float baseDamage = hunterBaseDamage;
        int level = HunterGetLevelByEXP(exp);

        float incrementDamage = hunterIncrementDamagePerLevel;
        float multiply = 0.0f;
        while (level > 1)
        {
            multiply += incrementDamage;
            level -= 1;
        }

        baseDamage += baseDamage *= incrementDamage;
        return baseDamage;
    }
    #endregion

    #region bow
    public static readonly Dictionary<string, int> entityExpBow = [];
    private static int bowEXPPerHit = 1;
    private static int bowEXPPerLevelBase = 10;
    private static double bowEXPMultiplyPerLevel = 1.3;
    private static float bowBaseDamage = 1.0f;
    private static float bowIncrementDamagePerLevel = 0.1f;
    private static float bowBaseDurabilityRestoreChance = 0.0f;
    private static float bowDurabilityRestoreChancePerLevel = 2.0f;
    private static int bowDurabilityRestoreEveryLevelReduceChance = 10;
    private static float bowDurabilityRestoreReduceChanceForEveryLevel = 0.5f;
    private static float bowBaseChanceToNotLoseArrow = 50.0f;
    private static float bowChanceToNotLoseArrowBaseIncreasePerLevel = 2.0f;
    private static int bowChanceToNotLoseArrowReduceIncreaseEveryLevel = 5;
    private static float bowChanceToNotLoseArrowReduceQuantityEveryLevel = 0.5f;

    public static int ExpPerHitBow => bowEXPPerHit;

    public static void PopulateBowConfiguration(ICoreAPI api)
    {
        Dictionary<string, object> bowLevelStats = api.Assets.Get(new AssetLocation("levelup:config/levelstats/bow.json")).ToObject<Dictionary<string, object>>();
        { //bowEXPPerLevelBase
            if (bowLevelStats.TryGetValue("bowEXPPerLevelBase", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: bowEXPPerLevelBase is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: bowEXPPerLevelBase is not int is {value.GetType()}");
                else bowEXPPerLevelBase = (int)(long)value;
            else Debug.Log("CONFIGURATION ERROR: bowEXPPerLevelBase not set");
        }
        { //bowEXPMultiplyPerLevel
            if (bowLevelStats.TryGetValue("bowEXPMultiplyPerLevel", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: bowEXPMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: bowEXPMultiplyPerLevel is not double is {value.GetType()}");
                else bowEXPMultiplyPerLevel = (double)value;
            else Debug.Log("CONFIGURATION ERROR: bowEXPMultiplyPerLevel not set");
        }
        { //bowBaseDamage
            if (bowLevelStats.TryGetValue("bowBaseDamage", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: bowBaseDamage is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: bowBaseDamage is not double is {value.GetType()}");
                else bowBaseDamage = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: bowBaseDamage not set");
        }
        { //bowIncrementDamagePerLevel
            if (bowLevelStats.TryGetValue("bowIncrementDamagePerLevel", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: bowIncrementDamagePerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: bowIncrementDamagePerLevel is not double is {value.GetType()}");
                else bowIncrementDamagePerLevel = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: bowIncrementDamagePerLevel not set");
        }
        { //bowEXPPerHit
            if (bowLevelStats.TryGetValue("bowEXPPerHit", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: bowEXPPerHit is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: bowEXPPerHit is not int is {value.GetType()}");
                else bowEXPPerHit = (int)(long)value;
            else Debug.Log("CONFIGURATION ERROR: bowEXPPerHit not set");
        }
        { //bowBaseDurabilityRestoreChance
            if (bowLevelStats.TryGetValue("bowBaseDurabilityRestoreChance", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: bowBaseDurabilityRestoreChance is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: bowBaseDurabilityRestoreChance is not double is {value.GetType()}");
                else bowBaseDurabilityRestoreChance = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: bowBaseDurabilityRestoreChance not set");
        }
        { //bowDurabilityRestoreChancePerLevel
            if (bowLevelStats.TryGetValue("bowDurabilityRestoreChancePerLevel", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: bowDurabilityRestoreChancePerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: bowDurabilityRestoreChancePerLevel is not double is {value.GetType()}");
                else bowDurabilityRestoreChancePerLevel = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: bowDurabilityRestoreChancePerLevel not set");
        }
        { //bowDurabilityRestoreEveryLevelReduceChance
            if (bowLevelStats.TryGetValue("bowDurabilityRestoreEveryLevelReduceChance", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: bowDurabilityRestoreEveryLevelReduceChance is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: bowDurabilityRestoreEveryLevelReduceChance is not int is {value.GetType()}");
                else bowDurabilityRestoreEveryLevelReduceChance = (int)(long)value;
            else Debug.Log("CONFIGURATION ERROR: bowDurabilityRestoreEveryLevelReduceChance not set");
        }
        { //bowDurabilityRestoreReduceChanceForEveryLevel
            if (bowLevelStats.TryGetValue("bowDurabilityRestoreReduceChanceForEveryLevel", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: bowDurabilityRestoreReduceChanceForEveryLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: bowDurabilityRestoreReduceChanceForEveryLevel is not double is {value.GetType()}");
                else bowDurabilityRestoreReduceChanceForEveryLevel = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: bowDurabilityRestoreReduceChanceForEveryLevel not set");
        }
        { //bowBaseChanceToNotLoseArrow
            if (bowLevelStats.TryGetValue("bowBaseChanceToNotLoseArrow", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: bowBaseChanceToNotLoseArrow is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: bowBaseChanceToNotLoseArrow is not double is {value.GetType()}");
                else bowBaseChanceToNotLoseArrow = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: bowBaseChanceToNotLoseArrow not set");
        }
        { //bowChanceToNotLoseArrowBaseIncreasePerLevel
            if (bowLevelStats.TryGetValue("bowChanceToNotLoseArrowBaseIncreasePerLevel", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: bowChanceToNotLoseArrowBaseIncreasePerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: bowChanceToNotLoseArrowBaseIncreasePerLevel is not double is {value.GetType()}");
                else bowChanceToNotLoseArrowBaseIncreasePerLevel = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: bowChanceToNotLoseArrowBaseIncreasePerLevel not set");
        }
        { //bowChanceToNotLoseArrowReduceIncreaseEveryLevel
            if (bowLevelStats.TryGetValue("bowChanceToNotLoseArrowReduceIncreaseEveryLevel", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: bowChanceToNotLoseArrowReduceIncreaseEveryLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: bowChanceToNotLoseArrowReduceIncreaseEveryLevel is not int is {value.GetType()}");
                else bowChanceToNotLoseArrowReduceIncreaseEveryLevel = (int)(long)value;
            else Debug.Log("CONFIGURATION ERROR: bowChanceToNotLoseArrowReduceIncreaseEveryLevel not set");
        }
        { //bowChanceToNotLoseArrowReduceQuantityEveryLevel
            if (bowLevelStats.TryGetValue("bowChanceToNotLoseArrowReduceQuantityEveryLevel", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: bowChanceToNotLoseArrowReduceQuantityEveryLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: bowChanceToNotLoseArrowReduceQuantityEveryLevel is not double is {value.GetType()}");
                else bowChanceToNotLoseArrowReduceQuantityEveryLevel = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: bowChanceToNotLoseArrowReduceQuantityEveryLevel not set");
        }

        // Get entity exp
        Dictionary<string, object> tmpentityExpBow = api.Assets.Get(new AssetLocation("levelup:config/entityexp/bow.json")).ToObject<Dictionary<string, object>>();
        foreach (KeyValuePair<string, object> pair in tmpentityExpBow)
        {
            if (pair.Value is long value) entityExpBow.Add(pair.Key, (int)value);
            else Debug.Log($"CONFIGURATION ERROR: entityExpBow {pair.Key} is not int");
        }

        Debug.Log("Bow configuration set");
    }

    public static int BowGetLevelByEXP(int exp)
    {

        int level = 0;
        // Exp base for level
        double expPerLevelBase = bowEXPPerLevelBase;
        double calcExp = double.Parse(exp.ToString());
        while (calcExp > 0)
        {
            level += 1;
            calcExp -= expPerLevelBase;
            // 10 percentage increasing per level
            expPerLevelBase *= bowEXPMultiplyPerLevel;
        }
        return level;
    }

    public static float BowGetDamageMultiplyByEXP(int exp)
    {
        float baseDamage = bowBaseDamage;
        int level = BowGetLevelByEXP(exp);

        float incrementDamage = bowIncrementDamagePerLevel;
        float multiply = 0.0f;
        while (level > 1)
        {
            multiply += incrementDamage;
            level -= 1;
        }

        baseDamage += baseDamage *= incrementDamage;
        return baseDamage;
    }

    public static bool BowRollChanceToNotReduceDurabilityByEXP(int exp)
    {
        int level = BowGetLevelByEXP(exp);
        float baseChanceToNotReduce = bowBaseDurabilityRestoreChance;
        float chanceToNotReduce = bowDurabilityRestoreChancePerLevel;
        while (level > 1)
        {
            level -= 1;
            // Every {} levels reduce the durability chance multiplicator
            if (level % bowDurabilityRestoreEveryLevelReduceChance == 0)
                chanceToNotReduce -= bowDurabilityRestoreReduceChanceForEveryLevel;
            // Increasing chance
            baseChanceToNotReduce += chanceToNotReduce;
        }
        // Check the chance 
        if (baseChanceToNotReduce >= new Random().Next(0, 100)) return true;
        else return false;
    }

    public static float BowGetChanceToNotLoseArrowByEXP(int exp)
    {
        int level = BowGetLevelByEXP(exp);
        float baseChanceToNotLose = bowBaseChanceToNotLoseArrow;
        float chanceToNotLose = bowChanceToNotLoseArrowBaseIncreasePerLevel;
        while (level > 1)
        {
            level -= 1;
            // Every {} levels reduce the lose arrow chance multiplicator
            if (level % bowChanceToNotLoseArrowReduceIncreaseEveryLevel == 0)
                chanceToNotLose -= bowChanceToNotLoseArrowReduceQuantityEveryLevel;
            // Increasing chance
            baseChanceToNotLose += chanceToNotLose;
        }
        // Returns the chance
        if (baseChanceToNotLose >= new Random().Next(0, 100)) return 1.0f;
        else return 0.0f;
    }

    #endregion

    #region cutlery
    public static readonly Dictionary<string, int> entityExpCutlery = [];
    private static int cutleryEXPPerHit = 1;
    private static int cutleryEXPPerHarvest = 5;

    private static int cutleryEXPPerLevelBase = 10;
    private static double cutleryEXPMultiplyPerLevel = 1.3;
    private static float cutleryBaseDamage = 1.0f;
    private static float cutleryIncrementDamagePerLevel = 0.1f;
    private static float cutleryBaseHarvestMultiply = 0.5f;
    private static float cutleryIncrementHarvestMultiplyPerLevel = 0.2f;
    private static float cutleryBaseDurabilityRestoreChance = 0.0f;
    private static float cutleryDurabilityRestoreChancePerLevel = 2.0f;
    private static int cutleryDurabilityRestoreEveryLevelReduceChance = 10;
    private static float cutleryDurabilityRestoreReduceChanceForEveryLevel = 0.5f;

    public static int ExpPerHitCutlery => cutleryEXPPerHit;
    public static int ExpPerHarvestCutlery => cutleryEXPPerHarvest;

    public static void PopulateCutleryConfiguration(ICoreAPI api)
    {
        Dictionary<string, object> cutleryLevelStats = api.Assets.Get(new AssetLocation("levelup:config/levelstats/cutlery.json")).ToObject<Dictionary<string, object>>();
        { //cutleryEXPPerLevelBase
            if (cutleryLevelStats.TryGetValue("cutleryEXPPerLevelBase", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: cutleryEXPPerLevelBase is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: cutleryEXPPerLevelBase is not int is {value.GetType()}");
                else cutleryEXPPerLevelBase = (int)(long)value;
            else Debug.Log("CONFIGURATION ERROR: cutleryEXPPerLevelBase not set");
        }
        { //cutleryEXPMultiplyPerLevel
            if (cutleryLevelStats.TryGetValue("cutleryEXPMultiplyPerLevel", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: cutleryEXPMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: cutleryEXPMultiplyPerLevel is not double is {value.GetType()}");
                else cutleryEXPMultiplyPerLevel = (double)value;
            else Debug.Log("CONFIGURATION ERROR: cutleryEXPMultiplyPerLevel not set");
        }
        { //cutleryBaseDamage
            if (cutleryLevelStats.TryGetValue("cutleryBaseDamage", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: cutleryBaseDamage is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: cutleryBaseDamage is not double is {value.GetType()}");
                else cutleryBaseDamage = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: cutleryBaseDamage not set");
        }
        { //cutleryIncrementDamagePerLevel
            if (cutleryLevelStats.TryGetValue("cutleryIncrementDamagePerLevel", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: cutleryIncrementDamagePerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: cutleryIncrementDamagePerLevel is not double is {value.GetType()}");
                else cutleryIncrementDamagePerLevel = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: cutleryIncrementDamagePerLevel not set");
        }
        { //cutleryEXPPerHit
            if (cutleryLevelStats.TryGetValue("cutleryEXPPerHit", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: cutleryEXPPerHit is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: cutleryEXPPerHit is not int is {value.GetType()}");
                else cutleryEXPPerHit = (int)(long)value;
            else Debug.Log("CONFIGURATION ERROR: cutleryEXPPerHit not set");
        }
        { //cutleryEXPPerHarvest
            if (cutleryLevelStats.TryGetValue("cutleryEXPPerHarvest", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: cutleryEXPPerHarvest is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: cutleryEXPPerHarvest is not int is {value.GetType()}");
                else cutleryEXPPerHarvest = (int)(long)value;
            else Debug.Log("CONFIGURATION ERROR: cutleryEXPPerHarvest not set");
        }
        { //cutleryBaseHarvestMultiply
            if (cutleryLevelStats.TryGetValue("cutleryBaseHarvestMultiply", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: cutleryBaseHarvestMultiply is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: cutleryBaseHarvestMultiply is not double is {value.GetType()}");
                else cutleryBaseHarvestMultiply = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: cutleryBaseHarvestMultiply not set");
        }
        { //cutleryIncrementHarvestMultiplyPerLevel
            if (cutleryLevelStats.TryGetValue("cutleryIncrementHarvestMultiplyPerLevel", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: cutleryIncrementHarvestMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: cutleryIncrementHarvestMultiplyPerLevel is not double is {value.GetType()}");
                else cutleryIncrementHarvestMultiplyPerLevel = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: cutleryIncrementHarvestMultiplyPerLevel not set");
        }
        { //cutleryBaseDurabilityRestoreChance
            if (cutleryLevelStats.TryGetValue("cutleryBaseDurabilityRestoreChance", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: cutleryBaseDurabilityRestoreChance is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: cutleryBaseDurabilityRestoreChance is not double is {value.GetType()}");
                else cutleryBaseDurabilityRestoreChance = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: cutleryBaseDurabilityRestoreChance not set");
        }
        { //cutleryDurabilityRestoreChancePerLevel
            if (cutleryLevelStats.TryGetValue("cutleryDurabilityRestoreChancePerLevel", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: cutleryDurabilityRestoreChancePerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: cutleryDurabilityRestoreChancePerLevel is not double is {value.GetType()}");
                else cutleryDurabilityRestoreChancePerLevel = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: cutleryDurabilityRestoreChancePerLevel not set");
        }
        { //cutleryDurabilityRestoreEveryLevelReduceChance
            if (cutleryLevelStats.TryGetValue("cutleryDurabilityRestoreEveryLevelReduceChance", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: cutleryDurabilityRestoreEveryLevelReduceChance is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: cutleryDurabilityRestoreEveryLevelReduceChance is not int is {value.GetType()}");
                else cutleryDurabilityRestoreEveryLevelReduceChance = (int)(long)value;
            else Debug.Log("CONFIGURATION ERROR: cutleryDurabilityRestoreEveryLevelReduceChance not set");
        }
        { //cutleryDurabilityRestoreReduceChanceForEveryLevel
            if (cutleryLevelStats.TryGetValue("cutleryDurabilityRestoreReduceChanceForEveryLevel", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: cutleryDurabilityRestoreReduceChanceForEveryLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: cutleryDurabilityRestoreReduceChanceForEveryLevel is not double is {value.GetType()}");
                else cutleryDurabilityRestoreReduceChanceForEveryLevel = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: cutleryDurabilityRestoreReduceChanceForEveryLevel not set");
        }

        // Get entity exp
        Dictionary<string, object> tmpentityExpCutlery = api.Assets.Get(new AssetLocation("levelup:config/entityexp/cutlery.json")).ToObject<Dictionary<string, object>>();
        foreach (KeyValuePair<string, object> pair in tmpentityExpCutlery)
        {
            if (pair.Value is long value) entityExpCutlery.Add(pair.Key, (int)value);
            else Debug.Log($"CONFIGURATION ERROR: entityExpCutlery {pair.Key} is not int");
        }

        Debug.Log("Cutlery configuration set");
    }

    public static int CutleryGetLevelByEXP(int exp)
    {

        int level = 0;
        // Exp base for level
        double expPerLevelBase = cutleryEXPPerLevelBase;
        double calcExp = double.Parse(exp.ToString());
        while (calcExp > 0)
        {
            level += 1;
            calcExp -= expPerLevelBase;
            // 10 percentage increasing per level
            expPerLevelBase *= cutleryEXPMultiplyPerLevel;
        }
        return level;
    }

    public static float CutleryGetDamageMultiplyByEXP(int exp)
    {
        float baseDamage = cutleryBaseDamage;
        int level = CutleryGetLevelByEXP(exp);

        float incrementDamage = cutleryIncrementDamagePerLevel;
        float multiply = 0.0f;
        while (level > 1)
        {
            multiply += incrementDamage;
            level -= 1;
        }

        baseDamage += baseDamage *= incrementDamage;
        return baseDamage;
    }

    public static float CutleryGetHarvestMultiplyByEXP(int exp)
    {
        float baseMultiply = cutleryBaseHarvestMultiply;
        int level = CutleryGetLevelByEXP(exp);

        float incrementMultiply = cutleryIncrementHarvestMultiplyPerLevel;
        float multiply = 0.0f;
        while (level > 1)
        {
            multiply += incrementMultiply;
            level -= 1;
        }

        baseMultiply += baseMultiply *= incrementMultiply;
        return baseMultiply;
    }

    public static bool CutleryRollChanceToNotReduceDurabilityByEXP(int exp)
    {
        int level = CutleryGetLevelByEXP(exp);
        float baseChanceToNotReduce = cutleryBaseDurabilityRestoreChance;
        float chanceToNotReduce = cutleryDurabilityRestoreChancePerLevel;
        while (level > 1)
        {
            level -= 1;
            // Every {} levels reduce the durability chance multiplicator
            if (level % cutleryDurabilityRestoreEveryLevelReduceChance == 0)
                chanceToNotReduce -= cutleryDurabilityRestoreReduceChanceForEveryLevel;
            // Increasing chance
            baseChanceToNotReduce += chanceToNotReduce;
        }
        // Check the chance 
        if (baseChanceToNotReduce >= new Random().Next(0, 100)) return true;
        else return false;
    }

    #endregion

    #region axe
    public static readonly Dictionary<string, int> entityExpAxe = [];
    private static int axeEXPPerHit = 1;
    private static int axeEXPPerBreaking = 1;
    private static int axeEXPPerTreeBreakingAxe = 10;

    private static int axeEXPPerLevelBase = 10;
    private static double axeEXPMultiplyPerLevel = 1.8;
    private static float axeBaseDamage = 1.0f;
    private static float axeIncrementDamagePerLevel = 0.1f;
    private static float axeBaseMiningSpeed = 1.0f;
    private static float axeIncrementMiningSpeedMultiplyPerLevel = 0.1f;
    private static float axeBaseDurabilityRestoreChance = 0.0f;
    private static float axeDurabilityRestoreChancePerLevel = 2.0f;
    private static int axeDurabilityRestoreEveryLevelReduceChance = 10;
    private static float axeDurabilityRestoreReduceChanceForEveryLevel = 0.5f;


    public static int ExpPerHitAxe => axeEXPPerHit;
    public static int ExpPerBreakingAxe => axeEXPPerBreaking;
    public static int ExpPerTreeBreakingAxe => axeEXPPerTreeBreakingAxe;

    public static void PopulateAxeConfiguration(ICoreAPI api)
    {
        Dictionary<string, object> axeLevelStats = api.Assets.Get(new AssetLocation("levelup:config/levelstats/axe.json")).ToObject<Dictionary<string, object>>();
        { //axeEXPPerLevelBase
            if (axeLevelStats.TryGetValue("axeEXPPerLevelBase", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: axeEXPPerLevelBase is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: axeEXPPerLevelBase is not int is {value.GetType()}");
                else axeEXPPerLevelBase = (int)(long)value;
            else Debug.Log("CONFIGURATION ERROR: axeEXPPerLevelBase not set");
        }
        { //axeEXPMultiplyPerLevel
            if (axeLevelStats.TryGetValue("axeEXPMultiplyPerLevel", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: axeEXPMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: axeEXPMultiplyPerLevel is not double is {value.GetType()}");
                else axeEXPMultiplyPerLevel = (double)value;
            else Debug.Log("CONFIGURATION ERROR: axeEXPMultiplyPerLevel not set");
        }
        { //axeBaseDamage
            if (axeLevelStats.TryGetValue("axeBaseDamage", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: axeBaseDamage is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: axeBaseDamage is not double is {value.GetType()}");
                else axeBaseDamage = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: axeBaseDamage not set");
        }
        { //axeIncrementDamagePerLevel
            if (axeLevelStats.TryGetValue("axeIncrementDamagePerLevel", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: axeIncrementDamagePerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: axeIncrementDamagePerLevel is not double is {value.GetType()}");
                else axeIncrementDamagePerLevel = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: axeIncrementDamagePerLevel not set");
        }
        { //axeEXPPerHit
            if (axeLevelStats.TryGetValue("axeEXPPerHit", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: axeEXPPerHit is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: axeEXPPerHit is not int is {value.GetType()}");
                else axeEXPPerHit = (int)(long)value;
            else Debug.Log("CONFIGURATION ERROR: axeEXPPerHit not set");
        }
        { //axeEXPPerBreaking
            if (axeLevelStats.TryGetValue("axeEXPPerBreaking", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: axeEXPPerBreaking is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: axeEXPPerBreaking is not int is {value.GetType()}");
                else axeEXPPerBreaking = (int)(long)value;
            else Debug.Log("CONFIGURATION ERROR: axeEXPPerBreaking not set");
        }
        { //axeEXPPerTreeBreakingAxe
            if (axeLevelStats.TryGetValue("axeEXPPerTreeBreakingAxe", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: axeEXPPerTreeBreakingAxe is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: axeEXPPerTreeBreakingAxe is not int is {value.GetType()}");
                else axeEXPPerTreeBreakingAxe = (int)(long)value;
            else Debug.Log("CONFIGURATION ERROR: axeEXPPerTreeBreakingAxe not set");
        }
        { //axeBaseMiningSpeed
            if (axeLevelStats.TryGetValue("axeBaseMiningSpeed", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: axeBaseMiningSpeed is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: axeBaseMiningSpeed is not double is {value.GetType()}");
                else axeBaseMiningSpeed = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: axeBaseMiningSpeed not set");
        }
        { //axeIncrementMiningSpeedMultiplyPerLevel
            if (axeLevelStats.TryGetValue("axeIncrementMiningSpeedMultiplyPerLevel", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: axeIncrementMiningSpeedMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: axeIncrementMiningSpeedMultiplyPerLevel is not double is {value.GetType()}");
                else axeIncrementMiningSpeedMultiplyPerLevel = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: axeIncrementMiningSpeedMultiplyPerLevel not set");
        }
        { //axeBaseDurabilityRestoreChance
            if (axeLevelStats.TryGetValue("axeBaseDurabilityRestoreChance", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: axeBaseDurabilityRestoreChance is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: axeBaseDurabilityRestoreChance is not double is {value.GetType()}");
                else axeBaseDurabilityRestoreChance = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: axeBaseDurabilityRestoreChance not set");
        }
        { //axeDurabilityRestoreChancePerLevel
            if (axeLevelStats.TryGetValue("axeDurabilityRestoreChancePerLevel", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: axeDurabilityRestoreChancePerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: axeDurabilityRestoreChancePerLevel is not double is {value.GetType()}");
                else axeDurabilityRestoreChancePerLevel = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: axeDurabilityRestoreChancePerLevel not set");
        }
        { //axeDurabilityRestoreEveryLevelReduceChance
            if (axeLevelStats.TryGetValue("axeDurabilityRestoreEveryLevelReduceChance", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: axeDurabilityRestoreEveryLevelReduceChance is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: axeDurabilityRestoreEveryLevelReduceChance is not int is {value.GetType()}");
                else axeDurabilityRestoreEveryLevelReduceChance = (int)(long)value;
            else Debug.Log("CONFIGURATION ERROR: axeDurabilityRestoreEveryLevelReduceChance not set");
        }
        { //axeDurabilityRestoreReduceChanceForEveryLevel
            if (axeLevelStats.TryGetValue("axeDurabilityRestoreReduceChanceForEveryLevel", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: axeDurabilityRestoreReduceChanceForEveryLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: axeDurabilityRestoreReduceChanceForEveryLevel is not double is {value.GetType()}");
                else axeDurabilityRestoreReduceChanceForEveryLevel = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: axeDurabilityRestoreReduceChanceForEveryLevel not set");
        }

        // Get entity exp
        Dictionary<string, object> tmpentityExpAxe = api.Assets.Get(new AssetLocation("levelup:config/entityexp/axe.json")).ToObject<Dictionary<string, object>>();
        foreach (KeyValuePair<string, object> pair in tmpentityExpAxe)
        {
            if (pair.Value is long value) entityExpAxe.Add(pair.Key, (int)value);
            else Debug.Log($"CONFIGURATION ERROR: entityExpAxe {pair.Key} is not int");
        }

        Debug.Log("Axe configuration set");
    }

    public static int AxeGetLevelByEXP(int exp)
    {

        int level = 0;
        // Exp base for level
        double expPerLevelBase = axeEXPPerLevelBase;
        double calcExp = double.Parse(exp.ToString());
        while (calcExp > 0)
        {
            level += 1;
            calcExp -= expPerLevelBase;
            // 10 percentage increasing per level
            expPerLevelBase *= axeEXPMultiplyPerLevel;
        }
        return level;
    }

    public static float AxeGetDamageMultiplyByEXP(int exp)
    {
        float baseDamage = axeBaseDamage;
        int level = AxeGetLevelByEXP(exp);

        float incrementDamage = axeIncrementDamagePerLevel;
        float multiply = 0.0f;
        while (level > 1)
        {
            multiply += incrementDamage;
            level -= 1;
        }

        baseDamage += baseDamage *= incrementDamage;
        return baseDamage;
    }

    public static float AxeGetMiningMultiplyByEXP(int exp)
    {
        float baseSpeed = axeBaseMiningSpeed;
        int level = AxeGetLevelByEXP(exp);

        float incrementSpeed = axeIncrementMiningSpeedMultiplyPerLevel;
        float multiply = 0.0f;
        while (level > 1)
        {
            level -= 1;
            multiply += incrementSpeed;
        }

        baseSpeed += baseSpeed *= incrementSpeed;
        return baseSpeed;
    }

    public static bool AxeRollChanceToNotReduceDurabilityByEXP(int exp)
    {
        int level = AxeGetLevelByEXP(exp);
        float baseChanceToNotReduce = axeBaseDurabilityRestoreChance;
        float chanceToNotReduce = axeDurabilityRestoreChancePerLevel;
        while (level > 1)
        {
            level -= 1;
            // Every {} levels reduce the durability chance multiplicator
            if (level % axeDurabilityRestoreEveryLevelReduceChance == 0)
                chanceToNotReduce -= axeDurabilityRestoreReduceChanceForEveryLevel;
            // Increasing chance
            baseChanceToNotReduce += chanceToNotReduce;
        }
        // Check the chance 
        if (baseChanceToNotReduce >= new Random().Next(0, 100)) return true;
        else return false;
    }
    #endregion

    #region pickaxe
    public static readonly Dictionary<string, int> entityExpPickaxe = [];
    private static int pickaxeEXPPerHit = 1;
    private static int pickaxeEXPPerBreaking = 1;
    private static int pickaxeEXPPerLevelBase = 10;
    private static double pickaxeEXPMultiplyPerLevel = 2.0;
    private static float pickaxeBaseDamage = 1.0f;
    private static float pickaxeIncrementDamagePerLevel = 0.1f;
    private static float pickaxeBaseMiningSpeed = 1.0f;
    private static float pickaxeIncrementMiningSpeedMultiplyPerLevel = 0.1f;
    private static float pickaxeBaseOreMultiply = 0.5f;
    private static float pickaxeIncrementOreMultiplyPerLevel = 0.2f;
    private static float pickaxeBaseDurabilityRestoreChance = 0.0f;
    private static float pickaxeDurabilityRestoreChancePerLevel = 2.0f;
    private static int pickaxeDurabilityRestoreEveryLevelReduceChance = 10;
    private static float pickaxeDurabilityRestoreReduceChanceForEveryLevel = 0.5f;


    public static int ExpPerHitPickaxe => pickaxeEXPPerHit;
    public static int ExpPerBreakingPickaxe => pickaxeEXPPerBreaking;

    public static void PopulatePickaxeConfiguration(ICoreAPI api)
    {
        Dictionary<string, object> pickaxeLevelStats = api.Assets.Get(new AssetLocation("levelup:config/levelstats/pickaxe.json")).ToObject<Dictionary<string, object>>();
        { //pickaxeEXPPerLevelBase
            if (pickaxeLevelStats.TryGetValue("pickaxeEXPPerLevelBase", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: pickaxeEXPPerLevelBase is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: pickaxeEXPPerLevelBase is not int is {value.GetType()}");
                else pickaxeEXPPerLevelBase = (int)(long)value;
            else Debug.Log("CONFIGURATION ERROR: pickaxeEXPPerLevelBase not set");
        }
        { //pickaxeEXPMultiplyPerLevel
            if (pickaxeLevelStats.TryGetValue("pickaxeEXPMultiplyPerLevel", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: pickaxeEXPMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: pickaxeEXPMultiplyPerLevel is not double is {value.GetType()}");
                else pickaxeEXPMultiplyPerLevel = (double)value;
            else Debug.Log("CONFIGURATION ERROR: pickaxeEXPMultiplyPerLevel not set");
        }
        { //pickaxeBaseDamage
            if (pickaxeLevelStats.TryGetValue("pickaxeBaseDamage", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: pickaxeBaseDamage is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: pickaxeBaseDamage is not double is {value.GetType()}");
                else pickaxeBaseDamage = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: pickaxeBaseDamage not set");
        }
        { //pickaxeIncrementDamagePerLevel
            if (pickaxeLevelStats.TryGetValue("pickaxeIncrementDamagePerLevel", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: pickaxeIncrementDamagePerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: pickaxeIncrementDamagePerLevel is not double is {value.GetType()}");
                else pickaxeIncrementDamagePerLevel = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: pickaxeIncrementDamagePerLevel not set");
        }
        { //pickaxeEXPPerHit
            if (pickaxeLevelStats.TryGetValue("pickaxeEXPPerHit", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: pickaxeEXPPerHit is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: pickaxeEXPPerHit is not int is {value.GetType()}");
                else pickaxeEXPPerHit = (int)(long)value;
            else Debug.Log("CONFIGURATION ERROR: pickaxeEXPPerHit not set");
        }
        { //pickaxeEXPPerBreaking
            if (pickaxeLevelStats.TryGetValue("pickaxeEXPPerBreaking", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: pickaxeEXPPerBreaking is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: pickaxeEXPPerBreaking is not int is {value.GetType()}");
                else pickaxeEXPPerBreaking = (int)(long)value;
            else Debug.Log("CONFIGURATION ERROR: pickaxeEXPPerBreaking not set");
        }
        { //pickaxeBaseMiningSpeed
            if (pickaxeLevelStats.TryGetValue("pickaxeBaseMiningSpeed", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: pickaxeBaseMiningSpeed is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: pickaxeBaseMiningSpeed is not double is {value.GetType()}");
                else pickaxeBaseMiningSpeed = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: pickaxeBaseMiningSpeed not set");
        }
        { //pickaxeIncrementMiningSpeedMultiplyPerLevel
            if (pickaxeLevelStats.TryGetValue("pickaxeIncrementMiningSpeedMultiplyPerLevel", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: pickaxeIncrementMiningSpeedMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: pickaxeIncrementMiningSpeedMultiplyPerLevel is not double is {value.GetType()}");
                else pickaxeIncrementMiningSpeedMultiplyPerLevel = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: pickaxeIncrementMiningSpeedMultiplyPerLevel not set");
        }
        { //pickaxeBaseOreMultiply
            if (pickaxeLevelStats.TryGetValue("pickaxeBaseOreMultiply", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: pickaxeBaseOreMultiply is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: pickaxeBaseOreMultiply is not double is {value.GetType()}");
                else pickaxeBaseOreMultiply = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: pickaxeBaseOreMultiply not set");
        }
        { //pickaxeIncrementOreMultiplyPerLevel
            if (pickaxeLevelStats.TryGetValue("pickaxeIncrementOreMultiplyPerLevel", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: pickaxeIncrementOreMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: pickaxeIncrementOreMultiplyPerLevel is not double is {value.GetType()}");
                else pickaxeIncrementOreMultiplyPerLevel = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: pickaxeIncrementOreMultiplyPerLevel not set");
        }
        { //pickaxeBaseDurabilityRestoreChance
            if (pickaxeLevelStats.TryGetValue("pickaxeBaseDurabilityRestoreChance", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: pickaxeBaseDurabilityRestoreChance is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: pickaxeBaseDurabilityRestoreChance is not double is {value.GetType()}");
                else pickaxeBaseDurabilityRestoreChance = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: pickaxeBaseDurabilityRestoreChance not set");
        }
        { //pickaxeDurabilityRestoreChancePerLevel
            if (pickaxeLevelStats.TryGetValue("pickaxeDurabilityRestoreChancePerLevel", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: pickaxeDurabilityRestoreChancePerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: pickaxeDurabilityRestoreChancePerLevel is not double is {value.GetType()}");
                else pickaxeDurabilityRestoreChancePerLevel = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: pickaxeDurabilityRestoreChancePerLevel not set");
        }
        { //pickaxeDurabilityRestoreEveryLevelReduceChance
            if (pickaxeLevelStats.TryGetValue("pickaxeDurabilityRestoreEveryLevelReduceChance", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: pickaxeDurabilityRestoreEveryLevelReduceChance is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: pickaxeDurabilityRestoreEveryLevelReduceChance is not int is {value.GetType()}");
                else pickaxeDurabilityRestoreEveryLevelReduceChance = (int)(long)value;
            else Debug.Log("CONFIGURATION ERROR: pickaxeDurabilityRestoreEveryLevelReduceChance not set");
        }
        { //pickaxeDurabilityRestoreReduceChanceForEveryLevel
            if (pickaxeLevelStats.TryGetValue("pickaxeDurabilityRestoreReduceChanceForEveryLevel", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: pickaxeDurabilityRestoreReduceChanceForEveryLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: pickaxeDurabilityRestoreReduceChanceForEveryLevel is not double is {value.GetType()}");
                else pickaxeDurabilityRestoreReduceChanceForEveryLevel = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: pickaxeDurabilityRestoreReduceChanceForEveryLevel not set");
        }

        // Get entity exp
        Dictionary<string, object> tmpentityExpPickaxe = api.Assets.Get(new AssetLocation("levelup:config/entityexp/pickaxe.json")).ToObject<Dictionary<string, object>>();
        foreach (KeyValuePair<string, object> pair in tmpentityExpPickaxe)
        {
            if (pair.Value is long value) entityExpPickaxe.Add(pair.Key, (int)value);
            else Debug.Log($"CONFIGURATION ERROR: entityExpPickaxe {pair.Key} is not int");
        }

        Debug.Log("Pickaxe configuration set");
    }

    public static int PickaxeGetLevelByEXP(int exp)
    {

        int level = 0;
        // Exp base for level
        double expPerLevelBase = pickaxeEXPPerLevelBase;
        double calcExp = double.Parse(exp.ToString());
        while (calcExp > 0)
        {
            level += 1;
            calcExp -= expPerLevelBase;
            // 10 percentage increasing per level
            expPerLevelBase *= pickaxeEXPMultiplyPerLevel;
        }
        return level;
    }

    public static float PickaxeGetOreMultiplyByEXP(int exp)
    {
        float baseMultiply = pickaxeBaseOreMultiply;
        int level = PickaxeGetLevelByEXP(exp);

        float incrementMultiply = pickaxeIncrementOreMultiplyPerLevel;
        float multiply = 0.0f;
        while (level > 1)
        {
            multiply += incrementMultiply;
            level -= 1;
        }

        baseMultiply += baseMultiply *= incrementMultiply;
        return baseMultiply;
    }

    public static float PickaxeGetDamageMultiplyByEXP(int exp)
    {
        float baseDamage = pickaxeBaseDamage;
        int level = PickaxeGetLevelByEXP(exp);

        float incrementDamage = pickaxeIncrementDamagePerLevel;
        float multiply = 0.0f;
        while (level > 1)
        {
            multiply += incrementDamage;
            level -= 1;
        }

        baseDamage += baseDamage *= incrementDamage;
        return baseDamage;
    }

    public static float PickaxeGetMiningMultiplyByEXP(int exp)
    {
        float baseSpeed = pickaxeBaseMiningSpeed;
        int level = PickaxeGetLevelByEXP(exp);

        float incrementSpeed = pickaxeIncrementMiningSpeedMultiplyPerLevel;
        float multiply = 0.0f;
        while (level > 1)
        {
            level -= 1;
            multiply += incrementSpeed;
        }

        baseSpeed += baseSpeed *= incrementSpeed;
        return baseSpeed;
    }

    public static bool PickaxeRollChanceToNotReduceDurabilityByEXP(int exp)
    {
        int level = PickaxeGetLevelByEXP(exp);
        float baseChanceToNotReduce = pickaxeBaseDurabilityRestoreChance;
        float chanceToNotReduce = pickaxeDurabilityRestoreChancePerLevel;
        while (level > 1)
        {
            level -= 1;
            // Every {} levels reduce the durability chance multiplicator
            if (level % pickaxeDurabilityRestoreEveryLevelReduceChance == 0)
                chanceToNotReduce -= pickaxeDurabilityRestoreReduceChanceForEveryLevel;
            // Increasing chance
            baseChanceToNotReduce += chanceToNotReduce;
        }
        // Check the chance 
        if (baseChanceToNotReduce >= new Random().Next(0, 100)) return true;
        else return false;
    }
    #endregion

    #region shovel
    public static readonly Dictionary<string, int> entityExpShovel = [];
    private static int shovelEXPPerHit = 1;
    private static int shovelEXPPerBreaking = 1;
    private static int shovelEXPPerLevelBase = 10;
    private static double shovelEXPMultiplyPerLevel = 2.0;
    private static float shovelBaseDamage = 1.0f;
    private static float shovelIncrementDamagePerLevel = 0.1f;
    private static float shovelBaseMiningSpeed = 1.0f;
    private static float shovelIncrementMiningSpeedMultiplyPerLevel = 0.1f;
    private static float shovelBaseDurabilityRestoreChance = 0.0f;
    private static float shovelDurabilityRestoreChancePerLevel = 2.0f;
    private static int shovelDurabilityRestoreEveryLevelReduceChance = 10;
    private static float shovelDurabilityRestoreReduceChanceForEveryLevel = 0.5f;


    public static int ExpPerHitShovel => shovelEXPPerHit;
    public static int ExpPerBreakingShovel => shovelEXPPerBreaking;

    public static void PopulateShovelConfiguration(ICoreAPI api)
    {
        Dictionary<string, object> shovelLevelStats = api.Assets.Get(new AssetLocation("levelup:config/levelstats/shovel.json")).ToObject<Dictionary<string, object>>();
        { //shovelEXPPerLevelBase
            if (shovelLevelStats.TryGetValue("shovelEXPPerLevelBase", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: shovelEXPPerLevelBase is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: shovelEXPPerLevelBase is not int is {value.GetType()}");
                else shovelEXPPerLevelBase = (int)(long)value;
            else Debug.Log("CONFIGURATION ERROR: shovelEXPPerLevelBase not set");
        }
        { //shovelEXPMultiplyPerLevel
            if (shovelLevelStats.TryGetValue("shovelEXPMultiplyPerLevel", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: shovelEXPMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: shovelEXPMultiplyPerLevel is not double is {value.GetType()}");
                else shovelEXPMultiplyPerLevel = (double)value;
            else Debug.Log("CONFIGURATION ERROR: shovelEXPMultiplyPerLevel not set");
        }
        { //shovelBaseDamage
            if (shovelLevelStats.TryGetValue("shovelBaseDamage", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: shovelBaseDamage is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: shovelBaseDamage is not double is {value.GetType()}");
                else shovelBaseDamage = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: shovelBaseDamage not set");
        }
        { //shovelIncrementDamagePerLevel
            if (shovelLevelStats.TryGetValue("shovelIncrementDamagePerLevel", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: shovelIncrementDamagePerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: shovelIncrementDamagePerLevel is not double is {value.GetType()}");
                else shovelIncrementDamagePerLevel = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: shovelIncrementDamagePerLevel not set");
        }
        { //shovelEXPPerHit
            if (shovelLevelStats.TryGetValue("shovelEXPPerHit", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: shovelEXPPerHit is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: shovelEXPPerHit is not int is {value.GetType()}");
                else shovelEXPPerHit = (int)(long)value;
            else Debug.Log("CONFIGURATION ERROR: shovelEXPPerHit not set");
        }
        { //shovelEXPPerBreaking
            if (shovelLevelStats.TryGetValue("shovelEXPPerBreaking", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: shovelEXPPerBreaking is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: shovelEXPPerBreaking is not int is {value.GetType()}");
                else shovelEXPPerBreaking = (int)(long)value;
            else Debug.Log("CONFIGURATION ERROR: shovelEXPPerBreaking not set");
        }
        { //shovelBaseMiningSpeed
            if (shovelLevelStats.TryGetValue("shovelBaseMiningSpeed", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: shovelBaseMiningSpeed is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: shovelBaseMiningSpeed is not double is {value.GetType()}");
                else shovelBaseMiningSpeed = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: shovelBaseMiningSpeed not set");
        }
        { //shovelIncrementMiningSpeedMultiplyPerLevel
            if (shovelLevelStats.TryGetValue("shovelIncrementMiningSpeedMultiplyPerLevel", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: shovelIncrementMiningSpeedMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: shovelIncrementMiningSpeedMultiplyPerLevel is not double is {value.GetType()}");
                else shovelIncrementMiningSpeedMultiplyPerLevel = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: shovelIncrementMiningSpeedMultiplyPerLevel not set");
        }
        { //shovelBaseDurabilityRestoreChance
            if (shovelLevelStats.TryGetValue("shovelBaseDurabilityRestoreChance", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: shovelBaseDurabilityRestoreChance is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: shovelBaseDurabilityRestoreChance is not double is {value.GetType()}");
                else shovelBaseDurabilityRestoreChance = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: shovelBaseDurabilityRestoreChance not set");
        }
        { //shovelDurabilityRestoreChancePerLevel
            if (shovelLevelStats.TryGetValue("shovelDurabilityRestoreChancePerLevel", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: shovelDurabilityRestoreChancePerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: shovelDurabilityRestoreChancePerLevel is not double is {value.GetType()}");
                else shovelDurabilityRestoreChancePerLevel = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: shovelDurabilityRestoreChancePerLevel not set");
        }
        { //shovelDurabilityRestoreEveryLevelReduceChance
            if (shovelLevelStats.TryGetValue("shovelDurabilityRestoreEveryLevelReduceChance", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: shovelDurabilityRestoreEveryLevelReduceChance is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: shovelDurabilityRestoreEveryLevelReduceChance is not int is {value.GetType()}");
                else shovelDurabilityRestoreEveryLevelReduceChance = (int)(long)value;
            else Debug.Log("CONFIGURATION ERROR: shovelDurabilityRestoreEveryLevelReduceChance not set");
        }
        { //shovelDurabilityRestoreReduceChanceForEveryLevel
            if (shovelLevelStats.TryGetValue("shovelDurabilityRestoreReduceChanceForEveryLevel", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: shovelDurabilityRestoreReduceChanceForEveryLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: shovelDurabilityRestoreReduceChanceForEveryLevel is not double is {value.GetType()}");
                else shovelDurabilityRestoreReduceChanceForEveryLevel = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: shovelDurabilityRestoreReduceChanceForEveryLevel not set");
        }
        // Get entity exp
        Dictionary<string, object> tmpentityExpShovel = api.Assets.Get(new AssetLocation("levelup:config/entityexp/shovel.json")).ToObject<Dictionary<string, object>>();
        foreach (KeyValuePair<string, object> pair in tmpentityExpShovel)
        {
            if (pair.Value is long value) entityExpShovel.Add(pair.Key, (int)value);
            else Debug.Log($"CONFIGURATION ERROR: entityExpShovel {pair.Key} is not int");
        }

        Debug.Log("Shovel configuration set");
    }

    public static int ShovelGetLevelByEXP(int exp)
    {

        int level = 0;
        // Exp base for level
        double expPerLevelBase = shovelEXPPerLevelBase;
        double calcExp = double.Parse(exp.ToString());
        while (calcExp > 0)
        {
            level += 1;
            calcExp -= expPerLevelBase;
            // 10 percentage increasing per level
            expPerLevelBase *= shovelEXPMultiplyPerLevel;
        }
        return level;
    }

    public static float ShovelGetDamageMultiplyByEXP(int exp)
    {
        float baseDamage = shovelBaseDamage;
        int level = ShovelGetLevelByEXP(exp);

        float incrementDamage = shovelIncrementDamagePerLevel;
        float multiply = 0.0f;
        while (level > 1)
        {
            multiply += incrementDamage;
            level -= 1;
        }

        baseDamage += baseDamage *= incrementDamage;
        return baseDamage;
    }

    public static float ShovelGetMiningMultiplyByEXP(int exp)
    {
        float baseSpeed = shovelBaseMiningSpeed;
        int level = ShovelGetLevelByEXP(exp);

        float incrementSpeed = shovelIncrementMiningSpeedMultiplyPerLevel;
        float multiply = 0.0f;
        while (level > 1)
        {
            level -= 1;
            multiply += incrementSpeed;
        }

        baseSpeed += baseSpeed *= incrementSpeed;
        return baseSpeed;
    }

    public static bool ShovelRollChanceToNotReduceDurabilityByEXP(int exp)
    {
        int level = ShovelGetLevelByEXP(exp);
        float baseChanceToNotReduce = shovelBaseDurabilityRestoreChance;
        float chanceToNotReduce = shovelDurabilityRestoreChancePerLevel;
        while (level > 1)
        {
            level -= 1;
            // Every {} levels reduce the durability chance multiplicator
            if (level % shovelDurabilityRestoreEveryLevelReduceChance == 0)
                chanceToNotReduce -= shovelDurabilityRestoreReduceChanceForEveryLevel;
            // Increasing chance
            baseChanceToNotReduce += chanceToNotReduce;
        }
        // Check the chance 
        if (baseChanceToNotReduce >= new Random().Next(0, 100)) return true;
        else return false;
    }
    #endregion

    #region spear
    public static readonly Dictionary<string, int> entityExpSpear = [];
    private static int spearEXPPerHit = 1;
    private static int spearEXPPerThrow = 2;
    private static int spearEXPPerLevelBase = 10;
    private static double spearEXPMultiplyPerLevel = 1.5;
    private static float spearBaseDamage = 1.0f;
    private static float spearIncrementDamagePerLevel = 0.1f;
    private static float spearBaseDurabilityRestoreChance = 0.0f;
    private static float spearDurabilityRestoreChancePerLevel = 2.0f;
    private static int spearDurabilityRestoreEveryLevelReduceChance = 10;
    private static float spearDurabilityRestoreReduceChanceForEveryLevel = 0.5f;


    public static int ExpPerHitSpear => spearEXPPerHit;
    public static int ExpPerThrowSpear => spearEXPPerThrow;

    public static void PopulateSpearConfiguration(ICoreAPI api)
    {
        Dictionary<string, object> spearLevelStats = api.Assets.Get(new AssetLocation("levelup:config/levelstats/spear.json")).ToObject<Dictionary<string, object>>();
        { //spearEXPPerLevelBase
            if (spearLevelStats.TryGetValue("spearEXPPerLevelBase", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: spearEXPPerLevelBase is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: spearEXPPerLevelBase is not int is {value.GetType()}");
                else spearEXPPerLevelBase = (int)(long)value;
            else Debug.Log("CONFIGURATION ERROR: spearEXPPerLevelBase not set");
        }
        { //spearEXPMultiplyPerLevel
            if (spearLevelStats.TryGetValue("spearEXPMultiplyPerLevel", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: spearEXPMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: spearEXPMultiplyPerLevel is not double is {value.GetType()}");
                else spearEXPMultiplyPerLevel = (double)value;
            else Debug.Log("CONFIGURATION ERROR: spearEXPMultiplyPerLevel not set");
        }
        { //spearBaseDamage
            if (spearLevelStats.TryGetValue("spearBaseDamage", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: spearBaseDamage is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: spearBaseDamage is not double is {value.GetType()}");
                else spearBaseDamage = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: spearBaseDamage not set");
        }
        { //spearIncrementDamagePerLevel
            if (spearLevelStats.TryGetValue("spearIncrementDamagePerLevel", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: spearIncrementDamagePerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: spearIncrementDamagePerLevel is not double is {value.GetType()}");
                else spearIncrementDamagePerLevel = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: spearIncrementDamagePerLevel not set");
        }
        { //spearEXPPerHit
            if (spearLevelStats.TryGetValue("spearEXPPerHit", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: spearEXPPerHit is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: spearEXPPerHit is not int is {value.GetType()}");
                else spearEXPPerHit = (int)(long)value;
            else Debug.Log("CONFIGURATION ERROR: spearEXPPerHit not set");
        }
        { //spearEXPPerThrow
            if (spearLevelStats.TryGetValue("spearEXPPerThrow", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: spearEXPPerThrow is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: spearEXPPerThrow is not int is {value.GetType()}");
                else spearEXPPerThrow = (int)(long)value;
            else Debug.Log("CONFIGURATION ERROR: spearEXPPerThrow not set");
        }
        { //spearBaseDurabilityRestoreChance
            if (spearLevelStats.TryGetValue("spearBaseDurabilityRestoreChance", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: spearBaseDurabilityRestoreChance is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: spearBaseDurabilityRestoreChance is not double is {value.GetType()}");
                else spearBaseDurabilityRestoreChance = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: spearBaseDurabilityRestoreChance not set");
        }
        { //spearDurabilityRestoreChancePerLevel
            if (spearLevelStats.TryGetValue("spearDurabilityRestoreChancePerLevel", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: spearDurabilityRestoreChancePerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: spearDurabilityRestoreChancePerLevel is not double is {value.GetType()}");
                else spearDurabilityRestoreChancePerLevel = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: spearDurabilityRestoreChancePerLevel not set");
        }
        { //spearDurabilityRestoreEveryLevelReduceChance
            if (spearLevelStats.TryGetValue("spearDurabilityRestoreEveryLevelReduceChance", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: spearDurabilityRestoreEveryLevelReduceChance is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: spearDurabilityRestoreEveryLevelReduceChance is not int is {value.GetType()}");
                else spearDurabilityRestoreEveryLevelReduceChance = (int)(long)value;
            else Debug.Log("CONFIGURATION ERROR: spearDurabilityRestoreEveryLevelReduceChance not set");
        }
        { //spearDurabilityRestoreReduceChanceForEveryLevel
            if (spearLevelStats.TryGetValue("spearDurabilityRestoreReduceChanceForEveryLevel", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: spearDurabilityRestoreReduceChanceForEveryLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: spearDurabilityRestoreReduceChanceForEveryLevel is not double is {value.GetType()}");
                else spearDurabilityRestoreReduceChanceForEveryLevel = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: spearDurabilityRestoreReduceChanceForEveryLevel not set");
        }

        // Get entity exp
        Dictionary<string, object> tmpentityExpSpear = api.Assets.Get(new AssetLocation("levelup:config/entityexp/spear.json")).ToObject<Dictionary<string, object>>();
        foreach (KeyValuePair<string, object> pair in tmpentityExpSpear)
        {
            if (pair.Value is long value) entityExpSpear.Add(pair.Key, (int)value);
            else Debug.Log($"CONFIGURATION ERROR: entityExpSpear {pair.Key} is not int");
        }
        Debug.Log("Spear configuration set");
    }

    public static int SpearGetLevelByEXP(int exp)
    {
        int level = 0;
        // Exp base for level
        double expPerLevelBase = spearEXPPerLevelBase;
        double calcExp = double.Parse(exp.ToString());
        while (calcExp > 0)
        {
            level += 1;
            calcExp -= expPerLevelBase;
            // 10 percentage increasing per level
            expPerLevelBase *= spearEXPMultiplyPerLevel;
        }
        return level;
    }

    public static float SpearGetDamageMultiplyByEXP(int exp)
    {
        float baseDamage = spearBaseDamage;
        int level = SpearGetLevelByEXP(exp);

        float incrementDamage = spearIncrementDamagePerLevel;
        float multiply = 0.0f;
        while (level > 1)
        {
            multiply += incrementDamage;
            level -= 1;
        }

        baseDamage += baseDamage *= incrementDamage;
        return baseDamage;
    }

    public static bool SpearRollChanceToNotReduceDurabilityByEXP(int exp)
    {
        int level = SpearGetLevelByEXP(exp);
        float baseChanceToNotReduce = spearBaseDurabilityRestoreChance;
        float chanceToNotReduce = spearDurabilityRestoreChancePerLevel;
        while (level > 1)
        {
            level -= 1;
            // Every {} levels reduce the durability chance multiplicator
            if (level % spearDurabilityRestoreEveryLevelReduceChance == 0)
                chanceToNotReduce -= spearDurabilityRestoreReduceChanceForEveryLevel;
            // Increasing chance
            baseChanceToNotReduce += chanceToNotReduce;
        }
        // Check the chance 
        if (baseChanceToNotReduce >= new Random().Next(0, 100)) return true;
        else return false;
    }
    #endregion

    #region farming
    public static readonly Dictionary<string, int> expPerHarvestFarming = [];
    private static int farmingEXPPerTill = 1;
    private static int farmingEXPPerLevelBase = 10;
    private static double farmingEXPMultiplyPerLevel = 2.5;
    private static float farmingBaseHarvestMultiply = 0.5f;
    private static float farmingIncrementHarvestMultiplyPerLevel = 0.2f;
    private static float farmingBaseDurabilityRestoreChance = 0.0f;
    private static float farmingDurabilityRestoreChancePerLevel = 2.0f;
    private static int farmingDurabilityRestoreEveryLevelReduceChance = 10;
    private static float farmingDurabilityRestoreReduceChanceForEveryLevel = 0.5f;

    public static int ExpPerTillFarming => farmingEXPPerTill;

    public static void PopulateFarmingConfiguration(ICoreAPI api)
    {
        Dictionary<string, object> farmingLevelStats = api.Assets.Get(new AssetLocation("levelup:config/levelstats/farming.json")).ToObject<Dictionary<string, object>>();
        { //farmingEXPPerLevelBase
            if (farmingLevelStats.TryGetValue("farmingEXPPerLevelBase", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: farmingEXPPerLevelBase is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: farmingEXPPerLevelBase is not int is {value.GetType()}");
                else farmingEXPPerLevelBase = (int)(long)value;
            else Debug.Log("CONFIGURATION ERROR: farmingEXPPerLevelBase not set");
        }
        { //farmingEXPMultiplyPerLevel
            if (farmingLevelStats.TryGetValue("farmingEXPMultiplyPerLevel", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: farmingEXPMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: farmingEXPMultiplyPerLevel is not double is {value.GetType()}");
                else farmingEXPMultiplyPerLevel = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: farmingEXPMultiplyPerLevel not set");
        }
        { //farmingEXPPerTill
            if (farmingLevelStats.TryGetValue("farmingEXPPerTill", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: farmingEXPPerTill is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: farmingEXPPerTill is not int is {value.GetType()}");
                else farmingEXPPerTill = (int)(long)value;
            else Debug.Log("CONFIGURATION ERROR: farmingEXPPerTill not set");
        }
        { //farmingBaseHarvestMultiply
            if (farmingLevelStats.TryGetValue("farmingBaseHarvestMultiply", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: farmingBaseHarvestMultiply is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: farmingBaseHarvestMultiply is not double is {value.GetType()}");
                else farmingBaseHarvestMultiply = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: farmingBaseHarvestMultiply not set");
        }
        { //farmingIncrementHarvestMultiplyPerLevel
            if (farmingLevelStats.TryGetValue("farmingIncrementHarvestMultiplyPerLevel", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: farmingIncrementHarvestMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: farmingIncrementHarvestMultiplyPerLevel is not double is {value.GetType()}");
                else farmingIncrementHarvestMultiplyPerLevel = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: farmingIncrementHarvestMultiplyPerLevel not set");
        }
        { //farmingBaseDurabilityRestoreChance
            if (farmingLevelStats.TryGetValue("farmingBaseDurabilityRestoreChance", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: farmingBaseDurabilityRestoreChance is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: farmingBaseDurabilityRestoreChance is not double is {value.GetType()}");
                else farmingBaseDurabilityRestoreChance = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: farmingBaseDurabilityRestoreChance not set");
        }
        { //farmingDurabilityRestoreChancePerLevel
            if (farmingLevelStats.TryGetValue("farmingDurabilityRestoreChancePerLevel", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: farmingDurabilityRestoreChancePerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: farmingDurabilityRestoreChancePerLevel is not double is {value.GetType()}");
                else farmingDurabilityRestoreChancePerLevel = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: farmingDurabilityRestoreChancePerLevel not set");
        }
        { //farmingDurabilityRestoreEveryLevelReduceChance
            if (farmingLevelStats.TryGetValue("farmingDurabilityRestoreEveryLevelReduceChance", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: farmingDurabilityRestoreEveryLevelReduceChance is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: farmingDurabilityRestoreEveryLevelReduceChance is not int is {value.GetType()}");
                else farmingDurabilityRestoreEveryLevelReduceChance = (int)(long)value;
            else Debug.Log("CONFIGURATION ERROR: farmingDurabilityRestoreEveryLevelReduceChance not set");
        }
        { //farmingDurabilityRestoreReduceChanceForEveryLevel
            if (farmingLevelStats.TryGetValue("farmingDurabilityRestoreReduceChanceForEveryLevel", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: farmingDurabilityRestoreReduceChanceForEveryLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: farmingDurabilityRestoreReduceChanceForEveryLevel is not double is {value.GetType()}");
                else farmingDurabilityRestoreReduceChanceForEveryLevel = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: farmingDurabilityRestoreReduceChanceForEveryLevel not set");
        }


        // Get crop exp
        Dictionary<string, object> tmpexpPerHarvestFarming = api.Assets.Get(new AssetLocation("levelup:config/levelstats/farmingcrops.json")).ToObject<Dictionary<string, object>>();
        foreach (KeyValuePair<string, object> pair in tmpexpPerHarvestFarming)
        {
            if (pair.Value is long value) expPerHarvestFarming.Add(pair.Key, (int)value);
            else Debug.Log($"CONFIGURATION ERROR: expPerHarvestFarming {pair.Key} is not int");
        }

        Debug.Log("Farming configuration set");
    }

    public static int FarmingGetLevelByEXP(int exp)
    {
        int level = 0;
        // Exp base for level
        double expPerLevelBase = farmingEXPPerLevelBase;
        double calcExp = double.Parse(exp.ToString());
        while (calcExp > 0)
        {
            level += 1;
            calcExp -= expPerLevelBase;
            // 10 percentage increasing per level
            expPerLevelBase *= farmingEXPMultiplyPerLevel;
        }
        return level;
    }

    public static float FarmingGetHarvestMultiplyByEXP(int exp)
    {
        float baseMultiply = farmingBaseHarvestMultiply;
        int level = FarmingGetLevelByEXP(exp);

        float incrementMultiply = farmingIncrementHarvestMultiplyPerLevel;
        float multiply = 0.0f;
        while (level > 1)
        {
            multiply += incrementMultiply;
            level -= 1;
        }

        baseMultiply += baseMultiply *= incrementMultiply;
        return baseMultiply;
    }

    public static bool FarmingRollChanceToNotReduceDurabilityByEXP(int exp)
    {
        int level = FarmingGetLevelByEXP(exp);
        float baseChanceToNotReduce = farmingBaseDurabilityRestoreChance;
        float chanceToNotReduce = farmingDurabilityRestoreChancePerLevel;
        while (level > 1)
        {
            level -= 1;
            // Every {} levels reduce the durability chance multiplicator
            if (level % farmingDurabilityRestoreEveryLevelReduceChance == 0)
                chanceToNotReduce -= farmingDurabilityRestoreReduceChanceForEveryLevel;
            // Increasing chance
            baseChanceToNotReduce += chanceToNotReduce;
        }
        // Check the chance 
        if (baseChanceToNotReduce >= new Random().Next(0, 100)) return true;
        else return false;
    }
    #endregion

    #region cooking
    public static readonly int expPerCookedCooking = 3;

    public static void PopulateCookingConfiguration(ICoreAPI _)
    {
        Debug.Log("Cooking configuration set");
    }

    public static int CookingGetLevelByEXP(int exp)
    {
        int level = 0;
        // Exp base for level
        double expPerLevelBase = 10;
        double calcExp = double.Parse(exp.ToString());
        while (calcExp > 0)
        {
            level += 1;
            calcExp -= expPerLevelBase;
            // 20 percentage increasing per level
            expPerLevelBase *= 1.2;
        }
        return level;
    }

    public static float CookingGetSaturationMultiplyByEXP(int exp)
    {
        float baseMultiply = 1.0f;
        int level = CookingGetLevelByEXP(exp);

        float incrementMultiply = 0.1f;
        float multiply = 0.0f;
        while (level > 1)
        {
            multiply += incrementMultiply;
            level -= 1;
        }

        baseMultiply += baseMultiply *= incrementMultiply;
        return baseMultiply;
    }

    public static float CookingGetFreshHoursMultiplyByEXP(int exp)
    {
        float baseMultiply = 1.0f;
        int level = CookingGetLevelByEXP(exp);

        float incrementMultiply = 0.1f;
        float multiply = 0.0f;
        while (level > 1)
        {
            multiply += incrementMultiply;
            level -= 1;
        }

        baseMultiply += baseMultiply *= incrementMultiply;
        return baseMultiply;
    }

    public static int CookingGetServingsByEXPAndServings(int exp, int quantityServings)
    {
        int level = CookingGetLevelByEXP(exp);
        float chanceToIncrease = 0.0f;
        int rolls = 1;
        while (level > 1)
        {
            level -= 1;
            if (chanceToIncrease < 20f)
                // 2% of chance each level
                chanceToIncrease += 2.0f;
            else if (chanceToIncrease < 40f)
                // 1% of chance each level
                chanceToIncrease += 1.0f;
            else if (chanceToIncrease < 60f)
                // 0.5% of chance each level
                chanceToIncrease += 0.5f;
            else
                // 0.2% of chance each level
                chanceToIncrease += 0.2f;
            // Increase rolls change by 1 every 5 levels
            if (level % 5 == 0) rolls += 1;
        }
        while (rolls > 0)
        {
            // Randomizes the chance and increase if chances hit
            if (chanceToIncrease >= new Random().Next(0, 100)) quantityServings += 1;
        }
        return quantityServings;
    }

    public static bool CookingRollChanceToNotReduceDurabilityByEXP(int exp)
    {
        int level = CookingGetLevelByEXP(exp);
        float chanceToNotReduce = 0.0f;
        while (level > 1)
        {
            level -= 1;
            if (chanceToNotReduce < 20f)
                // 2% of chance each level
                chanceToNotReduce += 2.0f;
            else if (chanceToNotReduce < 40f)
                // 1% of chance each level
                chanceToNotReduce += 1.0f;
            else if (chanceToNotReduce < 60f)
                // 0.5% of chance each level
                chanceToNotReduce += 0.5f;
            else
                // 0.2% of chance each level
                chanceToNotReduce += 0.2f;
        }
        // Check the chance 
        if (chanceToNotReduce >= new Random().Next(0, 100)) return true;
        else return false;
    }
    #endregion
}