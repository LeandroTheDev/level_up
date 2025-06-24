using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
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
    /// Temporary function to convert 1.0 exp system to 2.0
    /// </summary>
    /// <param name="player">Player instance</param>
    static internal void ConvertOldExperienceSystemToNew(IPlayer player)
    {
        if (player.Entity.Api.World.Side != EnumAppSide.Server || Instance.api == null)
        {
            Debug.Log("[EXPERIENCE] Conversion ignored because is not the server");
            return;
        }

        Debug.LogDebug($"[EXPERIENCE] Converting {player.PlayerName} experience from 1.0 to 2.0");

        { // Axe
            byte[] dataBytes = Instance.api.WorldManager.SaveGame.GetData("LevelUPData_Axe");
            string data = dataBytes == null ? "{}" : SerializerUtil.Deserialize<string>(dataBytes);
            Dictionary<string, ulong> levels = JsonSerializer.Deserialize<Dictionary<string, ulong>>(data);

            if (levels.TryGetValue(player.PlayerUID, out ulong exp))
            {
                ChangeExperience(player, "Axe", exp);
                levels.Remove(player.PlayerUID);
                Instance.api.WorldManager.SaveGame.StoreData("LevelUPData_Axe", JsonSerializer.Serialize(levels));
                Debug.Log($"[EXPERIENCE] Success converted {exp} experience for Axe to player: {player.PlayerName}");
            }
        }

        { // Bow
            byte[] dataBytes = Instance.api.WorldManager.SaveGame.GetData("LevelUPData_Bow");
            string data = dataBytes == null ? "{}" : SerializerUtil.Deserialize<string>(dataBytes);
            Dictionary<string, ulong> levels = JsonSerializer.Deserialize<Dictionary<string, ulong>>(data);

            if (levels.TryGetValue(player.PlayerUID, out ulong exp))
            {
                ChangeExperience(player, "Bow", exp);
                levels.Remove(player.PlayerUID);
                Instance.api.WorldManager.SaveGame.StoreData("LevelUPData_Bow", JsonSerializer.Serialize(levels));
                Debug.Log($"[EXPERIENCE] Success converted {exp} experience for Bow to player: {player.PlayerName}");
            }
        }

        { // BrigandineArmor
            byte[] dataBytes = Instance.api.WorldManager.SaveGame.GetData("LevelUPData_BrigandineArmor");
            string data = dataBytes == null ? "{}" : SerializerUtil.Deserialize<string>(dataBytes);
            Dictionary<string, ulong> levels = JsonSerializer.Deserialize<Dictionary<string, ulong>>(data);

            if (levels.TryGetValue(player.PlayerUID, out ulong exp))
            {
                ChangeExperience(player, "BrigandineArmor", exp);
                levels.Remove(player.PlayerUID);
                Instance.api.WorldManager.SaveGame.StoreData("LevelUPData_BrigandineArmor", JsonSerializer.Serialize(levels));
                Debug.Log($"[EXPERIENCE] Success converted {exp} experience for BrigandineArmor to player: {player.PlayerName}");
            }
        }

        { // ChainArmor
            byte[] dataBytes = Instance.api.WorldManager.SaveGame.GetData("LevelUPData_ChainArmor");
            string data = dataBytes == null ? "{}" : SerializerUtil.Deserialize<string>(dataBytes);
            Dictionary<string, ulong> levels = JsonSerializer.Deserialize<Dictionary<string, ulong>>(data);

            if (levels.TryGetValue(player.PlayerUID, out ulong exp))
            {
                ChangeExperience(player, "ChainArmor", exp);
                levels.Remove(player.PlayerUID);
                Instance.api.WorldManager.SaveGame.StoreData("LevelUPData_ChainArmor", JsonSerializer.Serialize(levels));
                Debug.Log($"[EXPERIENCE] Success converted {exp} experience for ChainArmor to player: {player.PlayerName}");
            }
        }

        { // Cooking
            byte[] dataBytes = Instance.api.WorldManager.SaveGame.GetData("LevelUPData_Cooking");
            string data = dataBytes == null ? "{}" : SerializerUtil.Deserialize<string>(dataBytes);
            Dictionary<string, ulong> levels = JsonSerializer.Deserialize<Dictionary<string, ulong>>(data);

            if (levels.TryGetValue(player.PlayerUID, out ulong exp))
            {
                ChangeExperience(player, "Cooking", exp);
                levels.Remove(player.PlayerUID);
                Instance.api.WorldManager.SaveGame.StoreData("LevelUPData_Cooking", JsonSerializer.Serialize(levels));
                Debug.Log($"[EXPERIENCE] Success converted {exp} experience for Cooking to player: {player.PlayerName}");
            }
        }

        { // Farming
            byte[] dataBytes = Instance.api.WorldManager.SaveGame.GetData("LevelUPData_Farming");
            string data = dataBytes == null ? "{}" : SerializerUtil.Deserialize<string>(dataBytes);
            Dictionary<string, ulong> levels = JsonSerializer.Deserialize<Dictionary<string, ulong>>(data);

            if (levels.TryGetValue(player.PlayerUID, out ulong exp))
            {
                ChangeExperience(player, "Farming", exp);
                levels.Remove(player.PlayerUID);
                Instance.api.WorldManager.SaveGame.StoreData("LevelUPData_Farming", JsonSerializer.Serialize(levels));
                Debug.Log($"[EXPERIENCE] Success converted {exp} experience for Farming to player: {player.PlayerName}");
            }
        }

        { // Hammer
            byte[] dataBytes = Instance.api.WorldManager.SaveGame.GetData("LevelUPData_Hammer");
            string data = dataBytes == null ? "{}" : SerializerUtil.Deserialize<string>(dataBytes);
            Dictionary<string, ulong> levels = JsonSerializer.Deserialize<Dictionary<string, ulong>>(data);

            if (levels.TryGetValue(player.PlayerUID, out ulong exp))
            {
                ChangeExperience(player, "Hammer", exp);
                levels.Remove(player.PlayerUID);
                Instance.api.WorldManager.SaveGame.StoreData("LevelUPData_Hammer", JsonSerializer.Serialize(levels));
                Debug.Log($"[EXPERIENCE] Success converted {exp} experience for Hammer to player: {player.PlayerName}");
            }
        }

        { // Hand
            byte[] dataBytes = Instance.api.WorldManager.SaveGame.GetData("LevelUPData_Hand");
            string data = dataBytes == null ? "{}" : SerializerUtil.Deserialize<string>(dataBytes);
            Dictionary<string, ulong> levels = JsonSerializer.Deserialize<Dictionary<string, ulong>>(data);

            if (levels.TryGetValue(player.PlayerUID, out ulong exp))
            {
                ChangeExperience(player, "Hand", exp);
                levels.Remove(player.PlayerUID);
                Instance.api.WorldManager.SaveGame.StoreData("LevelUPData_Hand", JsonSerializer.Serialize(levels));
                Debug.Log($"[EXPERIENCE] Success converted {exp} experience for Hand to player: {player.PlayerName}");
            }
        }

        { // Hunter
            byte[] dataBytes = Instance.api.WorldManager.SaveGame.GetData("LevelUPData_Hunter");
            string data = dataBytes == null ? "{}" : SerializerUtil.Deserialize<string>(dataBytes);
            Dictionary<string, ulong> levels = JsonSerializer.Deserialize<Dictionary<string, ulong>>(data);

            if (levels.TryGetValue(player.PlayerUID, out ulong exp))
            {
                ChangeExperience(player, "Hunter", exp);
                levels.Remove(player.PlayerUID);
                Instance.api.WorldManager.SaveGame.StoreData("LevelUPData_Hunter", JsonSerializer.Serialize(levels));
                Debug.Log($"[EXPERIENCE] Success converted {exp} experience for Hunter to player: {player.PlayerName}");
            }
        }

        { // Knife
            byte[] dataBytes = Instance.api.WorldManager.SaveGame.GetData("LevelUPData_Knife");
            string data = dataBytes == null ? "{}" : SerializerUtil.Deserialize<string>(dataBytes);
            Dictionary<string, ulong> levels = JsonSerializer.Deserialize<Dictionary<string, ulong>>(data);

            if (levels.TryGetValue(player.PlayerUID, out ulong exp))
            {
                ChangeExperience(player, "Knife", exp);
                levels.Remove(player.PlayerUID);
                Instance.api.WorldManager.SaveGame.StoreData("LevelUPData_Knife", JsonSerializer.Serialize(levels));
                Debug.Log($"[EXPERIENCE] Success converted {exp} experience for Knife to player: {player.PlayerName}");
            }
        }

        { // LeatherArmor
            byte[] dataBytes = Instance.api.WorldManager.SaveGame.GetData("LevelUPData_LeatherArmor");
            string data = dataBytes == null ? "{}" : SerializerUtil.Deserialize<string>(dataBytes);
            Dictionary<string, ulong> levels = JsonSerializer.Deserialize<Dictionary<string, ulong>>(data);

            if (levels.TryGetValue(player.PlayerUID, out ulong exp))
            {
                ChangeExperience(player, "LeatherArmor", exp);
                levels.Remove(player.PlayerUID);
                Instance.api.WorldManager.SaveGame.StoreData("LevelUPData_LeatherArmor", JsonSerializer.Serialize(levels));
                Debug.Log($"[EXPERIENCE] Success converted {exp} experience for LeatherArmor to player: {player.PlayerName}");
            }
        }

        { // Panning
            byte[] dataBytes = Instance.api.WorldManager.SaveGame.GetData("LevelUPData_Panning");
            string data = dataBytes == null ? "{}" : SerializerUtil.Deserialize<string>(dataBytes);
            Dictionary<string, ulong> levels = JsonSerializer.Deserialize<Dictionary<string, ulong>>(data);

            if (levels.TryGetValue(player.PlayerUID, out ulong exp))
            {
                ChangeExperience(player, "Panning", exp);
                levels.Remove(player.PlayerUID);
                Instance.api.WorldManager.SaveGame.StoreData("LevelUPData_Panning", JsonSerializer.Serialize(levels));
                Debug.Log($"[EXPERIENCE] Success converted {exp} experience for Panning to player: {player.PlayerName}");
            }
        }

        { // Pickaxe
            byte[] dataBytes = Instance.api.WorldManager.SaveGame.GetData("LevelUPData_Pickaxe");
            string data = dataBytes == null ? "{}" : SerializerUtil.Deserialize<string>(dataBytes);
            Dictionary<string, ulong> levels = JsonSerializer.Deserialize<Dictionary<string, ulong>>(data);

            if (levels.TryGetValue(player.PlayerUID, out ulong exp))
            {
                ChangeExperience(player, "Pickaxe", exp);
                levels.Remove(player.PlayerUID);
                Instance.api.WorldManager.SaveGame.StoreData("LevelUPData_Pickaxe", JsonSerializer.Serialize(levels));
                Debug.Log($"[EXPERIENCE] Success converted {exp} experience for Pickaxe to player: {player.PlayerName}");
            }
        }

        { // PlateArmor
            byte[] dataBytes = Instance.api.WorldManager.SaveGame.GetData("LevelUPData_PlateArmor");
            string data = dataBytes == null ? "{}" : SerializerUtil.Deserialize<string>(dataBytes);
            Dictionary<string, ulong> levels = JsonSerializer.Deserialize<Dictionary<string, ulong>>(data);

            if (levels.TryGetValue(player.PlayerUID, out ulong exp))
            {
                ChangeExperience(player, "PlateArmor", exp);
                levels.Remove(player.PlayerUID);
                Instance.api.WorldManager.SaveGame.StoreData("LevelUPData_PlateArmor", JsonSerializer.Serialize(levels));
                Debug.Log($"[EXPERIENCE] Success converted {exp} experience for PlateArmor to player: {player.PlayerName}");
            }
        }

        { // ScaleArmor
            byte[] dataBytes = Instance.api.WorldManager.SaveGame.GetData("LevelUPData_ScaleArmor");
            string data = dataBytes == null ? "{}" : SerializerUtil.Deserialize<string>(dataBytes);
            Dictionary<string, ulong> levels = JsonSerializer.Deserialize<Dictionary<string, ulong>>(data);

            if (levels.TryGetValue(player.PlayerUID, out ulong exp))
            {
                ChangeExperience(player, "ScaleArmor", exp);
                levels.Remove(player.PlayerUID);
                Instance.api.WorldManager.SaveGame.StoreData("LevelUPData_ScaleArmor", JsonSerializer.Serialize(levels));
                Debug.Log($"[EXPERIENCE] Success converted {exp} experience for ScaleArmor to player: {player.PlayerName}");
            }
        }

        { // Shield
            byte[] dataBytes = Instance.api.WorldManager.SaveGame.GetData("LevelUPData_Shield");
            string data = dataBytes == null ? "{}" : SerializerUtil.Deserialize<string>(dataBytes);
            Dictionary<string, ulong> levels = JsonSerializer.Deserialize<Dictionary<string, ulong>>(data);

            if (levels.TryGetValue(player.PlayerUID, out ulong exp))
            {
                ChangeExperience(player, "Shield", exp);
                levels.Remove(player.PlayerUID);
                Instance.api.WorldManager.SaveGame.StoreData("LevelUPData_Shield", JsonSerializer.Serialize(levels));
                Debug.Log($"[EXPERIENCE] Success converted {exp} experience for Shield to player: {player.PlayerName}");
            }
        }

        { // Shovel
            byte[] dataBytes = Instance.api.WorldManager.SaveGame.GetData("LevelUPData_Shovel");
            string data = dataBytes == null ? "{}" : SerializerUtil.Deserialize<string>(dataBytes);
            Dictionary<string, ulong> levels = JsonSerializer.Deserialize<Dictionary<string, ulong>>(data);

            if (levels.TryGetValue(player.PlayerUID, out ulong exp))
            {
                ChangeExperience(player, "Shovel", exp);
                levels.Remove(player.PlayerUID);
                Instance.api.WorldManager.SaveGame.StoreData("LevelUPData_Shovel", JsonSerializer.Serialize(levels));
                Debug.Log($"[EXPERIENCE] Success converted {exp} experience for Shovel to player: {player.PlayerName}");
            }
        }

        // Smithing does not exist on 1.0
        // { // Smithing
        //     byte[] dataBytes = Instance.api.WorldManager.SaveGame.GetData("LevelUPData_Smithing");
        //     string data = dataBytes == null ? "{}" : SerializerUtil.Deserialize<string>(dataBytes);
        //     Dictionary<string, ulong> levels = JsonSerializer.Deserialize<Dictionary<string, ulong>>(data);

        //     if (levels.TryGetValue(player.PlayerUID, out ulong exp))
        //     {
        //         ChangeExperience(player, "Smithing", exp);
        //         levels.Remove(player.PlayerUID);
        //         Instance.api.WorldManager.SaveGame.StoreData("LevelUPData_Smithing", JsonSerializer.Serialize(levels));
        //         Debug.Log($"[EXPERIENCE] Success converted {exp} experience for Smithing to player: {player.PlayerName}");
        //     }
        // }

        { // Spear
            byte[] dataBytes = Instance.api.WorldManager.SaveGame.GetData("LevelUPData_Spear");
            string data = dataBytes == null ? "{}" : SerializerUtil.Deserialize<string>(dataBytes);
            Dictionary<string, ulong> levels = JsonSerializer.Deserialize<Dictionary<string, ulong>>(data);

            if (levels.TryGetValue(player.PlayerUID, out ulong exp))
            {
                ChangeExperience(player, "Spear", exp);
                levels.Remove(player.PlayerUID);
                Instance.api.WorldManager.SaveGame.StoreData("LevelUPData_Spear", JsonSerializer.Serialize(levels));
                Debug.Log($"[EXPERIENCE] Success converted {exp} experience for Spear to player: {player.PlayerName}");
            }
        }

        { // Sword
            byte[] dataBytes = Instance.api.WorldManager.SaveGame.GetData("LevelUPData_Sword");
            string data = dataBytes == null ? "{}" : SerializerUtil.Deserialize<string>(dataBytes);
            Dictionary<string, ulong> levels = JsonSerializer.Deserialize<Dictionary<string, ulong>>(data);

            if (levels.TryGetValue(player.PlayerUID, out ulong exp))
            {
                ChangeExperience(player, "Sword", exp);
                levels.Remove(player.PlayerUID);
                Instance.api.WorldManager.SaveGame.StoreData("LevelUPData_Sword", JsonSerializer.Serialize(levels));
                Debug.Log($"[EXPERIENCE] Success converted {exp} experience for Sword to player: {player.PlayerName}");
            }
        }

        { // Vitality
            byte[] dataBytes = Instance.api.WorldManager.SaveGame.GetData("LevelUPData_Vitality");
            string data = dataBytes == null ? "{}" : SerializerUtil.Deserialize<string>(dataBytes);
            Dictionary<string, ulong> levels = JsonSerializer.Deserialize<Dictionary<string, ulong>>(data);

            if (levels.TryGetValue(player.PlayerUID, out ulong exp))
            {
                ChangeExperience(player, "Vitality", exp);
                levels.Remove(player.PlayerUID);
                Instance.api.WorldManager.SaveGame.StoreData("LevelUPData_Vitality", JsonSerializer.Serialize(levels));
                Debug.Log($"[EXPERIENCE] Success converted {exp} experience for Vitality to player: {player.PlayerName}");
            }
        }
    }

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

        ConvertOldExperienceSystemToNew(player);
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
        string playerDirectory = Path.Combine(_saveDirectory, player.PlayerUID);
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