namespace MeteoPortugal.App.Models;

public sealed record WeatherWarning(
    string AreaId,
    string Type,
    string Level,
    string Text,
    string StartsAt,
    string EndsAt);
