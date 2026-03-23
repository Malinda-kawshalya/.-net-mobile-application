namespace Recipes_app
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Apply saved theme preference
            var isDarkMode = Preferences.Get("DarkMode", false);
            UserAppTheme = isDarkMode ? AppTheme.Dark : AppTheme.Light;

            MainPage = new AppShell();
        }
    }
}
