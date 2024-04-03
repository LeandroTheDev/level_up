using System.Collections.Generic;
using System.Text.Json;
using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Util;

namespace LevelUP.Server;

[HarmonyPatch]
class LevelHunter
{
    private Instance instance;
    Harmony overrider;

    readonly Dictionary<string, int> entityExp = [];

    public void Init(Instance _instance)
    {
        instance = _instance;
        // Instanciate death event
        instance.api.Event.OnEntityDeath += OnEntityDeath;

        //Dummy data
        entityExp["Dead drifter"] = 10;
        entityExp["Dead bear"] = 50;
        entityExp["Dead rooster"] = 1;

        Debug.Log("Level Hunter initialized");
    }

    private Dictionary<string, int> GetSavedLevels()
    {
        byte[] dataBytes = instance.api.WorldManager.SaveGame.GetData("LevelUPData_Hunter");
        string data = dataBytes == null ? "{}" : SerializerUtil.Deserialize<string>(dataBytes);
        return JsonSerializer.Deserialize<Dictionary<string, int>>(data);
    }

    private void SaveLevels(Dictionary<string, int> HunterLevels)
    {
        instance.api.WorldManager.SaveGame.StoreData("LevelUPData_Hunter", JsonSerializer.Serialize(HunterLevels));
    }

    public void OnEntityDeath(Entity entity, DamageSource damageSource)
    {
        // Entity kill is not from a player
        if (damageSource.SourceEntity == null) return;
        if (damageSource.SourceEntity is not EntityPlayer) return;

        // Get all players levels
        Dictionary<string, int> hunterLevels = GetSavedLevels();

        // Get the exp received
        int exp = entityExp.GetValueOrDefault(entity.GetName(), 0);

        // Get the actual player total exp
        int playerExp = hunterLevels.GetValueOrDefault(damageSource.SourceEntity.GetName(), 0);

        Debug.Log($"Entity died: {entity.GetName()}, by: {damageSource.SourceEntity.GetName()} exp earned: {exp}, actual: {playerExp}");

        // Incrementing
        hunterLevels[damageSource.SourceEntity.GetName()] = playerExp + exp;

        // Saving
        SaveLevels(hunterLevels);
        // Updating
        damageSource.SourceEntity.WatchedAttributes.SetInt("LevelUP_Hunter", playerExp + exp);
    }


    public void OverrideNativeFunctions()
    {
        if (!Harmony.HasAnyPatches("levelup"))
        {
            overrider = new Harmony("levelup");
            // Applies all harmony patches
            overrider.PatchAll();
            Debug.Log("Server Damage function has been overrited");
        }
    }
    public void OverrideDispose()
    {
        // Unpatch if world exist
        overrider?.UnpatchAll("levelup");
    }

    // Override Damage Interaction
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Entity), "ReceiveDamage")]
    public static void IncreaseDamage(Entity __instance, DamageSource damageSource, EntityAgent targetEntity)
    {
        
    }
}