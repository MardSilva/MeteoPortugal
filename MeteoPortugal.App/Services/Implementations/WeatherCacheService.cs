using System.Text.Json;
using MeteoPortugal.App.Infrastructure.Cache;
using MeteoPortugal.App.Models;
using MeteoPortugal.App.Services.Interfaces;
using SQLite;

namespace MeteoPortugal.App.Services.Implementations;

public sealed class WeatherCacheService : IWeatherCacheService
{
    private readonly JsonSerializerOptions serializerOptions = new(JsonSerializerDefaults.Web);
    private readonly Lazy<SQLiteAsyncConnection> database;
    private bool isInitialized;

    public WeatherCacheService()
    {
        database = new Lazy<SQLiteAsyncConnection>(() =>
        {
            var databasePath = Path.Combine(FileSystem.AppDataDirectory, "meteoportugal.db3");

            return new SQLiteAsyncConnection(
                databasePath,
                SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);
        });
    }

    public async Task<WeatherSummary?> GetCurrentWeatherAsync(string locationId, CancellationToken cancellationToken = default)
    {
        await InitializeAsync();

        var entity = await database.Value
            .Table<WeatherSummaryCacheEntity>()
            .Where(item => item.LocationId == NormalizeLocationId(locationId))
            .FirstOrDefaultAsync();

        if (entity is null || string.IsNullOrWhiteSpace(entity.PayloadJson))
        {
            return null;
        }

        return JsonSerializer.Deserialize<WeatherSummary>(entity.PayloadJson, serializerOptions);
    }

    public async Task SaveCurrentWeatherAsync(WeatherSummary summary, CancellationToken cancellationToken = default)
    {
        await InitializeAsync();

        var entity = new WeatherSummaryCacheEntity
        {
            LocationId = NormalizeLocationId(summary.Location.Id),
            PayloadJson = JsonSerializer.Serialize(summary, serializerOptions),
            SavedAt = DateTimeOffset.UtcNow
        };

        await database.Value.InsertOrReplaceAsync(entity);
    }

    public Task<IReadOnlyList<WeatherLocation>?> GetLocationsAsync(CancellationToken cancellationToken = default)
    {
        return GetJsonAsync<IReadOnlyList<WeatherLocation>, List<WeatherLocation>>("locations", cancellationToken);
    }

    public Task SaveLocationsAsync(IReadOnlyList<WeatherLocation> locations, CancellationToken cancellationToken = default)
    {
        return SaveJsonAsync("locations", locations, cancellationToken);
    }

    public Task<IReadOnlyList<ForecastDailyItemModel>?> GetDailyForecastAsync(string locationId, CancellationToken cancellationToken = default)
    {
        return GetJsonAsync<IReadOnlyList<ForecastDailyItemModel>, List<ForecastDailyItemModel>>($"daily:{NormalizeLocationId(locationId)}", cancellationToken);
    }

    public Task SaveDailyForecastAsync(string locationId, IReadOnlyList<ForecastDailyItemModel> forecast, CancellationToken cancellationToken = default)
    {
        return SaveJsonAsync($"daily:{NormalizeLocationId(locationId)}", forecast, cancellationToken);
    }

    public Task<IReadOnlyList<ForecastHourlyItemModel>?> GetForecastPreviewAsync(string locationId, CancellationToken cancellationToken = default)
    {
        return GetJsonAsync<IReadOnlyList<ForecastHourlyItemModel>, List<ForecastHourlyItemModel>>($"preview:{NormalizeLocationId(locationId)}", cancellationToken);
    }

    public Task SaveForecastPreviewAsync(string locationId, IReadOnlyList<ForecastHourlyItemModel> forecast, CancellationToken cancellationToken = default)
    {
        return SaveJsonAsync($"preview:{NormalizeLocationId(locationId)}", forecast, cancellationToken);
    }

    public Task<IReadOnlyList<ObservationStation>?> GetObservationsAsync(string locationId, CancellationToken cancellationToken = default)
    {
        return GetJsonAsync<IReadOnlyList<ObservationStation>, List<ObservationStation>>($"observations:{NormalizeLocationId(locationId)}", cancellationToken);
    }

    public Task SaveObservationsAsync(string locationId, IReadOnlyList<ObservationStation> observations, CancellationToken cancellationToken = default)
    {
        return SaveJsonAsync($"observations:{NormalizeLocationId(locationId)}", observations, cancellationToken);
    }

    private async Task<TReturn?> GetJsonAsync<TReturn, TStored>(string cacheKey, CancellationToken cancellationToken)
        where TStored : TReturn
    {
        await InitializeAsync();

        var entity = await database.Value
            .Table<JsonCacheEntity>()
            .Where(item => item.CacheKey == cacheKey)
            .FirstOrDefaultAsync();

        var stored = entity is null || string.IsNullOrWhiteSpace(entity.PayloadJson)
            ? default
            : JsonSerializer.Deserialize<TStored>(entity.PayloadJson, serializerOptions);

        return stored is null ? default : stored;
    }

    private async Task SaveJsonAsync<T>(string cacheKey, T value, CancellationToken cancellationToken)
    {
        await InitializeAsync();

        var entity = new JsonCacheEntity
        {
            CacheKey = cacheKey,
            PayloadJson = JsonSerializer.Serialize(value, serializerOptions),
            SavedAt = DateTimeOffset.UtcNow
        };

        await database.Value.InsertOrReplaceAsync(entity);
    }

    private async Task InitializeAsync()
    {
        if (isInitialized)
        {
            return;
        }

        SQLitePCL.Batteries_V2.Init();
        await database.Value.CreateTableAsync<WeatherSummaryCacheEntity>();
        await database.Value.CreateTableAsync<JsonCacheEntity>();
        isInitialized = true;
    }

    private static string NormalizeLocationId(string locationId)
    {
        return locationId.Trim().ToLowerInvariant() switch
        {
            "lisbon" => "1110600",
            "porto" => "1131200",
            var value => value
        };
    }
}
