#pragma warning disable CA1822
using System.Collections.Generic;
using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.API.Util;
using Vintagestory.GameContent;

namespace LevelUP.Server;

class LevelPanning
{
    public readonly Harmony patch = new("levelup_panning");
    public void Patch()
    {
        if (!Harmony.HasAnyPatches("levelup_panning"))
        {
            patch.PatchCategory("levelup_panning");
        }
    }
    public void Unpatch()
    {
        if (Harmony.HasAnyPatches("levelup_panning"))
        {
            patch.UnpatchCategory("levelup_panning");
        }
    }

    public void Init()
    {
        Configuration.RegisterNewLevel("Panning");
        Configuration.RegisterNewLevelTypeEXP("Panning", Configuration.PanningGetLevelByEXP);
        Configuration.RegisterNewEXPLevelType("Panning", Configuration.PanningGetExpByLevel);

        Debug.Log("Level Panning initialized");
    }

    public void InitClient()
    {
        Debug.Log("Level Panning initialized");
    }

    public void Dispose()
    { }

    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulatePanningConfiguration(coreAPI);
        Configuration.RegisterNewMaxLevelByLevelTypeEXP("Panning", Configuration.panningMaxLevel);
    }

    [HarmonyPatchCategory("levelup_panning")]
    private class LevelPanningPatch
    {
        // Overwrite Panning mechanic
        [HarmonyPrefix]
        [HarmonyPatch(typeof(BlockPan), "CreateDrop")]
        public static bool CreateDrop(BlockPan __instance, EntityAgent byEntity, string fromBlockCode)
        {
            if (!Configuration.enableLevelPanning) return true;
            if (byEntity.Api.Side != EnumAppSide.Server) return true;

            // Recreation of a private function in BlockPan instance
            ItemStack Resolve(EnumItemClass type, string code)
            {
                if (type == EnumItemClass.Block)
                {
                    Block block = byEntity.Api.World.GetBlock(new AssetLocation(code));
                    if (block == null)
                    {
                        byEntity.Api.World.Logger.Error("Failed resolving panning block drop with code {0}. Will skip.", code);
                        return null;
                    }
                    return new ItemStack(block);
                }
                Item item = byEntity.Api.World.GetItem(new AssetLocation(code));
                if (item == null)
                {
                    byEntity.Api.World.Logger.Error("Failed resolving panning item drop with code {0}. Will skip.", code);
                    return null;
                }
                return new ItemStack(item);
            }

            var dropsField = AccessTools.Field(typeof(BlockPan), "dropsBySourceMat");
            var dropsBySourceMat = (Dictionary<string, PanningDrop[]>)dropsField.GetValue(__instance);

            // Now we are changing the panning code droprate
            {
                IPlayer player = (byEntity as EntityPlayer)?.Player;
                PanningDrop[] drops = null;
                foreach (string val2 in dropsBySourceMat.Keys)
                {
                    if (WildcardUtil.Match(val2, fromBlockCode))
                    {
                        drops = dropsBySourceMat[val2];
                    }
                }
                if (drops == null)
                {
                    Debug.LogError($"Coding error, no drops defined for source mat {fromBlockCode}");
                    return true;
                }
                string rocktype = byEntity.Api.World.GetBlock(new AssetLocation(fromBlockCode))?.Variant["rock"];
                drops.Shuffle(byEntity.Api.World.Rand);
                for (int i = 0; i < drops.Length; i++)
                {
                    PanningDrop drop = drops[i];
                    double num = byEntity.Api.World.Rand.NextDouble();
                    float extraMul = 1f;
                    if (drop.DropModbyStat != null)
                    {
                        extraMul = byEntity.Stats.GetBlended(drop.DropModbyStat);
                    }
                    // Increasing chance to drop a item
                    extraMul += extraMul * Configuration.PanningGetLootMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Panning"));
                    float val = drop.Chance.nextFloat() * extraMul;
                    ItemStack stack = drop.ResolvedItemstack;
                    if (drops[i].Code.Path.Contains("{rocktype}"))
                    {
                        stack = Resolve(drops[i].Type, drops[i].Code.Path.Replace("{rocktype}", rocktype));
                    }
                    // Clone the stack before
                    stack = stack.Clone();

                    // Multiplying drop quantity
                    stack.StackSize += stack.StackSize * Configuration.PanningGetLootQuantityMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Panning"));

                    // Integration
                    LevelPanningEvents.UpdateFromExternalPanning(player, ref val, ref stack);

                    if (num < (double)val && stack != null)
                    {
                        if (player == null || !player.InventoryManager.TryGiveItemstack(stack, slotNotifyEffect: true))
                        {
                            byEntity.Api.World.SpawnItemEntity(stack, byEntity.ServerPos.XYZ);
                        }
                        break;
                    }
                }

                // Increasing exp for panning                
                Experience.IncreaseExperience(player, "Panning", "Panning");
            }

            return false;
        }
    }
}

public class LevelPanningEvents
{
    public delegate void PlayerPanningHandler(IPlayer player, ref float chance, ref ItemStack itemStack);

    public static event PlayerPanningHandler OnPanning;

    internal static void UpdateFromExternalPanning(IPlayer player, ref float chance, ref ItemStack itemStack)
    {
        OnPanning?.Invoke(player, ref chance, ref itemStack);
    }
}