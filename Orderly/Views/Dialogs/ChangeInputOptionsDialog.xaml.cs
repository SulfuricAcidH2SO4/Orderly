using Orderly.Models;
using Orderly.Modules;
using Orderly.Views.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
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
            KeyListener.PauseMenuListener = true;
            KeyListener.Hook!.KeyDown += OnKeyDown;
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
            KeyListener.Hook!.KeyDown -= OnKeyDown;
            KeyListener.PauseMenuListener = false;
        }
    }
}
