using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Recipes_app.Interfaces;

namespace Recipes_app.ViewModels
{
    /// <summary>
    /// ViewModel for RegisterPage - handles Firebase Auth sign-up.
    /// </summary>
    public partial class RegisterViewModel : BaseViewModel
    {
        private readonly IAuthService _authService;
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private string _displayName = string.Empty;

        [ObservableProperty]
        private string _email = string.Empty;

        [ObservableProperty]
        private string _password = string.Empty;

        [ObservableProperty]
        private string _confirmPassword = string.Empty;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [ObservableProperty]
        private bool _hasError;

        public RegisterViewModel(IAuthService authService, INavigationService navigationService)
        {
            _authService = authService;
            _navigationService = navigationService;
            Title = "Create Account";
        }

        [RelayCommand]
        private async Task RegisterAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                HasError = false;
                ErrorMessage = string.Empty;

                // Validation
                if (string.IsNullOrWhiteSpace(DisplayName))
                {
                    ErrorMessage = "Please enter your name.";
                    HasError = true;
                    return;
                }

                if (string.IsNullOrWhiteSpace(Email) || !Email.Contains("@"))
                {
                    ErrorMessage = "Please enter a valid email address.";
                    HasError = true;
                    return;
                }

                if (string.IsNullOrWhiteSpace(Password) || Password.Length < 6)
                {
                    ErrorMessage = "Password must be at least 6 characters.";
                    HasError = true;
                    return;
                }

                if (Password != ConfirmPassword)
                {
                    ErrorMessage = "Passwords do not match.";
                    HasError = true;
                    return;
                }

                var response = await _authService.SignUpAsync(Email, Password, DisplayName);

                if (response != null && !string.IsNullOrEmpty(response.IdToken))
                {
                    await Shell.Current.DisplayAlert("Success", "Account created successfully!", "OK");
                    await Shell.Current.GoToAsync("//HomePage");
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message.Contains("EMAIL_EXISTS") ? "An account with this email already exists." :
                               $"Registration failed: {ex.Message}";
                HasError = true;
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task GoToLoginAsync()
        {
            await _navigationService.GoBackAsync();
        }
    }
}
