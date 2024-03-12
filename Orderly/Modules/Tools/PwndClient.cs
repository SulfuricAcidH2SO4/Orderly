using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Orderly.Modules.Tools
{
    public class PwndClient
    {
        public static int CheckPasswordBreach(string password)
        {
            Version? v = Assembly.GetExecutingAssembly().GetName().Version;
            SHA1 sha1 = SHA1.Create();

            string sourceHash = Convert.ToHexString(sha1.ComputeHash(Encoding.UTF8.GetBytes(password)));
            RestClient client = new($"https://api.pwnedpasswords.com/range/{sourceHash.Substring(0, 5)}");
            RestRequest request = new();
            request.AddHeader("User-Agent", $"Orderly-app.v{v.Major}.{v.Minor}.{v.Build}");
            var response = client.Get(request);

            if (response == null || response.StatusCode != System.Net.HttpStatusCode.OK || string.IsNullOrEmpty(response.Content)) return -1;

            string[] lines = response.Content!.Replace("\r", string.Empty).Split('\n');

            foreach ( string line in lines ) {
                string[] content = line.Split(':');
                if (sourceHash.Contains(content[0])) return int.Parse(content[1]);
            }

            return 0;
        }
    }
}
