// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using Orderly.Backups;
using Orderly.DaVault;
using Orderly.Interfaces;
using Orderly.Modules;
using Orderly.Views.Dialogs;
using System.Diagnostics;
using System.IO;
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
            if(e.PropertyName == nameof(Configuration.StartOnStartUp)) {
                Configuration.PropertyChanged -= OnPropertyChanged;
                if (Configuration.StartOnStartUp) AddStartup();
                else RemoveStartup();
                Configuration = IProgramConfiguration.Load(App.GetService<Vault>());
                Configuration.PropertyChanged += OnPropertyChanged;
            }
            Configuration.Save();
        }

        private void AddStartup()
        {
            string executablePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Orderly.Dog.exe");

            BackupWorker.RunAllBackups();

            if (File.Exists(executablePath)) {
                ProcessStartInfo startInfo = new ProcessStartInfo {
                    FileName = executablePath,
                    Arguments = $"add-startup {Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Orderly.exe")} {App.GetService<Vault>().ConfigEncryptionKey}"
                };

                Process.Start(startInfo)?.WaitForExit();
            }
        }

        private void RemoveStartup()
        {
            string executablePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Orderly.Dog.exe");

            BackupWorker.RunAllBackups();

            if (File.Exists(executablePath)) {
                ProcessStartInfo startInfo = new ProcessStartInfo {
                    FileName = executablePath,
                    Arguments = $"remove-startup {App.GetService<Vault>().ConfigEncryptionKey}"
                };

                Process.Start(startInfo)?.WaitForExit();
            }
        }
    }
}
