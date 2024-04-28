using System;
using System.Threading.Tasks;
using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Server;
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
    public static void SetHarvestedKnifeStart(EntityBehaviorHarvestable __instance, IPlayer byPlayer, float dropQuantityMultiplier = 1f)
    {
        if (!Configuration.enableLevelKnife) return;

        // Check if is from the server
        if (byPlayer is IServerPlayer && __instance.entity.World.Side == EnumAppSide.Server)
        {
            IServerPlayer player = byPlayer as IServerPlayer;
            // Earny xp by harvesting entity
            instance.serverAPI?.OnClientMessage(player, "Knife_Harvest_Entity");

            // Store the old drop rate
            player.Entity.Stats.Set("old_animalLootDropRate", "old_animalLootDropRate", player.Entity.Stats.GetBlended("animalLootDropRate"));

            // Increasing entity drop rate
            player.Entity.Stats.Set("animalLootDropRate", "animalLootDropRate", Configuration.KnifeGetHarvestMultiplyByEXP((ulong)player.Entity.WatchedAttributes.GetLong("LevelUP_Knife")));
            if (Configuration.enableExtendedLog)
                Debug.Log($"{player.PlayerName} harvested any entity with knife, multiply drop: {Configuration.KnifeGetHarvestMultiplyByEXP((ulong)player.Entity.WatchedAttributes.GetLong("LevelUP_Knife"))}");
        }
        // Single player treatment and lan treatment
        else if (instance.clientAPI != null && instance.clientAPI.api.IsSinglePlayer) instance.clientAPI.channel.SendPacket($"Knife_Harvest_Entity&lanplayername={byPlayer.PlayerName}");
    }
    // Overwrite Knife Harvesting
    [HarmonyPostfix]
    [HarmonyPatch(typeof(EntityBehaviorHarvestable), "SetHarvested")]
    public static void SetHarvestedKnifeFinish(EntityBehaviorHarvestable __instance, IPlayer byPlayer, float dropQuantityMultiplier = 1f)
    {
        if (!Configuration.enableLevelKnife) return;
        // Check if is from the server
        if (byPlayer is IServerPlayer && __instance.entity.World.Side == EnumAppSide.Server)
        {
            IServerPlayer player = byPlayer as IServerPlayer;

            // Reload old drop rate
            player.Entity.Stats.Set("animalLootDropRate", "animalLootDropRate", player.Entity.Stats.GetBlended("old_animalLootDropRate"));
        };
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
                if (playerEntity.Player is IServerPlayer) instance.serverAPI?.OnClientMessage(playerEntity.Player as IServerPlayer, "Soil_Till");
            }
        }
        // Single player treatment and lan treatment
        else if (instance.clientAPI != null && instance.clientAPI.api.IsSinglePlayer && secondsUsed >= 1.0f) instance.clientAPI.channel.SendPacket($"Soil_Till&lanplayername={byEntity.GetName()}");
    }
    #endregion

    #region cooking
    static int cookingFirePitOverflow = 0;
    // Overwrite Fire Pit
    [HarmonyPostfix]
    [HarmonyPatch(typeof(BlockEntityFirepit), "heatInput")]
    public static void HeatInput(BlockEntityFirepit __instance, float dt)
    {
        // if (!Configuration.enableLevelCooking || __instance.Api.World.Side != EnumAppSide.Server) return;

        // Hol up, let him cook
        float maxCookingTime = __instance.inputSlot.Itemstack.Collectible.GetMeltingDuration(__instance.Api.World, (ISlotProvider)__instance.Inventory, __instance.inputSlot);
        float cookingTime = __instance.inputStackCookingTime;

        if (Configuration.enableExtendedLog && (int)cookingTime % 10 == 0 && cookingTime > 0 && cookingTime < maxCookingTime)
            Debug.Log($"Cooking: {cookingTime} / {maxCookingTime}");

        // Check if him finished cooking
        if (cookingTime >= maxCookingTime)
        {
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
                        freshHours.value[0] *= Configuration.CookingGetFreshHoursMultiplyByEXP((ulong)player.Entity.WatchedAttributes.GetLong("LevelUP_Cooking"));
                        if (Configuration.enableExtendedLog)
                            Debug.Log($"Cooking: fresh hours increased to: {freshHours.value[0]} with multiply of {Configuration.CookingGetFreshHoursMultiplyByEXP((ulong)player.Entity.WatchedAttributes.GetLong("LevelUP_Cooking"))}");
                        attribute.SetAttribute("freshHours", freshHours);
                        output.Attributes["transitionstate"] = attribute;
                    }
                    // Dedicated Servers
                    if (instance.serverAPI != null)
                        instance.serverAPI.OnClientMessage(player as IServerPlayer, $"Cooking_Finished&forceexp={(int)Math.Round(Configuration.ExpPerCookingcooking + (Configuration.ExpPerCookingcooking * expMultiplySingle))}");
                    // Single player treatment and lan treatment
                    else if (instance.clientAPI?.api.IsSinglePlayer ?? false)
                        instance.clientAPI.channel.SendPacket($"Cooking_Finished&forceexp={(int)Math.Round(Configuration.ExpPerCookingcooking + (Configuration.ExpPerCookingcooking * expMultiplySingle))}&lanplayername={player.PlayerName}");
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
                                freshHours.value[0] *= Configuration.CookingGetFreshHoursMultiplyByEXP((ulong)player.Entity.WatchedAttributes.GetLong("LevelUP_Cooking"));

                                if (Configuration.enableExtendedLog)
                                    Debug.Log($"Cooking: fresh hours increased to: {freshHours.value[0]} with multiply of {Configuration.CookingGetFreshHoursMultiplyByEXP((ulong)player.Entity.WatchedAttributes.GetLong("LevelUP_Cooking"))}");

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
                            servingsQuantity.value = Configuration.CookingGetServingsByEXPAndServings((ulong)player.Entity.WatchedAttributes.GetLong("LevelUP_Cooking"), (int)servingsQuantity.value); ;

                            if (Configuration.enableExtendedLog)
                                Debug.Log($"Cooking: servings now: {servingsQuantity.value}");

                            // Updating
                            attribute["quantityServings"] = servingsQuantity;
                            output.Attributes = attribute;
                        }
                    }

                    // Dedicated Servers
                    if (instance.serverAPI != null)
                        instance.serverAPI.OnClientMessage(player as IServerPlayer, $"Cooking_Finished&forceexp={(int)Math.Round(Configuration.ExpPerCookingcooking + (Configuration.ExpPerCookingcooking * expMultiplyPots))}");
                    // Single player treatment and lan treatment
                    else if (instance.clientAPI?.api.IsSinglePlayer ?? false)
                        instance.clientAPI.channel.SendPacket($"Cooking_Finished&forceexp={(int)Math.Round(Configuration.ExpPerCookingcooking + (Configuration.ExpPerCookingcooking * expMultiplyPots))}&lanplayername={player.PlayerName}");
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
    // // Overwrite the hammer animations
    // [HarmonyPrefix]
    // [HarmonyPatch(typeof(ItemHammer), "startHitAction")]
    // public static bool StartHitAction(ItemHammer __instance, ItemSlot slot, EntityAgent byEntity, bool merge)
    // {
    //     #region native
    //     void strikeAnvilSound()
    //     {
    //         IPlayer player = (byEntity as EntityPlayer).Player;
    //         if (player != null && player.CurrentBlockSelection != null)
    //         {
    //             player.Entity.World.PlaySoundAt(merge ? new AssetLocation("sounds/effect/anvilmergehit") : new AssetLocation("sounds/effect/anvilhit"), player.Entity, player, 0.9f + (float)byEntity.World.Rand.NextDouble() * 0.2f, 16f, 0.35f);
    //         }
    //     }
    //     if (!slot.Itemstack.TempAttributes.GetBool("isAnvilAction"))
    //     {
    //         string heldTpHitAnimation = __instance.GetHeldTpHitAnimation(slot, byEntity);
    //         float soundAtFrame = CollectibleBehaviorAnimationAuthoritative.getSoundAtFrame(byEntity, heldTpHitAnimation);
    //         float hitDamageAtFrame = CollectibleBehaviorAnimationAuthoritative.getHitDamageAtFrame(byEntity, heldTpHitAnimation);
    //         #endregion

    //         // Receive player experience
    //         ulong playerExp = (ulong)byEntity.WatchedAttributes.GetLong("LevelUP_Hammer");
    //         // Reduce frame counts
    //         soundAtFrame /= Configuration.HammerGetAnimationSpeedByEXP(playerExp);
    //         hitDamageAtFrame /= Configuration.HammerGetAnimationSpeedByEXP(playerExp);

    //         #region native
    //         slot.Itemstack.TempAttributes.SetBool("isAnvilAction", value: true);
    //         byEntity.AnimManager.RegisterFrameCallback(new AnimFrameCallback
    //         {
    //             Animation = heldTpHitAnimation,
    //             Frame = soundAtFrame,
    //             Callback = delegate
    //             {
    //                 strikeAnvilSound();
    //             }
    //         });
    //         byEntity.AnimManager.RegisterFrameCallback(new AnimFrameCallback
    //         {
    //             Animation = heldTpHitAnimation,
    //             Frame = hitDamageAtFrame,
    //             Callback = delegate
    //             {
    //                 strikeAnvilSound();
    //             }
    //         });
    //     }
    //     #endregion
    //     return false;
    // }

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
                ulong playerExp = (ulong)byPlayer.Entity.WatchedAttributes.GetLong("LevelUP_Hammer");
                int multiply = Configuration.HammerGetResultMultiplyByEXP(playerExp);
                // Multiply by the change
                __instance.SelectedRecipe.Output.ResolvedItemstack.StackSize = __instance.SelectedRecipe.Output.Quantity * multiply;

                if (Configuration.enableExtendedLog)
                    Debug.Log($"{byPlayer.PlayerName} finished smithing {__instance.SelectedRecipe.Output.ResolvedItemstack.Collectible?.Code} drop quantity: {__instance.SelectedRecipe.Output.Quantity} with a final result size of: {__instance.SelectedRecipe.Output.ResolvedItemstack.StackSize} multiplied by: {multiply}");
            }

            // Check if player is using the hammer
            if (byPlayer?.InventoryManager?.ActiveTool == EnumTool.Hammer)
                // Dedicated Servers
                if (instance.serverAPI != null)
                    instance.serverAPI.OnClientMessage(byPlayer as IServerPlayer, "Increase_Hammer_Hit");
                // Single player treatment
                else if (instance.clientAPI?.api.IsSinglePlayer ?? false)
                    instance.clientAPI.channel.SendPacket($"Increase_Hammer_Hit&lanplayername={byPlayer.PlayerName}");
        }
    }
    #endregion
}