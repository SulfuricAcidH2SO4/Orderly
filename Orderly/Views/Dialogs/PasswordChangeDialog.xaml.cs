using Orderly.Backups;
using Orderly.Database;
using Orderly.DaVault;
using Orderly.Helpers;
using Orderly.Interfaces;
using Orderly.Modules;
using Orderly.Views.Windows;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Wpf.Ui.Controls;

namespace Orderly.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for PasswordChangeDialog.xaml
    /// </summary>
    public partial class PasswordChangeDialog : FluentWindow
    {
        private string clearPassword = string.Empty;
        public ICommand ConfirmCommand { get; set; }

        ProgramConfiguration config;

        public PasswordChangeDialog(IProgramConfiguration config)
        {
            Owner = MainWindow.Instance;
            this.config = (ProgramConfiguration?)config!;
            ConfirmCommand = new RelayCommand(Confirm);
            InitializeComponent();
            DataContext = this;

        }

        private void Confirm() => btnConfirm_Click(this, new());

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            string pass1 = pbPassword.Password;
            string pass2 = pbConfirmPassword.Password;

            if (pass1 != pass2) {
                tbError.Visibility = Visibility.Visible;
                tbError.Text = "Your passwords don't match!";
                return;
            }

            string pattern = @"^(?=.*[0-9])(?=.*[^a-zA-Z0-9]).{6,}$";

            if (!Regex.IsMatch(pass1, pattern)) {
                tbError.Visibility = Visibility.Visible;
                tbError.Text = "You did not meet the password requirements";
                return;
            }

            config.AbsolutePassword = EncryptionHelper.HashPassword(pass1);
            clearPassword = pass1;
            UpdatePassword();
            bool runNewBackup = cbBackup.IsChecked!.Value;
            Task.Factory.StartNew(() => {
                BackupWorker.ClearAllBackups();
                if (runNewBackup) BackupWorker.RunAllBackups();
            });
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void UpdatePassword()
        {
            string oldKey = App.GetService<Vault>().PasswordEncryptionKey;
            string newKey = EncryptionHelper.GetPasswordKey(clearPassword);
            using DatabaseContext db = new();
            foreach (var credential in db.Credentials) {
                string plainPassword = EncryptionHelper.DecryptString(credential.Password, oldKey);
                credential.Password = EncryptionHelper.EncryptString(plainPassword, newKey);
                db.Credentials.Update(credential);
            }
            db.SaveChanges();

            App.GetService<Vault>().PasswordEncryptionKey = newKey;

            config.Save();
        }
    }
}
