using Recipes_app.ViewModels;

namespace Recipes_app.Views;

public partial class RecipeListPage : ContentPage
{
    private readonly RecipeListViewModel _viewModel;

    public RecipeListPage(RecipeListViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.LoadRecipesCommand.Execute(null);
    }
}
