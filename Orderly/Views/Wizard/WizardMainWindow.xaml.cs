using Orderly.ViewModels.Wizard;
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
using System.Windows.Shapes;
using Wpf.Ui.Controls;

namespace Orderly.Views.Wizard
{
    /// <summary>
    /// Interaction logic for WizardMainWindow.xaml
    /// </summary>
    public partial class WizardMainWindow : INavigableView<WizardMainWIndowViewModel>
    {
        public WizardMainWIndowViewModel ViewModel { get; set; }

        public WizardMainWindow(WizardMainWIndowViewModel viewModel)
        {   
            ViewModel = viewModel;
            InitializeComponent();
            DataContext = this;
        }
    }
}
