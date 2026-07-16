#pragma warning disable CA1822
using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.GameContent;

namespace LevelUP.Server;

class OverwriteAimingAccuracy
{
    public readonly Harmony patch = new("levelup_aimingaccuracy");

    public void Patch()
    {
        if (!Harmony.HasAnyPatches("levelup_aimingaccuracy"))
            patch.PatchCategory("levelup_aimingaccuracy");
    }

    public void Unpatch()
    {
        if (Harmony.HasAnyPatches("levelup_aimingaccuracy"))
            patch.UnpatchCategory("levelup_aimingaccuracy");
    }

    [HarmonyPatchCategory("levelup_aimingaccuracy")]
    private class AimingAccuracyPatches
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MovingAimingAccuracy), "Update")]
        internal static void MovingUpdate(object __instance, float dt, ref float accuracy)
            => ApplyReduction(__instance, ref accuracy);

        [HarmonyPostfix]
        [HarmonyPatch(typeof(SprintAimingAccuracy), "Update")]
        internal static void SprintUpdate(object __instance, float dt, ref float accuracy)
            => ApplyReduction(__instance, ref accuracy);

        private static void ApplyReduction(object __instance, ref float accuracy)
        {
            EntityAgent entity = Traverse.Create(__instance).Field<EntityAgent>("entity").Value;
            if (entity is not EntityPlayer playerEntity) return;

            float reduction = GetReduction(playerEntity);
            if (reduction <= 0f) return;

            float accuracyPenalty = Traverse.Create(__instance).Field<float>("accuracyPenalty").Value;
            if (accuracyPenalty <= 0f) return;

            float blended = entity.Stats.GetBlended("rangedWeaponsAcc");
            float penaltyApplied = accuracyPenalty / System.Math.Max(1f, blended);

            accuracy += penaltyApplied * System.Math.Min(1f, reduction);
        }

        private static float GetReduction(EntityPlayer playerEntity)
        {
            EnumTool? tool = playerEntity.RightHandItemSlot?.Itemstack?.Item?.Tool;

            if (tool == EnumTool.Bow && Configuration.enableLevelBow)
                return Configuration.BowGetMovePenaltyReductionByLevel(
                    playerEntity.WatchedAttributes.GetInt("LevelUP_Level_Bow"));

            if (tool == EnumTool.Spear && Configuration.enableLevelSpear)
                return Configuration.SpearGetMovePenaltyReductionByLevel(
                    playerEntity.WatchedAttributes.GetInt("LevelUP_Level_Spear"));

            return 0f;
        }
    }
}
