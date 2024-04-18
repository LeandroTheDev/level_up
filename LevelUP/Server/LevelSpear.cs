using System.Collections.Generic;
using System.Text.Json;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Util;
using Vintagestory.GameContent;

namespace LevelUP.Server;

class LevelSpear
{
    private Instance instance;

    public void Init(Instance _instance)
    {
        instance = _instance;
        // Instanciate death event
        instance.api.Event.OnEntityDeath += OnEntityDeath;

        Debug.Log("Level Spear initialized");
    }

#pragma warning disable CA1822
    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulateSpearConfiguration(coreAPI);
    }
#pragma warning restore CA1822

    private Dictionary<string, ulong> GetSavedLevels()
    {
        byte[] dataBytes = instance.api.WorldManager.SaveGame.GetData("LevelUPData_Spear");
        string data = dataBytes == null ? "{}" : SerializerUtil.Deserialize<string>(dataBytes);
        return JsonSerializer.Deserialize<Dictionary<string, ulong>>(data);
    }

    private void SaveLevels(Dictionary<string, ulong> spearLevels)
    {
        instance.api.WorldManager.SaveGame.StoreData("LevelUPData_Spear", JsonSerializer.Serialize(spearLevels));
    }

    public void OnEntityDeath(Entity entity, DamageSource damageSource)
    {
        // Error treatment
        if (damageSource == null) return;

        EntityProjectile itemDamage;
        EntityPlayer playerEntity;
        IPlayer player;
        // If Spear is throwing
        if (damageSource.SourceEntity is EntityProjectile && damageSource.GetCauseEntity() is EntityPlayer)
        {
            // Get damage entity
            itemDamage = damageSource.SourceEntity as EntityProjectile;
            // Check if damage is from a spear
            if (!itemDamage.GetName().Contains("spear")) return;
            // Get player entity
            playerEntity = damageSource.GetCauseEntity() as EntityPlayer;
            // Get player instance
            player = instance.api.World.PlayerByUid(playerEntity.PlayerUID);
        }
        // If Spear is a normal attack
        else
        {
            // Check if the source damage is not from a player
            if (damageSource.SourceEntity is not EntityPlayer) return;
            // Get player entity
            playerEntity = damageSource.SourceEntity as EntityPlayer;
            // Get player instance
            player = instance.api.World.PlayerByUid(playerEntity.PlayerUID);
            // Check if player is using a spear
            if (player.InventoryManager.ActiveTool != EnumTool.Spear) return;
        }

        // Get all players levels
        Dictionary<string, ulong> spearLevels = GetSavedLevels();

        // Get the exp received
        int exp = Configuration.entityExpSpear.GetValueOrDefault(entity.GetName(), 0);

        // Get the actual player total exp
        ulong playerExp = spearLevels.GetValueOrDefault<string, ulong>(playerEntity.GetName(), 0);

        if (Configuration.enableExtendedLog)
            Debug.Log($"{playerEntity.GetName()} killed: {entity.GetName()}, spear exp earned: {exp}, actual: {playerExp}");

        // Incrementing
        spearLevels[playerEntity.GetName()] = playerExp + (ulong)exp;

        // Saving
        SaveLevels(spearLevels);
        // Updating
        Shared.Instance.UpdateLevelAndNotify(instance.api, player, "Spear", spearLevels[playerEntity.GetName()]);
    }
}