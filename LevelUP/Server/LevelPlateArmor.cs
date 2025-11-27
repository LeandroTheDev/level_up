#pragma warning disable CA1822
using System.Collections.Generic;
using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.GameContent;
namespace LevelUP.Server;

class LevelPlateArmor
{
    public readonly Harmony patch = new("levelup_platearmor");
    public void Patch()
    {
        if (!Harmony.HasAnyPatches("levelup_platearmor"))
        {
            patch.PatchCategory("levelup_platearmor");
        }
    }
    public void Unpatch()
    {
        if (Harmony.HasAnyPatches("levelup_platearmor"))
        {
            patch.UnpatchCategory("levelup_platearmor");
        }
    }

    public void Init()
    {
        Configuration.RegisterNewLevel("PlateArmor");
        Configuration.RegisterNewLevelTypeEXP("PlateArmor", Configuration.PlateArmorGetLevelByEXP);
        Configuration.RegisterNewEXPLevelType("PlateArmor", Configuration.PlateArmorGetExpByLevel);
        // Update armor stats before functions
        OverwriteDamageInteractionEvents.OnPlayerArmorReceiveHandleStats += StatsUpdated;
        OverwriteDamageInteractionEvents.OnPlayerArmorReceiveDamageStat += DamageReceived;

        Debug.Log("Level Plate Armor initialized");
    }

    public void InitClient()
    {
        OverwriteDamageInteractionEvents.OnPlayerArmorViewStats += ViewReceived;

        Debug.Log("Level Plate Armor initialized");
    }

    public void Dispose()
    {
        OverwriteDamageInteractionEvents.OnPlayerArmorReceiveHandleStats -= StatsUpdated;
        OverwriteDamageInteractionEvents.OnPlayerArmorReceiveDamageStat -= DamageReceived;
        OverwriteDamageInteractionEvents.OnPlayerArmorViewStats -= ViewReceived;
    }

    private void ViewReceived(IPlayer player, ItemSlot item)
    {
        if (!Configuration.expMultiplyHitPlateArmor.ContainsKey(item.Itemstack.Collectible.Code)) return;
        float statusIncrease = Configuration.PlateArmorStatsIncreaseByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_PlateArmor"));
        Shared.Instance.RefreshArmorAttributes(item, statusIncrease);
        LevelPlateArmorEvents.ExecuteItemInfoUpdated(item.Itemstack.Item as ItemWearable, player);
    }

    private void StatsUpdated(IPlayer player, List<ItemSlot> items)
    {
        float statusIncrease = Configuration.PlateArmorStatsIncreaseByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_PlateArmor"));
        foreach (ItemSlot armorSlot in items)
        {
            if (!Configuration.expMultiplyHitPlateArmor.ContainsKey(armorSlot.Itemstack.Collectible.Code)) continue;

            ItemWearable armor = armorSlot.Itemstack.Item as ItemWearable;

            Shared.Instance.RefreshArmorAttributes(armorSlot, statusIncrease);

            LevelPlateArmorEvents.ExecuteItemHandledStats(armor, player);
        }
    }

    private void DamageReceived(IPlayer player, List<ItemSlot> items, ref float damage)
    {
        float statusIncrease = Configuration.PlateArmorStatsIncreaseByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_PlateArmor"));
        foreach (ItemSlot armorSlot in items)
        {
            if (!Configuration.expMultiplyHitPlateArmor.ContainsKey(armorSlot.Itemstack.Collectible.Code)) continue;

            ItemWearable armor = armorSlot.Itemstack.Item as ItemWearable;

            double multiply = Configuration.expMultiplyHitPlateArmor[armor.Code];
            ulong exp = (ulong)(Configuration.PlateArmorBaseEXPEarnedByDAMAGE(damage) * multiply);
            Experience.IncreaseExperience(player, "PlateArmor", exp);

            Shared.Instance.RefreshArmorAttributes(armorSlot, statusIncrease);

            LevelPlateArmorEvents.ExecuteItemHandledDamage(armor, player);
        }
    }

    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulatePlateArmorConfiguration(coreAPI);
        if (Configuration.enableLevelPlateArmor)
        {
            Configuration.RegisterNewMaxLevelByLevelTypeEXP("PlateArmor", Configuration.plateArmorMaxLevel);
        }
    }

    [HarmonyPatchCategory("levelup_platearmor")]
    private class PlateArmorPatch
    { }
}

public class LevelPlateArmorEvents
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