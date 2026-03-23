using Recipes_app.ViewModels;

namespace Recipes_app.Views;

public partial class HomePage : ContentPage
{
    private readonly HomeViewModel _viewModel;

    public HomePage(HomeViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (_viewModel.FeaturedRecipes.Count == 0)
            _viewModel.LoadDataCommand.Execute(null);
    }
}
