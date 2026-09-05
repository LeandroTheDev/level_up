#pragma warning disable CA1822
using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using LevelUP.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
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
        StatusViewEvents.OnStatusRequested -= StatusViewRequested;
        StatusViewEvents.OnStatusRequested += StatusViewRequested;

        Debug.Log("Level Farming initialized");
    }

    public void Dispose()
    {
        StatusViewEvents.OnStatusRequested -= StatusViewRequested;
    }

    private void StatusViewRequested(IPlayer player, ref StringBuilder stringBuilder, string levelType)
    {
        if (levelType != "Farming") return;

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_forage",
                Utils.GetPorcentageFromFloatsStart1(Configuration.FarmingGetForageMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Farming")))
            )
        );

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_harvest",
                Utils.GetPorcentageFromFloatsStart0(Configuration.FarmingGetHarvestMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Farming")) - Configuration.BaseHarvestMultiplyFarming)
            )
        );
    }

    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulateFarmingConfiguration(coreAPI);
        Configuration.RegisterNewMaxLevelByLevelTypeEXP("Farming", Configuration.farmingMaxLevel);
    }

    public void OnBreakBlock(IServerPlayer player, BlockSelection breakedBlock, ref float dropQuantityMultiplier, ref EnumHandling handling)
    {
        ulong exp = (ulong)Configuration.expPerHarvestFarming.GetValueOrDefault(breakedBlock.Block.Code.ToString(), 0);
        if (exp <= 0) return;

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

        // Overwrite Mushroom Forage
        [HarmonyPostfix]
        [HarmonyPatch(typeof(BlockMushroom), "GetDrops")]
        internal static void GetDropsMushroomFinish(ItemStack[] __result, BlockMushroom __instance, IWorldAccessor world, BlockPos pos, IPlayer byPlayer)
        {
            if (!Configuration.enableLevelFarming) return;
            if (byPlayer == null || world.Side != EnumAppSide.Server) return;

            // Increasing the quantity drop multiply by the farming level
            ulong exp = 0;
            float multiply = Configuration.FarmingGetForageMultiplyByLevel(byPlayer.Entity.WatchedAttributes.GetInt("LevelUP_Level_Farming"));

            // Check the mushroom existence
            if (Configuration.expPerHarvestFarming.TryGetValue(__instance.Code.ToString(), out int intExp))
                exp = (ulong)intExp;

            LevelFarmingEvents.UpdateFromExternalFarmForage(byPlayer, __instance.Code.ToString(), ref exp, ref multiply);

            if (exp > 0)
                Experience.IncreaseExperience(byPlayer, "Farming", exp);

            // Multiply the drop directly instead of relying on the game's forageDropRate stat:
            // that stat is only read by blocks flagged forageStatAffected, and being set here in a
            // Postfix it would only take effect on the player's *next* forage action, not this one
            foreach (ItemStack itemStack in __result)
            {
                itemStack.StackSize = (int)Math.Round(itemStack.StackSize * multiply);
            }

            Debug.LogDebug($"{byPlayer.PlayerName} mushroom harvest: {__instance.Code} multiply: {multiply}");
        }

        // Overwrite Fruiting Bush Forage (right-click harvest)
        // Capture the growth state before the harvest so the postfix can tell whether it actually completed
        [HarmonyPrefix]
        [HarmonyPatch(typeof(BEBehaviorFruitingBush), "OnBlockInteractStop")]
        internal static void FruitingBushInteractStopStart(BEBehaviorFruitingBush __instance, out EnumFruitingBushGrowthState __state)
        {
            __state = __instance.BState.Growthstate;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(BEBehaviorFruitingBush), "OnBlockInteractStop")]
        internal static void FruitingBushInteractStopFinish(BEBehaviorFruitingBush __instance, IWorldAccessor world, IPlayer byPlayer, EnumFruitingBushGrowthState __state)
        {
            if (!Configuration.enableLevelFarming) return;
            if (byPlayer == null || world.Side != EnumAppSide.Server) return;

            // Only reward if a harvest actually completed this call (state transitions Ripe -> Mature)
            if (__state != EnumFruitingBushGrowthState.Ripe || __instance.BState.Growthstate != EnumFruitingBushGrowthState.Mature) return;

            ulong exp = 0;
            float multiply = Configuration.FarmingGetForageMultiplyByLevel(byPlayer.Entity.WatchedAttributes.GetInt("LevelUP_Level_Farming"));

            // Check the fruiting bush existence
            if (Configuration.expPerHarvestFarming.TryGetValue(__instance.Block.Code.ToString(), out int intExp))
                exp = (ulong)intExp;

            LevelFarmingEvents.UpdateFromExternalFarmForage(byPlayer, __instance.Block.Code.ToString(), ref exp, ref multiply);

            if (exp > 0)
                Experience.IncreaseExperience(byPlayer, "Farming", exp);

            // Unlike BlockCrop/BlockMushroom, this method hands the base drops straight to the player's
            // inventory instead of returning an ItemStack[], so we can't postfix-multiply the result.
            // Instead we top up the difference ourselves, using GetRipeDrops() (public API) as a
            // reference for what the base harvest looks like, without touching any vanilla game asset.
            float bonus = multiply - 1f;
            if (bonus > 0f)
            {
                foreach (ItemStack baseStack in __instance.GetRipeDrops(byPlayer) ?? [])
                {
                    int bonusAmount = (int)Math.Round(baseStack.StackSize * bonus);
                    if (bonusAmount <= 0) continue;

                    ItemStack bonusStack = baseStack.Clone();
                    bonusStack.StackSize = bonusAmount;
                    if (!byPlayer.InventoryManager.TryGiveItemstack(bonusStack))
                        world.SpawnItemEntity(bonusStack, __instance.Position);
                }
            }

            Debug.LogDebug($"{byPlayer.PlayerName} fruiting bush harvest: {__instance.Block.Code}, multiply: {multiply}, exp: {exp}");
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