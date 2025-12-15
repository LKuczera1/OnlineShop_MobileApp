using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

public class CartViewModel
{
    public ObservableCollection<CartItem> Items { get; } = new();

    public ICommand IncreaseCommand { get; }
    public ICommand DecreaseCommand { get; }
    public ICommand RemoveCommand { get; }

    public CartViewModel()
    {
        IncreaseCommand = new Command<CartItem>(item =>
        {
            if (item == null) return;
            item.Quantity++;
        });

        DecreaseCommand = new Command<CartItem>(item =>
        {
            if (item == null) return;
            if (item.Quantity > 1) item.Quantity--;
        });

        RemoveCommand = new Command<CartItem>(item =>
        {
            if (item == null) return;
            Items.Remove(item);
        });

        // MOCK
        Items.Add(new CartItem { Title = "Produkt 1", Subtitle = "Opis", Quantity = 2 });
        Items.Add(new CartItem { Title = "Produkt 2", Subtitle = "Opis", Quantity = 1 });
        Items.Add(new CartItem { Title = "Produkt 2", Subtitle = "Opis", Quantity = 1 });
        Items.Add(new CartItem { Title = "Produkt 2", Subtitle = "Opis", Quantity = 1 });
        Items.Add(new CartItem { Title = "Produkt 2", Subtitle = "Opis", Quantity = 1 });
        Items.Add(new CartItem { Title = "Produkt 2", Subtitle = "Opis", Quantity = 1 });
        Items.Add(new CartItem { Title = "Produkt 2", Subtitle = "Opis", Quantity = 1 });
        Items.Add(new CartItem { Title = "Produkt 2", Subtitle = "Opis", Quantity = 1 });
        Items.Add(new CartItem { Title = "Produkt 2", Subtitle = "Opis", Quantity = 1 });
        Items.Add(new CartItem { Title = "Produkt 2", Subtitle = "Opis", Quantity = 1 });
        Items.Add(new CartItem { Title = "Produkt 2", Subtitle = "Opis", Quantity = 1 });
        Items.Add(new CartItem { Title = "Produkt 2", Subtitle = "Opis", Quantity = 1 });
        Items.Add(new CartItem { Title = "Produkt 2", Subtitle = "Opis", Quantity = 1 });
    }
}

public class CartItem : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public string? Title { get; set; }
    public string? Subtitle { get; set; }

    private int _quantity = 1;
    public int Quantity
    {
        get => _quantity;
        set
        {
            if (_quantity == value) return;
            _quantity = value;
            OnPropertyChanged();
        }
    }

    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
