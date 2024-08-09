using Vintagestory.API.Common;

namespace LevelUP.Server;

class LevelPanning
{
    public Instance instance;

    public void Init(Instance _instance)
    {
        instance = _instance;
        Debug.Log("Level Panning initialized");
    }

#pragma warning disable CA1822
    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulatePanningConfiguration(coreAPI);
    }
#pragma warning restore CA1822
}