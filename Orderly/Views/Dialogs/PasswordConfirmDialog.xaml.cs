using Orderly.Helpers;
using Orderly.Interfaces;
using Orderly.Modules;
using Orderly.Views.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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
    /// Interaction logic for PasswordConfirmDialog.xaml
    /// </summary>
    public partial class PasswordConfirmDialog : FluentWindow
    {
        public ICommand ConfirmCommand { get; set; }
        public ProgramConfiguration Config { get; set; }

        public PasswordConfirmDialog()
        {
            Owner = MainWindow.Instance;
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
            

            if(hashPassword == Config.AbsolutePassword) {
                DialogResult = true;
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
    }
}
