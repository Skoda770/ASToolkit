using System.Text;
using ASToolkit.Storage.Interfaces;
using ASToolkit.Storage.System;
using Microsoft.Extensions.Logging;
using Moq;

namespace ASToolkit.Storage.SystemTests;

public class FileSystemStorageProviderTest : IAsyncLifetime
{
    private string? _testDirectory;

    private static ILogger<IStorage> CreateLogger()
    {
        var mock = new Mock<ILogger<IStorage>>();
        return mock.Object;
    }
    public static TheoryData<IStorage> Providers =>
        new()
        {
            new SystemStorage(CreateLogger(), new()
            {
                RootPath = "root",
                UseRootPath = true
            }),
            new SystemStorage(CreateLogger(), new()
            {
                RootPath = "",
                UseRootPath = false
            })
        };

    public Task InitializeAsync()
    {
        _testDirectory = Path.Combine(Directory.GetCurrentDirectory(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_testDirectory);
        Directory.SetCurrentDirectory(_testDirectory);
        if (Directory.Exists("root"))
            Directory.Delete("root", true);

        Directory.CreateDirectory("root");

        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        Directory.SetCurrentDirectory(Directory.GetParent(_testDirectory!)!.FullName);
        if (Directory.Exists(_testDirectory))
        {
            Directory.Delete(_testDirectory, true);
        }

        return Task.CompletedTask;
    }

    [Theory]
    [MemberData(nameof(Providers))]
    public void RenameFile_ValidPathAndNewFileName_RenamesFileSuccessfully(
        IStorage fileStorageProvider)
    {
        var filePath = "test.txt";
        var newFileName = "renamed.txt";
        fileStorageProvider.CreateFile(filePath, Encoding.UTF8.GetBytes("content"));
        fileStorageProvider.RenameFile(filePath, newFileName);

        Assert.False(fileStorageProvider.FileExists(filePath));
        Assert.True(fileStorageProvider.FileExists(newFileName));
    }

    [Theory]
    [MemberData(nameof(Providers))]
    public void RenameFile_FileDoesNotExist_ThrowsFileNotExistsException(IStorage fileStorageProvider)
    {
        var filePath = "nonexistent.txt";
        var newFileName = "renamed.txt";
        Assert.Throws<FileNotFoundException>(() => fileStorageProvider.RenameFile(filePath, newFileName));
    }

    [Theory]
    [MemberData(nameof(Providers))]
    public void RenameFile_NewFileNameAlreadyExists_ThrowsFileExistsException(
        IStorage fileStorageProvider)
    {
        var filePath = "test.txt";
        var newFileName = "existing.txt";
        fileStorageProvider.CreateFile(filePath, Encoding.UTF8.GetBytes("content"));
        fileStorageProvider.CreateFile(newFileName, Encoding.UTF8.GetBytes("other content"));

        Assert.Throws<InvalidOperationException>(() => fileStorageProvider.RenameFile(filePath, newFileName));
    }

    [Theory]
    [MemberData(nameof(Providers))]
    public void TryRenameFile_ValidPathAndNewFileName_ReturnsTrue(IStorage fileStorageProvider)
    {
        var filePath = "test.txt";
        var newFileName = "renamed.txt";
        fileStorageProvider.CreateFile(filePath, Encoding.UTF8.GetBytes("content"));
        var result = fileStorageProvider.TryRenameFile(filePath, newFileName);

        Assert.True(result);
        Assert.False(fileStorageProvider.FileExists(filePath));
        Assert.True(fileStorageProvider.FileExists(newFileName));
    }

    [Theory]
    [MemberData(nameof(Providers))]
    public void TryRenameFile_FileDoesNotExist_ReturnsFalse(IStorage fileStorageProvider)
    {
        var filePath = "nonexistent.txt";
        var newFileName = "renamed.txt";
        var result = fileStorageProvider.TryRenameFile(filePath, newFileName);

        Assert.False(result);
    }

    [Theory]
    [MemberData(nameof(Providers))]
    public void TryRenameFile_NewFileNameAlreadyExists_ReturnsFalse(IStorage fileStorageProvider)
    {
        var filePath = "test.txt";
        var newFileName = "existing.txt";
        fileStorageProvider.CreateFile(filePath, Encoding.UTF8.GetBytes("content"));
        fileStorageProvider.CreateFile(newFileName, Encoding.UTF8.GetBytes("other content"));

        var result = fileStorageProvider.TryRenameFile(filePath, newFileName);

        Assert.False(result);
    }

    [Theory]
    [MemberData(nameof(Providers))]
    public void DeleteFile_FileExists_DeletesFileSuccessfully(IStorage fileStorageProvider)
    {
        var filePath = "fileToDelete.txt";
        fileStorageProvider.CreateFile(filePath, Encoding.UTF8.GetBytes("content"));
        Assert.True(fileStorageProvider.FileExists(filePath));
        fileStorageProvider.DeleteFile(filePath);

        Assert.False(fileStorageProvider.FileExists(filePath));
    }

    [Theory]
    [MemberData(nameof(Providers))]
    public void DeleteFile_FileDoesNotExist_ThrowsFileNotFoundException(IStorage fileStorageProvider)
    {
        var filePath = "nonexistentFile.txt";
        Assert.Throws<FileNotFoundException>(() => fileStorageProvider.DeleteFile(filePath));
    }

    [Theory]
    [MemberData(nameof(Providers))]
    public void AddFile_ValidPathAndContent_CreatesFileSuccessfully(IStorage fileStorageProvider)
    {
        var filePath = "newFile.txt";
        var content = Encoding.UTF8.GetBytes("file content");
        fileStorageProvider.CreateFile(filePath, content);

        Assert.True(fileStorageProvider.FileExists(filePath));
        Assert.Equal(content, fileStorageProvider.ReadFile(filePath));
    }

    [Theory]
    [MemberData(nameof(Providers))]
    public void UpdateFile_FileExists_UpdatesFileContentSuccessfully(IStorage fileStorageProvider)
    {
        var filePath = "fileToUpdate.txt";
        var initialContent = Encoding.UTF8.GetBytes("initial content");
        var updatedContent = Encoding.UTF8.GetBytes("updated content");
        fileStorageProvider.CreateFile(filePath, initialContent);
        Assert.Equal(initialContent, fileStorageProvider.ReadFile(filePath));
        fileStorageProvider.UpdateFile(filePath, updatedContent);

        Assert.Equal(updatedContent, fileStorageProvider.ReadFile(filePath));
        Assert.NotEqual(initialContent, fileStorageProvider.ReadFile(filePath));
    }

    [Theory]
    [MemberData(nameof(Providers))]
    public void ReadAllText_FileExists_ReturnsFileContentAsString(IStorage fileStorageProvider)
    {
        var filePath = "fileToRead.txt";
        var content = "file content";
        fileStorageProvider.CreateFile(filePath, Encoding.UTF8.GetBytes(content));
        var result = fileStorageProvider.ReadAllText(filePath);

        Assert.Equal(content, result);
    }

    [Theory]
    [MemberData(nameof(Providers))]
    public void WriteAllText_ValidPathAndContent_WritesContentToFile(IStorage fileStorageProvider)
    {
        var filePath = "fileToWrite.txt";
        var content = "new file content";
        fileStorageProvider.WriteAllText(filePath, content);

        Assert.True(fileStorageProvider.FileExists(filePath));
        Assert.Equal(content, fileStorageProvider.ReadAllText(filePath));
    }

    [Theory]
    [MemberData(nameof(Providers))]
    public void MoveFile_FileExists_MovesFileSuccessfully(IStorage fileStorageProvider)
    {
        var originPath = "sourceFile.txt";
        var destinationPath = "destinationFile.txt";
        fileStorageProvider.CreateFile(originPath, Encoding.UTF8.GetBytes("file content"));
        fileStorageProvider.MoveFile(originPath, destinationPath);

        Assert.False(fileStorageProvider.FileExists(originPath));
        Assert.True(fileStorageProvider.FileExists(destinationPath));
        Assert.Equal("file content", fileStorageProvider.ReadAllText(destinationPath));
    }

    [Theory]
    [MemberData(nameof(Providers))]
    public void MoveFile_FileDoesNotExist_ThrowsFileNotFoundException(IStorage fileStorageProvider)
    {
        var originPath = "nonexistentFile.txt";
        var destinationPath = "destinationFile.txt";
        Assert.Throws<FileNotFoundException>(() => fileStorageProvider.MoveFile(originPath, destinationPath));
    }

    [Theory]
    [MemberData(nameof(Providers))]
    public void CopyFile_FileExists_CopiesFileSuccessfully(IStorage fileStorageProvider)
    {
        var originPath = "sourceFile.txt";
        var destinationPath = "copiedFile.txt";
        fileStorageProvider.CreateFile(originPath, Encoding.UTF8.GetBytes("file content"));
        fileStorageProvider.CopyFile(originPath, destinationPath);

        Assert.True(fileStorageProvider.FileExists(originPath));
        Assert.True(fileStorageProvider.FileExists(destinationPath));
        Assert.Equal("file content", fileStorageProvider.ReadAllText(destinationPath));
    }

    [Theory]
    [MemberData(nameof(Providers))]
    public void CopyFile_FileDoesNotExist_ThrowsFileNotFoundException(IStorage fileStorageProvider)
    {
        var originPath = "nonexistentFile.txt";
        var destinationPath = "copiedFile.txt";
        Assert.Throws<FileNotFoundException>(() => fileStorageProvider.CopyFile(originPath, destinationPath));
    }

    [Theory]
    [MemberData(nameof(Providers))]
    public void CreateDirectory_ValidPath_CreatesDirectorySuccessfully(IStorage fileStorageProvider)
    {
        var directoryPath = "newDirectory";
        fileStorageProvider.CreateDirectory(directoryPath);

        Assert.True(fileStorageProvider.DirectoryExists(directoryPath));
    }

    [Theory]
    [MemberData(nameof(Providers))]
    public void DeleteDirectory_DirectoryExists_DeletesDirectorySuccessfully(
        IStorage fileStorageProvider)
    {
        var directoryPath = "directoryToDelete";
        fileStorageProvider.CreateDirectory(directoryPath);
        Assert.True(fileStorageProvider.DirectoryExists(directoryPath));

        fileStorageProvider.DeleteDirectory(directoryPath);

        Assert.False(fileStorageProvider.DirectoryExists(directoryPath));
    }

    [Theory]
    [MemberData(nameof(Providers))]
    public void DeleteDirectory_DirectoryDoesNotExist_ThrowsDirectoryNotFoundException(
        IStorage fileStorageProvider)
    {
        var directoryPath = "nonexistentDirectory";
        Assert.Throws<DirectoryNotFoundException>(() => fileStorageProvider.DeleteDirectory(directoryPath));
    }

    [Theory]
    [MemberData(nameof(Providers))]
    public void GetFilesInDirectory_DirectoryExists_ReturnsFiles(IStorage fileStorageProvider)
    {
        var directoryPath = "directoryWithFiles";
        fileStorageProvider.CreateDirectory(directoryPath);
        fileStorageProvider.CreateFile(Path.Combine(directoryPath, "file1.txt"),
            Encoding.UTF8.GetBytes("content"));
        fileStorageProvider.CreateFile(Path.Combine(directoryPath, "file2.txt"),
            Encoding.UTF8.GetBytes("content"));

        var files = fileStorageProvider.GetFilesInDirectory(directoryPath);

        Assert.Equal(2, files.Length);
        Assert.Contains(Path.Combine(directoryPath, "file1.txt"), files);
        Assert.Contains(Path.Combine(directoryPath, "file2.txt"), files);
    }

    [Theory]
    [MemberData(nameof(Providers))]
    public void GetDirectoriesInDirectory_DirectoryExists_ReturnsSubdirectories(
        IStorage fileStorageProvider)
    {
        var directoryPath = "parentDirectory";
        fileStorageProvider.CreateDirectory(directoryPath);
        fileStorageProvider.CreateDirectory(Path.Combine(directoryPath, "subDir1"));
        fileStorageProvider.CreateDirectory(Path.Combine(directoryPath, "subDir2"));

        var directories = fileStorageProvider.GetDirectoriesInDirectory(directoryPath);

        Assert.Equal(2, directories.Length);
        Assert.Contains(Path.Combine(directoryPath, "subDir1"), directories);
        Assert.Contains(Path.Combine(directoryPath, "subDir2"), directories);
    }

    [Theory]
    [MemberData(nameof(Providers))]
    public void IsEmptyDirectory_EmptyDirectory_ReturnsTrue(IStorage fileStorageProvider)
    {
        var directoryPath = "emptyDirectory";
        fileStorageProvider.CreateDirectory(directoryPath);

        var isEmpty = fileStorageProvider.IsEmptyDirectory(directoryPath);

        Assert.True(isEmpty);
    }

    [Theory]
    [MemberData(nameof(Providers))]
    public void IsEmptyDirectory_NonEmptyDirectory_ReturnsFalse(IStorage fileStorageProvider)
    {
        var directoryPath = "nonEmptyDirectory";
        fileStorageProvider.CreateDirectory(directoryPath);
        fileStorageProvider.CreateFile(Path.Combine(directoryPath, "file.txt"),
            Encoding.UTF8.GetBytes("content"));

        var isEmpty = fileStorageProvider.IsEmptyDirectory(directoryPath);

        Assert.False(isEmpty);
    }

    [Theory]
    [MemberData(nameof(Providers))]
    public void MoveDirectory_DirectoryExists_MovesDirectorySuccessfully(IStorage fileStorageProvider)
    {
        var oldPath = "oldDirectory";
        var newPath = "newDirectory";
        fileStorageProvider.CreateDirectory(oldPath);

        fileStorageProvider.MoveDirectory(oldPath, newPath);

        Assert.False(fileStorageProvider.DirectoryExists(oldPath));
        Assert.True(fileStorageProvider.DirectoryExists(newPath));
    }

    [Theory]
    [MemberData(nameof(Providers))]
    public void MoveDirectory_DirectoryDoesNotExist_ThrowsDirectoryNotFoundException(
        IStorage fileStorageProvider)
    {
        var oldPath = "nonexistentDirectory";
        var newPath = "newDirectory";

        Assert.Throws<DirectoryNotFoundException>(() => fileStorageProvider.MoveDirectory(oldPath, newPath));
    }
}