using System;

namespace OnlineShop_MobileApp.GUI_elements
{
    public class Message : Banner
    {
        private string _description = string.Empty;
        public string Description
        {
            get => _description;
            set
            {
                if (_description == value) return;
                _description = value;
                OnPropertyChanged();
            }
        }

        public Message() : base() { }

        public Message(string description) : base()
        {
            Reset(description);
        }

        public void Reset(string description)
        {
            Description = description;
            PrepareToShow();
        }
    }
}
