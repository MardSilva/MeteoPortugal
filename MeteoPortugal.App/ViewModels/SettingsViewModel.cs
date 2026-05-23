using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MeteoPortugal.App.Models;
using MeteoPortugal.App.Services.Implementations;
using MeteoPortugal.App.Services.Interfaces;

namespace MeteoPortugal.App.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly ILocationService locationService;
    private readonly IUserPreferencesService userPreferencesService;
    private bool isLoadingPreferences;
    private List<WeatherLocation> allLocations = [];

    [ObservableProperty]
    public partial WeatherLocation? SelectedLocation { get; set; }

    [ObservableProperty]
    public partial string SelectedTheme { get; set; } = "Sistema";

    [ObservableProperty]
    public partial string LocationSearchText { get; set; } = string.Empty;

    [ObservableProperty]
    public partial int LocationCount { get; set; }

    public SettingsViewModel(ILocationService locationService, IUserPreferencesService userPreferencesService)
    {
        this.locationService = locationService;
        this.userPreferencesService = userPreferencesService;
    }

    public string Title { get; } = "Definicoes";

    public ObservableCollection<WeatherLocation> FilteredLocations { get; } = [];

    public ObservableCollection<string> Themes { get; } =
    [
        "Sistema",
        "Claro",
        "Escuro"
    ];

    [RelayCommand]
    private async Task LoadAsync()
    {
        isLoadingPreferences = true;

        try
        {
            allLocations = (await locationService.GetSavedLocationsAsync()).ToList();
            ApplyLocationFilter();

            var preferredLocationId = await userPreferencesService.GetPreferredLocationIdAsync();
            SelectedLocation = allLocations.FirstOrDefault(location => location.Id == preferredLocationId)
                ?? allLocations.FirstOrDefault(location => location.Name == "Lisboa")
                ?? allLocations.FirstOrDefault();

            SelectedTheme = ToDisplayTheme(await userPreferencesService.GetThemeAsync());
        }
        finally
        {
            isLoadingPreferences = false;
        }
    }

    [RelayCommand]
    private async Task SelectLocationAsync(WeatherLocation? location)
    {
        if (location is null)
        {
            return;
        }

        SelectedLocation = location;
        await userPreferencesService.SetPreferredLocationIdAsync(location.Id);
    }

    partial void OnSelectedLocationChanged(WeatherLocation? value)
    {
        if (isLoadingPreferences || value is null)
        {
            return;
        }

        _ = userPreferencesService.SetPreferredLocationIdAsync(value.Id);
    }

    partial void OnLocationSearchTextChanged(string value)
    {
        ApplyLocationFilter();
    }

    partial void OnSelectedThemeChanged(string value)
    {
        if (isLoadingPreferences)
        {
            return;
        }

        _ = userPreferencesService.SetThemeAsync(ToStoredTheme(value));
    }

    private void ApplyLocationFilter()
    {
        var query = LocationSearchText.Trim();
        var filteredLocations = (string.IsNullOrWhiteSpace(query)
            ? allLocations
            : allLocations.Where(location =>
                location.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                location.District.Contains(query, StringComparison.OrdinalIgnoreCase)))
            .ToList();

        var visibleLocations = filteredLocations
            .Take(24)
            .ToList();

        if (SelectedLocation is not null &&
            visibleLocations.All(location => location.Id != SelectedLocation.Id))
        {
            visibleLocations = filteredLocations
                .Prepend(SelectedLocation)
                .DistinctBy(location => location.Id)
                .Take(24)
                .ToList();
        }

        FilteredLocations.Clear();

        foreach (var location in visibleLocations)
        {
            FilteredLocations.Add(location);
        }

        LocationCount = allLocations.Count;
    }

    private static string ToDisplayTheme(string theme)
    {
        return theme switch
        {
            UserPreferencesService.LightTheme => "Claro",
            UserPreferencesService.DarkTheme => "Escuro",
            _ => "Sistema"
        };
    }

    private static string ToStoredTheme(string theme)
    {
        return theme switch
        {
            "Claro" => UserPreferencesService.LightTheme,
            "Escuro" => UserPreferencesService.DarkTheme,
            _ => UserPreferencesService.SystemTheme
        };
    }
}

