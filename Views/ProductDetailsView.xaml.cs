using OnlineShop_MobileApp.ViewModel;
using System.Windows.Input;

namespace OnlineShop_MobileApp.Views;

public partial class ProductDetailsView : ContentView
{
    public static readonly BindableProperty GoBackCommandProperty =
        BindableProperty.Create(nameof(GoBackCommand), typeof(ICommand), typeof(ProductDetailsView));

    public static readonly BindableProperty AddToCartCommandProperty =
        BindableProperty.Create(nameof(AddToCartCommand), typeof(ICommand), typeof(ProductDetailsView));

    public ICommand? GoBackCommand
    {
        get => (ICommand?)GetValue(GoBackCommandProperty);
        set => SetValue(GoBackCommandProperty, value);
    }

    public ICommand? AddToCartCommand
    {
        get => (ICommand?)GetValue(AddToCartCommandProperty);
        set => SetValue(AddToCartCommandProperty, value);
    }

    public ProductDetailsView(CatalogViewModel catalogViewModel)
    {
        InitializeComponent();
        GoBackCommand = catalogViewModel.BackToCatalogCommand;
        AddToCartCommand = catalogViewModel.AddToCartCommand;
    }
}