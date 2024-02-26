﻿using Newtonsoft.Json;
using Orderly.DaVault;
using Orderly.Helpers;
using Orderly.Interfaces;
using Orderly.Models;
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
        private string passwordHint = string.Empty;
        private string userName = string.Empty;
        private bool isDarkMode = true;
        private bool showMinimizeNotifaction = true;
        private bool startOnStartup = true;
        private bool startMinimized = false;
        private bool closeButtonClosesApp = false;
        private bool useHardwareRendering = false;
        private FilteringOptions filteringOptions = new();
        private InputOptions inputOptions = new();
        private ExtendedObservableCollection<IBackupRoutine> backupRoutines = new();
        
        public string AbsolutePassword
        {
            get => absolutePassword;
            set
            {
                SetProperty(ref absolutePassword, value);
            }
        }
        public string PasswordHint
        {
            get => passwordHint;
            set
            {
                SetProperty(ref passwordHint, value);
            }
        }
        public string UserName
        {
            get => userName;
            set
            {
                SetProperty(ref userName, value);
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
        public bool UseHardwareRendering
        {
            get => useHardwareRendering;
            set
            {
                SetProperty(ref useHardwareRendering, value);
            }
        }
        public FilteringOptions FilteringOptions
        {
            get => filteringOptions;
            set
            {
                SetProperty(ref filteringOptions, value);
            }
        }
        public InputOptions InputOptions
        {
            get => inputOptions;
            set
            {
                SetProperty(ref inputOptions, value);   
            }
        }
        public ExtendedObservableCollection<IBackupRoutine> BackupRoutines
        {
            get => backupRoutines;
            set
            {
                SetProperty(ref backupRoutines, value);
            }
        }

        public void Save()
        {
            string serializedString = JsonConvert.SerializeObject(this, new JsonSerializerSettings() {
                TypeNameHandling = TypeNameHandling.Objects, Formatting = Formatting.Indented
            });
            string encryptedFile = EncryptionHelper.EncryptString(serializedString, App.GetService<Vault>().ConfigEncryptionKey);
            File.WriteAllText(Constants.ConfigFileName, encryptedFile);
        }

        public void Save(Vault vault)
        {
            string serializedString = JsonConvert.SerializeObject(this, new JsonSerializerSettings() {
                TypeNameHandling = TypeNameHandling.Objects,
                Formatting = Formatting.Indented
            });
            string encryptedFile = EncryptionHelper.EncryptString(serializedString, vault.ConfigEncryptionKey);
            File.WriteAllText(Constants.ConfigFileName, encryptedFile);
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
