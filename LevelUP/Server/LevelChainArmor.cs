#pragma warning disable CA1822
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using LevelUP.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.GameContent;
namespace LevelUP.Server;

class LevelChainArmor
{
    public readonly Harmony patch = new("levelup_chainarmor");
    public void Patch()
    {
        if (!Harmony.HasAnyPatches("levelup_chainarmor"))
        {
            patch.PatchCategory("levelup_chainarmor");
        }
    }
    public void Unpatch()
    {
        if (Harmony.HasAnyPatches("levelup_chainarmor"))
        {
            patch.UnpatchCategory("levelup_chainarmor");
        }
    }

    public void Init()
    {
        Configuration.RegisterNewLevel("ChainArmor");
        Configuration.RegisterNewLevelTypeEXP("ChainArmor", Configuration.ChainArmorGetLevelByEXP);
        Configuration.RegisterNewEXPLevelType("ChainArmor", Configuration.ChainArmorGetExpByLevel);
        // Update armor stats before functions
        OverwriteDamageInteractionEvents.OnPlayerArmorReceiveHandleStats += StatsUpdated;
        OverwriteDamageInteractionEvents.OnPlayerArmorReceiveDamageStat += DamageReceived;

        Debug.Log("Level Chain Armor initialized");
    }

    public void InitClient()
    {
        OverwriteDamageInteractionEvents.OnPlayerArmorViewStats += ViewReceived;
        StatusViewEvents.OnStatusRequested += StatusViewRequested;

        Debug.Log("Level Chain Armor initialized");
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
        if (levelType != "ChainArmor") return;

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_statsincreaser",
                Utils.GetPorcentageFromFloatsStart1(Configuration.ChainArmorStatsIncreaseByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_ChainArmor")))
            )
        );
    }

    private void ViewReceived(IPlayer player, ItemSlot item)
    {
        if (!Configuration.expMultiplyHitChainArmor.ContainsKey(item.Itemstack.Collectible.Code)) return;
        float statusIncrease = Configuration.ChainArmorStatsIncreaseByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_ChainArmor"));
        Shared.Instance.RefreshArmorAttributes(item, statusIncrease);
        LevelChainArmorEvents.ExecuteItemInfoUpdated(item.Itemstack.Item as ItemWearable, player);
    }

    private void StatsUpdated(IPlayer player, List<ItemSlot> items)
    {
        float statusIncrease = Configuration.ChainArmorStatsIncreaseByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_ChainArmor"));
        foreach (ItemSlot armorSlot in items)
        {
            if (!Configuration.expMultiplyHitChainArmor.ContainsKey(armorSlot.Itemstack.Collectible.Code)) continue;

            ItemWearable armor = armorSlot.Itemstack.Item as ItemWearable;

            Shared.Instance.RefreshArmorAttributes(armorSlot, statusIncrease);

            LevelChainArmorEvents.ExecuteItemHandledStats(armor, player);
        }
    }

    private void DamageReceived(IPlayer player, List<ItemSlot> items, ref float damage)
    {
        float statusIncrease = Configuration.ChainArmorStatsIncreaseByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_ChainArmor"));
        foreach (ItemSlot armorSlot in items)
        {
            if (!Configuration.expMultiplyHitChainArmor.ContainsKey(armorSlot.Itemstack.Collectible.Code)) continue;

            ItemWearable armor = armorSlot.Itemstack.Item as ItemWearable;

            double multiply = Configuration.expMultiplyHitChainArmor[armor.Code];
            ulong exp = (ulong)(Configuration.ChainArmorBaseEXPEarnedByDAMAGE(damage) * multiply);
            Experience.IncreaseExperience(player, "ChainArmor", exp);

            Shared.Instance.RefreshArmorAttributes(armorSlot, statusIncrease);

            LevelChainArmorEvents.ExecuteItemHandledDamage(armor, player);
        }
    }

    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulateChainArmorConfiguration(coreAPI);
        if (Configuration.enableLevelChainArmor)
        {
            Configuration.RegisterNewMaxLevelByLevelTypeEXP("ChainArmor", Configuration.chainArmorMaxLevel);
        }
    }

    [HarmonyPatchCategory("levelup_chainarmor")]
    private class ChainArmorPatch
    { }
}

public class LevelChainArmorEvents
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