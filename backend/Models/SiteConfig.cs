namespace backend.Models;
public class SiteConfig
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Location { get; set; }
    public required string Address { get; set; }
    public required string ContactName { get; set; }
    public required string ContactPhone { get; set; }
    public double Capacity { get; set; }
    public required string Status { get; set; }
}
