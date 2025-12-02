#pragma warning disable CA1822
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using LevelUP.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
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
        OverwriteDamageInteractionEvents.OnPlayerArmorReceiveHandleStats += StatsUpdated;
        OverwriteDamageInteractionEvents.OnPlayerArmorReceiveDamageStat += DamageReceived;

        Debug.Log("Level Plate Armor initialized");
    }

    public void InitClient()
    {
        OverwriteDamageInteractionEvents.OnPlayerArmorViewStats += ViewReceived;
        StatusViewEvents.OnStatusRequested += StatusViewRequested;

        Debug.Log("Level Plate Armor initialized");
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
        { "-copper-", "Copper" },
        { "-tinbronze-", "TinBronze" },
        { "-bismuthbronze-", "BismuthBronze" },
        { "-blackbronze-", "Blackbronze" },
        { "-iron-", "Iron" },
        { "-meteoriciron-", "MeteoricIron" },
        { "-steel-", "Steel" },
        { "-gold-", "Gold" },
        { "-silver-", "Silver" }
    };

    private void StatusViewRequested(IPlayer player, ref StringBuilder stringBuilder, string levelType)
    {
        if (levelType != "PlateArmor") return;

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_relativeprotection",
                Utils.GetPorcentageFromFloatsStart1(Configuration.PlateArmorRelativeProtectionMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_PlateArmor")))
            )
        );

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_flatdamagereduction",
                Utils.GetPorcentageFromFloatsStart1(Configuration.PlateArmorFlatDamageReductionMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_PlateArmor")))
            )
        );

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_healingeffectivness",
                Utils.GetPorcentageFromFloatsStart1(Configuration.PlateArmorHealingEffectivnessMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_PlateArmor")))
            )
        );

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_hungerrate",
                Utils.GetPorcentageFromFloatsStart1(Configuration.PlateArmorHungerRateMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_PlateArmor")))
            )
        );

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_rangedaccuracy",
                Utils.GetPorcentageFromFloatsStart1(Configuration.PlateArmorRangedWeaponsAccuracyMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_PlateArmor")))
            )
        );

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_rangedspeed",
                Utils.GetPorcentageFromFloatsStart1(Configuration.PlateArmorRangedWeaponsSpeedMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_PlateArmor")))
            )
        );

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_walkspeed",
                Utils.GetPorcentageFromFloatsStart1(Configuration.PlateArmorWalkSpeedMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_PlateArmor")))
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
        if (!Configuration.expMultiplyHitPlateArmor.ContainsKey(armorSlot.Itemstack.Collectible.Code)) return;

        int playerLevel = player.Entity.WatchedAttributes.GetInt("LevelUP_Level_PlateArmor");
        float relativeProtection = Configuration.PlateArmorRelativeProtectionMultiplyByLevel(playerLevel);
        float flatDamageReduction = Configuration.PlateArmorFlatDamageReductionMultiplyByLevel(playerLevel);
        float healingEffectivness = Configuration.PlateArmorHealingEffectivnessMultiplyByLevel(playerLevel);
        float hungerRate = Configuration.PlateArmorHungerRateMultiplyByLevel(playerLevel);
        float rangedWeaponsAccuracy = Configuration.PlateArmorRangedWeaponsAccuracyMultiplyByLevel(playerLevel);
        float rangedWeaponsSpeed = Configuration.PlateArmorRangedWeaponsSpeedMultiplyByLevel(playerLevel);
        float walkSpeed = Configuration.PlateArmorWalkSpeedMultiplyByLevel(playerLevel);

        string code = armorSlot.Itemstack.Item.Code.ToString();
        foreach (var pair in SubLevelPatterns)
        {
            if (code.Contains(pair.Key))
            {
                int playerSubLevel = player.Entity.WatchedAttributes.GetInt($"LevelUP_Level_Sub_{pair.Value}");
                relativeProtection += Configuration.PlateArmorRelativeProtectionMultiplyByLevel(playerSubLevel) - 1f;
                flatDamageReduction += Configuration.PlateArmorFlatDamageReductionMultiplyByLevel(playerSubLevel) - 1f;
                healingEffectivness += Configuration.PlateArmorHealingEffectivnessMultiplyByLevel(playerSubLevel) - 1f;
                hungerRate += Configuration.PlateArmorHungerRateMultiplyByLevel(playerSubLevel) - 1f;
                rangedWeaponsAccuracy += Configuration.PlateArmorRangedWeaponsAccuracyMultiplyByLevel(playerSubLevel) - 1f;
                rangedWeaponsSpeed += Configuration.PlateArmorRangedWeaponsSpeedMultiplyByLevel(playerSubLevel) - 1f;
                walkSpeed += Configuration.PlateArmorWalkSpeedMultiplyByLevel(playerSubLevel) - 1f;
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

        LevelPlateArmorEvents.ExecuteItemInfoUpdated(armorSlot.Itemstack.Item as ItemWearable, player);
    }

    private void StatsUpdated(IPlayer player, List<ItemSlot> items)
    {
        int playerLevel = player.Entity.WatchedAttributes.GetInt("LevelUP_Level_PlateArmor");

        foreach (ItemSlot armorSlot in items)
        {
            if (!Configuration.expMultiplyHitPlateArmor.ContainsKey(armorSlot.Itemstack.Collectible.Code)) continue;

            ItemWearable armor = armorSlot.Itemstack.Item as ItemWearable;

            float relativeProtection = Configuration.PlateArmorRelativeProtectionMultiplyByLevel(playerLevel);
            float flatDamageReduction = Configuration.PlateArmorFlatDamageReductionMultiplyByLevel(playerLevel);
            float healingEffectivness = Configuration.PlateArmorHealingEffectivnessMultiplyByLevel(playerLevel);
            float hungerRate = Configuration.PlateArmorHungerRateMultiplyByLevel(playerLevel);
            float rangedWeaponsAccuracy = Configuration.PlateArmorRangedWeaponsAccuracyMultiplyByLevel(playerLevel);
            float rangedWeaponsSpeed = Configuration.PlateArmorRangedWeaponsSpeedMultiplyByLevel(playerLevel);
            float walkSpeed = Configuration.PlateArmorWalkSpeedMultiplyByLevel(playerLevel);

            string code = armor.Code.ToString();
            foreach (var pair in SubLevelPatterns)
            {
                if (code.Contains(pair.Key))
                {
                    int playerSubLevel = player.Entity.WatchedAttributes.GetInt($"LevelUP_Level_Sub_{pair.Value}");
                    relativeProtection += Configuration.PlateArmorRelativeProtectionMultiplyByLevel(playerSubLevel) - 1f;
                    flatDamageReduction += Configuration.PlateArmorFlatDamageReductionMultiplyByLevel(playerSubLevel) - 1f;
                    healingEffectivness += Configuration.PlateArmorHealingEffectivnessMultiplyByLevel(playerSubLevel) - 1f;
                    hungerRate += Configuration.PlateArmorHungerRateMultiplyByLevel(playerSubLevel) - 1f;
                    rangedWeaponsAccuracy += Configuration.PlateArmorRangedWeaponsAccuracyMultiplyByLevel(playerSubLevel) - 1f;
                    rangedWeaponsSpeed += Configuration.PlateArmorRangedWeaponsSpeedMultiplyByLevel(playerSubLevel) - 1f;
                    walkSpeed += Configuration.PlateArmorWalkSpeedMultiplyByLevel(playerSubLevel) - 1f;
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

            LevelPlateArmorEvents.ExecuteItemHandledStats(armor, player);
        }
    }

    private void DamageReceived(IPlayer player, List<ItemSlot> items, ref float damage)
    {
        int playerLevel = player.Entity.WatchedAttributes.GetInt("LevelUP_Level_PlateArmor");
        foreach (ItemSlot armorSlot in items)
        {
            if (!Configuration.expMultiplyHitPlateArmor.ContainsKey(armorSlot.Itemstack.Collectible.Code)) continue;

            ItemWearable armor = armorSlot.Itemstack.Item as ItemWearable;

            float relativeProtection = Configuration.PlateArmorRelativeProtectionMultiplyByLevel(playerLevel);
            float flatDamageReduction = Configuration.PlateArmorFlatDamageReductionMultiplyByLevel(playerLevel);

            double multiply = Configuration.expMultiplyHitPlateArmor[armor.Code];
            ulong exp = (ulong)(Configuration.PlateArmorBaseEXPEarnedByDAMAGE(damage) * multiply);
            Experience.IncreaseExperience(player, "PlateArmor", exp);
            string code = armor.Code.ToString();
            foreach (var pair in SubLevelPatterns)
            {
                if (code.Contains(pair.Key))
                {
                    int playerSubLevel = player.Entity.WatchedAttributes.GetInt($"LevelUP_Level_Sub_{pair.Value}");
                    relativeProtection += Configuration.PlateArmorRelativeProtectionMultiplyByLevel(playerSubLevel) - 1f;
                    flatDamageReduction += Configuration.PlateArmorFlatDamageReductionMultiplyByLevel(playerSubLevel) - 1f;
                    Experience.IncreaseSubExperience(player, "PlateArmor", pair.Value, exp);
                    break;
                }
            }

            Shared.Instance.RefreshArmorAttributes(armorSlot, relativeProtection, flatDamageReduction);

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