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
    public class UserController : ControllerBase, IUserController
    {
        private IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<FileResult> DownloadFile(long id)
        {
            return await _userService.DownloadFile(id, User.GetUserId());
        }

        public async Task<FileResult> GenerateExcel(BlankFile param)
        {
            return await _userService.GenerateExcel(param, User.GetUserId());
        }

        public IQueryable<BlankFile> GetFiles()
        {
            return _userService.GetFiles(User.GetUserId());
        }

        public async Task<long> RemoveFile(long id)
        {
            return await _userService.RemoveFile(id, User.GetUserId());
        }

        public Message SendMessage(Message mes)
        {
            return _userService.SendMessage(mes, User.GetUserId());
        }
    }
}