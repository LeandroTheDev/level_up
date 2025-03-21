using System;
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

    private Dictionary<string, ulong> GetSavedLevels()
    {
        byte[] dataBytes = instance.api.WorldManager.SaveGame.GetData("LevelUPData_Pickaxe");
        string data = dataBytes == null ? "{}" : SerializerUtil.Deserialize<string>(dataBytes);
        return JsonSerializer.Deserialize<Dictionary<string, ulong>>(data);
    }

    private void SaveLevels(Dictionary<string, ulong> pickaxeLevels)
    {
        instance.api.WorldManager.SaveGame.StoreData("LevelUPData_Pickaxe", JsonSerializer.Serialize(pickaxeLevels));
    }

    public void OnEntityDeath(Entity entity, DamageSource damageSource)
    {
        // Error treatment
        if (damageSource == null) return;
        // The cause of the death is from a projectile
        if (damageSource.GetCauseEntity() is not EntityPlayer && damageSource.SourceEntity is EntityProjectile) return;
        // Entity kill is not from a player
        if (damageSource.SourceEntity is not EntityPlayer) return;

        // Get player entity
        EntityPlayer playerEntity = damageSource.SourceEntity as EntityPlayer;

        // Get player instance
        IPlayer player = playerEntity.Player;

        // Check if player is using a Pickaxe
        if (player.InventoryManager.ActiveTool != EnumTool.Pickaxe) return;

        // Get all players levels
        Dictionary<string, ulong> pickaxeLevels = GetSavedLevels();

        // Get the exp received
        float experienceMultiplierCompatibility = player.Entity.Attributes.GetFloat("LevelUP_Server_Instance_ExperienceMultiplier_IncreaseExp");
        int exp = (int)(Configuration.entityExpPickaxe.GetValueOrDefault(entity.Code.ToString()) + (Configuration.entityExpPickaxe.GetValueOrDefault(entity.Code.ToString()) * experienceMultiplierCompatibility));
        // Increasing by player class
        exp = (int)Math.Round(exp * Configuration.GetEXPMultiplyByClassAndLevelType(player.Entity.WatchedAttributes.GetString("characterClass"), "Pickaxe"));
        // Minium exp earned is 1
        if (exp <= 0) exp = Configuration.minimumEXPEarned;

        // Get the actual player total exp
        ulong playerExp = pickaxeLevels.GetValueOrDefault<string, ulong>(player.PlayerUID, 0);
        if (Configuration.PickaxeIsMaxLevel(playerExp)) return;

        if (Configuration.enableLevelUpExperienceServerLog)
            Debug.Log($"{player.PlayerName} killed: {entity.Code}, pickaxe exp earned: {exp}, actual: {playerExp}");

        // Incrementing
        pickaxeLevels[player.PlayerUID] = playerExp + (ulong)exp;

        // Saving
        SaveLevels(pickaxeLevels);
        // Updating
        Shared.Instance.UpdateLevelAndNotify(instance.api, player, "Pickaxe", pickaxeLevels[player.PlayerUID]);
    }

    public void OnBreakBlock(IServerPlayer player, BlockSelection breakedBlock, ref float dropQuantityMultiplier, ref EnumHandling handling)
    {
        // If not a shovel ignore
        if (player.InventoryManager.ActiveTool != EnumTool.Pickaxe) return;
        if (breakedBlock.Block.BlockMaterial != EnumBlockMaterial.Stone && breakedBlock.Block.BlockMaterial != EnumBlockMaterial.Ore) return;

        // Get all players levels
        Dictionary<string, ulong> pickaxeLevels = GetSavedLevels();

        // Get the exp received
        float experienceMultiplierCompatibility = player.Entity.Attributes.GetFloat("LevelUP_Server_Instance_ExperienceMultiplier_IncreaseExp");
        int exp = (int)(Configuration.oresExpPickaxe.GetValueOrDefault(breakedBlock.Block.Code.ToString(), Configuration.ExpPerBreakingPickaxe) + (Configuration.oresExpPickaxe.GetValueOrDefault(breakedBlock.Block.Code.ToString(), Configuration.ExpPerBreakingPickaxe) * experienceMultiplierCompatibility));
        // Increasing by player class
        exp = (int)Math.Round(exp * Configuration.GetEXPMultiplyByClassAndLevelType(player.Entity.WatchedAttributes.GetString("characterClass"), "Pickaxe"));
        // Minium exp earned is 1
        if (exp <= 0) exp = Configuration.minimumEXPEarned;

        // Get the actual player total exp
        ulong playerExp = pickaxeLevels.GetValueOrDefault<string, ulong>(player.PlayerUID, 0);

        Debug.Log($"{player.PlayerName} breaked: {breakedBlock.Block.Code}, pickaxe exp earned: {exp}, actual: {playerExp}");
        // Incrementing
        pickaxeLevels[player.PlayerUID] = playerExp + (ulong)exp;

        // Saving
        SaveLevels(pickaxeLevels);
        // Updating
        Shared.Instance.UpdateLevelAndNotify(instance.api, player, "Pickaxe", pickaxeLevels[player.PlayerUID]);
    }
}