using Orderly.Backups;
using Orderly.DaVault;
using Orderly.Interfaces;
using Orderly.Modules;
using Orderly.Views.Dialogs;
using System.Diagnostics;
using System.IO;
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

            if (confirmDialog.ShowDialog() == true) {
                PasswordChangeDialog dialog = new(Configuration);
                dialog.ShowDialog();
            }
        }
        [RelayCommand]
        private void ChangeInpuKeybind()
        {
            ChangeInputOptionsDialog dialog = new();
            if (dialog.ShowDialog() == true) {
                Configuration.InputOptions = dialog.InputOptions;
            }
        }

        public void OnNavigatedTo()
        {
        }

        public void OnNavigatedFrom() { }

        private void OnPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Configuration.StartOnStartUp)) {
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
                    Verb = "runas",
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
                    Verb = "runas",
                    Arguments = $"remove-startup {App.GetService<Vault>().ConfigEncryptionKey}"
                };

                Process.Start(startInfo)?.WaitForExit();
            }
        }
    }
}
