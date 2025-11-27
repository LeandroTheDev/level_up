#pragma warning disable CA1822
using System;
using System.Collections.Generic;
using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace LevelUP.Server;

class LevelFarming
{
    public readonly Harmony patch = new("levelup_farming");
    public void Patch()
    {
        if (!Harmony.HasAnyPatches("levelup_farming"))
        {
            patch.PatchCategory("levelup_farming");
        }
    }
    public void Unpatch()
    {
        if (Harmony.HasAnyPatches("levelup_farming"))
        {
            patch.UnpatchCategory("levelup_farming");
        }
    }

    public void Init()
    {
        // Instanciate break block event
        Instance.api.Event.BreakBlock += OnBreakBlock;
        Configuration.RegisterNewLevel("Farming");
        Configuration.RegisterNewLevelTypeEXP("Farming", Configuration.FarmingGetLevelByEXP);
        Configuration.RegisterNewEXPLevelType("Farming", Configuration.FarmingGetExpByLevel);

        Debug.Log("Level Farming initialized");
    }

    public void InitClient()
    {
        Debug.Log("Level Farming initialized");
    }

    public void Dispose()
    { }

    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulateFarmingConfiguration(coreAPI);
        Configuration.RegisterNewMaxLevelByLevelTypeEXP("Farming", Configuration.farmingMaxLevel);
    }

    public void OnBreakBlock(IServerPlayer player, BlockSelection breakedBlock, ref float dropQuantityMultiplier, ref EnumHandling handling)
    {
        // Get the exp received
        ulong exp = (ulong)Configuration.expPerHarvestFarming.GetValueOrDefault(breakedBlock.Block.Code.ToString(), 0);
        // No crop exp finded
        if (exp <= 0) return;

        // Get the actual player total exp
        ulong playerExp = Experience.GetExperience(player, "Farming");

        Debug.LogDebug($"{player.PlayerName} breaked: {breakedBlock.Block.Code}, farming exp earned: {exp}, actual: {playerExp}");

        // Incrementing
        Experience.IncreaseExperience(player, "Farming", exp);
    }

    [HarmonyPatchCategory("levelup_farming")]
    private class LevelFarmingPatch
    {
        // Overwrite Crops Drop
        [HarmonyPostfix]
        [HarmonyPatch(typeof(BlockCrop), "GetDrops")]
        internal static ItemStack[] GetDrops(ItemStack[] __result, BlockCrop __instance, IWorldAccessor world, BlockPos pos, IPlayer byPlayer, ref float dropQuantityMultiplier)
        {
            if (!Configuration.enableLevelFarming) return __result;
            if (world.Side != EnumAppSide.Server) return __result;

            // Natural breaking without player treatment
            if (byPlayer == null) return __result;

            // Crop experience if exist
            ulong exp = 0;
            // Swipe all items stack drops
            int index = 0;
            foreach (ItemStack itemStack in __result)
            {
                // Check if exist the drop crop in configuration
                if (Configuration.expPerHarvestFarming.TryGetValue(itemStack.ToString(), out int _exp))
                {
                    exp = (ulong)_exp;
                    // Multiply crop drop
                    itemStack.StackSize = (int)Math.Round(itemStack.StackSize * Configuration.FarmingGetHarvestMultiplyByLevel(byPlayer.Entity.WatchedAttributes.GetInt("LevelUP_Level_Farming")));
                    // Update item stack result
                    __result[index] = itemStack;
                }
                index++;
            }

            LevelFarmingEvents.UpdateFromExternalHarvestCrop(byPlayer, ref __result, ref exp, ref dropQuantityMultiplier);

            // Add harvest experience
            if (exp > 0)
            {
                Experience.IncreaseExperience(byPlayer, "Farming", exp);
            }

            Debug.LogDebug($"{byPlayer.PlayerName} breaked a crop, multiply drop: {Configuration.FarmingGetHarvestMultiplyByLevel(byPlayer.Entity.WatchedAttributes.GetInt("LevelUP_Level_Farming"))}, experience: {exp}");

            return __result;
        }

        // Overwrite Hoe Till
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ItemHoe), "OnHeldInteractStep")]
        internal static void OnHeldInteractStep(bool __result, float secondsUsed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel)
        {
            if (!Configuration.enableLevelFarming) return;

            // Check if soil is tilled and is from the server
            if (byEntity.World.Side == EnumAppSide.Server && secondsUsed >= 1.0f)
            {
                // Check if is a player
                if (byEntity is EntityPlayer playerEntity)
                {
                    // Integration
                    LevelFarmingEvents.ExecuteHoeTill(playerEntity.Player);

                    // Earn xp by tilling the soil
                    Experience.IncreaseExperience(playerEntity.Player, "Farming", "Till");
                }
            }
        }

        // Overwrite Berry Forage while breaking
        [HarmonyPrefix]
        [HarmonyPatch(typeof(BlockBerryBush), "GetDrops")]
        internal static void GetDropsBerryBushFinish(BlockBerryBush __instance, IWorldAccessor world, BlockPos pos, IPlayer byPlayer, ref float dropQuantityMultiplier)
        {
            if (!Configuration.enableLevelFarming) return;
            if (byPlayer == null || world.Side != EnumAppSide.Server) return;

            // Increasing the quantity drop multiply by the farming level
            float multiply = Configuration.FarmingGetForageMultiplyByLevel(byPlayer.Entity.WatchedAttributes.GetInt("LevelUP_Level_Farming"));

            Debug.LogDebug($"{byPlayer.PlayerName} bush harvest: {__instance.Code}, by breaking multiply of: {multiply}");

            ulong exp = 0;

            // Check the berry existence
            if (Configuration.expPerHarvestFarming.TryGetValue(__instance.Code.ToString(), out int intExp))
                exp = (ulong)intExp;

            LevelFarmingEvents.UpdateFromExternalFarmForage(byPlayer, __instance.Code.ToString(), ref exp, ref multiply);

            if (exp > 0)
                Experience.IncreaseExperience(byPlayer, "Farming", exp);

            // Don't worry, it will be reseted automatically by the game
            // For some reason the -1 is necessary for this Set function so it will calculated at 0
            byPlayer.Entity.Stats.Set("forageDropRate", "forageDropRate", multiply - 1f);

        }

        // Overwrite Berry Forage while interacting
        [HarmonyPrefix]
        [HarmonyPatch(typeof(BlockBehaviorHarvestable), "OnBlockInteractStop")]
        internal static void OnBlockInteractStopStart(BlockBehaviorHarvestable __instance, float secondsUsed, IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel, ref EnumHandling handled)
        {
            if (!Configuration.enableLevelFarming) return;
            if (byPlayer != null && world.Side != EnumAppSide.Server) return;

            ulong exp = 0;
            float multiply = Configuration.FarmingGetForageMultiplyByLevel(byPlayer.Entity.WatchedAttributes.GetInt("LevelUP_Level_Farming"));

            // Check the berry existence
            if (Configuration.expPerHarvestFarming.TryGetValue(__instance.block.Code.ToString(), out int intExp))
                exp = (ulong)intExp;

            LevelFarmingEvents.UpdateFromExternalFarmForage(byPlayer, __instance.block.Code.ToString(), ref exp, ref multiply);

            if (exp > 0)
                Experience.IncreaseExperience(byPlayer, "Farming", exp);

            // Don't worry, it will be reseted automatically by the game
            // For some reason the -1 is necessary for this Set function so it will calculated at 0
            byPlayer.Entity.Stats.Set("forageDropRate", "forageDropRate", multiply - 1f);

            Debug.LogDebug($"{byPlayer.PlayerName} bush harvest: {__instance.block.Code}, by right clicking multiply of: {multiply}/{byPlayer.Entity.Stats.GetBlended("forageDropRate")}");
        }

        // Overwrite Mushroom Forage
        [HarmonyPostfix]
        [HarmonyPatch(typeof(BlockMushroom), "GetDrops")]
        internal static void GetDropsMushroomFinish(BlockMushroom __instance, IWorldAccessor world, BlockPos pos, IPlayer byPlayer, ref float dropQuantityMultiplier)
        {
            if (!Configuration.enableLevelFarming) return;
            if (byPlayer == null || world.Side != EnumAppSide.Server) return;

            // Increasing the quantity drop multiply by the farming level
            ulong exp = 0;
            float multiply = Configuration.FarmingGetForageMultiplyByLevel(byPlayer.Entity.WatchedAttributes.GetInt("LevelUP_Level_Farming"));

            // Check the berry existence
            if (Configuration.expPerHarvestFarming.TryGetValue(__instance.Code.ToString(), out int intExp))
                exp = (ulong)intExp;

            LevelFarmingEvents.UpdateFromExternalFarmForage(byPlayer, __instance.Code.ToString(), ref exp, ref multiply);

            if (exp > 0)
                Experience.IncreaseExperience(byPlayer, "Farming", exp);

            // Don't worry, it will be reseted automatically by the game
            // For some reason the -1 is necessary for this Set function so it will calculated at 0
            byPlayer.Entity.Stats.Set("forageDropRate", "forageDropRate", multiply - 1f);

            Debug.LogDebug($"{byPlayer.PlayerName} bush harvest: {__instance.Code} multiply: {multiply}/{byPlayer.Entity.Stats.GetBlended("forageDropRate")}");
        }
    }
}

public class LevelFarmingEvents
{
    public delegate void PlayerFarmHandler(IPlayer player, string code, ref ulong exp, ref float multiply);
    public delegate void PlayerHarvestCrop(IPlayer player, ref ItemStack[] itemStack, ref ulong exp, ref float dropQuantityMultiplier);
    public delegate void PlayerHandler(IPlayer player);

    public static event PlayerHarvestCrop OnHarvestCrop;
    public static event PlayerFarmHandler OnBerryForage;
    public static event PlayerHandler OnHoeTill;

    internal static void UpdateFromExternalHarvestCrop(IPlayer player, ref ItemStack[] itemStack, ref ulong exp, ref float dropQuantityMultiplier)
    {
        OnHarvestCrop?.Invoke(player, ref itemStack, ref exp, ref dropQuantityMultiplier);
    }

    internal static void ExecuteHoeTill(IPlayer player)
    {
        OnHoeTill?.Invoke(player);
    }

    internal static void UpdateFromExternalFarmForage(IPlayer player, string code, ref ulong exp, ref float multiply)
    {
        OnBerryForage?.Invoke(player, code, ref exp, ref multiply);
    }
}