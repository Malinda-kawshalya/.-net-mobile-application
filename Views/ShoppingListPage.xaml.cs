using Recipes_app.ViewModels;

namespace Recipes_app.Views;

public partial class ShoppingListPage : ContentPage
{
    private readonly ShoppingListViewModel _viewModel;

    public ShoppingListPage(ShoppingListViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.LoadItemsCommand.Execute(null);
    }
}
