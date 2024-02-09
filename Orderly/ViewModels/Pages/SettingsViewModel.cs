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

        public SettingsViewModel(IProgramConfiguration config)
        {
            AppVersion = $"{GetAssemblyVersion()}";
            Configuration = (ProgramConfiguration)config;
            Configuration.PropertyChanged += OnPropertyChanged;
        }

        public void OnNavigatedTo()
        {
        }

        public void OnNavigatedFrom() { }

        private void OnPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Configuration.Save();
        }

        private string GetAssemblyVersion()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString()
                ?? String.Empty;
        }

    }
}
