using TaskBoard.Api.Models;

namespace TaskBoard.Api.Dtos
{
    public class TaskDto : TaskSummaryDto
    {
        // Default
        //public int ID { get; set; }
        //public string? Name { get; set; }
        //public string? Description { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastEdited { get; set; }

        // Optionals
        public List<int>? UserIDS { get; set; }
        public List<UserSummaryDto>? Users { get; set; }

    }

    public class TaskCreateDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public List<int>? UserIDS { get; set; }
    }

    public class TaskUpdateDto
    {
        public int ID { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public List<int>? UserIDS { get; set; }
    }

    public class TaskSummaryDto
    {
        public int ID { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}
