using System.Collections.Generic;

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
            default: break;
        }
        return 1;
    }

    #region hunter
    public static readonly Dictionary<string, int> entityExpHunter = [];

    public static void PopulateHunterConfiguration()
    {
        entityExpHunter["Dead drifter"] = 5;
        entityExpHunter["Dead bear"] = 7;
        entityExpHunter["Dead rooster"] = 1;
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
            expPerLevelBase *= 1.1;
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
        entityExpBow["Dead drifter"] = 5;
        entityExpBow["Dead bear"] = 7;
        entityExpBow["Dead rooster"] = 1;
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
            expPerLevelBase *= 1.1;
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
    #endregion

    #region cutlery
    public static readonly Dictionary<string, int> entityExpCutlery = [];
    public static readonly int expPerHitCutlery = 1;

    public static void PopulateCutleryConfiguration()
    {
        entityExpCutlery["Dead drifter"] = 5;
        entityExpCutlery["Dead bear"] = 7;
        entityExpCutlery["Dead rooster"] = 1;
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
            expPerLevelBase *= 1.1;
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
    #endregion

    #region axe
    public static readonly Dictionary<string, int> entityExpAxe = [];
    public static readonly int expPerHitAxe = 1;
    public static readonly int expPerBreakingAxe = 1;

    public static void PopulateAxeConfiguration()
    {
        entityExpAxe["Dead drifter"] = 5;
        entityExpAxe["Dead bear"] = 7;
        entityExpAxe["Dead rooster"] = 1;
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
            expPerLevelBase *= 1.1;
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
            multiply += incrementSpeed;
        }

        baseSpeed += baseSpeed *= incrementSpeed;
        return baseSpeed;
    }
    #endregion

    #region pickaxe
    public static readonly Dictionary<string, int> entityExpPickaxe = [];
    public static readonly int expPerHitPickaxe = 1;
    public static readonly int expPerBreakingPickaxe = 1;

    public static void PopulatePickaxeConfiguration()
    {
        entityExpAxe["Dead drifter"] = 5;
        entityExpAxe["Dead bear"] = 7;
        entityExpAxe["Dead rooster"] = 1;
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
            expPerLevelBase *= 1.1;
        }
        return level;
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
            multiply += incrementSpeed;
        }

        baseSpeed += baseSpeed *= incrementSpeed;
        return baseSpeed;
    }
    #endregion

    #region shovel
    public static readonly Dictionary<string, int> entityExpShovel = [];
    public static readonly int expPerHitShovel = 1;
    public static readonly int expPerBreakingShovel = 1;

    public static void PopulateShovelConfiguration()
    {
        entityExpAxe["Dead drifter"] = 5;
        entityExpAxe["Dead bear"] = 7;
        entityExpAxe["Dead rooster"] = 1;
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
            expPerLevelBase *= 1.1;
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
            multiply += incrementSpeed;
        }

        baseSpeed += baseSpeed *= incrementSpeed;
        return baseSpeed;
    }
    #endregion

    #region spear
    public static readonly Dictionary<string, int> entityExpSpear = [];
    public static readonly int expPerHitSpear = 1;

    public static void PopulateSpearConfiguration()
    {
        entityExpAxe["Dead drifter"] = 5;
        entityExpAxe["Dead bear"] = 7;
        entityExpAxe["Dead rooster"] = 1;
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
            expPerLevelBase *= 1.1;
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
    #endregion
}