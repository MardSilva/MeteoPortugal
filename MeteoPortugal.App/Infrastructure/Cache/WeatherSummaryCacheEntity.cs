using SQLite;

namespace MeteoPortugal.App.Infrastructure.Cache;

public sealed class WeatherSummaryCacheEntity
{
    [PrimaryKey]
    public string LocationId { get; set; } = string.Empty;

    public string PayloadJson { get; set; } = string.Empty;

    public DateTimeOffset SavedAt { get; set; }
}
