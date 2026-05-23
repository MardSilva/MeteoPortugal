using MeteoPortugal.App.ViewModels;

namespace MeteoPortugal.App.Views;

public partial class SettingsPage : ContentPage
{
    private readonly SettingsViewModel viewModel;

    public SettingsPage(SettingsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        this.viewModel = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (!viewModel.LoadCommand.IsRunning)
        {
            viewModel.LoadCommand.Execute(null);
        }
    }
}
