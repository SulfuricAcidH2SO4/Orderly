// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using Orderly.Interfaces;
using Orderly.Modules;
using Orderly.Views.Dialogs;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace Orderly.ViewModels.Pages
{
    public partial class SettingsViewModel : ViewModelBase, INavigationAware
    {
        [ObservableProperty]
        ProgramConfiguration configuration;

        public SettingsViewModel(IProgramConfiguration config)
        {
            Configuration = (ProgramConfiguration)config;
            Configuration.PropertyChanged += OnPropertyChanged;
        }

        [RelayCommand]
        public void ChangeMasterPassword()
        {
            PasswordConfirmDialog confirmDialog = new();

            if(confirmDialog.ShowDialog() == true) {
                PasswordChangeDialog dialog = new(Configuration);
                dialog.ShowDialog();
            }
        }
        [RelayCommand]
        private void ChangeInpuKeybind()
        {
            ChangeInputOptionsDialog dialog = new();
            if(dialog.ShowDialog() == true) {
                Configuration.InputOptions = dialog.InputOptions;
            }
        }

        public void OnNavigatedTo()
        {
        }

        public void OnNavigatedFrom() { }

        private void OnPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Configuration.Save();
        }


    }
}
