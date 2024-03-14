using Orderly.Database.Entities;
using Orderly.Extensions;
using Orderly.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orderly.Models.Tools
{
    public partial class ExporterCategory : ObservableObject
    {
        public ExtendedObservableCollection<ExporterCredential> Credentials { get; set; } = new();
        public bool IsSelected
        {
            get => isSelected;
            set
            {
                SetProperty(ref isSelected, value);
                if(value) {
                    Credentials.ForEach(c => {
                        c.IsSelected = true;
                    });
                }
                else {
                    if(Credentials.All(x => x.IsSelected))
                        Credentials.ForEach(c => {
                            c.IsSelected = false;
                        });
                }
            }
        }
        [ObservableProperty]
        Category? category;

        private bool isSelected = false;
    }
}
