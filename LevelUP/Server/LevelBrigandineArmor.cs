#pragma warning disable CA1822
using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.GameContent;
namespace LevelUP.Server;

class LevelBrigandineArmor
{
    public readonly Harmony patch = new("levelup_brigandinearmor");
    public void Patch()
    {
        if (!Harmony.HasAnyPatches("levelup_brigandinearmor"))
        {
            patch.PatchCategory("levelup_brigandinearmor");
        }
    }
    public void Unpatch()
    {
        if (Harmony.HasAnyPatches("levelup_brigandinearmor"))
        {
            patch.UnpatchCategory("levelup_brigandinearmor");
        }
    }

    public void Init()
    {
        Configuration.RegisterNewLevel("BrigandineArmor");
        Configuration.RegisterNewLevelTypeEXP("BrigandineArmor", Configuration.BrigandineArmorGetLevelByEXP);
        Configuration.RegisterNewEXPLevelType("BrigandineArmor", Configuration.BrigandineArmorGetExpByLevel);
        OverwriteDamageInteractionEvents.OnPlayerArmorReceiveHandleStats += StatsUpdated;
        OverwriteDamageInteractionEvents.OnPlayerArmorReceiveDamageStat += DamageReceived;

        Debug.Log("Level Brigandine Armor initialized");
    }

    private void StatsUpdated(IPlayer player, List<ItemWearable> items)
    {
        float statusIncrease = Configuration.BrigandineArmorStatsIncreaseByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_BrigandineArmor"));
        foreach (ItemWearable armor in items)
        {
            if (!Configuration.expMultiplyHitBrigandineArmor.ContainsKey(armor.Code)) continue;

            if (armor.StatModifers == null)
            {
                Debug.LogDebug($"[BrigandineArmor] {player.PlayerName} {armor.Code} Armor System ignored because StatModifers is null");
                return;
            }

            Debug.LogDebug($"[BrigandineArmor] #### BEFORE STATS:");
            Debug.LogDebug($"[BrigandineArmor] healingeffectivness: {armor.StatModifers.healingeffectivness}");
            Debug.LogDebug($"[BrigandineArmor] hungerrate: {armor.StatModifers.hungerrate}");
            Debug.LogDebug($"[BrigandineArmor] rangedWeaponsAcc: {armor.StatModifers.rangedWeaponsAcc}");
            Debug.LogDebug($"[BrigandineArmor] rangedWeaponsSpeed: {armor.StatModifers.rangedWeaponsSpeed}");
            Debug.LogDebug($"[BrigandineArmor] walkSpeed: {armor.StatModifers.walkSpeed}");

            armor.StatModifers.healingeffectivness *= statusIncrease;
            armor.StatModifers.hungerrate *= statusIncrease;
            armor.StatModifers.rangedWeaponsAcc *= statusIncrease;
            armor.StatModifers.rangedWeaponsSpeed *= statusIncrease;
            armor.StatModifers.walkSpeed *= statusIncrease;

            Debug.LogDebug($"[BrigandineArmor] #### AFTER STATS:");
            Debug.LogDebug($"[BrigandineArmor] healingeffectivness: {armor.StatModifers.healingeffectivness}");
            Debug.LogDebug($"[BrigandineArmor] hungerrate: {armor.StatModifers.hungerrate}");
            Debug.LogDebug($"[BrigandineArmor] rangedWeaponsAcc: {armor.StatModifers.rangedWeaponsAcc}");
            Debug.LogDebug($"[BrigandineArmor] rangedWeaponsSpeed: {armor.StatModifers.rangedWeaponsSpeed}");
            Debug.LogDebug($"[BrigandineArmor] walkSpeed: {armor.StatModifers.walkSpeed}");

            LevelBrigandineArmorEvents.ExecuteItemHandledStats(armor, player);
        }
    }

    private void DamageReceived(IPlayer player, List<ItemWearable> items, ref float damage)
    {
        float statusIncrease = Configuration.BrigandineArmorStatsIncreaseByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_BrigandineArmor"));
        foreach (ItemWearable armor in items)
        {
            if (!Configuration.expMultiplyHitBrigandineArmor.ContainsKey(armor.Code)) continue;

            double multiply = Configuration.expMultiplyHitBrigandineArmor[armor.Code];
            ulong exp = (ulong)(Configuration.BrigandineArmorBaseEXPEarnedByDAMAGE(damage) * multiply);
            Experience.IncreaseExperience(player, "BrigandineArmor", exp);

            if (armor.ProtectionModifiers == null)
            {
                Debug.LogDebug($"[BrigandineArmor] {player.PlayerName} {armor.Code} Armor System ignored because ProtectionModifiers is null");
                return;
            }

            Debug.LogDebug($"[BrigandineArmor] {player.PlayerName} {armor.Code} Armor System Handling before R/F: {armor.ProtectionModifiers.RelativeProtection}");

            armor.ProtectionModifiers.RelativeProtection *= statusIncrease;
            armor.ProtectionModifiers.FlatDamageReduction *= statusIncrease;

            Debug.LogDebug($"[BrigandineArmor] {player.PlayerName} {armor.Code} Armor System Handling after R/F: {armor.ProtectionModifiers.RelativeProtection}/{armor.ProtectionModifiers.FlatDamageReduction}");

            LevelBrigandineArmorEvents.ExecuteItemHandledDamage(armor, player);
        }
    }

    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulateBrigandineArmorConfiguration(coreAPI);
        if (Configuration.enableLevelBrigandineArmor)
        {
            Configuration.RegisterNewMaxLevelByLevelTypeEXP("BrigandineArmor", Configuration.leatherArmorMaxLevel);
        }
    }

    [HarmonyPatchCategory("levelup_brigandinearmor")]
    private class BrigandineArmorPatch
    {
        // Client visual update
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ItemWearable), "GetHeldItemInfo")]
        [HarmonyPriority(Priority.VeryLow)]
        internal static void GetHeldItemInfo(ItemWearable __instance, ItemSlot inSlot, StringBuilder dsc, IWorldAccessor world, bool withDebugInfo)
        {
            if (world.Api is ICoreClientAPI api)
            {
                if (!Configuration.expMultiplyHitBrigandineArmor.ContainsKey(__instance.Code)) return;

                float statusIncrease = Configuration.BrigandineArmorStatsIncreaseByLevel(api.World.Player.Entity.WatchedAttributes.GetInt("LevelUP_Level_BrigandineArmor"));

                if (__instance.ProtectionModifiers != null)
                {
                    if (__instance.ProtectionModifiers.FlatDamageReduction != 0f)
                    {
                        Debug.LogDebug($"[BrigandineArmor] FlatDamageReduction Before: {__instance.ProtectionModifiers.FlatDamageReduction}");
                        __instance.ProtectionModifiers.FlatDamageReduction *= statusIncrease;
                        Debug.LogDebug($"[BrigandineArmor] FlatDamageReduction After: {__instance.ProtectionModifiers.FlatDamageReduction}");
                    }
                    if (__instance.ProtectionModifiers.RelativeProtection != 0f)
                    {
                        Debug.LogDebug($"[BrigandineArmor] RelativeProtection Before: {__instance.ProtectionModifiers.RelativeProtection}");
                        __instance.ProtectionModifiers.RelativeProtection *= statusIncrease;
                        Debug.LogDebug($"[BrigandineArmor] RelativeProtection After: {__instance.ProtectionModifiers.RelativeProtection}");
                    }
                }

                Debug.LogDebug($"[BrigandineArmor] #### BEFORE INFO:");
                Debug.LogDebug($"[BrigandineArmor] healingeffectivness: {__instance.StatModifers.healingeffectivness}");
                Debug.LogDebug($"[BrigandineArmor] hungerrate: {__instance.StatModifers.hungerrate}");
                Debug.LogDebug($"[BrigandineArmor] rangedWeaponsAcc: {__instance.StatModifers.rangedWeaponsAcc}");
                Debug.LogDebug($"[BrigandineArmor] rangedWeaponsSpeed: {__instance.StatModifers.rangedWeaponsSpeed}");
                Debug.LogDebug($"[BrigandineArmor] walkSpeed: {__instance.StatModifers.walkSpeed}");

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

                Debug.LogDebug($"[BrigandineArmor] ### AFTER INFO:");
                Debug.LogDebug($"[BrigandineArmor] healingeffectivness: {__instance.StatModifers.healingeffectivness}");
                Debug.LogDebug($"[BrigandineArmor] hungerrate: {__instance.StatModifers.hungerrate}");
                Debug.LogDebug($"[BrigandineArmor] rangedWeaponsAcc: {__instance.StatModifers.rangedWeaponsAcc}");
                Debug.LogDebug($"[BrigandineArmor] rangedWeaponsSpeed: {__instance.StatModifers.rangedWeaponsSpeed}");
                Debug.LogDebug($"[BrigandineArmor] walkSpeed: {__instance.StatModifers.walkSpeed}");

                LevelBrigandineArmorEvents.ExecuteItemInfoUpdated(__instance, api.World.Player);
            }
        }
    }
}

public class LevelBrigandineArmorEvents
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