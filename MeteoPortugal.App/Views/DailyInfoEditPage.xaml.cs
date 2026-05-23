using MeteoPortugal.App.ViewModels;

namespace MeteoPortugal.App.Views;

public partial class DailyInfoEditPage : ContentPage
{
    public DailyInfoEditPage(DailyInfoEditViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
