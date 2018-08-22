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

        public ICollection<User> Users { get; set; }

        public BlankFile()
        {
            Users = new List<User>();
        }

        public bool Validate()
        {
            throw new Exception();
        }

        public void Edit(BlankFile blankFile)
        {

        }
    }
}
