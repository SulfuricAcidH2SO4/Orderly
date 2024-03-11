using Microsoft.EntityFrameworkCore;
using Orderly.Database;
using Orderly.DaVault;
using Orderly.Extensions;
using Orderly.Helpers;
using Orderly.Models;
using Orderly.Views.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        string searchQuery = string.Empty;
        object searchLock = new object();

        public void OnNavigatedFrom()
        {
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
    }
}
