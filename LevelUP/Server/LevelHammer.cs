#pragma warning disable CA1822
using System.Collections.Generic;
using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace LevelUP.Server;

class LevelHammer
{
    public readonly Harmony patch = new("levelup_hammer");
    public void Patch()
    {
        if (!Harmony.HasAnyPatches("levelup_hammer"))
        {
            patch.PatchCategory("levelup_hammer");
        }
    }
    public void Unpatch()
    {
        if (Harmony.HasAnyPatches("levelup_hammer"))
        {
            patch.UnpatchCategory("levelup_hammer");
        }
    }

    public void Init()
    {
        Instance.api.Event.OnEntityDeath += OnEntityDeath;
        OverwriteDamageInteractionEvents.OnPlayerMeleeDoDamageStart += HandleDamage;
        Configuration.RegisterNewLevel("Hammer");
        Configuration.RegisterNewLevelTypeEXP("Hammer", Configuration.HammerGetLevelByEXP);
        Configuration.RegisterNewEXPLevelType("Hammer", Configuration.HammerGetExpByLevel);

        Debug.Log("Level Hammer initialized");
    }

    public void InitClient()
    {
        Debug.Log("Level Hammer initialized");
    }

    public void Dispose()
    {
        OverwriteDamageInteractionEvents.OnPlayerMeleeDoDamageStart -= HandleDamage;
    }

    private void HandleDamage(IPlayer player, DamageSource damageSource, ref float damage)
    {
        if (player.InventoryManager.ActiveTool == EnumTool.Hammer)
        {
            damage *= Configuration.HammerGetDamageMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Hammer"));
            Experience.IncreaseExperience(player, "Hammer", "Hit");
        }
    }

    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulateHammerConfiguration(coreAPI);
        Configuration.RegisterNewMaxLevelByLevelTypeEXP("Hammer", Configuration.hammerMaxLevel);
    }

    public void OnEntityDeath(Entity entity, DamageSource damageSource)
    {
        // Error treatment
        if (damageSource == null) return;
        // The cause of the death is from a projectile
        if (damageSource.GetCauseEntity() is not EntityPlayer && damageSource.SourceEntity is EntityProjectile) return;
        // Entity kill is not from a player
        if (damageSource.SourceEntity is not EntityPlayer) return;

        // Get player entity
        EntityPlayer playerEntity = damageSource.SourceEntity as EntityPlayer;

        // Get player instance
        IPlayer player = playerEntity.Player;

        // Check if player is using a Hammer
        if (player.InventoryManager.ActiveTool != EnumTool.Hammer) return;

        ulong exp = (ulong)Configuration.entityExpHammer.GetValueOrDefault(entity.Code.ToString());

        // Get the actual player total exp
        ulong playerExp = Experience.GetExperience(player, "Hammer");

        Debug.LogDebug($"{player.PlayerName} killed: {entity.Code}, hammer exp earned: {exp}, actual: {playerExp}");

        // Incrementing
        Experience.IncreaseExperience(player, "Hammer", exp);
    }

    [HarmonyPatchCategory("levelup_hammer")]
    private class LevelHammerPatch
    {
        // Overwrite the hammer smithing
        [HarmonyPrefix]
        [HarmonyPatch(typeof(BlockEntityAnvil), "CheckIfFinished")]
        internal static void CheckIfFinished(BlockEntityAnvil __instance, IPlayer byPlayer)
        {
            if (!Configuration.enableLevelHammer) return;
            if (__instance.Api.Side != EnumAppSide.Server) return;

            // Error treatment
            if (__instance.SelectedRecipe?.Output?.ResolvedItemstack != null || byPlayer != null)
            {
                bool MatchesRecipe()
                {
                    #region native
                    if (__instance.SelectedRecipe == null)
                    {
                        return false;
                    }
                    int ymax = GameMath.Min(6, __instance.SelectedRecipe.QuantityLayers);
                    bool[,,] recipeVoxels = __instance.recipeVoxels;
                    for (int x = 0; x < 16; x++)
                    {
                        for (int y = 0; y < ymax; y++)
                        {
                            for (int z = 0; z < 16; z++)
                            {
                                byte desiredMat = (byte)(recipeVoxels[x, y, z] ? 1u : 0u);
                                if (__instance.Voxels[x, y, z] != desiredMat)
                                {
                                    return false;
                                }
                            }
                        }
                    }
                    return true;
                    #endregion
                }

                // Check if is finished
                if (__instance.SelectedRecipe != null && MatchesRecipe() && __instance.Api.World is IServerWorldAccessor)
                {
                    // Get player hammer level
                    int multiply = Configuration.HammerGetResultMultiplyByLevel(byPlayer.Entity.WatchedAttributes.GetInt("LevelUP_Level_Hammer"));

                    if (__instance.SelectedRecipe.Output != null && __instance.SelectedRecipe.Output.ResolvedItemstack != null)
                    {
                        // Integration
                        multiply = LevelHammerEvents.GetExternalHammerMultiply(byPlayer, __instance.SelectedRecipe.Output.ResolvedItemstack.Collectible?.Code?.ToString(), multiply);

                        // Multiply by the chance
                        __instance.SelectedRecipe.Output.ResolvedItemstack.StackSize = __instance.SelectedRecipe.Output.Quantity * multiply;

                        Debug.LogDebug($"{byPlayer.PlayerName} finished smithing {__instance.SelectedRecipe.Output?.ResolvedItemstack?.Collectible?.Code} drop quantity: {__instance.SelectedRecipe.Output?.Quantity} with a final result size of: {__instance.SelectedRecipe.Output?.ResolvedItemstack?.StackSize} multiplied by: {multiply}");
                    }
                }

                // Check if player is using the hammer
                if (byPlayer?.InventoryManager?.ActiveTool == EnumTool.Hammer)
                    Experience.IncreaseExperience(byPlayer, "Hammer", "Hit");
            }
        }

        // Overwrite the hammer split
        [HarmonyPrefix]
        [HarmonyPatch(typeof(BlockEntityAnvil), "OnUseOver")]
        [HarmonyPatch("OnUseOver", [typeof(IPlayer), typeof(Vec3i), typeof(BlockSelection)])] // This is necessary because there is 2 functions with the same name
        internal static void OnUseOver(BlockEntityAnvil __instance, IPlayer byPlayer, Vec3i voxelPos, BlockSelection blockSel)
        {
            if (!Configuration.enableLevelHammer) return;
            if (__instance.Api.Side != EnumAppSide.Server) return;

            // Check if the weapon is on split mode
            if (byPlayer.InventoryManager.ActiveHotbarSlot?.Itemstack.Collectible.GetToolMode(byPlayer.InventoryManager.ActiveHotbarSlot, byPlayer, blockSel) == 5)
            {
                foreach (KeyValuePair<string, string> keyValue in Configuration.smithChanceHammer)
                {
                    // Check if currently recipe matchs the smith chance
                    if (__instance.SelectedRecipe.Output.Code.ToString().Contains(keyValue.Key))
                    {
                        bool shouldRetrieve = Configuration.HammerShouldRetrieveSmithByLevel(byPlayer.Entity.WatchedAttributes.GetInt("LevelUP_Level_Hammer"));
                        ItemStack itemToRetrieve = new(__instance.Api.World.GetItem(new AssetLocation(keyValue.Value)));

                        LevelHammerEvents.UpdateFromExternalHammerSplit(byPlayer, ref shouldRetrieve, ref itemToRetrieve);

                        // Alright its match lets get the chance
                        if (shouldRetrieve)
                        {
                            // The chance returned true so we need to retrieve the player the item
                            byPlayer.Entity.TryGiveItemStack(itemToRetrieve);
                        }
                    }
                }
            }
        }

    }
}

public class LevelHammerEvents
{
    public delegate void PlayerHammerItemHandler(IPlayer player, string code, ref int multiply);
    public delegate void PlayerHammerSplitHandler(IPlayer player, ref bool shouldRetrieve, ref ItemStack itemToRetrieve);

    public static event PlayerHammerItemHandler OnHammerItem;
    public static event PlayerHammerSplitHandler OnHammerSmith;

    internal static int GetExternalHammerMultiply(IPlayer player, string code, int multiply)
    {
        OnHammerItem?.Invoke(player, code, ref multiply);
        return multiply;
    }

    internal static void UpdateFromExternalHammerSplit(IPlayer player, ref bool shouldRetrieve, ref ItemStack itemToRetrieve)
    {
        OnHammerSmith?.Invoke(player, ref shouldRetrieve, ref itemToRetrieve);
    }
}