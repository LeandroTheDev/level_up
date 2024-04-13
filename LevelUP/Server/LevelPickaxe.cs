using System.Collections.Generic;
using System.Text.Json;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Server;
using Vintagestory.API.Util;
using Vintagestory.GameContent;

namespace LevelUP.Server;

class LevelPickaxe
{
    private Instance instance;

    public void Init(Instance _instance)
    {
        instance = _instance;
        // Instanciate death event
        instance.api.Event.OnEntityDeath += OnEntityDeath;
        // Instanciate break block event
        instance.api.Event.BreakBlock += OnBreakBlock;

        Debug.Log("Level Pickaxe initialized");
    }

    #pragma warning disable CA1822
    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulatePickaxeConfiguration(coreAPI);
    }    
    #pragma warning restore CA1822

    private Dictionary<string, int> GetSavedLevels()
    {
        byte[] dataBytes = instance.api.WorldManager.SaveGame.GetData("LevelUPData_Pickaxe");
        string data = dataBytes == null ? "{}" : SerializerUtil.Deserialize<string>(dataBytes);
        return JsonSerializer.Deserialize<Dictionary<string, int>>(data);
    }

    private void SaveLevels(Dictionary<string, int> pickaxeLevels)
    {
        instance.api.WorldManager.SaveGame.StoreData("LevelUPData_Pickaxe", JsonSerializer.Serialize(pickaxeLevels));
    }

    public void OnEntityDeath(Entity entity, DamageSource damageSource)
    {
        // Check if entity is alive
        if (!entity.Alive) return;
        // The cause of the death is from a projectile
        if (damageSource.GetCauseEntity() is not EntityPlayer && damageSource.SourceEntity is EntityProjectile) return;
        // Entity kill is not from a player
        if (damageSource.SourceEntity is not EntityPlayer) return;

        // Get player entity
        EntityPlayer playerEntity = damageSource.SourceEntity as EntityPlayer;

        // Get player instance
        IPlayer player = instance.api.World.PlayerByUid(playerEntity.PlayerUID);

        // Check if player is using a Pickaxe
        if (player.InventoryManager.ActiveTool != EnumTool.Pickaxe) return;

        // Get all players levels
        Dictionary<string, int> pickaxeLevels = GetSavedLevels();

        // Get the exp received
        int exp = Configuration.entityExpPickaxe.GetValueOrDefault(entity.GetName(), 0);

        // Get the actual player total exp
        int playerExp = pickaxeLevels.GetValueOrDefault(playerEntity.GetName(), 0);

        Debug.Log($"{playerEntity.GetName()} killed: {entity.GetName()}, pickaxe exp earned: {exp}, actual: {playerExp}");

        // Incrementing
        pickaxeLevels[playerEntity.GetName()] = playerExp + exp;

        // Saving
        SaveLevels(pickaxeLevels);
        // Updating
        Shared.Instance.UpdateLevelAndNotify(instance.api, player, "Pickaxe", playerExp + exp);
    }

    public void OnBreakBlock(IServerPlayer player, BlockSelection breakedBlock, ref float dropQuantityMultiplier, ref EnumHandling handling)
    {
        EntityPlayer playerEntity = player.Entity;
        // If not a shovel ignore
        if (player.InventoryManager.ActiveTool != EnumTool.Pickaxe) return;
        if (breakedBlock.Block.BlockMaterial != EnumBlockMaterial.Stone && breakedBlock.Block.BlockMaterial != EnumBlockMaterial.Ore) return;

        // Get all players levels
        Dictionary<string, int> pickaxeLevels = GetSavedLevels();

        // Get the exp received
        int exp = Configuration.ExpPerBreakingPickaxe;

        // Get the actual player total exp
        int playerExp = pickaxeLevels.GetValueOrDefault(playerEntity.GetName(), 0);

        Debug.Log($"{playerEntity.GetName()} breaked: {breakedBlock.Block.Code}, pickaxe exp earned: {exp}, actual: {playerExp}");
        // Incrementing
        pickaxeLevels[playerEntity.GetName()] = playerExp + exp;

        // Saving
        SaveLevels(pickaxeLevels);
        // Updating
        Shared.Instance.UpdateLevelAndNotify(instance.api, player, "Pickaxe", playerExp + exp);
    }
}