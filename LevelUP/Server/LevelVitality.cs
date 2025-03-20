using System;
using System.Collections.Generic;
using System.Text.Json;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using Vintagestory.API.Util;
using Vintagestory.GameContent;

namespace LevelUP.Server;

class LevelVitality
{
    private Instance instance;
    private Dictionary<string, double> playerState = [];

    public void Init(Instance _instance)
    {
        instance = _instance;
        // Instanciate Events
        instance.api.Event.PlayerNowPlaying += PlayerJoin;
        instance.api.Event.PlayerDisconnect += PlayerDisconnect;
        instance.api.Event.GameWorldSave += SaveState;

        // Load player state
        playerState = GetSavedState();

        Debug.Log("Level Vitality initialized");
    }

#pragma warning disable CA1822
    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulateVitalityConfiguration(coreAPI);
    }
#pragma warning restore CA1822

    private Dictionary<string, ulong> GetSavedLevels()
    {
        byte[] dataBytes = instance.api.WorldManager.SaveGame.GetData("LevelUPData_Vitality");
        string data = dataBytes == null ? "{}" : SerializerUtil.Deserialize<string>(dataBytes);
        return JsonSerializer.Deserialize<Dictionary<string, ulong>>(data);
    }

    private Dictionary<string, double> GetSavedState()
    {
        byte[] dataBytes = instance.api.WorldManager.SaveGame.GetData("LevelUPData_Vitality_Players_Health");
        string data = dataBytes == null ? "{}" : SerializerUtil.Deserialize<string>(dataBytes);
        return JsonSerializer.Deserialize<Dictionary<string, double>>(data);
    }

    private void SaveState()
    {
        // Exception Treatment
        foreach (KeyValuePair<string, double> keyValue in playerState)
        {
            // Check infinity bug
            if (double.IsInfinity(keyValue.Value))
            {
                Debug.Log($"ERROR: {keyValue.Key} vitalityState is infinity??, reseting to {Configuration.BaseHPVitality} before saving");
                playerState[keyValue.Key] = Configuration.BaseHPVitality;
            }
        }
        instance.api.WorldManager.SaveGame.StoreData("LevelUPData_Vitality_Players_Health", JsonSerializer.Serialize(playerState));
    }

    private void PlayerJoin(IServerPlayer player)
    {
        // Get all players levels
        Dictionary<string, ulong> VitalityLevels = GetSavedLevels();

        // Get the actual player total exp
        ulong playerExp = VitalityLevels.GetValueOrDefault<string, ulong>(player.PlayerUID, 0);

        // Get player stats
        EntityBehaviorHealth playerStats = player.Entity.GetBehavior<EntityBehaviorHealth>();
        // Check if stats is null
        if (playerStats == null) { Debug.Log($"ERROR SETTING MAX HEALTH: Player Stats is null, caused by {player.PlayerName}"); return; }

        // Getting health stats
        float playerMaxHealth = Configuration.VitalityGetMaxHealthByLevel(Configuration.VitalityGetLevelByEXP(playerExp));
        if (float.IsInfinity(playerMaxHealth))
        {
            Debug.Log($"ERROR: Max health calculation returned any infinity number, please contact BoboDev and report this issue, base health set to {Configuration.BaseHPVitality}");
            playerMaxHealth = Configuration.BaseHPVitality;
        }

        // Getting regen stats
        float playerRegen = Configuration.VitalityGetHealthRegenMultiplyByLevel(Configuration.VitalityGetLevelByEXP(playerExp));
        if (float.IsInfinity(playerRegen))
        {
            Debug.Log($"ERROR: Regeneration calculation returned any infinity number, please contact BoboDev and report this issue, base regen set to {Configuration.BaseHPRegenVitality}");
            playerRegen = Configuration.BaseHPRegenVitality;
        }

        playerStats.BaseMaxHealth = playerMaxHealth;
        playerStats.MaxHealth = playerMaxHealth;
        player.Entity.WatchedAttributes.SetFloat("regenSpeed", playerRegen);

        // Reload player health
        if (playerState.TryGetValue(player.PlayerUID, out double value)) playerStats.Health = (float)value;
        // If cannot find player will receive the base max health instead
        else
        {
            playerStats.Health = playerMaxHealth;
            if (Configuration.enableExtendedLog)
                Debug.Log($"Cannot find the player: {player.PlayerName} previous health, probably is the first login");
        };

        // Refresh for the player
        playerStats.UpdateMaxHealth();

        if (Configuration.enableExtendedLog)
        {
            Debug.Log($"{player.PlayerName} joined the world with max: {playerStats.MaxHealth} health and {playerStats.Health} actual health");
            Debug.Log($"VITALITY Calculation Variables: {playerMaxHealth}:{playerRegen}, Level: {Configuration.VitalityGetLevelByEXP(playerExp)}");
        }
    }

    private void PlayerDisconnect(IServerPlayer player)
    {
        // Disconnected during the loading
        if (player == null) return;

        // Get stats
        EntityBehaviorHealth playerStats = player.Entity.GetBehavior<EntityBehaviorHealth>();
        if (playerStats == null) { Debug.Log($"ERROR SAVING PLAYER STATE: Player Stats is null, caused by {player.PlayerName}"); return; }

        // Check error treatment
        if (float.IsInfinity(playerStats.Health))
        {
            Debug.Log($"ERROR SAVING PLAYER STATE: Player Health is infinity, caused by {player.PlayerName} setting the health to 1.0");
            playerState[player.PlayerUID] = 1.0f;
        }
        // Update it
        else playerState[player.PlayerUID] = playerStats.Health;

        // Save it
        try
        {
            SaveState();
        }
        catch (Exception ex)
        {
            Debug.Log($"VITALITY ERROR: Cannot save state after disconnecting, the player {player.PlayerName} crashed, playerLife: {playerStats.Health} reason: {ex.Message}");
            if (Configuration.enableExtendedLog)
            {
                Debug.Log("Debugging all vitality states...");
                foreach (KeyValuePair<string, double> keyValue in playerState)
                    Debug.Log($"VITALITY DEBUG: Player: {keyValue.Key} Health: {keyValue.Value}");
            }
        }
    }
}