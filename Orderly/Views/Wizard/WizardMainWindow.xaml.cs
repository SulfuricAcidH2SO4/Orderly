using Orderly.ViewModels.Wizard;
using Wpf.Ui.Controls;

namespace Orderly.Views.Wizard
{
    /// <summary>
    /// Interaction logic for WizardMainWindow.xaml
    /// </summary>
    public partial class WizardMainWindow : INavigableView<WizardMainWIndowViewModel>
    {
        public WizardMainWIndowViewModel ViewModel { get; set; }

        private bool wizardComplete;

        public WizardMainWindow(WizardMainWIndowViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
            DataContext = this;
        }

        public void CLoseWizard()
        {
            wizardComplete = true;
            Close();
        }

        private void OnWizardClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (wizardComplete) DialogResult = true;
            else DialogResult = false;
        }
    }
}
