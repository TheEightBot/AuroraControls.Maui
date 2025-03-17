namespace AuroraControls.TestApp;

public static class UIExtensions
{
    public static Button BindClicked(this Button button, Action action)
    {
        button.Command = new Command(action);
        return button;
    }
}
