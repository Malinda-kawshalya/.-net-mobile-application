namespace Recipes_app.Models
{
    public class FavoriteRecipe
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string RecipeId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public DateTime SavedAt { get; set; } = DateTime.UtcNow;
    }
}
