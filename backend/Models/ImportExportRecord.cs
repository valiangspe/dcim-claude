namespace backend.Models;
public class ImportExportRecord
{
    public int Id { get; set; }
    public required string Date { get; set; }
    public required string Type { get; set; }
    public required string File { get; set; }
    public int Records { get; set; }
    public required string Status { get; set; }
}
