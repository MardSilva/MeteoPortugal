namespace MeteoPortugal.App.Models;

public sealed record ForecastDailyItemModel(
    string Day,
    string Condition,
    string WeatherIcon,
    double MinimumTemperatureCelsius,
    double MaximumTemperatureCelsius);
