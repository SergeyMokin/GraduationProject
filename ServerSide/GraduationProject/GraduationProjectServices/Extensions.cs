using System.Text.RegularExpressions;

namespace GraduationProjectServices
{
    // Validation extensions.
    public static class Extensions
    {
        public static bool IsEmail(this string email)
        {
            const string pattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";

            return !string.IsNullOrWhiteSpace(email) && new Regex(pattern).Match(email).Success;
        }

        public static bool IsPassword(this string password)
        {
            const string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$";

            return !string.IsNullOrWhiteSpace(password) && new Regex(pattern).Match(password).Success;
        }
    }
}
