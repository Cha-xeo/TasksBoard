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

    public class UserCreateDto
    {
        public string Email { get; set; }
        public string? DisplayName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? UserName { get; set; }
        public string? PasswordHash { get; set; }
    }

    public class UserUpdateDto
    {
        public int ID { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public List<int>? UserIDS { get; set; }
    }

    public class UserSummaryDto
    {
        public int ID { get; set; }
        public string? UserName { get; set; }
        public DateTime? BirthDate { get; set; }
    }
}
