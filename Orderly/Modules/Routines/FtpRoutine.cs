﻿using FluentFTP;
using Newtonsoft.Json;
using Orderly.Helpers;
using Orderly.Interfaces;
using Orderly.Models;
using Orderly.Models.Backups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public bool Backup()
        {
            using (FtpClient client = new(Server, Username, Password)) {
                try {
                    client.Connect();
                    if (!client.DirectoryExists(Path)) {
                        client.CreateDirectory(Path);
                    }

                    client.UploadFile("CoreDB.ordb", System.IO.Path.Combine(Path, $"CoreDB{DateTime.Now.ToString("dd.MM.yyyy.HH.mm.ss")}.ordb"));
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

        public bool Restore(IBackup backup)
        {
            throw new NotImplementedException();
        }

        public bool Delete(IBackup backup)
        {
            throw new NotImplementedException();
        }

        public void ReloadBackups()
        {
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
                        if(LastBackupDate < item.Modified) LastBackupDate = item.Modified;
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
