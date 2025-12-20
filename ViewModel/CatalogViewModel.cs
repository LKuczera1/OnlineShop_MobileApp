using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShop_MobileApp.ViewModel
{
    using OnlineShop_MobileApp.Models;
    using OnlineShop_MobileApp.Services;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows.Input;

    public class CatalogViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private const int PageSize = 20;

        // Źródło danych (na start może być mock, docelowo np. z API/DB)
        private readonly List<Product> _allItems = new();

        public ObservableCollection<Product> Items { get; } = new();

        public ObservableCollection<int> PageNumbers { get; } = new();
        public ICommand GoToPageNumberCommand { get; }

        //-------------------------- DI troubleshotting
        private readonly ICatalogService _service;




        private int _currentPage = 1;
        public int CurrentPage
        {
            get => _currentPage;
            private set
            {
                if (_currentPage == value) return;
                _currentPage = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PageLabel));
                OnPropertyChanged(nameof(CanPrev));
                OnPropertyChanged(nameof(CanNext));
            }
        }

        private int _totalPages = 1;
        public int TotalPages
        {
            get => _totalPages;
            private set
            {
                if (_totalPages == value) return;
                _totalPages = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PageLabel));
                OnPropertyChanged(nameof(CanPrev));
                OnPropertyChanged(nameof(CanNext));
            }
        }

        public string PageLabel => $"Strona {CurrentPage} / {TotalPages}";
        public bool CanPrev => CurrentPage > 1;
        public bool CanNext => CurrentPage < TotalPages;

        private string? _goToPageText;
        public string? GoToPageText
        {
            get => _goToPageText;
            set { _goToPageText = value; OnPropertyChanged(); }
        }

        public ICommand PrevPageCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand GoToPageCommand { get; }



        public CatalogViewModel(ICatalogService service)
        {
            _service = service;

            PrevPageCommand = new Command(() => LoadPage(CurrentPage - 1), () => CanPrev);
            NextPageCommand = new Command(() => LoadPage(CurrentPage + 1), () => CanNext);
            GoToPageCommand = new Command(GoToPage);

            var temp = _service.GetProducts(0);
            _allItems = new List<Product>();

            _allItems = temp.Result;

            RecalcPagesAndLoad(1);
            GoToPageNumberCommand = new Command<int>(page => LoadPage(page));
        }

        private void RecalcPagesAndLoad(int page)
        {
            TotalPages = Math.Max(1, (int)Math.Ceiling(_allItems.Count / (double)PageSize));

            PageNumbers.Clear();
            for (int i = 1; i <= TotalPages; i++)
                PageNumbers.Add(i);

            LoadPage(page);
        }

        private void LoadPage(int page)
        {
            page = Math.Clamp(page, 1, TotalPages);
            CurrentPage = page;

            var slice = _allItems
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            Items.Clear();
            foreach (var it in slice) Items.Add(it);

            // odśwież stan CanExecute
            (PrevPageCommand as Command)?.ChangeCanExecute();
            (NextPageCommand as Command)?.ChangeCanExecute();
        }

        private void GoToPage()
        {
            if (!int.TryParse(GoToPageText, out var page))
                return;

            LoadPage(page);
        }

        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
