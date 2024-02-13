using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orderly.Database;
using Orderly.Database.Entities;
using Orderly.DaVault;
using Orderly.Helpers;
using Orderly.Interfaces;
using Orderly.Modules;
using Orderly.Services;
using Orderly.ViewModels.Pages;
using Orderly.ViewModels.Windows;
using Orderly.ViewModels.Wizard;
using Orderly.Views.Dialogs;
using Orderly.Views.Pages;
using Orderly.Views.Windows;
using Orderly.Views.Wizard;
using System.IO;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Threading;
using Wpf.Ui;
using Wpf.Ui.Appearance;

namespace Orderly
{
    public partial class App
    {
        #region WinUI WPF
        static ProgramConfiguration config;
        private static readonly IHost _host = Host
            .CreateDefaultBuilder()
            .ConfigureAppConfiguration(c => { c.SetBasePath(Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)); })
            .ConfigureServices((context, services) => {
                services.AddHostedService<ApplicationHostService>();

                // Page resolver service
                services.AddSingleton<IPageService, PageService>();

                // Program Configuration
                Vault v = Vault.Initialize();
                services.AddSingleton(v);
                config = IProgramConfiguration.Load(v);
                v.PasswordEncryptionKey = EncryptionHelper.HashPassword(config.AbsolutePassword).Substring(0, 24);
                services.AddSingleton<IProgramConfiguration>(config);

                // Theme manipulation
                services.AddSingleton<IThemeService, ThemeService>();

                // TaskBar manipulation
                services.AddSingleton<ITaskBarService, TaskBarService>();

                // Service containing navigation, same as INavigationWindow... but without window
                services.AddSingleton<INavigationService, NavigationService>();

                // Main window with navigation
                services.AddSingleton<INavigationWindow, MainWindow>();
                services.AddSingleton<MainWindowViewModel>();

                //Dialogs
                services.AddSingleton<PasswordConfirmDialog>();

                //Wizard
                services.AddSingleton<WizardMainWindow>();
                services.AddSingleton<WizardMainWIndowViewModel>();
                
                //General
                services.AddSingleton<DashboardPage>();
                services.AddSingleton<DashboardViewModel>();
                services.AddSingleton<SettingsPage>();
                services.AddSingleton<SettingsViewModel>();
                services.AddSingleton<AboutView>();
            }).Build();

        /// <summary>
        /// Gets registered service.
        /// </summary>
        /// <typeparam name="T">Type of the service to get.</typeparam>
        /// <returns>Instance of the service or <see langword="null"/>.</returns>
        public static T GetService<T>()
            where T : class
        {
            return _host.Services.GetService(typeof(T)) as T;
        }

        /// <summary>
        /// Occurs when the application is loading.
        /// </summary>
        private void OnStartup(object sender, StartupEventArgs e)
        {
            _host.Start();
        }

        /// <summary>
        /// Occurs when the application is closing.
        /// </summary>
        private async void OnExit(object sender, ExitEventArgs e)
        {
            await _host.StopAsync();

            _host.Dispose();
        }

        /// <summary>
        /// Occurs when an exception is thrown by an application but not handled.
        /// </summary>
        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // For more info see https://docs.microsoft.com/en-us/dotnet/api/system.windows.application.dispatcherunhandledexception?view=windowsdesktop-6.0
        }
        #endregion

        protected override void OnStartup(StartupEventArgs e)
        {
            SplashScreen sc = new("/Assets/SplashScreen.png");
            if (!config.StartMinimized) {
                sc.Show(false);
            }

            DatabaseContext db = new();
            if (!db.Categories.Any())
            {
                db.Categories.Add(new()
                {
                    Name = "General",
                });
                db.SaveChanges();
            }

            INavigationWindow window = GetService<INavigationWindow>();
            MainWindow = (Window)window;

            sc.Close(TimeSpan.FromSeconds(.5));

            if (config.StartMinimized) {
                TaskbarIcon tb = (TaskbarIcon)FindResource("TaskBarIcon");
                tb.Visibility = Visibility.Visible;
                tb.DataContext = new TaskbarViewModel();
            }
            else {
                window.ShowWindow();
            }
        }
    }
}
