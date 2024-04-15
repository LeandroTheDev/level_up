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
        composer.AddRichtext(GetText(), CairoFont.WhiteDetailText().WithLineHeightMultiplier(1.15), ElementBounds.Fixed(0.0, 25.0, 385.0, 200.0));
    }

    // Creates the full text for level tab
    private string GetText()
    {
        int hunterLevel = instance.api.World.Player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Hunter");
        int bowLevel = instance.api.World.Player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Bow");
        int knifeLevel = instance.api.World.Player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Knife");
        int spearLevel = instance.api.World.Player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Spear");
        int axeLevel = instance.api.World.Player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Axe");
        int pickaxeLevel = instance.api.World.Player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Pickaxe");
        int shovelLevel = instance.api.World.Player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Shovel");
        int farmingLevel = instance.api.World.Player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Farming");
        int cookingLevel = instance.api.World.Player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Cooking");
        int vitalityLevel = instance.api.World.Player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Vitality");
        int leatherArmorLevel = instance.api.World.Player.Entity.WatchedAttributes.GetInt("LevelUP_Level_LeatherArmor");
        int chainArmorLevel = instance.api.World.Player.Entity.WatchedAttributes.GetInt("LevelUP_Level_ChainArmor");

        StringBuilder fulldesc = new();
        fulldesc.AppendLine("Character Levels");
        fulldesc.AppendLine("-- Tools Levels --");
        fulldesc.AppendLine($"Axe: {axeLevel}");
        fulldesc.AppendLine($"Pickaxe: {pickaxeLevel}");
        fulldesc.AppendLine($"Shovel: {shovelLevel}");
        fulldesc.AppendLine($"Bow: {bowLevel}");
        fulldesc.AppendLine($"Spear: {spearLevel}");
        fulldesc.AppendLine($"Knife: {knifeLevel}");
        fulldesc.AppendLine("-- Armor Levels --");
        fulldesc.AppendLine($"Leather Armor: {leatherArmorLevel}");
        fulldesc.AppendLine($"Chain Armor: {chainArmorLevel}");
        fulldesc.AppendLine("-- Specialist Levels --");
        fulldesc.AppendLine($"Hunter: {hunterLevel}");
        fulldesc.AppendLine($"Farming: {farmingLevel}");
        fulldesc.AppendLine($"Cooking: {cookingLevel}");
        fulldesc.AppendLine($"Vitality: {vitalityLevel}");
        return fulldesc.ToString();
    }
}