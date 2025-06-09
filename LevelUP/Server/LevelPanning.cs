using Vintagestory.API.Common;

namespace LevelUP.Server;

class LevelPanning
{
#pragma warning disable CA1822
    public void Init()
    {
        Debug.Log("Level Panning initialized");
    }

    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulatePanningConfiguration(coreAPI);
    }
#pragma warning restore CA1822
}