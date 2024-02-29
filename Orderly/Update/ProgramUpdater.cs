using Newtonsoft.Json;
using Orderly.Backups;
using Orderly.Models;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;

namespace Orderly.Update
{
    public static class ProgramUpdater
    {
        public static bool CheckUpdate(out Version latestVersion, out string releaseUrl)
        {
            try {
                releaseUrl = string.Empty;
                Version v = Assembly.GetExecutingAssembly().GetName().Version!;
                string response = MakeGetRequest("https://api.github.com/repos/SulfuricAcidH2SO4/Orderly/releases");
                List<GitHubRelease> releases = JsonConvert.DeserializeObject<List<GitHubRelease>>(response)!;

                if (releases.Any(x => x.Version > v)) {
                    var latestRelease = releases.First(x => x.Version == releases.Max(v => x.Version));
                    latestVersion = latestRelease.Version;
                    releaseUrl = latestRelease.Assets.First(x => x.DownloadUrl.Contains(".zip")).DownloadUrl;
                    return true;
                }
                else {
                    latestVersion = v;
                    return false;
                }
            }
            catch (Exception e) {
                Version v = Assembly.GetExecutingAssembly().GetName().Version!;
                latestVersion = v;
                releaseUrl = string.Empty;
                return false;
            }
        }

        static string MakeGetRequest(string url)
        {
            using (WebClient webClient = new()) {
                webClient.Headers.Add(HttpRequestHeader.UserAgent, "Orderly-UpdateCheck");
                return webClient.DownloadString(url);
            }
        }

        public static void UpdateProgram(string downloadUrl)
        {
            string executablePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Orderly.Dog.exe");

            BackupWorker.RunAllBackups();

            if (File.Exists(executablePath)) {
                ProcessStartInfo startInfo = new ProcessStartInfo {
                    FileName = executablePath,
                    Arguments = $"update {downloadUrl}"
                };

                Process.Start(startInfo);
            }
        }
    }
}
