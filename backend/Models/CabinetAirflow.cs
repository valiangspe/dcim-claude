namespace backend.Models;
public class CabinetAirflow
{
    public int Id { get; set; }
    public required string CabinetId { get; set; }
    public double Cfm { get; set; }
    public double Kw { get; set; }
    public double CfmPerKw { get; set; }
    public double Pressure { get; set; }
    public double Leakage { get; set; }
}
