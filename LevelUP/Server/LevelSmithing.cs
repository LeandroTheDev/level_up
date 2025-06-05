using Vintagestory.API.Common;

namespace LevelUP.Server;

class LevelSmithing
{
    public Instance instance;

    public void Init(Instance _instance)
    {
        instance = _instance;
        Debug.Log("Level Smithing initialized");
    }

#pragma warning disable CA1822
    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulateSmithingConfiguration(coreAPI);
    }
#pragma warning restore CA1822
}