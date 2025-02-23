using System;
using System.Collections.Generic;
using System.Text.Json;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Util;

namespace LevelUP.Server;

class LevelHunter
{
    private Instance instance;

    public void Init(Instance _instance)
    {
        instance = _instance;
        // Instanciate death event
        instance.api.Event.OnEntityDeath += OnEntityDeath;

        Debug.Log("Level Hunter initialized");
    }

#pragma warning disable CA1822
    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulateHunterConfiguration(coreAPI);
    }
#pragma warning restore CA1822

    private Dictionary<string, ulong> GetSavedLevels()
    {
        byte[] dataBytes = instance.api.WorldManager.SaveGame.GetData("LevelUPData_Hunter");
        string data = dataBytes == null ? "{}" : SerializerUtil.Deserialize<string>(dataBytes);
        return JsonSerializer.Deserialize<Dictionary<string, ulong>>(data);
    }

    private void SaveLevels(Dictionary<string, ulong> hunterLevels)
    {
        instance.api.WorldManager.SaveGame.StoreData("LevelUPData_Hunter", JsonSerializer.Serialize(hunterLevels));
    }

    public void OnEntityDeath(Entity entity, DamageSource damageSource)
    {
        // Error treatment
        if (damageSource == null) return;
        // Get player entity
        EntityPlayer playerEntity;
        if (damageSource.SourceEntity is EntityPlayer) playerEntity = damageSource.SourceEntity as EntityPlayer;
        else if (damageSource.GetCauseEntity() is EntityPlayer) playerEntity = damageSource.GetCauseEntity() as EntityPlayer;
        else return;

        // Get player instance
        IPlayer player = instance.api.World.PlayerByUid(playerEntity.PlayerUID);

        // Get all players levels
        Dictionary<string, ulong> hunterLevels = GetSavedLevels();

        // Get the exp received
        float experienceMultiplierCompatibility = player.Entity.Attributes.GetFloat("LevelUP_Server_Instance_ExperienceMultiplier_IncreaseExp");
        int exp = (int)(Configuration.entityExpHunter.GetValueOrDefault(entity.Code.ToString()) + (Configuration.entityExpHunter.GetValueOrDefault(entity.Code.ToString()) * experienceMultiplierCompatibility));
        // Increasing by player class
        exp = (int)Math.Round(exp * Configuration.GetEXPMultiplyByClassAndLevelType(player.Entity.WatchedAttributes.GetString("characterClass"), "Hunter"));
        // Minium exp earned is 1
        if (exp <= 0) exp = Configuration.minimumEXPEarned;

        // Get the actual player total exp
        ulong playerExp = hunterLevels.GetValueOrDefault<string, ulong>(player.PlayerUID, 0);
        if (Configuration.HunterIsMaxLevel(playerExp)) return;

        if (Configuration.enableLevelUpExperienceServerLog)
            Debug.Log($"{player.PlayerName} killed: {entity.Code}, hunter exp earned: {exp}, actual: {playerExp}");

        // Incrementing
        hunterLevels[player.PlayerUID] = playerExp + (ulong)exp;

        // Saving
        SaveLevels(hunterLevels);
        // Updating
        Shared.Instance.UpdateLevelAndNotify(instance.api, player, "Hunter", hunterLevels[player.PlayerUID]);
    }
}