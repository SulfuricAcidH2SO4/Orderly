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
    }
}
