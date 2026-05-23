using MeteoPortugal.App.Services.Implementations;
using MeteoPortugal.App.Services.Interfaces;
using MeteoPortugal.App.ViewModels;
using MeteoPortugal.App.Views;
using MeteoPortugal.App.Infrastructure.Ipma;
using Microsoft.Extensions.Logging;

namespace MeteoPortugal.App
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("Manrope-Regular.ttf", "Manrope");
                    fonts.AddFont("Manrope-SemiBold.ttf", "ManropeSemiBold");
                    fonts.AddFont("Manrope-Bold.ttf", "ManropeBold");
                });

            builder.Services.AddSingleton(new HttpClient
            {
                BaseAddress = new Uri("https://api.ipma.pt/")
            });
            builder.Services.AddSingleton<IpmaWeatherClient>();

            builder.Services.AddSingleton<IWeatherService, IpmaWeatherService>();
            builder.Services.AddSingleton<ILocationService, LocationService>();
            builder.Services.AddSingleton<IUserPreferencesService, UserPreferencesService>();
            builder.Services.AddSingleton<IWeatherCacheService, WeatherCacheService>();
            builder.Services.AddSingleton<IObservationService, ObservationService>();
            builder.Services.AddSingleton<IWeatherWarningService, WeatherWarningService>();

            builder.Services.AddSingleton<AppShell>();

            builder.Services.AddTransient<OnboardingViewModel>();
            builder.Services.AddTransient<HomeViewModel>();
            builder.Services.AddTransient<LocationsViewModel>();
            builder.Services.AddTransient<ObservationViewModel>();
            builder.Services.AddTransient<SettingsViewModel>();
            builder.Services.AddTransient<DailyInfoEditViewModel>();

            builder.Services.AddTransient<OnboardingPage>();
            builder.Services.AddTransient<HomePage>();
            builder.Services.AddTransient<LocationsPage>();
            builder.Services.AddTransient<ObservationPage>();
            builder.Services.AddTransient<SettingsPage>();
            builder.Services.AddTransient<DailyInfoEditPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
