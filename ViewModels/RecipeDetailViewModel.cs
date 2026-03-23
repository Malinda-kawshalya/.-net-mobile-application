using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Recipes_app.Interfaces;
using Recipes_app.Models;

namespace Recipes_app.ViewModels
{
    /// <summary>
    /// ViewModel for RecipeDetailPage - full recipe view with ingredients, instructions.
    /// Uses Firebase Firestore exclusively via IDatabaseService.
    /// </summary>
    [QueryProperty(nameof(Recipe), "Recipe")]
    public partial class RecipeDetailViewModel : BaseViewModel
    {
        private readonly IDatabaseService _cloudDb;
        private readonly INavigationService _navigationService;
        private readonly IAuthService _authService;

        [ObservableProperty]
        private Recipe _recipe = new();

        [ObservableProperty]
        private ObservableCollection<Ingredient> _ingredients = new();

        [ObservableProperty]
        private ObservableCollection<string> _instructions = new();

        [ObservableProperty]
        private bool _isFavorite;

        [ObservableProperty]
        private int _servingsAdjusted;

        public RecipeDetailViewModel(
            IDatabaseService cloudDb,
            INavigationService navigationService,
            IAuthService authService)
        {
            _cloudDb = cloudDb;
            _navigationService = navigationService;
            _authService = authService;
        }

        partial void OnRecipeChanged(Recipe value)
        {
            if (value == null) return;

            Title = value.Title;
            ServingsAdjusted = value.Servings > 0 ? value.Servings : 4;

            // Populate ingredients and instructions
            Ingredients.Clear();
            if (value.Ingredients != null)
            {
                foreach (var i in value.Ingredients)
                    Ingredients.Add(i);
            }

            Instructions.Clear();
            if (value.Instructions != null)
            {
                foreach (var s in value.Instructions)
                    Instructions.Add(s);
            }

            // Check if it's a favorite
            _ = CheckFavoriteStatusAsync();
        }

        private async Task CheckFavoriteStatusAsync()
        {
            try
            {
                var userId = _authService.CurrentUserId ?? string.Empty;
                IsFavorite = await _cloudDb.IsFavoriteAsync(userId, Recipe.Id);
            }
            catch
            {
                IsFavorite = false;
            }
        }

        [RelayCommand]
        private async Task ToggleFavoriteAsync()
        {
            try
            {
                var userId = _authService.CurrentUserId ?? string.Empty;

                if (IsFavorite)
                {
                    // Remove from favorites (document ID = recipeId by convention)
                    await _cloudDb.RemoveFavoriteAsync(Recipe.Id);
                    IsFavorite = false;
                }
                else
                {
                    // Add to favorites
                    var favorite = new FavoriteRecipe
                    {
                        RecipeId = Recipe.Id,
                        UserId = userId,
                        SavedAt = DateTime.UtcNow
                    };
                    await _cloudDb.AddFavoriteAsync(favorite);

                    // Also save the recipe to Firestore so it can be fetched later
                    await _cloudDb.SaveRecipeAsync(Recipe);
                    IsFavorite = true;
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to update favorite: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task AddToShoppingListAsync()
        {
            try
            {
                foreach (var ingredient in Ingredients)
                {
                    var item = new ShoppingItem
                    {
                        Name = ingredient.Name,
                        Quantity = ingredient.Quantity,
                        Unit = ingredient.Unit,
                        RecipeId = Recipe.Id,
                        RecipeTitle = Recipe.Title,
                        IsChecked = false
                    };
                    await _cloudDb.AddShoppingItemAsync(item);
                }

                await Shell.Current.DisplayAlert("Added", $"{Ingredients.Count} ingredients added to shopping list!", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to add items: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task GoBackAsync()
        {
            await _navigationService.GoBackAsync();
        }

        [RelayCommand]
        private void IncrementServings()
        {
            ServingsAdjusted++;
        }

        [RelayCommand]
        private void DecrementServings()
        {
            if (ServingsAdjusted > 1)
                ServingsAdjusted--;
        }
    }
}
