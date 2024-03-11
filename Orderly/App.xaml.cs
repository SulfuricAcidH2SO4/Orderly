﻿using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orderly.Backups;
using Orderly.Database;
using Orderly.DaVault;
using Orderly.Helpers;
using Orderly.Interfaces;
using Orderly.Models.Notifications;
using Orderly.Modules;
using Orderly.Modules.Notifications;
using Orderly.Services;
using Orderly.Update;
using Orderly.ViewModels.Pages;
using Orderly.ViewModels.Pages.Tools;
using Orderly.ViewModels.RadialMenu;
using Orderly.ViewModels.Windows;
using Orderly.ViewModels.Wizard;
using Orderly.Views.Dialogs;
using Orderly.Views.Pages;
using Orderly.Views.Pages.Tools;
using Orderly.Views.RadialMenu;
using Orderly.Views.Windows;
using Orderly.Views.Wizard;
using System.Diagnostics;
using System.IO;
using System.Windows.Media;
using System.Windows.Threading;
using Wpf.Ui;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace Orderly
{
    public partial class App
    {
        #region WinUI WPF
        static ProgramConfiguration config;
        private static readonly IHost _host = Host
            .CreateDefaultBuilder()
            .ConfigureAppConfiguration(c => { c.SetBasePath(AppDomain.CurrentDomain.BaseDirectory); })
            .ConfigureServices((context, services) => {
                services.AddHostedService<ApplicationHostService>();

                // Page resolver service
                services.AddSingleton<IPageService, PageService>();

                // Program Configuration
                Vault v = Vault.Initialize();
                services.AddSingleton(v);
                config = IProgramConfiguration.Load(v);
                services.AddSingleton<IProgramConfiguration>(config);

                // Theme manipulation
                services.AddSingleton<IThemeService, ThemeService>();

                services.AddSingleton<ISnackbarService, SnackbarService>();
                services.AddSingleton<NotificationService>();


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

                //Radial Menu
                services.AddSingleton<InputTerminalView>();
                services.AddSingleton<InputTerminalViewModel>();

                //Tools
                services.AddSingleton<PasswordGeneratorTool>();
                services.AddSingleton<PasswordGeneratorToolViewModel>();
                services.AddSingleton<PwdBreachToolView>();
                services.AddSingleton<PwdBreachToolViewModel>();

                //General
                services.AddSingleton<BackupPage>();
                services.AddSingleton<BackupViewModel>();
                services.AddSingleton<DashboardPage>();
                services.AddSingleton<DashboardViewModel>();
                services.AddSingleton<SettingsPage>();
                services.AddSingleton<SettingsViewModel>();
                services.AddSingleton<AboutView>();
                services.AddSingleton<NotificationView>();
                services.AddSingleton<NotificationViewModel>();
                services.AddSingleton<ToolsPage>();
                services.AddSingleton<ToolsPageViewModel>();

            }).Build();

        /// <summary>
        /// Gets registered service.
        /// </summary>
        /// <typeparam name="T">Type of the service to get.</typeparam>
        /// <returns>Instance of the service or <see langword="null"/>.</returns>
        public static T? GetService<T>()
            where T : class
        {
            if (_host == null) return null;
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
            ConfirmDialog dialog = new(e.Exception.Message, "Unhandled exception");
            dialog.ShowDialog();
        }
        #endregion

        protected override void OnStartup(StartupEventArgs e)
        {
            bool isAlreadyRunning = System.Diagnostics.Process.GetProcessesByName("Orderly").Count() > 1;
            if (isAlreadyRunning) {
                Shutdown();
                return;
            }
            RenderOptions.ProcessRenderMode = config.UseHardwareRendering ? System.Windows.Interop.RenderMode.Default : System.Windows.Interop.RenderMode.SoftwareOnly;
            SplashScreen sc = new("/Assets/SplashScreen.png");
            if (!config.StartMinimized) {
                sc.Show(false);
            }

            ApplicationThemeManager.Apply(App.GetService<IProgramConfiguration>().IsDarkMode ? ApplicationTheme.Dark : ApplicationTheme.Light);
            App.GetService<IThemeService>().SetAccent(Color.FromRgb(252, 120, 58));

            CheckBackupRestore();

            using DatabaseContext db = new();
            if (!db.Categories.Any()) {
                db.Categories.Add(new() {
                    Name = "General",
                });
                db.SaveChanges();
            }

            App.GetService<NotificationService>().Initialize();

            INavigationWindow window = GetService<INavigationWindow>();
            MainWindow = (Window)window;

            BackupWorker.CheckBackups();

            if (ProgramUpdater.CheckUpdate(out Version latestVersion, out string downloadUrl)) {
                NotificationService ns = GetService<NotificationService>();
                UserNotification notification = new() {
                    Header = $"Version {latestVersion} available!",
                    Body = $"Hey guess what?\nVersion {latestVersion} is now out! You should probably update...",
                    KeepBetweenSessions = false
                };
                notification.NotificationActions.Add(new() {
                    Description = "Update now!",
                    Procedure = () => {
                        if (new ConfirmDialog("You are about to update Ordely. Before closing the program all your backup routines will run a backup of your data.\nAre you sure you want to continue?").ShowDialog() == false) return;
                        ProgramUpdater.UpdateProgram(downloadUrl);
                    }
                });
                ns.Add(notification);
            }

            sc.Close(TimeSpan.FromSeconds(.5));

            if (CheckWizardLaunch()) {
                GetService<NotificationService>().Add(new() {
                    Header = "Welcome to Orderly!",
                    Body = $"Hi {GetService<IProgramConfiguration>().UserName}, welcome to orderly!\n" +
                    $"Let's start by making yourself familiar with the layout. The project is still in development " +
                    $"so for any bug report or suggestion please head to the GitHub repository.\n" +
                    $"Thanks again for being here!"
                });
                RenderOptions.ProcessRenderMode = config.UseHardwareRendering ? System.Windows.Interop.RenderMode.Default : System.Windows.Interop.RenderMode.SoftwareOnly;
                MainWindow.Show();
            }

            if (config.StartMinimized) {
                TaskbarIcon tb = (TaskbarIcon)FindResource("TaskBarIcon");
                tb.Visibility = Visibility.Visible;
                tb.DataContext = new TaskbarViewModel();
            }
            else {
                window.ShowWindow();
            }
            KeyManager.InitializeHook();
        }

        private bool CheckWizardLaunch()
        {
            if (!string.IsNullOrEmpty(config.AbsolutePassword)) return false;

            WizardMainWindow wizardWindow = GetService<WizardMainWindow>();
            if (wizardWindow.ShowDialog() == false) {
                Shutdown();
                return true;
            }
            return true;
        }

        private void CheckBackupRestore()
        {
            BackupWorker.CheckBackupRestore();
        }
    }
}
