using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Recipes_app.Interfaces;
using Recipes_app.Models;
using Newtonsoft.Json;

namespace Recipes_app.ViewModels
{
    /// <summary>
    /// ViewModel for AddEditRecipePage - create or edit a user recipe.
    /// </summary>
    [QueryProperty(nameof(EditRecipe), "Recipe")]
    public partial class AddEditRecipeViewModel : BaseViewModel
    {
        private readonly IDatabaseService _cloudDb;
        private readonly IAuthService _authService;
        private readonly INavigationService _navigationService;

        private bool _isEditing;

        [ObservableProperty]
        private Recipe? _editRecipe;

        [ObservableProperty]
        private string _recipeName = string.Empty;

        [ObservableProperty]
        private string _recipeDescription = string.Empty;

        [ObservableProperty]
        private string _recipeImageUrl = string.Empty;

        [ObservableProperty]
        private string _selectedCategory = "Main Course";

        [ObservableProperty]
        private int _cookTime = 30;

        [ObservableProperty]
        private int _prepTime = 15;

        [ObservableProperty]
        private int _servings = 4;

        [ObservableProperty]
        private ObservableCollection<Ingredient> _ingredients = new();

        [ObservableProperty]
        private ObservableCollection<string> _instructionSteps = new();

        [ObservableProperty]
        private string _newIngredientName = string.Empty;

        [ObservableProperty]
        private string _newIngredientQuantity = string.Empty;

        [ObservableProperty]
        private string _newIngredientUnit = string.Empty;

        [ObservableProperty]
        private string _newInstructionStep = string.Empty;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [ObservableProperty]
        private bool _hasError;

        public List<string> CategoryOptions { get; } = new()
        {
            "Breakfast", "Lunch", "Dinner", "Dessert", "Snack",
            "Appetizer", "Main Course", "Side Dish", "Salad",
            "Soup", "Beverage", "Vegan", "Vegetarian"
        };

        public AddEditRecipeViewModel(
            IDatabaseService cloudDb,
            IAuthService authService,
            INavigationService navigationService)
        {
            _cloudDb = cloudDb;
            _authService = authService;
            _navigationService = navigationService;
            Title = "New Recipe";
        }

        partial void OnEditRecipeChanged(Recipe? value)
        {
            if (value != null)
            {
                _isEditing = true;
                Title = "Edit Recipe";
                RecipeName = value.Title;
                RecipeDescription = value.Description;
                RecipeImageUrl = value.ImageUrl;
                SelectedCategory = value.Category;
                CookTime = value.CookTimeMinutes;
                PrepTime = value.PrepTimeMinutes;
                Servings = value.Servings;

                Ingredients.Clear();
                if (value.Ingredients != null)
                    foreach (var i in value.Ingredients)
                        Ingredients.Add(i);

                InstructionSteps.Clear();
                if (value.Instructions != null)
                    foreach (var s in value.Instructions)
                        InstructionSteps.Add(s);
            }
        }

        [RelayCommand]
        private void AddIngredient()
        {
            if (string.IsNullOrWhiteSpace(NewIngredientName)) return;

            Ingredients.Add(new Ingredient
            {
                Name = NewIngredientName.Trim(),
                Quantity = NewIngredientQuantity.Trim(),
                Unit = NewIngredientUnit.Trim()
            });

            NewIngredientName = string.Empty;
            NewIngredientQuantity = string.Empty;
            NewIngredientUnit = string.Empty;
        }

        [RelayCommand]
        private void RemoveIngredient(Ingredient ingredient)
        {
            if (ingredient != null)
                Ingredients.Remove(ingredient);
        }

        [RelayCommand]
        private void AddInstruction()
        {
            if (string.IsNullOrWhiteSpace(NewInstructionStep)) return;
            InstructionSteps.Add(NewInstructionStep.Trim());
            NewInstructionStep = string.Empty;
        }

        [RelayCommand]
        private void RemoveInstruction(string step)
        {
            if (step != null)
                InstructionSteps.Remove(step);
        }

        [RelayCommand]
        private async Task SaveRecipeAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                HasError = false;

                // Validation
                if (string.IsNullOrWhiteSpace(RecipeName))
                {
                    ErrorMessage = "Please enter a recipe name.";
                    HasError = true;
                    return;
                }

                if (Ingredients.Count == 0)
                {
                    ErrorMessage = "Please add at least one ingredient.";
                    HasError = true;
                    return;
                }

                if (InstructionSteps.Count == 0)
                {
                    ErrorMessage = "Please add at least one instruction step.";
                    HasError = true;
                    return;
                }

                var recipe = _isEditing && EditRecipe != null ? EditRecipe : new Recipe();

                recipe.Title = RecipeName;
                recipe.Description = RecipeDescription;
                recipe.ImageUrl = RecipeImageUrl;
                recipe.Category = SelectedCategory;
                recipe.CookTimeMinutes = CookTime;
                recipe.PrepTimeMinutes = PrepTime;
                recipe.Servings = Servings;
                recipe.Ingredients = Ingredients.ToList();
                recipe.Instructions = InstructionSteps.ToList();
                recipe.IngredientsJson = JsonConvert.SerializeObject(recipe.Ingredients);
                recipe.InstructionsJson = JsonConvert.SerializeObject(recipe.Instructions);
                recipe.CreatedBy = _authService.CurrentUserId ?? "local";
                recipe.IsFromApi = false;

                if (!_isEditing)
                {
                    recipe.CreatedAt = DateTime.UtcNow;
                }

                // Save to Firebase Firestore
                if (_authService.IsSignedIn)
                {
                    if (_isEditing)
                        await _cloudDb.UpdateRecipeAsync(recipe);
                    else
                        await _cloudDb.SaveRecipeAsync(recipe);
                }
                else
                {
                    await Shell.Current.DisplayAlert("Not Signed In", "Please sign in to save recipes.", "OK");
                    return;
                }

                await Shell.Current.DisplayAlert("Success",
                    _isEditing ? "Recipe updated!" : "Recipe created!",
                    "OK");

                await _navigationService.GoBackAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to save recipe: {ex.Message}";
                HasError = true;
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task CancelAsync()
        {
            await _navigationService.GoBackAsync();
        }
    }
}
