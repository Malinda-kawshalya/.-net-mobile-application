using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Recipes_app.Interfaces;
using Recipes_app.Models;
using Recipes_app.Views;

namespace Recipes_app.ViewModels
{
    /// <summary>
    /// ViewModel for FavoritesPage - manage saved/favorite recipes.
    /// Uses Firebase Firestore exclusively via IDatabaseService.
    /// </summary>
    public partial class FavoritesViewModel : BaseViewModel
    {
        private readonly IDatabaseService _cloudDb;
        private readonly IAuthService _authService;
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private ObservableCollection<Recipe> _favoriteRecipes = new();

        [ObservableProperty]
        private bool _isEmptyState;

        public FavoritesViewModel(IDatabaseService cloudDb, IAuthService authService, INavigationService navigationService)
        {
            _cloudDb = cloudDb;
            _authService = authService;
            _navigationService = navigationService;
            Title = "Favorites";
        }

        [RelayCommand]
        private async Task LoadFavoritesAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                var userId = _authService.CurrentUserId;
                if (string.IsNullOrEmpty(userId))
                {
                    IsEmptyState = true;
                    return;
                }

                var favorites = await _cloudDb.GetFavoritesAsync(userId);
                FavoriteRecipes.Clear();

                foreach (var fav in favorites)
                {
                    var recipe = await _cloudDb.GetRecipeAsync(fav.RecipeId);
                    if (recipe != null)
                        FavoriteRecipes.Add(recipe);
                }

                IsEmptyState = FavoriteRecipes.Count == 0;
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to load favorites: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
                IsRefreshing = false;
            }
        }

        [RelayCommand]
        private async Task RemoveFavoriteAsync(Recipe recipe)
        {
            if (recipe == null) return;

            try
            {
                // The favorite document ID is the recipeId (set in RecipeDetailViewModel)
                await _cloudDb.RemoveFavoriteAsync(recipe.Id);
                FavoriteRecipes.Remove(recipe);
                IsEmptyState = FavoriteRecipes.Count == 0;
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to remove favorite: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task GoToRecipeDetailAsync(Recipe recipe)
        {
            if (recipe == null) return;

            await _navigationService.GoToAsync(nameof(Views.RecipeDetailPage), new Dictionary<string, object>
            {
                { "Recipe", recipe }
            });
        }
    }
}
