using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
