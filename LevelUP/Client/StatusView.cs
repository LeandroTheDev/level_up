using Vintagestory.API.Client;

public class DialogLevelInfo : GuiDialog
{
    private string message;

    public DialogLevelInfo(ICoreClientAPI capi, string message) : base(capi)
    {
        this.message = message;
    }

    public override string ToggleKeyCombinationCode => "levelinfo";

    public override void OnGuiOpened()
    {
        base.OnGuiOpened();

        ElementBounds bg = ElementBounds.Fixed(0, 0, 300, 150);

        ElementBounds textBounds = ElementBounds.Fixed(10, 30, 280, 100);

        SingleComposer = capi.Gui
            .CreateCompo("levelinfo", bg)
            .AddShadedDialogBG(bg)
            .AddDialogTitleBar("Level Info", OnClose)
            .AddStaticText(message, CairoFont.WhiteSmallText(), textBounds)
        .Compose();
    }

    private void OnClose()
    {
        TryClose();
    }
}
