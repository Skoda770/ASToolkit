using System.Text;
using ASToolkit.Storage.Abstracts;
using ASToolkit.Storage.Enums;
using ASToolkit.Storage.Interfaces;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;

namespace ASToolkit.Storage.Azure;

public class AzureStorage(ILogger<IStorage> logger, StorageOptions storageOptions) : StorageBase(logger)
{
    private readonly BlobContainerClient _container =
        new BlobContainerClient(storageOptions.ConnectionString, storageOptions.ContainerName);

    public override StorageType Type => StorageType.Azure;
    public void CreateContainerIfNotExists() => _container.CreateIfNotExists();

    public void DeleteContainerIfExists() => _container.DeleteIfExists();

    protected override void CreateFileLogic(string path, byte[] content)
    {
        var client = _container.GetBlobClient(path);
        var stream = new MemoryStream(content);
        client.Upload(stream, false);
    }

    protected override void UpdateFileLogic(string path, byte[] content)
    {
        var blobClient = _container.GetBlobClient(path);
        using var stream = new MemoryStream(content);
        blobClient.Upload(stream, overwrite: true);
    }

    protected override void DeleteFileLogic(string path)
    {
        _container.GetBlobClient(path).DeleteIfExists();
    }

    protected override byte[] ReadFileLogic(string path)
    {
        var blob = _container.GetBlobClient(path);
        var download = blob.DownloadContent();
        return download.Value.Content.ToArray();
    }

    protected override bool FileExistsLogic(string path)
    {
        return _container.GetBlobClient(path).Exists().Value;
    }

    protected override bool DirectoryExistsLogic(string path)
    {
        return _container.GetBlobsByHierarchy(prefix: path, delimiter: "/").Any();
    }

    protected override void RenameFileLogic(string path, string newFilePath)
    {
        CopyFileLogic(path, newFilePath);
        DeleteFileLogic(path);
    }

    protected override void MoveFileLogic(string originPath, string destinationPath)
    {
        RenameFileLogic(originPath, destinationPath);
    }

    protected override void CreateDirectoryLogic(string path)
    {
        var dummyPath = $"{path}.folder";
        _container.GetBlobClient(dummyPath).Upload(new MemoryStream(new byte[0]), overwrite: true);
    }

    protected override void DeleteDirectoryLogic(string path, bool recursive)
    {
        var blobs = _container.GetBlobsByHierarchy(prefix: path, delimiter: "/");
        foreach (var blob in blobs)
        {
            if (blob.IsBlob)
                _container.GetBlobClient(blob.Blob.Name).DeleteIfExists();
        }
    }

    protected override bool IsEmptyDirectoryLogic(string path)
    {
        path = path.TrimEnd(Path.AltDirectorySeparatorChar) + Path.AltDirectorySeparatorChar;
        return !_container.GetBlobsByHierarchy(prefix: path, delimiter: "/").Any();
    }

    protected override void MoveDirectoryLogic(string originPath, string destinationPath)
    {
        var blobs = _container.GetBlobs(prefix: originPath);

        foreach (var blob in blobs)
        {
            var newName = blob.Name.Replace(originPath, destinationPath);
            CopyFileLogic(blob.Name, newName);
            _container.GetBlobClient(blob.Name).DeleteIfExists();
        }
    }

    protected override void WriteAllTextLogic(string path, string contents, Encoding? encoding = null)
    {
        encoding ??= Encoding.UTF8;
        CreateFileLogic(path, encoding.GetBytes(contents));
    }

    protected override string ReadAllTextLogic(string path, Encoding? encoding = null)
    {
        encoding ??= Encoding.UTF8;
        var bytes = ReadFileLogic(path);
        return encoding.GetString(bytes);
    }

    protected override void CopyFileLogic(string originPath, string destinationPath)
    {
        var source = _container.GetBlobClient(originPath);
        var dest = _container.GetBlobClient(destinationPath);
        dest.StartCopyFromUri(source.Uri);
    }

    protected override string[] GetFilesInDirectoryLogic(string path, string searchPattern = "",
        SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        var result = new List<string>();
        path = path.TrimEnd(Path.AltDirectorySeparatorChar) + Path.AltDirectorySeparatorChar;
        // var blobs = _container.GetBlobsByHierarchy(prefix: path, delimiter: Path.AltDirectorySeparatorChar.ToString());

        foreach (var item in _container.GetBlobsByHierarchy(prefix: path, delimiter: "/"))
        {
            var blobPath = item.Blob.Name;
            if (item.IsBlob && (string.IsNullOrEmpty(searchPattern) || item.Blob.Name.Contains(searchPattern)))
                result.Add(blobPath.Replace(Path.AltDirectorySeparatorChar.ToString(), Path.DirectorySeparatorChar.ToString()));
        }

        return result.ToArray();
    }

    protected override string[] GetDirectoriesInDirectoryLogic(string path, string searchPattern = "",
        SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        var result = new List<string>();
        path = path.TrimEnd(Path.AltDirectorySeparatorChar) + Path.AltDirectorySeparatorChar;
        var blobs = _container.GetBlobsByHierarchy(prefix: path, delimiter: Path.AltDirectorySeparatorChar.ToString());
        foreach (var item in blobs)
        {
            var blobPath = item.Blob.Name;
            if (blobPath.EndsWith(".folder"))
                blobPath = blobPath[..^7];
            result.Add(blobPath.Replace(Path.AltDirectorySeparatorChar.ToString(), Path.DirectorySeparatorChar.ToString()));
        }

        return result.ToArray();
    }
}