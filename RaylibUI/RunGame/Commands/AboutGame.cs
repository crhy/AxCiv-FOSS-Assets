using System.Reflection;
using JetBrains.Annotations;
using Model;
using Model.Controls;
using Model.Input;

namespace RaylibUI.RunGame.Commands;

/// <summary>
/// The Civilopedia menu's About entry. It was in the menu with no command behind
/// it, so it opened nothing.
/// </summary>
[UsedImplicitly]
public class AboutGame(GameScreen gameScreen) : IGameCommand
{
    public string Id => CommandIds.AboutGame;

    public Shortcut[] ActivationKeys { get; set; } = [];
    public CommandStatus Status { get; private set; }

    public bool Update()
    {
        Status = CommandStatus.Normal;
        return true;
    }

    public void Action() => gameScreen.ShowPopup("ABOUT", replaceStrings: [BuildVersion]);

    /// <summary>
    /// The version the build was stamped with, from Directory.Build.props. The
    /// informational version carries the pre-release suffix; the plain assembly
    /// version does not, which is why it is not used here.
    /// </summary>
    private static string BuildVersion =>
        typeof(AboutGame).Assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
            .Split('+')[0]
        ?? typeof(AboutGame).Assembly.GetName().Version?.ToString()
        ?? "unknown";

    public bool Checked => false;
    public MenuCommand? Command { get; set; }
    public string ErrorDialog => string.Empty;
    public DialogImageElements? ErrorImage => null;
    public string? Name => "About rhYciv";
}
