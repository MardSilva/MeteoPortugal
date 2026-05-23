using System.Text.Json.Serialization;

namespace MeteoPortugal.App.Infrastructure.Ipma.Dtos;

public sealed class IpmaDailyForecastItemDto
{
    [JsonPropertyName("precipitaProb")]
    public string PrecipitationProbability { get; set; } = string.Empty;

    [JsonPropertyName("tMin")]
    public string MinimumTemperature { get; set; } = string.Empty;

    [JsonPropertyName("tMax")]
    public string MaximumTemperature { get; set; } = string.Empty;

    [JsonPropertyName("predWindDir")]
    public string WindDirection { get; set; } = string.Empty;

    [JsonPropertyName("idWeatherType")]
    public int WeatherTypeId { get; set; }

    [JsonPropertyName("classWindSpeed")]
    public int WindSpeedClass { get; set; }

    [JsonPropertyName("longitude")]
    public string Longitude { get; set; } = string.Empty;

    [JsonPropertyName("latitude")]
    public string Latitude { get; set; } = string.Empty;

    [JsonPropertyName("forecastDate")]
    public string ForecastDate { get; set; } = string.Empty;
}
