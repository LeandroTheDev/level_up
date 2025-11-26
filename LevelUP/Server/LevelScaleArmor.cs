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

    private void StatsUpdated(IPlayer player, List<ItemSlot> items)
    {
        float statusIncrease = Configuration.ScaleArmorStatsIncreaseByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_ScaleArmor"));
        foreach (ItemSlot armorSlot in items)
        {
            if (!Configuration.expMultiplyHitScaleArmor.ContainsKey(armorSlot.Itemstack.Collectible.Code)) continue;

            Shared.Instance.GenerateBaseArmorStatus(armorSlot.Itemstack);

            ItemWearable armor = armorSlot.Itemstack.Item as ItemWearable;

            if (armor.StatModifers == null)
            {
                Debug.LogDebug($"[ScaleArmor] {player.PlayerName} {armor.Code} Armor System ignored because StatModifers is null");
                return;
            }

            if (armorSlot.Itemstack.Attributes.TryGetFloat("BaseHealingEffectivness") != null)
            {
                float healingeffectivness = armorSlot.Itemstack.Attributes.GetFloat("BaseHealingEffectivness");
                if (healingeffectivness < 0f)
                {
                    float positiveValue = Math.Abs(healingeffectivness);
                    armor.StatModifers.healingeffectivness = positiveValue * statusIncrease;
                }
                else
                {
                    armor.StatModifers.healingeffectivness = healingeffectivness * statusIncrease;
                }
            }
            if (armorSlot.Itemstack.Attributes.TryGetFloat("BaseHungerRate") != null)
            {
                float hungerrate = armorSlot.Itemstack.Attributes.GetFloat("BaseHungerRate");
                if (hungerrate < 0f)
                {
                    float positiveValue = Math.Abs(hungerrate);
                    armor.StatModifers.hungerrate = positiveValue * statusIncrease;
                }
                else
                {
                    armor.StatModifers.hungerrate = hungerrate * statusIncrease;
                }
            }
            if (armorSlot.Itemstack.Attributes.TryGetFloat("BaseRangedWeaponsAccuracy") != null)
            {
                float rangedWeaponsAcc = armorSlot.Itemstack.Attributes.GetFloat("BaseRangedWeaponsAccuracy");
                if (rangedWeaponsAcc < 0f)
                {
                    float positiveValue = Math.Abs(rangedWeaponsAcc);
                    armor.StatModifers.rangedWeaponsAcc = positiveValue * statusIncrease;
                }
                else
                {
                    armor.StatModifers.rangedWeaponsAcc = rangedWeaponsAcc * statusIncrease;
                }
            }
            if (armorSlot.Itemstack.Attributes.TryGetFloat("BaseRangedWeaponsSpeed") != null)
            {
                float rangedWeaponsSpeed = armorSlot.Itemstack.Attributes.GetFloat("BaseRangedWeaponsSpeed");
                if (rangedWeaponsSpeed < 0f)
                {
                    float positiveValue = Math.Abs(rangedWeaponsSpeed);
                    armor.StatModifers.rangedWeaponsSpeed = positiveValue * statusIncrease;
                }
                else
                {
                    armor.StatModifers.rangedWeaponsSpeed = rangedWeaponsSpeed * statusIncrease;
                }
            }
            if (armorSlot.Itemstack.Attributes.TryGetFloat("BaseWalkSpeed") != null)
            {
                float walkSpeed = armorSlot.Itemstack.Attributes.GetFloat("BaseWalkSpeed");
                if (walkSpeed < 0f)
                {
                    float positiveValue = Math.Abs(walkSpeed);
                    armor.StatModifers.walkSpeed = positiveValue * statusIncrease;
                }
                else
                {
                    armor.StatModifers.walkSpeed = walkSpeed * statusIncrease;
                }
            }

            LevelScaleArmorEvents.ExecuteItemHandledStats(armor, player);
        }
    }

    private void DamageReceived(IPlayer player, List<ItemSlot> items, ref float damage)
    {
        float statusIncrease = Configuration.ScaleArmorStatsIncreaseByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_ScaleArmor"));
        foreach (ItemSlot armorSlot in items)
        {
            if (!Configuration.expMultiplyHitScaleArmor.ContainsKey(armorSlot.Itemstack.Collectible.Code)) continue;

            Shared.Instance.GenerateBaseArmorStatus(armorSlot.Itemstack);

            ItemWearable armor = armorSlot.Itemstack.Item as ItemWearable;

            double multiply = Configuration.expMultiplyHitScaleArmor[armor.Code];
            ulong exp = (ulong)(Configuration.ScaleArmorBaseEXPEarnedByDAMAGE(damage) * multiply);
            Experience.IncreaseExperience(player, "ScaleArmor", exp);

            if (armor.ProtectionModifiers == null)
            {
                Debug.LogDebug($"[ScaleArmor] {player.PlayerName} {armor.Code} Armor System ignored because ProtectionModifiers is null");
                return;
            }

            Debug.LogDebug($"[ScaleArmor] {player.PlayerName} {armor.Code} Armor System Handling before R/F: {armor.ProtectionModifiers.RelativeProtection}");

            if (armorSlot.Itemstack.Attributes.TryGetFloat("FlatDamageReduction") != null)
                armor.ProtectionModifiers.FlatDamageReduction = armorSlot.Itemstack.Attributes.GetFloat("BaseFlatDamageReduction") * statusIncrease;
            if (armorSlot.Itemstack.Attributes.TryGetFloat("BaseRelativeProtection") != null)
                armor.ProtectionModifiers.RelativeProtection = armorSlot.Itemstack.Attributes.GetFloat("BaseRelativeProtection") * statusIncrease;

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
            Configuration.RegisterNewMaxLevelByLevelTypeEXP("ScaleArmor", Configuration.scaleArmorMaxLevel);
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

                Shared.Instance.GenerateBaseArmorStatus(inSlot.Itemstack);

                if (__instance.ProtectionModifiers != null)
                {
                    if (inSlot.Itemstack.Attributes.TryGetFloat("FlatDamageReduction") != null)
                        __instance.ProtectionModifiers.FlatDamageReduction = inSlot.Itemstack.Attributes.GetFloat("BaseFlatDamageReduction") * statusIncrease;
                    if (inSlot.Itemstack.Attributes.TryGetFloat("BaseRelativeProtection") != null)
                        __instance.ProtectionModifiers.RelativeProtection = inSlot.Itemstack.Attributes.GetFloat("BaseRelativeProtection") * statusIncrease;
                }

                if (__instance.StatModifers != null)
                {
                    if (inSlot.Itemstack.Attributes.TryGetFloat("BaseHealingEffectivness") != null)
                    {
                        float healingeffectivness = inSlot.Itemstack.Attributes.GetFloat("BaseHealingEffectivness");
                        if (healingeffectivness < 0f)
                        {
                            float positiveValue = Math.Abs(healingeffectivness);
                            __instance.StatModifers.healingeffectivness = positiveValue * statusIncrease;
                        }
                        else
                        {
                            __instance.StatModifers.healingeffectivness = healingeffectivness * statusIncrease;
                        }
                    }
                    if (inSlot.Itemstack.Attributes.TryGetFloat("BaseHungerRate") != null)
                    {
                        float hungerrate = inSlot.Itemstack.Attributes.GetFloat("BaseHungerRate");
                        if (hungerrate < 0f)
                        {
                            float positiveValue = Math.Abs(hungerrate);
                            __instance.StatModifers.hungerrate = positiveValue * statusIncrease;
                        }
                        else
                        {
                            __instance.StatModifers.hungerrate = hungerrate * statusIncrease;
                        }
                    }
                    if (inSlot.Itemstack.Attributes.TryGetFloat("BaseRangedWeaponsAccuracy") != null)
                    {
                        float rangedWeaponsAcc = inSlot.Itemstack.Attributes.GetFloat("BaseRangedWeaponsAccuracy");
                        if (rangedWeaponsAcc < 0f)
                        {
                            float positiveValue = Math.Abs(rangedWeaponsAcc);
                            __instance.StatModifers.rangedWeaponsAcc = positiveValue * statusIncrease;
                        }
                        else
                        {
                            __instance.StatModifers.rangedWeaponsAcc = rangedWeaponsAcc * statusIncrease;
                        }
                    }
                    if (inSlot.Itemstack.Attributes.TryGetFloat("BaseRangedWeaponsSpeed") != null)
                    {
                        float rangedWeaponsSpeed = inSlot.Itemstack.Attributes.GetFloat("BaseRangedWeaponsSpeed");
                        if (rangedWeaponsSpeed < 0f)
                        {
                            float positiveValue = Math.Abs(rangedWeaponsSpeed);
                            __instance.StatModifers.rangedWeaponsSpeed = positiveValue * statusIncrease;
                        }
                        else
                        {
                            __instance.StatModifers.rangedWeaponsSpeed = rangedWeaponsSpeed * statusIncrease;
                        }
                    }
                    if (inSlot.Itemstack.Attributes.TryGetFloat("BaseWalkSpeed") != null)
                    {
                        float walkSpeed = inSlot.Itemstack.Attributes.GetFloat("BaseWalkSpeed");
                        if (walkSpeed < 0f)
                        {
                            float positiveValue = Math.Abs(walkSpeed);
                            __instance.StatModifers.walkSpeed = positiveValue * statusIncrease;
                        }
                        else
                        {
                            __instance.StatModifers.walkSpeed = walkSpeed * statusIncrease;
                        }
                    }
                }

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