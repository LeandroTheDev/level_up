// using System;
// using System.Collections.Generic;
// using System.IO;
// using Vintagestory.API.Common;
// using Vintagestory.API.Server;
// using Vintagestory.GameContent;

// namespace LevelUP.Server;

// class LevelMetabolism
// {
//     static private string _saveDirectory = "";
//     private Dictionary<string, double> _playerLoadedMetabolism = [];

//     public void Init()
//     {
//         // Instanciate Events
//         Instance.api.Event.PlayerNowPlaying += PlayerJoin;
//         Instance.api.Event.PlayerDisconnect += PlayerDisconnect;
//         Instance.api.Event.GameWorldSave += SaveState;
//         Configuration.RegisterNewLevelTypeEXP("Metabolism", Configuration.MetabolismGetLevelByEXP);
//         Configuration.RegisterNewEXPLevelType("Metabolism", Configuration.MetabolismGetExpByLevel);

//         Debug.Log("Level Metabolism initialized");
//     }

// #pragma warning disable CA1822
//     public void PopulateConfiguration(ICoreAPI coreAPI)
//     {
//         // Load player state
//         _saveDirectory = Path.Combine(coreAPI.DataBasePath, $"ModData/LevelUP/{coreAPI.World.SavegameIdentifier}-Metabolism");
//         Debug.Log($"LevelUP will save vitality data in: {_saveDirectory}");
//         Directory.CreateDirectory(_saveDirectory);

//         // Populate configuration
//         Configuration.PopulateMetabolismConfiguration(coreAPI);
//         Configuration.RegisterNewMaxLevelByLevelTypeEXP("Metabolism", Configuration.vitalityMaxLevel);
//     }
// #pragma warning restore CA1822

//     /// <summary>
//     /// Loads the player to the memory
//     /// </summary>
//     /// <param name="player"></param>
//     private void LoadPlayer(IPlayer player)
//     {
//         if (!Utils.ValidatePlayerUID(player))
//         {
//             if (player is IServerPlayer serverPlayer)
//                 serverPlayer.Disconnect("[LEVELUP SECURITY] Invalid UID");
//             return;
//         }

//         string playerDirectory = Path.Combine(_saveDirectory, player.PlayerUID);

//         // Conversion to the safe UID
//         {
//             string correctDirectory = Path.Combine(_saveDirectory, Utils.ConvertPlayerUID(player.PlayerUID));
//             // Wrong directory exists, lets move it
//             if (Directory.Exists(playerDirectory))
//             {
//                 if (playerDirectory != correctDirectory)
//                 {
//                     Debug.LogWarn($"{player.PlayerUID} is saved on unsafe directory, levelup will move from: {playerDirectory} to {correctDirectory}");
//                     Directory.Move(playerDirectory, correctDirectory);
//                 }
//             }
//         }

//         Debug.LogDebug($"Loading {player.PlayerName} vitality: {Utils.ConvertPlayerUID(player.PlayerUID)}");

//         playerDirectory = Path.Combine(_saveDirectory, Utils.ConvertPlayerUID(player.PlayerUID));

//         Directory.CreateDirectory(playerDirectory);

//         string lastSaturationFile = Path.Combine(playerDirectory, "lastSaturation.txt");
//         if (File.Exists(lastSaturationFile))
//         {
//             string lastSaturationString = File.ReadAllText(lastSaturationFile);

//             if (double.TryParse(lastSaturationString, out double lastSaturation))
//                 _playerLoadedMetabolism[player.PlayerUID] = lastSaturation;
//             else
//             {
//                 _playerLoadedMetabolism[player.PlayerUID] = Configuration.BaseHPMetabolism;
//                 Debug.LogError($"[METABOLISM] {player.PlayerName} has any invalid vitality saturation: {lastSaturationString}");
//             }
//         }
//         else
//         {
//             _playerLoadedMetabolism[player.PlayerUID] = Configuration.BaseHPMetabolism;
//             Debug.LogDebug($"Cannot find the player: {player.PlayerName} previous saturation, probably is the first login, setting HP to: {Configuration.BaseHPMetabolism}");
//         }
//     }

//     /// <summary>
//     /// Saves the player and unload it from the memory
//     /// </summary>
//     /// <param name="player"></param>
//     private void UnloadPlayer(IPlayer player)
//     {
//         if (!_playerLoadedMetabolism.ContainsKey(player.PlayerUID)) return;

//         SavePlayer(player);

//         _playerLoadedMetabolism.Remove(player.PlayerUID);
//     }

//     /// <summary>
//     /// Manually save the player experience and levels
//     /// </summary>
//     /// <param name="player"></param>
//     private void SavePlayer(IPlayer player)
//     {
//         if (!Utils.ValidatePlayerUID(player)) return;

//         string playerDirectory = Path.Combine(_saveDirectory, Utils.ConvertPlayerUID(player.PlayerUID));
//         Directory.CreateDirectory(playerDirectory);

//         string lastSaturationFile = Path.Combine(playerDirectory, "lastSaturation.txt");

//         if (_playerLoadedMetabolism.TryGetValue(player.PlayerUID, out double lastSaturation))
//         {
//             File.WriteAllText(lastSaturationFile, lastSaturation.ToString());
//             Debug.LogDebug($"[METABOLISM] {player.PlayerName} saved to the path: {playerDirectory}");
//         }
//         else
//             Debug.LogError($"[METABOLISM] {player.PlayerName} cannot be found in playerLoadedMetabolism");
//     }

//     private void SaveState()
//     {
//         foreach (IPlayer player in Instance.api.World.AllOnlinePlayers)
//         {
//             // Saving each online player
//             foreach (KeyValuePair<string, double> keyValue in _playerLoadedMetabolism)
//             {
//                 // Iteration is not the player
//                 if (keyValue.Key != player.PlayerUID) continue;

//                 // Check infinity bug
//                 if (double.IsInfinity(keyValue.Value))
//                 {
//                     Debug.LogError($"ERROR: {keyValue.Key} vitalityState is infinity??, reseting to {Configuration.BaseHPMetabolism} before saving");
//                     _playerLoadedMetabolism[keyValue.Key] = Configuration.BaseHPMetabolism;
//                 }
//                 else
//                 {
//                     SavePlayer(player);
//                 }
//             }
//         }
//     }

//     private void PlayerJoin(IServerPlayer player)
//     {
//         LoadPlayer(player);

//         // Get the actual player total exp
//         ulong playerExp = Experience.GetExperience(player, "Metabolism");

//         // Get player stats
//         EntityBehaviorSaturation playerStats = player.Entity.GetBehavior<EntityBehaviorSaturation>();
//         // Check if stats is null
//         if (playerStats == null) { Debug.LogError($"[METABOLISM] ERROR SETTING MAX HEALTH: Player Stats is null, caused by {player.PlayerName}"); return; }

//         // Getting saturation stats
//         float playerMaxSaturation = Configuration.MetabolismGetMaxSaturationByLevel(Configuration.MetabolismGetLevelByEXP(playerExp));
//         if (float.IsInfinity(playerMaxSaturation))
//         {
//             Debug.LogError($"[METABOLISM] ERROR: Max saturation calculation returned any infinity number, please report this issue, base saturation set to {Configuration.BaseHPMetabolism}");
//             playerMaxSaturation = Configuration.BaseHPMetabolism;
//         }

//         // Getting regen stats
//         float playerRegen = Configuration.MetabolismGetSaturationRegenMultiplyByLevel(Configuration.MetabolismGetLevelByEXP(playerExp));
//         if (float.IsInfinity(playerRegen))
//         {
//             Debug.LogError($"[METABOLISM] ERROR: Regeneration calculation returned any infinity number, please report this issue, base regen set to {Configuration.BaseHPRegenMetabolism}");
//             playerRegen = Configuration.BaseHPRegenMetabolism;
//         }

//         playerStats.BaseMaxSaturation = playerMaxSaturation;
//         playerStats.MaxSaturation = playerMaxSaturation;
//         player.Entity.WatchedAttributes.SetFloat("regenSpeed", playerRegen);

//         // Refresh for the player
//         playerStats.UpdateMaxSaturation();

//         // Reload player saturation
//         if (_playerLoadedMetabolism.TryGetValue(player.PlayerUID, out double value)) playerStats.Saturation = (float)value;
//         // If cannot find player will receive the base max saturation instead
//         else
//         {
//             playerStats.Saturation = playerMaxSaturation;
//             Debug.LogError($"[METABOLISM] Cannot find the player: {player.PlayerName} saturation, something goes wrong");
//         }

//         Debug.LogDebug($"[METABOLISM] {player.PlayerName} joined the world with max: {playerStats.MaxSaturation} saturation and {playerStats.Saturation} actual saturation");
//         Debug.LogDebug($"[METABOLISM] Calculation Variables: {playerMaxSaturation}:{playerRegen}, Level: {Configuration.MetabolismGetLevelByEXP(playerExp)}");
//     }

//     private void PlayerDisconnect(IServerPlayer player)
//     {
//         try
//         {
//             // Disconnected during the loading
//             if (player.Entity == null) { Debug.LogDebug($"[METABOLISM] {player.PlayerName} entity is null"); return; }

//             // Get stats
//             EntityBehaviorSaturation playerStats = player.Entity.GetBehavior<EntityBehaviorSaturation>();
//             if (playerStats == null) { Debug.LogError($"[METABOLISM] ERROR SAVING PLAYER STATE: Player Stats is null, caused by {player.PlayerName}"); return; }

//             // Check error treatment
//             if (float.IsInfinity(playerStats.Saturation))
//             {
//                 Debug.LogError($"[METABOLISM] ERROR SAVING PLAYER STATE: Player Saturation is infinity, caused by {player.PlayerName} setting the saturation to {Configuration.BaseHPMetabolism}");
//                 _playerLoadedMetabolism[player.PlayerUID] = Configuration.BaseHPMetabolism;
//             }
//             // Update it
//             else _playerLoadedMetabolism[player.PlayerUID] = playerStats.Saturation;
//         }
//         catch (Exception) { }
//         finally
//         {
//             UnloadPlayer(player);
//         }
//     }
// }