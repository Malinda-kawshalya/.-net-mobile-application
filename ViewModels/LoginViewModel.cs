using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Recipes_app.Interfaces;
using Recipes_app.Views;

namespace Recipes_app.ViewModels
{
    /// <summary>
    /// ViewModel for LoginPage - handles Firebase Auth sign-in.
    /// </summary>
    public partial class LoginViewModel : BaseViewModel
    {
        private readonly IAuthService _authService;
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private string _email = string.Empty;

        [ObservableProperty]
        private string _password = string.Empty;

        [ObservableProperty]
        private bool _rememberMe;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [ObservableProperty]
        private bool _hasError;

        public LoginViewModel(IAuthService authService, INavigationService navigationService)
        {
            _authService = authService;
            _navigationService = navigationService;
            Title = "Login";
        }

        [RelayCommand]
        private async Task LoginAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                HasError = false;
                ErrorMessage = string.Empty;

                if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
                {
                    ErrorMessage = "Please enter both email and password.";
                    HasError = true;
                    return;
                }

                var response = await _authService.SignInAsync(Email, Password);

                if (response != null && !string.IsNullOrEmpty(response.IdToken))
                {
                    // Navigate to main app shell
                    await Shell.Current.GoToAsync("//HomePage");
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message.Contains("INVALID_PASSWORD") ? "Invalid password." :
                               ex.Message.Contains("EMAIL_NOT_FOUND") ? "Email not found." :
                               ex.Message.Contains("INVALID_LOGIN_CREDENTIALS") ? "Invalid email or password." :
                               $"Login failed: {ex.Message}";
                HasError = true;
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task GoToRegisterAsync()
        {
            await _navigationService.GoToAsync(nameof(Views.RegisterPage));
        }

        [RelayCommand]
        private async Task ForgotPasswordAsync()
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                ErrorMessage = "Please enter your email address first.";
                HasError = true;
                return;
            }

            try
            {
                await _authService.SendPasswordResetAsync(Email);
                await Shell.Current.DisplayAlert("Password Reset", "A password reset email has been sent.", "OK");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to send reset email: {ex.Message}";
                HasError = true;
            }
        }
    }
}
