using System;
using System.Text.RegularExpressions;

namespace Participants
{
    public class PasswordCheck
    {
        public static bool IsValidPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                return false;

            // Check length > 6
            if (password.Length <= 6)
                return false;

            // Check for at least one uppercase
            if (!Regex.IsMatch(password, "[A-Z]"))
                return false;

            // Check for at least one special character
            if (!Regex.IsMatch(password, "[^a-zA-Z0-9]"))
                return false;

            return true;
        }
    }
}
