using MeteoPortugal.App.Models;

namespace MeteoPortugal.App.Services.Interfaces;

public interface IObservationService
{
    Task<IReadOnlyList<ObservationStation>> GetNearestStationsAsync(
        WeatherLocation location,
        int count = 8,
        CancellationToken cancellationToken = default);
}
