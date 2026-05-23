using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MeteoPortugal.App.Models;
using MeteoPortugal.App.Services.Interfaces;

namespace MeteoPortugal.App.ViewModels;

public partial class OnboardingViewModel : ObservableObject
{
    private readonly ILocationService locationService;
    private readonly IUserPreferencesService userPreferencesService;
    private List<WeatherLocation> allLocations = [];

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ContinueCommand))]
    public partial WeatherLocation? SelectedLocation { get; set; }

    [ObservableProperty]
    public partial string SearchText { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool IsLoading { get; set; }

    [ObservableProperty]
    public partial int LocationCount { get; set; }

    public OnboardingViewModel(ILocationService locationService, IUserPreferencesService userPreferencesService)
    {
        this.locationService = locationService;
        this.userPreferencesService = userPreferencesService;
    }

    public string Title { get; } = "Meteo Portugal";

    public string Subtitle { get; } = "Escolha a localização inicial para ver a meteorologia mais relevante.";

    public ObservableCollection<LocationGroup> GroupedLocations { get; } = [];

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
            allLocations = (await locationService.GetSavedLocationsAsync()).ToList();
            ApplyFilter();
            SelectedLocation ??= allLocations.FirstOrDefault(location => location.Name == "Lisboa")
                ?? allLocations.FirstOrDefault();
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand(CanExecute = nameof(CanContinue))]
    private async Task ContinueAsync()
    {
        if (SelectedLocation is null)
        {
            return;
        }

        await userPreferencesService.SetPreferredLocationIdAsync(SelectedLocation.Id);
        await userPreferencesService.SetOnboardingCompletedAsync(true);

        if (Shell.Current.Navigation.ModalStack.Count > 0)
        {
            await Shell.Current.Navigation.PopModalAsync();
        }

        await Shell.Current.GoToAsync("//home");
    }

    partial void OnSearchTextChanged(string value)
    {
        ApplyFilter();
    }

    private void ApplyFilter()
    {
        GroupedLocations.Clear();

        var query = SearchText.Trim();
        var filteredLocations = string.IsNullOrWhiteSpace(query)
            ? allLocations
            : allLocations.Where(location =>
                location.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                location.District.Contains(query, StringComparison.OrdinalIgnoreCase));

        var groupedLocations = filteredLocations
            .Take(24)
            .GroupBy(location => location.District)
            .OrderBy(group => group.Key);

        foreach (var group in groupedLocations)
        {
            GroupedLocations.Add(new LocationGroup(group.Key, group));
        }

        LocationCount = allLocations.Count;
    }

    private bool CanContinue()
    {
        return SelectedLocation is not null;
    }
}
