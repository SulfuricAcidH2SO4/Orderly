using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using Orderly.Database;
using Orderly.Database.Entities;
using Orderly.Extensions;
using Orderly.Helpers;
using Orderly.Models.Tools;
using Orderly.Modules.Exporting;
using Orderly.Views.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wpf.Ui.Controls;

namespace Orderly.ViewModels.Pages.Tools
{
    public partial class PasswordsExporterViewModel : ViewModelBase, INavigationAware
    {
        public ExtendedObservableCollection<ExporterCategory> Categories { get; set; } = new();

        [ObservableProperty]
        bool csvExport;
        [ObservableProperty]
        bool htmlExport;
        [ObservableProperty]
        bool txtExport;

        public void OnNavigatedFrom()
        {
            Categories.Clear();
        }

        public void OnNavigatedTo()
        {
            RunCommand(() => {
                using DatabaseContext db = new();
                {
                    foreach (var category in db.Categories.Include(x => x.Credentials)) {
                        ExporterCategory cat = new() {
                            Category = category
                        };
                        Categories.Add(cat);
                        foreach (var credential in category.Credentials!) {
                            cat.Credentials.Add(new() {
                                Category = cat,
                                Credential = credential,
                            });
                        }
                    }
                }
            });
        }

        [RelayCommand]
        public void SelectAll()
        {
            Categories.ForEach(c => c.IsSelected = true);
        }

        [RelayCommand]
        public void RemoveAll()
        {
            Categories.ForEach(c => c.IsSelected = false);
        }

        [RelayCommand]
        public void Export()
        {
            if (new ConfirmDialog("Your passwords will be saved in PLAIN TEXT.\n\nThis means anyone can just open the file and read all your passwords.\n\nDo you wish to proceed?")
                .ShowDialog() == false) return;

            if (new PasswordConfirmDialog().ShowDialog() == false) return;

            SaveFileDialog dialog = new();
            dialog.Filter = "Any file (.*)|*.*";
            if (dialog.ShowDialog() == false) return;

            RunCommand(() => {

                List<Credential> creds = new();
                Categories.ForEach(c => {
                    c.Credentials.ForEach(x => {
                        if (x.IsSelected) creds.Add(x.Credential!);
                    });
                });
                if(CsvExport) DataExporter.Export(creds, dialog.FileName, Models.ExportFormats.CSV);
                if(HtmlExport) DataExporter.Export(creds, dialog.FileName, Models.ExportFormats.HTML);
                if(TxtExport) DataExporter.Export(creds, dialog.FileName, Models.ExportFormats.TXT);
            });
        }
    }
}
