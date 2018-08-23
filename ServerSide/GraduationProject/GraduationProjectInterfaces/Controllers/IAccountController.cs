using GraduationProjectModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GraduationProjectInterfaces.Controllers
{
    // Controller of user accounts.
    public interface IAccountController
    {
        Task<LoginToken> Register(string email, string password);

        Task< LoginToken> Login(string email, string password);

        Task<User> ChangePassword(string oldPassword, string newPassword);

        Task<User> ChangeEmail(string email);

        Task<LoginToken> UpdateToken();
    }
}
