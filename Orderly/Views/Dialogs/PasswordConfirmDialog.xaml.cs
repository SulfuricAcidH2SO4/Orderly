﻿using Orderly.DaVault;
using Orderly.Helpers;
using Orderly.Interfaces;
using Orderly.Modules;
using System.Diagnostics;
using System.Windows.Input;
using Wpf.Ui.Controls;

namespace Orderly.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for PasswordConfirmDialog.xaml
    /// </summary>
    public partial class PasswordConfirmDialog : FluentWindow
    {
        public string InsertedPassword { get; set; } = string.Empty;
        public ICommand ConfirmCommand { get; set; }
        public ProgramConfiguration Config { get; set; }

        public PasswordConfirmDialog()
        {
            KeyManager.PauseClickListener = true;
            ConfirmCommand = new RelayCommand(Confirm);
            DataContext = this;
            Config = (ProgramConfiguration)App.GetService<IProgramConfiguration>();
            InitializeComponent();
            tbError.Visibility = Visibility.Collapsed;
        }

        private void Confirm() => OnConfirmClick(this, new());

        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void OnConfirmClick(object sender, RoutedEventArgs e)
        {
            string password = pbPassword.Password.ToString();
            var stop = Stopwatch.StartNew();
            string hashPassword = EncryptionHelper.HashPassword(password);
            stop.Stop();

            InsertedPassword = password;

            if (hashPassword == Config.AbsolutePassword) {
                if (cbRemember.IsChecked == true) {
                    SessionControl.SavedPassword = password;
                }
                DialogResult = true;
                App.GetService<Vault>()!.PasswordEncryptionKey = EncryptionHelper.GetPasswordKey(password);
                Close();
            }
            else {
                tbError.Visibility = Visibility.Visible;
            }
        }

        private void FluentWindow_Loaded(object sender, RoutedEventArgs e)
        {

            pbPassword.Focus();
        }

        private void FluentWindow_ContentRendered(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(SessionControl.SavedPassword) && EncryptionHelper.HashPassword(SessionControl.SavedPassword) == Config.AbsolutePassword) {
                App.GetService<Vault>()!.PasswordEncryptionKey = EncryptionHelper.GetPasswordKey(SessionControl.SavedPassword);
                DialogResult = true;
                Close();
                return;
            }
        }

        private void FluentWindow_Closed(object sender, EventArgs e)
        {
            KeyManager.PauseClickListener = false;
        }
    }
}
