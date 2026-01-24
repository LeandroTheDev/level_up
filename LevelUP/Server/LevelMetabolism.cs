#pragma warning disable CA1822
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using HarmonyLib;
using LevelUP.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
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

    static private string _saveDirectory = "";
    private static readonly Dictionary<string, double> _playerLoadedMetabolism = [];
    private static readonly Dictionary<string, float> _playerLoadedMetabolismReceiveMultiply = [];
    public static readonly IReadOnlyDictionary<string, float> PlayerLoadedMetabolismReceiveMultiply
        = new ReadOnlyDictionary<string, float>(_playerLoadedMetabolismReceiveMultiply);

    public void Init()
    {
        Instance.api.Event.PlayerJoin += (player) => RefreshSaturationReceiveMultiply(player);
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
        Thread thread = new(() =>
        {
            foreach (var kvp in _playerLoadedMetabolism)
            {
                IPlayer player = Instance.api.World.AllOnlinePlayers
                    .FirstOrDefault(p => p.PlayerUID == kvp.Key);

                if (player == null)
                {
                    Debug.LogError($"[METABOLISM] [OnGameTick] Cannot find player for {kvp.Key}");
                    continue;
                }

                var stats = player.Entity.GetBehavior<EntityBehaviorHunger>();
                if (stats == null)
                {
                    Debug.LogError($"[METABOLISM] [OnGameTick] ERROR GETTING SATURATION: Stats null for {player.PlayerName}");
                    continue;
                }

                if (stats.Saturation < kvp.Value)
                {
                    _playerLoadedMetabolism[kvp.Key] = stats.Saturation;
                    Experience.IncreaseExperience(player, "Metabolism",
                        (ulong)Configuration.EXPPerSaturationLostMetabolism);
                }
            }
        })
        {
            IsBackground = true,
            Priority = ThreadPriority.Lowest
        };
        thread.Start();
    }

    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Load player state
        _saveDirectory = Path.Combine(coreAPI.DataBasePath, $"ModData/LevelUP/{coreAPI.World.SavegameIdentifier}-Metabolism");
        Debug.Log($"LevelUP will save metabolism data in: {_saveDirectory}");
        Directory.CreateDirectory(_saveDirectory);

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
            if (!Configuration.enableLevelMetabolism)
                return 1f;

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
                // Sempre que encontrar ldarg.1, substitui pelo multiplicado
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
    }
}