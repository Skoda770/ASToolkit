using NPOI.SS.UserModel;

namespace ASToolkit.Parsing.Excel.Extensions;

public static class NpoiExtensions
{
    public static object? GetCellValue(this ICell cell)
    {
        return GetCellValue(cell, cell.CellType);
    }

    private static object? GetCellValue(ICell cell, CellType type)
    {
        return type switch
        {
            CellType.Numeric => DateUtil.IsCellDateFormatted(cell) ? cell.DateCellValue : cell.NumericCellValue,
            CellType.Boolean => cell.BooleanCellValue,
            CellType.String => string.IsNullOrEmpty(cell.StringCellValue) ? null : cell.StringCellValue,
            CellType.Formula => GetCellValue(cell, cell.CachedFormulaResultType),
            _ => null
        };
    }

    public static bool IsNullOrEmpty(this ICell cell) => string.IsNullOrEmpty(cell.GetCellValue()?.ToString() ?? "");
}