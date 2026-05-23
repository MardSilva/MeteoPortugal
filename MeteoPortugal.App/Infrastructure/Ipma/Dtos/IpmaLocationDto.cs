using System.Text.Json.Serialization;

namespace MeteoPortugal.App.Infrastructure.Ipma.Dtos;

public sealed class IpmaLocationDto
{
    [JsonPropertyName("idRegiao")]
    public int RegionId { get; set; }

    [JsonPropertyName("idAreaAviso")]
    public string WarningAreaId { get; set; } = string.Empty;

    [JsonPropertyName("globalIdLocal")]
    public int GlobalIdLocal { get; set; }

    [JsonPropertyName("latitude")]
    public string Latitude { get; set; } = string.Empty;

    [JsonPropertyName("longitude")]
    public string Longitude { get; set; } = string.Empty;

    [JsonPropertyName("local")]
    public string Local { get; set; } = string.Empty;
}
