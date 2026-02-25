namespace backend.Models;
public class WebhookConfig
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Url { get; set; }
    public required string Events { get; set; }
    public required string Status { get; set; }
    public required string LastEvent { get; set; }
    public double DeliveryRate { get; set; }
}
