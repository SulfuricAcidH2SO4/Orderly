using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Orderly.Helpers
{
    public class EncryptionHelper
    {
        public static string HashPassword(string password, int hashingTimes = 500)
        {
            string hashedPassword = $"{password}-odr";
            for (int i = 0; i < hashingTimes; i++) {
                byte[] hashedBytes = SHA256.HashData(Encoding.UTF8.GetBytes(hashedPassword));
                hashedPassword = Convert.ToHexString(hashedBytes);
            }
            return hashedPassword;
        }

        public static string EncryptString(string plainText, string key)
        {
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create()) {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream()) {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write)) {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream)) {
                            streamWriter.Write(plainText);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }

        public static string DecryptString(string cipherText, string key)
        {
            if (string.IsNullOrEmpty(cipherText)) return string.Empty;
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create()) {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer)) {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read)) {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream)) {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }

        public static string GetPasswordKey(string plainPassword)
        {
            return HashPassword(plainPassword, 20).Substring(0, 24);
        }
    }
}
