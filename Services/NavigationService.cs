using Recipes_app.Interfaces;

namespace Recipes_app.Services
{
    /// <summary>
    /// Shell-based navigation service implementation.
    /// Wraps Shell.Current.GoToAsync for testability and decoupling.
    /// </summary>
    public class NavigationService : INavigationService
    {
        public async Task GoToAsync(string route)
        {
            await Shell.Current.GoToAsync(route);
        }

        public async Task GoToAsync(string route, IDictionary<string, object> parameters)
        {
            await Shell.Current.GoToAsync(route, parameters);
        }

        public async Task GoBackAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
