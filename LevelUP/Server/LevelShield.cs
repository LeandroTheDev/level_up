#pragma warning disable CA1822
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
                    Debug.LogDebug($"[Shield] ProtectionChance Before: {protection.Token}");

                    foreach (var prop in protectionObj.Properties())
                    {
                        float current = prop.Value.Value<float>();
                        float updated = current * statsIncrease;

                        prop.Value = new JValue(updated);
                    }

                    Debug.LogDebug($"[Shield] ProtectionChance After: {protection.Token}");
                }

                JsonObject projectileAbosrption = attr["projectileDamageAbsorption"];
                if (projectileAbosrption != null && projectileAbosrption.Exists && projectileAbosrption.Token is JObject projectileAbosrptionObj)
                {
                    Debug.LogDebug($"[Shield] Projectile Absorption Before: {projectileAbosrption.Token}");

                    foreach (var prop in projectileAbosrptionObj.Properties())
                    {
                        float current = prop.Value.Value<float>();
                        float updated = current * statsIncrease;

                        prop.Value = new JValue(updated);
                    }

                    Debug.LogDebug($"[Shield] Projectile Absorption After: {projectileAbosrption.Token}");
                }

                JsonObject damageAbsorption = attr["damageAbsorption"];
                if (damageAbsorption != null && damageAbsorption.Exists && damageAbsorption.Token is JObject damageAbsorptionObj)
                {
                    Debug.LogDebug($"[Shield] Damage Absorption Before: {damageAbsorption.Token}");

                    foreach (var prop in damageAbsorptionObj.Properties())
                    {
                        float current = prop.Value.Value<float>();
                        float updated = current * statsIncrease;

                        prop.Value = new JValue(updated);
                    }

                    Debug.LogDebug($"[Shield] Damage Absorption After: {damageAbsorption.Token}");
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