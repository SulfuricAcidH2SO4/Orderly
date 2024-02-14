using Orderly.Views.RadialMenu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wpf.Ui.Controls;

namespace Orderly.ViewModels.RadialMenu
{
    public partial class RadialMenuViewModel : ViewModelBase, INavigationAware
    {
        [ObservableProperty]
        private bool isMenuOpen = false;

        public void OnNavigatedFrom()
        {
            IsMenuOpen = false;
        }

        public void OnNavigatedTo()
        {
            IsMenuOpen = true;
        }

        [RelayCommand]
        private void CloseMenu()
        {
            IsMenuOpen = false;
            RadialMenuView menu = App.GetService<RadialMenuView>();
            menu.CloseMenu();
        }
    }
}
