using ASToolkit.Parsing.Core.Infrastructure;
using ASToolkit.Parsing.Csv.Extensions;
using ASToolkit.Parsing.Excel.Extensions;
using ASToolkit.Parsing.Json.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace ASToolkit.Parsing.Extensions;

public static class DependencyInjection
{
    public static void AddParsingFactory(this IServiceCollection services, Action<ParserBuilder>? configure = null)
    {
        var builder = new ParserBuilder(services);
        services.AddTransient<ParserFactory>();
        configure?.Invoke(builder);
    }

    public static void AddSerializerFactory(this IServiceCollection services,
        Action<SerializerBuilder>? configure = null)
    {
        var builder = new SerializerBuilder(services);
        services.AddTransient<SerializerFactory>();
        configure?.Invoke(builder);
    }
    public static void ConfigureParsingToolkit(this IServiceCollection services)
    {
        services.AddParsingFactory(builder =>
        {
            builder.UseExcelParser();
            builder.UseCsvParser();
            builder.UseJsonParser();
        });
        services.AddSerializerFactory(builder =>
        {
            builder.UseExcelSerializer();
            builder.UseCsvSerializer();
            builder.UseJsonSerializer();
        });
    }
}