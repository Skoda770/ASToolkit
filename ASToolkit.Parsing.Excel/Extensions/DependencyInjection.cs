using ASToolkit.Parsing.Infrastructure;
using ASToolkit.Parsing.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace ASToolkit.Parsing.Excel.Extensions;

public static class DependencyInjection
{
    public static void AddExcelParser(this ParserBuilder builder)
    {
        builder.Services.AddTransient<IParser, ExcelParser>();
        builder.Services.AddTransient<IParser<ExcelParserConfig>, ExcelParser>();
    }
}