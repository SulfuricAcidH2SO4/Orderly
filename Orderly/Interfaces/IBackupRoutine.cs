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
        bool Backup(out string errorMessage);
    }
}
