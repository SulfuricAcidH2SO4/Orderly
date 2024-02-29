using Newtonsoft.Json;
using Orderly.Helpers;

namespace Orderly.Models.Notifications
{
    public partial class UserNotification : ObservableObject
    {
        [ObservableProperty]
        string header = "Notification";
        [ObservableProperty]
        string body = string.Empty;
        [ObservableProperty]
        bool isRead = false;

        public bool KeepBetweenSessions { get; set; } = true;

        [JsonIgnore]
        public ExtendedObservableCollection<NotificationAction> NotificationActions { get; set; } = new();
    }
}
