using MeteoPortugal.App.ViewModels;

namespace MeteoPortugal.App.Views;

public partial class OnboardingPage : ContentPage
{
    private readonly OnboardingViewModel viewModel;

    public OnboardingPage(OnboardingViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        this.viewModel = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (viewModel.GroupedLocations.Count == 0 && !viewModel.LoadCommand.IsRunning)
        {
            viewModel.LoadCommand.Execute(null);
        }
    }
}
