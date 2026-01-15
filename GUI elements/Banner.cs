using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace OnlineShop_MobileApp.GUI_elements
{
    public abstract class Banner : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        // --- Animacja / layout (wspólne dla wszystkich bannerów) ---
        public double HiddenPosition { get; protected set; } = -200;
        public double OnViewPosition { get; protected set; } = 10;
        public double AnimationLength { get; protected set; } = 500;
        public TimeSpan OnScreenTime { get; set; } = TimeSpan.FromSeconds(2);

        // --- Stan (wspólny) ---
        private bool _isVisible;
        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (_isVisible == value) return;
                _isVisible = value;
                OnPropertyChanged();
            }
        }

        private bool _isAnimating;
        public bool IsAnimating
        {
            get => _isAnimating;
            set
            {
                if (_isAnimating == value) return;
                _isAnimating = value;
                OnPropertyChanged();
            }
        }

        protected Banner()
        {
            IsVisible = false;
            IsAnimating = false;
        }

        /// <summary>
        /// Ustawia banner w stan "do pokazania" i resetuje flagi animacji.
        /// </summary>
        protected void PrepareToShow()
        {
            IsVisible = true;
            IsAnimating = false;
        }

        /// <summary>
        /// Ukrywa banner i resetuje flagi animacji.
        /// </summary>
        protected void PrepareToHide()
        {
            IsVisible = false;
            IsAnimating = false;
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
