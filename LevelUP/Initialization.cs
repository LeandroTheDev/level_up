using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace LevelUP;

public class Initialization : ModSystem
{
    private readonly Client.Instance clientInstance = new();
    private readonly Server.Instance serverInstance = new();

    public override void StartClientSide(ICoreClientAPI api)
    {
        base.StartClientSide(api);

        Shared.Instance.api = api;
        clientInstance.Init(api);
    }

    public override void StartServerSide(ICoreServerAPI api)
    {
        base.StartServerSide(api);

        Shared.Instance.api = api;
        serverInstance.Init(api);
    }

    public override void Start(ICoreAPI api)
    {
        base.Start(api);

        Debug.LoadLogger(api.Logger);
        Debug.Log($"Running on Version: {Mod.Info.Version}");
        Server.Experience.LoadInstance(api);
    }

    public override void AssetsLoaded(ICoreAPI api)
    {
        if (api.Side == EnumAppSide.Server)
            Server.Instance.PopulateConfigurations(api);
    }

    public override void Dispose()
    {
        base.Dispose();
        Configuration.ClearVariables();
        Shared.Instance.UnpatchAll();
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