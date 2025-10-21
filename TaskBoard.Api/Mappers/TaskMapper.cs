using TaskBoard.Api.Services.Helper;
using TaskBoard.Api.Dtos;
using TaskBoard.Api.Models;

namespace TaskBoard.Api.Mappers
{
    public class TaskMapper : IBaseMapper<Tasks, TaskDto>
    {
        public static TaskDto ToDto(Tasks entity, List<string>? expands = null)
        {
            // Default
            TaskDto dto = new TaskDto
            {
                ID = entity.ID,
                UserIDS = entity.User?.Select(u => u.ID).ToList() ?? new(),
                Name = entity.Name,
                Description = entity.Description,
            };

            // Optionals

            if (expands is null) return dto;

            if (ExpandHelper.ShouldExpand(expands, "users"))
            {
                dto.Users = entity.User?
                .Select(u => new UserSummaryDto
                {
                    ID = u.ID,
                    UserName = u.UserName,
                    BirthDate = u.BirthDate
                })
                .ToList() ?? new List<UserSummaryDto>();
            }

            return dto;
        }

        public static void FromTaskUpdateDto(Tasks existingTask, TaskUpdateDto newValue)
        {
            existingTask.UpdatedAt = DateTime.UtcNow;
            existingTask.Name = newValue.Name;
            existingTask.Description = newValue.Description;
            return;
        }
    }
}
