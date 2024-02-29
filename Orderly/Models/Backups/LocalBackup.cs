using Orderly.Interfaces;

namespace Orderly.Models.Backup
{
    public partial class LocalBackup : ObservableObject, IBackup
    {
        [ObservableProperty]
        private string backupPath = string.Empty;
        [ObservableProperty]
        private DateTime backupDate;
    }
}
