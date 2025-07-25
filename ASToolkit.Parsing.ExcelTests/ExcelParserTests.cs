using ASToolkit.Parsing.Excel;

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
    [Fact]
    public void ParseT_XlsFile_ValidData()
    {
        var parser = new ExcelParser();
        
        var data = parser.Parse<Figure>(File.ReadAllBytes("sample_test_file.xls"));
        
        Assert.NotNull(data);
        Assert.Equal(3, data.Count);
        Assert.Equal("Circle", data[0].Shape);
        Assert.Equal("Producer A", data[0].Producer);
        Assert.Equal("Triangle", data[2].Shape);
        Assert.Equal("Producer C", data[2].Producer);
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