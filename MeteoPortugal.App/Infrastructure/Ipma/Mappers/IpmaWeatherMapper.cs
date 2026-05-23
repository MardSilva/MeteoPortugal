using System.Globalization;
using MeteoPortugal.App.Infrastructure.Ipma.Dtos;
using MeteoPortugal.App.Models;

namespace MeteoPortugal.App.Infrastructure.Ipma.Mappers;

public static class IpmaWeatherMapper
{
    public static WeatherLocation ToWeatherLocation(IpmaLocationDto dto)
    {
        return new WeatherLocation(
            dto.GlobalIdLocal.ToString(CultureInfo.InvariantCulture),
            dto.Local,
            GetRegionName(dto.RegionId),
            ParseDouble(dto.Latitude),
            ParseDouble(dto.Longitude),
            dto.WarningAreaId);
    }

    public static WeatherSummary ToWeatherSummary(
        IpmaDailyForecastResponse forecast,
        IpmaDailyForecastItemDto today,
        WeatherLocation location,
        IReadOnlyDictionary<int, string> weatherTypes,
        int uvIndex,
        IpmaSurfaceObservationFeatureDto? observation)
    {
        var minimumTemperature = ParseDouble(today.MinimumTemperature);
        var maximumTemperature = ParseDouble(today.MaximumTemperature);
        var temperature = IsValidObservationValue(observation?.Properties.Temperature)
            ? observation!.Properties.Temperature
            : Math.Round((minimumTemperature + maximumTemperature) / 2, 1);
        var windSpeedKmh = IsValidObservationValue(observation?.Properties.WindSpeedKmh)
            ? observation!.Properties.WindSpeedKmh
            : EstimateWindSpeedKmh(today.WindSpeedClass);
        var humidity = IsValidObservationValue(observation?.Properties.Humidity)
            ? (int)Math.Round(observation!.Properties.Humidity)
            : 0;
        var pressure = IsValidObservationValue(observation?.Properties.Pressure)
            ? observation!.Properties.Pressure
            : 0;
        var precipitationProbability = (int)Math.Round(ParseDouble(today.PrecipitationProbability));

        return new WeatherSummary(
            location,
            GetWeatherDescription(today.WeatherTypeId, weatherTypes),
            GetWeatherIcon(today.WeatherTypeId),
            temperature,
            minimumTemperature,
            maximumTemperature,
            humidity,
            windSpeedKmh,
            pressure,
            precipitationProbability,
            uvIndex,
            observation?.Properties.StationName ?? "Sem estacao",
            FormatUpdatedAt(observation?.Properties.Time, forecast.DataUpdate));
    }

    public static ForecastDailyItemModel ToDailyForecastItem(
        IpmaDailyForecastItemDto dto,
        IReadOnlyDictionary<int, string> weatherTypes)
    {
        return new ForecastDailyItemModel(
            FormatForecastDay(dto.ForecastDate),
            GetWeatherDescription(dto.WeatherTypeId, weatherTypes),
            GetWeatherIcon(dto.WeatherTypeId),
            ParseDouble(dto.MinimumTemperature),
            ParseDouble(dto.MaximumTemperature));
    }

    public static ForecastHourlyItemModel ToForecastPreviewItem(
        IpmaDailyForecastItemDto dto,
        IReadOnlyDictionary<int, string> weatherTypes)
    {
        return new ForecastHourlyItemModel(
            FormatForecastDay(dto.ForecastDate),
            GetWeatherDescription(dto.WeatherTypeId, weatherTypes),
            GetWeatherIcon(dto.WeatherTypeId),
            ParseDouble(dto.MaximumTemperature));
    }

    public static WeatherMetric ToWindMetric(IpmaDailyForecastItemDto today)
    {
        return new WeatherMetric("Vento", GetWindClassDescription(today.WindSpeedClass), today.WindDirection);
    }

    public static double ParseDouble(string value)
    {
        return double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var result)
            ? result
            : 0;
    }

    public static bool IsValidObservationValue(double? value)
    {
        return value.HasValue && value.Value > -90;
    }

    private static string GetRegionName(int regionId)
    {
        return regionId switch
        {
            1 => "Continente",
            2 => "Madeira",
            3 => "Açores",
            _ => "Portugal"
        };
    }

    private static string GetWeatherDescription(int weatherTypeId, IReadOnlyDictionary<int, string> weatherTypes)
    {
        return weatherTypes.TryGetValue(weatherTypeId, out var description)
            ? description
            : "Sem informacao";
    }

    private static string GetWeatherIcon(int weatherTypeId)
    {
        if (weatherTypeId is < 1 or > 30)
        {
            return "w_ic_d_01.svg";
        }

        return $"w_ic_d_{weatherTypeId:00}.svg";
    }

    private static string FormatUpdatedAt(string? observationTime, string forecastUpdate)
    {
        var value = !string.IsNullOrWhiteSpace(observationTime)
            ? observationTime
            : forecastUpdate;

        return DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var date)
            ? $"Dados: IPMA · Atualizado {date.ToLocalTime():dd/MM HH:mm}"
            : "Dados: IPMA";
    }

    private static string FormatForecastDay(string value)
    {
        if (!DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
        {
            return value;
        }

        var today = DateTime.Today;

        if (date.Date == today)
        {
            return "Hoje";
        }

        if (date.Date == today.AddDays(1))
        {
            return "Amanha";
        }

        return date.ToString("ddd dd/MM", CultureInfo.GetCultureInfo("pt-PT"));
    }

    private static double EstimateWindSpeedKmh(int windSpeedClass)
    {
        return windSpeedClass switch
        {
            1 => 8,
            2 => 18,
            3 => 32,
            4 => 50,
            _ => 0
        };
    }

    private static string GetWindClassDescription(int windSpeedClass)
    {
        return windSpeedClass switch
        {
            1 => "Fraco",
            2 => "Moderado",
            3 => "Forte",
            4 => "Muito forte",
            _ => "Sem dados"
        };
    }
}
