using ASToolkit.Parsing.Core.Enums;
using ASToolkit.Parsing.Core.Interfaces;

namespace ASToolkit.Parsing.Core.Infrastructure;

public class SerializerFactory(IEnumerable<ISerializer> serializers)
{
    public ISerializer GetSerializer(SerializerType type)
        => serializers.FirstOrDefault(serializer => serializer.Type == type)
           ?? throw new ArgumentException($"Invalid serializer type: {type}", nameof(type));
    public ISerializer GetSerializer(string extension)
        => extension switch
        {
            ".xlsx" => GetSerializer(SerializerType.Excel),
            ".xls" => GetSerializer(SerializerType.Excel),
            ".csv" => GetSerializer(SerializerType.Csv),
            ".json" => GetSerializer(SerializerType.Json),
            _ => throw new ArgumentException($"No serializer found for extension: {extension}", nameof(extension))
        };
}