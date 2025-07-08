using ASToolkit.Storage.Infrastructure;
using ASToolkit.Storage.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace ASToolkit.Storage.System.Extensions;

public static class DependencyInjection
{
    public static StorageBuilder AddSystemStorage(this StorageBuilder builder, Action<StorageOptions> configureOptions)
    {
        var options = new StorageOptions();
        configureOptions(options);
        builder.Services.AddSingleton(options);
        builder.Services.AddTransient<IStorage, SystemStorage>();
        
        return builder;
    }
}