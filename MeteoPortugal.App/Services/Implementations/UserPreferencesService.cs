using MeteoPortugal.App.Services.Interfaces;
using Microsoft.Maui.Storage;

namespace MeteoPortugal.App.Services.Implementations;

public sealed class UserPreferencesService : IUserPreferencesService
{
    public const string SystemTheme = "system";
    public const string LightTheme = "light";
    public const string DarkTheme = "dark";

    private const string OnboardingCompletedKey = "onboarding_completed";
    private const string PreferredLocationIdKey = "preferred_location_id";
    private const string ThemeKey = "theme";

    public void ApplySavedTheme()
    {
        ApplyTheme(Preferences.Get(ThemeKey, SystemTheme));
    }

    public Task<bool> IsOnboardingCompletedAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Preferences.Get(OnboardingCompletedKey, false));
    }

    public Task SetOnboardingCompletedAsync(bool isCompleted, CancellationToken cancellationToken = default)
    {
        Preferences.Set(OnboardingCompletedKey, isCompleted);
        return Task.CompletedTask;
    }

    public Task<string?> GetPreferredLocationIdAsync(CancellationToken cancellationToken = default)
    {
        var locationId = Preferences.Get(PreferredLocationIdKey, string.Empty);
        return Task.FromResult(string.IsNullOrWhiteSpace(locationId) ? null : locationId);
    }

    public Task SetPreferredLocationIdAsync(string locationId, CancellationToken cancellationToken = default)
    {
        Preferences.Set(PreferredLocationIdKey, locationId);
        return Task.CompletedTask;
    }

    public Task<string> GetThemeAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Preferences.Get(ThemeKey, SystemTheme));
    }

    public Task SetThemeAsync(string theme, CancellationToken cancellationToken = default)
    {
        Preferences.Set(ThemeKey, theme);
        ApplyTheme(theme);
        return Task.CompletedTask;
    }

    private static void ApplyTheme(string theme)
    {
        if (Application.Current is null)
        {
            return;
        }

        Application.Current.UserAppTheme = theme switch
        {
            LightTheme => AppTheme.Light,
            DarkTheme => AppTheme.Dark,
            _ => AppTheme.Unspecified
        };
    }
}
