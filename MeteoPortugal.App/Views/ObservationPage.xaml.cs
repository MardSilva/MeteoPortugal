using MeteoPortugal.App.ViewModels;

namespace MeteoPortugal.App.Views;

public partial class ObservationPage : ContentPage
{
    private readonly ObservationViewModel viewModel;

    public ObservationPage(ObservationViewModel viewModel)
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
