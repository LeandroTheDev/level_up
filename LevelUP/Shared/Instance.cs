using LevelUP.Server;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace LevelUP.Shared;

class Instance
{
    public Client.Instance clientAPI;
    public Server.Instance serverAPI;
    public ICoreAPI coreAPI;
    public EnumAppSide side = EnumAppSide.Universal;

    // Overwrite
    readonly OverwriteDamageInteraction damageInteraction = new();
    readonly OverwriteBlockBreak blockBreak = new();
    readonly OverwriteBlockInteraction blockInteraction = new();

    public void InstanciateAPI(object api)
    {
        clientAPI = api is Client.Instance ? api as Client.Instance : null;
        serverAPI = api is Server.Instance ? api as Server.Instance : null;
        coreAPI = api is ICoreAPI ? api as ICoreAPI : null;
    }

    public void OverwriteFunctions()
    {
        damageInteraction.OverwriteNativeFunctions(this);
        blockBreak.OverwriteNativeFunctions(this);
        blockInteraction.OverwriteNativeFunctions(this);
    }
    public void OverwriteDispose()
    {
        damageInteraction.overwriter?.UnpatchAll();
        blockBreak.overwriter?.UnpatchAll();
        blockInteraction.overwriter?.UnpatchAll();
    }

    public static void UpdateLevelAndNotify(ICoreServerAPI _, IPlayer player, string levelType, ulong exp, bool disableLevelUpNotify = false)
    {
        // Previous exp level, before getting the new experience
        int previousLevel = Configuration.GetLevelByLevelTypeEXP(levelType, (ulong)player.Entity.WatchedAttributes.GetLong($"LevelUP_{levelType}", 0));
        // Actual player level
        int nextLevel = Configuration.GetLevelByLevelTypeEXP(levelType, exp);

        // Check if player leveled up
        if (previousLevel < nextLevel)
        {
            // Check if we want to notify
            if (!disableLevelUpNotify)
            {
                // Notify player
                if (Configuration.enableLevelUpChatMessages)
                    Server.Instance.CommunicationChannel.SendPacket(new ServerMessage() { message = $"playerlevelup&{nextLevel}&{levelType}" }, player as IServerPlayer);
            }
            Debug.Log($"{player.PlayerName} reached level {nextLevel} in {levelType}");

            ExperienceEvents.PlayerLeveledUp(player, levelType, exp, nextLevel);

            // if vitality leveled we need to update the player max health
            if (levelType == "Vitality")
            {
                // Get player stats
                EntityBehaviorHealth playerStats = player.Entity.GetBehavior<EntityBehaviorHealth>();
                // Check if stats is null
                if (playerStats == null) { Debug.LogError($"ERROR SETTING MAX HEALTH: Player Stats is null, caused by {player.PlayerName}"); return; }

                // Getting health stats
                playerStats.BaseMaxHealth = Configuration.VitalityGetMaxHealthByLevel(nextLevel);
                player.Entity.WatchedAttributes.SetFloat("regenSpeed", Configuration.VitalityGetHealthRegenMultiplyByLevel(nextLevel));

                // Refresh for the player
                playerStats.UpdateMaxHealth();

                Debug.LogDebug($"{player.PlayerName} updated the max: {playerStats.MaxHealth} health");
            }
        }

        // Experience
        player.Entity.WatchedAttributes.SetLong($"LevelUP_{levelType}", (long)exp);
        // Level
        player.Entity.WatchedAttributes.SetInt($"LevelUP_Level_{levelType}", nextLevel);

        // Mining speed
        float miningspeed = Configuration.GetMiningSpeedByLevelTypeLevel(levelType, nextLevel);
        // Check if this levelType has mining speed
        if (miningspeed != -1)
            // Set the mining speed for clients
            player.Entity.WatchedAttributes.SetFloat($"LevelUP_{levelType}_MiningSpeed", miningspeed);
    }

    public static void UpdateSubLevelAndNotify(ICoreServerAPI _, IPlayer player, string levelType, string subLevelType, ulong exp, bool disableLevelUpNotify = false)
    {
        // Previous exp level, before getting the new experience
        int previousLevel = Configuration.GetLevelByLevelTypeEXP(levelType, (ulong)player.Entity.WatchedAttributes.GetLong($"LevelUP_{levelType}_Sub_{subLevelType}", 0));
        // Actual player level
        int nextLevel = Configuration.GetLevelByLevelTypeEXP(levelType, exp);

        // Check if player leveled up
        if (previousLevel < nextLevel)
        {
            // Check if we want to notify
            if (!disableLevelUpNotify)
            {
                // Notify player
                if (Configuration.enableLevelUpChatMessages)
                    Server.Instance.CommunicationChannel.SendPacket(new ServerMessage() { message = $"playersublevelup&{nextLevel}&{levelType}&{subLevelType}" }, player as IServerPlayer);
            }
            Debug.Log($"{player.PlayerName} reached sub level {nextLevel} in {levelType}/{subLevelType}");

            ExperienceEvents.PlayerLeveledUp(player, levelType + "/" + subLevelType, exp, nextLevel);
        }

        // Experience
        player.Entity.WatchedAttributes.SetLong($"LevelUP_{levelType}_Sub_{subLevelType}", (long)exp);
        // Level
        player.Entity.WatchedAttributes.SetInt($"LevelUP_Level_{levelType}_Sub_{subLevelType}", nextLevel);
    }
}