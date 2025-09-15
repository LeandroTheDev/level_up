using Vintagestory.API.Common;

namespace LevelUP.Server;

class LevelBrigandineArmor
{
#pragma warning disable CA1822
    public void Init()
    {
        Configuration.RegisterNewLevelTypeEXP("BrigandineArmor", Configuration.BrigandineArmorGetLevelByEXP);
        Configuration.RegisterNewEXPLevelType("BrigandineArmor", Configuration.BrigandineArmorGetExpByLevel);

        Debug.Log("Level Brigandine Armor initialized");
    }

    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulateBrigandineArmorConfiguration(coreAPI);
        Configuration.RegisterNewMaxLevelByLevelTypeEXP("BrigandineArmor", Configuration.brigandineArmorMaxLevel);
    }
#pragma warning restore CA1822
}