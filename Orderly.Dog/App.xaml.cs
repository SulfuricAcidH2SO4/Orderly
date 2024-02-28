using System.Configuration;
using System.Data;
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

            if (e.Args[0] != "update") {
                Shutdown();
                return;
            }

            MainWindow = new MainWindow(e.Args[1]);

            MainWindow.Show();
        }
    }

}
