namespace ASToolkit.Storage.System;

public class StorageOptions
{
    public string? RootPath { get; init; }
    public bool UseRootPath { get; init; }

    public string PreparePath(string path)
    {
        if (!UseRootPath || string.IsNullOrWhiteSpace(RootPath))
            return path;
        var fullPath = Path.GetFullPath(path);
        var appPath = Path.GetFullPath(Directory.GetCurrentDirectory());

        var relativePath = Path.GetRelativePath(appPath, fullPath);
        return relativePath.StartsWith(RootPath) ? relativePath : Path.Combine(RootPath, relativePath);
    }

    public string RemoveRootPath(string path)
    {
        if (UseRootPath && !string.IsNullOrWhiteSpace(RootPath) && path.StartsWith(RootPath))
        {
            return path[RootPath.Length..]
                .TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }

        return path;
    }
}