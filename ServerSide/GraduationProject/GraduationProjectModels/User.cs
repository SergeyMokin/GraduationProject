using System;
using System.Collections.Generic;
using System.Text;

namespace GraduationProjectModels
{
    public class User: IEntity<User>
    {
        public long Id { get; set; }

        public string Email { get; set; }

        public ICollection<BlankFile> BlankFiles { get; set; }

        public User()
        {
            BlankFiles = new List<BlankFile>();
        }

        public bool Validate()
        {
            throw new Exception();
        }

        public void Edit(User user)
        {

        }
    }

    public class Password: IEntity<Password>
    {
        public long Id { get; set; }

        public long UserId { get; set; }

        public string Current { get; set; }

        public bool IsActive { get; set; }

        public bool Validate()
        {
            throw new Exception();
        }

        public void Edit(Password password)
        { 
            
        }
    }
}
