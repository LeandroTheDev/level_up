using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace LevelUP.Shared;

class Instance
{
    public Client.Instance clientAPI;
    public Server.Instance serverAPI;
    public ICoreAPI coreAPI;
    public EnumAppSide side = EnumAppSide.Universal;

    // Override
    readonly OverrideDamageInteraction damageInteraction = new();

    public void InstanciateAPI(object api)
    {
        clientAPI = api is Client.Instance ? api as Client.Instance : null;
        serverAPI = api is Server.Instance ? api as Server.Instance : null;
        coreAPI = api is ICoreAPI ? api as ICoreAPI : null;
    }

    public void OverrideFunctions() {
        damageInteraction.OverrideNativeFunctions(this);
    }
    public void OverrideDispose(){
        damageInteraction.OverrideDispose();
    }
}