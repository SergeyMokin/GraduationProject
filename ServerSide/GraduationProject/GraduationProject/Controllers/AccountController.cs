using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraduationProject.Extensions;
using GraduationProjectInterfaces.Controllers;
using GraduationProjectInterfaces.Services;
using GraduationProjectModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GraduationProject.Controllers
{
    [Route("api/Account/[action]")]
    [Produces("application/json")]
    [ApiController]
    public class AccountController : ControllerBase, IAccountController
    {
        private IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public async Task<LoginToken> Register(string email, string password)
        {
            return await _accountService.Register(email, password);
        }

        public async Task<LoginToken> Login(string email, string password)
        {
            return await _accountService.Login(email, password);
        }

        public async Task<User> ChangePassword(string oldPassword, string newPassword)
        {
            return await _accountService.ChangePassword(oldPassword, newPassword, User.GetUserId());
        }

        public async Task<User> ChangeEmail(string email)
        {
            return await _accountService.ChangeEmail(email, User.GetUserId());
        }

        public async Task<LoginToken> UpdateToken()
        {
            return await _accountService.UpdateToken(User.GetUserId());
        }
    }
}