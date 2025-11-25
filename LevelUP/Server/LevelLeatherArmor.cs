#pragma warning disable CA1822
using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.GameContent;
namespace LevelUP.Server;

class LevelLeatherArmor
{
    public readonly Harmony patch = new("levelup_leatherarmor");
    public void Patch()
    {
        if (!Harmony.HasAnyPatches("levelup_leatherarmor"))
        {
            patch.PatchCategory("levelup_leatherarmor");
        }
    }
    public void Unpatch()
    {
        if (Harmony.HasAnyPatches("levelup_leatherarmor"))
        {
            patch.UnpatchCategory("levelup_leatherarmor");
        }
    }

    public void Init()
    {
        Configuration.RegisterNewLevel("LeatherArmor");
        Configuration.RegisterNewLevelTypeEXP("LeatherArmor", Configuration.LeatherArmorGetLevelByEXP);
        Configuration.RegisterNewEXPLevelType("LeatherArmor", Configuration.LeatherArmorGetExpByLevel);
        OverwriteDamageInteractionEvents.OnPlayerArmorReceiveHandleStats += StatsUpdated;
        OverwriteDamageInteractionEvents.OnPlayerArmorReceiveDamageStat += DamageReceived;

        Debug.Log("Level Leather Armor initialized");
    }

    private void StatsUpdated(IPlayer player, List<ItemWearable> items)
    {
        float statusIncrease = Configuration.LeatherArmorStatsIncreaseByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_LeatherArmor"));
        foreach (ItemWearable armor in items)
        {
            if (!Configuration.expMultiplyHitLeatherArmor.ContainsKey(armor.Code)) continue;

            if (armor.StatModifers == null)
            {
                Debug.LogDebug($"[LeatherArmor] {player.PlayerName} {armor.Code} Armor System ignored because StatModifers is null");
                return;
            }

            Debug.LogDebug($"[LeatherArmor] #### BEFORE STATS:");
            Debug.LogDebug($"[LeatherArmor] healingeffectivness: {armor.StatModifers.healingeffectivness}");
            Debug.LogDebug($"[LeatherArmor] hungerrate: {armor.StatModifers.hungerrate}");
            Debug.LogDebug($"[LeatherArmor] rangedWeaponsAcc: {armor.StatModifers.rangedWeaponsAcc}");
            Debug.LogDebug($"[LeatherArmor] rangedWeaponsSpeed: {armor.StatModifers.rangedWeaponsSpeed}");
            Debug.LogDebug($"[LeatherArmor] walkSpeed: {armor.StatModifers.walkSpeed}");

            armor.StatModifers.healingeffectivness *= statusIncrease;
            armor.StatModifers.hungerrate *= statusIncrease;
            armor.StatModifers.rangedWeaponsAcc *= statusIncrease;
            armor.StatModifers.rangedWeaponsSpeed *= statusIncrease;
            armor.StatModifers.walkSpeed *= statusIncrease;

            Debug.LogDebug($"[LeatherArmor] #### AFTER STATS:");
            Debug.LogDebug($"[LeatherArmor] healingeffectivness: {armor.StatModifers.healingeffectivness}");
            Debug.LogDebug($"[LeatherArmor] hungerrate: {armor.StatModifers.hungerrate}");
            Debug.LogDebug($"[LeatherArmor] rangedWeaponsAcc: {armor.StatModifers.rangedWeaponsAcc}");
            Debug.LogDebug($"[LeatherArmor] rangedWeaponsSpeed: {armor.StatModifers.rangedWeaponsSpeed}");
            Debug.LogDebug($"[LeatherArmor] walkSpeed: {armor.StatModifers.walkSpeed}");

            LevelLeatherArmorEvents.ExecuteItemHandledStats(armor, player);
        }
    }

    private void DamageReceived(IPlayer player, List<ItemWearable> items, ref float damage)
    {
        float statusIncrease = Configuration.LeatherArmorStatsIncreaseByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_LeatherArmor"));
        foreach (ItemWearable armor in items)
        {
            if (!Configuration.expMultiplyHitLeatherArmor.ContainsKey(armor.Code)) continue;

            double multiply = Configuration.expMultiplyHitLeatherArmor[armor.Code];
            ulong exp = (ulong)(Configuration.LeatherArmorBaseEXPEarnedByDAMAGE(damage) * multiply);
            Experience.IncreaseExperience(player, "LeatherArmor", exp);

            if (armor.ProtectionModifiers == null)
            {
                Debug.LogDebug($"[LeatherArmor] {player.PlayerName} {armor.Code} Armor System ignored because ProtectionModifiers is null");
                return;
            }

            Debug.LogDebug($"[LeatherArmor] {player.PlayerName} {armor.Code} Armor System Handling before R/F: {armor.ProtectionModifiers.RelativeProtection}");

            armor.ProtectionModifiers.RelativeProtection *= statusIncrease;
            armor.ProtectionModifiers.FlatDamageReduction *= statusIncrease;

            Debug.LogDebug($"[LeatherArmor] {player.PlayerName} {armor.Code} Armor System Handling after R/F: {armor.ProtectionModifiers.RelativeProtection}/{armor.ProtectionModifiers.FlatDamageReduction}");

            LevelLeatherArmorEvents.ExecuteItemHandledDamage(armor, player);
        }
    }

    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulateLeatherArmorConfiguration(coreAPI);
        if (Configuration.enableLevelLeatherArmor)
        {
            Configuration.RegisterNewMaxLevelByLevelTypeEXP("LeatherArmor", Configuration.leatherArmorMaxLevel);
        }
    }

    [HarmonyPatchCategory("levelup_leatherarmor")]
    private class LeatherArmorPatch
    {
        // Client visual update
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ItemWearable), "GetHeldItemInfo")]
        [HarmonyPriority(Priority.VeryLow)]
        internal static void GetHeldItemInfo(ItemWearable __instance, ItemSlot inSlot, StringBuilder dsc, IWorldAccessor world, bool withDebugInfo)
        {
            if (world.Api is ICoreClientAPI api)
            {
                if (!Configuration.expMultiplyHitLeatherArmor.ContainsKey(__instance.Code)) return;

                float statusIncrease = Configuration.LeatherArmorStatsIncreaseByLevel(api.World.Player.Entity.WatchedAttributes.GetInt("LevelUP_Level_LeatherArmor"));

                if (__instance.ProtectionModifiers != null)
                {
                    if (__instance.ProtectionModifiers.FlatDamageReduction != 0f)
                    {
                        Debug.LogDebug($"[LeatherArmor] FlatDamageReduction Before: {__instance.ProtectionModifiers.FlatDamageReduction}");
                        __instance.ProtectionModifiers.FlatDamageReduction *= statusIncrease;
                        Debug.LogDebug($"[LeatherArmor] FlatDamageReduction After: {__instance.ProtectionModifiers.FlatDamageReduction}");
                    }
                    if (__instance.ProtectionModifiers.RelativeProtection != 0f)
                    {
                        Debug.LogDebug($"[LeatherArmor] RelativeProtection Before: {__instance.ProtectionModifiers.RelativeProtection}");
                        __instance.ProtectionModifiers.RelativeProtection *= statusIncrease;
                        Debug.LogDebug($"[LeatherArmor] RelativeProtection After: {__instance.ProtectionModifiers.RelativeProtection}");
                    }
                }

                Debug.LogDebug($"[LeatherArmor] #### BEFORE INFO:");
                Debug.LogDebug($"[LeatherArmor] healingeffectivness: {__instance.StatModifers.healingeffectivness}");
                Debug.LogDebug($"[LeatherArmor] hungerrate: {__instance.StatModifers.hungerrate}");
                Debug.LogDebug($"[LeatherArmor] rangedWeaponsAcc: {__instance.StatModifers.rangedWeaponsAcc}");
                Debug.LogDebug($"[LeatherArmor] rangedWeaponsSpeed: {__instance.StatModifers.rangedWeaponsSpeed}");
                Debug.LogDebug($"[LeatherArmor] walkSpeed: {__instance.StatModifers.walkSpeed}");

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

                Debug.LogDebug($"[LeatherArmor] ### AFTER INFO:");
                Debug.LogDebug($"[LeatherArmor] healingeffectivness: {__instance.StatModifers.healingeffectivness}");
                Debug.LogDebug($"[LeatherArmor] hungerrate: {__instance.StatModifers.hungerrate}");
                Debug.LogDebug($"[LeatherArmor] rangedWeaponsAcc: {__instance.StatModifers.rangedWeaponsAcc}");
                Debug.LogDebug($"[LeatherArmor] rangedWeaponsSpeed: {__instance.StatModifers.rangedWeaponsSpeed}");
                Debug.LogDebug($"[LeatherArmor] walkSpeed: {__instance.StatModifers.walkSpeed}");

                LevelLeatherArmorEvents.ExecuteItemInfoUpdated(__instance, api.World.Player);
            }
        }
    }
}

public class LevelLeatherArmorEvents
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