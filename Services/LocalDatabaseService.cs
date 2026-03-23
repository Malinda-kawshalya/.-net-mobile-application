using Recipes_app.Interfaces;
using Recipes_app.Models;

namespace Recipes_app.Services
{
    /// <summary>
    /// In-memory fallback database used when Firebase is not configured.
    /// Data persists only for the current app session.
    /// </summary>
    public class LocalDatabaseService : IDatabaseService
    {
        private readonly List<Recipe> _recipes = new();
        private readonly List<FavoriteRecipe> _favorites = new();
        private readonly List<ShoppingItem> _shoppingItems = new();

        public Task<List<Recipe>> GetUserRecipesAsync(string userId)
        {
            var items = _recipes
                .Where(r => string.Equals(r.CreatedBy, userId, StringComparison.OrdinalIgnoreCase))
                .ToList();
            return Task.FromResult(items);
        }

        public Task<Recipe?> GetRecipeAsync(string recipeId)
        {
            var recipe = _recipes.FirstOrDefault(r => r.Id == recipeId);
            return Task.FromResult(recipe);
        }

        public Task SaveRecipeAsync(Recipe recipe)
        {
            if (string.IsNullOrWhiteSpace(recipe.Id))
                recipe.Id = Guid.NewGuid().ToString();

            if (_recipes.All(r => r.Id != recipe.Id))
                _recipes.Add(recipe);

            return Task.CompletedTask;
        }

        public Task UpdateRecipeAsync(Recipe recipe)
        {
            var existing = _recipes.FirstOrDefault(r => r.Id == recipe.Id);
            if (existing == null)
            {
                _recipes.Add(recipe);
                return Task.CompletedTask;
            }

            existing.Title = recipe.Title;
            existing.Description = recipe.Description;
            existing.ImageUrl = recipe.ImageUrl;
            existing.Category = recipe.Category;
            existing.IngredientsJson = recipe.IngredientsJson;
            existing.InstructionsJson = recipe.InstructionsJson;
            existing.CookTimeMinutes = recipe.CookTimeMinutes;
            existing.PrepTimeMinutes = recipe.PrepTimeMinutes;
            existing.Servings = recipe.Servings;
            existing.Rating = recipe.Rating;
            existing.CreatedBy = recipe.CreatedBy;
            existing.CreatedAt = recipe.CreatedAt;
            existing.IsFromApi = recipe.IsFromApi;
            existing.Ingredients = recipe.Ingredients;
            existing.Instructions = recipe.Instructions;

            return Task.CompletedTask;
        }

        public Task DeleteRecipeAsync(string recipeId)
        {
            _recipes.RemoveAll(r => r.Id == recipeId);
            _favorites.RemoveAll(f => f.RecipeId == recipeId);
            _shoppingItems.RemoveAll(s => s.RecipeId == recipeId);
            return Task.CompletedTask;
        }

        public Task<List<FavoriteRecipe>> GetFavoritesAsync(string userId)
        {
            var items = _favorites
                .Where(f => string.Equals(f.UserId, userId, StringComparison.OrdinalIgnoreCase))
                .ToList();
            return Task.FromResult(items);
        }

        public Task AddFavoriteAsync(FavoriteRecipe favorite)
        {
            if (string.IsNullOrWhiteSpace(favorite.Id))
                favorite.Id = string.IsNullOrWhiteSpace(favorite.RecipeId)
                    ? Guid.NewGuid().ToString()
                    : favorite.RecipeId;

            if (_favorites.All(f => f.RecipeId != favorite.RecipeId || f.UserId != favorite.UserId))
                _favorites.Add(favorite);

            return Task.CompletedTask;
        }

        public Task RemoveFavoriteAsync(string favoriteId)
        {
            _favorites.RemoveAll(f => f.Id == favoriteId || f.RecipeId == favoriteId);
            return Task.CompletedTask;
        }

        public Task<bool> IsFavoriteAsync(string userId, string recipeId)
        {
            var isFavorite = _favorites.Any(f =>
                string.Equals(f.UserId, userId, StringComparison.OrdinalIgnoreCase) &&
                f.RecipeId == recipeId);
            return Task.FromResult(isFavorite);
        }

        public Task<List<ShoppingItem>> GetShoppingItemsAsync(string userId)
        {
            // Current model has no UserId, so return all in local mode.
            return Task.FromResult(_shoppingItems.ToList());
        }

        public Task AddShoppingItemAsync(ShoppingItem item)
        {
            if (string.IsNullOrWhiteSpace(item.Id))
                item.Id = Guid.NewGuid().ToString();

            if (_shoppingItems.All(s => s.Id != item.Id))
                _shoppingItems.Add(item);

            return Task.CompletedTask;
        }

        public Task UpdateShoppingItemAsync(ShoppingItem item)
        {
            var existing = _shoppingItems.FirstOrDefault(s => s.Id == item.Id);
            if (existing == null)
            {
                _shoppingItems.Add(item);
                return Task.CompletedTask;
            }

            existing.Name = item.Name;
            existing.Quantity = item.Quantity;
            existing.Unit = item.Unit;
            existing.IsChecked = item.IsChecked;
            existing.RecipeId = item.RecipeId;
            existing.RecipeTitle = item.RecipeTitle;

            return Task.CompletedTask;
        }

        public Task DeleteShoppingItemAsync(string itemId)
        {
            _shoppingItems.RemoveAll(s => s.Id == itemId);
            return Task.CompletedTask;
        }

        public Task ClearCompletedShoppingItemsAsync(string userId)
        {
            _shoppingItems.RemoveAll(s => s.IsChecked);
            return Task.CompletedTask;
        }
    }
}