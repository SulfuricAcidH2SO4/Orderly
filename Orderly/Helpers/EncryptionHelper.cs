using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Orderly.Helpers
{
    public class EncryptionHelper
    {
        public static string EncryptPassword(string password)
        {
            string hashedPassword = password;
            for (int i = 0; i < 1000; i++) {
                byte[] hashedBytes = SHA256.HashData(Encoding.UTF8.GetBytes(hashedPassword));
                hashedPassword = Convert.ToHexString(hashedBytes);
            }
            return hashedPassword;
        }
    }
}
