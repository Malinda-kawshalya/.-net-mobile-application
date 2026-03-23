using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Recipes_app.Helpers;
using Recipes_app.Interfaces;
using Recipes_app.Services;
using Recipes_app.ViewModels;
using Recipes_app.Views;

namespace Recipes_app
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // ── Services (Singleton) ──
            builder.Services.AddSingleton<HttpClient>();
            builder.Services.AddSingleton<IAuthService, AuthService>();
            builder.Services.AddSingleton<INavigationService, NavigationService>();
            builder.Services.AddSingleton<IRecipeApiService, RecipeApiService>();
            if (Constants.HasFirebaseConfig)
            {
                builder.Services.AddSingleton<IDatabaseService, FirestoreService>();
            }
            else
            {
                builder.Services.AddSingleton<IDatabaseService, LocalDatabaseService>();
            }

            // ── ViewModels (Transient) ──
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<RegisterViewModel>();
            builder.Services.AddTransient<HomeViewModel>();
            builder.Services.AddTransient<CategoriesViewModel>();
            builder.Services.AddTransient<RecipeListViewModel>();
            builder.Services.AddTransient<RecipeDetailViewModel>();
            builder.Services.AddTransient<AddEditRecipeViewModel>();
            builder.Services.AddTransient<FavoritesViewModel>();
            builder.Services.AddTransient<ShoppingListViewModel>();
            builder.Services.AddTransient<ProfileViewModel>();

            // ── Pages (Transient) ──
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<RegisterPage>();
            builder.Services.AddTransient<HomePage>();
            builder.Services.AddTransient<CategoriesPage>();
            builder.Services.AddTransient<RecipeListPage>();
            builder.Services.AddTransient<RecipeDetailPage>();
            builder.Services.AddTransient<AddEditRecipePage>();
            builder.Services.AddTransient<FavoritesPage>();
            builder.Services.AddTransient<ShoppingListPage>();
            builder.Services.AddTransient<ProfilePage>();

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
