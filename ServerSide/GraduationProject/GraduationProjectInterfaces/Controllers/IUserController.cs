﻿using GraduationProjectModels;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace GraduationProjectInterfaces.Controllers
{
    // Controller of user files.
    public interface IUserController
    {
        Task<FileContentResult> GenerateExcel(BlankFile param);

        Task<IEnumerable<BlankFileUserReturn>> GetFiles();

        IEnumerable<BlankType> GetBlankTypes();

        IEnumerable<User> GetUsers();

        Task<BlankType> AddBlankType(TypeFile typeFile);

        Task<long> RemoveFile(long id);

        Task<FileContentResult> DownloadFileAnonymous(long id, string token);

        Task<FileContentResult> DownloadTutorial(string template, string token);

        Task<FileContentResult> DownloadFile(long id);

        Task<IEnumerable<BlankFileUserReturn>> AcceptFile(long fileId);

        Task<Message> SendMessage(Message mes);

        IEnumerable<string> GetTypes();

    }
}
