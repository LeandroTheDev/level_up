using Vintagestory.API.Common;

namespace LevelUP.Server;

class LevelSmithing
{
    public Instance instance;

#pragma warning disable CA1822
    public void Init()
    {
        Debug.Log("Level Smithing initialized");
    }

    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulateSmithingConfiguration(coreAPI);
    }
#pragma warning restore CA1822
}