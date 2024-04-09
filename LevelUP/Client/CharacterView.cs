using System.Text;
using System.Threading.Tasks;
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
        instance.channel.SendPacket<string>("UpdateLevels");
        composer.AddRichtext(GetText(), CairoFont.WhiteDetailText().WithLineHeightMultiplier(1.15), ElementBounds.Fixed(0.0, 25.0, 385.0, 200.0));
    }

    // Creates the full text for level tab
    private string GetText()
    {
        int hunterLevel = instance.api.World.Player.Entity.WatchedAttributes.GetInt("LevelUP_Hunter");
        int bowLevel = instance.api.World.Player.Entity.WatchedAttributes.GetInt("LevelUP_Bow");
        int cutleryLevel = instance.api.World.Player.Entity.WatchedAttributes.GetInt("LevelUP_Cutlery");
        int spearLevel  = instance.api.World.Player.Entity.WatchedAttributes.GetInt("LevelUP_Spear");
        int axeLevel = instance.api.World.Player.Entity.WatchedAttributes.GetInt("LevelUP_Axe");
        int pickaxeLevel = instance.api.World.Player.Entity.WatchedAttributes.GetInt("LevelUP_Pickaxe");
        int shovelLevel  = instance.api.World.Player.Entity.WatchedAttributes.GetInt("LevelUP_Shovel");
        int farmingLevel  = instance.api.World.Player.Entity.WatchedAttributes.GetInt("LevelUP_Farming");

        StringBuilder fulldesc = new();
        fulldesc.AppendLine("Character Levels");
        fulldesc.AppendLine("");
        fulldesc.AppendLine("-- Hunting Levels -- ");
        fulldesc.AppendLine($"Bow: {Configuration.BowGetLevelByEXP(bowLevel)}");
        fulldesc.AppendLine($"Cutlery: {Configuration.CutleryGetLevelByEXP(cutleryLevel)}");
        fulldesc.AppendLine($"Spear: {Configuration.SpearGetLevelByEXP(spearLevel)}");
        fulldesc.AppendLine("-- Tools Levels --");
        fulldesc.AppendLine($"Axe: {Configuration.AxeGetLevelByEXP(axeLevel)}");
        fulldesc.AppendLine($"Pickaxe: {Configuration.PickaxeGetLevelByEXP(pickaxeLevel)}");
        fulldesc.AppendLine($"Shovel: {Configuration.ShovelGetLevelByEXP(shovelLevel)}");
        fulldesc.AppendLine("-- Specialist Levels --");
        fulldesc.AppendLine($"Hunter: {Configuration.HunterGetLevelByEXP(hunterLevel)}");
        fulldesc.AppendLine($"Farming: {Configuration.ShovelGetLevelByEXP(farmingLevel)}");
        return fulldesc.ToString();
    }
}