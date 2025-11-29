#pragma warning disable CA1822
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using LevelUP.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.GameContent;
namespace LevelUP.Server;

class LevelLeatherArmor
{
    public readonly Harmony patch = new("levelup_leatherarmor");
    public void Patch()
    {
        if (!Harmony.HasAnyPatches("levelup_leatherarmor"))
        {
            patch.PatchCategory("levelup_leatherarmor");
        }
    }
    public void Unpatch()
    {
        if (Harmony.HasAnyPatches("levelup_leatherarmor"))
        {
            patch.UnpatchCategory("levelup_leatherarmor");
        }
    }

    public void Init()
    {
        Configuration.RegisterNewLevel("LeatherArmor");
        Configuration.RegisterNewLevelTypeEXP("LeatherArmor", Configuration.LeatherArmorGetLevelByEXP);
        Configuration.RegisterNewEXPLevelType("LeatherArmor", Configuration.LeatherArmorGetExpByLevel);
        // Update armor stats before functions
        OverwriteDamageInteractionEvents.OnPlayerArmorReceiveHandleStats += StatsUpdated;
        OverwriteDamageInteractionEvents.OnPlayerArmorReceiveDamageStat += DamageReceived;

        Debug.Log("Level Leather Armor initialized");
    }

    public void InitClient()
    {
        OverwriteDamageInteractionEvents.OnPlayerArmorViewStats += ViewReceived;
        StatusViewEvents.OnStatusRequested += StatusViewRequested;

        Debug.Log("Level Leather Armor initialized");
    }

    public void Dispose()
    {
        OverwriteDamageInteractionEvents.OnPlayerArmorReceiveHandleStats -= StatsUpdated;
        OverwriteDamageInteractionEvents.OnPlayerArmorReceiveDamageStat -= DamageReceived;
        OverwriteDamageInteractionEvents.OnPlayerArmorViewStats -= ViewReceived;
        StatusViewEvents.OnStatusRequested -= StatusViewRequested;
    }

    private void StatusViewRequested(IPlayer player, ref StringBuilder stringBuilder, string levelType)
    {
        if (levelType != "LeatherArmor") return;

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_statsincreaser",
                Utils.GetPorcentageFromFloatsStart1(Configuration.LeatherArmorStatsIncreaseByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_LeatherArmor")))
            )
        );
    }

    private void ViewReceived(IPlayer player, ItemSlot item)
    {
        if (!Configuration.expMultiplyHitLeatherArmor.ContainsKey(item.Itemstack.Collectible.Code)) return;
        float statusIncrease = Configuration.LeatherArmorStatsIncreaseByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_LeatherArmor"));
        Shared.Instance.RefreshArmorAttributes(item, statusIncrease);
        LevelLeatherArmorEvents.ExecuteItemInfoUpdated(item.Itemstack.Item as ItemWearable, player);
    }

    private void StatsUpdated(IPlayer player, List<ItemSlot> items)
    {
        float statusIncrease = Configuration.LeatherArmorStatsIncreaseByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_LeatherArmor"));
        foreach (ItemSlot armorSlot in items)
        {
            if (!Configuration.expMultiplyHitLeatherArmor.ContainsKey(armorSlot.Itemstack.Collectible.Code)) continue;

            ItemWearable armor = armorSlot.Itemstack.Item as ItemWearable;

            Shared.Instance.RefreshArmorAttributes(armorSlot, statusIncrease);

            LevelLeatherArmorEvents.ExecuteItemHandledStats(armor, player);
        }
    }

    private void DamageReceived(IPlayer player, List<ItemSlot> items, ref float damage)
    {
        float statusIncrease = Configuration.LeatherArmorStatsIncreaseByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_LeatherArmor"));
        foreach (ItemSlot armorSlot in items)
        {
            if (!Configuration.expMultiplyHitLeatherArmor.ContainsKey(armorSlot.Itemstack.Collectible.Code)) continue;

            ItemWearable armor = armorSlot.Itemstack.Item as ItemWearable;

            double multiply = Configuration.expMultiplyHitLeatherArmor[armor.Code];
            ulong exp = (ulong)(Configuration.LeatherArmorBaseEXPEarnedByDAMAGE(damage) * multiply);
            Experience.IncreaseExperience(player, "LeatherArmor", exp);

            Shared.Instance.RefreshArmorAttributes(armorSlot, statusIncrease);

            LevelLeatherArmorEvents.ExecuteItemHandledDamage(armor, player);
        }
    }

    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulateLeatherArmorConfiguration(coreAPI);
        if (Configuration.enableLevelLeatherArmor)
        {
            Configuration.RegisterNewMaxLevelByLevelTypeEXP("LeatherArmor", Configuration.leatherArmorMaxLevel);
        }
    }

    [HarmonyPatchCategory("levelup_leatherarmor")]
    private class LeatherArmorPatch
    { }
}

public class LevelLeatherArmorEvents
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