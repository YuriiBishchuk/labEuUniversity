using Lab.API.Helpers;
using Lab.Business.Models;
using Lab.DataAccess.Entities;
using Lab.DataAccess.Repository;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Lab.Business.Services
{
    public interface IUserService
    {
        Task<TokenApiModel> Authenticate(UserLoginModel userObj);
        Task AddUser(UserRegistrationModel userObj);
        Task<IEnumerable<User>> GetAllUsers();
        Task<TokenApiModel> Refresh(TokenApiModel tokenApiDto);
    }

    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public UserService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<TokenApiModel> Authenticate(UserLoginModel userObj)
        {
            if (userObj == null)
                throw new ArgumentNullException(nameof(userObj));

            var user = await _userRepository.GetIQueryable()
                .FirstOrDefaultAsync(x => x.Email == userObj.Email);

            if (user == null)
                throw new Exception("Користувача не знайдено!");

            if (!PasswordHasher.VerifyPassword(userObj.Password, user.Password))
            {
                throw new Exception("Невірний пароль");
            }

            user.Token = CreateJwt(user);
            var newAccessToken = user.Token;
            var newRefreshToken = CreateRefreshToken();
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(5);
            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            return new TokenApiModel()
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };

        }

        public async Task AddUser(UserRegistrationModel userObj)
        {
            if (userObj == null)
                throw new ArgumentNullException(nameof(userObj));

            if (await _userRepository.GetIQueryable().AnyAsync(x => x.Email == userObj.Email))
                throw new Exception("Електронна пошта вже існує");

            if (await _userRepository.GetIQueryable().AnyAsync(x => x.Username == userObj.Username))
                throw new Exception("Ім'я користувача вже існує");

            var passMessage = CheckPasswordStrength(userObj.Password);
            if (!string.IsNullOrEmpty(passMessage))
                throw new Exception(passMessage.ToString());

            var newUser = userObj.Adapt<User>();

            newUser.Password = PasswordHasher.HashPassword(userObj.Password);
            newUser.Role = "Користувач";
            newUser.Token = "";
            await _userRepository.AddAsync(newUser);
            await _userRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            return await _userRepository.GetIQueryable().ToListAsync();
        }

        public async Task<TokenApiModel> Refresh(TokenApiModel tokenApiDto)
        {
            if (tokenApiDto == null)
                throw new ArgumentNullException(nameof(tokenApiDto));

            string accessToken = tokenApiDto.AccessToken;
            string refreshToken = tokenApiDto.RefreshToken;
            var principal = GetPrincipleFromExpiredToken(accessToken);
            var username = principal.Identity.Name;
            var user = await _userRepository.GetIQueryable().FirstOrDefaultAsync(u => u.Username == username);

            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
                throw new Exception("Недійсний запит");

            var newAccessToken = CreateJwt(user);
            var newRefreshToken = CreateRefreshToken();
            user.RefreshToken = newRefreshToken;
            await _userRepository.SaveChangesAsync();

            return new TokenApiModel()
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
            };
        }

        private static string CheckPasswordStrength(string pass)
        {
            StringBuilder sb = new StringBuilder();
            if (pass.Length < 9)
                sb.Append("Мінімальна довжина пароля повинна бути 8" + Environment.NewLine);
            if (!(Regex.IsMatch(pass, "[a-z]") && Regex.IsMatch(pass, "[A-Z]") && Regex.IsMatch(pass, "[0-9]")))
                sb.Append("Пароль повинен містити букви та цифри" + Environment.NewLine);
            if (!Regex.IsMatch(pass, "[<,>,@,!,#,$,%,^,&,*,(,),_,+,\\[,\\],{,},?,:,;,|,',\\,.,/,~,`,-,=]"))
                sb.Append("Пароль повинен містити спеціальні символи" + Environment.NewLine);
            return sb.ToString();
        }

        private string CreateJwt(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(GetJwtSecretKey());
            var identity = new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(ClaimTypes.Name, $"{user.Username}"),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            });

            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.Now.AddSeconds(10),
                SigningCredentials = credentials
            };
            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            return jwtTokenHandler.WriteToken(token);
        }

        public string GetJwtSecretKey()
        {

            return _configuration["Jwt:SecretKey"];
        }

        private string CreateRefreshToken()
        {
            var tokenBytes = RandomNumberGenerator.GetBytes(64);
            var refreshToken = Convert.ToBase64String(tokenBytes);

            var tokenInUser = _userRepository.GetIQueryable()
                .Any(a => a.RefreshToken == refreshToken);
            if (tokenInUser)
            {
                return CreateRefreshToken();
            }
            return refreshToken;
        }

        private ClaimsPrincipal GetPrincipleFromExpiredToken(string token)
        {
            var key = Encoding.ASCII.GetBytes(GetJwtSecretKey());
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateLifetime = false
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Некоретний токен");
            return principal;
        }
    }

}
