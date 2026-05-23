using Microsoft.Extensions.DependencyInjection;
using MeteoPortugal.App.Services.Interfaces;

#pragma warning disable CA1416

namespace MeteoPortugal.App
{
    public partial class AppShell : Shell
    {
        private readonly IServiceProvider services;
        private readonly IUserPreferencesService userPreferencesService;
        private bool checkedInitialRoute;

        public AppShell(IServiceProvider services, IUserPreferencesService userPreferencesService)
        {
            this.services = services;
            this.userPreferencesService = userPreferencesService;
            Title = "Meteo Portugal";
            FlyoutBehavior = FlyoutBehavior.Disabled;

            Items.Add(new TabBar
            {
                Items =
                {
                    CreateShellContent<Views.HomePage>(services, "Hoje", "home"),
                    CreateShellContent<Views.LocationsPage>(services, "Locais", "locations"),
                    CreateShellContent<Views.ObservationPage>(services, "Observação", "observation"),
                    CreateShellContent<Views.SettingsPage>(services, "Definições", "settings")
                }
            });

            Routing.RegisterRoute(nameof(Views.DailyInfoEditPage), typeof(Views.DailyInfoEditPage));
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (checkedInitialRoute)
            {
                return;
            }

            checkedInitialRoute = true;

            if (!await userPreferencesService.IsOnboardingCompletedAsync())
            {
                await Navigation.PushModalAsync(services.GetRequiredService<Views.OnboardingPage>());
            }
        }

        private static ShellContent CreateShellContent<TPage>(IServiceProvider services, string title, string route)
            where TPage : Page
        {
            return new ShellContent
            {
                Title = title,
                Route = route,
                ContentTemplate = new DataTemplate(() => services.GetRequiredService<TPage>())
            };
        }
    }
}

#pragma warning restore CA1416
