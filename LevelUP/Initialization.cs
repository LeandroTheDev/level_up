using System;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace LevelUP;

public class Initialization : ModSystem
{
    private readonly Client.Instance clientInstance = new();
    private readonly Server.Instance serverInstance = new();
    public bool isServer = false;

    public override void StartClientSide(ICoreClientAPI api)
    {
        base.StartClientSide(api);
        // Initializing the client instance
        clientInstance.Init(api);
    }
    public override void StartServerSide(ICoreServerAPI api)
    {
        base.StartServerSide(api);
        // Initializing the server instance
        serverInstance.Init(api);
    }

    public override void Start(ICoreAPI api)
    {
        base.Start(api);
        if (isServer) serverInstance.PreInit();
        else clientInstance.PreInit();
    }
    public override void Dispose()
    {
        base.Dispose();
        serverInstance.Dispose();
    }

    public override bool ShouldLoad(EnumAppSide forSide)
    {
        isServer = forSide == EnumAppSide.Server;
        return true;
    }
}

public class Debug
{
    static public void Log(string message)
    {
        Console.WriteLine($"{DateTime.Now:d.M.yyyy HH:mm:ss} [LevelUP] {message}");
    }
}