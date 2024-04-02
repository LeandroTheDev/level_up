using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Vintagestory.API.Client;

namespace LevelUP.Client;

class CharacterView
{
    Instance instance;

    GuiDialogCharacterBase characterView;

    public void Init(Instance _instance)
    {
        instance = _instance;
        // We need to wait because for some reason this mod is loading before
        // the traits page, and unfurtunally the dev made the traits page do not
        // use the data int as Tabs.Count making the things urrensposible (he used a const 1 integer)
        Task.Delay(1000).ContinueWith((_) =>
        {
            // Creating Level Table
            characterView = instance.api.Gui.LoadedGuis.Find((GuiDialog dlg) => dlg is GuiDialogCharacterBase) as GuiDialogCharacterBase;
            characterView.Tabs.Add(new GuiTab
            {
                Name = "Level",
                DataInt = characterView.Tabs.Count
            });
            characterView.RenderTabHandlers.Add(ComposeLevelTab);
            Debug.Log("Level Tab created in character view");
        });
    }

    // Compose the gui page for the level tab
    private void ComposeLevelTab(GuiComposer composer)
    {
        composer.AddRichtext(GetText(), CairoFont.WhiteDetailText().WithLineHeightMultiplier(1.15), ElementBounds.Fixed(0.0, 25.0, 385.0, 200.0));
    }

    // Creates the full text for level tab
    private string GetText()
    {
        int hunterLevel = instance.api.World.Player.Entity.WatchedAttributes.GetAsInt("LevelUP_Hunter");
        Debug.Log($"Character view rendering, hunter level: {hunterLevel}");
        StringBuilder fulldesc = new();
        fulldesc.AppendLine("Character Levels");
        fulldesc.AppendLine($"Hunter: {hunterLevel}");
        return fulldesc.ToString();
    }
}