using System.Threading.Tasks;
using HarmonyLib;
using Vintagestory.API.Common;
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

    // Overwrite Cutlery Harvesting
    [HarmonyPrefix]
    [HarmonyPatch(typeof(EntityBehaviorHarvestable), "SetHarvested")]
    public static void SetHarvested(EntityBehaviorHarvestable __instance, IPlayer byPlayer, float dropQuantityMultiplier = 1f)
    {
        // Check if is from the server
        if (byPlayer is IServerPlayer && __instance.entity.World.Side == EnumAppSide.Server)
        {
            IServerPlayer player = byPlayer as IServerPlayer;
            // Earny xp by harvesting entity
            instance.serverAPI?.OnClientMessage(player, "Cutlery_Harvest_Entity");

            // Increasing entity drop rate
            player.Entity.Stats.Set("animalLootDropRate", "animalLootDropRate", Configuration.CutleryGetHarvestMultiplyByEXP(player.Entity.WatchedAttributes.GetAsInt("LevelUP_Cutlery")));
        };
    }

    // Overwrite Hoe Till
    [HarmonyPostfix]
    [HarmonyPatch(typeof(ItemHoe), "OnHeldInteractStep")]
    public static void OnHeldInteractStep(bool __result, float secondsUsed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel)
    {
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
    }

    // // Used for cooking communication
    // private static IServerPlayer temporaryCookPlayer;
    // // Overwrite fire ticks
    // [HarmonyPrefix]
    // [HarmonyPatch(typeof(BlockEntityFirepit), "OnBurnTick")]
    // public static void OnBurnTick(BlockEntityFirepit __instance, float dt)
    // {
    //     // Check if is server side
    //     if (__instance.Api.World.Side == EnumAppSide.Server)
    //     {
    //         // Check if cooking finished
    //         if (__instance.canSmeltInput() && __instance.inputStackCookingTime > __instance.maxCookingTime())
    //         {
    //             Debug.Log("COOKING FINISHED"); //DELETE THIS
    //             // Get the nearest player from the cooking fire
    //             IPlayer player = __instance.Api.World.NearestPlayer(__instance.Pos.X, __instance.Pos.Y, __instance.Pos.Z);
    //             if (player != null && player is IServerPlayer)
    //             {
    //                 Debug.Log($"NEAREST PLAYER FINDED {player.PlayerName}"); //DELETE THIS
    //                 // Create the cook player temporary
    //                 temporaryCookPlayer = player as IServerPlayer;
    //                 // Hol up, let him cook
    //                 instance.serverAPI.OnClientMessage(player as IServerPlayer, "Cooking_Finished"); // Increase player cooking xp
    //             }
    //             else
    //             {
    //                 // Why did you let him cook? WHY?
    //             }
    //         }
    //     }
    // }

    // // Overwrite Cooking Smelt
    // [HarmonyPrefix]
    // [HarmonyPatch(typeof(BlockCookingContainer), "DoSmelt")]
    // public static bool DoSmelt(BlockCookingContainer __instance, IWorldAccessor world, ISlotProvider cookingSlotsProvider, ItemSlot inputSlot, ItemSlot outputSlot)
    // {
    //     // If temporary cook player doesnt exist run the native code instead
    //     if (temporaryCookPlayer == null) return true;

    //     // Get the player instance
    //     IServerPlayer player = temporaryCookPlayer;
    //     temporaryCookPlayer = null;

    //     Debug.Log($"TEMPORARY PLAYER EXIST {player.PlayerName}"); //DELETE THIS

    //     // This functions is overwrited from CollectiibleObjects.CarryOverFreshness
    //     void CarryOverFreshness(ICoreAPI api, ItemSlot[] inputSlots, ItemStack[] outStacks, TransitionableProperties perishProps)
    //     {
    //         #region native
    //         float num = 0f;
    //         float num2 = 0;
    //         float num3 = 0f;
    //         int num4 = 0;
    //         foreach (ItemSlot itemSlot in inputSlots)
    //         {
    //             if (!itemSlot.Empty)
    //             {
    //                 TransitionState transitionState = itemSlot.Itemstack?.Collectible?.UpdateAndGetTransitionState(api.World, itemSlot, EnumTransitionType.Perish);
    //                 if (transitionState != null)
    //                 {
    //                     num4++;
    //                     float num5 = transitionState.TransitionedHours / (transitionState.TransitionedHours + transitionState.FreshHours);
    //                     float num6 = Math.Max(0f, (transitionState.TransitionedHours - transitionState.FreshHours) / transitionState.TransitionedHours);
    //                     num2 = Math.Max(num6, num2);
    //                     num += num5;
    //                     num3 += num6;
    //                 }
    //             }
    //         }
    //         num /= (float)Math.Max(1, num4);
    //         num3 /= (float)Math.Max(1, num4);
    //         for (int j = 0; j < outStacks.Length; j++)
    //         {
    //             if (outStacks[j] != null)
    //             {
    //                 if (outStacks[j].Attributes["transitionstate"] is not ITreeAttribute)
    //                 {
    //                     outStacks[j].Attributes["transitionstate"] = new TreeAttribute();
    //                 }
    //                 float num7 = perishProps.TransitionHours.nextFloat(1f, api.World.Rand);
    //                 float num8 = perishProps.FreshHours.nextFloat(1f, api.World.Rand);
    //                 ITreeAttribute treeAttribute = (ITreeAttribute)outStacks[j].Attributes["transitionstate"];
    //                 treeAttribute.SetDouble("createdTotalHours", api.World.Calendar.TotalHours);
    //                 treeAttribute.SetDouble("lastUpdatedTotalHours", api.World.Calendar.TotalHours);
    //                 treeAttribute["freshHours"] = new FloatArrayAttribute([num8]);
    //                 #endregion

    //                 // Increasing fresh hours
    //                 treeAttribute["freshHours"] = new FloatArrayAttribute([num8 * Configuration.CookingGetFreshHoursMultiplyByEXP(player.Entity.WatchedAttributes.GetInt("LevelUP_Cooking", 0))]);

    //                 #region native
    //                 treeAttribute["transitionHours"] = new FloatArrayAttribute([num7]);
    //                 if (num3 > 0f)
    //                 {
    //                     num3 *= 0.6f;
    //                     treeAttribute["transitionedHours"] = new FloatArrayAttribute([num8 + Math.Max(0f, num7 * num3 - 2f)]);
    //                 }
    //                 else
    //                 {
    //                     treeAttribute["transitionedHours"] = new FloatArrayAttribute([Math.Max(0f, num * (0.8f + (float)(2 + num4) * num7 + num8))]);
    //                 }
    //             }
    //         }
    //         #endregion
    //     }


    //     #region native
    //     ItemStack[] array = __instance.GetCookingStacks(cookingSlotsProvider);
    //     CookingRecipe matchingCookingRecipe = __instance.GetMatchingCookingRecipe(world, array);
    //     Block block = world.GetBlock(__instance.CodeWithVariant("type", "cooked"));

    //     if (matchingCookingRecipe == null) return false;
    //     int quantityServings = matchingCookingRecipe.GetQuantityServings(array);
    //     #endregion

    //     // Increasing quantity servings based on cooking level
    //     quantityServings += Configuration.CookingGetServingsByEXPAndServings(player.Entity.WatchedAttributes.GetInt("LevelUP_Cooking"), quantityServings);

    //     #region native
    //     if (matchingCookingRecipe.DirtyPot)
    //     {
    //         Item item = world.GetItem(new AssetLocation(matchingCookingRecipe.DirtyPotOutput));
    //         if (item != null)
    //         {
    //             array = [new(item, quantityServings)];
    //             block = world.GetBlock(new AssetLocation(__instance.Attributes["dirtiedBlockCode"].AsString()));
    //         }
    //     }
    //     else
    //     {
    //         for (int i = 0; i < array.Length; i++)
    //         {
    //             ItemStack itemStack = matchingCookingRecipe.GetIngrendientFor(array[i]).GetMatchingStack(array[i])?.CookedStack?.ResolvedItemstack.Clone();
    //             if (itemStack != null)
    //             {
    //                 array[i] = itemStack;
    //             }
    //         }
    //     }

    //     ItemStack itemStack2 = new(block);
    //     TransitionableProperties transitionableProperties = matchingCookingRecipe.PerishableProps.Clone();
    //     transitionableProperties.TransitionedStack.Resolve(world, "cooking container perished stack");
    //     #endregion

    //     // Change the function from native CollectiibleObjects.CarryOverFreshness to new CarryOverFreshness
    //     CarryOverFreshness(instance.coreAPI, cookingSlotsProvider.Slots, array, transitionableProperties);

    //     #region native
    //     for (int j = 0; j < array.Length; j++)
    //     {
    //         array[j].StackSize /= quantityServings;
    //     }
    //     ((BlockCookedContainer)block).SetContents(matchingCookingRecipe.Code, quantityServings, itemStack2, array);
    //     itemStack2.Collectible.SetTemperature(world, itemStack2, BlockCookingContainer.GetIngredientsTemperature(world, array));
    //     outputSlot.Itemstack = itemStack2;
    //     inputSlot.Itemstack = null;
    //     for (int k = 0; k < cookingSlotsProvider.Slots.Length; k++)
    //     {
    //         cookingSlotsProvider.Slots[k].Itemstack = null;
    //     }
    //     return false;
    //     #endregion
    // }
}