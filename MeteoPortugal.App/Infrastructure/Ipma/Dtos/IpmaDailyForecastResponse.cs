using System.Text.Json.Serialization;

namespace MeteoPortugal.App.Infrastructure.Ipma.Dtos;

public sealed class IpmaDailyForecastResponse
{
    [JsonPropertyName("data")]
    public List<IpmaDailyForecastItemDto> Data { get; set; } = [];

    [JsonPropertyName("globalIdLocal")]
    public int GlobalIdLocal { get; set; }

    [JsonPropertyName("dataUpdate")]
    public string DataUpdate { get; set; } = string.Empty;
}
