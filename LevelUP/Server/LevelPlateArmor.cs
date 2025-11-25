#pragma warning disable CA1822
using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.GameContent;
namespace LevelUP.Server;

class LevelPlateArmor
{
    public readonly Harmony patch = new("levelup_platearmor");
    public void Patch()
    {
        if (!Harmony.HasAnyPatches("levelup_platearmor"))
        {
            patch.PatchCategory("levelup_platearmor");
        }
    }
    public void Unpatch()
    {
        if (Harmony.HasAnyPatches("levelup_platearmor"))
        {
            patch.UnpatchCategory("levelup_platearmor");
        }
    }

    public void Init()
    {
        Configuration.RegisterNewLevel("PlateArmor");
        Configuration.RegisterNewLevelTypeEXP("PlateArmor", Configuration.PlateArmorGetLevelByEXP);
        Configuration.RegisterNewEXPLevelType("PlateArmor", Configuration.PlateArmorGetExpByLevel);
        OverwriteDamageInteractionEvents.OnPlayerArmorReceiveHandleStats += StatsUpdated;
        OverwriteDamageInteractionEvents.OnPlayerArmorReceiveDamageStat += DamageReceived;

        Debug.Log("Level Plate Armor initialized");
    }

    private void StatsUpdated(IPlayer player, List<ItemWearable> items)
    {
        float statusIncrease = Configuration.PlateArmorStatsIncreaseByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_PlateArmor"));
        foreach (ItemWearable armor in items)
        {
            if (!Configuration.expMultiplyHitPlateArmor.ContainsKey(armor.Code)) continue;

            if (armor.StatModifers == null)
            {
                Debug.LogDebug($"[PlateArmor] {player.PlayerName} {armor.Code} Armor System ignored because StatModifers is null");
                return;
            }

            Debug.LogDebug($"[PlateArmor] #### BEFORE STATS:");
            Debug.LogDebug($"[PlateArmor] healingeffectivness: {armor.StatModifers.healingeffectivness}");
            Debug.LogDebug($"[PlateArmor] hungerrate: {armor.StatModifers.hungerrate}");
            Debug.LogDebug($"[PlateArmor] rangedWeaponsAcc: {armor.StatModifers.rangedWeaponsAcc}");
            Debug.LogDebug($"[PlateArmor] rangedWeaponsSpeed: {armor.StatModifers.rangedWeaponsSpeed}");
            Debug.LogDebug($"[PlateArmor] walkSpeed: {armor.StatModifers.walkSpeed}");

            armor.StatModifers.healingeffectivness *= statusIncrease;
            armor.StatModifers.hungerrate *= statusIncrease;
            armor.StatModifers.rangedWeaponsAcc *= statusIncrease;
            armor.StatModifers.rangedWeaponsSpeed *= statusIncrease;
            armor.StatModifers.walkSpeed *= statusIncrease;

            Debug.LogDebug($"[PlateArmor] #### AFTER STATS:");
            Debug.LogDebug($"[PlateArmor] healingeffectivness: {armor.StatModifers.healingeffectivness}");
            Debug.LogDebug($"[PlateArmor] hungerrate: {armor.StatModifers.hungerrate}");
            Debug.LogDebug($"[PlateArmor] rangedWeaponsAcc: {armor.StatModifers.rangedWeaponsAcc}");
            Debug.LogDebug($"[PlateArmor] rangedWeaponsSpeed: {armor.StatModifers.rangedWeaponsSpeed}");
            Debug.LogDebug($"[PlateArmor] walkSpeed: {armor.StatModifers.walkSpeed}");

            LevelPlateArmorEvents.ExecuteItemHandledStats(armor, player);
        }
    }

    private void DamageReceived(IPlayer player, List<ItemWearable> items, ref float damage)
    {
        float statusIncrease = Configuration.PlateArmorStatsIncreaseByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_PlateArmor"));
        foreach (ItemWearable armor in items)
        {
            if (!Configuration.expMultiplyHitPlateArmor.ContainsKey(armor.Code)) continue;

            double multiply = Configuration.expMultiplyHitPlateArmor[armor.Code];
            ulong exp = (ulong)(Configuration.PlateArmorBaseEXPEarnedByDAMAGE(damage) * multiply);
            Experience.IncreaseExperience(player, "PlateArmor", exp);

            if (armor.ProtectionModifiers == null)
            {
                Debug.LogDebug($"[PlateArmor] {player.PlayerName} {armor.Code} Armor System ignored because ProtectionModifiers is null");
                return;
            }

            Debug.LogDebug($"[PlateArmor] {player.PlayerName} {armor.Code} Armor System Handling before R/F: {armor.ProtectionModifiers.RelativeProtection}");

            armor.ProtectionModifiers.RelativeProtection *= statusIncrease;
            armor.ProtectionModifiers.FlatDamageReduction *= statusIncrease;

            Debug.LogDebug($"[PlateArmor] {player.PlayerName} {armor.Code} Armor System Handling after R/F: {armor.ProtectionModifiers.RelativeProtection}/{armor.ProtectionModifiers.FlatDamageReduction}");

            LevelPlateArmorEvents.ExecuteItemHandledDamage(armor, player);
        }
    }

    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulatePlateArmorConfiguration(coreAPI);
        if (Configuration.enableLevelPlateArmor)
        {
            Configuration.RegisterNewMaxLevelByLevelTypeEXP("PlateArmor", Configuration.leatherArmorMaxLevel);
        }
    }

    [HarmonyPatchCategory("levelup_platearmor")]
    private class PlateArmorPatch
    {
        // Client visual update
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ItemWearable), "GetHeldItemInfo")]
        [HarmonyPriority(Priority.VeryLow)]
        internal static void GetHeldItemInfo(ItemWearable __instance, ItemSlot inSlot, StringBuilder dsc, IWorldAccessor world, bool withDebugInfo)
        {
            if (world.Api is ICoreClientAPI api)
            {
                if (!Configuration.expMultiplyHitPlateArmor.ContainsKey(__instance.Code)) return;

                float statusIncrease = Configuration.PlateArmorStatsIncreaseByLevel(api.World.Player.Entity.WatchedAttributes.GetInt("LevelUP_Level_PlateArmor"));

                if (__instance.ProtectionModifiers != null)
                {
                    if (__instance.ProtectionModifiers.FlatDamageReduction != 0f)
                    {
                        Debug.LogDebug($"[PlateArmor] FlatDamageReduction Before: {__instance.ProtectionModifiers.FlatDamageReduction}");
                        __instance.ProtectionModifiers.FlatDamageReduction *= statusIncrease;
                        Debug.LogDebug($"[PlateArmor] FlatDamageReduction After: {__instance.ProtectionModifiers.FlatDamageReduction}");
                    }
                    if (__instance.ProtectionModifiers.RelativeProtection != 0f)
                    {
                        Debug.LogDebug($"[PlateArmor] RelativeProtection Before: {__instance.ProtectionModifiers.RelativeProtection}");
                        __instance.ProtectionModifiers.RelativeProtection *= statusIncrease;
                        Debug.LogDebug($"[PlateArmor] RelativeProtection After: {__instance.ProtectionModifiers.RelativeProtection}");
                    }
                }

                Debug.LogDebug($"[PlateArmor] #### BEFORE INFO:");
                Debug.LogDebug($"[PlateArmor] healingeffectivness: {__instance.StatModifers.healingeffectivness}");
                Debug.LogDebug($"[PlateArmor] hungerrate: {__instance.StatModifers.hungerrate}");
                Debug.LogDebug($"[PlateArmor] rangedWeaponsAcc: {__instance.StatModifers.rangedWeaponsAcc}");
                Debug.LogDebug($"[PlateArmor] rangedWeaponsSpeed: {__instance.StatModifers.rangedWeaponsSpeed}");
                Debug.LogDebug($"[PlateArmor] walkSpeed: {__instance.StatModifers.walkSpeed}");

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

                Debug.LogDebug($"[PlateArmor] ### AFTER INFO:");
                Debug.LogDebug($"[PlateArmor] healingeffectivness: {__instance.StatModifers.healingeffectivness}");
                Debug.LogDebug($"[PlateArmor] hungerrate: {__instance.StatModifers.hungerrate}");
                Debug.LogDebug($"[PlateArmor] rangedWeaponsAcc: {__instance.StatModifers.rangedWeaponsAcc}");
                Debug.LogDebug($"[PlateArmor] rangedWeaponsSpeed: {__instance.StatModifers.rangedWeaponsSpeed}");
                Debug.LogDebug($"[PlateArmor] walkSpeed: {__instance.StatModifers.walkSpeed}");

                LevelPlateArmorEvents.ExecuteItemInfoUpdated(__instance, api.World.Player);
            }
        }
    }
}

public class LevelPlateArmorEvents
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