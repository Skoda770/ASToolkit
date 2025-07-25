using ASToolkit.Parsing.Core.Infrastructure;
using ASToolkit.Parsing.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace ASToolkit.Parsing.Excel.Extensions;

public static class DependencyInjection
{
    public static void UseExcelParser(this ParserBuilder builder)
    {
        builder.Services.AddTransient<IParser, ExcelParser>();
    }
    public static void UseExcelSerializer(this SerializerBuilder builder)
    {
        builder.Services.AddTransient<ISerializer, ExcelSerializer>();
    }
}