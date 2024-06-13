using Vintagestory.API.Common;

namespace LevelUP.Server;

class LevelCooking
{
    public Instance instance;

    public void Init(Instance _instance)
    {
        instance = _instance;
        Debug.Log("Level Cooking initialized");
    }

#pragma warning disable CA1822
    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulateCookingConfiguration(coreAPI);
    }
#pragma warning restore CA1822
}