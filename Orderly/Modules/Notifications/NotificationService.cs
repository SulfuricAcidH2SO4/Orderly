using Orderly.Helpers;
using Orderly.Models.Notifications;
using Orderly.Views.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Wpf.Ui.Controls;

namespace Orderly.Modules.Notifications
{
    public partial class NotificationService : ObservableObject
    {
        public ExtendedObservableCollection<UserNotification> Notifications { get; set; } = new();
        [ObservableProperty]
        int unreadNotifications;

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
