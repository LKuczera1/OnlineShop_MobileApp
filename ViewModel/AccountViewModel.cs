using OnlineShop_MobileApp.Models.DTOs;
using OnlineShop_MobileApp.Navigators;
using OnlineShop_MobileApp.Services;
using OnlineShop_MobileApp.ViewModel.Common;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace OnlineShop_MobileApp.ViewModel
{

    public class AccountViewModel : INotifyPropertyChanged
    {

        //Straszny to AI bordel, trzeba to sprzatnac jak juz apka zacznie dzialac poprawnie

        //---Moje linie kodu---

        private readonly IIdentityService _service;

        private IMainPageNavigator? _navigator;
        public void SetNavigator(IMainPageNavigator navigator) => _navigator = navigator;

        private UserDataDto _userData = new UserDataDto();


        private CurrentView _currentView = CurrentView.LoginScreen;
        public enum CurrentView
        {
            LoginScreen = 0,
            UserData = 1,
            RegisterScreen = 2,
        }

        public CurrentView currentView
        {
            get => _currentView;
            set
            {
                if (_currentView == value) return;
                _currentView = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsLoginScreenvisible));
                OnPropertyChanged(nameof(IsUserDataVisible));
                OnPropertyChanged(nameof(IsRegisterFormVisible));
            }
        }

        public bool IsLoginScreenvisible => currentView == CurrentView.LoginScreen;
        public bool IsUserDataVisible => currentView == CurrentView.UserData;
        public bool IsRegisterFormVisible => currentView == CurrentView.RegisterScreen;

        public UserDataDto UserData
        {
            get { return _userData; }
            set
            {
                _userData = value;
                OnPropertyChanged();
            }
        }

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
            get
            {
                if (_showPassword) return _hidePasswordIcon;
                return _showPasswordIcon;
            }
        }

        private ImageSource _userProfilePicture;

        public ImageSource UserProfilePicture
        {
            get { return _userProfilePicture; }
            set
            {
                _userProfilePicture = value;
                OnPropertyChanged();
            }
        }

        // LOGIN
        private LoginRequestDto _loginRequest = new LoginRequestDto();

        public LoginRequestDto LoginRequest
        {
            get { return _loginRequest; }
            set
            {
                _loginRequest = value;
                OnPropertyChanged();
            }
        }

        private void ClearLoginForm()
        {
            LoginRequest.Clear();
            OnPropertyChanged(nameof(LoginRequest));
        }

        // REGISTER
        private RegisterRequestDto _registerRequest = new RegisterRequestDto();

        public RegisterRequestDto RegisterRequest
        {
            get { return _registerRequest; }
            set
            {
                _registerRequest = value;
                OnPropertyChanged();
            }
        }

        private void ClearRegistrationForm()
        {
            RegisterRequest.Clear();
            OnPropertyChanged(nameof(RegisterRequest));
        }

        public ICommand LoginCommand { get; }
        public ICommand ShowRegisterCommand { get; }
        public ICommand ShowLoginCommand { get; }
        public ICommand CreateAccountCommand { get; }
        public ICommand TogglePasswordVisibilityCommand { get; }

        public ICommand LogOutCommand { get; }

        private string HidePasswordIconPath = "Images/closedeyeicon.png";
        private string ShowPasswordIconPath = "Images/openeyeicon.png";
        private string UserProfilePicturePath = "Images/userprofilepicture.png";

        private async Task LoadGraphicResources()
        {
            _hidePasswordIcon = await ResourcesLoader.LoadImageFromPackageAsync(HidePasswordIconPath);
            _showPasswordIcon = await ResourcesLoader.LoadImageFromPackageAsync(ShowPasswordIconPath);
            UserProfilePicture = await ResourcesLoader.LoadImageFromPackageAsync(UserProfilePicturePath);

            //Required to notify App of loaded resources
            OnPropertyChanged(nameof(PasswordVisibilityIcon));
        }

        public AccountViewModel(IIdentityService service)
        {
            //-----------------------------------------

            _service = service;

            LoadGraphicResources();
            //-----------------------------------------


            LoginCommand = new Command(async () =>
            {
                if (string.IsNullOrWhiteSpace(LoginRequest.UserName) || string.IsNullOrWhiteSpace(LoginRequest.Password)) return;

                var loginAttemptResult = await service.LoginAsync(LoginRequest);

                if (loginAttemptResult == null) _navigator.ShowAllert("Login failed", "Provided login data was wrong.");
                else
                {
                    _navigator.ShowMessage("Succesfully logged in");
                    currentView = CurrentView.UserData;
                    UserData = await _service.GetUserData(loginAttemptResult.UserId);

                    ClearLoginForm();

                    if (UserData == null) UserData = new UserDataDto();
                }
            });

            ShowRegisterCommand = new Command(() =>
            {
                currentView = CurrentView.RegisterScreen;

                ClearLoginForm();
            });


            ShowLoginCommand = new Command(() =>
            {
                currentView = CurrentView.LoginScreen;

                ClearRegistrationForm();
            });


            TogglePasswordVisibilityCommand = new Command(() =>
            {
                if (ShowPassword) ShowPassword = false;
                else ShowPassword = true;
                OnPropertyChanged(nameof(PasswordVisibilityIcon));
            });

            CreateAccountCommand = new Command(async () =>
            {
                var regResponse = await _service.CreateNewAccount(RegisterRequest);

                if (regResponse == null)
                {
                    _navigator.ShowAllert("Error", "An unexpected error occured");
                    return;
                }

                if (regResponse.Success)
                {
                    _navigator.ShowMessage(regResponse.Message);
                    currentView = CurrentView.LoginScreen;

                    ClearRegistrationForm();
                }
                else
                {
                    _navigator.ShowAllert("Registration form filled incorectly", regResponse.Message);
                }

            });

            LogOutCommand = new Command(() =>
            {
                _service.LogOut();
                currentView = CurrentView.LoginScreen;
            }
            );
        }

        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

}
