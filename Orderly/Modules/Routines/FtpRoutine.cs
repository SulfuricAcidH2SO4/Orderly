using FluentFTP;
using Newtonsoft.Json;
using Orderly.Helpers;
using Orderly.Interfaces;
using Orderly.Models;
using Orderly.Models.Backup;
using Orderly.Models.Backups;

namespace Orderly.Modules.Routines
{
    public partial class FtpRoutine : ObservableObject, IBackupRoutine
    {
        [JsonIgnore]
        public ExtendedObservableCollection<IBackup> Backups { get; set; } = new();
        [ObservableProperty]
        private DateTime lastBackupDate;
        [ObservableProperty]
        private int backupFrequency = 14;
        [ObservableProperty]
        private int maxBackupsNumber = 50;
        [ObservableProperty]
        private RoutineStatus status;
        [ObservableProperty]
        private string statusMessage = string.Empty;

        [ObservableProperty]
        private string server = string.Empty;
        [ObservableProperty]
        private string username = string.Empty;
        [ObservableProperty]
        private string password = string.Empty;
        [ObservableProperty]
        private string path = "Orderly_Backups";

        private readonly object lockReload = new();

        public bool Backup()
        {
            using (FtpClient client = new(Server, Username, Password)) {
                try {
                    client.Connect();
                    if (!client.DirectoryExists(Path)) {
                        client.CreateDirectory(Path);
                    }

                    client.UploadFile(Constants.DbName, System.IO.Path.Combine(Path, $"CoreDB{DateTime.Now.ToString("dd.MM.yyyy.HH.mm.ss")}.ordb"));
                    Status = RoutineStatus.Ok;
                    ReloadBackups();
                    Task.Factory.StartNew(() => {
                        List<IBackup> bpsToRemove = Backups
                        .OrderBy(x => ((FtpBackup)x).BackupDate)
                        .Take(Math.Clamp(Backups.Count - MaxBackupsNumber, 0, int.MaxValue))
                        .ToList();

                        foreach (var bp in bpsToRemove) {
                            Delete(bp);
                        }
                        if (bpsToRemove.Count > 0) ReloadBackups();
                    });
                    return true;
                }
                catch (Exception ex) {
                    StatusMessage = ex.Message;
                    Status = RoutineStatus.Error;
                    return false;
                }
            }
        }

        public bool Restore(IBackup backup)
        {
            using (FtpClient client = new(Server, Username, Password)) {
                try {
                    client.Connect();
                    client.DownloadFile($"{Constants.DbName}.new", ((FtpBackup)backup).BackupPath, FtpLocalExists.Overwrite);
                    Status = RoutineStatus.Ok;
                    return true;
                }
                catch (Exception ex) {
                    StatusMessage = ex.Message;
                    Status = RoutineStatus.Error;
                    return false;
                }
            }
        }

        public bool Delete(IBackup backup)
        {
            using (FtpClient client = new(Server, Username, Password)) {
                try {
                    client.DeleteFile(((FtpBackup)backup).BackupPath);
                    Status = RoutineStatus.Ok;
                    ReloadBackups();
                    return true;
                }
                catch (Exception ex) {
                    StatusMessage = ex.Message;
                    Status = RoutineStatus.Error;
                    return false;
                }
            }
        }

        public void ReloadBackups()
        {
            lock (lockReload) {
                try {
                    using (FtpClient ftpClient = new FtpClient(Server, Username, Password)) {
                        ftpClient.Connect();
                        FtpListItem[] items = ftpClient.GetListing(Path);
                        Backups.Clear();
                        foreach (var item in items) {
                            Backups.Add(new FtpBackup() {
                                BackupDate = item.Modified,
                                BackupPath = item.FullName
                            });
                            if (LastBackupDate < item.Modified) LastBackupDate = item.Modified;
                        }
                        Status = RoutineStatus.Ok;
                    }
                }
                catch (Exception ex) {
                    Backups.Clear();
                    StatusMessage = ex.Message;
                    Status = RoutineStatus.Error;
                }
            }
        }
    }
}
