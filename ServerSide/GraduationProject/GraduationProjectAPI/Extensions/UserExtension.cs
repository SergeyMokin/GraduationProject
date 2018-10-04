using System.Linq;
using System.Security.Claims;

namespace GraduationProjectAPI.Extensions
{
    public static class UserExtension
    {
        // Get user ID from claims.
        public static long GetUserId(this ClaimsPrincipal user)
        {
            return long.Parse(user.Claims.FirstOrDefault()?.Value);
        }
    }
}
