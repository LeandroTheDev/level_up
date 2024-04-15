using System.Collections.Generic;
using System.Text.Json;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using Vintagestory.API.Util;
using Vintagestory.GameContent;

namespace LevelUP.Server;

class LevelChainArmor
{
    public Instance instance;

    public void Init(Instance _instance)
    {
        instance = _instance;
        Debug.Log("Level Chain Armor initialized");
    }

#pragma warning disable CA1822
    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulateChainArmorConfiguration(coreAPI);
    }
#pragma warning restore CA1822

    // private Dictionary<string, ulong> GetSavedLevels()
    // {
    //     byte[] dataBytes = instance.api.WorldManager.SaveGame.GetData("LevelUPData_ChainArmor");
    //     string data = dataBytes == null ? "{}" : SerializerUtil.Deserialize<string>(dataBytes);
    //     return JsonSerializer.Deserialize<Dictionary<string, ulong>>(data);
    // }

    // private void SaveState(Dictionary<string, ulong> chainArmorLevels)
    // {
    //     instance.api.WorldManager.SaveGame.StoreData("LevelUPData_Vitality_ChainArmor", JsonSerializer.Serialize(chainArmorLevels));
    // }
}