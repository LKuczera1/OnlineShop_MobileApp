using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShop_MobileApp.ViewModel
{
    using Chessie.ErrorHandling;
    using OnlineShop_MobileApp.Models;
    using OnlineShop_MobileApp.Navigators;
    using OnlineShop_MobileApp.Services;
    using OnlineShop_MobileApp.Views;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows.Input;

    public class CatalogViewModel : INotifyPropertyChanged
    {
        //----- Clearing all this messs
        public event PropertyChangedEventHandler? PropertyChanged;

        //Numbers of items on single page
        private const int PageSize = 20;

        //List of products
        private List<Product> _allItems = new();

        //List of produtcs (For UI)
        public ObservableCollection<Product> Items { get; } = new();

        //Numbers of pages
        public ObservableCollection<int> PageNumbers { get; } = new();

        private int _currentPage = 1;

        private int _totalPages = 1;

        //Predefying UI methods
        public ICommand GoToPageNumberCommand { get; }
        public ICommand PrevPageCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand GoToPageCommand { get; }

        //Service

        private readonly ICatalogService _service;

        //Task wait async (replacing canncelacion token)
        
        private TimeSpan _waitAsynctimeSpan = TimeSpan.FromSeconds(3);

        //More complex elements

        //Current page
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

        //Total available pages

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

        //Page control
        private bool _connectionFailed = false;

        //Background tasks
        private CancellationTokenSource? _bckTskCt;
        private readonly SemaphoreSlim _semaphore = new(1, 1);

        //Show main content
        public bool IsMainPageVisible
        {
            get => !_connectionFailed;
            set
            {
                if (_connectionFailed == value) return;
                _connectionFailed = value;
                OnPropertyChanged();
            }
        }

        //Show refresh button
        public bool IsRefreshButtonVisible
        {
            get => _connectionFailed;
            set
            {
                _connectionFailed = value;
                OnPropertyChanged();
            }
        }

        //Page buttons labels
        private int _pageCount;
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

        //Navigator

        private IMainPageNavigator? _navigator;
        public void SetNavigator(IMainPageNavigator navigator) => _navigator = navigator;

        public ICommand ProductDetailsCommand { get; }

        public ICommand BackToCatalogCommand { get; }

        //Methods


        //Constructor
        public CatalogViewModel(ICatalogService service)
        {
            _service = service;

            // Dobra todo list:
            // 1.Konczymy sprzatanie tego balaganu tutaj
            // 2.Doprowadzamy strone glowna do ladu
            // 3.Wprowadzamy pelna funkcjonalnosc services
            // 4. ???
            // 5. Profit
            //
            // A... No i wprowadzil pelna responsywnosc apki na stan serwisu... bo jak serwis jest off
            // to debugger wywala
            // No i tak. Jak jest: OK - Wyswietlamy strone, Brak polaczenia: "Connection error", wszystko pozostałe: "Wystapil blad."

            StartBackgroundProcessing();

            //Binding
            PrevPageCommand = new Command(async () => await SwitchPageAsync(CurrentPage - 1), () => CanPrev);
            NextPageCommand = new Command(async () => await SwitchPageAsync(CurrentPage + 1), () => CanNext);
            GoToPageNumberCommand = new Command<int>(async page => await SwitchPageAsync(page));

            ProductDetailsCommand = new Command<Product>(async product => await ShowProductDetails(product));
            BackToCatalogCommand = new Command( async () => await GoBackToCatalog());



            _allItems = new List<Product>();

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



        private void StartBackgroundProcessing()
        {
            if (_bckTskCt != null) return;

            _bckTskCt = new CancellationTokenSource();
            _ = BackgroundProcessing(_bckTskCt.Token);
        }

        public void StopBackgroundProcessing()
        {
            _bckTskCt?.Cancel();
            _bckTskCt?.Dispose();
            _bckTskCt = null;
        }

        private async Task BackgroundProcessing(CancellationToken ct)
        {
            await LoadPageContent(CurrentPage);

            try
            {
                //Second connection attempt, just in case first one was unsucsesful
                if(IsRefreshButtonVisible)
                {
                    IsRefreshButtonVisible = false;
                    await Task.Delay(TimeSpan.FromSeconds(3), ct);

                    await LoadPageContent(CurrentPage);
                }
            }
            catch (OperationCanceledException) { }
            finally
            {
                //timer.Dispose();
            }
        }

        private async Task LoadPageContent(int page)
        {
            //get items
            try
            {
                await LoadPage(page);
                IsRefreshButtonVisible = false;
            }
            catch (TimeoutException)
            {
                IsRefreshButtonVisible = true;
            }
            catch (TaskCanceledException)
            {
                IsRefreshButtonVisible = true;
            }
            catch(Service.ConnectionErrorException)
            {

            }
            catch (Exception)
            {
                IsRefreshButtonVisible = true;
            }
        }

        private async Task LoadPage(int page)
        {
            try
            {
                await GetItemsForCurrentPage(page);
                await GetNumberOfPages();

                CurrentPage = page;
                //execute state refresh
                (PrevPageCommand as Command)?.ChangeCanExecute();
                (NextPageCommand as Command)?.ChangeCanExecute();
            }
            catch(Exception e)
            {
                throw;
            }
        }

        //-----------------------------------------------------------------------------------------------

        private async Task ShowProductDetails(Product product)
        {
            if(_navigator!=null)
                _navigator.ShowProductDetails(product);
        }

        private async Task GoBackToCatalog()
        {
            if (_navigator != null)
                _navigator.ShowCatalog();
        }

        //-----------------------------------------------------------------------------------------------

        private async Task GetItemsForCurrentPage(int pageNumber)
        {
            var products = await _service.GetProducts(pageNumber).WaitAsync(_waitAsynctimeSpan);

            _allItems = products;

            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                Items.Clear();
                foreach (var p in _allItems)
                {
                    await LoadProductsPictures(p);
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

        private async Task LoadProductsPictures(Product product)
        {
            //If there is no photo for picture default string is "string", that should be changed in REST API
            if(string.IsNullOrWhiteSpace(product.PicturePath) || string.Equals("string", product.PicturePath))
            {
                //Temporally const path for nophoto icons
                string nophotoiconpath = "D:\\Programming\\Projects\\Visual Studio\\OnlineShop_MobileApp\\Resources\\Images\\nophotoicon.png";

                product.ImageSource = ImageSource.FromFile(nophotoiconpath);
            }
            else
            {
                try
                {
                    var pic = await _service.LoadProductPicture(product.Id);
                    product.ImageSource = ImageSource.FromStream(() => new MemoryStream(pic));
                }
                catch
                {

                }
            }
        }

        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
