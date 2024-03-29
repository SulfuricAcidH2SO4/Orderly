﻿using Microsoft.EntityFrameworkCore;
using Orderly.Database;
using Orderly.Database.Entities;
using Orderly.DaVault;
using Orderly.Extensions;
using Orderly.Helpers;
using Orderly.Interfaces;
using Orderly.Modules;
using Orderly.Views.Dialogs;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using Wpf.Ui.Controls;

namespace Orderly.ViewModels.Pages
{
    public partial class DashboardViewModel : ViewModelBase, INavigationAware
    {
        bool isInitialized;

        [ObservableProperty]
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
            using DatabaseContext db = new();
            category.LastEditDate = DateTime.Now.ToString();
            if (db.Categories.Update(category).Entity == null) {
                //Failed to update
            }
            db.SaveChanges();
        }

        [RelayCommand]
        public void AddCategory()
        {
            using DatabaseContext db = new();
            Category addedCategory = db.Categories.Add(new() {
                Name = "New Category",
                AdditionDate = DateTime.Now.ToString(),
                LastEditDate = DateTime.Now.ToString(),
                IsOpen = true
            }).Entity;
            db.SaveChanges();

            if (addedCategory != null) Categories.Add(addedCategory);
            SortList();
        }

        [RelayCommand]
        public void RemoveCategory(Category category)
        {
            ConfirmDialog dialg = new($"Are you sure you want to delete {category.Name}? You will lose all the credentials associated with it. This action cannot be undone");
            if (dialg.ShowDialog() == false) return;

            PasswordConfirmDialog pdialog = new();
            if (category.Credentials!.Count != 0 && pdialog.ShowDialog() == false) return;

            using DatabaseContext db = new();
            db.Categories.Remove(category);
            db.SaveChanges();
            Categories.Remove(category);
        }

        [RelayCommand]
        public void AddCredentials(Category CredentialCategory)
        {
            using DatabaseContext db = new();
            Credential cr = new() {
                CategoryId = CredentialCategory.Id,
                ServiceName = string.Empty,
                Password = string.Empty,
                Username = string.Empty,
                IsEditing = true,
                AdditionDate = DateTime.Now.ToString(),
                LastEditDate = DateTime.Now.ToString()
            };
            var cred = db.Credentials.Add(cr).Entity;
            db.SaveChanges();
            cred.Category = CredentialCategory;
            CredentialCategory.Credentials!.Add(cred);
            CredentialCategory.IsOpen = true;
            CollectionViewSource.GetDefaultView(CredentialCategory.Credentials).Refresh();
            cred.PropertyChanged += OnCredentialPropertyChanged;
        }

        [RelayCommand]
        public void RemoveCredentials(Credential credential)
        {
            PasswordConfirmDialog dialog = new();
            if (dialog.ShowDialog() == false) return;

            if (new ConfirmDialog($"Are you sure you want to delete credentials for {credential.ServiceName}?").ShowDialog() == false) return;

            credential.PropertyChanged -= OnCredentialPropertyChanged;
            Category categoryToUpdate = Categories.First(x => x == credential.Category);
            using DatabaseContext db = new();
            db.Credentials.Remove(credential);
            db.SaveChanges();
            CollectionViewSource.GetDefaultView(categoryToUpdate.Credentials).Refresh();
        }

        [RelayCommand]
        public void EnableEditing(Credential credential)
        {
            if (new PasswordConfirmDialog().ShowDialog() == false) return;
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
            if (new PasswordConfirmDialog().ShowDialog() == false) return;
            credential.IsEditing = false;
            credential.LastEditDate = DateTime.Now.ToString();
            Vault v = App.GetService<Vault>();
            credential.Password = EncryptionHelper.EncryptString(credential.Password, v.PasswordEncryptionKey);
            using DatabaseContext db = new();
            db.Credentials.Update(credential);
            db.SaveChanges();
        }

        [RelayCommand]
        public void GeneratePassword(Credential credential)
        {
            PasswordGeneratorDialog dialog = new();
            if (dialog.ShowDialog() == false) return;
            credential.Password = dialog.GeneratedPassword;
        }

        public void MoveCredentialCategory(Credential credential, Category destCategory)
        {
            if (credential.Category == destCategory) return;
            Categories.Single(x => x == credential.Category).Credentials!.Remove(credential);
            Categories.Single(x => x == destCategory).Credentials!.Add(credential);

            CollectionViewSource.GetDefaultView(Categories.Single(x => x.Id == credential.CategoryId).Credentials).Refresh();
            CollectionViewSource.GetDefaultView(Categories.Single(x => x.Id == destCategory.Id).Credentials).Refresh();
            credential.CategoryId = destCategory.Id;
            credential.Category = destCategory;
            using DatabaseContext db = new();
            db.Credentials.Update(credential);
            db.SaveChanges(true);
        }

        public void OnNavigatedFrom()
        {
            Task.Factory.StartNew(() => {
                Categories.Clear();
                GC.Collect();
                isInitialized = false;
            });
        }

        public void OnNavigatedTo()
        {
            if (!isInitialized) Initialize();
        }

        public void Initialize()
        {
            isInitialized = true;
            using DatabaseContext db = new();
            Categories.AddRange(db.Categories.Include(x => x.Credentials));

            foreach (var category in Categories) {
                category.PropertyChanged += OnCategoryPropertyChanged;
                foreach (var credential in category.Credentials) {
                    credential.PropertyChanged += OnCredentialPropertyChanged;
                }
            }
            Config.FilteringOptions.PropertyChanged += OnFilteringOptionChanged;
            SortList();
        }

        private void OnCredentialPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is Credential cr) {
                if (e.PropertyName == nameof(cr.Pinned)) {
                    using DatabaseContext db = new();
                    db.Credentials.Update(cr);
                    db.SaveChanges();
                }
            }
        }

        private void OnFilteringOptionChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            config.Save();
            SortList();
        }

        private void OnCategoryPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Category.IsFavorite)) {
                SortList();
            }
        }

        public void SortList(string searchString = "", bool reloadOrder = true)
        {
            Task.Factory.StartNew(() => {
                //This might break EF
                List<Category> appregory = new(Categories);
                if (!string.IsNullOrEmpty(searchString.ToLower())) {
                    var textFiltered = appregory.Where(x => x.Name.ToLower().Contains(searchString) || x.Credentials.Any(x => x.ServiceName.ToLower().Contains(searchString))).Distinct().ToList();
                    appregory.Clear();
                    appregory.AddRange(textFiltered);
                }

                if (reloadOrder) {
                    switch (Config.FilteringOptions.SortingOption) {
                        case Models.SortingOption.AlphabeticalDescending:
                            var sortedADesc = appregory.OrderByDescending(x => x.Name).ToList();
                            appregory.Clear();
                            appregory.AddRange(sortedADesc);
                            break;
                        case Models.SortingOption.AlphabeticalAscending:
                            var sortedAAsc = appregory.OrderBy(x => x.Name).ToList();
                            appregory.Clear();
                            appregory.AddRange(sortedAAsc);
                            break;
                        case Models.SortingOption.NewestAdded:
                            var sortedNA = appregory.OrderBy(x => x.AdditionDate).ToList();
                            appregory.Clear();
                            appregory.AddRange(sortedNA);
                            break;
                        case Models.SortingOption.OldestAdded:
                            var sortedOA = appregory.OrderByDescending(x => x.AdditionDate).ToList();
                            appregory.Clear();
                            appregory.AddRange(sortedOA);
                            break;
                        case Models.SortingOption.NewestEdited:
                            var sortedNE = appregory.OrderBy(x => x.AdditionDate).ToList();
                            appregory.Clear();
                            appregory.AddRange(sortedNE);
                            break;
                        case Models.SortingOption.OldestEdited:
                            var sortedOE = appregory.OrderByDescending(x => x.AdditionDate).ToList();
                            appregory.Clear();
                            appregory.AddRange(sortedOE);
                            break;
                    }

                    if (Config.FilteringOptions.FavoriteOnTop) {
                        var favoriteCategories = appregory.Where(x => x.IsFavorite).ToList();
                        appregory.RemoveAll(x => x.IsFavorite);
                        foreach (var category in favoriteCategories) {
                            appregory.Insert(0, category);
                        }
                    }
                    Categories.Clear();
                    Categories.AddRange(appregory);
                }

                Categories.ForEach(x => x.IsVisibile = false);

                foreach (var cat in appregory) {
                    Categories.First(x => x.Id == cat.Id).IsVisibile = true;
                }
            });
        }
    }
}
