using MeteoPortugal.App.Models;

namespace MeteoPortugal.App.Services.Interfaces;

public interface ILocationService
{
    Task<IReadOnlyList<WeatherLocation>> GetSavedLocationsAsync(CancellationToken cancellationToken = default);

    Task<WeatherLocation> GetDefaultLocationAsync(CancellationToken cancellationToken = default);
}
