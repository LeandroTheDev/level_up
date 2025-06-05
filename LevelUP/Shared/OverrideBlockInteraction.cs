using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HarmonyLib;
using LevelUP.Server;
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
class OverwriteBlockInteraction
{
    private static Instance instance;
    public Harmony overwriter;

    public void OverwriteNativeFunctions(Instance _instance)
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
    [HarmonyPatch(typeof(EntityBehaviorHarvestable), "SetHarvested")]
    public static void SetHarvestedKnifeStart(EntityBehaviorHarvestable __instance, IPlayer byPlayer, ref float dropQuantityMultiplier)
    {
        if (!Configuration.enableLevelKnife) return;

        // Check if is from the server
        if (__instance.entity.World.Side == EnumAppSide.Server)
        {
            // Earn xp by harvesting the entity
            Experience.IncreaseExperience(byPlayer, "Knife", "Harvest");

            // Get the final droprate
            float dropRate = Configuration.KnifeGetHarvestMultiplyByLevel(byPlayer.Entity.WatchedAttributes.GetInt("LevelUP_Level_Knife"));

            // Increasing entity drop rate
            dropQuantityMultiplier += dropRate;

            Debug.LogDebug($"{byPlayer.PlayerName} harvested any entity with knife, multiply drop: {dropRate}, values: {Configuration.KnifeGetHarvestMultiplyByLevel(byPlayer.Entity.WatchedAttributes.GetInt("LevelUP_Level_Knife"))}");
        }
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
            if (byEntity is EntityPlayer)
                // Earn xp by tilling the soil
                Experience.IncreaseExperience((byEntity as EntityPlayer).Player, "Farming", "Till");
        }
    }

    // Overwrite Berry Forage while breaking
    [HarmonyPrefix]
    [HarmonyPatch(typeof(BlockBerryBush), "GetDrops")]
    public static void GetDropsBerryBushFinish(BlockBerryBush __instance, IWorldAccessor world, BlockPos pos, IPlayer byPlayer, ref float dropQuantityMultiplier)
    {
        if (!Configuration.enableLevelFarming) return;

        if (byPlayer != null && world.Side == EnumAppSide.Server)
        {
            // Increasing the quantity drop multiply by the farming level
            dropQuantityMultiplier = Configuration.FarmingGetForageMultiplyByLevel(byPlayer.Entity.WatchedAttributes.GetInt("LevelUP_Level_Farming"));

            Debug.LogDebug($"{byPlayer.PlayerName} bush harvest: {__instance.Code}, by breaking");

            // Check the berry existence
            if (Configuration.expPerHarvestFarming.TryGetValue(__instance.Code.ToString(), out int exp))
                Experience.IncreaseExperience(byPlayer, "Farming", (ulong)exp);
        }
    }

    // Overwrite Berry Forage while interacting
    [HarmonyPrefix]
    [HarmonyPatch(typeof(BlockBehaviorHarvestable), "OnBlockInteractStop")]
    public static void OnBlockInteractStopStart(BlockBehaviorHarvestable __instance, float secondsUsed, IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel, ref EnumHandling handled)
    {
        // We temporary store the droprate to be reseted after the function called
        byPlayer.Entity.Stats.Set("forageDropRatePreviously", "forageDropRatePreviously", byPlayer.Entity.Stats.GetBlended("forageDropRate"));

        if (!Configuration.enableLevelFarming) return;

        if (byPlayer != null && world.Side == EnumAppSide.Server)
        {
            // This is necessary unfurtunally because the devs forgot to add the "dropQuantityMultiplier" on the function
            // we are changing the drop rate from entity status, that is very dangerous but is the only way...
            byPlayer.Entity.Stats.Set("forageDropRate", "forageDropRate", Configuration.FarmingGetForageMultiplyByLevel(byPlayer.Entity.WatchedAttributes.GetInt("LevelUP_Level_Farming")));

            Debug.LogDebug($"{byPlayer.PlayerName} bush harvest: {__instance.block.Code}, by right clicking");

            // Check the berry existence
            if (Configuration.expPerHarvestFarming.TryGetValue(__instance.block.Code.ToString(), out int exp))
                Experience.IncreaseExperience(byPlayer, "Farming", (ulong)exp);

        }
    }
    [HarmonyPostfix] // This is necessary to back to default value after the function is complete
    [HarmonyPatch(typeof(BlockBehaviorHarvestable), "OnBlockInteractStop")]
    public static void OnBlockInteractStopFinish(BlockBehaviorHarvestable __instance, float secondsUsed, IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel, ref EnumHandling handled)
    {
        // After the function called we need to reset the forage if changed
        byPlayer.Entity.Stats.Set("forageDropRate", "forageDropRate", byPlayer.Entity.Stats.GetBlended("forageDropRatePreviously"));
    }

    // Overwrite Mushroom Forage
    [HarmonyPostfix]
    [HarmonyPatch(typeof(BlockMushroom), "GetDrops")]
    public static void GetDropsMushroomFinish(BlockMushroom __instance, IWorldAccessor world, BlockPos pos, IPlayer byPlayer, ref float dropQuantityMultiplier)
    {
        if (!Configuration.enableLevelFarming) return;

        if (byPlayer != null && world.Side == EnumAppSide.Server)
        {
            // Increasing the quantity drop multiply by the farming level
            dropQuantityMultiplier = Configuration.FarmingGetForageMultiplyByLevel(byPlayer.Entity.WatchedAttributes.GetInt("LevelUP_Level_Farming"));

            Debug.LogDebug($"{byPlayer.PlayerName} bush harvest: {__instance.Code}");

            // Check the berry existence
            if (Configuration.expPerHarvestFarming.TryGetValue(__instance.Code.ToString(), out int exp))
                Experience.IncreaseExperience(byPlayer, "Farming", (ulong)exp);
        }
    }
    #endregion

    #region cooking
    static int cookingFirePitOverflow = 0;
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
            cookingFirePitOverflow++;
            // Check if input stack exists on exp earn, this means the player is reheating the food, disabling the experience mechanic
            if (Configuration.expMultiplySingleCooking.TryGetValue(__instance.inputStack.Collectible.Code.ToString(), out double _)) return;
            else if (Configuration.expMultiplyPotsCooking.TryGetValue(__instance.inputStack.Collectible.Code.ToString(), out double _)) return;

            // Check if the output existed before the cooking finished
            bool firstOutput = __instance.outputStack == null;

            Debug.LogDebug($"{__instance.inputStack.Collectible.Code} finished cooking, X: {__instance.Pos.X}, Y: {__instance.Pos.Y}, Z: {__instance.Pos.Z}");

            // Overflow check
            if (cookingFirePitOverflow >= Configuration.cookingFirePitOverflow)
            {
                Debug.Log($"Cooking overflowed, too many players? or there is a missing recipe causing reheating bug? recipe: {__instance.inputStack.Collectible.Code}, stats and experience ignored, X: {__instance.Pos.X}, Y: {__instance.Pos.Y}, Z: {__instance.Pos.Z}, if recipe is missing please add in configs/levelstats/cookingpots.json or configs/levelstats/cookingsingles.json");
                return;
            }

            // Run on secondary thread to not freeze the server
            Task thread = Task.Run(() =>
            {
                // Because output is magically added by something we need to constantly check it
                while (__instance.outputStack == null) { }
                // Finally receive output
                ItemStack output = __instance.outputStack;
                // Check if output doesn't exist
                if (output is null || output.Collectible is null) return;

                Debug.LogDebug($"Cooking output: {output.Collectible.Code}, X: {__instance.Pos.X}, Y: {__instance.Pos.Y}, Z: {__instance.Pos.Z}");

                // Update player experience to the most proximity player
                // or if is single player get the player playing
                IPlayer player = instance.serverAPI?.api.World.NearestPlayer(__instance.Pos.X, __instance.Pos.Y, __instance.Pos.Z);
                if (instance.clientAPI != null && instance.clientAPI.api.IsSinglePlayer)
                    player = instance.clientAPI.api.World.Player;

                // If cannot find the nearest player
                if (player == null)
                {
                    Debug.LogDebug("Cooking: player is null, cooking experience and stats has been ignored");
                    return;
                }

                // For single cooking
                if (Configuration.expMultiplySingleCooking.TryGetValue(output.Collectible.Code.ToString(), out double expMultiplySingle))
                {
                    if (firstOutput)
                    {
                        // Increase the fresh hours based in player experience
                        TreeAttribute attribute = output.Attributes["transitionstate"] as TreeAttribute;
                        FloatArrayAttribute freshHours = attribute.GetAttribute("freshHours") as FloatArrayAttribute;
                        Debug.LogDebug($"Cooking: previously fresh hours: {freshHours.value[0]}");
                        freshHours.value[0] *= Configuration.CookingGetFreshHoursMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Cooking"));
                        Debug.LogDebug($"Cooking: fresh hours increased to: {freshHours.value[0]} with multiply of {Configuration.CookingGetFreshHoursMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Cooking"))}");
                        attribute.SetAttribute("freshHours", freshHours);
                        output.Attributes["transitionstate"] = attribute;
                    }

                    Experience.IncreaseExperience(player, "Cooking", (ulong)Math.Round(Configuration.ExpPerCookingcooking + (Configuration.ExpPerCookingcooking * expMultiplySingle)));
                }
                // For pots cooking
                else if (Configuration.expMultiplyPotsCooking.TryGetValue(output.Collectible.Code.ToString(), out double expMultiplyPots))
                {
                    if (firstOutput)
                    {
                        // Increase the fresh hours based in player experience
                        {
                            // For pots the fresh foods is stored as raw in the pot
                            // by knowing that we need to increase fresh hours foreach inventory slot from this pot
                            TreeAttribute attribute = output.Attributes["contents"] as TreeAttribute;

                            Debug.LogDebug("Increasing cooking ingredients fresh hours...");

                            // Swipe all foods in inventory
                            foreach (var contents in attribute)
                            {
                                ItemstackAttribute contentAttribute = contents.Value as ItemstackAttribute;
                                ItemStack item = contentAttribute.value;

                                // Get food datas
                                TreeAttribute itemAttribute = item.Attributes["transitionstate"] as TreeAttribute;
                                FloatArrayAttribute freshHours = itemAttribute.GetAttribute("freshHours") as FloatArrayAttribute;

                                Debug.LogDebug($"Cooking: previously fresh hours: {freshHours.value[0]}");

                                // Increase fresh hours
                                freshHours.value[0] *= Configuration.CookingGetFreshHoursMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Cooking"));

                                Debug.LogDebug($"Cooking: fresh hours increased to: {freshHours.value[0]} with multiply of {Configuration.CookingGetFreshHoursMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Cooking"))}");

                                // Updating
                                itemAttribute.SetAttribute("freshHours", freshHours);
                                item.Attributes["transitionstate"] = itemAttribute;
                                contentAttribute.value = item;
                            }
                            output.Attributes["contents"] = attribute;
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

                            // Updating
                            attribute["quantityServings"] = servingsQuantity;
                            output.Attributes = attribute;
                        }
                    }

                    Experience.IncreaseExperience(player, "Cooking", (ulong)Math.Round(Configuration.ExpPerCookingcooking + (Configuration.ExpPerCookingcooking * expMultiplyPots)));
                }
            });
            // Thread timeout
            Task.Delay(100).ContinueWith((_) =>
            {
                if (!thread.IsCompleted)
                    Debug.Log("WARNING: Output thread in cooking function has forcelly disposed, did the server is overloaded? cooking experience and status have been lost");

                thread.Dispose();
                if (cookingFirePitOverflow > 0)
                    cookingFirePitOverflow -= 1;
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
                    // Alright its match lets get the chance
                    if (Configuration.HammerShouldRetrieveSmithByLevel(byPlayer.Entity.WatchedAttributes.GetInt("LevelUP_Level_Hammer")))
                    {
                        // The chance returned true so we need to retrieve the player the item
                        byPlayer.Entity.TryGiveItemStack(new ItemStack(__instance.Api.World.GetItem(new AssetLocation(keyValue.Value))));
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

        // Another function setted as private...
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
                if (num < (double)val && stack != null)
                {
                    stack = stack.Clone();
                    // Multiplying drop quantity
                    stack.StackSize += stack.StackSize * Configuration.PanningGetLootQuantityMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Panning"));
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
    // Overwrite Craft
    [HarmonyPostfix]
    [HarmonyPatch(typeof(ItemSlotCraftingOutput), "CraftSingle")]
    public static void CraftSingleFinish(ItemSlotCraftingOutput __instance, ItemSlot sinkSlot, ref ItemStackMoveOperation op)
    {
        if (op.World.Api.Side != EnumAppSide.Server) return;
        if (sinkSlot == null || sinkSlot.Itemstack == null) return;
        if (op.ActingPlayer == null) return;

        // Increasing durability based on smithing level
        if (sinkSlot.Itemstack.Collectible.Durability > 0)
        {
            float multiply = Configuration.SmithingGetDurabilityMultiplyByLevel(op.ActingPlayer.Entity.WatchedAttributes.GetInt("LevelUP_Level_Smithing"));

            sinkSlot.Itemstack.Attributes.SetInt("durability", (int)Math.Round(sinkSlot.Itemstack.Collectible.Durability * multiply));
            sinkSlot.Itemstack.Attributes.SetInt("maxdurability", (int)Math.Round(sinkSlot.Itemstack.Collectible.GetMaxDurability(sinkSlot.Itemstack) * multiply));
            sinkSlot.MarkDirty();
        }

        // Increasing smithing experience
        if (Configuration.expPerCraftSmithing.TryGetValue(sinkSlot.Itemstack.Collectible.Code.ToString(), out int exp))
            Experience.IncreaseExperience(op.ActingPlayer, "Smithing", (ulong)exp);
    }

    // Overwrite Craft Multiples
    [HarmonyPostfix]
    [HarmonyPatch(typeof(ItemSlotCraftingOutput), "CraftMany")]
    public static void CraftManyFinish(ItemSlotCraftingOutput __instance, ItemSlot sinkSlot, ref ItemStackMoveOperation op)
    {
        if (op.World.Api.Side != EnumAppSide.Server) return;
        if (sinkSlot == null || sinkSlot.Itemstack == null) return;
        if (op.ActingPlayer == null) return;

        // Increasing durability based on smithing level
        if (sinkSlot.Itemstack.Collectible.Durability > 0)
        {
            float multiply = Configuration.SmithingGetDurabilityMultiplyByLevel(op.ActingPlayer.Entity.WatchedAttributes.GetInt("LevelUP_Level_Smithing"));

            sinkSlot.Itemstack.Attributes.SetInt("durability", (int)Math.Round(sinkSlot.Itemstack.Collectible.Durability * multiply));
            sinkSlot.Itemstack.Attributes.SetInt("maxdurability", (int)Math.Round(sinkSlot.Itemstack.Collectible.GetMaxDurability(sinkSlot.Itemstack) * multiply));
            sinkSlot.MarkDirty();
        }

        // Increasing smithing experience
        if (Configuration.expPerCraftSmithing.TryGetValue(sinkSlot.Itemstack.Collectible.Code.ToString(), out int exp))
            for (int i = 0; i < sinkSlot.Itemstack.StackSize; i++)
                Experience.IncreaseExperience(op.ActingPlayer, "Smithing", (ulong)exp);
    }

    // Overwrite Visual Max Durability
    // This is necessary so the durability system is more accurate
    [HarmonyPostfix]
    [HarmonyPatch(typeof(CollectibleObject), "GetMaxDurability")]
    public static void GetMaxDurabilityFinish(ItemStack itemstack, ref int __result)
    {
        int maxDurability = itemstack.Attributes.GetInt("maxdurability", -1);
        if (maxDurability != -1)
        {
            __result = maxDurability;
        }
    }
    #endregion
}