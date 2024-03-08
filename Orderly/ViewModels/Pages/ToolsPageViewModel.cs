using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wpf.Ui;
using Wpf.Ui.Extensions;

namespace Orderly.ViewModels.Pages
{
    public partial class ToolsPageViewModel : ViewModelBase
    {
        INavigationService navigationService;

        public ToolsPageViewModel(INavigationService navigationService)
        {
            this.navigationService = navigationService;
        }

        [RelayCommand]
        public void NavigateTool(Type toolView)
        {
            navigationService.NavigateWithHierarchy(toolView);
        }
    }
}
