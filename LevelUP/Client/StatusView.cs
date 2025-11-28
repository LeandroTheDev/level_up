using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;

namespace LevelUP.Client;

public class StatusViewDialog(ICoreClientAPI capi, string levelType) : GuiDialog(capi)
{
    public override string ToggleKeyCombinationCode => "levelinfo";

    public override void OnGuiOpened()
    {
        base.OnGuiOpened();

        ElementBounds bg = ElementBounds.Fixed(0, 0, 300, 150);

        ElementBounds textBounds = ElementBounds.Fixed(10, 30, 280, 100);

        StringBuilder stringBuilder = StatusViewEvents.GetExternalStringBuilder(capi.World.Player, new(), levelType);

        SingleComposer = capi.Gui
            .CreateCompo("levelinfo", bg)
            .AddShadedDialogBG(bg)
            .AddDialogTitleBar(Lang.Get("levelup:status_tab", Lang.Get($"levelup:{levelType.ToLower()}")), OnClose)
            .AddStaticText(stringBuilder.ToString(), CairoFont.WhiteSmallText(), textBounds)
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