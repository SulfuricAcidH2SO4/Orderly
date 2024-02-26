using Orderly.Helpers;
using Orderly.Models;
using Orderly.Models.Backup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orderly.Interfaces
{
    public interface IBackupRoutine
    {
        DateTime LastBackupDate { get; set; }
        int BackupFrequency {  get; set; }
        int MaxBackupsNumber { get; set; }
        ExtendedObservableCollection<IBackup> Backups { get; set; }
        RoutineStatus Status { get; set; }
        string StatusMessage { get; set; }

        bool Backup();
        bool Restore(IBackup backup);
        bool Delete(IBackup backup);
        void ReloadBackups();
    }
}
