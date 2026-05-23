using CommunityToolkit.Mvvm.ComponentModel;

namespace MeteoPortugal.App.ViewModels;

public partial class DailyInfoEditViewModel : ObservableObject
{
    public string Title { get; } = "Editar informacao diaria";

    public string Message { get; } = "Area preparada para configuracoes diarias futuras.";
}
