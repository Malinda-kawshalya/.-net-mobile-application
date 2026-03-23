namespace Recipes_app.Interfaces
{
    /// <summary>
    /// Contract for navigation abstraction using Shell.
    /// </summary>
    public interface INavigationService
    {
        Task GoToAsync(string route);
        Task GoToAsync(string route, IDictionary<string, object> parameters);
        Task GoBackAsync();
    }
}
