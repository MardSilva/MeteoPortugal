using System.Text.Json.Serialization;

namespace MeteoPortugal.App.Infrastructure.Ipma.Dtos;

public sealed class IpmaSurfaceObservationFeatureDto
{
    [JsonPropertyName("geometry")]
    public IpmaSurfaceObservationGeometryDto Geometry { get; set; } = new();

    [JsonPropertyName("properties")]
    public IpmaSurfaceObservationPropertiesDto Properties { get; set; } = new();
}
