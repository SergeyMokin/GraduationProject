using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraduationProject.Extensions;
using GraduationProjectInterfaces.Controllers;
using GraduationProjectInterfaces.Services;
using GraduationProjectModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GraduationProject.Controllers
{
    [Route("api/User/[action]")]
    [Produces("application/json")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase, IUserController
    {
        private IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<FileContentResult> DownloadFile(long id)
        {
            return await _userService.DownloadFile(id, User.GetUserId());
        }

        [HttpPost]
        public async Task<FileContentResult> GenerateExcel([FromBody]BlankFile param)
        {
            return await _userService.GenerateExcel(param, User.GetUserId());
        }

        [HttpGet]
        public async Task<IEnumerable<BlankFile>> GetFiles()
        {
            return await _userService.GetFiles(User.GetUserId());
        }

        [HttpDelete]
        public async Task<long> RemoveFile(long id)
        {
            return await _userService.RemoveFile(id, User.GetUserId());
        }

        [HttpPost]
        public Message SendMessage([FromBody]Message mes)
        {
            return _userService.SendMessage(mes, User.GetUserId());
        }
    }
}