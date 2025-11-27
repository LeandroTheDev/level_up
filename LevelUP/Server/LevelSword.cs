#pragma warning disable CA1822
using System.Collections.Generic;
using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.GameContent;

namespace LevelUP.Server;

class LevelSword
{
    public readonly Harmony patch = new("levelup_sword");
    public void Patch()
    {
        if (!Harmony.HasAnyPatches("levelup_sword"))
        {
            patch.PatchCategory("levelup_sword");
        }
    }
    public void Unpatch()
    {
        if (Harmony.HasAnyPatches("levelup_sword"))
        {
            patch.UnpatchCategory("levelup_sword");
        }
    }

    public void Init()
    {
        // Instanciate death event
        Instance.api.Event.OnEntityDeath += OnEntityDeath;
        OverwriteDamageInteractionEvents.OnPlayerMeleeDoDamageStart += HandleDamage;
        Configuration.RegisterNewLevel("Sword");
        Configuration.RegisterNewLevelTypeEXP("Sword", Configuration.SwordGetLevelByEXP);
        Configuration.RegisterNewEXPLevelType("Sword", Configuration.SwordGetExpByLevel);

        Debug.Log("Level Sword initialized");
    }

    public void InitClient()
    {
        Debug.Log("Level Sword initialized");
    }

    public void Dispose()
    {
        OverwriteDamageInteractionEvents.OnPlayerMeleeDoDamageStart -= HandleDamage;
    }

    private void HandleDamage(IPlayer player, DamageSource damageSource, ref float damage)
    {
        if (player.InventoryManager.ActiveTool == EnumTool.Sword)
        {
            damage *= Configuration.SwordGetDamageMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Sword"));
            Experience.IncreaseExperience(player, "Sword", "Hit");
        }
    }

    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulateSwordConfiguration(coreAPI);
        Configuration.RegisterNewMaxLevelByLevelTypeEXP("Sword", Configuration.swordMaxLevel);
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

        // Check if player is using a Sword
        if (player.InventoryManager.ActiveTool != EnumTool.Sword) return;

        // Get the exp received
        ulong exp = (ulong)Configuration.entityExpSword.GetValueOrDefault(entity.Code.ToString());

        // Get the actual player total exp
        ulong playerExp = Experience.GetExperience(player, "Sword");

        Debug.LogDebug($"{player.PlayerName} killed: {entity.Code}, sword exp earned: {exp}, actual: {playerExp}");

        // Incrementing
        Experience.IncreaseExperience(player, "Sword", exp);
    }

    [HarmonyPatchCategory("levelup_sword")]
    private class LevelSwordPatch
    { }
}