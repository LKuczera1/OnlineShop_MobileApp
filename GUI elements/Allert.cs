namespace OnlineShop_MobileApp.GUI_elements
{
    public class Allert : Banner
    {
        private string _title = string.Empty;
        public string Title
        {
            get => _title;
            set
            {
                if (_title == value) return;
                _title = value;
                OnPropertyChanged();
            }
        }

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

        public Allert() : base() { }

        public Allert(string title, string description) : base()
        {
            Reset(title, description);
        }

        public void Reset(string title, string description)
        {
            Title = title;
            Description = description;
            PrepareToShow();
        }
    }
}
