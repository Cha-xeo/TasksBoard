using TaskBoard.Api.Models;

namespace TaskBoard.Api.Dtos
{
    public class TaskDto
    {
        // Default
        public int ID { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public List<int>? UserIDS { get; set; }

        // Optionals
        public List<UserSummaryDto>? Users { get; set; }

    }

    public class TaskSummaryDto
    {
        public int ID { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}
