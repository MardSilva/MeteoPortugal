namespace MeteoPortugal.App.Services.Interfaces;

public interface IUserPreferencesService
{
    void ApplySavedTheme();

    Task<bool> IsOnboardingCompletedAsync(CancellationToken cancellationToken = default);

    Task SetOnboardingCompletedAsync(bool isCompleted, CancellationToken cancellationToken = default);

    Task<string?> GetPreferredLocationIdAsync(CancellationToken cancellationToken = default);

    Task SetPreferredLocationIdAsync(string locationId, CancellationToken cancellationToken = default);

    Task<string> GetThemeAsync(CancellationToken cancellationToken = default);

    Task SetThemeAsync(string theme, CancellationToken cancellationToken = default);
}
