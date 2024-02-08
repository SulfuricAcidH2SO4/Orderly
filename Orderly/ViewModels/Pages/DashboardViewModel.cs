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

        public void UpdateCategory(Category category)
        {
            db = new();
            if (db.Categories.Update(category).Entity == null) {
                //Failed to update
            }
            db.SaveChanges();
        }

        private void Initalize()
        {
            isInitialized = true;
            db = new();
            Categories.AddRange(db.Categories);
        }
    }
}
