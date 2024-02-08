using Orderly.Views.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace Orderly.ViewModels.Pages
{
    public partial class TaskbarViewModel : ViewModelBase, INavigationAware
    {
        public void OnNavigatedFrom()
        {
        }

        public void OnNavigatedTo()
        {
        }

        [RelayCommand]
        public void OpenMainWindow()
        {
            MainWindow.Instance.Show();
        }

        [RelayCommand]
        public void CloseApp()
        {
            Application.Current.Shutdown();
        }
    }
}
