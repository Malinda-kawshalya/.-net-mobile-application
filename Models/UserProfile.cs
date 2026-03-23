namespace Recipes_app.Models
{
    public class UserProfile
    {
        public string UserId { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public DateTime JoinedDate { get; set; } = DateTime.UtcNow;
    }
}
