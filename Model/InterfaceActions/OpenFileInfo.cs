namespace Model.InterfaceActions;

public class OpenFileInfo
{
    public string Title { get; init; } = string.Empty;
    public string InitialDirectory { get; init; } = string.Empty;
    public IList<FileFilter> Filters { get; init; } = [];
}
