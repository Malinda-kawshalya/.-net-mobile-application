using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Recipes_app.Interfaces;
using Recipes_app.Models;
using Recipes_app.Views;

namespace Recipes_app.ViewModels
{
    /// <summary>
    /// ViewModel for HomePage - dashboard with featured recipes, categories, search.
    /// </summary>
    public partial class HomeViewModel : BaseViewModel
    {
        private readonly IRecipeApiService _recipeApiService;
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private ObservableCollection<Recipe> _featuredRecipes = new();

        [ObservableProperty]
        private ObservableCollection<Category> _categories = new();

        [ObservableProperty]
        private ObservableCollection<Recipe> _recentRecipes = new();

        [ObservableProperty]
        private string _searchText = string.Empty;

        public HomeViewModel(IRecipeApiService recipeApiService, INavigationService navigationService)
        {
            _recipeApiService = recipeApiService;
            _navigationService = navigationService;
            Title = "FlavorVault";
        }

        [RelayCommand]
        private async Task LoadDataAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                // Load categories
                var categories = await _recipeApiService.GetCategoriesAsync();
                Categories.Clear();
                foreach (var category in categories.Take(8))
                    Categories.Add(category);

                // Load featured recipes (random)
                FeaturedRecipes.Clear();
                for (int i = 0; i < 5; i++)
                {
                    var recipe = await _recipeApiService.GetRandomRecipeAsync();
                    if (recipe != null)
                        FeaturedRecipes.Add(recipe);
                }

                // Load recent/popular recipes
                var recentResults = await _recipeApiService.SearchRecipesAsync("chicken");
                RecentRecipes.Clear();
                foreach (var recipe in recentResults.Take(6))
                    RecentRecipes.Add(recipe);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to load data: {ex.Message}", "OK");
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
            if (string.IsNullOrWhiteSpace(SearchText)) return;

            await _navigationService.GoToAsync(nameof(Views.RecipeListPage), new Dictionary<string, object>
            {
                { "SearchQuery", SearchText }
            });
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

        [RelayCommand]
        private async Task GoToCategoryAsync(Category category)
        {
            if (category == null) return;

            await _navigationService.GoToAsync(nameof(Views.RecipeListPage), new Dictionary<string, object>
            {
                { "Category", category.Name }
            });
        }

        [RelayCommand]
        private async Task GoToAllCategoriesAsync()
        {
            await _navigationService.GoToAsync(nameof(Views.CategoriesPage));
        }
    }
}
