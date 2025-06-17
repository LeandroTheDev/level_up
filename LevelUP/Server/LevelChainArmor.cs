using Vintagestory.API.Common;

namespace LevelUP.Server;

class LevelChainArmor
{
#pragma warning disable CA1822
    public void Init()
    {
        Configuration.RegisterNewLevelTypeEXP("ChainArmor", Configuration.ChainArmorGetLevelByEXP);
        Debug.Log("Level Chain Armor initialized");
    }

    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulateChainArmorConfiguration(coreAPI);
    }
#pragma warning restore CA1822
}