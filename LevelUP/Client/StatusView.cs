using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;

namespace LevelUP.Client;

public class StatusViewDialog(ICoreClientAPI capi, string levelType, ElementBounds buttonBounds) : GuiDialog(capi)
{
    public string levelType = levelType;
    public override string ToggleKeyCombinationCode => "levelinfo";

    public override void OnGuiOpened()
    {
        base.OnGuiOpened();
        ElementBounds windowBounds = ElementBounds.Fixed(
            buttonBounds.absX + 60,
            buttonBounds.absY,
            400,
            500
        );

        ElementBounds bg = ElementBounds.Fill;

        ElementBounds contentBounds = ElementBounds.Fixed(10, 35, 380, 455);

        StringBuilder stringBuilder = StatusViewEvents.GetExternalStringBuilder(capi.World.Player, new(), levelType);

        SingleComposer = capi.Gui
            .CreateCompo("levelinfo", windowBounds)
            .AddShadedDialogBG(bg)
            .AddDialogTitleBar(Lang.Get("levelup:status_tab", Lang.Get($"levelup:{levelType.ToLower()}")), OnClose)
            .AddStaticText(stringBuilder.ToString(), CairoFont.WhiteSmallText(), contentBounds)
            .Compose();
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