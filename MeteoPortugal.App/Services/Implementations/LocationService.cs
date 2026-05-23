using MeteoPortugal.App.Models;
using MeteoPortugal.App.Infrastructure.Ipma;
using MeteoPortugal.App.Infrastructure.Ipma.Mappers;
using MeteoPortugal.App.Services.Interfaces;

namespace MeteoPortugal.App.Services.Implementations;

public sealed class LocationService(IpmaWeatherClient ipmaWeatherClient, IWeatherCacheService weatherCacheService) : ILocationService
{
    private static readonly IReadOnlyList<WeatherLocation> Locations =
    [
        new WeatherLocation("1110600", "Lisboa", "Continente", 38.7660, -9.1286, "LSB"),
        new WeatherLocation("1131200", "Porto", "Continente", 41.1580, -8.6294, "PTO")
    ];

    public async Task<IReadOnlyList<WeatherLocation>> GetSavedLocationsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await ipmaWeatherClient.GetLocationsAsync(cancellationToken);
            var locations = response.Data
                .Select(IpmaWeatherMapper.ToWeatherLocation)
                .OrderBy(location => location.Name)
                .ToList();

            await weatherCacheService.SaveLocationsAsync(locations, cancellationToken);
            return locations;
        }
        catch
        {
            return await weatherCacheService.GetLocationsAsync(cancellationToken) ?? Locations;
        }
    }

    public Task<WeatherLocation> GetDefaultLocationAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Locations[0]);
    }
}
