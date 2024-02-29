using Orderly.Database.Entities;
using Orderly.EE;
using Orderly.Interfaces;
using Orderly.ViewModels.Pages;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Wpf.Ui;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;
using TextBlock = System.Windows.Controls.TextBlock;
using TextBox = Wpf.Ui.Controls.TextBox;

namespace Orderly.Views.Pages
{
    public partial class DashboardPage : INavigableView<DashboardViewModel>
    {
        private bool dragDeadZoneTime; 

        public DashboardViewModel ViewModel { get; }

        public DashboardPage(DashboardViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();
            ApplicationThemeManager.Apply(App.GetService<IProgramConfiguration>().IsDarkMode ? ApplicationTheme.Dark : ApplicationTheme.Light);
            App.GetService<IThemeService>().SetAccent(Color.FromRgb(252, 120, 58));
        }

        private void Popup_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void Popup_Closed(object sender, EventArgs e)
        {
            if (sender is not Popup pop || pop.DataContext is not Category) return;
            ViewModel.UpdateCategory((Category)pop.DataContext);
        }

        private void OnTextSearchChanged(object sender, TextChangedEventArgs e)
        {
            string textQuery = (sender as TextBox).Text;
            ViewModel.SortList(textQuery, false);
        }

        private void CredentialMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            dragDeadZoneTime = true;
            Task.Factory.StartNew(() => {
                Thread.Sleep(200);
                if (dragDeadZoneTime) {
                    
                    Dispatcher.Invoke(() => {
                        ((Expander)sender).Visibility = Visibility.Hidden;
                        DragDrop.DoDragDrop(sender as FrameworkElement, ((FrameworkElement)sender).DataContext, DragDropEffects.Move);
                        ((Expander)sender).Visibility = Visibility.Visible;
                        dragDeadZoneTime = false;
                    });
                }
            });
        }

        private void CredentialMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            dragDeadZoneTime = false;
        }


        private void CardExpander_DragEnter(object sender, DragEventArgs e)
        {
            if (sender is CardExpander ex)
            {
                ex.BorderBrush = new SolidColorBrush(Colors.Cyan);
            }
        }

        private void CardExpander_DragLeave(object sender, DragEventArgs e)
        {
            if (sender is CardExpander ex)
            {
                ex.BorderBrush = new SolidColorBrush(Colors.Transparent);
            }
        }

        private void Expander_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            
        }

        private void CardExpander_Drop(object sender, DragEventArgs e)
        {
            if (sender is CardExpander ex)
            {
                ex.BorderBrush = new SolidColorBrush(Colors.Transparent);
                Credential? data = (Credential)e.Data.GetData(typeof(Credential));

                if(data is not null)
                {
                    ViewModel.MoveCredentialCategory(data, (Category)((FrameworkElement)sender).DataContext);
                }
            }
        }
    }
}
