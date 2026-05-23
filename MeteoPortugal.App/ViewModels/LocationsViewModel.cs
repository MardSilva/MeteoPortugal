using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MeteoPortugal.App.Models;
using MeteoPortugal.App.Services.Interfaces;

namespace MeteoPortugal.App.ViewModels;

public partial class LocationsViewModel : ObservableObject
{
    private readonly ILocationService locationService;
    private readonly IWeatherService weatherService;

    [ObservableProperty]
    public partial bool IsLoading { get; set; }

    [ObservableProperty]
    public partial string SearchText { get; set; } = string.Empty;

    [ObservableProperty]
    public partial int LocationCount { get; set; }

    public LocationsViewModel(ILocationService locationService, IWeatherService weatherService)
    {
        this.locationService = locationService;
        this.weatherService = weatherService;
    }

    private List<WeatherSummary> allLocations = [];

    public ObservableCollection<WeatherSummary> Locations { get; } = [];

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
            allLocations.Clear();
            var savedLocations = await locationService.GetSavedLocationsAsync();

            foreach (var location in savedLocations)
            {
                allLocations.Add(await weatherService.GetCurrentWeatherAsync(location.Id));
            }

            ApplyFilter();
        }
        finally
        {
            IsLoading = false;
        }
    }

    partial void OnSearchTextChanged(string value)
    {
        ApplyFilter();
    }

    private void ApplyFilter()
    {
        Locations.Clear();

        var query = SearchText.Trim();
        var filteredLocations = string.IsNullOrWhiteSpace(query)
            ? allLocations
            : allLocations.Where(summary =>
                summary.Location.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                summary.Location.District.Contains(query, StringComparison.OrdinalIgnoreCase));

        foreach (var location in filteredLocations)
        {
            Locations.Add(location);
        }

        LocationCount = Locations.Count;
    }
}
