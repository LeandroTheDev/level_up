#pragma warning disable CA1822
using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.API.Util;
using Vintagestory.Common;
using Vintagestory.GameContent;

namespace LevelUP.Server;

class LevelSmithing
{
    public readonly Harmony patch = new("levelup_smithing");
    public void Patch()
    {
        if (!Harmony.HasAnyPatches("levelup_smithing"))
        {
            patch.PatchCategory("levelup_smithing");
        }
    }
    public void Unpatch()
    {
        if (Harmony.HasAnyPatches("levelup_smithing"))
        {
            patch.UnpatchCategory("levelup_smithing");
        }
    }

    public void Init()
    {
        Configuration.RegisterNewLevel("Smithing");
        Configuration.RegisterNewLevelTypeEXP("Smithing", Configuration.SmithingGetLevelByEXP);
        Configuration.RegisterNewEXPLevelType("Smithing", Configuration.SmithingGetExpByLevel);

        Debug.Log("Level Smithing initialized");
    }

    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulateSmithingConfiguration(coreAPI);
        if (Configuration.enableLevelSmithing)
        {
            Configuration.RegisterNewMaxLevelByLevelTypeEXP("Smithing", Configuration.smithingMaxLevel);
        }
    }

    /// <summary>
    /// Execute the crafting calculations for smithing level
    /// </summary>
    /// <param name="player"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public static ItemStack ExecuteSmithItemCraftedCalculations(IPlayer player, ItemStack item)
    {
        if (item.Attributes.GetInt("durability", item.Collectible.GetMaxDurability(item)) != item.Collectible.GetMaxDurability(item))
        {
            Debug.LogDebug($"Smith item crafted ignored because durability is different");
            return item;
        }
        else if (item.Attributes.GetBool("repaired", false))
        {
            Debug.LogDebug($"Smith item crafted ignored because item is repaired by {item.Attributes.GetString("repaired_by")}");
            return item;
        }

        int? durability = null;
        int? maxDurability = null;
        float? attackPower = null;
        float? miningSpeed = null;

        // Increasing durability based on smithing level
        if (item.Collectible.Durability > 0)
        {
            float multiply = Configuration.SmithingGetDurabilityMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Smithing"));

            durability = (int)Math.Round(item.Collectible.Durability * multiply);
            maxDurability = (int)Math.Round(item.Collectible.GetMaxDurability(item) * multiply);

            Debug.LogDebug($"[Smithing] Craft new durability: {maxDurability}");
        }

        foreach (KeyValuePair<string, int> kvp in Configuration.expPerCraftSmithing)
        {
            string collectableCode = kvp.Key;

            string codeToDetect = null;
            if (collectableCode.Contains('*'))
            {
                if (collectableCode.Contains('?'))
                {
                    codeToDetect = kvp.Key.Split("?")[1].Replace("*", "");
                }
                else
                {
                    codeToDetect = kvp.Key.Replace("*", "");
                }
            }

            if (collectableCode.EndsWith(item.Collectible.Code.ToString()) ||
                (codeToDetect != null && item.Collectible.Code.ToString().Contains(codeToDetect)))
            {
                string levelType = null;
                if (collectableCode.Contains('?'))
                {
                    int questionMarkIndex = collectableCode.IndexOf('?');
                    levelType = questionMarkIndex != -1 ? collectableCode[..questionMarkIndex] : "";
                }

                int exp = kvp.Value;
                { // Getting total experience earned
                    for (int i = 0; i < item.StackSize; i++)
                    {
                        Experience.IncreaseExperience(player, "Smithing", (ulong)exp);

                        Debug.LogDebug($"[Smithing] Craft levelType: {levelType}");

                        // If the levelType is null, is a tool
                        if (levelType == null)
                            // Increasing sub tool levels
                            switch (item.Item.Tool)
                            {
                                case EnumTool.Knife:
                                    Experience.IncreaseSubExperience(player, "Smithing", "Knife", (ulong)exp);
                                    break;
                                case EnumTool.Axe:
                                    Experience.IncreaseSubExperience(player, "Smithing", "Axe", (ulong)exp);
                                    break;
                                case EnumTool.Bow:
                                    Experience.IncreaseSubExperience(player, "Smithing", "Bow", (ulong)exp);
                                    break;
                                case EnumTool.Chisel:
                                    Experience.IncreaseSubExperience(player, "Smithing", "Chisel", (ulong)exp);
                                    break;
                                case EnumTool.Club:
                                    Experience.IncreaseSubExperience(player, "Smithing", "Club", (ulong)exp);
                                    break;
                                case EnumTool.Crossbow:
                                    Experience.IncreaseSubExperience(player, "Smithing", "Crossbow", (ulong)exp);
                                    break;
                                case EnumTool.Drill:
                                    Experience.IncreaseSubExperience(player, "Smithing", "Drill", (ulong)exp);
                                    break;
                                case EnumTool.Firearm:
                                    Experience.IncreaseSubExperience(player, "Smithing", "Firearm", (ulong)exp);
                                    break;
                                case EnumTool.Halberd:
                                    Experience.IncreaseSubExperience(player, "Smithing", "Halberd", (ulong)exp);
                                    break;
                                case EnumTool.Hammer:
                                    Experience.IncreaseSubExperience(player, "Smithing", "Hammer", (ulong)exp);
                                    break;
                                case EnumTool.Hoe:
                                    Experience.IncreaseSubExperience(player, "Smithing", "Hoe", (ulong)exp);
                                    break;
                                case EnumTool.Javelin:
                                    Experience.IncreaseSubExperience(player, "Smithing", "Javelin", (ulong)exp);
                                    break;
                                case EnumTool.Mace:
                                    Experience.IncreaseSubExperience(player, "Smithing", "Mace", (ulong)exp);
                                    break;
                                case EnumTool.Meter:
                                    Experience.IncreaseSubExperience(player, "Smithing", "Meter", (ulong)exp);
                                    break;
                                case EnumTool.Pickaxe:
                                    Experience.IncreaseSubExperience(player, "Smithing", "Pickaxe", (ulong)exp);
                                    break;
                                case EnumTool.Pike:
                                    Experience.IncreaseSubExperience(player, "Smithing", "Pike", (ulong)exp);
                                    break;
                                case EnumTool.Polearm:
                                    Experience.IncreaseSubExperience(player, "Smithing", "Polearm", (ulong)exp);
                                    break;
                                case EnumTool.Poleaxe:
                                    Experience.IncreaseSubExperience(player, "Smithing", "Poleaxe", (ulong)exp);
                                    break;
                                case EnumTool.Probe:
                                    Experience.IncreaseSubExperience(player, "Smithing", "Probe", (ulong)exp);
                                    break;
                                case EnumTool.Saw:
                                    Experience.IncreaseSubExperience(player, "Smithing", "Saw", (ulong)exp);
                                    break;
                                case EnumTool.Scythe:
                                    Experience.IncreaseSubExperience(player, "Smithing", "Scythe", (ulong)exp);
                                    break;
                                case EnumTool.Shears:
                                    Experience.IncreaseSubExperience(player, "Smithing", "Shears", (ulong)exp);
                                    break;
                                case EnumTool.Shield:
                                    Experience.IncreaseSubExperience(player, "Smithing", "Shield", (ulong)exp);
                                    break;
                                case EnumTool.Shovel:
                                    Experience.IncreaseSubExperience(player, "Smithing", "Shovel", (ulong)exp);
                                    break;
                                case EnumTool.Sickle:
                                    Experience.IncreaseSubExperience(player, "Smithing", "Sickle", (ulong)exp);
                                    break;
                                case EnumTool.Sling:
                                    Experience.IncreaseSubExperience(player, "Smithing", "Sling", (ulong)exp);
                                    break;
                                case EnumTool.Spear:
                                    Experience.IncreaseSubExperience(player, "Smithing", "Spear", (ulong)exp);
                                    break;
                                case EnumTool.Staff:
                                    Experience.IncreaseSubExperience(player, "Smithing", "Staff", (ulong)exp);
                                    break;
                                case EnumTool.Sword:
                                    Experience.IncreaseSubExperience(player, "Smithing", "Sword", (ulong)exp);
                                    break;
                                case EnumTool.Warhammer:
                                    Experience.IncreaseSubExperience(player, "Smithing", "Warhammer", (ulong)exp);
                                    break;
                                case EnumTool.Wrench:
                                    Experience.IncreaseSubExperience(player, "Smithing", "Wrench", (ulong)exp);
                                    break;
                            }
                        else // Code with custom level type
                            Experience.IncreaseSubExperience(player, "Smithing", levelType, (ulong)exp);
                    }
                }

                { // Increasing status
                  // If the levelType is null, is a tool
                    if (levelType == null)
                    {
                        // Do not calculate durability in the handle functions
                        // we alredy calculate the durability before

                        void HandleWeapon(IPlayer player, string subLevelType)
                        {
                            { // Main Level Calculation
                                int level = Configuration.SmithingGetLevelByEXP(Experience.GetExperience(player, "Smithing"));
                                attackPower = item.Item.AttackPower * Configuration.SmithingGetAttackPowerMultiplyByLevel(level);
                            }

                            { // Sub Level Calculation
                                int level = Configuration.SmithingGetLevelByEXP(Experience.GetSubExperience(player, "Smithing", subLevelType));
                                float multiply = Configuration.SmithingGetDurabilityMultiplyByLevel(level);
                                if (item.Collectible.Durability > 0)
                                {
                                    durability = (int)Math.Round((int)durability * multiply);
                                    maxDurability = (int)Math.Round((int)maxDurability * multiply);
                                }

                                attackPower *= Configuration.SmithingGetAttackPowerMultiplyByLevel(level);
                            }
                        }

                        void HandleMiningTool(IPlayer player, string subLevelType)
                        {
                            { // Main Level Calculation
                                int level = Configuration.SmithingGetLevelByEXP(Experience.GetExperience(player, "Smithing"));
                                attackPower = item.Item.AttackPower * Configuration.SmithingGetAttackPowerMultiplyByLevel(level);
                                miningSpeed = Configuration.SmithingGetMiningSpeedMultiplyByLevel(level);
                            }

                            { // Sub Level Calculation
                                int level = Configuration.SmithingGetLevelByEXP(Experience.GetSubExperience(player, "Smithing", subLevelType));
                                if (item.Collectible.Durability > 0)
                                {
                                    float multiply = Configuration.SmithingGetDurabilityMultiplyByLevel(level);
                                    durability = (int)Math.Round((int)durability * multiply);
                                    maxDurability = (int)Math.Round((int)maxDurability * multiply);
                                }

                                attackPower *= Configuration.SmithingGetAttackPowerMultiplyByLevel(level);
                                miningSpeed *= Configuration.SmithingGetMiningSpeedMultiplyByLevel(level);
                            }
                        }

                        void HandleTool(IPlayer player, string subLevelType)
                        {
                            { // Sub Level Calculation
                                int level = Configuration.SmithingGetLevelByEXP(Experience.GetSubExperience(player, "Smithing", subLevelType));
                                if (item.Collectible.Durability > 0)
                                {
                                    float multiply = Configuration.SmithingGetDurabilityMultiplyByLevel(level);
                                    durability = (int)Math.Round((int)durability * multiply);
                                    maxDurability = (int)Math.Round((int)maxDurability * multiply);
                                }
                            }
                        }

                        Debug.LogDebug($"[Smithing] Craft tool: {item.Item.Tool}");

                        // Increasing sub tool levels
                        switch (item.Item.Tool)
                        {
                            case EnumTool.Knife:
                                HandleMiningTool(player, "Knife");
                                break;
                            case EnumTool.Axe:
                                HandleMiningTool(player, "Axe");
                                break;
                            case EnumTool.Bow:
                                HandleWeapon(player, "Bow");
                                break;
                            case EnumTool.Chisel:
                                HandleTool(player, "Chisel");
                                break;
                            case EnumTool.Club:
                                HandleWeapon(player, "Club");
                                break;
                            case EnumTool.Crossbow:
                                HandleWeapon(player, "Crossbow");
                                break;
                            case EnumTool.Drill:
                                HandleTool(player, "Drill");
                                break;
                            case EnumTool.Firearm:
                                HandleWeapon(player, "Firearm");
                                break;
                            case EnumTool.Halberd:
                                HandleWeapon(player, "Halberd");
                                break;
                            case EnumTool.Hammer:
                                HandleWeapon(player, "Hammer");
                                break;
                            case EnumTool.Hoe:
                                HandleTool(player, "Hoe");
                                break;
                            case EnumTool.Javelin:
                                HandleWeapon(player, "Javelin");
                                break;
                            case EnumTool.Mace:
                                HandleTool(player, "Mace");
                                break;
                            case EnumTool.Meter:
                                HandleTool(player, "Meter");
                                break;
                            case EnumTool.Pickaxe:
                                HandleMiningTool(player, "Pickaxe");
                                break;
                            case EnumTool.Pike:
                                HandleTool(player, "Pike");
                                break;
                            case EnumTool.Polearm:
                                HandleTool(player, "Polearm");
                                break;
                            case EnumTool.Poleaxe:
                                HandleTool(player, "Poleaxe");
                                break;
                            case EnumTool.Probe:
                                HandleTool(player, "Probe");
                                break;
                            case EnumTool.Saw:
                                HandleTool(player, "Saw");
                                break;
                            case EnumTool.Scythe:
                                HandleTool(player, "Scythe");
                                break;
                            case EnumTool.Shears:
                                HandleTool(player, "Shears");
                                break;
                            case EnumTool.Shield:
                                HandleTool(player, "Shield");
                                break;
                            case EnumTool.Shovel:
                                HandleMiningTool(player, "Shovel");
                                break;
                            case EnumTool.Sickle:
                                HandleTool(player, "Sickle");
                                break;
                            case EnumTool.Sling:
                                HandleWeapon(player, "Sling");
                                break;
                            case EnumTool.Spear:
                                HandleWeapon(player, "Spear");
                                break;
                            case EnumTool.Staff:
                                HandleTool(player, "Staff");
                                break;
                            case EnumTool.Sword:
                                HandleWeapon(player, "Sword");
                                break;
                            case EnumTool.Warhammer:
                                HandleWeapon(player, "Warhammer");
                                break;
                            case EnumTool.Wrench:
                                HandleTool(player, "Wrench");
                                break;
                        }

                        LevelSmithingEvents.UpdateFromExternalSmithCraftingItem(player,
                            item.Collectible.Code.ToString(),
                            ref durability,
                            ref attackPower,
                            ref miningSpeed);
                    }
                    else // Code with custom level type
                    {
                        // Check if is a armor with protection properties
                        if (item.Item is ItemWearable itemWearable)
                        {
                            float multiplyProtection;
                            { // Main Level Calculation
                                int level = Configuration.SmithingGetLevelByEXP(Experience.GetExperience(player, "Smithing"));
                                multiplyProtection = Configuration.SmithingGetArmorProtectionMultiplyByLevel(level);

                                // Increasing the armor durability
                                if (item.Collectible.Durability > 0)
                                {
                                    float multiplyDurability = Configuration.SmithingGetDurabilityMultiplyByLevel(level);
                                    durability = (int)Math.Round((int)durability * multiplyDurability);
                                    maxDurability = (int)Math.Round((int)maxDurability * multiplyDurability);
                                }
                            }

                            { // Sub Level Calculation
                                int level = Configuration.SmithingGetLevelByEXP(Experience.GetSubExperience(player, "Smithing", levelType));
                                multiplyProtection *= Configuration.SmithingGetArmorProtectionMultiplyByLevel(level);

                                // Increasing the armor durability
                                if (item.Collectible.Durability > 0)
                                {
                                    float multiplyDurability = Configuration.SmithingGetDurabilityMultiplyByLevel(level);
                                    durability = (int)Math.Round((int)durability * multiplyDurability);
                                    maxDurability = (int)Math.Round((int)maxDurability * multiplyDurability);
                                }
                            }

                            LevelSmithingEvents.UpdateFromExternalSmithCraftingArmor(player,
                                item.Collectible.Code.ToString(),
                                ref durability,
                                ref multiplyProtection);

                            // Generate Base Stats
                            Shared.Instance.GenerateBaseArmorStatus(item);

                            if (item.Attributes.TryGetFloat("BaseFlatDamageReduction") != null)
                            {
                                item.Attributes.SetFloat("BaseFlatDamageReduction", item.Attributes.GetFloat("BaseFlatDamageReduction") * multiplyProtection);
                                Debug.LogDebug($"{player} armor BaseFlatDamageReduction: {item.Attributes.GetFloat("BaseFlatDamageReduction")}/{multiplyProtection}");
                            }

                            if (item.Attributes.TryGetFloat("BaseRelativeProtection") != null)
                            {
                                item.Attributes.SetFloat("BaseRelativeProtection", item.Attributes.GetFloat("BaseRelativeProtection") * multiplyProtection);
                                Debug.LogDebug($"{player} armor BaseRelativeProtection: {item.Attributes.GetFloat("BaseRelativeProtection")}/{multiplyProtection}");
                            }

                            if (item.Attributes.TryGetFloat("BaseHealingEffectivness") != null)
                            {
                                item.Attributes.SetFloat("BaseHealingEffectivness", item.Attributes.GetFloat("BaseHealingEffectivness") + (Math.Abs(item.Attributes.GetFloat("BaseHealingEffectivness")) * Math.Min(multiplyProtection - 1, 0)));
                                Debug.LogDebug($"{player} armor BaseHealingEffectivness: {item.Attributes.GetFloat("BaseHealingEffectivness")}/{multiplyProtection}");
                            }

                            if (item.Attributes.TryGetFloat("BaseHungerRate") != null)
                            {
                                item.Attributes.SetFloat("BaseHungerRate", item.Attributes.GetFloat("BaseHungerRate") + (Math.Abs(item.Attributes.GetFloat("BaseHungerRate")) * Math.Min(multiplyProtection - 1, 0)));
                                Debug.LogDebug($"{player} armor BaseHungerRate: {item.Attributes.GetFloat("BaseHungerRate")}/{multiplyProtection}");
                            }

                            if (item.Attributes.TryGetFloat("BaseRangedWeaponsAccuracy") != null)
                            {
                                item.Attributes.SetFloat("BaseRangedWeaponsAccuracy", item.Attributes.GetFloat("BaseRangedWeaponsAccuracy") + (Math.Abs(item.Attributes.GetFloat("BaseRangedWeaponsAccuracy")) * Math.Min(multiplyProtection - 1, 0)));
                                Debug.LogDebug($"{player} armor BaseRangedWeaponsAccuracy: {item.Attributes.GetFloat("BaseRangedWeaponsAccuracy")}/{multiplyProtection}");
                            }

                            if (item.Attributes.TryGetFloat("BaseRangedWeaponsSpeed") != null)
                            {
                                item.Attributes.SetFloat("BaseRangedWeaponsSpeed", item.Attributes.GetFloat("BaseRangedWeaponsSpeed") + (Math.Abs(item.Attributes.GetFloat("BaseRangedWeaponsSpeed")) * Math.Min(multiplyProtection - 1, 0)));
                                Debug.LogDebug($"{player} armor BaseRangedWeaponsSpeed: {item.Attributes.GetFloat("BaseRangedWeaponsSpeed")}/{multiplyProtection}");
                            }

                            if (item.Attributes.TryGetFloat("BaseWalkSpeed") != null)
                            {
                                item.Attributes.SetFloat("BaseWalkSpeed", item.Attributes.GetFloat("BaseWalkSpeed") + (Math.Abs(item.Attributes.GetFloat("BaseWalkSpeed")) * Math.Min(multiplyProtection - 1, 0)));
                                Debug.LogDebug($"{player} armor BaseWalkSpeed: {item.Attributes.GetFloat("BaseWalkSpeed")}/{multiplyProtection}");
                            }

                            Debug.LogDebug($"{player.PlayerName} crafted any armor protection increased to: {multiplyProtection}");
                        }
                        else if (item.Item is ItemShield itemShield)
                        {
                            float multiplyProtection;
                            { // Main Level Calculation
                                int level = Configuration.SmithingGetLevelByEXP(Experience.GetExperience(player, "Smithing"));
                                multiplyProtection = Configuration.SmithingGetArmorProtectionMultiplyByLevel(level);

                                // Increasing the armor durability
                                if (item.Collectible.Durability > 0)
                                {
                                    float multiplyDurability = Configuration.SmithingGetDurabilityMultiplyByLevel(level);
                                    durability = (int)Math.Round((int)durability * multiplyDurability);
                                    maxDurability = (int)Math.Round((int)maxDurability * multiplyDurability);
                                }
                            }

                            { // Sub Level Calculation
                                int level = Configuration.SmithingGetLevelByEXP(Experience.GetSubExperience(player, "Smithing", levelType));
                                multiplyProtection *= Configuration.SmithingGetArmorProtectionMultiplyByLevel(level);

                                // Increasing the armor durability
                                if (item.Collectible.Durability > 0)
                                {
                                    float multiplyDurability = Configuration.SmithingGetDurabilityMultiplyByLevel(level);
                                    durability = (int)Math.Round((int)durability * multiplyDurability);
                                    maxDurability = (int)Math.Round((int)maxDurability * multiplyDurability);
                                }
                            }

                            // Generate Base Stats
                            Shared.Instance.GenerateBaseShieldStatus(item);

                            if (item.Attributes.TryGetFloat("BasePassiveProjectile") != null)
                            {
                                item.Attributes.SetFloat("BasePassiveProjectile", item.Attributes.GetFloat("BasePassiveProjectile") * multiplyProtection);
                                Debug.LogDebug($"{player} shield BasePassiveProjectile: {item.Attributes.GetFloat("BasePassiveProjectile")}/{multiplyProtection}");
                            }

                            if (item.Attributes.TryGetFloat("BaseActiveProjectile") != null)
                            {
                                item.Attributes.SetFloat("BaseActiveProjectile", item.Attributes.GetFloat("BaseActiveProjectile") * multiplyProtection);
                                Debug.LogDebug($"{player} shield BaseActiveProjectile: {item.Attributes.GetFloat("BaseActiveProjectile")}/{multiplyProtection}");
                            }

                            if (item.Attributes.TryGetFloat("BasePassive") != null)
                            {
                                item.Attributes.SetFloat("BasePassive", item.Attributes.GetFloat("BasePassive") * multiplyProtection);
                                Debug.LogDebug($"{player} shield BasePassive: {item.Attributes.GetFloat("BasePassive")}/{multiplyProtection}");
                            }

                            if (item.Attributes.TryGetFloat("BaseActive") != null)
                            {
                                item.Attributes.SetFloat("BaseActive", item.Attributes.GetFloat("BaseActive") * multiplyProtection);
                                Debug.LogDebug($"{player} shield BaseActive: {item.Attributes.GetFloat("BaseActive")}/{multiplyProtection}");
                            }

                            if (item.Attributes.TryGetFloat("BaseProjectileDamageAbsorption") != null)
                            {
                                item.Attributes.SetFloat("BaseProjectileDamageAbsorption", item.Attributes.GetFloat("BaseProjectileDamageAbsorption") * multiplyProtection);
                                Debug.LogDebug($"{player} shield BaseProjectileDamageAbsorption: {item.Attributes.GetFloat("BaseProjectileDamageAbsorption")}/{multiplyProtection}");
                            }

                            if (item.Attributes.TryGetFloat("BaseDamageAbsorption") != null)
                            {
                                item.Attributes.SetFloat("BaseDamageAbsorption", item.Attributes.GetFloat("BaseDamageAbsorption") * multiplyProtection);
                                Debug.LogDebug($"{player} shield BaseDamageAbsorption: {item.Attributes.GetFloat("BaseDamageAbsorption")}/{multiplyProtection}");
                            }
                        }
                        else
                        {
                            if (Configuration.enableExtendedLog)
                                Debug.LogWarn($"[Smithing] Not a tool, not a armor, not a shield, unhandled item: {item.Collectible.Code}");
                        }
                    }
                }
                Debug.LogDebug($"{player.PlayerName} crafted: {item.Collectible.Code}");

                break;
            }
        }

        if (durability != null && maxDurability != null)
        {
            item.Attributes.SetInt("durability", (int)durability);
            item.Attributes.SetInt("maxdurability", (int)maxDurability);

            Debug.LogDebug($"{player.PlayerName} crafted any item durability increased to: {maxDurability}");
        }
        if (attackPower != null)
        {
            item.Attributes.SetFloat("attackpower", (float)attackPower);

            Debug.LogDebug($"{player.PlayerName} crafted any item attack increased to: {attackPower}");
        }
        if (miningSpeed != null)
        {
            List<EnumBlockMaterial> keys = [.. item.Item.MiningSpeed.Keys];

            foreach (EnumBlockMaterial key in keys)
            {
                item.Attributes.SetFloat($"{key}_miningspeed", item.Collectible.MiningSpeed[key] * (float)miningSpeed);
            }

            Debug.LogDebug($"{player.PlayerName} crafted any item mining speed increased to: {miningSpeed}");
        }

        return item;
    }


    [HarmonyPatchCategory("levelup_smithing")]
    private class SmithingPatch
    {
        // Luckly the durability, miningspeed and attackpower is unique by the item
        // so we just save the attribute and change it to be shared with the client and update the viewbox

        // Overwrite Craft
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ItemSlotCraftingOutput), "CraftSingle")]
        internal static void CraftSingleFinish(ItemSlotCraftingOutput __instance, ItemSlot sinkSlot, ref ItemStackMoveOperation op)
        {
            if (!Configuration.enableLevelSmithing) return;
            if (op.World.Api.Side != EnumAppSide.Server) return;
            if (sinkSlot == null || sinkSlot.Itemstack == null) return;
            if (op.ActingPlayer == null) return;

            sinkSlot.Itemstack = ExecuteSmithItemCraftedCalculations(op.ActingPlayer, sinkSlot.Itemstack);
            sinkSlot.MarkDirty();
        }

        // Overwrite Craft Multiples
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ItemSlotCraftingOutput), "CraftMany")]
        internal static void CraftManyFinish(ItemSlotCraftingOutput __instance, ItemSlot sinkSlot, ref ItemStackMoveOperation op)
        {
            if (!Configuration.enableLevelSmithing) return;
            if (op.World.Api.Side != EnumAppSide.Server) return;
            if (sinkSlot == null || sinkSlot.Itemstack == null) return;
            if (op.ActingPlayer == null) return;

            sinkSlot.Itemstack = ExecuteSmithItemCraftedCalculations(op.ActingPlayer, sinkSlot.Itemstack);
            sinkSlot.MarkDirty();
        }

        // Overwrite Visual Max Durability
        // This is necessary so the durability system is more accurate
        [HarmonyPostfix]
        [HarmonyPatch(typeof(CollectibleObject), "GetMaxDurability")]
        internal static void GetMaxDurabilityFinish(ItemStack itemstack, ref int __result)
        {
            int maxDurability = itemstack.Attributes.GetInt("maxdurability", -1);
            if (maxDurability != -1)
            {
                __result = maxDurability;
            }
        }

        /// In the next part of the code, we will edit the view of the client to show
        /// the modified protection (GetHeldArmorInfoStart), because vintage story share the ProtectionModifiers
        /// between all items of the same type we can't edit and modify a unique item,
        /// so every time a player handle the armor damage we edit the ProtectionModifier
        /// based on the attribute set in craft (HandleDamagedStart), this will refresh
        /// the ProtectionModifiers every time it will be used, making the item "unique"
        /// 
        /// The same happens for attackPower and miningSpeed

        // Update visual protections and stats
        [HarmonyPrefix] // Client Side
        [HarmonyPatch(typeof(ItemWearable), "GetHeldItemInfo")]
        [HarmonyPriority(Priority.VeryHigh)]
        internal static void GetHeldArmorInfoStart(ItemWearable __instance, ItemSlot inSlot, StringBuilder dsc, IWorldAccessor world, bool withDebugInfo)
        {
            if (inSlot.Itemstack.Attributes.GetFloat("BaseRelativeProtection", -1f) != -1f)
                if (__instance.ProtectionModifiers != null)
                    __instance.ProtectionModifiers.RelativeProtection = inSlot.Itemstack.Attributes.GetFloat("BaseRelativeProtection");

            if (inSlot.Itemstack.Attributes.GetFloat("BaseFlatDamageReduction", -1f) != -1f)
                if (__instance.ProtectionModifiers != null)
                    __instance.ProtectionModifiers.FlatDamageReduction = inSlot.Itemstack.Attributes.GetFloat("BaseFlatDamageReduction");

            if (inSlot.Itemstack.Attributes.GetFloat("BaseHealingEffectivness", -1f) != -1f)
                if (__instance.StatModifers != null)
                    __instance.StatModifers.healingeffectivness = inSlot.Itemstack.Attributes.GetFloat("BaseHealingEffectivness");

            if (inSlot.Itemstack.Attributes.GetFloat("BaseHungerRate", -1f) != -1f)
                if (__instance.StatModifers != null)
                    __instance.StatModifers.hungerrate = inSlot.Itemstack.Attributes.GetFloat("BaseHungerRate");

            if (inSlot.Itemstack.Attributes.GetFloat("BaseRangedWeaponsAccuracy", -1f) != -1f)
                if (__instance.StatModifers != null)
                    __instance.StatModifers.rangedWeaponsAcc = inSlot.Itemstack.Attributes.GetFloat("BaseRangedWeaponsAccuracy");

            if (inSlot.Itemstack.Attributes.GetFloat("BaseRangedWeaponsSpeed", -1f) != -1f)
                if (__instance.StatModifers != null)
                    __instance.StatModifers.rangedWeaponsSpeed = inSlot.Itemstack.Attributes.GetFloat("BaseRangedWeaponsSpeed");

            if (inSlot.Itemstack.Attributes.GetFloat("BaseWalkSpeed", -1f) != -1f)
                if (__instance.StatModifers != null)
                    __instance.StatModifers.walkSpeed = inSlot.Itemstack.Attributes.GetFloat("BaseWalkSpeed");
        }

        // Overwrite Visual and Interaction Attack Power
        // This is necessary so the attack power system is more accurate
        [HarmonyPostfix]
        [HarmonyPatch(typeof(CollectibleObject), "GetAttackPower")]
        internal static void GetAttackPowerFinish(ItemStack withItemStack, ref float __result)
        {
            float attackPower = withItemStack.Attributes.GetFloat("attackpower", -1f);
            if (attackPower != -1f)
            {
                __result = attackPower;
            }
        }

        // Overwrite Visual Mining Speed
        // This is necessary so the mining speed system is more accurate
        [HarmonyPrefix] // Client Side
        [HarmonyPatch(typeof(CollectibleObject), "GetHeldItemInfo")]
        internal static void GetHeldItemInfoStart(CollectibleObject __instance, ItemSlot inSlot, StringBuilder dsc, IWorldAccessor world, bool withDebugInfo)
        {
            inSlot.Itemstack.Collectible.MiningSpeed?.Foreach(x =>
            {
                if (inSlot.Itemstack.Attributes.GetFloat($"{x.Key}_miningspeed", -1f) != -1f)
                    inSlot.Itemstack.Collectible.MiningSpeed[x.Key] = inSlot.Itemstack.Attributes.GetFloat($"{x.Key}_miningspeed");
            });
        }

        // Overwrite Block break Mining Speed
        [HarmonyPostfix] // Client Side
        [HarmonyPatch(typeof(CollectibleObject), "GetMiningSpeed")]
        [HarmonyPriority(Priority.VeryHigh)]
        internal static float GetMiningSpeed(float __result, IItemStack itemstack, BlockSelection blockSel, Block block, IPlayer forPlayer)
        {
            if (forPlayer == null) return __result;

            float miningSpeed = itemstack.Attributes.GetFloat($"{block.BlockMaterial}_miningspeed", -1f);
            Debug.LogDebug($"[GetMiningSpeed] {forPlayer.PlayerName} mining speed after break: {miningSpeed}/{__result}");

            if (miningSpeed == -1f) return __result;
            else __result = miningSpeed;

            return __result;
        }
    }
}

public class LevelSmithingEvents
{
    public delegate void PlayerSmithingItemHandler(IPlayer player, string code, ref int? durability, ref float? attackPower, ref float? miningSpeed);
    public delegate void PlayerSmithingArmorHandler(IPlayer player, string code, ref int? durability, ref float armorProtectionMultiply);

    public static event PlayerSmithingItemHandler OnSmithingItem;
    public static event PlayerSmithingArmorHandler OnSmithingArmor;

    internal static void UpdateFromExternalSmithCraftingItem(IPlayer player, string code, ref int? durability, ref float? attackPower, ref float? miningSpeed)
    {
        OnSmithingItem?.Invoke(player, code, ref durability, ref attackPower, ref miningSpeed);
    }

    internal static void UpdateFromExternalSmithCraftingArmor(IPlayer player, string code, ref int? durability, ref float armorProtectionMultiply)
    {
        OnSmithingArmor?.Invoke(player, code, ref durability, ref armorProtectionMultiply);
    }
}