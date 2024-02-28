using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace Orderly.Dog
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string DownloadUrl;

        Dictionary<string, byte[]> OldFiles = new();
        Dictionary<string, byte[]> NewFiles = new();

        public MainWindow(string downloadUrl)
        {
            DownloadUrl = downloadUrl;
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => {
                DownloadZip();
                DecompressZip();
                Dictionary<string, string> updateFilesHashes = GetFolderFilesAndHashes("update.temp", false);
                CopyAndOverwriteFiles(updateFilesHashes, "update.temp");
                ClearFiles();
                Restart();
            });
        }

        private void DownloadZip()
        {
            Dispatcher.Invoke(() => {
                tbStatusMessage.Text = "Downloading update...";
            });

            if (Directory.Exists("update.temp")) {
                Directory.Delete("update.temp", true);
            }
            Directory.CreateDirectory("update.temp");

            using (WebClient client = new()) {
                client.Headers.Add(HttpRequestHeader.UserAgent, "Orderly-Updater");
                client.DownloadProgressChanged += (sender, e) => {
                    Dispatcher.Invoke(() => {
                        pbProgress.Value = (double)((double)e.ProgressPercentage / 100);
                    });
                };
                client.DownloadFileAsync(new Uri(DownloadUrl), @"update.temp\package.zip");
                while (client.IsBusy) {

                }
            }
        }

        private void DecompressZip()
        {
            Dispatcher.Invoke(() => {
                tbStatusMessage.Text = "Decompressing files...";
            });
            ZipFile.ExtractToDirectory(@"update.temp\package.zip", @"update.temp\");
            Dispatcher.Invoke(() => {
                tbStatusMessage.Text = "Deleting package...";
            });
            File.Delete(@"update.temp\package.zip");
            Dispatcher.Invoke(() => {
                pbProgress.Value += 1;
            });
        }

        private void Restart()
        {
            Dispatcher.Invoke(() =>
            {
                tbStatusMessage.Text = "Update complete! Restarting Orderly...";
            });

            Thread.Sleep(1000);

            string executablePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Orderly.exe");

            if (File.Exists(executablePath))
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = executablePath
                };

                Process.Start(startInfo);
            }

            Dispatcher.Invoke(() =>
            {
                App.Current.Shutdown();
            });
        }


        private Dictionary<string, string> GetFolderFilesAndHashes(string folderPath, bool updateFolder)
        {
            Dispatcher.Invoke(() => {
                tbStatusMessage.Text = "Calculating file hashes...";
            });

            Dictionary<string, string> fileHashes = new Dictionary<string, string>();

            if (Directory.Exists(folderPath))
            {
                foreach (string filePath in Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories))
                {
                    if (updateFolder && filePath.Contains("update.temp")) continue;
                    byte[] fileBytes = File.ReadAllBytes(filePath);
                    string hash = CalculateMD5(fileBytes);
                    string relativePath = Path.GetRelativePath(folderPath, filePath);
                    fileHashes[relativePath] = hash;
                }
            }

            Dispatcher.Invoke(() =>
            {
                pbProgress.Value += 1;
            });

            return fileHashes;
        }

        private void CopyAndOverwriteFiles(Dictionary<string, string> sourceFiles, string destinationFolder)
        {

            Dispatcher.Invoke(() => {
                tbStatusMessage.Text = "Replacing outadate files...";
            });

            foreach (var kvp in sourceFiles)
            {
                string sourcePath = Path.Combine(destinationFolder, kvp.Key);
                string destinationPath = Path.Combine("", kvp.Key);

                string destinationDirectory = Path.GetDirectoryName(destinationPath);
                if (!string.IsNullOrEmpty(destinationDirectory) && !Directory.Exists(destinationDirectory))
                {
                    // Create the directory if it doesn't exist
                    Directory.CreateDirectory(destinationDirectory);
                }

                if (!File.Exists(destinationPath) || !FileHashMatches(destinationPath, kvp.Value))
                {
                    File.Copy(sourcePath, destinationPath, true);
                }
            }

            Dispatcher.Invoke(() => {
                pbProgress.Value += 1;
            });
        }

        private bool FileHashMatches(string filePath, string expectedHash)
        {
            if (File.Exists(filePath))
            {
                byte[] fileBytes = File.ReadAllBytes(filePath);
                string actualHash = CalculateMD5(fileBytes);
                return actualHash == expectedHash;
            }

            return false;
        }

        private string CalculateMD5(byte[] inputBytes)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

        private void ClearFiles()
        {
            Dispatcher.Invoke(() =>
            {
                tbStatusMessage.Text = "Removing temporary files...";
            });

            Directory.Delete("update.temp", true);

            Dispatcher.Invoke(() =>
            {
                pbProgress.Value += 1;
            });
        }
    }
}