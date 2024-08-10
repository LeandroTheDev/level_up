using System;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;

namespace LevelUP.Client;

class CharacterView
{
    Instance instance;

    GuiDialogCharacterBase characterView;
    private GuiElementContainer levelContainer;

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
                Name = Lang.Get("levelup:levels_tab"),
                DataInt = characterView.Tabs.Count
            });
            characterView.RenderTabHandlers.Add(ComposeLevelTab);
            Debug.Log("Level Tab created in character view");
        });
    }

    private void ComposeLevelTab(GuiComposer composer)
    {
        // Bounds instanciation
        ElementBounds insetBounds;
        ElementBounds leftColumn = ElementBounds.Fixed(0.0, 0.0, 300.0, 30.0);
        ElementBounds clippingBounds;
        ElementBounds listBounds;
        ElementBounds containerBounds = ElementBounds.Fixed(0.0, 10.0, 82.0, 82);
        containerBounds = containerBounds.FlatCopy();
        double insetWidth = Math.Max(350.0, 350 * 0.5);
        double insetHeight = Math.Max(325.0, 425 - 160.0);

        // Creating the dark background
        composer.AddInset(insetBounds = leftColumn.BelowCopy(0.0, -3.0).WithFixedSize(insetWidth, insetHeight - leftColumn.fixedY - leftColumn.fixedHeight));
        // Adding a scrollbar to it in the left side
        composer.AddVerticalScrollbar(OnNewScrollbarValue, ElementStdBounds.VerticalScrollbar(insetBounds), "LevelUP_Scrollbar");
        // Creating the Clip for the Scrollbar
        composer.BeginClip(clippingBounds = insetBounds.ForkContainingChild(10.0, 0.0, 10.0, 10.0));
        // Creating the container to add items in clip
        composer.AddContainer(listBounds = clippingBounds.ForkContainingChild(10.0, 10.0, 10.0, 10.0), "LevelUP_Levels_List");
        levelContainer = composer.GetContainer("LevelUP_Levels_List");

        // Adding the levels images and texts
        #region axe
        levelContainer.Add(new GuiElementImage(instance.api, containerBounds, new AssetLocation("levelup:axe.png")));
        levelContainer.Add(new GuiElementStaticText(instance.api, $"{Lang.Get("levelup:axe")}: {GetLevelByLevelName("Axe")}", EnumTextOrientation.Left, containerBounds.RightCopy().ForkChildOffseted(0, 25, 500, 0), CairoFont.WhiteSmallText()));
        #endregion

        #region pickaxe
        levelContainer.Add(new GuiElementImage(instance.api, containerBounds = containerBounds.BelowCopy(), new AssetLocation("levelup:pickaxe.png")));
        levelContainer.Add(new GuiElementStaticText(instance.api, $"{Lang.Get("levelup:pickaxe")}: {GetLevelByLevelName("Pickaxe")}", EnumTextOrientation.Left, containerBounds.RightCopy().ForkChildOffseted(0, 25, 500, 0), CairoFont.WhiteSmallText()));
        #endregion

        #region shovel
        levelContainer.Add(new GuiElementImage(instance.api, containerBounds = containerBounds.BelowCopy(), new AssetLocation("levelup:shovel.png")));
        levelContainer.Add(new GuiElementStaticText(instance.api, $"{Lang.Get("levelup:shovel")}: {GetLevelByLevelName("Shovel")}", EnumTextOrientation.Left, containerBounds.RightCopy().ForkChildOffseted(0, 25, 500, 0), CairoFont.WhiteSmallText()));
        #endregion

        #region knife
        levelContainer.Add(new GuiElementImage(instance.api, containerBounds = containerBounds.BelowCopy(), new AssetLocation("levelup:knife.png")));
        levelContainer.Add(new GuiElementStaticText(instance.api, $"{Lang.Get("levelup:knife")}: {GetLevelByLevelName("Knife")}", EnumTextOrientation.Left, containerBounds.RightCopy().ForkChildOffseted(0, 25, 500, 0), CairoFont.WhiteSmallText()));
        #endregion

        #region bow
        levelContainer.Add(new GuiElementImage(instance.api, containerBounds = containerBounds.BelowCopy(), new AssetLocation("levelup:bow.png")));
        levelContainer.Add(new GuiElementStaticText(instance.api, $"{Lang.Get("levelup:bow")}: {GetLevelByLevelName("Bow")}", EnumTextOrientation.Left, containerBounds.RightCopy().ForkChildOffseted(0, 25, 500, 0), CairoFont.WhiteSmallText()));
        #endregion

        #region spear
        levelContainer.Add(new GuiElementImage(instance.api, containerBounds = containerBounds.BelowCopy(), new AssetLocation("levelup:spear.png")));
        levelContainer.Add(new GuiElementStaticText(instance.api, $"{Lang.Get("levelup:spear")}: {GetLevelByLevelName("Spear")}", EnumTextOrientation.Left, containerBounds.RightCopy().ForkChildOffseted(0, 25, 500, 0), CairoFont.WhiteSmallText()));
        #endregion

        #region hammer
        levelContainer.Add(new GuiElementImage(instance.api, containerBounds = containerBounds.BelowCopy(), new AssetLocation("levelup:hammer.png")));
        levelContainer.Add(new GuiElementStaticText(instance.api, $"{Lang.Get("levelup:hammer")}: {GetLevelByLevelName("Hammer")}", EnumTextOrientation.Left, containerBounds.RightCopy().ForkChildOffseted(0, 25, 500, 0), CairoFont.WhiteSmallText()));
        #endregion

        #region sword
        levelContainer.Add(new GuiElementImage(instance.api, containerBounds = containerBounds.BelowCopy(), new AssetLocation("levelup:sword.png")));
        levelContainer.Add(new GuiElementStaticText(instance.api, $"{Lang.Get("levelup:sword")}: {GetLevelByLevelName("Sword")}", EnumTextOrientation.Left, containerBounds.RightCopy().ForkChildOffseted(0, 25, 500, 0), CairoFont.WhiteSmallText()));
        #endregion

        #region shield
        levelContainer.Add(new GuiElementImage(instance.api, containerBounds = containerBounds.BelowCopy(), new AssetLocation("levelup:shield.png")));
        levelContainer.Add(new GuiElementStaticText(instance.api, $"{Lang.Get("levelup:shield")}: {GetLevelByLevelName("Shield")}", EnumTextOrientation.Left, containerBounds.RightCopy().ForkChildOffseted(0, 25, 500, 0), CairoFont.WhiteSmallText()));
        #endregion

        #region hand
        levelContainer.Add(new GuiElementImage(instance.api, containerBounds = containerBounds.BelowCopy(), new AssetLocation("levelup:hand.png")));
        levelContainer.Add(new GuiElementStaticText(instance.api, $"{Lang.Get("levelup:hand")}: {GetLevelByLevelName("Hand")}", EnumTextOrientation.Left, containerBounds.RightCopy().ForkChildOffseted(0, 25, 500, 0), CairoFont.WhiteSmallText()));
        #endregion

        #region farming
        levelContainer.Add(new GuiElementImage(instance.api, containerBounds = containerBounds.BelowCopy(), new AssetLocation("levelup:farming.png")));
        levelContainer.Add(new GuiElementStaticText(instance.api, $"{Lang.Get("levelup:farming")}: {GetLevelByLevelName("Farming")}", EnumTextOrientation.Left, containerBounds.RightCopy().ForkChildOffseted(0, 25, 500, 0), CairoFont.WhiteSmallText()));
        #endregion

        #region cooking
        levelContainer.Add(new GuiElementImage(instance.api, containerBounds = containerBounds.BelowCopy(), new AssetLocation("levelup:cooking.png")));
        levelContainer.Add(new GuiElementStaticText(instance.api, $"{Lang.Get("levelup:cooking")}: {GetLevelByLevelName("Cooking")}", EnumTextOrientation.Left, containerBounds.RightCopy().ForkChildOffseted(0, 25, 500, 0), CairoFont.WhiteSmallText()));
        #endregion

        #region panning
        levelContainer.Add(new GuiElementImage(instance.api, containerBounds = containerBounds.BelowCopy(), new AssetLocation("levelup:panning.png")));
        levelContainer.Add(new GuiElementStaticText(instance.api, $"{Lang.Get("levelup:panning")}: {GetLevelByLevelName("Panning")}", EnumTextOrientation.Left, containerBounds.RightCopy().ForkChildOffseted(0, 25, 500, 0), CairoFont.WhiteSmallText()));
        #endregion

        #region vitality
        levelContainer.Add(new GuiElementImage(instance.api, containerBounds = containerBounds.BelowCopy(), new AssetLocation("levelup:vitality.png")));
        levelContainer.Add(new GuiElementStaticText(instance.api, $"{Lang.Get("levelup:vitality")}: {GetLevelByLevelName("Vitality")}", EnumTextOrientation.Left, containerBounds.RightCopy().ForkChildOffseted(0, 25, 500, 0), CairoFont.WhiteSmallText()));
        #endregion

        #region leatherarmor
        levelContainer.Add(new GuiElementImage(instance.api, containerBounds = containerBounds.BelowCopy(), new AssetLocation("levelup:leather_armor.png")));
        levelContainer.Add(new GuiElementStaticText(instance.api, $"{Lang.Get("levelup:leather_armor")}: {GetLevelByLevelName("LeatherArmor")}", EnumTextOrientation.Left, containerBounds.RightCopy().ForkChildOffseted(0, 25, 500, 0), CairoFont.WhiteSmallText()));
        #endregion

        #region chainarmor
        levelContainer.Add(new GuiElementImage(instance.api, containerBounds = containerBounds.BelowCopy(), new AssetLocation("levelup:chain_armor.png")));
        levelContainer.Add(new GuiElementStaticText(instance.api, $"{Lang.Get("levelup:chain_armor")}: {GetLevelByLevelName("ChainArmor")}", EnumTextOrientation.Left, containerBounds.RightCopy().ForkChildOffseted(0, 25, 500, 0), CairoFont.WhiteSmallText()));
        #endregion

        // Finishing Clip for scrollbar
        composer.EndClip();

        // Adding the size of scroll button
        GuiElementScrollbar scrollBar = composer.GetScrollbar("LevelUP_Scrollbar");
        scrollBar.SetHeights((float)containerBounds.fixedHeight, (float)(clippingBounds.fixedHeight * 3.9));
    }

    private void OnNewScrollbarValue(float value)
    {
        levelContainer.Bounds.fixedY = 0f - value;
        levelContainer.Bounds.CalcWorldBounds();
    }

    private int GetLevelByLevelName(string levelName) => instance.api.World.Player.Entity.WatchedAttributes.GetInt($"LevelUP_Level_{levelName}");
}