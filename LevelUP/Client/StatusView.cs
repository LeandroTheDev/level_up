using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;

namespace LevelUP.Client;

public class StatusViewDialog(ICoreClientAPI capi, string levelType, ElementBounds buttonBounds) : GuiDialog(capi)
{
    public string levelType = levelType;
    public override string ToggleKeyCombinationCode => "levelinfo";

    private GuiElementContainer _textContainer;

    public override void OnGuiOpened()
    {
        base.OnGuiOpened();

        StringBuilder stringBuilder = StatusViewEvents.GetExternalStringBuilder(capi.World.Player, new(), levelType);
        string[] lines = stringBuilder.ToString().Split('\n');

        double viewHeight = 455.0;
        double lineHeight = 20.0;

        ElementBounds windowBounds = ElementBounds.Fixed(
            buttonBounds.absX + 60,
            buttonBounds.absY,
            420,
            500
        );

        ElementBounds clipBounds = ElementBounds.Fixed(10, 35, 370, viewHeight);
        ElementBounds listBounds = clipBounds.ForkContainingChild(5, 5, 5, 5);

        GuiComposer composer = capi.Gui.CreateCompo("levelinfo", windowBounds);
        composer.AddShadedDialogBG(ElementBounds.Fill);
        composer.AddDialogTitleBar(Lang.Get("levelup:status_tab", Lang.Get($"levelup:{levelType.ToLower()}")), OnClose);
        composer.AddVerticalScrollbar(OnNewScrollbarValue, ElementStdBounds.VerticalScrollbar(clipBounds), "statusscroll");
        composer.BeginClip(clipBounds);
        composer.AddContainer(listBounds, "statuscontent");
        _textContainer = composer.GetContainer("statuscontent");

        double offsetY = 0;
        foreach (string line in lines)
        {
            ElementBounds lineBounds = listBounds.ForkChild()
                .WithFixedPosition(0, offsetY)
                .WithFixedSize(355, lineHeight);

            _textContainer.Add(new GuiElementStaticText(
                capi,
                line,
                EnumTextOrientation.Left,
                lineBounds,
                CairoFont.WhiteSmallText()
            ));

            offsetY += lineHeight;
        }

        composer.EndClip();
        SingleComposer = composer.Compose();

        double totalHeight = offsetY;
        capi.Event.EnqueueMainThreadTask(() =>
        {
            SingleComposer.GetScrollbar("statusscroll").SetHeights((float)viewHeight, (float)totalHeight);
        }, "StatusScrollInit");
    }

    private void OnNewScrollbarValue(float value)
    {
        if (_textContainer == null) return;
        _textContainer.Bounds.fixedY = 0f - value;
        _textContainer.Bounds.CalcWorldBounds();
    }

    private void OnClose()
    {
        TryClose();
    }
}

public class StatusViewEvents
{
    public delegate void PlayerStringBuilder(IPlayer player, ref StringBuilder stringBuilder, string levelType);

    public static event PlayerStringBuilder OnStatusRequested;

    internal static StringBuilder GetExternalStringBuilder(IPlayer player, StringBuilder stringBuilder, string levelType)
    {
        OnStatusRequested?.Invoke(player, ref stringBuilder, levelType);
        return stringBuilder;
    }
}