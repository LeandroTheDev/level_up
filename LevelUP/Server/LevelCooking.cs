#pragma warning disable CA1822
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using LevelUP.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
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

    public void InitClient()
    {
        StatusViewEvents.OnStatusRequested += StatusViewRequested;

        Debug.Log("Level Cooking initialized");
    }

    public void Dispose()
    {
        StatusViewEvents.OnStatusRequested -= StatusViewRequested;
    }

    public static readonly string[] SubLevelTypes = ["Firepit", "Oven"];

    private void StatusViewRequested(IPlayer player, ref StringBuilder stringBuilder, string levelType)
    {
        if (levelType != "Cooking") return;

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_freshhours",
                Utils.GetPorcentageFromFloatsStart1(Configuration.CookingGetFreshHoursMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Cooking")))
            )
        );

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_servingsrolls",
                Configuration.CookingGetRollsByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Cooking"))
            )
        );

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_servingchanceroll",
                Math.Round(Configuration.CookingGetRollChanceByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Cooking")), 2)
            )
        );

        stringBuilder.AppendLine("");

        stringBuilder.AppendLine(Lang.Get("levelup:status_proficiency"));

        foreach (string subType in SubLevelTypes)
        {
            stringBuilder.AppendLine($"{Lang.Get($"levelup:{subType.ToLower()}")}: {player.Entity.WatchedAttributes.GetInt($"LevelUP_Level_Cooking_Sub_{subType}")}");
        }
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

                    // Firepit sub level, combined multiplicatively with the main Cooking level
                    int firepitSubLevel = Configuration.CookingGetLevelByEXP(Experience.GetSubExperience(player, "Cooking", "Firepit"));
                    float firepitFreshHoursMultiply = Configuration.CookingGetFreshHoursMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Cooking"))
                        * Configuration.CookingGetFreshHoursMultiplyByLevel(firepitSubLevel);

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
                            freshHours.value[0] *= firepitFreshHoursMultiply;
                            LevelCookingEvents.UpdateFromExternalCookingSingle(player, output.Collectible.Code.ToString(), ref exp, ref freshHours.value[0]);
                            Debug.LogDebug($"Cooking: fresh hours increased to: {freshHours.value[0]} with multiply of {firepitFreshHoursMultiply}");
                            attribute.SetAttribute("freshHours", freshHours);
                            output.Attributes["transitionstate"] = attribute;
                        }

                        Experience.IncreaseExperience(player, "Cooking", exp);
                        Experience.IncreaseSubExperience(player, "Cooking", "Firepit", (ulong)Math.Round(exp * Configuration.cookingSubLevelEXPMultiply));
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
                                    freshHours.value[0] *= firepitFreshHoursMultiply;

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

                                // Increasing servings quantity (main level, then Firepit sub level)
                                int servings = Configuration.CookingGetServingsByLevelAndServings(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Cooking"), (int)servingsQuantity.value);
                                servings = Configuration.CookingGetServingsByLevelAndServings(firepitSubLevel, servings);
                                servingsQuantity.value = servings;

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
                                        freshHours.value[0] *= firepitFreshHoursMultiply;

                                        Debug.LogDebug($"Cooking: fresh hours increased to: {freshHours.value[0]} with multiply of {firepitFreshHoursMultiply}");

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
                        Experience.IncreaseSubExperience(player, "Cooking", "Firepit", (ulong)Math.Round(exp * Configuration.cookingSubLevelEXPMultiply));
                    }
                });
            }
        }

        // Overwrite Clay Oven baking (bread, pies, ...)
        [HarmonyPrefix]
        [HarmonyPatch(typeof(BlockEntityOven), "IncrementallyBake")]
        internal static void IncrementallyBakeStart(BlockEntityOven __instance, int slotIndex, out string __state)
        {
            __state = null;

            if (!Configuration.enableLevelCooking) return;
            if (__instance.Api.World.Side != EnumAppSide.Server) return;

            __state = __instance.Inventory[slotIndex].Itemstack?.Collectible.Code.ToString();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(BlockEntityOven), "IncrementallyBake")]
        internal static void IncrementallyBakeFinish(BlockEntityOven __instance, int slotIndex, string __state)
        {
            if (!Configuration.enableLevelCooking) return;
            if (__instance.Api.World.Side != EnumAppSide.Server) return;
            if (__state == null) return;

            ItemStack output = __instance.Inventory[slotIndex].Itemstack;
            if (output == null) return;

            // Item did not finish transitioning into a new stage this tick
            string outputCode = output.Collectible.Code.ToString();
            if (outputCode == __state) return;

            if (!Configuration.expMultiplyOvenCooking.TryGetValue(outputCode, out double expMultiplyOven)) return;

            IPlayer player = __instance.Api.World.NearestPlayer(__instance.Pos.X, __instance.Pos.Y, __instance.Pos.Z);

            // If cannot find the nearest player
            if (player == null)
            {
                Debug.LogDebug("Cooking: player is null, oven experience and stats has been ignored");
                return;
            }

            ulong exp = (ulong)Math.Round(Configuration.ExpPerCookingcooking + (Configuration.ExpPerCookingcooking * expMultiplyOven));

            // Oven sub level, combined multiplicatively with the main Cooking level
            int ovenSubLevel = Configuration.CookingGetLevelByEXP(Experience.GetSubExperience(player, "Cooking", "Oven"));
            float ovenFreshHoursMultiply = Configuration.CookingGetFreshHoursMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Cooking"))
                * Configuration.CookingGetFreshHoursMultiplyByLevel(ovenSubLevel);

            // Increase the fresh hours based in player experience, if the baked item perishes
            if (output.Attributes["transitionstate"] is TreeAttribute attribute && attribute.GetAttribute("freshHours") is FloatArrayAttribute freshHours)
            {
                Debug.LogDebug($"Cooking: previously fresh hours: {freshHours.value[0]}");
                freshHours.value[0] *= ovenFreshHoursMultiply;
                Debug.LogDebug($"Cooking: fresh hours increased to: {freshHours.value[0]} with multiply of {ovenFreshHoursMultiply}");
                attribute.SetAttribute("freshHours", freshHours);
                output.Attributes["transitionstate"] = attribute;
            }

            // Increase servings quantity (main level, then Oven sub level), if the baked item has servings, like a pie
            if (output.Attributes["quantityServings"] is FloatAttribute servingsQuantity)
            {
                Debug.LogDebug($"Cooking: previously servings: {servingsQuantity.value}");
                int servings = Configuration.CookingGetServingsByLevelAndServings(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Cooking"), (int)servingsQuantity.value);
                servings = Configuration.CookingGetServingsByLevelAndServings(ovenSubLevel, servings);
                servingsQuantity.value = servings;
                Debug.LogDebug($"Cooking: servings now: {servingsQuantity.value}");
            }

            Experience.IncreaseExperience(player, "Cooking", exp);
            Experience.IncreaseSubExperience(player, "Cooking", "Oven", (ulong)Math.Round(exp * Configuration.cookingSubLevelEXPMultiply));

            Debug.LogDebug($"{player.PlayerName} finished baking {outputCode} in the clay oven, experience: {exp}");
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