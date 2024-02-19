using Microsoft.EntityFrameworkCore;
using Orderly.Database;
using Orderly.Database.Entities;
using Orderly.DaVault;
using Orderly.Extensions;
using Orderly.Helpers;
using Orderly.Modules;
using Orderly.Views.Dialogs;
using Orderly.Views.RadialMenu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Wpf.Ui.Controls;

namespace Orderly.ViewModels.RadialMenu
{
    public partial class InputTerminalViewModel : ViewModelBase
    {
        [ObservableProperty]
        private ExtendedObservableCollection<Credential> credentials = new();

        DatabaseContext? db;

        public InputTerminalViewModel()
        {
            db = new();
            Credentials.AddRange(db.Credentials.Include(c => c.Category).ToList());
        }

        [RelayCommand]
        private void CloseMenu()
        {
            InputTerminalView menu = App.GetService<InputTerminalView>();
            menu.CloseMenu();
        }

        [RelayCommand]
        private void ProcessUsername(string username)
        {
            App.GetService<InputTerminalView>().CloseMenu();
            KeyManager.SendTextAt(username, KeyManager.lastOpenedPoint);
        }

        [RelayCommand]
        private void ProcessPassword(string password)
        {
            PasswordConfirmDialog pwDialog = new();

            if (pwDialog.ShowDialog() == false) return;

            Vault v = App.GetService<Vault>();
            string decryptedPassw = EncryptionHelper.DecryptString(password, v.PasswordEncryptionKey);

            App.GetService<InputTerminalView>().CloseMenu();
            KeyManager.SendTextAt(decryptedPassw, KeyManager.lastOpenedPoint);
        }

        public void FilterCredentials(string name)
        {
            Task.Factory.StartNew(() => {
                if (string.IsNullOrEmpty(name)) {
                    OrderPinnedTop();
                    Credentials.ForEach(x => x.IsVisibile = true);
                    return;
                }

                Credentials.ForEach(x => x.IsVisibile = false);

                Credentials.Where(c => c.ServiceName.ToLower().Contains(name.ToLower()) || c.Username.ToLower().Contains(name.ToLower()))
                            .ForEach(x => x.IsVisibile = true );

                OrderPinnedTop();
            });
        }

        private void OrderPinnedTop()
        {
            var topCredentials = Credentials.Where(x => x.Pinned).ToList();
            var normalCredentials = Credentials.Where(x => !x.Pinned).ToList();

            Credentials.Clear();
            Credentials.AddRange(topCredentials);
            Credentials.AddRange(normalCredentials);
        }
    }
}
