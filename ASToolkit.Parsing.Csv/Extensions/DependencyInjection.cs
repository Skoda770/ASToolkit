using ASToolkit.Parsing.Core.Infrastructure;
using ASToolkit.Parsing.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace ASToolkit.Parsing.Csv.Extensions;

public static class DependencyInjection
{
    public static void UseCsvParser(this ParserBuilder builder)
    {
        builder.Services.AddTransient<IParser, CsvParser>();
    }
    public static void UseCsvSerializer(this SerializerBuilder builder)
    {
        builder.Services.AddTransient<ISerializer, CsvSerializer>();
    }
}