using Vintagestory.API.Common;

namespace LevelUP.Server;

class LevelCooking
{
#pragma warning disable CA1822
    public void Init()
    {
        Configuration.RegisterNewLevelTypeEXP("Cooking", Configuration.CookingGetLevelByEXP);
        Debug.Log("Level Cooking initialized");
    }

    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulateCookingConfiguration(coreAPI);
        Configuration.RegisterNewMaxLevelByLevelTypeEXP("Cooking", Configuration.cookingMaxLevel);
    }
#pragma warning restore CA1822
}