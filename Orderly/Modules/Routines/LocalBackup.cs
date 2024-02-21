using Orderly.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
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
        private int backupFrequency = 14;
        [ObservableProperty]
        private string path = "C:\\Orderly_Backups\\";
        [ObservableProperty]
        private int maxBackupsNumber = 50;
        

        public bool Backup(out string errorMessage)
        {
            if(!Directory.Exists(Path)) Directory.CreateDirectory(Path);
            File.Copy("CoreDB.ordb", System.IO.Path.Combine(Path, $"CoreDB{DateTime.Now.ToString("dd.MM.yyyy")}.ordb"), true);
            errorMessage = string.Empty;    
            LastBackupDate = DateTime.Now;
            App.GetService<IProgramConfiguration>().Save();
            return true;
        }
    }
}
