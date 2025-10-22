using TaskBoard.Api.Dtos;
using TaskBoard.Api.Handler;
using TaskBoard.Api.Models;
using TaskBoard.Api.Services.Helper;

namespace TaskBoard.Api.Mappers
{
    public class UserMapper : IBaseMapper<Users, UserDto>
    {
        public static UserDto ToDto(Users entity, List<string>? expands = null)
        {
            // Default
            UserDto dto = new UserDto
            {
                ID = entity.ID,
                Email = entity.Email,
                DisplayName = entity.DisplayName,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                BirthDate = entity.BirthDate,
                TasksIDS = entity.Tasks?.Select(t => t.ID).ToList() ?? new()
            };
            // Optionals
            if (expands is null) return dto;
            if (ExpandHelper.ShouldExpand(expands, "tasks"))
            {
                dto.Tasks = entity.Tasks?
                .Select(t => TaskMapper.ToTaskrSummaryDto(t))
                .ToList() ?? new List<TaskSummaryDto>();
            }
            return dto;
        }

        public static UserSummaryDto ToUserSummaryDto(Users user)
        {
            return new UserSummaryDto
            {
                ID = user.ID,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                BirthDate = user.BirthDate
            };
        }

        public static void FromUserUpdateDto(Users existingUser, UserUpdateDto newValue)
        {
            existingUser.UpdatedAt = DateTime.UtcNow;
            existingUser.Email = newValue.Email ?? existingUser.Email;
            existingUser.DisplayName = newValue.DisplayName ?? existingUser.DisplayName;
            existingUser.FirstName = newValue.FirstName ?? existingUser.FirstName;
            existingUser.LastName = newValue.LastName ?? existingUser.LastName;
            existingUser.UserName = newValue.UserName ?? existingUser.UserName;
            existingUser.BirthDate = newValue.BirthDate ?? existingUser.BirthDate;

            if (newValue.PassWord is not null)
            { 
                existingUser.PasswordHash = PasswordHashingHandler.HashPassword(newValue.PassWord);
            }
            return;
        }
    }

}
