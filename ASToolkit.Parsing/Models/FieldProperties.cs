namespace ASToolkit.Parsing.Models;

public record FieldProperties
{
    public int? Key { get; init; }
    public string? Name { get; init; }
    public int? LongestValueLength { get; init; }
    public string? LongestValue { get; init; }
    public bool IsAllCellsFilled { get; init; }
    public bool IsAllCellsInteger { get; init; }
    public bool IsAllCellsNumber { get; init; }
    public bool IsAllCellsBool { get; init; }
}