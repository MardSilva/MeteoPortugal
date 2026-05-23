using MeteoPortugal.App.Models;
using MeteoPortugal.App.Services.Interfaces;

namespace MeteoPortugal.App.Services.Implementations;

public sealed class MockWeatherService : IWeatherService
{
    private static readonly WeatherLocation Lisbon = new("lisbon", "Lisboa", "Lisboa", 38.7223, -9.1393);
    private static readonly WeatherLocation Porto = new("porto", "Porto", "Porto", 41.1579, -8.6291);

    public Task<WeatherSummary> GetCurrentWeatherAsync(string locationId, CancellationToken cancellationToken = default)
    {
        var summary = NormalizeLocationId(locationId) == "porto"
            ? new WeatherSummary(Porto, "Ceu pouco nublado", "w_ic_d_02.svg", 18, 14, 21, 72, 18, 1015, 35, 4, "Porto", "Atualizado as 15:00")
            : new WeatherSummary(Lisbon, "Ceu limpo", "w_ic_d_01.svg", 22, 16, 26, 58, 12, 1018, 5, 7, "Lisboa", "Atualizado as 15:00");

        return Task.FromResult(summary);
    }

    public Task<IReadOnlyList<ForecastHourlyItemModel>> GetHourlyForecastAsync(string locationId, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<ForecastHourlyItemModel> forecast = NormalizeLocationId(locationId) == "porto"
            ? new[]
            {
                new ForecastHourlyItemModel("16:00", "Nuvens", "w_ic_d_04.svg", 18),
                new ForecastHourlyItemModel("17:00", "Nuvens", "w_ic_d_04.svg", 17),
                new ForecastHourlyItemModel("18:00", "Aguaceiros", "w_ic_d_06.svg", 16)
            }
            : new[]
            {
                new ForecastHourlyItemModel("16:00", "Sol", "w_ic_d_01.svg", 22),
                new ForecastHourlyItemModel("17:00", "Sol", "w_ic_d_01.svg", 23),
                new ForecastHourlyItemModel("18:00", "Pouco nublado", "w_ic_d_02.svg", 21)
            };

        return Task.FromResult(forecast);
    }

    public Task<IReadOnlyList<ForecastDailyItemModel>> GetDailyForecastAsync(string locationId, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<ForecastDailyItemModel> forecast = NormalizeLocationId(locationId) == "porto"
            ? new[]
            {
                new ForecastDailyItemModel("Hoje", "Nublado", "w_ic_d_04.svg", 14, 21),
                new ForecastDailyItemModel("Amanha", "Chuva fraca", "w_ic_d_10.svg", 13, 19),
                new ForecastDailyItemModel("Domingo", "Abertas", "w_ic_d_03.svg", 12, 20)
            }
            : new[]
            {
                new ForecastDailyItemModel("Hoje", "Limpo", "w_ic_d_01.svg", 16, 26),
                new ForecastDailyItemModel("Amanha", "Pouco nublado", "w_ic_d_02.svg", 15, 25),
                new ForecastDailyItemModel("Domingo", "Limpo", "w_ic_d_01.svg", 17, 27)
            };

        return Task.FromResult(forecast);
    }

    public Task<IReadOnlyList<WeatherMetric>> GetWeatherMetricsAsync(string locationId, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<WeatherMetric> metrics = NormalizeLocationId(locationId) == "porto"
            ? new[]
            {
                new WeatherMetric("Humidade", "72", "%"),
                new WeatherMetric("Vento", "18", "km/h"),
                new WeatherMetric("Chuva", "35", "%"),
                new WeatherMetric("UV", "4", ""),
                new WeatherMetric("Pressão", "1015", "hPa")
            }
            : new[]
            {
                new WeatherMetric("Humidade", "58", "%"),
                new WeatherMetric("Vento", "12", "km/h"),
                new WeatherMetric("Chuva", "5", "%"),
                new WeatherMetric("UV", "7", ""),
                new WeatherMetric("Pressão", "1018", "hPa")
            };

        return Task.FromResult(metrics);
    }

    private static string NormalizeLocationId(string locationId)
    {
        return locationId.Trim().ToLowerInvariant();
    }
}
