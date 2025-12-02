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

    public readonly Dictionary<string, string> SubLevelPatterns = new()
    {
        { "-sewn-", "Sewn" },
        { "-jerkin-", "Jerkin" },
        { "-bear-", "Bear" }
    };

    private void StatusViewRequested(IPlayer player, ref StringBuilder stringBuilder, string levelType)
    {
        if (levelType != "LeatherArmor") return;

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_relativeprotection",
                Utils.GetPorcentageFromFloatsStart1(Configuration.LeatherArmorRelativeProtectionMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_LeatherArmor")))
            )
        );

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_flatdamagereduction",
                Utils.GetPorcentageFromFloatsStart1(Configuration.LeatherArmorFlatDamageReductionMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_LeatherArmor")))
            )
        );

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_healingeffectivness",
                Utils.GetPorcentageFromFloatsStart1(Configuration.LeatherArmorHealingEffectivnessMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_LeatherArmor")))
            )
        );

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_hungerrate",
                Utils.GetPorcentageFromFloatsStart1(Configuration.LeatherArmorHungerRateMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_LeatherArmor")))
            )
        );

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_rangedaccuracy",
                Utils.GetPorcentageFromFloatsStart1(Configuration.LeatherArmorRangedWeaponsAccuracyMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_LeatherArmor")))
            )
        );

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_rangedspeed",
                Utils.GetPorcentageFromFloatsStart1(Configuration.LeatherArmorRangedWeaponsSpeedMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_LeatherArmor")))
            )
        );

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_walkspeed",
                Utils.GetPorcentageFromFloatsStart1(Configuration.LeatherArmorWalkSpeedMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_LeatherArmor")))
            )
        );

        stringBuilder.AppendLine("");

        stringBuilder.AppendLine(Lang.Get("levelup:status_proficiency"));

        foreach (var pair in SubLevelPatterns)
        {
            stringBuilder.AppendLine($"{Lang.Get($"levelup:{pair.Value.ToLower()}")}: {player.Entity.WatchedAttributes.GetInt($"LevelUP_Level_Sub_{pair.Value}")}");
        }
    }

    private void ViewReceived(IPlayer player, ItemSlot armorSlot)
    {
        if (!Configuration.expMultiplyHitLeatherArmor.ContainsKey(armorSlot.Itemstack.Collectible.Code)) return;

        int playerLevel = player.Entity.WatchedAttributes.GetInt("LevelUP_Level_LeatherArmor");
        float relativeProtection = Configuration.LeatherArmorRelativeProtectionMultiplyByLevel(playerLevel);
        float flatDamageReduction = Configuration.LeatherArmorFlatDamageReductionMultiplyByLevel(playerLevel);
        float healingEffectivness = Configuration.LeatherArmorHealingEffectivnessMultiplyByLevel(playerLevel);
        float hungerRate = Configuration.LeatherArmorHungerRateMultiplyByLevel(playerLevel);
        float rangedWeaponsAccuracy = Configuration.LeatherArmorRangedWeaponsAccuracyMultiplyByLevel(playerLevel);
        float rangedWeaponsSpeed = Configuration.LeatherArmorRangedWeaponsSpeedMultiplyByLevel(playerLevel);
        float walkSpeed = Configuration.LeatherArmorWalkSpeedMultiplyByLevel(playerLevel);

        string code = armorSlot.Itemstack.Item.Code.ToString();
        foreach (var pair in SubLevelPatterns)
        {
            if (code.Contains(pair.Key))
            {
                int playerSubLevel = player.Entity.WatchedAttributes.GetInt($"LevelUP_Level_Sub_{pair.Value}");
                relativeProtection += Configuration.LeatherArmorRelativeProtectionMultiplyByLevel(playerSubLevel) - 1f;
                flatDamageReduction += Configuration.LeatherArmorFlatDamageReductionMultiplyByLevel(playerSubLevel) - 1f;
                healingEffectivness += Configuration.LeatherArmorHealingEffectivnessMultiplyByLevel(playerSubLevel) - 1f;
                hungerRate += Configuration.LeatherArmorHungerRateMultiplyByLevel(playerSubLevel) - 1f;
                rangedWeaponsAccuracy += Configuration.LeatherArmorRangedWeaponsAccuracyMultiplyByLevel(playerSubLevel) - 1f;
                rangedWeaponsSpeed += Configuration.LeatherArmorRangedWeaponsSpeedMultiplyByLevel(playerSubLevel) - 1f;
                walkSpeed += Configuration.LeatherArmorWalkSpeedMultiplyByLevel(playerSubLevel) - 1f;
                break;
            }
        }

        Shared.Instance.RefreshArmorAttributes(
                armorSlot,
                relativeProtection,
                flatDamageReduction,
                healingEffectivness,
                hungerRate,
                rangedWeaponsAccuracy,
                rangedWeaponsSpeed,
                walkSpeed);

        LevelLeatherArmorEvents.ExecuteItemInfoUpdated(armorSlot.Itemstack.Item as ItemWearable, player);
    }

    private void StatsUpdated(IPlayer player, List<ItemSlot> items)
    {
        int playerLevel = player.Entity.WatchedAttributes.GetInt("LevelUP_Level_LeatherArmor");

        foreach (ItemSlot armorSlot in items)
        {
            if (!Configuration.expMultiplyHitLeatherArmor.ContainsKey(armorSlot.Itemstack.Collectible.Code)) continue;

            ItemWearable armor = armorSlot.Itemstack.Item as ItemWearable;

            float relativeProtection = Configuration.LeatherArmorRelativeProtectionMultiplyByLevel(playerLevel);
            float flatDamageReduction = Configuration.LeatherArmorFlatDamageReductionMultiplyByLevel(playerLevel);
            float healingEffectivness = Configuration.LeatherArmorHealingEffectivnessMultiplyByLevel(playerLevel);
            float hungerRate = Configuration.LeatherArmorHungerRateMultiplyByLevel(playerLevel);
            float rangedWeaponsAccuracy = Configuration.LeatherArmorRangedWeaponsAccuracyMultiplyByLevel(playerLevel);
            float rangedWeaponsSpeed = Configuration.LeatherArmorRangedWeaponsSpeedMultiplyByLevel(playerLevel);
            float walkSpeed = Configuration.LeatherArmorWalkSpeedMultiplyByLevel(playerLevel);

            string code = armor.Code.ToString();
            foreach (var pair in SubLevelPatterns)
            {
                if (code.Contains(pair.Key))
                {
                    int playerSubLevel = player.Entity.WatchedAttributes.GetInt($"LevelUP_Level_Sub_{pair.Value}");
                    relativeProtection += Configuration.LeatherArmorRelativeProtectionMultiplyByLevel(playerSubLevel) - 1f;
                    flatDamageReduction += Configuration.LeatherArmorFlatDamageReductionMultiplyByLevel(playerSubLevel) - 1f;
                    healingEffectivness += Configuration.LeatherArmorHealingEffectivnessMultiplyByLevel(playerSubLevel) - 1f;
                    hungerRate += Configuration.LeatherArmorHungerRateMultiplyByLevel(playerSubLevel) - 1f;
                    rangedWeaponsAccuracy += Configuration.LeatherArmorRangedWeaponsAccuracyMultiplyByLevel(playerSubLevel) - 1f;
                    rangedWeaponsSpeed += Configuration.LeatherArmorRangedWeaponsSpeedMultiplyByLevel(playerSubLevel) - 1f;
                    walkSpeed += Configuration.LeatherArmorWalkSpeedMultiplyByLevel(playerSubLevel) - 1f;
                    break;
                }
            }

            Shared.Instance.RefreshArmorAttributes(
                armorSlot,
                relativeProtection,
                flatDamageReduction,
                healingEffectivness,
                hungerRate,
                rangedWeaponsAccuracy,
                rangedWeaponsSpeed,
                walkSpeed);

            LevelLeatherArmorEvents.ExecuteItemHandledStats(armor, player);
        }
    }

    private void DamageReceived(IPlayer player, List<ItemSlot> items, ref float damage)
    {
        int playerLevel = player.Entity.WatchedAttributes.GetInt("LevelUP_Level_LeatherArmor");
        foreach (ItemSlot armorSlot in items)
        {
            if (!Configuration.expMultiplyHitLeatherArmor.ContainsKey(armorSlot.Itemstack.Collectible.Code)) continue;

            ItemWearable armor = armorSlot.Itemstack.Item as ItemWearable;

            float relativeProtection = Configuration.LeatherArmorRelativeProtectionMultiplyByLevel(playerLevel);
            float flatDamageReduction = Configuration.LeatherArmorFlatDamageReductionMultiplyByLevel(playerLevel);

            double multiply = Configuration.expMultiplyHitLeatherArmor[armor.Code];
            ulong exp = (ulong)(Configuration.LeatherArmorBaseEXPEarnedByDAMAGE(damage) * multiply);
            Experience.IncreaseExperience(player, "LeatherArmor", exp);
            string code = armor.Code.ToString();
            foreach (var pair in SubLevelPatterns)
            {
                if (code.Contains(pair.Key))
                {
                    int playerSubLevel = player.Entity.WatchedAttributes.GetInt($"LevelUP_Level_Sub_{pair.Value}");
                    relativeProtection += Configuration.LeatherArmorRelativeProtectionMultiplyByLevel(playerSubLevel) - 1f;
                    flatDamageReduction += Configuration.LeatherArmorFlatDamageReductionMultiplyByLevel(playerSubLevel) - 1f;
                    Experience.IncreaseSubExperience(player, "LeatherArmor", pair.Value, exp);
                    break;
                }
            }

            Shared.Instance.RefreshArmorAttributes(armorSlot, relativeProtection, flatDamageReduction);

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