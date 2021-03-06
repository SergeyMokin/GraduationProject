﻿using GraduationProjectModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using GraduationProjectInterfaces.ImageHandler;

namespace GraduationProjectInterfaces.Services
{
    // Service what realize logic of work controller of user documents.
    public interface IUserService
    {
        Task<FileContentResult> GenerateExcel(BlankFile param, long userId, IImageHandler imageHandler = null);

        Task<IEnumerable<BlankFileUserReturn>> GetFiles(long userId);

        IEnumerable<BlankType> GetBlankTypes();

        IEnumerable<User> GetUsers(long userId);

        Task<BlankType> AddBlankType(TypeFile typeFile, IImageHandler imageHandler = null);

        Task<long> RemoveFile(long fileId, long userId);

        Task<FileContentResult> DownloadFile(long fileId, long userId);

        Task<FileContentResult> DownloadTutorial(string template);

        Task<IEnumerable<BlankFileUserReturn>> AcceptFile(long fileId, long userId);

        Task<Message> SendMessage(Message mes, long userId);

        IEnumerable<string> GetTypes();
    }
}
