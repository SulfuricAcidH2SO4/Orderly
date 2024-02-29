using Newtonsoft.Json;
using Orderly.Helpers;
using Orderly.Models.Notifications;
using Orderly.Views.Pages;
using System.IO;
using System.Windows.Controls;

namespace Orderly.Modules.Notifications
{
    public partial class NotificationService : ObservableObject
    {
        public ExtendedObservableCollection<UserNotification> Notifications { get; set; } = new();
        [ObservableProperty]
        int unreadNotifications;
        [ObservableProperty]
        bool isOpen;

        public void Initialize()
        {
            if (File.Exists("Notis.ord")) {
                NotificationService sv = JsonConvert.DeserializeObject<NotificationService>(File.ReadAllText("Notis.ord"))!;
                Notifications.Clear();
                Notifications.AddRange(sv.Notifications.Where(x => x.KeepBetweenSessions));
                UpdateNotifications();
            }
        }
        public void Save()
        {
            File.WriteAllText("Notis.ord", JsonConvert.SerializeObject(this));
        }
        public void Add(UserNotification notification)
        {
            Notifications.Add(notification);
            UpdateNotifications();
        }
        public void Remove(UserNotification notification)
        {
            Notifications.Remove(notification);
            UpdateNotifications();
        }
        public void Clear()
        {
            Notifications.Clear();
            UpdateNotifications();
        }
        public void UpdateNotifications()
        {
            Save();
            UnreadNotifications = Notifications.Where(x => !x.IsRead).Count();
        }
        public void MarkAsRead(UserNotification notification)
        {
            notification.IsRead = true;
            UpdateNotifications();
        }
        public void SetContentControl(ContentControl contentControl)
        {
            NotificationView view = App.GetService<NotificationView>();
            contentControl.Content = view;
        }
    }
}
