using ASToolkit.Parsing.Enums;
using ASToolkit.Parsing.Infrastructure;

namespace ASToolkit.Parsing.Excel.Extensions;

public static class ParserFactoryExtensions
{
    public static ExcelParser GetExcelParser(this ParserFactory factory)
        => factory.GetParser(ParserType.Excel) as ExcelParser ?? 
           throw new InvalidOperationException("Parser is not of type ExcelParser");
}