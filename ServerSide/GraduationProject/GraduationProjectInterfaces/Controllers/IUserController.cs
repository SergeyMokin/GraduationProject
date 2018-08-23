using GraduationProjectModels;
using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace GraduationProjectInterfaces.Controllers
{
    public interface IUserController
    {
        Task<FileContentResult> GenerateExcel(BlankFile param);

        Task<IEnumerable<BlankFile>> GetFiles();

        Task<long> RemoveFile(long id);

        Task<FileContentResult> DownloadFile(long id);

        Message SendMessage(Message mes);

    }
}
