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
            case "Farming": return FarmingGetLevelByEXP(exp);
            default: break;
        }
        return 1;
    }

    #region hunter
    public static readonly Dictionary<string, int> entityExpHunter = [];

    public static void PopulateHunterConfiguration()
    {
        entityExpHunter["Dead bighorn ram"] = 5;
        entityExpHunter["Dead bighorn ewe"] = 5;
        entityExpHunter["Dead bighorn lamb"] = 2;
        entityExpHunter["Dead rooster"] = 1;
        entityExpHunter["Dead hen"] = 1;
        entityExpHunter["Dead chick"] = 1;
        entityExpHunter["Dead boar"] = 3;
        entityExpHunter["Dead sow"] = 3;
        entityExpHunter["Dead piglet"] = 1;
        entityExpHunter["Dead male wolf"] = 4;
        entityExpHunter["Dead female wolf"] = 4;
        entityExpHunter["Dead wolf pup"] = 1;
        entityExpHunter["Dead male hyena"] = 4;
        entityExpHunter["Dead female hyena"] = 4;
        entityExpHunter["Dead hyena pup"] = 1;
        entityExpHunter["Dead male fox"] = 2;
        entityExpHunter["Dead female fox"] = 2;
        entityExpHunter["Dead fox pup"] = 1;
        entityExpHunter["Dead male raccoon"] = 2;
        entityExpHunter["Dead female raccoon"] = 2;
        entityExpHunter["Dead raccoon pup"] = 1;
        entityExpHunter["Dead hare"] = 1;
        entityExpHunter["Dead drifter"] = 5;
        entityExpHunter["Dead deep drifter"] = 6;
        entityExpHunter["Dead tainted drifter"] = 7;
        entityExpHunter["Dead corrupt drifter"] = 8;
        entityExpHunter["Dead nightmare drifter"] = 9;
        entityExpHunter["Dead double-headed drifter"] = 10;
        entityExpHunter["Dead bronze locust"] = 1;
        entityExpHunter["Dead corrupt locust"] = 5;
        entityExpHunter["Dead corrupt sawblade locust"] = 7;
        entityExpHunter["Dead bell"] = 10;
        entityExpHunter["Dead salmon"] = 1;
        entityExpHunter["Dead female black bear"] = 5;
        entityExpHunter["Dead female brown bear"] = 7;
        entityExpHunter["Dead female sun bear"] = 3;
        entityExpHunter["Dead female panda bear"] = 5;
        entityExpHunter["Dead female polar bear"] = 7;
        entityExpHunter["Dead male black bear"] = 5;
        entityExpHunter["Dead male brown bear"] = 7;
        entityExpHunter["Dead male sun bear"] = 3;
        entityExpHunter["Dead male panda bear"] = 5;
        entityExpHunter["Dead male polar bear"] = 7;
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
        entityExpHunter["Dead bighorn ram"] = 5;
        entityExpHunter["Dead bighorn ewe"] = 5;
        entityExpHunter["Dead bighorn lamb"] = 2;
        entityExpHunter["Dead rooster"] = 1;
        entityExpHunter["Dead hen"] = 1;
        entityExpHunter["Dead chick"] = 1;
        entityExpHunter["Dead boar"] = 3;
        entityExpHunter["Dead sow"] = 3;
        entityExpHunter["Dead piglet"] = 1;
        entityExpHunter["Dead male wolf"] = 4;
        entityExpHunter["Dead female wolf"] = 4;
        entityExpHunter["Dead wolf pup"] = 1;
        entityExpHunter["Dead male hyena"] = 4;
        entityExpHunter["Dead female hyena"] = 4;
        entityExpHunter["Dead hyena pup"] = 1;
        entityExpHunter["Dead male fox"] = 2;
        entityExpHunter["Dead female fox"] = 2;
        entityExpHunter["Dead fox pup"] = 1;
        entityExpHunter["Dead male raccoon"] = 2;
        entityExpHunter["Dead female raccoon"] = 2;
        entityExpHunter["Dead raccoon pup"] = 1;
        entityExpHunter["Dead hare"] = 1;
        entityExpHunter["Dead drifter"] = 5;
        entityExpHunter["Dead deep drifter"] = 6;
        entityExpHunter["Dead tainted drifter"] = 7;
        entityExpHunter["Dead corrupt drifter"] = 8;
        entityExpHunter["Dead nightmare drifter"] = 9;
        entityExpHunter["Dead double-headed drifter"] = 10;
        entityExpHunter["Dead bronze locust"] = 1;
        entityExpHunter["Dead corrupt locust"] = 5;
        entityExpHunter["Dead corrupt sawblade locust"] = 7;
        entityExpHunter["Dead bell"] = 10;
        entityExpHunter["Dead salmon"] = 1;
        entityExpHunter["Dead female black bear"] = 5;
        entityExpHunter["Dead female brown bear"] = 7;
        entityExpHunter["Dead female sun bear"] = 3;
        entityExpHunter["Dead female panda bear"] = 5;
        entityExpHunter["Dead female polar bear"] = 7;
        entityExpHunter["Dead male black bear"] = 5;
        entityExpHunter["Dead male brown bear"] = 7;
        entityExpHunter["Dead male sun bear"] = 3;
        entityExpHunter["Dead male panda bear"] = 5;
        entityExpHunter["Dead male polar bear"] = 7;
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
    #endregion

    #region cutlery
    public static readonly Dictionary<string, int> entityExpCutlery = [];
    public static readonly int expPerHitCutlery = 1;
    public static readonly int expPerHarvestCutlery = 5;

    public static void PopulateCutleryConfiguration()
    {
        entityExpHunter["Dead bighorn ram"] = 5;
        entityExpHunter["Dead bighorn ewe"] = 5;
        entityExpHunter["Dead bighorn lamb"] = 2;
        entityExpHunter["Dead rooster"] = 1;
        entityExpHunter["Dead hen"] = 1;
        entityExpHunter["Dead chick"] = 1;
        entityExpHunter["Dead boar"] = 3;
        entityExpHunter["Dead sow"] = 3;
        entityExpHunter["Dead piglet"] = 1;
        entityExpHunter["Dead male wolf"] = 4;
        entityExpHunter["Dead female wolf"] = 4;
        entityExpHunter["Dead wolf pup"] = 1;
        entityExpHunter["Dead male hyena"] = 4;
        entityExpHunter["Dead female hyena"] = 4;
        entityExpHunter["Dead hyena pup"] = 1;
        entityExpHunter["Dead male fox"] = 2;
        entityExpHunter["Dead female fox"] = 2;
        entityExpHunter["Dead fox pup"] = 1;
        entityExpHunter["Dead male raccoon"] = 2;
        entityExpHunter["Dead female raccoon"] = 2;
        entityExpHunter["Dead raccoon pup"] = 1;
        entityExpHunter["Dead hare"] = 1;
        entityExpHunter["Dead drifter"] = 5;
        entityExpHunter["Dead deep drifter"] = 6;
        entityExpHunter["Dead tainted drifter"] = 7;
        entityExpHunter["Dead corrupt drifter"] = 8;
        entityExpHunter["Dead nightmare drifter"] = 9;
        entityExpHunter["Dead double-headed drifter"] = 10;
        entityExpHunter["Dead bronze locust"] = 1;
        entityExpHunter["Dead corrupt locust"] = 5;
        entityExpHunter["Dead corrupt sawblade locust"] = 7;
        entityExpHunter["Dead bell"] = 10;
        entityExpHunter["Dead salmon"] = 1;
        entityExpHunter["Dead female black bear"] = 5;
        entityExpHunter["Dead female brown bear"] = 7;
        entityExpHunter["Dead female sun bear"] = 3;
        entityExpHunter["Dead female panda bear"] = 5;
        entityExpHunter["Dead female polar bear"] = 7;
        entityExpHunter["Dead male black bear"] = 5;
        entityExpHunter["Dead male brown bear"] = 7;
        entityExpHunter["Dead male sun bear"] = 3;
        entityExpHunter["Dead male panda bear"] = 5;
        entityExpHunter["Dead male polar bear"] = 7;
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
    #endregion

    #region axe
    public static readonly Dictionary<string, int> entityExpAxe = [];
    public static readonly int expPerHitAxe = 1;
    public static readonly int expPerBreakingAxe = 1;
    public static readonly int expPerTreeBreakingAxe = 10;

    public static void PopulateAxeConfiguration()
    {
        entityExpHunter["Dead bighorn ram"] = 5;
        entityExpHunter["Dead bighorn ewe"] = 5;
        entityExpHunter["Dead bighorn lamb"] = 2;
        entityExpHunter["Dead rooster"] = 1;
        entityExpHunter["Dead hen"] = 1;
        entityExpHunter["Dead chick"] = 1;
        entityExpHunter["Dead boar"] = 3;
        entityExpHunter["Dead sow"] = 3;
        entityExpHunter["Dead piglet"] = 1;
        entityExpHunter["Dead male wolf"] = 4;
        entityExpHunter["Dead female wolf"] = 4;
        entityExpHunter["Dead wolf pup"] = 1;
        entityExpHunter["Dead male hyena"] = 4;
        entityExpHunter["Dead female hyena"] = 4;
        entityExpHunter["Dead hyena pup"] = 1;
        entityExpHunter["Dead male fox"] = 2;
        entityExpHunter["Dead female fox"] = 2;
        entityExpHunter["Dead fox pup"] = 1;
        entityExpHunter["Dead male raccoon"] = 2;
        entityExpHunter["Dead female raccoon"] = 2;
        entityExpHunter["Dead raccoon pup"] = 1;
        entityExpHunter["Dead hare"] = 1;
        entityExpHunter["Dead drifter"] = 5;
        entityExpHunter["Dead deep drifter"] = 6;
        entityExpHunter["Dead tainted drifter"] = 7;
        entityExpHunter["Dead corrupt drifter"] = 8;
        entityExpHunter["Dead nightmare drifter"] = 9;
        entityExpHunter["Dead double-headed drifter"] = 10;
        entityExpHunter["Dead bronze locust"] = 1;
        entityExpHunter["Dead corrupt locust"] = 5;
        entityExpHunter["Dead corrupt sawblade locust"] = 7;
        entityExpHunter["Dead bell"] = 10;
        entityExpHunter["Dead salmon"] = 1;
        entityExpHunter["Dead female black bear"] = 5;
        entityExpHunter["Dead female brown bear"] = 7;
        entityExpHunter["Dead female sun bear"] = 3;
        entityExpHunter["Dead female panda bear"] = 5;
        entityExpHunter["Dead female polar bear"] = 7;
        entityExpHunter["Dead male black bear"] = 5;
        entityExpHunter["Dead male brown bear"] = 7;
        entityExpHunter["Dead male sun bear"] = 3;
        entityExpHunter["Dead male panda bear"] = 5;
        entityExpHunter["Dead male polar bear"] = 7;
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
    #endregion

    #region pickaxe
    public static readonly Dictionary<string, int> entityExpPickaxe = [];
    public static readonly int expPerHitPickaxe = 1;
    public static readonly int expPerBreakingPickaxe = 1;

    public static void PopulatePickaxeConfiguration()
    {
        entityExpHunter["Dead bighorn ram"] = 5;
        entityExpHunter["Dead bighorn ewe"] = 5;
        entityExpHunter["Dead bighorn lamb"] = 2;
        entityExpHunter["Dead rooster"] = 1;
        entityExpHunter["Dead hen"] = 1;
        entityExpHunter["Dead chick"] = 1;
        entityExpHunter["Dead boar"] = 3;
        entityExpHunter["Dead sow"] = 3;
        entityExpHunter["Dead piglet"] = 1;
        entityExpHunter["Dead male wolf"] = 4;
        entityExpHunter["Dead female wolf"] = 4;
        entityExpHunter["Dead wolf pup"] = 1;
        entityExpHunter["Dead male hyena"] = 4;
        entityExpHunter["Dead female hyena"] = 4;
        entityExpHunter["Dead hyena pup"] = 1;
        entityExpHunter["Dead male fox"] = 2;
        entityExpHunter["Dead female fox"] = 2;
        entityExpHunter["Dead fox pup"] = 1;
        entityExpHunter["Dead male raccoon"] = 2;
        entityExpHunter["Dead female raccoon"] = 2;
        entityExpHunter["Dead raccoon pup"] = 1;
        entityExpHunter["Dead hare"] = 1;
        entityExpHunter["Dead drifter"] = 5;
        entityExpHunter["Dead deep drifter"] = 6;
        entityExpHunter["Dead tainted drifter"] = 7;
        entityExpHunter["Dead corrupt drifter"] = 8;
        entityExpHunter["Dead nightmare drifter"] = 9;
        entityExpHunter["Dead double-headed drifter"] = 10;
        entityExpHunter["Dead bronze locust"] = 1;
        entityExpHunter["Dead corrupt locust"] = 5;
        entityExpHunter["Dead corrupt sawblade locust"] = 7;
        entityExpHunter["Dead bell"] = 10;
        entityExpHunter["Dead salmon"] = 1;
        entityExpHunter["Dead female black bear"] = 5;
        entityExpHunter["Dead female brown bear"] = 7;
        entityExpHunter["Dead female sun bear"] = 3;
        entityExpHunter["Dead female panda bear"] = 5;
        entityExpHunter["Dead female polar bear"] = 7;
        entityExpHunter["Dead male black bear"] = 5;
        entityExpHunter["Dead male brown bear"] = 7;
        entityExpHunter["Dead male sun bear"] = 3;
        entityExpHunter["Dead male panda bear"] = 5;
        entityExpHunter["Dead male polar bear"] = 7;
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
        float baseMultiply = 1.0f;
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
    #endregion

    #region shovel
    public static readonly Dictionary<string, int> entityExpShovel = [];
    public static readonly int expPerHitShovel = 1;
    public static readonly int expPerBreakingShovel = 1;

    public static void PopulateShovelConfiguration()
    {
        entityExpHunter["Dead bighorn ram"] = 5;
        entityExpHunter["Dead bighorn ewe"] = 5;
        entityExpHunter["Dead bighorn lamb"] = 2;
        entityExpHunter["Dead rooster"] = 1;
        entityExpHunter["Dead hen"] = 1;
        entityExpHunter["Dead chick"] = 1;
        entityExpHunter["Dead boar"] = 3;
        entityExpHunter["Dead sow"] = 3;
        entityExpHunter["Dead piglet"] = 1;
        entityExpHunter["Dead male wolf"] = 4;
        entityExpHunter["Dead female wolf"] = 4;
        entityExpHunter["Dead wolf pup"] = 1;
        entityExpHunter["Dead male hyena"] = 4;
        entityExpHunter["Dead female hyena"] = 4;
        entityExpHunter["Dead hyena pup"] = 1;
        entityExpHunter["Dead male fox"] = 2;
        entityExpHunter["Dead female fox"] = 2;
        entityExpHunter["Dead fox pup"] = 1;
        entityExpHunter["Dead male raccoon"] = 2;
        entityExpHunter["Dead female raccoon"] = 2;
        entityExpHunter["Dead raccoon pup"] = 1;
        entityExpHunter["Dead hare"] = 1;
        entityExpHunter["Dead drifter"] = 5;
        entityExpHunter["Dead deep drifter"] = 6;
        entityExpHunter["Dead tainted drifter"] = 7;
        entityExpHunter["Dead corrupt drifter"] = 8;
        entityExpHunter["Dead nightmare drifter"] = 9;
        entityExpHunter["Dead double-headed drifter"] = 10;
        entityExpHunter["Dead bronze locust"] = 1;
        entityExpHunter["Dead corrupt locust"] = 5;
        entityExpHunter["Dead corrupt sawblade locust"] = 7;
        entityExpHunter["Dead bell"] = 10;
        entityExpHunter["Dead salmon"] = 1;
        entityExpHunter["Dead female black bear"] = 5;
        entityExpHunter["Dead female brown bear"] = 7;
        entityExpHunter["Dead female sun bear"] = 3;
        entityExpHunter["Dead female panda bear"] = 5;
        entityExpHunter["Dead female polar bear"] = 7;
        entityExpHunter["Dead male black bear"] = 5;
        entityExpHunter["Dead male brown bear"] = 7;
        entityExpHunter["Dead male sun bear"] = 3;
        entityExpHunter["Dead male panda bear"] = 5;
        entityExpHunter["Dead male polar bear"] = 7;
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
    #endregion

    #region spear
    public static readonly Dictionary<string, int> entityExpSpear = [];
    public static readonly int expPerHitSpear = 1;
    public static readonly int expPerThrowSpear = 2;

    public static void PopulateSpearConfiguration()
    {
        entityExpHunter["Dead bighorn ram"] = 5;
        entityExpHunter["Dead bighorn ewe"] = 5;
        entityExpHunter["Dead bighorn lamb"] = 2;
        entityExpHunter["Dead rooster"] = 1;
        entityExpHunter["Dead hen"] = 1;
        entityExpHunter["Dead chick"] = 1;
        entityExpHunter["Dead boar"] = 3;
        entityExpHunter["Dead sow"] = 3;
        entityExpHunter["Dead piglet"] = 1;
        entityExpHunter["Dead male wolf"] = 4;
        entityExpHunter["Dead female wolf"] = 4;
        entityExpHunter["Dead wolf pup"] = 1;
        entityExpHunter["Dead male hyena"] = 4;
        entityExpHunter["Dead female hyena"] = 4;
        entityExpHunter["Dead hyena pup"] = 1;
        entityExpHunter["Dead male fox"] = 2;
        entityExpHunter["Dead female fox"] = 2;
        entityExpHunter["Dead fox pup"] = 1;
        entityExpHunter["Dead male raccoon"] = 2;
        entityExpHunter["Dead female raccoon"] = 2;
        entityExpHunter["Dead raccoon pup"] = 1;
        entityExpHunter["Dead hare"] = 1;
        entityExpHunter["Dead drifter"] = 5;
        entityExpHunter["Dead deep drifter"] = 6;
        entityExpHunter["Dead tainted drifter"] = 7;
        entityExpHunter["Dead corrupt drifter"] = 8;
        entityExpHunter["Dead nightmare drifter"] = 9;
        entityExpHunter["Dead double-headed drifter"] = 10;
        entityExpHunter["Dead bronze locust"] = 1;
        entityExpHunter["Dead corrupt locust"] = 5;
        entityExpHunter["Dead corrupt sawblade locust"] = 7;
        entityExpHunter["Dead bell"] = 10;
        entityExpHunter["Dead salmon"] = 1;
        entityExpHunter["Dead female black bear"] = 5;
        entityExpHunter["Dead female brown bear"] = 7;
        entityExpHunter["Dead female sun bear"] = 3;
        entityExpHunter["Dead female panda bear"] = 5;
        entityExpHunter["Dead female polar bear"] = 7;
        entityExpHunter["Dead male black bear"] = 5;
        entityExpHunter["Dead male brown bear"] = 7;
        entityExpHunter["Dead male sun bear"] = 3;
        entityExpHunter["Dead male panda bear"] = 5;
        entityExpHunter["Dead male polar bear"] = 7;
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
    #endregion

    #region farming
    public static readonly Dictionary<int, int> expPerHarvestFarming = [];
    public static readonly int expPerPlowFarming = 1;

    public static void PopulateFarmingConfiguration()
    {
        expPerHarvestFarming[4356] = 5; // Turnip
        expPerHarvestFarming[4340] = 7; // Carrot
        expPerHarvestFarming[4224] = 4; // Flax
        expPerHarvestFarming[4313] = 6; // Onion
        expPerHarvestFarming[4233] = 4; // Spelt
        expPerHarvestFarming[4274] = 7; // Parsnip
        expPerHarvestFarming[4331] = 8; // Rye
        expPerHarvestFarming[4291] = 4; // Rice
        expPerHarvestFarming[4320] = 5; // Soybean
        expPerHarvestFarming[4347] = 9; // Amarant
        expPerHarvestFarming[4282] = 5; // Cassava
        expPerHarvestFarming[4215] = 7; // Peanut
        expPerHarvestFarming[4361] = 6; // Pineapple
        expPerHarvestFarming[4242] = 8; // Sunflower
        expPerHarvestFarming[4198] = 10; // Pumpkin
        expPerHarvestFarming[4301] = 11; // Cabbage
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
    #endregion
}