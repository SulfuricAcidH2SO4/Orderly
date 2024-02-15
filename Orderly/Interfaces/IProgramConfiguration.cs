using Newtonsoft.Json;
using Orderly.DaVault;
using Orderly.Helpers;
using Orderly.Models;
using Orderly.Modules;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orderly.Interfaces
{
    public interface IProgramConfiguration
    {
        string AbsolutePassword { get; set; }
        string PasswordHint { get; set; }
        string UserName { get; set; }
        bool IsDarkMode {  get; set; }
        bool ShowMinimizeNotification { get; set; }
        bool StartOnStartUp {  get; set; }
        bool StartMinimized { get; set; }
        bool CloseButtonClosesApp { get; set; }  
        bool UseHardwareRendering { get; set; }
        FilteringOptions FilteringOptions { get; set; }

        void Save();
        static ProgramConfiguration Load(Vault vault)
        {
            if (!File.Exists("CoreConfig.ordcf"))
            {
                ProgramConfiguration config = new();
                config.Save(vault);
            }
            string encryptedFile = File.ReadAllText("CoreConfig.ordcf");
            string decryptedFile = EncryptionHelper.DecryptString(encryptedFile, vault.ConfigEncryptionKey);
            return JsonConvert.DeserializeObject<ProgramConfiguration>(decryptedFile)!;
        }
    }
}
