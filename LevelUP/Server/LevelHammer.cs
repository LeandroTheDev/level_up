using System;
using System.Collections.Generic;
using System.Text.Json;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Server;
using Vintagestory.API.Util;
using Vintagestory.GameContent;

namespace LevelUP.Server;

class LevelHammer
{
    public Instance instance;

    public void Init(Instance _instance)
    {
        instance = _instance;
        instance.api.Event.OnEntityDeath += OnEntityDeath;
        Debug.Log("Level Hammer initialized");
    }

#pragma warning disable CA1822
    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulateHammerConfiguration(coreAPI);
    }
#pragma warning restore CA1822

    private Dictionary<string, ulong> GetSavedLevels()
    {
        byte[] dataBytes = instance.api.WorldManager.SaveGame.GetData("LevelUPData_Hammer");
        string data = dataBytes == null ? "{}" : SerializerUtil.Deserialize<string>(dataBytes);
        return JsonSerializer.Deserialize<Dictionary<string, ulong>>(data);
    }

    private void SaveLevels(Dictionary<string, ulong> hammerLevels)
    {
        instance.api.WorldManager.SaveGame.StoreData("LevelUPData_Hammer", JsonSerializer.Serialize(hammerLevels));
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

        // Check if player is using a Hammer
        if (player.InventoryManager.ActiveTool != EnumTool.Hammer) return;

        // Get all players levels
        Dictionary<string, ulong> hammerLevels = GetSavedLevels();

        // Get the exp received
        float experienceMultiplierCompatibility = player.Entity.Attributes.GetFloat("LevelUP_Server_Instance_ExperienceMultiplier_IncreaseExp");
        int exp = (int)(Configuration.entityExpHammer.GetValueOrDefault(entity.Code.ToString()) + (Configuration.entityExpHammer.GetValueOrDefault(entity.Code.ToString()) * experienceMultiplierCompatibility));
        // Increasing by player class
        exp = (int)Math.Round(exp * Configuration.GetEXPMultiplyByClassAndLevelType(player.Entity.WatchedAttributes.GetString("characterClass"), "Hammer"));
        // Minium exp earned is 1
        if (exp <= 0) exp = Configuration.minimumEXPEarned;

        // Get the actual player total exp
        ulong playerExp = hammerLevels.GetValueOrDefault<string, ulong>(player.PlayerUID, 0);
        if (Configuration.HammerIsMaxLevel(playerExp)) return;

        if (Configuration.enableLevelUpExperienceServerLog)
            Debug.Log($"{player.PlayerName} killed: {entity.Code}, hammer exp earned: {exp}, actual: {playerExp}");

        // Incrementing
        hammerLevels[player.PlayerUID] = playerExp + (ulong)exp;

        // Saving
        SaveLevels(hammerLevels);
        // Updating
        Shared.Instance.UpdateLevelAndNotify(instance.api, player, "Hammer", hammerLevels[player.PlayerUID]);
    }
}