using Vintagestory.API.Common;

namespace LevelUP.Server;

class LevelScaleArmor
{
#pragma warning disable CA1822
    public void Init()
    {
        Configuration.RegisterNewLevelTypeEXP("ScaleArmor", Configuration.ScaleArmorGetLevelByEXP);
        Configuration.RegisterNewEXPLevelType("ScaleArmor", Configuration.ScaleArmorGetExpByLevel);

        Debug.Log("Level Scale Armor initialized");
    }

    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulateScaleArmorConfiguration(coreAPI);
        Configuration.RegisterNewMaxLevelByLevelTypeEXP("ScaleArmor", Configuration.scaleArmorMaxLevel);
    }
#pragma warning restore CA1822
}