using System.Net.Http.Json;
using MeteoPortugal.App.Infrastructure.Ipma.Dtos;

namespace MeteoPortugal.App.Infrastructure.Ipma;

public sealed class IpmaWeatherClient(HttpClient httpClient)
{
    public async Task<IpmaLocationsResponse> GetLocationsAsync(CancellationToken cancellationToken = default)
    {
        return await GetRequiredAsync<IpmaLocationsResponse>("open-data/distrits-islands.json", cancellationToken);
    }

    public async Task<IpmaDailyForecastResponse> GetDailyForecastAsync(string globalIdLocal, CancellationToken cancellationToken = default)
    {
        return await GetRequiredAsync<IpmaDailyForecastResponse>(
            $"open-data/forecast/meteorology/cities/daily/{globalIdLocal}.json",
            cancellationToken);
    }

    public async Task<IpmaWeatherTypeResponse> GetWeatherTypesAsync(CancellationToken cancellationToken = default)
    {
        return await GetRequiredAsync<IpmaWeatherTypeResponse>("open-data/weather-type-classe.json", cancellationToken);
    }

    public async Task<IReadOnlyList<IpmaUvItemDto>> GetUvForecastAsync(CancellationToken cancellationToken = default)
    {
        return await GetRequiredAsync<IReadOnlyList<IpmaUvItemDto>>(
            "open-data/forecast/meteorology/uv/uv.json",
            cancellationToken);
    }

    public async Task<IpmaSurfaceObservationResponse> GetSurfaceObservationsAsync(CancellationToken cancellationToken = default)
    {
        return await GetRequiredAsync<IpmaSurfaceObservationResponse>(
            "open-data/observation/meteorology/stations/obs-surface.geojson",
            cancellationToken);
    }

    public async Task<IReadOnlyList<IpmaWeatherWarningDto>> GetWarningsAsync(CancellationToken cancellationToken = default)
    {
        return await GetRequiredAsync<IReadOnlyList<IpmaWeatherWarningDto>>(
            "open-data/forecast/warnings/warnings_www.json",
            cancellationToken);
    }

    private async Task<T> GetRequiredAsync<T>(string requestUri, CancellationToken cancellationToken)
    {
        var value = await httpClient.GetFromJsonAsync<T>(requestUri, cancellationToken);

        if (value is null)
        {
            throw new InvalidOperationException($"IPMA returned an empty response for '{requestUri}'.");
        }

        return value;
    }
}
