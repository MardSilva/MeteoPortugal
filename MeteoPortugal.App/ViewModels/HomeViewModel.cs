using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MeteoPortugal.App.Models;
using MeteoPortugal.App.Services.Interfaces;

namespace MeteoPortugal.App.ViewModels;

public partial class HomeViewModel : ObservableObject
{
    private readonly ILocationService locationService;
    private readonly IWeatherService weatherService;
    private readonly IWeatherCacheService weatherCacheService;
    private readonly IUserPreferencesService userPreferencesService;
    private readonly IWeatherWarningService weatherWarningService;

    [ObservableProperty]
    public partial WeatherSummary? CurrentWeather { get; set; }

    [ObservableProperty]
    public partial bool IsLoading { get; set; }

    [ObservableProperty]
    public partial bool HasWarnings { get; set; }

    [ObservableProperty]
    public partial bool HasStatusMessage { get; set; }

    [ObservableProperty]
    public partial string StatusMessage { get; set; } = string.Empty;

    public HomeViewModel(
        ILocationService locationService,
        IWeatherService weatherService,
        IWeatherCacheService weatherCacheService,
        IUserPreferencesService userPreferencesService,
        IWeatherWarningService weatherWarningService)
    {
        this.locationService = locationService;
        this.weatherService = weatherService;
        this.weatherCacheService = weatherCacheService;
        this.userPreferencesService = userPreferencesService;
        this.weatherWarningService = weatherWarningService;
    }

    public ObservableCollection<WeatherMetric> Metrics { get; } = [];

    public ObservableCollection<ForecastHourlyItemModel> HourlyForecast { get; } = [];

    public ObservableCollection<ForecastDailyItemModel> DailyForecast { get; } = [];

    public ObservableCollection<WeatherWarning> Warnings { get; } = [];

    [RelayCommand]
    private async Task LoadAsync()
    {
        if (IsLoading)
        {
            return;
        }

        IsLoading = true;

        try
        {
            var locationId = await GetSelectedLocationIdAsync();
            await LoadFromNetworkAsync(locationId);
        }
        catch
        {
            await LoadFromCacheAsync();
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task LoadFromNetworkAsync(string locationId)
    {
        HasStatusMessage = false;
        StatusMessage = string.Empty;

        CurrentWeather = await weatherService.GetCurrentWeatherAsync(locationId);
        await weatherCacheService.SaveCurrentWeatherAsync(CurrentWeather);

        var metrics = await weatherService.GetWeatherMetricsAsync(locationId);
        var hourlyForecast = await weatherService.GetHourlyForecastAsync(locationId);
        var dailyForecast = await weatherService.GetDailyForecastAsync(locationId);

        ReplaceItems(Metrics, metrics);
        ReplaceItems(HourlyForecast, hourlyForecast);
        ReplaceItems(DailyForecast, dailyForecast);

        await weatherCacheService.SaveForecastPreviewAsync(locationId, hourlyForecast);
        await weatherCacheService.SaveDailyForecastAsync(locationId, dailyForecast);

        var locations = await locationService.GetSavedLocationsAsync();
        var selectedLocation = locations.FirstOrDefault(location => location.Id == locationId)
            ?? CurrentWeather.Location;

        try
        {
            ReplaceItems(Warnings, await weatherWarningService.GetWarningsAsync(selectedLocation));
        }
        catch
        {
            Warnings.Clear();
        }

        HasWarnings = Warnings.Count > 0;
    }

    private async Task LoadFromCacheAsync()
    {
        var locationId = await GetSelectedLocationIdAsync();
        var cachedWeather = await weatherCacheService.GetCurrentWeatherAsync(locationId);

        if (cachedWeather is null)
        {
            HasStatusMessage = true;
            StatusMessage = "Não foi possível contactar o IPMA e ainda não existem dados guardados para esta localidade.";
            return;
        }

        CurrentWeather = cachedWeather with
        {
            UpdatedAt = $"{cachedWeather.UpdatedAt} · Cache local"
        };

        ReplaceItems(Metrics, CreateMetricsFromSummary(cachedWeather));
        ReplaceItems(HourlyForecast, await weatherCacheService.GetForecastPreviewAsync(locationId) ?? []);
        ReplaceItems(DailyForecast, await weatherCacheService.GetDailyForecastAsync(locationId) ?? []);

        Warnings.Clear();
        HasWarnings = false;
        HasStatusMessage = true;
        StatusMessage = "Sem ligação ao IPMA. A mostrar os últimos dados guardados no dispositivo.";
    }

    private async Task<string> GetSelectedLocationIdAsync()
    {
        var preferredLocationId = await userPreferencesService.GetPreferredLocationIdAsync();
        var location = await locationService.GetDefaultLocationAsync();
        return preferredLocationId ?? location.Id;
    }

    private static IReadOnlyList<WeatherMetric> CreateMetricsFromSummary(WeatherSummary summary)
    {
        return
        [
            new WeatherMetric("Chuva", summary.PrecipitationProbabilityPercent.ToString(), "%"),
            new WeatherMetric("Vento", Math.Round(summary.WindSpeedKmh).ToString(), "km/h"),
            new WeatherMetric("Humidade", summary.HumidityPercent.ToString(), "%"),
            new WeatherMetric("Pressão", Math.Round(summary.PressureHpa).ToString(), "hPa"),
            new WeatherMetric("UV", summary.UvIndex.ToString(), ""),
            new WeatherMetric("Máx", Math.Round(summary.MaximumTemperatureCelsius).ToString(), "°C"),
            new WeatherMetric("Mín", Math.Round(summary.MinimumTemperatureCelsius).ToString(), "°C")
        ];
    }

    private static void ReplaceItems<T>(ObservableCollection<T> target, IEnumerable<T> items)
    {
        target.Clear();

        foreach (var item in items)
        {
            target.Add(item);
        }
    }
}
