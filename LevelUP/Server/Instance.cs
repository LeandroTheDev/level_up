using Vintagestory.API.Server;

namespace LevelUP.Server;

class Instance
{
    public ICoreServerAPI api;

    // Levels
    public LevelHunter levelHunter = new();
    public void Init(ICoreServerAPI serverAPI)
    {
        api = serverAPI;
        levelHunter.Init(this);
        Debug.Log("Server side fully initialized");
    }
}