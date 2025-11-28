using System;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;

namespace LevelUP.Client;

class LevelsView
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
            characterView = instance.api.Gui.LoadedGuis.Find(dlg => dlg is GuiDialogCharacterBase) as GuiDialogCharacterBase;
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
        // Checking if enabled levels has been loaded
        if (instance.enabledLevels.Count == 0) return;
        // Creating the Clip for the Scrollbar
        composer.BeginClip(clippingBounds = insetBounds.ForkContainingChild(10.0, 0.0, 10.0, 10.0));
        // Creating the container to add items in clip
        composer.AddContainer(listBounds = clippingBounds.ForkContainingChild(10.0, 10.0, 10.0, 10.0), "LevelUP_Levels_List");
        levelContainer = composer.GetContainer("LevelUP_Levels_List");

        // Adding the levels images and texts
        double offsetY = 0;
        double itemHeight = 72;

        foreach (var keyValuePair in instance.enabledLevels)
        {
            if (!keyValuePair.Value) continue;

            string levelType = keyValuePair.Key;

            AssetLocation imageAsset = new($"levelup:textures/{levelType.ToLower()}.png");

            if (instance.api.Assets.Exists(imageAsset))
            {
                // Create bounds for actual line
                ElementBounds itemBounds = containerBounds.FlatCopy().WithFixedOffset(0, offsetY);

                levelContainer.Add(new GuiElementImage(instance.api, itemBounds, imageAsset));

                levelContainer.Add(new GuiElementStaticText(
                    instance.api,
                    $"{Lang.Get($"levelup:{levelType.ToLower()}")}: {GetLevelByLevelName(levelType)}",
                    EnumTextOrientation.Left,
                    itemBounds.RightCopy().ForkChildOffseted(0, 12, 500, 0),
                    CairoFont.WhiteSmallText()
                ));

                levelContainer.Add(new GuiElementStaticText(
                    instance.api,
                    Lang.Get("levelup:progress", GetEXPRemainingByLevelName(levelType)),
                    EnumTextOrientation.Left,
                    itemBounds.RightCopy().ForkChildOffseted(0, 30, 500, 0),
                    CairoFont.WhiteSmallishText().WithFontSize(13)
                ));

                // levelContainer.Add(new GuiElementTextButton(
                //     instance.api,
                //     ">",
                //     CairoFont.ButtonText(),
                //     CairoFont.ButtonPressedText(),
                //     () => OnButtonClick(levelType),
                //     ElementBounds.FixedSize(60.0, 30.0).WithFixedPadding(10.0, 2.0),
                //     EnumButtonStyle.Small
                // ));

                offsetY += itemHeight;
            }
        }

        // Finishing Clip for scrollbar
        composer.EndClip();

        instance.api.Event.EnqueueMainThreadTask(() =>
        {
            // Adding the size of scroll button
            GuiElementScrollbar scrollBar = composer.GetScrollbar("LevelUP_Scrollbar");
            scrollBar.SetHeights((float)clippingBounds.fixedHeight, (float)offsetY);
        }, "FixScrollInit");
    }

    private bool OnButtonClick(string levelType)
    {
        Console.WriteLine("FKSMIOAPFJKMSAOPFK");
        string msg = $"Informações sobre o nível {levelType}\n\nSeu nível atual: {GetLevelByLevelName(levelType)}";
        var dialog = new DialogLevelInfo(instance.api, msg);
        dialog.TryOpen();
        return true;
    }

    private void OnNewScrollbarValue(float value)
    {
        if (levelContainer != null)
        {
            levelContainer.Bounds.fixedY = 0f - value;
            levelContainer.Bounds.CalcWorldBounds();
        }
    }

    private int GetLevelByLevelName(string levelName)
        => instance.api.World.Player.Entity.WatchedAttributes.GetInt($"LevelUP_Level_{levelName}");

    private float GetEXPRemainingByLevelName(string levelName)
        => MathF.Round(instance.api.World.Player.Entity.WatchedAttributes.GetFloat($"LevelUP_Level_{levelName}_RemainingNextLevelPercentage"), 2);
}