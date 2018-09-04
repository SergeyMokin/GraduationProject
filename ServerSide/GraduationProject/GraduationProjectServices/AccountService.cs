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

            var userPassword = await _passwordRepository
                .Get()
                .FirstOrDefaultAsync(p => p.UserId == userId && p.Current.Equals(oldPassword))
                ?? throw new InvalidOperationException();

            userPassword.Current = newPassword;

            await _passwordRepository.EditAsync(userPassword);

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
                Current = password,
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
    }
}
