using Microsoft.Extensions.DependencyInjection;

namespace ASToolkit.Parsing.Infrastructure;

public class ParserBuilder(IServiceCollection services)
{
    public readonly IServiceCollection Services = services;
}