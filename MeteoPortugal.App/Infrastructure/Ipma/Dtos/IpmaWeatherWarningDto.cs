using System.Text.Json.Serialization;

namespace MeteoPortugal.App.Infrastructure.Ipma.Dtos;

public sealed class IpmaWeatherWarningDto
{
    [JsonPropertyName("idAreaAviso")]
    public string AreaId { get; set; } = string.Empty;

    [JsonPropertyName("awarenessTypeName")]
    public string TypeName { get; set; } = string.Empty;

    [JsonPropertyName("awarenessLevelID")]
    public string LevelId { get; set; } = string.Empty;

    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;

    [JsonPropertyName("startTime")]
    public string StartTime { get; set; } = string.Empty;

    [JsonPropertyName("endTime")]
    public string EndTime { get; set; } = string.Empty;
}
