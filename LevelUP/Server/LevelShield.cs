#pragma warning disable CA1822
using System;
using System.Text;
using HarmonyLib;
using Newtonsoft.Json.Linq;
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

    [HarmonyPatchCategory("levelup_shield")]
    private class LevelShieldPatch
    {
        // Overwrite the Shield function end
        [HarmonyPrefix] // Client Side
        [HarmonyPatch(typeof(ItemShield), "GetHeldItemInfo")]
        internal static void GetHeldItemInfo(ItemShield __instance, ItemSlot inSlot, StringBuilder dsc, IWorldAccessor world, bool withDebugInfo)
        {

        }

        // Overwrite the Shield function end
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ModSystemWearableStats), "applyShieldProtection")]
        internal static void ApplyShieldProtectionStart(ModSystemWearableStats __instance, IPlayer player, ref float damage, DamageSource dmgSource)
        {
            ItemSlot[] shieldSlots =
            [
                player.Entity.LeftHandItemSlot,
                player.Entity.RightHandItemSlot
            ];

            float statsIncrease = Configuration.ShieldGetStatsIncreaseByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Shield"));

            for (int i = 0; i < shieldSlots.Length; i++)
            {
                ItemSlot shieldSlot = shieldSlots[i];

                JsonObject attr = shieldSlot.Itemstack?.ItemAttributes?["shield"];
                if (attr == null || !attr.Exists) continue;

                JsonObject protection = attr["protectionChance"];
                if (protection != null && protection.Exists && protection.Token is JObject protectionObj)
                {
                    foreach (var prop in protectionObj.Properties())
                    {
                        float current = prop.Value.Value<float>();
                        float updated = current * statsIncrease;

                        prop.Value = new JValue(updated);
                    }
                }

                JsonObject projectileDamageAbsorption = attr["projectileDamageAbsorption"];
                if (projectileDamageAbsorption != null && projectileDamageAbsorption.Exists && projectileDamageAbsorption.Token is JObject projectileDamageAbsorptionObj)
                {
                    foreach (var prop in projectileDamageAbsorptionObj.Properties())
                    {
                        float current = prop.Value.Value<float>();
                        float updated = current * statsIncrease;

                        prop.Value = new JValue(updated);
                    }
                }

                JsonObject damageAbsorption = attr["damageAbsorption"];
                if (damageAbsorption != null && damageAbsorption.Exists && damageAbsorption.Token is JObject damageAbsorptionObj)
                {
                    foreach (var prop in damageAbsorptionObj.Properties())
                    {
                        float current = prop.Value.Value<float>();
                        float updated = current * statsIncrease;

                        prop.Value = new JValue(updated);
                    }
                }

                LevelShieldEvents.ExecuteOnShieldRefreshed(player, shieldSlot);
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