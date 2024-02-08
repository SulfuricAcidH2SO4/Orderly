using Orderly.Database;
using Orderly.Database.Entities;
using Orderly.Helpers;
using System.Collections.ObjectModel;
using Wpf.Ui.Controls;

namespace Orderly.ViewModels.Pages
{
    public partial class DashboardViewModel : ViewModelBase, INavigationAware
    {
        bool isInitialized;
        DatabaseContext db;

        #region Properties
        [ObservableProperty]
        ExtendedObservableCollection<Category> categories = new();
        #endregion

        public void OnNavigatedFrom()
        {
            
        }

        public void OnNavigatedTo()
        {
            if (!isInitialized) Initalize();
        }

        private void Initalize()
        {
            isInitialized = true;
            db = new();
            Categories.AddRange(db.Categories);
        }
    }
}
