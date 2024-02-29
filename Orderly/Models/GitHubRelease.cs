using Newtonsoft.Json;

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

        [JsonProperty("assets")]
        public List<Asset> Assets { get; set; } = new();

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

    public sealed class Asset
    {
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;

        [JsonProperty("browser_download_url")]
        public string DownloadUrl { get; set; } = string.Empty;

        [JsonProperty("content_type")]
        public string ContentType { get; set; } = string.Empty;
    }
}
