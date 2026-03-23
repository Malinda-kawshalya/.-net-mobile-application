using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Recipes_app.Interfaces;
using Recipes_app.Models;
using Recipes_app.Views;

namespace Recipes_app.ViewModels
{
    /// <summary>
    /// ViewModel for RecipeListPage - filterable/searchable list of recipes.
    /// </summary>
    [QueryProperty(nameof(SearchQuery), "SearchQuery")]
    [QueryProperty(nameof(CategoryFilter), "Category")]
    public partial class RecipeListViewModel : BaseViewModel
    {
        private readonly IRecipeApiService _recipeApiService;
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private ObservableCollection<Recipe> _recipes = new();

        [ObservableProperty]
        private string _searchQuery = string.Empty;

        [ObservableProperty]
        private string _categoryFilter = string.Empty;

        [ObservableProperty]
        private bool _isEmptyState;

        public RecipeListViewModel(IRecipeApiService recipeApiService, INavigationService navigationService)
        {
            _recipeApiService = recipeApiService;
            _navigationService = navigationService;
            Title = "Recipes";
        }

        partial void OnSearchQueryChanged(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                Title = $"Search: {value}";
                _ = PerformSearchAsync(value);
            }
        }

        partial void OnCategoryFilterChanged(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                Title = value;
                _ = LoadByCategoryAsync(value);
            }
        }

        [RelayCommand]
        private async Task LoadRecipesAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                if (!string.IsNullOrWhiteSpace(CategoryFilter))
                {
                    await LoadByCategoryAsync(CategoryFilter);
                }
                else if (!string.IsNullOrWhiteSpace(SearchQuery))
                {
                    await PerformSearchAsync(SearchQuery);
                }
                else
                {
                    // Default: load popular recipes
                    await PerformSearchAsync("chicken");
                }
            }
            finally
            {
                IsBusy = false;
                IsRefreshing = false;
            }
        }

        [RelayCommand]
        private async Task SearchAsync()
        {
            if (string.IsNullOrWhiteSpace(SearchQuery)) return;
            CategoryFilter = string.Empty;
            await PerformSearchAsync(SearchQuery);
        }

        private async Task PerformSearchAsync(string query)
        {
            try
            {
                IsBusy = true;
                var results = await _recipeApiService.SearchRecipesAsync(query);
                Recipes.Clear();
                foreach (var recipe in results)
                    Recipes.Add(recipe);
                IsEmptyState = Recipes.Count == 0;
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Search failed: {ex.Message}", "OK");
                IsEmptyState = true;
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task LoadByCategoryAsync(string category)
        {
            try
            {
                IsBusy = true;
                var results = await _recipeApiService.GetRecipesByCategoryAsync(category);
                Recipes.Clear();
                foreach (var recipe in results)
                    Recipes.Add(recipe);
                IsEmptyState = Recipes.Count == 0;
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to load recipes: {ex.Message}", "OK");
                IsEmptyState = true;
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task GoToRecipeDetailAsync(Recipe recipe)
        {
            if (recipe == null) return;

            // If recipe came from category filter, it may only have basic info
            // Fetch full details if needed
            var fullRecipe = recipe;
            if (recipe.Ingredients == null || recipe.Ingredients.Count == 0)
            {
                fullRecipe = await _recipeApiService.GetRecipeByIdAsync(recipe.Id) ?? recipe;
            }

            await _navigationService.GoToAsync(nameof(Views.RecipeDetailPage), new Dictionary<string, object>
            {
                { "Recipe", fullRecipe }
            });
        }
    }
}
