using Orderly.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orderly.Models.Backups
{
    public partial class GoogleDriveBackup : ObservableObject, IBackup
    {
        [ObservableProperty]
        private string fileId = string.Empty;
        [ObservableProperty]
        private string backupName = string.Empty;
        [ObservableProperty]
        private DateTime backupDate;
        
    }
}
