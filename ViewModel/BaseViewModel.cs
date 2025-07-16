using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;

namespace FlowerGame.ViewModel
{
    /// <summary>
    /// Base class for all ViewModels, providing common properties and helpers.
    /// </summary>
    public partial class BaseViewModel : ObservableObject, IDisposable
    {
        private bool disposed;

        public BaseViewModel() { }

        [ObservableProperty]
        string title = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotBusy))]
        bool isBusy;

        [ObservableProperty]
        string message = string.Empty;

        [ObservableProperty]
        bool isInitialized;

        [ObservableProperty]
        string error = string.Empty;

        /// <summary>
        /// True if not busy (for UI binding).
        /// </summary>
        public bool IsNotBusy => !IsBusy;

        /// <summary>
        /// Helper for custom property setters in derived classes.
        /// </summary>
        protected bool SetProperty<T>(ref T backingStore, T value, string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;
            backingStore = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// Helper to raise property changed notifications.
        /// </summary>
        protected void RaisePropertyChanged(string propertyName) => OnPropertyChanged(propertyName);

        /// <summary>
        /// Dispose pattern for cleanup in derived classes.
        /// </summary>
        public virtual void Dispose()
        {
            if (disposed) return;
            disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
