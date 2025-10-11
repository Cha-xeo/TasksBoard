using TaskBoard.Api.Dtos;

namespace TaskBoard.Api.Models.Api
{
    public class RegisterResponseModel
    {
        public UserDto? User { get; set; }
        public string? Message { get; set; }
        public bool status { get; set; }
    }
}
