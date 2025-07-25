using Microsoft.Extensions.DependencyInjection;

namespace ASToolkit.Parsing.Core.Infrastructure;

public class SerializerBuilder(IServiceCollection services)
{
    public readonly IServiceCollection Services = services;
}