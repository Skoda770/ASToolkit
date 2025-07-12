using System.Text;
using ASToolkit.Storage.Abstracts;
using ASToolkit.Storage.Enums;
using ASToolkit.Storage.Interfaces;
using Microsoft.Extensions.Logging;

namespace ASToolkit.Storage.Desktop;

public class AppDataStorage(ILogger<IStorage> logger, StorageOptions options) : StorageBase(logger)
{
    public override StorageType Type => StorageType.AppData;
    protected override void CreateFileLogic(string path, byte[] content)
    {
        var internalPath = options.PreparePath(path);
        File.WriteAllBytes(internalPath, content);
    }

    protected override void UpdateFileLogic(string path, byte[] content)
    {
        var internalPath = options.PreparePath(path);
        File.WriteAllBytes(internalPath, content);
    }

    protected override void DeleteFileLogic(string path)
    {
        var internalPath = options.PreparePath(path);
        File.Delete(internalPath);
    }

    protected override byte[] ReadFileLogic(string path)
    {
        var internalPath = options.PreparePath(path);
        return File.ReadAllBytes(internalPath);
    }

    protected override bool FileExistsLogic(string path)
    {
        var internalPath = options.PreparePath(path);
        return File.Exists(internalPath);
    }

    protected override bool DirectoryExistsLogic(string path)
    {
        var internalPath = options.PreparePath(path);
        return Directory.Exists(internalPath);
    }

    protected override void RenameFileLogic(string path, string newFilePath)
    {
        var internalPath = options.PreparePath(path);
        var internalNewFilePath = options.PreparePath(newFilePath);
        File.Move(internalPath, internalNewFilePath);
    }

    protected override void MoveFileLogic(string originPath, string destinationPath)
    {
        var internalOriginPath = options.PreparePath(originPath);
        var internalDestinationPath = options.PreparePath(destinationPath);
        File.Move(internalOriginPath, internalDestinationPath);
    }

    protected override void CreateDirectoryLogic(string path)
    {
        var internalPath = options.PreparePath(path);
        Directory.CreateDirectory(internalPath);
    }

    protected override void DeleteDirectoryLogic(string path, bool recursive)
    {
        var internalPath = options.PreparePath(path);
        Directory.Delete(internalPath, recursive);
    }

    protected override bool IsEmptyDirectoryLogic(string path)
    {
        return GetFilesInDirectoryLogic(path).Length == 0 &&
               GetDirectoriesInDirectoryLogic(path).Length == 0;
    }

    protected override void MoveDirectoryLogic(string originPath, string destinationPath)
    {
        var internalOriginPath = options.PreparePath(originPath);
        var internalDestinationPath = options.PreparePath(destinationPath);
        Directory.Move(internalOriginPath, internalDestinationPath);
    }

    protected override void WriteAllTextLogic(string path, string contents, Encoding? encoding = null)
    {
        var internalPath = options.PreparePath(path);
        if (encoding is null)
            File.WriteAllText(internalPath, contents);
        else
            File.WriteAllText(internalPath, contents, encoding);
    }

    protected override string ReadAllTextLogic(string path, Encoding? encoding = null)
    {
        var internalPath = options.PreparePath(path);
        return encoding is not null ? File.ReadAllText(internalPath, encoding) : File.ReadAllText(internalPath);
    }

    protected override void CopyFileLogic(string originPath, string destinationPath)
    {
        var internalOriginPath = options.PreparePath(originPath);
        var internalDestinationPath = options.PreparePath(destinationPath);
        File.Copy(internalOriginPath, internalDestinationPath);
    }

    protected override string[] GetFilesInDirectoryLogic(string path, string searchPattern = "",
        SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        var internalPath = options.PreparePath(path);
        var result = Directory.GetFiles(internalPath, searchPattern, searchOption);
        return result.Select(options.RemoveRootPath).ToArray();

    }

    protected override string[] GetDirectoriesInDirectoryLogic(string path, string searchPattern = "",
        SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        var internalPath = options.PreparePath(path);
        var result = Directory.GetDirectories(internalPath, searchPattern, searchOption);
        return result.Select(options.RemoveRootPath).ToArray();

    }
}