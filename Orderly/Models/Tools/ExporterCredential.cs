using Orderly.Database.Entities;
using Orderly.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace Orderly.Models.Tools
{
    public partial class ExporterCredential : ObservableObject
    {
        [ObservableProperty]
        Credential? credential;
        [ObservableProperty]
        ExporterCategory? category;

        public bool IsSelected
        {
            get => isSelected;
            set
            {
                SetProperty(ref isSelected, value);
                if (!value) {
                    Category.IsSelected = false;
                }
            }
        }

        private bool isSelected = false;
    }
}
