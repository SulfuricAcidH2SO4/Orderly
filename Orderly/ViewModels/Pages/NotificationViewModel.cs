using Orderly.Models.Notifications;
using Orderly.Modules.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orderly.ViewModels.Pages
{
    public partial class NotificationViewModel : ObservableObject
    {
        [ObservableProperty]
        NotificationService notificationService;
        
        public NotificationViewModel(NotificationService notificationService)
        {
            NotificationService = notificationService;
        }

        [RelayCommand]
        public void DeleteNotification(UserNotification notification)
        {
            NotificationService.Remove(notification);
        }

        [RelayCommand]
        public void MarkAsRead(UserNotification notification)
        {
            NotificationService.MarkAsRead(notification);
        }

        [RelayCommand]
        public void DismissAll()
        {
            NotificationService.Clear();
        }

        [RelayCommand]
        public void ExecuteButtonAction(Action action)
        {
            action.Invoke();
        }
    }
}
