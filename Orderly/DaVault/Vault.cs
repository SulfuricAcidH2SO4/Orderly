using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Orderly.DaVault
{
    public class Vault
    {
        public string ConfigEncryptionKey { get; set; } = string.Empty;

        public static Vault Initialize()
        {
            string resourceName = "Orderly.DaVault.DaVault.json";

            string jsonContent;
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName)!) {
                using (StreamReader reader = new StreamReader(stream)) {
                    jsonContent = reader.ReadToEnd();
                }
            }

            return JsonConvert.DeserializeObject<Vault>(jsonContent)!;
        }
    }
}
