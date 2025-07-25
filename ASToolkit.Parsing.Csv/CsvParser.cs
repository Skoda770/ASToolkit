using System.Globalization;
using ASToolkit.Parsing.Core.Abstracts;
using ASToolkit.Parsing.Core.Enums;
using ASToolkit.Parsing.Core.Interfaces;
using ASToolkit.Parsing.Core.Models;
using CsvHelper;
using CsvHelper.Configuration;

namespace ASToolkit.Parsing.Csv;

public class CsvParser : ParserBase, IParser<CsvConfig>
{
    private CsvConfig? _config;
    public void SetConfig(CsvConfig config)
    {
        _config = config;
    }

    public override ParserType Type => ParserType.Csv;
    public override List<T> Parse<T>(Stream stream)
    {
        using var reader = new StreamReader(stream);
        var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = _config?.Delimiter.ToString() ?? ",",
            HasHeaderRecord = true
        };
        using var csv = new CsvReader(reader, csvConfig);
        
        return csv.GetRecords<T>().ToList();
    }

    public override List<Dictionary<string, object?>> Parse(Stream stream)
    {
        using var reader = new StreamReader(stream);
        var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = _config?.Delimiter.ToString() ?? ",",
        };
        using var csv = new CsvReader(reader, csvConfig);
        var records = new List<Dictionary<string, object?>>();
        csv.Read();
        csv.ReadHeader();
        var headers = csv.HeaderRecord;
        if (headers is null)
            throw new ArgumentException("Headers not found");

        while (csv.Read())
        {
            var record = new Dictionary<string, object?>();
            foreach (var header in headers)
                record[header] = csv.GetField(header);

            records.Add(record);
        }

        return records;
    }

    public override List<FieldProperties> GetFieldsProperties(Stream stream)
    {
        using var reader = new StreamReader(stream);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        csv.Read();
        csv.ReadHeader();
        var headers = csv.HeaderRecord;
        if (headers is null)
            throw new ArgumentException("Headers not found");
        return headers.Select(header => new FieldProperties { Name = header }).ToList();
    }
}