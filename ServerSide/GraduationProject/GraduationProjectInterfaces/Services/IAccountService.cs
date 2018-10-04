using GraduationProjectModels;
using System.Threading.Tasks;

namespace GraduationProjectInterfaces.Services
{
    // Service what realize logic of work controller of accounts.
    public interface IAccountService
    {
        Task<LoginToken> Register(string email, string password);

        Task<LoginToken> Login(string email, string password);

        Task<User> ChangePassword(string oldPassword, string newPassword, long userId);

        Task<User> ChangeEmail(string email, long userId);

        Task<LoginToken> UpdateToken(long userId);
    }
}
