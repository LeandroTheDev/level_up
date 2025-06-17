using Vintagestory.API.Common;

namespace LevelUP.Server;

class LevelBrigandineArmor
{
#pragma warning disable CA1822
    public void Init()
    {
        Configuration.RegisterNewLevelTypeEXP("BrigandineArmor", Configuration.BrigandineArmorGetLevelByEXP);
        Debug.Log("Level Brigandine Armor initialized");
    }

    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulateBrigandineArmorConfiguration(coreAPI);
    }
#pragma warning restore CA1822
}