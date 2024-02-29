namespace Orderly.Models.Notifications
{
    public class NotificationAction
    {
        public string Description { get; set; } = string.Empty;

        public Action? Procedure { get; set; }
    }
}
