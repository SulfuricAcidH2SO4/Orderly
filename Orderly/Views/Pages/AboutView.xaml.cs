using System.Diagnostics;
using System.Reflection;
using System.Windows.Controls;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace Orderly.Views.Pages
{
    /// <summary>
    /// Interaction logic for AboutView.xaml
    /// </summary>
    public partial class AboutView : UserControl
    {
        public AboutView()
        {
            InitializeComponent();
            Version ver = Assembly.GetExecutingAssembly().GetName().Version!;
            tbVersion.Text = $"Ver. {ver.Major}.{ver.Minor}.{ver.Build}";
        }

        private void OnKofiClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            OpenUrl("https://ko-fi.com/sulfuricacidh2s04");
        }

        void OpenUrl(string url)
        {
            try {
                Process.Start(new ProcessStartInfo {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch (Exception ex) {
            }
        }

        private void OnPaypalClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            OpenUrl("https://www.paypal.com/donate/?hosted_button_id=7TCNVYSU58NZC");
        }

        private void CopyWallet(object sender, RoutedEventArgs e)
        {
            tbWallet.Focus();
            tbWallet.SelectAll();
            Clipboard.SetText(tbWallet.Text);
            App.GetService<ISnackbarService>().Show("Wallet copied", "Wallet copied successfully!", Wpf.Ui.Controls.ControlAppearance.Primary, new SymbolIcon(SymbolRegular.Checkmark28), TimeSpan.FromSeconds(3));
        }
    }
}
