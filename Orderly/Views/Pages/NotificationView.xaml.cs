using Orderly.ViewModels.Pages;
using Wpf.Ui.Controls;

namespace Orderly.Views.Pages
{
    /// <summary>
    /// Interaction logic for NotificationView.xaml
    /// </summary>
    public partial class NotificationView : INavigableView<NotificationViewModel>
    {
        public NotificationViewModel ViewModel { get; set; }

        public NotificationView(NotificationViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;
            InitializeComponent();
        }
    }
}
