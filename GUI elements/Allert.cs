using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShop_MobileApp.GUI_elements
{
    public class Allert :INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public double hiddenPosition { get; } = -200;
        public double onViewPosition { get; } = 10;
        public double animationLength { get; } = 500;
        public TimeSpan onScreenTime { get; set; } = TimeSpan.FromSeconds(2);

        private bool _isAllertVisible;
        public bool IsAllertVisible
        {
            get { return _isAllertVisible; }
            set {
                _isAllertVisible = value;
                OnPropertyChanged();
            }
        }

        private string _title = string.Empty;
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        private string _description = string.Empty;
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged();
            }
        }

        public bool _animating = false;

        public Allert()
        {
            IsAllertVisible = false;
            _animating = false;
        }

        public Allert(string title, string description)
        {
            IsAllertVisible = true;
            Title = title;
            Description = description;
            _animating = false;
        }

        public void ResetAllert(string title, string description)
        {
            IsAllertVisible = true;
            Title = title;
            Description = description;
            _animating = false;
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
