namespace ASToolkit.Storage.Desktop;

public class StorageOptions
{
    public string ApplicationName { get; set; } = null!;
    private static string RootPath => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

    public string PreparePath(string path)
    {
        var combinedPath = Path.GetFullPath(Path.Combine(RootPath, ApplicationName, path));
        var fullRoot = Path.GetFullPath(RootPath);

        if (!combinedPath.StartsWith(fullRoot, StringComparison.OrdinalIgnoreCase))
            throw new UnauthorizedAccessException($"Attempted access outside of RootPath: {combinedPath}");

        return combinedPath;
    }

    public string RemoveRootPath(string path)
    {
        return path[RootPath.Length..]
            .TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
    }
}