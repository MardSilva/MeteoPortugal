using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MeteoPortugal.App.Models;
using MeteoPortugal.App.Services.Interfaces;

namespace MeteoPortugal.App.ViewModels;

public partial class ObservationViewModel : ObservableObject
{
    private const string RadarUrl = "https://www.ipma.pt/pt/otempo/obs.radar/";
    private const string SatelliteUrl = "https://www.ipma.pt/pt/otempo/obs.satelite/";

    private readonly ILocationService locationService;
    private readonly IObservationService observationService;
    private readonly IUserPreferencesService userPreferencesService;
    private readonly IWeatherCacheService weatherCacheService;

    [ObservableProperty]
    public partial string SelectedMode { get; set; } = "Mapa";

    [ObservableProperty]
    public partial bool IsLoading { get; set; }

    [ObservableProperty]
    public partial bool IsMapVisible { get; set; } = true;

    [ObservableProperty]
    public partial bool IsRemoteVisible { get; set; }

    [ObservableProperty]
    public partial string RemoteSource { get; set; } = RadarUrl;

    [ObservableProperty]
    public partial string RemoteTitle { get; set; } = "Radar IPMA";

    [ObservableProperty]
    public partial string LocationName { get; set; } = "Portugal";

    [ObservableProperty]
    public partial string LastUpdated { get; set; } = "Dados: IPMA";

    [ObservableProperty]
    public partial bool HasStatusMessage { get; set; }

    [ObservableProperty]
    public partial string StatusMessage { get; set; } = string.Empty;

    public ObservationViewModel(
        ILocationService locationService,
        IObservationService observationService,
        IUserPreferencesService userPreferencesService,
        IWeatherCacheService weatherCacheService)
    {
        this.locationService = locationService;
        this.observationService = observationService;
        this.userPreferencesService = userPreferencesService;
        this.weatherCacheService = weatherCacheService;
    }

    public string Title { get; } = "Observação";

    public ObservableCollection<string> Modes { get; } =
    [
        "Mapa",
        "Radar",
        "Satélite"
    ];

    public ObservableCollection<ObservationStation> Stations { get; } = [];

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
            var location = await GetSelectedLocationAsync();
            LocationName = location.Name;
            await LoadStationsFromNetworkAsync(location);
        }
        catch
        {
            await LoadStationsFromCacheAsync();
        }
        finally
        {
            IsLoading = false;
        }
    }

    partial void OnSelectedModeChanged(string value)
    {
        IsMapVisible = value == "Mapa";
        IsRemoteVisible = !IsMapVisible;

        if (value == "Radar")
        {
            RemoteTitle = "Radar IPMA";
            RemoteSource = RadarUrl;
        }
        else if (value == "Satélite")
        {
            RemoteTitle = "Satélite IPMA";
            RemoteSource = SatelliteUrl;
        }
    }

    private async Task LoadStationsFromNetworkAsync(WeatherLocation location)
    {
        HasStatusMessage = false;
        StatusMessage = string.Empty;

        var stations = await observationService.GetNearestStationsAsync(location);
        ReplaceStations(stations);
        await weatherCacheService.SaveObservationsAsync(location.Id, stations);
        UpdateTimestamp();
    }

    private async Task LoadStationsFromCacheAsync()
    {
        var location = await GetSelectedLocationAsync();
        LocationName = location.Name;
        var cachedStations = await weatherCacheService.GetObservationsAsync(location.Id);

        if (cachedStations is null || cachedStations.Count == 0)
        {
            HasStatusMessage = true;
            StatusMessage = "Não foi possível carregar observações IPMA e ainda não há cache local.";
            return;
        }

        ReplaceStations(cachedStations);
        UpdateTimestamp();
        HasStatusMessage = true;
        StatusMessage = "Sem ligação ao IPMA. A mostrar observações guardadas no dispositivo.";
    }

    private async Task<WeatherLocation> GetSelectedLocationAsync()
    {
        var preferredLocationId = await userPreferencesService.GetPreferredLocationIdAsync();
        var locations = await locationService.GetSavedLocationsAsync();

        return locations.FirstOrDefault(location => location.Id == preferredLocationId)
            ?? await locationService.GetDefaultLocationAsync();
    }

    private void ReplaceStations(IEnumerable<ObservationStation> stations)
    {
        Stations.Clear();

        foreach (var station in stations)
        {
            Stations.Add(station);
        }
    }

    private void UpdateTimestamp()
    {
        LastUpdated = Stations.FirstOrDefault() is { } nearestStation
            ? $"Dados: IPMA · {nearestStation.ObservedAt}"
            : "Dados: IPMA";
    }
}
