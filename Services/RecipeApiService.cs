using Newtonsoft.Json;
using Recipes_app.Helpers;
using Recipes_app.Interfaces;
using Recipes_app.Models;

namespace Recipes_app.Services
{
    /// <summary>
    /// TheMealDB API service implementation.
    /// Fetches recipes, categories, and random meals from the public API.
    /// Design Pattern: Strategy (interchangeable API services via interface).
    /// </summary>
    public class RecipeApiService : IRecipeApiService
    {
        private readonly HttpClient _httpClient;

        public RecipeApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(Constants.MealDbBaseUrl);
        }

        /// <summary>
        /// Search recipes by name using TheMealDB search endpoint.
        /// GET: /search.php?s={query}
        /// </summary>
        public async Task<List<Recipe>> SearchRecipesAsync(string query)
        {
            try
            {
                var response = await _httpClient.GetStringAsync($"search.php?s={Uri.EscapeDataString(query)}");
                var mealResponse = JsonConvert.DeserializeObject<MealDbResponse>(response);

                if (mealResponse?.Meals == null)
                    return new List<Recipe>();

                return mealResponse.Meals.Select(m => m.ToRecipe()).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[RecipeApiService] SearchRecipesAsync error: {ex.Message}");
                return new List<Recipe>();
            }
        }

        /// <summary>
        /// Get recipes filtered by category using TheMealDB filter endpoint.
        /// GET: /filter.php?c={category}
        /// Returns lightweight results (id, name, thumb only). 
        /// Full details must be fetched separately with GetRecipeByIdAsync.
        /// </summary>
        public async Task<List<Recipe>> GetRecipesByCategoryAsync(string category)
        {
            try
            {
                var response = await _httpClient.GetStringAsync($"filter.php?c={Uri.EscapeDataString(category)}");
                var filterResponse = JsonConvert.DeserializeObject<MealDbFilterResponse>(response);

                if (filterResponse?.Meals == null)
                    return new List<Recipe>();

                return filterResponse.Meals.Select(m => new Recipe
                {
                    Id = m.IdMeal,
                    Title = m.StrMeal,
                    ImageUrl = m.StrMealThumb,
                    Category = category,
                    IsFromApi = true,
                    CreatedAt = DateTime.UtcNow
                }).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[RecipeApiService] GetRecipesByCategoryAsync error: {ex.Message}");
                return new List<Recipe>();
            }
        }

        /// <summary>
        /// Get a single recipe by its MealDB ID.
        /// GET: /lookup.php?i={id}
        /// </summary>
        public async Task<Recipe?> GetRecipeByIdAsync(string id)
        {
            try
            {
                var response = await _httpClient.GetStringAsync($"lookup.php?i={id}");
                var mealResponse = JsonConvert.DeserializeObject<MealDbResponse>(response);

                return mealResponse?.Meals?.FirstOrDefault()?.ToRecipe();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[RecipeApiService] GetRecipeByIdAsync error: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Get a random recipe from TheMealDB.
        /// GET: /random.php
        /// </summary>
        public async Task<Recipe?> GetRandomRecipeAsync()
        {
            try
            {
                var response = await _httpClient.GetStringAsync("random.php");
                var mealResponse = JsonConvert.DeserializeObject<MealDbResponse>(response);

                return mealResponse?.Meals?.FirstOrDefault()?.ToRecipe();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[RecipeApiService] GetRandomRecipeAsync error: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Get all available meal categories from TheMealDB.
        /// GET: /categories.php
        /// </summary>
        public async Task<List<Category>> GetCategoriesAsync()
        {
            try
            {
                var response = await _httpClient.GetStringAsync("categories.php");
                var categoriesResponse = JsonConvert.DeserializeObject<MealDbCategoriesResponse>(response);

                if (categoriesResponse?.Categories == null)
                    return new List<Category>();

                return categoriesResponse.Categories.Select(c => new Category
                {
                    Id = c.IdCategory,
                    Name = c.StrCategory,
                    ImageUrl = c.StrCategoryThumb,
                    Icon = GetCategoryIcon(c.StrCategory),
                    Color = GetCategoryColor(c.StrCategory)
                }).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[RecipeApiService] GetCategoriesAsync error: {ex.Message}");
                return new List<Category>();
            }
        }

        /// <summary>
        /// Maps category name to an emoji icon for UI display.
        /// </summary>
        private static string GetCategoryIcon(string category)
        {
            return category.ToLower() switch
            {
                "beef" => "🥩",
                "chicken" => "🍗",
                "dessert" => "🍰",
                "lamb" => "🐑",
                "miscellaneous" => "🍽️",
                "pasta" => "🍝",
                "pork" => "🥓",
                "seafood" => "🐟",
                "side" => "🥗",
                "starter" => "🥄",
                "vegan" => "🌱",
                "vegetarian" => "🥬",
                "breakfast" => "🥞",
                "goat" => "🐐",
                _ => "🍴"
            };
        }

        /// <summary>
        /// Maps category name to a color for UI card backgrounds.
        /// </summary>
        private static string GetCategoryColor(string category)
        {
            return category.ToLower() switch
            {
                "beef" => "#E85D2C",
                "chicken" => "#F9A825",
                "dessert" => "#E91E63",
                "lamb" => "#795548",
                "miscellaneous" => "#607D8B",
                "pasta" => "#FF9800",
                "pork" => "#F44336",
                "seafood" => "#03A9F4",
                "side" => "#8BC34A",
                "starter" => "#9C27B0",
                "vegan" => "#2E7D32",
                "vegetarian" => "#4CAF50",
                "breakfast" => "#FFEB3B",
                "goat" => "#A1887F",
                _ => "#9E9E9E"
            };
        }
    }
}
