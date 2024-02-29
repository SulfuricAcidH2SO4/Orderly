using Orderly.Interfaces;
using Orderly.Modules;
using Orderly.ViewModels.RadialMenu;
using System.Windows.Controls;
using System.Windows.Input;
using Wpf.Ui.Controls;
using TextBox = Wpf.Ui.Controls.TextBox;

namespace Orderly.Views.RadialMenu
{
    /// <summary>
    /// Interaction logic for RadialMenuView.xaml
    /// </summary>
    public partial class InputTerminalView : INavigableView<InputTerminalViewModel>
    {
        public InputTerminalViewModel ViewModel { get; private set; }

        private ProgramConfiguration config;
        public InputTerminalView(InputTerminalViewModel viewModel, IProgramConfiguration config)
        {
            ViewModel = viewModel;
            DataContext = this;
            this.config = (ProgramConfiguration?)config;
            InitializeComponent();
        }

        public void CloseMenu()
        {
            Hide();
        }

        public void OpenMenu()
        {
            ViewModel.UpdateCredentials();
            Show();
        }

        private void OnFocusLost(object sender, EventArgs e)
        {
            var window = (Window)sender;
            window.Topmost = true;
            CloseMenu();
        }

        private void Window_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var window = (Window)sender;
            window.Topmost = true;
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            ViewModel.FilterCredentials(((TextBox)sender).Text);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Focus();
            tbInput.Focus();
        }
    }
}
