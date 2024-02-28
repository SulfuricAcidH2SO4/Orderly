using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Orderly.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Orderly.Update
{
    public static class ProgramUpdater
    {
        public static bool CheckUpdate(out Version currentVersion, out Version latestVersion)
        {
            try {
                Version v = Assembly.GetExecutingAssembly().GetName().Version!;
                currentVersion = v;
                //string response = MakeGetRequest("https://api.github.com/repos/SulfuricAcidH2SO4/Orderly/releases");
                string response = MakeGetRequest("https://api.github.com/repos/SixLabors/ImageSharp/releases");
                List<GitHubRelease> releases = JsonConvert.DeserializeObject<List<GitHubRelease>>(response)!;

                if (releases.Any(x => x.Version > v)) {
                    latestVersion = releases.Max(x => x.Version)!;
                    return true;
                }
                else {
                    latestVersion = currentVersion;
                    return false;
                }
            }
            catch (Exception e) {
                Version v = Assembly.GetExecutingAssembly().GetName().Version!;
                currentVersion = v;
                latestVersion = v;
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

        public static void UpdateProgram()
        {

        }
    }
}
