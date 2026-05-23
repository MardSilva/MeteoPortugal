using SQLite;

namespace MeteoPortugal.App.Infrastructure.Cache;

public sealed class JsonCacheEntity
{
    [PrimaryKey]
    public string CacheKey { get; set; } = string.Empty;

    public string PayloadJson { get; set; } = string.Empty;

    public DateTimeOffset SavedAt { get; set; }
}
