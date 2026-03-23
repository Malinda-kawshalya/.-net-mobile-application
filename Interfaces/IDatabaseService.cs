using Recipes_app.Models;

namespace Recipes_app.Interfaces
{
    /// <summary>
    /// Contract for cloud database operations (Firebase Firestore).
    /// </summary>
    public interface IDatabaseService
    {
        // ---- Recipes CRUD ----
        Task<List<Recipe>> GetUserRecipesAsync(string userId);
        Task<Recipe?> GetRecipeAsync(string recipeId);
        Task SaveRecipeAsync(Recipe recipe);
        Task UpdateRecipeAsync(Recipe recipe);
        Task DeleteRecipeAsync(string recipeId);

        // ---- Favorites ----
        Task<List<FavoriteRecipe>> GetFavoritesAsync(string userId);
        Task AddFavoriteAsync(FavoriteRecipe favorite);
        Task RemoveFavoriteAsync(string favoriteId);
        Task<bool> IsFavoriteAsync(string userId, string recipeId);

        // ---- Shopping List ----
        Task<List<ShoppingItem>> GetShoppingItemsAsync(string userId);
        Task AddShoppingItemAsync(ShoppingItem item);
        Task UpdateShoppingItemAsync(ShoppingItem item);
        Task DeleteShoppingItemAsync(string itemId);
        Task ClearCompletedShoppingItemsAsync(string userId);
    }
}
