using MeteoPortugal.App.Models;

namespace MeteoPortugal.App.Services.Interfaces;

public interface IWeatherCacheService
{
    Task<WeatherSummary?> GetCurrentWeatherAsync(string locationId, CancellationToken cancellationToken = default);

    Task SaveCurrentWeatherAsync(WeatherSummary summary, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<WeatherLocation>?> GetLocationsAsync(CancellationToken cancellationToken = default);

    Task SaveLocationsAsync(IReadOnlyList<WeatherLocation> locations, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ForecastDailyItemModel>?> GetDailyForecastAsync(string locationId, CancellationToken cancellationToken = default);

    Task SaveDailyForecastAsync(string locationId, IReadOnlyList<ForecastDailyItemModel> forecast, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ForecastHourlyItemModel>?> GetForecastPreviewAsync(string locationId, CancellationToken cancellationToken = default);

    Task SaveForecastPreviewAsync(string locationId, IReadOnlyList<ForecastHourlyItemModel> forecast, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ObservationStation>?> GetObservationsAsync(string locationId, CancellationToken cancellationToken = default);

    Task SaveObservationsAsync(string locationId, IReadOnlyList<ObservationStation> observations, CancellationToken cancellationToken = default);
}
