using Orderly.Interfaces;

namespace Orderly.Models.Backups
{
    public partial class FtpBackup : ObservableObject, IBackup
    {
        [ObservableProperty]
        private string backupPath = string.Empty;
        [ObservableProperty]
        private DateTime backupDate;
    }
}
