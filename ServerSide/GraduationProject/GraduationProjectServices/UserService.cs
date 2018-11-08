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
using GraduationProjectImageHandler;

namespace GraduationProjectServices
{
    // IUserService implementation to work with user documents.
    public class UserService : IUserService
    {
        private IImageHandler _imageHandler;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<BlankFile> _blankFileRepository;
        private readonly IRepository<BlankType> _blankTypeRepository;
        private readonly IRepository<QuestionEntity> _questionRepository;

        public UserService(IRepository<User> userRepository,
                IRepository<BlankFile> blankFileRepository,
                IRepository<BlankType> blankTypeRepository,
                IRepository<QuestionEntity> questionRepository)
        {
            _userRepository = userRepository;
            _blankFileRepository = blankFileRepository;
            _blankTypeRepository = blankTypeRepository;
            _questionRepository = questionRepository;
        }

        public async Task<BlankType> AddBlankType(TypeFile param, IImageHandler imageHandler = null)
        {
            if (await _blankTypeRepository.Get()
                .AnyAsync(entity => entity.Name.Equals(param.BlankTypeName, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException();
            }

            _imageHandler = imageHandler ?? ImageHandlerFactory.GetImageHanlderByType(param.Type);

            var questions = await _imageHandler.GetQuestionsFromBlank(param);

            var questionsToAdd = questions?.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

            const int minQuestionsCount = 1;

            if (questionsToAdd == null || questionsToAdd.Length < minQuestionsCount)
            {
                throw new ArgumentException();
            }

            if (questionsToAdd.Count(q => q.Contains("?")) != questionsToAdd.Length)
            {
                var questionsWithoutQuestion = questionsToAdd.Where(q => !q.Contains("?")).Select(x => x + "?").ToList();
                questionsToAdd = questionsToAdd.Select(q =>
                    q.Contains("?") ? q : questionsWithoutQuestion.FirstOrDefault(qwq => qwq.Contains(q))).ToArray();
            }

            var addedType = await _blankTypeRepository.AddAsync(new BlankType {Type = param.Type, Name = param.BlankTypeName});

            foreach (var q in questionsToAdd)
            {
                await _questionRepository.AddAsync(new QuestionEntity { BlankTypeId = addedType.Id, Question = q});
            }

            return addedType;
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

        public async Task<FileContentResult> GenerateExcel(BlankFile param, long userId, IImageHandler imageHandler = null)
        {
            if (!param.Validate())
            {
                throw new ArgumentException();
            }

            var blankType = await _blankTypeRepository.Get()
                .FirstOrDefaultAsync(bt => bt.Name.Equals(param.Type, StringComparison.OrdinalIgnoreCase));

            if (blankType == null)
            {
                throw new InvalidOperationException();
            }

            var questions = _questionRepository.Get().Where(q => q.BlankTypeId == blankType.Id).Select(q => q.Question);

            _imageHandler = imageHandler ?? ImageHandlerFactory.GetImageHanlderByType(blankType.Type);

            var file = await _imageHandler.GenerateExcel(param, questions) 
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

        public IEnumerable<BlankType> GetBlankTypes()
        {
            return _blankTypeRepository.Get();
        }

        public IEnumerable<User> GetUsers(long userId)
        {
            return _userRepository.Get().Where(u => u.Id != userId);
        }

        public async Task<IEnumerable<BlankFileUserReturn>> GetFiles(long userId)
        {
            var files = (await _userRepository.Get().Include(u => u.BlankFileUsers)
                .FirstOrDefaultAsync(u => u.Id == userId))
                .BlankFileUsers
                .Select(x => new BlankFileUserReturn(x))
                .ToArray();

            var blankFiles = _blankFileRepository
                .Get()
                .Where(x => files.Any(f => f.BlankFileId == x.Id))
                .Select(x => x)
                .ToArray();

            foreach (var file in files)
            {
                file.FileName = blankFiles.FirstOrDefault(x => x.Id == file.BlankFileId)?.Name;
            }

            return files.OrderBy(x => x.IsAccepted).ThenByDescending(x => x.BlankFileId);
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

        public async Task<Message> SendMessage(Message mes, long userId)
        {
            if (!mes.Validate())
            {
                throw new ArgumentException();
            }

            var user = (await _userRepository.Get().Include(u => u.BlankFileUsers)
                .FirstOrDefaultAsync(u => u.Id == mes.Id))
                ?? throw new ArgumentNullException();

            var fileList = mes.FileIds.Select(id => new BlankFileUser { UserId = user.Id, BlankFileId = id, IsAccepted = false }).ToList();

            user.BlankFileUsers.AddRange(fileList);

            await _userRepository.EditAsync(user);

            return mes;
        }

        public async Task<IEnumerable<BlankFileUserReturn>> AcceptFile(long fileId, long userId)
        {
            var user = (await _userRepository.Get().Include(u => u.BlankFileUsers)
                           .FirstOrDefaultAsync(u => u.Id == userId))
                       ?? throw new ArgumentNullException();

            user.BlankFileUsers = user.BlankFileUsers.Select(x =>
                x.BlankFileId == fileId
                    ? new BlankFileUser {UserId = x.UserId, BlankFileId = x.BlankFileId, IsAccepted = true}
                    : x).ToList();

            await _userRepository.EditAsync(user);

            return await GetFiles(userId);
        }

        public IEnumerable<string> GetTypes()
        {
            return ImageHandlerFactory.GetTypes();
        }
    }
}
