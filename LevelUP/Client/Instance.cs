using Vintagestory.API.Client;

namespace LevelUP.Client;

class Instance {
    public ICoreClientAPI api;
    private readonly CharacterView characterView = new();

    public void Init(ICoreClientAPI clientAPI){
        api = clientAPI;
        characterView.Init(this);
        Debug.Log("Client side initialized");
    }
}