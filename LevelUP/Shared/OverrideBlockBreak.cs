using System;
using System.Linq;
using System.Threading.Tasks;
using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.GameContent;
using Vintagestory.Server;

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
            else Debug.Log("ERROR: Block break overwriter has already patched, did some mod already has levelup_blockbreak in harmony?");
        }
    }

    // Overwrite Breaking Speed
    [HarmonyPostfix]
    [HarmonyPatch(typeof(BlockBehavior), "GetMiningSpeedModifier")]
    public static float GetMiningSpeedModifier(float __result, IWorldAccessor world, BlockPos pos, IPlayer byPlayer)
    {
        switch (byPlayer.InventoryManager.ActiveTool)
        {
            case EnumTool.Axe:
                return Configuration.enableLevelAxe ? __result * byPlayer.Entity.WatchedAttributes.GetFloat("LevelUP_Axe_MiningSpeed", 1.0f) : __result;
            case EnumTool.Pickaxe:
                return Configuration.enableLevelPickaxe ? __result * byPlayer.Entity.WatchedAttributes.GetFloat("LevelUP_Pickaxe_MiningSpeed", 1.0f) : __result;
            case EnumTool.Shovel:
                return Configuration.enableLevelShovel ? __result * byPlayer.Entity.WatchedAttributes.GetFloat("LevelUP_Shovel_MiningSpeed", 1.0f) : __result;
            case null: break;
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
        {
            EntityPlayer playerEntity = byEntity as EntityPlayer;
            // Check if is server side
            if (playerEntity.Player is IServerPlayer && world.Side == EnumAppSide.Server)
            {
                // Earny xp by breaking tree
                instance.serverAPI?.OnClientMessage(playerEntity.Player as IServerPlayer, "Tree_Breaked_Axe");
            }
            // Single player treatment
            else if (instance.clientAPI != null && instance.clientAPI.api.IsSinglePlayer) instance.clientAPI.channel.SendPacket("Tree_Breaked_Axe");
        }
    }

    // Overwrite Ores Drop
    [HarmonyPrefix]
    [HarmonyPatch(typeof(BlockOre), "OnBlockBroken")]
    public static void OnBlockBroken(BlockOre __instance, IWorldAccessor world, IPlayer byPlayer, float dropQuantityMultiplier = 1f)
    {
        // Check if is from the server
        if (Configuration.enableLevelPickaxe && byPlayer is IServerPlayer && world.Side == EnumAppSide.Server)
        {
            IServerPlayer player = byPlayer as IServerPlayer;
            // Increasing ore drop rate
            player.Entity.Stats.Set("oreDropRate", "oreDropRate", Configuration.PickaxeGetOreMultiplyByEXP((ulong)player.Entity.WatchedAttributes.GetLong("LevelUP_Pickaxe")));

            if (Configuration.enableExtendedLog)
                Debug.Log($"{player.PlayerName} breaked a ore, multiply drop: {Configuration.PickaxeGetOreMultiplyByEXP((ulong)player.Entity.WatchedAttributes.GetLong("LevelUP_Pickaxe"))}");
        }
    }

    // Overwrite Crops Drop
    [HarmonyPostfix]
    [HarmonyPatch(typeof(BlockCrop), "GetDrops")]
    public static ItemStack[] GetDrops(ItemStack[] __result, BlockCrop __instance, IWorldAccessor world, BlockPos pos, IPlayer byPlayer, float dropQuantityMultiplier = 1f)
    {
        // Check if is from the server
        if (Configuration.enableLevelFarming && byPlayer is IServerPlayer && world.Side == EnumAppSide.Server)
        {
            // Swipe all items tack drops
            int index = 0;
            foreach (ItemStack itemStack in __result)
            {
                // Check if exist the drop crop in configuration
                if (Configuration.expPerHarvestFarming.TryGetValue(itemStack.ToString(), out _))
                {
                    IServerPlayer player = byPlayer as IServerPlayer;
                    // Multiply crop drop
                    itemStack.StackSize = (int)Math.Round(itemStack.StackSize * Configuration.FarmingGetHarvestMultiplyByEXP((ulong)player.Entity.WatchedAttributes.GetLong("LevelUP_Farming")));
                    // Update item stack result
                    __result[index] = itemStack;
                }
                index++;
            }
            if (Configuration.enableExtendedLog)
                Debug.Log($"{byPlayer.PlayerName} breaked a crop, multiply drop: {Configuration.FarmingGetHarvestMultiplyByEXP((ulong)byPlayer.Entity.WatchedAttributes.GetLong("LevelUP_Farming"))}");
        }
        return __result;
    }

}