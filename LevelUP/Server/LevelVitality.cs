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
        instance.api.WorldManager.SaveGame.StoreData("LevelUPData_Vitality_Players_Health", JsonSerializer.Serialize(playerState));
    }

    private void PlayerJoin(IServerPlayer player)
    {
        // Get all players levels
        Dictionary<string, ulong> VitalityLevels = GetSavedLevels();

        // Get the actual player total exp
        ulong playerExp = VitalityLevels.GetValueOrDefault<string, ulong>(player.PlayerName, 0);

        // Get player stats
        EntityBehaviorHealth playerStats = player.Entity.GetBehavior<EntityBehaviorHealth>();
        // Check if stats is null
        if (playerStats == null) { Debug.Log($"ERROR SETTING MAX HEALTH: Player Stats is null, caused by {player.PlayerName}"); return; }

        // Getting health stats
        playerStats.BaseMaxHealth = Configuration.VitalityGetMaxHealthByEXP(playerExp);
        playerStats.MaxHealth = playerStats.BaseMaxHealth;
        playerStats._playerHealthRegenSpeed = Configuration.VitalityGetHealthRegenMultiplyByEXP(playerExp);

        // Reload player health
        if (playerState.TryGetValue(player.PlayerName, out double value)) playerStats.Health = (float)value;

        // Refresh for the player
        playerStats.UpdateMaxHealth();

        Debug.Log($"{player.PlayerName} joined the world with max: {playerStats.MaxHealth} health and {playerStats.Health} actual health");
    }

    private void PlayerDisconnect(IServerPlayer player)
    {
        // Get stats
        EntityBehaviorHealth playerStats = player.Entity.GetBehavior<EntityBehaviorHealth>();
        if (playerStats == null) { Debug.Log($"ERROR SAVING PLAYER STATE: Player Stats is null, caused by {player.PlayerName}"); return; }

        // Update it
        playerState[player.PlayerName] = playerStats.Health;

        // Save it
        SaveState();
    }
}