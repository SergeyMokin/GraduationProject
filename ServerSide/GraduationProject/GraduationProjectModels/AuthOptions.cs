using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraduationProjectModels
{
    // Some auth options.
    public class AuthOptions
    {
        //Encryption key.
        private const string KEY = "2019MSAGraduationProject";

        //The lifetime of the token is 4 weeks (in minutes).
        public const int LIFE_TIME = 40320;

        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}
