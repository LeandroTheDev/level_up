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

    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulateShieldConfiguration(coreAPI);
        Configuration.RegisterNewMaxLevelByLevelTypeEXP("Shield", Configuration.shieldMaxLevel);
    }

    private static void RefreshShieldAttributes(ItemSlot shieldSlot, float statsIncrease)
    {
        ResetShieldAttributes(shieldSlot);

        JsonObject attr = shieldSlot.Itemstack?.ItemAttributes?["shield"];
        if (attr == null || !attr.Exists) return;
        JObject shieldObj = (JObject)attr.Token;

        Console.WriteLine("AAAAAAAAAAAAAAAAAAAAAAAAA");
        Console.WriteLine(shieldObj);

        JsonObject protection = attr["protectionChance"];
        if (protection != null && protection.Exists && protection.Token is JObject originalProtObj)
        {
            JObject protectionObj = (JObject)originalProtObj.DeepClone();

            {
                Debug.Log($"TEST 1 {shieldSlot.Itemstack.Attributes.GetString("BasePassiveProjectile")}");
                float currentMeasured = protectionObj["passive-projectile"].Value<float>();
                float difference = Utils.GetDifferenceBetweenTwoFloats(
                    float.Parse(shieldSlot.Itemstack.Attributes.GetString("BasePassiveProjectile")),
                    currentMeasured
                );
                Debug.Log($"{difference}");

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
                shieldSlot.Itemstack.Attributes.GetFloat("BaseProjectileDamageAbsorption"),
                currentMeasured
            );

            shieldObj["projectileDamageAbsorption"] =
                (shieldSlot.Itemstack.Attributes.GetFloat("BaseProjectileDamageAbsorption") + difference) * statsIncrease;
        }

        JsonObject damageAbsorption = attr["damageAbsorption"];
        if (damageAbsorption?.Token is JValue damageAbsorptionJVal)
        {
            float currentMeasured = damageAbsorptionJVal.Value<float>();

            float difference = Utils.GetDifferenceBetweenTwoFloats(
                shieldSlot.Itemstack.Attributes.GetFloat("BaseDamageAbsorption"),
                currentMeasured
            );

            shieldObj["damageAbsorption"] =
                (shieldSlot.Itemstack.Attributes.GetFloat("BaseDamageAbsorption") + difference) * statsIncrease;
        }

        Debug.Log($"TEST 2 {shieldSlot.Itemstack.Attributes.GetString("BasePassiveProjectile")}");
        Console.WriteLine("BBBBBBBBBBBBBBBBBBBBBBB");
        Console.WriteLine(shieldObj);
    }

    private static void ResetShieldAttributes(ItemSlot shieldSlot)
    {
        JsonObject attr = shieldSlot.Itemstack?.ItemAttributes?["shield"];
        if (attr == null || !attr.Exists) return;

        Shared.Instance.GenerateBaseShieldStatus(shieldSlot.Itemstack);

        string defaultToken = shieldSlot.Itemstack.Attributes.GetString("BaseProtectionModifiers", null);
        if (defaultToken != null)
        {
            Console.WriteLine("VALUE: ");
            Console.WriteLine(defaultToken);
            Console.WriteLine("#### RESETED TO: ");
            shieldSlot.Itemstack.ItemAttributes["shield"].Token = JObject.Parse(defaultToken);
            Console.WriteLine(shieldSlot.Itemstack.ItemAttributes["shield"].Token);
        }
        else
        {
            Debug.LogError("?");
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

            RefreshShieldAttributes(inSlot, statsIncrease);
        }

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

                RefreshShieldAttributes(shieldSlot, statsIncrease);

                LevelShieldEvents.ExecuteOnShieldRefreshed(player, shieldSlot);
            }
        }

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