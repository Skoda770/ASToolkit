using ASToolkit.Parsing.Core.Enums;
using ASToolkit.Parsing.Core.Interfaces;

namespace ASToolkit.Parsing.Core.Infrastructure;

public class ParserFactory(IEnumerable<IParser> parsers)
{
    public IParser GetParser(ParserType type)
        => parsers.FirstOrDefault(parser => parser.Type == type)
           ?? throw new ArgumentException($"Invalid parser type: {type}", nameof(type));

    public IParser GetParser(string extension)
        => extension switch
        {
            ".xlsx" => GetParser(ParserType.Excel),
            ".xls" => GetParser(ParserType.Excel),
            ".csv" => GetParser(ParserType.Csv),
            ".json" => GetParser(ParserType.Json),
            _ => throw new ArgumentException($"No parser found for extension: {extension}", nameof(extension))
        };
}