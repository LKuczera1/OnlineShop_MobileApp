namespace OnlineShop_MobileApp.ViewModel
{
    using OnlineShop_MobileApp.Models;
    using OnlineShop_MobileApp.Navigators;
    using OnlineShop_MobileApp.Services;
    using OnlineShop_MobileApp.Services.Resolver;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows.Input;
    using ViewModel.Common;

    public class CatalogViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private const int PageSize = 20;
        private const int connectionTimeout = 0;

        private readonly ICatalogService _service;
        private readonly IServicesResolver _servicesResolver;

        private List<Product> _allItems = new();

        private int _currentPage = 1;
        private int _totalPages = 1;
        private int _pageCount;

        private TimeSpan _waitAsynctimeSpan = TimeSpan.FromSeconds(3);

        private ImageSource NoThumbnailIcon;
        private ImageSource NoPhotoIcon;

        private string noThumbnailRelativePath = "Images/nophotothumbnail.png";
        private string noPhotoRelativePath = "Images/nophotoicon.png";

        private CancellationTokenSource? _bckTskCt;
        private readonly SemaphoreSlim _semaphore = new(1, 1);

        private IMainPageNavigator? _navigator;

        private ConnectionState _connectionState = ConnectionState.Loading;
        public enum ConnectionState
        {
            Connected = 0,
            ConnectionFailed = 1,
            Loading = 2,
        }

        public ObservableCollection<Product> Items { get; } = new();
        public ObservableCollection<int> PageNumbers { get; } = new();

        //---Commands---
        public ICommand GoToPageNumberCommand { get; }
        public ICommand PrevPageCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand GoToPageCommand { get; }

        public ICommand RefreshCommand { get; }

        public ICommand ProductDetailsCommand { get; }
        public ICommand BackToCatalogCommand { get; }
        public ICommand AddToCartCommand { get; }

        public CatalogViewModel(ICatalogService service, IServicesResolver servicesResolver)
        {
            _service = service;
            _servicesResolver = servicesResolver;

            StartBackgroundProcessing();

            PrevPageCommand = new Command(async () => await SwitchPageAsync(CurrentPage - 1), () => CanPrev);
            NextPageCommand = new Command(async () => await SwitchPageAsync(CurrentPage + 1), () => CanNext);
            GoToPageNumberCommand = new Command<int>(async page => await SwitchPageAsync(page));

            ProductDetailsCommand = new Command<Product>(async product => await ShowProductDetails(product));
            BackToCatalogCommand = new Command(async () => await GoBackToCatalog());
            RefreshCommand = new Command(() => Refresh());

            AddToCartCommand = new Command<Product>(async product => await AddToCart(product.Id));

            _allItems = new List<Product>();

            LoadGraphicResources();
        }

        public int CurrentPage
        {
            get => _currentPage;
            private set
            {
                if (_currentPage == value) return;
                _currentPage = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanPrev));
                OnPropertyChanged(nameof(CanNext));
            }
        }

        public int TotalPages
        {
            get => _totalPages;
            private set
            {
                if (_totalPages == value) return;
                _totalPages = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanPrev));
                OnPropertyChanged(nameof(CanNext));
            }
        }

        public ConnectionState ConnectionStatus
        {
            get => _connectionState;
            set
            {
                if (_connectionState == value) return;
                _connectionState = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsLoadingVisible));
                OnPropertyChanged(nameof(IsMainPageVisible));
                OnPropertyChanged(nameof(IsRefreshButtonVisible));
            }
        }

        public bool IsLoadingVisible => ConnectionStatus == ConnectionState.Loading;
        public bool IsMainPageVisible => ConnectionStatus == ConnectionState.Connected;
        public bool IsRefreshButtonVisible => ConnectionStatus == ConnectionState.ConnectionFailed;
        public bool IsOverlayVisible => IsLoadingVisible || IsRefreshButtonVisible;

        public int PageCount
        {
            get => _pageCount;
            private set
            {
                if (_pageCount == value) return;
                _pageCount = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanPrev));
                OnPropertyChanged(nameof(CanNext));
            }
        }

        public bool CanPrev => CurrentPage > 1;
        public bool CanNext => CurrentPage < PageCount;

        public void SetNavigator(IMainPageNavigator navigator) => _navigator = navigator;

        public void StopBackgroundProcessing()
        {
            _bckTskCt?.Cancel();
            _bckTskCt?.Dispose();
            _bckTskCt = null;
        }

        private void Refresh()
        {
            ConnectionStatus = ConnectionState.Loading;
            StopBackgroundProcessing();
            StartBackgroundProcessing();
        }

        private async Task LoadGraphicResources()
        {
            NoThumbnailIcon = await ResourcesLoader.LoadImageFromPackageAsync(noThumbnailRelativePath);
            NoPhotoIcon = await ResourcesLoader.LoadImageFromPackageAsync(noPhotoRelativePath);
        }

        private void StartBackgroundProcessing()
        {
            if (_bckTskCt != null) return;

            _bckTskCt = new CancellationTokenSource();
            _bckTskCt.CancelAfter(TimeSpan.FromSeconds(3));
            _ = BackgroundProcessing(_bckTskCt.Token);
        }

        private async Task BackgroundProcessing(CancellationToken ct)
        {
            await LoadPageContent(CurrentPage);

            try
            {
                if (IsRefreshButtonVisible)
                {
                    await Task.Delay(TimeSpan.FromSeconds(connectionTimeout), ct);
                    await LoadPageContent(CurrentPage);
                }
            }
            catch (OperationCanceledException) { }

            StopBackgroundProcessing();
        }

        private async Task SwitchPageAsync(int pageIndex)
        {
            if (pageIndex < 0 || pageIndex > PageCount) return;

            await _semaphore.WaitAsync();
            try
            {
                await LoadPageContent(pageIndex);
                (PrevPageCommand as Command)?.ChangeCanExecute();
                (NextPageCommand as Command)?.ChangeCanExecute();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task LoadPageContent(int page)
        {
            try
            {
                await LoadPage(page);
                ConnectionStatus = ConnectionState.Connected;
            }
            catch (Exception ex) when (ex is TimeoutException or TaskCanceledException)
            {
                ConnectionStatus = ConnectionState.ConnectionFailed;
            }
            catch (Exception)
            {
                ConnectionStatus = ConnectionState.ConnectionFailed;
            }
        }

        private async Task LoadPage(int page)
        {
            try
            {
                await GetItemsForCurrentPage(page);
                await GetNumberOfPages();

                CurrentPage = page;
                (PrevPageCommand as Command)?.ChangeCanExecute();
                (NextPageCommand as Command)?.ChangeCanExecute();
            }
            catch (Exception e)
            {
                ConnectionStatus = ConnectionState.ConnectionFailed;
                throw;
            }
        }

        private async Task ShowProductDetails(Product product)
        {
            if (_navigator != null)
            {
                await LoadProductPicture(product, false);
                _navigator.ShowProductDetails(product);
            }
        }

        private async Task AddToCart(int productId)
        {
            if (_navigator != null)
            {
                if (_service.isUserLoggedIn())
                {
                    await _servicesResolver.ResolveInsertItemIntoCart(productId);
                }
                else
                {
                    _navigator.ShowAllert("User not logged in", "Please log in to perform this operation");
                    _navigator.RedirectToLoginPage();
                }
            }
        }

        private async Task GoBackToCatalog()
        {
            if (_navigator != null)
                _navigator.ShowCatalog();
        }

        private async Task GetItemsForCurrentPage(int pageNumber)
        {
            var products = await _service.GetProducts(pageNumber).WaitAsync(_waitAsynctimeSpan);

            _allItems = products;

            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                Items.Clear();
                foreach (var p in _allItems)
                {
                    await LoadProductPicture(p, true);
                    Items.Add(p);
                }
            });
        }

        private async Task GetNumberOfPages()
        {
            var totalProducts = await _service.GetNumberOfPages().WaitAsync(_waitAsynctimeSpan);

            PageCount = (totalProducts + PageSize - 1) / PageSize;

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                PageNumbers.Clear();
                for (int i = 1; i <= PageCount; i++)
                    PageNumbers.Add(i);
            });
        }

        private async Task LoadProductPicture(Product product, bool LoadThumbnail = false)
        {
            Func<ImageSource> nopicture = () => LoadThumbnail ? NoThumbnailIcon : NoPhotoIcon;

            Func<Task<byte[]?>> LoadProductPicture = async () =>
                LoadThumbnail
                    ? await _service.LoadProductThumbnail(product.Id)
                    : await _service.LoadProductPicture(product.Id);

            Func<string?> getter = () => LoadThumbnail ? product.ThumbnailName : product.PictureName;
            Action<ImageSource> setter = value =>
            {
                if (LoadThumbnail) product.ThumbnailSource = value;
                else product.ImageSource = value;
            };

            Action setNoPhotoIcon = () => setter(nopicture());

            if (string.IsNullOrEmpty(getter()))
            {
                setNoPhotoIcon();
                return;
            }

            try
            {
                var picture = await LoadProductPicture();
                if (picture is null)
                {
                    setNoPhotoIcon();
                    return;
                }

                setter(ImageSource.FromStream(() => new MemoryStream(picture)));
            }
            catch
            {
                setNoPhotoIcon();
            }
        }

        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}