#pragma warning disable CA1822
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.GameContent;

namespace LevelUP.Server;

class LevelCooking
{
    public readonly Harmony patch = new("levelup_cooking");
    public void Patch()
    {
        if (!Harmony.HasAnyPatches("levelup_cooking"))
        {
            patch.PatchCategory("levelup_cooking");
        }
    }
    public void Unpatch()
    {
        if (Harmony.HasAnyPatches("levelup_cooking"))
        {
            patch.UnpatchCategory("levelup_cooking");
        }
    }

    public void Init()
    {
        Configuration.RegisterNewLevel("Cooking");
        Configuration.RegisterNewLevelTypeEXP("Cooking", Configuration.CookingGetLevelByEXP);
        Configuration.RegisterNewEXPLevelType("Cooking", Configuration.CookingGetExpByLevel);

        Debug.Log("Level Cooking initialized");
    }

    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulateCookingConfiguration(coreAPI);
        Configuration.RegisterNewMaxLevelByLevelTypeEXP("Cooking", Configuration.cookingMaxLevel);
    }

    [HarmonyPatchCategory("levelup_cooking")]
    private class LevelCookingPatch
    {
        // Overwrite Fire Pit
        [HarmonyPostfix]
        [HarmonyPatch(typeof(BlockEntityFirepit), "heatInput")]
        internal static void HeatInput(BlockEntityFirepit __instance, float dt)
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
                            LevelCookingEvents.UpdateFromExternalCookingSingle(player, output.Collectible.Code.ToString(), ref exp, ref freshHours.value[0]);
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
                                LevelCookingEvents.UpdateFromExternalCookingPot(player, output.Collectible.Code.ToString(), ref exp, ref indexFreshHours, ref servingsQuantity.value);

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
                });
            }
        }
    }
}

public class LevelCookingEvents
{
    public delegate void PlayerCookingSingleHandler(IPlayer player, string code, ref ulong exp, ref float freshHours);
    public delegate void PlayerCookingPotHandler(IPlayer player, string code, ref ulong exp, ref List<float> freshHours, ref float servings);

    public static event PlayerCookingSingleHandler OnCookedSingle;
    public static event PlayerCookingPotHandler OnCookedPot;

    internal static void UpdateFromExternalCookingSingle(IPlayer player, string code, ref ulong exp, ref float freshHours)
    {
        OnCookedSingle?.Invoke(player, code, ref exp, ref freshHours);
    }

    internal static void UpdateFromExternalCookingPot(IPlayer player, string code, ref ulong exp, ref List<float> freshHours, ref float servings)
    {
        OnCookedPot?.Invoke(player, code, ref exp, ref freshHours, ref servings);
    }
}