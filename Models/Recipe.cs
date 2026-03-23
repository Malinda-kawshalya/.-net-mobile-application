namespace Recipes_app.Models
{
    public class Recipe
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string IngredientsJson { get; set; } = string.Empty;
        public string InstructionsJson { get; set; } = string.Empty;
        public int CookTimeMinutes { get; set; }
        public int PrepTimeMinutes { get; set; }
        public int Servings { get; set; }
        public double Rating { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsFromApi { get; set; }

        // Populated at runtime from JSON fields
        public List<Ingredient> Ingredients { get; set; } = new();
        public List<string> Instructions { get; set; } = new();
    }
}
