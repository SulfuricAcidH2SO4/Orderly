using Orderly.Modules;
using Orderly.Views.Windows;
using Wpf.Ui.Controls;

namespace Orderly.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for PasswordGeneratorDialog.xaml
    /// </summary>
    public partial class PasswordGeneratorDialog : FluentWindow
    {
        public string GeneratedPassword { get; private set; } = string.Empty;
        public PasswordGeneratorDialog()
        {
            Owner = MainWindow.Instance;
            InitializeComponent();
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            GeneratedPassword = pbPassword.Text;
            DialogResult = true;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void CopyPassword(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(GeneratedPassword);
            pbPassword.Focus();
            pbPassword.SelectAll();
        }

        private void UpdatePassword(object sender, RoutedEventArgs e)
        {
            if (cbUpper is null || cbSymbols is null || cbNumbers is null || slLength is null || pbPassword is null || tbStrength is null) return;
            bool upperCase = cbUpper.IsChecked!.Value;
            bool symbols = cbSymbols.IsChecked!.Value;
            bool numbers = cbNumbers.IsChecked!.Value;
            int length = (int)slLength.Value;

            GeneratedPassword = PasswordGenerator.GenerateSecurePassword(length, upperCase, numbers, symbols);
            pbPassword.Password = GeneratedPassword;

            int strength = PasswordStrengthChecker.CalculatePasswordStrength(GeneratedPassword);

            if (strength == 0) tbStrength.Text = "Weak";
            else if (strength == 1) tbStrength.Text = "Medium";
            else tbStrength.Text = "Strong";
        }

        private void slLength_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UpdatePassword(this, new());
        }
    }
}
