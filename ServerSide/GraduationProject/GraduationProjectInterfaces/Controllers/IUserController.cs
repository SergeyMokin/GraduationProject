using GraduationProjectModels;
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

        Task<long> RemoveFile(long id);

        Task<FileContentResult> DownloadFile(long id);

        Task<Message> SendMessage(Message mes);

    }
}
