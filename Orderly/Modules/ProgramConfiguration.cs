using Newtonsoft.Json;
using Orderly.DaVault;
using Orderly.Helpers;
using Orderly.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Wpf.Ui.Appearance;

namespace Orderly.Modules
{
    public class ProgramConfiguration : IProgramConfiguration, INotifyPropertyChanged
    {
        private string absolutePassword = string.Empty;
        private bool isDarkMode = true;
        private bool showMinimizeNotifaction = true;
        private bool startOnStartup = true;
        private bool startMinimized = false;
        private bool closeButtonClosesApp = false;
        
        public string AbsolutePassword
        {
            get => absolutePassword;
            set
            {
                SetProperty(ref absolutePassword, value);
            }
        }
        public bool IsDarkMode
        {
            get => isDarkMode;
            set
            {
                SetProperty(ref isDarkMode, value);
                if (value) ApplicationThemeManager.Apply(ApplicationTheme.Dark);
                else ApplicationThemeManager.Apply(ApplicationTheme.Light);
            }
        }
        public bool ShowMinimizeNotification
        {
            get => showMinimizeNotifaction;
            set
            {
                SetProperty(ref showMinimizeNotifaction, value);
            }
        }
        public bool StartOnStartUp
        {
            get => startOnStartup;
            set
            {
                SetProperty(ref startOnStartup, value);
            }
        }
        public bool StartMinimized
        {
            get => startMinimized;
            set
            {
                SetProperty(ref startMinimized, value);
            }
        }
        public bool CloseButtonClosesApp
        {
            get => closeButtonClosesApp;
            set
            {
                SetProperty(ref closeButtonClosesApp, value);
            }
        }


        public void Save()
        {
            string serializedString = JsonConvert.SerializeObject(this, Formatting.Indented);
            string encryptedFile = EncryptionHelper.EncryptString(serializedString, App.GetService<Vault>().ConfigEncryptionKey);
            File.WriteAllText("CoreConfig.ordcf", encryptedFile);
        }

        public void Save(Vault vault)
        {
            string serializedString = JsonConvert.SerializeObject(this, Formatting.Indented);
            string encryptedFile = EncryptionHelper.EncryptString(serializedString, vault.ConfigEncryptionKey);
            File.WriteAllText("CoreConfig.ordcf", encryptedFile);
        }

        #region Property Changed
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void SetProperty<T>(ref T destination, T value, [CallerMemberName] string propertyName = null!)
        {
            destination = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
