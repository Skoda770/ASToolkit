using System.Collections.Generic;
using System.IO;
using System.Text;
using ASToolkit.Parsing.Json;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Xunit;

namespace ASToolkit.Parsing.JsonTests;

[TestSubject(typeof(JsonParser))]
public class JsonParserTest
{
    [Fact]
    public void GetTableColumnProperties_ReturnsEmptyList_WhenJsonIsEmpty()
    {
        var parser = new JsonParser();
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("[]"));

        var result = parser.GetFieldsProperties(stream);

        Assert.Empty(result);
    }

    [Fact]
    public void GetTableColumnProperties_ReturnsCorrectProperties_ForValidJson()
    {
        var parser = new JsonParser();
        var json = "[{\"Name\":\"John\",\"Age\":30},{\"Name\":\"Jane\",\"Age\":25}]";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));

        var result = parser.GetFieldsProperties(stream);

        Assert.Equal(2, result.Count);
        Assert.Equal("Name", result[0].Name);
        Assert.Equal("John", result[0].LongestValue);
        Assert.Equal(4, result[0].LongestValueLength);
        Assert.True(result[0].IsAllCellsFilled);
        Assert.False(result[0].IsAllCellsBool);
        Assert.False(result[0].IsAllCellsInteger);
        Assert.False(result[0].IsAllCellsNumber);

        Assert.Equal("Age", result[1].Name);
        Assert.Equal("30", result[1].LongestValue);
        Assert.Equal(2, result[1].LongestValueLength);
        Assert.True(result[1].IsAllCellsFilled);
        Assert.False(result[1].IsAllCellsBool);
        Assert.True(result[1].IsAllCellsInteger);
        Assert.True(result[1].IsAllCellsNumber);
    }

    [Fact]
    public void ParseT_ReturnsDeserializedObjects_ForValidJson()
    {
        var parser = new JsonParser();
        var json = "[{\"Name\":\"John\",\"Age\":30},{\"Name\":\"Jane\",\"Age\":25}]";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));

        var result = parser.Parse<Dictionary<string, object>>(stream);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("John", result[0]["Name"]);
        Assert.Equal(30L, result[0]["Age"]);
        Assert.Equal("Jane", result[1]["Name"]);
        Assert.Equal(25L, result[1]["Age"]);
    }

    [Fact]
    public void Parse_ReturnsEmptyList_WhenJsonIsEmpty()
    {
        var parser = new JsonParser();
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("[]"));

        var result = parser.Parse(stream);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void Parse_ThrowsJsonReaderException_ForInvalidJson()
    {
        var parser = new JsonParser();
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("{invalid json}"));

        Assert.Throws<JsonReaderException>(() => parser.Parse(stream));
    }

    [Fact]
    public void ParseT_ReturnsDeserializedUserObjects_ForValidJson()
    {
        var parser = new JsonParser();
        var json = "[{\"Username\":\"JohnDoe\",\"Description\":\"A user\",\"Email\":\"john.doe@example.com\",\"Birthday\":1990}," +
                   "{\"Username\":\"JaneDoe\",\"Description\":null,\"Email\":\"jane.doe@example.com\",\"Birthday\":null}]";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));

        var result = parser.Parse<User>(stream);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);

        Assert.Equal("JohnDoe", result[0].Username);
        Assert.Equal("A user", result[0].Description);
        Assert.Equal("john.doe@example.com", result[0].Email);
        Assert.Equal(1990, result[0].Birthday);

        Assert.Equal("JaneDoe", result[1].Username);
        Assert.Null(result[1].Description);
        Assert.Equal("jane.doe@example.com", result[1].Email);
        Assert.Null(result[1].Birthday);
    }

    [Fact]
    public void GetTableColumnProperties_ReturnsCorrectProperties_ForUserJson()
    {
        var parser = new JsonParser();
        var json = "[{\"Username\":\"JohnDoe\",\"Description\":\"A user\",\"Email\":\"john.doe@example.com\",\"Birthday\":1990}," +
                   "{\"Username\":\"JaneDoe\",\"Description\":null,\"Email\":\"jane.doe@example.com\",\"Birthday\":null}]";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));

        var result = parser.GetFieldsProperties(stream);

        Assert.Equal(4, result.Count);

        Assert.Equal("Username", result[0].Name);
        Assert.Equal("JohnDoe", result[0].LongestValue);
        Assert.Equal(7, result[0].LongestValueLength);
        Assert.True(result[0].IsAllCellsFilled);

        Assert.Equal("Description", result[1].Name);
        Assert.Equal("A user", result[1].LongestValue);
        Assert.Equal(6, result[1].LongestValueLength);
        Assert.False(result[1].IsAllCellsFilled);

        Assert.Equal("Email", result[2].Name);
        Assert.Equal("john.doe@example.com", result[2].LongestValue);
        Assert.Equal(20, result[2].LongestValueLength);
        Assert.True(result[2].IsAllCellsFilled);

        Assert.Equal("Birthday", result[3].Name);
        Assert.Equal("1990", result[3].LongestValue);
        Assert.Equal(4, result[3].LongestValueLength);
        Assert.False(result[3].IsAllCellsFilled);
    }
}