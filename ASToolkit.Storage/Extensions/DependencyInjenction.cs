using ASToolkit.Storage.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace ASToolkit.Storage.Extensions;

public static class DependencyInjection
{
    public static StorageBuilder AddStorageFactory(this IServiceCollection services)
    {
        services.AddTransient<StorageFactory>();
        return new StorageBuilder(services);
    }
}