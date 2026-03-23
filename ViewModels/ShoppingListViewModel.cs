using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Recipes_app.Interfaces;
using Recipes_app.Models;

namespace Recipes_app.ViewModels
{
    /// <summary>
    /// ViewModel for ShoppingListPage - manage ingredients shopping list.
    /// Uses Firebase Firestore exclusively via IDatabaseService.
    /// </summary>
    public partial class ShoppingListViewModel : BaseViewModel
    {
        private readonly IDatabaseService _cloudDb;
        private readonly IAuthService _authService;

        [ObservableProperty]
        private ObservableCollection<ShoppingItem> _shoppingItems = new();

        [ObservableProperty]
        private bool _isEmptyState;

        [ObservableProperty]
        private int _checkedCount;

        [ObservableProperty]
        private int _totalCount;

        public ShoppingListViewModel(IDatabaseService cloudDb, IAuthService authService)
        {
            _cloudDb = cloudDb;
            _authService = authService;
            Title = "Shopping List";
        }

        [RelayCommand]
        private async Task LoadItemsAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                var userId = _authService.CurrentUserId;
                if (string.IsNullOrEmpty(userId))
                {
                    IsEmptyState = true;
                    return;
                }

                var items = await _cloudDb.GetShoppingItemsAsync(userId);
                ShoppingItems.Clear();
                foreach (var item in items)
                    ShoppingItems.Add(item);

                UpdateCounts();
                IsEmptyState = ShoppingItems.Count == 0;
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to load shopping list: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
                IsRefreshing = false;
            }
        }

        [RelayCommand]
        private async Task ToggleItemAsync(ShoppingItem item)
        {
            if (item == null) return;

            try
            {
                item.IsChecked = !item.IsChecked;
                await _cloudDb.UpdateShoppingItemAsync(item);
                UpdateCounts();

                // Refresh the collection to update UI
                var index = ShoppingItems.IndexOf(item);
                if (index >= 0)
                {
                    ShoppingItems.RemoveAt(index);
                    ShoppingItems.Insert(index, item);
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to update item: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task DeleteItemAsync(ShoppingItem item)
        {
            if (item == null) return;

            try
            {
                await _cloudDb.DeleteShoppingItemAsync(item.Id);
                ShoppingItems.Remove(item);
                UpdateCounts();
                IsEmptyState = ShoppingItems.Count == 0;
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to delete item: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task ClearCompletedAsync()
        {
            try
            {
                var userId = _authService.CurrentUserId ?? string.Empty;
                await _cloudDb.ClearCompletedShoppingItemsAsync(userId);
                var toRemove = ShoppingItems.Where(i => i.IsChecked).ToList();
                foreach (var item in toRemove)
                    ShoppingItems.Remove(item);
                UpdateCounts();
                IsEmptyState = ShoppingItems.Count == 0;
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to clear items: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task ClearAllAsync()
        {
            bool confirm = await Shell.Current.DisplayAlert(
                "Clear All",
                "Are you sure you want to clear the entire shopping list?",
                "Yes", "No");

            if (!confirm) return;

            try
            {
                foreach (var item in ShoppingItems.ToList())
                    await _cloudDb.DeleteShoppingItemAsync(item.Id);

                ShoppingItems.Clear();
                UpdateCounts();
                IsEmptyState = true;
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to clear list: {ex.Message}", "OK");
            }
        }

        private void UpdateCounts()
        {
            TotalCount = ShoppingItems.Count;
            CheckedCount = ShoppingItems.Count(i => i.IsChecked);
        }
    }
}
