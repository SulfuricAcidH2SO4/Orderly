using Orderly.ViewModels.Pages;
using Wpf.Ui.Controls;

namespace Orderly.Views.Pages
{
    /// <summary>
    /// Interaction logic for TaskbarView.xaml
    /// </summary>
    public partial class TaskbarView : INavigableView<TaskbarViewModel>
    {
        public TaskbarViewModel ViewModel { get; set; } = new();
        public TaskbarView()
        {
            DataContext = this;
            InitializeComponent();
        }

    }
}
