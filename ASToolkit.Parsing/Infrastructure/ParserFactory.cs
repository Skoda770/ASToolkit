using ASToolkit.Parsing.Enums;
using ASToolkit.Parsing.Interfaces;

namespace ASToolkit.Parsing.Infrastructure;

public class ParserFactory(IEnumerable<IParser> parsers)
{
    public IParser GetParser(ParserType type)
        => parsers.FirstOrDefault(parser => parser.Type == type)
           ?? throw new ArgumentException($"Invalid parser type: {type}", nameof(type));
}