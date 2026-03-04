using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;

namespace backend;

public static class TimeseriesEndpoints
{
    private const string Org = "datacenter";
    private const string Bucket = "sensors";

    public static void MapTimeseriesEndpoints(this WebApplication app)
    {
        var influxUrl = app.Configuration["InfluxDB:Url"] ?? "http://localhost:8086";
        var influxToken = app.Configuration["InfluxDB:Token"] ?? "datacenter-influx-token-1234";

        // POST /api/timeseries/temperature
        // Body: { "sensorId": "S-01", "cabinet": "A1", "inlet": 22.5, "outlet": 34.1 }
        app.MapPost("/api/timeseries/temperature", async (TemperatureWriteRequest req) =>
        {
            using var client = new InfluxDBClient(influxUrl, influxToken);
            var writeApi = client.GetWriteApiAsync();

            var point = PointData.Measurement("cabinet_temperature")
                .Tag("sensor_id", req.SensorId)
                .Tag("cabinet", req.Cabinet)
                .Field("inlet", req.Inlet)
                .Field("outlet", req.Outlet)
                .Field("delta", req.Outlet - req.Inlet)
                .Timestamp(DateTime.UtcNow, WritePrecision.S);

            await writeApi.WritePointAsync(point, Bucket, Org);
            return Results.Created("/api/timeseries/temperature", req);
        });

        // GET /api/timeseries/temperature?range=24h
        // Returns time-bucketed averages per sensor
        app.MapGet("/api/timeseries/temperature", async (string? range) =>
        {
            var timeRange = range ?? "24h";
            using var client = new InfluxDBClient(influxUrl, influxToken);
            var queryApi = client.GetQueryApi();

            var flux = $"""
                from(bucket: "{Bucket}")
                  |> range(start: -{timeRange})
                  |> filter(fn: (r) => r._measurement == "cabinet_temperature")
                  |> filter(fn: (r) => r._field == "inlet" or r._field == "outlet" or r._field == "delta")
                  |> aggregateWindow(every: 5m, fn: mean, createEmpty: false)
                  |> pivot(rowKey: ["_time", "sensor_id", "cabinet"], columnKey: ["_field"], valueColumn: "_value")
                  |> sort(columns: ["_time"])
                """;

            var tables = await queryApi.QueryAsync(flux, Org);

            var results = tables
                .SelectMany(t => t.Records)
                .Select(r => new TemperatureReading
                {
                    Time = r.GetTime()?.ToDateTimeUtc().ToString("o") ?? "",
                    SensorId = r.GetValueByKey("sensor_id")?.ToString() ?? "",
                    Cabinet = r.GetValueByKey("cabinet")?.ToString() ?? "",
                    Inlet = r.GetValueByKey("inlet") is double i ? Math.Round(i, 1) : 0,
                    Outlet = r.GetValueByKey("outlet") is double o ? Math.Round(o, 1) : 0,
                    Delta = r.GetValueByKey("delta") is double d ? Math.Round(d, 1) : 0,
                })
                .ToList();

            return Results.Ok(results);
        });

        // POST /api/timeseries/temperature/seed
        // Seed InfluxDB with 24h of mock data for demo
        app.MapPost("/api/timeseries/temperature/seed", async () =>
        {
            using var client = new InfluxDBClient(influxUrl, influxToken);
            var writeApi = client.GetWriteApiAsync();

            var sensors = new[]
            {
                ("S-01", "Row-A / Cabinet 01"),
                ("S-02", "Row-A / Cabinet 02"),
                ("S-03", "Row-B / Cabinet 01"),
            };

            var points = new List<PointData>();
            var rng = new Random(42);
            var now = DateTime.UtcNow;

            foreach (var (sensorId, cabinet) in sensors)
            {
                for (int i = 288; i >= 0; i--) // 24h at 5min intervals
                {
                    var ts = now.AddMinutes(-5 * i);
                    double inlet = 21.0 + rng.NextDouble() * 4.0;
                    double outlet = inlet + 10.0 + rng.NextDouble() * 4.0;

                    points.Add(PointData.Measurement("cabinet_temperature")
                        .Tag("sensor_id", sensorId)
                        .Tag("cabinet", cabinet)
                        .Field("inlet", Math.Round(inlet, 1))
                        .Field("outlet", Math.Round(outlet, 1))
                        .Field("delta", Math.Round(outlet - inlet, 1))
                        .Timestamp(ts, WritePrecision.S));
                }
            }

            await writeApi.WritePointsAsync(points, Bucket, Org);
            return Results.Ok(new { written = points.Count });
        });
    }
}

public record TemperatureWriteRequest(string SensorId, string Cabinet, double Inlet, double Outlet);

public record TemperatureReading
{
    public string Time { get; init; } = "";
    public string SensorId { get; init; } = "";
    public string Cabinet { get; init; } = "";
    public double Inlet { get; init; }
    public double Outlet { get; init; }
    public double Delta { get; init; }
}
