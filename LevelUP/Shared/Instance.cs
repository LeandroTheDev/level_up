using System.Threading.Tasks;
using LevelUP.Server;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
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
        }

        // This is a heavy formula calculations, we run on task to reduce and prevent lag spikes
        Task.Run(() =>
        {
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

            // Refresh metabolism
            if (levelType == "Metabolism")
            {
                float saturationConsumeReducer = Configuration.MetabolismGetSaturationReceiveMultiplyByLevel(nextLevel);
                player.Entity.WatchedAttributes.SetFloat($"LevelUP_MetabolismReceiveMultiply", saturationConsumeReducer);

                LevelMetabolism.RefreshMaxSaturation(player);
            }
            // Refresh vitality
            else if (levelType == "Vitality")
            {
                LevelVitality.RefreshMaxHealth(player);
            }
        });
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