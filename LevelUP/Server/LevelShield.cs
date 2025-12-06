#pragma warning disable CA1822
using System.Text;
using HarmonyLib;
using LevelUP.Client;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
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
        StatusViewEvents.OnStatusRequested += StatusViewRequested;

        Debug.Log("Level Shield initialized");
    }

    public void Dispose()
    {
        StatusViewEvents.OnStatusRequested -= StatusViewRequested;
    }

    private void StatusViewRequested(IPlayer player, ref StringBuilder stringBuilder, string levelType)
    {
        if (levelType != "Shield") return;

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_passiveprojectile",
                Utils.GetPorcentageFromFloatsStart1(Configuration.ShieldGetPassiveProjectileByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Shield")))
            )
        );

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_activeprojectile",
                Utils.GetPorcentageFromFloatsStart1(Configuration.ShieldGetActiveProjectileByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Shield")))
            )
        );

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_passive",
                Utils.GetPorcentageFromFloatsStart1(Configuration.ShieldGetPassiveByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Shield")))
            )
        );

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_active",
                Utils.GetPorcentageFromFloatsStart1(Configuration.ShieldGetPassiveByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Shield")))
            )
        );

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_projectiledamageabsorption",
                Utils.GetPorcentageFromFloatsStart1(Configuration.ShieldGetProjectileDamageAbsorptionByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Shield")))
            )
        );

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_damageabsorption",
                Utils.GetPorcentageFromFloatsStart1(Configuration.ShieldGetDamageAbsorptionByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Shield")))
            )
        );
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
        internal static void GetHeldItemInfoStart(ItemShield __instance, ItemSlot inSlot, StringBuilder dsc, IWorldAccessor world, bool withDebugInfo)
        {
            if (world is not IClientWorldAccessor cworld) return;
            IPlayer player = cworld.Player;

            int playerLevel = player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Shield");
            float passiveProjectile = Configuration.ShieldGetPassiveProjectileByLevel(playerLevel);
            float activeProjectile = Configuration.ShieldGetActiveProjectileByLevel(playerLevel);
            float passive = Configuration.ShieldGetPassiveByLevel(playerLevel);
            float active = Configuration.ShieldGetActiveByLevel(playerLevel);
            float projectileDamageAbsorption = Configuration.ShieldGetProjectileDamageAbsorptionByLevel(playerLevel);
            float damageAbsorption = Configuration.ShieldGetDamageAbsorptionByLevel(playerLevel);

            Shared.Instance.ResetShieldAttributes(inSlot);

            float smithingBaseMultiply = float.Parse(inSlot.Itemstack.Attributes.GetString("LevelUP_Smithing_StatsMultiply", "1"));

            Shared.Instance.RefreshShieldAttributes(
                inSlot,
                smithingBaseMultiply * passiveProjectile,
                smithingBaseMultiply * activeProjectile,
                smithingBaseMultiply * passive,
                smithingBaseMultiply * active,
                smithingBaseMultiply * projectileDamageAbsorption,
                smithingBaseMultiply * damageAbsorption);
        }

        // Post function call, reset the shield to default
        [HarmonyPostfix] // Client Side
        [HarmonyPatch(typeof(ItemShield), "GetHeldItemInfo")]
        internal static void GetHeldItemInfoFinish(ItemShield __instance, ItemSlot inSlot, StringBuilder dsc, IWorldAccessor world, bool withDebugInfo)
        {
            Shared.Instance.ResetShieldAttributes(inSlot);
        }

        // Overwrite the Shield function end
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ModSystemWearableStats), "applyShieldProtection")]
        internal static void ApplyShieldProtectionStart(ModSystemWearableStats __instance, IPlayer player, ref float damage, DamageSource dmgSource)
        {
            int playerLevel = player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Shield");
            float passiveProjectile = Configuration.ShieldGetPassiveProjectileByLevel(playerLevel);
            float activeProjectile = Configuration.ShieldGetActiveProjectileByLevel(playerLevel);
            float passive = Configuration.ShieldGetPassiveByLevel(playerLevel);
            float active = Configuration.ShieldGetActiveByLevel(playerLevel);
            float projectileDamageAbsorption = Configuration.ShieldGetProjectileDamageAbsorptionByLevel(playerLevel);
            float damageAbsorption = Configuration.ShieldGetDamageAbsorptionByLevel(playerLevel);

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

                Shared.Instance.ResetShieldAttributes(shieldSlot);

                float smithingBaseMultiply = float.Parse(shieldSlot.Itemstack.Attributes.GetString("LevelUP_Smithing_StatsMultiply", "1"));

                Shared.Instance.RefreshShieldAttributes(
                    shieldSlot,
                    smithingBaseMultiply * passiveProjectile,
                    smithingBaseMultiply * activeProjectile,
                    smithingBaseMultiply * passive,
                    smithingBaseMultiply * active,
                    smithingBaseMultiply * projectileDamageAbsorption,
                    smithingBaseMultiply * damageAbsorption);

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

                Shared.Instance.ResetShieldAttributes(shieldSlot);
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