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

    public readonly Dictionary<string, string> SubLevelPatterns = new()
    {
        { "-copper-", "Copper" },
        { "-tinbronze-", "TinBronze" },
        { "-bismuthbronze-", "BismuthBronze" },
        { "-blackbronze-", "BlackBronze" },
        { "-iron-", "Iron" },
        { "-meteoriciron-", "MeteoricIron" },
        { "-steel-", "Steel" },
        { "-gold-", "Gold" },
        { "-silver-", "Silver" }
    };

    private void StatusViewRequested(IPlayer player, ref StringBuilder stringBuilder, string levelType)
    {
        if (levelType != "ChainArmor") return;

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_relativeprotection",
                Utils.GetPorcentageFromFloatsStart1(Configuration.ChainArmorRelativeProtectionMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_ChainArmor")))
            )
        );

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_flatdamagereduction",
                Utils.GetPorcentageFromFloatsStart1(Configuration.ChainArmorFlatDamageReductionMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_ChainArmor")))
            )
        );

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_healingeffectivness",
                Utils.GetPorcentageFromFloatsStart1(Configuration.ChainArmorHealingEffectivnessMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_ChainArmor")))
            )
        );

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_hungerrate",
                Utils.GetPorcentageFromFloatsStart1(Configuration.ChainArmorHungerRateMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_ChainArmor")))
            )
        );

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_rangedaccuracy",
                Utils.GetPorcentageFromFloatsStart1(Configuration.ChainArmorRangedWeaponsAccuracyMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_ChainArmor")))
            )
        );

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_rangedspeed",
                Utils.GetPorcentageFromFloatsStart1(Configuration.ChainArmorRangedWeaponsSpeedMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_ChainArmor")))
            )
        );

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_walkspeed",
                Utils.GetPorcentageFromFloatsStart1(Configuration.ChainArmorWalkSpeedMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_ChainArmor")))
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
        if (!Configuration.expMultiplyHitChainArmor.ContainsKey(armorSlot.Itemstack.Collectible.Code)) return;

        int playerLevel = player.Entity.WatchedAttributes.GetInt("LevelUP_Level_ChainArmor");
        float relativeProtection = Configuration.ChainArmorRelativeProtectionMultiplyByLevel(playerLevel);
        float flatDamageReduction = Configuration.ChainArmorFlatDamageReductionMultiplyByLevel(playerLevel);
        float healingEffectivness = Configuration.ChainArmorHealingEffectivnessMultiplyByLevel(playerLevel);
        float hungerRate = Configuration.ChainArmorHungerRateMultiplyByLevel(playerLevel);
        float rangedWeaponsAccuracy = Configuration.ChainArmorRangedWeaponsAccuracyMultiplyByLevel(playerLevel);
        float rangedWeaponsSpeed = Configuration.ChainArmorRangedWeaponsSpeedMultiplyByLevel(playerLevel);
        float walkSpeed = Configuration.ChainArmorWalkSpeedMultiplyByLevel(playerLevel);

        string code = armorSlot.Itemstack.Item.Code.ToString();
        foreach (var pair in SubLevelPatterns)
        {
            if (code.Contains(pair.Key))
            {
                int playerSubLevel = player.Entity.WatchedAttributes.GetInt($"LevelUP_Level_Sub_{pair.Value}");
                relativeProtection += Configuration.ChainArmorRelativeProtectionMultiplyByLevel(playerSubLevel) - 1f;
                flatDamageReduction += Configuration.ChainArmorFlatDamageReductionMultiplyByLevel(playerSubLevel) - 1f;
                healingEffectivness += Configuration.ChainArmorHealingEffectivnessMultiplyByLevel(playerSubLevel) - 1f;
                hungerRate += Configuration.ChainArmorHungerRateMultiplyByLevel(playerSubLevel) - 1f;
                rangedWeaponsAccuracy += Configuration.ChainArmorRangedWeaponsAccuracyMultiplyByLevel(playerSubLevel) - 1f;
                rangedWeaponsSpeed += Configuration.ChainArmorRangedWeaponsSpeedMultiplyByLevel(playerSubLevel) - 1f;
                walkSpeed += Configuration.ChainArmorWalkSpeedMultiplyByLevel(playerSubLevel) - 1f;
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

        LevelChainArmorEvents.ExecuteItemInfoUpdated(armorSlot.Itemstack.Item as ItemWearable, player);
    }

    private void StatsUpdated(IPlayer player, List<ItemSlot> items)
    {
        int playerLevel = player.Entity.WatchedAttributes.GetInt("LevelUP_Level_ChainArmor");

        foreach (ItemSlot armorSlot in items)
        {
            if (!Configuration.expMultiplyHitChainArmor.ContainsKey(armorSlot.Itemstack.Collectible.Code)) continue;

            ItemWearable armor = armorSlot.Itemstack.Item as ItemWearable;

            float relativeProtection = Configuration.ChainArmorRelativeProtectionMultiplyByLevel(playerLevel);
            float flatDamageReduction = Configuration.ChainArmorFlatDamageReductionMultiplyByLevel(playerLevel);
            float healingEffectivness = Configuration.ChainArmorHealingEffectivnessMultiplyByLevel(playerLevel);
            float hungerRate = Configuration.ChainArmorHungerRateMultiplyByLevel(playerLevel);
            float rangedWeaponsAccuracy = Configuration.ChainArmorRangedWeaponsAccuracyMultiplyByLevel(playerLevel);
            float rangedWeaponsSpeed = Configuration.ChainArmorRangedWeaponsSpeedMultiplyByLevel(playerLevel);
            float walkSpeed = Configuration.ChainArmorWalkSpeedMultiplyByLevel(playerLevel);

            string code = armor.Code.ToString();
            foreach (var pair in SubLevelPatterns)
            {
                if (code.Contains(pair.Key))
                {
                    int playerSubLevel = player.Entity.WatchedAttributes.GetInt($"LevelUP_Level_Sub_{pair.Value}");
                    relativeProtection += Configuration.ChainArmorRelativeProtectionMultiplyByLevel(playerSubLevel) - 1f;
                    flatDamageReduction += Configuration.ChainArmorFlatDamageReductionMultiplyByLevel(playerSubLevel) - 1f;
                    healingEffectivness += Configuration.ChainArmorHealingEffectivnessMultiplyByLevel(playerSubLevel) - 1f;
                    hungerRate += Configuration.ChainArmorHungerRateMultiplyByLevel(playerSubLevel) - 1f;
                    rangedWeaponsAccuracy += Configuration.ChainArmorRangedWeaponsAccuracyMultiplyByLevel(playerSubLevel) - 1f;
                    rangedWeaponsSpeed += Configuration.ChainArmorRangedWeaponsSpeedMultiplyByLevel(playerSubLevel) - 1f;
                    walkSpeed += Configuration.ChainArmorWalkSpeedMultiplyByLevel(playerSubLevel) - 1f;
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

            LevelChainArmorEvents.ExecuteItemHandledStats(armor, player);
        }
    }

    private void DamageReceived(IPlayer player, List<ItemSlot> items, ref float damage)
    {
        int playerLevel = player.Entity.WatchedAttributes.GetInt("LevelUP_Level_ChainArmor");
        foreach (ItemSlot armorSlot in items)
        {
            if (!Configuration.expMultiplyHitChainArmor.ContainsKey(armorSlot.Itemstack.Collectible.Code)) continue;

            ItemWearable armor = armorSlot.Itemstack.Item as ItemWearable;

            float relativeProtection = Configuration.ChainArmorRelativeProtectionMultiplyByLevel(playerLevel);
            float flatDamageReduction = Configuration.ChainArmorFlatDamageReductionMultiplyByLevel(playerLevel);

            double multiply = Configuration.expMultiplyHitChainArmor[armor.Code];
            ulong exp = (ulong)(Configuration.ChainArmorBaseEXPEarnedByDAMAGE(damage) * multiply);
            Experience.IncreaseExperience(player, "ChainArmor", exp);
            string code = armor.Code.ToString();
            foreach (var pair in SubLevelPatterns)
            {
                if (code.Contains(pair.Key))
                {
                    int playerSubLevel = player.Entity.WatchedAttributes.GetInt($"LevelUP_Level_Sub_{pair.Value}");
                    relativeProtection += Configuration.ChainArmorRelativeProtectionMultiplyByLevel(playerSubLevel) - 1f;
                    flatDamageReduction += Configuration.ChainArmorFlatDamageReductionMultiplyByLevel(playerSubLevel) - 1f;
                    Experience.IncreaseSubExperience(player, "ChainArmor", pair.Value, exp);
                    break;
                }
            }

            Shared.Instance.RefreshArmorAttributes(armorSlot, relativeProtection, flatDamageReduction);

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