using GraduationProjectModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraduationProjectInterfaces.Services
{
    public interface IUserService
    {
        Task<FileResult> GenerateExcel(BlankFile param, long userId);

        IQueryable<BlankFile> GetFiles(long userId);

        Task<long> RemoveFile(long fileId, long userId);

        Task<FileResult> DownloadFile(long fileId, long userId);

        Message SendMessage(Message mes, long userId);
    }
}
