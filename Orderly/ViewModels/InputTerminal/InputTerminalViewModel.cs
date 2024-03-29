﻿using Microsoft.EntityFrameworkCore;
using Orderly.Database;
using Orderly.Database.Entities;
using Orderly.DaVault;
using Orderly.Extensions;
using Orderly.Helpers;
using Orderly.Interfaces;
using Orderly.Modules;
using Orderly.ViewModels.Pages;
using Orderly.Views.Dialogs;
using Orderly.Views.RadialMenu;

namespace Orderly.ViewModels.RadialMenu
{
    public partial class InputTerminalViewModel : ViewModelBase
    {
        private ProgramConfiguration config;

        [ObservableProperty]
        private ExtendedObservableCollection<Credential> credentials = new();

        public InputTerminalViewModel(IProgramConfiguration configuration)
        {
            config = (ProgramConfiguration?)configuration!;
            using DatabaseContext db = new();
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
            if(config.TerminalInputBehaviour == Models.TerminalInputBehaviour.Insert) KeyManager.SendTextAt(username, KeyManager.lastOpenedPoint);
            else Clipboard.SetText(username);
        }

        [RelayCommand]
        private void ProcessPassword(string password)
        {
            PasswordConfirmDialog pwDialog = new();

            if (pwDialog.ShowDialog() == false) return;

            Vault v = App.GetService<Vault>();
            string decryptedPassw = EncryptionHelper.DecryptString(password, v.PasswordEncryptionKey);

            App.GetService<InputTerminalView>().CloseMenu();
            if (config.TerminalInputBehaviour == Models.TerminalInputBehaviour.Insert) KeyManager.SendTextAt(decryptedPassw, KeyManager.lastOpenedPoint);
            else Clipboard.SetText(decryptedPassw);
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
                            .ForEach(x => x.IsVisibile = true);

                OrderPinnedTop();
            });
        }

        public void OrderPinnedTop()
        {
            var topCredentials = Credentials.Where(x => x.Pinned).ToList();
            var normalCredentials = Credentials.Where(x => !x.Pinned).ToList();

            Credentials.Clear();
            Credentials.AddRange(topCredentials);
            Credentials.AddRange(normalCredentials);
        }

        public void UpdateCredentials()
        {
            Task.Factory.StartNew(() => {
                using DatabaseContext db = new();
                Credentials.Clear();
                Credentials.AddRange(db.Credentials.Include(c => c.Category).ToList());
                Credentials.ForEach(x => x.PropertyChanged += OnCredentialPropertyChanged);
                OrderPinnedTop();
            });
        }

        private void OnCredentialPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Task.Factory.StartNew(() => {
                if (sender is Credential cr) {
                    if (e.PropertyName == nameof(cr.Pinned)) {
                        OrderPinnedTop();
                        using DatabaseContext db = new();
                        db.Credentials.Update(cr);
                        db.SaveChanges();

                        DashboardViewModel dbvm = App.GetService<DashboardViewModel>();
                        foreach (var category in dbvm.Categories) {
                            Credential c = category.Credentials!.FirstOrDefault(x => x.Id == cr.Id)!;
                            if (c == null) continue;

                            c.Pinned = cr.Pinned;
                            return;
                        }
                    }
                }
            });
        }
    }
}
