using ASToolkit.Parsing.Core.Abstracts;
using ASToolkit.Parsing.Core.Enums;
using Newtonsoft.Json;

namespace ASToolkit.Parsing.Json;

public class JsonSerializer : SerializerBase
{
    public override SerializerType Type => SerializerType.Json;

    public override Stream Serialize(IEnumerable<Dictionary<string, object?>> data)
    {
        var dictionaries = data.ToList();
        if (!dictionaries.Any())
            throw new ArgumentException("Data cannot be null or empty", nameof(data));

        var json = JsonConvert.SerializeObject(dictionaries);
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(json);
        writer.Flush();
        stream.Position = 0; // Reset stream position to the beginning
        return stream;
    }
}