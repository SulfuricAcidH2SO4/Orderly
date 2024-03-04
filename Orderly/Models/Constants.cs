using System.IO;

namespace Orderly.Models
{
    public static class Constants
    {
        public static string DbName { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Orderly", "CoreDB.ordb");
        public static string ConfigFileName { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Orderly" , "CoreConfig.ordcf");
        public static string NotificationsFile { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Orderly" , "Notis.ordf");
    }
}
