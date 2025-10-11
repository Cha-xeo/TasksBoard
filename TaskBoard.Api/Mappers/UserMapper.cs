using TaskBoard.Api.Dtos;
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
                Age = entity.Age,
                UserName = entity.UserName,
                BirthDate = entity.BirthDate,
                TasksIDS = entity.Tasks?.Select(t => t.ID).ToList() ?? new()
            };

            // Optionals
            if (expands is null) return dto;
            if (ExpandHelper.ShouldExpand(expands, "tasks"))
            {
                dto.Tasks = entity.Tasks?
                .Select(t => new TaskSummaryDto
                {
                    ID = t.ID,
                    Name = t.Name,
                    Description = t.Description,
                })
                .ToList() ?? new List<TaskSummaryDto>();
            }
            return dto;
        }
    }
}
