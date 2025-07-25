using ASToolkit.Parsing.Core.Enums;
using ASToolkit.Parsing.Core.Interfaces;
using ASToolkit.Parsing.Core.Models;

namespace ASToolkit.Parsing.Core.Abstracts;

public abstract class ParserBase : IParser
{
    public abstract ParserType Type { get; }

    public List<T>? Parse<T>(byte[] file) where T : class
        => Parse<T>(new MemoryStream(file));

    public abstract List<T>? Parse<T>(Stream stream) where T : class;

    public List<Dictionary<string, object?>> Parse(byte[] file)
        => Parse(new MemoryStream(file));

    public abstract List<Dictionary<string, object?>> Parse(Stream stream);

    public List<FieldProperties> GetFieldsProperties(byte[] file)
        => GetFieldsProperties(new MemoryStream(file));

    public abstract List<FieldProperties> GetFieldsProperties(Stream stream);
}