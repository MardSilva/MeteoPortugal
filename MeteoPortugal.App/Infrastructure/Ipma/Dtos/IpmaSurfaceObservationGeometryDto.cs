using System.Text.Json.Serialization;

namespace MeteoPortugal.App.Infrastructure.Ipma.Dtos;

public sealed class IpmaSurfaceObservationGeometryDto
{
    [JsonPropertyName("coordinates")]
    public List<double> Coordinates { get; set; } = [];
}
