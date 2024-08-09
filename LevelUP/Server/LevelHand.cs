using System;
using System.Collections.Generic;
using System.Text.Json;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Util;
using Vintagestory.GameContent;

namespace LevelUP.Server;

class LevelHand
{
    private Instance instance;

    public void Init(Instance _instance)
    {
        instance = _instance;
        // Instanciate death event
        instance.api.Event.OnEntityDeath += OnEntityDeath;

        Debug.Log("Level Hand initialized");
    }

#pragma warning disable CA1822
    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulateHandConfiguration(coreAPI);
    }
#pragma warning restore CA1822

    private Dictionary<string, ulong> GetSavedLevels()
    {
        byte[] dataBytes = instance.api.WorldManager.SaveGame.GetData("LevelUPData_Hand");
        string data = dataBytes == null ? "{}" : SerializerUtil.Deserialize<string>(dataBytes);
        return JsonSerializer.Deserialize<Dictionary<string, ulong>>(data);
    }

    private void SaveLevels(Dictionary<string, ulong> handLevels)
    {
        instance.api.WorldManager.SaveGame.StoreData("LevelUPData_Hand", JsonSerializer.Serialize(handLevels));
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

        // Check if player is using the hands
        if (player.InventoryManager.ActiveHotbarSlot.Itemstack != null) return;

        // Get all players levels
        Dictionary<string, ulong> handLevels = GetSavedLevels();

        // Get the exp received
        float experienceMultiplierCompatibility = player.Entity.Attributes.GetFloat("LevelUP_Server_Instance_ExperienceMultiplier_IncreaseExp");
        int exp = (int)(Configuration.entityExpSword.GetValueOrDefault(entity.Code.ToString()) + (Configuration.entityExpSword.GetValueOrDefault(entity.Code.ToString()) * experienceMultiplierCompatibility));
        // Increasing by player class
        exp = (int)Math.Round(exp * Configuration.GetEXPMultiplyByClassAndLevelType(player.Entity.WatchedAttributes.GetString("characterClass"), "Hand"));
        // Minium exp earned is 1
        if (exp <= 0) exp = Configuration.minimumEXPEarned;

        // Get the actual player total exp
        ulong playerExp = handLevels.GetValueOrDefault<string, ulong>(player.PlayerUID, 0);

        if (Configuration.enableLevelUpExperienceServerLog)
            Debug.Log($"{player.PlayerName} killed: {entity.Code}, hand exp earned: {exp}, actual: {playerExp}");

        // Incrementing
        handLevels[player.PlayerUID] = playerExp + (ulong)exp;

        // Saving
        SaveLevels(handLevels);
        // Updating
        Shared.Instance.UpdateLevelAndNotify(instance.api, player, "Hand", handLevels[player.PlayerUID]);
    }
}