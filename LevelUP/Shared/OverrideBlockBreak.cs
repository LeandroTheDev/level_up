using System;
using HarmonyLib;
using LevelUP.Server;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace LevelUP.Shared;

#pragma warning disable IDE0060
[HarmonyPatchCategory("levelup_blockbreak")]
class OverwriteBlockBreak
{
    private static Instance instance;
    public Harmony overwriter;

    public void OverwriteNativeFunctions(Instance _instance)
    {
        instance = _instance;
        instance.ToString(); //Suppress Alerts
        if (!Harmony.HasAnyPatches("levelup_blockbreak"))
        {
            overwriter = new Harmony("levelup_blockbreak");
            overwriter.PatchCategory("levelup_blockbreak");
            Debug.Log("Block break has been overwrited");
        }
        else
        {
            if (instance.side == EnumAppSide.Client) Debug.Log("Block break overwriter has already patched, probably by the singleplayer server");
            else Debug.LogError("ERROR: Block break overwriter has already patched, did some mod already has levelup_blockbreak in harmony?");
        }
    }

    // Overwrite Breaking Speed
    [HarmonyPostfix]
    [HarmonyPatch(typeof(BlockBehavior), "GetMiningSpeedModifier")]
    public static float GetMiningSpeedModifier(float __result, IWorldAccessor world, BlockPos pos, IPlayer byPlayer)
    {
        float miningSpeedCompatibility = byPlayer.Entity.Attributes.GetFloat("LevelUP_BlockBreak_ExtendMiningSpeed_GetMiningSpeedModifier");
        byPlayer.Entity.Attributes.RemoveAttribute("LevelUP_BlockBreak_ExtendMiningSpeed_GetMiningSpeedModifier");
        Block blockBreaking = world.GetBlockAccessor(false, false, false).GetBlock(pos);
        switch (byPlayer.InventoryManager.ActiveTool)
        {
            case EnumTool.Axe:
                if (blockBreaking.BlockMaterial == EnumBlockMaterial.Wood)
                    return Configuration.enableLevelAxe ? __result * byPlayer.Entity.WatchedAttributes.GetFloat("LevelUP_Axe_MiningSpeed", 1.0f) + miningSpeedCompatibility : __result;
                else return 1.0f;
            case EnumTool.Pickaxe:
                if (blockBreaking.BlockMaterial == EnumBlockMaterial.Brick ||
                    blockBreaking.BlockMaterial == EnumBlockMaterial.Ceramic ||
                    blockBreaking.BlockMaterial == EnumBlockMaterial.Metal ||
                    blockBreaking.BlockMaterial == EnumBlockMaterial.Ore ||
                    blockBreaking.BlockMaterial == EnumBlockMaterial.Stone)
                    return Configuration.enableLevelPickaxe ? __result * byPlayer.Entity.WatchedAttributes.GetFloat("LevelUP_Pickaxe_MiningSpeed", 1.0f) + miningSpeedCompatibility : __result;
                else return 1.0f;
            case EnumTool.Shovel:
                if (blockBreaking.BlockMaterial == EnumBlockMaterial.Soil ||
                    blockBreaking.BlockMaterial == EnumBlockMaterial.Gravel ||
                    blockBreaking.BlockMaterial == EnumBlockMaterial.Sand ||
                    blockBreaking.BlockMaterial == EnumBlockMaterial.Snow)
                    return Configuration.enableLevelShovel ? __result * byPlayer.Entity.WatchedAttributes.GetFloat("LevelUP_Shovel_MiningSpeed", 1.0f) + miningSpeedCompatibility : __result;
                else return 1.0f;
            case EnumTool.Knife:
                if (blockBreaking.BlockMaterial == EnumBlockMaterial.Plant ||
                    blockBreaking.BlockMaterial == EnumBlockMaterial.Leaves)
                    return Configuration.enableLevelKnife ? __result * byPlayer.Entity.WatchedAttributes.GetFloat("LevelUP_Knife_MiningSpeed", 1.0f) + miningSpeedCompatibility : __result;
                else return 1.0f;
        }
        return __result;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(CollectibleObject), "GetMiningSpeed")]
    public static float GetMiningSpeed(float __result, IItemStack itemstack, BlockSelection blockSel, Block block, IPlayer forPlayer)
    {
        if (forPlayer == null || forPlayer.Entity == null) return __result;

        float miningSpeedCompatibility = forPlayer.Entity.Attributes.GetFloat("LevelUP_BlockBreak_ExtendMiningSpeed_GetMiningSpeedModifier");
        forPlayer.Entity.Attributes.RemoveAttribute("LevelUP_BlockBreak_ExtendMiningSpeed_GetMiningSpeedModifier");
        switch (forPlayer.InventoryManager.ActiveTool)
        {
            case EnumTool.Axe:
                if (block.BlockMaterial == EnumBlockMaterial.Wood)
                    return Configuration.enableLevelAxe ? __result * forPlayer.Entity.WatchedAttributes.GetFloat("LevelUP_Axe_MiningSpeed", 1.0f) + miningSpeedCompatibility : __result;
                else return 1.0f;
            case EnumTool.Pickaxe:
                if (block.BlockMaterial == EnumBlockMaterial.Brick ||
                    block.BlockMaterial == EnumBlockMaterial.Ceramic ||
                    block.BlockMaterial == EnumBlockMaterial.Metal ||
                    block.BlockMaterial == EnumBlockMaterial.Ore ||
                    block.BlockMaterial == EnumBlockMaterial.Stone)
                    return Configuration.enableLevelPickaxe ? __result * forPlayer.Entity.WatchedAttributes.GetFloat("LevelUP_Pickaxe_MiningSpeed", 1.0f) + miningSpeedCompatibility : __result;
                else return 1.0f;
            case EnumTool.Shovel:
                if (block.BlockMaterial == EnumBlockMaterial.Soil ||
                    block.BlockMaterial == EnumBlockMaterial.Gravel ||
                    block.BlockMaterial == EnumBlockMaterial.Sand ||
                    block.BlockMaterial == EnumBlockMaterial.Snow)
                    return Configuration.enableLevelShovel ? __result * forPlayer.Entity.WatchedAttributes.GetFloat("LevelUP_Shovel_MiningSpeed", 1.0f) + miningSpeedCompatibility : __result;
                else return 1.0f;
            case EnumTool.Knife:
                if (block.BlockMaterial == EnumBlockMaterial.Plant ||
                    block.BlockMaterial == EnumBlockMaterial.Leaves)
                    return Configuration.enableLevelKnife ? __result * forPlayer.Entity.WatchedAttributes.GetFloat("LevelUP_Knife_MiningSpeed", 1.0f) + miningSpeedCompatibility : __result;
                else return 1.0f;
        }
        return __result;
    }

    // Overwrite Wood Axe Breaking
    [HarmonyPrefix]
    [HarmonyPatch(typeof(ItemAxe), "OnBlockBrokenWith")]
    public static void OnBlockBrokenWith(ItemAxe __instance, IWorldAccessor world, Entity byEntity, ItemSlot itemslot, BlockSelection blockSel, float dropQuantityMultiplier = 1f)
    {
        // Check if axe breaked is a player
        if (Configuration.enableLevelAxe && byEntity is EntityPlayer)
            Experience.IncreaseExperience((byEntity as EntityPlayer).Player, "Axe", "TreeBreak");
    }

    // Overwrite Ores Drop
    [HarmonyPrefix]
    [HarmonyPatch(typeof(BlockOre), "OnBlockBroken")]
    public static void OnBlockBroken(BlockOre __instance, IWorldAccessor world, IPlayer byPlayer, ref float dropQuantityMultiplier)
    {
        // Check if is from the server
        if (Configuration.enableLevelPickaxe && byPlayer != null && world.Side == EnumAppSide.Server)
        {
            // Increasing ore drop rate
            dropQuantityMultiplier += Configuration.PickaxeGetOreMultiplyByLevel(byPlayer.Entity.WatchedAttributes.GetInt("LevelUP_Level_Pickaxe"));
            Debug.LogDebug($"{byPlayer.PlayerName} breaked a ore, multiply drop: {Configuration.PickaxeGetOreMultiplyByLevel(byPlayer.Entity.WatchedAttributes.GetInt("LevelUP_Level_Pickaxe"))}");
        }
    }

    // Overwrite Crops Drop
    [HarmonyPostfix]
    [HarmonyPatch(typeof(BlockCrop), "GetDrops")]
    public static ItemStack[] GetDrops(ItemStack[] __result, BlockCrop __instance, IWorldAccessor world, BlockPos pos, IPlayer byPlayer, float dropQuantityMultiplier = 1f)
    {
        // Natural breaking without player treatment
        if (byPlayer == null) return __result;

        // Check if is from the server
        if (Configuration.enableLevelFarming && byPlayer != null && world.Side == EnumAppSide.Server)
        {
            // Crop experience if exist
            int? exp = null;
            // Swipe all items stack drops
            int index = 0;
            foreach (ItemStack itemStack in __result)
            {
                // Check if exist the drop crop in configuration
                if (Configuration.expPerHarvestFarming.TryGetValue(itemStack.ToString(), out int _exp))
                {
                    exp = _exp;
                    // Multiply crop drop
                    itemStack.StackSize = (int)Math.Round(itemStack.StackSize * Configuration.FarmingGetHarvestMultiplyByLevel(byPlayer.Entity.WatchedAttributes.GetInt("LevelUP_Level_Farming")));
                    // Update item stack result
                    __result[index] = itemStack;
                }
                index++;
            }

            // Add harvest experience
            if (exp != null)
                Experience.IncreaseExperience(byPlayer, "Farming", (ulong)exp);

            Debug.LogDebug($"{byPlayer.PlayerName} breaked a crop, multiply drop: {Configuration.FarmingGetHarvestMultiplyByLevel(byPlayer.Entity.WatchedAttributes.GetInt("LevelUP_Level_Farming"))}, experience: {exp}");
        }
        return __result;
    }

}