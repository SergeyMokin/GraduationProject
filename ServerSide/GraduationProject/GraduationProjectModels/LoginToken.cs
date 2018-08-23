using System;
using System.Collections.Generic;
using System.Text;

namespace GraduationProjectModels
{
    // Creditionals of user to get access to app.
    public class LoginToken
    {
        public User User { get; set; }

        public string BearerToken { get; set; }

        public DateTime DateExpires { get; set; }
    }
}
