using ASToolkit.Parsing.Core.Infrastructure;
using ASToolkit.Parsing.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace ASToolkit.Parsing.Json.Extensions;

public static class DependencyInjection
{
    public static void UseJsonParser(this ParserBuilder builder)
    {
        builder.Services.AddTransient<IParser, JsonParser>();
    }
    public static void UseJsonSerializer(this SerializerBuilder builder)
    {
        builder.Services.AddTransient<ISerializer, JsonSerializer>();
    }
}