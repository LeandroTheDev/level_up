using System;
using System.Collections.Generic;
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
        foreach (KeyValuePair<string, bool> keyValuePair in instance.enabledLevels)
        {
            if (!keyValuePair.Value) continue;

            string levelType = keyValuePair.Key;
            levelContainer.Add(new GuiElementImage(instance.api, containerBounds, new AssetLocation($"levelup:{levelType.ToLower()}.png")));
            levelContainer.Add(new GuiElementStaticText(instance.api, $"{Lang.Get($"levelup:{levelType.ToLower()}")}: {GetLevelByLevelName(levelType)}", EnumTextOrientation.Left, containerBounds.RightCopy().ForkChildOffseted(0, 12, 500, 0), CairoFont.WhiteSmallText()));
            levelContainer.Add(new GuiElementStaticText(instance.api, Lang.Get("levelup:progress", GetEXPRemainingByLevelName(levelType)), EnumTextOrientation.Left, containerBounds.RightCopy().ForkChildOffseted(0, 28, 500, 0), CairoFont.WhiteSmallishText().WithFontSize(13)));
        }

        // Finishing Clip for scrollbar
        composer.EndClip();

        int increaser = 0;
        if (levelsAdded > 18) increaser = 72;
        else if (levelsAdded > 16) increaser = 70;
        else if (levelsAdded > 14) increaser = 68;
        else if (levelsAdded > 12) increaser = 66;
        else if (levelsAdded > 10) increaser = 66;
        else if (levelsAdded > 8) increaser = 64;
        else if (levelsAdded > 6) increaser = 64;
        else if (levelsAdded > 4) increaser = 58;
        else if (levelsAdded > 2) increaser = 58;
        else increaser = 54;

        // Adding the size of scroll button
        GuiElementScrollbar scrollBar = composer.GetScrollbar("LevelUP_Scrollbar");
        double listHeight = 0;
        for (int i = 0; i < levelsAdded; i++) listHeight += increaser;
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

    private float GetEXPRemainingByLevelName(string levelName) =>
        MathF.Round(instance.api.World.Player.Entity.WatchedAttributes.GetFloat($"LevelUP_Level_{levelName}_RemainingNextLevelPercentage"), 2);
}