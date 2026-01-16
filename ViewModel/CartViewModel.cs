using OnlineShop_MobileApp.Models;
using OnlineShop_MobileApp.Models.DTOs;
using OnlineShop_MobileApp.Services;
using OnlineShop_MobileApp.Services.Resolver;
using OnlineShop_MobileApp.ViewModel.Common;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json.Nodes;
using System.Windows.Input;

public class CartViewModel : INotifyPropertyChanged
{
    //---Dependencies---
    private readonly IShoppingService _service;
    private readonly IServicesResolver _servicesResolver;
    public event PropertyChangedEventHandler? PropertyChanged;
    public ObservableCollection<CartItem> Items { get; } = new();

    //---Properties---

    private bool _isCartViewAvailable = false;
    public bool IsCartViewAvailable
    {
        get => _isCartViewAvailable;
        set
        {
            _isCartViewAvailable = value;
            OnPropertyChanged();
        }
    }

    private double _totalPrice = 0;
    public double TotalPrice
    {
        get => _totalPrice;
        set
        {
            _totalPrice = value;
            OnPropertyChanged();
        }
    }

    //---Commands---
    public ICommand IncreaseCommand { get; }
    public ICommand DecreaseCommand { get; }
    public ICommand RemoveCommand { get; }
    public ICommand PlaceOrderCommand { get; }

    ImageSource NoPhotoIcon;

    private string noPhotoRelativePath = "Images/nophotoicon.png";


    public CartViewModel(IShoppingService service, IServicesResolver servicesResolver)
    {
        _service = service;
        _servicesResolver = servicesResolver;
        LoadGraphicResources();

        IncreaseCommand = new Command<CartItem>(OnIncrease);
        DecreaseCommand = new Command<CartItem>(OnDecrease);
        RemoveCommand = new Command<CartItem>(item => _ = RemoveItemAsync(item));
        PlaceOrderCommand = new Command(() => PlaceOrder());
    }

    public async Task Refresh()
    {
        IsCartViewAvailable = _service.isUserLoggedIn();

        if (!IsCartViewAvailable)
            return;

        try
        {
            await GetCartItems();
        }
        catch
        {
            //I know, this shoud not be empty....
        }
    }

    public async Task GetCartItems()
    {
        var items = await _service.GetCartItems();

        Items.Clear();
        foreach (var item in items)
        {
            var newCartItem = new CartItem
            {
                dto = item
            };

            newCartItem.product = await _servicesResolver.ResolveForProduct(item.ProductId);
            if (newCartItem.product == null)
                continue;

            try
            {
                MemoryStream temp = new MemoryStream(
                    await _servicesResolver.ResolveForProductThumbnail(item.ProductId)
                );

                newCartItem.product.ThumbnailSource = ImageSource.FromStream(() => temp);
            }
            catch
            {
                newCartItem.product.ThumbnailSource = NoPhotoIcon;
            }

            newCartItem.TotalPrice = newCartItem.dto.Price * newCartItem.Quantity;
            Items.Add(newCartItem);
        }

        CalculateTotalPrice();
    }

    public void CalculateTotalPrice()
    {
        double sum = 0;

        foreach (var item in Items)
            sum += item.TotalPrice;

        TotalPrice = sum;
    }

    private void OnIncrease(CartItem? item)
    {
        if (item == null) return;

        item.Quantity++;
        item.TotalPrice = item.dto.Price * item.Quantity;

        CalculateTotalPrice();
    }

    private void OnDecrease(CartItem? item)
    {
        if (item == null) return;

        if (item.Quantity > 1)
            item.Quantity--;

        item.TotalPrice = item.dto.Price * item.Quantity;

        CalculateTotalPrice();
    }
    private async Task PlaceOrder()
    {
        if (Items.Count == 0) return;

        var jsonArray = new JsonArray();

        foreach (var item in Items)
        {
            jsonArray.Add(new JsonObject
            {
                ["id"] = item.dto.Id,
                ["quantity"] = item.dto.Quantity
            });
        }

        var result = await _service.PlaceOrder(JsonContent.Create(jsonArray));

        if (result)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Items.Clear();
                CalculateTotalPrice();
                OnPropertyChanged(nameof(Items));
            });
        }
        else
        {
            
        }
    }

    private async Task RemoveItemAsync(CartItem? item)
    {
        if (item == null) return;

        var id = item.dto?.Id;
        if (id is null) return;

        if (await _service.RemoveCartItem(id.Value))
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Items.Remove(item);
                CalculateTotalPrice();
                OnPropertyChanged(nameof(Items));
            });
        }
        else
        {
            
        }
    }
    private async Task LoadGraphicResources()
    {
        NoPhotoIcon = await ResourcesLoader.LoadImageFromPackageAsync(noPhotoRelativePath);
    }

    
    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}

public class CartItem : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public Product product { get; set; }
    public CartItemDto dto { get; set; }

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

    private double _totalPrice = 1;
    public double TotalPrice
    {
        get => _totalPrice;
        set
        {
            _totalPrice = value;
            OnPropertyChanged();
        }
    }

    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
