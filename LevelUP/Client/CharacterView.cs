using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace LevelUP.Client;

class CharacterView
{
    Instance instance;

    GuiDialogCharacterBase characterView;
    private GuiComposer levelTabComposer;

    byte page = 0;

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

    private void ComposeLevelTab(GuiComposer composer)
    {
        switch (page)
        {
            case 0: ComposeFirstPage(composer); break;

            case 1: break;
        }
        composer.AddButton("Back", OnBackClicked, ElementBounds.Fixed(0.0, 300.0, 20.0, 15.0));
        composer.AddButton("Next", OnNextClicked, ElementBounds.Fixed(310.0, 300.0, 30.0, 15.0));

        levelTabComposer = composer;
        // #region 3
        // // Leather Armor
        // composer.AddImage(ElementBounds.Fixed(0.0, 225.0, 385.0, 200.0), new AssetLocation("levelup:unknown.png"));
        // composer.AddRichtext(
        //     $"{Lang.Get("levelup:leather_armor")}: {GetLevelByLevelName("LeatherArmor")}",
        //     CairoFont.WhiteDetailText().WithLineHeightMultiplier(1.15),
        //     ElementBounds.Fixed(0.0, 300.0, 385.0, 200.0)
        // );
        // // Chain Armor
        // composer.AddImage(ElementBounds.Fixed(100.0, 225.0, 385.0, 200.0), new AssetLocation("levelup:unknown.png"));
        // composer.AddRichtext(
        //     $"{Lang.Get("levelup:chain_armor")}: {GetLevelByLevelName("ChainArmor")}",
        //     CairoFont.WhiteDetailText().WithLineHeightMultiplier(1.15),
        //     ElementBounds.Fixed(100.0, 300.0, 385.0, 200.0)
        // );
        // // Hunter
        // composer.AddImage(ElementBounds.Fixed(200.0, 225.0, 385.0, 200.0), new AssetLocation("levelup:unknown.png"));
        // composer.AddRichtext(
        //     $"{Lang.Get("levelup:hunter")}: {GetLevelByLevelName("Hunter")}",
        //     CairoFont.WhiteDetailText().WithLineHeightMultiplier(1.15),
        //     ElementBounds.Fixed(200.0, 300.0, 385.0, 200.0)
        // );
        // // Farming
        // composer.AddImage(ElementBounds.Fixed(300.0, 225.0, 385.0, 200.0), new AssetLocation("levelup:unknown.png"));
        // composer.AddRichtext(
        //     $"{Lang.Get("levelup:farming")}: {GetLevelByLevelName("Farming")}",
        //     CairoFont.WhiteDetailText().WithLineHeightMultiplier(1.15),
        //     ElementBounds.Fixed(300.0, 300.0, 385.0, 200.0)
        // );
        // #endregion
        // #region 4
        // // Cooking
        // composer.AddImage(ElementBounds.Fixed(0.0, 325.0, 385.0, 200.0), new AssetLocation("levelup:unknown.png"));
        // composer.AddRichtext(
        //     $"{Lang.Get("levelup:cooking")}: {GetLevelByLevelName("Cooking")}",
        //     CairoFont.WhiteDetailText().WithLineHeightMultiplier(1.15),
        //     ElementBounds.Fixed(0.0, 400.0, 385.0, 200.0)
        // );
        // // Vitality
        // composer.AddImage(ElementBounds.Fixed(100.0, 325.0, 385.0, 200.0), new AssetLocation("levelup:unknown.png"));
        // composer.AddRichtext(
        //     $"{Lang.Get("levelup:vitality")}: {GetLevelByLevelName("Vitality")}",
        //     CairoFont.WhiteDetailText().WithLineHeightMultiplier(1.15),
        //     ElementBounds.Fixed(100.0, 400.0, 385.0, 200.0)
        // );
        // #endregion
    }

    private void ComposeFirstPage(GuiComposer composer)
    {
        #region 1
        // Axe
        composer.AddImage(ElementBounds.Fixed(0.0, 25.0, 385.0, 200.0), new AssetLocation("levelup:axe.png"));
        SetComposerLevel(composer, ElementBounds.Fixed(0.0, 25.0, 385.0, 200.0), GetLevelByLevelName("Axe"));
        // Pickaxe
        composer.AddImage(ElementBounds.Fixed(150.0, 25.0, 385.0, 200.0), new AssetLocation("levelup:pickaxe.png"));
        SetComposerLevel(composer, ElementBounds.Fixed(150.0, 25.0, 385.0, 200.0), GetLevelByLevelName("Pickaxe"));
        // Shovel
        composer.AddImage(ElementBounds.Fixed(300.0, 25.0, 385.0, 200.0), new AssetLocation("levelup:shovel.png"));
        SetComposerLevel(composer, ElementBounds.Fixed(300.0, 25.0, 385.0, 200.0), GetLevelByLevelName("Shovel"));
        #endregion
        #region 2
        // Knife
        composer.AddImage(ElementBounds.Fixed(0.0, 125.0, 385.0, 200.0), new AssetLocation("levelup:unknown.png"));
        SetComposerLevel(composer, ElementBounds.Fixed(0.0, 125.0, 385.0, 200.0), GetLevelByLevelName("Knife"));
        // Bow
        composer.AddImage(ElementBounds.Fixed(150.0, 125.0, 385.0, 200.0), new AssetLocation("levelup:unknown.png"));
        SetComposerLevel(composer, ElementBounds.Fixed(150.0, 125.0, 385.0, 200.0), GetLevelByLevelName("Bow"));
        // Spear
        composer.AddImage(ElementBounds.Fixed(300.0, 125.0, 385.0, 200.0), new AssetLocation("levelup:unknown.png"));
        SetComposerLevel(composer, ElementBounds.Fixed(300.0, 125.0, 385.0, 200.0), GetLevelByLevelName("Spear"));
        #endregion
        #region 3
        // Hammer
        composer.AddImage(ElementBounds.Fixed(0.0, 225.0, 385.0, 200.0), new AssetLocation("levelup:unknown.png"));
        SetComposerLevel(composer, ElementBounds.Fixed(0.0, 225.0, 385.0, 200.0), GetLevelByLevelName("Hammer"));
        // Sword
        composer.AddImage(ElementBounds.Fixed(150.0, 225.0, 385.0, 200.0), new AssetLocation("levelup:unknown.png"));
        SetComposerLevel(composer, ElementBounds.Fixed(150.0, 225.0, 385.0, 200.0), GetLevelByLevelName("Sword"));
        // Shield
        composer.AddImage(ElementBounds.Fixed(300.0, 225.0, 385.0, 200.0), new AssetLocation("levelup:unknown.png"));
        SetComposerLevel(composer, ElementBounds.Fixed(300.0, 225.0, 385.0, 200.0), GetLevelByLevelName("Shield"));
        #endregion
    }

    private int GetLevelByLevelName(string levelName) => instance.api.World.Player.Entity.WatchedAttributes.GetInt($"LevelUP_Level_{levelName}");

    private static void SetComposerLevel(GuiComposer composer, ElementBounds position, int level)
    {
        // Single Digits
        if (level < 10)
            // Creating the imagem
            composer.AddImage(position, new AssetLocation($"levelup:{level}.png"));
        // Double Digits
        else if (level < 100)
        {
            string levelStr = level.ToString();
            int firstDigit = int.Parse(levelStr[0].ToString());
            int secondDigit = int.Parse(levelStr[1].ToString());

            // First digit instanciation
            ElementBounds firstDigitPosition = ElementBounds.Fixed(
                position.fixedX - 15,
                position.fixedY,
                position.fixedWidth,
                position.fixedHeight
            );
            composer.AddImage(firstDigitPosition, new AssetLocation($"levelup:{firstDigit}.png"));

            // Second digit instanciation
            ElementBounds secondDigitPosition = ElementBounds.Fixed(
                position.fixedX + 15,
                position.fixedY,
                position.fixedWidth,
                position.fixedHeight
            );
            composer.AddImage(secondDigitPosition, new AssetLocation($"levelup:{secondDigit}.png"));
        }

        Debug.Log("Ops, you reached the gui limit :P            ");
    }
    
    private bool OnBackClicked()
    {
        page--;
        if (page < 0) page = 0;
        return true;
    }

    private bool OnNextClicked()
    {
        page++;
        if (page > 1) page = 1;
        return true;
    }
}