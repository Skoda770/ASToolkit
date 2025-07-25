using ASToolkit.Parsing.Core.Abstracts;
using ASToolkit.Parsing.Core.Enums;
using ASToolkit.Parsing.Core.Models;
using Newtonsoft.Json;

namespace ASToolkit.Parsing.Json;

public class JsonParser : ParserBase
{
    public override ParserType Type => ParserType.Json;
    public override List<T>? Parse<T>(Stream stream)
    {
        using var reader = new StreamReader(stream);
        var json = reader.ReadToEnd();
        return JsonConvert.DeserializeObject<List<T>>(json);
    }

    public override List<Dictionary<string, object?>> Parse(Stream stream)
    {
        using var reader = new StreamReader(stream);
        var json = reader.ReadToEnd();
        return JsonConvert.DeserializeObject<List<Dictionary<string,object?>>>(json)!;
    }

    public override List<FieldProperties> GetFieldsProperties(Stream stream)
    {
        using var reader = new StreamReader(stream);
        var json = reader.ReadToEnd();
        var jsonObject = JsonConvert.DeserializeObject<List<Dictionary<string, object?>>>(json);

        if (jsonObject == null || jsonObject.Count == 0)
            return [];

        var properties = new List<FieldProperties>();

        foreach (var key in jsonObject.First().Keys)
        {
            var values = jsonObject.Select(obj => obj.TryGetValue(key, out var value) ? value?.ToString() : null).ToList();
            var longestWord = values.Where(v => v != null).OrderByDescending(v => v!.Length).FirstOrDefault();

            properties.Add(new FieldProperties
            {
                Name = key,
                LongestValueLength = longestWord?.Length,
                LongestValue = longestWord,
                IsAllCellsFilled = values.Count == values.Count(v => v is not null && !string.IsNullOrEmpty(v)),
                IsAllCellsBool = values.All(v => string.IsNullOrEmpty(v) || bool.TryParse(v, out _)),
                IsAllCellsInteger = values.All(v => string.IsNullOrEmpty(v) || long.TryParse(v, out _)),
                IsAllCellsNumber = values.All(v => string.IsNullOrEmpty(v) || decimal.TryParse(v, out _))
            });
        }

        return properties;
    }
}