using GraduationProjectModels;
using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GraduationProjectInterfaces.Controllers
{
    public interface IUserController
    {
        Task<FileResult> GenerateExcel(BlankFile param);

        IQueryable<BlankFile> GetFiles();

        Task<long> RemoveFile(long id);

        Task<FileResult> DownloadFile(long id);

        Message SendMessage(Message mes);

    }
}
