using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace GraduationProjectModels
{
    // Some auth options.
    public class AuthOptions
    {
        //Encryption key.
        private const string Key = "2019MSAGraduationProject";

        //The lifetime of the token is 4 weeks (in minutes).
        public const int LifeTime = 40320;

        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Key));
        }
    }
}
