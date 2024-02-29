using System.Reflection;
using System.Windows.Controls;

namespace Orderly.Views.Pages
{
    /// <summary>
    /// Interaction logic for AboutView.xaml
    /// </summary>
    public partial class AboutView : UserControl
    {
        public AboutView()
        {
            InitializeComponent();
            Version ver = Assembly.GetExecutingAssembly().GetName().Version!;
            tbVersion.Text = $"Ver. {ver.Major}.{ver.Minor}.{ver.Build}";
        }
    }
}
