using Orderly.Views.Windows;
using System.Windows.Input;
using Wpf.Ui.Controls;

namespace Orderly.Views.Dialogs
{
    public partial class ConfirmDialog : FluentWindow
    {
        public string Header { get; set; }
        public string TextContent { get; set; }
        public ICommand ConfirmCommand { get; set; }

        public ConfirmDialog(string content, string header = "Confirmation required")
        {
            ConfirmCommand = new RelayCommand(Confirm);
            Owner = MainWindow.Instance;
            TextContent = content;
            Header = header;
            DataContext = this;
            InitializeComponent();
        }

        private void Confirm() => btnConfirm_Click(this, new());

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
