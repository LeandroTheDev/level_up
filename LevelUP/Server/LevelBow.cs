using System.Collections.Generic;
using System.Text.Json;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Util;
using Vintagestory.GameContent;

namespace LevelUP.Server;

class LevelBow
{
    private Instance instance;

    public void Init(Instance _instance)
    {
        instance = _instance;
        // Instanciate death event
        instance.api.Event.OnEntityDeath += OnEntityDeath;

        Debug.Log("Level Bow initialized");
    }

#pragma warning disable CA1822
    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulateBowConfiguration(coreAPI);
    }
#pragma warning restore CA1822

    private Dictionary<string, ulong> GetSavedLevels()
    {
        byte[] dataBytes = instance.api.WorldManager.SaveGame.GetData("LevelUPData_Bow");
        string data = dataBytes == null ? "{}" : SerializerUtil.Deserialize<string>(dataBytes);
        return JsonSerializer.Deserialize<Dictionary<string, ulong>>(data);
    }

    private void SaveLevels(Dictionary<string, ulong> bowLevels)
    {
        instance.api.WorldManager.SaveGame.StoreData("LevelUPData_Bow", JsonSerializer.Serialize(bowLevels));
    }

    public void OnEntityDeath(Entity entity, DamageSource damageSource)
    {
        // Error treatment
        if (damageSource == null) return;
        // Checking ranged weapon damage
        if (damageSource.SourceEntity is EntityProjectile && damageSource.GetCauseEntity() is EntityPlayer)
        {
            // Get entities
            EntityProjectile itemDamage = damageSource.SourceEntity as EntityProjectile;
            // Check if projectile is not from any arrow
            if (!itemDamage.GetName().Contains("arrow")) return;
            EntityPlayer playerEntity = damageSource.GetCauseEntity() as EntityPlayer;

            // Get player instance
            IPlayer player = playerEntity.Player;

            // Get all players levels
            Dictionary<string, ulong> bowLevels = GetSavedLevels();

            // Get the exp received
            float experienceMultiplierCompatibility = player.Entity.Attributes.GetFloat("LevelUP_Server_Instance_ExperienceMultiplier_IncreaseExp");
            int exp = (int)(Configuration.entityExpBow.GetValueOrDefault(entity.Code.ToString(), 0) + (Configuration.entityExpBow.GetValueOrDefault(entity.Code.ToString(), 0) * experienceMultiplierCompatibility));

            // Get the actual player total exp
            ulong playerExp = bowLevels.GetValueOrDefault<string, ulong>(playerEntity.GetName(), 0);

            if (Configuration.enableLevelUpExperienceServerLog)
                Debug.Log($"{playerEntity.GetName()} killed: {entity.Code}, bow exp earned: {exp}, actual: {playerExp}");

            // Incrementing
            bowLevels[playerEntity.GetName()] = playerExp + (ulong)exp;

            // Saving
            SaveLevels(bowLevels);
            // Updating
            Shared.Instance.UpdateLevelAndNotify(instance.api, player, "Bow", bowLevels[playerEntity.GetName()]);
        }
    }
}