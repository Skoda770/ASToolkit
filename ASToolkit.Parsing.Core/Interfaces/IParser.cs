using ASToolkit.Parsing.Core.Enums;
using ASToolkit.Parsing.Core.Models;

namespace ASToolkit.Parsing.Core.Interfaces;

public interface IParser
{
    ParserType Type { get; }
    List<T>? Parse<T>(byte[] file) where T : class;

    List<T>? Parse<T>(Stream stream) where T : class;

    List<Dictionary<string, object?>> Parse(byte[] file);

    List<Dictionary<string, object?>> Parse(Stream stream);

    List<FieldProperties> GetFieldsProperties(byte[] file);

    List<FieldProperties> GetFieldsProperties(Stream stream);
    
}
public interface IParser<in TConfig> : IParser
    where TConfig : class
{
    void SetConfig(TConfig config);
}