using Orderly.Database;
using Orderly.Extensions;
using Orderly.Interfaces;
using Orderly.Modules;
using Orderly.Modules.Notifications;
using Orderly.Modules.Routines;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orderly.Backups
{
    public static class BackupWorker
    {
        private static int backupCheckFrequency = 36000000;

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

        public static void ClearAllBackups()
        {
            ProgramConfiguration config = (ProgramConfiguration)App.GetService<IProgramConfiguration>();
            
            foreach(var routine in config.BackupRoutines) {
                routine.Backups.ToList().ForEach(x => routine.Delete(x, out _));
            }
        }

        public static void RunAllBackups()
        {
            ProgramConfiguration config = (ProgramConfiguration)App.GetService<IProgramConfiguration>();

            foreach (var routine in config.BackupRoutines) {
                routine.Backup(out _);
            }
        }

        public static void CheckBackupRestore()
        {
            if (File.Exists("CoreDB.ordb.new")) {
                using DatabaseContext db = new();
                db.EnsureClosed();
                db.Dispose();
                File.Move("CoreDB.ordb", "CoreDB.ordb.old", true);
                File.Move("CoreDB.ordb.new", "CoreDB.ordb");
            }
        }
    }
}
