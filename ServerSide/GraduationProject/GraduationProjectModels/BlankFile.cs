using System;
using System.Collections.Generic;
using System.Text;

namespace GraduationProjectModels
{
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
            return true;
        }

        public void Edit(BlankFile blankFile)
        {

        }
    }

    public class BlankFileUser
    {
        public long UserId { get; set; }
        public User User { get; set; }

        public long BlankFileId { get; set; }
        public BlankFile BlankFile { get; set; }
    }
}
