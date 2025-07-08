using Microsoft.Extensions.DependencyInjection;

namespace ASToolkit.Storage.Infrastructure;

public class StorageBuilder(IServiceCollection services)
{
    public readonly IServiceCollection Services = services;
}