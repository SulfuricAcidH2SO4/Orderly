using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Orderly.DaVault;
using Orderly.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orderly.Modules.Routines
{
    public partial class GoogleDriveRoutine : ObservableObject, IBackupRoutine
    {
        [ObservableProperty]
        private DateTime lastBackupDate;
        [ObservableProperty]
        private int backupFrequency;
        [ObservableProperty]
        private string email = string.Empty;
        [ObservableProperty]
        private bool isAuthenticated = false;
        [ObservableProperty]
        private string folderName = "Orderly_Backups";

        private DriveService? service;

        public void Authenticate()
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
        }

        public bool Backup(out string errorMessage)
        {
            errorMessage = string.Empty;
            UploadFile(Path.GetFullPath("CoreDB.ordb"), $"Backup DB for orderly. Date: {DateTime.Now}");
            return true;
        }

        public bool Delete(IBackup backup, out string errorMessage)
        {
            throw new NotImplementedException();
        }

        public bool Restore(IBackup backup, out string errorMessage)
        {
            throw new NotImplementedException();
        }

        private bool UploadFile(string filePath, string description)
        {
            if (service == null) Authenticate();
            string fileName = Path.GetFileName(filePath);

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
