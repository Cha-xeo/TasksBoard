namespace TaskBoard.Api.Dtos
{
    public class UserDto : UserSummaryDto
    {
        // Default
        public string? DisplayName { get; set; }


        // Optionals
        public List<int>? TasksIDS { get; set; }
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
        public string Email { get; set; }
        public string? DisplayName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? UserName { get; set; }
        public string? PassWord { get; set; }
        public DateTime? BirthDate { get; set; }
        public List<int>? TasksIDS { get; set; }
    }

    public class UserSummaryDto
    {
        public int ID { get; set; }
        public string Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int? age
        {
            get
            {
                if (BirthDate is null) return null;

                var today = DateTime.UtcNow;
                var age = today.Year - BirthDate.Value.Year;

                // Subtract 1 if birthday hasn’t occurred yet this year
                if (BirthDate.Value.Date > today.AddYears(-age))
                    age--;

                return age;
            }
        }
        public DateTime? BirthDate { get; set; }
    }
}
