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
    }
    public override void StartServerSide(ICoreServerAPI api)
    {
        base.StartServerSide(api);
        // Initializing the server instance
        serverInstance.Init(api);
        // Instancianting the Server api for shared functions
        sharedInstance.InstanciateAPI(serverInstance);
    }

    public override void Start(ICoreAPI api)
    {
        base.Start(api);

        Configuration.ClearVariables();

        Debug.LoadLogger(api.Logger);
        Debug.Log($"Running on Version: {Mod.Info.Version}");
        Server.Experience.LoadInstance(api);
        sharedInstance.InstanciateAPI(api);
        sharedInstance.OverwriteFunctions();
    }

    public override void AssetsLoaded(ICoreAPI api)
    {
        if (api.Side == EnumAppSide.Server)
            serverInstance.PopulateConfigurations(api);
    }

    public override void Dispose()
    {
        base.Dispose();
        sharedInstance.OverwriteDispose();
    }

    public override bool ShouldLoad(EnumAppSide forSide)
    {
        sharedInstance.side = forSide;
        return base.ShouldLoad(forSide);
    }

    public override double ExecuteOrder()
    {
        return 1;
    }
}

public class Debug
{
    static private ILogger logger;

    static public void LoadLogger(ILogger _logger) => logger = _logger;
    static public void Log(string message)
    {
        logger?.Log(EnumLogType.Notification, $"[LevelUP] {message}");
    }
    static public void LogDebug(string message)
    {
        if (Configuration.enableExtendedLog)
            logger?.Log(EnumLogType.Debug, $"[LevelUP] {message}");
    }
    static public void LogWarn(string message)
    {
        logger?.Log(EnumLogType.Warning, $"[LevelUP] {message}");
    }
    static public void LogError(string message)
    {
        logger?.Log(EnumLogType.Error, $"[LevelUP] {message}");
    }
}