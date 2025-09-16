using System.IO;
using System.Text;
using ASToolkit.Storage.Azure;
using ASToolkit.Storage.Interfaces;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ASToolkit.Storage.AzureTests;

[TestSubject(typeof(AzureStorage))]
public class AzureStorageTest
{
    private static ILogger<IStorage> CreateLogger()
    {
        var mock = new Mock<ILogger<IStorage>>();
        return mock.Object;
    }
    private static IStorage FileStorageProvider => new AzureStorage(CreateLogger(), new()
    {
        ConnectionString = "UseDevelopmentStorage=true",
        ContainerName = "tests"
    });
    [Fact]
    public void DeleteFile_FileExists_DeletesFileSuccessfully()
    {
        var filePath = "fileToDelete.txt";
        FileStorageProvider.CreateFile(filePath, Encoding.UTF8.GetBytes("content"));
        Assert.True(FileStorageProvider.FileExists(filePath));
        FileStorageProvider.DeleteFile(filePath);

        Assert.False(FileStorageProvider.FileExists(filePath));
    }

    [Fact]
    public void DeleteFile_FileDoesNotExist_ThrowsFileNotFoundException()
    {
        var filePath = "nonexistentFile.txt";
        Assert.Throws<FileNotFoundException>(() => FileStorageProvider.DeleteFile(filePath));
    }

    [Fact]
    public void AddFile_ValidPathAndContent_CreatesFileSuccessfully()
    {
        var filePath = "newFile.txt";
        var content = Encoding.UTF8.GetBytes("file content");
        FileStorageProvider.CreateFile(filePath, content);

        Assert.True(FileStorageProvider.FileExists(filePath));
        Assert.Equal(content, FileStorageProvider.ReadFile(filePath));
    }

    [Fact]
    public void UpdateFile_FileExists_UpdatesFileContentSuccessfully()
    {
        var filePath = "fileToUpdate.txt";
        var initialContent = Encoding.UTF8.GetBytes("initial content");
        var updatedContent = Encoding.UTF8.GetBytes("updated content");
        FileStorageProvider.CreateFile(filePath, initialContent);
        Assert.Equal(initialContent, FileStorageProvider.ReadFile(filePath));
        FileStorageProvider.UpdateFile(filePath, updatedContent);

        Assert.Equal(updatedContent, FileStorageProvider.ReadFile(filePath));
        Assert.NotEqual(initialContent, FileStorageProvider.ReadFile(filePath));
    }

    [Fact]
    public void ReadAllText_FileExists_ReturnsFileContentAsString()
    {
        var filePath = "fileToRead.txt";
        var content = "file content";
        FileStorageProvider.CreateFile(filePath, Encoding.UTF8.GetBytes(content));
        var result = FileStorageProvider.ReadAllText(filePath);

        Assert.Equal(content, result);
    }

    [Fact]
    public void WriteAllText_ValidPathAndContent_WritesContentToFile()
    {
        var filePath = "fileToWrite.txt";
        var content = "new file content";
        FileStorageProvider.WriteAllText(filePath, content);

        Assert.True(FileStorageProvider.FileExists(filePath));
        Assert.Equal(content, FileStorageProvider.ReadAllText(filePath));
    }

    [Fact]
    public void MoveFile_FileExists_MovesFileSuccessfully()
    {
        var originPath = "sourceFile.txt";
        var destinationPath = "destinationFile.txt";
        FileStorageProvider.CreateFile(originPath, Encoding.UTF8.GetBytes("file content"));
        FileStorageProvider.MoveFile(originPath, destinationPath);

        Assert.False(FileStorageProvider.FileExists(originPath));
        Assert.True(FileStorageProvider.FileExists(destinationPath));
        Assert.Equal("file content", FileStorageProvider.ReadAllText(destinationPath));
    }


    [Fact]
    public void CreateDirectory_ValidPath_CreatesDirectorySuccessfully()
    {
        var directoryPath = "newDirectory";
        FileStorageProvider.CreateDirectory(directoryPath);

        Assert.True(FileStorageProvider.DirectoryExists(directoryPath));
    }

    [Fact]
    public void DeleteDirectory_DirectoryDoesNotExist_ThrowsDirectoryNotFoundException(
        )
    {
        var directoryPath = "nonexistentDirectory";
        Assert.Throws<DirectoryNotFoundException>(() => FileStorageProvider.DeleteDirectory(directoryPath));
    }

    [Fact]
    public void GetFilesInDirectory_DirectoryExists_ReturnsFiles()
    {
        var directoryPath = "directoryWithFiles";
        FileStorageProvider.CreateDirectory(directoryPath);
        FileStorageProvider.CreateFile(Path.Combine(directoryPath, "file1.txt"),
            Encoding.UTF8.GetBytes("content"));
        FileStorageProvider.CreateFile(Path.Combine(directoryPath, "file2.txt"),
            Encoding.UTF8.GetBytes("content"));

        var files = FileStorageProvider.GetFilesInDirectory(directoryPath);

        Assert.Equal(2, files.Length);
        Assert.Contains(Path.Combine(directoryPath, "file1.txt"), files);
        Assert.Contains(Path.Combine(directoryPath, "file2.txt"), files);
    }

    [Fact]
    public void GetDirectoriesInDirectory_DirectoryExists_ReturnsSubdirectories()
    {
        var directoryPath = "parentDirectory";
        FileStorageProvider.CreateDirectory(directoryPath);
        FileStorageProvider.CreateDirectory(Path.Combine(directoryPath, "subDir1"));
        FileStorageProvider.CreateDirectory(Path.Combine(directoryPath, "subDir2"));

        var directories = FileStorageProvider.GetDirectoriesInDirectory(directoryPath);

        Assert.Equal(2, directories.Length);
        Assert.Contains(Path.Combine(directoryPath, "subDir1"), directories);
        Assert.Contains(Path.Combine(directoryPath, "subDir2"), directories);
    }

    [Fact]
    public void IsEmptyDirectory_EmptyDirectory_ReturnsTrue()
    {
        var directoryPath = "emptyDirectory";
        FileStorageProvider.CreateDirectory(directoryPath);

        var isEmpty = FileStorageProvider.IsEmptyDirectory(directoryPath);

        Assert.True(isEmpty);
    }

    [Fact]
    public void IsEmptyDirectory_NonEmptyDirectory_ReturnsFalse()
    {
        var directoryPath = "nonEmptyDirectory";
        FileStorageProvider.CreateDirectory(directoryPath);
        FileStorageProvider.CreateFile(Path.Combine(directoryPath, "file.txt"),
            Encoding.UTF8.GetBytes("content"));

        var isEmpty = FileStorageProvider.IsEmptyDirectory(directoryPath);

        Assert.False(isEmpty);
    }
}