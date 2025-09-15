using Vintagestory.API.Common;

namespace LevelUP.Server;

class LevelChainArmor
{
#pragma warning disable CA1822
    public void Init()
    {
        Configuration.RegisterNewLevelTypeEXP("ChainArmor", Configuration.ChainArmorGetLevelByEXP);
        Configuration.RegisterNewEXPLevelType("ChainArmor", Configuration.ChainArmorGetExpByLevel);

        Debug.Log("Level Chain Armor initialized");
    }

    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulateChainArmorConfiguration(coreAPI);
        Configuration.RegisterNewMaxLevelByLevelTypeEXP("ChainArmor", Configuration.chainArmorMaxLevel);
    }
#pragma warning restore CA1822
}