namespace backend.Models;
public class WeatherLocation
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Region { get; set; }
    public double Temp { get; set; }
    public double FeelsLike { get; set; }
    public required string Condition { get; set; }
    public double WindSpeed { get; set; }
    public int Humidity { get; set; }
    public double Pressure { get; set; }
    public int Aqi { get; set; }
    public required string Impact { get; set; }
    public required string LastUpdated { get; set; }
}
