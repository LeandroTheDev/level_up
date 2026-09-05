#pragma warning disable CA1822
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using HarmonyLib;
using LevelUP.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
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

    private static readonly Dictionary<string, float> _playerLoadedMetabolismReceiveMultiply = [];
    public static readonly IReadOnlyDictionary<string, float> PlayerLoadedMetabolismReceiveMultiply
        = new ReadOnlyDictionary<string, float>(_playerLoadedMetabolismReceiveMultiply);

    private static readonly Dictionary<string, float> _previousSaturation = [];

    public void Init()
    {
        Instance.api.Event.PlayerJoin += (player) => RefreshSaturationReceiveMultiply(player);
        Instance.api.Event.PlayerDisconnect += (player) =>
        {
            _playerLoadedMetabolismReceiveMultiply.Remove(player.PlayerUID);
            _previousSaturation.Remove(player.PlayerUID);
        };
        Instance.api.Event.RegisterGameTickListener(OnGameTick, 1000, 10000);
        OverwriteDamageInteractionEvents.OnPlayerReceiveDamageUnkown += HandleUnkownDamage;
        Configuration.RegisterNewLevel("Metabolism");
        Configuration.RegisterNewLevelTypeEXP("Metabolism", Configuration.MetabolismGetLevelByEXP);
        Configuration.RegisterNewEXPLevelType("Metabolism", Configuration.MetabolismGetExpByLevel);

        Debug.Log("Level Metabolism initialized");
    }

    public void InitClient()
    {
        StatusViewEvents.OnStatusRequested -= StatusViewRequested;
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
            if (player.Entity == null || !player.Entity.Alive) continue;
            EntityBehaviorHunger stats;
            try { stats = player.Entity.GetBehavior<EntityBehaviorHunger>(); }
            catch { continue; }
            if (stats == null)
            {
                Debug.LogError($"[METABOLISM] [OnGameTick] ERROR GETTING SATURATION: Stats null for {player.PlayerName}");
                continue;
            }

            string uid = player.PlayerUID;
            if (!_previousSaturation.TryGetValue(uid, out float prevSaturation))
            {
                _previousSaturation[uid] = stats.Saturation;
                continue;
            }

            float saturationLost = prevSaturation - stats.Saturation;
            _previousSaturation[uid] = stats.Saturation;

            if (saturationLost <= 0f) continue;

            ulong exp = (ulong)Math.Round(Configuration.EXPPerSaturationLostMetabolism * saturationLost);

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

        [HarmonyPrefix]
        [HarmonyPatch(typeof(EntityBehaviorHunger), "ConsumeSaturation")]
        internal static void ConsumeSaturationPrefix(EntityBehaviorHunger __instance, ref float amount)
        {
            amount *= GetReducerForPlayer(__instance);
        }
    }
}