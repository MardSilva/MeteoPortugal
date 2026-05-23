using System.Text.Json.Serialization;

namespace MeteoPortugal.App.Infrastructure.Ipma.Dtos;

public sealed class IpmaWeatherTypeDto
{
    [JsonPropertyName("idWeatherType")]
    public int WeatherTypeId { get; set; }

    [JsonPropertyName("descWeatherTypePT")]
    public string DescriptionPt { get; set; } = string.Empty;
}
