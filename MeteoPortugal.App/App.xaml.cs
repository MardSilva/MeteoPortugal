namespace MeteoPortugal.App
{
    public partial class App : Application
    {
        private readonly AppShell appShell;

        public App(AppShell appShell, Services.Interfaces.IUserPreferencesService userPreferencesService)
        {
            InitializeComponent();
            userPreferencesService.ApplySavedTheme();
            this.appShell = appShell;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(appShell);
        }
    }
}
