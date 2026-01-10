using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShop_MobileApp.ViewModel
{
    using OnlineShop_MobileApp.Services;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows.Input;

    public class AccountViewModel : INotifyPropertyChanged
    {

        //Straszny to AI bordel, trzeba to sprzatnac jak juz apka zacznie dzialac poprawnie

        //---Moje linie kodu---

        private readonly IIdentityService _service;

        //---------------------

        public event PropertyChangedEventHandler? PropertyChanged;

        private ImageSource _showPasswordIcon;

        private ImageSource _hidePasswordIcon;

        private bool _showPassword = true;

        public bool ShowPassword
        {
            get { return _showPassword; }
            set
            {
                _showPassword = value;
                OnPropertyChanged();
            }
        }

        public ImageSource PasswordVisibilityIcon
        {
            get {
                if (_showPassword) return _hidePasswordIcon;
                return _showPasswordIcon;
            }
        }

        private bool _isLoggedIn;
        public bool IsLoggedIn
        {
            get => _isLoggedIn;
            set { _isLoggedIn = value; OnPropertyChanged(); OnPropertyChanged(nameof(ShowLoginForm)); OnPropertyChanged(nameof(ShowRegisterForm)); OnPropertyChanged(nameof(ShowLoggedIn)); }
        }

        private bool _isRegisterMode;
        public bool IsRegisterMode
        {
            get => _isRegisterMode;
            set { _isRegisterMode = value; OnPropertyChanged(); OnPropertyChanged(nameof(ShowLoginForm)); OnPropertyChanged(nameof(ShowRegisterForm)); }
        }

        public bool ShowLoginForm => !IsLoggedIn && !IsRegisterMode;
        public bool ShowRegisterForm => !IsLoggedIn && IsRegisterMode;
        public bool ShowLoggedIn => IsLoggedIn;

        // LOGIN
        private string? _login;
        public string? Login { get => _login; set { _login = value; OnPropertyChanged(); } }

        private string? _password;
        public string? Password { get => _password; set { _password = value; OnPropertyChanged(); } }

        // REGISTER
        private string? _regLogin;
        public string? RegLogin { get => _regLogin; set { _regLogin = value; OnPropertyChanged(); } }

        private string? _regEmail;
        public string? RegEmail { get => _regEmail; set { _regEmail = value; OnPropertyChanged(); } }

        private string? _regPassword;
        public string? RegPassword { get => _regPassword; set { _regPassword = value; OnPropertyChanged(); } }

        

        public ICommand LoginCommand { get; }
        public ICommand ShowRegisterCommand { get; }
        public ICommand ShowLoginCommand { get; }
        public ICommand CreateAccountCommand { get; }

        public ICommand TogglePasswordVisibilityCommand { get; }

        public AccountViewModel(IIdentityService service)
        {
            //-----------------------------------------

            _service = service;

            _hidePasswordIcon = ImageSource.FromFile("D:\\Programming\\Projects\\Visual Studio\\OnlineShop_MobileApp\\Resources\\Images\\openeyeicon.png");
            _showPasswordIcon = ImageSource.FromFile("D:\\Programming\\Projects\\Visual Studio\\OnlineShop_MobileApp\\Resources\\Images\\closedeyeicon.png");

            //-----------------------------------------


            LoginCommand = new Command(() =>
            {
                // na razie "na sztywno": po kliknięciu uznajemy, że zalogowany
                IsLoggedIn = true;
            });

            ShowRegisterCommand = new Command(() => IsRegisterMode = true);
            ShowLoginCommand = new Command(() => IsRegisterMode = false);
            TogglePasswordVisibilityCommand = new Command(() =>
            {
                if (ShowPassword) ShowPassword = false;
                else ShowPassword = true;
                OnPropertyChanged(nameof(PasswordVisibilityIcon));
            });

            CreateAccountCommand = new Command(() =>
            {
                // na razie: "utwórz konto" i od razu zaloguj / albo wróć do loginu — jak wolisz
                IsLoggedIn = true;
            });
        }

        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

}
