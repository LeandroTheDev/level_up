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

    private void StatsUpdated(IPlayer player, List<ItemSlot> items)
    {
        float statusIncrease = Configuration.PlateArmorStatsIncreaseByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_PlateArmor"));
        foreach (ItemSlot armorSlot in items)
        {
            if (!Configuration.expMultiplyHitPlateArmor.ContainsKey(armorSlot.Itemstack.Collectible.Code)) continue;

            Shared.Instance.GenerateBaseArmorStatus(armorSlot.Itemstack);

            ItemWearable armor = armorSlot.Itemstack.Item as ItemWearable;

            if (armor.StatModifers != null)
            {
                if (armorSlot.Itemstack.Attributes.TryGetFloat("BaseHealingEffectivness") != null)
                {
                    float difference = Utils.GetDifferenceBetweenTwoFloats(armorSlot.Itemstack.Attributes.GetFloat("BaseHealingEffectivness"), armor.StatModifers.healingeffectivness);
                    float baseValue = armorSlot.Itemstack.Attributes.GetFloat("BaseHealingEffectivness") + difference;
                    float positiveValue = Math.Abs(baseValue);
                    armor.StatModifers.healingeffectivness = baseValue + (positiveValue * Math.Min(statusIncrease - 1, 0));
                    Debug.LogDebug($"[PlateArmor] Stats updated healingeffectivness: {baseValue}/{armor.StatModifers.healingeffectivness}");
                }
                if (armorSlot.Itemstack.Attributes.TryGetFloat("BaseHungerRate") != null)
                {
                    float difference = Utils.GetDifferenceBetweenTwoFloats(armorSlot.Itemstack.Attributes.GetFloat("BaseHungerRate"), armor.StatModifers.hungerrate);
                    float baseValue = armorSlot.Itemstack.Attributes.GetFloat("BaseHungerRate") + difference;
                    float positiveValue = Math.Abs(baseValue);
                    armor.StatModifers.hungerrate = baseValue - (positiveValue * Math.Min(statusIncrease - 1, 0));
                    Debug.LogDebug($"[PlateArmor] Stats updated hungerrate: {baseValue}/{armor.StatModifers.hungerrate}");
                }
                if (armorSlot.Itemstack.Attributes.TryGetFloat("BaseRangedWeaponsAccuracy") != null)
                {
                    float difference = Utils.GetDifferenceBetweenTwoFloats(armorSlot.Itemstack.Attributes.GetFloat("BaseRangedWeaponsAccuracy"), armor.StatModifers.rangedWeaponsAcc);
                    float baseValue = armorSlot.Itemstack.Attributes.GetFloat("BaseRangedWeaponsAccuracy") + difference;
                    float positiveValue = Math.Abs(baseValue);
                    armor.StatModifers.rangedWeaponsAcc = baseValue + (positiveValue * Math.Min(statusIncrease - 1, 0));
                    Debug.LogDebug($"[PlateArmor] Stats updated rangedWeaponsAcc: {baseValue}/{armor.StatModifers.rangedWeaponsAcc}");
                }
                if (armorSlot.Itemstack.Attributes.TryGetFloat("BaseRangedWeaponsSpeed") != null)
                {
                    float difference = Utils.GetDifferenceBetweenTwoFloats(armorSlot.Itemstack.Attributes.GetFloat("BaseRangedWeaponsSpeed"), armor.StatModifers.rangedWeaponsSpeed);
                    float baseValue = armorSlot.Itemstack.Attributes.GetFloat("BaseRangedWeaponsSpeed") + difference;
                    float positiveValue = Math.Abs(baseValue);
                    armor.StatModifers.rangedWeaponsSpeed = baseValue + (positiveValue * Math.Min(statusIncrease - 1, 0));
                    Debug.LogDebug($"[PlateArmor] Stats updated rangedWeaponsSpeed: {baseValue}/{armor.StatModifers.rangedWeaponsSpeed}");
                }
                if (armorSlot.Itemstack.Attributes.TryGetFloat("BaseWalkSpeed") != null)
                {
                    float difference = Utils.GetDifferenceBetweenTwoFloats(armorSlot.Itemstack.Attributes.GetFloat("BaseWalkSpeed"), armor.StatModifers.walkSpeed);
                    float baseValue = armorSlot.Itemstack.Attributes.GetFloat("BaseWalkSpeed") + difference;
                    float positiveValue = Math.Abs(baseValue);
                    armor.StatModifers.walkSpeed = baseValue + (positiveValue * Math.Min(statusIncrease - 1, 0));
                    Debug.LogDebug($"[PlateArmor] Stats updated walkSpeed: {baseValue}/{armor.StatModifers.walkSpeed}");
                }
            }

            LevelPlateArmorEvents.ExecuteItemHandledStats(armor, player);
        }
    }

    private void DamageReceived(IPlayer player, List<ItemSlot> items, ref float damage)
    {
        float statusIncrease = Configuration.PlateArmorStatsIncreaseByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_PlateArmor"));
        foreach (ItemSlot armorSlot in items)
        {
            if (!Configuration.expMultiplyHitPlateArmor.ContainsKey(armorSlot.Itemstack.Collectible.Code)) continue;

            Shared.Instance.GenerateBaseArmorStatus(armorSlot.Itemstack);

            ItemWearable armor = armorSlot.Itemstack.Item as ItemWearable;

            double multiply = Configuration.expMultiplyHitPlateArmor[armor.Code];
            ulong exp = (ulong)(Configuration.PlateArmorBaseEXPEarnedByDAMAGE(damage) * multiply);
            Experience.IncreaseExperience(player, "PlateArmor", exp);

            if (armor.ProtectionModifiers != null)
            {
                Debug.LogDebug($"[PlateArmor] {player.PlayerName} {armor.Code} Armor System Handling before R/F: {armor.ProtectionModifiers.RelativeProtection}");

                if (armorSlot.Itemstack.Attributes.TryGetFloat("FlatDamageReduction") != null)
                {
                    float difference = Utils.GetDifferenceBetweenTwoFloats(armorSlot.Itemstack.Attributes.GetFloat("BaseFlatDamageReduction"), armor.ProtectionModifiers.FlatDamageReduction);
                    armor.ProtectionModifiers.FlatDamageReduction = (armorSlot.Itemstack.Attributes.GetFloat("BaseFlatDamageReduction") + difference) * statusIncrease;
                }
                if (armorSlot.Itemstack.Attributes.TryGetFloat("BaseRelativeProtection") != null)
                {
                    float difference = Utils.GetDifferenceBetweenTwoFloats(armorSlot.Itemstack.Attributes.GetFloat("BaseRelativeProtection"), armor.ProtectionModifiers.RelativeProtection);
                    armor.ProtectionModifiers.RelativeProtection = (armorSlot.Itemstack.Attributes.GetFloat("BaseRelativeProtection") + difference) * statusIncrease;
                }

                Debug.LogDebug($"[PlateArmor] {player.PlayerName} {armor.Code} Armor System Handling after R/F: {armor.ProtectionModifiers.RelativeProtection}/{armor.ProtectionModifiers.FlatDamageReduction}");
            }

            LevelPlateArmorEvents.ExecuteItemHandledDamage(armor, player);
        }
    }

    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulatePlateArmorConfiguration(coreAPI);
        if (Configuration.enableLevelPlateArmor)
        {
            Configuration.RegisterNewMaxLevelByLevelTypeEXP("PlateArmor", Configuration.plateArmorMaxLevel);
        }
    }

    [HarmonyPatchCategory("levelup_platearmor")]
    private class PlateArmorPatch
    {
        // Update visual protections and stats
        [HarmonyPrefix] // Client Side
        [HarmonyPatch(typeof(ItemWearable), "GetHeldItemInfo")]
        [HarmonyPriority(Priority.VeryLow)]
        internal static void GetHeldItemInfo(ItemWearable __instance, ItemSlot inSlot, StringBuilder dsc, IWorldAccessor world, bool withDebugInfo)
        {
            if (world.Api is ICoreClientAPI api)
            {
                if (!Configuration.expMultiplyHitPlateArmor.ContainsKey(__instance.Code)) return;

                float statusIncrease = Configuration.PlateArmorStatsIncreaseByLevel(api.World.Player.Entity.WatchedAttributes.GetInt("LevelUP_Level_PlateArmor"));

                Shared.Instance.GenerateBaseArmorStatus(inSlot.Itemstack);

                Debug.LogDebug($"---------------- GetHeldItemInfo: {__instance.Code} START ----------------");

                if (__instance.ProtectionModifiers != null)
                {
                    if (inSlot.Itemstack.Attributes.TryGetFloat("BaseFlatDamageReduction") != null)
                    {
                        float difference = Utils.GetDifferenceBetweenTwoFloats(inSlot.Itemstack.Attributes.GetFloat("BaseFlatDamageReduction"), __instance.ProtectionModifiers.FlatDamageReduction);
                        __instance.ProtectionModifiers.FlatDamageReduction = (inSlot.Itemstack.Attributes.GetFloat("BaseFlatDamageReduction") + difference) * statusIncrease;
                        Debug.LogDebug($"[PlateArmor] Info updated FlatDamageReduction: {inSlot.Itemstack.Attributes.GetFloat("BaseFlatDamageReduction") + difference}/{__instance.ProtectionModifiers.FlatDamageReduction}");
                    }
                    if (inSlot.Itemstack.Attributes.TryGetFloat("BaseRelativeProtection") != null)
                    {
                        float difference = Utils.GetDifferenceBetweenTwoFloats(inSlot.Itemstack.Attributes.GetFloat("BaseRelativeProtection"), __instance.ProtectionModifiers.RelativeProtection);
                        __instance.ProtectionModifiers.RelativeProtection = (inSlot.Itemstack.Attributes.GetFloat("BaseRelativeProtection") + difference) * statusIncrease;
                        Debug.LogDebug($"[PlateArmor] Info updated RelativeProtection: {inSlot.Itemstack.Attributes.GetFloat("BaseRelativeProtection") + difference}/{__instance.ProtectionModifiers.RelativeProtection}");
                    }
                }

                if (__instance.StatModifers != null)
                {
                    if (inSlot.Itemstack.Attributes.TryGetFloat("BaseHealingEffectivness") != null)
                    {
                        float difference = Utils.GetDifferenceBetweenTwoFloats(inSlot.Itemstack.Attributes.GetFloat("BaseHealingEffectivness"), __instance.StatModifers.healingeffectivness);
                        float baseValue = inSlot.Itemstack.Attributes.GetFloat("BaseHealingEffectivness") + difference;
                        float positiveValue = Math.Abs(baseValue);
                        __instance.StatModifers.healingeffectivness = baseValue + (positiveValue * Math.Min(statusIncrease - 1, 0));
                        Debug.LogDebug($"[PlateArmor] Info updated healingeffectivness: {baseValue}/{__instance.StatModifers.healingeffectivness}");
                    }
                    if (inSlot.Itemstack.Attributes.TryGetFloat("BaseHungerRate") != null)
                    {
                        float difference = Utils.GetDifferenceBetweenTwoFloats(inSlot.Itemstack.Attributes.GetFloat("BaseHungerRate"), __instance.StatModifers.hungerrate);
                        float baseValue = inSlot.Itemstack.Attributes.GetFloat("BaseHungerRate") + difference;
                        float positiveValue = Math.Abs(baseValue);
                        __instance.StatModifers.hungerrate = baseValue - (positiveValue * Math.Min(statusIncrease - 1, 0));
                        Debug.LogDebug($"[PlateArmor] Info updated hungerrate: {baseValue}/{__instance.StatModifers.hungerrate}");
                    }
                    if (inSlot.Itemstack.Attributes.TryGetFloat("BaseRangedWeaponsAccuracy") != null)
                    {
                        float difference = Utils.GetDifferenceBetweenTwoFloats(inSlot.Itemstack.Attributes.GetFloat("BaseRangedWeaponsAccuracy"), __instance.StatModifers.rangedWeaponsAcc);
                        float baseValue = inSlot.Itemstack.Attributes.GetFloat("BaseRangedWeaponsAccuracy") + difference;
                        float positiveValue = Math.Abs(baseValue);
                        __instance.StatModifers.rangedWeaponsAcc = baseValue + (positiveValue * Math.Min(statusIncrease - 1, 0));
                        Debug.LogDebug($"[PlateArmor] Info updated rangedWeaponsAcc: {baseValue}/{__instance.StatModifers.rangedWeaponsAcc}");
                    }
                    if (inSlot.Itemstack.Attributes.TryGetFloat("BaseRangedWeaponsSpeed") != null)
                    {
                        float difference = Utils.GetDifferenceBetweenTwoFloats(inSlot.Itemstack.Attributes.GetFloat("BaseRangedWeaponsSpeed"), __instance.StatModifers.rangedWeaponsSpeed);
                        float baseValue = inSlot.Itemstack.Attributes.GetFloat("BaseRangedWeaponsSpeed") + difference;
                        float positiveValue = Math.Abs(baseValue);
                        __instance.StatModifers.rangedWeaponsSpeed = baseValue + (positiveValue * Math.Min(statusIncrease - 1, 0));
                        Debug.LogDebug($"[PlateArmor] Info updated rangedWeaponsSpeed: {baseValue}/{__instance.StatModifers.rangedWeaponsSpeed}");
                    }
                    if (inSlot.Itemstack.Attributes.TryGetFloat("BaseWalkSpeed") != null)
                    {
                        float difference = Utils.GetDifferenceBetweenTwoFloats(inSlot.Itemstack.Attributes.GetFloat("BaseWalkSpeed"), __instance.StatModifers.walkSpeed);
                        float baseValue = inSlot.Itemstack.Attributes.GetFloat("BaseWalkSpeed") + difference;
                        float positiveValue = Math.Abs(baseValue);
                        __instance.StatModifers.walkSpeed = baseValue + (positiveValue * Math.Min(statusIncrease - 1, 0));
                        Debug.LogDebug($"[PlateArmor] Info updated walkSpeed: {baseValue}/{__instance.StatModifers.walkSpeed}");
                    }
                }

                Debug.LogDebug($"---------------- GetHeldItemInfo: {__instance.Code} END ----------------");

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