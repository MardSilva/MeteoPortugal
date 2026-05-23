using MeteoPortugal.App.Infrastructure.Ipma;
using MeteoPortugal.App.Infrastructure.Ipma.Mappers;
using MeteoPortugal.App.Models;
using MeteoPortugal.App.Services.Interfaces;

namespace MeteoPortugal.App.Services.Implementations;

public sealed class ObservationService(IpmaWeatherClient ipmaWeatherClient) : IObservationService
{
    public async Task<IReadOnlyList<ObservationStation>> GetNearestStationsAsync(
        WeatherLocation location,
        int count = 8,
        CancellationToken cancellationToken = default)
    {
        var observations = await ipmaWeatherClient.GetSurfaceObservationsAsync(cancellationToken);

        return observations.Features
            .Where(feature =>
                feature.Geometry.Coordinates.Count >= 2 &&
                IpmaWeatherMapper.IsValidObservationValue(feature.Properties.Temperature))
            .Select(feature =>
            {
                var longitude = feature.Geometry.Coordinates[0];
                var latitude = feature.Geometry.Coordinates[1];
                var properties = feature.Properties;

                return new ObservationStation(
                    properties.StationName,
                    properties.StationName,
                    latitude,
                    longitude,
                    properties.Temperature,
                    NormalizeObservationValue(properties.Humidity),
                    NormalizeObservationValue(properties.WindSpeedKmh),
                    NormalizeObservationValue(properties.Pressure),
                    NormalizeObservationValue(properties.AccumulatedPrecipitation),
                    string.IsNullOrWhiteSpace(properties.WindDirection) ? "-" : properties.WindDirection,
                    FormatObservedAt(properties.Time),
                    GetDistanceKm(location.Latitude, location.Longitude, latitude, longitude));
            })
            .OrderBy(station => station.DistanceKm)
            .Take(count)
            .ToList();
    }

    private static double NormalizeObservationValue(double value)
    {
        return IpmaWeatherMapper.IsValidObservationValue(value) ? value : 0;
    }

    private static string FormatObservedAt(string value)
    {
        return DateTime.TryParse(value, out var date)
            ? date.ToLocalTime().ToString("dd/MM HH:mm")
            : value;
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
}
