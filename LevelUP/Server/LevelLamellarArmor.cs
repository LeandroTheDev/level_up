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

    public readonly Dictionary<string, string> SubLevelPatterns = new()
    {
        { "-wood-", "Wood" },
        { "-copper-", "Copper" },
        { "-tinbronze-", "TinBronze" },
        { "-bismuthbronze-", "BismuthBronze" },
        { "-blackbronze-", "BlackBronze" }
    };

    private void StatusViewRequested(IPlayer player, ref StringBuilder stringBuilder, string levelType)
    {
        if (levelType != "LamellarArmor") return;

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_relativeprotection",
                Utils.GetPorcentageFromFloatsStart1(Configuration.LamellarArmorRelativeProtectionMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_LamellarArmor")))
            )
        );

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_flatdamagereduction",
                Utils.GetPorcentageFromFloatsStart1(Configuration.LamellarArmorFlatDamageReductionMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_LamellarArmor")))
            )
        );

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_healingeffectivness",
                Utils.GetPorcentageFromFloatsStart1(Configuration.LamellarArmorHealingEffectivnessMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_LamellarArmor")))
            )
        );

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_hungerrate",
                Utils.GetPorcentageFromFloatsStart1(Configuration.LamellarArmorHungerRateMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_LamellarArmor")))
            )
        );

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_rangedaccuracy",
                Utils.GetPorcentageFromFloatsStart1(Configuration.LamellarArmorRangedWeaponsAccuracyMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_LamellarArmor")))
            )
        );

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_rangedspeed",
                Utils.GetPorcentageFromFloatsStart1(Configuration.LamellarArmorRangedWeaponsSpeedMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_LamellarArmor")))
            )
        );

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_walkspeed",
                Utils.GetPorcentageFromFloatsStart1(Configuration.LamellarArmorWalkSpeedMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_LamellarArmor")))
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
        if (!Configuration.expMultiplyHitLamellarArmor.ContainsKey(armorSlot.Itemstack.Collectible.Code)) return;

        int playerLevel = player.Entity.WatchedAttributes.GetInt("LevelUP_Level_LamellarArmor");
        float relativeProtection = Configuration.LamellarArmorRelativeProtectionMultiplyByLevel(playerLevel);
        float flatDamageReduction = Configuration.LamellarArmorFlatDamageReductionMultiplyByLevel(playerLevel);
        float healingEffectivness = Configuration.LamellarArmorHealingEffectivnessMultiplyByLevel(playerLevel);
        float hungerRate = Configuration.LamellarArmorHungerRateMultiplyByLevel(playerLevel);
        float rangedWeaponsAccuracy = Configuration.LamellarArmorRangedWeaponsAccuracyMultiplyByLevel(playerLevel);
        float rangedWeaponsSpeed = Configuration.LamellarArmorRangedWeaponsSpeedMultiplyByLevel(playerLevel);
        float walkSpeed = Configuration.LamellarArmorWalkSpeedMultiplyByLevel(playerLevel);

        string code = armorSlot.Itemstack.Item.Code.ToString();
        foreach (var pair in SubLevelPatterns)
        {
            if (code.Contains(pair.Key))
            {
                int playerSubLevel = player.Entity.WatchedAttributes.GetInt($"LevelUP_Level_Sub_{pair.Value}");
                relativeProtection += Configuration.LamellarArmorRelativeProtectionMultiplyByLevel(playerSubLevel) - 1f;
                flatDamageReduction += Configuration.LamellarArmorFlatDamageReductionMultiplyByLevel(playerSubLevel) - 1f;
                healingEffectivness += Configuration.LamellarArmorHealingEffectivnessMultiplyByLevel(playerSubLevel) - 1f;
                hungerRate += Configuration.LamellarArmorHungerRateMultiplyByLevel(playerSubLevel) - 1f;
                rangedWeaponsAccuracy += Configuration.LamellarArmorRangedWeaponsAccuracyMultiplyByLevel(playerSubLevel) - 1f;
                rangedWeaponsSpeed += Configuration.LamellarArmorRangedWeaponsSpeedMultiplyByLevel(playerSubLevel) - 1f;
                walkSpeed += Configuration.LamellarArmorWalkSpeedMultiplyByLevel(playerSubLevel) - 1f;
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

        LevelLamellarArmorEvents.ExecuteItemInfoUpdated(armorSlot.Itemstack.Item as ItemWearable, player);
    }

    private void StatsUpdated(IPlayer player, List<ItemSlot> items)
    {
        int playerLevel = player.Entity.WatchedAttributes.GetInt("LevelUP_Level_LamellarArmor");

        foreach (ItemSlot armorSlot in items)
        {
            if (!Configuration.expMultiplyHitLamellarArmor.ContainsKey(armorSlot.Itemstack.Collectible.Code)) continue;

            ItemWearable armor = armorSlot.Itemstack.Item as ItemWearable;

            float relativeProtection = Configuration.LamellarArmorRelativeProtectionMultiplyByLevel(playerLevel);
            float flatDamageReduction = Configuration.LamellarArmorFlatDamageReductionMultiplyByLevel(playerLevel);
            float healingEffectivness = Configuration.LamellarArmorHealingEffectivnessMultiplyByLevel(playerLevel);
            float hungerRate = Configuration.LamellarArmorHungerRateMultiplyByLevel(playerLevel);
            float rangedWeaponsAccuracy = Configuration.LamellarArmorRangedWeaponsAccuracyMultiplyByLevel(playerLevel);
            float rangedWeaponsSpeed = Configuration.LamellarArmorRangedWeaponsSpeedMultiplyByLevel(playerLevel);
            float walkSpeed = Configuration.LamellarArmorWalkSpeedMultiplyByLevel(playerLevel);

            string code = armor.Code.ToString();
            foreach (var pair in SubLevelPatterns)
            {
                if (code.Contains(pair.Key))
                {
                    int playerSubLevel = player.Entity.WatchedAttributes.GetInt($"LevelUP_Level_Sub_{pair.Value}");
                    relativeProtection += Configuration.LamellarArmorRelativeProtectionMultiplyByLevel(playerSubLevel) - 1f;
                    flatDamageReduction += Configuration.LamellarArmorFlatDamageReductionMultiplyByLevel(playerSubLevel) - 1f;
                    healingEffectivness += Configuration.LamellarArmorHealingEffectivnessMultiplyByLevel(playerSubLevel) - 1f;
                    hungerRate += Configuration.LamellarArmorHungerRateMultiplyByLevel(playerSubLevel) - 1f;
                    rangedWeaponsAccuracy += Configuration.LamellarArmorRangedWeaponsAccuracyMultiplyByLevel(playerSubLevel) - 1f;
                    rangedWeaponsSpeed += Configuration.LamellarArmorRangedWeaponsSpeedMultiplyByLevel(playerSubLevel) - 1f;
                    walkSpeed += Configuration.LamellarArmorWalkSpeedMultiplyByLevel(playerSubLevel) - 1f;
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

            LevelLamellarArmorEvents.ExecuteItemHandledStats(armor, player);
        }
    }

    private void DamageReceived(IPlayer player, List<ItemSlot> items, ref float damage)
    {
        int playerLevel = player.Entity.WatchedAttributes.GetInt("LevelUP_Level_LamellarArmor");
        foreach (ItemSlot armorSlot in items)
        {
            if (!Configuration.expMultiplyHitLamellarArmor.ContainsKey(armorSlot.Itemstack.Collectible.Code)) continue;

            ItemWearable armor = armorSlot.Itemstack.Item as ItemWearable;

            float relativeProtection = Configuration.LamellarArmorRelativeProtectionMultiplyByLevel(playerLevel);
            float flatDamageReduction = Configuration.LamellarArmorFlatDamageReductionMultiplyByLevel(playerLevel);

            double multiply = Configuration.expMultiplyHitLamellarArmor[armor.Code];
            ulong exp = (ulong)(Configuration.LamellarArmorBaseEXPEarnedByDAMAGE(damage) * multiply);
            Experience.IncreaseExperience(player, "LamellarArmor", exp);
            string code = armor.Code.ToString();
            foreach (var pair in SubLevelPatterns)
            {
                if (code.Contains(pair.Key))
                {
                    int playerSubLevel = player.Entity.WatchedAttributes.GetInt($"LevelUP_Level_Sub_{pair.Value}");
                    relativeProtection += Configuration.LamellarArmorRelativeProtectionMultiplyByLevel(playerSubLevel) - 1f;
                    flatDamageReduction += Configuration.LamellarArmorFlatDamageReductionMultiplyByLevel(playerSubLevel) - 1f;
                    Experience.IncreaseSubExperience(player, "LamellarArmor", pair.Value, exp);
                    break;
                }
            }

            Shared.Instance.RefreshArmorAttributes(armorSlot, relativeProtection, flatDamageReduction);

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