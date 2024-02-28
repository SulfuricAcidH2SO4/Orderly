using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orderly.Models
{
    public sealed class GitHubRelease
    {
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;

        [JsonProperty("url")]
        public string Url { get; set; } = string.Empty;

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("zipball_url")]
        public string ZipBallUrl { get; set; } = string.Empty;

        [JsonIgnore]
        public Version Version
        {
            get
            {
                try {
                    Version v = new(Name.Replace("v", string.Empty));
                    return v;
                }
                catch {
                    return new Version("1.0.0");
                }
            }
        }
    }
}
