using ASToolkit.Parsing.Core.Enums;

namespace ASToolkit.Parsing.Core.Interfaces;

public interface ISerializer
{
    SerializerType Type { get; }
    Stream Serialize<T>(IEnumerable<T> data) where T : class;
    Stream Serialize(IEnumerable<Dictionary<string, object?>> data);
}
public interface ISerializer<in TConfig> : ISerializer
    where TConfig : class
{
    void SetConfig(TConfig config);
}