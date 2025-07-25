using ASToolkit.Parsing.Excel;
using JetBrains.Annotations;

namespace ASToolkit.Parsing.ExcelTests;

[TestSubject(typeof(ExcelSerializer))]
public class ExcelSerializerTest
{
    private readonly ExcelParser _parser = new ExcelParser();
    private readonly ExcelSerializer _serializer = new ExcelSerializer();
    
    [Fact]
    public void Serialize_ValidData_ShouldReturnStream()
    {
        var data = new List<Person>
        {
            new() { Name = "John Doe", Age = 30, Location = "New York" },
            new() { Name = "Jane Smith", Age = 25, Location = "Los Angeles" }
        };
        
        using var stream = _serializer.Serialize(data);
        Assert.NotNull(stream);
        Assert.True(stream.Length > 0);
        Assert.NotEmpty(_parser.Parse(stream));
    }
    [Fact]
    public void Serialize_EmptyGenericCollection_ShouldThrowsException()
    {
        Assert.Throws<ArgumentException>(() => _serializer.Serialize(new List<Person>()));
    }
}