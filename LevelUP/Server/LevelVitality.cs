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
    private Dictionary<string, double> playerState = [];

    public void Init()
    {
        // Instanciate Events
        Instance.api.Event.PlayerNowPlaying += PlayerJoin;
        Instance.api.Event.PlayerDisconnect += PlayerDisconnect;
        Instance.api.Event.GameWorldSave += SaveState;

        // Load player state
        playerState = GetSavedState();
        Configuration.RegisterNewLevelTypeEXP("Vitality", Configuration.VitalityGetLevelByEXP);

        Debug.Log("Level Vitality initialized");
    }

#pragma warning disable CA1822
    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulateVitalityConfiguration(coreAPI);
    }
#pragma warning restore CA1822

    private Dictionary<string, double> GetSavedState()
    {
        byte[] dataBytes = Instance.api.WorldManager.SaveGame.GetData("LevelUPData_Vitality_Players_Health");
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
                Debug.LogError($"ERROR: {keyValue.Key} vitalityState is infinity??, reseting to {Configuration.BaseHPVitality} before saving");
                playerState[keyValue.Key] = Configuration.BaseHPVitality;
            }
        }
        Instance.api.WorldManager.SaveGame.StoreData("LevelUPData_Vitality_Players_Health", JsonSerializer.Serialize(playerState));
    }

    private void PlayerJoin(IServerPlayer player)
    {
        // Get the actual player total exp
        ulong playerExp = Experience.GetExperience(player, "Vitality");

        // Get player stats
        EntityBehaviorHealth playerStats = player.Entity.GetBehavior<EntityBehaviorHealth>();
        // Check if stats is null
        if (playerStats == null) { Debug.LogError($"ERROR SETTING MAX HEALTH: Player Stats is null, caused by {player.PlayerName}"); return; }

        // Getting health stats
        float playerMaxHealth = Configuration.VitalityGetMaxHealthByLevel(Configuration.VitalityGetLevelByEXP(playerExp));
        if (float.IsInfinity(playerMaxHealth))
        {
            Debug.LogError($"ERROR: Max health calculation returned any infinity number, please contact BoboDev and report this issue, base health set to {Configuration.BaseHPVitality}");
            playerMaxHealth = Configuration.BaseHPVitality;
        }

        // Getting regen stats
        float playerRegen = Configuration.VitalityGetHealthRegenMultiplyByLevel(Configuration.VitalityGetLevelByEXP(playerExp));
        if (float.IsInfinity(playerRegen))
        {
            Debug.LogError($"ERROR: Regeneration calculation returned any infinity number, please contact BoboDev and report this issue, base regen set to {Configuration.BaseHPRegenVitality}");
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
            Debug.LogDebug($"Cannot find the player: {player.PlayerName} previous health, probably is the first login");
        }

        // Refresh for the player
        playerStats.UpdateMaxHealth();


        Debug.LogDebug($"{player.PlayerName} joined the world with max: {playerStats.MaxHealth} health and {playerStats.Health} actual health");
        Debug.LogDebug($"VITALITY Calculation Variables: {playerMaxHealth}:{playerRegen}, Level: {Configuration.VitalityGetLevelByEXP(playerExp)}");
    }

    private void PlayerDisconnect(IServerPlayer player)
    {
        // Disconnected during the loading
        if (player == null) return;

        // Get stats
        EntityBehaviorHealth playerStats = player.Entity.GetBehavior<EntityBehaviorHealth>();
        if (playerStats == null) { Debug.LogError($"ERROR SAVING PLAYER STATE: Player Stats is null, caused by {player.PlayerName}"); return; }

        // Check error treatment
        if (float.IsInfinity(playerStats.Health))
        {
            Debug.LogError($"ERROR SAVING PLAYER STATE: Player Health is infinity, caused by {player.PlayerName} setting the health to 1.0");
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
            Debug.LogError($"VITALITY ERROR: Cannot save state after disconnecting, the player {player.PlayerName} crashed, playerLife: {playerStats.Health} reason: {ex.Message}");

            Debug.LogDebug("Debugging all vitality states...");
            foreach (KeyValuePair<string, double> keyValue in playerState)
                Debug.LogDebug($"VITALITY DEBUG: Player: {keyValue.Key} Health: {keyValue.Value}");

        }
    }
}