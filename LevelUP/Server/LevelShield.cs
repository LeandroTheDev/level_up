#pragma warning disable CA1822
using System;
using System.Text;
using HarmonyLib;
using Newtonsoft.Json.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.GameContent;

namespace LevelUP.Server;

class LevelShield
{
    public readonly Harmony patch = new("levelup_shield");
    public void Patch()
    {
        if (!Harmony.HasAnyPatches("levelup_shield"))
        {
            patch.PatchCategory("levelup_shield");
        }
    }
    public void Unpatch()
    {
        if (Harmony.HasAnyPatches("levelup_shield"))
        {
            patch.UnpatchCategory("levelup_shield");
        }
    }

    public void Init()
    {
        Configuration.RegisterNewLevel("Shield");
        Configuration.RegisterNewLevelTypeEXP("Shield", Configuration.ShieldGetLevelByEXP);
        Configuration.RegisterNewEXPLevelType("Shield", Configuration.ShieldGetExpByLevel);

        Debug.Log("Level Shield initialized");
    }

    public void InitClient()
    {
        Debug.Log("Level Shield initialized");
    }

    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulateShieldConfiguration(coreAPI);
        Configuration.RegisterNewMaxLevelByLevelTypeEXP("Shield", Configuration.shieldMaxLevel);
    }

    private static void RefreshShieldAttributes(ItemSlot shieldSlot, float statsIncrease)
    {
        JsonObject attr = shieldSlot.Itemstack?.ItemAttributes?["shield"];
        if (attr == null || !attr.Exists) return;
        JObject shieldObj = (JObject)attr.Token;

        JsonObject protection = attr["protectionChance"];
        if (protection != null && protection.Exists && protection.Token is JObject originalProtObj)
        {
            JObject protectionObj = (JObject)originalProtObj.DeepClone();

            {
                float currentMeasured = protectionObj["passive-projectile"].Value<float>();
                float difference = Utils.GetDifferenceBetweenTwoFloats(
                    float.Parse(shieldSlot.Itemstack.Attributes.GetString("BasePassiveProjectile")),
                    currentMeasured
                );

                protectionObj["passive-projectile"] =
                    (float.Parse(shieldSlot.Itemstack.Attributes.GetString("BasePassiveProjectile")) + difference) * statsIncrease;
            }

            {
                float currentMeasured = protectionObj["active-projectile"].Value<float>();
                float difference = Utils.GetDifferenceBetweenTwoFloats(
                    float.Parse(shieldSlot.Itemstack.Attributes.GetString("BaseActiveProjectile")),
                    currentMeasured
                );

                protectionObj["active-projectile"] =
                    (float.Parse(shieldSlot.Itemstack.Attributes.GetString("BaseActiveProjectile")) + difference) * statsIncrease;
            }

            {
                float currentMeasured = protectionObj["passive"].Value<float>();
                float difference = Utils.GetDifferenceBetweenTwoFloats(
                    float.Parse(shieldSlot.Itemstack.Attributes.GetString("BasePassive")),
                    currentMeasured
                );

                protectionObj["passive"] =
                    (float.Parse(shieldSlot.Itemstack.Attributes.GetString("BasePassive")) + difference) * statsIncrease;
            }

            {
                float currentMeasured = protectionObj["active"].Value<float>();
                float difference = Utils.GetDifferenceBetweenTwoFloats(
                    float.Parse(shieldSlot.Itemstack.Attributes.GetString("BaseActive")),
                    currentMeasured
                );

                protectionObj["active"] =
                    (float.Parse(shieldSlot.Itemstack.Attributes.GetString("BaseActive")) + difference) * statsIncrease;
            }

            shieldObj["protectionChance"] = protectionObj;
        }

        JsonObject projectileDamageAbsorption = attr["projectileDamageAbsorption"];
        if (projectileDamageAbsorption?.Token is JValue projectileJVal)
        {
            float currentMeasured = projectileJVal.Value<float>();

            float difference = Utils.GetDifferenceBetweenTwoFloats(
                float.Parse(shieldSlot.Itemstack.Attributes.GetString("BaseProjectileDamageAbsorption")),
                currentMeasured
            );

            shieldObj["projectileDamageAbsorption"] =
                (float.Parse(shieldSlot.Itemstack.Attributes.GetString("BaseProjectileDamageAbsorption")) + difference) * statsIncrease;
        }

        JsonObject damageAbsorption = attr["damageAbsorption"];
        if (damageAbsorption?.Token is JValue damageAbsorptionJVal)
        {
            float currentMeasured = damageAbsorptionJVal.Value<float>();

            float difference = Utils.GetDifferenceBetweenTwoFloats(
                float.Parse(shieldSlot.Itemstack.Attributes.GetString("BaseDamageAbsorption")),
                currentMeasured
            );

            shieldObj["damageAbsorption"] =
                (float.Parse(shieldSlot.Itemstack.Attributes.GetString("BaseDamageAbsorption")) + difference) * statsIncrease;
        }
    }

    private static void ResetShieldAttributes(ItemSlot shieldSlot)
    {
        JsonObject attr = shieldSlot.Itemstack?.ItemAttributes?["shield"];
        if (attr == null || !attr.Exists) return;
        JObject shieldObj = (JObject)attr.Token;

        Shared.Instance.GenerateBaseShieldStatus(shieldSlot.Itemstack);

        JsonObject protection = attr["protectionChance"];
        if (protection != null && protection.Exists && protection.Token is JObject originalProtObj)
        {
            JObject protectionObj = (JObject)originalProtObj.DeepClone();

            protectionObj["passive-projectile"] = float.Parse(shieldSlot.Itemstack.Attributes.GetString("BasePassiveProjectile"));
            protectionObj["active-projectile"] = float.Parse(shieldSlot.Itemstack.Attributes.GetString("BaseActiveProjectile"));
            protectionObj["passive"] = float.Parse(shieldSlot.Itemstack.Attributes.GetString("BasePassive"));
            protectionObj["active"] = float.Parse(shieldSlot.Itemstack.Attributes.GetString("BaseActive"));

            shieldObj["protectionChance"] = protectionObj;
        }

        JsonObject projectileDamageAbsorption = attr["projectileDamageAbsorption"];
        if (projectileDamageAbsorption?.Token is JValue)
        {
            shieldObj["projectileDamageAbsorption"] = float.Parse(shieldSlot.Itemstack.Attributes.GetString("BaseProjectileDamageAbsorption"));
        }

        JsonObject damageAbsorption = attr["damageAbsorption"];
        if (damageAbsorption?.Token is JValue)
        {
            shieldObj["damageAbsorption"] = float.Parse(shieldSlot.Itemstack.Attributes.GetString("BaseDamageAbsorption"));
        }
    }

    [HarmonyPatchCategory("levelup_shield")]
    private class LevelShieldPatch
    {
        // Overwrite the Shield function end
        [HarmonyPrefix] // Client Side
        [HarmonyPatch(typeof(ItemShield), "GetHeldItemInfo")]
        internal static void GetHeldItemInfoStart(ItemShield __instance, ItemSlot inSlot, StringBuilder dsc, IWorldAccessor world, bool withDebugInfo)
        {
            if (world is not IClientWorldAccessor cworld) return;
            IPlayer player = cworld.Player;

            float statsIncrease = Configuration.ShieldGetStatsIncreaseByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Shield"));

            ResetShieldAttributes(inSlot);

            RefreshShieldAttributes(inSlot, statsIncrease);
        }

        // Post function call, reset the shield to default
        [HarmonyPostfix] // Client Side
        [HarmonyPatch(typeof(ItemShield), "GetHeldItemInfo")]
        internal static void GetHeldItemInfoFinish(ItemShield __instance, ItemSlot inSlot, StringBuilder dsc, IWorldAccessor world, bool withDebugInfo)
        {
            ResetShieldAttributes(inSlot);
        }

        // Overwrite the Shield function end
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ModSystemWearableStats), "applyShieldProtection")]
        internal static void ApplyShieldProtectionStart(ModSystemWearableStats __instance, IPlayer player, ref float damage, DamageSource dmgSource)
        {
            float statsIncrease = Configuration.ShieldGetStatsIncreaseByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Shield"));

            for (int i = 0; i < 2; i++)
            {
                ItemSlot shieldSlot;
                if (i == 0)
                {
                    shieldSlot = player.Entity.LeftHandItemSlot;
                }
                else
                {
                    shieldSlot = player.Entity.RightHandItemSlot;
                }

                ResetShieldAttributes(shieldSlot);

                RefreshShieldAttributes(shieldSlot, statsIncrease);

                LevelShieldEvents.ExecuteOnShieldRefreshed(player, shieldSlot);
            }
        }

        // Post function call, reset the shield to default
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ModSystemWearableStats), "applyShieldProtection")]
        internal static void ApplyShieldProtectionFinish(ModSystemWearableStats __instance, IPlayer player, ref float damage, DamageSource dmgSource)
        {
            for (int i = 0; i < 2; i++)
            {
                ItemSlot shieldSlot;
                if (i == 0)
                {
                    shieldSlot = player.Entity.LeftHandItemSlot;
                }
                else
                {
                    shieldSlot = player.Entity.RightHandItemSlot;
                }

                ResetShieldAttributes(shieldSlot);
            }
        }

    }
}

public class LevelShieldEvents
{
    public delegate void PlayerItemSlotHandler(IPlayer player, ItemSlot item);

    public static event PlayerItemSlotHandler OnShieldRefreshed;

    internal static void ExecuteOnShieldRefreshed(IPlayer player, ItemSlot shield)
    {
        OnShieldRefreshed?.Invoke(player, shield);
    }
}