using Microsoft.EntityFrameworkCore;
using Orderly.Database;
using Orderly.Database.Entities;
using Orderly.Helpers;
using System.Collections.ObjectModel;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
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
        
        [RelayCommand]
        public void UpdateCategory(Category category)
        {
            db = new();
            if (db.Categories.Update(category).Entity == null) {
                //Failed to update
            }
            db.SaveChanges();
        }

        [RelayCommand]
        public void AddCategory()
        {
            db = new();
            Category addedCategory = db.Categories.Add(new() {
                Name = "New Category"
            }).Entity;
            db.SaveChanges();

            if(addedCategory != null) Categories.Add(addedCategory);
        }

        [RelayCommand]
        public void RemoveCategory(Category category)
        {
            db = new();
            db.Categories.Remove(category);
            db.SaveChanges();
            Categories.Remove(category);
        }

        [RelayCommand]
        public void AddCredentials(Category CredentialCategory)
        {
            db = new();
            Credential cr = new() {
                CategoryId = CredentialCategory.Id,
                ServiceName = string.Empty,
                Password = string.Empty,
                Username = string.Empty,
                IsEditing = true
            };
            var cred = db.Credentials.Add(cr).Entity;
            db.SaveChanges();
            cred.Category = CredentialCategory;
            CredentialCategory.Credentials!.Add(cred);
            CollectionViewSource.GetDefaultView(CredentialCategory.Credentials).Refresh();
        }

        [RelayCommand]
        public void RemoveCredentials(Credential credential)
        {
            Category categoryToUpdate = Categories.First(x => x == credential.Category);
            db = new();
            db.Credentials.Remove(credential);
            db.SaveChanges();
            CollectionViewSource.GetDefaultView(categoryToUpdate.Credentials).Refresh();
        }

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
            Categories.AddRange(db.Categories.Include(x => x.Credentials));

            foreach (var category in Categories) {
                category.PropertyChanged += OnCategoryPropertyChanged;
            }
        }

        private void OnCategoryPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(Category.IsFavorite)) {
                //This might break EF
                var sortedCollection = Categories.OrderBy(x => !x.IsFavorite).ToList();
                Categories.Clear();
                Categories.AddRange(sortedCollection);
            }
        }
    }
}
