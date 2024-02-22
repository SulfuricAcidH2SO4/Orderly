using Orderly.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orderly.Models.Backup
{
    public partial class LocalBackup : ObservableObject, IBackup
    {
        [ObservableProperty]
        private string backupPath = string.Empty;
        [ObservableProperty]
        private DateTime backupDate;
    }
}
