using Microsoft.Maui.Graphics;
using OnlineShop_MobileApp.Models;
using OnlineShop_MobileApp.Models.DTOs;
using OnlineShop_MobileApp.Services;
using OnlineShop_MobileApp.Services.Resolver;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

public class CartViewModel: INotifyPropertyChanged
{
    //--------------------

    private readonly IShoppingService _service;
    private readonly IServicesResolver _servicesResolver;

    public event PropertyChangedEventHandler? PropertyChanged;

    //-------------------
    public ObservableCollection<CartItem> Items { get; } = new();

    public ICommand IncreaseCommand { get; }
    public ICommand DecreaseCommand { get; }
    public ICommand RemoveCommand { get; }

    private bool _isCartViewAvailable = false;

    public bool IsCartViewAvailable
    {
        get { return _isCartViewAvailable; }
        set
        {
            _isCartViewAvailable = value;
            OnPropertyChanged();
        }
    }


    public CartViewModel(IShoppingService service, IServicesResolver servicesResolver)
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

        _service = service;
        _servicesResolver = servicesResolver;
    }

    public async Task GetCartItems()
    {
        string nophotothumbnailpath = "D:\\Programming\\Projects\\Visual Studio\\OnlineShop_MobileApp\\Resources\\Images\\nophotothumbnail.png";


        var items = await _service.GetCartItems();

        Items.Clear();
        foreach (var item in items)
        {
            CartItem newCartItem = new CartItem()
            {
                dto = item,
            };

            newCartItem.product = await _servicesResolver.ResolveForProduct(item.ProductId);

            if (newCartItem.product == null) continue;

            try
            {
                MemoryStream temp = new MemoryStream(await _servicesResolver.ResolveForProductThumbnail(item.ProductId));
                newCartItem.product.ThumbnailSource =
                    ImageSource.FromStream(() => temp);
            }
            catch
            {
                newCartItem.product.ThumbnailSource = ImageSource.FromFile(nophotothumbnailpath);
            }

            Items.Add(newCartItem);
        }
    }

    public async Task Refresh()
    {
        IsCartViewAvailable = _service.isUserLoggedIn();

        if(IsCartViewAvailable)
        {
            try
            {
                await GetCartItems();
            }
            catch
            {

            }
        }
    }

    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}

public class CartItem : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public Product product { get; set; }
    public CartItemDto dto { get; set; }
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
