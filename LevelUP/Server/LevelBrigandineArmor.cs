#pragma warning disable CA1822
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using Newtonsoft.Json.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
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
        // Update armor stats before functions
        OverwriteDamageInteractionEvents.OnPlayerArmorReceiveHandleStats += StatsUpdated;
        OverwriteDamageInteractionEvents.OnPlayerArmorReceiveDamageStat += DamageReceived;
        // Post function call, reset armors to default
        OverwriteDamageInteractionEvents.OnPlayerArmorReceiveHandleStatsPos += (_, items) => items.ToList().ForEach(ResetArmorAttributes);
        OverwriteDamageInteractionEvents.OnPlayerArmorReceiveDamageStatPos += (_, items, ref __) => items.ToList().ForEach(ResetArmorAttributes);

        Debug.Log("Level Brigandine Armor initialized");
    }

    private void StatsUpdated(IPlayer player, List<ItemSlot> items)
    {
        float statusIncrease = Configuration.BrigandineArmorStatsIncreaseByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_BrigandineArmor"));
        foreach (ItemSlot armorSlot in items)
        {
            if (!Configuration.expMultiplyHitBrigandineArmor.ContainsKey(armorSlot.Itemstack.Collectible.Code)) continue;            

            ItemWearable armor = armorSlot.Itemstack.Item as ItemWearable;

            ResetArmorAttributes(armorSlot);

            RefreshArmorAttributes(armorSlot, statusIncrease);

            LevelBrigandineArmorEvents.ExecuteItemHandledStats(armor, player);
        }
    }

    private void DamageReceived(IPlayer player, List<ItemSlot> items, ref float damage)
    {
        float statusIncrease = Configuration.BrigandineArmorStatsIncreaseByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_BrigandineArmor"));
        foreach (ItemSlot armorSlot in items)
        {
            if (!Configuration.expMultiplyHitBrigandineArmor.ContainsKey(armorSlot.Itemstack.Collectible.Code)) continue;

            ItemWearable armor = armorSlot.Itemstack.Item as ItemWearable;

            double multiply = Configuration.expMultiplyHitBrigandineArmor[armor.Code];
            ulong exp = (ulong)(Configuration.BrigandineArmorBaseEXPEarnedByDAMAGE(damage) * multiply);
            Experience.IncreaseExperience(player, "BrigandineArmor", exp);

            ResetArmorAttributes(armorSlot);

            RefreshArmorAttributes(armorSlot, statusIncrease);

            LevelBrigandineArmorEvents.ExecuteItemHandledDamage(armor, player);
        }
    }

    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulateBrigandineArmorConfiguration(coreAPI);
        if (Configuration.enableLevelBrigandineArmor)
        {
            Configuration.RegisterNewMaxLevelByLevelTypeEXP("BrigandineArmor", Configuration.brigandineArmorMaxLevel);
        }
    }

    private static void RefreshArmorAttributes(ItemSlot armorSlot, float statsIncrease)
    {
        if (armorSlot.Itemstack?.Item is not ItemWearable itemWearable) return;
        JsonObject attr = armorSlot.Itemstack.Item?.Attributes;
        if (attr == null || !attr.Exists) return;
        JObject armorObj = (JObject)attr.Token;

        JsonObject protectionModifiers = attr["protectionModifiers"];
        if (protectionModifiers != null && protectionModifiers.Exists && protectionModifiers.Token is JObject protectionModifiersObj)
        {
            JObject protectionObj = (JObject)protectionModifiersObj.DeepClone();

            if (armorSlot.Itemstack.Attributes.GetString("BaseRelativeProtection") != null)
            {
                float currentMeasured = protectionObj["relativeProtection"].Value<float>();
                float difference = Utils.GetDifferenceBetweenTwoFloats(
                    float.Parse(armorSlot.Itemstack.Attributes.GetString("BaseRelativeProtection")),
                    currentMeasured
                );

                float result = (float.Parse(armorSlot.Itemstack.Attributes.GetString("BaseRelativeProtection")) + difference) * statsIncrease;
                protectionObj["relativeProtection"] = result;
                itemWearable.ProtectionModifiers.RelativeProtection = result;
            }

            if (armorSlot.Itemstack.Attributes.GetString("BaseFlatDamageReduction") != null)
            {
                float currentMeasured = protectionObj["flatDamageReduction"].Value<float>();
                float difference = Utils.GetDifferenceBetweenTwoFloats(
                    float.Parse(armorSlot.Itemstack.Attributes.GetString("BaseFlatDamageReduction")),
                    currentMeasured
                );

                float result = (float.Parse(armorSlot.Itemstack.Attributes.GetString("BaseFlatDamageReduction")) + difference) * statsIncrease;
                protectionObj["flatDamageReduction"] = result;
                itemWearable.ProtectionModifiers.FlatDamageReduction = result;
            }

            armorObj["protectionModifiers"] = protectionObj;
        }

        JsonObject statModifiers = attr["statModifiers"];
        if (statModifiers != null && statModifiers.Exists && statModifiers.Token is JObject statModifiersObj)
        {
            JObject statsObj = (JObject)statModifiersObj.DeepClone();

            if (armorSlot.Itemstack.Attributes.GetString("BaseHealingEffectivness") != null)
            {
                float currentMeasured = statsObj["healingeffectivness"].Value<float>();
                float difference = Utils.GetDifferenceBetweenTwoFloats(float.Parse(armorSlot.Itemstack.Attributes.GetString("BaseHealingEffectivness")), currentMeasured);
                float baseValue = float.Parse(armorSlot.Itemstack.Attributes.GetString("BaseHealingEffectivness")) + difference;
                float positiveValue = Math.Abs(baseValue);

                float result = baseValue + (positiveValue * Math.Min(statsIncrease - 1, 0));
                statsObj["healingeffectivness"] = result;
                itemWearable.StatModifers.healingeffectivness = result;
            }

            if (armorSlot.Itemstack.Attributes.GetString("BaseHungerRate") != null)
            {
                float currentMeasured = statsObj["hungerrate"].Value<float>();
                float difference = Utils.GetDifferenceBetweenTwoFloats(float.Parse(armorSlot.Itemstack.Attributes.GetString("BaseHungerRate")), currentMeasured);
                float baseValue = float.Parse(armorSlot.Itemstack.Attributes.GetString("BaseHungerRate")) + difference;
                float positiveValue = Math.Abs(baseValue);

                float result = baseValue + (positiveValue * Math.Min(statsIncrease - 1, 0));
                statsObj["hungerrate"] = result;
                itemWearable.StatModifers.hungerrate = result;
            }

            if (armorSlot.Itemstack.Attributes.GetString("BaseRangedWeaponsAccuracy") != null)
            {
                float currentMeasured = statsObj["rangedWeaponsAcc"].Value<float>();
                float difference = Utils.GetDifferenceBetweenTwoFloats(float.Parse(armorSlot.Itemstack.Attributes.GetString("BaseRangedWeaponsAccuracy")), currentMeasured);
                float baseValue = float.Parse(armorSlot.Itemstack.Attributes.GetString("BaseRangedWeaponsAccuracy")) + difference;
                float positiveValue = Math.Abs(baseValue);

                float result = baseValue + (positiveValue * Math.Min(statsIncrease - 1, 0));
                statsObj["rangedWeaponsAcc"] = result;
                itemWearable.StatModifers.rangedWeaponsAcc = result;
            }

            if (armorSlot.Itemstack.Attributes.GetString("BaseRangedWeaponsSpeed") != null)
            {
                float currentMeasured = statsObj["rangedWeaponsSpeed"].Value<float>();
                float difference = Utils.GetDifferenceBetweenTwoFloats(float.Parse(armorSlot.Itemstack.Attributes.GetString("BaseRangedWeaponsSpeed")), currentMeasured);
                float baseValue = float.Parse(armorSlot.Itemstack.Attributes.GetString("BaseRangedWeaponsSpeed")) + difference;
                float positiveValue = Math.Abs(baseValue);

                float result = baseValue + (positiveValue * Math.Min(statsIncrease - 1, 0));
                statsObj["rangedWeaponsSpeed"] = result;
                itemWearable.StatModifers.rangedWeaponsSpeed = result;
            }

            if (armorSlot.Itemstack.Attributes.GetString("BaseWalkSpeed") != null)
            {
                float currentMeasured = statsObj["walkSpeed"].Value<float>();
                float difference = Utils.GetDifferenceBetweenTwoFloats(float.Parse(armorSlot.Itemstack.Attributes.GetString("BaseWalkSpeed")), currentMeasured);
                float baseValue = float.Parse(armorSlot.Itemstack.Attributes.GetString("BaseWalkSpeed")) + difference;
                float positiveValue = Math.Abs(baseValue);

                float result = baseValue + (positiveValue * Math.Min(statsIncrease - 1, 0));
                statsObj["walkSpeed"] = result;
                itemWearable.StatModifers.walkSpeed = result;
            }

            armorObj["statModifiers"] = statsObj;
        }
    }

    private static void ResetArmorAttributes(ItemSlot armorSlot)
    {
        if (armorSlot.Itemstack?.Item is not ItemWearable itemWearable) return;
        JsonObject attr = armorSlot.Itemstack.Item?.Attributes;
        if (attr == null || !attr.Exists) return;
        JObject armorObj = (JObject)attr.Token;

        Shared.Instance.GenerateBaseArmorStatus(armorSlot.Itemstack);

        JsonObject protectionModifiers = attr["protectionModifiers"];
        if (protectionModifiers != null && protectionModifiers.Exists && protectionModifiers.Token is JObject protectionModifiersObj)
        {
            JObject protectionObj = (JObject)protectionModifiersObj.DeepClone();

            if (armorSlot.Itemstack.Attributes.GetString("BaseRelativeProtection") != null)
            {
                float result = float.Parse(armorSlot.Itemstack.Attributes.GetString("BaseRelativeProtection"));
                protectionObj["relativeProtection"] = result;
                itemWearable.ProtectionModifiers.RelativeProtection = result;
            }

            if (armorSlot.Itemstack.Attributes.GetString("BaseFlatDamageReduction") != null)
            {
                float result = float.Parse(armorSlot.Itemstack.Attributes.GetString("BaseFlatDamageReduction"));
                protectionObj["flatDamageReduction"] = result;
                itemWearable.ProtectionModifiers.FlatDamageReduction = result;
            }

            armorObj["protectionModifiers"] = protectionObj;
        }

        JsonObject statModifiers = attr["statModifiers"];
        if (statModifiers != null && statModifiers.Exists && statModifiers.Token is JObject statModifiersObj)
        {
            JObject statsObj = (JObject)statModifiersObj.DeepClone();

            if (armorSlot.Itemstack.Attributes.GetString("BaseHealingEffectivness") != null)
            {
                float result = float.Parse(armorSlot.Itemstack.Attributes.GetString("BaseHealingEffectivness"));
                statsObj["healingeffectivness"] = result;
                itemWearable.StatModifers.healingeffectivness = result;
            }

            if (armorSlot.Itemstack.Attributes.GetString("BaseHungerRate") != null)
            {
                float result = float.Parse(armorSlot.Itemstack.Attributes.GetString("BaseHungerRate"));
                statsObj["hungerrate"] = result;
                itemWearable.StatModifers.hungerrate = result;
            }

            if (armorSlot.Itemstack.Attributes.GetString("BaseRangedWeaponsAccuracy") != null)
            {
                float result = float.Parse(armorSlot.Itemstack.Attributes.GetString("BaseRangedWeaponsAccuracy"));
                statsObj["rangedWeaponsAcc"] = result;
                itemWearable.StatModifers.rangedWeaponsAcc = result;
            }

            if (armorSlot.Itemstack.Attributes.GetString("BaseRangedWeaponsSpeed") != null)
            {
                float result = float.Parse(armorSlot.Itemstack.Attributes.GetString("BaseRangedWeaponsSpeed"));
                statsObj["rangedWeaponsSpeed"] = result;
                itemWearable.StatModifers.rangedWeaponsSpeed = result;
            }

            if (armorSlot.Itemstack.Attributes.GetString("BaseWalkSpeed") != null)
            {
                float result = float.Parse(armorSlot.Itemstack.Attributes.GetString("BaseWalkSpeed"));
                statsObj["walkSpeed"] = result;
                itemWearable.StatModifers.walkSpeed = result;
            }

            armorObj["statModifiers"] = statsObj;
        }
    }


    [HarmonyPatchCategory("levelup_brigandinearmor")]
    private class BrigandineArmorPatch
    {
        // Update visual protections and stats
        [HarmonyPrefix] // Client Side
        [HarmonyPatch(typeof(ItemWearable), "GetHeldItemInfo")]
        [HarmonyPriority(Priority.VeryLow)]
        internal static void GetHeldItemInfoStart(ItemWearable __instance, ItemSlot inSlot, StringBuilder dsc, IWorldAccessor world, bool withDebugInfo)
        {
            if (world.Api is ICoreClientAPI api)
            {
                if (!Configuration.expMultiplyHitBrigandineArmor.ContainsKey(__instance.Code)) return;

                float statusIncrease = Configuration.BrigandineArmorStatsIncreaseByLevel(api.World.Player.Entity.WatchedAttributes.GetInt("LevelUP_Level_BrigandineArmor"));

                ResetArmorAttributes(inSlot);

                RefreshArmorAttributes(inSlot, statusIncrease);

                LevelBrigandineArmorEvents.ExecuteItemInfoUpdated(__instance, api.World.Player);
            }
        }

        // Update visual protections and stats
        [HarmonyPostfix] // Client Side
        [HarmonyPatch(typeof(ItemWearable), "GetHeldItemInfo")]
        [HarmonyPriority(Priority.VeryLow)]
        internal static void GetHeldItemInfoFinish(ItemWearable __instance, ItemSlot inSlot, StringBuilder dsc, IWorldAccessor world, bool withDebugInfo)
        {
            ResetArmorAttributes(inSlot);
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