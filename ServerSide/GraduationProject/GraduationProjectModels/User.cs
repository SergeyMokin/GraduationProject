using System;
using System.Collections.Generic;
using System.Text;

namespace GraduationProjectModels
{
    public class User: IEntity<User>
    {
        public long Id { get; set; }

        public string Email { get; set; }

        public List<BlankFileUser> BlankFileUsers { get; set; }

        public User()
        {
            BlankFileUsers = new List<BlankFileUser>();
        }

        public bool Validate()
        {
            return true;
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
            return true;
        }

        public void Edit(Password password)
        { 
            
        }
    }
}
