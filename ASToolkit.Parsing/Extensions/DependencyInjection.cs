using ASToolkit.Parsing.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace ASToolkit.Parsing.Extensions;

public static class DependencyInjection
{
    public static ParserBuilder AddParsingFactory(this IServiceCollection services)
    {
        services.AddTransient<ParserFactory>();
        return new ParserBuilder(services);
    }
}