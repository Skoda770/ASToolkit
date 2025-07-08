using ASToolkit.Parsing.Infrastructure;
using ASToolkit.Parsing.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace ASToolkit.Parsing.Csv.Extensions;

public static class DependencyInjection
{
    public static void AddCsvParser(this ParserBuilder builder)
    {
        builder.Services.AddTransient<IParser, CsvParser>();
        builder.Services.AddTransient<IParser<CsvParserConfig>, CsvParser>();
    }
}