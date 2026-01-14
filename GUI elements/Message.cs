using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShop_MobileApp.GUI_elements
{
    public class Message: Banner, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public double hiddenPosition { get; } = -200;
        public double onViewPosition { get; } = 10;
        public double animationLength { get; } = 500;
        public TimeSpan onScreenTime { get; set; } = TimeSpan.FromSeconds(2);

        private bool _isMessageVisible;
        public bool IsMessageVisible
        {
            get { return _isMessageVisible; }
            set
            {
                _isMessageVisible = value;
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

        public Message()
        {
            IsMessageVisible = false;
            _animating = false;
        }

        public Message(string description)
        {
            IsMessageVisible = true;
            Description = description;
            _animating = false;
        }

        public void ResetAllert(string description)
        {
            IsMessageVisible = true;
            Description = description;
            _animating = false;
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
