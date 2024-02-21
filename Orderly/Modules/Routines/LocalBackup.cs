using Orderly.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orderly.Modules.Routines
{
    public partial class LocalBackup : ObservableObject, IBackupRoutine
    {
        [ObservableProperty]
        private DateTime lastBackupDate;
        [ObservableProperty]
        private int backupFrequency;
        [ObservableProperty]
        private string path = "C:\\Orderly_Backups\\";
        

        public bool Backup(out string errorMessage)
        {
            throw new NotImplementedException();
        }
    }
}
