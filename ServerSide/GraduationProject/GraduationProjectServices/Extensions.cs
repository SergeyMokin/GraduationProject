using System;
using System.Collections.Generic;
using System.Text;

namespace GraduationProjectServices
{
    // Validation extensions.
    public static class Extensions
    {
        public static bool IsEmail(this string email)
        {
            return true;
        }

        public static bool IsPassword(this string password)
        {
            return true;
        }
    }
}
