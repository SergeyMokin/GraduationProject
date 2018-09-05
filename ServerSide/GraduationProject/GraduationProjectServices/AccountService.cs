using GraduationProjectInterfaces.Repository;
using GraduationProjectInterfaces.Services;
using GraduationProjectModels;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Linq;

namespace GraduationProjectServices
{
    // IAccountService implementation to work with user account.
    public class AccountService : IAccountService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Password> _passwordRepository;

        public AccountService(IRepository<User> userRepository,
            IRepository<Password> passwordRepository)
        {
            _userRepository = userRepository;
            _passwordRepository = passwordRepository;
        }

        public async Task<User> ChangeEmail(string email, long userId)
        {
            email = email.IsEmail() ? email : throw new ArgumentException();

            var user = new User
            {
                Id = userId,
                Email = email
            };

            return await _userRepository.EditAsync(user);
        }

        public async Task<User> ChangePassword(string oldPassword, string newPassword, long userId)
        {
            newPassword = newPassword.IsPassword() ? newPassword : throw new ArgumentException();

            var userPasswords = _passwordRepository
                .Get()
                .Where(p => p.UserId == userId)
                ?? throw new InvalidOperationException();

            var currentUserPassword = userPasswords.FirstOrDefault(p => p.IsActive) ?? throw new ArgumentNullException();

            if (!VerifyHashedPassword(currentUserPassword.Current, oldPassword))
            {
                throw new InvalidOperationException();
            }

            foreach (var password in userPasswords)
            {
                if (VerifyHashedPassword(password.Current, newPassword))
                {
                    throw new InvalidOperationException();
                }
            }

            currentUserPassword.IsActive = false;

            await _passwordRepository.EditAsync(currentUserPassword);

            await _passwordRepository.AddAsync(new Password()
            {
                UserId = userId,
                Current = HashPassword(newPassword),
                IsActive = true
            });

            return await _userRepository.GetAsync(userId);
        }

        public async Task<LoginToken> Login(string email, string password)
        {
            var user = await _userRepository
                    .Get()
                    .FirstOrDefaultAsync(x => x.Email.Equals(email, StringComparison.CurrentCultureIgnoreCase))
                    ?? throw new InvalidOperationException();

            var userPassword = await _passwordRepository
                    .Get()
                    .FirstOrDefaultAsync(x => x.UserId == user.Id && x.Current.Equals(password))
                    ?? throw new InvalidOperationException();

            return GenerateToken(user);
        }

        public async Task<LoginToken> Register(string email, string password)
        {
            var user = !(email.IsEmail()
                && password.IsPassword())
                ? throw new ArgumentException()
                : await _userRepository.AddAsync(new User
                {
                    Email = email
                });

            await _passwordRepository.AddAsync(new Password
            {
                UserId = user.Id,
                Current = HashPassword(password),
                IsActive = true
            });

            return GenerateToken(user);
        }

        public async Task<LoginToken> UpdateToken(long userId)
        {
            return GenerateToken(await _userRepository.GetAsync(userId) ?? throw new ArgumentNullException());
        }

        private LoginToken GenerateToken(User user)
        {
            var claims = new List<Claim>
                {
                    new Claim("Id", user.Id.ToString()),
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, "User")
                };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

            var now = DateTime.UtcNow;
            var expires = now.Add(TimeSpan.FromMinutes(AuthOptions.LIFE_TIME));

            var jwt = new JwtSecurityToken(
                    notBefore: now,
                    claims: claimsIdentity.Claims,
                    expires: expires,
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
                    );

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return new LoginToken()
            {
                User = user,
                BearerToken = "Bearer " + encodedJwt,
                DateExpires = expires
            };
        }


        #region Work with passwords.
        //Create hash of password.
        private string HashPassword(string password)
        {
            byte[] salt;
            byte[] buffer2;

            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, 0x10, 0x3e8))
            {
                salt = bytes.Salt;
                buffer2 = bytes.GetBytes(0x20);
            }

            byte[] dst = new byte[0x31];

            Buffer.BlockCopy(salt, 0, dst, 1, 0x10);
            Buffer.BlockCopy(buffer2, 0, dst, 0x11, 0x20);

            return Convert.ToBase64String(dst);
        }

        //Verify password.
        private bool VerifyHashedPassword(string hashedPassword, string password)
        {
            byte[] buffer4;

            byte[] src = Convert.FromBase64String(hashedPassword);

            if ((src.Length != 0x31) || (src[0] != 0))
            {
                return false;
            }

            byte[] dst = new byte[0x10];

            Buffer.BlockCopy(src, 1, dst, 0, 0x10);

            byte[] buffer3 = new byte[0x20];

            Buffer.BlockCopy(src, 0x11, buffer3, 0, 0x20);

            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, dst, 0x3e8))
            {
                buffer4 = bytes.GetBytes(0x20);
            }

            return buffer3.SequenceEqual(buffer4);
        }
        #endregion
    }
}
