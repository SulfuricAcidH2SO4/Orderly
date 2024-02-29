using Newtonsoft.Json;
using System.IO;
using System.Reflection;

namespace Orderly.DaVault
{
    public class Vault
    {
        public string ConfigEncryptionKey { get; set; } = string.Empty;
        public string PasswordEncryptionKey { get; set; } = string.Empty;
        public string DriveClientId { get; set; } = string.Empty;
        public string DriveClientSecret { get; set; } = string.Empty;

        public static Vault Initialize()
        {
            string resourceName = "Orderly.DaVault.DaVault.json";

            string jsonContent;
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName)!) {
                using (StreamReader reader = new StreamReader(stream)) {
                    jsonContent = reader.ReadToEnd();
                }
            }

            Vault v = JsonConvert.DeserializeObject<Vault>(jsonContent)!;
            v.PasswordEncryptionKey = v.ConfigEncryptionKey.Substring(0, 24);

            return v;
        }
    }
}
