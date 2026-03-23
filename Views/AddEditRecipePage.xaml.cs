using Recipes_app.ViewModels;

namespace Recipes_app.Views;

public partial class AddEditRecipePage : ContentPage
{
    public AddEditRecipePage(AddEditRecipeViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
