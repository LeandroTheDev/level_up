using HarmonyLib;
using Vintagestory.API.Common;

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
        if (!Harmony.HasAnyPatches("levelup_blockbreak") && instance.side == EnumAppSide.Client)
        {
            overwriter = new Harmony("levelup_blockbreak");
            overwriter.PatchCategory("levelup_blockbreak");
            Debug.Log("Block break has been overwrited");
        }
    }

    // Override Breaking Speed
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Block), "OnGettingBroken")]
    public static void OnGettingBroken(Block __instance, IPlayer player, BlockSelection blockSel, ItemSlot itemslot, float remainingResistance, float dt, int counter)
    {
        // switch (byPlayer.InventoryManager.ActiveTool)
        // {
        //     case EnumTool.Axe: return Configuration.AxeGetMiningMultiplyByEXP(byPlayer.Entity.WatchedAttributes.GetInt("LevelUP_Axe"));
        //     case EnumTool.Pickaxe: return Configuration.PickaxeGetMiningMultiplyByEXP(byPlayer.Entity.WatchedAttributes.GetInt("LevelUP_Pickaxe"));
        //     case EnumTool.Shovel: return Configuration.ShovelGetMiningMultiplyByEXP(byPlayer.Entity.WatchedAttributes.GetInt("LevelUP_Shovel"));
        //     case null: break;
        // }

        // #region native
        // return 1f;
        // #endregion
    }
}