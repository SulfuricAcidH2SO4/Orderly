using Orderly.EE;
using Orderly.Interfaces;
using Orderly.Modules;
using System.Collections.ObjectModel;
using Wpf.Ui.Controls;

namespace Orderly.ViewModels.Windows
{
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _applicationTitle = $"Orderly - {EEManager.GetRandomPhrase()}";
        [ObservableProperty]
        private ProgramConfiguration config;

        public MainWindowViewModel(IProgramConfiguration config)
        {
            Config = (ProgramConfiguration)config;
        }
    }
}
