using MeteoPortugal.App.Infrastructure.Ipma;
using MeteoPortugal.App.Infrastructure.Ipma.Dtos;
using MeteoPortugal.App.Infrastructure.Ipma.Mappers;
using MeteoPortugal.App.Models;
using MeteoPortugal.App.Services.Interfaces;

namespace MeteoPortugal.App.Services.Implementations;

public sealed class IpmaWeatherService(IpmaWeatherClient ipmaWeatherClient, ILocationService locationService) : IWeatherService
{
    public async Task<WeatherSummary> GetCurrentWeatherAsync(string locationId, CancellationToken cancellationToken = default)
    {
        var normalizedLocationId = NormalizeLocationId(locationId);
        var location = await GetLocationAsync(normalizedLocationId, cancellationToken);
        var forecast = await ipmaWeatherClient.GetDailyForecastAsync(normalizedLocationId, cancellationToken);
        var weatherTypes = await GetWeatherTypesAsync(cancellationToken);
        var uvIndex = await GetUvIndexAsync(normalizedLocationId, cancellationToken);
        var observation = await GetNearestObservationAsync(location, cancellationToken);
        var today = GetToday(forecast);

        return IpmaWeatherMapper.ToWeatherSummary(forecast, today, location, weatherTypes, uvIndex, observation);
    }

    public async Task<IReadOnlyList<ForecastHourlyItemModel>> GetHourlyForecastAsync(string locationId, CancellationToken cancellationToken = default)
    {
        var normalizedLocationId = NormalizeLocationId(locationId);
        var forecast = await ipmaWeatherClient.GetDailyForecastAsync(normalizedLocationId, cancellationToken);
        var weatherTypes = await GetWeatherTypesAsync(cancellationToken);

        return forecast.Data
            .Take(3)
            .Select(item => IpmaWeatherMapper.ToForecastPreviewItem(item, weatherTypes))
            .ToList();
    }

    public async Task<IReadOnlyList<ForecastDailyItemModel>> GetDailyForecastAsync(string locationId, CancellationToken cancellationToken = default)
    {
        var normalizedLocationId = NormalizeLocationId(locationId);
        var forecast = await ipmaWeatherClient.GetDailyForecastAsync(normalizedLocationId, cancellationToken);
        var weatherTypes = await GetWeatherTypesAsync(cancellationToken);

        return forecast.Data
            .Select(item => IpmaWeatherMapper.ToDailyForecastItem(item, weatherTypes))
            .ToList();
    }

    public async Task<IReadOnlyList<WeatherMetric>> GetWeatherMetricsAsync(string locationId, CancellationToken cancellationToken = default)
    {
        var normalizedLocationId = NormalizeLocationId(locationId);
        var forecast = await ipmaWeatherClient.GetDailyForecastAsync(normalizedLocationId, cancellationToken);
        var today = GetToday(forecast);
        var uvIndex = await GetUvIndexAsync(normalizedLocationId, cancellationToken);
        var location = await GetLocationAsync(normalizedLocationId, cancellationToken);
        var observation = await GetNearestObservationAsync(location, cancellationToken);
        var humidity = IpmaWeatherMapper.IsValidObservationValue(observation?.Properties.Humidity)
            ? Math.Round(observation!.Properties.Humidity).ToString()
            : "0";
        var pressure = IpmaWeatherMapper.IsValidObservationValue(observation?.Properties.Pressure)
            ? Math.Round(observation!.Properties.Pressure).ToString()
            : "0";
        var wind = IpmaWeatherMapper.IsValidObservationValue(observation?.Properties.WindSpeedKmh)
            ? Math.Round(observation!.Properties.WindSpeedKmh).ToString()
            : IpmaWeatherMapper.ToWindMetric(today).Value;

        return
        [
            new WeatherMetric("Chuva", today.PrecipitationProbability, "%"),
            new WeatherMetric("Vento", wind, "km/h"),
            new WeatherMetric("Humidade", humidity, "%"),
            new WeatherMetric("Pressão", pressure, "hPa"),
            new WeatherMetric("UV", uvIndex.ToString(), ""),
            new WeatherMetric("Max", today.MaximumTemperature, "°C"),
            new WeatherMetric("Min", today.MinimumTemperature, "°C")
        ];
    }

    private async Task<WeatherLocation> GetLocationAsync(string locationId, CancellationToken cancellationToken)
    {
        var locations = await locationService.GetSavedLocationsAsync(cancellationToken);
        return locations.FirstOrDefault(location => location.Id == locationId)
            ?? await locationService.GetDefaultLocationAsync(cancellationToken);
    }

    private async Task<IReadOnlyDictionary<int, string>> GetWeatherTypesAsync(CancellationToken cancellationToken)
    {
        IpmaWeatherTypeResponse response;

        try
        {
            response = await ipmaWeatherClient.GetWeatherTypesAsync(cancellationToken);
        }
        catch
        {
            return new Dictionary<int, string>();
        }

        return response.Data
            .GroupBy(item => item.WeatherTypeId)
            .ToDictionary(group => group.Key, group => group.First().DescriptionPt);
    }

    private async Task<int> GetUvIndexAsync(string locationId, CancellationToken cancellationToken)
    {
        IReadOnlyList<IpmaUvItemDto> uvForecast;

        try
        {
            uvForecast = await ipmaWeatherClient.GetUvForecastAsync(cancellationToken);
        }
        catch
        {
            return 0;
        }

        var uvItem = uvForecast.FirstOrDefault(item => item.GlobalIdLocal.ToString() == locationId);
        return uvItem is null
            ? 0
            : (int)Math.Round(IpmaWeatherMapper.ParseDouble(uvItem.UvIndex));
    }

    private async Task<IpmaSurfaceObservationFeatureDto?> GetNearestObservationAsync(
        WeatherLocation location,
        CancellationToken cancellationToken)
    {
        IpmaSurfaceObservationResponse observations;

        try
        {
            observations = await ipmaWeatherClient.GetSurfaceObservationsAsync(cancellationToken);
        }
        catch
        {
            return null;
        }

        return observations.Features
            .Where(feature =>
                feature.Geometry.Coordinates.Count >= 2 &&
                IpmaWeatherMapper.IsValidObservationValue(feature.Properties.Temperature))
            .OrderBy(feature => GetDistanceKm(
                location.Latitude,
                location.Longitude,
                feature.Geometry.Coordinates[1],
                feature.Geometry.Coordinates[0]))
            .FirstOrDefault();
    }

    private static double GetDistanceKm(double latitude1, double longitude1, double latitude2, double longitude2)
    {
        const double earthRadiusKm = 6371;

        var dLat = ToRadians(latitude2 - latitude1);
        var dLon = ToRadians(longitude2 - longitude1);
        var lat1 = ToRadians(latitude1);
        var lat2 = ToRadians(latitude2);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
            + Math.Cos(lat1) * Math.Cos(lat2) * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return earthRadiusKm * c;
    }

    private static double ToRadians(double degrees)
    {
        return degrees * Math.PI / 180;
    }

    private static IpmaDailyForecastItemDto GetToday(IpmaDailyForecastResponse forecast)
    {
        return forecast.Data.FirstOrDefault()
            ?? throw new InvalidOperationException("IPMA daily forecast did not include any forecast items.");
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
