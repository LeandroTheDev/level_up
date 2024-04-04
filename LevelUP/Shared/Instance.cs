using Vintagestory.API.Common;

namespace LevelUP.Shared;

class Instance
{
    public Client.Instance clientAPI;
    private static Server.Instance serverAPI;
    public ICoreAPI coreAPI;
    public EnumAppSide side = EnumAppSide.Universal;

    // Override
    readonly OverrideDamageInteraction damageInteraction = new();
    readonly OverrideBlockBreak blockBreak = new();

    public void InstanciateAPI(object api)
    {
        clientAPI = api is Client.Instance ? api as Client.Instance : null;
        serverAPI = api is Server.Instance ? api as Server.Instance : null;
        coreAPI = api is ICoreAPI ? api as ICoreAPI : null;
    }

    public void OverwriteFunctions() {
        damageInteraction.OverwriteNativeFunctions(this);
        blockBreak.OverwriteNativeFunctions(this);
    }
    public void OverwriteDispose(){
        // damageInteraction.overwriter.UnpatchAll();
        // blockBreak.overwriter.UnpatchAll();
    }

    public static void UpdateLevelAndNotify(IPlayer player, string levelType, int exp, bool disableLevelUpNotify = false) {
        if(!disableLevelUpNotify) {
            // Previous exp level < Next exp level
            if(Configuration.GetLevelByLevelTypeEXP(levelType, player.Entity.WatchedAttributes.GetInt($"LevelUP_{levelType}", 0)) < Configuration.GetLevelByLevelTypeEXP(levelType, exp)) {
                // Notify player
                serverAPI.api.SendMessage(player, 0, $"You leveled up in {levelType}", EnumChatType.Notification);
                Debug.Log($"{player.PlayerName} reached {Configuration.GetLevelByLevelTypeEXP(levelType, exp)} in {levelType}");
            }
        }
        player.Entity.WatchedAttributes.SetInt($"LevelUP_{levelType}", exp);
    }
}