﻿using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Newtonsoft.Json;
using Orderly.DaVault;
using Orderly.Helpers;
using Orderly.Interfaces;
using Orderly.Models.Backups;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orderly.Modules.Routines
{
    public partial class GoogleDriveRoutine : ObservableObject, IBackupRoutine
    {
        [JsonIgnore]
        public ExtendedObservableCollection<GoogleDriveBackup> Backups { get; set; } = new();

        private bool isAuthenticated = false;

        [JsonIgnore]
        public bool IsAuthenticated
        {
            get => isAuthenticated;
            set
            {
                SetProperty(ref isAuthenticated, value);
            }
        }

        [ObservableProperty]
        private DateTime lastBackupDate;
        [ObservableProperty]
        private int backupFrequency = 14;
        [ObservableProperty]
        private int maxBackupsNumber = 50;
        [ObservableProperty]
        private string user = string.Empty;
        [ObservableProperty]
        private string folderName = "Orderly_Backups";

        private DriveService? service;
        private UserCredential? credential;

        public bool Authenticate()
        {
            Vault v = App.GetService<Vault>();  
            string[] scopes = new string[] { DriveService.Scope.DriveAppdata,
                                             DriveService.Scope.DriveFile};

            var clientId = v.DriveClientId;
            var clientSecret = v.DriveClientSecret;
            
            var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(new ClientSecrets {
                                                                            ClientId = clientId,
                                                                            ClientSecret = clientSecret
                                                                        },
                                                                        scopes,
                                                                        Environment.UserName,
                                                                        CancellationToken.None,
                                                                        new FileDataStore("Orderly")).Result;

            service = new DriveService(new BaseClientService.Initializer() {
                HttpClientInitializer = credential,
                ApplicationName = "Orderly",
            });
            service.HttpClient.Timeout = TimeSpan.FromMinutes(10);
            IsAuthenticated = true;

            var aboutRequest = service.About.Get();
            aboutRequest.Fields = "user(emailAddress)";
            var about = aboutRequest.Execute();
            User = about.User.EmailAddress;
            ReloadBackups();
            return true;
        }

        public bool Backup(out string errorMessage)
        {
            if (!IsAuthenticated) Authenticate();
            errorMessage = string.Empty;
            UploadFile(Path.GetFullPath("CoreDB.ordb"), $"Backup DB for orderly. Date: {DateTime.Now}");
            return true;
        }

        public bool Delete(IBackup backup, out string errorMessage)
        { 
            errorMessage = string.Empty;
            try {
                Backups.Remove((GoogleDriveBackup)backup);
                var result = service?.Files.Delete((backup as GoogleDriveBackup)!.FileId).Execute();
                return true;
            }
            catch (Exception e) {
                errorMessage = $"Error deleting file from Google Drive: {e.Message}";
                return false;
            }
        }

        public bool Restore(IBackup backup, out string errorMessage)
        {
            throw new NotImplementedException();
        }

        public void ReloadBackups()
        {
            if (!IsAuthenticated) Authenticate();
            Backups.Clear();
            string folderId = GetFolderId(FolderName);

            string query = $"'{folderId}' in parents";

            var request = service.Files.List();
            request.Q = query;
            var result = request.Execute();

            foreach ( var backup in result.Files.ToList()) {
                string dateString = backup.Name.Replace("CoreDB", string.Empty).Replace(".ordb", string.Empty);
                DateTime.TryParseExact(dateString, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate);
                Backups.Add(new() {
                    FileId = backup.Id,
                    BackupDate = parsedDate,
                    BackupName = backup.Name,
                });
                if(parsedDate > LastBackupDate) LastBackupDate = parsedDate;
            }

        }

        private bool UploadFile(string filePath, string description)
        {
            if (!IsAuthenticated) Authenticate();
            string fileName = $"CoreDB{DateTime.Now.ToString("dd.MM.yyyy")}.ordb";

            string folderId = GetFolderId(FolderName);

            var fileMetadata = new Google.Apis.Drive.v3.Data.File() {
                Name = fileName,
                MimeType = GetMimeType(filePath),
                Description = description,
                Parents = new[] { folderId }
            };

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
                var request = service!.Files.Create(fileMetadata, stream, GetMimeType(filePath));
                var uploadedFile = request.Upload();

                if (uploadedFile.Status == Google.Apis.Upload.UploadStatus.Completed) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }

        #region Utility

        private string GetMimeType(string fileName)
        {
            string mimeType = "application/unknown";
            string ext = System.IO.Path.GetExtension(fileName).ToLower();
            Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext)!;
            if (regKey != null && regKey.GetValue("Content Type") != null)
                mimeType = regKey.GetValue("Content Type")!.ToString()!;
            return mimeType;
        }

        private string GetFolderId(string folderName)
        {
            var listRequest = service.Files.List();
            listRequest.Q = $"name = '{folderName}' and mimeType = 'application/vnd.google-apps.folder'";
            var result = listRequest.Execute();

            if (result.Files != null && result.Files.Count > 0) {
                return result.Files[0].Id;
            }
            else {
                var folderMetadata = new Google.Apis.Drive.v3.Data.File {
                    Name = folderName,
                    MimeType = "application/vnd.google-apps.folder"
                };

                var createFolderRequest = service.Files.Create(folderMetadata);
                var createdFolder = createFolderRequest.Execute();
                return createdFolder.Id;
            }
        }

        #endregion
    }
}
