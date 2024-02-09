using Hardcodet.Wpf.TaskbarNotification;
using Orderly.Views.Pages;
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
    public partial class TaskbarViewModel : ViewModelBase
    {
        TaskbarIcon tb;

        [RelayCommand]
        public void OpenMainWindow()
        {
            MainWindow.Instance.ShowWindowAgain();

            if(tb == null) tb = (TaskbarIcon)Application.Current.FindResource("TaskBarIcon");
            tb.TrayPopupResolved.IsOpen = false;
        }

        [RelayCommand]
        public void CloseApp()
        {
            Application.Current.Shutdown();

            if (tb == null) tb = (TaskbarIcon)Application.Current.FindResource("TaskBarIcon");
            tb.TrayPopupResolved.IsOpen = false;
        }

        [RelayCommand]
        public void OpenSetting()
        {
            MainWindow.Instance.ShowWindowAgain();
            MainWindow.Instance.Navigate(typeof(SettingsPage));

            if (tb == null) tb = (TaskbarIcon)Application.Current.FindResource("TaskBarIcon");
            tb.TrayPopupResolved.IsOpen = false;
        }
    }
}
