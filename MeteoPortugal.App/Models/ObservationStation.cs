namespace MeteoPortugal.App.Models;

public sealed record ObservationStation(
    string Id,
    string Name,
    double Latitude,
    double Longitude,
    double TemperatureCelsius,
    double HumidityPercent,
    double WindSpeedKmh,
    double PressureHpa,
    double AccumulatedPrecipitationMm,
    string WindDirection,
    string ObservedAt,
    double DistanceKm);
