using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Recipes_app.Interfaces;
using Recipes_app.Views;

namespace Recipes_app.ViewModels
{
    /// <summary>
    /// ViewModel for ProfilePage - user settings, dark mode, logout.
    /// </summary>
    public partial class ProfileViewModel : BaseViewModel
    {
        private readonly IAuthService _authService;
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private string _displayName = string.Empty;

        [ObservableProperty]
        private string _email = string.Empty;

        [ObservableProperty]
        private bool _isDarkMode;

        [ObservableProperty]
        private string _memberSince = string.Empty;

        [ObservableProperty]
        private bool _isLoggedIn;

        public ProfileViewModel(IAuthService authService, INavigationService navigationService)
        {
            _authService = authService;
            _navigationService = navigationService;
            Title = "Profile";
        }

        [RelayCommand]
        private async Task LoadProfileAsync()
        {
            try
            {
                IsLoggedIn = _authService.IsSignedIn;
                Email = _authService.CurrentUserEmail ?? "Not signed in";
                DisplayName = Email.Split('@').FirstOrDefault() ?? "User";
                MemberSince = "Member since 2025";

                // Load dark mode preference
                IsDarkMode = Preferences.Get("DarkMode", false);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to load profile: {ex.Message}", "OK");
            }
        }

        partial void OnIsDarkModeChanged(bool value)
        {
            Preferences.Set("DarkMode", value);

            if (Application.Current != null)
            {
                Application.Current.UserAppTheme = value ? AppTheme.Dark : AppTheme.Light;
            }
        }

        [RelayCommand]
        private async Task LogoutAsync()
        {
            bool confirm = await Shell.Current.DisplayAlert(
                "Logout",
                "Are you sure you want to logout?",
                "Yes", "No");

            if (!confirm) return;

            try
            {
                await _authService.SignOutAsync();
                await Shell.Current.GoToAsync(nameof(Views.LoginPage));
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Logout failed: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task GoToLoginAsync()
        {
            await Shell.Current.GoToAsync(nameof(Views.LoginPage));
        }

        [RelayCommand]
        private async Task EditProfileAsync()
        {
            await Shell.Current.DisplayAlert("Coming Soon", "Profile editing will be available in a future update.", "OK");
        }
    }
}
