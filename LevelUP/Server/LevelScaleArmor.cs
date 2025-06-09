using Vintagestory.API.Common;

namespace LevelUP.Server;

class LevelScaleArmor
{
#pragma warning disable CA1822
    public void Init()
    {
        Debug.Log("Level Scale Armor initialized");
    }

    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulateScaleArmorConfiguration(coreAPI);
    }
#pragma warning restore CA1822
}