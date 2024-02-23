using Orderly.Interfaces;
using Orderly.Modules;
using Orderly.Modules.Routines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orderly.Backups
{
    public static class BackupWorker
    {
        private static int backupCheckFrequency = 60000;

        public static void CheckBackups()
        {
            ProgramConfiguration config = (ProgramConfiguration)App.GetService<IProgramConfiguration>();
            Task.Factory.StartNew(() => {
                while (true) {
                    foreach (var routine in config.BackupRoutines) {
                        DateTime nextRoutineTime = routine.LastBackupDate.AddDays(routine.BackupFrequency);
                        if (routine is GoogleDriveRoutine rt) rt.Authenticate();
                        if (nextRoutineTime > DateTime.Now) continue;
                        routine.Backup(out string error);
                    }
                    Thread.Sleep(backupCheckFrequency);
                }
            });
        }


    }
}
