using System.Text;
using ASToolkit.Storage.Enums;

namespace ASToolkit.Storage.Interfaces;

public interface IStorage
{
    public StorageType Type { get; }
    #region Files

    void CreateFile(string path, byte[] content);

    void UpdateFile(string path, byte[] content);

    void DeleteFile(string path);

    byte[] ReadFile(string path);

    bool FileExists(string path);

    bool TryCreateFile(string path, byte[] content);

    bool TryUpdateFile(string path, byte[] content);

    bool TryDeleteFile(string path);

    bool TryReadFile(string path, out byte[]? content);

    void RenameFile(string path, string newFileName);
    bool TryRenameFile(string path, string newFileName);
    void MoveFile(string originPath, string destinationPath);
    bool TryMoveFile(string originPath, string destinationPath);
    void CopyFile(string originPath, string destinationPath);
    bool TryCopyFile(string originPath, string destinationPath);

    string ReadAllText(string path, Encoding? encoding = null);
    bool TryReadAllText(string path, out string text, Encoding? encoding = null);
    void WriteAllText(string path, string contents, Encoding? encoding = null);
    bool TryWriteAllText(string path, string contents, Encoding? encoding = null);

    #endregion Files

    #region Directories

    void CreateDirectory(string path);

    void DeleteDirectory(string path, bool recursive = false);

    bool DirectoryExists(string path);

    string[] GetFilesInDirectory(string path, string searchPattern = "",
        SearchOption searchOption = SearchOption.TopDirectoryOnly);

    string[] GetDirectoriesInDirectory(string path, string searchPattern = "",
        SearchOption searchOption = SearchOption.TopDirectoryOnly);

    bool TryGetDirectoriesInDirectory(string path, out string[]? directories, string searchPattern = "",
        SearchOption searchOption = SearchOption.TopDirectoryOnly);

    bool TryCreateDirectory(string path);

    bool TryDeleteDirectory(string path, bool recursive = false);

    bool TryGetFilesInDirectory(string path, out string[]? files, string searchPattern = "",
        SearchOption searchOption = SearchOption.TopDirectoryOnly);

    bool IsEmptyDirectory(string path);
    void MoveDirectory(string oldPath, string newPath);
    bool TryMoveDirectory(string oldPath, string newPath);

    #endregion Directories
    
}