using ASToolkit.Parsing.Infrastructure;
using ASToolkit.Parsing.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace ASToolkit.Parsing.Json.Extensions;

public static class DependencyInjection
{
    public static void AddCsvParser(this ParserBuilder builder)
    {
        builder.Services.AddTransient<IParser, JsonParser>();
    }
}