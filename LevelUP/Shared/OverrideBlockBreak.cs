using System.Linq;
using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;

namespace LevelUP.Shared;

[HarmonyPatchCategory("levelup_blockbreak")]
class OverrideBlockBreak
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

    // Override Breaking Speed
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
}