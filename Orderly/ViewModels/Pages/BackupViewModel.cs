using Orderly.Helpers;
using Orderly.Interfaces;
using Orderly.Modules;
using Orderly.Modules.Routines;
using Orderly.Views.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Wpf.Ui.Controls;

namespace Orderly.ViewModels.Pages
{
    public partial class BackupViewModel : ViewModelBase, INavigationAware
    {
        [ObservableProperty]
        ProgramConfiguration config;
        [ObservableProperty]
        private bool isFlyoutOpen = false;
        [ObservableProperty]
        private IBackupRoutine? selectedRoutine;

        public void OnNavigatedFrom()
        {
        }

        public void OnNavigatedTo()
        {
        }

        public BackupViewModel(IProgramConfiguration config)
        {
            Config = (ProgramConfiguration?)config!;
        }

        [RelayCommand]
        public void ApplyRoutines()
        {
            Config.Save();
        }

        [RelayCommand]
        public void OpenFlyout()
        {
            IsFlyoutOpen = true;
        }

        [RelayCommand]
        public void AddRoutine(string type)
        {
            switch (type) {
                case "local":
                    LocalBackup local = new();
                    Config.BackupRoutines.Add(local);
                    break;
            }

            IsFlyoutOpen = false;
        }

        [RelayCommand]
        public void RemoveRoutine()
        {
            if (SelectedRoutine == null) return;
            if (new ConfirmDialog("Are you sure you want to delete this backup routine?\nThis action cannot be reverted.").ShowDialog() == false) return;
            if(new PasswordConfirmDialog().ShowDialog() == false) return;
            Config.BackupRoutines.Remove(SelectedRoutine);
            Config.Save();
        }

        [RelayCommand]
        public void DoBackup()
        {
            if (SelectedRoutine == null) return;
            SelectedRoutine.Backup(out string error);
        }
        
        [RelayCommand]
        public void PickLocalPath()
        {
            if (SelectedRoutine is not LocalBackup lb) return;

            FolderBrowserDialog dialog = new() {
                Description = "Select a backup folder",
                ShowNewFolderButton = true,
            };

            if (dialog.ShowDialog() == DialogResult.OK) {
                lb.Path = dialog.SelectedPath;
            }
        }
    }
}
