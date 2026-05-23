using System.Text.Json.Serialization;

namespace MeteoPortugal.App.Infrastructure.Ipma.Dtos;

public sealed class IpmaSurfaceObservationPropertiesDto
{
    [JsonPropertyName("intensidadeVentoKM")]
    public double WindSpeedKmh { get; set; }

    [JsonPropertyName("temperatura")]
    public double Temperature { get; set; }

    [JsonPropertyName("pressao")]
    public double Pressure { get; set; }

    [JsonPropertyName("humidade")]
    public double Humidity { get; set; }

    [JsonPropertyName("localEstacao")]
    public string StationName { get; set; } = string.Empty;

    [JsonPropertyName("precAcumulada")]
    public double AccumulatedPrecipitation { get; set; }

    [JsonPropertyName("time")]
    public string Time { get; set; } = string.Empty;

    [JsonPropertyName("descDirVento")]
    public string WindDirection { get; set; } = string.Empty;
}
