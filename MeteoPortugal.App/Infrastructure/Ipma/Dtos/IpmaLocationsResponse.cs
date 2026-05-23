using System.Text.Json.Serialization;

namespace MeteoPortugal.App.Infrastructure.Ipma.Dtos;

public sealed class IpmaLocationsResponse
{
    [JsonPropertyName("data")]
    public List<IpmaLocationDto> Data { get; set; } = [];
}
