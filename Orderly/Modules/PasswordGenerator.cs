using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Orderly.Modules
{
    public static class PasswordGenerator
    {
        private const string LowercaseLetters = "abcdefghijklmnopqrstuvwxyz";
        private const string UppercaseLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string Numbers = "0123456789";
        private const string SpecialCharacters = "!@#$%^&*()_-+<>?";

        public static string GenerateSecurePassword(int length, bool useUppercase, bool useNumbers, bool useSpecialChars)
        {
            StringBuilder password = new StringBuilder();

            password.Append(GetRandomCharacter(LowercaseLetters));

            if (useUppercase)
                password.Append(GetRandomCharacter(UppercaseLetters));

            if (useNumbers)
                password.Append(GetRandomCharacter(Numbers));

            if (useSpecialChars)
                password.Append(GetRandomCharacter(SpecialCharacters));

            using (RandomNumberGenerator rng = RandomNumberGenerator.Create()) {
                byte[] buffer = new byte[length - password.Length];
                rng.GetBytes(buffer);

                string allChars = LowercaseLetters;

                if (useUppercase)
                    allChars += UppercaseLetters;

                if (useNumbers)
                    allChars += Numbers;

                if (useSpecialChars)
                    allChars += SpecialCharacters;

                for (int i = 0; i < buffer.Length; i++) {
                    int index = buffer[i] % allChars.Length;
                    password.Append(allChars[index]);
                }
            }
            return ShuffleString(password.ToString());
        }

        private static char GetRandomCharacter(string characterSet)
        {
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create()) {
                byte[] buffer = new byte[1];
                rng.GetBytes(buffer);
                int index = buffer[0] % characterSet.Length;
                return characterSet[index];
            }
        }

        private static string ShuffleString(string str)
        {
            char[] array = str.ToCharArray();
            //Ye, idk about this 
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create()) {
                int n = array.Length;
                while (n > 1) {
                    byte[] box = new byte[1];
                    do rng.GetBytes(box); while (!(box[0] < n * (byte.MaxValue / n)));
                    int k = box[0] % n;
                    n--;
                    char value = array[k];
                    array[k] = array[n];
                    array[n] = value;
                }
            }
            return new string(array);
        }
    }
}
