using MeteoPortugal.App.Infrastructure.Ipma;
using MeteoPortugal.App.Models;
using MeteoPortugal.App.Services.Interfaces;

namespace MeteoPortugal.App.Services.Implementations;

public sealed class WeatherWarningService(IpmaWeatherClient ipmaWeatherClient) : IWeatherWarningService
{
    public async Task<IReadOnlyList<WeatherWarning>> GetWarningsAsync(
        WeatherLocation location,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(location.WarningAreaId))
        {
            return [];
        }

        var warnings = await ipmaWeatherClient.GetWarningsAsync(cancellationToken);

        return warnings
            .Where(warning =>
                warning.AreaId.Equals(location.WarningAreaId, StringComparison.OrdinalIgnoreCase) &&
                !warning.LevelId.Equals("green", StringComparison.OrdinalIgnoreCase))
            .OrderBy(warning => GetLevelWeight(warning.LevelId))
            .ThenBy(warning => warning.StartTime)
            .Select(warning => new WeatherWarning(
                warning.AreaId,
                warning.TypeName,
                warning.LevelId,
                string.IsNullOrWhiteSpace(warning.Text) ? "Aviso meteorologico ativo." : warning.Text,
                FormatDate(warning.StartTime),
                FormatDate(warning.EndTime)))
            .ToList();
    }

    private static int GetLevelWeight(string level)
    {
        return level.ToLowerInvariant() switch
        {
            "red" => 0,
            "orange" => 1,
            "yellow" => 2,
            _ => 3
        };
    }

    private static string FormatDate(string value)
    {
        return DateTime.TryParse(value, out var date)
            ? date.ToLocalTime().ToString("dd/MM HH:mm")
            : value;
    }
}
