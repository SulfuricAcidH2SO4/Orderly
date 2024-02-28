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
                CalculateHashes();
                ReplaceFiles();
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

        private void CalculateHashes()
        {
            Dispatcher.Invoke(() => {
                tbStatusMessage.Text = "Checking local files";
            });
            IEnumerable<string> oldFiles = Directory.GetFiles(Directory.GetCurrentDirectory());

            using (var md5 = MD5.Create()) {
                foreach (string file in oldFiles) {
                    using (var stream = File.OpenRead(file)) {
                        OldFiles.Add(file, md5.ComputeHash(stream));
                    }
                }
            }

            Dispatcher.Invoke(() => {
                pbProgress.Value += 1;
                tbStatusMessage.Text = "Checking new files";
            });

            IEnumerable<string> newFiles = Directory.GetFiles(@"update.temp\");

            using (var md5 = MD5.Create()) {
                foreach (string file in newFiles) {
                    using (var stream = File.OpenRead(file)) {
                        NewFiles.Add(file, md5.ComputeHash(stream));
                    }
                }
            }

            Dispatcher.Invoke(() => {
                pbProgress.Value += 1;
            });

        }

        private void ReplaceFiles()
        {
            Dispatcher.Invoke(() => {
                tbStatusMessage.Text = "Replacing files...";
            });
            foreach (var file in NewFiles) {
                KeyValuePair<string, byte[]>? oldFile = OldFiles.FirstOrDefault(x => x.Key == file.Key);

                if(oldFile == null) {
                    File.WriteAllBytes(file.Key, file.Value);
                }
                else {
                    File.WriteAllBytes(file.Key, file.Value);
                }
            }

            Dispatcher.Invoke(() => {
                pbProgress.Value += 1;
            });
        }
    }
}