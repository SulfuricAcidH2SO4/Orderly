using Orderly.Database;
using Orderly.Database.Entities;
using Orderly.Helpers;
using System.Collections.ObjectModel;
using System.Windows.Controls.Primitives;
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

        [RelayCommand]
        public void SaveEditMode(ToggleButton editButton)
        {
            if (editButton == null) return;
            if (editButton.DataContext is not Category) return;
            UpdateCategory((Category)editButton.DataContext);
            editButton.IsChecked = false;
        }

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
