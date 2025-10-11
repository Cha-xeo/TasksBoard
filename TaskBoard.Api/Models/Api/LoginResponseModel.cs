using TaskBoard.Api.Dtos;

namespace TaskBoard.Api.Models.Api
{
    public class LoginResponseModel
    {
        public UserDto? User { get; set; }
        public string? AccessToken { get; set; }
        public int ExpiresIn { get; set; }
    }
}
