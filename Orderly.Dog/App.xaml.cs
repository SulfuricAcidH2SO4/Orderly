using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

namespace Orderly.Dog
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            RenderOptions.ProcessRenderMode = System.Windows.Interop.RenderMode.SoftwareOnly;

            if (e.Args.Length < 2) {
                Shutdown();
                return;
            }

            string action = e.Args[0];

            switch (action) {
                case "update":
                    MainWindow = new MainWindow(e.Args[1]);
                    MainWindow.Show();
                    break;
                case "add-start":
                    StartManager.AddToWindowsStart(e.Args[1]);
                    Shutdown();
                    break;
                case "add-startup":
                    StartManager.AddToStartup(e.Args[1], e.Args[2]);
                    Shutdown();
                    break;
                case "remove-startup":
                    StartManager.RemoveFromStartup(e.Args[1]);
                    Shutdown();
                    break;
                default:
                    Shutdown();
                    break;
            }
           
        }
    }

}
