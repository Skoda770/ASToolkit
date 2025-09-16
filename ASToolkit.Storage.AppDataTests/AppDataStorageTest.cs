using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ASToolkit.Storage.Desktop;
using ASToolkit.Storage.Interfaces;
using ASToolkit.Storage.Tests;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ASToolkit.Storage.DesktopTests;

[TestSubject(typeof(AppDataStorage))]
public class AppDataStorageTest : BaseStorageTest
{
    private static string ApplicationName => "ASToolkit.Storage.AppDataTests";

    public static TheoryData<IStorage> Providers =>
        new()
        {
            new AppDataStorage(CreateLogger(), new()
            {
                ApplicationName = ApplicationName
            })
        };

    public override Task InitializeAsync()
    {
        var root = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            ApplicationName);
        Directory.CreateDirectory(root);

        return Task.CompletedTask;
    }

    public override Task DisposeAsync()
    {
        var root = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            ApplicationName);
        if (Directory.Exists(root))
            Directory.Delete(root, true);

        return Task.CompletedTask;
    }
}