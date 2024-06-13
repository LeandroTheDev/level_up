using System.Collections.Generic;
using System.Text.Json;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Util;
using Vintagestory.GameContent;

namespace LevelUP.Server;

class LevelKnife
{
    private Instance instance;

    public void Init(Instance _instance)
    {
        instance = _instance;
        // Instanciate death event
        instance.api.Event.OnEntityDeath += OnEntityDeath;

        Debug.Log("Level Knife initialized");
    }

#pragma warning disable CA1822
    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulateKnifeConfiguration(coreAPI);
    }
#pragma warning restore CA1822

    private Dictionary<string, ulong> GetSavedLevels()
    {
        byte[] dataBytes = instance.api.WorldManager.SaveGame.GetData("LevelUPData_Knife");
        string data = dataBytes == null ? "{}" : SerializerUtil.Deserialize<string>(dataBytes);
        return JsonSerializer.Deserialize<Dictionary<string, ulong>>(data);
    }

    private void SaveLevels(Dictionary<string, ulong> knifeLevels)
    {
        instance.api.WorldManager.SaveGame.StoreData("LevelUPData_Knife", JsonSerializer.Serialize(knifeLevels));
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

        // Check if player is using a bow
        if (player.InventoryManager.ActiveTool != EnumTool.Knife) return;

        // Get all players levels
        Dictionary<string, ulong> knifeLevels = GetSavedLevels();

        // Get the exp received
        float experienceMultiplierCompatibility = player.Entity.Attributes.GetFloat("LevelUP_Server_Instance_ExperienceMultiplier_IncreaseExp");
        int exp = (int)(Configuration.entityExpKnife.GetValueOrDefault(playerEntity.GetName(), 0) + (Configuration.entityExpKnife.GetValueOrDefault(playerEntity.GetName(), 0) * experienceMultiplierCompatibility));

        // Get the actual player total exp
        ulong playerExp = knifeLevels.GetValueOrDefault<string, ulong>(playerEntity.GetName(), 0);

        if (Configuration.enableExtendedLog)
            Debug.Log($"{playerEntity.GetName()} killed: {entity.Code}, knife exp earned: {exp}, actual: {playerExp}");

        // Incrementing
        knifeLevels[playerEntity.GetName()] = playerExp + (ulong)exp;

        // Saving
        SaveLevels(knifeLevels);
        // Updating
        Shared.Instance.UpdateLevelAndNotify(instance.api, player, "Knife", knifeLevels[playerEntity.GetName()]);
    }
}