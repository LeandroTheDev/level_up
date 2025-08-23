using System.Collections.Generic;
using System.IO;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace LevelUP.Server;

class LevelVitality
{
    static private string _saveDirectory = "";
    private Dictionary<string, double> _playerLoadedVitality = [];

    public void Init()
    {
        // Instanciate Events
        Instance.api.Event.PlayerNowPlaying += PlayerJoin;
        Instance.api.Event.PlayerDisconnect += PlayerDisconnect;
        Instance.api.Event.GameWorldSave += SaveState;

        Debug.Log("Level Vitality initialized");
    }

#pragma warning disable CA1822
    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Load player state
        _saveDirectory = Path.Combine(coreAPI.DataBasePath, $"ModData/LevelUP/{coreAPI.World.SavegameIdentifier}-Vitality");
        Debug.Log($"LevelUP will save vitality data in: {_saveDirectory}");
        Directory.CreateDirectory(_saveDirectory);

        // Populate configuration
        Configuration.PopulateVitalityConfiguration(coreAPI);
        Configuration.RegisterNewLevelTypeEXP("Vitality", Configuration.VitalityGetLevelByEXP);
        Configuration.RegisterNewMaxLevelByLevelTypeEXP("Vitality", Configuration.vitalityMaxLevel);
    }
#pragma warning restore CA1822

    /// <summary>
    /// Loads the player to the memory
    /// </summary>
    /// <param name="player"></param>
    private void LoadPlayer(IPlayer player)
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
                Debug.LogWarn($"{player.PlayerUID} is saved on unsafe directory, levelup will move from: {playerDirectory} to {correctDirectory}");
                Directory.Move(playerDirectory, correctDirectory);
            }
        }

        Debug.LogDebug($"Loading {player.PlayerName} vitality: {Utils.ConvertPlayerUID(player.PlayerUID)}");

        playerDirectory = Path.Combine(_saveDirectory, Utils.ConvertPlayerUID(player.PlayerUID));

        Directory.CreateDirectory(playerDirectory);

        string lastHealthFile = Path.Combine(playerDirectory, "lastHealth.txt");
        if (File.Exists(lastHealthFile))
        {
            string lastHealthString = File.ReadAllText(lastHealthFile);

            if (double.TryParse(lastHealthString, out double lastHealth))
                _playerLoadedVitality[player.PlayerUID] = lastHealth;
            else
            {
                _playerLoadedVitality[player.PlayerUID] = Configuration.BaseHPVitality;
                Debug.LogError($"[VITALITY] {player.PlayerName} has any invalid vitality health: {lastHealthString}");
            }
        }
        else
        {
            _playerLoadedVitality[player.PlayerUID] = Configuration.BaseHPVitality;
            Debug.LogDebug($"Cannot find the player: {player.PlayerName} previous health, probably is the first login, setting HP to: {Configuration.BaseHPVitality}");
        }
    }

    /// <summary>
    /// Saves the player and unload it from the memory
    /// </summary>
    /// <param name="player"></param>
    private void UnloadPlayer(IPlayer player)
    {
        if (!_playerLoadedVitality.ContainsKey(player.PlayerUID)) return;

        SavePlayer(player);

        _playerLoadedVitality.Remove(player.PlayerUID);
    }

    /// <summary>
    /// Manually save the player experience and levels
    /// </summary>
    /// <param name="player"></param>
    private void SavePlayer(IPlayer player)
    {
        if (!Utils.ValidatePlayerUID(player)) return;

        string playerDirectory = Path.Combine(_saveDirectory, Utils.ConvertPlayerUID(player.PlayerUID));
        Directory.CreateDirectory(playerDirectory);

        string lastHealthFile = Path.Combine(playerDirectory, "lastHealth.txt");

        if (_playerLoadedVitality.TryGetValue(player.PlayerUID, out double lastHealth))
        {
            File.WriteAllText(lastHealthFile, lastHealth.ToString());
            Debug.LogDebug($"[VITALITY] {player.PlayerName} saved to the path: {playerDirectory}");
        }
        else
            Debug.LogError($"[VITALITY] {player.PlayerName} cannot be found in playerLoadedVitality");
    }

    private void SaveState()
    {
        foreach (IPlayer player in Instance.api.World.AllOnlinePlayers)
        {
            // Saving each online player
            foreach (KeyValuePair<string, double> keyValue in _playerLoadedVitality)
            {
                // Iteration is not the player
                if (keyValue.Key != player.PlayerUID) continue;

                // Check infinity bug
                if (double.IsInfinity(keyValue.Value))
                {
                    Debug.LogError($"ERROR: {keyValue.Key} vitalityState is infinity??, reseting to {Configuration.BaseHPVitality} before saving");
                    _playerLoadedVitality[keyValue.Key] = Configuration.BaseHPVitality;
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

        // Get the actual player total exp
        ulong playerExp = Experience.GetExperience(player, "Vitality");

        // Get player stats
        EntityBehaviorHealth playerStats = player.Entity.GetBehavior<EntityBehaviorHealth>();
        // Check if stats is null
        if (playerStats == null) { Debug.LogError($"[VITALITY] ERROR SETTING MAX HEALTH: Player Stats is null, caused by {player.PlayerName}"); return; }

        // Getting health stats
        float playerMaxHealth = Configuration.VitalityGetMaxHealthByLevel(Configuration.VitalityGetLevelByEXP(playerExp));
        if (float.IsInfinity(playerMaxHealth))
        {
            Debug.LogError($"[VITALITY] ERROR: Max health calculation returned any infinity number, please report this issue, base health set to {Configuration.BaseHPVitality}");
            playerMaxHealth = Configuration.BaseHPVitality;
        }

        // Getting regen stats
        float playerRegen = Configuration.VitalityGetHealthRegenMultiplyByLevel(Configuration.VitalityGetLevelByEXP(playerExp));
        if (float.IsInfinity(playerRegen))
        {
            Debug.LogError($"[VITALITY] ERROR: Regeneration calculation returned any infinity number, please report this issue, base regen set to {Configuration.BaseHPRegenVitality}");
            playerRegen = Configuration.BaseHPRegenVitality;
        }

        playerStats.BaseMaxHealth = playerMaxHealth;
        playerStats.MaxHealth = playerMaxHealth;
        player.Entity.WatchedAttributes.SetFloat("regenSpeed", playerRegen);

        // Refresh for the player
        playerStats.UpdateMaxHealth();

        // Reload player health
        if (_playerLoadedVitality.TryGetValue(player.PlayerUID, out double value)) playerStats.Health = (float)value;
        // If cannot find player will receive the base max health instead
        else
        {
            playerStats.Health = playerMaxHealth;
            Debug.LogError($"[VITALITY] Cannot find the player: {player.PlayerName} health, something goes wrong");
        }

        Debug.LogDebug($"[VITALITY] {player.PlayerName} joined the world with max: {playerStats.MaxHealth} health and {playerStats.Health} actual health");
        Debug.LogDebug($"[VITALITY] Calculation Variables: {playerMaxHealth}:{playerRegen}, Level: {Configuration.VitalityGetLevelByEXP(playerExp)}");
    }

    private void PlayerDisconnect(IServerPlayer player)
    {
        // Disconnected during the loading
        if (player.Entity == null) { Debug.LogDebug($"[VITALITY] {player.PlayerName} entity is null"); return; }

        // Get stats
        EntityBehaviorHealth playerStats = player.Entity.GetBehavior<EntityBehaviorHealth>();
        if (playerStats == null) { Debug.LogError($"[VITALITY] ERROR SAVING PLAYER STATE: Player Stats is null, caused by {player.PlayerName}"); return; }

        // Check error treatment
        if (float.IsInfinity(playerStats.Health))
        {
            Debug.LogError($"[VITALITY] ERROR SAVING PLAYER STATE: Player Health is infinity, caused by {player.PlayerName} setting the health to {Configuration.BaseHPVitality}");
            _playerLoadedVitality[player.PlayerUID] = Configuration.BaseHPVitality;
        }
        // Update it
        else _playerLoadedVitality[player.PlayerUID] = playerStats.Health;

        UnloadPlayer(player);
    }
}