using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using CSharpLib;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace Orderly.Dog
{
    public static class StartManager
    {
        public static void AddToWindowsStart(string sourcePath)
        {
            Directory.CreateDirectory(@"C:\ProgramData\Microsoft\Windows\Start Menu\Programs\Orderly\");
            Shortcuts.CreateShortcutToFile(sourcePath, 
                @"C:\ProgramData\Microsoft\Windows\Start Menu\Programs\Orderly\Orderly.lnk",
                "Orderly, Secure Password Manager",
                null,
                null,
                Path.GetDirectoryName(sourcePath));
        }

        public static void AddToStartup(string exePath, string encryptionKey)
        {
            RegistryKey? rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (rk == null) return;

            rk.SetValue("Orderly", $"\"{exePath}\"");

            string encryptedConfig = File.ReadAllText(@"C:\LocalProjects\Orderly\Orderly\bin\Debug\net8.0-windows10.0.17763.0\CoreConfig.ordcf");
            string plainConfig = DecryptString(encryptedConfig, encryptionKey);

            dynamic config = JsonConvert.DeserializeObject(plainConfig)!;

            config.StartOnStartUp = true;

            plainConfig = JsonConvert.SerializeObject(config);
            encryptedConfig = EncryptString(plainConfig, encryptionKey);
            File.WriteAllText(@"C:\LocalProjects\Orderly\Orderly\bin\Debug\net8.0-windows10.0.17763.0\CoreConfig.ordcf", encryptedConfig);
        }

        public static void RemoveFromStartup(string encryptionKey)
        {
            RegistryKey? rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (rk == null) return;

            rk.DeleteValue("Orderly");

            string encryptedConfig = File.ReadAllText(@"C:\LocalProjects\Orderly\Orderly\bin\Debug\net8.0-windows10.0.17763.0\CoreConfig.ordcf");
            string plainConfig = DecryptString(encryptedConfig, encryptionKey);

            dynamic config = JsonConvert.DeserializeObject(plainConfig)!;

            config.StartOnStartUp = false;

            plainConfig = JsonConvert.SerializeObject(config);
            encryptedConfig = EncryptString(plainConfig, encryptionKey);
            File.WriteAllText(@"C:\LocalProjects\Orderly\Orderly\bin\Debug\net8.0-windows10.0.17763.0\CoreConfig.ordcf", encryptedConfig);
        }

        private static string EncryptString(string plainText, string key)
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

        private static string DecryptString(string cipherText, string key)
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
    }
}
