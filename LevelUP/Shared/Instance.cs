using Vintagestory.API.Common;
using Vintagestory.API.Server;

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

    public static void UpdateLevelAndNotify(ICoreServerAPI api, IPlayer player, string levelType, int exp, bool disableLevelUpNotify = false)
    {

        // Previous exp level, before getting the new experience
        int previousLevel = Configuration.GetLevelByLevelTypeEXP(levelType, player.Entity.WatchedAttributes.GetInt($"LevelUP_{levelType}", 0));
        // Actual player level
        int nextLevel = Configuration.GetLevelByLevelTypeEXP(levelType, exp);

        // Check if player leveled up
        if (previousLevel < nextLevel)
        {
            // Check if we want to notify
            if (!disableLevelUpNotify)
            {
                // Notify player
                api.SendMessage(player, 0, $"You reached level {nextLevel} in {levelType}", EnumChatType.Notification);

                Debug.Log($"{player.PlayerName} reached level {nextLevel} in {levelType}");
            }
        }

        // Experience
        player.Entity.WatchedAttributes.SetInt($"LevelUP_{levelType}", exp);
        // Level
        player.Entity.WatchedAttributes.SetInt($"LevelUP_Level_{levelType}", nextLevel);

        // Mining speed
        float miningspeed = Configuration.GetMiningSpeedByLevelTypeEXP(levelType, nextLevel);
        // Check if this levelType has mining speed
        if (miningspeed != -1)
            // Set the mining speed for clients
            player.Entity.WatchedAttributes.SetFloat($"LevelUP_{levelType}_MiningSpeed", miningspeed);
    }
}