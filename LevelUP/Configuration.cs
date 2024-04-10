using System;
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
            case "Cooking": return CookingGetLevelByEXP(exp);
            default: break;
        }
        return 1;
    }

    #region hunter
    public static readonly Dictionary<string, int> entityExpHunter = [];

    public static void PopulateHunterConfiguration()
    {
        #region vanilla
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
        entityExpHunter["Dead bronze locust (hacked)"] = 5;
        entityExpHunter["Dead corrupt locust (hacked)"] = 10;
        entityExpHunter["Dead gazelle (male)"] = 5;
        entityExpHunter["Dead gazelle (female)"] = 5;
        entityExpHunter["Dead gazelle calf"] = 2;
        entityExpHunter["Dead male moose"] = 6;
        entityExpHunter["Dead female moose"] = 6;
        entityExpHunter["Dead male moose calf"] = 2;
        entityExpHunter["Dead female moose calf"] = 2;
        entityExpHunter["Dead whitetail deer (male)"] = 3;
        entityExpHunter["Dead whitetail deer (female)"] = 3;
        entityExpHunter["Dead whitetail deer fawn (male)"] = 3;
        entityExpHunter["Dead whitetail deer fawn (female)"] = 3;
        entityExpHunter["Dead red brocket deer (male)"] = 3;
        entityExpHunter["Dead red brocket deer (female)"] = 3;
        entityExpHunter["Dead red brocket deer fawn (female)"] = 3;
        entityExpHunter["Dead red brocket deer fawn (male)"] = 3;
        entityExpHunter["Dead marsh deer (male)"] = 5;
        entityExpHunter["Dead marsh deer (female)"] = 5;
        entityExpHunter["Dead marsh deer fawn (male)"] = 3;
        entityExpHunter["Dead marsh deer fawn (female)"] = 3;
        entityExpHunter["Dead caribou (male)"] = 5;
        entityExpHunter["Dead caribou (female)"] = 5;
        entityExpHunter["Dead caribou calf (male)"] = 3;
        entityExpHunter["Dead caribou calf (female)"] = 3;
        entityExpHunter["Dead water deer (male)"] = 5;
        entityExpHunter["Dead water deer (female)"] = 5;
        entityExpHunter["Dead water deer fawn (male)"] = 3;
        entityExpHunter["Dead water deer fawn (female)"] = 3;
        entityExpHunter["Dead pudu deer (male)"] = 3;
        entityExpHunter["Dead pudu deer (female)"] = 3;
        entityExpHunter["Dead pudu deer fawn (male)"] = 1;
        entityExpHunter["Dead pudu deer fawn (female)"] = 1;
        entityExpHunter["Dead elk (male)"] = 6;
        entityExpHunter["Dead elk (female)"] = 6;
        entityExpHunter["Dead elk fawn (male)"] = 2;
        entityExpHunter["Dead elk fawn (female)"] = 2;
        entityExpHunter["Dead taruca deer (male)"] = 6;
        entityExpHunter["Dead taruca deer (female)"] = 6;
        entityExpHunter["Dead taruca deer fawn (male)"] = 7;
        entityExpHunter["Dead taruca deer fawn (female)"] = 7;
        entityExpHunter["Dead chital deer (male)"] = 4;
        entityExpHunter["Dead chital deer (female)"] = 4;
        entityExpHunter["Dead chital deer fawn (male)"] = 1;
        entityExpHunter["Dead chital deer fawn (female)"] = 1;
        entityExpHunter["Dead guemal deer (male)"] = 6;
        entityExpHunter["Dead guemal deer (female)"] = 6;
        entityExpHunter["Dead guemal deer fawn (male)"] = 2;
        entityExpHunter["Dead guemal deer fawn (female)"] = 2;
        entityExpHunter["Dead pampas deer (male)"] = 5;
        entityExpHunter["Dead pampas deer (female)"] = 5;
        entityExpHunter["Dead pampas deer fawn (male)"] = 2;
        entityExpHunter["Dead pampas deer fawn (female)"] = 2;
        entityExpHunter["Dead fallow deer (male)"] = 6;
        entityExpHunter["Dead fallow deer (female)"] = 6;
        entityExpHunter["Dead fallow deer fawn (male)"] = 2;
        entityExpHunter["Dead fallow deer fawn (female)"] = 2;
        entityExpHunter["Dead angora goat (male)"] = 6;
        entityExpHunter["Dead angora goat (female)"] = 6;
        entityExpHunter["Dead angora goat kid (male)"] = 2;
        entityExpHunter["Dead angora goat kid (female)"] = 2;
        entityExpHunter["Dead alpine ibex (male)"] = 7;
        entityExpHunter["Dead alpine ibex (female)"] = 7;
        entityExpHunter["Dead alpine ibex kid (male)"] = 3;
        entityExpHunter["Dead alpine ibex kid (female)"] = 3;
        entityExpHunter["Dead nubian ibex (male)"] = 7;
        entityExpHunter["Dead nubian ibex (female)"] = 7;
        entityExpHunter["Dead nubian ibex kid (male)"] = 3;
        entityExpHunter["Dead nubian ibex kid (female)"] = 3;
        entityExpHunter["Dead markhor goat kid(male)"] = 5;
        entityExpHunter["Dead markhor goat (female)"] = 5;
        entityExpHunter["Dead markhor goat kid (male)"] = 2;
        entityExpHunter["Dead markhor goat kid (female)"] = 2;
        entityExpHunter["Dead mountain goat (male)"] = 6;
        entityExpHunter["Dead mountain goat (female)"] = 6;
        entityExpHunter["Dead mountain goat kid (male)"] = 2;
        entityExpHunter["Dead mountain goat kid (female)"] = 2;
        entityExpHunter["Dead musk ox (male)"] = 8;
        entityExpHunter["Dead musk ox (female)"] = 8;
        entityExpHunter["Dead musk ox calf (male)"] = 3;
        entityExpHunter["Dead musk ox calf (female)"] = 3;
        entityExpHunter["Dead sihori goat (male)"] = 7;
        entityExpHunter["Dead sihori goat (female)"] = 7;
        entityExpHunter["Dead sihori goat kid (male)"] = 3;
        entityExpHunter["Dead sihori goat kid (female)"] = 3;
        entityExpHunter["Dead golden takin (male)"] = 7;
        entityExpHunter["Dead golden takin (female)"] = 7;
        entityExpHunter["Dead golden takin kid (male)"] = 4;
        entityExpHunter["Dead golden takin kid (female)"] = 4;
        entityExpHunter["Dead tur goat (male)"] = 5;
        entityExpHunter["Dead tur goat (female)"] = 5;
        entityExpHunter["Dead tur goat kid (male)"] = 2;
        entityExpHunter["Dead tur goat kid (female)"] = 2;
        entityExpHunter["Dead valais goat (male)"] = 6;
        entityExpHunter["Dead valais goat (female)"] = 6;
        entityExpHunter["Dead valais goat kid (male)"] = 3;
        entityExpHunter["Dead valais goat kid (female)"] = 3;
        #endregion
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
        #region vanilla
        entityExpBow["Dead bighorn ram"] = 5;
        entityExpBow["Dead bighorn ewe"] = 5;
        entityExpBow["Dead bighorn lamb"] = 2;
        entityExpBow["Dead rooster"] = 1;
        entityExpBow["Dead hen"] = 1;
        entityExpBow["Dead chick"] = 1;
        entityExpBow["Dead boar"] = 3;
        entityExpBow["Dead sow"] = 3;
        entityExpBow["Dead piglet"] = 1;
        entityExpBow["Dead male wolf"] = 4;
        entityExpBow["Dead female wolf"] = 4;
        entityExpBow["Dead wolf pup"] = 1;
        entityExpBow["Dead male hyena"] = 4;
        entityExpBow["Dead female hyena"] = 4;
        entityExpBow["Dead hyena pup"] = 1;
        entityExpBow["Dead male fox"] = 2;
        entityExpBow["Dead female fox"] = 2;
        entityExpBow["Dead fox pup"] = 1;
        entityExpBow["Dead male raccoon"] = 2;
        entityExpBow["Dead female raccoon"] = 2;
        entityExpBow["Dead raccoon pup"] = 1;
        entityExpBow["Dead hare"] = 1;
        entityExpBow["Dead drifter"] = 5;
        entityExpBow["Dead deep drifter"] = 6;
        entityExpBow["Dead tainted drifter"] = 7;
        entityExpBow["Dead corrupt drifter"] = 8;
        entityExpBow["Dead nightmare drifter"] = 9;
        entityExpBow["Dead double-headed drifter"] = 10;
        entityExpBow["Dead bronze locust"] = 1;
        entityExpBow["Dead corrupt locust"] = 5;
        entityExpBow["Dead corrupt sawblade locust"] = 7;
        entityExpBow["Dead bell"] = 10;
        entityExpBow["Dead salmon"] = 1;
        entityExpBow["Dead female black bear"] = 5;
        entityExpBow["Dead female brown bear"] = 7;
        entityExpBow["Dead female sun bear"] = 3;
        entityExpBow["Dead female panda bear"] = 5;
        entityExpBow["Dead female polar bear"] = 7;
        entityExpBow["Dead male black bear"] = 5;
        entityExpBow["Dead male brown bear"] = 7;
        entityExpBow["Dead male sun bear"] = 3;
        entityExpBow["Dead male panda bear"] = 5;
        entityExpBow["Dead male polar bear"] = 7;
        entityExpBow["Dead bronze locust (hacked)"] = 5;
        entityExpBow["Dead corrupt locust (hacked)"] = 10;
        entityExpBow["Dead gazelle (male)"] = 5;
        entityExpBow["Dead gazelle (female)"] = 5;
        entityExpBow["Dead gazelle calf"] = 2;
        entityExpBow["Dead male moose"] = 6;
        entityExpBow["Dead female moose"] = 6;
        entityExpBow["Dead male moose calf"] = 2;
        entityExpBow["Dead female moose calf"] = 2;
        entityExpBow["Dead whitetail deer (male)"] = 3;
        entityExpBow["Dead whitetail deer (female)"] = 3;
        entityExpBow["Dead whitetail deer fawn (male)"] = 3;
        entityExpBow["Dead whitetail deer fawn (female)"] = 3;
        entityExpBow["Dead red brocket deer (male)"] = 3;
        entityExpBow["Dead red brocket deer (female)"] = 3;
        entityExpBow["Dead red brocket deer fawn (female)"] = 3;
        entityExpBow["Dead red brocket deer fawn (male)"] = 3;
        entityExpBow["Dead marsh deer (male)"] = 5;
        entityExpBow["Dead marsh deer (female)"] = 5;
        entityExpBow["Dead marsh deer fawn (male)"] = 3;
        entityExpBow["Dead marsh deer fawn (female)"] = 3;
        entityExpBow["Dead caribou (male)"] = 5;
        entityExpBow["Dead caribou (female)"] = 5;
        entityExpBow["Dead caribou calf (male)"] = 3;
        entityExpBow["Dead caribou calf (female)"] = 3;
        entityExpBow["Dead water deer (male)"] = 5;
        entityExpBow["Dead water deer (female)"] = 5;
        entityExpBow["Dead water deer fawn (male)"] = 3;
        entityExpBow["Dead water deer fawn (female)"] = 3;
        entityExpBow["Dead pudu deer (male)"] = 3;
        entityExpBow["Dead pudu deer (female)"] = 3;
        entityExpBow["Dead pudu deer fawn (male)"] = 1;
        entityExpBow["Dead pudu deer fawn (female)"] = 1;
        entityExpBow["Dead elk (male)"] = 6;
        entityExpBow["Dead elk (female)"] = 6;
        entityExpBow["Dead elk fawn (male)"] = 2;
        entityExpBow["Dead elk fawn (female)"] = 2;
        entityExpBow["Dead taruca deer (male)"] = 6;
        entityExpBow["Dead taruca deer (female)"] = 6;
        entityExpBow["Dead taruca deer fawn (male)"] = 7;
        entityExpBow["Dead taruca deer fawn (female)"] = 7;
        entityExpBow["Dead chital deer (male)"] = 4;
        entityExpBow["Dead chital deer (female)"] = 4;
        entityExpBow["Dead chital deer fawn (male)"] = 1;
        entityExpBow["Dead chital deer fawn (female)"] = 1;
        entityExpBow["Dead guemal deer (male)"] = 6;
        entityExpBow["Dead guemal deer (female)"] = 6;
        entityExpBow["Dead guemal deer fawn (male)"] = 2;
        entityExpBow["Dead guemal deer fawn (female)"] = 2;
        entityExpBow["Dead pampas deer (male)"] = 5;
        entityExpBow["Dead pampas deer (female)"] = 5;
        entityExpBow["Dead pampas deer fawn (male)"] = 2;
        entityExpBow["Dead pampas deer fawn (female)"] = 2;
        entityExpBow["Dead fallow deer (male)"] = 6;
        entityExpBow["Dead fallow deer (female)"] = 6;
        entityExpBow["Dead fallow deer fawn (male)"] = 2;
        entityExpBow["Dead fallow deer fawn (female)"] = 2;
        entityExpBow["Dead angora goat (male)"] = 6;
        entityExpBow["Dead angora goat (female)"] = 6;
        entityExpBow["Dead angora goat kid (male)"] = 2;
        entityExpBow["Dead angora goat kid (female)"] = 2;
        entityExpBow["Dead alpine ibex (male)"] = 7;
        entityExpBow["Dead alpine ibex (female)"] = 7;
        entityExpBow["Dead alpine ibex kid (male)"] = 3;
        entityExpBow["Dead alpine ibex kid (female)"] = 3;
        entityExpBow["Dead nubian ibex (male)"] = 7;
        entityExpBow["Dead nubian ibex (female)"] = 7;
        entityExpBow["Dead nubian ibex kid (male)"] = 3;
        entityExpBow["Dead nubian ibex kid (female)"] = 3;
        entityExpBow["Dead markhor goat kid(male)"] = 5;
        entityExpBow["Dead markhor goat (female)"] = 5;
        entityExpBow["Dead markhor goat kid (male)"] = 2;
        entityExpBow["Dead markhor goat kid (female)"] = 2;
        entityExpBow["Dead mountain goat (male)"] = 6;
        entityExpBow["Dead mountain goat (female)"] = 6;
        entityExpBow["Dead mountain goat kid (male)"] = 2;
        entityExpBow["Dead mountain goat kid (female)"] = 2;
        entityExpBow["Dead musk ox (male)"] = 8;
        entityExpBow["Dead musk ox (female)"] = 8;
        entityExpBow["Dead musk ox calf (male)"] = 3;
        entityExpBow["Dead musk ox calf (female)"] = 3;
        entityExpBow["Dead sihori goat (male)"] = 7;
        entityExpBow["Dead sihori goat (female)"] = 7;
        entityExpBow["Dead sihori goat kid (male)"] = 3;
        entityExpBow["Dead sihori goat kid (female)"] = 3;
        entityExpBow["Dead golden takin (male)"] = 7;
        entityExpBow["Dead golden takin (female)"] = 7;
        entityExpBow["Dead golden takin kid (male)"] = 4;
        entityExpBow["Dead golden takin kid (female)"] = 4;
        entityExpBow["Dead tur goat (male)"] = 5;
        entityExpBow["Dead tur goat (female)"] = 5;
        entityExpBow["Dead tur goat kid (male)"] = 2;
        entityExpBow["Dead tur goat kid (female)"] = 2;
        entityExpBow["Dead valais goat (male)"] = 6;
        entityExpBow["Dead valais goat (female)"] = 6;
        entityExpBow["Dead valais goat kid (male)"] = 3;
        entityExpBow["Dead valais goat kid (female)"] = 3;
        #endregion
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
        #region vanilla
        entityExpCutlery["Dead bighorn ram"] = 5;
        entityExpCutlery["Dead bighorn ewe"] = 5;
        entityExpCutlery["Dead bighorn lamb"] = 2;
        entityExpCutlery["Dead rooster"] = 1;
        entityExpCutlery["Dead hen"] = 1;
        entityExpCutlery["Dead chick"] = 1;
        entityExpCutlery["Dead boar"] = 3;
        entityExpCutlery["Dead sow"] = 3;
        entityExpCutlery["Dead piglet"] = 1;
        entityExpCutlery["Dead male wolf"] = 4;
        entityExpCutlery["Dead female wolf"] = 4;
        entityExpCutlery["Dead wolf pup"] = 1;
        entityExpCutlery["Dead male hyena"] = 4;
        entityExpCutlery["Dead female hyena"] = 4;
        entityExpCutlery["Dead hyena pup"] = 1;
        entityExpCutlery["Dead male fox"] = 2;
        entityExpCutlery["Dead female fox"] = 2;
        entityExpCutlery["Dead fox pup"] = 1;
        entityExpCutlery["Dead male raccoon"] = 2;
        entityExpCutlery["Dead female raccoon"] = 2;
        entityExpCutlery["Dead raccoon pup"] = 1;
        entityExpCutlery["Dead hare"] = 1;
        entityExpCutlery["Dead drifter"] = 5;
        entityExpCutlery["Dead deep drifter"] = 6;
        entityExpCutlery["Dead tainted drifter"] = 7;
        entityExpCutlery["Dead corrupt drifter"] = 8;
        entityExpCutlery["Dead nightmare drifter"] = 9;
        entityExpCutlery["Dead double-headed drifter"] = 10;
        entityExpCutlery["Dead bronze locust"] = 1;
        entityExpCutlery["Dead corrupt locust"] = 5;
        entityExpCutlery["Dead corrupt sawblade locust"] = 7;
        entityExpCutlery["Dead bell"] = 10;
        entityExpCutlery["Dead salmon"] = 1;
        entityExpCutlery["Dead female black bear"] = 5;
        entityExpCutlery["Dead female brown bear"] = 7;
        entityExpCutlery["Dead female sun bear"] = 3;
        entityExpCutlery["Dead female panda bear"] = 5;
        entityExpCutlery["Dead female polar bear"] = 7;
        entityExpCutlery["Dead male black bear"] = 5;
        entityExpCutlery["Dead male brown bear"] = 7;
        entityExpCutlery["Dead male sun bear"] = 3;
        entityExpCutlery["Dead male panda bear"] = 5;
        entityExpCutlery["Dead male polar bear"] = 7;
        entityExpCutlery["Dead bronze locust (hacked)"] = 5;
        entityExpCutlery["Dead corrupt locust (hacked)"] = 10;
        entityExpCutlery["Dead gazelle (male)"] = 5;
        entityExpCutlery["Dead gazelle (female)"] = 5;
        entityExpCutlery["Dead gazelle calf"] = 2;
        entityExpCutlery["Dead male moose"] = 6;
        entityExpCutlery["Dead female moose"] = 6;
        entityExpCutlery["Dead male moose calf"] = 2;
        entityExpCutlery["Dead female moose calf"] = 2;
        entityExpCutlery["Dead whitetail deer (male)"] = 3;
        entityExpCutlery["Dead whitetail deer (female)"] = 3;
        entityExpCutlery["Dead whitetail deer fawn (male)"] = 3;
        entityExpCutlery["Dead whitetail deer fawn (female)"] = 3;
        entityExpCutlery["Dead red brocket deer (male)"] = 3;
        entityExpCutlery["Dead red brocket deer (female)"] = 3;
        entityExpCutlery["Dead red brocket deer fawn (female)"] = 3;
        entityExpCutlery["Dead red brocket deer fawn (male)"] = 3;
        entityExpCutlery["Dead marsh deer (male)"] = 5;
        entityExpCutlery["Dead marsh deer (female)"] = 5;
        entityExpCutlery["Dead marsh deer fawn (male)"] = 3;
        entityExpCutlery["Dead marsh deer fawn (female)"] = 3;
        entityExpCutlery["Dead caribou (male)"] = 5;
        entityExpCutlery["Dead caribou (female)"] = 5;
        entityExpCutlery["Dead caribou calf (male)"] = 3;
        entityExpCutlery["Dead caribou calf (female)"] = 3;
        entityExpCutlery["Dead water deer (male)"] = 5;
        entityExpCutlery["Dead water deer (female)"] = 5;
        entityExpCutlery["Dead water deer fawn (male)"] = 3;
        entityExpCutlery["Dead water deer fawn (female)"] = 3;
        entityExpCutlery["Dead pudu deer (male)"] = 3;
        entityExpCutlery["Dead pudu deer (female)"] = 3;
        entityExpCutlery["Dead pudu deer fawn (male)"] = 1;
        entityExpCutlery["Dead pudu deer fawn (female)"] = 1;
        entityExpCutlery["Dead elk (male)"] = 6;
        entityExpCutlery["Dead elk (female)"] = 6;
        entityExpCutlery["Dead elk fawn (male)"] = 2;
        entityExpCutlery["Dead elk fawn (female)"] = 2;
        entityExpCutlery["Dead taruca deer (male)"] = 6;
        entityExpCutlery["Dead taruca deer (female)"] = 6;
        entityExpCutlery["Dead taruca deer fawn (male)"] = 7;
        entityExpCutlery["Dead taruca deer fawn (female)"] = 7;
        entityExpCutlery["Dead chital deer (male)"] = 4;
        entityExpCutlery["Dead chital deer (female)"] = 4;
        entityExpCutlery["Dead chital deer fawn (male)"] = 1;
        entityExpCutlery["Dead chital deer fawn (female)"] = 1;
        entityExpCutlery["Dead guemal deer (male)"] = 6;
        entityExpCutlery["Dead guemal deer (female)"] = 6;
        entityExpCutlery["Dead guemal deer fawn (male)"] = 2;
        entityExpCutlery["Dead guemal deer fawn (female)"] = 2;
        entityExpCutlery["Dead pampas deer (male)"] = 5;
        entityExpCutlery["Dead pampas deer (female)"] = 5;
        entityExpCutlery["Dead pampas deer fawn (male)"] = 2;
        entityExpCutlery["Dead pampas deer fawn (female)"] = 2;
        entityExpCutlery["Dead fallow deer (male)"] = 6;
        entityExpCutlery["Dead fallow deer (female)"] = 6;
        entityExpCutlery["Dead fallow deer fawn (male)"] = 2;
        entityExpCutlery["Dead fallow deer fawn (female)"] = 2;
        entityExpCutlery["Dead angora goat (male)"] = 6;
        entityExpCutlery["Dead angora goat (female)"] = 6;
        entityExpCutlery["Dead angora goat kid (male)"] = 2;
        entityExpCutlery["Dead angora goat kid (female)"] = 2;
        entityExpCutlery["Dead alpine ibex (male)"] = 7;
        entityExpCutlery["Dead alpine ibex (female)"] = 7;
        entityExpCutlery["Dead alpine ibex kid (male)"] = 3;
        entityExpCutlery["Dead alpine ibex kid (female)"] = 3;
        entityExpCutlery["Dead nubian ibex (male)"] = 7;
        entityExpCutlery["Dead nubian ibex (female)"] = 7;
        entityExpCutlery["Dead nubian ibex kid (male)"] = 3;
        entityExpCutlery["Dead nubian ibex kid (female)"] = 3;
        entityExpCutlery["Dead markhor goat kid(male)"] = 5;
        entityExpCutlery["Dead markhor goat (female)"] = 5;
        entityExpCutlery["Dead markhor goat kid (male)"] = 2;
        entityExpCutlery["Dead markhor goat kid (female)"] = 2;
        entityExpCutlery["Dead mountain goat (male)"] = 6;
        entityExpCutlery["Dead mountain goat (female)"] = 6;
        entityExpCutlery["Dead mountain goat kid (male)"] = 2;
        entityExpCutlery["Dead mountain goat kid (female)"] = 2;
        entityExpCutlery["Dead musk ox (male)"] = 8;
        entityExpCutlery["Dead musk ox (female)"] = 8;
        entityExpCutlery["Dead musk ox calf (male)"] = 3;
        entityExpCutlery["Dead musk ox calf (female)"] = 3;
        entityExpCutlery["Dead sihori goat (male)"] = 7;
        entityExpCutlery["Dead sihori goat (female)"] = 7;
        entityExpCutlery["Dead sihori goat kid (male)"] = 3;
        entityExpCutlery["Dead sihori goat kid (female)"] = 3;
        entityExpCutlery["Dead golden takin (male)"] = 7;
        entityExpCutlery["Dead golden takin (female)"] = 7;
        entityExpCutlery["Dead golden takin kid (male)"] = 4;
        entityExpCutlery["Dead golden takin kid (female)"] = 4;
        entityExpCutlery["Dead tur goat (male)"] = 5;
        entityExpCutlery["Dead tur goat (female)"] = 5;
        entityExpCutlery["Dead tur goat kid (male)"] = 2;
        entityExpCutlery["Dead tur goat kid (female)"] = 2;
        entityExpCutlery["Dead valais goat (male)"] = 6;
        entityExpCutlery["Dead valais goat (female)"] = 6;
        entityExpCutlery["Dead valais goat kid (male)"] = 3;
        entityExpCutlery["Dead valais goat kid (female)"] = 3;
        #endregion
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
        #region vanilla
        entityExpAxe["Dead bighorn ram"] = 5;
        entityExpAxe["Dead bighorn ewe"] = 5;
        entityExpAxe["Dead bighorn lamb"] = 2;
        entityExpAxe["Dead rooster"] = 1;
        entityExpAxe["Dead hen"] = 1;
        entityExpAxe["Dead chick"] = 1;
        entityExpAxe["Dead boar"] = 3;
        entityExpAxe["Dead sow"] = 3;
        entityExpAxe["Dead piglet"] = 1;
        entityExpAxe["Dead male wolf"] = 4;
        entityExpAxe["Dead female wolf"] = 4;
        entityExpAxe["Dead wolf pup"] = 1;
        entityExpAxe["Dead male hyena"] = 4;
        entityExpAxe["Dead female hyena"] = 4;
        entityExpAxe["Dead hyena pup"] = 1;
        entityExpAxe["Dead male fox"] = 2;
        entityExpAxe["Dead female fox"] = 2;
        entityExpAxe["Dead fox pup"] = 1;
        entityExpAxe["Dead male raccoon"] = 2;
        entityExpAxe["Dead female raccoon"] = 2;
        entityExpAxe["Dead raccoon pup"] = 1;
        entityExpAxe["Dead hare"] = 1;
        entityExpAxe["Dead drifter"] = 5;
        entityExpAxe["Dead deep drifter"] = 6;
        entityExpAxe["Dead tainted drifter"] = 7;
        entityExpAxe["Dead corrupt drifter"] = 8;
        entityExpAxe["Dead nightmare drifter"] = 9;
        entityExpAxe["Dead double-headed drifter"] = 10;
        entityExpAxe["Dead bronze locust"] = 1;
        entityExpAxe["Dead corrupt locust"] = 5;
        entityExpAxe["Dead corrupt sawblade locust"] = 7;
        entityExpAxe["Dead bell"] = 10;
        entityExpAxe["Dead salmon"] = 1;
        entityExpAxe["Dead female black bear"] = 5;
        entityExpAxe["Dead female brown bear"] = 7;
        entityExpAxe["Dead female sun bear"] = 3;
        entityExpAxe["Dead female panda bear"] = 5;
        entityExpAxe["Dead female polar bear"] = 7;
        entityExpAxe["Dead male black bear"] = 5;
        entityExpAxe["Dead male brown bear"] = 7;
        entityExpAxe["Dead male sun bear"] = 3;
        entityExpAxe["Dead male panda bear"] = 5;
        entityExpAxe["Dead male polar bear"] = 7;
        entityExpAxe["Dead bronze locust (hacked)"] = 5;
        entityExpAxe["Dead corrupt locust (hacked)"] = 10;
        entityExpAxe["Dead gazelle (male)"] = 5;
        entityExpAxe["Dead gazelle (female)"] = 5;
        entityExpAxe["Dead gazelle calf"] = 2;
        entityExpAxe["Dead male moose"] = 6;
        entityExpAxe["Dead female moose"] = 6;
        entityExpAxe["Dead male moose calf"] = 2;
        entityExpAxe["Dead female moose calf"] = 2;
        entityExpAxe["Dead whitetail deer (male)"] = 3;
        entityExpAxe["Dead whitetail deer (female)"] = 3;
        entityExpAxe["Dead whitetail deer fawn (male)"] = 3;
        entityExpAxe["Dead whitetail deer fawn (female)"] = 3;
        entityExpAxe["Dead red brocket deer (male)"] = 3;
        entityExpAxe["Dead red brocket deer (female)"] = 3;
        entityExpAxe["Dead red brocket deer fawn (female)"] = 3;
        entityExpAxe["Dead red brocket deer fawn (male)"] = 3;
        entityExpAxe["Dead marsh deer (male)"] = 5;
        entityExpAxe["Dead marsh deer (female)"] = 5;
        entityExpAxe["Dead marsh deer fawn (male)"] = 3;
        entityExpAxe["Dead marsh deer fawn (female)"] = 3;
        entityExpAxe["Dead caribou (male)"] = 5;
        entityExpAxe["Dead caribou (female)"] = 5;
        entityExpAxe["Dead caribou calf (male)"] = 3;
        entityExpAxe["Dead caribou calf (female)"] = 3;
        entityExpAxe["Dead water deer (male)"] = 5;
        entityExpAxe["Dead water deer (female)"] = 5;
        entityExpAxe["Dead water deer fawn (male)"] = 3;
        entityExpAxe["Dead water deer fawn (female)"] = 3;
        entityExpAxe["Dead pudu deer (male)"] = 3;
        entityExpAxe["Dead pudu deer (female)"] = 3;
        entityExpAxe["Dead pudu deer fawn (male)"] = 1;
        entityExpAxe["Dead pudu deer fawn (female)"] = 1;
        entityExpAxe["Dead elk (male)"] = 6;
        entityExpAxe["Dead elk (female)"] = 6;
        entityExpAxe["Dead elk fawn (male)"] = 2;
        entityExpAxe["Dead elk fawn (female)"] = 2;
        entityExpAxe["Dead taruca deer (male)"] = 6;
        entityExpAxe["Dead taruca deer (female)"] = 6;
        entityExpAxe["Dead taruca deer fawn (male)"] = 7;
        entityExpAxe["Dead taruca deer fawn (female)"] = 7;
        entityExpAxe["Dead chital deer (male)"] = 4;
        entityExpAxe["Dead chital deer (female)"] = 4;
        entityExpAxe["Dead chital deer fawn (male)"] = 1;
        entityExpAxe["Dead chital deer fawn (female)"] = 1;
        entityExpAxe["Dead guemal deer (male)"] = 6;
        entityExpAxe["Dead guemal deer (female)"] = 6;
        entityExpAxe["Dead guemal deer fawn (male)"] = 2;
        entityExpAxe["Dead guemal deer fawn (female)"] = 2;
        entityExpAxe["Dead pampas deer (male)"] = 5;
        entityExpAxe["Dead pampas deer (female)"] = 5;
        entityExpAxe["Dead pampas deer fawn (male)"] = 2;
        entityExpAxe["Dead pampas deer fawn (female)"] = 2;
        entityExpAxe["Dead fallow deer (male)"] = 6;
        entityExpAxe["Dead fallow deer (female)"] = 6;
        entityExpAxe["Dead fallow deer fawn (male)"] = 2;
        entityExpAxe["Dead fallow deer fawn (female)"] = 2;
        entityExpAxe["Dead angora goat (male)"] = 6;
        entityExpAxe["Dead angora goat (female)"] = 6;
        entityExpAxe["Dead angora goat kid (male)"] = 2;
        entityExpAxe["Dead angora goat kid (female)"] = 2;
        entityExpAxe["Dead alpine ibex (male)"] = 7;
        entityExpAxe["Dead alpine ibex (female)"] = 7;
        entityExpAxe["Dead alpine ibex kid (male)"] = 3;
        entityExpAxe["Dead alpine ibex kid (female)"] = 3;
        entityExpAxe["Dead nubian ibex (male)"] = 7;
        entityExpAxe["Dead nubian ibex (female)"] = 7;
        entityExpAxe["Dead nubian ibex kid (male)"] = 3;
        entityExpAxe["Dead nubian ibex kid (female)"] = 3;
        entityExpAxe["Dead markhor goat kid(male)"] = 5;
        entityExpAxe["Dead markhor goat (female)"] = 5;
        entityExpAxe["Dead markhor goat kid (male)"] = 2;
        entityExpAxe["Dead markhor goat kid (female)"] = 2;
        entityExpAxe["Dead mountain goat (male)"] = 6;
        entityExpAxe["Dead mountain goat (female)"] = 6;
        entityExpAxe["Dead mountain goat kid (male)"] = 2;
        entityExpAxe["Dead mountain goat kid (female)"] = 2;
        entityExpAxe["Dead musk ox (male)"] = 8;
        entityExpAxe["Dead musk ox (female)"] = 8;
        entityExpAxe["Dead musk ox calf (male)"] = 3;
        entityExpAxe["Dead musk ox calf (female)"] = 3;
        entityExpAxe["Dead sihori goat (male)"] = 7;
        entityExpAxe["Dead sihori goat (female)"] = 7;
        entityExpAxe["Dead sihori goat kid (male)"] = 3;
        entityExpAxe["Dead sihori goat kid (female)"] = 3;
        entityExpAxe["Dead golden takin (male)"] = 7;
        entityExpAxe["Dead golden takin (female)"] = 7;
        entityExpAxe["Dead golden takin kid (male)"] = 4;
        entityExpAxe["Dead golden takin kid (female)"] = 4;
        entityExpAxe["Dead tur goat (male)"] = 5;
        entityExpAxe["Dead tur goat (female)"] = 5;
        entityExpAxe["Dead tur goat kid (male)"] = 2;
        entityExpAxe["Dead tur goat kid (female)"] = 2;
        entityExpAxe["Dead valais goat (male)"] = 6;
        entityExpAxe["Dead valais goat (female)"] = 6;
        entityExpAxe["Dead valais goat kid (male)"] = 3;
        entityExpAxe["Dead valais goat kid (female)"] = 3;
        #endregion
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
        #region vanilla
        entityExpPickaxe["Dead bighorn ram"] = 5;
        entityExpPickaxe["Dead bighorn ewe"] = 5;
        entityExpPickaxe["Dead bighorn lamb"] = 2;
        entityExpPickaxe["Dead rooster"] = 1;
        entityExpPickaxe["Dead hen"] = 1;
        entityExpPickaxe["Dead chick"] = 1;
        entityExpPickaxe["Dead boar"] = 3;
        entityExpPickaxe["Dead sow"] = 3;
        entityExpPickaxe["Dead piglet"] = 1;
        entityExpPickaxe["Dead male wolf"] = 4;
        entityExpPickaxe["Dead female wolf"] = 4;
        entityExpPickaxe["Dead wolf pup"] = 1;
        entityExpPickaxe["Dead male hyena"] = 4;
        entityExpPickaxe["Dead female hyena"] = 4;
        entityExpPickaxe["Dead hyena pup"] = 1;
        entityExpPickaxe["Dead male fox"] = 2;
        entityExpPickaxe["Dead female fox"] = 2;
        entityExpPickaxe["Dead fox pup"] = 1;
        entityExpPickaxe["Dead male raccoon"] = 2;
        entityExpPickaxe["Dead female raccoon"] = 2;
        entityExpPickaxe["Dead raccoon pup"] = 1;
        entityExpPickaxe["Dead hare"] = 1;
        entityExpPickaxe["Dead drifter"] = 5;
        entityExpPickaxe["Dead deep drifter"] = 6;
        entityExpPickaxe["Dead tainted drifter"] = 7;
        entityExpPickaxe["Dead corrupt drifter"] = 8;
        entityExpPickaxe["Dead nightmare drifter"] = 9;
        entityExpPickaxe["Dead double-headed drifter"] = 10;
        entityExpPickaxe["Dead bronze locust"] = 1;
        entityExpPickaxe["Dead corrupt locust"] = 5;
        entityExpPickaxe["Dead corrupt sawblade locust"] = 7;
        entityExpPickaxe["Dead bell"] = 10;
        entityExpPickaxe["Dead salmon"] = 1;
        entityExpPickaxe["Dead female black bear"] = 5;
        entityExpPickaxe["Dead female brown bear"] = 7;
        entityExpPickaxe["Dead female sun bear"] = 3;
        entityExpPickaxe["Dead female panda bear"] = 5;
        entityExpPickaxe["Dead female polar bear"] = 7;
        entityExpPickaxe["Dead male black bear"] = 5;
        entityExpPickaxe["Dead male brown bear"] = 7;
        entityExpPickaxe["Dead male sun bear"] = 3;
        entityExpPickaxe["Dead male panda bear"] = 5;
        entityExpPickaxe["Dead male polar bear"] = 7;
        entityExpPickaxe["Dead bronze locust (hacked)"] = 5;
        entityExpPickaxe["Dead corrupt locust (hacked)"] = 10;
        entityExpPickaxe["Dead gazelle (male)"] = 5;
        entityExpPickaxe["Dead gazelle (female)"] = 5;
        entityExpPickaxe["Dead gazelle calf"] = 2;
        entityExpPickaxe["Dead male moose"] = 6;
        entityExpPickaxe["Dead female moose"] = 6;
        entityExpPickaxe["Dead male moose calf"] = 2;
        entityExpPickaxe["Dead female moose calf"] = 2;
        entityExpPickaxe["Dead whitetail deer (male)"] = 3;
        entityExpPickaxe["Dead whitetail deer (female)"] = 3;
        entityExpPickaxe["Dead whitetail deer fawn (male)"] = 3;
        entityExpPickaxe["Dead whitetail deer fawn (female)"] = 3;
        entityExpPickaxe["Dead red brocket deer (male)"] = 3;
        entityExpPickaxe["Dead red brocket deer (female)"] = 3;
        entityExpPickaxe["Dead red brocket deer fawn (female)"] = 3;
        entityExpPickaxe["Dead red brocket deer fawn (male)"] = 3;
        entityExpPickaxe["Dead marsh deer (male)"] = 5;
        entityExpPickaxe["Dead marsh deer (female)"] = 5;
        entityExpPickaxe["Dead marsh deer fawn (male)"] = 3;
        entityExpPickaxe["Dead marsh deer fawn (female)"] = 3;
        entityExpPickaxe["Dead caribou (male)"] = 5;
        entityExpPickaxe["Dead caribou (female)"] = 5;
        entityExpPickaxe["Dead caribou calf (male)"] = 3;
        entityExpPickaxe["Dead caribou calf (female)"] = 3;
        entityExpPickaxe["Dead water deer (male)"] = 5;
        entityExpPickaxe["Dead water deer (female)"] = 5;
        entityExpPickaxe["Dead water deer fawn (male)"] = 3;
        entityExpPickaxe["Dead water deer fawn (female)"] = 3;
        entityExpPickaxe["Dead pudu deer (male)"] = 3;
        entityExpPickaxe["Dead pudu deer (female)"] = 3;
        entityExpPickaxe["Dead pudu deer fawn (male)"] = 1;
        entityExpPickaxe["Dead pudu deer fawn (female)"] = 1;
        entityExpPickaxe["Dead elk (male)"] = 6;
        entityExpPickaxe["Dead elk (female)"] = 6;
        entityExpPickaxe["Dead elk fawn (male)"] = 2;
        entityExpPickaxe["Dead elk fawn (female)"] = 2;
        entityExpPickaxe["Dead taruca deer (male)"] = 6;
        entityExpPickaxe["Dead taruca deer (female)"] = 6;
        entityExpPickaxe["Dead taruca deer fawn (male)"] = 7;
        entityExpPickaxe["Dead taruca deer fawn (female)"] = 7;
        entityExpPickaxe["Dead chital deer (male)"] = 4;
        entityExpPickaxe["Dead chital deer (female)"] = 4;
        entityExpPickaxe["Dead chital deer fawn (male)"] = 1;
        entityExpPickaxe["Dead chital deer fawn (female)"] = 1;
        entityExpPickaxe["Dead guemal deer (male)"] = 6;
        entityExpPickaxe["Dead guemal deer (female)"] = 6;
        entityExpPickaxe["Dead guemal deer fawn (male)"] = 2;
        entityExpPickaxe["Dead guemal deer fawn (female)"] = 2;
        entityExpPickaxe["Dead pampas deer (male)"] = 5;
        entityExpPickaxe["Dead pampas deer (female)"] = 5;
        entityExpPickaxe["Dead pampas deer fawn (male)"] = 2;
        entityExpPickaxe["Dead pampas deer fawn (female)"] = 2;
        entityExpPickaxe["Dead fallow deer (male)"] = 6;
        entityExpPickaxe["Dead fallow deer (female)"] = 6;
        entityExpPickaxe["Dead fallow deer fawn (male)"] = 2;
        entityExpPickaxe["Dead fallow deer fawn (female)"] = 2;
        entityExpPickaxe["Dead angora goat (male)"] = 6;
        entityExpPickaxe["Dead angora goat (female)"] = 6;
        entityExpPickaxe["Dead angora goat kid (male)"] = 2;
        entityExpPickaxe["Dead angora goat kid (female)"] = 2;
        entityExpPickaxe["Dead alpine ibex (male)"] = 7;
        entityExpPickaxe["Dead alpine ibex (female)"] = 7;
        entityExpPickaxe["Dead alpine ibex kid (male)"] = 3;
        entityExpPickaxe["Dead alpine ibex kid (female)"] = 3;
        entityExpPickaxe["Dead nubian ibex (male)"] = 7;
        entityExpPickaxe["Dead nubian ibex (female)"] = 7;
        entityExpPickaxe["Dead nubian ibex kid (male)"] = 3;
        entityExpPickaxe["Dead nubian ibex kid (female)"] = 3;
        entityExpPickaxe["Dead markhor goat kid(male)"] = 5;
        entityExpPickaxe["Dead markhor goat (female)"] = 5;
        entityExpPickaxe["Dead markhor goat kid (male)"] = 2;
        entityExpPickaxe["Dead markhor goat kid (female)"] = 2;
        entityExpPickaxe["Dead mountain goat (male)"] = 6;
        entityExpPickaxe["Dead mountain goat (female)"] = 6;
        entityExpPickaxe["Dead mountain goat kid (male)"] = 2;
        entityExpPickaxe["Dead mountain goat kid (female)"] = 2;
        entityExpPickaxe["Dead musk ox (male)"] = 8;
        entityExpPickaxe["Dead musk ox (female)"] = 8;
        entityExpPickaxe["Dead musk ox calf (male)"] = 3;
        entityExpPickaxe["Dead musk ox calf (female)"] = 3;
        entityExpPickaxe["Dead sihori goat (male)"] = 7;
        entityExpPickaxe["Dead sihori goat (female)"] = 7;
        entityExpPickaxe["Dead sihori goat kid (male)"] = 3;
        entityExpPickaxe["Dead sihori goat kid (female)"] = 3;
        entityExpPickaxe["Dead golden takin (male)"] = 7;
        entityExpPickaxe["Dead golden takin (female)"] = 7;
        entityExpPickaxe["Dead golden takin kid (male)"] = 4;
        entityExpPickaxe["Dead golden takin kid (female)"] = 4;
        entityExpPickaxe["Dead tur goat (male)"] = 5;
        entityExpPickaxe["Dead tur goat (female)"] = 5;
        entityExpPickaxe["Dead tur goat kid (male)"] = 2;
        entityExpPickaxe["Dead tur goat kid (female)"] = 2;
        entityExpPickaxe["Dead valais goat (male)"] = 6;
        entityExpPickaxe["Dead valais goat (female)"] = 6;
        entityExpPickaxe["Dead valais goat kid (male)"] = 3;
        entityExpPickaxe["Dead valais goat kid (female)"] = 3;
        #endregion
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
        #region vanilla
        entityExpShovel["Dead bighorn ram"] = 5;
        entityExpShovel["Dead bighorn ewe"] = 5;
        entityExpShovel["Dead bighorn lamb"] = 2;
        entityExpShovel["Dead rooster"] = 1;
        entityExpShovel["Dead hen"] = 1;
        entityExpShovel["Dead chick"] = 1;
        entityExpShovel["Dead boar"] = 3;
        entityExpShovel["Dead sow"] = 3;
        entityExpShovel["Dead piglet"] = 1;
        entityExpShovel["Dead male wolf"] = 4;
        entityExpShovel["Dead female wolf"] = 4;
        entityExpShovel["Dead wolf pup"] = 1;
        entityExpShovel["Dead male hyena"] = 4;
        entityExpShovel["Dead female hyena"] = 4;
        entityExpShovel["Dead hyena pup"] = 1;
        entityExpShovel["Dead male fox"] = 2;
        entityExpShovel["Dead female fox"] = 2;
        entityExpShovel["Dead fox pup"] = 1;
        entityExpShovel["Dead male raccoon"] = 2;
        entityExpShovel["Dead female raccoon"] = 2;
        entityExpShovel["Dead raccoon pup"] = 1;
        entityExpShovel["Dead hare"] = 1;
        entityExpShovel["Dead drifter"] = 5;
        entityExpShovel["Dead deep drifter"] = 6;
        entityExpShovel["Dead tainted drifter"] = 7;
        entityExpShovel["Dead corrupt drifter"] = 8;
        entityExpShovel["Dead nightmare drifter"] = 9;
        entityExpShovel["Dead double-headed drifter"] = 10;
        entityExpShovel["Dead bronze locust"] = 1;
        entityExpShovel["Dead corrupt locust"] = 5;
        entityExpShovel["Dead corrupt sawblade locust"] = 7;
        entityExpShovel["Dead bell"] = 10;
        entityExpShovel["Dead salmon"] = 1;
        entityExpShovel["Dead female black bear"] = 5;
        entityExpShovel["Dead female brown bear"] = 7;
        entityExpShovel["Dead female sun bear"] = 3;
        entityExpShovel["Dead female panda bear"] = 5;
        entityExpShovel["Dead female polar bear"] = 7;
        entityExpShovel["Dead male black bear"] = 5;
        entityExpShovel["Dead male brown bear"] = 7;
        entityExpShovel["Dead male sun bear"] = 3;
        entityExpShovel["Dead male panda bear"] = 5;
        entityExpShovel["Dead male polar bear"] = 7;
        entityExpShovel["Dead bronze locust (hacked)"] = 5;
        entityExpShovel["Dead corrupt locust (hacked)"] = 10;
        entityExpShovel["Dead gazelle (male)"] = 5;
        entityExpShovel["Dead gazelle (female)"] = 5;
        entityExpShovel["Dead gazelle calf"] = 2;
        entityExpShovel["Dead male moose"] = 6;
        entityExpShovel["Dead female moose"] = 6;
        entityExpShovel["Dead male moose calf"] = 2;
        entityExpShovel["Dead female moose calf"] = 2;
        entityExpShovel["Dead whitetail deer (male)"] = 3;
        entityExpShovel["Dead whitetail deer (female)"] = 3;
        entityExpShovel["Dead whitetail deer fawn (male)"] = 3;
        entityExpShovel["Dead whitetail deer fawn (female)"] = 3;
        entityExpShovel["Dead red brocket deer (male)"] = 3;
        entityExpShovel["Dead red brocket deer (female)"] = 3;
        entityExpShovel["Dead red brocket deer fawn (female)"] = 3;
        entityExpShovel["Dead red brocket deer fawn (male)"] = 3;
        entityExpShovel["Dead marsh deer (male)"] = 5;
        entityExpShovel["Dead marsh deer (female)"] = 5;
        entityExpShovel["Dead marsh deer fawn (male)"] = 3;
        entityExpShovel["Dead marsh deer fawn (female)"] = 3;
        entityExpShovel["Dead caribou (male)"] = 5;
        entityExpShovel["Dead caribou (female)"] = 5;
        entityExpShovel["Dead caribou calf (male)"] = 3;
        entityExpShovel["Dead caribou calf (female)"] = 3;
        entityExpShovel["Dead water deer (male)"] = 5;
        entityExpShovel["Dead water deer (female)"] = 5;
        entityExpShovel["Dead water deer fawn (male)"] = 3;
        entityExpShovel["Dead water deer fawn (female)"] = 3;
        entityExpShovel["Dead pudu deer (male)"] = 3;
        entityExpShovel["Dead pudu deer (female)"] = 3;
        entityExpShovel["Dead pudu deer fawn (male)"] = 1;
        entityExpShovel["Dead pudu deer fawn (female)"] = 1;
        entityExpShovel["Dead elk (male)"] = 6;
        entityExpShovel["Dead elk (female)"] = 6;
        entityExpShovel["Dead elk fawn (male)"] = 2;
        entityExpShovel["Dead elk fawn (female)"] = 2;
        entityExpShovel["Dead taruca deer (male)"] = 6;
        entityExpShovel["Dead taruca deer (female)"] = 6;
        entityExpShovel["Dead taruca deer fawn (male)"] = 7;
        entityExpShovel["Dead taruca deer fawn (female)"] = 7;
        entityExpShovel["Dead chital deer (male)"] = 4;
        entityExpShovel["Dead chital deer (female)"] = 4;
        entityExpShovel["Dead chital deer fawn (male)"] = 1;
        entityExpShovel["Dead chital deer fawn (female)"] = 1;
        entityExpShovel["Dead guemal deer (male)"] = 6;
        entityExpShovel["Dead guemal deer (female)"] = 6;
        entityExpShovel["Dead guemal deer fawn (male)"] = 2;
        entityExpShovel["Dead guemal deer fawn (female)"] = 2;
        entityExpShovel["Dead pampas deer (male)"] = 5;
        entityExpShovel["Dead pampas deer (female)"] = 5;
        entityExpShovel["Dead pampas deer fawn (male)"] = 2;
        entityExpShovel["Dead pampas deer fawn (female)"] = 2;
        entityExpShovel["Dead fallow deer (male)"] = 6;
        entityExpShovel["Dead fallow deer (female)"] = 6;
        entityExpShovel["Dead fallow deer fawn (male)"] = 2;
        entityExpShovel["Dead fallow deer fawn (female)"] = 2;
        entityExpShovel["Dead angora goat (male)"] = 6;
        entityExpShovel["Dead angora goat (female)"] = 6;
        entityExpShovel["Dead angora goat kid (male)"] = 2;
        entityExpShovel["Dead angora goat kid (female)"] = 2;
        entityExpShovel["Dead alpine ibex (male)"] = 7;
        entityExpShovel["Dead alpine ibex (female)"] = 7;
        entityExpShovel["Dead alpine ibex kid (male)"] = 3;
        entityExpShovel["Dead alpine ibex kid (female)"] = 3;
        entityExpShovel["Dead nubian ibex (male)"] = 7;
        entityExpShovel["Dead nubian ibex (female)"] = 7;
        entityExpShovel["Dead nubian ibex kid (male)"] = 3;
        entityExpShovel["Dead nubian ibex kid (female)"] = 3;
        entityExpShovel["Dead markhor goat kid(male)"] = 5;
        entityExpShovel["Dead markhor goat (female)"] = 5;
        entityExpShovel["Dead markhor goat kid (male)"] = 2;
        entityExpShovel["Dead markhor goat kid (female)"] = 2;
        entityExpShovel["Dead mountain goat (male)"] = 6;
        entityExpShovel["Dead mountain goat (female)"] = 6;
        entityExpShovel["Dead mountain goat kid (male)"] = 2;
        entityExpShovel["Dead mountain goat kid (female)"] = 2;
        entityExpShovel["Dead musk ox (male)"] = 8;
        entityExpShovel["Dead musk ox (female)"] = 8;
        entityExpShovel["Dead musk ox calf (male)"] = 3;
        entityExpShovel["Dead musk ox calf (female)"] = 3;
        entityExpShovel["Dead sihori goat (male)"] = 7;
        entityExpShovel["Dead sihori goat (female)"] = 7;
        entityExpShovel["Dead sihori goat kid (male)"] = 3;
        entityExpShovel["Dead sihori goat kid (female)"] = 3;
        entityExpShovel["Dead golden takin (male)"] = 7;
        entityExpShovel["Dead golden takin (female)"] = 7;
        entityExpShovel["Dead golden takin kid (male)"] = 4;
        entityExpShovel["Dead golden takin kid (female)"] = 4;
        entityExpShovel["Dead tur goat (male)"] = 5;
        entityExpShovel["Dead tur goat (female)"] = 5;
        entityExpShovel["Dead tur goat kid (male)"] = 2;
        entityExpShovel["Dead tur goat kid (female)"] = 2;
        entityExpShovel["Dead valais goat (male)"] = 6;
        entityExpShovel["Dead valais goat (female)"] = 6;
        entityExpShovel["Dead valais goat kid (male)"] = 3;
        entityExpShovel["Dead valais goat kid (female)"] = 3;
        #endregion
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
        #region vanilla
        entityExpSpear["Dead bighorn ram"] = 5;
        entityExpSpear["Dead bighorn ewe"] = 5;
        entityExpSpear["Dead bighorn lamb"] = 2;
        entityExpSpear["Dead rooster"] = 1;
        entityExpSpear["Dead hen"] = 1;
        entityExpSpear["Dead chick"] = 1;
        entityExpSpear["Dead boar"] = 3;
        entityExpSpear["Dead sow"] = 3;
        entityExpSpear["Dead piglet"] = 1;
        entityExpSpear["Dead male wolf"] = 4;
        entityExpSpear["Dead female wolf"] = 4;
        entityExpSpear["Dead wolf pup"] = 1;
        entityExpSpear["Dead male hyena"] = 4;
        entityExpSpear["Dead female hyena"] = 4;
        entityExpSpear["Dead hyena pup"] = 1;
        entityExpSpear["Dead male fox"] = 2;
        entityExpSpear["Dead female fox"] = 2;
        entityExpSpear["Dead fox pup"] = 1;
        entityExpSpear["Dead male raccoon"] = 2;
        entityExpSpear["Dead female raccoon"] = 2;
        entityExpSpear["Dead raccoon pup"] = 1;
        entityExpSpear["Dead hare"] = 1;
        entityExpSpear["Dead drifter"] = 5;
        entityExpSpear["Dead deep drifter"] = 6;
        entityExpSpear["Dead tainted drifter"] = 7;
        entityExpSpear["Dead corrupt drifter"] = 8;
        entityExpSpear["Dead nightmare drifter"] = 9;
        entityExpSpear["Dead double-headed drifter"] = 10;
        entityExpSpear["Dead bronze locust"] = 1;
        entityExpSpear["Dead corrupt locust"] = 5;
        entityExpSpear["Dead corrupt sawblade locust"] = 7;
        entityExpSpear["Dead bell"] = 10;
        entityExpSpear["Dead salmon"] = 1;
        entityExpSpear["Dead female black bear"] = 5;
        entityExpSpear["Dead female brown bear"] = 7;
        entityExpSpear["Dead female sun bear"] = 3;
        entityExpSpear["Dead female panda bear"] = 5;
        entityExpSpear["Dead female polar bear"] = 7;
        entityExpSpear["Dead male black bear"] = 5;
        entityExpSpear["Dead male brown bear"] = 7;
        entityExpSpear["Dead male sun bear"] = 3;
        entityExpSpear["Dead male panda bear"] = 5;
        entityExpSpear["Dead male polar bear"] = 7;
        entityExpSpear["Dead bronze locust (hacked)"] = 5;
        entityExpSpear["Dead corrupt locust (hacked)"] = 10;
        entityExpSpear["Dead gazelle (male)"] = 5;
        entityExpSpear["Dead gazelle (female)"] = 5;
        entityExpSpear["Dead gazelle calf"] = 2;
        entityExpSpear["Dead male moose"] = 6;
        entityExpSpear["Dead female moose"] = 6;
        entityExpSpear["Dead male moose calf"] = 2;
        entityExpSpear["Dead female moose calf"] = 2;
        entityExpSpear["Dead whitetail deer (male)"] = 3;
        entityExpSpear["Dead whitetail deer (female)"] = 3;
        entityExpSpear["Dead whitetail deer fawn (male)"] = 3;
        entityExpSpear["Dead whitetail deer fawn (female)"] = 3;
        entityExpSpear["Dead red brocket deer (male)"] = 3;
        entityExpSpear["Dead red brocket deer (female)"] = 3;
        entityExpSpear["Dead red brocket deer fawn (female)"] = 3;
        entityExpSpear["Dead red brocket deer fawn (male)"] = 3;
        entityExpSpear["Dead marsh deer (male)"] = 5;
        entityExpSpear["Dead marsh deer (female)"] = 5;
        entityExpSpear["Dead marsh deer fawn (male)"] = 3;
        entityExpSpear["Dead marsh deer fawn (female)"] = 3;
        entityExpSpear["Dead caribou (male)"] = 5;
        entityExpSpear["Dead caribou (female)"] = 5;
        entityExpSpear["Dead caribou calf (male)"] = 3;
        entityExpSpear["Dead caribou calf (female)"] = 3;
        entityExpSpear["Dead water deer (male)"] = 5;
        entityExpSpear["Dead water deer (female)"] = 5;
        entityExpSpear["Dead water deer fawn (male)"] = 3;
        entityExpSpear["Dead water deer fawn (female)"] = 3;
        entityExpSpear["Dead pudu deer (male)"] = 3;
        entityExpSpear["Dead pudu deer (female)"] = 3;
        entityExpSpear["Dead pudu deer fawn (male)"] = 1;
        entityExpSpear["Dead pudu deer fawn (female)"] = 1;
        entityExpSpear["Dead elk (male)"] = 6;
        entityExpSpear["Dead elk (female)"] = 6;
        entityExpSpear["Dead elk fawn (male)"] = 2;
        entityExpSpear["Dead elk fawn (female)"] = 2;
        entityExpSpear["Dead taruca deer (male)"] = 6;
        entityExpSpear["Dead taruca deer (female)"] = 6;
        entityExpSpear["Dead taruca deer fawn (male)"] = 7;
        entityExpSpear["Dead taruca deer fawn (female)"] = 7;
        entityExpSpear["Dead chital deer (male)"] = 4;
        entityExpSpear["Dead chital deer (female)"] = 4;
        entityExpSpear["Dead chital deer fawn (male)"] = 1;
        entityExpSpear["Dead chital deer fawn (female)"] = 1;
        entityExpSpear["Dead guemal deer (male)"] = 6;
        entityExpSpear["Dead guemal deer (female)"] = 6;
        entityExpSpear["Dead guemal deer fawn (male)"] = 2;
        entityExpSpear["Dead guemal deer fawn (female)"] = 2;
        entityExpSpear["Dead pampas deer (male)"] = 5;
        entityExpSpear["Dead pampas deer (female)"] = 5;
        entityExpSpear["Dead pampas deer fawn (male)"] = 2;
        entityExpSpear["Dead pampas deer fawn (female)"] = 2;
        entityExpSpear["Dead fallow deer (male)"] = 6;
        entityExpSpear["Dead fallow deer (female)"] = 6;
        entityExpSpear["Dead fallow deer fawn (male)"] = 2;
        entityExpSpear["Dead fallow deer fawn (female)"] = 2;
        entityExpSpear["Dead angora goat (male)"] = 6;
        entityExpSpear["Dead angora goat (female)"] = 6;
        entityExpSpear["Dead angora goat kid (male)"] = 2;
        entityExpSpear["Dead angora goat kid (female)"] = 2;
        entityExpSpear["Dead alpine ibex (male)"] = 7;
        entityExpSpear["Dead alpine ibex (female)"] = 7;
        entityExpSpear["Dead alpine ibex kid (male)"] = 3;
        entityExpSpear["Dead alpine ibex kid (female)"] = 3;
        entityExpSpear["Dead nubian ibex (male)"] = 7;
        entityExpSpear["Dead nubian ibex (female)"] = 7;
        entityExpSpear["Dead nubian ibex kid (male)"] = 3;
        entityExpSpear["Dead nubian ibex kid (female)"] = 3;
        entityExpSpear["Dead markhor goat kid(male)"] = 5;
        entityExpSpear["Dead markhor goat (female)"] = 5;
        entityExpSpear["Dead markhor goat kid (male)"] = 2;
        entityExpSpear["Dead markhor goat kid (female)"] = 2;
        entityExpSpear["Dead mountain goat (male)"] = 6;
        entityExpSpear["Dead mountain goat (female)"] = 6;
        entityExpSpear["Dead mountain goat kid (male)"] = 2;
        entityExpSpear["Dead mountain goat kid (female)"] = 2;
        entityExpSpear["Dead musk ox (male)"] = 8;
        entityExpSpear["Dead musk ox (female)"] = 8;
        entityExpSpear["Dead musk ox calf (male)"] = 3;
        entityExpSpear["Dead musk ox calf (female)"] = 3;
        entityExpSpear["Dead sihori goat (male)"] = 7;
        entityExpSpear["Dead sihori goat (female)"] = 7;
        entityExpSpear["Dead sihori goat kid (male)"] = 3;
        entityExpSpear["Dead sihori goat kid (female)"] = 3;
        entityExpSpear["Dead golden takin (male)"] = 7;
        entityExpSpear["Dead golden takin (female)"] = 7;
        entityExpSpear["Dead golden takin kid (male)"] = 4;
        entityExpSpear["Dead golden takin kid (female)"] = 4;
        entityExpSpear["Dead tur goat (male)"] = 5;
        entityExpSpear["Dead tur goat (female)"] = 5;
        entityExpSpear["Dead tur goat kid (male)"] = 2;
        entityExpSpear["Dead tur goat kid (female)"] = 2;
        entityExpSpear["Dead valais goat (male)"] = 6;
        entityExpSpear["Dead valais goat (female)"] = 6;
        entityExpSpear["Dead valais goat kid (male)"] = 3;
        entityExpSpear["Dead valais goat kid (female)"] = 3;
        #endregion

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
        expPerHarvestFarming["game:crop-turnip-5"] = 4;
        expPerHarvestFarming["game:crop-turnip-4"] = 1;
        expPerHarvestFarming["game:crop-carrot-6"] = 2;
        expPerHarvestFarming["game:crop-carrot-7"] = 5;
        expPerHarvestFarming["game:crop-flax-9"] = 8;
        expPerHarvestFarming["game:crop-flax-8"] = 4;
        expPerHarvestFarming["game:crop-onion-7"] = 5;
        expPerHarvestFarming["game:crop-onion-6"] = 3;
        expPerHarvestFarming["game:crop-spelt-9"] = 8;
        expPerHarvestFarming["game:crop-spelt-8"] = 4;
        expPerHarvestFarming["game:crop-parsnip-8"] = 7;
        expPerHarvestFarming["game:crop-parsnip-7"] = 3;
        expPerHarvestFarming["game:crop-rye-9"] = 8;
        expPerHarvestFarming["game:crop-rye-8"] = 4;
        expPerHarvestFarming["game:crop-rice-10"] = 10;
        expPerHarvestFarming["game:crop-rice-9"] = 5;
        expPerHarvestFarming["game:crop-soybean-11"] = 12;
        expPerHarvestFarming["game:crop-soybean-10"] = 5;
        expPerHarvestFarming["game:crop-amaranth-9"] = 7;
        expPerHarvestFarming["game:crop-amaranth-8"] = 3;
        expPerHarvestFarming["game:crop-cassava-9"] = 8;
        expPerHarvestFarming["game:crop-cassava-8"] = 3;
        expPerHarvestFarming["game:crop-peanut-9"] = 7;
        expPerHarvestFarming["game:crop-peanut-8"] = 2;
        expPerHarvestFarming["game:crop-pineapple-16"] = 20;
        expPerHarvestFarming["game:crop-pineapple-15"] = 9;
        expPerHarvestFarming["game:crop-sunflower-12"] = 10;
        expPerHarvestFarming["game:crop-sunflower-11"] = 6;
        expPerHarvestFarming["game:crop-pumpkin-8"] = 12;
        expPerHarvestFarming["game:crop-pumpkin-8"] = 5;
        expPerHarvestFarming["game:crop-cabbage-12"] = 14;
        expPerHarvestFarming["game:crop-cabbage-12"] = 6;
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