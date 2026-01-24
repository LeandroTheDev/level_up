#pragma warning disable IDE0060
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace LevelUP.Server;

[HarmonyPatchCategory("levelup_blockbreak")]
class OverwriteBlockBreak
{
    public readonly Harmony patch = new("levelup_blockbreak");
    public void Patch()
    {
        // No patch needed in server for block break
        if (Shared.Instance.api is ICoreServerAPI) return;

        if (!Harmony.HasAnyPatches("levelup_blockbreak"))
        {
            patch.PatchCategory("levelup_blockbreak");
        }
    }
    public void Unpatch()
    {
        if (Harmony.HasAnyPatches("levelup_blockbreak"))
        {
            patch.UnpatchCategory("levelup_blockbreak");
        }
    }

    // Multiplier function called on the end of the transpiler
    internal static float GetMultiplier(IItemStack itemstack, BlockSelection blockSel, Block block, IPlayer player, CollectibleObject collectible)
    {
        return OverwriteBlockBreakEvents.GetExternalMiningSpeedMultiply(collectible, itemstack, blockSel, block, player, 1.0f);
    }

    // Transpiler to increment mining speed
    [HarmonyTranspiler]
    [HarmonyPatch(typeof(CollectibleObject), "GetMiningSpeed")]
    internal static List<CodeInstruction> GetMiningSpeed_Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var list = new List<CodeInstruction>(instructions);

        FieldInfo toolMiningSpeedField = AccessTools.Field(typeof(CollectibleObject)
            .GetNestedTypes(BindingFlags.NonPublic | BindingFlags.Instance)
            .First(t => t.GetFields().Any(f => f.Name.Contains("toolMiningSpeed"))),
            "toolMiningSpeed"
        );

        MethodInfo getMultiplierMethod =
            AccessTools.Method(typeof(OverwriteBlockBreak), "GetMultiplier");

        // Insert multiplier method before return
        for (int i = 0; i < list.Count - 2; i++)
        {
            if (list[i].opcode == OpCodes.Ldloc_0 &&
                list[i + 1].opcode == OpCodes.Ldfld &&
                (FieldInfo)list[i + 1].operand == toolMiningSpeedField &&
                list[i + 2].opcode == OpCodes.Ret)
            {
                var insert = new List<CodeInstruction>()
                {
                    new(OpCodes.Ldloc_0),
                    new(OpCodes.Ldloc_0),
                    new(OpCodes.Ldfld, toolMiningSpeedField),

                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Ldarg_2),
                    new(OpCodes.Ldarg_3),
                    new(OpCodes.Ldarg_S, 4),

                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call, getMultiplierMethod),

                    new(OpCodes.Mul),
                    new(OpCodes.Stfld, toolMiningSpeedField)
                };

                list.InsertRange(i, insert);
                break;
            }
        }

        return list;
    }

    [HarmonyPrefix] // Client Side
    [HarmonyPatch(typeof(CollectibleObject), "GetMiningSpeed")]
    [HarmonyPriority(Priority.VeryLow)]
    internal static void GetMiningSpeedStart(CollectibleObject __instance, IItemStack itemstack, BlockSelection blockSel, Block block, IPlayer forPlayer)
    {
        Shared.Instance.ResetToolAttributes(itemstack);
        OverwriteBlockBreakEvents.ExecuteMiningSpeedAttribute(itemstack);
    }

    [HarmonyPostfix] // Client Side
    [HarmonyPatch(typeof(CollectibleObject), "GetMiningSpeed")]
    [HarmonyPriority(Priority.VeryLow)]
    internal static void GetMiningSpeedFinish(CollectibleObject __instance, IItemStack itemstack, BlockSelection blockSel, Block block, IPlayer forPlayer)
    {
        Shared.Instance.ResetToolAttributes(itemstack);
    }

    [HarmonyPrefix] // Client Side
    [HarmonyPatch(typeof(CollectibleObject), "GetHeldItemInfo")]
    internal static void GetHeldItemInfoStart(CollectibleObject __instance, ItemSlot inSlot, StringBuilder dsc, IWorldAccessor world, bool withDebugInfo)
    {
        if (inSlot.Itemstack?.Item == null || inSlot.Itemstack.Item.Tool == null) return;

        Shared.Instance.ResetToolAttributes(inSlot.Itemstack);
        OverwriteBlockBreakEvents.ExecuteMiningSpeedAttribute(inSlot.Itemstack);
    }

    [HarmonyPostfix] // Client Side
    [HarmonyPatch(typeof(CollectibleObject), "GetHeldItemInfo")]
    internal static void GetHeldItemInfoFinish(CollectibleObject __instance, ItemSlot inSlot, StringBuilder dsc, IWorldAccessor world, bool withDebugInfo)
    {
        if (inSlot.Itemstack?.Item == null || inSlot.Itemstack.Item.Tool == null) return;

        Shared.Instance.ResetToolAttributes(inSlot.Itemstack);
    }
}

public static class OverwriteBlockBreakEvents
{
    public delegate void MiningSpeedModifier(CollectibleObject collectible, IItemStack itemstack, BlockSelection blockSel, Block block, IPlayer player, ref float multiply);
    public delegate void MiningSpeedHandle(IItemStack collectible);
    /// After all attributes and behaviors calculation this is called.
    /// Use when you need only to change the speed
    public static event MiningSpeedModifier OnMiningSpeedRefreshed;
    /// Called before OnMiningSpeedRefreshed, used to change attributes before calculations.
    /// Use when you need to show the mining difference in player view
    public static event MiningSpeedHandle OnMiningSpeedAttributeRefreshed;

    internal static float GetExternalMiningSpeedMultiply(CollectibleObject collectible, IItemStack itemstack, BlockSelection blockSel, Block block, IPlayer player, float multiply)
    {
        OnMiningSpeedRefreshed?.Invoke(collectible, itemstack, blockSel, block, player, ref multiply);
        return multiply;
    }

    internal static void ExecuteMiningSpeedAttribute(IItemStack itemstack)
    {
        OnMiningSpeedAttributeRefreshed?.Invoke(itemstack);
    }
}