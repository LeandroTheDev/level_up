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
            Lang.Get("levelup:status_statsincreaser",
                Utils.GetPorcentageFromFloatsStart1(Configuration.ShieldGetStatsIncreaseByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Shield")))
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

            float statsIncrease = Configuration.ShieldGetStatsIncreaseByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Shield"));

            Shared.Instance.ResetShieldAttributes(inSlot);

            Shared.Instance.RefreshShieldAttributes(inSlot, statsIncrease);
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

                Shared.Instance.ResetShieldAttributes(shieldSlot);

                Shared.Instance.RefreshShieldAttributes(shieldSlot, statsIncrease);

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