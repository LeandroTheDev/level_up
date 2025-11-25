#pragma warning disable CA1822
using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.GameContent;
namespace LevelUP.Server;

class LevelChainArmor
{
    public readonly Harmony patch = new("levelup_chainarmor");
    public void Patch()
    {
        if (!Harmony.HasAnyPatches("levelup_chainarmor"))
        {
            patch.PatchCategory("levelup_chainarmor");
        }
    }
    public void Unpatch()
    {
        if (Harmony.HasAnyPatches("levelup_chainarmor"))
        {
            patch.UnpatchCategory("levelup_chainarmor");
        }
    }

    public void Init()
    {
        Configuration.RegisterNewLevel("ChainArmor");
        Configuration.RegisterNewLevelTypeEXP("ChainArmor", Configuration.ChainArmorGetLevelByEXP);
        Configuration.RegisterNewEXPLevelType("ChainArmor", Configuration.ChainArmorGetExpByLevel);
        OverwriteDamageInteractionEvents.OnPlayerArmorReceiveHandleStats += StatsUpdated;
        OverwriteDamageInteractionEvents.OnPlayerArmorReceiveDamageStat += DamageReceived;

        Debug.Log("Level Chain Armor initialized");
    }

    private void StatsUpdated(IPlayer player, List<ItemWearable> items)
    {
        float statusIncrease = Configuration.ChainArmorStatsIncreaseByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_ChainArmor"));
        foreach (ItemWearable armor in items)
        {
            if (!Configuration.expMultiplyHitChainArmor.ContainsKey(armor.Code)) continue;

            if (armor.StatModifers == null)
            {
                Debug.LogDebug($"[ChainArmor] {player.PlayerName} {armor.Code} Armor System ignored because StatModifers is null");
                return;
            }

            Debug.LogDebug($"[ChainArmor] #### BEFORE STATS:");
            Debug.LogDebug($"[ChainArmor] healingeffectivness: {armor.StatModifers.healingeffectivness}");
            Debug.LogDebug($"[ChainArmor] hungerrate: {armor.StatModifers.hungerrate}");
            Debug.LogDebug($"[ChainArmor] rangedWeaponsAcc: {armor.StatModifers.rangedWeaponsAcc}");
            Debug.LogDebug($"[ChainArmor] rangedWeaponsSpeed: {armor.StatModifers.rangedWeaponsSpeed}");
            Debug.LogDebug($"[ChainArmor] walkSpeed: {armor.StatModifers.walkSpeed}");

            armor.StatModifers.healingeffectivness *= statusIncrease;
            armor.StatModifers.hungerrate *= statusIncrease;
            armor.StatModifers.rangedWeaponsAcc *= statusIncrease;
            armor.StatModifers.rangedWeaponsSpeed *= statusIncrease;
            armor.StatModifers.walkSpeed *= statusIncrease;

            Debug.LogDebug($"[ChainArmor] #### AFTER STATS:");
            Debug.LogDebug($"[ChainArmor] healingeffectivness: {armor.StatModifers.healingeffectivness}");
            Debug.LogDebug($"[ChainArmor] hungerrate: {armor.StatModifers.hungerrate}");
            Debug.LogDebug($"[ChainArmor] rangedWeaponsAcc: {armor.StatModifers.rangedWeaponsAcc}");
            Debug.LogDebug($"[ChainArmor] rangedWeaponsSpeed: {armor.StatModifers.rangedWeaponsSpeed}");
            Debug.LogDebug($"[ChainArmor] walkSpeed: {armor.StatModifers.walkSpeed}");

            LevelChainArmorEvents.ExecuteItemHandledStats(armor, player);
        }
    }

    private void DamageReceived(IPlayer player, List<ItemWearable> items, ref float damage)
    {
        float statusIncrease = Configuration.ChainArmorStatsIncreaseByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_ChainArmor"));
        foreach (ItemWearable armor in items)
        {
            if (!Configuration.expMultiplyHitChainArmor.ContainsKey(armor.Code)) continue;

            double multiply = Configuration.expMultiplyHitChainArmor[armor.Code];
            ulong exp = (ulong)(Configuration.ChainArmorBaseEXPEarnedByDAMAGE(damage) * multiply);
            Experience.IncreaseExperience(player, "ChainArmor", exp);

            if (armor.ProtectionModifiers == null)
            {
                Debug.LogDebug($"[ChainArmor] {player.PlayerName} {armor.Code} Armor System ignored because ProtectionModifiers is null");
                return;
            }

            Debug.LogDebug($"[ChainArmor] {player.PlayerName} {armor.Code} Armor System Handling before R/F: {armor.ProtectionModifiers.RelativeProtection}");

            armor.ProtectionModifiers.RelativeProtection *= statusIncrease;
            armor.ProtectionModifiers.FlatDamageReduction *= statusIncrease;

            Debug.LogDebug($"[ChainArmor] {player.PlayerName} {armor.Code} Armor System Handling after R/F: {armor.ProtectionModifiers.RelativeProtection}/{armor.ProtectionModifiers.FlatDamageReduction}");

            LevelChainArmorEvents.ExecuteItemHandledDamage(armor, player);
        }
    }

    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulateChainArmorConfiguration(coreAPI);
        if (Configuration.enableLevelChainArmor)
        {
            Configuration.RegisterNewMaxLevelByLevelTypeEXP("ChainArmor", Configuration.leatherArmorMaxLevel);
        }
    }

    [HarmonyPatchCategory("levelup_chainarmor")]
    private class ChainArmorPatch
    {
        // Client visual update
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ItemWearable), "GetHeldItemInfo")]
        [HarmonyPriority(Priority.VeryLow)]
        internal static void GetHeldItemInfo(ItemWearable __instance, ItemSlot inSlot, StringBuilder dsc, IWorldAccessor world, bool withDebugInfo)
        {
            if (world.Api is ICoreClientAPI api)
            {
                if (!Configuration.expMultiplyHitChainArmor.ContainsKey(__instance.Code)) return;

                float statusIncrease = Configuration.ChainArmorStatsIncreaseByLevel(api.World.Player.Entity.WatchedAttributes.GetInt("LevelUP_Level_ChainArmor"));

                if (__instance.ProtectionModifiers != null)
                {
                    if (__instance.ProtectionModifiers.FlatDamageReduction != 0f)
                    {
                        Debug.LogDebug($"[ChainArmor] FlatDamageReduction Before: {__instance.ProtectionModifiers.FlatDamageReduction}");
                        __instance.ProtectionModifiers.FlatDamageReduction *= statusIncrease;
                        Debug.LogDebug($"[ChainArmor] FlatDamageReduction After: {__instance.ProtectionModifiers.FlatDamageReduction}");
                    }
                    if (__instance.ProtectionModifiers.RelativeProtection != 0f)
                    {
                        Debug.LogDebug($"[ChainArmor] RelativeProtection Before: {__instance.ProtectionModifiers.RelativeProtection}");
                        __instance.ProtectionModifiers.RelativeProtection *= statusIncrease;
                        Debug.LogDebug($"[ChainArmor] RelativeProtection After: {__instance.ProtectionModifiers.RelativeProtection}");
                    }
                }

                Debug.LogDebug($"[ChainArmor] #### BEFORE INFO:");
                Debug.LogDebug($"[ChainArmor] healingeffectivness: {__instance.StatModifers.healingeffectivness}");
                Debug.LogDebug($"[ChainArmor] hungerrate: {__instance.StatModifers.hungerrate}");
                Debug.LogDebug($"[ChainArmor] rangedWeaponsAcc: {__instance.StatModifers.rangedWeaponsAcc}");
                Debug.LogDebug($"[ChainArmor] rangedWeaponsSpeed: {__instance.StatModifers.rangedWeaponsSpeed}");
                Debug.LogDebug($"[ChainArmor] walkSpeed: {__instance.StatModifers.walkSpeed}");

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

                Debug.LogDebug($"[ChainArmor] ### AFTER INFO:");
                Debug.LogDebug($"[ChainArmor] healingeffectivness: {__instance.StatModifers.healingeffectivness}");
                Debug.LogDebug($"[ChainArmor] hungerrate: {__instance.StatModifers.hungerrate}");
                Debug.LogDebug($"[ChainArmor] rangedWeaponsAcc: {__instance.StatModifers.rangedWeaponsAcc}");
                Debug.LogDebug($"[ChainArmor] rangedWeaponsSpeed: {__instance.StatModifers.rangedWeaponsSpeed}");
                Debug.LogDebug($"[ChainArmor] walkSpeed: {__instance.StatModifers.walkSpeed}");

                LevelChainArmorEvents.ExecuteItemInfoUpdated(__instance, api.World.Player);
            }
        }
    }
}

public class LevelChainArmorEvents
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