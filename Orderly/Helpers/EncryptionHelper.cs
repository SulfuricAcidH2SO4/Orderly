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
        public static string HashPassword(string password)
        {
            string hashedPassword = $"{password}-odr";
            for (int i = 0; i < 500; i++) {
                byte[] hashedBytes = SHA256.HashData(Encoding.UTF8.GetBytes(hashedPassword));
                hashedPassword = Convert.ToHexString(hashedBytes);
            }
            return hashedPassword;
        }
    }
}
