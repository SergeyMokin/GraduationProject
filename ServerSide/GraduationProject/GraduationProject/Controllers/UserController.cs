﻿using System.Collections.Generic;
using System.Threading.Tasks;
using GraduationProjectControllers.Extensions;
using GraduationProjectControllers.Filters;
using GraduationProjectInterfaces.Controllers;
using GraduationProjectInterfaces.Services;
using GraduationProjectModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GraduationProjectControllers.Controllers
{
    [DisableFormValueModelBinding]
    [Route("api/[controller]/[action]")]
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

        // GET api/user/downloadfile?id=
        [HttpGet]
        public async Task<FileContentResult> DownloadFile(long id)
        {
            return await _userService.DownloadFile(id, User.GetUserId());
        }

        // POST api/user/generateexcel
        // param from body
        [HttpPost]
        public async Task<FileContentResult> GenerateExcel([FromBody]BlankFile param)
        {
            return await _userService.GenerateExcel(param, User.GetUserId());
        }

        // GET api/user/getfiles
        [HttpGet]
        public async Task<IEnumerable<BlankFileUserReturn>> GetFiles()
        {
            return await _userService.GetFiles(User.GetUserId());
        }

        // DELETE api/user/removefile?id=
        [HttpDelete]
        public async Task<long> RemoveFile(long id)
        {
            return await _userService.RemoveFile(id, User.GetUserId());
        }

        // POST api/user/sendmessage
        // mes from body
        [HttpPost]
        public async Task<Message> SendMessage([FromBody]Message mes)
        {
            return await _userService.SendMessage(mes, User.GetUserId());
        }
    }
}