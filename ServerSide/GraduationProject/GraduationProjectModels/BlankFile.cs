﻿using System.Collections.Generic;

namespace GraduationProjectModels
{
    // BlankFile image/document what contain in database.
    public class BlankFile : IEntity<BlankFile>
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Data { get; set; }

        public string Type { get; set; }

        public string FileType { get; set; }

        public List<BlankFileUser> BlankFileUsers { get; set; }

        public BlankFile()
        {
            BlankFileUsers = new List<BlankFileUser>();
        }

        public bool Validate()
        {
            return !(string.IsNullOrWhiteSpace(Name)
                && string.IsNullOrWhiteSpace(Data)
                && string.IsNullOrWhiteSpace(Type)
                && string.IsNullOrWhiteSpace(FileType));
        }

        public void Edit(BlankFile blankFile)
        {
            Name = blankFile.Name;
            Data = blankFile.Data;
            Type = blankFile.Type;
            FileType = blankFile.Type;
            BlankFileUsers = blankFile.BlankFileUsers;
        }
    }

    // Many-to-many relationship implementation.
    public class BlankFileUser
    {
        public long UserId { get; set; }
        public User User { get; set; }

        public long BlankFileId { get; set; }
        public BlankFile BlankFile { get; set; }

        public bool IsAccepted { get; set; } = true;
    }

    public class BlankFileUserReturn
    {
        public BlankFileUserReturn(BlankFileUser fileUser)
        {
            UserId = fileUser.UserId;
            BlankFileId = fileUser.BlankFileId;
            IsAccepted = fileUser.IsAccepted;
        }

        public long UserId { get; set; }

        public long BlankFileId { get; set; }

        public string FileName { get; set; }

        public bool IsAccepted { get; set; }
    }
}
