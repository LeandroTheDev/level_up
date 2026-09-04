using System;
using System.Collections.Generic;
using System.IO;
using OpenConfiguration;
using Vintagestory.API.Common;

namespace LevelUP;

public class HunterClassConfiguration : Dictionary<string, double>
{
    public HunterClassConfiguration() : base(new Dictionary<string, double>
    {
        ["classHunterLevelMultiply"] = 2.0,
        ["classBowLevelMultiply"] = 2.5,
        ["classSlingshotLevelMultiply"] = 0.9,
        ["classKnifeLevelMultiply"] = 1.5,
        ["classAxeLevelMultiply"] = 0.7,
        ["classPickaxeLevelMultiply"] = 0.5,
        ["classShovelLevelMultiply"] = 0.5,
        ["classSpearLevelMultiply"] = 1.8,
        ["classHammerLevelMultiply"] = 0.7,
        ["classSwordLevelMultiply"] = 1.0,
        ["classShieldLevelMultiply"] = 0.9,
        ["classHandLevelMultiply"] = 1.0,
        ["classFarmingLevelMultiply"] = 0.7,
        ["classCookingLevelMultiply"] = 0.6,
        ["classPanningLevelMultiply"] = 0.5,
        ["classVitalityLevelMultiply"] = 0.5,
        ["classMetabolismLevelMultiply"] = 2.0,
        ["classLeatherArmorLevelMultiply"] = 2.0,
        ["classChainArmorLevelMultiply"] = 0.4,
        ["classBrigandineArmorLevelMultiply"] = 1.0,
        ["classPlateArmorLevelMultiply"] = 1.0,
        ["classScaleArmorLevelMultiply"] = 1.0,
        ["classLamellarArmorLevelMultiply"] = 1.0,
        ["classSmithingLevelMultiply"] = 0.5,
    })
    { }
}

public class CommonerClassConfiguration : Dictionary<string, double>
{
    public CommonerClassConfiguration() : base(new Dictionary<string, double>
    {
        ["classHunterLevelMultiply"] = 1.0,
        ["classBowLevelMultiply"] = 1.0,
        ["classSlingshotLevelMultiply"] = 1.0,
        ["classKnifeLevelMultiply"] = 1.0,
        ["classAxeLevelMultiply"] = 1.0,
        ["classPickaxeLevelMultiply"] = 1.0,
        ["classShovelLevelMultiply"] = 1.0,
        ["classSpearLevelMultiply"] = 1.0,
        ["classHammerLevelMultiply"] = 1.0,
        ["classSwordLevelMultiply"] = 1.0,
        ["classShieldLevelMultiply"] = 1.0,
        ["classHandLevelMultiply"] = 1.0,
        ["classFarmingLevelMultiply"] = 1.0,
        ["classCookingLevelMultiply"] = 1.0,
        ["classPanningLevelMultiply"] = 1.0,
        ["classVitalityLevelMultiply"] = 1.0,
        ["classMetabolismLevelMultiply"] = 1.0,
        ["classLeatherArmorLevelMultiply"] = 1.0,
        ["classChainArmorLevelMultiply"] = 1.0,
        ["classBrigandineArmorLevelMultiply"] = 1.0,
        ["classPlateArmorLevelMultiply"] = 1.0,
        ["classScaleArmorLevelMultiply"] = 1.0,
        ["classLamellarArmorLevelMultiply"] = 1.0,
        ["classSmithingLevelMultiply"] = 1.0,
    })
    { }
}

public class BlackguardClassConfiguration : Dictionary<string, double>
{
    public BlackguardClassConfiguration() : base(new Dictionary<string, double>
    {
        ["classHunterLevelMultiply"] = 0.5,
        ["classBowLevelMultiply"] = 0.3,
        ["classSlingshotLevelMultiply"] = 0.5,
        ["classKnifeLevelMultiply"] = 0.4,
        ["classAxeLevelMultiply"] = 1.3,
        ["classPickaxeLevelMultiply"] = 2.0,
        ["classShovelLevelMultiply"] = 2.0,
        ["classSpearLevelMultiply"] = 0.6,
        ["classHammerLevelMultiply"] = 1.0,
        ["classSwordLevelMultiply"] = 2.5,
        ["classShieldLevelMultiply"] = 2.0,
        ["classHandLevelMultiply"] = 2.0,
        ["classFarmingLevelMultiply"] = 0.7,
        ["classCookingLevelMultiply"] = 0.5,
        ["classPanningLevelMultiply"] = 0.5,
        ["classVitalityLevelMultiply"] = 1.5,
        ["classMetabolismLevelMultiply"] = 0.5,
        ["classLeatherArmorLevelMultiply"] = 1.0,
        ["classChainArmorLevelMultiply"] = 1.0,
        ["classBrigandineArmorLevelMultiply"] = 1.0,
        ["classPlateArmorLevelMultiply"] = 1.0,
        ["classScaleArmorLevelMultiply"] = 1.0,
        ["classLamellarArmorLevelMultiply"] = 1.0,
        ["classSmithingLevelMultiply"] = 1.5,
    })
    { }
}

public class ClockmakerClassConfiguration : Dictionary<string, double>
{
    public ClockmakerClassConfiguration() : base(new Dictionary<string, double>
    {
        ["classHunterLevelMultiply"] = 0.8,
        ["classBowLevelMultiply"] = 0.7,
        ["classSlingshotLevelMultiply"] = 1.0,
        ["classKnifeLevelMultiply"] = 1.0,
        ["classAxeLevelMultiply"] = 1.5,
        ["classPickaxeLevelMultiply"] = 2.0,
        ["classShovelLevelMultiply"] = 2.0,
        ["classSpearLevelMultiply"] = 0.7,
        ["classHammerLevelMultiply"] = 1.5,
        ["classSwordLevelMultiply"] = 0.4,
        ["classShieldLevelMultiply"] = 0.6,
        ["classHandLevelMultiply"] = 0.5,
        ["classFarmingLevelMultiply"] = 1.2,
        ["classCookingLevelMultiply"] = 1.0,
        ["classPanningLevelMultiply"] = 1.4,
        ["classVitalityLevelMultiply"] = 0.7,
        ["classMetabolismLevelMultiply"] = 1.3,
        ["classLeatherArmorLevelMultiply"] = 1.0,
        ["classChainArmorLevelMultiply"] = 1.0,
        ["classBrigandineArmorLevelMultiply"] = 1.0,
        ["classPlateArmorLevelMultiply"] = 1.0,
        ["classScaleArmorLevelMultiply"] = 1.0,
        ["classLamellarArmorLevelMultiply"] = 1.0,
        ["classSmithingLevelMultiply"] = 2.0,
    })
    { }
}

public class MalefactorClassConfiguration : Dictionary<string, double>
{
    public MalefactorClassConfiguration() : base(new Dictionary<string, double>
    {
        ["classHunterLevelMultiply"] = 1.2,
        ["classBowLevelMultiply"] = 0.6,
        ["classSlingshotLevelMultiply"] = 2.5,
        ["classKnifeLevelMultiply"] = 1.5,
        ["classAxeLevelMultiply"] = 1.0,
        ["classPickaxeLevelMultiply"] = 1.0,
        ["classShovelLevelMultiply"] = 1.0,
        ["classSpearLevelMultiply"] = 1.0,
        ["classHammerLevelMultiply"] = 1.0,
        ["classSwordLevelMultiply"] = 1.0,
        ["classShieldLevelMultiply"] = 1.0,
        ["classHandLevelMultiply"] = 1.5,
        ["classFarmingLevelMultiply"] = 1.5,
        ["classCookingLevelMultiply"] = 2.0,
        ["classPanningLevelMultiply"] = 1.8,
        ["classVitalityLevelMultiply"] = 0.5,
        ["classMetabolismLevelMultiply"] = 1.0,
        ["classLeatherArmorLevelMultiply"] = 1.5,
        ["classChainArmorLevelMultiply"] = 1.5,
        ["classBrigandineArmorLevelMultiply"] = 1.0,
        ["classPlateArmorLevelMultiply"] = 1.0,
        ["classScaleArmorLevelMultiply"] = 1.0,
        ["classLamellarArmorLevelMultiply"] = 1.0,
        ["classSmithingLevelMultiply"] = 0.7,
    })
    { }
}

public class TailorClassConfiguration : Dictionary<string, double>
{
    public TailorClassConfiguration() : base(new Dictionary<string, double>
    {
        ["classHunterLevelMultiply"] = 0.7,
        ["classBowLevelMultiply"] = 1.8,
        ["classSlingshotLevelMultiply"] = 1.0,
        ["classKnifeLevelMultiply"] = 1.8,
        ["classAxeLevelMultiply"] = 1.7,
        ["classPickaxeLevelMultiply"] = 1.4,
        ["classShovelLevelMultiply"] = 1.6,
        ["classSpearLevelMultiply"] = 1.0,
        ["classHammerLevelMultiply"] = 1.0,
        ["classSwordLevelMultiply"] = 0.6,
        ["classShieldLevelMultiply"] = 0.6,
        ["classHandLevelMultiply"] = 0.4,
        ["classFarmingLevelMultiply"] = 2.0,
        ["classCookingLevelMultiply"] = 2.0,
        ["classPanningLevelMultiply"] = 2.5,
        ["classVitalityLevelMultiply"] = 0.3,
        ["classMetabolismLevelMultiply"] = 1.5,
        ["classLeatherArmorLevelMultiply"] = 2.0,
        ["classChainArmorLevelMultiply"] = 2.0,
        ["classBrigandineArmorLevelMultiply"] = 1.0,
        ["classPlateArmorLevelMultiply"] = 1.0,
        ["classScaleArmorLevelMultiply"] = 1.0,
        ["classLamellarArmorLevelMultiply"] = 1.0,
        ["classSmithingLevelMultiply"] = 2.0,
    })
    { }
}

public static partial class Configuration
{
    public static Dictionary<string, Dictionary<string, object>> ClassExperience { get; private set; } = [];

    public static void RegisterNewClassLevel(string currentClass, string levelType, float multiply)
    {
        if (ClassExperience.TryGetValue(currentClass, out Dictionary<string, object> availableLevels))
            if (!availableLevels.TryGetValue(levelType, out _))
                availableLevels.Add(levelType, multiply);
            else
                Debug.LogWarn($"You are trying to set up {levelType} but that level already exist in {currentClass}, it will be ignored...");
        else
            ClassExperience.Add(currentClass, new Dictionary<string, object>
            {
                { levelType, multiply }
            });

        Debug.LogDebug($"Class added: {currentClass} levelType: {levelType} multiply: {multiply}");
    }

    public static float GetEXPMultiplyByClassAndLevelType(string playerClass, string levelType)
    {
        // Class converssion
        playerClass += "class";
        if (ClassExperience.TryGetValue(playerClass, out Dictionary<string, object> classConfigs))
        {
            try
            {
                return (float)Convert.ToSingle(classConfigs[$"class{levelType}LevelMultiply"]);
            }
            catch (Exception ex)
            {
                Debug.LogError($"ERROR: Unable to find the value from {levelType} in {playerClass} configurations, you probably miss something in the json configuration, ex message: {ex.Message}");
                return 1.0f;
            }
        }
        Debug.LogError($"ERROR: The class {playerClass} does not exist in the configurations, probably a custom class without configs");
        return 1.0f;
    }

    private static void RegisterClassFromDictionary(string configname, Dictionary<string, double> config)
    {
        ClassExperience.Add(configname, []);
        foreach (KeyValuePair<string, double> pair in config)
        {
            RegisterNewClassLevel(configname, pair.Key, Convert.ToSingle(pair.Value));
        }
    }

    public static void PopulateClassConfigurations(ICoreAPI api)
    {
        ClassExperience.Clear();
        string directoryPath = Path.Combine(api.DataBasePath, "ModConfig/LevelUP/classexp");
        // Classes directory exists
        if (Directory.Exists(directoryPath))
        {
            Debug.Log("Loading server classes...");
            string[] configs = Directory.GetFiles(directoryPath);
            // Swipe all files
            foreach (string confignameWithExtension in configs)
            {
                // Get only the file name
                string configname = Path.GetFileNameWithoutExtension(confignameWithExtension);
                try
                {
                    // Null check
                    if (ClassExperience.ContainsKey(configname))
                    {
                        Debug.LogWarn($"WARNING: {configname} already exist in memory, duplicated class? how?");
                        continue;
                    }

                    // Get the configuration for the respective file
                    Dictionary<string, double> configClass = ConfigManager.Load<Dictionary<string, double>>(
                        api, "ModConfig/LevelUP/classexp", configname, Logger(api));
                    RegisterClassFromDictionary(configname, configClass);
                    Debug.Log($"{configname} configuration set");
                }
                catch (Exception ex)
                {
                    Debug.Log($"ERROR: Cannot load the class {configname}, probably the file is invalid? reason: {ex.Message}");
                }
            }

            Debug.Log($"Server classes loaded, total loaded classes: {ClassExperience.Count}");
        }
        // Classes directory doesn't exist
        else
        {
            Debug.LogWarn("WARNING: Server configuration classes directory doesn't exist, creating default classes");

            RegisterClassFromDictionary("hunterclass", ConfigManager.Load<HunterClassConfiguration>(api, "ModConfig/LevelUP/classexp", "hunterclass", Logger(api)));
            RegisterClassFromDictionary("commonerclass", ConfigManager.Load<CommonerClassConfiguration>(api, "ModConfig/LevelUP/classexp", "commonerclass", Logger(api)));
            RegisterClassFromDictionary("blackguardclass", ConfigManager.Load<BlackguardClassConfiguration>(api, "ModConfig/LevelUP/classexp", "blackguardclass", Logger(api)));
            RegisterClassFromDictionary("clockmakerclass", ConfigManager.Load<ClockmakerClassConfiguration>(api, "ModConfig/LevelUP/classexp", "clockmakerclass", Logger(api)));
            RegisterClassFromDictionary("malefactorclass", ConfigManager.Load<MalefactorClassConfiguration>(api, "ModConfig/LevelUP/classexp", "malefactorclass", Logger(api)));
            RegisterClassFromDictionary("tailorclass", ConfigManager.Load<TailorClassConfiguration>(api, "ModConfig/LevelUP/classexp", "tailorclass", Logger(api)));
        }
    }
}
