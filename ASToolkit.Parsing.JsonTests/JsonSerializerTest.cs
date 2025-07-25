using System;
using System.Collections.Generic;
using ASToolkit.Parsing.Json;
using JetBrains.Annotations;
using Xunit;

namespace ASToolkit.Parsing.JsonTests;

[TestSubject(typeof(JsonSerializer))]
public class JsonSerializerTest
{

    private readonly JsonParser _parser = new JsonParser();
    private readonly JsonSerializer _serializer = new JsonSerializer();
    
    [Fact]
    public void Serialize_ValidData_ShouldReturnStream()
    {
        var data = new List<User>
        {
            new() { Username = "John.Doe", Description = "New York" },
            new() { Username = "Jane Smith", Description = "Los Angeles" }
        };
        
        using var stream = _serializer.Serialize(data);
        Assert.NotNull(stream);
        Assert.True(stream.Length > 0);
        Assert.NotEmpty(_parser.Parse(stream));
    }
    [Fact]
    public void Serialize_EmptyGenericCollection_ShouldThrowsException()
    {
        Assert.Throws<ArgumentException>(() => _serializer.Serialize(new List<User>()));
    }
}