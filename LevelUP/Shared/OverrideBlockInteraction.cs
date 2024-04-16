using System;
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
        // Single player treatment
        else if (instance.clientAPI != null && instance.clientAPI.api.IsSinglePlayer) instance.clientAPI.channel.SendPacket("Knife_Harvest_Entity");
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
        // Single player treatment
        else if (instance.clientAPI != null && instance.clientAPI.api.IsSinglePlayer && secondsUsed >= 1.0f) instance.clientAPI.channel.SendPacket("Soil_Till");
    }
    #endregion

    #region cooking
    // Overwrite Fire Pit
    [HarmonyPostfix]
    [HarmonyPatch(typeof(BlockEntityFirepit), "heatInput")]
    public static void HeatInput(BlockEntityFirepit __instance, float dt)
    {
        float maxCookingTime = __instance.inputSlot.Itemstack.Collectible.GetMeltingDuration(__instance.Api.World, (ISlotProvider)__instance.Inventory, __instance.inputSlot);
        float cookingTime = __instance.inputStackCookingTime;
        if (cookingTime >= maxCookingTime)
        {
            // Check if the output existed before the cooking finished
            bool firstOutput = __instance.outputStack == null;

            if (Configuration.enableExtendedLog) Debug.Log($"{__instance.inputStack.Collectible.Code} finished cooking");
            // Run on secondary thread to not freeze the server
            Task thread = Task.Run(() =>
            {
                // Because output is magically added by something we need to constantly check it
                while (__instance.outputStack == null) { }
                // Finally receive output
                ItemStack output = __instance.outputStack;
                // Check if output is not any item
                if (output is null) return;

                if (Configuration.enableExtendedLog) Debug.Log($"Cooking output: {output.Collectible.Code}");

                // Update player experience to the most proximity player
                if (instance.serverAPI?.api.World.NearestPlayer(__instance.Pos.X, __instance.Pos.Y, __instance.Pos.Z) is not IServerPlayer player)
                {
                    if (Configuration.enableExtendedLog) Debug.Log("Cooking: player is null, cooking experience and stats has been ignored");
                    return;
                }

                // Check if is first output
                if (Configuration.expMultiplySingleCooking.TryGetValue(output.Collectible.Code.ToString(), out double expMultiply))
                {
                    if (firstOutput)
                    {
                        // Increase the fresh hours based in player experience
                        TreeAttribute attribute = output.Attributes["transitionstate"] as TreeAttribute;
                        FloatArrayAttribute freshHours = attribute.GetAttribute("freshHours") as FloatArrayAttribute;
                        freshHours.value[0] *= Configuration.CookingGetFreshHoursMultiplyByEXP((ulong)player.Entity.WatchedAttributes.GetLong("LevelUP_Cooking"));
                        if (Configuration.enableExtendedLog)
                            Debug.Log($"Cooking: fresh hours increased to: {freshHours.value[0]} with multiply of {Configuration.CookingGetFreshHoursMultiplyByEXP((ulong)player.Entity.WatchedAttributes.GetLong("LevelUP_Cooking"))}");
                        attribute.SetAttribute("freshHours", freshHours);
                    }
                    instance.serverAPI.OnClientMessage(player, $"Cooking_Finished&forceexp={(int)Math.Round(Configuration.ExpPerCookingcooking + (Configuration.ExpPerCookingcooking * expMultiply))}");
                }
            });
            // Thread timeout
            Task.Delay(100).ContinueWith((_) =>
            {
                if (!thread.IsCompleted)
                {
                    Debug.Log("WARNING: Output thread in cooking function has forcelly disposed, did the server is overloaded? cooking experience and status have been lost");
                    thread.Dispose();
                };
            });
        }
    }

    // Overwrite Cooking Pot Creation
    [HarmonyPrefix]
    [HarmonyPatch(typeof(BlockCookingContainer), "DoSmelt")]
    public static bool DoSmelt(BlockCookingContainer __instance, IWorldAccessor world, ISlotProvider cookingSlotsProvider, ItemSlot inputSlot, ItemSlot outputSlot)
    {
        // Getting the cooking level from previous player
        long playerCookingEXP = inputSlot.Itemstack.Attributes.GetLong("LevelUP_Cooking", 0);
        // Check if found the cooking level
        if (playerCookingEXP == 0)
        {
            if (Configuration.enableExtendedLog) Debug.Log("No level found for cooking, ignoring");
            return true;
        }

        // This functions is overwrited from CollectiibleObjects.CarryOverFreshness
        void CarryOverFreshness(ICoreAPI api, ItemSlot[] inputSlots, ItemStack[] outStacks, TransitionableProperties perishProps)
        {
            #region native
            float num = 0f;
            float num2 = 0;
            float num3 = 0f;
            int num4 = 0;
            foreach (ItemSlot itemSlot in inputSlots)
            {
                if (!itemSlot.Empty)
                {
                    TransitionState transitionState = itemSlot.Itemstack?.Collectible?.UpdateAndGetTransitionState(api.World, itemSlot, EnumTransitionType.Perish);
                    if (transitionState != null)
                    {
                        num4++;
                        float num5 = transitionState.TransitionedHours / (transitionState.TransitionedHours + transitionState.FreshHours);
                        float num6 = Math.Max(0f, (transitionState.TransitionedHours - transitionState.FreshHours) / transitionState.TransitionedHours);
                        num2 = Math.Max(num6, num2);
                        num += num5;
                        num3 += num6;
                    }
                }
            }
            num /= (float)Math.Max(1, num4);
            num3 /= (float)Math.Max(1, num4);
            for (int j = 0; j < outStacks.Length; j++)
            {
                if (outStacks[j] != null)
                {
                    if (outStacks[j].Attributes["transitionstate"] is not ITreeAttribute)
                    {
                        outStacks[j].Attributes["transitionstate"] = new TreeAttribute();
                    }
                    float num7 = perishProps.TransitionHours.nextFloat(1f, api.World.Rand);
                    float num8 = perishProps.FreshHours.nextFloat(1f, api.World.Rand);
                    ITreeAttribute treeAttribute = (ITreeAttribute)outStacks[j].Attributes["transitionstate"];
                    treeAttribute.SetDouble("createdTotalHours", api.World.Calendar.TotalHours);
                    treeAttribute.SetDouble("lastUpdatedTotalHours", api.World.Calendar.TotalHours);
                    treeAttribute["freshHours"] = new FloatArrayAttribute([num8]);
                    #endregion

                    // Increasing fresh hours
                    treeAttribute["freshHours"] = new FloatArrayAttribute([num8 * Configuration.CookingGetFreshHoursMultiplyByEXP((ulong)playerCookingEXP)]);

                    #region native
                    treeAttribute["transitionHours"] = new FloatArrayAttribute([num7]);
                    if (num3 > 0f)
                    {
                        num3 *= 0.6f;
                        treeAttribute["transitionedHours"] = new FloatArrayAttribute([num8 + Math.Max(0f, num7 * num3 - 2f)]);
                    }
                    else
                    {
                        treeAttribute["transitionedHours"] = new FloatArrayAttribute([Math.Max(0f, num * (0.8f + (float)(2 + num4) * num7 + num8))]);
                    }
                }
            }
            #endregion
        }

        #region native
        ItemStack[] array = __instance.GetCookingStacks(cookingSlotsProvider);
        CookingRecipe matchingCookingRecipe = __instance.GetMatchingCookingRecipe(world, array);
        Block block = world.GetBlock(__instance.CodeWithVariant("type", "cooked"));

        if (matchingCookingRecipe == null) return false;
        int quantityServings = matchingCookingRecipe.GetQuantityServings(array);
        #endregion

        // Increasing quantity servings based on cooking level
        quantityServings += Configuration.CookingGetServingsByEXPAndServings((ulong)playerCookingEXP, quantityServings);

        #region native
        if (matchingCookingRecipe.DirtyPot)
        {
            Item item = world.GetItem(new AssetLocation(matchingCookingRecipe.DirtyPotOutput));
            if (item != null)
            {
                array = [new(item, quantityServings)];
                block = world.GetBlock(new AssetLocation(__instance.Attributes["dirtiedBlockCode"].AsString()));
            }
        }
        else
        {
            for (int i = 0; i < array.Length; i++)
            {
                ItemStack itemStack = matchingCookingRecipe.GetIngrendientFor(array[i]).GetMatchingStack(array[i])?.CookedStack?.ResolvedItemstack.Clone();
                if (itemStack != null)
                {
                    array[i] = itemStack;
                }
            }
        }

        ItemStack itemStack2 = new(block);
        TransitionableProperties transitionableProperties = matchingCookingRecipe.PerishableProps.Clone();
        transitionableProperties.TransitionedStack.Resolve(world, "cooking container perished stack");
        #endregion

        // Change the function from native CollectiibleObjects.CarryOverFreshness to new CarryOverFreshness
        CarryOverFreshness(instance.coreAPI, cookingSlotsProvider.Slots, array, transitionableProperties);

        #region native
        for (int j = 0; j < array.Length; j++)
        {
            array[j].StackSize /= quantityServings;
        }
        ((BlockCookedContainer)block).SetContents(matchingCookingRecipe.Code, quantityServings, itemStack2, array);
        itemStack2.Collectible.SetTemperature(world, itemStack2, BlockCookingContainer.GetIngredientsTemperature(world, array));
        outputSlot.Itemstack = itemStack2;
        inputSlot.Itemstack = null;
        for (int k = 0; k < cookingSlotsProvider.Slots.Length; k++)
        {
            cookingSlotsProvider.Slots[k].Itemstack = null;
        }
        return false;
        #endregion
    }
    #endregion
}