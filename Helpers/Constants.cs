namespace Recipes_app.Helpers
{
    /// <summary>
    /// Application-wide constants.
    /// </summary>
    public static class Constants
    {
        // Firebase configuration:
        // 1) Environment variables (recommended): FIREBASE_API_KEY, FIREBASE_PROJECT_ID
        // 2) Fallback values below
        public static string FirebaseApiKey =>
            Environment.GetEnvironmentVariable("FIREBASE_API_KEY") ?? "YOUR_FIREBASE_API_KEY";

        public const string FirebaseAuthUrl = "https://identitytoolkit.googleapis.com/v1/accounts";

        public static string FirebaseProjectId =>
            Environment.GetEnvironmentVariable("FIREBASE_PROJECT_ID") ?? "YOUR_PROJECT_ID";

        public const string FirestoreBaseUrl = "https://firestore.googleapis.com/v1/projects/{0}/databases/(default)/documents";

        public static bool HasFirebaseConfig =>
            !string.IsNullOrWhiteSpace(FirebaseApiKey)
            && !string.IsNullOrWhiteSpace(FirebaseProjectId)
            && !FirebaseApiKey.StartsWith("YOUR_", StringComparison.OrdinalIgnoreCase)
            && !FirebaseProjectId.StartsWith("YOUR_", StringComparison.OrdinalIgnoreCase);

        // TheMealDB API
        public const string MealDbBaseUrl = "https://www.themealdb.com/api/json/v1/1/";

        // Secure storage keys
        public const string AuthTokenKey = "auth_token";
        public const string RefreshTokenKey = "refresh_token";
        public const string UserIdKey = "user_id";
        public const string UserEmailKey = "user_email";
    }
}
