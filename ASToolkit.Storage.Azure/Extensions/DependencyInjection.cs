using ASToolkit.Storage.Infrastructure;
using ASToolkit.Storage.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace ASToolkit.Storage.Azure.Extensions;

public static class DependencyInjection
{
    public static StorageBuilder AddAzureBlobStorage(this StorageBuilder builder, Action<StorageOptions> configureOptions)
    {
        var options = new StorageOptions();
        configureOptions(options);
        builder.Services.AddSingleton(options);
        builder.Services.AddTransient<IStorage, AzureStorage>();
        
        return builder;
    }
    
}