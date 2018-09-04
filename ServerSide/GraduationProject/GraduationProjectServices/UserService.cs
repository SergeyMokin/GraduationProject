using GraduationProjectInterfaces.ImageHandler;
using GraduationProjectInterfaces.Repository;
using GraduationProjectInterfaces.Services;
using GraduationProjectModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraduationProjectServices
{
    // IUserService implementation to work with user documents.
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
            var fileUser = (await _userRepository.Get().Include(u => u.BlankFileUsers)
                .FirstOrDefaultAsync(u => u.Id == userId))
                .BlankFileUsers
                .FirstOrDefault(f => f.BlankFileId == fileId) 
                ?? throw new ArgumentNullException();

            var file = await _blankFileRepository.GetAsync(fileUser.BlankFileId);

            return new FileContentResult(Convert.FromBase64String(file.Data), file.FileType)
            {
                FileDownloadName = file.Name
            };
        }

        public async Task<FileContentResult> GenerateExcel(BlankFile param, long userId)
        {
            var file = await _imageHandler.GenerateExcel(param) 
                ?? throw new InvalidOperationException();

            var user = await _userRepository.GetAsync(userId);

            file = await _blankFileRepository.AddAsync(file);

            user.BlankFileUsers.Add(new BlankFileUser
            {
                BlankFile = file,
                BlankFileId = file.Id,
                User = user,
                UserId = user.Id
            });

            await _userRepository.EditAsync(user);

            return new FileContentResult(Convert.FromBase64String(file.Data), file.FileType)
            {
                FileDownloadName = file.Name
            };
        }

        public async Task<IEnumerable<BlankFileUserReturn>> GetFiles(long userId)
        {
            var files = (await _userRepository.Get().Include(u => u.BlankFileUsers)
                .FirstOrDefaultAsync(u => u.Id == userId))
                .BlankFileUsers
                .Select(x => new BlankFileUserReturn(x))
                .ToArray();

            var blankFilesNames = _blankFileRepository
                .Get()
                .Where(x => files.Any(f => f.BlankFileId == x.Id))
                .Select(x => x.Name)
                .ToArray();

            for (var i = 0; i < blankFilesNames.Length; i++)
            {
                files[i].FileName = blankFilesNames[i];
            }

            return files;
        }

        public async Task<long> RemoveFile(long fileId, long userId)
        {
            var user = (await _userRepository.Get().Include(u => u.BlankFileUsers)
                .FirstOrDefaultAsync(u => u.Id == userId)) 
                ?? throw new ArgumentNullException();

            user.BlankFileUsers.Remove(user.BlankFileUsers.FirstOrDefault(x => x.BlankFileId == fileId));

            await _userRepository.EditAsync(user);

            return fileId;
        }

        public Message SendMessage(Message mes, long userId)
        {
            throw new NotImplementedException();
        }
    }
}
