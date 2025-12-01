#pragma warning disable CA1822
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using LevelUP.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.GameContent;

namespace LevelUP.Server;

class LevelHand
{
    public readonly Harmony patch = new("levelup_hand");
    public void Patch()
    {
        if (!Harmony.HasAnyPatches("levelup_hand"))
        {
            patch.PatchCategory("levelup_hand");
        }
    }
    public void Unpatch()
    {
        if (Harmony.HasAnyPatches("levelup_hand"))
        {
            patch.UnpatchCategory("levelup_hand");
        }
    }

    public void Init()
    {
        // Instanciate death event
        Instance.api.Event.OnEntityDeath += OnEntityDeath;
        OverwriteDamageInteractionEvents.OnPlayerMeleeDoDamageStart += HandleDamage;
        Configuration.RegisterNewLevel("Hand");
        Configuration.RegisterNewLevelTypeEXP("Hand", Configuration.HandGetLevelByEXP);
        Configuration.RegisterNewEXPLevelType("Hand", Configuration.HandGetExpByLevel);

        Debug.Log("Level Hand initialized");
    }

    public void InitClient()
    {
        StatusViewEvents.OnStatusRequested += StatusViewRequested;

        Debug.Log("Level Hand initialized");
    }

    public void Dispose()
    {
        OverwriteDamageInteractionEvents.OnPlayerMeleeDoDamageStart -= HandleDamage;
        StatusViewEvents.OnStatusRequested -= StatusViewRequested;
    }

    private void StatusViewRequested(IPlayer player, ref StringBuilder stringBuilder, string levelType)
    {
        if (levelType != "Hand") return;

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_damage",
                Utils.GetPorcentageFromFloatsStart1(Configuration.HandGetDamageMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Hand")))
            )
        );
    }

    private void HandleDamage(IPlayer player, DamageSource damageSource, ref float damage)
    {
        if (player.InventoryManager.ActiveHotbarSlot != null)
        {
            // Check if the active slot is empty
            if (player.InventoryManager.ActiveHotbarSlot.Itemstack == null)
            {
                damage *= Configuration.HandGetDamageMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Hand"));
                Experience.IncreaseExperience(player, "Hand", "Hit");
            }
        }
    }

    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulateHandConfiguration(coreAPI);
        Configuration.RegisterNewMaxLevelByLevelTypeEXP("Hand", Configuration.handMaxLevel);
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

        ulong exp = (ulong)Configuration.entityExpSword.GetValueOrDefault(entity.Code.ToString());
        Experience.IncreaseExperience(player, "Hand", exp);
    }

    [HarmonyPatchCategory("levelup_hand")]
    private class LevelHandPatch
    { }
}