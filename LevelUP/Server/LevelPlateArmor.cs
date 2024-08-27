using Vintagestory.API.Common;

namespace LevelUP.Server;

class LevelPlateArmor
{
    public Instance instance;

    public void Init(Instance _instance)
    {
        instance = _instance;
        Debug.Log("Level Plate Armor initialized");
    }

#pragma warning disable CA1822
    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulatePlateArmorConfiguration(coreAPI);
    }
#pragma warning restore CA1822
}