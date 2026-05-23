using CommunityToolkit.Mvvm.ComponentModel;

namespace MeteoPortugal.App.ViewModels;

public partial class DailyInfoEditViewModel : ObservableObject
{
    public string Title { get; } = "Editar informação diária";

    public string Message { get; } = "Área preparada para configurações diárias futuras.";
}
