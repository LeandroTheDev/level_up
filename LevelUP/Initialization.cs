using System;
using System.Collections.Generic;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace LevelUP;

public class Initialization : ModSystem
{
    private readonly Client.Instance clientInstance = new();
    private readonly Server.Instance serverInstance = new();
    private readonly Shared.Instance sharedInstance = new();

    public static Dictionary<string, int> DefaultConfigAxe = new();
    public static Dictionary<string, int> DefaultConfigBow = new();
    public static Dictionary<string, int> DefaultConfigCutlery = new();
    public static Dictionary<string, int> DefaultConfigHarvestFarming = new();
    public static Dictionary<string, int> DefaultConfigHunter = new();
    public static Dictionary<string, int> DefaultConfigPickaxe = new();
    public static Dictionary<string, int> DefaultConfigShovel = new();
    public static Dictionary<string, int> DefaultConfigSpear = new();

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
        sharedInstance.InstanciateAPI(api);
        sharedInstance.OverwriteFunctions();
    }

    public override void AssetsLoaded(ICoreAPI api)
    {
        DefaultConfigAxe = api.Assets.Get(new AssetLocation("levelup:config/axe.json")).ToObject<Dictionary<string, int>>();
        DefaultConfigBow = api.Assets.Get(new AssetLocation("levelup:config/bow.json")).ToObject<Dictionary<string, int>>();
        DefaultConfigCutlery = api.Assets.Get(new AssetLocation("levelup:config/cutlery.json")).ToObject<Dictionary<string, int>>();
        DefaultConfigHarvestFarming = api.Assets.Get(new AssetLocation("levelup:config/harvestfarming.json")).ToObject<Dictionary<string, int>>();
        DefaultConfigHunter = api.Assets.Get(new AssetLocation("levelup:config/hunter.json")).ToObject<Dictionary<string, int>>();
        DefaultConfigPickaxe = api.Assets.Get(new AssetLocation("levelup:config/pickaxe.json")).ToObject<Dictionary<string, int>>();
        DefaultConfigShovel = api.Assets.Get(new AssetLocation("levelup:config/shovel.json")).ToObject<Dictionary<string, int>>();
        DefaultConfigSpear = api.Assets.Get(new AssetLocation("levelup:config/spear.json")).ToObject<Dictionary<string, int>>();
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
    static public void Log(string message)
    {
        Console.WriteLine($"{DateTime.Now:d.M.yyyy HH:mm:ss} [LevelUP] {message}");
    }
}