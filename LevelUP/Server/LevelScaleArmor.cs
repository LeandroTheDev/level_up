#pragma warning disable CA1822
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using LevelUP.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.GameContent;
namespace LevelUP.Server;

class LevelScaleArmor
{
    public readonly Harmony patch = new("levelup_scalearmor");
    public void Patch()
    {
        if (!Harmony.HasAnyPatches("levelup_scalearmor"))
        {
            patch.PatchCategory("levelup_scalearmor");
        }
    }
    public void Unpatch()
    {
        if (Harmony.HasAnyPatches("levelup_scalearmor"))
        {
            patch.UnpatchCategory("levelup_scalearmor");
        }
    }

    public void Init()
    {
        Configuration.RegisterNewLevel("ScaleArmor");
        Configuration.RegisterNewLevelTypeEXP("ScaleArmor", Configuration.ScaleArmorGetLevelByEXP);
        Configuration.RegisterNewEXPLevelType("ScaleArmor", Configuration.ScaleArmorGetExpByLevel);
        OverwriteDamageInteractionEvents.OnPlayerArmorReceiveHandleStats += StatsUpdated;
        OverwriteDamageInteractionEvents.OnPlayerArmorReceiveDamageStat += DamageReceived;

        Debug.Log("Level Scale Armor initialized");
    }

    public void InitClient()
    {
        OverwriteDamageInteractionEvents.OnPlayerArmorViewStats += ViewReceived;
        StatusViewEvents.OnStatusRequested += StatusViewRequested;

        Debug.Log("Level Scale Armor initialized");
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
        if (levelType != "ScaleArmor") return;

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_relativeprotection",
                Utils.GetPorcentageFromFloatsStart1(Configuration.ScaleArmorRelativeProtectionMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_ScaleArmor")))
            )
        );

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_flatdamagereduction",
                Utils.GetPorcentageFromFloatsStart1(Configuration.ScaleArmorFlatDamageReductionMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_ScaleArmor")))
            )
        );

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_healingeffectivness",
                Utils.GetPorcentageFromFloatsStart1(Configuration.ScaleArmorHealingEffectivnessMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_ScaleArmor")))
            )
        );

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_hungerrate",
                Utils.GetPorcentageFromFloatsStart1(Configuration.ScaleArmorHungerRateMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_ScaleArmor")))
            )
        );

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_rangedaccuracy",
                Utils.GetPorcentageFromFloatsStart1(Configuration.ScaleArmorRangedWeaponsAccuracyMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_ScaleArmor")))
            )
        );

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_rangedspeed",
                Utils.GetPorcentageFromFloatsStart1(Configuration.ScaleArmorRangedWeaponsSpeedMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_ScaleArmor")))
            )
        );

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_walkspeed",
                Utils.GetPorcentageFromFloatsStart1(Configuration.ScaleArmorWalkSpeedMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_ScaleArmor")))
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
        if (!Configuration.expMultiplyHitScaleArmor.ContainsKey(armorSlot.Itemstack.Collectible.Code)) return;

        int playerLevel = player.Entity.WatchedAttributes.GetInt("LevelUP_Level_ScaleArmor");
        float relativeProtection = Configuration.ScaleArmorRelativeProtectionMultiplyByLevel(playerLevel);
        float flatDamageReduction = Configuration.ScaleArmorFlatDamageReductionMultiplyByLevel(playerLevel);
        float healingEffectivness = Configuration.ScaleArmorHealingEffectivnessMultiplyByLevel(playerLevel);
        float hungerRate = Configuration.ScaleArmorHungerRateMultiplyByLevel(playerLevel);
        float rangedWeaponsAccuracy = Configuration.ScaleArmorRangedWeaponsAccuracyMultiplyByLevel(playerLevel);
        float rangedWeaponsSpeed = Configuration.ScaleArmorRangedWeaponsSpeedMultiplyByLevel(playerLevel);
        float walkSpeed = Configuration.ScaleArmorWalkSpeedMultiplyByLevel(playerLevel);

        string code = armorSlot.Itemstack.Item.Code.ToString();
        foreach (var pair in SubLevelPatterns)
        {
            if (code.Contains(pair.Key))
            {
                int playerSubLevel = player.Entity.WatchedAttributes.GetInt($"LevelUP_Level_Sub_{pair.Value}");
                relativeProtection += Configuration.ScaleArmorRelativeProtectionMultiplyByLevel(playerSubLevel) - 1f;
                flatDamageReduction += Configuration.ScaleArmorFlatDamageReductionMultiplyByLevel(playerSubLevel) - 1f;
                healingEffectivness += Configuration.ScaleArmorHealingEffectivnessMultiplyByLevel(playerSubLevel) - 1f;
                hungerRate += Configuration.ScaleArmorHungerRateMultiplyByLevel(playerSubLevel) - 1f;
                rangedWeaponsAccuracy += Configuration.ScaleArmorRangedWeaponsAccuracyMultiplyByLevel(playerSubLevel) - 1f;
                rangedWeaponsSpeed += Configuration.ScaleArmorRangedWeaponsSpeedMultiplyByLevel(playerSubLevel) - 1f;
                walkSpeed += Configuration.ScaleArmorWalkSpeedMultiplyByLevel(playerSubLevel) - 1f;
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

        LevelScaleArmorEvents.ExecuteItemInfoUpdated(armorSlot.Itemstack.Item as ItemWearable, player);
    }

    private void StatsUpdated(IPlayer player, List<ItemSlot> items)
    {
        int playerLevel = player.Entity.WatchedAttributes.GetInt("LevelUP_Level_ScaleArmor");

        foreach (ItemSlot armorSlot in items)
        {
            if (!Configuration.expMultiplyHitScaleArmor.ContainsKey(armorSlot.Itemstack.Collectible.Code)) continue;

            ItemWearable armor = armorSlot.Itemstack.Item as ItemWearable;

            float relativeProtection = Configuration.ScaleArmorRelativeProtectionMultiplyByLevel(playerLevel);
            float flatDamageReduction = Configuration.ScaleArmorFlatDamageReductionMultiplyByLevel(playerLevel);
            float healingEffectivness = Configuration.ScaleArmorHealingEffectivnessMultiplyByLevel(playerLevel);
            float hungerRate = Configuration.ScaleArmorHungerRateMultiplyByLevel(playerLevel);
            float rangedWeaponsAccuracy = Configuration.ScaleArmorRangedWeaponsAccuracyMultiplyByLevel(playerLevel);
            float rangedWeaponsSpeed = Configuration.ScaleArmorRangedWeaponsSpeedMultiplyByLevel(playerLevel);
            float walkSpeed = Configuration.ScaleArmorWalkSpeedMultiplyByLevel(playerLevel);

            string code = armor.Code.ToString();
            foreach (var pair in SubLevelPatterns)
            {
                if (code.Contains(pair.Key))
                {
                    int playerSubLevel = player.Entity.WatchedAttributes.GetInt($"LevelUP_Level_Sub_{pair.Value}");
                    relativeProtection += Configuration.ScaleArmorRelativeProtectionMultiplyByLevel(playerSubLevel) - 1f;
                    flatDamageReduction += Configuration.ScaleArmorFlatDamageReductionMultiplyByLevel(playerSubLevel) - 1f;
                    healingEffectivness += Configuration.ScaleArmorHealingEffectivnessMultiplyByLevel(playerSubLevel) - 1f;
                    hungerRate += Configuration.ScaleArmorHungerRateMultiplyByLevel(playerSubLevel) - 1f;
                    rangedWeaponsAccuracy += Configuration.ScaleArmorRangedWeaponsAccuracyMultiplyByLevel(playerSubLevel) - 1f;
                    rangedWeaponsSpeed += Configuration.ScaleArmorRangedWeaponsSpeedMultiplyByLevel(playerSubLevel) - 1f;
                    walkSpeed += Configuration.ScaleArmorWalkSpeedMultiplyByLevel(playerSubLevel) - 1f;
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

            LevelScaleArmorEvents.ExecuteItemHandledStats(armor, player);
        }
    }

    private void DamageReceived(IPlayer player, List<ItemSlot> items, ref float damage)
    {
        int playerLevel = player.Entity.WatchedAttributes.GetInt("LevelUP_Level_ScaleArmor");
        foreach (ItemSlot armorSlot in items)
        {
            if (!Configuration.expMultiplyHitScaleArmor.ContainsKey(armorSlot.Itemstack.Collectible.Code)) continue;

            ItemWearable armor = armorSlot.Itemstack.Item as ItemWearable;

            float relativeProtection = Configuration.ScaleArmorRelativeProtectionMultiplyByLevel(playerLevel);
            float flatDamageReduction = Configuration.ScaleArmorFlatDamageReductionMultiplyByLevel(playerLevel);

            double multiply = Configuration.expMultiplyHitScaleArmor[armor.Code];
            ulong exp = (ulong)(Configuration.ScaleArmorBaseEXPEarnedByDAMAGE(damage) * multiply);
            Experience.IncreaseExperience(player, "ScaleArmor", exp);
            string code = armor.Code.ToString();
            foreach (var pair in SubLevelPatterns)
            {
                if (code.Contains(pair.Key))
                {
                    int playerSubLevel = player.Entity.WatchedAttributes.GetInt($"LevelUP_Level_Sub_{pair.Value}");
                    relativeProtection += Configuration.ScaleArmorRelativeProtectionMultiplyByLevel(playerSubLevel) - 1f;
                    flatDamageReduction += Configuration.ScaleArmorFlatDamageReductionMultiplyByLevel(playerSubLevel) - 1f;
                    Experience.IncreaseSubExperience(player, "ScaleArmor", pair.Value, exp);
                    break;
                }
            }

            Shared.Instance.RefreshArmorAttributes(armorSlot, relativeProtection, flatDamageReduction);

            LevelScaleArmorEvents.ExecuteItemHandledDamage(armor, player);
        }
    }

    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulateScaleArmorConfiguration(coreAPI);
        if (Configuration.enableLevelScaleArmor)
        {
            Configuration.RegisterNewMaxLevelByLevelTypeEXP("ScaleArmor", Configuration.scaleArmorMaxLevel);
        }
    }

    [HarmonyPatchCategory("levelup_scalearmor")]
    private class ScaleArmorPatch
    { }
}

public class LevelScaleArmorEvents
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