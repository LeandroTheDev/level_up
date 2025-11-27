#pragma warning disable CA1822
using System;
using System.Collections.Generic;
using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.GameContent;

namespace LevelUP.Server;

class LevelBow
{
    public readonly Harmony patch = new("levelup_bow");
    public void Patch()
    {
        if (!Harmony.HasAnyPatches("levelup_bow"))
        {
            patch.PatchCategory("levelup_bow");
        }
    }
    public void Unpatch()
    {
        if (Harmony.HasAnyPatches("levelup_bow"))
        {
            patch.UnpatchCategory("levelup_bow");
        }
    }

    public void Init()
    {
        // Instanciate death event
        Instance.api.Event.OnEntityDeath += OnEntityDeath;
        OverwriteDamageInteractionEvents.OnPlayerRangedDoDamageStart += HandleRangedDamage;
        Configuration.RegisterNewLevel("Bow");
        Configuration.RegisterNewLevelTypeEXP("Bow", Configuration.BowGetLevelByEXP);
        Configuration.RegisterNewEXPLevelType("Bow", Configuration.BowGetExpByLevel);

        Debug.Log("Level Bow initialized");
    }

    public void InitClient()
    {
        Debug.Log("Level Bow initialized");
    }

    private void HandleRangedDamage(IPlayer player, DamageSource damageSource, ref float damage)
    {
        if (damageSource.SourceEntity.GetName().Contains("arrow"))
        {
            damage *= Configuration.BowGetDamageMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Bow"));
            Experience.IncreaseExperience(player, "Bow", "Hit");
        }
    }

    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulateBowConfiguration(coreAPI);
        Configuration.RegisterNewMaxLevelByLevelTypeEXP("Bow", Configuration.bowMaxLevel);
    }

    public void OnEntityDeath(Entity entity, DamageSource damageSource)
    {
        // Error treatment
        if (damageSource == null) return;
        // Checking ranged weapon damage
        if (damageSource.SourceEntity is not EntityProjectile || damageSource.GetCauseEntity() is not EntityPlayer) return;

        // Get entities
        EntityProjectile itemDamage = damageSource.SourceEntity as EntityProjectile;
        // Check if projectile is not from any arrow
        if (!itemDamage.GetName().Contains("arrow")) return;
        EntityPlayer playerEntity = damageSource.GetCauseEntity() as EntityPlayer;

        // Get player instance
        IPlayer player = playerEntity.Player;

        // Get the exp received
        ulong exp = (ulong)Configuration.entityExpBow.GetValueOrDefault(entity.Code.ToString(), 0);

        // Get the actual player total exp
        ulong playerExp = Experience.GetExperience(player, "Bow");

        Debug.LogDebug($"{player.PlayerName} killed: {entity.Code}, bow exp earned: {exp}, actual: {playerExp}");

        // Incrementing
        Experience.IncreaseExperience(player, "Bow", exp);
    }

    [HarmonyPatchCategory("levelup_bow")]
    private class LevelBowPatch
    {
        // Overwrite Projectile impact
        [HarmonyPrefix]
        [HarmonyPatch(typeof(EntityProjectile), "impactOnEntity")]
        internal static void ImpactOnEntity(EntityProjectile __instance, Entity entity)
        {
            if (!Configuration.enableLevelBow) return;
            if (__instance.World.Side != EnumAppSide.Server) return;

            // Check if is a arrow
            if (__instance.Code.ToString().Contains("arrow"))
            {
                // Check if arrow is shotted by a player
                if (__instance.FiredBy is EntityPlayer)
                {
                    EntityPlayer playerEntity = __instance.FiredBy as EntityPlayer;

                    float chance = Configuration.BowGetChanceToNotLoseArrowByLevel(playerEntity.WatchedAttributes.GetInt("LevelUP_Level_Bow"));

                    // Integration
                    chance = LevelBowEvents.GetExternalBowDropChance(playerEntity.Player, chance);

                    // Change the change based on level
                    __instance.DropOnImpactChance = chance;
                }
            }
        }

        // Overwrite Bow shot start
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ItemBow), "OnHeldInteractStop")]
        internal static void OnHeldInteractBowStop(float secondsUsed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel)
        {
            if (!Configuration.enableLevelBow) return;
            if (byEntity.Api.Side != EnumAppSide.Server) return;

            if (byEntity is EntityPlayer)
            {
                float chance = Configuration.BowGetAimAccuracyByLevel(byEntity.WatchedAttributes.GetInt("LevelUP_Level_Bow", 0));

                // Integration
                chance = LevelBowEvents.GetExternalBowAiming((byEntity as EntityPlayer).Player, chance);

                // Setting new aim accuracy
                byEntity.Attributes.SetFloat("aimingAccuracy", chance);

                Debug.LogDebug($"Bow Accuracy: {chance}");
            }
        }
    }
}

public class LevelBowEvents
{
    public delegate void PlayerFloatModifierHandler(IPlayer player, ref float number);

    public static event PlayerFloatModifierHandler OnBowDropChanceRefresh;
    public static event PlayerFloatModifierHandler OnBowAimingRefresh;

    internal static float GetExternalBowDropChance(IPlayer player, float chance)
    {
        OnBowDropChanceRefresh?.Invoke(player, ref chance);
        return chance;
    }

    internal static float GetExternalBowAiming(IPlayer player, float chance)
    {
        OnBowAimingRefresh?.Invoke(player, ref chance);
        return chance;
    }
}