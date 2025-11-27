#pragma warning disable CA1822
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.GameContent;

namespace LevelUP.Server;

class LevelSlingshot
{
    public readonly Harmony patch = new("levelup_slingshot");
    public void Patch()
    {
        if (!Harmony.HasAnyPatches("levelup_slingshot"))
        {
            patch.PatchCategory("levelup_slingshot");
        }
    }
    public void Unpatch()
    {
        if (Harmony.HasAnyPatches("levelup_slingshot"))
        {
            patch.UnpatchCategory("levelup_slingshot");
        }
    }

    public void Init()
    {
        // Instanciate death event
        Instance.api.Event.OnEntityDeath += OnEntityDeath;
        OverwriteDamageInteractionEvents.OnPlayerRangedDoDamageStart += HandleRangedDamage;
        Configuration.RegisterNewLevel("Slingshot");
        Configuration.RegisterNewLevelTypeEXP("Slingshot", Configuration.SlingshotGetLevelByEXP);
        Configuration.RegisterNewEXPLevelType("Slingshot", Configuration.SlingshotGetExpByLevel);

        Debug.Log("Level Slingshot initialized");
    }

    public void InitClient()
    {
        Debug.Log("Level Slingshot initialized");
    }

    private void HandleRangedDamage(IPlayer player, DamageSource damageSource, ref float damage)
    {
        if (damageSource.SourceEntity.GetName().Contains("thrownstone"))
        {
            damage *= Configuration.SlingshotGetDamageMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Slingshot"));
            Experience.IncreaseExperience(player, "Slingshot", "Hit");
        }
    }

    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulateSlingshotConfiguration(coreAPI);
        Configuration.RegisterNewMaxLevelByLevelTypeEXP("Slingshot", Configuration.slingshotMaxLevel);
    }

    public void OnEntityDeath(Entity entity, DamageSource damageSource)
    {
        // Error treatment
        if (damageSource == null) return;
        // Checking ranged weapon damage
        if (damageSource.SourceEntity is not EntityProjectile || damageSource.GetCauseEntity() is not EntityPlayer) return;

        // Get entities
        EntityProjectile itemDamage = damageSource.SourceEntity as EntityProjectile;

        // Check if projectile is not from any thrown stone
        if (!itemDamage.GetName().Contains("thrownstone")) return;
        EntityPlayer playerEntity = damageSource.GetCauseEntity() as EntityPlayer;

        // Get player instance
        IPlayer player = playerEntity.Player;

        // Get the exp received
        ulong exp = (ulong)Configuration.entityExpSlingshot.GetValueOrDefault(entity.Code.ToString(), 0);

        // Get the actual player total exp
        ulong playerExp = Experience.GetExperience(player, "Slingshot");

        Debug.LogDebug($"{player.PlayerName} killed: {entity.Code}, slingshot exp earned: {exp}, actual: {playerExp}");

        // Incrementing
        Experience.IncreaseExperience(player, "Slingshot", exp);
    }

    [HarmonyPatchCategory("levelup_slingshot")]
    private class LevelSlingshotPatch
    {
        // Overwrite Stone shot
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ItemStone), "OnHeldInteractStop")]
        internal static void OnHeldInteractSlingshotStoneStop(float secondsUsed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel)
        {
            if (!Configuration.enableLevelSlingshot) return;
            if (byEntity.Api.Side != EnumAppSide.Server) return;

            if (byEntity is EntityPlayer)
            {
                float chance = Configuration.SlingshotGetAimAccuracyByLevel(byEntity.WatchedAttributes.GetInt("LevelUP_Level_Slingshot", 0));

                // Integration
                chance = LevelSlingshotEvents.GetExternalSlingshotAiming((byEntity as EntityPlayer).Player, chance);

                // Setting new aim accuracy
                byEntity.Attributes.SetFloat("aimingAccuracy", chance);

                Debug.LogDebug($"Slingshot Accuracy: {chance}");
            }
        }

        // Overwrite Slingshot shot
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ItemSling), "OnHeldInteractStop")]
        internal static void OnHeldInteractSlingshotStop(float secondsUsed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel)
        {
            if (!Configuration.enableLevelSlingshot) return;
            if (byEntity.Api.Side != EnumAppSide.Server) return;

            if (byEntity is EntityPlayer)
            {
                float chance = Configuration.SlingshotGetAimAccuracyByLevel(byEntity.WatchedAttributes.GetInt("LevelUP_Level_Slingshot", 0));

                // Integration
                chance = LevelSlingshotEvents.GetExternalSlingshotAiming((byEntity as EntityPlayer).Player, chance);

                // Setting new aim accuracy
                byEntity.Attributes.SetFloat("aimingAccuracy", chance);

                Debug.LogDebug($"Slingshot Accuracy: {chance}");
            }
        }

        // Disable stone tickrate if damage already done
        [HarmonyPrefix]
        [HarmonyPatch(typeof(EntityThrownStone), "OnGameTick")]
        internal static bool EntityThrownStoneOnGameTick(EntityThrownStone __instance, float dt)
        {
            if (!Configuration.enableLevelSlingshot) return true;
            if (__instance.Api.Side != EnumAppSide.Server) return true;

            // Disable function if damage was done
            if (__instance.WatchedAttributes.GetBool("damageDone")) return false;
            else return true;
        }

        // Transpiler to remove Die() function from entity hit on rock throw
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(EntityThrownStone), "OnGameTick")]
        internal static List<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var list = new List<CodeInstruction>(instructions);
            if (!Configuration.enableLevelSlingshot) return list;

            var dieMethod = AccessTools.Method(typeof(Entity), "Die", [typeof(EnumDespawnReason), typeof(DamageSource)]);

            int lastCallIndex = -1;

            // Try to find last Die() function
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Calls(dieMethod))
                {
                    lastCallIndex = i;
                }
            }

            // Can't find the Die() function, probably broken by new update
            if (lastCallIndex == -1)
            {
                Debug.LogError("[EntityThrownStone] Oh no function not found, please report it");
                return list;
            }

            // Overwrite the die to your own die function
            var stubMethod = AccessTools.Method(typeof(DieStubs), "DieSkip");

            list[lastCallIndex].opcode = OpCodes.Call;
            list[lastCallIndex].operand = stubMethod;

            Debug.LogDebug("[EntityThrownStone] Die() function transpiled");

            return list;
        }

        // Transpiler class to fake the die function
        public static class DieStubs
        {
            public static void DieSkip(EntityThrownStone self, EnumDespawnReason reason, DamageSource source)
            {
                // Add a new boolean to detect if the rock damage was already done
                self.WatchedAttributes.SetBool("damageDone", true);

                if (self.FiredBy is EntityPlayer entityPlayer)
                {
                    if (!Configuration.SlingshotGetChanceToNotLoseRockByLevel(entityPlayer.WatchedAttributes.GetInt("LevelUP_Level_Slingshot")))
                    {
                        self.Die();
                    }
                }
                else
                {
                    self.Die();
                }
            }
        }

    }
}

public class LevelSlingshotEvents
{
    public delegate void PlayerFloatModifierHandler(IPlayer player, ref float number);
    public static event PlayerFloatModifierHandler OnSlingshotAimingRefresh;

    internal static float GetExternalSlingshotAiming(IPlayer player, float chance)
    {
        OnSlingshotAimingRefresh?.Invoke(player, ref chance);
        return chance;
    }
}