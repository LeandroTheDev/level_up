#pragma warning disable CA1822
using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using LevelUP.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.Common;
using Vintagestory.GameContent;

namespace LevelUP.Server;

// Due to laziness, the stats multiply for smithing on shields is being handled in LevelShield.
// The correct way is to create a event in levelshield to be called here in smithing

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
        OverwriteDamageInteractionEvents.OnPlayerArmorReceiveHandleStats += StatsUpdated;
        OverwriteDamageInteractionEvents.OnPlayerArmorReceiveDamageStat += DamageReceived;

        Debug.Log("Level Smithing initialized");
    }

    public void InitClient()
    {
        OverwriteDamageInteractionEvents.OnPlayerArmorViewStats += ViewReceived;
        OverwriteBlockBreakEvents.OnMiningSpeedAttributeRefreshed += MiningSpeedRefreshed;
        StatusViewEvents.OnStatusRequested += StatusViewRequested;

        Debug.Log("Level Smithing initialized");
    }

    public void Dispose()
    {
        OverwriteDamageInteractionEvents.OnPlayerArmorViewStats -= ViewReceived;
        OverwriteDamageInteractionEvents.OnPlayerArmorReceiveHandleStats -= StatsUpdated;
        OverwriteDamageInteractionEvents.OnPlayerArmorReceiveDamageStat -= DamageReceived;
        OverwriteBlockBreakEvents.OnMiningSpeedAttributeRefreshed -= MiningSpeedRefreshed;
        StatusViewEvents.OnStatusRequested -= StatusViewRequested;
    }

    public static readonly Dictionary<EnumTool, string> SubLevelPatterns = new()
    {
        { EnumTool.Knife, "Knife" },
        { EnumTool.Axe, "Axe" },
        { EnumTool.Bow, "Bow" },
        { EnumTool.Chisel, "Chisel" },
        { EnumTool.Club, "Club" },
        { EnumTool.Crossbow, "Crossbow" },
        { EnumTool.Drill, "Drill" },
        { EnumTool.Firearm, "Firearm" },
        { EnumTool.Halberd, "Halberd" },
        { EnumTool.Hammer, "Hammer" },
        { EnumTool.Hoe, "Hoe" },
        { EnumTool.Javelin, "Javelin" },
        { EnumTool.Mace, "Mace" },
        { EnumTool.Meter, "Meter" },
        { EnumTool.Pickaxe, "Pickaxe" },
        { EnumTool.Pike, "Pike" },
        { EnumTool.Polearm, "Polearm" },
        { EnumTool.Poleaxe, "Poleaxe" },
        { EnumTool.Probe, "Probe" },
        { EnumTool.Saw, "Saw" },
        { EnumTool.Scythe, "Scythe" },
        { EnumTool.Shears, "Shears" },
        { EnumTool.Shovel, "Shovel" },
        { EnumTool.Sickle, "Sickle" },
        { EnumTool.Sling, "Sling" },
        { EnumTool.Spear, "Spear" },
        { EnumTool.Staff, "Staff" },
        { EnumTool.Warhammer, "Warhammer" },
        { EnumTool.Wrench, "Wrench" }
    };

    private void StatusViewRequested(IPlayer player, ref StringBuilder stringBuilder, string levelType)
    {
        if (levelType != "Smithing") return;

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_protectionmultiply",
                Utils.GetPorcentageFromFloatsStart1(Configuration.SmithingGetArmorProtectionMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Smithing")))
            )
        );

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_statusmultiply",
                Utils.GetPorcentageFromFloatsStart1(Configuration.SmithingGetArmorStatusMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Smithing")))
            )
        );

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_damage",
                Utils.GetPorcentageFromFloatsStart1(Configuration.SmithingGetAttackPowerMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Smithing")))
            )
        );

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_durability",
                Utils.GetPorcentageFromFloatsStart1(Configuration.SmithingGetDurabilityMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Smithing")))
            )
        );

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_miningspeed",
                Utils.GetPorcentageFromFloatsStart1(Configuration.SmithingGetMiningSpeedMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Smithing")))
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
        bool parsedStatus = float.TryParse(armorSlot.Itemstack.Attributes.GetString("LevelUP_Smithing_StatsMultiply"), out float statusIncrease);
        bool parsedProtection = float.TryParse(armorSlot.Itemstack.Attributes.GetString("LevelUP_Smithing_ProtectionMultiply"), out float protectionIncrease);
        statusIncrease = parsedStatus ? statusIncrease : 1.0f;
        protectionIncrease = parsedProtection ? protectionIncrease : 1.0f;

        Shared.Instance.RefreshArmorAttributes(
            armorSlot,
            protectionIncrease,
            protectionIncrease,
            statusIncrease,
            statusIncrease,
            statusIncrease,
            statusIncrease,
            statusIncrease);
    }

    private void StatsUpdated(IPlayer player, List<ItemSlot> items)
    {
        foreach (ItemSlot armorSlot in items)
        {
            bool parsedStatus = float.TryParse(armorSlot.Itemstack.Attributes.GetString("LevelUP_Smithing_StatsMultiply"), out float statusIncrease);
            bool parsedProtection = float.TryParse(armorSlot.Itemstack.Attributes.GetString("LevelUP_Smithing_ProtectionMultiply"), out float protectionIncrease);
            statusIncrease = parsedStatus ? statusIncrease : 1.0f;
            protectionIncrease = parsedProtection ? protectionIncrease : 1.0f;

            Shared.Instance.RefreshArmorAttributes(
                armorSlot,
                protectionIncrease,
                protectionIncrease,
                statusIncrease,
                statusIncrease,
                statusIncrease,
                statusIncrease,
                statusIncrease);
        }
    }

    private void DamageReceived(IPlayer player, List<ItemSlot> items, ref float damage)
    {
        foreach (ItemSlot armorSlot in items)
        {
            bool parsedStatus = float.TryParse(armorSlot.Itemstack.Attributes.GetString("LevelUP_Smithing_StatsMultiply"), out float statusIncrease);
            bool parsedProtection = float.TryParse(armorSlot.Itemstack.Attributes.GetString("LevelUP_Smithing_ProtectionMultiply"), out float protectionIncrease);
            statusIncrease = parsedStatus ? statusIncrease : 1.0f;
            protectionIncrease = parsedProtection ? protectionIncrease : 1.0f;

            Shared.Instance.RefreshArmorAttributes(
                armorSlot,
                protectionIncrease,
                protectionIncrease,
                statusIncrease,
                statusIncrease,
                statusIncrease,
                statusIncrease,
                statusIncrease);
        }
    }

    private void MiningSpeedRefreshed(IItemStack itemStack)
    {
        float miningspeed = itemStack.Attributes.GetFloat("miningspeed", 1f);
        if (miningspeed > 1)
            Shared.Instance.RefreshToolAttributes(itemStack, miningspeed);
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
        if (item.Attributes.GetBool("repaired", false))
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
                        if (levelType == null && item.Item != null)
                        {
                            // Increasing sub tool levels
                            if (SubLevelPatterns.TryGetValue((EnumTool)item.Item.Tool, out string subName))
                            {
                                Experience.IncreaseSubExperience(player, "Smithing", subName, (ulong)exp);
                            }
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

                        // Generate Base Stats
                        Shared.Instance.GenerateBaseToolStatus(item);

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
                            float multiplyStats;
                            { // Main Level Calculation
                                int level = Configuration.SmithingGetLevelByEXP(Experience.GetExperience(player, "Smithing"));
                                multiplyProtection = Configuration.SmithingGetArmorProtectionMultiplyByLevel(level);
                                multiplyStats = Configuration.SmithingGetArmorStatusMultiplyByLevel(level);

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
                                multiplyStats *= Configuration.SmithingGetArmorStatusMultiplyByLevel(level);

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

                            item.Attributes.SetString("LevelUP_Smithing_ProtectionMultiply", multiplyProtection.ToString());
                            item.Attributes.SetString("LevelUP_Smithing_StatsMultiply", multiplyStats.ToString());

                            Debug.LogDebug($"{player.PlayerName} crafted any armor protection increased to: {multiplyProtection}/{multiplyStats}");
                        }
                        // Check if is a shield with protection properties
                        else if (item.Item is ItemShield itemShield)
                        {
                            float multiplyProtection;
                            { // Main Level Calculation
                                int level = Configuration.SmithingGetLevelByEXP(Experience.GetExperience(player, "Smithing"));
                                multiplyProtection = Configuration.SmithingGetArmorStatusMultiplyByLevel(level);

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
                                multiplyProtection *= Configuration.SmithingGetArmorStatusMultiplyByLevel(level);

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

                            item.Attributes.SetString("LevelUP_Smithing_StatsMultiply", multiplyProtection.ToString());

                            Debug.LogDebug($"{player.PlayerName} crafted any shield protection increased to: {multiplyProtection}");
                        }
                        // Unkown
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
            item.Attributes.SetFloat("miningspeed", (float)miningSpeed);

            Debug.LogDebug($"{player.PlayerName} crafted any item mining speed increased to: {miningSpeed}");
        }

        return item;
    }


    [HarmonyPatchCategory("levelup_smithing")]
    private class SmithingPatch
    {
        // Collect recipe items before consumption
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ItemSlotCraftingOutput), "CraftSingle")]
        internal static void CraftSingleStart(ItemSlotCraftingOutput __instance, ItemSlot sinkSlot, ref ItemStackMoveOperation op, out List<string> __state)
        {
            __state = null;

            if (!Configuration.enableLevelSmithing) return;
            if (op.World.Api.Side != EnumAppSide.Server) return;
            if (op.ActingPlayer == null) return;

            IPlayer player = op.ActingPlayer;
            IInventory craftingGrid = player.InventoryManager.GetOwnInventory("craftinggrid");

            if (craftingGrid == null) return;

            List<string> recipeItems = [];
            for (int i = 0; i < craftingGrid.Count; i++)
            {
                // Index 9 is the output, output needs to be ignored
                if (i == 9) continue;
                ItemSlot slot = craftingGrid[i];
                if (slot?.Itemstack != null)
                {
                    recipeItems.Add(slot.Itemstack.Collectible.Code);
                }
            }

            if (recipeItems.Count > 0)
                __state = recipeItems;
        }

        // Overwrite Craft
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ItemSlotCraftingOutput), "CraftSingle")]
        internal static void CraftSingleFinish(ItemSlotCraftingOutput __instance, ItemSlot sinkSlot, ref ItemStackMoveOperation op, List<string> __state)
        {
            if (!Configuration.enableLevelSmithing) return;
            if (op.World.Api.Side != EnumAppSide.Server) return;
            if (sinkSlot == null || sinkSlot.Itemstack == null) return;
            if (op.ActingPlayer == null) return;

            // If the recipe contains the currently item code, them ignore the smith mechanic
            if (__state != null && __state.Contains(sinkSlot.Itemstack.Collectible.Code)) return;

            sinkSlot.Itemstack = ExecuteSmithItemCraftedCalculations(op.ActingPlayer, sinkSlot.Itemstack);
            sinkSlot.MarkDirty();
        }

        // Collect recipe items before consumption
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ItemSlotCraftingOutput), "CraftMany")]
        internal static void CraftManyStart(ItemSlotCraftingOutput __instance, ItemSlot sinkSlot, ref ItemStackMoveOperation op, out List<string> __state)
        {
            __state = null;

            if (!Configuration.enableLevelSmithing) return;
            if (op.World.Api.Side != EnumAppSide.Server) return;
            if (op.ActingPlayer == null) return;

            IPlayer player = op.ActingPlayer;
            IInventory craftingGrid = player.InventoryManager.GetOwnInventory("craftinggrid");

            if (craftingGrid == null) return;

            List<string> recipeItems = [];
            for (int i = 0; i < craftingGrid.Count; i++)
            {
                ItemSlot slot = craftingGrid[i];
                if (slot?.Itemstack != null)
                {
                    recipeItems.Add(slot.Itemstack.Collectible.Code);
                }
            }

            if (recipeItems.Count > 0)
                __state = recipeItems;
        }

        // Overwrite Craft Multiples
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ItemSlotCraftingOutput), "CraftMany")]
        internal static void CraftManyFinish(ItemSlotCraftingOutput __instance, ItemSlot sinkSlot, ref ItemStackMoveOperation op, List<string> __state)
        {
            if (!Configuration.enableLevelSmithing) return;
            if (op.World.Api.Side != EnumAppSide.Server) return;
            if (sinkSlot == null || sinkSlot.Itemstack == null) return;
            if (op.ActingPlayer == null) return;
            // If the recipe contains the currently item code, them ignore the smith mechanic
            if (__state != null && __state.Contains(sinkSlot.Itemstack.Collectible.Code)) return;

            sinkSlot.Itemstack = ExecuteSmithItemCraftedCalculations(op.ActingPlayer, sinkSlot.Itemstack);
            sinkSlot.MarkDirty();
        }

        // Overwrite Visual Max Durability
        // This is necessary so the durability system is more accurate
        [HarmonyPrefix]
        [HarmonyPatch(typeof(CollectibleObject), "GetMaxDurability")]
        internal static bool GetMaxDurabilityFinish(ItemStack itemstack, ref int __result)
        {
            int maxDurability = itemstack.Attributes.GetInt("maxdurability", -1);
            if (maxDurability != -1)
            {
                __result = maxDurability;
                return false;
            }
            return true;
        }

        // Overwrite Visual Attack Power
        // Should be called before other codes because smithing is the base damage
        [HarmonyPostfix]
        [HarmonyPatch(typeof(CollectibleObject), "GetAttackPower")]
        [HarmonyPriority(Priority.High)]
        internal static void GetAttackPowerFinish(ItemStack withItemStack, ref float __result)
        {
            float attackPower = withItemStack.Attributes.GetFloat("attackpower", -1f);
            if (attackPower != -1f)
            {
                __result = attackPower;
            }
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