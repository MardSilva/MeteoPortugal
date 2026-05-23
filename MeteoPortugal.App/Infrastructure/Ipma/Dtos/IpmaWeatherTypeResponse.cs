using System.Text.Json.Serialization;

namespace MeteoPortugal.App.Infrastructure.Ipma.Dtos;

public sealed class IpmaWeatherTypeResponse
{
    [JsonPropertyName("data")]
    public List<IpmaWeatherTypeDto> Data { get; set; } = [];
}
