using System.Linq;
using HarmonyLib;
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
            case EnumTool.Axe: return __result * Configuration.AxeGetMiningMultiplyByEXP(byPlayer.Entity.WatchedAttributes.GetInt("LevelUP_Axe"));
            case EnumTool.Pickaxe: return __result * Configuration.PickaxeGetMiningMultiplyByEXP(byPlayer.Entity.WatchedAttributes.GetInt("LevelUP_Pickaxe"));
            case EnumTool.Shovel: return __result * Configuration.ShovelGetMiningMultiplyByEXP(byPlayer.Entity.WatchedAttributes.GetInt("LevelUP_Shovel"));
            case null: break;
        }
        return __result;
    }

    // Overwrite Wood Axe Breaking
    [HarmonyPostfix]
    [HarmonyPatch(typeof(ItemAxe), "OnBlockBrokenWith")]
    public static void OnBlockBrokenWith(ItemAxe __instance, IWorldAccessor world, Entity byEntity, ItemSlot itemslot, BlockSelection blockSel, float dropQuantityMultiplier = 1f)
    {
        // Check if axe breaked is a player
        if (byEntity is EntityPlayer)
        {
            EntityPlayer playerEntity = byEntity as EntityPlayer;
            // Check if is server side
            if (playerEntity.Player is IServerPlayer && world.Side == EnumAppSide.Server)
            {
                // Earny xp by breaking tree
                instance.serverAPI?.OnClientMessage(playerEntity.Player as IServerPlayer, "Tree_Breaked_Axe");
            }
        }
    }

    // Overwrite Cutlery Harvesting
    [HarmonyPostfix]
    [HarmonyPatch(typeof(EntityBehaviorHarvestable), "SetHarvested")]
    public static void SetHarvested(EntityBehavior __instance, IPlayer byPlayer, float dropQuantityMultiplier = 1f)
    {
        // Check if is from the server
        if (byPlayer is IServerPlayer && __instance.entity.World.Side == EnumAppSide.Server)
        {
            // Earny xp by breaking tree
            instance.serverAPI?.OnClientMessage(byPlayer as IServerPlayer, "Cutlery_Harvest_Entity");
        };
    }

    // Overwrite Ores Drop
    [HarmonyPostfix]
    [HarmonyPatch(typeof(BlockOre), "OnBlockBroken")]
    public static void OnBlockBroken(BlockOre __instance, IWorldAccessor world, IPlayer byPlayer, float dropQuantityMultiplier = 1f)
    {
        // Check if is from the server
        if (byPlayer is IServerPlayer && world.Side == EnumAppSide.Server)
        {
            IServerPlayer player = byPlayer as IServerPlayer;
            // Increasing ore drop rate
            player.Entity.Stats.Set("oreDropRate", "oreDropRate", Configuration.PickaxeGetOreMultiplyByEXP(player.Entity.WatchedAttributes.GetAsInt("LevelUP_Pickaxe")));
        }
    }

}