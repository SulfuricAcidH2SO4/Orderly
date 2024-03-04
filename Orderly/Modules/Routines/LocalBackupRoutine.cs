using Newtonsoft.Json;
using Orderly.Database;
using Orderly.Helpers;
using Orderly.Interfaces;
using Orderly.Models;
using Orderly.Models.Backup;
using System.Globalization;
using System.IO;

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
        [ObservableProperty]
        private RoutineStatus status;
        [ObservableProperty]
        private string statusMessage = string.Empty;

        private readonly object lockReload = new();

        public bool Backup()
        {
            if (!Directory.Exists(Path)) Directory.CreateDirectory(Path);
            using DatabaseContext db = new();
            db.SaveChanges();
            db.EnsureClosed();
            File.Copy(Constants.DbName, System.IO.Path.Combine(Path, $"CoreDB{DateTime.Now.ToString("dd.MM.yyyy.HH.mm.ss")}.ordb"), true);
            LastBackupDate = DateTime.Now;
            App.GetService<IProgramConfiguration>().Save();
            ReloadBackups();
            Task.Factory.StartNew(() => {
                List<IBackup> bpsToRemove = Backups
                .OrderBy(x => ((LocalBackup)x).BackupDate)
                .Take(Math.Clamp(Backups.Count - MaxBackupsNumber, 0, int.MaxValue))
                .ToList();

                foreach (var bp in bpsToRemove) {
                    Delete(bp);
                }
                if (bpsToRemove.Count > 0) ReloadBackups();
            });
            return true;
        }

        public bool Delete(IBackup backup)
        {
            if (backup is not LocalBackup lb) {
                return false;
            }

            try {
                File.Delete(lb.BackupPath);
                ReloadBackups();
            }
            catch { }
            return true;
        }

        public bool Restore(IBackup backup)
        {
            try {
                LocalBackup bp = (LocalBackup)backup;
                File.Copy(bp.BackupPath, $"{Constants.DbName}.new");
                return true;
            }
            catch (Exception e) {
                return false;
            }
        }

        public void ReloadBackups()
        {
            lock (lockReload) {
                var filesInFolder = Directory.EnumerateFiles(Path);
                Backups.Clear();
                foreach (var backup in filesInFolder.Where(x => x.Contains("CoreDB") && x.EndsWith(".ordb"))) {
                    LocalBackup bp = new() {
                        BackupPath = backup
                    };

                    string dateString = System.IO.Path.GetFileName(backup).Replace("CoreDB", string.Empty).Replace(".ordb", string.Empty);

                    DateTime.TryParseExact(dateString, "dd.MM.yyyy.HH.mm.ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate);
                    bp.BackupDate = parsedDate;
                    Backups.Add(bp);
                    if (LastBackupDate < parsedDate) LastBackupDate = parsedDate;
                }
            }
        }
    }
}
