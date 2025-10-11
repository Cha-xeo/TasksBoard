using TaskBoard.Api.Dtos;
using TaskBoard.Api.Models.Api;

namespace TaskBoard.Api.Services.Interface
{
    public interface IAuthService
    {
        Task<LoginResponseModel?> Authenticate(LoginRequestModel request);
        Task<UserDto?> Register(RegisterRequestModel request);
    }
}
