namespace Recipes_app.Models
{
    public class ShoppingItem
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string Quantity { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public bool IsChecked { get; set; }
        public string RecipeId { get; set; } = string.Empty;
        public string RecipeTitle { get; set; } = string.Empty;
    }
}
