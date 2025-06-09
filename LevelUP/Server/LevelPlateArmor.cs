using Vintagestory.API.Common;

namespace LevelUP.Server;

class LevelPlateArmor
{
#pragma warning disable CA1822
    public void Init()
    {
        Debug.Log("Level Plate Armor initialized");
    }

    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulatePlateArmorConfiguration(coreAPI);
    }
#pragma warning restore CA1822
}