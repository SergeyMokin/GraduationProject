using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GraduationProject.Extensions
{
    public static class UserExtension
    {
        public static long GetUserId(this ClaimsPrincipal user)
        {
            return long.Parse(user.Claims.FirstOrDefault().Value);
        }
    }
}
