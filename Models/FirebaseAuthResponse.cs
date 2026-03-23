namespace Recipes_app.Models
{
    /// <summary>
    /// Represents the response from Firebase Auth REST API for sign-in/sign-up.
    /// </summary>
    public class FirebaseAuthResponse
    {
        public string IdToken { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public string ExpiresIn { get; set; } = string.Empty;
        public string LocalId { get; set; } = string.Empty;
        public bool Registered { get; set; }
        public string DisplayName { get; set; } = string.Empty;
    }

    /// <summary>
    /// Represents a Firebase Auth error response.
    /// </summary>
    public class FirebaseErrorResponse
    {
        public FirebaseError? Error { get; set; }
    }

    public class FirebaseError
    {
        public int Code { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
