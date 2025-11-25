#pragma warning disable CA1822
using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.GameContent;
namespace LevelUP.Server;

class LevelScaleArmor
{
    public readonly Harmony patch = new("levelup_scalearmor");
    public void Patch()
    {
        if (!Harmony.HasAnyPatches("levelup_scalearmor"))
        {
            patch.PatchCategory("levelup_scalearmor");
        }
    }
    public void Unpatch()
    {
        if (Harmony.HasAnyPatches("levelup_scalearmor"))
        {
            patch.UnpatchCategory("levelup_scalearmor");
        }
    }

    public void Init()
    {
        Configuration.RegisterNewLevel("ScaleArmor");
        Configuration.RegisterNewLevelTypeEXP("ScaleArmor", Configuration.ScaleArmorGetLevelByEXP);
        Configuration.RegisterNewEXPLevelType("ScaleArmor", Configuration.ScaleArmorGetExpByLevel);
        OverwriteDamageInteractionEvents.OnPlayerArmorReceiveHandleStats += StatsUpdated;
        OverwriteDamageInteractionEvents.OnPlayerArmorReceiveDamageStat += DamageReceived;

        Debug.Log("Level Scale Armor initialized");
    }

    private void StatsUpdated(IPlayer player, List<ItemWearable> items)
    {
        float statusIncrease = Configuration.ScaleArmorStatsIncreaseByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_ScaleArmor"));
        foreach (ItemWearable armor in items)
        {
            if (!Configuration.expMultiplyHitScaleArmor.ContainsKey(armor.Code)) continue;

            if (armor.StatModifers == null)
            {
                Debug.LogDebug($"[ScaleArmor] {player.PlayerName} {armor.Code} Armor System ignored because StatModifers is null");
                return;
            }

            Debug.LogDebug($"[ScaleArmor] #### BEFORE STATS:");
            Debug.LogDebug($"[ScaleArmor] healingeffectivness: {armor.StatModifers.healingeffectivness}");
            Debug.LogDebug($"[ScaleArmor] hungerrate: {armor.StatModifers.hungerrate}");
            Debug.LogDebug($"[ScaleArmor] rangedWeaponsAcc: {armor.StatModifers.rangedWeaponsAcc}");
            Debug.LogDebug($"[ScaleArmor] rangedWeaponsSpeed: {armor.StatModifers.rangedWeaponsSpeed}");
            Debug.LogDebug($"[ScaleArmor] walkSpeed: {armor.StatModifers.walkSpeed}");

            armor.StatModifers.healingeffectivness *= statusIncrease;
            armor.StatModifers.hungerrate *= statusIncrease;
            armor.StatModifers.rangedWeaponsAcc *= statusIncrease;
            armor.StatModifers.rangedWeaponsSpeed *= statusIncrease;
            armor.StatModifers.walkSpeed *= statusIncrease;

            Debug.LogDebug($"[ScaleArmor] #### AFTER STATS:");
            Debug.LogDebug($"[ScaleArmor] healingeffectivness: {armor.StatModifers.healingeffectivness}");
            Debug.LogDebug($"[ScaleArmor] hungerrate: {armor.StatModifers.hungerrate}");
            Debug.LogDebug($"[ScaleArmor] rangedWeaponsAcc: {armor.StatModifers.rangedWeaponsAcc}");
            Debug.LogDebug($"[ScaleArmor] rangedWeaponsSpeed: {armor.StatModifers.rangedWeaponsSpeed}");
            Debug.LogDebug($"[ScaleArmor] walkSpeed: {armor.StatModifers.walkSpeed}");

            LevelScaleArmorEvents.ExecuteItemHandledStats(armor, player);
        }
    }

    private void DamageReceived(IPlayer player, List<ItemWearable> items, ref float damage)
    {
        float statusIncrease = Configuration.ScaleArmorStatsIncreaseByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_ScaleArmor"));
        foreach (ItemWearable armor in items)
        {
            if (!Configuration.expMultiplyHitScaleArmor.ContainsKey(armor.Code)) continue;

            double multiply = Configuration.expMultiplyHitScaleArmor[armor.Code];
            ulong exp = (ulong)(Configuration.ScaleArmorBaseEXPEarnedByDAMAGE(damage) * multiply);
            Experience.IncreaseExperience(player, "ScaleArmor", exp);

            if (armor.ProtectionModifiers == null)
            {
                Debug.LogDebug($"[ScaleArmor] {player.PlayerName} {armor.Code} Armor System ignored because ProtectionModifiers is null");
                return;
            }

            Debug.LogDebug($"[ScaleArmor] {player.PlayerName} {armor.Code} Armor System Handling before R/F: {armor.ProtectionModifiers.RelativeProtection}");

            armor.ProtectionModifiers.RelativeProtection *= statusIncrease;
            armor.ProtectionModifiers.FlatDamageReduction *= statusIncrease;

            Debug.LogDebug($"[ScaleArmor] {player.PlayerName} {armor.Code} Armor System Handling after R/F: {armor.ProtectionModifiers.RelativeProtection}/{armor.ProtectionModifiers.FlatDamageReduction}");

            LevelScaleArmorEvents.ExecuteItemHandledDamage(armor, player);
        }
    }

    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulateScaleArmorConfiguration(coreAPI);
        if (Configuration.enableLevelScaleArmor)
        {
            Configuration.RegisterNewMaxLevelByLevelTypeEXP("ScaleArmor", Configuration.leatherArmorMaxLevel);
        }
    }

    [HarmonyPatchCategory("levelup_scalearmor")]
    private class ScaleArmorPatch
    {
        // Client visual update
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ItemWearable), "GetHeldItemInfo")]
        [HarmonyPriority(Priority.VeryLow)]
        internal static void GetHeldItemInfo(ItemWearable __instance, ItemSlot inSlot, StringBuilder dsc, IWorldAccessor world, bool withDebugInfo)
        {
            if (world.Api is ICoreClientAPI api)
            {
                if (!Configuration.expMultiplyHitScaleArmor.ContainsKey(__instance.Code)) return;

                float statusIncrease = Configuration.ScaleArmorStatsIncreaseByLevel(api.World.Player.Entity.WatchedAttributes.GetInt("LevelUP_Level_ScaleArmor"));

                if (__instance.ProtectionModifiers != null)
                {
                    if (__instance.ProtectionModifiers.FlatDamageReduction != 0f)
                    {
                        Debug.LogDebug($"[ScaleArmor] FlatDamageReduction Before: {__instance.ProtectionModifiers.FlatDamageReduction}");
                        __instance.ProtectionModifiers.FlatDamageReduction *= statusIncrease;
                        Debug.LogDebug($"[ScaleArmor] FlatDamageReduction After: {__instance.ProtectionModifiers.FlatDamageReduction}");
                    }
                    if (__instance.ProtectionModifiers.RelativeProtection != 0f)
                    {
                        Debug.LogDebug($"[ScaleArmor] RelativeProtection Before: {__instance.ProtectionModifiers.RelativeProtection}");
                        __instance.ProtectionModifiers.RelativeProtection *= statusIncrease;
                        Debug.LogDebug($"[ScaleArmor] RelativeProtection After: {__instance.ProtectionModifiers.RelativeProtection}");
                    }
                }

                Debug.LogDebug($"[ScaleArmor] #### BEFORE INFO:");
                Debug.LogDebug($"[ScaleArmor] healingeffectivness: {__instance.StatModifers.healingeffectivness}");
                Debug.LogDebug($"[ScaleArmor] hungerrate: {__instance.StatModifers.hungerrate}");
                Debug.LogDebug($"[ScaleArmor] rangedWeaponsAcc: {__instance.StatModifers.rangedWeaponsAcc}");
                Debug.LogDebug($"[ScaleArmor] rangedWeaponsSpeed: {__instance.StatModifers.rangedWeaponsSpeed}");
                Debug.LogDebug($"[ScaleArmor] walkSpeed: {__instance.StatModifers.walkSpeed}");

                if (__instance.StatModifers != null)
                {
                    if (__instance.StatModifers.healingeffectivness != 0f)
                    {
                        if (__instance.StatModifers.healingeffectivness < 0f)
                        {
                            float positiveValue = Math.Abs(__instance.StatModifers.healingeffectivness);
                            __instance.StatModifers.healingeffectivness += positiveValue * statusIncrease;
                        }
                        else
                        {
                            __instance.StatModifers.healingeffectivness *= statusIncrease;
                        }
                    }
                    if (__instance.StatModifers.hungerrate != 0f)
                    {
                        if (__instance.StatModifers.hungerrate < 0f)
                        {
                            float positiveValue = Math.Abs(__instance.StatModifers.hungerrate);
                            __instance.StatModifers.hungerrate += positiveValue * statusIncrease;
                        }
                        else
                        {
                            __instance.StatModifers.hungerrate *= statusIncrease;
                        }
                    }
                    if (__instance.StatModifers.rangedWeaponsAcc != 0f)
                    {
                        if (__instance.StatModifers.rangedWeaponsAcc < 0f)
                        {
                            float positiveValue = Math.Abs(__instance.StatModifers.rangedWeaponsAcc);
                            __instance.StatModifers.rangedWeaponsAcc += positiveValue * statusIncrease;
                        }
                        else
                        {
                            __instance.StatModifers.rangedWeaponsAcc *= statusIncrease;
                        }
                    }
                    if (__instance.StatModifers.rangedWeaponsSpeed != 0f)
                    {
                        if (__instance.StatModifers.rangedWeaponsSpeed < 0f)
                        {
                            float positiveValue = Math.Abs(__instance.StatModifers.rangedWeaponsSpeed);
                            __instance.StatModifers.rangedWeaponsSpeed += positiveValue * statusIncrease;
                        }
                        else
                        {
                            __instance.StatModifers.rangedWeaponsSpeed *= statusIncrease;
                        }
                    }
                    if (__instance.StatModifers.walkSpeed != 0f)
                    {
                        if (__instance.StatModifers.walkSpeed < 0f)
                        {
                            float positiveValue = Math.Abs(__instance.StatModifers.walkSpeed);
                            __instance.StatModifers.walkSpeed += positiveValue * statusIncrease;
                        }
                        else
                        {
                            __instance.StatModifers.walkSpeed *= statusIncrease;
                        }
                    }
                }

                Debug.LogDebug($"[ScaleArmor] ### AFTER INFO:");
                Debug.LogDebug($"[ScaleArmor] healingeffectivness: {__instance.StatModifers.healingeffectivness}");
                Debug.LogDebug($"[ScaleArmor] hungerrate: {__instance.StatModifers.hungerrate}");
                Debug.LogDebug($"[ScaleArmor] rangedWeaponsAcc: {__instance.StatModifers.rangedWeaponsAcc}");
                Debug.LogDebug($"[ScaleArmor] rangedWeaponsSpeed: {__instance.StatModifers.rangedWeaponsSpeed}");
                Debug.LogDebug($"[ScaleArmor] walkSpeed: {__instance.StatModifers.walkSpeed}");

                LevelScaleArmorEvents.ExecuteItemInfoUpdated(__instance, api.World.Player);
            }
        }
    }
}

public class LevelScaleArmorEvents
{
    public delegate void PlayerItemWearableHandler(ItemWearable item, IPlayer player);

    public static event PlayerItemWearableHandler OnItemInfoUpdated;
    public static event PlayerItemWearableHandler OnItemHandledDamage;
    public static event PlayerItemWearableHandler OnItemHandledStats;

    internal static void ExecuteItemInfoUpdated(ItemWearable item, IPlayer player)
    {
        OnItemInfoUpdated?.Invoke(item, player);
    }

    internal static void ExecuteItemHandledDamage(ItemWearable item, IPlayer player)
    {
        OnItemHandledDamage?.Invoke(item, player);
    }

    internal static void ExecuteItemHandledStats(ItemWearable item, IPlayer player)
    {
        OnItemHandledStats?.Invoke(item, player);
    }
}