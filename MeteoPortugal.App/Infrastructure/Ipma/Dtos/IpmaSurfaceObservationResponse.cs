using System.Text.Json.Serialization;

namespace MeteoPortugal.App.Infrastructure.Ipma.Dtos;

public sealed class IpmaSurfaceObservationResponse
{
    [JsonPropertyName("features")]
    public List<IpmaSurfaceObservationFeatureDto> Features { get; set; } = [];
}
