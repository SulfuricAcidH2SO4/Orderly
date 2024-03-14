using Microsoft.EntityFrameworkCore;
using Orderly.Database;
using Orderly.Database.Entities;
using Orderly.DaVault;
using Orderly.Extensions;
using Orderly.Helpers;
using Orderly.Models;
using Orderly.Modules.Tools;
using Orderly.Views.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace Orderly.ViewModels.Pages.Tools
{
    public partial class PwdBreachToolViewModel : ViewModelBase, INavigationAware
    {
        public ExtendedObservableCollection<PwdBreachPasswordItem> Passwords { get; set; } = new();
        public string SearchQuery
        {
            get => searchQuery;
            set
            {
                SetProperty(ref searchQuery, value);
                UpdateSearch(value);
            }
        }

        [ObservableProperty]
        string inputPassword = string.Empty;
        [ObservableProperty]
        bool isCheckFound;
        [ObservableProperty]
        bool isNoCheckFound;
        [ObservableProperty]
        bool isCheckingCredentials;
        [ObservableProperty]
        int occurrences;
        [ObservableProperty]
        string progressText = string.Empty;

        string searchQuery = string.Empty;
        object searchLock = new object();

        public void OnNavigatedFrom()
        {
            IsCheckingCredentials = false;
        }

        public void OnNavigatedTo()
        {
            RunCommand(() => {
                Passwords.Clear();
                using DatabaseContext db = new();
                {
                    var categories = db.Categories.Include(c => c.Credentials);
                    foreach (var category in categories) {
                        category.Credentials?.ForEach(x => {
                            x.Category = category;
                            Passwords.Add(new() {
                                Credential = x,
                                Status = CredentialBreachStatus.Unknown,
                            });
                        });
                    }
                }
            });
        }

        public void UpdateSearch(string search)
        {
            Task.Factory.StartNew(() => {
                lock (searchLock) {
                    if (string.IsNullOrEmpty(search)) {
                        Passwords.ForEach(x => x.IsVisible = true);
                        return;
                    }
                    Passwords.ForEach(x => x.IsVisible = false);
                    Passwords.Where(x => x.Credential
                                        .ServiceName.ToLower()
                                        .Contains(search.ToLower())
                                        || x.Credential.Category.Name.ToLower()
                                        .Contains(search.ToLower()))
                    .ForEach(x => x.IsVisible = true);
                }
            });
        }

        [RelayCommand]
        public void ChangePasswordVisibility(PwdBreachPasswordItem credential)
        {
            if (credential == null) return;

            if (new PasswordConfirmDialog().ShowDialog() == false) return;
            credential.IsPasswordVisibile = !credential.IsPasswordVisibile;
            if (credential.IsPasswordVisibile) 
                credential.Credential!.Password = EncryptionHelper.DecryptString(credential.Credential.Password, App.GetService<Vault>()!.PasswordEncryptionKey);
            else credential.Credential!.Password = EncryptionHelper.EncryptString(credential.Credential.Password, App.GetService<Vault>()!.PasswordEncryptionKey);
        }

        [RelayCommand]
        public void CheckPasswordBreach(string password)
        {
            if (string.IsNullOrEmpty(password)) return;
            RunCommand(() => {
                int result = PwndClient.CheckPasswordBreach(password);
                if (result < 0) App.GetService<ISnackbarService>()!.Show("Something went wrong...",
                                                                        "Something went wrong during the API execution...",
                                                                        ControlAppearance.Danger,
                                                                        new SymbolIcon(SymbolRegular.EmojiSad24),
                                                                        TimeSpan.FromSeconds(4));
                else if(result > 0) {
                    IsCheckFound = true;
                    IsNoCheckFound = !IsCheckFound;
                    Occurrences = result;
                }
                else {
                    IsCheckFound = false;
                    IsNoCheckFound = !IsCheckFound;
                    Occurrences = 0;
                }
            });
        }

        [RelayCommand]
        public void CheckUserCredential(PwdBreachPasswordItem credential)
        {
            if (new PasswordConfirmDialog().ShowDialog() == false) return;

            RunCommand(() => {
               CheckCredential(credential);
            });
        }

        [RelayCommand]
        public void CheckAllCredentials()
        {
            if (new PasswordConfirmDialog().ShowDialog() == false) return;
            Passwords.ForEach(p => p.Status = CredentialBreachStatus.Unknown);
            IsCheckingCredentials = true;
            Task.Factory.StartNew(() => {
                int i = 0;
                ProgressText = $"{i} / {Passwords.Count}";
                foreach (var credential in Passwords) {
                    if (!IsCheckingCredentials) return;
                    Thread.Sleep(500);
                    CheckCredential(credential);
                    i++;
                    ProgressText = $"{i} / {Passwords.Count}";
                }
                IsCheckingCredentials = false;
            });
        }

        private void CheckCredential(PwdBreachPasswordItem credential)
        {
            string passToCheck;
            if (!credential.IsPasswordVisibile) passToCheck = EncryptionHelper.DecryptString(credential.Credential!.Password, App.GetService<Vault>()!.PasswordEncryptionKey);
            else passToCheck = credential.Credential!.Password;
            int result = PwndClient.CheckPasswordBreach(passToCheck);
            if (result < 0) App.GetService<ISnackbarService>()!.Show("Something went wrong...",
                                                                    "Something went wrong during the API execution...",
                                                                    ControlAppearance.Danger,
                                                                    new SymbolIcon(SymbolRegular.EmojiSad24),
                                                                    TimeSpan.FromSeconds(4));
            else if (result > 0) {
                credential.Occurrences = result;
                credential.Status = CredentialBreachStatus.Breached;
            }
            else {
                credential.Occurrences = 0;
                credential.Status = CredentialBreachStatus.Safe;
            }
        }
    
    }
}
