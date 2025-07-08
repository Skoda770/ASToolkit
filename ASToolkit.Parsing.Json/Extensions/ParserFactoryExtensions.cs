using ASToolkit.Parsing.Enums;
using ASToolkit.Parsing.Infrastructure;

namespace ASToolkit.Parsing.Json.Extensions;

public static class ParserFactoryExtensions
{
    public static JsonParser GetCsvParser(this ParserFactory factory)
        => factory.GetParser(ParserType.Json) as JsonParser ?? 
           throw new InvalidOperationException("Parser is not of type JsonParser");
    
}