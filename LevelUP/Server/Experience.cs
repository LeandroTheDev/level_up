using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using Vintagestory.API.Util;

namespace LevelUP.Server;

public class Experience
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

    /// <summary>
    /// Creates necessary directory and save the path to save experience
    /// </summary>
    /// <param name="api"></param>
    static internal void LoadInstance(ICoreAPI api)
    {
        _saveDirectory = Path.Combine(api.DataBasePath, $"ModData/LevelUP/{api.World.SavegameIdentifier}");
        Debug.Log($"LevelUP will save experience data in: {_saveDirectory}");
        Directory.CreateDirectory(_saveDirectory);
    }

    /// <summary>
    /// Loads the player to the memory
    /// </summary>
    /// <param name="player"></param>
    static internal void LoadPlayer(IPlayer player)
    {
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

        Debug.LogDebug($"Loading {player.PlayerName} experience: {Utils.ConvertPlayerUID(player.PlayerUID)}");

        playerDirectory = Path.Combine(_saveDirectory, Utils.ConvertPlayerUID(player.PlayerUID));

        Directory.CreateDirectory(playerDirectory);

        if (_playerLoadedExperience.ContainsKey(player.PlayerUID)) _playerLoadedExperience.Remove(player.PlayerUID);
        _playerLoadedExperience.Add(player.PlayerUID, []);

        string[] levelFiles = Directory.GetFiles(playerDirectory);
        foreach (string levelFile in levelFiles)
        {
            string levelName = Path.GetFileNameWithoutExtension(levelFile);

            string levelJsonString = File.ReadAllText(levelFile);
            try
            {
                Dictionary<string, ulong> levelJson = JsonSerializer.Deserialize<Dictionary<string, ulong>>(levelJsonString);
                _playerLoadedExperience[player.PlayerUID][levelName] = levelJson;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Cannot deserialize player: {player.PlayerUID} experience: {levelName}, exception: {ex.Message} full text json:");
                Debug.LogError("----------------------------------");
                Debug.LogError(levelJsonString);
                Debug.LogError("----------------------------------");
                _playerLoadedExperience[player.PlayerUID][levelName] = [];
            }
        }

        SavePlayer(player);
    }

    /// <summary>
    /// Saves the player and unload it from the memory
    /// </summary>
    /// <param name="player"></param>
    static internal void UnloadPlayer(IPlayer player)
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
        string playerDirectory = Path.Combine(_saveDirectory, Utils.ConvertPlayerUID(player.PlayerUID));
        Directory.CreateDirectory(playerDirectory);

        foreach (string levelName in _playerLoadedExperience[player.PlayerUID].Keys)
        {
            string levelFile = Path.Combine(playerDirectory, levelName + ".json");
            string levelJsonString = JsonSerializer.Serialize(_playerLoadedExperience[player.PlayerUID][levelName]);

            File.WriteAllText(levelFile, levelJsonString);
        }

        Debug.LogDebug($"{player.PlayerName} saved to the path: {playerDirectory}");
    }

    /// <summary>
    /// Loads the experience for the reasons
    /// </summary>
    /// <param name="type">Level type</param>
    /// <param name="reason">The reason to increase experience</param>
    /// <param name="amount">The experience amount to be received from the reason</param>
    static internal void LoadExperience(string type, string reason, ulong amount)
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
    /// Calculation of the remaining experience for the next level
    /// </summary>
    /// <param name="player"></param>
    /// <param name="type">Level type</param>
    static internal void CalculateRemainingExperience(IPlayer player, string type)
    {
        ulong currentExp = GetExperience(player, type);
        int playerLevel = Configuration.GetLevelByLevelTypeEXP(type, currentExp);
        ulong minimumExp = Configuration.GetEXPByLevelTypeLevel(playerLevel, type);
        ulong maximumExp = Configuration.GetEXPByLevelTypeLevel(playerLevel + 1, type);

        ulong range = maximumExp - minimumExp;
        float progress = range > 0
            ? (float)(currentExp - minimumExp) / range
            : 1f;
        float percent = progress * 100f;

        player.Entity.WatchedAttributes.SetFloat($"LevelUP_Level_{type}_RemainingNextLevelPercentage", percent);
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

        CalculateRemainingExperience(player, type);
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

        CalculateRemainingExperience(player, type);
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
        try
        {
            if (!_playerLoadedExperience.TryGetValue(player.PlayerUID, out _))
                _playerLoadedExperience.Add(player.PlayerUID, []);

            if (!_playerLoadedExperience[player.PlayerUID].TryGetValue(type, out _))
                _playerLoadedExperience[player.PlayerUID].Add(type, []);

            if (!_playerLoadedExperience[player.PlayerUID][type].TryGetValue("experience", out _))
                _playerLoadedExperience[player.PlayerUID][type].Add("experience", 0);

            amount = ExperienceEvents.GetExternalAmountIncrease(player, type, amount);

            if (Configuration.CheckMaxLevelByLevelTypeEXP(type, _playerLoadedExperience[player.PlayerUID][type]["experience"] + amount))
                return;

            _playerLoadedExperience[player.PlayerUID][type]["experience"] += amount;

            Shared.Instance.UpdateLevelAndNotify(null, player, type, _playerLoadedExperience[player.PlayerUID][type]["experience"]);

            if (Configuration.enableLevelUpExperienceServerLog)
                Debug.Log($"[EXPERIENCE] {player.PlayerName}: {amount}, {type}");
        }
        catch (Exception nex)
        {
            Debug.LogError(nex.Message);
        }
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

        amount = ExperienceEvents.GetExternalAmountSubIncrease(player, type, subType, amount);

        if (Configuration.CheckMaxLevelByLevelTypeEXP(type, _playerLoadedExperience[player.PlayerUID][type][subType] + amount))
            return;

        _playerLoadedExperience[player.PlayerUID][type][subType] += amount;

        Shared.Instance.UpdateSubLevelAndNotify(null, player, type, subType, _playerLoadedExperience[player.PlayerUID][type][subType]);

        if (Configuration.enableLevelUpExperienceServerLog)
            Debug.Log($"[EXPERIENCE] {player.PlayerName}: {amount}, {type}/{subType}");
    }

    /// <summary>
    /// Force change the experience of a specific level type from a player
    /// </summary>
    /// <param name="player">Player instance</param>
    /// <param name="type">The level type</param>
    /// <param name="amount">Amount of experience earned</param>
    static internal void ChangeExperience(IPlayer player, string type, ulong amount)
    {
        if (!_playerLoadedExperience.TryGetValue(player.PlayerUID, out _))
            _playerLoadedExperience.Add(player.PlayerUID, []);

        if (!_playerLoadedExperience[player.PlayerUID].TryGetValue(type, out _))
            _playerLoadedExperience[player.PlayerUID].Add(type, []);

        if (!_playerLoadedExperience[player.PlayerUID][type].TryGetValue("experience", out _))
            _playerLoadedExperience[player.PlayerUID][type].Add("experience", 0);

        _playerLoadedExperience[player.PlayerUID][type]["experience"] = amount;

        Shared.Instance.UpdateLevelAndNotify(null, player, type, _playerLoadedExperience[player.PlayerUID][type]["experience"]);

        Debug.Log($"[EXPERIENCE] All experience of {player.PlayerName} has changed to: {amount}, {type}");
    }

    /// <summary>
    /// Manually reduce player experience by the type, but will not reduce if the level is below the currently player level    
    /// unless ``ignoreLevel`` is set to true
    /// </summary>
    /// <param name="player">Player instance</param>
    /// <param name="type">Level type</param>
    /// <param name="amount">Amount to be reduced</param>
    /// <param name="ignoreLevel">If the amount to reduce will decrease the player level the function will be cancelled, unless ``ignoreLevel`` is true</param>
    /// <returns>false if not reduced, true if is reduced</returns>
    static internal bool ReduceExperience(IPlayer player, string type, ulong amount, bool ignoreLevel = false)
    {

        ulong currentlyExp = GetExperience(player, type);
        if (ignoreLevel)
        {
            amount = ExperienceEvents.GetExternalAmountReduce(player, type, amount);

            if (currentlyExp - amount < 0)
            {
                DecrementExperience(player, type, currentlyExp);
                return true;
            }
            else
            {
                DecrementExperience(player, type, amount);
                return true;
            }
        }

        amount = ExperienceEvents.GetExternalAmountReduce(player, type, amount);
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

        if (!_playerLoadedExperience[player.PlayerUID][type].TryGetValue("experience", out _))
            _playerLoadedExperience[player.PlayerUID][type].Add("experience", 0);
        else
            _playerLoadedExperience[player.PlayerUID][type]["experience"] -= amount;

        if (_playerLoadedExperience[player.PlayerUID][type]["experience"] < 0)
            _playerLoadedExperience[player.PlayerUID][type]["experience"] = 0;

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

    /// <summary>
    /// Save all players epxerience
    /// </summary>
    /// <param name="api"></param>
    static internal void SaveExperience(ICoreServerAPI api)
    {
        foreach (IPlayer player in api.World.AllOnlinePlayers)
            SavePlayer(player);
    }
}

#region Compatibility
public static class ExperienceEvents
{
    public delegate void ExperienceModifierHandler(IPlayer player, string type, ref ulong amount);
    public delegate void ExperienceLevelUPHandler(IPlayer player, string type, ulong exp, int level);
    public delegate void SubExperienceModifierHandler(IPlayer player, string type, string subType, ref ulong amount);

    public static event ExperienceModifierHandler OnExperienceIncrease;
    public static event SubExperienceModifierHandler OnSubExperienceIncrease;
    public static event ExperienceModifierHandler OnExperienceReduced;
    public static event ExperienceLevelUPHandler OnLevelUp;

    public static ulong GetExternalAmountIncrease(IPlayer player, string type, ulong amount)
    {
        OnExperienceIncrease?.Invoke(player, type, ref amount);
        return amount;
    }

    public static ulong GetExternalAmountSubIncrease(IPlayer player, string type, string subType, ulong amount)
    {
        OnSubExperienceIncrease?.Invoke(player, type, subType, ref amount);
        return amount;
    }

    public static ulong GetExternalAmountReduce(IPlayer player, string type, ulong amount)
    {
        OnExperienceReduced?.Invoke(player, type, ref amount);
        return amount;
    }

    public static void PlayerLeveledUp(IPlayer player, string type, ulong exp, int level)
        => OnLevelUp?.Invoke(player, type, exp, level);
}
#endregion