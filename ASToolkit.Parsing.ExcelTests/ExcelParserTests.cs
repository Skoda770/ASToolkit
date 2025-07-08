using ASToolkit.Parsing.Excel;
using ASToolkit.Parsing.Excel.Extensions;
using ASToolkit.Parsing.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace ASToolkit.Parsing.ExcelTests;

public class ExcelParserTests
{
    [Theory]
    [InlineData("sample_test_file.xls", 3)]
    [InlineData("sample_test_file_hiddenColumns.xls", 3)]
    public void Correct_Xls_ReadFile(string filePath, int rows)
    {
        var parser = new ExcelParser();
        var data = parser.Parse(File.ReadAllBytes(filePath));
        Assert.Equal(rows, data.Count);
    }

    [Fact]
    public void Invalid_XlsSheet_InvalidSheetName()
    {
        var config = new ExcelParserConfig()
        {
            Sheet = "Invalid sheet"
        };
        var parser = new ExcelParser();
        parser.SetConfig(config);
        Assert.Throws<ArgumentException>(() =>
            parser.Parse(File.ReadAllBytes("sample_test_file.xls")));
    }

    [Theory]
    [InlineData("sample_test_file.xls", new[] { "Shape", "Producer", "Description", "Name" })]
    [InlineData("sample_test_file_hiddenColumns.xls",
        new[] { "Shape", "Producer", "Description", "Name", "Color" })]
    public void Valid_XlsTable_CheckTableColumnsProperties(string filePath, string[] headerColumns)
    {
        var parser = new ExcelParser();
        var properties = parser.GetFieldsProperties(File.ReadAllBytes(filePath));
        Assert.Equal(headerColumns, properties.Select(e => e.Name).ToArray());
    }
}