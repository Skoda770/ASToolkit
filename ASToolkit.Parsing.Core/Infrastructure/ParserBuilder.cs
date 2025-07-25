using Microsoft.Extensions.DependencyInjection;

namespace ASToolkit.Parsing.Core.Infrastructure;

public class ParserBuilder(IServiceCollection services)
{
    public readonly IServiceCollection Services = services;
}