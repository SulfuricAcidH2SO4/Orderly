using Newtonsoft.Json;
using Orderly.Helpers;
using Orderly.Models;
using Orderly.Models.Notifications;
using Orderly.Views.Pages;
using System.IO;
using System.Net;
using System.Reflection;
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
            if (File.Exists(Constants.NotificationsFile)) {
                NotificationService sv = JsonConvert.DeserializeObject<NotificationService>(File.ReadAllText(Constants.NotificationsFile))!;
                Notifications.Clear();
                Notifications.AddRange(sv.Notifications.Where(x => x.KeepBetweenSessions));

                GetRemoteNotifications();
                UpdateNotifications();
            }
        }
        public void Save()
        {
            File.WriteAllText(Constants.NotificationsFile, JsonConvert.SerializeObject(this));
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

        public void GetRemoteNotifications()
        {
            string url = "https://orderlyapp.altervista.org/news/";

            using (WebClient webClient = new WebClient()) {
                try {
                    string content = webClient.DownloadString(url);
                    string[] notifications = content.Split('|');
                    
                    foreach (string notification in notifications) {
                        string[] splitString = notification.Split(';');
                        if(splitString.Length == 3) {
                            Version v = Assembly.GetExecutingAssembly().GetName().Version!;
                            Version targetV = Version.Parse(splitString[2]);
                            if (v > targetV) continue;
                        }
                        Add(new() {
                            Header = splitString[0],
                            Body = splitString[1],
                            KeepBetweenSessions = false,
                        });
                    }
                }
                catch { }
            }
        }
    }
}
