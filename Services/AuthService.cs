using System.Text;
using Newtonsoft.Json;
using Recipes_app.Helpers;
using Recipes_app.Interfaces;
using Recipes_app.Models;

namespace Recipes_app.Services
{
    /// <summary>
    /// Firebase Authentication REST API service implementation.
    /// Handles sign-in, sign-up, password reset, and token management.
    /// Design Pattern: Singleton (registered as singleton in DI).
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private string? _idToken;
        private string? _userId;
        private string? _userEmail;
        private string? _refreshToken;

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public bool IsSignedIn => !string.IsNullOrEmpty(_idToken);
        public string? CurrentUserToken => _idToken;
        public string? CurrentUserId => _userId;
        public string? CurrentUserEmail => _userEmail;

        /// <summary>
        /// Sign in with email and password using Firebase Auth REST API.
        /// Endpoint: signInWithPassword
        /// </summary>
        public async Task<FirebaseAuthResponse> SignInAsync(string email, string password)
        {
            EnsureFirebaseConfigured();
            var url = $"{Constants.FirebaseAuthUrl}:signInWithPassword?key={Constants.FirebaseApiKey}";

            var payload = new
            {
                email,
                password,
                returnSecureToken = true
            };

            var json = JsonConvert.SerializeObject(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = JsonConvert.DeserializeObject<FirebaseErrorResponse>(responseBody);
                throw new Exception(errorResponse?.Error?.Message ?? "Authentication failed");
            }

            var authResponse = JsonConvert.DeserializeObject<FirebaseAuthResponse>(responseBody)
                ?? throw new Exception("Invalid response from Firebase");

            // Store tokens
            await StoreTokensAsync(authResponse);

            return authResponse;
        }

        /// <summary>
        /// Register a new user with email, password, and display name.
        /// Endpoint: signUp, then update profile for displayName.
        /// </summary>
        public async Task<FirebaseAuthResponse> SignUpAsync(string email, string password, string displayName)
        {
            EnsureFirebaseConfigured();
            // Step 1: Create account
            var url = $"{Constants.FirebaseAuthUrl}:signUp?key={Constants.FirebaseApiKey}";

            var payload = new
            {
                email,
                password,
                returnSecureToken = true
            };

            var json = JsonConvert.SerializeObject(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = JsonConvert.DeserializeObject<FirebaseErrorResponse>(responseBody);
                throw new Exception(errorResponse?.Error?.Message ?? "Registration failed");
            }

            var authResponse = JsonConvert.DeserializeObject<FirebaseAuthResponse>(responseBody)
                ?? throw new Exception("Invalid response from Firebase");

            // Step 2: Update display name
            await UpdateProfileAsync(authResponse.IdToken, displayName);
            authResponse.DisplayName = displayName;

            // Store tokens
            await StoreTokensAsync(authResponse);

            return authResponse;
        }

        /// <summary>
        /// Send password reset email via Firebase Auth REST API.
        /// Endpoint: sendOobCode with requestType PASSWORD_RESET.
        /// </summary>
        public async Task SendPasswordResetAsync(string email)
        {
            EnsureFirebaseConfigured();
            var url = $"{Constants.FirebaseAuthUrl}:sendOobCode?key={Constants.FirebaseApiKey}";

            var payload = new
            {
                requestType = "PASSWORD_RESET",
                email
            };

            var json = JsonConvert.SerializeObject(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var errorResponse = JsonConvert.DeserializeObject<FirebaseErrorResponse>(responseBody);
                throw new Exception(errorResponse?.Error?.Message ?? "Failed to send password reset email");
            }
        }

        /// <summary>
        /// Sign out the current user and clear stored tokens.
        /// </summary>
        public async Task SignOutAsync()
        {
            _idToken = null;
            _userId = null;
            _userEmail = null;
            _refreshToken = null;

            try
            {
                SecureStorage.Remove(Constants.AuthTokenKey);
                SecureStorage.Remove(Constants.RefreshTokenKey);
                SecureStorage.Remove(Constants.UserIdKey);
                SecureStorage.Remove(Constants.UserEmailKey);
            }
            catch
            {
                // Ignore secure storage errors during signout
            }

            await Task.CompletedTask;
        }

        /// <summary>
        /// Try to restore session from SecureStorage on app launch.
        /// </summary>
        public async Task<bool> TryAutoLoginAsync()
        {
            try
            {
                var token = await SecureStorage.GetAsync(Constants.AuthTokenKey);
                var userId = await SecureStorage.GetAsync(Constants.UserIdKey);
                var email = await SecureStorage.GetAsync(Constants.UserEmailKey);
                var refreshToken = await SecureStorage.GetAsync(Constants.RefreshTokenKey);

                if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(userId))
                {
                    _idToken = token;
                    _userId = userId;
                    _userEmail = email;
                    _refreshToken = refreshToken;
                    return true;
                }
            }
            catch
            {
                // SecureStorage not available, ignore
            }

            return false;
        }

        /// <summary>
        /// Updates the user's display name using Firebase Auth update profile.
        /// </summary>
        private async Task UpdateProfileAsync(string idToken, string displayName)
        {
            EnsureFirebaseConfigured();
            var url = $"{Constants.FirebaseAuthUrl}:update?key={Constants.FirebaseApiKey}";

            var payload = new
            {
                idToken,
                displayName,
                returnSecureToken = false
            };

            var json = JsonConvert.SerializeObject(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            await _httpClient.PostAsync(url, content);
        }

        /// <summary>
        /// Persists authentication tokens in SecureStorage.
        /// </summary>
        private async Task StoreTokensAsync(FirebaseAuthResponse authResponse)
        {
            _idToken = authResponse.IdToken;
            _userId = authResponse.LocalId;
            _userEmail = authResponse.Email;
            _refreshToken = authResponse.RefreshToken;

            try
            {
                await SecureStorage.SetAsync(Constants.AuthTokenKey, authResponse.IdToken);
                await SecureStorage.SetAsync(Constants.RefreshTokenKey, authResponse.RefreshToken);
                await SecureStorage.SetAsync(Constants.UserIdKey, authResponse.LocalId);
                await SecureStorage.SetAsync(Constants.UserEmailKey, authResponse.Email);
            }
            catch
            {
                // SecureStorage may not be available on all platforms
            }
        }

        private static void EnsureFirebaseConfigured()
        {
            if (Constants.HasFirebaseConfig)
                return;

            throw new InvalidOperationException(
                "Firebase is not configured. Set FIREBASE_API_KEY and FIREBASE_PROJECT_ID environment variables or update Helpers/Constants.cs.");
        }
    }
}
