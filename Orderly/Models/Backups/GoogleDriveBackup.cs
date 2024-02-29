using Orderly.Interfaces;

namespace Orderly.Models.Backups
{
    public partial class GoogleDriveBackup : ObservableObject, IBackup
    {
        [ObservableProperty]
        private string fileId = string.Empty;
        [ObservableProperty]
        private string backupName = string.Empty;
        [ObservableProperty]
        private DateTime backupDate;

    }
}
