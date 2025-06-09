using Vintagestory.API.Common;

namespace LevelUP.Server;

class LevelShield
{
#pragma warning disable CA1822
    public void Init()
    {
        Debug.Log("Level Shield initialized");
    }

    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulateShieldConfiguration(coreAPI);
    }
#pragma warning restore CA1822
}