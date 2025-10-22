using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using TaskBoard.Api.Configuration;
using TaskBoard.Api.Data;
using TaskBoard.Api.Dtos;
using TaskBoard.Api.Handler;
using TaskBoard.Api.Mappers;
using TaskBoard.Api.Models;
using TaskBoard.Api.Models.Api;
using TaskBoard.Api.Services.Interface;

namespace TaskBoard.Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly TasksContext _tasksContext;
        private readonly JwtOptions _jwtOptions;
        private readonly IUserService _userService;

        public AuthService(TasksContext tasksContext, IOptions<JwtOptions> jwtOptions, IUserService userService)
        {

            _tasksContext = tasksContext;
            _jwtOptions = jwtOptions.Value;
            if (string.IsNullOrEmpty(_jwtOptions.Key))
            {
                var keyFromEnv = Environment.GetEnvironmentVariable("Jwt__Key");
                if (string.IsNullOrEmpty(keyFromEnv))
                    throw new InvalidOperationException("JWT Key environment variable is not set.");

                _jwtOptions.Key = keyFromEnv;
            }
            _userService = userService;

        }

        public async Task<LoginResponseModel?> Authenticate(LoginRequestModel request)
        {
            if (request is null || string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.Password)) return null;

            Users?  userAccount = await _tasksContext.Users.FirstOrDefaultAsync(x => x.UserName == request.UserName);
            if (userAccount == null || !PasswordHashingHandler.VerifyPassword(request.Password, userAccount.PasswordHash)) return null;

            var tokenExpiryTimeStamp = DateTime.UtcNow.AddMinutes(_jwtOptions.TokenValidityMins);
            try
            {
                SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim(JwtRegisteredClaimNames.Name, request.UserName)
                    }),
                    Expires = tokenExpiryTimeStamp,
                    Issuer = _jwtOptions.Issuer.FirstOrDefault(),
                    Audience = _jwtOptions.Audience.FirstOrDefault(),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key))
                        , SecurityAlgorithms.HmacSha256)
                };

                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                SecurityToken securityToken = tokenHandler.CreateToken(tokenDescriptor);
                string accessToken = tokenHandler.WriteToken(securityToken);

                
                userAccount.LastLoginAt = DateTime.UtcNow;
                await _tasksContext.SaveChangesAsync();
                
                return new LoginResponseModel
                {
                    User = UserMapper.ToDto(userAccount),
                    AccessToken = accessToken,
                    ExpiresIn = (int)tokenExpiryTimeStamp.Subtract(DateTime.UtcNow).TotalSeconds
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }

        }

        public async Task<UserDto?> Register(RegisterRequestModel request)
        {
            if (request is null || string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.Password)) return null;

            bool exist = await _tasksContext.Users.AnyAsync(x => x.UserName == request.UserName);
            //Users?  userAccount = await _tasksContext.Users.FirstOrDefaultAsync(x => x.UserName == request.UserName);
            if (exist) return null;

            Users newUser = new Users
            {
                Email = request.Email,
                DisplayName = request.DisplayName,
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = request.UserName,
                PasswordHash = PasswordHashingHandler.HashPassword(request.Password),
                BirthDate = request.Birthdate,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                Tasks = new List<Tasks>()
            };
            return await _userService.Create(newUser);

            _tasksContext.Users.Add(newUser);
            await _tasksContext.SaveChangesAsync();

            return UserMapper.ToDto(newUser);
        }
    }
}
