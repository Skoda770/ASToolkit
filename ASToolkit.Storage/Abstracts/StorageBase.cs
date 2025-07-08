using System.Text;
using ASToolkit.Storage.Enums;
using ASToolkit.Storage.Interfaces;
using Microsoft.Extensions.Logging;

namespace ASToolkit.Storage.Abstracts;

public abstract class StorageBase(ILogger<IStorage> logger) : IStorage
{
    public abstract StorageType Type { get; }

    public void CreateFile(string path, byte[] content)
    {
        if (FileExists(path))
        {
            logger.LogError("File already exists at path: {Path}", path);
            throw new InvalidOperationException("File already exists.");
        }

        CreateFileLogic(path, content);
    }

    public void UpdateFile(string path, byte[] content)
    {
        if (!FileExists(path))
        {
            logger.LogError("File does not exist at path: {Path}", path);
            throw new FileNotFoundException("File does not exist.", path);
        }

        UpdateFileLogic(path, content);
    }

    public void DeleteFile(string path)
    {
        if (!FileExists(path))
        {
            logger.LogError("File does not exist at path: {Path}", path);
            throw new FileNotFoundException("File does not exist.", path);
        }
        DeleteFileLogic(path);
    }

    public byte[] ReadFile(string path)
    {
        if (!FileExists(path))
        {
            logger.LogError("File does not exist at path: {Path}", path);
            throw new FileNotFoundException("File does not exist.", path);
        }
        return ReadFileLogic(path);
    }

    public bool FileExists(string path) => FileExistsLogic(path);

    public bool TryCreateFile(string path, byte[] content)
    {
        try
        {
            CreateFile(path, content);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to create file at path: {Path}", path);
            return false;
        }
    }

    public bool TryUpdateFile(string path, byte[] content)
    {
        try
        {
            UpdateFile(path, content);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to update file at path: {Path}", path);
            return false;
        }
    }

    public bool TryDeleteFile(string path)
    {
        try
        {
            DeleteFile(path);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to delete file at path: {Path}", path);
            return false;
        }
    }

    public bool TryReadFile(string path, out byte[]? content)
    {
        try
        {
            content = ReadFile(path);
            return true;
        }
        catch (FileNotFoundException)
        {
            content = null;
            logger.LogWarning("File not found at path: {Path}", path);
            return false;
        }
        catch (Exception ex)
        {
            content = null;
            logger.LogWarning(ex, "Failed to read file at path: {Path}", path);
            return false;
        }
    }

    public void RenameFile(string path, string newFileName)
    {
        if (!FileExists(path))
        {
            logger.LogError("File does not exist at path: {Path}", path);
            throw new FileNotFoundException("File does not exist.", path);
        }
        var newFilePath = Path.Combine(Path.GetDirectoryName(path) ?? string.Empty, newFileName);
        if (FileExists(newFilePath))
        {
            logger.LogError("File already exists at new path: {NewFilePath}", newFilePath);
            throw new InvalidOperationException("File already exists at the new path.");
        }
        RenameFileLogic(path, newFilePath);
    }

    public bool TryRenameFile(string path, string newFileName)
    {
        try
        {
            RenameFile(path, newFileName);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to rename file from {Path} to {NewFileName}", path, newFileName);
            return false;
        }
    }

    public void MoveFile(string originPath, string destinationPath)
    {
        if (!FileExists(originPath))
        {
            logger.LogError("Origin file does not exist at path: {OriginPath}", originPath);
            throw new FileNotFoundException("Origin file does not exist.", originPath);
        }
        if (FileExists(destinationPath))
        {
            logger.LogError("Destination file already exists at path: {DestinationPath}", destinationPath);
            throw new InvalidOperationException("Destination file already exists.");
        }
        MoveFileLogic(originPath, destinationPath);
    }

    public bool TryMoveFile(string originPath, string destinationPath)
    {
        try
        {
            MoveFile(originPath, destinationPath);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to move file from {OriginPath} to {DestinationPath}", originPath, destinationPath);
            return false;
        }
    }

    public void CopyFile(string originPath, string destinationPath)
    {
        if (!FileExists(originPath))
        {
            logger.LogError("Origin file does not exist at path: {OriginPath}", originPath);
            throw new FileNotFoundException("Origin file does not exist.", originPath);
        }
        if (FileExists(destinationPath))
        {
            logger.LogError("Destination file already exists at path: {DestinationPath}", destinationPath);
            throw new InvalidOperationException("Destination file already exists.");
        }
        CopyFileLogic(originPath, destinationPath);
    }

    public bool TryCopyFile(string originPath, string destinationPath)
    {
        try
        {
            CopyFile(originPath, destinationPath);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to copy file from {OriginPath} to {DestinationPath}", originPath, destinationPath);
            return false;
        }
    }

    public string ReadAllText(string path, Encoding? encoding = null)
    {
        if (!FileExists(path))
        {
            logger.LogError("File does not exist at path: {Path}", path);
            throw new FileNotFoundException("File does not exist.", path);
        }
        return ReadAllTextLogic(path, encoding);
    }

    public bool TryReadAllText(string path, out string text, Encoding? encoding = null)
    {
        try
        {
            text = ReadAllText(path, encoding);
            return true;
        }
        catch (FileNotFoundException)
        {
            text = string.Empty;
            logger.LogWarning("File not found at path: {Path}", path);
            return false;
        }
        catch (Exception ex)
        {
            text = string.Empty;
            logger.LogWarning(ex, "Failed to read text from file at path: {Path}", path);
            return false;
        }
    }

    public void WriteAllText(string path, string contents, Encoding? encoding = null)
    {
        if (FileExists(path))
        {
            logger.LogError("File already exists at path: {Path}", path);
            throw new InvalidOperationException("File already exists.");
        }
        WriteAllTextLogic(path, contents, encoding);
    }

    public bool TryWriteAllText(string path, string contents, Encoding? encoding = null)
    {
        try
        {
            WriteAllText(path, contents, encoding);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to write text to file at path: {Path}", path);
            return false;
        }
    }

    public void CreateDirectory(string path)
    {
        if (DirectoryExists(path))
        {
            logger.LogError("Directory already exists at path: {Path}", path);
            throw new InvalidOperationException("Directory already exists.");
        }
        CreateDirectoryLogic(path);
    }

    public void DeleteDirectory(string path, bool recursive = false)
    {
        if (!DirectoryExists(path))
        {
            logger.LogError("Directory does not exist at path: {Path}", path);
            throw new DirectoryNotFoundException("Directory does not exist.");
        }
        DeleteDirectoryLogic(path, recursive);
    }

    public bool DirectoryExists(string path) => DirectoryExistsLogic(path);

    public string[] GetFilesInDirectory(string path, string searchPattern = "",
        SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        if (!DirectoryExists(path))
        {
            logger.LogError("Directory does not exist at path: {Path}", path);
            throw new DirectoryNotFoundException("Directory does not exist.");
        }
        return GetFilesInDirectoryLogic(path, searchPattern, searchOption);
    }

    public string[] GetDirectoriesInDirectory(string path, string searchPattern = "",
        SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        if (!DirectoryExists(path))
        {
            logger.LogError("Directory does not exist at path: {Path}", path);
            throw new DirectoryNotFoundException("Directory does not exist.");
        }
        return GetDirectoriesInDirectoryLogic(path, searchPattern, searchOption);
    }

    public bool TryGetDirectoriesInDirectory(string path, out string[]? directories, string searchPattern = "",
        SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        directories = null;
        try
        {
            directories = GetDirectoriesInDirectory(path, searchPattern, searchOption);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to get directories in path: {Path}", path);
            return false;
        }
    }

    public bool TryCreateDirectory(string path)
    {
        try
        {
            CreateDirectory(path);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to create directory at path: {Path}", path);
            return false;
        }
    }

    public bool TryDeleteDirectory(string path, bool recursive = false)
    {
        try
        {
            DeleteDirectory(path, recursive);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to delete directory at path: {Path}", path);
            return false;
        }
    }

    public bool TryGetFilesInDirectory(string path, out string[]? files, string searchPattern = "",
        SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        files = null;
        try
        {
            files = GetFilesInDirectory(path, searchPattern, searchOption);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to get files in directory at path: {Path}", path);
            return false;
        }
    }

    public bool IsEmptyDirectory(string path)
    {
        if (!DirectoryExists(path))
        {
            logger.LogError("Directory does not exist at path: {Path}", path);
            throw new DirectoryNotFoundException("Directory does not exist.");
        }
        return IsEmptyDirectoryLogic(path);
    }

    public void MoveDirectory(string oldPath, string newPath)
    {
        if (!DirectoryExists(oldPath))
        {
            logger.LogError("Old directory does not exist at path: {OldPath}", oldPath);
            throw new DirectoryNotFoundException("Old directory does not exist.");
        }
        if (DirectoryExists(newPath))
        {
            logger.LogError("New directory already exists at path: {NewPath}", newPath);
            throw new InvalidOperationException("New directory already exists.");
        }
        MoveDirectoryLogic(oldPath, newPath);
    }

    public bool TryMoveDirectory(string oldPath, string newPath)
    {
        try
        {
            MoveDirectory(oldPath, newPath);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to move directory from {OldPath} to {NewPath}", oldPath, newPath);
            return false;
        }
    }

    protected abstract void CreateFileLogic(string path, byte[] content);
    protected abstract void UpdateFileLogic(string path, byte[] content);
    protected abstract void DeleteFileLogic(string path);
    protected abstract byte[] ReadFileLogic(string path);
    protected abstract bool FileExistsLogic(string path);
    protected abstract bool DirectoryExistsLogic(string path);
    protected abstract void RenameFileLogic(string path, string newFilePath);
    protected abstract void MoveFileLogic(string originPath, string destinationPath);
    protected abstract void CreateDirectoryLogic(string path);
    protected abstract void DeleteDirectoryLogic(string path, bool recursive);
    protected abstract bool IsEmptyDirectoryLogic(string path);
    protected abstract void MoveDirectoryLogic(string originPath, string destinationPath);
    protected abstract void WriteAllTextLogic(string path, string contents, Encoding? encoding = null);
    protected abstract string ReadAllTextLogic(string path, Encoding? encoding = null);
    protected abstract void CopyFileLogic(string originPath, string destinationPath);

    protected abstract string[] GetFilesInDirectoryLogic(string path, string searchPattern = "",
        SearchOption searchOption = SearchOption.TopDirectoryOnly);

    protected abstract string[] GetDirectoriesInDirectoryLogic(string path, string searchPattern = "",
        SearchOption searchOption = SearchOption.TopDirectoryOnly);
}