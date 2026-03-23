using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Recipes_app.Interfaces;
using Recipes_app.Models;
using Recipes_app.Views;

namespace Recipes_app.ViewModels
{
    /// <summary>
    /// ViewModel for CategoriesPage - browse all recipe categories.
    /// </summary>
    public partial class CategoriesViewModel : BaseViewModel
    {
        private readonly IRecipeApiService _recipeApiService;
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private ObservableCollection<Category> _categories = new();

        public CategoriesViewModel(IRecipeApiService recipeApiService, INavigationService navigationService)
        {
            _recipeApiService = recipeApiService;
            _navigationService = navigationService;
            Title = "Categories";
        }

        [RelayCommand]
        private async Task LoadCategoriesAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                var categories = await _recipeApiService.GetCategoriesAsync();
                Categories.Clear();
                foreach (var category in categories)
                    Categories.Add(category);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to load categories: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
                IsRefreshing = false;
            }
        }

        [RelayCommand]
        private async Task SelectCategoryAsync(Category category)
        {
            if (category == null) return;

            await _navigationService.GoToAsync(nameof(Views.RecipeListPage), new Dictionary<string, object>
            {
                { "Category", category.Name }
            });
        }
    }
}
