using Recipes_app.Models;

namespace Recipes_app.Interfaces
{
    /// <summary>
    /// Contract for Firebase Authentication operations.
    /// </summary>
    public interface IAuthService
    {
        /// <summary>Gets whether any user is currently signed in.</summary>
        bool IsSignedIn { get; }

        /// <summary>Gets the current user's ID token.</summary>
        string? CurrentUserToken { get; }

        /// <summary>Gets the current user's local ID.</summary>
        string? CurrentUserId { get; }

        /// <summary>Gets the current user's email.</summary>
        string? CurrentUserEmail { get; }

        /// <summary>Sign in with email and password via Firebase Auth REST API.</summary>
        Task<FirebaseAuthResponse> SignInAsync(string email, string password);

        /// <summary>Register a new user with email and password.</summary>
        Task<FirebaseAuthResponse> SignUpAsync(string email, string password, string displayName);

        /// <summary>Send a password reset email.</summary>
        Task SendPasswordResetAsync(string email);

        /// <summary>Sign out the current user and clear stored tokens.</summary>
        Task SignOutAsync();

        /// <summary>Try to restore a previous session from secure storage.</summary>
        Task<bool> TryAutoLoginAsync();
    }
}
