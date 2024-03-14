using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Toolkit.Uwp.Notifications;
using Orderly.Interfaces;
using Orderly.Modules.Notifications;
using Orderly.ViewModels.Pages;
using Orderly.ViewModels.Windows;
using Orderly.Views.Pages;
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
            INavigationService navigationService,
            ISnackbarService snackBarService,
            NotificationService notificationService
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
            snackBarService.SetSnackbarPresenter(SnackbarPresenter);
            notificationService.SetContentControl(NotificationViewControl);
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
            if (Configuration.CloseButtonClosesApp) {
                Application.Current.Shutdown();
                return;
            }
            else {
                e.Cancel = true;
                ShowTaskbarIcon();
            }
        }

        private void ShowTaskbarIcon()
        {
            Hide();
            TaskbarIcon tb = (TaskbarIcon)FindResource("TaskBarIcon");
            tb.Visibility = Visibility.Visible;
            tb.DataContext = new TaskbarViewModel();

            if (Configuration.ShowMinimizeNotification) {
                new ToastContentBuilder()
                    .AddText("Orderly has been minimized!")
                    .AddAttributionText("You can find it again in your system tray!")
                    .Show();
            }
        }

        private void FluentWindow_Loaded(object sender, RoutedEventArgs e)
        {
             Navigate(typeof(DashboardPage));
        }
    }
}
