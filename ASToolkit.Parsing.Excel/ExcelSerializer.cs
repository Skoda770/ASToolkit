using ASToolkit.Parsing.Core.Abstracts;
using ASToolkit.Parsing.Core.Enums;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace ASToolkit.Parsing.Excel;

public class ExcelSerializer : SerializerBase
{
    public override SerializerType Type => SerializerType.Excel;

    public override Stream Serialize(IEnumerable<Dictionary<string, object?>> data)
    {
        var dictionaries = data.ToList();
        if (!dictionaries.Any())
            throw new ArgumentException("Data cannot be null or empty", nameof(data));
        var headerKeys = dictionaries.First().Keys.ToList();
        var workbook = CreateWorkbook();
        CreateHeaderRow(workbook.GetSheetAt(0), headerKeys);
        CreateDataRows(workbook.GetSheetAt(0), dictionaries);
        var stream = new MemoryStream();
        workbook.Write(stream, true);
        stream.Position = 0;
        
        return stream;
    }

    private static IWorkbook CreateWorkbook()
    {
        var workbook = new XSSFWorkbook();
        workbook.CreateSheet();
        return workbook;
    }

    private static void CreateHeaderRow(ISheet sheet, IEnumerable<string> headers)
    {
        var headerRow = sheet.CreateRow(0);
        var cellStyle = sheet.Workbook.CreateCellStyle();
        cellStyle.FillForegroundColor = IndexedColors.LightBlue.Index;
        cellStyle.FillPattern = FillPattern.SolidForeground;

        int columnIndex = 0;
        foreach (var header in headers)
        {
            var cell = headerRow.CreateCell(columnIndex++);
            cell.SetCellValue(header);
            cell.CellStyle = cellStyle;
        }
    }

    private static void CreateDataRows(ISheet sheet, IEnumerable<Dictionary<string, object?>> data)
    {
        var rowIndex = 1;
        foreach (var rowData in data)
        {
            var row = sheet.CreateRow(rowIndex++);
            var columnIndex = 0;
            foreach (var value in rowData.Values)
            {
                var cell = row.CreateCell(columnIndex++);
                if (value is DateTime dateTimeValue)
                    cell.SetCellValue(dateTimeValue);
                else
                    cell.SetCellValue(value?.ToString() ?? string.Empty);
            }
        }
    }
}