using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Orderly.Modules.Tools
{
    public class PwndClient
    {
        public static int CheckPasswordBreach(string password)
        {
            SHA1 sha1 = SHA1.Create();

            string sourceHash = sha1.ComputeHash(Encoding.UTF8.GetBytes(password)).ToString()!;
            RestClient client = new($"https://api.pwnedpasswords.com/range/{sourceHash.Substring(0, 5)}");
        }
    }
}
