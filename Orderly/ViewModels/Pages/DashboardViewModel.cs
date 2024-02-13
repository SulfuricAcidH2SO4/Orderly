using Microsoft.EntityFrameworkCore;
using Orderly.Database;
using Orderly.Database.Entities;
using Orderly.DaVault;
using Orderly.Helpers;
using Orderly.Interfaces;
using Orderly.Modules;
using Orderly.Views.Dialogs;
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
        ProgramConfiguration config;

        #region Properties
        [ObservableProperty]
        ExtendedObservableCollection<Category> categories = new();
        #endregion

        public DashboardViewModel(IProgramConfiguration config)
        {
            this.config = (ProgramConfiguration?)config!;
        }

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

            if (addedCategory != null) Categories.Add(addedCategory);
        }

        [RelayCommand]
        public void RemoveCategory(Category category)
        {
            ConfirmDialog dialg = new($"Are you sure you want to delete {category.Name}? You will lose all the credentials associated with it. This action cannot be undone");
            if (dialg.ShowDialog() == false) return;

            PasswordConfirmDialog pdialog = new();
            if (pdialog.ShowDialog() == false) return;

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
            PasswordConfirmDialog dialog = new();
            if (dialog.ShowDialog() == false) return;

            Category categoryToUpdate = Categories.First(x => x == credential.Category);
            db = new();
            db.Credentials.Remove(credential);
            db.SaveChanges();
            CollectionViewSource.GetDefaultView(categoryToUpdate.Credentials).Refresh();
        }

        [RelayCommand]
        public void EnableEditing(Credential credential)
        {
            if (credential.IsEditing) return;
            PasswordConfirmDialog dialog = new();
            if (dialog.ShowDialog() == false) return;

            Vault v = App.GetService<Vault>();
            credential.Password = EncryptionHelper.DecryptString(credential.Password, v.PasswordEncryptionKey);
            credential.IsEditing = true;
        }

        [RelayCommand]
        public void SaveEditing(Credential credential)
        {
            credential.IsEditing = false;
            Vault v = App.GetService<Vault>();
            credential.Password = EncryptionHelper.EncryptString(credential.Password, v.PasswordEncryptionKey);
            db = new();
            db.Credentials.Update(credential);
            db.SaveChanges();
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
            SortList();
        }

        private void OnCategoryPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(Category.IsFavorite)) {
                SortList();
            }
        }

        private void SortList()
        {
            //This might break EF
            var sortedCollection = Categories.OrderBy(x => !x.IsFavorite).ToList();
            Categories.Clear();
            Categories.AddRange(sortedCollection);
        }
    }
}
