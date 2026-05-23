using MeteoPortugal.App.Models;

namespace MeteoPortugal.App.Services.Interfaces;

public interface IWeatherService
{
    Task<WeatherSummary> GetCurrentWeatherAsync(string locationId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ForecastHourlyItemModel>> GetHourlyForecastAsync(string locationId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ForecastDailyItemModel>> GetDailyForecastAsync(string locationId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<WeatherMetric>> GetWeatherMetricsAsync(string locationId, CancellationToken cancellationToken = default);
}
