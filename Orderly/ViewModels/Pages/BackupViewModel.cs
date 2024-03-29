﻿using Orderly.Helpers;
using Orderly.Interfaces;
using Orderly.Models.Backup;
using Orderly.Modules;
using Orderly.Modules.Routines;
using Orderly.Views.Dialogs;
using System.Windows.Forms;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace Orderly.ViewModels.Pages
{
    public partial class BackupViewModel : ViewModelBase, INavigationAware
    {
        private IBackupRoutine? selectedRoutine;

        [ObservableProperty]
        ProgramConfiguration config;
        [ObservableProperty]
        private bool isFlyoutOpen = false;

        public IBackupRoutine? SelectedRoutine
        {
            get => selectedRoutine;
            set
            {
                SetProperty(ref selectedRoutine, value);
                if (value == null) return;
                if (value is LocalBackupRoutine lb) {
                    RunCommand(() => {
                        lb.ReloadBackups();
                    });
                }
                else if (value is GoogleDriveRoutine gd && gd.IsAuthenticated) {
                    RunCommand(() => {
                        gd.ReloadBackups();
                    });
                }
                else {
                    RunCommand(() => {
                        value.ReloadBackups();
                    });
                }
            }
        }


        public ExtendedObservableCollection<LocalBackup> BackupsInFolderList { get; set; } = new();

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
                    LocalBackupRoutine local = new();
                    Config.BackupRoutines.Add(local);
                    break;
                case "google":
                    //GoogleDriveRoutine gdrive = new();
                    //Config.BackupRoutines.Add(gdrive);
                    break;
                case "ftp":
                    FtpRoutine ftp = new();
                    Config.BackupRoutines.Add(ftp);
                    break;
            }

            IsFlyoutOpen = false;
        }

        [RelayCommand]
        public void RemoveRoutine()
        {
            if (SelectedRoutine == null) return;
            if (new ConfirmDialog("Are you sure you want to delete this backup routine?\nThis action cannot be reverted.").ShowDialog() == false) return;
            if (new PasswordConfirmDialog().ShowDialog() == false) return;
            Config.BackupRoutines.Remove(SelectedRoutine);
            Config.Save();
        }

        [RelayCommand]
        public void DoBackup()
        {
            RunCommand(() => {
                if (SelectedRoutine == null) return;
                if (SelectedRoutine.Backup()) {
                    App.Current.Dispatcher.Invoke(() => {
                        App.GetService<ISnackbarService>().Show(
                            "You did it!",
                            "Backup created successfully!",
                            ControlAppearance.Success,
                            new SymbolIcon(SymbolRegular.Check20),
                            TimeSpan.FromSeconds(2)

                    );
                    });
                }
                else {
                    App.Current.Dispatcher.Invoke(() => {
                        App.GetService<ISnackbarService>().Show(
                            "Oh no! :(",
                            "Something went wrong...",
                            ControlAppearance.Danger,
                            new SymbolIcon(SymbolRegular.Dismiss12),
                            TimeSpan.FromSeconds(2)

                    );
                    });
                }
                SelectedRoutine = SelectedRoutine;
            });
        }

        [RelayCommand]
        public void PickLocalPath()
        {
            if (SelectedRoutine is not LocalBackupRoutine lb) return;

            FolderBrowserDialog dialog = new() {
                Description = "Select a backup folder",
                ShowNewFolderButton = true,
            };

            if (dialog.ShowDialog() == DialogResult.OK) {
                lb.Path = dialog.SelectedPath;
            }
        }

        [RelayCommand]
        public void ReloadList()
        {
            RunCommand(() => {
                SelectedRoutine?.ReloadBackups();
            });
        }

        [RelayCommand]
        public void DeleteBackup(IBackup backup)
        {
            if (new PasswordConfirmDialog().ShowDialog() == false) return;
            RunCommand(() => {
                if (backup is LocalBackup lb) {
                    if (((LocalBackupRoutine)SelectedRoutine!).Delete(backup)) {
                        BackupsInFolderList.Remove(lb);
                    }
                }
                else {
                    SelectedRoutine?.Delete(backup);
                }
            });
        }

        [RelayCommand]
        public void RestoreBackup(IBackup backup)
        {
            if (new ConfirmDialog("Are you sure want to restore this backup?\nYou will need to restart orderly").ShowDialog() == false) return;
            RunCommand(() => {
                SelectedRoutine?.Restore(backup);
                App.Current.Dispatcher.Invoke(() => {
                    App.Current.Shutdown();
                    System.Windows.Forms.Application.Restart();
                });
            });
        }

        [RelayCommand]
        public void AuthGoogle()
        {
            RunCommand(() => {
                ((GoogleDriveRoutine)SelectedRoutine!).Authenticate();
            });
        }
    }
}
