﻿using Newtonsoft.Json;
using Orderly.DaVault;
using Orderly.Helpers;
using Orderly.Models;
using Orderly.Modules;
using System.IO;

namespace Orderly.Interfaces
{
    public interface IProgramConfiguration
    {
        string AbsolutePassword { get; set; }
        string PasswordHint { get; set; }
        string UserName { get; set; }
        bool IsDarkMode { get; set; }
        bool ShowMinimizeNotification { get; set; }
        bool StartOnStartUp { get; set; }
        bool StartMinimized { get; set; }
        bool CloseButtonClosesApp { get; set; }
        bool UseHardwareRendering { get; set; }
        FilteringOptions FilteringOptions { get; set; }
        InputOptions InputOptions { get; set; }
        ExtendedObservableCollection<IBackupRoutine> BackupRoutines { get; set; }
        TerminalInputBehaviour TerminalInputBehaviour { get; set; }

        void Save();
        static ProgramConfiguration Load(Vault vault)
        {
            if (!File.Exists(Constants.ConfigFileName)) {
                Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),  "Orderly"));
                ProgramConfiguration config = new();
                config.Save(vault);
            }
            string encryptedFile = File.ReadAllText(Constants.ConfigFileName);
            string decryptedFile = EncryptionHelper.DecryptString(encryptedFile, vault.ConfigEncryptionKey);
            var desObj = JsonConvert.DeserializeObject<ProgramConfiguration>(decryptedFile, new JsonSerializerSettings() {
                TypeNameHandling = TypeNameHandling.Objects
            })!;

            desObj.StartOnStartUp = File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup), "Orderly.lnk"));

            return desObj;
        }
    }
}
