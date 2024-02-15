using Orderly.ViewModels.RadialMenu;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Wpf.Ui.Controls;

namespace Orderly.Views.RadialMenu
{
    /// <summary>
    /// Interaction logic for RadialMenuView.xaml
    /// </summary>
    public partial class RadialMenuView : INavigableView<RadialMenuViewModel>
    {
        public RadialMenuViewModel ViewModel { get; private set; }
        public RadialMenuView(RadialMenuViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;
            InitializeComponent();
        }

        public void CloseMenu()
        {
            Storyboard yourStoryboard = (Storyboard)Resources["FadeOutTextBox"];
            yourStoryboard.Begin();
            Task.Factory.StartNew(() =>
            {
                ViewModel.IsMenuOpen = false;
                Thread.Sleep(350);
                Dispatcher.Invoke(() =>
                {
                    Hide();
                });
            });
        }

        public void OpenMenu()
        {
            ViewModel.IsMenuOpen = true;
            asBox.Opacity = 0;
            Storyboard yourStoryboard = (Storyboard)Resources["FadeInTextBox"];
            yourStoryboard.Begin();
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
    }
}
