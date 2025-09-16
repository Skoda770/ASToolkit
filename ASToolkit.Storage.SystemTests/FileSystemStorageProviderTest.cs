using System.Text;
using ASToolkit.Storage.Interfaces;
using ASToolkit.Storage.System;
using ASToolkit.Storage.Tests;
using Microsoft.Extensions.Logging;
using Moq;

namespace ASToolkit.Storage.SystemTests;

public class FileSystemStorageProviderTest : BaseStorageTest
{
    private string? _testDirectory;


    public static TheoryData<IStorage> Providers => new()
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
    public override Task InitializeAsync()
    {
        _testDirectory = Path.Combine(Directory.GetCurrentDirectory(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_testDirectory);
        Directory.SetCurrentDirectory(_testDirectory);
        if (Directory.Exists("root"))
            Directory.Delete("root", true);

        Directory.CreateDirectory("root");

        return Task.CompletedTask;
    }

    public override Task DisposeAsync()
    {
        Directory.SetCurrentDirectory(Directory.GetParent(_testDirectory!)!.FullName);
        if (Directory.Exists(_testDirectory))
        {
            Directory.Delete(_testDirectory, true);
        }

        return Task.CompletedTask;
    }
}