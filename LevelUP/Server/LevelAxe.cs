using System.Collections.Generic;
using System.Text.Json;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Server;
using Vintagestory.API.Util;

namespace LevelUP.Server;

class LevelAxe
{
    private Instance instance;

    readonly Dictionary<string, int> entityExp = [];

    public void Init(Instance _instance)
    {
        instance = _instance;
        // Instanciate death event
        instance.api.Event.OnEntityDeath += OnEntityDeath;
        // Instanciate break block event
        instance.api.Event.BreakBlock += OnBreakBlock;

        // Populate configuration
        Configuration.PopulateAxeConfiguration();

        Debug.Log("Level Axe initialized");
    }

    private Dictionary<string, int> GetSavedLevels()
    {
        byte[] dataBytes = instance.api.WorldManager.SaveGame.GetData("LevelUPData_Axe");
        string data = dataBytes == null ? "{}" : SerializerUtil.Deserialize<string>(dataBytes);
        return JsonSerializer.Deserialize<Dictionary<string, int>>(data);
    }

    private void SaveLevels(Dictionary<string, int> axeLevels)
    {
        instance.api.WorldManager.SaveGame.StoreData("LevelUPData_Axe", JsonSerializer.Serialize(axeLevels));
    }

    public void OnEntityDeath(Entity entity, DamageSource damageSource)
    {
        // Error treatment
        if (damageSource == null || damageSource.SourceEntity == null) return;
        // Entity kill is not from a player
        if (damageSource.SourceEntity is not EntityPlayer) return;

        // Get player entity
        EntityPlayer playerEntity = damageSource.SourceEntity as EntityPlayer;

        // Get player instance
        IPlayer player = instance.api.World.PlayerByUid(playerEntity.PlayerUID);

        // Check if player is using a Axe
        if(player.InventoryManager.ActiveTool != EnumTool.Axe) return;

        // Get all players levels
        Dictionary<string, int> axeLevels = GetSavedLevels();

        // Get the exp received
        int exp = entityExp.GetValueOrDefault(entity.GetName(), 0);

        // Get the actual player total exp
        int playerExp = axeLevels.GetValueOrDefault(playerEntity.GetName(), 0);

        Debug.Log($"{playerEntity.GetName()} killed: {entity.GetName()}, axe exp earned: {exp}, actual: {playerExp}");

        // Incrementing
        axeLevels[playerEntity.GetName()] = playerExp + exp;

        // Saving
        SaveLevels(axeLevels);
        Shared.Instance.UpdateLevelAndNotify(instance.api, player, "Axe", playerExp + exp);
    }

    public void OnBreakBlock(IServerPlayer player, BlockSelection breakedBlock, ref float dropQuantityMultiplier, ref EnumHandling handling) {
        EntityPlayer playerEntity = player.Entity;
        // If not a shovel ignore
        if(player.InventoryManager.ActiveTool != EnumTool.Axe) return;
        if(breakedBlock.Block.BlockMaterial != EnumBlockMaterial.Wood) return;

        // Get all players levels
        Dictionary<string, int> axeLevels = GetSavedLevels();

        // Get the exp received
        int exp = Configuration.expPerBreakingAxe;

        // Get the actual player total exp
        int playerExp = axeLevels.GetValueOrDefault(playerEntity.GetName(), 0);

        Debug.Log($"{playerEntity.GetName()} breaked: {breakedBlock.Block.BlockMaterial}, axe exp earned: {exp}, actual: {playerExp}");
        // Incrementing
        axeLevels[playerEntity.GetName()] = playerExp + exp;

        // Saving
        SaveLevels(axeLevels);
        // Updating
        Shared.Instance.UpdateLevelAndNotify(instance.api, player, "Axe", playerExp + exp);
    }
}