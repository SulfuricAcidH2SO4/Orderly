using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Orderly.Modules
{
    public static class PasswordStrengthChecker
    {
        public static int CalculatePasswordStrength(string password)
        {
            if (string.IsNullOrEmpty(password))
                return 0; 

            int strength = 0;

            if (password.Length >= 12)
                strength++;

            int distinctCharacterCount = password.Distinct().Count();
            if (distinctCharacterCount >= 10)
                strength++;

            bool hasUppercase = Regex.IsMatch(password, @"[A-Z]");
            bool hasLowercase = Regex.IsMatch(password, @"[a-z]");
            bool hasNumber = Regex.IsMatch(password, @"[0-9]");

            if (hasUppercase && hasLowercase && hasNumber)
                strength++;
            else strength--; 

            return Math.Clamp(strength, 0, 2);
        }
    }
}
