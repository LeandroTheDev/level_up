using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using LevelUP.Server;
using Newtonsoft.Json.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.API.Util;
using Vintagestory.Common;
using Vintagestory.GameContent;

namespace LevelUP.Shared;

#pragma warning disable IDE0060
[HarmonyPatchCategory("levelup_block_interaction")]
public class OverwriteBlockInteraction
{
    private static Instance instance;
    internal Harmony overwriter;

    internal void OverwriteNativeFunctions(Instance _instance)
    {
        instance = _instance;
        instance.ToString(); //Suppress Alerts
        if (!Harmony.HasAnyPatches("levelup_block_interaction"))
        {
            overwriter = new Harmony("levelup_block_interaction");
            overwriter.PatchCategory("levelup_block_interaction");
            Debug.Log("Block interaction has been overwrited");
        }
        else
        {
            if (instance.side == EnumAppSide.Client) Debug.Log("Block break overwriter has already patched, probably by the singleplayer server");
            else Debug.LogError("ERROR: Block interaction overwriter has already patched, did some mod already has levelup_block_interaction in harmony?");
        }
    }

    #region knife
    // Overwrite Knife Harvesting
    [HarmonyPrefix]
    [HarmonyPatch(typeof(EntityBehaviorHarvestable), "GenerateDrops")]
    public static void GenerateDropsStart(EntityBehaviorHarvestable __instance, IPlayer byPlayer)
    {
        if (!Configuration.enableLevelKnife) return;
        if (__instance.entity.World.Side != EnumAppSide.Server) return;

        // Get the final droprate
        float dropRate = Configuration.KnifeGetHarvestMultiplyByLevel(byPlayer.Entity.WatchedAttributes.GetInt("LevelUP_Level_Knife"));

        // Integration
        dropRate = OverwriteBlockInteractionEvents.GetExternalKnifeHarvest(byPlayer, dropRate);

        // Don't worry, it will be reseted automatically by the game
        // For some reason the -1 is necessary for this Set function so it will calculated at 0
        byPlayer.Entity.Stats.Set("animalLootDropRate", "animalLootDropRate", dropRate - 1f);

        // Earn xp by harvesting the entity
        Experience.IncreaseExperience(byPlayer, "Knife", "Harvest");

        Debug.LogDebug($"{byPlayer.PlayerName} harvested any entity with knife, multiply drop: {dropRate}/{byPlayer.Entity.Stats.GetBlended("animalLootDropRate")}");
    }
    #endregion

    #region farming
    // Overwrite Hoe Till
    [HarmonyPostfix]
    [HarmonyPatch(typeof(ItemHoe), "OnHeldInteractStep")]
    public static void OnHeldInteractStep(bool __result, float secondsUsed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel)
    {
        if (!Configuration.enableLevelFarming) return;

        // Check if soil is tilled and is from the server
        if (byEntity.World.Side == EnumAppSide.Server && secondsUsed >= 1.0f)
        {
            // Check if is a player
            if (byEntity is EntityPlayer playerEntity)
            {
                // Integration
                OverwriteBlockInteractionEvents.ExecuteHoeTill(playerEntity.Player);

                // Earn xp by tilling the soil
                Experience.IncreaseExperience(playerEntity.Player, "Farming", "Till");
            }
        }
    }

    // Overwrite Berry Forage while breaking
    [HarmonyPrefix]
    [HarmonyPatch(typeof(BlockBerryBush), "GetDrops")]
    public static void GetDropsBerryBushFinish(BlockBerryBush __instance, IWorldAccessor world, BlockPos pos, IPlayer byPlayer, ref float dropQuantityMultiplier)
    {
        if (!Configuration.enableLevelFarming) return;
        if (byPlayer == null || world.Side != EnumAppSide.Server) return;

        // Increasing the quantity drop multiply by the farming level
        float multiply = Configuration.FarmingGetForageMultiplyByLevel(byPlayer.Entity.WatchedAttributes.GetInt("LevelUP_Level_Farming"));

        Debug.LogDebug($"{byPlayer.PlayerName} bush harvest: {__instance.Code}, by breaking multiply of: {multiply}");

        ulong exp = 0;

        // Check the berry existence
        if (Configuration.expPerHarvestFarming.TryGetValue(__instance.Code.ToString(), out int intExp))
            exp = (ulong)intExp;

        OverwriteBlockInteractionEvents.UpdateFromExternalFarmForage(byPlayer, __instance.Code.ToString(), ref exp, ref multiply);

        if (exp > 0)
            Experience.IncreaseExperience(byPlayer, "Farming", exp);

        // Don't worry, it will be reseted automatically by the game
        // For some reason the -1 is necessary for this Set function so it will calculated at 0
        byPlayer.Entity.Stats.Set("forageDropRate", "forageDropRate", multiply - 1f);

    }

    // Overwrite Berry Forage while interacting
    [HarmonyPrefix]
    [HarmonyPatch(typeof(BlockBehaviorHarvestable), "OnBlockInteractStop")]
    public static void OnBlockInteractStopStart(BlockBehaviorHarvestable __instance, float secondsUsed, IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel, ref EnumHandling handled)
    {
        if (!Configuration.enableLevelFarming) return;
        if (byPlayer != null && world.Side != EnumAppSide.Server) return;

        ulong exp = 0;
        float multiply = Configuration.FarmingGetForageMultiplyByLevel(byPlayer.Entity.WatchedAttributes.GetInt("LevelUP_Level_Farming"));

        // Check the berry existence
        if (Configuration.expPerHarvestFarming.TryGetValue(__instance.block.Code.ToString(), out int intExp))
            exp = (ulong)intExp;

        OverwriteBlockInteractionEvents.UpdateFromExternalFarmForage(byPlayer, __instance.block.Code.ToString(), ref exp, ref multiply);

        if (exp > 0)
            Experience.IncreaseExperience(byPlayer, "Farming", exp);

        // Don't worry, it will be reseted automatically by the game
        // For some reason the -1 is necessary for this Set function so it will calculated at 0
        byPlayer.Entity.Stats.Set("forageDropRate", "forageDropRate", multiply - 1f);

        Debug.LogDebug($"{byPlayer.PlayerName} bush harvest: {__instance.block.Code}, by right clicking multiply of: {multiply}/{byPlayer.Entity.Stats.GetBlended("forageDropRate")}");
    }

    // Overwrite Mushroom Forage
    [HarmonyPostfix]
    [HarmonyPatch(typeof(BlockMushroom), "GetDrops")]
    public static void GetDropsMushroomFinish(BlockMushroom __instance, IWorldAccessor world, BlockPos pos, IPlayer byPlayer, ref float dropQuantityMultiplier)
    {
        if (!Configuration.enableLevelFarming) return;
        if (byPlayer == null || world.Side != EnumAppSide.Server) return;

        // Increasing the quantity drop multiply by the farming level
        ulong exp = 0;
        float multiply = Configuration.FarmingGetForageMultiplyByLevel(byPlayer.Entity.WatchedAttributes.GetInt("LevelUP_Level_Farming"));

        // Check the berry existence
        if (Configuration.expPerHarvestFarming.TryGetValue(__instance.Code.ToString(), out int intExp))
            exp = (ulong)intExp;

        OverwriteBlockInteractionEvents.UpdateFromExternalFarmForage(byPlayer, __instance.Code.ToString(), ref exp, ref multiply);

        if (exp > 0)
            Experience.IncreaseExperience(byPlayer, "Farming", exp);

        // Don't worry, it will be reseted automatically by the game
        // For some reason the -1 is necessary for this Set function so it will calculated at 0
        byPlayer.Entity.Stats.Set("forageDropRate", "forageDropRate", multiply - 1f);

        Debug.LogDebug($"{byPlayer.PlayerName} bush harvest: {__instance.Code} multiply: {multiply}/{byPlayer.Entity.Stats.GetBlended("forageDropRate")}");
    }
    #endregion

    #region cooking
    // Overwrite Fire Pit
    [HarmonyPostfix]
    [HarmonyPatch(typeof(BlockEntityFirepit), "heatInput")]
    public static void HeatInput(BlockEntityFirepit __instance, float dt)
    {
        if (!Configuration.enableLevelCooking) return;
        if (__instance.Api.World.Side != EnumAppSide.Server) return;

        // Hol up, let him cook
        float maxCookingTime = __instance.inputSlot.Itemstack.Collectible.GetMeltingDuration(__instance.Api.World, (ISlotProvider)__instance.Inventory, __instance.inputSlot);
        float cookingTime = __instance.inputStackCookingTime;

        if ((int)cookingTime % 10 == 0 && cookingTime > 0 && cookingTime < maxCookingTime)
            Debug.LogDebug($"Cooking: {cookingTime} / {maxCookingTime}");

        // Check if him finished cooking
        if (cookingTime >= maxCookingTime)
        {
            // Check if input stack exists on exp earn, this means the player is reheating the food, disabling the experience mechanic
            if (Configuration.expMultiplySingleCooking.TryGetValue(__instance.inputStack.Collectible.Code.ToString(), out double _)) return;
            else if (Configuration.expMultiplyPotsCooking.TryGetValue(__instance.inputStack.Collectible.Code.ToString(), out double _)) return;

            // Check if the output existed before the cooking finished
            bool firstOutput = __instance.outputStack == null;

            Debug.LogDebug($"{__instance.inputStack.Collectible.Code} finished cooking, X: {__instance.Pos.X}, Y: {__instance.Pos.Y}, Z: {__instance.Pos.Z}");

            // Run on secondary thread to not freeze the server
            // This is necessary because we have a loop to receive the outputStack
            Task.Run(async () =>
            {
                Debug.LogDebug("Thread created, waiting for cooking outputStack...");

                // Because output is magically added by something we need to constantly check it
                int tries = 0;
                while (__instance.outputStack == null && tries < 3)
                {
                    await Task.Delay(50);
                    tries++;
                }
                // After that time if the output stack is still null we just give up
                if (__instance.outputStack == null)
                {
                    if (Configuration.enableExtendedLog)
                        Debug.LogWarn($"[COOKING] Server is overloaded? someone finished cooking but the output is still null, or maybe a ninja pickup the food in 50 milliseconds after completion");
                    return;
                }

                // Finally receive output
                ItemStack output = __instance.outputStack;

                Debug.LogDebug($"Cooking outputStack: {output.Collectible.Code}, cooking position: X: {__instance.Pos.X}, Y: {__instance.Pos.Y}, Z: {__instance.Pos.Z}");

                // Update player experience to the most proximity player
                IPlayer player = __instance.Api.World.NearestPlayer(__instance.Pos.X, __instance.Pos.Y, __instance.Pos.Z);

                // If cannot find the nearest player
                if (player == null)
                {
                    Debug.LogDebug("Cooking: player is null, cooking experience and stats has been ignored");
                    return;
                }

                // For single cooking
                if (Configuration.expMultiplySingleCooking.TryGetValue(output.Collectible.Code.ToString(), out double expMultiplySingle))
                {
                    ulong exp = (ulong)Math.Round(Configuration.ExpPerCookingcooking + (Configuration.ExpPerCookingcooking * expMultiplySingle));

                    if (firstOutput)
                    {
                        // Increase the fresh hours based in player experience
                        TreeAttribute attribute = output.Attributes["transitionstate"] as TreeAttribute;
                        FloatArrayAttribute freshHours = attribute.GetAttribute("freshHours") as FloatArrayAttribute;
                        Debug.LogDebug($"Cooking: previously fresh hours: {freshHours.value[0]}");
                        freshHours.value[0] *= Configuration.CookingGetFreshHoursMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Cooking"));
                        OverwriteBlockInteractionEvents.UpdateFromExternalCookingSingle(player, output.Collectible.Code.ToString(), ref exp, ref freshHours.value[0]);
                        Debug.LogDebug($"Cooking: fresh hours increased to: {freshHours.value[0]} with multiply of {Configuration.CookingGetFreshHoursMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Cooking"))}");
                        attribute.SetAttribute("freshHours", freshHours);
                        output.Attributes["transitionstate"] = attribute;
                    }

                    Experience.IncreaseExperience(player, "Cooking", exp);
                }
                // For pots cooking
                else if (Configuration.expMultiplyPotsCooking.TryGetValue(output.Collectible.Code.ToString(), out double expMultiplyPots))
                {
                    ulong exp = (ulong)Math.Round(Configuration.ExpPerCookingcooking + (Configuration.ExpPerCookingcooking * expMultiplyPots));

                    if (firstOutput)
                    {
                        List<float> indexFreshHours = [];

                        // Getting fresh hours index first, for the integration
                        {
                            TreeAttribute attribute = output.Attributes["contents"] as TreeAttribute;

                            // Swipe all foods in inventory
                            foreach (var contents in attribute)
                            {
                                ItemstackAttribute contentAttribute = contents.Value as ItemstackAttribute;
                                ItemStack item = contentAttribute.value;

                                // Get food datas
                                TreeAttribute itemAttribute = item.Attributes["transitionstate"] as TreeAttribute;
                                FloatArrayAttribute freshHours = itemAttribute.GetAttribute("freshHours") as FloatArrayAttribute;

                                Debug.LogDebug($"Cooking: previously fresh hours: {freshHours.value[0]}");

                                // Increase fresh hours by levelup
                                freshHours.value[0] *= Configuration.CookingGetFreshHoursMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Cooking"));

                                // Integration
                                indexFreshHours.Add(freshHours.value[0]);
                            }
                        }
                        // Increase servings quantity
                        {
                            // Get data
                            TreeAttribute attribute = output.Attributes as TreeAttribute;
                            // Get the servings quantity
                            FloatAttribute servingsQuantity = attribute["quantityServings"] as FloatAttribute;

                            Debug.LogDebug($"Cooking: previously servings: {servingsQuantity.value}");

                            // Increasing servings quantity
                            servingsQuantity.value = Configuration.CookingGetServingsByLevelAndServings(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Cooking"), (int)servingsQuantity.value); ;

                            Debug.LogDebug($"Cooking: servings now: {servingsQuantity.value}");

                            // Integration
                            OverwriteBlockInteractionEvents.UpdateFromExternalCookingPot(player, output.Collectible.Code.ToString(), ref exp, ref indexFreshHours, ref servingsQuantity.value);

                            // Updating fresh hours
                            {
                                // For pots the fresh foods is stored as raw in the pot
                                // by knowing that we need to increase fresh hours foreach inventory slot from this pot
                                TreeAttribute attributeFreshHours = output.Attributes["contents"] as TreeAttribute;

                                Debug.LogDebug("Increasing cooking ingredients fresh hours...");

                                // Swipe all foods in inventory
                                int i = 0;
                                foreach (var contents in attributeFreshHours)
                                {
                                    ItemstackAttribute contentAttribute = contents.Value as ItemstackAttribute;
                                    ItemStack item = contentAttribute.value;

                                    // Get food datas
                                    TreeAttribute itemAttribute = item.Attributes["transitionstate"] as TreeAttribute;
                                    FloatArrayAttribute freshHours = itemAttribute.GetAttribute("freshHours") as FloatArrayAttribute;

                                    Debug.LogDebug($"Cooking: previously fresh hours: {freshHours.value[0]}");

                                    // Increase fresh hours
                                    freshHours.value[0] = indexFreshHours[i];
                                    freshHours.value[0] *= Configuration.CookingGetFreshHoursMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Cooking"));

                                    Debug.LogDebug($"Cooking: fresh hours increased to: {freshHours.value[0]} with multiply of {Configuration.CookingGetFreshHoursMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Cooking"))}");

                                    // Updating
                                    itemAttribute.SetAttribute("freshHours", freshHours);
                                    item.Attributes["transitionstate"] = itemAttribute;
                                    contentAttribute.value = item;

                                    i++;
                                }
                                output.Attributes["contents"] = attributeFreshHours;
                            }

                            // Updating servings
                            attribute["quantityServings"] = servingsQuantity;
                            output.Attributes = attribute;
                        }
                    }

                    Experience.IncreaseExperience(player, "Cooking", exp);
                }
                // Unkown
                else
                {
                    Debug.LogWarn($"[COOKING] Unkown recipe, giving default experience to {player.PlayerName}, recipe: {output.Collectible.Code}, please add in configs/levelstats/cookingpots.json or configs/levelstats/cookingsingles.json");
                    Experience.IncreaseExperience(player, "Cooking", (ulong)Configuration.ExpPerCookingcooking);
                }
            });
        }
    }
    #endregion

    #region hammer
    // Overwrite the hammer smithing
    [HarmonyPrefix]
    [HarmonyPatch(typeof(BlockEntityAnvil), "CheckIfFinished")]
    public static void CheckIfFinished(BlockEntityAnvil __instance, IPlayer byPlayer)
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
                int ymax = Math.Min(6, __instance.SelectedRecipe.QuantityLayers);
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
                    multiply = OverwriteBlockInteractionEvents.GetExternalHammerMultiply(byPlayer, __instance.SelectedRecipe.Output.ResolvedItemstack.Collectible?.Code?.ToString(), multiply);

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
    public static void OnUseOver(BlockEntityAnvil __instance, IPlayer byPlayer, Vec3i voxelPos, BlockSelection blockSel)
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

                    OverwriteBlockInteractionEvents.UpdateFromExternalHammerSplit(byPlayer, ref shouldRetrieve, ref itemToRetrieve);

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
    #endregion

    #region panning
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

        // Unfurtunally the dev set the dropsBySourceMat as private, we cannot access it in normal ways
        // the best we can do is to recreate it again...
        var dropsBySourceMat = __instance.Attributes["panningDrops"].AsObject<Dictionary<string, PanningDrop[]>>();
        {
            foreach (PanningDrop[] drops in dropsBySourceMat.Values)
            {
                for (int i = 0; i < drops.Length; i++)
                {
                    if (!drops[i].Code.Path.Contains("{rocktype}"))
                    {
                        // Checking if is the server
                        drops[i].Resolve(byEntity.Api.World, "panningdrop");
                    }
                }
            }
        }

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
                throw new InvalidOperationException("Coding error, no drops defined for source mat " + fromBlockCode);
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
                OverwriteBlockInteractionEvents.UpdateFromExternalPanning(player, ref val, ref stack);

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

    #endregion

    #region smithing

    // Luckly the durability, miningspeed and attackpower is unique by the item
    // so we just save the attribute and change it to be shared with the client and update the viewbox

    // Overwrite Craft
    [HarmonyPostfix]
    [HarmonyPatch(typeof(ItemSlotCraftingOutput), "CraftSingle")]
    internal static void CraftSingleFinish(ItemSlotCraftingOutput __instance, ItemSlot sinkSlot, ref ItemStackMoveOperation op)
    {
        if (!Configuration.enableLevelSmithing) return;
        if (op.World.Api.Side != EnumAppSide.Server) return;
        if (sinkSlot == null || sinkSlot.Itemstack == null) return;
        if (op.ActingPlayer == null) return;

        sinkSlot.Itemstack = ExecuteSmithItemCraftedCalculations(op.ActingPlayer, sinkSlot.Itemstack);
        sinkSlot.MarkDirty();
    }

    // Overwrite Craft Multiples
    [HarmonyPostfix]
    [HarmonyPatch(typeof(ItemSlotCraftingOutput), "CraftMany")]
    internal static void CraftManyFinish(ItemSlotCraftingOutput __instance, ItemSlot sinkSlot, ref ItemStackMoveOperation op)
    {
        if (!Configuration.enableLevelSmithing) return;
        if (op.World.Api.Side != EnumAppSide.Server) return;
        if (sinkSlot == null || sinkSlot.Itemstack == null) return;
        if (op.ActingPlayer == null) return;

        sinkSlot.Itemstack = ExecuteSmithItemCraftedCalculations(op.ActingPlayer, sinkSlot.Itemstack);
        sinkSlot.MarkDirty();
    }

    // Overwrite Visual Max Durability
    // This is necessary so the durability system is more accurate
    [HarmonyPostfix]
    [HarmonyPatch(typeof(CollectibleObject), "GetMaxDurability")]
    internal static void GetMaxDurabilityFinish(ItemStack itemstack, ref int __result)
    {
        int maxDurability = itemstack.Attributes.GetInt("maxdurability", -1);
        if (maxDurability != -1)
        {
            __result = maxDurability;
        }
    }

    /// In the next part of the code, we will edit the view of the client to show
    /// the modified protection (GetHeldArmorInfoStart), because vintage story share the ProtectionModifiers
    /// between all items of the same type we can't edit and modify a unique item,
    /// so every time a player handle the armor damage we edit the ProtectionModifier
    /// based on the attribute set in craft (HandleDamagedStart), this will refresh
    /// the ProtectionModifiers every time it will be used, making the item "unique"
    /// 
    /// The same happens for attackPower and miningSpeed

    // Overwrite Visual Protections
    // This is necessary so the protection system is more accurate
    [HarmonyPrefix]
    [HarmonyPatch(typeof(ItemWearable), "GetHeldItemInfo")]
    internal static void GetHeldArmorInfoStart(ItemWearable __instance, ItemSlot inSlot, StringBuilder dsc, IWorldAccessor world, bool withDebugInfo)
    {
        if (inSlot.Itemstack.Attributes.GetFloat("relativeProtection", -1f) != -1f)
            __instance.ProtectionModifiers.RelativeProtection = inSlot.Itemstack.Attributes.GetFloat("relativeProtection");

        if (inSlot.Itemstack.Attributes.GetFloat("flatDamageReduction", -1f) != -1f)
            __instance.ProtectionModifiers.FlatDamageReduction = inSlot.Itemstack.Attributes.GetFloat("flatDamageReduction");
    }

    // Overwrite Protection Damage Handle
    // This is necessary so the protection system is more accurate
    [HarmonyPrefix]
    [HarmonyPatch(typeof(ModSystemWearableStats), "handleDamaged")]
    internal static void HandleDamagedStart(ModSystemWearableStats __instance, IPlayer player, float damage, DamageSource dmgSource)
    {
        if (!Configuration.enableLevelSmithing) return;
        if (player.Entity.World.Side != EnumAppSide.Server) return;

        IInventory inv = player.InventoryManager.GetOwnInventory("character");

        // In the native code, only use the inventory 12,13,14 to calculate the damage protection,
        // also is random which part of the armor is used to be calculated, but we recalculate everthing
        // because we don't know what part will be used on the prefix
        for (int i = 12; i <= 14; i++)
        {
            ItemSlot armorSlot = inv[i];
            if (armorSlot.Itemstack?.Item is ItemWearable armorWearable)
            {
                if (armorWearable.ProtectionModifiers == null)
                {
                    Debug.LogDebug($"{player.PlayerName} {armorSlot.Itemstack.GetName()} Armor System ignored because ProtectionModifiers is null");
                    return;
                }

                // If the armor is created from a non player source, and a player can craft
                // the armor, they will be incosistent, so we need to refresh the default values
                // too unfurtunally

                Debug.LogDebug($"{player.PlayerName} {armorSlot.Itemstack.GetName()} Armor System Handling before R/F: {armorWearable.ProtectionModifiers.RelativeProtection}");

                // Only modify the relativeProtection if exist
                if (armorSlot.Itemstack.Attributes.GetFloat("relativeProtection", -1f) != -1f)
                    armorWearable.ProtectionModifiers.RelativeProtection = armorSlot.Itemstack.Attributes.GetFloat("relativeProtection");
                else // Otherwises we need to refresh from default
                {
                    if (armorSlot.Itemstack.Collectible.Attributes.KeyExists("protectionModifiers"))
                        if (armorSlot.Itemstack.Collectible.Attributes["protectionModifiers"].KeyExists("relativeProtection"))
                            armorWearable.ProtectionModifiers.RelativeProtection = armorSlot.Itemstack.Collectible.Attributes["protectionModifiers"]["relativeProtection"].AsFloat();
                }
                // Only modify the relativeProtection if exist
                if (armorSlot.Itemstack.Attributes.GetFloat("flatDamageReduction", -1f) != -1f)
                    armorWearable.ProtectionModifiers.FlatDamageReduction = armorSlot.Itemstack.Attributes.GetFloat("flatDamageReduction");
                else // Otherwises we need to refresh from default
                {
                    if (armorSlot.Itemstack.Collectible.Attributes.KeyExists("protectionModifiers"))
                        if (armorSlot.Itemstack.Collectible.Attributes["protectionModifiers"].KeyExists("relativeProtection"))
                            armorWearable.ProtectionModifiers.RelativeProtection = armorSlot.Itemstack.Collectible.Attributes["protectionModifiers"]["relativeProtection"].AsFloat();
                }

                Debug.LogDebug($"{player.PlayerName} {armorSlot.Itemstack.GetName()} Armor System Handling after R/F: {armorWearable.ProtectionModifiers.RelativeProtection}/{armorWearable.ProtectionModifiers.FlatDamageReduction}");
            }
        }
    }

    // Overwrite Visual and Interaction Attack Power
    // This is necessary so the attack power system is more accurate
    [HarmonyPostfix]
    [HarmonyPatch(typeof(CollectibleObject), "GetAttackPower")]
    internal static void GetAttackPowerFinish(ItemStack withItemStack, ref float __result)
    {
        float attackPower = withItemStack.Attributes.GetFloat("attackpower", -1f);
        if (attackPower != -1f)
        {
            __result = attackPower;
        }
    }

    // Overwrite Visual Mining Speed
    // This is necessary so the mining speed system is more accurate
    [HarmonyPrefix]
    [HarmonyPatch(typeof(CollectibleObject), "GetHeldItemInfo")]
    internal static void GetHeldItemInfoStart(CollectibleObject __instance, ItemSlot inSlot, StringBuilder dsc, IWorldAccessor world, bool withDebugInfo)
    {
        inSlot.Itemstack.Collectible.MiningSpeed?.Foreach(x =>
        {
            if (inSlot.Itemstack.Attributes.GetFloat($"{x.Key}_miningspeed", -1f) != -1f)
                inSlot.Itemstack.Collectible.MiningSpeed[x.Key] = inSlot.Itemstack.Attributes.GetFloat($"{x.Key}_miningspeed");
        });
    }

    // Overwrite Interaction Mining Speed
    [HarmonyPostfix]
    [HarmonyPatch(typeof(BlockBehavior), "GetMiningSpeedModifier")]
    [HarmonyPriority(Priority.VeryHigh)]
    internal static float GetMiningSpeedModifier(float __result, IWorldAccessor world, BlockPos pos, IPlayer byPlayer)
    {
        if (byPlayer == null) return __result;
        ItemStack equippedItemStack = byPlayer.InventoryManager.ActiveHotbarSlot.Itemstack;
        if (equippedItemStack == null) return __result;

        Block blockBreaking = world.GetBlockAccessor(false, false, false).GetBlock(pos);
        if (blockBreaking == null) return __result;

        float miningSpeed = equippedItemStack.Attributes.GetFloat($"{blockBreaking.BlockMaterial}_miningspeed", -1f);
        if (miningSpeed == -1f) return __result;
        else __result = miningSpeed;

        return __result;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(CollectibleObject), "GetMiningSpeed")]
    [HarmonyPriority(Priority.VeryHigh)]
    internal static float GetMiningSpeed(float __result, IItemStack itemstack, BlockSelection blockSel, Block block, IPlayer forPlayer)
    {
        if (forPlayer == null) return __result;

        float miningSpeed = itemstack.Attributes.GetFloat($"{block.BlockMaterial}_miningspeed", -1f);

        if (miningSpeed == -1f) return __result;
        else __result = miningSpeed;

        return __result;
    }

    /// <summary>
    /// Execute the crafting calculations for smithing level
    /// </summary>
    /// <param name="player"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public static ItemStack ExecuteSmithItemCraftedCalculations(IPlayer player, ItemStack item)
    {
        if (item.Attributes.GetInt("durability", item.Collectible.GetMaxDurability(item)) != item.Collectible.GetMaxDurability(item))
        {
            Debug.LogDebug($"Smith item crafted ignored because durability is different");
            return item;
        }
        else if (item.Attributes.GetBool("repaired", false))
        {
            Debug.LogDebug($"Smith item crafted ignored because item is repaired by {item.Attributes.GetString("repaired_by")}");
            return item;
        }

        int? durability = null;
        int? maxDurability = null;
        float? attackPower = null;
        float? miningSpeed = null;

        // Increasing durability based on smithing level
        if (item.Collectible.Durability > 0)
        {
            float multiply = Configuration.SmithingGetDurabilityMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Smithing"));

            durability = (int)Math.Round(item.Collectible.Durability * multiply);
            maxDurability = (int)Math.Round(item.Collectible.GetMaxDurability(item) * multiply);

            Debug.LogDebug($"[Smithing] Craft new durability: {maxDurability}");
        }

        foreach (KeyValuePair<string, int> kvp in Configuration.expPerCraftSmithing)
        {
            string collectableCode = kvp.Key;

            if (collectableCode.EndsWith(item.Collectible.Code.ToString()))
            {
                string levelType = null;
                if (collectableCode.Contains('?'))
                {
                    int questionMarkIndex = collectableCode.IndexOf('?');
                    levelType = questionMarkIndex != -1 ? collectableCode[..questionMarkIndex] : "";
                }

                int exp = kvp.Value;
                { // Getting total experience earned
                    for (int i = 0; i < item.StackSize; i++)
                    {
                        Experience.IncreaseExperience(player, "Smithing", (ulong)exp);

                        Debug.LogDebug($"[Smithing] Craft levelType: {levelType}");

                        // If the levelType is null, is a tool
                        if (levelType == null)
                            // Increasing sub tool levels
                            switch (item.Item.Tool)
                            {
                                case EnumTool.Knife:
                                    Experience.IncreaseSubExperience(player, "Smithing", "Knife", (ulong)exp);
                                    break;
                                case EnumTool.Axe:
                                    Experience.IncreaseSubExperience(player, "Smithing", "Axe", (ulong)exp);
                                    break;
                                case EnumTool.Bow:
                                    Experience.IncreaseSubExperience(player, "Smithing", "Bow", (ulong)exp);
                                    break;
                                case EnumTool.Chisel:
                                    Experience.IncreaseSubExperience(player, "Smithing", "Chisel", (ulong)exp);
                                    break;
                                case EnumTool.Club:
                                    Experience.IncreaseSubExperience(player, "Smithing", "Club", (ulong)exp);
                                    break;
                                case EnumTool.Crossbow:
                                    Experience.IncreaseSubExperience(player, "Smithing", "Crossbow", (ulong)exp);
                                    break;
                                case EnumTool.Drill:
                                    Experience.IncreaseSubExperience(player, "Smithing", "Drill", (ulong)exp);
                                    break;
                                case EnumTool.Firearm:
                                    Experience.IncreaseSubExperience(player, "Smithing", "Firearm", (ulong)exp);
                                    break;
                                case EnumTool.Halberd:
                                    Experience.IncreaseSubExperience(player, "Smithing", "Halberd", (ulong)exp);
                                    break;
                                case EnumTool.Hammer:
                                    Experience.IncreaseSubExperience(player, "Smithing", "Hammer", (ulong)exp);
                                    break;
                                case EnumTool.Hoe:
                                    Experience.IncreaseSubExperience(player, "Smithing", "Hoe", (ulong)exp);
                                    break;
                                case EnumTool.Javelin:
                                    Experience.IncreaseSubExperience(player, "Smithing", "Javelin", (ulong)exp);
                                    break;
                                case EnumTool.Mace:
                                    Experience.IncreaseSubExperience(player, "Smithing", "Mace", (ulong)exp);
                                    break;
                                case EnumTool.Meter:
                                    Experience.IncreaseSubExperience(player, "Smithing", "Meter", (ulong)exp);
                                    break;
                                case EnumTool.Pickaxe:
                                    Experience.IncreaseSubExperience(player, "Smithing", "Pickaxe", (ulong)exp);
                                    break;
                                case EnumTool.Pike:
                                    Experience.IncreaseSubExperience(player, "Smithing", "Pike", (ulong)exp);
                                    break;
                                case EnumTool.Polearm:
                                    Experience.IncreaseSubExperience(player, "Smithing", "Polearm", (ulong)exp);
                                    break;
                                case EnumTool.Poleaxe:
                                    Experience.IncreaseSubExperience(player, "Smithing", "Poleaxe", (ulong)exp);
                                    break;
                                case EnumTool.Probe:
                                    Experience.IncreaseSubExperience(player, "Smithing", "Probe", (ulong)exp);
                                    break;
                                case EnumTool.Saw:
                                    Experience.IncreaseSubExperience(player, "Smithing", "Saw", (ulong)exp);
                                    break;
                                case EnumTool.Scythe:
                                    Experience.IncreaseSubExperience(player, "Smithing", "Scythe", (ulong)exp);
                                    break;
                                case EnumTool.Shears:
                                    Experience.IncreaseSubExperience(player, "Smithing", "Shears", (ulong)exp);
                                    break;
                                case EnumTool.Shield:
                                    Experience.IncreaseSubExperience(player, "Smithing", "Shield", (ulong)exp);
                                    break;
                                case EnumTool.Shovel:
                                    Experience.IncreaseSubExperience(player, "Smithing", "Shovel", (ulong)exp);
                                    break;
                                case EnumTool.Sickle:
                                    Experience.IncreaseSubExperience(player, "Smithing", "Sickle", (ulong)exp);
                                    break;
                                case EnumTool.Sling:
                                    Experience.IncreaseSubExperience(player, "Smithing", "Sling", (ulong)exp);
                                    break;
                                case EnumTool.Spear:
                                    Experience.IncreaseSubExperience(player, "Smithing", "Spear", (ulong)exp);
                                    break;
                                case EnumTool.Staff:
                                    Experience.IncreaseSubExperience(player, "Smithing", "Staff", (ulong)exp);
                                    break;
                                case EnumTool.Sword:
                                    Experience.IncreaseSubExperience(player, "Smithing", "Sword", (ulong)exp);
                                    break;
                                case EnumTool.Warhammer:
                                    Experience.IncreaseSubExperience(player, "Smithing", "Warhammer", (ulong)exp);
                                    break;
                                case EnumTool.Wrench:
                                    Experience.IncreaseSubExperience(player, "Smithing", "Wrench", (ulong)exp);
                                    break;
                            }
                        else // Code with custom level type
                            Experience.IncreaseSubExperience(player, "Smithing", levelType, (ulong)exp);
                    }
                }

                { // Increasing status
                    // If the levelType is null, is a tool
                    if (levelType == null)
                    {
                        // Do not calculate durability in the handle functions
                        // we alredy calculate the durability before

                        void HandleWeapon(IPlayer player, string subLevelType)
                        {
                            { // Main Level Calculation
                                int level = Configuration.SmithingGetLevelByEXP(Experience.GetExperience(player, "Smithing"));
                                attackPower = item.Item.AttackPower * Configuration.SmithingGetAttackPowerMultiplyByLevel(level);
                            }

                            { // Sub Level Calculation
                                int level = Configuration.SmithingGetLevelByEXP(Experience.GetSubExperience(player, "Smithing", subLevelType));
                                float multiply = Configuration.SmithingGetDurabilityMultiplyByLevel(level);
                                if (item.Collectible.Durability > 0)
                                {
                                    durability = (int)Math.Round((int)durability * multiply);
                                    maxDurability = (int)Math.Round((int)maxDurability * multiply);
                                }

                                attackPower *= Configuration.SmithingGetAttackPowerMultiplyByLevel(level);
                            }
                        }

                        void HandleMiningTool(IPlayer player, string subLevelType)
                        {
                            { // Main Level Calculation
                                int level = Configuration.SmithingGetLevelByEXP(Experience.GetExperience(player, "Smithing"));
                                attackPower = item.Item.AttackPower * Configuration.SmithingGetAttackPowerMultiplyByLevel(level);
                                miningSpeed = Configuration.SmithingGetMiningSpeedMultiplyByLevel(level);
                            }

                            { // Sub Level Calculation
                                int level = Configuration.SmithingGetLevelByEXP(Experience.GetSubExperience(player, "Smithing", subLevelType));
                                if (item.Collectible.Durability > 0)
                                {
                                    float multiply = Configuration.SmithingGetDurabilityMultiplyByLevel(level);
                                    durability = (int)Math.Round((int)durability * multiply);
                                    maxDurability = (int)Math.Round((int)maxDurability * multiply);
                                }

                                attackPower *= Configuration.SmithingGetAttackPowerMultiplyByLevel(level);
                                miningSpeed *= Configuration.SmithingGetMiningSpeedMultiplyByLevel(level);
                            }
                        }

                        void HandleTool(IPlayer player, string subLevelType)
                        {
                            { // Sub Level Calculation
                                int level = Configuration.SmithingGetLevelByEXP(Experience.GetSubExperience(player, "Smithing", subLevelType));
                                if (item.Collectible.Durability > 0)
                                {
                                    float multiply = Configuration.SmithingGetDurabilityMultiplyByLevel(level);
                                    durability = (int)Math.Round((int)durability * multiply);
                                    maxDurability = (int)Math.Round((int)maxDurability * multiply);
                                }
                            }
                        }

                        Debug.LogDebug($"[Smithing] Craft tool: {item.Item.Tool}");

                        // Increasing sub tool levels
                        switch (item.Item.Tool)
                        {
                            case EnumTool.Knife:
                                HandleMiningTool(player, "Knife");
                                break;
                            case EnumTool.Axe:
                                HandleMiningTool(player, "Axe");
                                break;
                            case EnumTool.Bow:
                                HandleWeapon(player, "Bow");
                                break;
                            case EnumTool.Chisel:
                                HandleTool(player, "Chisel");
                                break;
                            case EnumTool.Club:
                                HandleWeapon(player, "Club");
                                break;
                            case EnumTool.Crossbow:
                                HandleWeapon(player, "Crossbow");
                                break;
                            case EnumTool.Drill:
                                HandleTool(player, "Drill");
                                break;
                            case EnumTool.Firearm:
                                HandleWeapon(player, "Firearm");
                                break;
                            case EnumTool.Halberd:
                                HandleWeapon(player, "Halberd");
                                break;
                            case EnumTool.Hammer:
                                HandleWeapon(player, "Hammer");
                                break;
                            case EnumTool.Hoe:
                                HandleTool(player, "Hoe");
                                break;
                            case EnumTool.Javelin:
                                HandleWeapon(player, "Javelin");
                                break;
                            case EnumTool.Mace:
                                HandleTool(player, "Mace");
                                break;
                            case EnumTool.Meter:
                                HandleTool(player, "Meter");
                                break;
                            case EnumTool.Pickaxe:
                                HandleMiningTool(player, "Pickaxe");
                                break;
                            case EnumTool.Pike:
                                HandleTool(player, "Pike");
                                break;
                            case EnumTool.Polearm:
                                HandleTool(player, "Polearm");
                                break;
                            case EnumTool.Poleaxe:
                                HandleTool(player, "Poleaxe");
                                break;
                            case EnumTool.Probe:
                                HandleTool(player, "Probe");
                                break;
                            case EnumTool.Saw:
                                HandleTool(player, "Saw");
                                break;
                            case EnumTool.Scythe:
                                HandleTool(player, "Scythe");
                                break;
                            case EnumTool.Shears:
                                HandleTool(player, "Shears");
                                break;
                            case EnumTool.Shield:
                                HandleTool(player, "Shield");
                                break;
                            case EnumTool.Shovel:
                                HandleMiningTool(player, "Shovel");
                                break;
                            case EnumTool.Sickle:
                                HandleTool(player, "Sickle");
                                break;
                            case EnumTool.Sling:
                                HandleWeapon(player, "Sling");
                                break;
                            case EnumTool.Spear:
                                HandleWeapon(player, "Spear");
                                break;
                            case EnumTool.Staff:
                                HandleTool(player, "Staff");
                                break;
                            case EnumTool.Sword:
                                HandleWeapon(player, "Sword");
                                break;
                            case EnumTool.Warhammer:
                                HandleWeapon(player, "Warhammer");
                                break;
                            case EnumTool.Wrench:
                                HandleTool(player, "Wrench");
                                break;
                        }

                        OverwriteBlockInteractionEvents.UpdateFromExternalSmithCraftingItem(player,
                            item.Collectible.Code.ToString(),
                            ref durability,
                            ref attackPower,
                            ref miningSpeed);
                    }
                    else // Code with custom level type
                    {
                        // Check if is a armor with protection properties
                        if (item.Collectible.Attributes.KeyExists("protectionModifiers"))
                        {
                            // Now we are converting the attributes to a modifiable json

                            string data = item.Collectible.Attributes.Token.ToString();
                            JObject jsonObject = JObject.Parse(data);

                            // Getting the protectionModifiers
                            if (jsonObject.TryGetValue("protectionModifiers", out JToken protectionModifiersToken))
                            {
                                float multiplyProtection;
                                { // Main Level Calculation
                                    int level = Configuration.SmithingGetLevelByEXP(Experience.GetExperience(player, "Smithing"));
                                    multiplyProtection = Configuration.SmithingGetArmorProtectionMultiplyByLevel(level);

                                    // Increasing the armor durability
                                    if (item.Collectible.Durability > 0)
                                    {
                                        float multiplyDurability = Configuration.SmithingGetDurabilityMultiplyByLevel(level);
                                        durability = (int)Math.Round((int)durability * multiplyDurability);
                                        maxDurability = (int)Math.Round((int)maxDurability * multiplyDurability);
                                    }
                                }

                                { // Sub Level Calculation
                                    int level = Configuration.SmithingGetLevelByEXP(Experience.GetSubExperience(player, "Smithing", levelType));
                                    multiplyProtection *= Configuration.SmithingGetArmorProtectionMultiplyByLevel(level);

                                    // Increasing the armor durability
                                    if (item.Collectible.Durability > 0)
                                    {
                                        float multiplyDurability = Configuration.SmithingGetDurabilityMultiplyByLevel(level);
                                        durability = (int)Math.Round((int)durability * multiplyDurability);
                                        maxDurability = (int)Math.Round((int)maxDurability * multiplyDurability);
                                    }
                                }

                                OverwriteBlockInteractionEvents.UpdateFromExternalSmithCraftingArmor(player,
                                    item.Collectible.Code.ToString(),
                                    ref durability,
                                    ref multiplyProtection);

                                // Getting the modifiable protectionModifiers
                                if (protectionModifiersToken is JObject protectionModifiers)
                                {
                                    // Check if exist, and change it
                                    if (protectionModifiers.TryGetValue("relativeProtection", out _))
                                    {
                                        item.Attributes.SetFloat("relativeProtection", (float)jsonObject["protectionModifiers"]["relativeProtection"] * multiplyProtection);
                                        Debug.LogDebug($"{player} armor relativeProtection: {jsonObject["protectionModifiers"]["relativeProtection"]}/{multiplyProtection}");
                                    }
                                    // Check if exist, and change it
                                    if (protectionModifiers.TryGetValue("flatDamageReduction", out _))
                                    {
                                        item.Attributes.SetFloat("flatDamageReduction", (float)jsonObject["protectionModifiers"]["flatDamageReduction"] * multiplyProtection);
                                        Debug.LogDebug($"{player} armor flatDamageReduction: {jsonObject["protectionModifiers"]["flatDamageReduction"]}/{multiplyProtection}");
                                    }
                                }

                                // Convert again to JsonObject and replace it in attributes
                                item.Collectible.Attributes = new(JToken.Parse(jsonObject.ToString()));

                                Debug.LogDebug($"{player.PlayerName} crafted any armor protection increased to: {multiplyProtection}");
                            }
                        }
                        else
                        {
                            Debug.LogWarn($"[Smithing] Not a tool, and not a armor, unhandled item: {item.Collectible.Code}");
                        }
                    }
                }
                Debug.LogDebug($"{player.PlayerName} crafted: {item.Collectible.Code}");

                break;
            }
        }

        if (durability != null && maxDurability != null)
        {
            item.Attributes.SetInt("durability", (int)durability);
            item.Attributes.SetInt("maxdurability", (int)maxDurability);

            Debug.LogDebug($"{player.PlayerName} crafted any item durability increased to: {maxDurability}");
        }
        if (attackPower != null)
        {
            // item.Collectible.AttackPower = (float)attackPower; // Never do that, this will change all "tool" damage
            item.Attributes.SetFloat("attackpower", (float)attackPower);

            Debug.LogDebug($"{player.PlayerName} crafted any item attack increased to: {attackPower}");
        }
        if (miningSpeed != null)
        {
            List<EnumBlockMaterial> keys = [.. item.Item.MiningSpeed.Keys];

            foreach (EnumBlockMaterial key in keys)
            {
                // item.Collectible.MiningSpeed[key] *= (float)miningSpeed; // Never do that, this will change all "tool" mining speed
                item.Attributes.SetFloat($"{key}_miningspeed", item.Collectible.MiningSpeed[key] * (float)miningSpeed);
            }

            Debug.LogDebug($"{player.PlayerName} crafted any item mining speed increased to: {miningSpeed}");
        }

        return item;
    }

    #endregion
}

#region Compatibility
public static class OverwriteBlockInteractionEvents
{
    public delegate void PlayerFloatModifierHandler(IPlayer player, ref float number);
    public delegate void PlayerFarmHandler(IPlayer player, string code, ref ulong exp, ref float multiply);
    public delegate void PlayerCookingSingleHandler(IPlayer player, string code, ref ulong exp, ref float freshHours);
    public delegate void PlayerCookingPotHandler(IPlayer player, string code, ref ulong exp, ref List<float> freshHours, ref float servings);
    public delegate void PlayerHammerItemHandler(IPlayer player, string code, ref int multiply);
    public delegate void PlayerHammerSplitHandler(IPlayer player, ref bool shouldRetrieve, ref ItemStack itemToRetrieve);
    public delegate void PlayerPanningHandler(IPlayer player, ref float chance, ref ItemStack itemStack);
    public delegate void PlayerSmithingItemHandler(IPlayer player, string code, ref int? durability, ref float? attackPower, ref float? miningSpeed);
    public delegate void PlayerSmithingArmorHandler(IPlayer player, string code, ref int? durability, ref float armorProtectionMultiply);
    public delegate void PlayerHandler(IPlayer player);

    public static event PlayerFloatModifierHandler OnKnifeHarvested;
    public static event PlayerHandler OnHoeTill;
    public static event PlayerFarmHandler OnBerryForage;
    public static event PlayerCookingSingleHandler OnCookedSingle;
    public static event PlayerCookingPotHandler OnCookedPot;
    public static event PlayerHammerItemHandler OnHammerItem;
    public static event PlayerHammerSplitHandler OnHammerSmith;
    public static event PlayerPanningHandler OnPanning;
    public static event PlayerSmithingItemHandler OnSmithingItem;
    public static event PlayerSmithingArmorHandler OnSmithingArmor;

    internal static float GetExternalKnifeHarvest(IPlayer player, float multiply)
    {
        OnKnifeHarvested?.Invoke(player, ref multiply);
        return multiply;
    }

    internal static void ExecuteHoeTill(IPlayer player)
    {
        OnHoeTill?.Invoke(player);
    }

    internal static void UpdateFromExternalFarmForage(IPlayer player, string code, ref ulong exp, ref float multiply)
    {
        OnBerryForage?.Invoke(player, code, ref exp, ref multiply);
    }

    internal static void UpdateFromExternalCookingSingle(IPlayer player, string code, ref ulong exp, ref float freshHours)
    {
        OnCookedSingle?.Invoke(player, code, ref exp, ref freshHours);
    }

    internal static void UpdateFromExternalCookingPot(IPlayer player, string code, ref ulong exp, ref List<float> freshHours, ref float servings)
    {
        OnCookedPot?.Invoke(player, code, ref exp, ref freshHours, ref servings);
    }

    internal static int GetExternalHammerMultiply(IPlayer player, string code, int multiply)
    {
        OnHammerItem?.Invoke(player, code, ref multiply);
        return multiply;
    }

    internal static void UpdateFromExternalHammerSplit(IPlayer player, ref bool shouldRetrieve, ref ItemStack itemToRetrieve)
    {
        OnHammerSmith?.Invoke(player, ref shouldRetrieve, ref itemToRetrieve);
    }

    internal static void UpdateFromExternalPanning(IPlayer player, ref float chance, ref ItemStack itemStack)
    {
        OnPanning?.Invoke(player, ref chance, ref itemStack);
    }

    internal static void UpdateFromExternalSmithCraftingItem(IPlayer player, string code, ref int? durability, ref float? attackPower, ref float? miningSpeed)
    {
        OnSmithingItem?.Invoke(player, code, ref durability, ref attackPower, ref miningSpeed);
    }

    internal static void UpdateFromExternalSmithCraftingArmor(IPlayer player, string code, ref int? durability, ref float armorProtectionMultiply)
    {
        OnSmithingArmor?.Invoke(player, code, ref durability, ref armorProtectionMultiply);
    }
}
#endregion