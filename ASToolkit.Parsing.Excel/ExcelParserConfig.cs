namespace ASToolkit.Parsing.Excel;

public class ExcelParserConfig
{
    public int StartColumn { get; set; } = 1;
    public int? EndColumn { get; set; }
    public int StartRow { get; set; } = 1;
    public int? EndRow { get; set; }

    public string? Sheet { get; init; }
    
    public ICollection<KeyValuePair<int, string>>? ExcelColumns { get; set; }
    public HashSet<int>? CustomRowsIndexes { get; set; }
    
}