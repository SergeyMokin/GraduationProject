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

        public static TokenValidationParameters GetTokenValidationParameters()
        {
            return new TokenValidationParameters
            {
                // Specifies whether the publisher will validate when validating the token.
                ValidateIssuer = false,
                // Will the token consumer be validated.
                ValidateAudience = false,
                // Will the lifetime be validated.
                ValidateLifetime = true,
                // Set the security key.
                IssuerSigningKey = GetSymmetricSecurityKey(),
                // Validate the security key.
                ValidateIssuerSigningKey = true
            };
        }
    }
}
