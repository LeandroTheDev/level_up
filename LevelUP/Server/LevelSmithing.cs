#pragma warning disable CA1822
using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using Newtonsoft.Json.Linq;
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

            if (collectableCode.EndsWith(item.Collectible.Code.ToString()))
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
                        if (item.Collectible.Attributes.KeyExists("protectionModifiers"))
                        {
                            // Now we are converting the attributes to a modifiable json

                            string data = item.Collectible.Attributes.Token.ToString();
                            JObject jsonObject = JObject.Parse(data);

                            // Getting the protectionModifiers
                            if (jsonObject.TryGetValue("protectionModifiers", out JToken protectionModifiersToken))
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

                                // Getting the modifiable protectionModifiers
                                if (protectionModifiersToken is JObject protectionModifiers)
                                {
                                    // Check if exist, and change it
                                    if (protectionModifiers.TryGetValue("relativeProtection", out _))
                                    {
                                        item.Attributes.SetFloat("relativeProtection", (float)jsonObject["protectionModifiers"]["relativeProtection"] * multiplyProtection);
                                        Debug.LogDebug($"{player} armor relativeProtection: {jsonObject["protectionModifiers"]["relativeProtection"]}/{multiplyProtection}");
                                    }
                                    // Check if exist, and change it
                                    if (protectionModifiers.TryGetValue("flatDamageReduction", out _))
                                    {
                                        item.Attributes.SetFloat("flatDamageReduction", (float)jsonObject["protectionModifiers"]["flatDamageReduction"] * multiplyProtection);
                                        Debug.LogDebug($"{player} armor flatDamageReduction: {jsonObject["protectionModifiers"]["flatDamageReduction"]}/{multiplyProtection}");
                                    }
                                }

                                // Convert again to JsonObject and replace it in attributes
                                item.Collectible.Attributes = new(JToken.Parse(jsonObject.ToString()));

                                Debug.LogDebug($"{player.PlayerName} crafted any armor protection increased to: {multiplyProtection}");
                            }
                        }
                        else
                        {
                            Debug.LogWarn($"[Smithing] Not a tool, and not a armor, unhandled item: {item.Collectible.Code}");
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
            // item.Collectible.AttackPower = (float)attackPower; // Never do that, this will change all "tool" damage
            item.Attributes.SetFloat("attackpower", (float)attackPower);

            Debug.LogDebug($"{player.PlayerName} crafted any item attack increased to: {attackPower}");
        }
        if (miningSpeed != null)
        {
            List<EnumBlockMaterial> keys = [.. item.Item.MiningSpeed.Keys];

            foreach (EnumBlockMaterial key in keys)
            {
                // item.Collectible.MiningSpeed[key] *= (float)miningSpeed; // Never do that, this will change all "tool" mining speed
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

        // Overwrite Visual Protections
        // This is necessary so the protection system is more accurate
        [HarmonyPrefix] // Client Side
        [HarmonyPatch(typeof(ItemWearable), "GetHeldItemInfo")]
        [HarmonyPriority(Priority.VeryHigh)]
        internal static void GetHeldArmorInfoStart(ItemWearable __instance, ItemSlot inSlot, StringBuilder dsc, IWorldAccessor world, bool withDebugInfo)
        {
            if (inSlot.Itemstack.Attributes.GetFloat("relativeProtection", -1f) != -1f)
                __instance.ProtectionModifiers.RelativeProtection = inSlot.Itemstack.Attributes.GetFloat("relativeProtection");

            if (inSlot.Itemstack.Attributes.GetFloat("flatDamageReduction", -1f) != -1f)
                __instance.ProtectionModifiers.FlatDamageReduction = inSlot.Itemstack.Attributes.GetFloat("flatDamageReduction");
        }

        // Overwrite Protection Damage Handle
        // This is necessary so the protection system is more accurate
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ModSystemWearableStats), "handleDamaged")]
        internal static void HandleDamagedStart(ModSystemWearableStats __instance, IPlayer player, float damage, DamageSource dmgSource)
        {
            if (!Configuration.enableLevelSmithing) return;
            if (player.Entity.World.Side != EnumAppSide.Server) return;

            IInventory inv = player.InventoryManager.GetOwnInventory("character");

            // In the native code, only use the inventory 12,13,14 to calculate the damage protection,
            // also is random which part of the armor is used to be calculated, but we recalculate everthing
            // because we don't know what part will be used on the prefix
            for (int i = 12; i <= 14; i++)
            {
                ItemSlot armorSlot = inv[i];
                if (armorSlot.Itemstack?.Item is ItemWearable armorWearable)
                {
                    if (armorWearable.ProtectionModifiers == null)
                    {
                        Debug.LogDebug($"{player.PlayerName} {armorSlot.Itemstack.GetName()} Armor System ignored because ProtectionModifiers is null");
                        return;
                    }

                    // If the armor is created from a non player source, and a player can craft
                    // the armor, they will be incosistent, so we need to refresh the default values
                    // too unfurtunally

                    Debug.LogDebug($"{player.PlayerName} {armorSlot.Itemstack.GetName()} Armor System Handling before R/F: {armorWearable.ProtectionModifiers.RelativeProtection}");

                    // Only modify the relativeProtection if exist
                    if (armorSlot.Itemstack.Attributes.GetFloat("relativeProtection", -1f) != -1f)
                        armorWearable.ProtectionModifiers.RelativeProtection = armorSlot.Itemstack.Attributes.GetFloat("relativeProtection");
                    else // Otherwises we need to refresh from default
                    {
                        if (armorSlot.Itemstack.Collectible.Attributes.KeyExists("protectionModifiers"))
                            if (armorSlot.Itemstack.Collectible.Attributes["protectionModifiers"].KeyExists("relativeProtection"))
                                armorWearable.ProtectionModifiers.RelativeProtection = armorSlot.Itemstack.Collectible.Attributes["protectionModifiers"]["relativeProtection"].AsFloat();
                    }
                    // Only modify the relativeProtection if exist
                    if (armorSlot.Itemstack.Attributes.GetFloat("flatDamageReduction", -1f) != -1f)
                        armorWearable.ProtectionModifiers.FlatDamageReduction = armorSlot.Itemstack.Attributes.GetFloat("flatDamageReduction");
                    else // Otherwises we need to refresh from default
                    {
                        if (armorSlot.Itemstack.Collectible.Attributes.KeyExists("protectionModifiers"))
                            if (armorSlot.Itemstack.Collectible.Attributes["protectionModifiers"].KeyExists("relativeProtection"))
                                armorWearable.ProtectionModifiers.RelativeProtection = armorSlot.Itemstack.Collectible.Attributes["protectionModifiers"]["relativeProtection"].AsFloat();
                    }

                    Debug.LogDebug($"{player.PlayerName} {armorSlot.Itemstack.GetName()} Armor System Handling after R/F: {armorWearable.ProtectionModifiers.RelativeProtection}/{armorWearable.ProtectionModifiers.FlatDamageReduction}");
                }
            }
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