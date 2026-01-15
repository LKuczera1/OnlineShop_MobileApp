using Mono.Cecil;
using OnlineShop_MobileApp.Resources.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShop_MobileApp.Services
{
    public enum AppThemeMode
    {
        Light = 0,
        Dark = 1,
        Gold = 2,
    }

    public class ThemeService
    {
        public AppThemeMode Current { get; private set; } = AppThemeMode.Light;

        private CustomStyles _customStyles;

        private ResourceDictionary _light;
        private ResourceDictionary _dark;
        private ResourceDictionary _gold;

        private static string themePath = "Resources/Themes/";

        private static string darkThemePath = "Resources/Themes/DarkTheme.xaml";
        private static string lightThemePath = "Resources/Themes/LightTheme.xaml";
        private static string goldThemePath = "Resources/Themes/GoldTheme.xaml";


        private ResourceDictionary? _currentDictionary;

        public void Initialize()
        {
            _customStyles = new CustomStyles();

            _light = new LightTheme();
            _dark = new DarkTheme();
            _gold = new GoldTheme();

            EnsureStylesLoaded();
            Apply(Current);
        }

        private void EnsureStylesLoaded()
        {
            var app = Application.Current;
            if (app is null) return;

            var merged = app.Resources.MergedDictionaries;

            if (!merged.Contains(_customStyles!))
                merged.Add(_customStyles!);
        }

        public void Apply(AppThemeMode mode)
        {
            Current = mode;

            var app = Application.Current;
            if (app is null) return;

            EnsureStylesLoaded();

            var merged = app.Resources.MergedDictionaries;

            if (_currentDictionary is not null)
                merged.Remove(_currentDictionary);

            _currentDictionary = mode switch
            {
                AppThemeMode.Dark => _dark,
                AppThemeMode.Gold => _gold,
                AppThemeMode.Light => _light,
                _ => _light
            };

            merged.Add(_currentDictionary);
        }
    }
}
