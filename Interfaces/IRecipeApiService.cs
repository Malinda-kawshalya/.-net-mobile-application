using Recipes_app.Models;

namespace Recipes_app.Interfaces
{
    /// <summary>
    /// Contract for external recipe API calls (TheMealDB).
    /// </summary>
    public interface IRecipeApiService
    {
        /// <summary>Search recipes by name.</summary>
        Task<List<Recipe>> SearchRecipesAsync(string query);

        /// <summary>Get recipes filtered by category.</summary>
        Task<List<Recipe>> GetRecipesByCategoryAsync(string category);

        /// <summary>Get a single recipe by its API ID.</summary>
        Task<Recipe?> GetRecipeByIdAsync(string id);

        /// <summary>Get a random recipe for the featured section.</summary>
        Task<Recipe?> GetRandomRecipeAsync();

        /// <summary>Get all available categories from the API.</summary>
        Task<List<Category>> GetCategoriesAsync();
    }
}
