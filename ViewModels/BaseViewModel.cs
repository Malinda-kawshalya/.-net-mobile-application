using CommunityToolkit.Mvvm.ComponentModel;

namespace Recipes_app.ViewModels
{
    /// <summary>
    /// Base ViewModel providing common properties for all ViewModels.
    /// Uses CommunityToolkit.Mvvm ObservableObject for DRY principle adherence.
    /// </summary>
    public partial class BaseViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private string _title = string.Empty;

        [ObservableProperty]
        private bool _isRefreshing;

        public bool IsNotBusy => !IsBusy;

        /// <summary>
        /// Called when IsBusy changes - also notifies IsNotBusy.
        /// </summary>
        partial void OnIsBusyChanged(bool value)
        {
            OnPropertyChanged(nameof(IsNotBusy));
        }
    }
}
