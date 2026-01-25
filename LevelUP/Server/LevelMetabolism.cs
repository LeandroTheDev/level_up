#pragma warning disable CA1822
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection.Emit;
using System.Text;
using HarmonyLib;
using LevelUP.Client;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.Client.NoObf;
using Vintagestory.GameContent;

namespace LevelUP.Server;

class LevelMetabolism
{
    public readonly Harmony patch = new("levelup_metabolism");
    public void Patch()
    {
        if (!Harmony.HasAnyPatches("levelup_metabolism"))
        {
            patch.PatchCategory("levelup_metabolism");
        }
    }
    public void Unpatch()
    {
        if (Harmony.HasAnyPatches("levelup_metabolism"))
        {
            patch.UnpatchCategory("levelup_metabolism");
        }
    }

    private static readonly Dictionary<string, float> _playerLoadedMetabolismReceiveMultiply = [];
    public static readonly IReadOnlyDictionary<string, float> PlayerLoadedMetabolismReceiveMultiply
        = new ReadOnlyDictionary<string, float>(_playerLoadedMetabolismReceiveMultiply);

    public void Init()
    {
        Instance.api.Event.PlayerJoin += (player) => RefreshSaturationReceiveMultiply(player);
        Instance.api.Event.PlayerDisconnect += (player) => _playerLoadedMetabolismReceiveMultiply.Remove(player.PlayerUID);
        Instance.api.Event.RegisterGameTickListener(OnGameTick, 1000, 10000);
        OverwriteDamageInteractionEvents.OnPlayerReceiveDamageUnkown += HandleUnkownDamage;
        Configuration.RegisterNewLevel("Metabolism");
        Configuration.RegisterNewLevelTypeEXP("Metabolism", Configuration.MetabolismGetLevelByEXP);
        Configuration.RegisterNewEXPLevelType("Metabolism", Configuration.MetabolismGetExpByLevel);

        Debug.Log("Level Metabolism initialized");
    }

    public void InitClient()
    {
        StatusViewEvents.OnStatusRequested += StatusViewRequested;

        Debug.Log("Level Metabolism initialized");
    }

    public void Dispose()
    {
        OverwriteDamageInteractionEvents.OnPlayerReceiveDamageUnkown -= HandleUnkownDamage;
        StatusViewEvents.OnStatusRequested -= StatusViewRequested;
    }

    private void StatusViewRequested(IPlayer player, ref StringBuilder stringBuilder, string levelType)
    {
        if (levelType != "Metabolism") return;

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_saturationreducer",
                Utils.GetPorcentageFromDecrementalFloat(Configuration.MetabolismGetSaturationReceiveMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Metabolism")))
            )
        );

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_additionalsaturation",
                Configuration.MetabolismGetMaxSaturationByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Metabolism")) - Configuration.BaseSaturationMetabolism
            )
        );
    }

    private void HandleUnkownDamage(IPlayer player, DamageSource damageSource, ref float damage)
    {
        if (damageSource.Type == EnumDamageType.Hunger)
        {
            Experience.IncreaseExperience(player, "Metabolism", (ulong)Configuration.EXPPerHitMetabolism);
        }
    }

    private void OnGameTick(float obj)
    {
        foreach (IPlayer player in Instance.api.World.AllOnlinePlayers)
        {
            var stats = player.Entity.GetBehavior<EntityBehaviorHunger>();
            if (stats == null)
            {
                Debug.LogError($"[METABOLISM] [OnGameTick] ERROR GETTING SATURATION: Stats null for {player.PlayerName}");
                continue;
            }

            float expMultiply = 1.5f - (stats.Saturation / stats.MaxSaturation);
            ulong exp = (ulong)Math.Round(
                Configuration.EXPPerSaturationLostMetabolism * expMultiply
            );

            Experience.IncreaseExperience(player, "Metabolism", exp);
        }
    }

    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulateMetabolismConfiguration(coreAPI);
        Configuration.RegisterNewMaxLevelByLevelTypeEXP("Metabolism", Configuration.metabolismMaxLevel);
    }

    static public EntityBehaviorHunger RefreshMaxSaturation(IPlayer player)
    {
        // Get the actual player total exp
        ulong playerExp = Experience.GetExperience(player, "Metabolism");

        // Get player stats
        EntityBehaviorHunger playerStats = player.Entity.GetBehavior<EntityBehaviorHunger>();
        // Check if stats is null
        if (playerStats == null) { Debug.LogError($"[METABOLISM] ERROR SETTING SATURATION: Player Stats is null, caused by {player.PlayerName}"); return playerStats; }

        // Getting saturation stats
        float playerMaxSaturation = Configuration.MetabolismGetMaxSaturationByLevel(Configuration.MetabolismGetLevelByEXP(playerExp));
        if (float.IsInfinity(playerMaxSaturation))
        {
            Debug.LogError($"[METABOLISM] ERROR: Max saturation calculation returned any infinity number, please report this issue, base saturation set to {Configuration.BaseSaturationMetabolism}");
            playerMaxSaturation = Configuration.BaseSaturationMetabolism;
        }

        playerStats.MaxSaturation = playerMaxSaturation;
        player.Entity.WatchedAttributes.SetFloat("maxsaturation", playerStats.MaxSaturation);

        playerStats.UpdateNutrientHealthBoost();

        return playerStats;
    }

    static public float RefreshSaturationReceiveMultiply(IPlayer player)
    {
        ulong playerExp = Experience.GetExperience(player, "Metabolism");

        float saturationConsumeReducer = Configuration.MetabolismGetSaturationReceiveMultiplyByLevel(Configuration.MetabolismGetLevelByEXP(playerExp));

        if (_playerLoadedMetabolismReceiveMultiply.TryGetValue(player.PlayerUID, out float _))
            _playerLoadedMetabolismReceiveMultiply[player.PlayerUID] = saturationConsumeReducer;
        else
            _playerLoadedMetabolismReceiveMultiply.Add(player.PlayerUID, saturationConsumeReducer);

        return saturationConsumeReducer;
    }

    [HarmonyPatchCategory("levelup_metabolism")]
    private class LevelMetabolismPatch
    {
        internal static float GetReducerForPlayer(EntityBehaviorHunger instance)
        {
            if (instance.entity is EntityPlayer entityPlayer &&
                PlayerLoadedMetabolismReceiveMultiply.TryGetValue(entityPlayer.PlayerUID, out float reducer))
            {
                return reducer;
            }

            return 1f;
        }

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(EntityBehaviorHunger), "ConsumeSaturation")]
        internal static IEnumerable<CodeInstruction> ConsumeSaturationTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            var getReducerMethod = AccessTools.Method(typeof(LevelMetabolismPatch), nameof(GetReducerForPlayer));

            foreach (var code in instructions)
            {
                // Whenever you find ldarg.1, replace it with the multiplier.
                if (code.opcode == OpCodes.Ldarg_1)
                {
                    // ldarg.0 (this)
                    yield return new CodeInstruction(OpCodes.Ldarg_0);

                    // call GetReducerForPlayer
                    yield return new CodeInstruction(OpCodes.Call, getReducerMethod);

                    // ldarg.1 (amount)
                    yield return new CodeInstruction(OpCodes.Ldarg_1);

                    // mul  (amount * reducer)
                    yield return new CodeInstruction(OpCodes.Mul);

                    continue;
                }

                yield return code;
            }
        }

        // I don't know the reason, but some random function is changing the maxsaturation to default value randomly
        [HarmonyTranspiler] // Client side
        [HarmonyPatch(typeof(HudStatbar), "UpdateSaturation")]
        internal static IEnumerable<CodeInstruction> UpdateSaturationTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            static bool IsStloc(CodeInstruction c)
            {
                return c.opcode == OpCodes.Stloc_0
                    || c.opcode == OpCodes.Stloc_1
                    || c.opcode == OpCodes.Stloc_2
                    || c.opcode == OpCodes.Stloc_3
                    || c.opcode == OpCodes.Stloc_S
                    || c.opcode == OpCodes.Stloc;
            }

            var nullableCtor = typeof(float?)
                .GetConstructor([typeof(float)]);

            var getFloatMethod = AccessTools.Method(
                typeof(ITreeAttribute),
                "GetFloat",
                [typeof(string), typeof(float)]
            );

            var getWorld = AccessTools.PropertyGetter(typeof(ICoreClientAPI), "World");
            var getPlayer = AccessTools.PropertyGetter(typeof(IClientWorldAccessor), "Player");
            var getEntity = AccessTools.PropertyGetter(typeof(IPlayer), "Entity");

            var watchedAttributesField =
                AccessTools.Field(typeof(Entity), "WatchedAttributes");

            CodeInstruction prev = null;
            CodeInstruction prevPrev = null;

            foreach (var code in instructions)
            {
                // ldstr "maxsaturation" -> callvirt TryGetFloat -> stloc.*
                if (prevPrev != null &&
                    prev != null &&
                    prevPrev.opcode == OpCodes.Ldstr &&
                    prevPrev.operand is string s &&
                    s == "maxsaturation" &&
                    prev.opcode == OpCodes.Callvirt &&
                    IsStloc(code))
                {
                    // keep original store
                    yield return code;

                    // capi.World.Player.Entity.WatchedAttributes
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(GuiDialog), "capi"));
                    yield return new CodeInstruction(OpCodes.Callvirt, getWorld);
                    yield return new CodeInstruction(OpCodes.Callvirt, getPlayer);
                    yield return new CodeInstruction(OpCodes.Callvirt, getEntity);
                    yield return new CodeInstruction(OpCodes.Ldfld, watchedAttributesField);

                    // GetFloat("maxsaturation", 1500f)
                    yield return new CodeInstruction(OpCodes.Ldstr, "maxsaturation");
                    yield return new CodeInstruction(OpCodes.Ldc_R4, 1500f);
                    yield return new CodeInstruction(OpCodes.Callvirt, getFloatMethod);

                    // new float?(result)
                    yield return new CodeInstruction(OpCodes.Newobj, nullableCtor);

                    // store back into maxSaturation
                    yield return new CodeInstruction(code.opcode, code.operand);

                    prevPrev = prev;
                    prev = code;
                    continue;
                }

                yield return code;

                prevPrev = prev;
                prev = code;
            }
        }
    }
}