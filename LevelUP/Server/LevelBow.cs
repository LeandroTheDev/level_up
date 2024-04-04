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

    readonly Dictionary<string, int> entityExp = [];

    public void Init(Instance _instance)
    {
        instance = _instance;
        // Instanciate death event
        instance.api.Event.OnEntityDeath += OnEntityDeath;

        // Populate configuration
        Configuration.PopulateBowConfiguration();

        Debug.Log("Level Bow initialized");
    }

    private Dictionary<string, int> GetSavedLevels()
    {
        byte[] dataBytes = instance.api.WorldManager.SaveGame.GetData("LevelUPData_Bow");
        string data = dataBytes == null ? "{}" : SerializerUtil.Deserialize<string>(dataBytes);
        return JsonSerializer.Deserialize<Dictionary<string, int>>(data);
    }

    private void SaveLevels(Dictionary<string, int> bowLevels)
    {
        instance.api.WorldManager.SaveGame.StoreData("LevelUPData_Bow", JsonSerializer.Serialize(bowLevels));
    }

    public void OnEntityDeath(Entity entity, DamageSource damageSource)
    {
        Debug.Log($"SOURCE ENTITY: {damageSource.SourceEntity?.GetType()}");
        Debug.Log($"SOURCE BLOCK: {damageSource.SourceBlock?.GetType()}");
        Debug.Log($"SOURCE: {damageSource.Source}");
        Debug.Log($"CAUSE: {damageSource.GetCauseEntity()?.GetType()}");
        
        if (damageSource.SourceEntity is not EntityProjectile && damageSource.GetCauseEntity() is not EntityPlayer) return;
        // Get entities
        EntityProjectile itemDamage = damageSource.SourceEntity as EntityProjectile;
        if(!itemDamage.GetName().Contains("Arrow")) return;
        EntityPlayer playerEntity = damageSource.GetCauseEntity() as EntityPlayer;

        // Get player instance
        IPlayer player = playerEntity.Api.World.PlayerByUid(playerEntity.PlayerUID);

        // Check if player is using a bow
        if (player.InventoryManager.ActiveTool != EnumTool.Bow) return;

        // Get all players levels
        Dictionary<string, int> bowLevels = GetSavedLevels();

        // Get the exp received
        int exp = entityExp.GetValueOrDefault(entity.GetName(), 0);

        // Get the actual player total exp
        int playerExp = bowLevels.GetValueOrDefault(playerEntity.GetName(), 0);

        Debug.Log($"{playerEntity.GetName()} killed: {entity.GetName()}, bow exp earned: {exp}, actual: {playerExp}");

        // Incrementing
        bowLevels[playerEntity.GetName()] = playerExp + exp;

        // Saving
        SaveLevels(bowLevels);
        // Updating
        Shared.Instance.UpdateLevelAndNotify(player, "Bow", playerExp + exp);
    }
}