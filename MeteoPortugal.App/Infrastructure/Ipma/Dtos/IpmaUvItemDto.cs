using System.Text.Json.Serialization;

namespace MeteoPortugal.App.Infrastructure.Ipma.Dtos;

public sealed class IpmaUvItemDto
{
    [JsonPropertyName("globalIdLocal")]
    public int GlobalIdLocal { get; set; }

    [JsonPropertyName("iUv")]
    public string UvIndex { get; set; } = string.Empty;

    [JsonPropertyName("data")]
    public string Date { get; set; } = string.Empty;
}
