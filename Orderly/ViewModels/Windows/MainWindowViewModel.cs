using Orderly.EE;
using Orderly.Interfaces;
using Orderly.Modules;
using Orderly.Modules.Notifications;
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
        [ObservableProperty]
        private NotificationService notificationService;

        public MainWindowViewModel(IProgramConfiguration config, NotificationService notificationService)
        {
            Config = (ProgramConfiguration)config;
            NotificationService = notificationService;
        }
    }
}
