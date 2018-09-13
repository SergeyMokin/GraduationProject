using System.Collections.Generic;

namespace GraduationProjectModels
{
    // User account.
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
            return !string.IsNullOrWhiteSpace(Email);
        }

        public void Edit(User user)
        {
            Email = user.Email;
            BlankFileUsers = user.BlankFileUsers;
        }
    }

    // Model to contain password in database.
    public class Password: IEntity<Password>
    {
        public long Id { get; set; }

        public long UserId { get; set; }

        public string Current { get; set; }

        public bool IsActive { get; set; }

        public bool Validate()
        {
            return !string.IsNullOrWhiteSpace(Current);
        }

        public void Edit(Password password)
        {
            Current = password.Current;
            IsActive = password.IsActive;
        }
    }
}
