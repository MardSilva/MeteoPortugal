namespace MeteoPortugal.App.Models;

public sealed record ForecastHourlyItemModel(
    string Time,
    string Condition,
    string WeatherIcon,
    double TemperatureCelsius);
