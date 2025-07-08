using System.Text;
using ASToolkit.Parsing.Csv;

namespace ASToolkit.Parsing.CsvTests;

public class CsvParserTests
{
    [Fact]
    public void GetTableColumnProperties_ReturnsCorrectHeaders()
    {
        var csvContent = "Name,Age,Location\nJohn,30,USA";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));
        var parser = new CsvParser();

        var result = parser.GetFieldsProperties(stream);

        Assert.Equal(3, result.Count);
        Assert.Equal("Name", result[0].Name);
        Assert.Equal("Age", result[1].Name);
        Assert.Equal("Location", result[2].Name);
    }

    [Fact]
    public void Parse_Generic_ReturnsCorrectRecords()
    {
        var csvContent = "Name,Age,Location\nJohn,30,USA\nJane,25,UK";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));
        var parser = new CsvParser();

        var result = parser.Parse<Person>(stream);

        Assert.Equal(2, result.Count);
        Assert.Equal("John", result[0].Name);
        Assert.Equal(30, result[0].Age);
        Assert.Equal("USA", result[0].Location);
        Assert.Equal("Jane", result[1].Name);
        Assert.Equal(25, result[1].Age);
        Assert.Equal("UK", result[1].Location);
    }

    [Fact]
    public void Parse_Dictionary_ReturnsCorrectRecords()
    {
        var csvContent = "Name,Age,Location\nJohn,30,USA\nJane,25,UK";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));
        var parser = new CsvParser();

        var result = parser.Parse(stream);

        Assert.Equal(2, result.Count);
        Assert.Equal("John", result[0]["Name"]);
        Assert.Equal("30", result[0]["Age"]);
        Assert.Equal("USA", result[0]["Location"]);
        Assert.Equal("Jane", result[1]["Name"]);
        Assert.Equal("25", result[1]["Age"]);
        Assert.Equal("UK", result[1]["Location"]);
    }

    [Fact]
    public void Parse_EmptyStream_ReturnsEmptyList()
    {
        var csvContent = "";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));
        var parser = new CsvParser();

        var result = parser.Parse<Person>(stream);

        Assert.Empty(result);
    }

    [Fact]
    public void Parse_InvalidCsvFormat_ThrowsException()
    {
        var csvContent = "Name,Age,Location\nJohn,30";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));
        var parser = new CsvParser();

        Assert.Throws<CsvHelper.MissingFieldException>(() => parser.Parse<Person>(stream));
    }

}