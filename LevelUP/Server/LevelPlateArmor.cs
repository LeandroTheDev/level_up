using Vintagestory.API.Common;

namespace LevelUP.Server;

class LevelPlateArmor
{
#pragma warning disable CA1822
    public void Init()
    {
        Configuration.RegisterNewLevelTypeEXP("PlateArmor", Configuration.PlateArmorGetLevelByEXP);
        Configuration.RegisterNewEXPLevelType("PlateArmor", Configuration.PlateArmorGetExpByLevel);

        Debug.Log("Level Plate Armor initialized");
    }

    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulatePlateArmorConfiguration(coreAPI);
        Configuration.RegisterNewMaxLevelByLevelTypeEXP("PlateArmor", Configuration.plateArmorMaxLevel);
    }
#pragma warning restore CA1822
}