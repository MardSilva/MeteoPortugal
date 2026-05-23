using MeteoPortugal.App.ViewModels;

namespace MeteoPortugal.App.Views;

public partial class HomePage : ContentPage
{
    private readonly HomeViewModel viewModel;

    public HomePage(HomeViewModel viewModel)
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
