using Orderly.Database;
using Orderly.DaVault;
using Orderly.Helpers;
using Orderly.Interfaces;
using Orderly.Modules;
using Orderly.ViewModels;
using Orderly.ViewModels.Pages;
using Orderly.Views.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Wpf.Ui.Controls;

namespace Orderly.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for PasswordChangeDialog.xaml
    /// </summary>
    public partial class PasswordChangeDialog : FluentWindow
    {
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

            if(pass1 != pass2) {
                tbError.Visibility = Visibility.Visible;
                tbError.Text = "Your passwords don't match!";
                return;
            }

            string pattern = @"^(?=.*[0-9])(?=.*[^a-zA-Z0-9]).{6,}$";

            if(!Regex.IsMatch(pass1, pattern)) {
                tbError.Visibility = Visibility.Visible;
                tbError.Text = "You did not meet the password requirements";
                return;
            }

            config.AbsolutePassword = EncryptionHelper.HashPassword(pass1);
            UpdatePassword();
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
            string newKey = EncryptionHelper.HashPassword(config.AbsolutePassword).Substring(0, 24);

            DatabaseContext db = new();
            foreach (var credential in db.Credentials) {
                string plainPassword = EncryptionHelper.DecryptString(credential.Password, oldKey);
                credential.Password = EncryptionHelper.EncryptString(plainPassword, newKey);
                db.Credentials.Update(credential);
            }
            db.SaveChanges();
            App.GetService<Vault>().PasswordEncryptionKey = newKey;
            //App.GetService<DashboardViewModel>().Initalize();

            config.Save();
        }
    }
}
