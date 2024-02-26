using Newtonsoft.Json;
using Orderly.Database;
using Orderly.Helpers;
using Orderly.Interfaces;
using Orderly.Models.Backup;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orderly.Modules.Routines
{
    public partial class LocalBackupRoutine : ObservableObject, IBackupRoutine
    {
        [JsonIgnore]
        public ExtendedObservableCollection<IBackup> Backups { get; set; } = new();
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
            using DatabaseContext db = new();
            db.SaveChanges();
            db.EnsureClosed();
            File.Copy("CoreDB.ordb", System.IO.Path.Combine(Path, $"CoreDB{DateTime.Now.ToString("dd.MM.yyyy")}.ordb"), true);
            errorMessage = string.Empty;    
            LastBackupDate = DateTime.Now;
            App.GetService<IProgramConfiguration>().Save();
            ReloadBackups();
            return true;
        }

        public bool Delete(IBackup backup, out string errorMessage)
        {
            errorMessage = string.Empty;
            if(backup is not LocalBackup lb) {
                errorMessage = "Wrong backup type";
                return false;
            }

            try {
                File.Delete(lb.BackupPath);
                ReloadBackups();
            }
            catch { }
            return true;
        }

        public bool Restore(IBackup backup, out string errorMessage)
        {
            errorMessage = string.Empty;
            try {
                LocalBackup bp = (LocalBackup)backup;
                File.Copy(bp.BackupPath, "CoreDB.ordb.new");
                return true;
            }
            catch (Exception e){
                errorMessage = $"Error restoring localbackup {(backup as LocalBackup).BackupPath}: {e.Message}";
                return false;
            }
        }

        public void ReloadBackups()
        {
            var filesInFolder = Directory.EnumerateFiles(Path);
            Backups.Clear();
            foreach (var backup in filesInFolder.Where(x => x.Contains("CoreDB") && x.EndsWith(".ordb"))) {
                LocalBackup bp = new() {
                    BackupPath = backup
                };

                string dateString = System.IO.Path.GetFileName(backup).Replace("CoreDB", string.Empty).Replace(".ordb", string.Empty);

                DateTime.TryParseExact(dateString, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate);
                bp.BackupDate = parsedDate;
                Backups.Add(bp);
            }
        }
    }
}
