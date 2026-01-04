using OnlineShop_MobileApp.ViewModel;
using System.Windows.Input;

namespace OnlineShop_MobileApp.Views;

public partial class ProductDetailsView : ContentView
{
    public static readonly BindableProperty GoBackCommandProperty =
        BindableProperty.Create(nameof(GoBackCommand), typeof(ICommand), typeof(ProductDetailsView));

    public ICommand? GoBackCommand
    {
        get => (ICommand?)GetValue(GoBackCommandProperty);
        set => SetValue(GoBackCommandProperty, value);
    }

    public ProductDetailsView(CatalogViewModel catalogViewModel)
    {
        InitializeComponent();
        GoBackCommand = catalogViewModel.BackToCatalogCommand;
    }
}