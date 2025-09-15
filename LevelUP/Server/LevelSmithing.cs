using Vintagestory.API.Common;

namespace LevelUP.Server;

class LevelSmithing
{
#pragma warning disable CA1822
    public void Init()
    {
        Configuration.RegisterNewLevelTypeEXP("Smithing", Configuration.SmithingGetLevelByEXP);
        Configuration.RegisterNewEXPLevelType("Smithing", Configuration.SmithingGetExpByLevel);

        Debug.Log("Level Smithing initialized");
    }

    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulateSmithingConfiguration(coreAPI);
        Configuration.RegisterNewMaxLevelByLevelTypeEXP("Smithing", Configuration.smithingMaxLevel);
    }
#pragma warning restore CA1822
}