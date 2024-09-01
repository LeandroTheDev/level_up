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
        // Quantity of levels added to the list
        int levelsAdded = 0;
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
        // Checking if enabled levels has been loaded
        if (instance.enabledLevels.Count == 0) return;
        // Creating the Clip for the Scrollbar
        composer.BeginClip(clippingBounds = insetBounds.ForkContainingChild(10.0, 0.0, 10.0, 10.0));
        // Creating the container to add items in clip
        composer.AddContainer(listBounds = clippingBounds.ForkContainingChild(10.0, 10.0, 10.0, 10.0), "LevelUP_Levels_List");
        levelContainer = composer.GetContainer("LevelUP_Levels_List");

        // Adding the levels images and texts
        #region axe
        if (instance.enabledLevels["Axe"])
        {
            levelsAdded++;
            levelContainer.Add(new GuiElementImage(instance.api, containerBounds, new AssetLocation("levelup:axe.png")));
            levelContainer.Add(new GuiElementStaticText(instance.api, $"{Lang.Get("levelup:axe")}: {GetLevelByLevelName("Axe")}", EnumTextOrientation.Left, containerBounds.RightCopy().ForkChildOffseted(0, 25, 500, 0), CairoFont.WhiteSmallText()));
        }
        #endregion

        #region pickaxe
        if (instance.enabledLevels["Pickaxe"])
        {
            levelsAdded++;
            levelContainer.Add(new GuiElementImage(instance.api, containerBounds = containerBounds.BelowCopy(), new AssetLocation("levelup:pickaxe.png")));
            levelContainer.Add(new GuiElementStaticText(instance.api, $"{Lang.Get("levelup:pickaxe")}: {GetLevelByLevelName("Pickaxe")}", EnumTextOrientation.Left, containerBounds.RightCopy().ForkChildOffseted(0, 25, 500, 0), CairoFont.WhiteSmallText()));
        }
        #endregion

        #region shovel
        if (instance.enabledLevels["Shovel"])
        {
            levelsAdded++;
            levelContainer.Add(new GuiElementImage(instance.api, containerBounds = containerBounds.BelowCopy(), new AssetLocation("levelup:shovel.png")));
            levelContainer.Add(new GuiElementStaticText(instance.api, $"{Lang.Get("levelup:shovel")}: {GetLevelByLevelName("Shovel")}", EnumTextOrientation.Left, containerBounds.RightCopy().ForkChildOffseted(0, 25, 500, 0), CairoFont.WhiteSmallText()));
        }
        #endregion

        #region knife
        if (instance.enabledLevels["Knife"])
        {
            levelsAdded++;
            levelContainer.Add(new GuiElementImage(instance.api, containerBounds = containerBounds.BelowCopy(), new AssetLocation("levelup:knife.png")));
            levelContainer.Add(new GuiElementStaticText(instance.api, $"{Lang.Get("levelup:knife")}: {GetLevelByLevelName("Knife")}", EnumTextOrientation.Left, containerBounds.RightCopy().ForkChildOffseted(0, 25, 500, 0), CairoFont.WhiteSmallText()));
        }
        #endregion

        #region bow
        if (instance.enabledLevels["Bow"])
        {
            levelsAdded++;
            levelContainer.Add(new GuiElementImage(instance.api, containerBounds = containerBounds.BelowCopy(), new AssetLocation("levelup:bow.png")));
            levelContainer.Add(new GuiElementStaticText(instance.api, $"{Lang.Get("levelup:bow")}: {GetLevelByLevelName("Bow")}", EnumTextOrientation.Left, containerBounds.RightCopy().ForkChildOffseted(0, 25, 500, 0), CairoFont.WhiteSmallText()));
        }
        #endregion

        #region spear
        if (instance.enabledLevels["Spear"])
        {
            levelsAdded++;
            levelContainer.Add(new GuiElementImage(instance.api, containerBounds = containerBounds.BelowCopy(), new AssetLocation("levelup:spear.png")));
            levelContainer.Add(new GuiElementStaticText(instance.api, $"{Lang.Get("levelup:spear")}: {GetLevelByLevelName("Spear")}", EnumTextOrientation.Left, containerBounds.RightCopy().ForkChildOffseted(0, 25, 500, 0), CairoFont.WhiteSmallText()));
        }
        #endregion

        #region hammer
        if (instance.enabledLevels["Hammer"])
        {
            levelsAdded++;
            levelContainer.Add(new GuiElementImage(instance.api, containerBounds = containerBounds.BelowCopy(), new AssetLocation("levelup:hammer.png")));
            levelContainer.Add(new GuiElementStaticText(instance.api, $"{Lang.Get("levelup:hammer")}: {GetLevelByLevelName("Hammer")}", EnumTextOrientation.Left, containerBounds.RightCopy().ForkChildOffseted(0, 25, 500, 0), CairoFont.WhiteSmallText()));
        }
        #endregion

        #region sword
        if (instance.enabledLevels["Sword"])
        {
            levelsAdded++;
            levelContainer.Add(new GuiElementImage(instance.api, containerBounds = containerBounds.BelowCopy(), new AssetLocation("levelup:sword.png")));
            levelContainer.Add(new GuiElementStaticText(instance.api, $"{Lang.Get("levelup:sword")}: {GetLevelByLevelName("Sword")}", EnumTextOrientation.Left, containerBounds.RightCopy().ForkChildOffseted(0, 25, 500, 0), CairoFont.WhiteSmallText()));
        }
        #endregion

        #region shield
        if (instance.enabledLevels["Shield"])
        {
            levelsAdded++;
            levelContainer.Add(new GuiElementImage(instance.api, containerBounds = containerBounds.BelowCopy(), new AssetLocation("levelup:shield.png")));
            levelContainer.Add(new GuiElementStaticText(instance.api, $"{Lang.Get("levelup:shield")}: {GetLevelByLevelName("Shield")}", EnumTextOrientation.Left, containerBounds.RightCopy().ForkChildOffseted(0, 25, 500, 0), CairoFont.WhiteSmallText()));
        }
        #endregion

        #region hand
        if (instance.enabledLevels["Hand"])
        {
            levelsAdded++;
            levelContainer.Add(new GuiElementImage(instance.api, containerBounds = containerBounds.BelowCopy(), new AssetLocation("levelup:hand.png")));
            levelContainer.Add(new GuiElementStaticText(instance.api, $"{Lang.Get("levelup:hand")}: {GetLevelByLevelName("Hand")}", EnumTextOrientation.Left, containerBounds.RightCopy().ForkChildOffseted(0, 25, 500, 0), CairoFont.WhiteSmallText()));
        }
        #endregion

        #region hunter
        if (instance.enabledLevels["Hunter"])
        {
            levelsAdded++;
            levelContainer.Add(new GuiElementImage(instance.api, containerBounds = containerBounds.BelowCopy(), new AssetLocation("levelup:hunter.png")));
            levelContainer.Add(new GuiElementStaticText(instance.api, $"{Lang.Get("levelup:hunter")}: {GetLevelByLevelName("Hunter")}", EnumTextOrientation.Left, containerBounds.RightCopy().ForkChildOffseted(0, 25, 500, 0), CairoFont.WhiteSmallText()));
        }
        #endregion

        #region farming
        if (instance.enabledLevels["Farming"])
        {
            levelsAdded++;
            levelContainer.Add(new GuiElementImage(instance.api, containerBounds = containerBounds.BelowCopy(), new AssetLocation("levelup:farming.png")));
            levelContainer.Add(new GuiElementStaticText(instance.api, $"{Lang.Get("levelup:farming")}: {GetLevelByLevelName("Farming")}", EnumTextOrientation.Left, containerBounds.RightCopy().ForkChildOffseted(0, 25, 500, 0), CairoFont.WhiteSmallText()));
        }
        #endregion

        #region cooking
        if (instance.enabledLevels["Cooking"])
        {
            levelsAdded++;
            levelContainer.Add(new GuiElementImage(instance.api, containerBounds = containerBounds.BelowCopy(), new AssetLocation("levelup:cooking.png")));
            levelContainer.Add(new GuiElementStaticText(instance.api, $"{Lang.Get("levelup:cooking")}: {GetLevelByLevelName("Cooking")}", EnumTextOrientation.Left, containerBounds.RightCopy().ForkChildOffseted(0, 25, 500, 0), CairoFont.WhiteSmallText()));
        }
        #endregion

        #region panning
        if (instance.enabledLevels["Panning"])
        {
            levelsAdded++;
            levelContainer.Add(new GuiElementImage(instance.api, containerBounds = containerBounds.BelowCopy(), new AssetLocation("levelup:panning.png")));
            levelContainer.Add(new GuiElementStaticText(instance.api, $"{Lang.Get("levelup:panning")}: {GetLevelByLevelName("Panning")}", EnumTextOrientation.Left, containerBounds.RightCopy().ForkChildOffseted(0, 25, 500, 0), CairoFont.WhiteSmallText()));
        }
        #endregion

        #region vitality
        if (instance.enabledLevels["Vitality"])
        {
            levelsAdded++;
            levelContainer.Add(new GuiElementImage(instance.api, containerBounds = containerBounds.BelowCopy(), new AssetLocation("levelup:vitality.png")));
            levelContainer.Add(new GuiElementStaticText(instance.api, $"{Lang.Get("levelup:vitality")}: {GetLevelByLevelName("Vitality")}", EnumTextOrientation.Left, containerBounds.RightCopy().ForkChildOffseted(0, 25, 500, 0), CairoFont.WhiteSmallText()));
        }
        #endregion

        #region leatherarmor
        if (instance.enabledLevels["LeatherArmor"])
        {
            levelsAdded++;
            levelContainer.Add(new GuiElementImage(instance.api, containerBounds = containerBounds.BelowCopy(), new AssetLocation("levelup:leatherarmor.png")));
            levelContainer.Add(new GuiElementStaticText(instance.api, $"{Lang.Get("levelup:leatherarmor")}: {GetLevelByLevelName("LeatherArmor")}", EnumTextOrientation.Left, containerBounds.RightCopy().ForkChildOffseted(0, 25, 500, 0), CairoFont.WhiteSmallText()));
        }
        #endregion

        #region chainarmor
        if (instance.enabledLevels["ChainArmor"])
        {
            levelsAdded++;
            levelContainer.Add(new GuiElementImage(instance.api, containerBounds = containerBounds.BelowCopy(), new AssetLocation("levelup:chainarmor.png")));
            levelContainer.Add(new GuiElementStaticText(instance.api, $"{Lang.Get("levelup:chainarmor")}: {GetLevelByLevelName("ChainArmor")}", EnumTextOrientation.Left, containerBounds.RightCopy().ForkChildOffseted(0, 25, 500, 0), CairoFont.WhiteSmallText()));
        }
        #endregion
        
        #region brigandinearmor
        if (instance.enabledLevels["BrigandineArmor"])
        {
            levelsAdded++;
            levelContainer.Add(new GuiElementImage(instance.api, containerBounds = containerBounds.BelowCopy(), new AssetLocation("levelup:brigandinearmor.png")));
            levelContainer.Add(new GuiElementStaticText(instance.api, $"{Lang.Get("levelup:brigandinearmor")}: {GetLevelByLevelName("BrigandineArmor")}", EnumTextOrientation.Left, containerBounds.RightCopy().ForkChildOffseted(0, 25, 500, 0), CairoFont.WhiteSmallText()));
        }
        #endregion
        
        #region platearmor
        if (instance.enabledLevels["PlateArmor"])
        {
            levelsAdded++;
            levelContainer.Add(new GuiElementImage(instance.api, containerBounds = containerBounds.BelowCopy(), new AssetLocation("levelup:platearmor.png")));
            levelContainer.Add(new GuiElementStaticText(instance.api, $"{Lang.Get("levelup:platearmor")}: {GetLevelByLevelName("PlateArmor")}", EnumTextOrientation.Left, containerBounds.RightCopy().ForkChildOffseted(0, 25, 500, 0), CairoFont.WhiteSmallText()));
        }
        #endregion
        
        #region scale
        if (instance.enabledLevels["ScaleArmor"])
        {
            levelsAdded++;
            levelContainer.Add(new GuiElementImage(instance.api, containerBounds = containerBounds.BelowCopy(), new AssetLocation("levelup:scalearmor.png")));
            levelContainer.Add(new GuiElementStaticText(instance.api, $"{Lang.Get("levelup:scalearmor")}: {GetLevelByLevelName("ScaleArmor")}", EnumTextOrientation.Left, containerBounds.RightCopy().ForkChildOffseted(0, 25, 500, 0), CairoFont.WhiteSmallText()));
        }
        #endregion

        // Finishing Clip for scrollbar
        composer.EndClip();

        // Adding the size of scroll button
        GuiElementScrollbar scrollBar = composer.GetScrollbar("LevelUP_Scrollbar");
        double listHeight = 0;
        for (int i = 0; i < levelsAdded; i++) listHeight += 72  ;
        scrollBar.SetHeights((float)containerBounds.fixedHeight, (float)listHeight);
    }

    private void OnNewScrollbarValue(float value)
    {
        if (levelContainer != null)
        {
            levelContainer.Bounds.fixedY = 0f - value;
            levelContainer.Bounds.CalcWorldBounds();
        }
    }

    private int GetLevelByLevelName(string levelName) => instance.api.World.Player.Entity.WatchedAttributes.GetInt($"LevelUP_Level_{levelName}");
}