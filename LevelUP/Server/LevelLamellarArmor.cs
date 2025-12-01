#pragma warning disable CA1822
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using LevelUP.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.GameContent;
namespace LevelUP.Server;

class LevelLamellarArmor
{
    public readonly Harmony patch = new("levelup_lamellararmor");
    public void Patch()
    {
        if (!Harmony.HasAnyPatches("levelup_lamellararmor"))
        {
            patch.PatchCategory("levelup_lamellararmor");
        }
    }
    public void Unpatch()
    {
        if (Harmony.HasAnyPatches("levelup_lamellararmor"))
        {
            patch.UnpatchCategory("levelup_lamellararmor");
        }
    }

    public void Init()
    {
        Configuration.RegisterNewLevel("LamellarArmor");
        Configuration.RegisterNewLevelTypeEXP("LamellarArmor", Configuration.LamellarArmorGetLevelByEXP);
        Configuration.RegisterNewEXPLevelType("LamellarArmor", Configuration.LamellarArmorGetExpByLevel);
        OverwriteDamageInteractionEvents.OnPlayerArmorReceiveHandleStats += StatsUpdated;
        OverwriteDamageInteractionEvents.OnPlayerArmorReceiveDamageStat += DamageReceived;

        Debug.Log("Level Lamellar Armor initialized");
    }

    public void InitClient()
    {
        OverwriteDamageInteractionEvents.OnPlayerArmorViewStats += ViewReceived;
        StatusViewEvents.OnStatusRequested += StatusViewRequested;

        Debug.Log("Level Lamellar Armor initialized");
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
        if (levelType != "LamellarArmor") return;

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_statsincreaser",
                Utils.GetPorcentageFromFloatsStart1(Configuration.LamellarArmorStatsIncreaseByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_LamellarArmor")))
            )
        );
    }

    private void ViewReceived(IPlayer player, ItemSlot item)
    {
        if (!Configuration.expMultiplyHitLamellarArmor.ContainsKey(item.Itemstack.Collectible.Code)) return;
        float statusIncrease = Configuration.LamellarArmorStatsIncreaseByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_LamellarArmor"));
        Shared.Instance.RefreshArmorAttributes(item, statusIncrease);
        LevelLamellarArmorEvents.ExecuteItemInfoUpdated(item.Itemstack.Item as ItemWearable, player);
    }

    private void StatsUpdated(IPlayer player, List<ItemSlot> items)
    {
        float statusIncrease = Configuration.LamellarArmorStatsIncreaseByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_LamellarArmor"));
        foreach (ItemSlot armorSlot in items)
        {
            if (!Configuration.expMultiplyHitLamellarArmor.ContainsKey(armorSlot.Itemstack.Collectible.Code)) continue;

            ItemWearable armor = armorSlot.Itemstack.Item as ItemWearable;

            Shared.Instance.RefreshArmorAttributes(armorSlot, statusIncrease);

            LevelLamellarArmorEvents.ExecuteItemHandledStats(armor, player);
        }
    }

    private void DamageReceived(IPlayer player, List<ItemSlot> items, ref float damage)
    {
        float statusIncrease = Configuration.LamellarArmorStatsIncreaseByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_LamellarArmor"));
        foreach (ItemSlot armorSlot in items)
        {
            if (!Configuration.expMultiplyHitLamellarArmor.ContainsKey(armorSlot.Itemstack.Collectible.Code)) continue;

            ItemWearable armor = armorSlot.Itemstack.Item as ItemWearable;

            double multiply = Configuration.expMultiplyHitLamellarArmor[armor.Code];
            ulong exp = (ulong)(Configuration.LamellarArmorBaseEXPEarnedByDAMAGE(damage) * multiply);
            Experience.IncreaseExperience(player, "LamellarArmor", exp);

            Shared.Instance.RefreshArmorAttributes(armorSlot, statusIncrease);

            LevelLamellarArmorEvents.ExecuteItemHandledDamage(armor, player);
        }
    }

    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulateLamellarArmorConfiguration(coreAPI);
        if (Configuration.enableLevelLamellarArmor)
        {
            Configuration.RegisterNewMaxLevelByLevelTypeEXP("LamellarArmor", Configuration.lamellarArmorMaxLevel);
        }
    }

    [HarmonyPatchCategory("levelup_lamellararmor")]
    private class LamellarArmorPatch
    { }
}

public class LevelLamellarArmorEvents
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