using System;
using System.Collections.Generic;
using Vintagestory.API.Util;

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

    #region hunter
    public static readonly Dictionary<string, int> entityExpHunter = [];

    public static void PopulateHunterConfiguration()
    {
        entityExpHunter.AddRange(Initialization.DefaultConfigHunter);
        Debug.Log("Hunter configuration set");
    }

    public static int HunterGetLevelByEXP(int exp)
    {

        int level = 0;
        // Exp base for level
        double expPerLevelBase = 10;
        double calcExp = double.Parse(exp.ToString());
        while (calcExp > 0)
        {
            level += 1;
            calcExp -= expPerLevelBase;
            // 10 percentage increasing per level
            expPerLevelBase *= 1.5;
        }
        return level;
    }

    public static float HunterGetDamageMultiplyByEXP(int exp)
    {
        float baseDamage = 1.0f;
        int level = HunterGetLevelByEXP(exp);

        float incrementDamage = 0.1f;
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
    public static readonly int expPerHitBow = 1;

    public static void PopulateBowConfiguration()
    {
        entityExpBow.AddRange(Initialization.DefaultConfigBow);
        Debug.Log("Bow configuration set");
    }

    public static int BowGetLevelByEXP(int exp)
    {

        int level = 0;
        // Exp base for level
        double expPerLevelBase = 10;
        double calcExp = double.Parse(exp.ToString());
        while (calcExp > 0)
        {
            level += 1;
            calcExp -= expPerLevelBase;
            // 10 percentage increasing per level
            expPerLevelBase *= 1.3;
        }
        return level;
    }

    public static float BowGetDamageMultiplyByEXP(int exp)
    {
        float baseDamage = 1.0f;
        int level = BowGetLevelByEXP(exp);

        float incrementDamage = 0.1f;
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

    #region cutlery
    public static readonly Dictionary<string, int> entityExpCutlery = [];
    public static readonly int expPerHitCutlery = 1;
    public static readonly int expPerHarvestCutlery = 5;

    public static void PopulateCutleryConfiguration()
    {
        entityExpCutlery.AddRange(Initialization.DefaultConfigCutlery);
        Debug.Log("Cutlery configuration set");
    }

    public static int CutleryGetLevelByEXP(int exp)
    {

        int level = 0;
        // Exp base for level
        double expPerLevelBase = 10;
        double calcExp = double.Parse(exp.ToString());
        while (calcExp > 0)
        {
            level += 1;
            calcExp -= expPerLevelBase;
            // 10 percentage increasing per level
            expPerLevelBase *= 1.2;
        }
        return level;
    }

    public static float CutleryGetDamageMultiplyByEXP(int exp)
    {
        float baseDamage = 1.0f;
        int level = CutleryGetLevelByEXP(exp);

        float incrementDamage = 0.1f;
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
        float baseMultiply = 0.5f;
        int level = CutleryGetLevelByEXP(exp);

        float incrementMultiply = 0.2f;
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

    #region axe
    public static readonly Dictionary<string, int> entityExpAxe = [];
    public static readonly int expPerHitAxe = 1;
    public static readonly int expPerBreakingAxe = 1;
    public static readonly int expPerTreeBreakingAxe = 10;

    public static void PopulateAxeConfiguration()
    {
        entityExpAxe.AddRange(Initialization.DefaultConfigAxe);
        Debug.Log("Axe configuration set");
    }

    public static int AxeGetLevelByEXP(int exp)
    {

        int level = 0;
        // Exp base for level
        double expPerLevelBase = 10;
        double calcExp = double.Parse(exp.ToString());
        while (calcExp > 0)
        {
            level += 1;
            calcExp -= expPerLevelBase;
            // 10 percentage increasing per level
            expPerLevelBase *= 1.8;
        }
        return level;
    }

    public static float AxeGetDamageMultiplyByEXP(int exp)
    {
        float baseDamage = 1.0f;
        int level = AxeGetLevelByEXP(exp);

        float incrementDamage = 0.1f;
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
        float baseSpeed = 1.0f;
        int level = AxeGetLevelByEXP(exp);

        float incrementSpeed = 0.1f;
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

    #region pickaxe
    public static readonly Dictionary<string, int> entityExpPickaxe = [];
    public static readonly int expPerHitPickaxe = 1;
    public static readonly int expPerBreakingPickaxe = 1;

    public static void PopulatePickaxeConfiguration()
    {
        entityExpPickaxe.AddRange(Initialization.DefaultConfigPickaxe);
        Debug.Log("Pickaxe configuration set");
    }

    public static int PickaxeGetLevelByEXP(int exp)
    {

        int level = 0;
        // Exp base for level
        double expPerLevelBase = 10;
        double calcExp = double.Parse(exp.ToString());
        while (calcExp > 0)
        {
            level += 1;
            calcExp -= expPerLevelBase;
            // 10 percentage increasing per level
            expPerLevelBase *= 2.0;
        }
        return level;
    }

    public static float PickaxeGetOreMultiplyByEXP(int exp)
    {
        float baseMultiply = 0.5f;
        int level = PickaxeGetLevelByEXP(exp);

        float incrementMultiply = 0.2f;
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
        float baseDamage = 1.0f;
        int level = PickaxeGetLevelByEXP(exp);

        float incrementDamage = 0.1f;
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
        float baseSpeed = 1.0f;
        int level = PickaxeGetLevelByEXP(exp);

        float incrementSpeed = 0.1f;
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

    #region shovel
    public static readonly Dictionary<string, int> entityExpShovel = [];
    public static readonly int expPerHitShovel = 1;
    public static readonly int expPerBreakingShovel = 1;

    public static void PopulateShovelConfiguration()
    {
        entityExpShovel.AddRange(Initialization.DefaultConfigShovel);
        Debug.Log("Shovel configuration set");
    }

    public static int ShovelGetLevelByEXP(int exp)
    {

        int level = 0;
        // Exp base for level
        double expPerLevelBase = 10;
        double calcExp = double.Parse(exp.ToString());
        while (calcExp > 0)
        {
            level += 1;
            calcExp -= expPerLevelBase;
            // 10 percentage increasing per level
            expPerLevelBase *= 2.0;
        }
        return level;
    }

    public static float ShovelGetDamageMultiplyByEXP(int exp)
    {
        float baseDamage = 1.0f;
        int level = ShovelGetLevelByEXP(exp);

        float incrementDamage = 0.1f;
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
        float baseSpeed = 1.0f;
        int level = ShovelGetLevelByEXP(exp);

        float incrementSpeed = 0.1f;
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

    #region spear
    public static readonly Dictionary<string, int> entityExpSpear = [];
    public static readonly int expPerHitSpear = 1;
    public static readonly int expPerThrowSpear = 2;

    public static void PopulateSpearConfiguration()
    {

        Debug.Log("Spear configuration set");
    }

    public static int SpearGetLevelByEXP(int exp)
    {
        int level = 0;
        // Exp base for level
        double expPerLevelBase = 10;
        double calcExp = double.Parse(exp.ToString());
        while (calcExp > 0)
        {
            level += 1;
            calcExp -= expPerLevelBase;
            // 10 percentage increasing per level
            expPerLevelBase *= 1.5;
        }
        return level;
    }

    public static float SpearGetDamageMultiplyByEXP(int exp)
    {
        float baseDamage = 1.0f;
        int level = SpearGetLevelByEXP(exp);

        float incrementDamage = 0.1f;
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

    #region farming
    public static readonly Dictionary<string, int> expPerHarvestFarming = [];
    public static readonly int expPerTillFarming = 1;

    public static void PopulateFarmingConfiguration()
    {
        expPerHarvestFarming.AddRange(Initialization.DefaultConfigHarvestFarming);
        Debug.Log("Farming configuration set");
    }

    public static int FarmingGetLevelByEXP(int exp)
    {
        int level = 0;
        // Exp base for level
        double expPerLevelBase = 10;
        double calcExp = double.Parse(exp.ToString());
        while (calcExp > 0)
        {
            level += 1;
            calcExp -= expPerLevelBase;
            // 10 percentage increasing per level
            expPerLevelBase *= 2.5;
        }
        return level;
    }

    public static float FarmingGetHarvestMultiplyByEXP(int exp)
    {
        float baseMultiply = 1.0f;
        int level = FarmingGetLevelByEXP(exp);

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

    public static bool FarmingRollChanceToNotReduceDurabilityByEXP(int exp)
    {
        int level = FarmingGetLevelByEXP(exp);
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

    #region cooking
    public static readonly int expPerCookedCooking = 3;

    public static void PopulateCookingConfiguration()
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