using Orderly.Models;
using Orderly.Modules;
using Orderly.Views.Windows;
using Wpf.Ui.Controls;

namespace Orderly.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for ChangeInputOptionsDialog.xaml
    /// </summary>
    public partial class ChangeInputOptionsDialog : FluentWindow
    {
        public InputOptions InputOptions { get; set; } = new();

        public ChangeInputOptionsDialog()
        {
            Owner = MainWindow.Instance;
            DataContext = this;
            InitializeComponent();
            KeyManager.PauseMenuListener = true;
            KeyManager.Hook!.KeyDown += OnKeyDown;
        }

        private void OnKeyDown(object? sender, System.Windows.Forms.KeyEventArgs e)
        {
            InputOptions.UseCtrl = e.Control;
            InputOptions.UseAlt = e.Alt;
            InputOptions.UseShift = e.Shift;
            InputOptions.KeyCode = e.KeyCode;
            tbInput.Text = InputOptions.ToString();
        }

        private void OnConfirm(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void OnClosed(object sender, EventArgs e)
        {
            KeyManager.Hook!.KeyDown -= OnKeyDown;
            KeyManager.PauseMenuListener = false;
        }
    }
}
