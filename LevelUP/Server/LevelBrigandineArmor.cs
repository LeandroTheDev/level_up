#pragma warning disable CA1822
using System.Collections.Generic;
using HarmonyLib;
using Vintagestory.API.Common;
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

        Debug.Log("Level Brigandine Armor initialized");
    }

    public void InitClient()
    {
        OverwriteDamageInteractionEvents.OnPlayerArmorViewStats += ViewReceived;

        Debug.Log("Level Brigandine Armor initialized");
    }

    public void Dispose()
    {
        OverwriteDamageInteractionEvents.OnPlayerArmorReceiveHandleStats -= StatsUpdated;
        OverwriteDamageInteractionEvents.OnPlayerArmorReceiveDamageStat -= DamageReceived;
        OverwriteDamageInteractionEvents.OnPlayerArmorViewStats -= ViewReceived;
    }

    private void ViewReceived(IPlayer player, ItemSlot item)
    {
        if (!Configuration.expMultiplyHitBrigandineArmor.ContainsKey(item.Itemstack.Collectible.Code)) return;
        float statusIncrease = Configuration.BrigandineArmorStatsIncreaseByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_BrigandineArmor"));
        Shared.Instance.RefreshArmorAttributes(item, statusIncrease);
        LevelBrigandineArmorEvents.ExecuteItemInfoUpdated(item.Itemstack.Item as ItemWearable, player);
    }

    private void StatsUpdated(IPlayer player, List<ItemSlot> items)
    {
        float statusIncrease = Configuration.BrigandineArmorStatsIncreaseByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_BrigandineArmor"));
        foreach (ItemSlot armorSlot in items)
        {
            if (!Configuration.expMultiplyHitBrigandineArmor.ContainsKey(armorSlot.Itemstack.Collectible.Code)) continue;

            ItemWearable armor = armorSlot.Itemstack.Item as ItemWearable;

            Shared.Instance.RefreshArmorAttributes(armorSlot, statusIncrease);

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

            Shared.Instance.RefreshArmorAttributes(armorSlot, statusIncrease);

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

    [HarmonyPatchCategory("levelup_brigandinearmor")]
    private class BrigandineArmorPatch
    { }
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