using Orderly.ViewModels.Pages.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wpf.Ui.Controls;

namespace Orderly.Views.Pages.Tools
{
    /// <summary>
    /// Interaction logic for PwdBreachToolView.xaml
    /// </summary>
    public partial class PwdBreachToolView : INavigableView<PwdBreachToolViewModel>
    {
        public PwdBreachToolViewModel ViewModel { get; set; }

        public PwdBreachToolView(PwdBreachToolViewModel viewmodel)
        {
            ViewModel = viewmodel;
            DataContext = this;
            InitializeComponent();
        }
    }
}
