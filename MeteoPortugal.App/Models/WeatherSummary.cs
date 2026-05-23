namespace MeteoPortugal.App.Models;

public sealed record WeatherSummary(
    WeatherLocation Location,
    string Condition,
    string WeatherIcon,
    double TemperatureCelsius,
    double MinimumTemperatureCelsius,
    double MaximumTemperatureCelsius,
    int HumidityPercent,
    double WindSpeedKmh,
    double PressureHpa,
    int PrecipitationProbabilityPercent,
    int UvIndex,
    string ObservationStation,
    string UpdatedAt);
