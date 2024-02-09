// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Toolkit.Uwp.Notifications;
using Newtonsoft.Json.Linq;
using Orderly.Interfaces;
using Orderly.Modules;
using Orderly.ViewModels.Pages;
using Orderly.ViewModels.Windows;
using Wpf.Ui;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace Orderly.Views.Windows
{
    public partial class MainWindow : INavigationWindow
    {
        public MainWindowViewModel ViewModel { get; }
        public static MainWindow? Instance { get; private set; }

        private IProgramConfiguration Configuration { get; set; }
        public MainWindow(
            MainWindowViewModel viewModel,
            IPageService pageService,
            IProgramConfiguration config,
            INavigationService navigationService
        )
        {
            ViewModel = viewModel;
            DataContext = this;
            Instance = this;

            Configuration = config!;
            
            SystemThemeWatcher.Watch(this);

            InitializeComponent();
            SetPageService(pageService);

            navigationService.SetNavigationControl(RootNavigation);
        }

        #region INavigationWindow methods

        public INavigationView GetNavigation() => RootNavigation;

        public bool Navigate(Type pageType) => RootNavigation.Navigate(pageType);

        public void SetPageService(IPageService pageService) => RootNavigation.SetPageService(pageService);

        public void ShowWindow() => Show();

        public void CloseWindow() => Close();

        #endregion INavigationWindow methods

        /// <summary>
        /// Raises the closed event.
        /// </summary>
        protected override void OnClosed(EventArgs e)
        {
            Instance = null;
            base.OnClosed(e);
        }

        INavigationView INavigationWindow.GetNavigation()
        {
            throw new NotImplementedException();
        }

        public void SetServiceProvider(IServiceProvider serviceProvider)
        {
            throw new NotImplementedException();
        }
        public void ShowWindowAgain()
        {
            Show();
            TaskbarIcon tb = (TaskbarIcon)FindResource("TaskBarIcon");
            tb.Visibility = Visibility.Collapsed;
            tb.DataContext = new TaskbarViewModel();
        }

        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
            TaskbarIcon tb = (TaskbarIcon)FindResource("TaskBarIcon");
            tb.Visibility = Visibility.Visible;
            tb.DataContext = new TaskbarViewModel();

            if (Configuration.ShowMinimizeNotification)
            {
                new ToastContentBuilder()
                    .AddText("Orderly has been minimized!")
                    .Show();
            }
        }

        private void FluentWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (Configuration.IsDarkMode) ApplicationThemeManager.Apply(ApplicationTheme.Dark);
            else ApplicationThemeManager.Apply(ApplicationTheme.Light);
        }
    }
}
