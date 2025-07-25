using System.Globalization;
using ASToolkit.Parsing.Core.Abstracts;
using ASToolkit.Parsing.Core.Enums;
using ASToolkit.Parsing.Core.Interfaces;
using CsvHelper;
using CsvHelper.Configuration;

namespace ASToolkit.Parsing.Csv;

public class CsvSerializer : SerializerBase, ISerializer<CsvConfig>
{
    private CsvConfig? _config;

    public void SetConfig(CsvConfig config)
    {
        _config = config;
    }

    public override SerializerType Type => SerializerType.Csv;

    public override Stream Serialize(IEnumerable<Dictionary<string, object?>> data)
    {
        var dictionaries = data.ToList();
        if (!dictionaries.Any())
            throw new ArgumentException("Data cannot be null or empty", nameof(data));
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = _config?.Delimiter.ToString() ?? ",",
            HasHeaderRecord = true
        });
        var headerKeys = dictionaries.First().Keys.ToList();
        foreach (var headerKey in headerKeys)
            csv.WriteField(headerKey);
        csv.NextRecord();
        foreach (var dictionary in dictionaries)
        {
            foreach (var keyValuePair in dictionary)
                csv.WriteField(keyValuePair.Value?.ToString());
            csv.NextRecord();
        }
        writer.Flush();
        stream.Position = 0; // Reset stream position to the beginning

        return stream;
    }
}