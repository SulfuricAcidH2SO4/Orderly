using Orderly.EE;
using System.Collections.ObjectModel;
using Wpf.Ui.Controls;

namespace Orderly.ViewModels.Windows
{
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _applicationTitle = $"Orderly - {EEManager.GetRandomPhrase()}";

        public MainWindowViewModel()
        {
        }
    }
}
