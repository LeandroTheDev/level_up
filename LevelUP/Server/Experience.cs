using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace LevelUP.Server;

class Experience
{
    static private string _saveDirectory = "";

    /// <summary>
    /// {
    ///     "playerUID": {
    ///         "levelName": {
    ///             "experience": 10000    
    ///         }
    ///     }
    /// }
    /// </summary>
    private static readonly Dictionary<string, Dictionary<string, Dictionary<string, ulong>>> _playerLoadedExperience = [];
    /// <summary>
    /// {
    ///     "levelName" {
    ///         "reason1": 10,
    ///         "reason2": 20
    ///     }
    /// }
    /// </summary>
    private static readonly Dictionary<string, Dictionary<string, ulong>> _serverLoadedExperience = [];

    static public void LoadInstance(ICoreAPI api)
    {
        _saveDirectory = Path.Combine(api.DataBasePath, $"ModData/LevelUP/{api.World.SavegameIdentifier}");
        Debug.Log($"LevelUP will save experience data in: {_saveDirectory}");
        Directory.CreateDirectory(_saveDirectory);
    }

    /// <summary>
    /// Loads the player to the memory
    /// </summary>
    /// <param name="player"></param>
    static public void LoadPlayer(IPlayer player)
    {
        string playerDirectory = Path.Combine(_saveDirectory, player.PlayerUID);
        Directory.CreateDirectory(playerDirectory);

        if (_playerLoadedExperience.ContainsKey(player.PlayerUID)) _playerLoadedExperience.Remove(player.PlayerUID);
        _playerLoadedExperience.Add(player.PlayerUID, []);

        string[] levelFiles = Directory.GetFiles(playerDirectory);
        foreach (string levelFile in levelFiles)
        {
            string levelName = Path.GetFileNameWithoutExtension(levelFile);

            string levelJsonString = File.ReadAllText(levelFile);
            Dictionary<string, ulong> levelJson = JsonSerializer.Deserialize<Dictionary<string, ulong>>(levelJsonString);

            _playerLoadedExperience[player.PlayerUID][levelName] = levelJson;
        }
    }

    /// <summary>
    /// Saves the player and unload it from the memory
    /// </summary>
    /// <param name="player"></param>
    static public void UnloadPlayer(IPlayer player)
    {
        if (!_playerLoadedExperience.ContainsKey(player.PlayerUID)) return;

        SavePlayer(player);

        _playerLoadedExperience.Remove(player.PlayerUID);
    }

    /// <summary>
    /// Manually save the player experience and levels
    /// </summary>
    /// <param name="player"></param>
    static private void SavePlayer(IPlayer player)
    {
        string playerDirectory = Path.Combine(_saveDirectory, player.PlayerUID);
        Directory.CreateDirectory(playerDirectory);

        foreach (string levelName in _playerLoadedExperience[player.PlayerUID].Keys)
        {
            string levelFile = Path.Combine(playerDirectory, levelName + ".json");
            string levelJsonString = JsonSerializer.Serialize(_playerLoadedExperience[player.PlayerUID][levelName]);

            File.WriteAllText(levelFile, levelJsonString);
        }

        Debug.LogDebug($"{player.PlayerName} saved");
    }

    /// <summary>
    /// Loads the experience for the reasons
    /// </summary>
    /// <param name="type">Level type</param>
    /// <param name="reason">The reason to increase experience</param>
    /// <param name="amount">The experience amount to be received from the reason</param>
    static public void LoadExperience(string type, string reason, ulong amount)
    {
        if (_serverLoadedExperience.TryGetValue(type, out Dictionary<string, ulong> reasons))
            if (reasons.TryGetValue(reason, out ulong _))
                _serverLoadedExperience[type][reason] = amount;
            else
                _serverLoadedExperience[type].Add(reason, amount);
        else _serverLoadedExperience.Add(type, new Dictionary<string, ulong>
        {
            {reason, amount}
        });
    }

    /// <summary>
    ///  Increase the experience with a reason
    /// </summary>
    /// <param name="player">Player instance</param>
    /// <param name="type">Level type</param>
    /// <param name="reason">The reason to increase experience</param>
    static public void IncreaseExperience(IPlayer player, string type, string reason)
    {
        if (!_serverLoadedExperience.TryGetValue(type, out _))
        {
            Debug.LogError($"ERROR: the experience type: '{type}' does not exist in loaded experience, please report this error");
            return;
        }
        if (!_serverLoadedExperience[type].TryGetValue(reason, out _))
        {
            Debug.LogError($"ERROR: the experience reason '{type}' does not exist in loaded experience, please report this error");
            return;
        }

        ulong experienceEarned = _serverLoadedExperience[type][reason];
        string playerClass = player.Entity.WatchedAttributes.GetString("characterClass");
        float classMultiply = Configuration.GetEXPMultiplyByClassAndLevelType(playerClass, type);

        IncrementExperience(player, type, (ulong)Math.Round(experienceEarned * classMultiply));
    }

    /// <summary>
    /// Manually increase the experience from a level type with any desired amount
    /// </summary>
    /// <param name="player">Player instance</param>
    /// <param name="type">Level type</param>
    /// <param name="amount">Experience quantity to be increased</param>
    static public void IncreaseExperience(IPlayer player, string type, ulong amount)
    {
        ulong experienceEarned = amount;
        string playerClass = player.Entity.WatchedAttributes.GetString("characterClass");
        float classMultiply = Configuration.GetEXPMultiplyByClassAndLevelType(playerClass, type);

        IncrementExperience(player, type, (ulong)Math.Round(experienceEarned * classMultiply));
    }

    /// <summary>
    /// Manually increase a sub experience from a level type with any desired amount
    /// </summary>
    /// <param name="player">Player instance</param>
    /// <param name="type">Level type</param>
    /// <param name="subType">Sub level type</param>
    /// <param name="amount">Experience quantity to be increased</param>
    static public void IncreaseSubExperience(IPlayer player, string type, string subType, ulong amount)
    {
        ulong experienceEarned = amount;
        string playerClass = player.Entity.WatchedAttributes.GetString("characterClass");
        float classMultiply = Configuration.GetEXPMultiplyByClassAndLevelType(playerClass, type);

        IncrementSubExperience(player, type, subType, (ulong)Math.Round(experienceEarned * classMultiply));
    }

    /// <summary>
    /// Will safely add more experience to the desired player
    /// </summary>
    /// <param name="player">Player instance</param>
    /// <param name="type">The level type</param>
    /// <param name="amount">Amount of experience earned</param>
    static private void IncrementExperience(IPlayer player, string type, ulong amount)
    {
        if (!_playerLoadedExperience.TryGetValue(player.PlayerUID, out _))
            _playerLoadedExperience.Add(player.PlayerUID, []);

        if (!_playerLoadedExperience[player.PlayerUID].TryGetValue(type, out _))
            _playerLoadedExperience[player.PlayerUID].Add(type, []);

        if (!_playerLoadedExperience[player.PlayerUID][type].TryGetValue("experience", out _))
            _playerLoadedExperience[player.PlayerUID][type].Add("experience", 0);

        _playerLoadedExperience[player.PlayerUID][type]["experience"] += amount;

        Shared.Instance.UpdateLevelAndNotify(null, player, type, _playerLoadedExperience[player.PlayerUID][type]["experience"]);

        if (Configuration.enableLevelUpExperienceServerLog)
            Debug.Log($"[EXPERIENCE] {player.PlayerName}: {amount}, {type}");
    }

    /// <summary>
    /// Will safely add more sub experience to the desired player
    /// </summary>
    /// <param name="player">Player instance</param>
    /// <param name="type">The level type</param>
    /// <param name="subType">The sub level type</param>
    /// <param name="amount">Amount of experience earned</param>
    static private void IncrementSubExperience(IPlayer player, string type, string subType, ulong amount)
    {
        if (!_playerLoadedExperience.TryGetValue(player.PlayerUID, out _))
            _playerLoadedExperience.Add(player.PlayerUID, []);

        if (!_playerLoadedExperience[player.PlayerUID].TryGetValue(type, out _))
            _playerLoadedExperience[player.PlayerUID].Add(type, []);

        if (!_playerLoadedExperience[player.PlayerUID][type].TryGetValue(subType, out _))
            _playerLoadedExperience[player.PlayerUID][type].Add(subType, 0);

        _playerLoadedExperience[player.PlayerUID][type][subType] += amount;

        Shared.Instance.UpdateLevelAndNotify(null, player, type, _playerLoadedExperience[player.PlayerUID][type]["experience"]);

        if (Configuration.enableLevelUpExperienceServerLog)
            Debug.Log($"[EXPERIENCE] {player.PlayerName}: {amount}, {type}/{subType}");
    }

    /// <summary>
    /// Manually reduce player experience by the type, but will not reduce if the level is below the currently player level    
    /// </summary>
    /// <param name="player">Player instance</param>
    /// <param name="type">Level type</param>
    /// <param name="amount">Amount to be reduced</param>
    /// <returns>false if not reduced, true if is reduced</returns>
    static public bool ReduceExperience(IPlayer player, string type, ulong amount)
    {
        ulong currentlyExp = GetExperience(player, type);
        int currentlyLevel = Configuration.GetLevelByLevelTypeEXP(type, currentlyExp);
        int nextLevel = Configuration.GetLevelByLevelTypeEXP(type, currentlyExp - amount);

        if (nextLevel < currentlyLevel)
            return false;

        DecrementExperience(player, type, amount);
        return true;
    }

    /// <summary>
    /// Will safely remove experience to the desired player
    /// </summary>
    /// <param name="player">Player instance</param>
    /// <param name="type">The level type</param>
    /// <param name="amount">Amount of experience earned</param>
    static private void DecrementExperience(IPlayer player, string type, ulong amount)
    {
        if (!_playerLoadedExperience.TryGetValue(player.PlayerUID, out _))
            _playerLoadedExperience.Add(player.PlayerUID, []);

        if (!_playerLoadedExperience[player.PlayerUID].TryGetValue(type, out _))
            _playerLoadedExperience[player.PlayerUID].Add(type, []);

        if (_playerLoadedExperience[player.PlayerUID][type].TryGetValue("experience", out _))
            _playerLoadedExperience[player.PlayerUID][type].Add("experience", 0);

        _playerLoadedExperience[player.PlayerUID][type]["experience"] -= amount;

        Shared.Instance.UpdateLevelAndNotify(null, player, type, amount, true);
    }


    /// <summary>
    /// Returns the player experience
    /// </summary>
    /// <param name="player">Player instance</param>
    /// <param name="type">Level type</param>
    /// <returns></returns>
    static public ulong GetExperience(IPlayer player, string type)
    {
        if (_playerLoadedExperience.TryGetValue(player.PlayerUID, out Dictionary<string, Dictionary<string, ulong>> levels))
            if (levels.TryGetValue(type, out Dictionary<string, ulong> data))
                if (data.TryGetValue("experience", out ulong experience)) return experience;
        return 0;
    }

    /// <summary>
    /// Returns the player sub level type experience
    /// </summary>
    /// <param name="player">Player instance</param>
    /// <param name="type">Level type</param>
    /// <param name="subType">Sub level type</param>
    /// <returns></returns>
    static public ulong GetSubExperience(IPlayer player, string type, string subType)
    {
        if (_playerLoadedExperience.TryGetValue(player.PlayerUID, out Dictionary<string, Dictionary<string, ulong>> levels))
            if (levels.TryGetValue(type, out Dictionary<string, ulong> data))
                if (data.TryGetValue(subType, out ulong experience)) return experience;
        return 0;
    }


    static public void SaveExperience(ICoreServerAPI api)
    {
        foreach (IPlayer player in api.World.AllOnlinePlayers)
            SavePlayer(player);
    }
}