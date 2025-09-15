using Vintagestory.API.Common;

namespace LevelUP.Server;

class LevelPanning
{
#pragma warning disable CA1822
    public void Init()
    {
        Configuration.RegisterNewLevelTypeEXP("Panning", Configuration.PanningGetLevelByEXP);
        Configuration.RegisterNewEXPLevelType("Panning", Configuration.PanningGetExpByLevel);

        Debug.Log("Level Panning initialized");
    }

    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulatePanningConfiguration(coreAPI);
        Configuration.RegisterNewMaxLevelByLevelTypeEXP("Panning", Configuration.panningMaxLevel);
    }
#pragma warning restore CA1822
}