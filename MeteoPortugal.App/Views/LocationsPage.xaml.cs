using MeteoPortugal.App.ViewModels;

namespace MeteoPortugal.App.Views;

public partial class LocationsPage : ContentPage
{
    private readonly LocationsViewModel viewModel;

    public LocationsPage(LocationsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        this.viewModel = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (viewModel.Locations.Count == 0 && !viewModel.LoadCommand.IsRunning)
        {
            viewModel.LoadCommand.Execute(null);
        }
    }
}
