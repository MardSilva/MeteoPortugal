using MeteoPortugal.App.Models;

namespace MeteoPortugal.App.Services.Interfaces;

public interface IWeatherWarningService
{
    Task<IReadOnlyList<WeatherWarning>> GetWarningsAsync(
        WeatherLocation location,
        CancellationToken cancellationToken = default);
}
