using GraduationProjectInterfaces.ImageHandler;
using GraduationProjectInterfaces.Repository;
using GraduationProjectInterfaces.Services;
using GraduationProjectModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraduationProjectServices
{
    public class UserService : IUserService
    {
        private readonly IImageHandler _imageHandler;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<BlankFile> _blankFileRepository;

        public UserService(IImageHandler imageHandler,
                IRepository<User> userRepository,
                IRepository<BlankFile> blankFileRepository)
        {
            _imageHandler = imageHandler;
            _userRepository = userRepository;
            _blankFileRepository = blankFileRepository;
        }

        public async Task<FileContentResult> DownloadFile(long fileId, long userId)
        {
            var file = (await _userRepository.Get().Include(u => u.BlankFileUsers)
                .FirstOrDefaultAsync(u => u.Id == userId))
                .BlankFileUsers
                .Select(x => x.BlankFile)
                .FirstOrDefault(f => f.Id == fileId);

            return new FileContentResult(Convert.FromBase64String(file.Data), file.FileType)
            {
                FileDownloadName = file.Name
            };
        }

        public async Task<FileContentResult> GenerateExcel(BlankFile param, long userId)
        {
            var file = await _imageHandler.GenerateExcel(param);

            return new FileContentResult(Convert.FromBase64String(file.Data), file.FileType)
            {
                FileDownloadName = file.Name
            };
        }

        public async Task<IEnumerable<BlankFile>> GetFiles(long userId)
        {
            return (await _userRepository.Get().Include(u => u.BlankFileUsers)
                .FirstOrDefaultAsync(u => u.Id == userId))
                .BlankFileUsers
                .Select(x => x.BlankFile);
        }

        public async Task<long> RemoveFile(long fileId, long userId)
        {
            throw new NotImplementedException();
        }

        public Message SendMessage(Message mes, long userId)
        {
            throw new NotImplementedException();
        }
    }
}
