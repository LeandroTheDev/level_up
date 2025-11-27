#pragma warning disable CA1822
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HarmonyLib;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Server;
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

    static private string _saveDirectory = "";
    private static readonly Dictionary<string, double> _playerLoadedMetabolism = [];
    private static readonly Dictionary<string, float> _playerLoadedMetabolismReceiveMultiply = [];
    public static readonly IReadOnlyDictionary<string, float> PlayerLoadedMetabolismReceiveMultiply
        = new ReadOnlyDictionary<string, float>(_playerLoadedMetabolismReceiveMultiply);

    public void Init()
    {
        // Instanciate Events
        Instance.api.Event.PlayerNowPlaying += PlayerJoin;
        Instance.api.Event.PlayerDisconnect += PlayerDisconnect;
        Instance.api.Event.GameWorldSave += SaveState;
        Instance.api.Event.RegisterGameTickListener(OnGameTick, 1000, 10000);
        OverwriteDamageInteractionEvents.OnPlayerReceiveDamageUnkown += HandleUnkownDamage;
        Configuration.RegisterNewLevel("Metabolism");
        Configuration.RegisterNewLevelTypeEXP("Metabolism", Configuration.MetabolismGetLevelByEXP);
        Configuration.RegisterNewEXPLevelType("Metabolism", Configuration.MetabolismGetExpByLevel);

        Debug.Log("Level Metabolism initialized");
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
        Task.Run(() =>
        {
            foreach (KeyValuePair<string, double> keyValuePair in _playerLoadedMetabolism)
            {
                IPlayer player = Instance.api.World.AllOnlinePlayers.First((player) => player.PlayerUID == keyValuePair.Key);
                // Should never be null, but just in case...
                if (player == null) { Debug.LogError($"[METABOLISM] [OnGameTick] Cannot find player when refreshing saturation, caused by {keyValuePair.Key}"); continue; }

                // Get player stats
                EntityBehaviorHunger playerStats = player.Entity.GetBehavior<EntityBehaviorHunger>();
                // Check if stats is null
                if (playerStats == null) { Debug.LogError($"[METABOLISM] [OnGameTick] ERROR GETTING SATURATION: Player Stats is null, caused by {player.PlayerName}"); continue; }

                if (playerStats.Saturation < keyValuePair.Value)
                {
                    _playerLoadedMetabolism[keyValuePair.Key] = playerStats.Saturation;
                    Experience.IncreaseExperience(player, "Metabolism", (ulong)Configuration.EXPPerSaturationLostMetabolism);
                }
            }
        });
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

    /// <summary>
    /// Loads the player to the memory
    /// </summary>
    /// <param name="player"></param>
    private static void LoadPlayer(IPlayer player)
    {
        if (!Utils.ValidatePlayerUID(player))
        {
            if (player is IServerPlayer serverPlayer)
                serverPlayer.Disconnect("[LEVELUP SECURITY] Invalid UID");
            return;
        }

        string playerDirectory = Path.Combine(_saveDirectory, player.PlayerUID);

        // Conversion to the safe UID
        {
            string correctDirectory = Path.Combine(_saveDirectory, Utils.ConvertPlayerUID(player.PlayerUID));
            // Wrong directory exists, lets move it
            if (Directory.Exists(playerDirectory))
            {
                if (playerDirectory != correctDirectory)
                {
                    Debug.LogWarn($"{player.PlayerUID} is saved on unsafe directory, levelup will move from: {playerDirectory} to {correctDirectory}");
                    Directory.Move(playerDirectory, correctDirectory);
                }
            }
        }

        Debug.LogDebug($"Loading {player.PlayerName} metabolism: {Utils.ConvertPlayerUID(player.PlayerUID)}");

        playerDirectory = Path.Combine(_saveDirectory, Utils.ConvertPlayerUID(player.PlayerUID));

        Directory.CreateDirectory(playerDirectory);

        string lastSaturationFile = Path.Combine(playerDirectory, "lastSaturation.txt");
        if (File.Exists(lastSaturationFile))
        {
            string lastSaturationString = File.ReadAllText(lastSaturationFile);

            if (double.TryParse(lastSaturationString, out double lastSaturation))
                _playerLoadedMetabolism[player.PlayerUID] = lastSaturation;
            else
            {
                _playerLoadedMetabolism[player.PlayerUID] = Configuration.BaseSaturationMetabolism;
                Debug.LogError($"[METABOLISM] {player.PlayerName} has any invalid metabolism saturation: {lastSaturationString}");
            }
        }
        else
        {
            _playerLoadedMetabolism[player.PlayerUID] = Configuration.BaseSaturationMetabolism;
            Debug.LogDebug($"Cannot find the player: {player.PlayerName} previous saturation, probably is the first login, setting Saturation to: {Configuration.BaseSaturationMetabolism}");
        }
    }

    /// <summary>
    /// Saves the player and unload it from the memory
    /// </summary>
    /// <param name="player"></param>
    private static void UnloadPlayer(IPlayer player)
    {
        if (!_playerLoadedMetabolism.ContainsKey(player.PlayerUID)) return;

        SavePlayer(player);

        _playerLoadedMetabolism.Remove(player.PlayerUID);
        _playerLoadedMetabolismReceiveMultiply.Remove(player.PlayerUID);
    }

    /// <summary>
    /// Manually save the player experience and levels
    /// </summary>
    /// <param name="player"></param>
    private static void SavePlayer(IPlayer player)
    {
        if (!Utils.ValidatePlayerUID(player)) return;

        string playerDirectory = Path.Combine(_saveDirectory, Utils.ConvertPlayerUID(player.PlayerUID));
        Directory.CreateDirectory(playerDirectory);

        string lastSaturationFile = Path.Combine(playerDirectory, "lastSaturation.txt");

        if (_playerLoadedMetabolism.TryGetValue(player.PlayerUID, out double lastSaturation))
        {
            File.WriteAllText(lastSaturationFile, lastSaturation.ToString());
            Debug.LogDebug($"[METABOLISM] {player.PlayerName} saved to the path: {playerDirectory}");
        }
        else
            Debug.LogError($"[METABOLISM] {player.PlayerName} cannot be found in playerLoadedMetabolism");
    }

    private void SaveState()
    {
        foreach (IPlayer player in Instance.api.World.AllOnlinePlayers)
        {
            // Saving each online player
            foreach (KeyValuePair<string, double> keyValue in _playerLoadedMetabolism)
            {
                // Iteration is not the player
                if (keyValue.Key != player.PlayerUID) continue;

                // Check infinity bug
                if (double.IsInfinity(keyValue.Value))
                {
                    Debug.LogError($"ERROR: {keyValue.Key} saturationState is infinity??, reseting to {Configuration.BaseSaturationMetabolism} before saving");
                    _playerLoadedMetabolism[keyValue.Key] = Configuration.BaseSaturationMetabolism;
                }
                else
                {
                    SavePlayer(player);
                }
            }
        }
    }

    private void PlayerJoin(IServerPlayer player)
    {
        LoadPlayer(player);

        EntityBehaviorHunger playerStats = RefreshMaxSaturation(player);
        RefreshSaturationReceiveMultiply(player);

        if (playerStats != null)
        {
            // Reload player saturation
            if (_playerLoadedMetabolism.TryGetValue(player.PlayerUID, out double value)) playerStats.Saturation = (float)value;
            // If cannot find player will receive the base max saturation instead
            else
            {
                playerStats.Saturation = playerStats.MaxSaturation;
                Debug.LogError($"[METABOLISM] Cannot find the player: {player.PlayerName} saturation, something goes wrong");
            }
        }

    }

    private void PlayerDisconnect(IServerPlayer player)
    {
        try
        {
            // Disconnected during the loading
            if (player.Entity == null) { Debug.LogDebug($"[METABOLISM] {player.PlayerName} entity is null"); return; }

            // Get stats
            EntityBehaviorHunger playerStats = player.Entity.GetBehavior<EntityBehaviorHunger>();
            if (playerStats == null) { Debug.LogError($"[METABOLISM] ERROR SAVING PLAYER STATE: Player Stats is null, caused by {player.PlayerName}"); return; }

            // Check error treatment
            if (float.IsInfinity(playerStats.Saturation))
            {
                Debug.LogError($"[METABOLISM] ERROR SAVING PLAYER STATE: Player Saturation is infinity, caused by {player.PlayerName} setting the saturation to {Configuration.BaseSaturationMetabolism}");
                _playerLoadedMetabolism[player.PlayerUID] = Configuration.BaseSaturationMetabolism;
            }
            // Update it
            else _playerLoadedMetabolism[player.PlayerUID] = playerStats.Saturation;
        }
        catch (Exception) { }
        finally
        {
            UnloadPlayer(player);
        }
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
        // Overwrite Saturation consume
        [HarmonyPrefix]
        [HarmonyPatch(typeof(EntityBehaviorHunger), "ConsumeSaturation")]
        internal static void ConsumeSaturation(EntityBehaviorHunger __instance, ref float amount)
        {
            if (!Configuration.enableLevelMetabolism) return;

            if (__instance.entity is EntityPlayer entityPlayer)
            {
                float reducer = PlayerLoadedMetabolismReceiveMultiply.TryGetValue(entityPlayer.PlayerUID, out float result)
                    ? result
                    : 1f;

                amount *= reducer;
            }
        }

        // Clientside view
        // I don't know the reason, but some random function is changing the maxsaturation to default value randomly
        [HarmonyPrefix]
        [HarmonyPatch(typeof(HudStatbar), "UpdateSaturation")]
        internal static void UpdateSaturation(HudStatbar __instance)
        {
            var capiField = AccessTools.Field(typeof(HudStatbar), "capi");
            ICoreClientAPI capi = capiField.GetValue(__instance) as ICoreClientAPI;

            ITreeAttribute hungerTree = capi.World.Player.Entity.WatchedAttributes.GetTreeAttribute("hunger");
            hungerTree.SetFloat("maxsaturation", capi.World.Player.Entity.WatchedAttributes.GetFloat("maxsaturation"));
        }
    }
}