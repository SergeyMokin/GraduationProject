using System.Threading.Tasks;
using GraduationProject.Extensions;
using GraduationProject.Filters;
using GraduationProjectInterfaces.Controllers;
using GraduationProjectInterfaces.Services;
using GraduationProjectModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GraduationProject.Controllers
{
    [DisableFormValueModelBinding]
    [Route("api/[controller]/[action]")]
    [Produces("application/json")]
    [ApiController]
    public class AccountController : ControllerBase, IAccountController
    {
        private IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        // POST api/account/register?email=&password=
        [HttpPost]
        public async Task<LoginToken> Register(string email, string password)
        {
            return await _accountService.Register(email, password);
        }

        // POST api/account/login?email=&password=
        [HttpPost]
        public async Task<LoginToken> Login(string email, string password)
        {
            return await _accountService.Login(email, password);
        }

        // PUT api/account/changepassword?oldPassword=&newPassword=
        [Authorize]
        [HttpPut]
        public async Task<User> ChangePassword(string oldPassword, string newPassword)
        {
            return await _accountService.ChangePassword(oldPassword, newPassword, User.GetUserId());
        }

        // PUT api/account/changeemail?email=
        [Authorize]
        [HttpPut]
        public async Task<User> ChangeEmail(string email)
        {
            return await _accountService.ChangeEmail(email, User.GetUserId());
        }

        // POST api/account/updatetoken
        [Authorize]
        [HttpPost]
        public async Task<LoginToken> UpdateToken()
        {
            return await _accountService.UpdateToken(User.GetUserId());
        }
    }
}