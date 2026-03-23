using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Recipes_app.Helpers;
using Recipes_app.Interfaces;
using Recipes_app.Models;

namespace Recipes_app.Services
{
    /// <summary>
    /// Firebase Firestore REST API service implementation.
    /// Provides cloud-based CRUD operations for recipes, favorites, and shopping items.
    /// Design Pattern: Repository (encapsulates data access behind interface).
    /// </summary>
    public class FirestoreService : IDatabaseService
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthService _authService;

        private string BaseUrl => string.Format(Constants.FirestoreBaseUrl, Constants.FirebaseProjectId);

        public FirestoreService(HttpClient httpClient, IAuthService authService)
        {
            _httpClient = httpClient;
            _authService = authService;
        }

        #region Recipes

        public async Task<List<Recipe>> GetUserRecipesAsync(string userId)
        {
            EnsureFirebaseConfigured();
            try
            {
                var url = $"{BaseUrl}/users/{userId}/recipes";
                var request = CreateAuthorizedRequest(HttpMethod.Get, url);
                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                    return new List<Recipe>();

                var json = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(json);
                var documents = jObject["documents"] as JArray;

                if (documents == null)
                    return new List<Recipe>();

                return documents.Select(doc => DocumentToRecipe(doc)).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[FirestoreService] GetUserRecipesAsync error: {ex.Message}");
                return new List<Recipe>();
            }
        }

        public async Task<Recipe?> GetRecipeAsync(string recipeId)
        {
            EnsureFirebaseConfigured();
            try
            {
                var userId = _authService.CurrentUserId;
                if (string.IsNullOrEmpty(userId)) return null;

                var url = $"{BaseUrl}/users/{userId}/recipes/{recipeId}";
                var request = CreateAuthorizedRequest(HttpMethod.Get, url);
                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                    return null;

                var json = await response.Content.ReadAsStringAsync();
                var doc = JObject.Parse(json);
                return DocumentToRecipe(doc);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[FirestoreService] GetRecipeAsync error: {ex.Message}");
                return null;
            }
        }

        public async Task SaveRecipeAsync(Recipe recipe)
        {
            EnsureFirebaseConfigured();
            try
            {
                var userId = _authService.CurrentUserId;
                if (string.IsNullOrEmpty(userId)) return;

                var url = $"{BaseUrl}/users/{userId}/recipes?documentId={recipe.Id}";
                var firestoreDoc = RecipeToDocument(recipe);
                var content = new StringContent(firestoreDoc, Encoding.UTF8, "application/json");
                var request = CreateAuthorizedRequest(HttpMethod.Post, url);
                request.Content = content;

                await _httpClient.SendAsync(request);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[FirestoreService] SaveRecipeAsync error: {ex.Message}");
            }
        }

        public async Task UpdateRecipeAsync(Recipe recipe)
        {
            EnsureFirebaseConfigured();
            try
            {
                var userId = _authService.CurrentUserId;
                if (string.IsNullOrEmpty(userId)) return;

                var url = $"{BaseUrl}/users/{userId}/recipes/{recipe.Id}";
                var firestoreDoc = RecipeToDocument(recipe);
                var content = new StringContent(firestoreDoc, Encoding.UTF8, "application/json");
                var request = CreateAuthorizedRequest(new HttpMethod("PATCH"), url);
                request.Content = content;

                await _httpClient.SendAsync(request);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[FirestoreService] UpdateRecipeAsync error: {ex.Message}");
            }
        }

        public async Task DeleteRecipeAsync(string recipeId)
        {
            EnsureFirebaseConfigured();
            try
            {
                var userId = _authService.CurrentUserId;
                if (string.IsNullOrEmpty(userId)) return;

                var url = $"{BaseUrl}/users/{userId}/recipes/{recipeId}";
                var request = CreateAuthorizedRequest(HttpMethod.Delete, url);
                await _httpClient.SendAsync(request);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[FirestoreService] DeleteRecipeAsync error: {ex.Message}");
            }
        }

        #endregion

        #region Favorites

        public async Task<List<FavoriteRecipe>> GetFavoritesAsync(string userId)
        {
            EnsureFirebaseConfigured();
            try
            {
                var url = $"{BaseUrl}/users/{userId}/favorites";
                var request = CreateAuthorizedRequest(HttpMethod.Get, url);
                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                    return new List<FavoriteRecipe>();

                var json = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(json);
                var documents = jObject["documents"] as JArray;

                if (documents == null)
                    return new List<FavoriteRecipe>();

                return documents.Select(doc => DocumentToFavorite(doc)).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[FirestoreService] GetFavoritesAsync error: {ex.Message}");
                return new List<FavoriteRecipe>();
            }
        }

        public async Task AddFavoriteAsync(FavoriteRecipe favorite)
        {
            EnsureFirebaseConfigured();
            try
            {
                var userId = _authService.CurrentUserId;
                if (string.IsNullOrEmpty(userId)) return;

                // Use recipeId as the document ID so we can delete by recipeId later
                var url = $"{BaseUrl}/users/{userId}/favorites?documentId={favorite.RecipeId}";
                var firestoreDoc = FavoriteToDocument(favorite);
                var content = new StringContent(firestoreDoc, Encoding.UTF8, "application/json");
                var request = CreateAuthorizedRequest(HttpMethod.Post, url);
                request.Content = content;

                await _httpClient.SendAsync(request);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[FirestoreService] AddFavoriteAsync error: {ex.Message}");
            }
        }

        public async Task RemoveFavoriteAsync(string favoriteId)
        {
            EnsureFirebaseConfigured();
            try
            {
                var userId = _authService.CurrentUserId;
                if (string.IsNullOrEmpty(userId)) return;

                var url = $"{BaseUrl}/users/{userId}/favorites/{favoriteId}";
                var request = CreateAuthorizedRequest(HttpMethod.Delete, url);
                await _httpClient.SendAsync(request);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[FirestoreService] RemoveFavoriteAsync error: {ex.Message}");
            }
        }

        public async Task<bool> IsFavoriteAsync(string userId, string recipeId)
        {
            EnsureFirebaseConfigured();
            try
            {
                var favorites = await GetFavoritesAsync(userId);
                return favorites.Any(f => f.RecipeId == recipeId);
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Shopping List

        public async Task<List<ShoppingItem>> GetShoppingItemsAsync(string userId)
        {
            EnsureFirebaseConfigured();
            try
            {
                var url = $"{BaseUrl}/users/{userId}/shoppingItems";
                var request = CreateAuthorizedRequest(HttpMethod.Get, url);
                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                    return new List<ShoppingItem>();

                var json = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(json);
                var documents = jObject["documents"] as JArray;

                if (documents == null)
                    return new List<ShoppingItem>();

                return documents.Select(doc => DocumentToShoppingItem(doc)).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[FirestoreService] GetShoppingItemsAsync error: {ex.Message}");
                return new List<ShoppingItem>();
            }
        }

        public async Task AddShoppingItemAsync(ShoppingItem item)
        {
            EnsureFirebaseConfigured();
            try
            {
                var userId = _authService.CurrentUserId;
                if (string.IsNullOrEmpty(userId)) return;

                // Assign a GUID if no ID so we can update/delete by ID later
                if (string.IsNullOrEmpty(item.Id))
                    item.Id = Guid.NewGuid().ToString();

                var url = $"{BaseUrl}/users/{userId}/shoppingItems?documentId={item.Id}";
                var firestoreDoc = ShoppingItemToDocument(item);
                var content = new StringContent(firestoreDoc, Encoding.UTF8, "application/json");
                var request = CreateAuthorizedRequest(HttpMethod.Post, url);
                request.Content = content;

                await _httpClient.SendAsync(request);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[FirestoreService] AddShoppingItemAsync error: {ex.Message}");
            }
        }

        public async Task UpdateShoppingItemAsync(ShoppingItem item)
        {
            EnsureFirebaseConfigured();
            try
            {
                var userId = _authService.CurrentUserId;
                if (string.IsNullOrEmpty(userId)) return;

                var url = $"{BaseUrl}/users/{userId}/shoppingItems/{item.Id}";
                var firestoreDoc = ShoppingItemToDocument(item);
                var content = new StringContent(firestoreDoc, Encoding.UTF8, "application/json");
                var request = CreateAuthorizedRequest(new HttpMethod("PATCH"), url);
                request.Content = content;

                await _httpClient.SendAsync(request);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[FirestoreService] UpdateShoppingItemAsync error: {ex.Message}");
            }
        }

        public async Task DeleteShoppingItemAsync(string itemId)
        {
            EnsureFirebaseConfigured();
            try
            {
                var userId = _authService.CurrentUserId;
                if (string.IsNullOrEmpty(userId)) return;

                var url = $"{BaseUrl}/users/{userId}/shoppingItems/{itemId}";
                var request = CreateAuthorizedRequest(HttpMethod.Delete, url);
                await _httpClient.SendAsync(request);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[FirestoreService] DeleteShoppingItemAsync error: {ex.Message}");
            }
        }

        public async Task ClearCompletedShoppingItemsAsync(string userId)
        {
            EnsureFirebaseConfigured();
            try
            {
                var items = await GetShoppingItemsAsync(userId);
                var completedItems = items.Where(i => i.IsChecked);

                foreach (var item in completedItems)
                {
                    await DeleteShoppingItemAsync(item.Id);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[FirestoreService] ClearCompletedShoppingItemsAsync error: {ex.Message}");
            }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Creates an HttpRequestMessage with Firebase Auth bearer token.
        /// </summary>
        private HttpRequestMessage CreateAuthorizedRequest(HttpMethod method, string url)
        {
            var request = new HttpRequestMessage(method, url);
            if (!string.IsNullOrEmpty(_authService.CurrentUserToken))
            {
                request.Headers.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _authService.CurrentUserToken);
            }
            return request;
        }

        /// <summary>
        /// Extracts a string field from a Firestore document's fields object.
        /// </summary>
        private static string GetStringField(JToken fields, string fieldName)
        {
            return fields[fieldName]?["stringValue"]?.ToString() ?? string.Empty;
        }

        /// <summary>
        /// Extracts an integer field from a Firestore document's fields object.
        /// </summary>
        private static int GetIntField(JToken fields, string fieldName)
        {
            var val = fields[fieldName]?["integerValue"]?.ToString();
            return int.TryParse(val, out var result) ? result : 0;
        }

        /// <summary>
        /// Extracts a double field from a Firestore document's fields object.
        /// </summary>
        private static double GetDoubleField(JToken fields, string fieldName)
        {
            var val = fields[fieldName]?["doubleValue"]?.ToString();
            return double.TryParse(val, out var result) ? result : 0.0;
        }

        /// <summary>
        /// Extracts a boolean field from a Firestore document's fields object.
        /// </summary>
        private static bool GetBoolField(JToken fields, string fieldName)
        {
            return fields[fieldName]?["booleanValue"]?.Value<bool>() ?? false;
        }

        /// <summary>
        /// Converts Firestore document JSON to Recipe model.
        /// </summary>
        private static Recipe DocumentToRecipe(JToken doc)
        {
            var fields = doc["fields"]!;
            var name = doc["name"]?.ToString() ?? "";
            var docId = name.Contains('/') ? name.Split('/').Last() : name;

            return new Recipe
            {
                Id = docId,
                Title = GetStringField(fields, "title"),
                Description = GetStringField(fields, "description"),
                ImageUrl = GetStringField(fields, "imageUrl"),
                Category = GetStringField(fields, "category"),
                IngredientsJson = GetStringField(fields, "ingredientsJson"),
                InstructionsJson = GetStringField(fields, "instructionsJson"),
                CookTimeMinutes = GetIntField(fields, "cookTimeMinutes"),
                PrepTimeMinutes = GetIntField(fields, "prepTimeMinutes"),
                Servings = GetIntField(fields, "servings"),
                Rating = GetDoubleField(fields, "rating"),
                IsFromApi = GetBoolField(fields, "isFromApi"),
                CreatedBy = GetStringField(fields, "createdBy")
            };
        }

        /// <summary>
        /// Converts Recipe model to Firestore document JSON.
        /// </summary>
        private static string RecipeToDocument(Recipe recipe)
        {
            var doc = new
            {
                fields = new Dictionary<string, object>
                {
                    ["title"] = new { stringValue = recipe.Title },
                    ["description"] = new { stringValue = recipe.Description ?? "" },
                    ["imageUrl"] = new { stringValue = recipe.ImageUrl ?? "" },
                    ["category"] = new { stringValue = recipe.Category ?? "" },
                    ["ingredientsJson"] = new { stringValue = recipe.IngredientsJson ?? "[]" },
                    ["instructionsJson"] = new { stringValue = recipe.InstructionsJson ?? "[]" },
                    ["cookTimeMinutes"] = new { integerValue = recipe.CookTimeMinutes },
                    ["prepTimeMinutes"] = new { integerValue = recipe.PrepTimeMinutes },
                    ["servings"] = new { integerValue = recipe.Servings },
                    ["rating"] = new { doubleValue = recipe.Rating },
                    ["isFromApi"] = new { booleanValue = recipe.IsFromApi },
                    ["createdBy"] = new { stringValue = recipe.CreatedBy ?? "" }
                }
            };

            return JsonConvert.SerializeObject(doc);
        }

        /// <summary>
        /// Converts Firestore document JSON to FavoriteRecipe model.
        /// </summary>
        private static FavoriteRecipe DocumentToFavorite(JToken doc)
        {
            var fields = doc["fields"]!;
            var name = doc["name"]?.ToString() ?? "";
            var docId = name.Contains('/') ? name.Split('/').Last() : name;

            return new FavoriteRecipe
            {
                Id = docId,
                RecipeId = GetStringField(fields, "recipeId"),
                UserId = GetStringField(fields, "userId"),
                SavedAt = DateTime.TryParse(GetStringField(fields, "savedAt"), out var dt)
                    ? dt : DateTime.UtcNow
            };
        }

        /// <summary>
        /// Converts FavoriteRecipe model to Firestore document JSON.
        /// </summary>
        private static string FavoriteToDocument(FavoriteRecipe favorite)
        {
            var doc = new
            {
                fields = new Dictionary<string, object>
                {
                    ["recipeId"] = new { stringValue = favorite.RecipeId },
                    ["userId"] = new { stringValue = favorite.UserId ?? "" },
                    ["savedAt"] = new { stringValue = favorite.SavedAt.ToString("o") }
                }
            };

            return JsonConvert.SerializeObject(doc);
        }

        /// <summary>
        /// Converts Firestore document JSON to ShoppingItem model.
        /// </summary>
        private static ShoppingItem DocumentToShoppingItem(JToken doc)
        {
            var fields = doc["fields"]!;
            var name = doc["name"]?.ToString() ?? "";
            var docId = name.Contains('/') ? name.Split('/').Last() : name;

            return new ShoppingItem
            {
                Id = docId,
                Name = GetStringField(fields, "name"),
                Quantity = GetStringField(fields, "quantity"),
                Unit = GetStringField(fields, "unit"),
                IsChecked = GetBoolField(fields, "isChecked"),
                RecipeId = GetStringField(fields, "recipeId"),
                RecipeTitle = GetStringField(fields, "recipeTitle")
            };
        }

        /// <summary>
        /// Converts ShoppingItem model to Firestore document JSON.
        /// </summary>
        private static string ShoppingItemToDocument(ShoppingItem item)
        {
            var doc = new
            {
                fields = new Dictionary<string, object>
                {
                    ["name"] = new { stringValue = item.Name ?? "" },
                    ["quantity"] = new { stringValue = item.Quantity ?? "" },
                    ["unit"] = new { stringValue = item.Unit ?? "" },
                    ["isChecked"] = new { booleanValue = item.IsChecked },
                    ["recipeId"] = new { stringValue = item.RecipeId ?? "" },
                    ["recipeTitle"] = new { stringValue = item.RecipeTitle ?? "" }
                }
            };

            return JsonConvert.SerializeObject(doc);
        }

        private static void EnsureFirebaseConfigured()
        {
            if (Constants.HasFirebaseConfig)
                return;

            throw new InvalidOperationException(
                "Firebase is not configured. Set FIREBASE_API_KEY and FIREBASE_PROJECT_ID environment variables or update Helpers/Constants.cs.");
        }

        #endregion
    }
}
