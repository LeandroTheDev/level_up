using Vintagestory.API.Common;
namespace LevelUP.Server;

class LevelLeatherArmor
{
#pragma warning disable CA1822
    public void Init()
    {
        Debug.Log("Level Leather Armor initialized");
    }

    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulateLeatherArmorConfiguration(coreAPI);
    }
#pragma warning restore CA1822
}