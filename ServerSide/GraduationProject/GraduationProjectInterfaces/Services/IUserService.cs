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
        Task<FileContentResult> GenerateExcel(BlankFile param, long userId);

        Task<IEnumerable<BlankFile>> GetFiles(long userId);

        Task<long> RemoveFile(long fileId, long userId);

        Task<FileContentResult> DownloadFile(long fileId, long userId);

        Message SendMessage(Message mes, long userId);
    }
}
