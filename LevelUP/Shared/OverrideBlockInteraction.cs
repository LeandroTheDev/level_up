using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.API.Util;
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
            else Debug.Log("ERROR: Block break overwriter has already patched, did some mod already has levelup_block_interaction in harmony?");
        }
    }

    #region knife
    // Overwrite Knife Harvesting
    [HarmonyPrefix]
    [HarmonyPatch(typeof(EntityBehaviorHarvestable), "SetHarvested")]
    public static void SetHarvestedKnifeStart(EntityBehaviorHarvestable __instance, IPlayer byPlayer, ref float dropQuantityMultiplier)
    {
        if (!Configuration.enableLevelKnife) return;

        // Receive the droprate from other mods
        float compatibilityDroprate = byPlayer.Entity.Attributes.GetFloat("LevelUP_BlockInteraction_Compatibility_ExtendHarvestDrop_SetHarvestedKnife");

        // Check if is from the server
        if (byPlayer is IServerPlayer && __instance.entity.World.Side == EnumAppSide.Server)
        {
            IServerPlayer player = byPlayer as IServerPlayer;
            // Earny xp by harvesting entity
            instance.serverAPI?.OnExperienceEarned(player, "Knife_Harvest_Entity");

            // Get the final droprate
            float dropRate = Configuration.KnifeGetHarvestMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Knife")) + compatibilityDroprate;

            // Increasing entity drop rate
            dropQuantityMultiplier += dropRate;
            if (Configuration.enableExtendedLog)
                Debug.Log($"{player.PlayerName} harvested any entity with knife, multiply drop: {dropRate}, values: {Configuration.KnifeGetHarvestMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Knife"))} + {compatibilityDroprate}");
        }
        // Single player treatment and lan treatment
        else if (instance.clientAPI != null && instance.clientAPI.api.IsSinglePlayer)
        {
            instance.clientAPI.compatibilityChannel.SendPacket($"Knife_Harvest_Entity&lanplayername={byPlayer.PlayerName}");

            // Get the final droprate
            float dropRate = Configuration.KnifeGetHarvestMultiplyByLevel(byPlayer.Entity.WatchedAttributes.GetInt("LevelUP_Level_Knife")) + compatibilityDroprate;

            // Increasing entity drop rate
            dropQuantityMultiplier += dropRate;
            if (Configuration.enableExtendedLog)
                Debug.Log($"{byPlayer.PlayerName} harvested any entity with knife, multiply drop: {dropRate}, values: {Configuration.KnifeGetHarvestMultiplyByLevel(byPlayer.Entity.WatchedAttributes.GetInt("LevelUP_Level_Knife"))} + {compatibilityDroprate}");
        }
    }
    // Overwrite Knife Harvesting
    [HarmonyPostfix]
    [HarmonyPatch(typeof(EntityBehaviorHarvestable), "SetHarvested")]
    public static void SetHarvestedKnifeFinish(EntityBehaviorHarvestable __instance, IPlayer byPlayer, float dropQuantityMultiplier = 1f)
    {
        byPlayer.Entity.Attributes.RemoveAttribute("LevelUP_BlockInteraction_Compatibility_ExtendHarvestDrop_SetHarvestedKnife");
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
            {
                // Get player entity
                EntityPlayer playerEntity = byEntity as EntityPlayer;
                // Increase farming experience
                if (playerEntity.Player is IServerPlayer) instance.serverAPI?.OnExperienceEarned(playerEntity.Player as IServerPlayer, "Soil_Till");
            }
        }
        // Single player treatment and lan treatment
        else if (instance.clientAPI != null && instance.clientAPI.api.IsSinglePlayer && secondsUsed >= 1.0f) instance.clientAPI.compatibilityChannel.SendPacket($"Soil_Till&lanplayername={byEntity.GetName()}");
    }

    // Overwrite Berry Forage while breaking
    [HarmonyPrefix]
    [HarmonyPatch(typeof(BlockBerryBush), "GetDrops")]
    public static void GetDropsBerryBushFinish(BlockBerryBush __instance, IWorldAccessor world, BlockPos pos, IPlayer byPlayer, float dropQuantityMultiplier = 1f)
    {
        if (Configuration.enableLevelFarming && byPlayer != null)
        {
            // In creasing forage drop rate based on level
            byPlayer.Entity.Stats.Set("forageDropRate", "forageDropRate", Configuration.FarmingGetHarvestMultiplyByLevel(byPlayer.Entity.WatchedAttributes.GetInt("LevelUP_Level_Farming")));

            if (Configuration.enableExtendedLog) Debug.Log($"Bush harvest: {__instance.Code}");

            // Check the berry existence
            if (Configuration.expPerHarvestFarming.TryGetValue(__instance.Code.ToString(), out int exp))
            {
                // Dedicated Servers
                if (instance.serverAPI != null)
                    instance.serverAPI.OnExperienceEarned(byPlayer as IServerPlayer, $"Farming_Harvest&forceexp={exp}");
                // Single player treatment and lan treatment
                else if (instance.clientAPI?.api.IsSinglePlayer ?? false)
                {
                    instance.clientAPI.compatibilityChannel.SendPacket($"Farming_Harvest&forceexp={exp}");
                }
            }
        }
    }

    // Overwrite Berry Forage while interacting
    [HarmonyPrefix]
    [HarmonyPatch(typeof(BlockBehaviorHarvestable), "OnBlockInteractStop")]
    public static void OnBlockInteractStopFinish(BlockBehaviorHarvestable __instance, float secondsUsed, IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel, ref EnumHandling handled)
    {
        if (Configuration.enableLevelFarming && byPlayer != null)
        {
            // In creasing forage drop rate based on level
            byPlayer.Entity.Stats.Set("forageDropRate", "forageDropRate", Configuration.FarmingGetHarvestMultiplyByLevel(byPlayer.Entity.WatchedAttributes.GetInt("LevelUP_Level_Farming")));

            if (Configuration.enableExtendedLog) Debug.Log($"Bush harvest: {__instance.block.Code}");

            // Check the berry existence
            if (Configuration.expPerHarvestFarming.TryGetValue(__instance.block.Code.ToString(), out int exp))
            {
                // Dedicated Servers
                if (instance.serverAPI != null)
                    instance.serverAPI.OnExperienceEarned(byPlayer as IServerPlayer, $"Farming_Harvest&forceexp={exp}");
                // Single player treatment and lan treatment
                else if (instance.clientAPI?.api.IsSinglePlayer ?? false)
                {
                    instance.clientAPI.compatibilityChannel.SendPacket($"Farming_Harvest&forceexp={exp}");
                }
            }
        }
    }

    // Overwrite Mushroom Forage
    [HarmonyPostfix]
    [HarmonyPatch(typeof(BlockMushroom), "GetDrops")]
    public static void GetDropsMushroomFinish(BlockMushroom __instance, IWorldAccessor world, BlockPos pos, IPlayer byPlayer, float dropQuantityMultiplier = 1f)
    {
        if (Configuration.enableLevelFarming && byPlayer != null)
        {
            // In creasing forage drop rate based on level
            byPlayer.Entity.Stats.Set("forageDropRate", "forageDropRate", Configuration.FarmingGetHarvestMultiplyByLevel(byPlayer.Entity.WatchedAttributes.GetInt("LevelUP_Level_Farming")));

            if (Configuration.enableExtendedLog) Debug.Log($"Bush harvest: {__instance.Code}");

            // Check the berry existence
            if (Configuration.expPerHarvestFarming.TryGetValue(__instance.Code.ToString(), out int exp))
            {
                // Dedicated Servers
                if (instance.serverAPI != null)
                    instance.serverAPI.OnExperienceEarned(byPlayer as IServerPlayer, $"Farming_Harvest&forceexp={exp}");
                // Single player treatment and lan treatment
                else if (instance.clientAPI?.api.IsSinglePlayer ?? false)
                {
                    instance.clientAPI.compatibilityChannel.SendPacket($"Farming_Harvest&forceexp={exp}");
                }
            }
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
        if (!Configuration.enableLevelCooking || __instance.Api.World.Side != EnumAppSide.Server) return;

        // Hol up, let him cook
        float maxCookingTime = __instance.inputSlot.Itemstack.Collectible.GetMeltingDuration(__instance.Api.World, (ISlotProvider)__instance.Inventory, __instance.inputSlot);
        float cookingTime = __instance.inputStackCookingTime;

        if (Configuration.enableExtendedLog && (int)cookingTime % 10 == 0 && cookingTime > 0 && cookingTime < maxCookingTime)
            Debug.Log($"Cooking: {cookingTime} / {maxCookingTime}");

        // Check if him finished cooking
        if (cookingTime >= maxCookingTime)
        {
            cookingFirePitOverflow++;
            // Check if input stack exists on exp earn, this means the player is reheating the food, disabling the experience mechanic
            if (Configuration.expMultiplySingleCooking.TryGetValue(__instance.inputStack.Collectible.Code.ToString(), out double _)) return;
            else if (Configuration.expMultiplyPotsCooking.TryGetValue(__instance.inputStack.Collectible.Code.ToString(), out double _)) return;

            // Check if the output existed before the cooking finished
            bool firstOutput = __instance.outputStack == null;

            if (Configuration.enableExtendedLog)
                Debug.Log($"{__instance.inputStack.Collectible.Code} finished cooking, X: {__instance.Pos.X}, Y: {__instance.Pos.Y}, Z: {__instance.Pos.Z}");

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

                if (Configuration.enableExtendedLog)
                    Debug.Log($"Cooking output: {output.Collectible.Code}, X: {__instance.Pos.X}, Y: {__instance.Pos.Y}, Z: {__instance.Pos.Z}");

                // Update player experience to the most proximity player
                // or if is single player get the player playing
                IPlayer player;
                if (__instance.Api.World.Side == EnumAppSide.Server)
                {
                    player = instance.serverAPI?.api.World.NearestPlayer(__instance.Pos.X, __instance.Pos.Y, __instance.Pos.Z);
                    if (instance.clientAPI != null && instance.clientAPI.api.IsSinglePlayer)
                        player = instance.clientAPI.api.World.Player;
                }
                else return;

                // If cannot find the nearest player
                if (player == null)
                {
                    if (Configuration.enableExtendedLog) Debug.Log("Cooking: player is null, cooking experience and stats has been ignored");
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
                        if (Configuration.enableExtendedLog)
                            Debug.Log($"Cooking: previously fresh hours: {freshHours.value[0]}");
                        freshHours.value[0] *= Configuration.CookingGetFreshHoursMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Cooking"));
                        if (Configuration.enableExtendedLog)
                            Debug.Log($"Cooking: fresh hours increased to: {freshHours.value[0]} with multiply of {Configuration.CookingGetFreshHoursMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Cooking"))}");
                        attribute.SetAttribute("freshHours", freshHours);
                        output.Attributes["transitionstate"] = attribute;
                    }
                    // Dedicated Servers
                    if (instance.serverAPI != null)
                        instance.serverAPI.OnExperienceEarned(player as IServerPlayer, $"Cooking_Finished&forceexp={(int)Math.Round(Configuration.ExpPerCookingcooking + (Configuration.ExpPerCookingcooking * expMultiplySingle))}");
                    // Single player treatment and lan treatment
                    else if (instance.clientAPI?.api.IsSinglePlayer ?? false)
                        instance.clientAPI.compatibilityChannel.SendPacket($"Cooking_Finished&forceexp={(int)Math.Round(Configuration.ExpPerCookingcooking + (Configuration.ExpPerCookingcooking * expMultiplySingle))}&lanplayername={player.PlayerName}");
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

                            if (Configuration.enableExtendedLog)
                                Debug.Log("Increasing cooking ingredients fresh hours...");

                            // Swipe all foods in inventory
                            foreach (var contents in attribute)
                            {
                                ItemstackAttribute contentAttribute = contents.Value as ItemstackAttribute;
                                ItemStack item = contentAttribute.value;

                                // Get food datas
                                TreeAttribute itemAttribute = item.Attributes["transitionstate"] as TreeAttribute;
                                FloatArrayAttribute freshHours = itemAttribute.GetAttribute("freshHours") as FloatArrayAttribute;

                                if (Configuration.enableExtendedLog)
                                    Debug.Log($"Cooking: previously fresh hours: {freshHours.value[0]}");

                                // Increase fresh hours
                                freshHours.value[0] *= Configuration.CookingGetFreshHoursMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Cooking"));

                                if (Configuration.enableExtendedLog)
                                    Debug.Log($"Cooking: fresh hours increased to: {freshHours.value[0]} with multiply of {Configuration.CookingGetFreshHoursMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Cooking"))}");

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

                            if (Configuration.enableExtendedLog)
                                Debug.Log($"Cooking: previously servings: {servingsQuantity.value}");

                            // Increasing servings quantity
                            servingsQuantity.value = Configuration.CookingGetServingsByLevelAndServings(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Cooking"), (int)servingsQuantity.value); ;

                            if (Configuration.enableExtendedLog)
                                Debug.Log($"Cooking: servings now: {servingsQuantity.value}");

                            // Updating
                            attribute["quantityServings"] = servingsQuantity;
                            output.Attributes = attribute;
                        }
                    }

                    // Dedicated Servers
                    if (instance.serverAPI != null)
                        instance.serverAPI.OnExperienceEarned(player as IServerPlayer, $"Cooking_Finished&forceexp={(int)Math.Round(Configuration.ExpPerCookingcooking + (Configuration.ExpPerCookingcooking * expMultiplyPots))}");
                    // Single player treatment and lan treatment
                    else if (instance.clientAPI?.api.IsSinglePlayer ?? false)
                        instance.clientAPI.compatibilityChannel.SendPacket($"Cooking_Finished&forceexp={(int)Math.Round(Configuration.ExpPerCookingcooking + (Configuration.ExpPerCookingcooking * expMultiplyPots))}&lanplayername={player.PlayerName}");
                }
            });
            // Thread timeout
            Task.Delay(100).ContinueWith((_) =>
            {
                if (!thread.IsCompleted)
                {
                    Debug.Log("WARNING: Output thread in cooking function has forcelly disposed, did the server is overloaded? cooking experience and status have been lost");
                };
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

                    if (Configuration.enableExtendedLog)
                        Debug.Log($"{byPlayer.PlayerName} finished smithing {__instance.SelectedRecipe.Output?.ResolvedItemstack?.Collectible?.Code} drop quantity: {__instance.SelectedRecipe.Output?.Quantity} with a final result size of: {__instance.SelectedRecipe.Output?.ResolvedItemstack?.StackSize} multiplied by: {multiply}");
                }
            }

            // Check if player is using the hammer
            if (byPlayer?.InventoryManager?.ActiveTool == EnumTool.Hammer)
            {
                // Dedicated Servers
                if (instance.serverAPI != null)
                    instance.serverAPI.OnExperienceEarned(byPlayer as IServerPlayer, "Increase_Hammer_Hit");
                // Single player treatment
                else if (instance.clientAPI?.api.IsSinglePlayer ?? false)
                    instance.clientAPI.compatibilityChannel.SendPacket($"Increase_Hammer_Hit&lanplayername={byPlayer.PlayerName}");
            }
        }
    }

    // Overwrite the hammer split
    [HarmonyPrefix]
    [HarmonyPatch(typeof(BlockEntityAnvil), "OnUseOver")]
    [HarmonyPatch("OnUseOver", [typeof(IPlayer), typeof(Vec3i), typeof(BlockSelection)])] // This is necessary because there is 2 functions with the same name
    public static void OnUseOver(BlockEntityAnvil __instance, IPlayer byPlayer, Vec3i voxelPos, BlockSelection blockSel)
    {
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
        // Dedicated Servers
        if (instance.serverAPI != null)
        {
            // Another function setted as private...
            static ItemStack Resolve(EnumItemClass type, string code)
            {
                if (type == EnumItemClass.Block)
                {
                    Block block = instance.serverAPI.api.World.GetBlock(new AssetLocation(code));
                    if (block == null)
                    {
                        instance.serverAPI.api.World.Logger.Error("Failed resolving panning block drop with code {0}. Will skip.", code);
                        return null;
                    }
                    return new ItemStack(block);
                }
                Item item = instance.serverAPI.api.World.GetItem(new AssetLocation(code));
                if (item == null)
                {
                    instance.serverAPI.api.World.Logger.Error("Failed resolving panning item drop with code {0}. Will skip.", code);
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
                            drops[i].Resolve(instance.serverAPI.api.World, "panningdrop");
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
                string rocktype = instance.serverAPI.api.World.GetBlock(new AssetLocation(fromBlockCode))?.Variant["rock"];
                drops.Shuffle(instance.serverAPI.api.World.Rand);
                for (int i = 0; i < drops.Length; i++)
                {
                    PanningDrop drop = drops[i];
                    double num = instance.serverAPI.api.World.Rand.NextDouble();
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
                            instance.serverAPI.api.World.SpawnItemEntity(stack, byEntity.ServerPos.XYZ);
                        }
                        break;
                    }
                }

                // Increasing exp for panning
                instance.serverAPI.OnExperienceEarned(player as IServerPlayer, "Panning_Finished");
            }
        }
        // Singleplayer/Lan
        else if (instance.clientAPI != null)
        {
            // Another function setted as private...
            static ItemStack Resolve(EnumItemClass type, string code)
            {
                if (type == EnumItemClass.Block)
                {
                    Block block = instance.clientAPI.api.World.GetBlock(new AssetLocation(code));
                    if (block == null)
                    {
                        instance.clientAPI.api.World.Logger.Error("Failed resolving panning block drop with code {0}. Will skip.", code);
                        return null;
                    }
                    return new ItemStack(block);
                }
                Item item = instance.clientAPI.api.World.GetItem(new AssetLocation(code));
                if (item == null)
                {
                    instance.clientAPI.api.World.Logger.Error("Failed resolving panning item drop with code {0}. Will skip.", code);
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
                            drops[i].Resolve(instance.clientAPI.api.World, "panningdrop");
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
                string rocktype = instance.clientAPI.api.World.GetBlock(new AssetLocation(fromBlockCode))?.Variant["rock"];
                drops.Shuffle(instance.clientAPI.api.World.Rand);
                for (int i = 0; i < drops.Length; i++)
                {
                    PanningDrop drop = drops[i];
                    double num = instance.clientAPI.api.World.Rand.NextDouble();
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
                        int lootMultiplier = Configuration.PanningGetLootQuantityMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Panning"));
                        stack.StackSize += stack.StackSize * lootMultiplier;
                        if (Configuration.enableExtendedLog)
                            Debug.Log($"{player.PlayerName} successfully panned, with a item multiply of: {lootMultiplier}");
                        if (player == null || !player.InventoryManager.TryGiveItemstack(stack, slotNotifyEffect: true))
                        {
                            instance.clientAPI.api.World.SpawnItemEntity(stack, byEntity.ServerPos.XYZ);
                        }
                        break;
                    }
                }

                // Increasing exp for panning
                instance.clientAPI.compatibilityChannel.SendPacket($"Panning_Finished&lanplayername={player.PlayerName}");
            }

        }
        return false;
    }

    #endregion
}