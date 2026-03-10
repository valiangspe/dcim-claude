namespace backend.Models;

public class IsoClassRating
{
    public int Id { get; set; }
    public required string Site { get; set; }
    public required string System { get; set; }
    public int ClassRating { get; set; }
    public required string Justification { get; set; }
    public required string AssessedBy { get; set; }
    public required string AssessedDate { get; set; }
    public required string Status { get; set; }
}
