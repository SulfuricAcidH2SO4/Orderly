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
        bool Backup(out string errorMessage);
        bool Restore(IBackup backup, out string errorMessage);
        bool Delete(IBackup backup, out string errorMessage);
    }
}
