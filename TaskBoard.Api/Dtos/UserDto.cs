namespace TaskBoard.Api.Dtos
{
    public class UserDto
    {
        // Default
        public int ID { get; set; }
        public int? Age { set; get; }
        public string? UserName { get; set; }
        public DateTime? BirthDate { get; set; }
        public List<int>? TasksIDS { get; set; }

        // Optionals
        public List<TaskSummaryDto>? Tasks { get; set; }

    }

    public class UserSummaryDto
    {
        public int ID { get; set; }
        public string? UserName { get; set; }
        public DateTime? BirthDate { get; set; }
    }
}
