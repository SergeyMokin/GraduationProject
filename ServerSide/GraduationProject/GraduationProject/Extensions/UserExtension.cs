using System.Linq;
using System.Security.Claims;

namespace GraduationProjectControllers.Extensions
{
    public static class UserExtension
    {
        // Get user ID from claims.
        public static long GetUserId(this ClaimsPrincipal user)
        {
            return long.Parse(user.Claims.FirstOrDefault().Value);
        }
    }
}
