#pragma warning disable IDE0060
using HarmonyLib;
using Vintagestory.API.Common;

namespace LevelUP.Server;

[HarmonyPatchCategory("levelup_blockbreak")]
class OverwriteBlockBreak
{
    public readonly Harmony patch = new("levelup_blockbreak");
    public void Patch()
    {
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

    [HarmonyPostfix] // Client Side
    [HarmonyPatch(typeof(CollectibleObject), "GetMiningSpeed")]
    [HarmonyPriority(Priority.VeryLow)]
    internal static float GetMiningSpeed(float __result, IItemStack itemstack, BlockSelection blockSel, Block block, IPlayer forPlayer)
    {
        if (forPlayer == null) return __result;

        switch (forPlayer.InventoryManager.ActiveTool)
        {
            case EnumTool.Axe:
                if (block.BlockMaterial == EnumBlockMaterial.Wood)
                    return Configuration.enableLevelAxe ? __result * forPlayer.Entity.WatchedAttributes.GetFloat("LevelUP_Axe_MiningSpeed", 1.0f) : __result;
                else return 1.0f;
            case EnumTool.Pickaxe:
                if (block.BlockMaterial == EnumBlockMaterial.Brick ||
                    block.BlockMaterial == EnumBlockMaterial.Ceramic ||
                    block.BlockMaterial == EnumBlockMaterial.Metal ||
                    block.BlockMaterial == EnumBlockMaterial.Ore ||
                    block.BlockMaterial == EnumBlockMaterial.Stone)
                    return Configuration.enableLevelPickaxe ? __result * forPlayer.Entity.WatchedAttributes.GetFloat("LevelUP_Pickaxe_MiningSpeed", 1.0f) : __result;
                else return 1.0f;
            case EnumTool.Shovel:
                if (block.BlockMaterial == EnumBlockMaterial.Soil ||
                    block.BlockMaterial == EnumBlockMaterial.Gravel ||
                    block.BlockMaterial == EnumBlockMaterial.Sand ||
                    block.BlockMaterial == EnumBlockMaterial.Snow)
                    return Configuration.enableLevelShovel ? __result * forPlayer.Entity.WatchedAttributes.GetFloat("LevelUP_Shovel_MiningSpeed", 1.0f) : __result;
                else return 1.0f;
            case EnumTool.Knife:
                if (block.BlockMaterial == EnumBlockMaterial.Plant ||
                    block.BlockMaterial == EnumBlockMaterial.Leaves)
                    return Configuration.enableLevelKnife ? __result * forPlayer.Entity.WatchedAttributes.GetFloat("LevelUP_Knife_MiningSpeed", 1.0f) : __result;
                else return 1.0f;
        }
        return __result;
    }
}