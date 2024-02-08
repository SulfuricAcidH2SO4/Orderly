// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using Orderly.Interfaces;
using Orderly.Modules;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace Orderly.ViewModels.Pages
{
    public partial class SettingsViewModel : ViewModelBase, INavigationAware
    {
        private bool _isInitialized = false;

        [ObservableProperty]
        private string _appVersion = String.Empty;

        [ObservableProperty]
        ProgramConfiguration configuration;

        public void OnNavigatedTo()
        {
            if (!_isInitialized)
                InitializeViewModel();
        }

        public void OnNavigatedFrom() { }

        private void InitializeViewModel()
        {
            AppVersion = $"{GetAssemblyVersion()}";
            Configuration = (ProgramConfiguration?)App.GetService<IProgramConfiguration>()!;
            Configuration = IProgramConfiguration.Load();
            Configuration.PropertyChanged += OnPropertyChanged;
            _isInitialized = true;
        }

        private void OnPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Configuration.Save();
            if(e.PropertyName == nameof(Configuration.IsDarkMode))
            {
                if (Configuration.IsDarkMode) ApplicationThemeManager.Apply(ApplicationTheme.Dark);
                else ApplicationThemeManager.Apply(ApplicationTheme.Light);
            }
        }

        private string GetAssemblyVersion()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString()
                ?? String.Empty;
        }

    }
}
