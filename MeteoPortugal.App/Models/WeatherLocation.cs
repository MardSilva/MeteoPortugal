namespace MeteoPortugal.App.Models;

public sealed record WeatherLocation(
    string Id,
    string Name,
    string District,
    double Latitude,
    double Longitude,
    string WarningAreaId = "");
