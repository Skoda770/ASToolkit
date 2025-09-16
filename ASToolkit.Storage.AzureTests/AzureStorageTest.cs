using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASToolkit.Storage.Azure;
using ASToolkit.Storage.Interfaces;
using ASToolkit.Storage.Tests;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ASToolkit.Storage.AzureTests;

[TestSubject(typeof(AzureStorage))]
public class AzureStorageTest : BaseStorageTest
{
    public static string ContainerName => $"tests-{System.Guid.NewGuid()}".ToLower();

    public static TheoryData<AzureStorage> Providers() => new()
    {
        new AzureStorage(CreateLogger(), CreateOptions())
    };

    private static StorageOptions CreateOptions()
    {
        return new StorageOptions
        {
            ConnectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING") ??
                               throw new InvalidOperationException(
                                   "AZURE_STORAGE_CONNECTION_STRING environment variable is not set."),
            ContainerName = ContainerName
        };
    }


    public override Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public override Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    public override void RenameFile_ValidPathAndNewFileName_RenamesFileSuccessfully(IStorage fileStorageProvider)
    {
        (fileStorageProvider as AzureStorage)!.CreateContainerIfNotExists();
        base.RenameFile_ValidPathAndNewFileName_RenamesFileSuccessfully(fileStorageProvider);
        (fileStorageProvider as AzureStorage)!.DeleteContainerIfExists();
    }

    public override void RenameFile_FileDoesNotExist_ThrowsFileNotExistsException(IStorage fileStorageProvider)
    {
        (fileStorageProvider as AzureStorage)!.CreateContainerIfNotExists();
        base.RenameFile_FileDoesNotExist_ThrowsFileNotExistsException(fileStorageProvider);
        (fileStorageProvider as AzureStorage)!.DeleteContainerIfExists();
    }

    public override void RenameFile_NewFileNameAlreadyExists_ThrowsFileExistsException(IStorage fileStorageProvider)
    {
        (fileStorageProvider as AzureStorage)!.CreateContainerIfNotExists();
        base.RenameFile_NewFileNameAlreadyExists_ThrowsFileExistsException(fileStorageProvider);
        (fileStorageProvider as AzureStorage)!.DeleteContainerIfExists();
    }

    public override void TryRenameFile_ValidPathAndNewFileName_ReturnsTrue(IStorage fileStorageProvider)
    {
        (fileStorageProvider as AzureStorage)!.CreateContainerIfNotExists();
        base.TryRenameFile_ValidPathAndNewFileName_ReturnsTrue(fileStorageProvider);
        (fileStorageProvider as AzureStorage)!.DeleteContainerIfExists();
    }

    public override void TryRenameFile_FileDoesNotExist_ReturnsFalse(IStorage fileStorageProvider)
    {
        (fileStorageProvider as AzureStorage)!.CreateContainerIfNotExists();
        base.TryRenameFile_FileDoesNotExist_ReturnsFalse(fileStorageProvider);
        (fileStorageProvider as AzureStorage)!.DeleteContainerIfExists();
    }

    public override void TryRenameFile_NewFileNameAlreadyExists_ReturnsFalse(IStorage fileStorageProvider)
    {
        (fileStorageProvider as AzureStorage)!.CreateContainerIfNotExists();
        base.TryRenameFile_NewFileNameAlreadyExists_ReturnsFalse(fileStorageProvider);
        (fileStorageProvider as AzureStorage)!.DeleteContainerIfExists();
    }

    public override void DeleteFile_FileExists_DeletesFileSuccessfully(IStorage fileStorageProvider)
    {
        (fileStorageProvider as AzureStorage)!.CreateContainerIfNotExists();
        base.DeleteFile_FileExists_DeletesFileSuccessfully(fileStorageProvider);
        (fileStorageProvider as AzureStorage)!.DeleteContainerIfExists();
    }

    public override void DeleteFile_FileDoesNotExist_ThrowsFileNotFoundException(IStorage fileStorageProvider)
    {
        (fileStorageProvider as AzureStorage)!.CreateContainerIfNotExists();
        base.DeleteFile_FileDoesNotExist_ThrowsFileNotFoundException(fileStorageProvider);
        (fileStorageProvider as AzureStorage)!.DeleteContainerIfExists();
    }

    public override void AddFile_ValidPathAndContent_CreatesFileSuccessfully(IStorage fileStorageProvider)
    {
        (fileStorageProvider as AzureStorage)!.CreateContainerIfNotExists();
        base.AddFile_ValidPathAndContent_CreatesFileSuccessfully(fileStorageProvider);
        (fileStorageProvider as AzureStorage)!.DeleteContainerIfExists();
    }

    public override void UpdateFile_FileExists_UpdatesFileContentSuccessfully(IStorage fileStorageProvider)
    {
        (fileStorageProvider as AzureStorage)!.CreateContainerIfNotExists();
        base.UpdateFile_FileExists_UpdatesFileContentSuccessfully(fileStorageProvider);
        (fileStorageProvider as AzureStorage)!.DeleteContainerIfExists();
    }

    public override void ReadAllText_FileExists_ReturnsFileContentAsString(IStorage fileStorageProvider)
    {
        (fileStorageProvider as AzureStorage)!.CreateContainerIfNotExists();
        base.ReadAllText_FileExists_ReturnsFileContentAsString(fileStorageProvider);
        (fileStorageProvider as AzureStorage)!.DeleteContainerIfExists();
    }

    public override void WriteAllText_ValidPathAndContent_WritesContentToFile(IStorage fileStorageProvider)
    {
        (fileStorageProvider as AzureStorage)!.CreateContainerIfNotExists();
        base.WriteAllText_ValidPathAndContent_WritesContentToFile(fileStorageProvider);
        (fileStorageProvider as AzureStorage)!.DeleteContainerIfExists();
    }

    public override void MoveFile_FileExists_MovesFileSuccessfully(IStorage fileStorageProvider)
    {
        (fileStorageProvider as AzureStorage)!.CreateContainerIfNotExists();
        base.MoveFile_FileExists_MovesFileSuccessfully(fileStorageProvider);
        (fileStorageProvider as AzureStorage)!.DeleteContainerIfExists();
    }

    public override void MoveFile_FileDoesNotExist_ThrowsFileNotFoundException(IStorage fileStorageProvider)
    {
        (fileStorageProvider as AzureStorage)!.CreateContainerIfNotExists();
        base.MoveFile_FileDoesNotExist_ThrowsFileNotFoundException(fileStorageProvider);
        (fileStorageProvider as AzureStorage)!.DeleteContainerIfExists();
    }

    public override void CopyFile_FileExists_CopiesFileSuccessfully(IStorage fileStorageProvider)
    {
        (fileStorageProvider as AzureStorage)!.CreateContainerIfNotExists();
        base.CopyFile_FileExists_CopiesFileSuccessfully(fileStorageProvider);
        (fileStorageProvider as AzureStorage)!.DeleteContainerIfExists();
    }

    public override void CopyFile_FileDoesNotExist_ThrowsFileNotFoundException(IStorage fileStorageProvider)
    {
        (fileStorageProvider as AzureStorage)!.CreateContainerIfNotExists();
        base.CopyFile_FileDoesNotExist_ThrowsFileNotFoundException(fileStorageProvider);
        (fileStorageProvider as AzureStorage)!.DeleteContainerIfExists();
    }

    public override void CreateDirectory_ValidPath_CreatesDirectorySuccessfully(IStorage fileStorageProvider)
    {
        (fileStorageProvider as AzureStorage)!.CreateContainerIfNotExists();
        base.CreateDirectory_ValidPath_CreatesDirectorySuccessfully(fileStorageProvider);
        (fileStorageProvider as AzureStorage)!.DeleteContainerIfExists();
    }

    public override void DeleteDirectory_DirectoryExists_DeletesDirectorySuccessfully(IStorage fileStorageProvider)
    {
        (fileStorageProvider as AzureStorage)!.CreateContainerIfNotExists();
        base.DeleteDirectory_DirectoryExists_DeletesDirectorySuccessfully(fileStorageProvider);
        (fileStorageProvider as AzureStorage)!.DeleteContainerIfExists();
    }

    public override void DeleteDirectory_DirectoryDoesNotExist_ThrowsDirectoryNotFoundException(IStorage fileStorageProvider)
    {
        (fileStorageProvider as AzureStorage)!.CreateContainerIfNotExists();
        base.DeleteDirectory_DirectoryDoesNotExist_ThrowsDirectoryNotFoundException(fileStorageProvider);
        (fileStorageProvider as AzureStorage)!.DeleteContainerIfExists();
    }

    public override void GetFilesInDirectory_DirectoryExists_ReturnsFiles(IStorage fileStorageProvider)
    {
        (fileStorageProvider as AzureStorage)!.CreateContainerIfNotExists();
        base.GetFilesInDirectory_DirectoryExists_ReturnsFiles(fileStorageProvider);
        (fileStorageProvider as AzureStorage)!.DeleteContainerIfExists();
    }

    public override void GetDirectoriesInDirectory_DirectoryExists_ReturnsSubdirectories(IStorage fileStorageProvider)
    {
        (fileStorageProvider as AzureStorage)!.CreateContainerIfNotExists();
        base.GetDirectoriesInDirectory_DirectoryExists_ReturnsSubdirectories(fileStorageProvider);
        (fileStorageProvider as AzureStorage)!.DeleteContainerIfExists();
    }

    public override void IsEmptyDirectory_EmptyDirectory_ReturnsTrue(IStorage fileStorageProvider)
    {
        (fileStorageProvider as AzureStorage)!.CreateContainerIfNotExists();
        base.IsEmptyDirectory_EmptyDirectory_ReturnsTrue(fileStorageProvider);
        (fileStorageProvider as AzureStorage)!.DeleteContainerIfExists();
    }

    public override void IsEmptyDirectory_NonEmptyDirectory_ReturnsFalse(IStorage fileStorageProvider)
    {
        (fileStorageProvider as AzureStorage)!.CreateContainerIfNotExists();
        base.IsEmptyDirectory_NonEmptyDirectory_ReturnsFalse(fileStorageProvider);
        (fileStorageProvider as AzureStorage)!.DeleteContainerIfExists();
    }

    public override void MoveDirectory_DirectoryExists_MovesDirectorySuccessfully(IStorage fileStorageProvider)
    {
        (fileStorageProvider as AzureStorage)!.CreateContainerIfNotExists();
        base.MoveDirectory_DirectoryExists_MovesDirectorySuccessfully(fileStorageProvider);
        (fileStorageProvider as AzureStorage)!.DeleteContainerIfExists();
    }

    public override void MoveDirectory_DirectoryDoesNotExist_ThrowsDirectoryNotFoundException(IStorage fileStorageProvider)
    {
        (fileStorageProvider as AzureStorage)!.CreateContainerIfNotExists();
        base.MoveDirectory_DirectoryDoesNotExist_ThrowsDirectoryNotFoundException(fileStorageProvider);
        (fileStorageProvider as AzureStorage)!.DeleteContainerIfExists();
    }
}