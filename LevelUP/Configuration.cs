using System;
using System.Collections.Generic;
using Vintagestory.API.Common;

namespace LevelUP;

#pragma warning disable CA2211
public static class Configuration
{
    #region baseconfigs
    public static bool enableHardcore = false;
    public static double hardcoreLosePercentage = 0.8;
    public static bool enableDurabilityMechanic = true;
    public static bool enableLevelHunter = true;
    public static bool enableLevelBow = true;
    public static bool enableLevelKnife = true;
    public static bool enableLevelSpear = true;
    public static bool enableLevelAxe = true;
    public static bool enableLevelPickaxe = true;
    public static bool enableLevelShovel = true;
    public static bool enableLevelFarming = true;
    public static bool enableLevelCooking = true;
    public static bool enableLevelVitality = true;
    public static bool enableLevelLeatherArmor = true;
    public static bool enableLevelChainArmor = true;
    public static bool disableServerChannel = false;
    public static bool enableExtendedLog = false;

    public static void UpdateBaseConfigurations(ICoreAPI api)
    {
        Dictionary<string, object> baseConfigs = api.Assets.Get(new AssetLocation("levelup:config/base.json")).ToObject<Dictionary<string, object>>();
        { //enableHardcore
            if (baseConfigs.TryGetValue("enableHardcore", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: enableHardcore is null");
                else if (value is not bool) Debug.Log($"CONFIGURATION ERROR: enableHardcore is not boolean is {value.GetType()}");
                else enableHardcore = (bool)value;
            else Debug.Log("CONFIGURATION ERROR: enableHardcore not set");
        }
        { //hardcoreLosePercentage
            if (baseConfigs.TryGetValue("hardcoreLosePercentage", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: hardcoreLosePercentage is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: hardcoreLosePercentage is not double is {value.GetType()}");
                else hardcoreLosePercentage = (double)value;
            else Debug.Log("CONFIGURATION ERROR: hardcoreLosePercentage not set");
        }
        { //enableDurabilityMechanic
            if (baseConfigs.TryGetValue("enableDurabilityMechanic", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: enableDurabilityMechanic is null");
                else if (value is not bool) Debug.Log($"CONFIGURATION ERROR: enableDurabilityMechanic is not boolean is {value.GetType()}");
                else enableDurabilityMechanic = (bool)value;
            else Debug.Log("CONFIGURATION ERROR: enableDurabilityMechanic not set");
        }
        { //enableLevelHunter
            if (baseConfigs.TryGetValue("enableLevelHunter", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: enableLevelHunter is null");
                else if (value is not bool) Debug.Log($"CONFIGURATION ERROR: enableLevelHunter is not boolean is {value.GetType()}");
                else enableLevelHunter = (bool)value;
            else Debug.Log("CONFIGURATION ERROR: enableLevelHunter not set");
        }
        { //enableLevelBow
            if (baseConfigs.TryGetValue("enableLevelBow", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: enableLevelBow is null");
                else if (value is not bool) Debug.Log($"CONFIGURATION ERROR: enableLevelBow is not boolean is {value.GetType()}");
                else enableLevelBow = (bool)value;
            else Debug.Log("CONFIGURATION ERROR: enableLevelBow not set");
        }
        { //enableLevelKnife
            if (baseConfigs.TryGetValue("enableLevelKnife", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: enableLevelKnife is null");
                else if (value is not bool) Debug.Log($"CONFIGURATION ERROR: enableLevelKnife is not boolean is {value.GetType()}");
                else enableLevelKnife = (bool)value;
            else Debug.Log("CONFIGURATION ERROR: enableLevelKnife not set");
        }
        { //enableLevelSpear
            if (baseConfigs.TryGetValue("enableLevelSpear", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: enableLevelSpear is null");
                else if (value is not bool) Debug.Log($"CONFIGURATION ERROR: enableLevelSpear is not boolean is {value.GetType()}");
                else enableLevelSpear = (bool)value;
            else Debug.Log("CONFIGURATION ERROR: enableLevelSpear not set");
        }
        { //enableLevelAxe
            if (baseConfigs.TryGetValue("enableLevelAxe", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: enableLevelAxe is null");
                else if (value is not bool) Debug.Log($"CONFIGURATION ERROR: enableLevelAxe is not boolean is {value.GetType()}");
                else enableLevelAxe = (bool)value;
            else Debug.Log("CONFIGURATION ERROR: enableLevelAxe not set");
        }
        { //enableLevelPickaxe
            if (baseConfigs.TryGetValue("enableLevelPickaxe", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: enableLevelPickaxe is null");
                else if (value is not bool) Debug.Log($"CONFIGURATION ERROR: enableLevelPickaxe is not boolean is {value.GetType()}");
                else enableLevelPickaxe = (bool)value;
            else Debug.Log("CONFIGURATION ERROR: enableLevelPickaxe not set");
        }
        { //enableLevelShovel
            if (baseConfigs.TryGetValue("enableLevelShovel", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: enableLevelShovel is null");
                else if (value is not bool) Debug.Log($"CONFIGURATION ERROR: enableLevelShovel is not boolean is {value.GetType()}");
                else enableLevelShovel = (bool)value;
            else Debug.Log("CONFIGURATION ERROR: enableLevelShovel not set");
        }
        { //enableLevelFarming
            if (baseConfigs.TryGetValue("enableLevelFarming", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: enableLevelFarming is null");
                else if (value is not bool) Debug.Log($"CONFIGURATION ERROR: enableLevelFarming is not boolean is {value.GetType()}");
                else enableLevelFarming = (bool)value;
            else Debug.Log("CONFIGURATION ERROR: enableLevelFarming not set");
        }

        { //enableLevelCooking
            if (baseConfigs.TryGetValue("enableLevelCooking", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: enableLevelCooking is null");
                else if (value is not bool) Debug.Log($"CONFIGURATION ERROR: enableLevelCooking is not boolean is {value.GetType()}");
                else enableLevelCooking = (bool)value;
            else Debug.Log("CONFIGURATION ERROR: enableLevelCooking not set");
        }
        { //enableLevelVitality
            if (baseConfigs.TryGetValue("enableLevelVitality", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: enableLevelVitality is null");
                else if (value is not bool) Debug.Log($"CONFIGURATION ERROR: enableLevelVitality is not boolean is {value.GetType()}");
                else enableLevelVitality = (bool)value;
            else Debug.Log("CONFIGURATION ERROR: enableLevelVitality not set");
        }
        { //enableLevelLeatherArmor
            if (baseConfigs.TryGetValue("enableLevelLeatherArmor", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: enableLevelLeatherArmor is null");
                else if (value is not bool) Debug.Log($"CONFIGURATION ERROR: enableLevelLeatherArmor is not boolean is {value.GetType()}");
                else enableLevelLeatherArmor = (bool)value;
            else Debug.Log("CONFIGURATION ERROR: enableLevelLeatherArmor not set");
        }
        { //enableLevelChainArmor
            if (baseConfigs.TryGetValue("enableLevelChainArmor", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: enableLevelChainArmor is null");
                else if (value is not bool) Debug.Log($"CONFIGURATION ERROR: enableLevelChainArmor is not boolean is {value.GetType()}");
                else enableLevelChainArmor = (bool)value;
            else Debug.Log("CONFIGURATION ERROR: enableLevelChainArmor not set");
        }
        { //disableServerChannel
            if (baseConfigs.TryGetValue("disableServerChannel", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: disableServerChannel is null");
                else if (value is not bool) Debug.Log($"CONFIGURATION ERROR: disableServerChannel is not boolean is {value.GetType()}");
                else disableServerChannel = (bool)value;
            else Debug.Log("CONFIGURATION ERROR: disableServerChannel not set");
        }
        { //enableExtendedLog
            if (baseConfigs.TryGetValue("enableExtendedLog", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: enableExtendedLog is null");
                else if (value is not bool) Debug.Log($"CONFIGURATION ERROR: enableExtendedLog is not boolean is {value.GetType()}");
                else enableExtendedLog = (bool)value;
            else Debug.Log("CONFIGURATION ERROR: enableExtendedLog not set");
        }
    }

    public static void LogConfigurations()
    {
        Debug.Log($"CONFIG: hunterEXPPerLevelBase, value: {hunterEXPPerLevelBase}");
        Debug.Log($"CONFIG: hunterEXPMultiplyPerLevel, value: {hunterEXPMultiplyPerLevel}");
        Debug.Log($"CONFIG: hunterBaseDamage, value: {hunterBaseDamage}");
        Debug.Log($"CONFIG: hunterIncrementDamagePerLevel, value: {hunterIncrementDamagePerLevel}");
        Debug.Log($"CONFIG: bowEXPPerHit, value: {bowEXPPerHit}");
        Debug.Log($"CONFIG: bowEXPPerLevelBase, value: {bowEXPPerLevelBase}");
        Debug.Log($"CONFIG: bowEXPMultiplyPerLevel, value: {bowEXPMultiplyPerLevel}");
        Debug.Log($"CONFIG: bowBaseDamage, value: {bowBaseDamage}");
        Debug.Log($"CONFIG: bowIncrementDamagePerLevel, value: {bowIncrementDamagePerLevel}");
        Debug.Log($"CONFIG: bowBaseDurabilityRestoreChance, value: {bowBaseDurabilityRestoreChance}");
        Debug.Log($"CONFIG: bowDurabilityRestoreChancePerLevel, value: {bowDurabilityRestoreChancePerLevel}");
        Debug.Log($"CONFIG: bowDurabilityRestoreEveryLevelReduceChance, value: {bowDurabilityRestoreEveryLevelReduceChance}");
        Debug.Log($"CONFIG: bowDurabilityRestoreReduceChanceForEveryLevel, value: {bowDurabilityRestoreReduceChanceForEveryLevel}");
        Debug.Log($"CONFIG: bowBaseChanceToNotLoseArrow, value: {bowBaseChanceToNotLoseArrow}");
        Debug.Log($"CONFIG: bowChanceToNotLoseArrowBaseIncreasePerLevel, value: {bowChanceToNotLoseArrowBaseIncreasePerLevel}");
        Debug.Log($"CONFIG: bowChanceToNotLoseArrowReduceIncreaseEveryLevel, value: {bowChanceToNotLoseArrowReduceIncreaseEveryLevel}");
        Debug.Log($"CONFIG: bowChanceToNotLoseArrowReduceQuantityEveryLevel, value: {bowChanceToNotLoseArrowReduceQuantityEveryLevel}");
        Debug.Log($"CONFIG: bowBaseAimAccuracy, value: {bowBaseAimAccuracy}");
        Debug.Log($"CONFIG: bowIncreaseAimAccuracyPerLevel, value: {bowIncreaseAimAccuracyPerLevel}");
        Debug.Log($"CONFIG: knifeEXPPerHit, value: {knifeEXPPerHit}");
        Debug.Log($"CONFIG: knifeEXPPerHarvest, value: {knifeEXPPerHarvest}");
        Debug.Log($"CONFIG: knifeEXPPerLevelBase, value: {knifeEXPPerLevelBase}");
        Debug.Log($"CONFIG: knifeEXPMultiplyPerLevel, value: {knifeEXPMultiplyPerLevel}");
        Debug.Log($"CONFIG: knifeBaseDamage, value: {knifeBaseDamage}");
        Debug.Log($"CONFIG: knifeIncrementDamagePerLevel, value: {knifeIncrementDamagePerLevel}");
        Debug.Log($"CONFIG: knifeBaseHarvestMultiply, value: {knifeBaseHarvestMultiply}");
        Debug.Log($"CONFIG: knifeIncrementHarvestMultiplyPerLevel, value: {knifeIncrementHarvestMultiplyPerLevel}");
        Debug.Log($"CONFIG: knifeBaseDurabilityRestoreChance, value: {knifeBaseDurabilityRestoreChance}");
        Debug.Log($"CONFIG: knifeDurabilityRestoreChancePerLevel, value: {knifeDurabilityRestoreChancePerLevel}");
        Debug.Log($"CONFIG: knifeDurabilityRestoreEveryLevelReduceChance, value: {knifeDurabilityRestoreEveryLevelReduceChance}");
        Debug.Log($"CONFIG: knifeDurabilityRestoreReduceChanceForEveryLevel, value: {knifeDurabilityRestoreReduceChanceForEveryLevel}");
        Debug.Log($"CONFIG: axeEXPPerHit, value: {axeEXPPerHit}");
        Debug.Log($"CONFIG: axeEXPPerBreaking, value: {axeEXPPerBreaking}");
        Debug.Log($"CONFIG: axeEXPPerTreeBreaking, value: {axeEXPPerTreeBreaking}");
        Debug.Log($"CONFIG: axeEXPPerLevelBase, value: {axeEXPPerLevelBase}");
        Debug.Log($"CONFIG: axeEXPMultiplyPerLevel, value: {axeEXPMultiplyPerLevel}");
        Debug.Log($"CONFIG: axeBaseDamage, value: {axeBaseDamage}");
        Debug.Log($"CONFIG: axeIncrementDamagePerLevel, value: {axeIncrementDamagePerLevel}");
        Debug.Log($"CONFIG: axeBaseMiningSpeed, value: {axeBaseMiningSpeed}");
        Debug.Log($"CONFIG: axeIncrementMiningSpeedMultiplyPerLevel, value: {axeIncrementMiningSpeedMultiplyPerLevel}");
        Debug.Log($"CONFIG: axeBaseDurabilityRestoreChance, value: {axeBaseDurabilityRestoreChance}");
        Debug.Log($"CONFIG: axeDurabilityRestoreChancePerLevel, value: {axeDurabilityRestoreChancePerLevel}");
        Debug.Log($"CONFIG: axeDurabilityRestoreEveryLevelReduceChance, value: {axeDurabilityRestoreEveryLevelReduceChance}");
        Debug.Log($"CONFIG: axeDurabilityRestoreReduceChanceForEveryLevel, value: {axeDurabilityRestoreReduceChanceForEveryLevel}");
        Debug.Log($"CONFIG: pickaxeEXPPerHit, value: {pickaxeEXPPerHit}");
        Debug.Log($"CONFIG: pickaxeEXPPerBreaking, value: {pickaxeEXPPerBreaking}");
        Debug.Log($"CONFIG: pickaxeEXPPerLevelBase, value: {pickaxeEXPPerLevelBase}");
        Debug.Log($"CONFIG: pickaxeEXPMultiplyPerLevel, value: {pickaxeEXPMultiplyPerLevel}");
        Debug.Log($"CONFIG: pickaxeBaseDamage, value: {pickaxeBaseDamage}");
        Debug.Log($"CONFIG: pickaxeIncrementDamagePerLevel, value: {pickaxeIncrementDamagePerLevel}");
        Debug.Log($"CONFIG: pickaxeBaseMiningSpeed, value: {pickaxeBaseMiningSpeed}");
        Debug.Log($"CONFIG: pickaxeIncrementMiningSpeedMultiplyPerLevel, value: {pickaxeIncrementMiningSpeedMultiplyPerLevel}");
        Debug.Log($"CONFIG: pickaxeBaseOreMultiply, value: {pickaxeBaseOreMultiply}");
        Debug.Log($"CONFIG: pickaxeIncrementOreMultiplyPerLevel, value: {pickaxeIncrementOreMultiplyPerLevel}");
        Debug.Log($"CONFIG: pickaxeBaseDurabilityRestoreChance, value: {pickaxeBaseDurabilityRestoreChance}");
        Debug.Log($"CONFIG: pickaxeDurabilityRestoreChancePerLevel, value: {pickaxeDurabilityRestoreChancePerLevel}");
        Debug.Log($"CONFIG: pickaxeDurabilityRestoreEveryLevelReduceChance, value: {pickaxeDurabilityRestoreEveryLevelReduceChance}");
        Debug.Log($"CONFIG: pickaxeDurabilityRestoreReduceChanceForEveryLevel, value: {pickaxeDurabilityRestoreReduceChanceForEveryLevel}");
        Debug.Log($"CONFIG: shovelEXPPerHit, value: {shovelEXPPerHit}");
        Debug.Log($"CONFIG: shovelEXPPerBreaking, value: {shovelEXPPerBreaking}");
        Debug.Log($"CONFIG: shovelEXPPerLevelBase, value: {shovelEXPPerLevelBase}");
        Debug.Log($"CONFIG: shovelEXPMultiplyPerLevel, value: {shovelEXPMultiplyPerLevel}");
        Debug.Log($"CONFIG: shovelBaseDamage, value: {shovelBaseDamage}");
        Debug.Log($"CONFIG: shovelIncrementDamagePerLevel, value: {shovelIncrementDamagePerLevel}");
        Debug.Log($"CONFIG: shovelBaseMiningSpeed, value: {shovelBaseMiningSpeed}");
        Debug.Log($"CONFIG: shovelIncrementMiningSpeedMultiplyPerLevel, value: {shovelIncrementMiningSpeedMultiplyPerLevel}");
        Debug.Log($"CONFIG: shovelBaseDurabilityRestoreChance, value: {shovelBaseDurabilityRestoreChance}");
        Debug.Log($"CONFIG: shovelDurabilityRestoreChancePerLevel, value: {shovelDurabilityRestoreChancePerLevel}");
        Debug.Log($"CONFIG: shovelDurabilityRestoreEveryLevelReduceChance, value: {shovelDurabilityRestoreEveryLevelReduceChance}");
        Debug.Log($"CONFIG: shovelDurabilityRestoreReduceChanceForEveryLevel, value: {shovelDurabilityRestoreReduceChanceForEveryLevel}");
        Debug.Log($"CONFIG: spearEXPPerHit, value: {spearEXPPerHit}");
        Debug.Log($"CONFIG: spearEXPPerThrow, value: {spearEXPPerThrow}");
        Debug.Log($"CONFIG: spearEXPPerLevelBase, value: {spearEXPPerLevelBase}");
        Debug.Log($"CONFIG: spearEXPMultiplyPerLevel, value: {spearEXPMultiplyPerLevel}");
        Debug.Log($"CONFIG: spearBaseDamage, value: {spearBaseDamage}");
        Debug.Log($"CONFIG: spearIncrementDamagePerLevel, value: {spearIncrementDamagePerLevel}");
        Debug.Log($"CONFIG: spearBaseDurabilityRestoreChance, value: {spearBaseDurabilityRestoreChance}");
        Debug.Log($"CONFIG: spearDurabilityRestoreChancePerLevel, value: {spearDurabilityRestoreChancePerLevel}");
        Debug.Log($"CONFIG: spearDurabilityRestoreEveryLevelReduceChance, value: {spearDurabilityRestoreEveryLevelReduceChance}");
        Debug.Log($"CONFIG: spearDurabilityRestoreReduceChanceForEveryLevel, value: {spearDurabilityRestoreReduceChanceForEveryLevel}");
        Debug.Log($"CONFIG: farmingEXPPerTill, value: {farmingEXPPerTill}");
        Debug.Log($"CONFIG: farmingEXPPerLevelBase, value: {farmingEXPPerLevelBase}");
        Debug.Log($"CONFIG: farmingEXPMultiplyPerLevel, value: {farmingEXPMultiplyPerLevel}");
        Debug.Log($"CONFIG: farmingBaseHarvestMultiply, value: {farmingBaseHarvestMultiply}");
        Debug.Log($"CONFIG: farmingIncrementHarvestMultiplyPerLevel, value: {farmingIncrementHarvestMultiplyPerLevel}");
        Debug.Log($"CONFIG: farmingBaseDurabilityRestoreChance, value: {farmingBaseDurabilityRestoreChance}");
        Debug.Log($"CONFIG: farmingDurabilityRestoreChancePerLevel, value: {farmingDurabilityRestoreChancePerLevel}");
        Debug.Log($"CONFIG: farmingDurabilityRestoreEveryLevelReduceChance, value: {farmingDurabilityRestoreEveryLevelReduceChance}");
        Debug.Log($"CONFIG: farmingDurabilityRestoreReduceChanceForEveryLevel, value: {farmingDurabilityRestoreReduceChanceForEveryLevel}");
        Debug.Log($"CONFIG: cookingBaseExpPerCooking, value: {cookingBaseExpPerCooking}");
        Debug.Log($"CONFIG: cookingEXPPerLevelBase, value: {cookingEXPPerLevelBase}");
        Debug.Log($"CONFIG: cookingEXPMultiplyPerLevel, value: {cookingEXPMultiplyPerLevel}");
        Debug.Log($"CONFIG: cookingBaseFreshHoursMultiply, value: {cookingBaseFreshHoursMultiply}");
        Debug.Log($"CONFIG: cookingFreshHoursMultiplyPerLevel, value: {cookingFreshHoursMultiplyPerLevel}");
        Debug.Log($"CONFIG: cookingBaseChanceToIncreaseServings, value: {cookingBaseChanceToIncreaseServings}");
        Debug.Log($"CONFIG: cookingIncrementChanceToIncreaseServings, value: {cookingIncrementChanceToIncreaseServings}");
        Debug.Log($"CONFIG: cookingEveryChanceToIncreaseServingsReduceChance, value: {cookingEveryChanceToIncreaseServingsReduceChance}");
        Debug.Log($"CONFIG: cookingChanceToIncreaseServingsReducerTotal, value: {cookingChanceToIncreaseServingsReducerTotal}");
        Debug.Log($"CONFIG: cookingBaseRollsChanceToIncreaseServings, value: {cookingBaseRollsChanceToIncreaseServings}");
        Debug.Log($"CONFIG: cookingEarnRollsChanceToIncreaseServingsEveryLevel, value: {cookingEarnRollsChanceToIncreaseServingsEveryLevel}");
        Debug.Log($"CONFIG: cookingEarnRollsChanceToIncreaseServingsQuantity, value: {cookingEarnRollsChanceToIncreaseServingsQuantity}");
        Debug.Log($"CONFIG: vitalityEXPPerReceiveHit, value: {vitalityEXPPerReceiveHit}");
        Debug.Log($"CONFIG: vitalityEXPMultiplyByDamage, value: {vitalityEXPMultiplyByDamage}");
        Debug.Log($"CONFIG: vitalityEXPIncreaseByAmountDamage, value: {vitalityEXPIncreaseByAmountDamage}");
        Debug.Log($"CONFIG: vitalityEXPPerLevelBase, value: {vitalityEXPPerLevelBase}");
        Debug.Log($"CONFIG: vitalityEXPMultiplyPerLevel, value: {vitalityEXPMultiplyPerLevel}");
        Debug.Log($"CONFIG: vitalityHPIncreasePerLevel, value: {vitalityHPIncreasePerLevel}");
        Debug.Log($"CONFIG: vitalityBaseHP, value: {vitalityBaseHP}");
        Debug.Log($"CONFIG: leatherArmorEXPPerReceiveHit, value: {leatherArmorEXPPerReceiveHit}");
        Debug.Log($"CONFIG: leatherArmorEXPMultiplyByDamage, value: {leatherArmorEXPMultiplyByDamage}");
        Debug.Log($"CONFIG: leatherArmorEXPIncreaseByAmountDamage, value: {leatherArmorEXPIncreaseByAmountDamage}");
        Debug.Log($"CONFIG: leatherArmorEXPPerLevelBase, value: {leatherArmorEXPPerLevelBase}");
        Debug.Log($"CONFIG: leatherArmorEXPMultiplyPerLevel, value: {leatherArmorEXPMultiplyPerLevel}");
        Debug.Log($"CONFIG: leatherArmorBaseDamageReduction, value: {leatherArmorBaseDamageReduction}");
        Debug.Log($"CONFIG: leatherArmorDamageReductionPerLevel, value: {leatherArmorDamageReductionPerLevel}");
        Debug.Log($"CONFIG: leatherArmorBaseDurabilityRestoreChance, value: {leatherArmorBaseDurabilityRestoreChance}");
        Debug.Log($"CONFIG: leatherArmorDurabilityRestoreChancePerLevel, value: {leatherArmorDurabilityRestoreChancePerLevel}");
        Debug.Log($"CONFIG: leatherArmorDurabilityRestoreEveryLevelReduceChance, value: {leatherArmorDurabilityRestoreEveryLevelReduceChance}");
        Debug.Log($"CONFIG: leatherArmorDurabilityRestoreReduceChanceForEveryLevel, value: {leatherArmorDurabilityRestoreReduceChanceForEveryLevel}");
        Debug.Log($"CONFIG: chainArmorEXPPerReceiveHit, value: {chainArmorEXPPerReceiveHit}");
        Debug.Log($"CONFIG: chainArmorEXPMultiplyByDamage, value: {chainArmorEXPMultiplyByDamage}");
        Debug.Log($"CONFIG: chainArmorEXPIncreaseByAmountDamage, value: {chainArmorEXPIncreaseByAmountDamage}");
        Debug.Log($"CONFIG: chainArmorEXPPerLevelBase, value: {chainArmorEXPPerLevelBase}");
        Debug.Log($"CONFIG: chainArmorEXPMultiplyPerLevel, value: {chainArmorEXPMultiplyPerLevel}");
        Debug.Log($"CONFIG: chainArmorBaseDamageReduction, value: {chainArmorBaseDamageReduction}");
        Debug.Log($"CONFIG: chainArmorDamageReductionPerLevel, value: {chainArmorDamageReductionPerLevel}");
        Debug.Log($"CONFIG: chainArmorBaseDurabilityRestoreChance, value: {chainArmorBaseDurabilityRestoreChance}");
        Debug.Log($"CONFIG: chainArmorDurabilityRestoreChancePerLevel, value: {chainArmorDurabilityRestoreChancePerLevel}");
        Debug.Log($"CONFIG: chainArmorDurabilityRestoreEveryLevelReduceChance, value: {chainArmorDurabilityRestoreEveryLevelReduceChance}");
        Debug.Log($"CONFIG: chainArmorDurabilityRestoreReduceChanceForEveryLevel, value: {chainArmorDurabilityRestoreReduceChanceForEveryLevel}");
        Debug.Log($"CONFIG: enableHardcore, value: {enableHardcore}");
        Debug.Log($"CONFIG: hardcoreLosePercentage, value: {hardcoreLosePercentage}");
        Debug.Log($"CONFIG: enableDurabilityMechanic, value: {enableDurabilityMechanic}");
        Debug.Log($"CONFIG: enableLevelHunter, value: {enableLevelHunter}");
        Debug.Log($"CONFIG: enableLevelBow, value: {enableLevelBow}");
        Debug.Log($"CONFIG: enableLevelKnife, value: {enableLevelKnife}");
        Debug.Log($"CONFIG: enableLevelSpear, value: {enableLevelSpear}");
        Debug.Log($"CONFIG: enableLevelAxe, value: {enableLevelAxe}");
        Debug.Log($"CONFIG: enableLevelPickaxe, value: {enableLevelPickaxe}");
        Debug.Log($"CONFIG: enableLevelShovel, value: {enableLevelShovel}");
        Debug.Log($"CONFIG: enableLevelFarming, value: {enableLevelFarming}");
        Debug.Log($"CONFIG: enableLevelCooking, value: {enableLevelCooking}");
        Debug.Log($"CONFIG: enableLevelVitality, value: {enableLevelVitality}");
        Debug.Log($"CONFIG: enableLevelLeatherArmor, value: {enableLevelLeatherArmor}");
        Debug.Log($"CONFIG: enableLevelChainArmor, value: {enableLevelChainArmor}");
        Debug.Log($"CONFIG: disableServerChannel, value: {disableServerChannel}");
        Debug.Log($"CONFIG: enableExtendedLog, value: {enableExtendedLog}");
    }
    #endregion
    public static int GetLevelByLevelTypeEXP(string levelType, ulong exp)
    {
        switch (levelType)
        {
            case "Hunter": return HunterGetLevelByEXP(exp);
            case "Bow": return BowGetLevelByEXP(exp);
            case "Knife": return KnifeGetLevelByEXP(exp);
            case "Axe": return AxeGetLevelByEXP(exp);
            case "Pickaxe": return PickaxeGetLevelByEXP(exp);
            case "Shovel": return ShovelGetLevelByEXP(exp);
            case "Spear": return SpearGetLevelByEXP(exp);
            case "Farming": return FarmingGetLevelByEXP(exp);
            case "Cooking": return CookingGetLevelByEXP(exp);
            case "Vitality": return VitalityGetLevelByEXP(exp);
            case "LeatherArmor": return LeatherArmorGetLevelByEXP(exp);
            case "ChainArmor": return ChainArmorGetLevelByEXP(exp);
            default: break;
        }
        Debug.Log($"WARNING: {levelType} doesn't belong to the function GetLevelByLevelTypeEXP did you forget to add it? check the Configuratiin.cs");
        return 1;
    }

    public static float GetMiningSpeedByLevelTypeEXP(string levelType, ulong exp)
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
        entityExpHunter.Clear();
        Dictionary<string, object> tmpentityExpHunter = api.Assets.Get(new AssetLocation("levelup:config/entityexp/hunter.json")).ToObject<Dictionary<string, object>>();
        foreach (KeyValuePair<string, object> pair in tmpentityExpHunter)
        {
            if (pair.Value is long value) entityExpHunter.Add(pair.Key, (int)value);
            else Debug.Log($"CONFIGURATION ERROR: entityExpHunter {pair.Key} is not int");
        }

        Debug.Log("Hunter configuration set");
    }

    public static int HunterGetLevelByEXP(ulong exp)
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

    public static float HunterGetDamageMultiplyByEXP(ulong exp)
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

        baseDamage += baseDamage * incrementDamage;
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
    private static float bowBaseAimAccuracy = 1.0f;
    private static float bowIncreaseAimAccuracyPerLevel = 0.5f;

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
        { //bowBaseAimAccuracy
            if (bowLevelStats.TryGetValue("bowBaseAimAccuracy", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: bowBaseAimAccuracy is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: bowBaseAimAccuracy is not double is {value.GetType()}");
                else bowBaseAimAccuracy = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: bowBaseAimAccuracy not set");
        }
        { //bowIncreaseAimAccuracyPerLevel
            if (bowLevelStats.TryGetValue("bowIncreaseAimAccuracyPerLevel", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: bowIncreaseAimAccuracyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: bowIncreaseAimAccuracyPerLevel is not double is {value.GetType()}");
                else bowIncreaseAimAccuracyPerLevel = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: bowIncreaseAimAccuracyPerLevel not set");
        }

        // Get entity exp
        entityExpBow.Clear();
        Dictionary<string, object> tmpentityExpBow = api.Assets.Get(new AssetLocation("levelup:config/entityexp/bow.json")).ToObject<Dictionary<string, object>>();
        foreach (KeyValuePair<string, object> pair in tmpentityExpBow)
        {
            if (pair.Value is long value) entityExpBow.Add(pair.Key, (int)value);
            else Debug.Log($"CONFIGURATION ERROR: entityExpBow {pair.Key} is not int");
        }

        Debug.Log("Bow configuration set");
    }

    public static int BowGetLevelByEXP(ulong exp)
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

    public static float BowGetDamageMultiplyByEXP(ulong exp)
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

        baseDamage += baseDamage * incrementDamage;
        return baseDamage;
    }

    public static bool BowRollChanceToNotReduceDurabilityByEXP(ulong exp)
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

    public static float BowGetChanceToNotLoseArrowByEXP(ulong exp)
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

    public static float BowGetAimAccuracyByEXP(ulong exp)
    {

        int level = BowGetLevelByEXP(exp);
        // Exp base for level
        float accuracyPerLevelBase = bowBaseAimAccuracy;
        while (level > 1)
        {
            accuracyPerLevelBase += bowIncreaseAimAccuracyPerLevel;
            level -= 1;
        }
        return accuracyPerLevelBase;
    }

    #endregion

    #region knife
    public static readonly Dictionary<string, int> entityExpKnife = [];
    private static int knifeEXPPerHit = 1;
    private static int knifeEXPPerHarvest = 5;

    private static int knifeEXPPerLevelBase = 10;
    private static double knifeEXPMultiplyPerLevel = 1.3;
    private static float knifeBaseDamage = 1.0f;
    private static float knifeIncrementDamagePerLevel = 0.1f;
    private static float knifeBaseHarvestMultiply = 0.5f;
    private static float knifeIncrementHarvestMultiplyPerLevel = 0.2f;
    private static float knifeBaseDurabilityRestoreChance = 0.0f;
    private static float knifeDurabilityRestoreChancePerLevel = 2.0f;
    private static int knifeDurabilityRestoreEveryLevelReduceChance = 10;
    private static float knifeDurabilityRestoreReduceChanceForEveryLevel = 0.5f;

    public static int ExpPerHitKnife => knifeEXPPerHit;
    public static int ExpPerHarvestKnife => knifeEXPPerHarvest;

    public static void PopulateKnifeConfiguration(ICoreAPI api)
    {
        Dictionary<string, object> knifeLevelStats = api.Assets.Get(new AssetLocation("levelup:config/levelstats/knife.json")).ToObject<Dictionary<string, object>>();
        { //knifeEXPPerLevelBase
            if (knifeLevelStats.TryGetValue("knifeEXPPerLevelBase", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: knifeEXPPerLevelBase is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: knifeEXPPerLevelBase is not int is {value.GetType()}");
                else knifeEXPPerLevelBase = (int)(long)value;
            else Debug.Log("CONFIGURATION ERROR: knifeEXPPerLevelBase not set");
        }
        { //knifeEXPMultiplyPerLevel
            if (knifeLevelStats.TryGetValue("knifeEXPMultiplyPerLevel", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: knifeEXPMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: knifeEXPMultiplyPerLevel is not double is {value.GetType()}");
                else knifeEXPMultiplyPerLevel = (double)value;
            else Debug.Log("CONFIGURATION ERROR: knifeEXPMultiplyPerLevel not set");
        }
        { //knifeBaseDamage
            if (knifeLevelStats.TryGetValue("knifeBaseDamage", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: knifeBaseDamage is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: knifeBaseDamage is not double is {value.GetType()}");
                else knifeBaseDamage = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: knifeBaseDamage not set");
        }
        { //knifeIncrementDamagePerLevel
            if (knifeLevelStats.TryGetValue("knifeIncrementDamagePerLevel", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: knifeIncrementDamagePerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: knifeIncrementDamagePerLevel is not double is {value.GetType()}");
                else knifeIncrementDamagePerLevel = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: knifeIncrementDamagePerLevel not set");
        }
        { //knifeEXPPerHit
            if (knifeLevelStats.TryGetValue("knifeEXPPerHit", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: knifeEXPPerHit is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: knifeEXPPerHit is not int is {value.GetType()}");
                else knifeEXPPerHit = (int)(long)value;
            else Debug.Log("CONFIGURATION ERROR: knifeEXPPerHit not set");
        }
        { //knifeEXPPerHarvest
            if (knifeLevelStats.TryGetValue("knifeEXPPerHarvest", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: knifeEXPPerHarvest is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: knifeEXPPerHarvest is not int is {value.GetType()}");
                else knifeEXPPerHarvest = (int)(long)value;
            else Debug.Log("CONFIGURATION ERROR: knifeEXPPerHarvest not set");
        }
        { //knifeBaseHarvestMultiply
            if (knifeLevelStats.TryGetValue("knifeBaseHarvestMultiply", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: knifeBaseHarvestMultiply is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: knifeBaseHarvestMultiply is not double is {value.GetType()}");
                else knifeBaseHarvestMultiply = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: knifeBaseHarvestMultiply not set");
        }
        { //knifeIncrementHarvestMultiplyPerLevel
            if (knifeLevelStats.TryGetValue("knifeIncrementHarvestMultiplyPerLevel", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: knifeIncrementHarvestMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: knifeIncrementHarvestMultiplyPerLevel is not double is {value.GetType()}");
                else knifeIncrementHarvestMultiplyPerLevel = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: knifeIncrementHarvestMultiplyPerLevel not set");
        }
        { //knifeBaseDurabilityRestoreChance
            if (knifeLevelStats.TryGetValue("knifeBaseDurabilityRestoreChance", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: knifeBaseDurabilityRestoreChance is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: knifeBaseDurabilityRestoreChance is not double is {value.GetType()}");
                else knifeBaseDurabilityRestoreChance = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: knifeBaseDurabilityRestoreChance not set");
        }
        { //knifeDurabilityRestoreChancePerLevel
            if (knifeLevelStats.TryGetValue("knifeDurabilityRestoreChancePerLevel", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: knifeDurabilityRestoreChancePerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: knifeDurabilityRestoreChancePerLevel is not double is {value.GetType()}");
                else knifeDurabilityRestoreChancePerLevel = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: knifeDurabilityRestoreChancePerLevel not set");
        }
        { //knifeDurabilityRestoreEveryLevelReduceChance
            if (knifeLevelStats.TryGetValue("knifeDurabilityRestoreEveryLevelReduceChance", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: knifeDurabilityRestoreEveryLevelReduceChance is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: knifeDurabilityRestoreEveryLevelReduceChance is not int is {value.GetType()}");
                else knifeDurabilityRestoreEveryLevelReduceChance = (int)(long)value;
            else Debug.Log("CONFIGURATION ERROR: knifeDurabilityRestoreEveryLevelReduceChance not set");
        }
        { //knifeDurabilityRestoreReduceChanceForEveryLevel
            if (knifeLevelStats.TryGetValue("knifeDurabilityRestoreReduceChanceForEveryLevel", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: knifeDurabilityRestoreReduceChanceForEveryLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: knifeDurabilityRestoreReduceChanceForEveryLevel is not double is {value.GetType()}");
                else knifeDurabilityRestoreReduceChanceForEveryLevel = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: knifeDurabilityRestoreReduceChanceForEveryLevel not set");
        }

        // Get entity exp
        entityExpKnife.Clear();
        Dictionary<string, object> tmpentityExpKnife = api.Assets.Get(new AssetLocation("levelup:config/entityexp/knife.json")).ToObject<Dictionary<string, object>>();
        foreach (KeyValuePair<string, object> pair in tmpentityExpKnife)
        {
            if (pair.Value is long value) entityExpKnife.Add(pair.Key, (int)value);
            else Debug.Log($"CONFIGURATION ERROR: entityExpKnife {pair.Key} is not int");
        }

        Debug.Log("Knife configuration set");
    }

    public static int KnifeGetLevelByEXP(ulong exp)
    {

        int level = 0;
        // Exp base for level
        double expPerLevelBase = knifeEXPPerLevelBase;
        double calcExp = double.Parse(exp.ToString());
        while (calcExp > 0)
        {
            level += 1;
            calcExp -= expPerLevelBase;
            // 10 percentage increasing per level
            expPerLevelBase *= knifeEXPMultiplyPerLevel;
        }
        return level;
    }

    public static float KnifeGetDamageMultiplyByEXP(ulong exp)
    {
        float baseDamage = knifeBaseDamage;
        int level = KnifeGetLevelByEXP(exp);

        float incrementDamage = knifeIncrementDamagePerLevel;
        float multiply = 0.0f;
        while (level > 1)
        {
            multiply += incrementDamage;
            level -= 1;
        }

        baseDamage += baseDamage * incrementDamage;
        return baseDamage;
    }

    public static float KnifeGetHarvestMultiplyByEXP(ulong exp)
    {
        float baseMultiply = knifeBaseHarvestMultiply;
        int level = KnifeGetLevelByEXP(exp);

        float incrementMultiply = knifeIncrementHarvestMultiplyPerLevel;
        float multiply = 0.0f;
        while (level > 1)
        {
            multiply += incrementMultiply;
            level -= 1;
        }
        baseMultiply += baseMultiply * multiply;
        return baseMultiply;
    }

    public static bool KnifeRollChanceToNotReduceDurabilityByEXP(ulong exp)
    {
        int level = KnifeGetLevelByEXP(exp);
        float baseChanceToNotReduce = knifeBaseDurabilityRestoreChance;
        float chanceToNotReduce = knifeDurabilityRestoreChancePerLevel;
        while (level > 1)
        {
            level -= 1;
            // Every {} levels reduce the durability chance multiplicator
            if (level % knifeDurabilityRestoreEveryLevelReduceChance == 0)
                chanceToNotReduce -= knifeDurabilityRestoreReduceChanceForEveryLevel;
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
    private static int axeEXPPerTreeBreaking = 10;

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
    public static int ExpPerTreeBreakingAxe => axeEXPPerTreeBreaking;

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
        { //axeEXPPerTreeBreaking
            if (axeLevelStats.TryGetValue("axeEXPPerTreeBreaking", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: axeEXPPerTreeBreaking is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: axeEXPPerTreeBreaking is not int is {value.GetType()}");
                else axeEXPPerTreeBreaking = (int)(long)value;
            else Debug.Log("CONFIGURATION ERROR: axeEXPPerTreeBreaking not set");
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
        entityExpAxe.Clear();
        Dictionary<string, object> tmpentityExpAxe = api.Assets.Get(new AssetLocation("levelup:config/entityexp/axe.json")).ToObject<Dictionary<string, object>>();
        foreach (KeyValuePair<string, object> pair in tmpentityExpAxe)
        {
            if (pair.Value is long value) entityExpAxe.Add(pair.Key, (int)value);
            else Debug.Log($"CONFIGURATION ERROR: entityExpAxe {pair.Key} is not int");
        }

        Debug.Log("Axe configuration set");
    }

    public static int AxeGetLevelByEXP(ulong exp)
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

    public static float AxeGetDamageMultiplyByEXP(ulong exp)
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

        baseDamage += baseDamage * incrementDamage;
        return baseDamage;
    }

    public static float AxeGetMiningMultiplyByEXP(ulong exp)
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

        baseSpeed += baseSpeed * incrementSpeed;
        return baseSpeed;
    }

    public static bool AxeRollChanceToNotReduceDurabilityByEXP(ulong exp)
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
    public static readonly Dictionary<string, int> oresExpPickaxe = [];
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
        entityExpPickaxe.Clear();
        Dictionary<string, object> tmpentityExpPickaxe = api.Assets.Get(new AssetLocation("levelup:config/entityexp/pickaxe.json")).ToObject<Dictionary<string, object>>();
        foreach (KeyValuePair<string, object> pair in tmpentityExpPickaxe)
        {
            if (pair.Value is long value) entityExpPickaxe.Add(pair.Key, (int)value);
            else Debug.Log($"CONFIGURATION ERROR: entityExpPickaxe {pair.Key} is not int");
        }

        // Get ores exp
        oresExpPickaxe.Clear();
        Dictionary<string, object> tmporesExpPickaxe = api.Assets.Get(new AssetLocation("levelup:config/levelstats/pickaxesores.json")).ToObject<Dictionary<string, object>>();
        foreach (KeyValuePair<string, object> pair in tmporesExpPickaxe)
        {
            if (pair.Value is long value) oresExpPickaxe.Add(pair.Key, (int)value);
            else Debug.Log($"CONFIGURATION ERROR: oresExpPickaxe {pair.Key} is not int");
        }


        Debug.Log("Pickaxe configuration set");
    }

    public static int PickaxeGetLevelByEXP(ulong exp)
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

    public static float PickaxeGetOreMultiplyByEXP(ulong exp)
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

        baseMultiply += baseMultiply * multiply;
        return baseMultiply;
    }

    public static float PickaxeGetDamageMultiplyByEXP(ulong exp)
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

        baseDamage += baseDamage * incrementDamage;
        return baseDamage;
    }

    public static float PickaxeGetMiningMultiplyByEXP(ulong exp)
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

        baseSpeed += baseSpeed * incrementSpeed;
        return baseSpeed;
    }

    public static bool PickaxeRollChanceToNotReduceDurabilityByEXP(ulong exp)
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
        entityExpShovel.Clear();
        Dictionary<string, object> tmpentityExpShovel = api.Assets.Get(new AssetLocation("levelup:config/entityexp/shovel.json")).ToObject<Dictionary<string, object>>();
        foreach (KeyValuePair<string, object> pair in tmpentityExpShovel)
        {
            if (pair.Value is long value) entityExpShovel.Add(pair.Key, (int)value);
            else Debug.Log($"CONFIGURATION ERROR: entityExpShovel {pair.Key} is not int");
        }

        Debug.Log("Shovel configuration set");
    }

    public static int ShovelGetLevelByEXP(ulong exp)
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

    public static float ShovelGetDamageMultiplyByEXP(ulong exp)
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

        baseDamage += baseDamage * incrementDamage;
        return baseDamage;
    }

    public static float ShovelGetMiningMultiplyByEXP(ulong exp)
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

        baseSpeed += baseSpeed * incrementSpeed;
        return baseSpeed;
    }

    public static bool ShovelRollChanceToNotReduceDurabilityByEXP(ulong exp)
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
    private static float spearBaseAimAccuracy = 1.0f;
    private static float spearIncreaseAimAccuracyPerLevel = 0.5f;


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
        { //spearBaseAimAccuracy
            if (spearLevelStats.TryGetValue("spearBaseAimAccuracy", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: spearBaseAimAccuracy is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: spearBaseAimAccuracy is not double is {value.GetType()}");
                else spearBaseAimAccuracy = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: spearBaseAimAccuracy not set");
        }
        { //spearIncreaseAimAccuracyPerLevel
            if (spearLevelStats.TryGetValue("spearIncreaseAimAccuracyPerLevel", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: spearIncreaseAimAccuracyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: spearIncreaseAimAccuracyPerLevel is not double is {value.GetType()}");
                else spearIncreaseAimAccuracyPerLevel = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: spearIncreaseAimAccuracyPerLevel not set");
        }


        // Get entity exp
        entityExpSpear.Clear();
        Dictionary<string, object> tmpentityExpSpear = api.Assets.Get(new AssetLocation("levelup:config/entityexp/spear.json")).ToObject<Dictionary<string, object>>();
        foreach (KeyValuePair<string, object> pair in tmpentityExpSpear)
        {
            if (pair.Value is long value) entityExpSpear.Add(pair.Key, (int)value);
            else Debug.Log($"CONFIGURATION ERROR: entityExpSpear {pair.Key} is not int");
        }
        Debug.Log("Spear configuration set");
    }

    public static int SpearGetLevelByEXP(ulong exp)
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

    public static float SpearGetDamageMultiplyByEXP(ulong exp)
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

        baseDamage += baseDamage * incrementDamage;
        return baseDamage;
    }

    public static bool SpearRollChanceToNotReduceDurabilityByEXP(ulong exp)
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

    public static float SpearGetAimAccuracyByEXP(ulong exp)
    {

        int level = SpearGetLevelByEXP(exp);
        // Exp base for level
        float accuracyPerLevelBase = spearBaseAimAccuracy;
        while (level > 1)
        {
            accuracyPerLevelBase += spearIncreaseAimAccuracyPerLevel;
            level -= 1;
        }
        return accuracyPerLevelBase;
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
        expPerHarvestFarming.Clear();
        Dictionary<string, object> tmpexpPerHarvestFarming = api.Assets.Get(new AssetLocation("levelup:config/levelstats/farmingcrops.json")).ToObject<Dictionary<string, object>>();
        foreach (KeyValuePair<string, object> pair in tmpexpPerHarvestFarming)
        {
            if (pair.Value is long value) expPerHarvestFarming.Add(pair.Key, (int)value);
            else Debug.Log($"CONFIGURATION ERROR: expPerHarvestFarming {pair.Key} is not int");
        }

        Debug.Log("Farming configuration set");
    }

    public static int FarmingGetLevelByEXP(ulong exp)
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

    public static float FarmingGetHarvestMultiplyByEXP(ulong exp)
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

        baseMultiply += baseMultiply * multiply;
        return baseMultiply;
    }

    public static bool FarmingRollChanceToNotReduceDurabilityByEXP(ulong exp)
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
    public static readonly Dictionary<string, double> expMultiplySingleCooking = [];
    public static readonly Dictionary<string, double> expMultiplyPotsCooking = [];
    private static int cookingBaseExpPerCooking = 3;
    private static int cookingEXPPerLevelBase = 10;
    private static double cookingEXPMultiplyPerLevel = 1.3;
    private static float cookingBaseFreshHoursMultiply = 1.0f;
    private static float cookingFreshHoursMultiplyPerLevel = 0.1f;
    private static float cookingBaseChanceToIncreaseServings = 1.0f;
    private static float cookingIncrementChanceToIncreaseServings = 2.0f;
    private static float cookingEveryChanceToIncreaseServingsReduceChance = 20.0f;
    private static float cookingChanceToIncreaseServingsReducerTotal = 0.5f;
    private static int cookingBaseRollsChanceToIncreaseServings = 1;
    private static int cookingEarnRollsChanceToIncreaseServingsEveryLevel = 5;
    private static int cookingEarnRollsChanceToIncreaseServingsQuantity = 1;
    public static int ExpPerCookingcooking => cookingBaseExpPerCooking;

    public static void PopulateCookingConfiguration(ICoreAPI api)
    {
        Dictionary<string, object> cookingLevelStats = api.Assets.Get(new AssetLocation("levelup:config/levelstats/cooking.json")).ToObject<Dictionary<string, object>>();
        { //cookingBaseExpPerCooking
            if (cookingLevelStats.TryGetValue("cookingBaseExpPerCooking", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: cookingBaseExpPerCooking is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: cookingBaseExpPerCooking is not int is {value.GetType()}");
                else cookingBaseExpPerCooking = (int)(long)value;
            else Debug.Log("CONFIGURATION ERROR: cookingBaseExpPerCooking not set");
        }
        { //cookingEXPPerLevelBase
            if (cookingLevelStats.TryGetValue("cookingEXPPerLevelBase", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: cookingEXPPerLevelBase is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: cookingEXPPerLevelBase is not int is {value.GetType()}");
                else cookingEXPPerLevelBase = (int)(long)value;
            else Debug.Log("CONFIGURATION ERROR: cookingEXPPerLevelBase not set");
        }
        { //cookingEXPMultiplyPerLevel
            if (cookingLevelStats.TryGetValue("cookingEXPMultiplyPerLevel", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: cookingEXPMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: cookingEXPMultiplyPerLevel is not double is {value.GetType()}");
                else cookingEXPMultiplyPerLevel = (double)value;
            else Debug.Log("CONFIGURATION ERROR: cookingEXPMultiplyPerLevel not set");
        }
        { //cookingBaseFreshHoursMultiply
            if (cookingLevelStats.TryGetValue("cookingBaseFreshHoursMultiply", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: cookingBaseFreshHoursMultiply is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: cookingBaseFreshHoursMultiply is not double is {value.GetType()}");
                else cookingBaseFreshHoursMultiply = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: cookingBaseFreshHoursMultiply not set");
        }
        { //cookingFreshHoursMultiplyPerLevel
            if (cookingLevelStats.TryGetValue("cookingFreshHoursMultiplyPerLevel", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: cookingFreshHoursMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: cookingFreshHoursMultiplyPerLevel is not double is {value.GetType()}");
                else cookingFreshHoursMultiplyPerLevel = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: cookingFreshHoursMultiplyPerLevel not set");
        }
        { //cookingBaseChanceToIncreaseServings
            if (cookingLevelStats.TryGetValue("cookingBaseChanceToIncreaseServings", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: cookingBaseChanceToIncreaseServings is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: cookingBaseChanceToIncreaseServings is not double is {value.GetType()}");
                else cookingBaseChanceToIncreaseServings = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: cookingBaseChanceToIncreaseServings not set");
        }
        { //cookingIncrementChanceToIncreaseServings
            if (cookingLevelStats.TryGetValue("cookingIncrementChanceToIncreaseServings", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: cookingIncrementChanceToIncreaseServings is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: cookingIncrementChanceToIncreaseServings is not double is {value.GetType()}");
                else cookingIncrementChanceToIncreaseServings = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: cookingIncrementChanceToIncreaseServings not set");
        }
        { //cookingEveryChanceToIncreaseServingsReduceChance
            if (cookingLevelStats.TryGetValue("cookingEveryChanceToIncreaseServingsReduceChance", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: cookingEveryChanceToIncreaseServingsReduceChance is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: cookingEveryChanceToIncreaseServingsReduceChance is not double is {value.GetType()}");
                else cookingEveryChanceToIncreaseServingsReduceChance = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: cookingEveryChanceToIncreaseServingsReduceChance not set");
        }
        { //cookingChanceToIncreaseServingsReducerTotal
            if (cookingLevelStats.TryGetValue("cookingChanceToIncreaseServingsReducerTotal", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: cookingChanceToIncreaseServingsReducerTotal is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: cookingChanceToIncreaseServingsReducerTotal is not double is {value.GetType()}");
                else cookingChanceToIncreaseServingsReducerTotal = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: cookingChanceToIncreaseServingsReducerTotal not set");
        }
        { //cookingBaseRollsChanceToIncreaseServings
            if (cookingLevelStats.TryGetValue("cookingBaseRollsChanceToIncreaseServings", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: cookingBaseRollsChanceToIncreaseServings is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: cookingBaseRollsChanceToIncreaseServings is not int is {value.GetType()}");
                else cookingBaseRollsChanceToIncreaseServings = (int)(long)value;
            else Debug.Log("CONFIGURATION ERROR: cookingBaseRollsChanceToIncreaseServings not set");
        }
        { //cookingEarnRollsChanceToIncreaseServingsEveryLevel
            if (cookingLevelStats.TryGetValue("cookingEarnRollsChanceToIncreaseServingsEveryLevel", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: cookingEarnRollsChanceToIncreaseServingsEveryLevel is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: cookingEarnRollsChanceToIncreaseServingsEveryLevel is not int is {value.GetType()}");
                else cookingEarnRollsChanceToIncreaseServingsEveryLevel = (int)(long)value;
            else Debug.Log("CONFIGURATION ERROR: cookingEarnRollsChanceToIncreaseServingsEveryLevel not set");
        }
        { //cookingEarnRollsChanceToIncreaseServingsQuantity
            if (cookingLevelStats.TryGetValue("cookingEarnRollsChanceToIncreaseServingsQuantity", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: cookingEarnRollsChanceToIncreaseServingsQuantity is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: cookingEarnRollsChanceToIncreaseServingsQuantity is not int is {value.GetType()}");
                else cookingEarnRollsChanceToIncreaseServingsQuantity = (int)(long)value;
            else Debug.Log("CONFIGURATION ERROR: cookingEarnRollsChanceToIncreaseServingsQuantity not set");
        }
        
        // Get single food exp multiply
        expMultiplySingleCooking.Clear();
        Dictionary<string, object> tmpexpMultiplySingleCooking = api.Assets.Get(new AssetLocation("levelup:config/levelstats/cookingsingles.json")).ToObject<Dictionary<string, object>>();
        foreach (KeyValuePair<string, object> pair in tmpexpMultiplySingleCooking)
        {
            if (pair.Value is double value) expMultiplySingleCooking.Add(pair.Key, value);
            else Debug.Log($"CONFIGURATION ERROR: expMultiplySingleCooking {pair.Key} is not double");
        }
        // Get pots food exp multiply
        expMultiplyPotsCooking.Clear();
        Dictionary<string, object> tmpexpMultiplyPotsCooking = api.Assets.Get(new AssetLocation("levelup:config/levelstats/cookingpots.json")).ToObject<Dictionary<string, object>>();
        foreach (KeyValuePair<string, object> pair in tmpexpMultiplyPotsCooking)
        {
            if (pair.Value is double value) expMultiplyPotsCooking.Add(pair.Key, value);
            else Debug.Log($"CONFIGURATION ERROR: expMultiplyPotsCooking {pair.Key} is not double");
        }
        
        Debug.Log("Cooking configuration set");
    }

    public static int CookingGetLevelByEXP(ulong exp)
    {
        int level = 0;
        // Exp base for level
        double expPerLevelBase = cookingEXPPerLevelBase;
        double calcExp = double.Parse(exp.ToString());
        while (calcExp > 0)
        {
            level += 1;
            calcExp -= expPerLevelBase;
            // 20 percentage increasing per level
            expPerLevelBase *= cookingEXPMultiplyPerLevel;
        }
        return level;
    }

    public static float CookingGetSaturationMultiplyByEXP(ulong exp)
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

        baseMultiply += baseMultiply * multiply;
        return baseMultiply;
    }

    public static float CookingGetFreshHoursMultiplyByEXP(ulong exp)
    {
        float baseMultiply = cookingBaseFreshHoursMultiply;
        int level = CookingGetLevelByEXP(exp);

        float incrementMultiply = cookingFreshHoursMultiplyPerLevel;
        float multiply = 0.0f;
        while (level > 1)
        {
            multiply += incrementMultiply;
            level -= 1;
        }

        baseMultiply += baseMultiply * multiply;
        return baseMultiply;
    }

    public static int CookingGetServingsByEXPAndServings(ulong exp, int quantityServings)
    {
        int level = CookingGetLevelByEXP(exp);
        float chanceToIncrease = cookingBaseChanceToIncreaseServings;
        float incrementChance = cookingIncrementChanceToIncreaseServings;
        int rolls = cookingBaseRollsChanceToIncreaseServings;
        while (level > 1)
        {
            level -= 1;
            // Reduce change every {} chanceToIncrease
            if (chanceToIncrease % cookingEveryChanceToIncreaseServingsReduceChance == 0) incrementChance -= cookingChanceToIncreaseServingsReducerTotal;
            chanceToIncrease += incrementChance;
            // Increase rolls change by 1 every 5 levels
            if (level % cookingEarnRollsChanceToIncreaseServingsEveryLevel == 0) rolls += cookingEarnRollsChanceToIncreaseServingsQuantity;
        }
        while (rolls > 0)
        {
            // Randomizes the chance and increase if chances hit
            if (chanceToIncrease >= new Random().Next(0, 100)) quantityServings += 1;
        }
        return quantityServings;
    }
    #endregion

    #region vitality
    private static int vitalityEXPPerReceiveHit = 1;
    private static float vitalityEXPMultiplyByDamage = 0.5f;
    private static int vitalityEXPIncreaseByAmountDamage = 1;
    private static int vitalityEXPPerLevelBase = 10;
    private static double vitalityEXPMultiplyPerLevel = 2.0;
    private static float vitalityHPIncreasePerLevel = 10.0f;
    private static float vitalityBaseHP = 10.0f;
    private static float vitalityBaseHPRegen = 1.0f;
    private static float vitalityHPRegenIncreasePerLevel = 0.1f;

    public static void PopulateVitalityConfiguration(ICoreAPI api)
    {
        Dictionary<string, object> vitalityLevelStats = api.Assets.Get(new AssetLocation("levelup:config/levelstats/vitality.json")).ToObject<Dictionary<string, object>>();
        { //vitalityEXPPerLevelBase
            if (vitalityLevelStats.TryGetValue("vitalityEXPPerLevelBase", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: vitalityEXPPerLevelBase is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: vitalityEXPPerLevelBase is not int is {value.GetType()}");
                else vitalityEXPPerLevelBase = (int)(long)value;
            else Debug.Log("CONFIGURATION ERROR: vitalityEXPPerLevelBase not set");
        }
        { //vitalityEXPMultiplyPerLevel
            if (vitalityLevelStats.TryGetValue("vitalityEXPMultiplyPerLevel", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: vitalityEXPMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: vitalityEXPMultiplyPerLevel is not double is {value.GetType()}");
                else vitalityEXPMultiplyPerLevel = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: vitalityEXPMultiplyPerLevel not set");
        }
        { //vitalityEXPPerReceiveHit
            if (vitalityLevelStats.TryGetValue("vitalityEXPPerReceiveHit", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: vitalityEXPPerReceiveHit is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: vitalityEXPPerReceiveHit is not int is {value.GetType()}");
                else vitalityEXPPerReceiveHit = (int)(long)value;
            else Debug.Log("CONFIGURATION ERROR: vitalityEXPPerReceiveHit not set");
        }
        { //vitalityEXPMultiplyByDamage
            if (vitalityLevelStats.TryGetValue("vitalityEXPMultiplyByDamage", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: vitalityEXPMultiplyByDamage is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: vitalityEXPMultiplyByDamage is not double is {value.GetType()}");
                else vitalityEXPMultiplyByDamage = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: vitalityEXPMultiplyByDamage not set");
        }
        { //vitalityHPIncreasePerLevel
            if (vitalityLevelStats.TryGetValue("vitalityHPIncreasePerLevel", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: vitalityHPIncreasePerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: vitalityHPIncreasePerLevel is not double is {value.GetType()}");
                else vitalityHPIncreasePerLevel = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: vitalityHPIncreasePerLevel not set");
        }
        { //vitalityBaseHP
            if (vitalityLevelStats.TryGetValue("vitalityBaseHP", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: vitalityBaseHP is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: vitalityBaseHP is not double is {value.GetType()}");
                else vitalityBaseHP = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: vitalityBaseHP not set");
        }
        { //vitalityEXPIncreaseByAmountDamage
            if (vitalityLevelStats.TryGetValue("vitalityEXPIncreaseByAmountDamage", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: vitalityEXPIncreaseByAmountDamage is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: vitalityEXPIncreaseByAmountDamage is not int is {value.GetType()}");
                else vitalityEXPIncreaseByAmountDamage = (int)(long)value;
            else Debug.Log("CONFIGURATION ERROR: vitalityEXPIncreaseByAmountDamage not set");
        }
        { //vitalityBaseHPRegen
            if (vitalityLevelStats.TryGetValue("vitalityBaseHPRegen", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: vitalityBaseHPRegen is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: vitalityBaseHPRegen is not double is {value.GetType()}");
                else vitalityBaseHPRegen = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: vitalityBaseHPRegen not set");
        }
        { //vitalityHPRegenIncreasePerLevel
            if (vitalityLevelStats.TryGetValue("vitalityHPRegenIncreasePerLevel", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: vitalityHPRegenIncreasePerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: vitalityHPRegenIncreasePerLevel is not double is {value.GetType()}");
                else vitalityHPRegenIncreasePerLevel = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: vitalityHPRegenIncreasePerLevel not set");
        }

        Debug.Log("Vitality configuration set");
    }

    public static int VitalityGetLevelByEXP(ulong exp)
    {
        int level = 0;
        // Exp base for level
        double expPerLevelBase = vitalityEXPPerLevelBase;
        double calcExp = double.Parse(exp.ToString());
        while (calcExp > 0)
        {
            level += 1;
            calcExp -= expPerLevelBase;
            // 10 percentage increasing per level
            expPerLevelBase *= vitalityEXPMultiplyPerLevel;
        }
        return level;
    }

    public static float VitalityGetMaxHealthByEXP(ulong exp)
    {
        float baseMultiply = vitalityBaseHP;
        int level = VitalityGetLevelByEXP(exp);

        float incrementMultiply = vitalityHPIncreasePerLevel;
        while (level > 1)
        {
            baseMultiply += incrementMultiply;
            level -= 1;
        }
        return baseMultiply;
    }

    public static float VitalityGetHealthRegenMultiplyByEXP(ulong exp)
    {
        float baseMultiply = vitalityBaseHPRegen;
        int level = VitalityGetLevelByEXP(exp);

        float incrementMultiply = vitalityHPRegenIncreasePerLevel;
        while (level > 1)
        {
            baseMultiply += incrementMultiply;
            level -= 1;
        }
        return baseMultiply;
    }

    public static int VitalityEXPEarnedByDAMAGE(float damage)
    {
        float baseMultiply = vitalityEXPPerReceiveHit;
        int calcDamage = (int)Math.Round(damage);

        float multiply = (float)vitalityEXPMultiplyByDamage;
        while (calcDamage > 1)
        {
            // Increase experience
            if (calcDamage % vitalityEXPIncreaseByAmountDamage == 0) baseMultiply += baseMultiply * multiply;
            calcDamage -= 1;
        }
        return (int)Math.Round(baseMultiply);
    }
    #endregion

    #region leatherarmor
    public static readonly Dictionary<string, double> expMultiplyHitLeatherArmor = [];
    private static int leatherArmorEXPPerReceiveHit = 1;
    private static float leatherArmorEXPMultiplyByDamage = 0.5f;
    private static int leatherArmorEXPIncreaseByAmountDamage = 1;
    private static int leatherArmorEXPPerLevelBase = 10;
    private static double leatherArmorEXPMultiplyPerLevel = 2.0;
    private static float leatherArmorBaseDamageReduction = 0.0f;
    private static float leatherArmorDamageReductionPerLevel = 0.05f;
    private static float leatherArmorBaseDurabilityRestoreChance = 0.0f;
    private static float leatherArmorDurabilityRestoreChancePerLevel = 2.0f;
    private static int leatherArmorDurabilityRestoreEveryLevelReduceChance = 10;
    private static float leatherArmorDurabilityRestoreReduceChanceForEveryLevel = 0.5f;

    public static void PopulateLeatherArmorConfiguration(ICoreAPI api)
    {
        Dictionary<string, object> leatherArmorLevelStats = api.Assets.Get(new AssetLocation("levelup:config/levelstats/leatherarmor.json")).ToObject<Dictionary<string, object>>();
        { //leatherArmorEXPPerReceiveHit
            if (leatherArmorLevelStats.TryGetValue("leatherArmorEXPPerReceiveHit", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: leatherArmorEXPPerReceiveHit is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: leatherArmorEXPPerReceiveHit is not int is {value.GetType()}");
                else leatherArmorEXPPerReceiveHit = (int)(long)value;
            else Debug.Log("CONFIGURATION ERROR: leatherArmorEXPPerReceiveHit not set");
        }
        { //leatherArmorEXPMultiplyByDamage
            if (leatherArmorLevelStats.TryGetValue("leatherArmorEXPMultiplyByDamage", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: leatherArmorEXPMultiplyByDamage is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: leatherArmorEXPMultiplyByDamage is not double is {value.GetType()}");
                else leatherArmorEXPMultiplyByDamage = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: leatherArmorEXPMultiplyByDamage not set");
        }
        { //leatherArmorEXPIncreaseByAmountDamage
            if (leatherArmorLevelStats.TryGetValue("leatherArmorEXPIncreaseByAmountDamage", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: leatherArmorEXPIncreaseByAmountDamage is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: leatherArmorEXPIncreaseByAmountDamage is not int is {value.GetType()}");
                else leatherArmorEXPIncreaseByAmountDamage = (int)(long)value;
            else Debug.Log("CONFIGURATION ERROR: leatherArmorEXPIncreaseByAmountDamage not set");
        }
        { //leatherArmorEXPPerLevelBase
            if (leatherArmorLevelStats.TryGetValue("leatherArmorEXPPerLevelBase", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: leatherArmorEXPPerLevelBase is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: leatherArmorEXPPerLevelBase is not int is {value.GetType()}");
                else leatherArmorEXPPerLevelBase = (int)(long)value;
            else Debug.Log("CONFIGURATION ERROR: leatherArmorEXPPerLevelBase not set");
        }
        { //leatherArmorEXPMultiplyPerLevel
            if (leatherArmorLevelStats.TryGetValue("leatherArmorEXPMultiplyPerLevel", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: leatherArmorEXPMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: leatherArmorEXPMultiplyPerLevel is not double is {value.GetType()}");
                else leatherArmorEXPMultiplyPerLevel = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: leatherArmorEXPMultiplyPerLevel not set");
        }
        { //leatherArmorBaseDamageReduction
            if (leatherArmorLevelStats.TryGetValue("leatherArmorBaseDamageReduction", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: leatherArmorBaseDamageReduction is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: leatherArmorBaseDamageReduction is not double is {value.GetType()}");
                else leatherArmorBaseDamageReduction = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: leatherArmorBaseDamageReduction not set");
        }
        { //leatherArmorDamageReductionPerLevel
            if (leatherArmorLevelStats.TryGetValue("leatherArmorDamageReductionPerLevel", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: leatherArmorDamageReductionPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: leatherArmorDamageReductionPerLevel is not double is {value.GetType()}");
                else leatherArmorDamageReductionPerLevel = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: leatherArmorDamageReductionPerLevel not set");
        }
        { //leatherArmorBaseDurabilityRestoreChance
            if (leatherArmorLevelStats.TryGetValue("leatherArmorBaseDurabilityRestoreChance", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: leatherArmorBaseDurabilityRestoreChance is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: leatherArmorBaseDurabilityRestoreChance is not double is {value.GetType()}");
                else leatherArmorBaseDurabilityRestoreChance = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: leatherArmorBaseDurabilityRestoreChance not set");
        }
        { //leatherArmorDurabilityRestoreChancePerLevel
            if (leatherArmorLevelStats.TryGetValue("leatherArmorDurabilityRestoreChancePerLevel", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: leatherArmorDurabilityRestoreChancePerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: leatherArmorDurabilityRestoreChancePerLevel is not double is {value.GetType()}");
                else leatherArmorDurabilityRestoreChancePerLevel = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: leatherArmorDurabilityRestoreChancePerLevel not set");
        }
        { //leatherArmorDurabilityRestoreEveryLevelReduceChance
            if (leatherArmorLevelStats.TryGetValue("leatherArmorDurabilityRestoreEveryLevelReduceChance", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: leatherArmorDurabilityRestoreEveryLevelReduceChance is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: leatherArmorDurabilityRestoreEveryLevelReduceChance is not int is {value.GetType()}");
                else leatherArmorDurabilityRestoreEveryLevelReduceChance = (int)(long)value;
            else Debug.Log("CONFIGURATION ERROR: leatherArmorDurabilityRestoreEveryLevelReduceChance not set");
        }
        { //leatherArmorDurabilityRestoreReduceChanceForEveryLevel
            if (leatherArmorLevelStats.TryGetValue("leatherArmorDurabilityRestoreReduceChanceForEveryLevel", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: leatherArmorDurabilityRestoreReduceChanceForEveryLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: leatherArmorDurabilityRestoreReduceChanceForEveryLevel is not double is {value.GetType()}");
                else leatherArmorDurabilityRestoreReduceChanceForEveryLevel = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: leatherArmorDurabilityRestoreReduceChanceForEveryLevel not set");
        }

        // Get leather armor multiply exp
        expMultiplyHitLeatherArmor.Clear();
        Dictionary<string, object> tmpexpMultiplyHitLeatherArmor = api.Assets.Get(new AssetLocation("levelup:config/levelstats/leatherarmoritems.json")).ToObject<Dictionary<string, object>>();
        foreach (KeyValuePair<string, object> pair in tmpexpMultiplyHitLeatherArmor)
        {
            if (pair.Value is double value) expMultiplyHitLeatherArmor.Add(pair.Key, (double)value);
            else Debug.Log($"CONFIGURATION ERROR: expMultiplyHitLeatherArmor {pair.Key} is not double");
        }
        Debug.Log("Leather Armor configuration set");
    }


    public static int LeatherArmorGetLevelByEXP(ulong exp)
    {
        int level = 0;
        // Exp base for level
        double expPerLevelBase = leatherArmorEXPPerLevelBase;
        double calcExp = double.Parse(exp.ToString());
        while (calcExp > 0)
        {
            level += 1;
            calcExp -= expPerLevelBase;
            // 10 percentage increasing per level
            expPerLevelBase *= leatherArmorEXPMultiplyPerLevel;
        }
        return level;
    }
    public static int LeatherArmorBaseEXPEarnedByDAMAGE(float damage)
    {
        float baseMultiply = leatherArmorEXPPerReceiveHit;
        int calcDamage = (int)Math.Round(damage);

        float multiply = (float)leatherArmorEXPMultiplyByDamage;
        while (calcDamage > 1)
        {
            // Increase experience
            if (calcDamage % leatherArmorEXPIncreaseByAmountDamage == 0) baseMultiply += baseMultiply * multiply;
            calcDamage -= 1;
        }
        return (int)Math.Round(baseMultiply);
    }
    public static float LeatherArmorDamageReductionByEXP(ulong exp)
    {
        int level = LeatherArmorGetLevelByEXP(exp);
        float baseMultiply = leatherArmorBaseDamageReduction;
        while (level > 1)
        {
            baseMultiply += leatherArmorDamageReductionPerLevel;
            level -= 1;
        }
        return baseMultiply;
    }
    public static bool LeatherArmorRollChanceToNotReduceDurabilityByEXP(ulong exp)
    {
        int level = LeatherArmorGetLevelByEXP(exp);
        float baseChanceToNotReduce = leatherArmorBaseDurabilityRestoreChance;
        float chanceToNotReduce = leatherArmorDurabilityRestoreChancePerLevel;
        while (level > 1)
        {
            level -= 1;
            // Every {} levels reduce the durability chance multiplicator
            if (level % leatherArmorDurabilityRestoreEveryLevelReduceChance == 0)
                chanceToNotReduce -= leatherArmorDurabilityRestoreReduceChanceForEveryLevel;
            // Increasing chance
            baseChanceToNotReduce += chanceToNotReduce;
        }
        // Check the chance 
        if (baseChanceToNotReduce >= new Random().Next(0, 100)) return true;
        else return false;
    }
    #endregion

    #region chainarmor
    public static readonly Dictionary<string, double> expMultiplyHitChainArmor = [];
    private static int chainArmorEXPPerReceiveHit = 1;
    private static float chainArmorEXPMultiplyByDamage = 0.5f;
    private static int chainArmorEXPIncreaseByAmountDamage = 1;
    private static int chainArmorEXPPerLevelBase = 10;
    private static double chainArmorEXPMultiplyPerLevel = 2.0;
    private static float chainArmorBaseDamageReduction = 0.0f;
    private static float chainArmorDamageReductionPerLevel = 0.05f;
    private static float chainArmorBaseDurabilityRestoreChance = 0.0f;
    private static float chainArmorDurabilityRestoreChancePerLevel = 2.0f;
    private static int chainArmorDurabilityRestoreEveryLevelReduceChance = 10;
    private static float chainArmorDurabilityRestoreReduceChanceForEveryLevel = 0.5f;

    public static void PopulateChainArmorConfiguration(ICoreAPI api)
    {
        Dictionary<string, object> chainArmorLevelStats = api.Assets.Get(new AssetLocation("levelup:config/levelstats/chainarmor.json")).ToObject<Dictionary<string, object>>();
        { //chainArmorEXPPerReceiveHit
            if (chainArmorLevelStats.TryGetValue("chainArmorEXPPerReceiveHit", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: chainArmorEXPPerReceiveHit is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: chainArmorEXPPerReceiveHit is not int is {value.GetType()}");
                else chainArmorEXPPerReceiveHit = (int)(long)value;
            else Debug.Log("CONFIGURATION ERROR: chainArmorEXPPerReceiveHit not set");
        }
        { //chainArmorEXPMultiplyByDamage
            if (chainArmorLevelStats.TryGetValue("chainArmorEXPMultiplyByDamage", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: chainArmorEXPMultiplyByDamage is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: chainArmorEXPMultiplyByDamage is not double is {value.GetType()}");
                else chainArmorEXPMultiplyByDamage = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: chainArmorEXPMultiplyByDamage not set");
        }
        { //chainArmorEXPIncreaseByAmountDamage
            if (chainArmorLevelStats.TryGetValue("chainArmorEXPIncreaseByAmountDamage", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: chainArmorEXPIncreaseByAmountDamage is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: chainArmorEXPIncreaseByAmountDamage is not int is {value.GetType()}");
                else chainArmorEXPIncreaseByAmountDamage = (int)(long)value;
            else Debug.Log("CONFIGURATION ERROR: chainArmorEXPIncreaseByAmountDamage not set");
        }
        { //chainArmorEXPPerLevelBase
            if (chainArmorLevelStats.TryGetValue("chainArmorEXPPerLevelBase", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: chainArmorEXPPerLevelBase is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: chainArmorEXPPerLevelBase is not int is {value.GetType()}");
                else chainArmorEXPPerLevelBase = (int)(long)value;
            else Debug.Log("CONFIGURATION ERROR: chainArmorEXPPerLevelBase not set");
        }
        { //chainArmorEXPMultiplyPerLevel
            if (chainArmorLevelStats.TryGetValue("chainArmorEXPMultiplyPerLevel", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: chainArmorEXPMultiplyPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: chainArmorEXPMultiplyPerLevel is not double is {value.GetType()}");
                else chainArmorEXPMultiplyPerLevel = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: chainArmorEXPMultiplyPerLevel not set");
        }
        { //chainArmorBaseDamageReduction
            if (chainArmorLevelStats.TryGetValue("chainArmorBaseDamageReduction", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: chainArmorBaseDamageReduction is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: chainArmorBaseDamageReduction is not double is {value.GetType()}");
                else chainArmorBaseDamageReduction = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: chainArmorBaseDamageReduction not set");
        }
        { //chainArmorDamageReductionPerLevel
            if (chainArmorLevelStats.TryGetValue("chainArmorDamageReductionPerLevel", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: chainArmorDamageReductionPerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: chainArmorDamageReductionPerLevel is not double is {value.GetType()}");
                else chainArmorDamageReductionPerLevel = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: chainArmorDamageReductionPerLevel not set");
        }
        { //chainArmorBaseDurabilityRestoreChance
            if (chainArmorLevelStats.TryGetValue("chainArmorBaseDurabilityRestoreChance", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: chainArmorBaseDurabilityRestoreChance is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: chainArmorBaseDurabilityRestoreChance is not double is {value.GetType()}");
                else chainArmorBaseDurabilityRestoreChance = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: chainArmorBaseDurabilityRestoreChance not set");
        }
        { //chainArmorDurabilityRestoreChancePerLevel
            if (chainArmorLevelStats.TryGetValue("chainArmorDurabilityRestoreChancePerLevel", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: chainArmorDurabilityRestoreChancePerLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: chainArmorDurabilityRestoreChancePerLevel is not double is {value.GetType()}");
                else chainArmorDurabilityRestoreChancePerLevel = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: chainArmorDurabilityRestoreChancePerLevel not set");
        }
        { //chainArmorDurabilityRestoreEveryLevelReduceChance
            if (chainArmorLevelStats.TryGetValue("chainArmorDurabilityRestoreEveryLevelReduceChance", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: chainArmorDurabilityRestoreEveryLevelReduceChance is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: chainArmorDurabilityRestoreEveryLevelReduceChance is not int is {value.GetType()}");
                else chainArmorDurabilityRestoreEveryLevelReduceChance = (int)(long)value;
            else Debug.Log("CONFIGURATION ERROR: chainArmorDurabilityRestoreEveryLevelReduceChance not set");
        }
        { //chainArmorDurabilityRestoreReduceChanceForEveryLevel
            if (chainArmorLevelStats.TryGetValue("chainArmorDurabilityRestoreReduceChanceForEveryLevel", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: chainArmorDurabilityRestoreReduceChanceForEveryLevel is null");
                else if (value is not double) Debug.Log($"CONFIGURATION ERROR: chainArmorDurabilityRestoreReduceChanceForEveryLevel is not double is {value.GetType()}");
                else chainArmorDurabilityRestoreReduceChanceForEveryLevel = (float)(double)value;
            else Debug.Log("CONFIGURATION ERROR: chainArmorDurabilityRestoreReduceChanceForEveryLevel not set");
        }

        // Get leather armor multiply exp
        expMultiplyHitChainArmor.Clear();
        Dictionary<string, object> tmpexpMultiplyHitChainArmor = api.Assets.Get(new AssetLocation("levelup:config/levelstats/chainarmoritems.json")).ToObject<Dictionary<string, object>>();
        foreach (KeyValuePair<string, object> pair in tmpexpMultiplyHitChainArmor)
        {
            if (pair.Value is double value) expMultiplyHitChainArmor.Add(pair.Key, (double)value);
            else Debug.Log($"CONFIGURATION ERROR: expMultiplyHitChainArmor {pair.Key} is not double");
        }
        Debug.Log("Chain Armor configuration set");
    }

    public static int ChainArmorGetLevelByEXP(ulong exp)
    {
        int level = 0;
        // Exp base for level
        double expPerLevelBase = chainArmorEXPPerLevelBase;
        double calcExp = double.Parse(exp.ToString());
        while (calcExp > 0)
        {
            level += 1;
            calcExp -= expPerLevelBase;
            // 10 percentage increasing per level
            expPerLevelBase *= chainArmorEXPMultiplyPerLevel;
        }
        return level;
    }
    public static int ChainArmorBaseEXPEarnedByDAMAGE(float damage)
    {
        float baseMultiply = chainArmorEXPPerReceiveHit;
        int calcDamage = (int)Math.Round(damage);

        float multiply = (float)chainArmorEXPMultiplyByDamage;
        while (calcDamage > 1)
        {
            // Increase experience
            if (calcDamage % chainArmorEXPIncreaseByAmountDamage == 0) baseMultiply += baseMultiply * multiply;
            calcDamage -= 1;
        }
        return (int)Math.Round(baseMultiply);
    }
    public static float ChainArmorDamageReductionByEXP(ulong exp)
    {
        int level = ChainArmorGetLevelByEXP(exp);
        float baseMultiply = chainArmorBaseDamageReduction;
        while (level > 1)
        {
            baseMultiply += chainArmorDamageReductionPerLevel;
            level -= 1;
        }
        return baseMultiply;
    }
    public static bool ChainArmorRollChanceToNotReduceDurabilityByEXP(ulong exp)
    {
        int level = ChainArmorGetLevelByEXP(exp);
        float baseChanceToNotReduce = chainArmorBaseDurabilityRestoreChance;
        float chanceToNotReduce = chainArmorDurabilityRestoreChancePerLevel;
        while (level > 1)
        {
            level -= 1;
            // Every {} levels reduce the durability chance multiplicator
            if (level % chainArmorDurabilityRestoreEveryLevelReduceChance == 0)
                chanceToNotReduce -= chainArmorDurabilityRestoreReduceChanceForEveryLevel;
            // Increasing chance
            baseChanceToNotReduce += chanceToNotReduce;
        }
        // Check the chance 
        if (baseChanceToNotReduce >= new Random().Next(0, 100)) return true;
        else return false;
    }

    #endregion
}