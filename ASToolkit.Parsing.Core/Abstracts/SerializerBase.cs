using ASToolkit.Parsing.Core.Enums;
using ASToolkit.Parsing.Core.Interfaces;

namespace ASToolkit.Parsing.Core.Abstracts;

public abstract class SerializerBase : ISerializer
{
    public abstract SerializerType Type { get; }
    public Stream Serialize<T>(IEnumerable<T> data) where T : class
    {
        var dictList = data
            .Select(item => item.GetType()
                .GetProperties()
                .ToDictionary(
                    prop => prop.Name,
                    prop => prop.GetValue(item, null)
                )
            ).ToList();

        return Serialize(dictList);
    }

    public abstract Stream Serialize(IEnumerable<Dictionary<string, object?>> data);
}