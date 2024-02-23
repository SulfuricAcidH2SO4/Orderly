using Orderly.Interfaces;
using Orderly.Modules;
using Orderly.Modules.Notifications;
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
                        if (routine is GoogleDriveRoutine rt) {
                            if(!rt.Authenticate()) {
                                App.GetService<NotificationService>().Add(new() {
                                    Header = "Error authenticating in your Google Drive routine",
                                    Body = "Could not authenticate in one of your Google Drive backup routines. Please log in again."
                                });
                            }
                        }
                        if (nextRoutineTime > DateTime.Now) continue;
                        routine.Backup(out string error);
                    }
                    Thread.Sleep(backupCheckFrequency);
                }
            });
        }


    }
}
