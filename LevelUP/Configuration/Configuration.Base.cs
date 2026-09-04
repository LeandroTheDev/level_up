using System.Collections.Generic;
using OpenConfiguration;
using Vintagestory.API.Common;

namespace LevelUP;

public class BaseLevelStatsConfiguration
{
    public bool enableHardcore = false;
    public double hardcoreLosePercentage = 0.8;
    public int hardcorePenaltyDelayInWorldSeconds = 1000;
    public bool hardcoreIgnoreLevelMinimum = false;
    public bool hardcoreMessageWhenDying = true;
    public bool enableLevelHunter = true;
    public bool enableLevelBow = true;
    public bool enableLevelSlingshot = true;
    public bool enableLevelKnife = true;
    public bool enableLevelSpear = true;
    public bool enableLevelHammer = true;
    public bool enableLevelAxe = true;
    public bool enableLevelPickaxe = true;
    public bool enableLevelShovel = true;
    public bool enableLevelSword = true;
    public bool enableLevelShield = true;
    public bool enableLevelHand = true;
    public bool enableLevelFarming = true;
    public bool enableLevelCooking = true;
    public bool enableLevelPanning = true;
    public bool enableLevelVitality = true;
    public bool enableLevelMetabolism = true;
    public bool enableLevelLeatherArmor = true;
    public bool enableLevelChainArmor = true;
    public bool enableLevelBrigandineArmor = true;
    public bool enableLevelLamellarArmor = true;
    public bool enableLevelPlateArmor = true;
    public bool enableLevelScaleArmor = true;
    public bool enableLevelSmithing = true;
    public bool enableLevelQuenching = true;
    public int minimumEXPEarned = 1;
    public bool enableLevelUPUIDSecurity = false;
    public bool enableLevelUpChatMessages = true;
    public bool enableLevelUpExperienceServerLog = false;
    public bool enableExtendedLog = false;
}

public static partial class Configuration
{
    public static bool enableHardcore = false;
    public static double hardcoreLosePercentage = 0.8;
    public static int hardcorePenaltyDelayInWorldSeconds = 1000;
    public static bool hardcoreIgnoreLevelMinimum = false;
    public static bool hardcoreMessageWhenDying = true;
    public static bool enableLevelHunter = true;
    public static bool enableLevelBow = true;
    public static bool enableLevelSlingshot = true;
    public static bool enableLevelKnife = true;
    public static bool enableLevelSpear = true;
    public static bool enableLevelHammer = true;
    public static bool enableLevelAxe = true;
    public static bool enableLevelPickaxe = true;
    public static bool enableLevelShovel = true;
    public static bool enableLevelSword = true;
    public static bool enableLevelShield = true;
    public static bool enableLevelHand = true;
    public static bool enableLevelFarming = true;
    public static bool enableLevelCooking = true;
    public static bool enableLevelPanning = true;
    public static bool enableLevelVitality = true;
    public static bool enableLevelMetabolism = true;
    public static bool enableLevelLeatherArmor = true;
    public static bool enableLevelChainArmor = true;
    public static bool enableLevelBrigandineArmor = true;
    public static bool enableLevelLamellarArmor = true;
    public static bool enableLevelPlateArmor = true;
    public static bool enableLevelScaleArmor = true;
    public static bool enableLevelSmithing = true;
    public static bool enableLevelQuenching = true;
    public static int minimumEXPEarned = 1;
    public static bool enableLevelUPUIDSecurity = false;
    public static bool enableLevelUpChatMessages = true;
    public static bool enableLevelUpExperienceServerLog = false;
    public static bool enableExtendedLog = false;

    private static Dictionary<string, bool> enabledLevels = [];
    public static IReadOnlyDictionary<string, bool> EnabledLevels => enabledLevels;

    internal static void UpdateBaseConfigurations(ICoreAPI api)
    {
        BaseLevelStatsConfiguration baseConfigs = ConfigManager.Load<BaseLevelStatsConfiguration>(api, "ModConfig/LevelUP/config", "base", Logger(api));

        enableHardcore = baseConfigs.enableHardcore;
        hardcoreLosePercentage = baseConfigs.hardcoreLosePercentage;
        hardcorePenaltyDelayInWorldSeconds = baseConfigs.hardcorePenaltyDelayInWorldSeconds;
        hardcoreIgnoreLevelMinimum = baseConfigs.hardcoreIgnoreLevelMinimum;
        hardcoreMessageWhenDying = baseConfigs.hardcoreMessageWhenDying;
        enableLevelHunter = baseConfigs.enableLevelHunter;
        enableLevelBow = baseConfigs.enableLevelBow;
        enableLevelSlingshot = baseConfigs.enableLevelSlingshot;
        enableLevelKnife = baseConfigs.enableLevelKnife;
        enableLevelSpear = baseConfigs.enableLevelSpear;
        enableLevelHammer = baseConfigs.enableLevelHammer;
        enableLevelAxe = baseConfigs.enableLevelAxe;
        enableLevelPickaxe = baseConfigs.enableLevelPickaxe;
        enableLevelShovel = baseConfigs.enableLevelShovel;
        enableLevelSword = baseConfigs.enableLevelSword;
        enableLevelShield = baseConfigs.enableLevelShield;
        enableLevelHand = baseConfigs.enableLevelHand;
        enableLevelFarming = baseConfigs.enableLevelFarming;
        enableLevelCooking = baseConfigs.enableLevelCooking;
        enableLevelPanning = baseConfigs.enableLevelPanning;
        enableLevelVitality = baseConfigs.enableLevelVitality;
        enableLevelMetabolism = baseConfigs.enableLevelMetabolism;
        enableLevelLeatherArmor = baseConfigs.enableLevelLeatherArmor;
        enableLevelChainArmor = baseConfigs.enableLevelChainArmor;
        enableLevelBrigandineArmor = baseConfigs.enableLevelBrigandineArmor;
        enableLevelLamellarArmor = baseConfigs.enableLevelLamellarArmor;
        enableLevelPlateArmor = baseConfigs.enableLevelPlateArmor;
        enableLevelScaleArmor = baseConfigs.enableLevelScaleArmor;
        enableLevelSmithing = baseConfigs.enableLevelSmithing;
        enableLevelQuenching = baseConfigs.enableLevelQuenching;
        minimumEXPEarned = baseConfigs.minimumEXPEarned;
        enableLevelUPUIDSecurity = baseConfigs.enableLevelUPUIDSecurity;
        enableLevelUpChatMessages = baseConfigs.enableLevelUpChatMessages;
        enableLevelUpExperienceServerLog = baseConfigs.enableLevelUpExperienceServerLog;
        enableExtendedLog = baseConfigs.enableExtendedLog;
    }

    /// <summary>
    /// Register a new level in EnabledLevels variable class
    /// </summary>
    public static void RegisterNewLevel(string levelType, bool enabled = true)
    {
        if (levelsByLevelTypeEXP.ContainsKey(levelType))
        {
            Debug.LogError($"The leveltype {levelType} already exist in enabledLevels");
            return;
        }

        enabledLevels.Add(levelType, enabled);
    }
}
