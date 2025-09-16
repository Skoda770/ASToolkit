using System.Text;
using ASToolkit.Storage.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ASToolkit.Storage.Tests;

public abstract class BaseStorageTest : IAsyncLifetime
{
    protected static ILogger<IStorage> CreateLogger()
    {
        var mock = new Mock<ILogger<IStorage>>();
        return mock.Object;
    }
    public abstract Task InitializeAsync();
    public abstract Task DisposeAsync();
    
    [Theory]
    [MemberData("Providers")]
    public virtual void RenameFile_ValidPathAndNewFileName_RenamesFileSuccessfully(
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
    [MemberData("Providers")]
    public virtual void RenameFile_FileDoesNotExist_ThrowsFileNotExistsException(IStorage fileStorageProvider)
    {
        var filePath = "nonexistent.txt";
        var newFileName = "renamed.txt";
        Assert.Throws<FileNotFoundException>(() => fileStorageProvider.RenameFile(filePath, newFileName));
    }

    [Theory]
    [MemberData("Providers")]
    public virtual void RenameFile_NewFileNameAlreadyExists_ThrowsFileExistsException(
        IStorage fileStorageProvider)
    {
        var filePath = "test.txt";
        var newFileName = "existing.txt";
        fileStorageProvider.CreateFile(filePath, Encoding.UTF8.GetBytes("content"));
        fileStorageProvider.CreateFile(newFileName, Encoding.UTF8.GetBytes("other content"));

        Assert.Throws<InvalidOperationException>(() => fileStorageProvider.RenameFile(filePath, newFileName));
    }

    [Theory]
    [MemberData("Providers")]
    public virtual void TryRenameFile_ValidPathAndNewFileName_ReturnsTrue(IStorage fileStorageProvider)
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
    [MemberData("Providers")]
    public virtual void TryRenameFile_FileDoesNotExist_ReturnsFalse(IStorage fileStorageProvider)
    {
        var filePath = "nonexistent.txt";
        var newFileName = "renamed.txt";
        var result = fileStorageProvider.TryRenameFile(filePath, newFileName);

        Assert.False(result);
    }

    [Theory]
    [MemberData("Providers")]
    public virtual void TryRenameFile_NewFileNameAlreadyExists_ReturnsFalse(IStorage fileStorageProvider)
    {
        var filePath = "test.txt";
        var newFileName = "existing.txt";
        fileStorageProvider.CreateFile(filePath, Encoding.UTF8.GetBytes("content"));
        fileStorageProvider.CreateFile(newFileName, Encoding.UTF8.GetBytes("other content"));

        var result = fileStorageProvider.TryRenameFile(filePath, newFileName);

        Assert.False(result);
    }

    [Theory]
    [MemberData("Providers")]
    public virtual void DeleteFile_FileExists_DeletesFileSuccessfully(IStorage fileStorageProvider)
    {
        var filePath = "fileToDelete.txt";
        fileStorageProvider.CreateFile(filePath, Encoding.UTF8.GetBytes("content"));
        Assert.True(fileStorageProvider.FileExists(filePath));
        fileStorageProvider.DeleteFile(filePath);

        Assert.False(fileStorageProvider.FileExists(filePath));
    }

    [Theory]
    [MemberData("Providers")]
    public virtual void DeleteFile_FileDoesNotExist_ThrowsFileNotFoundException(IStorage fileStorageProvider)
    {
        var filePath = "nonexistentFile.txt";
        Assert.Throws<FileNotFoundException>(() => fileStorageProvider.DeleteFile(filePath));
    }

    [Theory]
    [MemberData("Providers")]
    public virtual void AddFile_ValidPathAndContent_CreatesFileSuccessfully(IStorage fileStorageProvider)
    {
        var filePath = "newFile.txt";
        var content = Encoding.UTF8.GetBytes("file content");
        fileStorageProvider.CreateFile(filePath, content);

        Assert.True(fileStorageProvider.FileExists(filePath));
        Assert.Equal(content, fileStorageProvider.ReadFile(filePath));
    }

    [Theory]
    [MemberData("Providers")]
    public virtual void UpdateFile_FileExists_UpdatesFileContentSuccessfully(IStorage fileStorageProvider)
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
    [MemberData("Providers")]
    public virtual void ReadAllText_FileExists_ReturnsFileContentAsString(IStorage fileStorageProvider)
    {
        var filePath = "fileToRead.txt";
        var content = "file content";
        fileStorageProvider.CreateFile(filePath, Encoding.UTF8.GetBytes(content));
        var result = fileStorageProvider.ReadAllText(filePath);

        Assert.Equal(content, result);
    }

    [Theory]
    [MemberData("Providers")]
    public virtual void WriteAllText_ValidPathAndContent_WritesContentToFile(IStorage fileStorageProvider)
    {
        var filePath = "fileToWrite.txt";
        var content = "new file content";
        fileStorageProvider.WriteAllText(filePath, content);

        Assert.True(fileStorageProvider.FileExists(filePath));
        Assert.Equal(content, fileStorageProvider.ReadAllText(filePath));
    }

    [Theory]
    [MemberData("Providers")]
    public virtual void MoveFile_FileExists_MovesFileSuccessfully(IStorage fileStorageProvider)
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
    [MemberData("Providers")]
    public virtual void MoveFile_FileDoesNotExist_ThrowsFileNotFoundException(IStorage fileStorageProvider)
    {
        var originPath = "nonexistentFile.txt";
        var destinationPath = "destinationFile.txt";
        Assert.Throws<FileNotFoundException>(() => fileStorageProvider.MoveFile(originPath, destinationPath));
    }

    [Theory]
    [MemberData("Providers")]
    public virtual void CopyFile_FileExists_CopiesFileSuccessfully(IStorage fileStorageProvider)
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
    [MemberData("Providers")]
    public virtual void CopyFile_FileDoesNotExist_ThrowsFileNotFoundException(IStorage fileStorageProvider)
    {
        var originPath = "nonexistentFile.txt";
        var destinationPath = "copiedFile.txt";
        Assert.Throws<FileNotFoundException>(() => fileStorageProvider.CopyFile(originPath, destinationPath));
    }

    [Theory]
    [MemberData("Providers")]
    public virtual void CreateDirectory_ValidPath_CreatesDirectorySuccessfully(IStorage fileStorageProvider)
    {
        var directoryPath = "newDirectory";
        fileStorageProvider.CreateDirectory(directoryPath);

        Assert.True(fileStorageProvider.DirectoryExists(directoryPath));
    }

    [Theory]
    [MemberData("Providers")]
    public virtual void DeleteDirectory_DirectoryExists_DeletesDirectorySuccessfully(
        IStorage fileStorageProvider)
    {
        var directoryPath = "directoryToDelete";
        fileStorageProvider.CreateDirectory(directoryPath);
        Assert.True(fileStorageProvider.DirectoryExists(directoryPath));

        fileStorageProvider.DeleteDirectory(directoryPath);

        Assert.False(fileStorageProvider.DirectoryExists(directoryPath));
    }

    [Theory]
    [MemberData("Providers")]
    public virtual void DeleteDirectory_DirectoryDoesNotExist_ThrowsDirectoryNotFoundException(
        IStorage fileStorageProvider)
    {
        var directoryPath = "nonexistentDirectory";
        Assert.Throws<DirectoryNotFoundException>(() => fileStorageProvider.DeleteDirectory(directoryPath));
    }

    [Theory]
    [MemberData("Providers")]
    public virtual void GetFilesInDirectory_DirectoryExists_ReturnsFiles(IStorage fileStorageProvider)
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
    [MemberData("Providers")]
    public virtual void GetDirectoriesInDirectory_DirectoryExists_ReturnsSubdirectories(
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
    [MemberData("Providers")]
    public virtual void IsEmptyDirectory_EmptyDirectory_ReturnsTrue(IStorage fileStorageProvider)
    {
        var directoryPath = "emptyDirectory";
        fileStorageProvider.CreateDirectory(directoryPath);

        var isEmpty = fileStorageProvider.IsEmptyDirectory(directoryPath);

        Assert.True(isEmpty);
    }

    [Theory]
    [MemberData("Providers")]
    public virtual void IsEmptyDirectory_NonEmptyDirectory_ReturnsFalse(IStorage fileStorageProvider)
    {
        var directoryPath = "nonEmptyDirectory";
        fileStorageProvider.CreateDirectory(directoryPath);
        fileStorageProvider.CreateFile(Path.Combine(directoryPath, "file.txt"),
            Encoding.UTF8.GetBytes("content"));

        var isEmpty = fileStorageProvider.IsEmptyDirectory(directoryPath);

        Assert.False(isEmpty);
    }

    [Theory]
    [MemberData("Providers")]
    public virtual void MoveDirectory_DirectoryExists_MovesDirectorySuccessfully(IStorage fileStorageProvider)
    {
        var oldPath = "oldDirectory";
        var newPath = "newDirectory";
        fileStorageProvider.CreateDirectory(oldPath);

        fileStorageProvider.MoveDirectory(oldPath, newPath);

        Assert.False(fileStorageProvider.DirectoryExists(oldPath));
        Assert.True(fileStorageProvider.DirectoryExists(newPath));
    }

    [Theory]
    [MemberData("Providers")]
    public virtual void MoveDirectory_DirectoryDoesNotExist_ThrowsDirectoryNotFoundException(
        IStorage fileStorageProvider)
    {
        var oldPath = "nonexistentDirectory";
        var newPath = "newDirectory";

        Assert.Throws<DirectoryNotFoundException>(() => fileStorageProvider.MoveDirectory(oldPath, newPath));
    }
}