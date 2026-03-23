using Recipes_app.ViewModels;

namespace Recipes_app.Views;

public partial class CategoriesPage : ContentPage
{
    private readonly CategoriesViewModel _viewModel;

    public CategoriesPage(CategoriesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (_viewModel.Categories.Count == 0)
            _viewModel.LoadCategoriesCommand.Execute(null);
    }
}
