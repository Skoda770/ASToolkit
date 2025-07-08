using ASToolkit.Parsing.Abstracts;
using ASToolkit.Parsing.Enums;
using ASToolkit.Parsing.Excel.Extensions;
using ASToolkit.Parsing.Interfaces;
using ASToolkit.Parsing.Models;
using NPOI.HSSF.Record.AutoFilter;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace ASToolkit.Parsing.Excel;

public class ExcelParser : ParserBase, IParser<ExcelParserConfig>
{
    private ExcelParserConfig _config = new();
    public void SetConfig(ExcelParserConfig config)
    {
        _config = config;
    }

    public override ParserType Type => ParserType.Excel;
    public override List<T>? Parse<T>(Stream stream)
    {
        throw new NotImplementedException();
    }

    public override List<Dictionary<string, object?>> Parse(Stream stream)
    {
        var worksheet = GetExcelWorksheet(stream);
        PrepareConfigWithWorksheet(worksheet);
        var result = GetRows(worksheet,_config.CustomRowsIndexes);

        return result;
    }

    private void PrepareConfigWithWorksheet(ISheet worksheet)
    {
        _config.StartRow--;
        _config.StartColumn--; // NPOI uses 0-based index for rows and columns
        _config.EndColumn ??= GetTableColumnDimension(worksheet, _config.StartColumn, _config.StartRow) + _config.StartColumn;
        _config.EndRow ??= _config.StartRow + GetRowsCount(worksheet);
        _config.ExcelColumns ??= GetTableHeader(worksheet);
        _config.CustomRowsIndexes ??= [..Enumerable.Range(_config.StartRow + 1, _config.EndRow.Value - _config.StartRow)];
    }
    private List<Dictionary<string, object?>> GetRows(ISheet worksheet, HashSet<int>? rowsIndexes)
    {
        var result = new List<Dictionary<string, object?>>();

        foreach (var rowIndex in rowsIndexes ?? [])
        {
            var worksheetRow = worksheet.GetRow(rowIndex);
            if (IsNullExcelRow(worksheet, worksheetRow) ||
                worksheetRow.RowStyle?.IsHidden == true) break;
            var row = _config.ExcelColumns!
                .Select(header =>
                    new KeyValuePair<string, object?>(header.Value, worksheetRow.GetCell(header.Key)?.GetCellValue()))
                .ToDictionary(e => e.Key, e => e.Value);
            if (row.Any(e => e.Value != null))
                result.Add(row);
        }

        return result;
    }

    private List<KeyValuePair<int, string>> GetTableHeader(ISheet worksheet)
    {
        var result = new List<KeyValuePair<int, string>>();
        var row = worksheet.GetRow(_config.StartRow);
        for (var columnIndex = _config.StartColumn; columnIndex <= _config.EndColumn; columnIndex++)
        {
            if (worksheet.IsColumnHidden(columnIndex)) continue;
            result.Add(new KeyValuePair<int, string>(columnIndex, row.GetCell(columnIndex).StringCellValue));
        }

        return result;
    }
    private int GetRowsCount(ISheet worksheet)
    {
        var count = 0;
        var dimensionRows = worksheet.LastRowNum - worksheet.FirstRowNum + 1;
        if (_config.StartRow >= worksheet.FirstRowNum - 1) dimensionRows += _config.StartRow;

        if (IsTableHasFilter(worksheet, dimensionRows))
        {
            var sheet = worksheet as HSSFSheet;
            var customRowsIndexes = new HashSet<int>();
            for (var i = _config.StartRow;
                 i <= dimensionRows;
                 i++) 
            {
                var row = sheet!.GetRow(i);
                if (IsNullExcelRow(worksheet, row) ||
                    row.RowStyle?.IsHidden == true || row.ZeroHeight)
                    continue;
                customRowsIndexes.Add(i);
            }
            _config.CustomRowsIndexes = customRowsIndexes;
            return customRowsIndexes.Count;
        }

        for (var i = _config.StartRow; i <= dimensionRows; i++)
        {
            var row = worksheet.GetRow(i);
            if (IsNullExcelRow(worksheet, row) ||
                row.RowStyle?.IsHidden == true) break;
            count++;
        }

        count -= 1;
        return count < 0 ? 0 : count;
    }
    private bool IsNullExcelRow(ISheet worksheet, IRow? row)
    {
        if (row is null) return true;
        return !row.Cells.Exists(cell =>
        {
            if (cell.ColumnIndex < _config.StartColumn || cell.ColumnIndex > _config.EndColumn!.Value) return false;
            var colStyle = worksheet.GetColumnStyle(cell.ColumnIndex);
            var isColumnHidden = worksheet.IsColumnHidden(cell.ColumnIndex);
            return cell.GetCellValue() is not null && !isColumnHidden && colStyle?.IsHidden != true;
        });
    }
    private ISheet GetExcelWorksheet(Stream stream)
    {
        var workbook = new HSSFWorkbook(stream);
        var worksheet = string.IsNullOrEmpty(_config.Sheet) ? workbook.GetSheetAt(0) : workbook.GetSheet(_config.Sheet);

        if (worksheet == null)
            throw new ArgumentException("Sheet not found");

        return worksheet;
    }
    private bool IsTableHasFilter(ISheet worksheet, int dimensionRows)
    {
        if (worksheet is not HSSFSheet sheet) return false;
        
        var filterExists = sheet.Sheet.Records.Exists(record => record is AutoFilterInfoRecord);
        if (!filterExists) return false;
        for (var i = _config.StartRow; i <= dimensionRows; i++)
        {
            var row = sheet.GetRow(i);
            if (row is null) return false;
            if (row.ZeroHeight) return true;
        }

        return false;
    }
    private static int GetTableColumnDimension(ISheet worksheet, int startColumn, int startRow)
    {
        return worksheet.GetRow(startRow).LastCellNum - (startColumn + 1);
    }
    private Dictionary<int, List<ICell>> GetColumns(ISheet worksheet)
    {
        var result = new Dictionary<int, List<ICell>>();

        for (var columnIndex = _config.StartColumn; columnIndex <= _config.EndColumn; columnIndex++)
        {
            if (worksheet.IsColumnHidden(columnIndex)) continue;
            result.Add(columnIndex, []);
        }

        foreach (var rowIndex in _config.CustomRowsIndexes ?? [])
        {
            var row = worksheet.GetRow(rowIndex);
            if (IsNullExcelRow(worksheet, row)) break;
            foreach (var cell in row.Where(cell => result.ContainsKey(cell.ColumnIndex)))
                result[cell.ColumnIndex].Add(cell);
        }

        return result.Where(e => e.Value.Count != 0).ToDictionary(e => e.Key, e => e.Value);
    }
    public override List<FieldProperties> GetFieldsProperties(Stream stream)
    {
        var worksheet = GetExcelWorksheet(stream);
        PrepareConfigWithWorksheet(worksheet);

        var columns = GetColumns(worksheet);

        return columns.Select(col =>
        {
            var filledCells = col.Value.Where(cell => !cell.IsNullOrEmpty()).ToList();
            return new FieldProperties()
            {
                Key =  col.Key,
                Name = worksheet.GetRow(_config.StartRow).GetCell(col.Key).GetCellValue()?.ToString(),
                LongestValueLength = filledCells.Max(cell => cell.GetCellValue()?.ToString()?.Length) ?? 0,
                LongestValue = filledCells.OrderByDescending(cell => cell.GetCellValue()?.ToString()?.Length)
                    .FirstOrDefault()?.GetCellValue()?.ToString(),
                IsAllCellsFilled = filledCells.Count == col.Value.Count,
                IsAllCellsInteger = filledCells.TrueForAll(cell => int.TryParse(cell.GetCellValue()?.ToString(), out _)),
                IsAllCellsNumber = filledCells.TrueForAll(cell => decimal.TryParse(cell.GetCellValue()?.ToString(), out _)),
                IsAllCellsBool = filledCells.TrueForAll(cell => bool.TryParse(cell.GetCellValue()?.ToString(), out _))
            };
        }).ToList();
    }
}