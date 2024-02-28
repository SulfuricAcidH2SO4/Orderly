using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orderly.Models.Notifications
{
    public class NotificationAction
    {
        public string Description { get; set; } = string.Empty;

        public Action? Procedure { get; set; }
    }
}
