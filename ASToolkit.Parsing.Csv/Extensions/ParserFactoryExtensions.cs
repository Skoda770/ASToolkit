using ASToolkit.Parsing.Enums;
using ASToolkit.Parsing.Infrastructure;

namespace ASToolkit.Parsing.Csv.Extensions;

public static class ParserFactoryExtensions
{
    public static CsvParser GetCsvParser(this ParserFactory factory)
        => factory.GetParser(ParserType.Csv) as CsvParser ?? 
           throw new InvalidOperationException("Parser is not of type CsvParser");
}