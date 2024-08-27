using Vintagestory.API.Common;

namespace LevelUP.Server;

class LevelBrigandineArmor
{
    public Instance instance;

    public void Init(Instance _instance)
    {
        instance = _instance;
        Debug.Log("Level Brigandine Armor initialized");
    }

#pragma warning disable CA1822
    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulateBrigandineArmorConfiguration(coreAPI);
    }
#pragma warning restore CA1822
}