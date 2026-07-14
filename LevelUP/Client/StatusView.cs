using System;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;

namespace LevelUP.Client;

public class StatusViewDialog(ICoreClientAPI capi, string levelType, ElementBounds buttonBounds) : GuiDialog(capi)
{
    public string levelType = levelType;
    public override string ToggleKeyCombinationCode => "levelinfo";

    private ElementBounds _textBounds;

    public override void OnGuiOpened()
    {
        base.OnGuiOpened();

        StringBuilder stringBuilder = StatusViewEvents.GetExternalStringBuilder(capi.World.Player, new(), levelType);
        string text = stringBuilder.ToString();

        double viewHeight = 455.0;
        double lineHeight = 20.0;
        double textTotalHeight = Math.Max(text.Split('\n').Length * lineHeight, viewHeight);

        ElementBounds windowBounds = ElementBounds.Fixed(
            buttonBounds.absX + 60,
            buttonBounds.absY,
            420,
            500
        );

        ElementBounds bg = ElementBounds.Fill;
        ElementBounds clipBounds = ElementBounds.Fixed(10, 35, 370, viewHeight);
        _textBounds = ElementBounds.Fixed(0, 0, 360, textTotalHeight);

        SingleComposer = capi.Gui
            .CreateCompo("levelinfo", windowBounds)
            .AddShadedDialogBG(bg)
            .AddDialogTitleBar(Lang.Get("levelup:status_tab", Lang.Get($"levelup:{levelType.ToLower()}")), OnClose)
            .BeginClip(clipBounds)
            .AddStaticText(text, CairoFont.WhiteSmallText(), _textBounds)
            .EndClip()
            .AddVerticalScrollbar(OnNewScrollbarValue, ElementStdBounds.VerticalScrollbar(clipBounds), "statusscroll")
            .Compose();

        capi.Event.EnqueueMainThreadTask(() =>
        {
            SingleComposer.GetScrollbar("statusscroll").SetHeights((float)viewHeight, (float)textTotalHeight);
        }, "StatusScrollInit");
    }

    private void OnNewScrollbarValue(float value)
    {
        _textBounds.fixedY = 3 - value;
        _textBounds.CalcWorldBounds();
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