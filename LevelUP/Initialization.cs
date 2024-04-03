using System;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace LevelUP;

public class Initialization : ModSystem
{
    private readonly Client.Instance clientInstance = new();
    private readonly Server.Instance serverInstance = new();
    private readonly Shared.Instance sharedInstance = new();

    public override void StartClientSide(ICoreClientAPI api)
    {
        base.StartClientSide(api);
        // Initializing the client instance
        clientInstance.Init(api);
        // Instancianting the Client api for shared functions
        sharedInstance.InstanciateAPI(clientInstance);
        // Change the shared side to client
        sharedInstance.side = EnumAppSide.Client;
    }
    public override void StartServerSide(ICoreServerAPI api)
    {
        base.StartServerSide(api);
        // Initializing the server instance
        serverInstance.Init(api);
        // Instancianting the Server api for shared functions
        sharedInstance.InstanciateAPI(serverInstance);
        // Change the shared side to server
        sharedInstance.side = EnumAppSide.Server;
    }

    public override void Start(ICoreAPI api)
    {
        base.Start(api);
        sharedInstance.InstanciateAPI(api);
        sharedInstance.OverrideFunctions();
    }
    public override void Dispose()
    {
        base.Dispose();
        sharedInstance.OverrideDispose();
    }
}

public class Debug
{
    static public void Log(string message)
    {
        Console.WriteLine($"{DateTime.Now:d.M.yyyy HH:mm:ss} [LevelUP] {message}");
    }
}