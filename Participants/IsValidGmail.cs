using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Participants
{
    public static class IsValidGmail
    {
        public static bool GmailCheck(string email)
        {
           // if (string.IsNullOrWhiteSpace(email))
           //     return false;

            // Regex chuẩn kiểm tra email
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
        }

    }

}
