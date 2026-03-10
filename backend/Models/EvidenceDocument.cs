namespace backend.Models;

public class EvidenceDocument
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Type { get; set; }
    public string IsoReference { get; set; } = "";
    public string Description { get; set; } = "";
    public string FileName { get; set; } = "";
    public string FileSize { get; set; } = "";
    public string UploadedBy { get; set; } = "";
    public string UploadedDate { get; set; } = "";
    public string ExpiryDate { get; set; } = "";
    public required string Status { get; set; }
    public string Tags { get; set; } = "";
}
