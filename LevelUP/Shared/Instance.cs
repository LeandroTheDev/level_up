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
        if (!disableLevelUpNotify)
        {
            // Previous exp level < Next exp level
            int previousLevel = Configuration.GetLevelByLevelTypeEXP(levelType, player.Entity.WatchedAttributes.GetInt($"LevelUP_{levelType}", 0));
            int nextLevel = Configuration.GetLevelByLevelTypeEXP(levelType, exp);
            if (previousLevel < nextLevel)
            {
                // Notify player
                api.SendMessage(player, 0, $"You reached level {nextLevel} in {levelType}", EnumChatType.Notification);
                Debug.Log($"{player.PlayerName} reached level {nextLevel} in {levelType}");
            }
        }
        player.Entity.WatchedAttributes.SetInt($"LevelUP_{levelType}", exp);
    }
}